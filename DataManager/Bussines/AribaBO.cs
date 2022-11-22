using DataIntegrator.Clases;
using DataIntegrator.DataAccess;
using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NucleoBase.Core;
using System.Data;

namespace DataIntegrator.Bussines
{
    public class AribaBO
    {
        public bool Import()
        {
            try
            {
                bool ban = false;
                List<PedidoAriba> oLstPedidos = new List<PedidoAriba>();
                MyGlobals.sStepLog = "Consulta Pedidos Pendientes";
                Utils.GuardarBitacora(MyGlobals.sStepLog);
                oLstPedidos = new DBPedidosAriba().GetPedidoPendientes;
                Utils.GuardarBitacora("Obtuvo los siguientes pedidos: " + oLstPedidos.Count.S());
                foreach (PedidoAriba oF in oLstPedidos)
                {
                    if (CreateSapDoc(oF))
                    {
                        new DBPedidosAriba().DBSetActualizaEstatusPedido(oF, 2);
                    }
                    else
                        new DBPedidosAriba().DBSetActualizaEstatusPedido(oF, 1);
                }


                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateSapDoc(PedidoAriba oF)
        {
            bool ban = false;

            SAPbobsCOM.Documents oSapDoc = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
            oSapDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
            //oSapDoc.DocObjectCodeEx = "13";

            //Facturas --------------  13
            //Nota de credito -------  14
            //socios de negocio -----  2
            //Factura de proveedores-  18

            try
            {
                MyGlobals.sStepLog = "CreateSapDoc: Empresa[ ARIBA ] - ID[" + oF.IdPedido.S() + "]";
                Utils.GuardarBitacora(MyGlobals.sStepLog);

                int iDiasVenc = Globales.GetConfigApp<string>("DiasAdicionAriba").S().I();
                DateTime dtFechaPed = oF.OrderTimeStamp.Substring(0, 19).S().Dt();

                //string Cliente = oF.OrderRHBillName == "Merck S.A. de C.V." ? "MERCK" : oF.OrderRHBillName;
                object oCardCode = new DBMetodos().GetValueByQuery("select top 1 CardCode from OCRD (NOLOCK)WHERE U_CompanyCode = '" + oF.OrderRHBillFaxAddressID + "'");
                if (!String.IsNullOrEmpty(oCardCode.S()))
                {
                    oSapDoc.CardCode = oCardCode.S();
                }

                // Obtiene el Address mediante la dirección de envío
                object oAddress = new DBMetodos().GetValueByQuery("select top 1 [Address] from CRD1 (NOLOCK) WHERE U_ShipToAddressID = '" + oF.OrderRHShipDeliverAddressID.S() + "'");
                if (!String.IsNullOrEmpty(oAddress.S()))
                {
                    oSapDoc.ShipToCode = oAddress.S();
                }


                oSapDoc.DocDate = dtFechaPed;
                oSapDoc.DocDueDate = dtFechaPed.AddDays(iDiasVenc);  // IMPORTANTE
                oSapDoc.NumAtCard = oF.OrderRHBillExtrinOrderID;
                //oSapDoc.Comments = oF.OrderRHComments.Length > 253 ? oF.OrderRHComments;
                oSapDoc.DocCurrency = oF.OrderRHCurrency == "MXN" ? "$" : "";
                oSapDoc.DiscountPercent = double.Parse(0.S());

                oSapDoc.UserFields.Fields.Item("U_PedidoAutorizado").Value = 99; //oF.autorizado;

                //if (oF.NumCtroCosto != string.Empty)
                //    oSapDoc.UserFields.Fields.Item("U_ClvCentCosto").Value = oF.NumCtroCosto.S();

                //if (oF.NumUsrCte > 0)
                //    oSapDoc.UserFields.Fields.Item("U_NumUsrMod").Value = oF.NumUsrCte.S();

                oSapDoc.UserFields.Fields.Item("U_Comment").Value = oF.OrderRHComments;
                oSapDoc.UserFields.Fields.Item("U_NomUsuaCliente").Value = oF.olsConceptos[0].ExtriItemDeliverTo;


                //oSapDoc.UserFields.Fields.Item("U_DescripcionCC").Value = oF.DescripcionCC;
                //oSapDoc.UserFields.Fields.Item("U_Area").Value = oF.Area;
                //oSapDoc.UserFields.Fields.Item("U_Autorizador").Value = oF.Autorizador.S();
                //oSapDoc.UserFields.Fields.Item("U_Autorizadores").Value = oF.Autorizadores;


                //if (oF.FechaAutoriza.S().Length > 10)
                //{
                //    oSapDoc.UserFields.Fields.Item("U_FechaAutoriza").Value = oF.FechaAutoriza.S().Substring(0, 10);
                //}
                //else
                //    oSapDoc.UserFields.Fields.Item("U_FechaAutoriza").Value = oF.FechaAutoriza.S();

                //oSapDoc.UserFields.Fields.Item("U_CveDireccionEnvio").Value = oF.ClaveDireccionEnvio;
                //oSapDoc.UserFields.Fields.Item("U_IP_Source").Value = oF.IPSource;
                //oSapDoc.UserFields.Fields.Item("U_FolioNR").Value = oF.FolioNR;
                oSapDoc.UserFields.Fields.Item("U_Pedido").Value = oF.OrderRHBillExtrinOrderID;
                //oSapDoc.UserFields.Fields.Item("U_PedidoAutorizado").Value = oF.PedidoAutorizado == 1 ? 99 : oF.PedidoAutorizado;

                oSapDoc.Confirmed = SAPbobsCOM.BoYesNoEnum.tYES;

                //oSapDoc.UserFields.Fields.Item("U_NumPedidoEBO").Value = oF.NumPeridoEBO;
                //oSapDoc.UserFields.Fields.Item("U_CenCostClie").Value = oF.sCtoCostoCli;
                oSapDoc.UserFields.Fields.Item("U_PMX_PLTY").Value = "LO";
                oSapDoc.TransportationCode = 1;
                
                string sWhsCode = string.Empty;
                string sQuery = "SELECT TOP 1 T3.WhsCode, T3.BPLid FROM dbo.OCRD AS T0  LEFT OUTER JOIN dbo.CRD1 AS T1 ON T0.CardCode = T1.CardCode  LEFT OUTER JOIN dbo.OWHS AS T3 ON T1.U_Almacen = T3.WhsCode WHERE T1.[Address] = '" + oAddress.S() + "'";
                DataTable dtDirEnv = new DBSAP_PRO().oBD_SP.EjecutarDT_DeQuery(sQuery);
                //Utils.GuardarBitacora("Registros: " + dtDirEnv.Rows.Count.S());
                if (dtDirEnv.Rows.Count > 0)
                {
                    oSapDoc.BPL_IDAssignedToInvoice = dtDirEnv.Rows[0]["BPLid"].S().I();
                    sWhsCode = dtDirEnv.Rows[0]["WhsCode"].S();
                }
                else
                {
                    sWhsCode = "10010"; // TLACEN
                    oSapDoc.BPL_IDAssignedToInvoice = 2;
                    oSapDoc.UserFields.Fields.Item("U_Comment").Value = oF.OrderRHComments + ", NO SE ENCONTRÓ UN ALMACEN, POR LO CUAL SE ASIGNO POR DEFAULT TLACEN. FAVOR DE VERIFICARLO!";
                }



                string sTaxCode = string.Empty;
                int iLinea = 1;
                foreach (ConceptosPedidoAriba oCF in oF.olsConceptos)
                {
                    string sQueryItem = "SELECT TOP 1 U_ClaveProdServ, U_ClaveUnidad, TaxCodeAR, ItemName  FROM OITM oi WHERE oi.ItemCode = '" + oCF.SupplierPartID + "'";
                    DataTable dt = new DBSAP_PRO().oBD_SP.EjecutarDT_DeQuery(sQueryItem);

                    string sClaveUnidad = string.Empty;
                    string sClaveProdServ = string.Empty;

                    if (dt.Columns.Count == 0)
                    {
                        throw new Exception("No existe el item " + oCF.SupplierPartID + " en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] ");
                    }
                    else if (dt.Rows.Count > 0)
                    {
                        sClaveUnidad = dt.Rows[0]["U_ClaveUnidad"].S();
                        sClaveProdServ = dt.Rows[0]["U_ClaveProdServ"].S();
                    }
                    else
                    {
                        throw new Exception("No existe el item " + oCF.SupplierPartID + " en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] ");
                    }

                    int iOuMEntry = new DBMetodos().GetValueByQuery("SELECT TOP 1 ou.UomEntry FROM OITM oi INNER JOIN OUOM ou ON oi.InvntryUom = ou.UomName WHERE oi.ITEMCODE ='" + oCF.SupplierPartID + "' ").S().I();
                    oSapDoc.Lines.UoMEntry = iOuMEntry;

                    string sCodBar = new DBMetodos().GetValueByQuery("SELECT TOP 1 BcdCode  FROM OITM oi INNER JOIN OUOM ou ON oi.InvntryUom = ou.UomName INNER JOIN OBCD ob ON ou.UomEntry = ob.UomEntry WHERE oi.ItemCode = '" + oCF.SupplierPartID + "'").S();
                    string sCosCode2 = new DBMetodos().GetValueByQuery("SELECT TOP 1 U_RutaRep FROM CRD1 WHERE [Address] = '" + oAddress.S() + "'").S();
                    string sCosCode4 = new DBMetodos().GetValueByQuery("SELECT TOP 1 op.PrcCode FROM OCRD ocr INNER JOIN OPRC op ON ocr.CardName = op.PrcName WHERE ocr.CardCode = '" + oCardCode.S() + "'").S();
                    string sCodImpuesto = dt.Rows[0]["TaxCodeAR"].S();
                    string sNombreItem = dt.Rows[0]["ItemName"].S();

                    // FACTURA DE ARTICULOS
                    oSapDoc.Lines.ItemCode = oCF.SupplierPartID.S();
                    oSapDoc.Lines.ItemDescription = sNombreItem;
                    oSapDoc.Lines.Quantity = oCF.ScheQuantity.S().Db();
                    oSapDoc.Lines.BarCode = sCodBar;
                    oSapDoc.Lines.UnitPrice = oCF.UnitPrice.S().Db();

                    // FACTOR DE IMPUESTOS
                    Utils.GuardarBitacora("Item: " + oCF.SupplierPartID + " Impuesto: " + sCodImpuesto);
                    oSapDoc.Lines.TaxCode = sCodImpuesto;

                    if (!String.IsNullOrEmpty(sWhsCode))
                    {
                        oSapDoc.Lines.WarehouseCode = sWhsCode.S();
                    }

                    //oSapDoc.Lines.CostingCode = oCF.dimension1;
                    oSapDoc.Lines.CostingCode2 = sCosCode2;
                    //oSapDoc.Lines.CostingCode3 = oCF.dimension3;
                    oSapDoc.Lines.CostingCode4 = sCosCode4;
                    //oSapDoc.Lines.CostingCode5 = oCF.dimension5;
                    //oSapDoc.Lines.ProjectCode = oCF.proyecto;

                    oSapDoc.Lines.UserFields.Fields.Item("U_LineNum").Value = iLinea.S();
                    oSapDoc.Lines.UserFields.Fields.Item("U_ClaveUnidad").Value = sClaveUnidad;
                    oSapDoc.Lines.UserFields.Fields.Item("U_ClaveProdServ").Value = sClaveProdServ;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_ValorConcurso").Value = oCF.ValorConcurso;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_ClaveArtCte").Value = oCF.ClaveArticuloCliente;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_DescArtCte").Value = sNombreItem;

                    oSapDoc.Lines.Add();

                    iLinea++;
                }

                string sMensaje = string.Empty;
                if (oSapDoc.Add() != 0)
                {
                    oF.oEstatus.iEstatus = 0;
                    sMensaje = "Error al guardar el documento en SAP.  [" + MyGlobals.oCompany.GetLastErrorCode().S() + "] - " + MyGlobals.oCompany.GetLastErrorDescription();
                    Utils.GuardarBitacora(sMensaje);
                    throw new Exception(sMensaje);
                }
                else
                {
                    ban = true;
                    oF.oEstatus.iEstatus = 1;
                    oF.oEstatus.iSapDoc = MyGlobals.oCompany.GetNewObjectKey().S().I();

                    if (oF.oEstatus.iSapDoc < 1)
                        oF.oEstatus.iSapDoc = new DBMetodos().GetValueByQuery("SELECT TOP 1 DocNum FROM ORDR WHERE DocEntry = " + oF.oEstatus.iSapDoc.S()).S().I();
                    else
                        oF.oEstatus.sMensaje = "Se creo una orden de venta en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] - DocNum[" + oF.oEstatus.iSapDoc.S() + "] - ID_Tabla[" + oF.oEstatus.iID + "]";
                }

                return ban;
            }
            catch (Exception ex)
            {
                string sMsg = string.Empty;
                sMsg = "Error al importar registro de DB Intermedia. Tabla[" + oF.oEstatus.sTabla + "] - " + "ID[" + oF.oEstatus.iID.S() + "] Mensaje de Error: " + ex.Message;
                oF.oEstatus.iEstatus = 0;
                oF.oEstatus.sMensaje = sMsg.Replace("'", "");

                return false;
            }
            finally
            {
                Utils.DestroyCOMObject(oSapDoc);
            }
        }

    }
}
