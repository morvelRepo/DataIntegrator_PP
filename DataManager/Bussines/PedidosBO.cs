using DataIntegrator.Clases;
using DataIntegrator.DataAccess;
using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.Core;
using System.Data;
using System.Net;
using System.IO;


using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.Web.Script.Serialization;

namespace DataIntegrator.Bussines
{
    public class PedidosBO
    {
        public bool Import()
        {
            try
            {
                bool ban = false;
                new DBPedidos().TransfierePedidosAntiguoCarrito();
                List<Pedido> oLstPedidos = new List<Pedido>();
                MyGlobals.sStepLog = "Consulta Pedidos Pendientes";
                Utils.GuardarBitacora(MyGlobals.sStepLog);
                oLstPedidos = new DBPedidos().GetPedidoPendientes;
                Utils.GuardarBitacora("Obtuvo los siguientes pedidos: " + oLstPedidos.Count.S());
                foreach (Pedido oF in oLstPedidos)
                {
                    if (CreateSapDoc(oF))
                    {
                        new DBMetodos().ActualizaRegistro(oF.oEstatus);
                        if (oF.IdCarrito.S().I() == 0)
                        {
                            try
                            {
                                ActualizaPedidoFB(oF.IdCarrito, oF.oEstatus.iSapDoc);
                            }
                            catch (Exception ex)
                            {
                                Utils.GuardarBitacora("Error paso 1  desc:" + ex.Message);
                            }

                            try
                            {
                                ActualizaPedidoNew(oF.IdCarrito, oF.oEstatus.iSapDoc);
                            }
                            catch (Exception ex)
                            {
                                Utils.GuardarBitacora("Error paso 2  desc:" + ex.Message);
                            }
                        }
                    }
                    else
                        new DBMetodos().ActualizaRegistro(oF.oEstatus);
                }

                ActualizaEstatusFacturas();

                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateSapDoc(Pedido oF)
        {
            bool ban = false;
            //SAPbobsCOM.Documents oFactProv = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
            //SAPbobsCOM.Documents oSapDoc = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            //SAPbobsCOM.Documents oSapDoc = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            SAPbobsCOM.Documents oSapDoc = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
            oSapDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
            //oSapDoc.DocObjectCodeEx = "13";

            //Facturas --------------  13
            //Nota de credito -------  14
            //socios de negocio -----  2
            //Factura de proveedores-  18

            //U_PMX_PLTY    -- LO local
            //Tmspcope      -- Local

            try
            {
                MyGlobals.sStepLog = "CreateSapDoc: Empresa[" + oF.empresa.S() + "] - ID[" + oF.id.S() + "]";
                Utils.GuardarBitacora(MyGlobals.sStepLog);

                // Obtiene el Address mediante la dirección de envío
                if (!String.IsNullOrEmpty(oF.ClaveDireccionEnvio))
                {
                    object oAddress = new DBMetodos().GetValueByQuery("SELECT TOP 1 [Address] FROM CRD1 (NOLOCK) WHERE U_ClaveEnvio = '" + oF.ClaveDireccionEnvio + "'");
                    if (!String.IsNullOrEmpty(oAddress.S()))
                    {
                        oSapDoc.ShipToCode = oAddress.S();
                    }
                }

                oSapDoc.DocDate = oF.fecha;
                oSapDoc.DocDueDate = oF.fecha.AddDays(1);  // IMPORTANTE
                oSapDoc.CardCode = oF.cardcode;
                oSapDoc.NumAtCard = oF.referencia;
                oSapDoc.Comments = oF.comentarios;
                oSapDoc.DocCurrency = oF.moneda;
                oSapDoc.DiscountPercent = double.Parse(oF.descuento.S());
                oSapDoc.UserFields.Fields.Item("U_PedidoAutorizado").Value = oF.autorizado;

                //if (oF.NumUsrCte > 0)
                //    oSapDoc.UserFields.Fields.Item("U_NumUsrCte").Value = oF.NumUsrCte.S();

                if (oF.NumCtroCosto != string.Empty)
                    //oSapDoc.UserFields.Fields.Item("U_CenCostClie").Value = oF.NumCtroCosto.S();
                    oSapDoc.UserFields.Fields.Item("U_ClvCentCosto").Value = oF.NumCtroCosto.S();

                //if (oF.NumUsrMod > 0)
                //    oSapDoc.UserFields.Fields.Item("U_NumUsrMod").Value = oF.NumUsrMod.S();
                if (oF.NumUsrCte > 0)
                    oSapDoc.UserFields.Fields.Item("U_NumUsrMod").Value = oF.NumUsrCte.S();


                oSapDoc.UserFields.Fields.Item("U_UsuarioCliente").Value = oF.UsrCte;
                oSapDoc.UserFields.Fields.Item("U_NomUsuaCliente").Value = oF.NomUsuaCte;
                oSapDoc.UserFields.Fields.Item("U_DescripcionCC").Value = oF.DescripcionCC;
                oSapDoc.UserFields.Fields.Item("U_Area").Value = oF.Area;
                oSapDoc.UserFields.Fields.Item("U_Autorizador").Value = oF.Autorizador.S();
                oSapDoc.UserFields.Fields.Item("U_Autorizadores").Value = oF.Autorizadores;


                if (oF.FechaAutoriza.S().Length > 10)
                {
                    oSapDoc.UserFields.Fields.Item("U_FechaAutoriza").Value = oF.FechaAutoriza.S().Substring(0, 10);
                }
                else
                    oSapDoc.UserFields.Fields.Item("U_FechaAutoriza").Value = oF.FechaAutoriza.S();



                //oSapDoc.UserFields.Fields.Item("prueba").Value = oF.DireccionFactura;
                oSapDoc.UserFields.Fields.Item("U_CveDireccionEnvio").Value = oF.ClaveDireccionEnvio;
                //oSapDoc.UserFields.Fields.Item("prueba").Value = oF.DireccionEnvio;
                oSapDoc.UserFields.Fields.Item("U_IP_Source").Value = oF.IPSource;
                oSapDoc.UserFields.Fields.Item("U_FolioNR").Value = oF.FolioNR;
                oSapDoc.UserFields.Fields.Item("U_Pedido").Value = oF.sPedido == string.Empty ? oF.NumPeridoEBO.S() : oF.sPedido;
                oSapDoc.UserFields.Fields.Item("U_PedidoAutorizado").Value = oF.PedidoAutorizado == 1 ? 99 : oF.PedidoAutorizado;

                oSapDoc.Confirmed = oF.PedidoAutorizado == 1 ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;

                oSapDoc.UserFields.Fields.Item("U_NumPedidoEBO").Value = oF.NumPeridoEBO;
                oSapDoc.UserFields.Fields.Item("U_CenCostClie").Value = oF.sCtoCostoCli;
                oSapDoc.UserFields.Fields.Item("U_PMX_PLTY").Value = "LO";
                oSapDoc.TransportationCode = 1;
                
                //oSapDoc.UserFields.Fields.Item("TrnspCode").Value = "Local";

                string sWhsCode = string.Empty;

                ConceptosPedido oCP1 = oF.conceptos[0];

                Utils.GuardarBitacora("BPLid: " + oCP1.BPLid.S());
                if (oCP1.BPLid == 0)
                {
                    string sQuery = "SELECT TOP 1 T3.WhsCode, T3.BPLid FROM dbo.OCRD AS T0  LEFT OUTER JOIN dbo.CRD1 AS T1 ON T0.CardCode = T1.CardCode  LEFT OUTER JOIN dbo.OWHS AS T3 ON T1.U_Almacen = T3.WhsCode WHERE T1.U_ClaveEnvio = '" + oF.ClaveDireccionEnvio + "'";
                    DataTable dtDirEnv = new DBSAP_PRO().oBD_SP.EjecutarDT_DeQuery(sQuery);
                    //Utils.GuardarBitacora("Registros: " + dtDirEnv.Rows.Count.S());
                    if (dtDirEnv.Rows.Count > 0)
                    {
                        oSapDoc.BPL_IDAssignedToInvoice = dtDirEnv.Rows[0]["BPLid"].S().I();
                        sWhsCode = dtDirEnv.Rows[0]["WhsCode"].S();
                        //Utils.GuardarBitacora(new DBSAP_PRO().oBD_SP.sConexionSQL);
                        //Utils.GuardarBitacora(sQuery);
                        //Utils.GuardarBitacora("WhsCode: " + dtDirEnv.Rows[0]["WhsCode"].S() + " y  BPLid: "+ dtDirEnv.Rows[0]["BPLid"].S() + ", DirEnvio: " + oF.ClaveDireccionEnvio);
                    }
                    else
                    {
                        sWhsCode = "10010"; // TLACEN
                        oSapDoc.BPL_IDAssignedToInvoice = 2;
                        oSapDoc.Comments = oF.comentarios + ", NO SE ENCONTRÓ UN ALMACEN, POR LO CUAL SE ASIGNO POR DEFAULT TLACEN. FAVOR DE VERIFICARLO!";
                    }
                }
                else
                {
                    oSapDoc.BPL_IDAssignedToInvoice = oCP1.BPLid;
                }

                



                string sTaxCode = string.Empty;
                foreach (ConceptosPedido oCF in oF.conceptos)
                {
                    if (sWhsCode != string.Empty)
                        oCF.WhsCode = sWhsCode;


                    string sQuery = "SELECT TOP 1 U_ClaveProdServ, U_ClaveUnidad FROM OITM oi WHERE oi.ItemCode = '" + oCF.item + "'";
                    DataTable dt = new DBSAP_PRO().oBD_SP.EjecutarDT_DeQuery(sQuery);

                    string sClaveUnidad = string.Empty;
                    string sClaveProdServ = string.Empty;

                    //if (new DBMetodos().GetValueByQuery("SELECT COUNT(1) FROM OITM WHERE ITEMCODE='" + oCF.item + "' ").S().I() == 0)
                    if (dt.Columns.Count == 0)
                    {
                        throw new Exception("No existe el item " + oCF.item + " en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] ");
                    }
                    else
                    {
                        sClaveUnidad = dt.Rows[0]["U_ClaveUnidad"].S();
                        sClaveProdServ = dt.Rows[0]["U_ClaveProdServ"].S();
                    }

                    int iOuMEntry = new DBMetodos().GetValueByQuery("SELECT TOP 1 ou.UomEntry FROM OITM oi INNER JOIN OUOM ou ON oi.InvntryUom = ou.UomName WHERE oi.ITEMCODE ='" + oCF.item + "' ").S().I();
                    oSapDoc.Lines.UoMEntry = iOuMEntry;

                    string sCodBar = new DBMetodos().GetValueByQuery("SELECT TOP 1 BcdCode  FROM OITM oi INNER JOIN OUOM ou ON oi.InvntryUom = ou.UomName INNER JOIN OBCD ob ON ou.UomEntry = ob.UomEntry WHERE oi.ItemCode = '" + oCF.item + "'").S();
                    string sCosCode2 = new DBMetodos().GetValueByQuery("SELECT TOP 1 U_RutaRep FROM CRD1 WHERE U_ClaveEnvio = " + oF.ClaveDireccionEnvio).S();
                    string sCosCode4 = new DBMetodos().GetValueByQuery("SELECT TOP 1 op.PrcCode FROM OCRD ocr INNER JOIN OPRC op ON ocr.CardName = op.PrcName WHERE ocr.CardCode = '" + oF.cardcode + "'").S();



                    // FACTURA DE ARTICULOS
                    oSapDoc.Lines.ItemCode = oCF.item;
                    oSapDoc.Lines.ItemDescription = oCF.descripcionusuario;
                    oSapDoc.Lines.Quantity = oCF.cantidad.S().Db();
                    oSapDoc.Lines.BarCode = sCodBar;
                    oSapDoc.Lines.UnitPrice = oCF.precio.S().Db();
                    //oSapDoc.Lines.DiscountPercent = oCF.descuento.S().Db();

                    // FACTOR DE IMPUESTOS
                    Utils.GuardarBitacora("Item: " + oCF.item + " Impuesto: " + oCF.impuesto.S());
                    oSapDoc.Lines.TaxCode = oCF.codimpuesto.S();

                    if (!String.IsNullOrEmpty(oCF.WhsCode))
                    {
                        oSapDoc.Lines.WarehouseCode = oCF.WhsCode.S();
                    }

                    
                    oSapDoc.Lines.CostingCode = oCF.dimension1;
                    oSapDoc.Lines.CostingCode2 = sCosCode2;
                    oSapDoc.Lines.CostingCode3 = oCF.dimension3;
                    oSapDoc.Lines.CostingCode4 = sCosCode4;
                    oSapDoc.Lines.CostingCode5 = oCF.dimension5;
                    oSapDoc.Lines.ProjectCode = oCF.proyecto;


                    oSapDoc.Lines.UserFields.Fields.Item("U_LineNum").Value = oCF.linea.S();
                    oSapDoc.Lines.UserFields.Fields.Item("U_ClaveUnidad").Value = sClaveUnidad;
                    oSapDoc.Lines.UserFields.Fields.Item("U_ClaveProdServ").Value = sClaveProdServ;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_UF_ServiciosCC").Value = oCF.UnidadMedida;
                    oSapDoc.Lines.UserFields.Fields.Item("U_ValorConcurso").Value = oCF.ValorConcurso;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_CostSTD").Value = oCF.CostoSTD;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_CostoReposicion").Value = oCF.CostoReposicion;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_CostoPromedio").Value = oCF.CostoPromedio;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_UF_ServiciosCC").Value = oCF.WhsCode;
                    oSapDoc.Lines.UserFields.Fields.Item("U_ClaveArtCte").Value = oCF.ClaveArticuloCliente;
                    oSapDoc.Lines.UserFields.Fields.Item("U_DescArtCte").Value = oCF.DescripcionArticuloCliente;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_UF_ServiciosCC").Value = oCF.DescripcionArticulo;
                    //oSapDoc.Lines.UserFields.Fields.Item("U_UF_ServiciosCC").Value = oCF.PrecioVentaFinal;


                    oSapDoc.Lines.Add();

                }

                string sMensaje = string.Empty;
                if (oSapDoc.Add() != 0)
                {
                    oF.oEstatus.iEstatus = 0;
                    sMensaje = "Error al guardar el documento en SAP.  [" + MyGlobals.oCompany.GetLastErrorCode().S() + "] - " + MyGlobals.oCompany.GetLastErrorDescription();
                    Utils.GuardarBitacora("Descripción error: " + MyGlobals.oCompany.GetLastErrorDescription());
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
                        oF.oEstatus.sMensaje = "Se creo una factura de venta en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] - DocNum[" + oF.oEstatus.iSapDoc.S() + "] - ID_Tabla[" + oF.oEstatus.iID + "]";
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

        private void ActualizaEstatusFacturas()
        {
            try
            {
                //DataTable dtFacturas = new DBFacturas().GetFacturasAProcesar;

                //foreach (DataRow row in dtFacturas.Rows)
                //{
                //    if (row["Estatus"].S() == "1")
                //    {
                //        if (new DBFacturas().GetVerificaSiFacturaCancelada(row["FolioSAP"].S().I()))
                //        {
                //            // FacturaCancelada
                //            ActualizaEstatusFacturaEnIntegratoryMJ(3, row["IdFactura"].S().I(), 0, row["FolioMJ"].S().I(), row["Tipo"].S().I());
                //        }
                //        else
                //        {
                //            //Verifica si ya fue asentada la factura en SAP, si es así, actualiza los folio en MexJet y la envía al 100%
                //            int iDocumento = new DBFacturas().GetObtieneFolioFacturaEnSAP(row["FolioSAP"].S().I());
                //            if (iDocumento > 0)
                //            {
                //                ActualizaEstatusFacturaEnIntegratoryMJ(2, row["IdFactura"].S().I(), iDocumento, row["FolioMJ"].S().I(), row["Tipo"].S().I());
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (new DBFacturas().GetVerificaSiFacturaCancelada(row["FolioSAP"].S().I()))
                //        {
                //            // FacturaCancelada
                //            ActualizaEstatusFacturaEnIntegratoryMJ(3, row["IdFactura"].S().I(), 0, row["FolioMJ"].S().I(), row["Tipo"].S().I());
                //        }
                //    }
                //}

            }
            catch (Exception)
            {
                throw;
            }
        }

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

        private void ActualizaEstatusFacturaEnIntegratoryMJ(int iStatus, int iIdFactura, int iFolioSAP, int iFolioMJ, int iTipoFact)
        {
            try
            {
                //if (iStatus == 3)
                //{
                //    //Factura cancelada, solo cambia de estatus la Factura en Integrator
                //    new DBFacturas().SetActualizaEstatusFacturaEnIntegrator(iIdFactura, 3);
                //    new DBFacturas().SetActualizaNumeroFacturaSAPEnMexJet("Cancelada", iFolioMJ, iTipoFact);
                //}
                //else
                //{
                //    //Factura asentada, adicional hay que marcar el folio en MexJet
                //    new DBFacturas().SetActualizaEstatusFacturaEnIntegrator(iIdFactura, 2);
                //    new DBFacturas().SetActualizaNumeroFacturaSAPEnMexJet(iFolioSAP.S(), iFolioMJ, iTipoFact);
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ActualizaPedidoFB(string IdCarrito, int iSapDoc)
        {
            //Invocar validacion SSL
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            try
            {
                ActualizacionCarrito oA = new ActualizacionCarrito();
                oA.idSAP = iSapDoc;


                string sURL = Globales.GetConfigApp<string>("PutIdPedido");
                sURL = sURL + "/" + IdCarrito;
                string str = string.Empty;

                str = oA.ToJson();

                Utils.GuardarBitacora("Intentará enviar 1: " + str);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "PUT";

                byte[] byteArray = Encoding.UTF8.GetBytes(str);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();


                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                JavaScriptSerializer ser = new JavaScriptSerializer();
                EstatusRes Er = ser.Deserialize<EstatusRes>(responseFromServer);
                if (Er != null)
                {
                    int succes = Er.success == true ? 1 : 0;
                    //Activar bandera de notificado a FB

                    if (succes == 1)
                        Utils.GuardarBitacora("Envío completado");
                }

                Utils.GuardarBitacora(responseFromServer.S());

                reader.Close();
                dataStream.Close();


                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void ActualizaPedidoNew(string IdCarrito, int iSapDoc)
        {
            //Invocar validacion SSL
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            try
            {
                ActualizacionCarrito oA = new ActualizacionCarrito();
                oA.idSAP = iSapDoc;


                string sURL = Globales.GetConfigApp<string>("PutIdPedidoNew");
                sURL = sURL + "/" + IdCarrito;
                string str = string.Empty;

                str = oA.ToJson();

                Utils.GuardarBitacora("Intentará enviar 2: " + str);
                Utils.GuardarBitacora("Url: " + sURL);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "PUT";

                byte[] byteArray = Encoding.UTF8.GetBytes(str);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                JavaScriptSerializer ser = new JavaScriptSerializer();
                EstatusRes Er = ser.Deserialize<EstatusRes>(responseFromServer);

                if (Er != null)
                {
                    int succes = Er.success == true ? 1 : 0;
                    //Activar bandera de notificado a FB

                    if (succes == 1)
                        Utils.GuardarBitacora("Envío completado");
                }

                Utils.GuardarBitacora(responseFromServer.S());

                reader.Close();
                dataStream.Close();

                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ImportPedidos()
        {
            try
            {
                bool ban = false;
                int iRes = 0;
                List<PagoPedidos> oLstPagos = new List<PagoPedidos>();
                MyGlobals.sStepLog = "Consulta Pagos de Pedidos Pendientes de procesar";
                Utils.GuardarBitacora(MyGlobals.sStepLog);
                oLstPagos = new DBPedidos().GetPagosPendientes;
                Utils.GuardarBitacora("Obtuvo los siguientes pagos: " + oLstPagos.Count.S());

                foreach (PagoPedidos oP in oLstPagos)
                {
                    try
                    {
                        SAPbobsCOM.Documents oSapDoc = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                        oSapDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;

                        if (oSapDoc.GetByKey(oP.iIdPedido.I()))
                        {
                            oSapDoc.UserFields.Fields.Item("U_IDTransaccion").Value = oP.sIdTransaccion;
                            oSapDoc.UserFields.Fields.Item("U_ReferenciaPago").Value = oP.sReferenciaPago;
                            oSapDoc.UserFields.Fields.Item("U_TipoPago").Value = oP.sTipoPago;
                            oSapDoc.UserFields.Fields.Item("U_FechaCreacionP").Value = oP.dtFechaCreacion.S();

                            iRes = oSapDoc.Update();
                        }

                        string sMensaje = string.Empty;

                        if (iRes != 0)
                        {
                            sMensaje = "Error al guardar el documento en SAP.  [" + MyGlobals.oCompany.GetLastErrorCode().S() + "] - " + MyGlobals.oCompany.GetLastErrorDescription();
                            Utils.GuardarBitacora("Error --> Pedido: " + oP.iIdPedido.S() + ", monto: " + oP.mMonto.S());
                            throw new Exception(sMensaje);
                        }
                        else
                        {
                            Utils.GuardarBitacora("Pedido: " + oP.iIdPedido.S() + " actualizado");
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
