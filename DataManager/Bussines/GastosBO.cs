using DataIntegrator.Clases;
using DataIntegrator.DataAccess;
using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.Core;
using System.Data;
using System.Configuration;

namespace DataIntegrator.Bussines
{
    public class GastosBO
    {
        public bool Import()
        {
            try
            {
                bool ban = false;
                MyGlobals.sStepLog = "Prepara Gastos de Concur";
                PreparaGastos();
                List<Gasto> oLstGastos = new List<Gasto>();
                MyGlobals.sStepLog = "Consulta Facturas Pendientes";
                oLstGastos = new DBGastos().GetGastosPendientes;
                foreach (Gasto oF in oLstGastos)
                {
                    if (CreateSapDoc(oF))
                        new DBMetodos().ActualizaRegistro(oF.oEstatus);
                    else
                        new DBMetodos().ActualizaRegistro(oF.oEstatus);
                }

                RegresaGastosAPendientesDeProcesar();

                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void PreparaGastos()
        {
            try
            {
                DBGastos oG = new DBGastos();
                System.Data.DataSet ds = oG.DBGetObtieneGastosConcur;

                foreach (DataRow rowH in ds.Tables[0].Rows)
                {
                    DataRow[] rowsD = ds.Tables[1].Select("Id = " + rowH.S("Id"));
                    for (int i = 0; i < rowsD.Length; i++)
                    {
                        string sItemCode = oG.DBGetObtieneClaveArticulo(rowsD[i]["TipoDeGasto"].S());
                        string sTaxCode = ObtieneTaxCode(rowsD[i]["TasadeIva"].S());
                        string sFlota = oG.DBGetObtieneFlotaMatricula("Matricula");

                        rowsD[i]["Descripcion"] = rowsD[i]["TipoDeGasto"].S();
                        rowsD[i]["TaxCode"] = sTaxCode;

                        if (sTaxCode == string.Empty)
                        {
                            rowH["IsOK"] = 0;
                            rowH["Msg"] = "No se encontró el codigo de IVA: " + rowsD[i]["TasadeIva"].S();
                            break;
                        }
                        else
                            rowsD[i]["Item"] = sItemCode;


                        if (sItemCode == string.Empty)
                        {
                            rowH["IsOK"] = 0;
                            rowH["Msg"] = "No se encontró el item: " + rowsD[i]["TipoDeGasto"].S();
                            break;
                        }
                        else
                            rowsD[i]["Item"] = sItemCode;
                    }
                }

                foreach (DataRow rowH in ds.Tables[0].Rows)
                {
                    if (rowH["IsOk"].S() == "1")
                    {
                        //Inserta gastos en tablas finales.
                        Gasto oGasto = new Gasto();
                        oGasto.sEmpresa = Helpers.sEmpresa;
                        int iIdGasto = new DBMetodos().GetObtieneConsecutivoPorClave("FACTCONCUR");
                        int iIdHeader = rowH["Id"].S().I();
                        oGasto.iId = iIdGasto;
                        oGasto.sSucursal = Helpers.sSucursal;
                        oGasto.dtFecha = rowH["FechaFactura"].S().Dt();
                        oGasto.sEmpleado = rowH["ClaveEmpleado"].S();  //GetObtieneClienteAleatorio(); //
                        oGasto.sNoReporte = rowH["NumeroReporte"].S();
                        oGasto.sFormaPago = rowH["FormaPago"].S();
                        oGasto.sComentarios = rowH["ReportDescrip"].S();
                        oGasto.sSerie = Helpers.sSerie;
                        oGasto.sMoneda = rowH["TipoMoneda"].S();
                        oGasto.dDescuento = rowH["Descuento"].S().D();
                        oGasto.dTipoCambio = rowH["TipoCambio"].S().D();
                        oGasto.sMsg = rowH["Msg"].S();

                        int iH = oG.DBSetInsertaHeaderGasto(oGasto);

                        DataRow[] rowsD = ds.Tables[1].Select("Id = " + iIdHeader);

                        int iCont = 0;
                        for (int i = 0; i < rowsD.Length; i++)
                        {
                            iCont++;
                            ConceptosGasto oCG = new ConceptosGasto();
                            oCG.lIdGasto = rowsD[i]["IdGasto"].S().L();
                            oCG.sEmpresa = Helpers.sEmpresa;
                            oCG.iId = iIdGasto;
                            oCG.iLinea = iCont;
                            oCG.sItem = rowsD[i]["Item"].S();
                            oCG.sDescripcionUsuario = rowsD[i]["Descripcion"].S();

                            if(rowsD[i]["DiferenciaImp"].S().D() == 0)
                                oCG.dPrecio = rowsD[i]["PrecioUnit"].S().D();
                            else
                                oCG.dPrecio = rowsD[i]["BaseCalculada"].S().D();

                            oCG.dDescuento = rowsD[i]["Descuento"].S().D();
                            oCG.dImpuesto = rowsD[i]["TasadeIva"].S().Replace("%", "").S().D();
                            oCG.sCodigoImpuesto = rowsD[i]["TaxCode"].S();

                            if (rowsD[i]["DiferenciaImp"].S().D() == 0)
                                oCG.dTotalLinea = rowsD[i]["PrecioUnit"].S().D();
                            else
                                oCG.dTotalLinea = rowsD[i]["BaseCalculada"].S().D();

                            oCG.sCuenta = rowsD[i]["Cuenta"].S();
                            oCG.sAlmacen = string.Empty;
                            oCG.sProyecto = rowsD[i]["Proyecto"].S();

                            if (new DBGastos().DBGetObtieneDimensionPorCodigo(rowsD[i]["Dimension2"].S()) == "1")
                                oCG.sDimension1 = rowsD[i]["Dimension2"].S();
                            else 
                                oCG.sDimension2 = rowsD[i]["Dimension2"].S();

                            oCG.sDimension3 = rowsD[i]["Dimension3"].S();
                            oCG.sDimension4 = rowsD[i]["Dimension4"].S();
                            oCG.sDimension5 = rowsD[i]["Dimension5"].S();
                            oCG.sDescripcionAmpliada = rowsD[i]["DescripcionAmpliada"].S();
                            
                            int iDet = oG.DBSetInsertaDetalleGasto(oCG);

                            if (rowsD[i]["DiferenciaImp"].S().D() != 0)
                            {
                                ConceptosGasto oCG2 = new ConceptosGasto();
                                oCG2 = oCG;

                                //Inserta linea adicional
                                iCont++;
                                oCG2.dPrecio = rowsD[i]["DiferenciaImp"].S().D();
                                oCG2.sCodigoImpuesto = "IVAANA";
                                oCG2.iLinea = iCont;
                                
                                int iVal = oG.DBSetInsertaDetalleGasto(oCG2);
                            }

                            oG.DBSetMarcaProcesadoGasto(rowsD[i]["IdGasto"].S().I(), "Procesado");
                        }
                    }
                    else
                    {
                        DataRow[] rowsD = ds.Tables[1].Select("Id = " + rowH.S("Id"));
                        for (int i = 0; i < rowsD.Length; i++)
                        {
                            oG.DBSetMarcaProcesadoGasto(rowsD[i]["IdGasto"].S().I(), rowH["Msg"].S());
                        }
                    }
                }
                
                #region NOTAS
                /*
                    1.- Consulta los gastos a procesar de Concur
                    2.- Busca los articulos en SAP Basado en el Tipo de Gasto
                    3.- Desglosa el IVA y verificar si es necesario agregar nuevo gasto con el mismo artículo
                    4.- Si agregó nuevo gasto por IVA, asociarlo al mismo numero de voucher
                    
                    5.- 
                    
                    
                    Header
                        oSapDoc.CardCode        claveEmpleado   
                        oSapDoc.DocDate         FechaFactura    
                        oSapDoc.NumAtCard       NumeroReporte   
                        oSapDoc.DocCurrency     USD y MXP Todo lo que sea moneda extranjera, se debe mandar a USD
                        oSapDoc.Comments        ReporDescription
                        oSapDoc.DiscountPercent 0

                        CAMPO USUARIO
                           oSapDoc.U_REFCONCUR  FormaPago
                    
                    Lineas
                        sTaxCode        
                        oSapDoc.Lines.ItemCode          = oCF.sItem;                    Documento a buscar en SAP
                        oSapDoc.Lines.ItemDescription   = oCF.sDescripcionUsuario;      No se envía
                        oSapDoc.Lines.Quantity          = oCF.iCantidad.S().Db();       siempre 1
                        oSapDoc.Lines.UnitPrice         = oCF.dPrecio.S().Db();         Gasto sin IVA
                        oSapDoc.Lines.DiscountPercent   = oCF.dDescuento.S().Db();      0
                        //oSapDoc.Lines.Currency
                        //oSapDoc.Lines.Rate
                        oSapDoc.Lines.TaxCode           = sTaxCode;                     Tasa de IVA (Archivo de Configuracion)
                        oSapDoc.Lines.ProjectCode       = oCF.sProyecto;                
                        oSapDoc.Lines.CostingCode       = oCF.sDimension1;              vacio
                        oSapDoc.Lines.CostingCode2      = oCF.sDimension2;              CodigoUnidad2
                        oSapDoc.Lines.CostingCode3      = oCF.sDimension3;              CodigoUnidad3
                        oSapDoc.Lines.CostingCode4      = oCF.sDimension4;              AreaGerencia
                        oSapDoc.Lines.CostingCode4      = oCF.sDimension5;              vacio

                 */
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Este método regresa los gastos de concur que no se generaron de manera adecuada por catálogos 
        /// a un punto anterior, de tal modo que cuando se actualice la información pasen correctamente.
        /// </summary>
        private void RegresaGastosAPendientesDeProcesar()
        {
            try
            {
                new DBGastos().DBSetRegresaGastosConcurPasoAnterior();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string ObtieneTaxCode(string sIva)
        {
            try
            {
                string sCodigoIva = string.Empty;
                string sIvaN = ConfigurationManager.AppSettings["IvaNal"].S();
                string sIvaI = ConfigurationManager.AppSettings["IvaInt"].S();

                if (sIvaN != string.Empty)
                {
                    string[] sIVas = sIvaN.Split('|');
                    if (sIva == sIVas[0])
                        sCodigoIva = sIVas[1];
                }

                if (sIvaI != string.Empty)
                {
                    string[] sIVasI = sIvaI.Split('|');
                    if (sIva == sIVasI[0])
                        sCodigoIva = sIVasI[1];
                }

                if (sIva == "0%")
                    sCodigoIva = "IVAA00";

                return sCodigoIva;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool CreateSapDoc(Gasto oF)
        {
            bool ban = false;
            SAPbobsCOM.Documents oSapDoc = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            oSapDoc.DocObjectCodeEx = "18";

            //Facturas --------------  13
            //Nota de credito -------  14
            //socios de negocio -----  2
            //Factura de proveedores-  18

            try
            {
                MyGlobals.sStepLog = "CreateSapDoc: Empresa[" + oF.sEmpresa.S() + "] - ID[" + oF.iId.S() + "]";
                
                oSapDoc.CardCode = oF.sEmpleado;
                oSapDoc.DocDate = oF.dtFecha;
                oSapDoc.DocDueDate = oF.dtFecha.AddMonths(1);
                oSapDoc.NumAtCard = oF.sNoReporte;
                oSapDoc.DocCurrency = oF.sMoneda;
                oSapDoc.Comments = oF.sComentarios;
                oSapDoc.Series = Helpers.sSerie.S().I();
                oSapDoc.DiscountPercent = double.Parse(oF.dDescuento.S());
                string sTaxCode = string.Empty;
                oSapDoc.DocRate = double.Parse(oF.dTipoCambio.S());
                //oSapDoc.UserFields.Fields.Item("U_REFCONCUR").Value = "Interfaz";
                oSapDoc.UserFields.Fields.Item("U_TIPOCOMP").Value = oF.sFormaPago;
                oSapDoc.ControlAccount = ConfigurationManager.AppSettings["CuentaEmp"].S();

                foreach (ConceptosGasto oCF in oF.oLstConceptos)
                {
                    if (new DBMetodos().GetValueByQuery("SELECT COUNT(1) FROM OITM WHERE ITEMCODE='" + oCF.sItem + "' ").S().I() == 0)
                    {
                        throw new Exception("No existe el item " + oCF.sItem + " en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] ");
                    }

                    sTaxCode = oCF.sCodigoImpuesto;

                    // FACTURA DE ARTICULOS
                    oSapDoc.Lines.ItemCode = oCF.sItem;
                    oSapDoc.Lines.ItemDescription = oCF.sDescripcionUsuario;
                    //oSapDoc.Lines.Text = "";
                    oSapDoc.Lines.Quantity = oCF.iCantidad.S().Db();
                    oSapDoc.Lines.UnitPrice = oCF.dPrecio.S().Db();
                    oSapDoc.Lines.DiscountPercent = oCF.dDescuento.S().Db();

                    oSapDoc.Lines.TaxCode = sTaxCode;
                    if (!String.IsNullOrEmpty(oCF.sAlmacen))
                    {
                        oSapDoc.Lines.WarehouseCode = oCF.sAlmacen;
                    }

                    //oSapDoc.Lines.AccountCode = oCF.sCuenta;
                    oSapDoc.Lines.ProjectCode = oCF.sProyecto;
                    
                    oSapDoc.Lines.CostingCode =  oCF.sDimension1;
                    oSapDoc.Lines.CostingCode2 = oCF.sDimension2;
                    oSapDoc.Lines.CostingCode3 = oCF.sDimension3;
                    oSapDoc.Lines.CostingCode4 = oCF.sDimension4;
                    oSapDoc.Lines.CostingCode5 = oCF.sDimension5;
                    oSapDoc.Lines.UserFields.Fields.Item("U_OBS").Value = oCF.sDescripcionAmpliada;
                    oSapDoc.Lines.Add();    


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
                }

                string sMensaje = string.Empty;
                if (oSapDoc.Add() != 0)
                {
                    oF.oEstatus.iEstatus = 0;
                    sMensaje = "Error al guardar el documento en SAP.  [" + MyGlobals.oCompany.GetLastErrorCode().S() + "] - " + MyGlobals.oCompany.GetLastErrorDescription();
                    oF.oEstatus.sMensaje = sMensaje;
                }
                else
                {
                    ban = true;
                    oF.oEstatus.iEstatus = 1;
                    oF.oEstatus.iSapDoc = MyGlobals.oCompany.GetNewObjectKey().S().I();

                    if (oF.oEstatus.iSapDoc < 1)
                        oF.oEstatus.iSapDoc = new DBMetodos().GetValueByQuery("SELECT MAX(DocEntry) FROM OINV WHERE DataSource='O' AND UserSign=" + MyGlobals.oCompany.UserSignature.S()).S().I();
                    else
                        oF.oEstatus.sMensaje = "Se creo una factura de proveedor en SAP. DB[" + MyGlobals.oCompany.CompanyDB + "] - DocEntry[" + oF.oEstatus.iSapDoc.S() + "] - ID_Tabla[" + oF.oEstatus.iID + "]";
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

                return false;
            }
            finally
            {
                Utils.DestroyCOMObject(oSapDoc);
            }
        }
        private string GetObtieneClienteAleatorio()
        {
            try
            {
                string[] sEmpleados = new string[]{"E000001", "E000010", "E000015", "E000018",
                                                        "E000022", "E000043", "E000045", "E000046",
                                                        "E000049", "E000254", "E000705", "E000999", "E001180"};
                int iRandom = new Random().Next(0, 12);

                return sEmpleados[iRandom];
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
