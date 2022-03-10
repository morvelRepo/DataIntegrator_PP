using DataIntegrator.Clases;
using DataIntegrator.DataAccess;
using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.Core;
using System.Data;
using SAPbobsCOM;

namespace DataIntegrator.Bussines
{
    public class FacturasBO
    {
        public bool Import(List<Factura> oLstFacturas)
        {
            try
            {
                bool ban = false;
                MyGlobals.sStepLog = "Consulta Facturas Pendientes";
                oLstFacturas = new DBFacturas().GetFacturasPendientes;
                foreach (Factura oF in oLstFacturas)
                {
                    if (CreateSapDoc(oF))
                    {
                        new DBFacturas().SetActualizaFRenPedido(oF.oEstatus.iSapDoc, oF.id, oF.iSapDocPay);
                    }
                    else
                        new DBMetodos().ActualizaRegistro(oF.oEstatus);
                }

                //ActualizaEstatusFacturas();

                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public bool CreateSapDoc(Factura oF)
        {
            bool ban = false;
            MyGlobals.oCompany.StartTransaction();

            SAPbobsCOM.Documents oSapDoc = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
            //oSapDoc.WareHouseUpdateType = SAPbobsCOM.BoDocWhsUpdateTypes.dwh_CustomerOrders;
            oSapDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
            oSapDoc.ReserveInvoice = SAPbobsCOM.BoYesNoEnum.tYES;

            //SAPbobsCOM.Documents oSapDoc = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
            //oSapDoc.WareHouseUpdateType = SAPbobsCOM.BoDocWhsUpdateTypes.dwh_CustomerOrders;
            //oSapDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;

            //// oDraft's
            //oSapDoc.DocObjectCodeEx = "13";

            //Facturas --------------  13
            //Nota de credito -------  14
            //socios de negocio -----  2
            //Factura de proveedores-  18

            try
            {
                MyGlobals.sStepLog = "CreateSapDoc: Empresa[" + oF.empresa.S() + "] - ID[" + oF.id.S() + "]";
                
                // REGISTRAR UNA SERIE PARA LOS CONSECUTIVOS EN SAP
                //if (String.IsNullOrEmpty(oF.sSerie))
                //{
                //    object oSerie = new DBMetodos().GetValueByQuery("SELECT Series FROM NNM1 WHERE SeriesName='Interfaz' ");
                //    if (!String.IsNullOrEmpty(oSerie.S()))
                //    {
                //        oSapDoc.Series = oSerie.S().I();
                //    }
                //}

                oSapDoc.CardCode = oF.cardcode;
                oSapDoc.DocDate = oF.fecha;
                oSapDoc.DocDueDate = oF.fecha.AddDays(1);
                oSapDoc.NumAtCard = oF.referencia;
                
                if (oF.moneda != "MXP")
                    oSapDoc.DocCurrency = oF.moneda;

                oSapDoc.Comments = oF.comentarios;
                oSapDoc.DiscountPercent = double.Parse(oF.descuento.S());
                //oSapDoc.UserFields.Fields.Item("U_TIPODOC").Value = oF.sTipoFactura;
                //oSapDoc.UserFields.Fields.Item("U_REMISION").Value = oF.iIdPrefactura.S();
                //oSapDoc.UserFields.Fields.Item("U_OrigenDocumento").Value = Helpers.sOrigenDocumento;

                string sTaxCode = string.Empty;
                foreach (ConceptosFactura oCF in oF.conceptos)
                {
                    if (new DBMetodos().GetValueByQuery("SELECT COUNT(1) FROM OITM WHERE ITEMCODE='" + oCF.item + "' ").S().I() == 0)
                    {
                        throw new Exception("No existe el item " + oCF.item + " en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] ");
                    }

                    // FACTURA DE ARTICULOS

                    oSapDoc.Lines.BaseEntry = oF.iSapDoc;
                    oSapDoc.Lines.BaseLine = oCF.linea - 1;
                    oSapDoc.Lines.BaseType = Convert.ToInt32(SAPbobsCOM.BoObjectTypes.oOrders);

                    //oSapDoc.Lines.ItemCode = oCF.item;
                    //oSapDoc.Lines.ItemDescription = oCF.descripcionusuario;
                    //oSapDoc.Lines.Quantity = oCF.cantidad.S().Db();
                    //oSapDoc.Lines.UnitPrice = oCF.precio.S().Db();
                    //oSapDoc.Lines.DiscountPercent = oCF.descuento.S().Db();

                    // FACTOR DE IMPUESTOS
                    sTaxCode = oCF.impuesto.S();
                    
                    Utils.GuardarBitacora("Item: " + oCF.item + " Impuesto: " + oCF.impuesto.S());
                    oSapDoc.Lines.TaxCode = sTaxCode;

                    if (!String.IsNullOrEmpty(oCF.WhsCode))
                    {
                        oSapDoc.Lines.WarehouseCode = oCF.WhsCode;
                    }

                    oSapDoc.Lines.CostingCode = oCF.dimension1;
                    oSapDoc.Lines.CostingCode2 = oCF.dimension2;
                    oSapDoc.Lines.CostingCode3 = oCF.dimension3;
                    oSapDoc.Lines.CostingCode4 = oCF.dimension4;
                    oSapDoc.Lines.CostingCode5 = oCF.dimension5;
                    oSapDoc.Lines.ProjectCode = oCF.proyecto;
                    
                    oSapDoc.Lines.Add();


                    #region CODIGO COMENTADO
                    /* CODIGO DE RESPALDO
                    oSapDoc.Lines.ItemCode = oCF.item;
                    oSapDoc.Lines.ItemDescription = oCF.descripcionusuario;
                    oSapDoc.Lines.Quantity = oCF.cantidad.S().Db();
                    //oSapDoc.Lines.UnitPrice = oCF.dPrecio.S().Db();
                    oSapDoc.Lines.DiscountPercent = oCF.descuento.S().Db();

                    // FACTOR DE IMPUESTOS
                    sTaxCode = oCF.impuesto.S();


                    Utils.GuardarBitacora("Item: " + oCF.item + " Impuesto: " + oCF.impuesto.S());
                    oSapDoc.Lines.TaxCode = sTaxCode;

                    if (!String.IsNullOrEmpty(oCF.WhsCode))
                    {
                        oSapDoc.Lines.WarehouseCode = oCF.WhsCode;
                    }

                    oSapDoc.Lines.CostingCode = oCF.dimension1;
                    oSapDoc.Lines.CostingCode2 = oCF.dimension2;
                    oSapDoc.Lines.CostingCode3 = oCF.dimension3;
                    oSapDoc.Lines.CostingCode4 = oCF.dimension4;
                    oSapDoc.Lines.CostingCode5 = oCF.dimension5;
                    oSapDoc.Lines.ProjectCode = oCF.proyecto;
    
                    oSapDoc.Lines.Add();
                    */



                    //// FACTURA DE SERVICIOS
                    //oSapDoc.Lines.AccountCode = "6102-5300";
                    //oSapDoc.Lines.ItemDescription = oCF.sDescripcionUsuario;
                    //oSapDoc.Lines.UnitPrice = oCF.dPrecio.S().Db();
                    //oSapDoc.Lines.TaxCode = sTaxCode;
                    //oSapDoc.Lines.ProjectCode = oCF.sProyecto;
                    //oSapDoc.Lines.CostingCode = oCF.sDimension1;
                    //oSapDoc.Lines.CostingCode2 = oCF.sDimension2;
                    //oSapDoc.Lines.CostingCode3 = oCF.sDimension3;
                    //oSapDoc.Lines.Add();
                    #endregion
                }

                string sMensaje = string.Empty;
                if (oSapDoc.Add() != 0)
                {
                    oF.oEstatus.iEstatus = 0;
                    sMensaje = "Error al guardar el documento en SAP.  [" + MyGlobals.oCompany.GetLastErrorCode().S() + "] - " + MyGlobals.oCompany.GetLastErrorDescription();
                        if (MyGlobals.oCompany.InTransaction == true)
                    {
                        MyGlobals.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    }
                    throw new Exception(sMensaje);
                }
                else
                {
                    ban = true;
                    oF.oEstatus.iEstatus = 1;
                    oF.oEstatus.iSapDoc = MyGlobals.oCompany.GetNewObjectKey().S().I();

                    // adición de pagos a la factura creada.
                    SAPbobsCOM.Payments pagoRECIBIDO = MyGlobals.oCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);
                    pagoRECIBIDO.DocType = BoRcptTypes.rCustomer;
                    pagoRECIBIDO.CardCode = oF.cardcode;
                    pagoRECIBIDO.DocDate = oF.fecha;
                    pagoRECIBIDO.DueDate = oF.fecha.AddDays(1);
                    pagoRECIBIDO.TaxDate = DateTime.Now;
                    pagoRECIBIDO.VatDate = DateTime.Now;
                    pagoRECIBIDO.Remarks = oF.sDescTipoPagoSAP + " " + oF.sReferenciaPago;
                    //pagoRECIBIDO.JournalRemarks = "Concepto de la póliza del pago";

                    //pagoRECIBIDO.Series = 99;     // en caso de tener varias series, espeficiar la que se desea
                    // DETALE DE LAS FACTURAS, EN ESTE CASO SOLO ES LA QUE ACABAMOS DE CREAR
                    pagoRECIBIDO.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                    pagoRECIBIDO.Invoices.DocEntry = oF.oEstatus.iSapDoc;
                    pagoRECIBIDO.Invoices.SumApplied = oF.dMontoPago.S().Db();

                    pagoRECIBIDO.CashAccount = oF.sCuentaCont;
                    pagoRECIBIDO.CashSum = oF.dMontoPago.S().Db();

                    pagoRECIBIDO.UserFields.Fields.Item("U_FormaDePago").Value = oF.sTipoPagoSAP;

                    int resultado = 0;
                    resultado = pagoRECIBIDO.Add();
                    if (resultado != 0)
                    {
                        //int errNumero = 0; string errMensaje = "";
                        MyGlobals.oCompany.GetLastError(out int errNumero, out string errMensaje);
                        if (MyGlobals.oCompany.InTransaction == true)
                        {
                            MyGlobals.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        // si la transacción sigue abierta, la cierra deshaciendo todos los cambios realizados hasta el momento
                        throw new Exception("Ha ocurrido el siguiente error, al intentar crear el Pago Recibido, revise por favor ...\n\n" + errMensaje);
                    }
                    else
                    {
                        oF.iSapDocPay = MyGlobals.oCompany.GetNewObjectKey().S().I();
                    }
                    pagoRECIBIDO = null;

                    

                    // GUARDA ESTATUS EN BITÁCORA
                    oF.oEstatus.sMensaje = "Se creo una factura de reserva en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] - DocEntry[" + oF.oEstatus.iSapDoc.S() + "] - ID_Tabla[" + oF.oEstatus.iID + "]";
                    Utils.GuardarBitacora(oF.oEstatus.sMensaje);
                    Utils.GuardarBitacora("Se creo el pago en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "]");

                    


                    // ACTUALIZACIÓN PEDIDO
                    SAPbobsCOM.Documents oPedido = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                    if (oPedido.GetByKey(oF.iSapDoc))
                    {
                        oPedido.UserFields.Fields.Item("U_IDTransaccion").Value = oF.sIdTransaccion;
                        oPedido.UserFields.Fields.Item("U_ReferenciaPago").Value = oF.sReferenciaPago;
                        oPedido.UserFields.Fields.Item("U_TipoPago").Value = oF.sTipoPago.S();
                        oPedido.UserFields.Fields.Item("U_FechaCreacionP").Value = DateTime.Now.ToString("dd/MM/yyyy");

                        int iRes = oPedido.Update();

                        if(iRes == 0)
                            Utils.GuardarBitacora("Se actualizó el pedido en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "]");
                    }


                    // guarda en firma la información en la base de datos
                    MyGlobals.oCompany.EndTransaction(BoWfTransOpt.wf_Commit);

                    
                    //if (MyGlobals.oCompany.Connected)
                    //{
                    //    MyGlobals.oCompany.Disconnect();
                    //}

                    //MyGlobals.oCompany = null;



                    //if (oF.oEstatus.iSapDoc < 1)
                    //    oF.oEstatus.iSapDoc = new DBMetodos().GetValueByQuery("SELECT MAX(DocEntry) FROM OINV WHERE DataSource='O' AND UserSign=" + MyGlobals.oCompany.UserSignature.S()).S().I();
                    //else
                    //{
                    //    oF.oEstatus.sMensaje = "Se creo una factura de reserva en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] - DocEntry[" + oF.oEstatus.iSapDoc.S() + "] - ID_Tabla[" + oF.oEstatus.iID + "]";
                    //}
                }

                return ban;
            }
            catch (Exception ex)
            {
                string sMsg = string.Empty;
                sMsg = "Error al importar registro de DB Intermedia. Tabla[" + oF.oEstatus.sTabla + "] - " + "ID[" + oF.oEstatus.iID.S() + "] Mensaje de Error: " + ex.Message;
                oF.oEstatus.iEstatus = 0;
                oF.oEstatus.sMensaje = sMsg.Replace("'", "");
                //throw new Exception(oF.oEstatus.sMensaje);

                if (MyGlobals.oCompany.InTransaction == true)
                {
                    MyGlobals.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                }

                return false;
            }
            finally
            {
                Utils.DestroyCOMObject(oSapDoc);
            }
        }

        //private void ActualizaEstatusFacturas()
        //{
        //    try
        //    {
        //        DataTable dtFacturas = new DBFacturas().GetFacturasAProcesar;

        //        foreach (DataRow row in dtFacturas.Rows)
        //        {
        //            if (row["Estatus"].S() == "1")
        //            {
        //                if (new DBFacturas().GetVerificaSiFacturaCancelada(row["FolioSAP"].S().I()))
        //                {
        //                    // FacturaCancelada
        //                    ActualizaEstatusFacturaEnIntegratoryMJ(3, row["IdFactura"].S().I(), 0, row["FolioMJ"].S().I(), row["Tipo"].S().I());
        //                }
        //                else
        //                {
        //                    //Verifica si ya fue asentada la factura en SAP, si es así, actualiza los folio en MexJet y la envía al 100%
        //                    int iDocumento = new DBFacturas().GetObtieneFolioFacturaEnSAP(row["FolioSAP"].S().I());
        //                    if (iDocumento > 0)
        //                    {
        //                        ActualizaEstatusFacturaEnIntegratoryMJ(2, row["IdFactura"].S().I(), iDocumento, row["FolioMJ"].S().I(), row["Tipo"].S().I());
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (new DBFacturas().GetVerificaSiFacturaCancelada(row["FolioSAP"].S().I()))
        //                {
        //                    // FacturaCancelada
        //                    ActualizaEstatusFacturaEnIntegratoryMJ(3, row["IdFactura"].S().I(), 0, row["FolioMJ"].S().I(), row["Tipo"].S().I());
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        /// <summary>
        /// Actualiza las facturas al status correspondiente
        /// 0.- En espera o con error a procesarse en SAP.
        /// 1.- En proceso de asentarse.
        /// 2.- Asentada Correctamente en SAP.
        /// 3.- Cancelada en SAP.
        /// </summary>
        /// <param name="iStatus">Indica el estatus al que va a pasar el documento.</param>
        /// <param name="iIdFactura">Id de factura en el Integrator.</param>
        /// <param name="iFolioSAP">Folio de la factura en SAP si es que existe, sino enviar 0.</param>
        //private void ActualizaEstatusFacturaEnIntegratoryMJ(int iStatus, int iIdFactura, int iFolioSAP, int iFolioMJ, int iTipoFact)
        //{
        //    try
        //    {
        //        if (iStatus == 3)
        //        {
        //            //Factura cancelada, solo cambia de estatus la Factura en Integrator
        //            new DBFacturas().SetActualizaEstatusFacturaEnIntegrator(iIdFactura, 3);
        //            new DBFacturas().SetActualizaNumeroFacturaSAPEnMexJet("Cancelada", iFolioMJ, iTipoFact);
        //        }
        //        else
        //        {
        //            //Factura asentada, adicional hay que marcar el folio en MexJet
        //            new DBFacturas().SetActualizaEstatusFacturaEnIntegrator(iIdFactura, 2);
        //            new DBFacturas().SetActualizaNumeroFacturaSAPEnMexJet(iFolioSAP.S(), iFolioMJ, iTipoFact);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        
    }
}
