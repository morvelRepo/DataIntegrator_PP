using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.Core;
using System.Data;
using DataIntegrator.Clases;

namespace DataIntegrator.DataAccess
{
    public class DBFacturas : DBBase
    {
        public List<Factura> GetFacturasPendientes
        {
            get
            {
                int iIdSAPTemp = 0;
                try
                {
                    DataSet dsRes = oBD_SP.EjecutarDS("[Principales].[spS_DI_ConsultaPagosRecibidosYPedidos]");
                    if (dsRes != null && dsRes.Tables[0].Rows.Count > 0)
                    {
                        List<Factura> oLst = dsRes.Tables[0].AsEnumerable().Select(r => new Factura()
                        {
                            iSapDoc = r["SapDoc"].S().I(),
                            id = r["ID"].S().I(),
                            empresa = r["Empresa"].S().I(),
                            sucursal = r["Sucursal"].S().I(),
                            fecha = r["Fecha"].S().Dt(),
                            cardcode = r["CardCode"].S(),
                            referencia = r["Referencia"].S(),
                            comentarios = r["Comentarios"].S(),
                            moneda = r["Moneda"].S(),
                            descuento = r["Descuento"].S().D(),
                            tipocambio = r["TipoCambio"].S().D(),
                            autorizado = r["Autorizado"].S().I(),
                            NumUsrCte = r["NumUsrCte"].S().I(),
                            UsrCte = r["UsrCte"].S(),
                            NomUsuaCte = r["NomUsuaCliente"].S(),
                            NumCtroCosto = r["NumCtroCosto"].S(),
                            DescripcionCC = r["DescripcionCC"].S(),
                            Area = r["Area"].S(),
                            NumUsrMod = r["NumUsrMod"].S().I(),
                            Autorizador = r["Autorizador"].S().I(),
                            Autorizadores = r["Autorizadores"].S(),
                            FechaAutoriza = r["FechaAutoriza"].S(),
                            DireccionFactura = r["DireccionFactura"].S(),
                            ClaveDireccionEnvio = r["ClaveDireccionEnvio"].S(),
                            DireccionEnvio = r["DireccionEnvio"].S(),
                            IPSource = r["IPSource"].S(),
                            FolioNR = r["FolioNR"].S(),
                            sPedido = r["Pedido"].S(),
                            PedidoAutorizado = r["PedidoAutorizado"].S().I(),
                            NumPeridoEBO = r["NumPedidoEBO"].S().I(),
                            IdCarrito = r["IdCarrito"].S(),
                            sCtoCostoCli = r["CenCostoCli"].S(),

                            sCuentaCont = r["CuentaContDeposito"].S(),
                            //iSapDoc = r["IdSAP_FR"].S().I(),
                            sIdTransaccion = r["IdTransaccion"].S(),
                            dMontoPago = r["MontoPago"].S().D(),
                            sReferenciaPago = r["ReferenciaPago"].S(),
                            sTipoPago = r["TipoPago"].S(),
                            sTipoPagoSAP = r["TipoPagoSAP"].S(),
                            sDescTipoPagoSAP = r["DescripcionSAP"].S()
                        }).ToList();

                        foreach (Factura oF in oLst)
                        {
                            // Obtiene conceptos de la factura
                            iIdSAPTemp = oF.id;
                            DataTable dtDetalle = dsRes.Tables[1].Select("ID = " + oF.id.S()).CopyToDataTable();
                            Utils.GuardarBitacora("Detalle: " + dsRes.Tables[1].Rows.Count.S() + ", ID_PEDIDO: " + oF.id.S() + ", No. Detalles: " + dtDetalle.Rows.Count.S());
                            if (dtDetalle.Rows.Count > 0)
                            {
                                // Obtiene conceptos de la factura
                                oF.conceptos = dtDetalle.AsEnumerable().Select(r => new ConceptosFactura()
                                {
                                    linea = r["Linea"].S().I(),
                                    item = r["Item"].S(),
                                    descripcionusuario = r["DescripcionUsuario"].S(),
                                    codbarras = r["CodBarras"].S(),
                                    cantidad = Convert.ToInt32(r["Cantidad"].S().D()),
                                    precio = r["Precio"].S().D(),
                                    descuento = r["Descuento"].S().D(),
                                    impuesto = r["Impuesto"].S().I(),
                                    codimpuesto = r["CodigoImpuesto"].S(),
                                    totallinea = r["TotalLinea"].S().D(),
                                    almacen = r["Almacen"].S(),
                                    proyecto = r["Proyecto"].S(),
                                    dimension1 = r["Dimension1"].S(),
                                    dimension2 = r["Dimension2"].S(),
                                    dimension3 = r["Dimension3"].S(),
                                    dimension4 = r["Dimension4"].S(),
                                    dimension5 = r["Dimension5"].S(),
                                    UnidadMedida = r["UnidadMedida"].S(),
                                    ValorConcurso = r["ValorConcurso"].S(),
                                    CostoSTD = r["CostoSTD"].S().D(),
                                    CostoReposicion = r["CostoReposicion"].S().D(),
                                    CostoPromedio = r["CostoPromedio"].S().D(),
                                    WhsCode = r["WhsCode"].S(),
                                    ClaveArticuloCliente = r["ClaveArticuloCliente"].S(),
                                    DescripcionArticuloCliente = r["DescripcionArticuloCliente"].S(),
                                    DescripcionArticulo = r["DescripcionArticulo"].S(),
                                    PrecioVentaFinal = r["PrecioVentaFinal"].S().D()
                                }).ToList();

                                oF.oEstatus.iID = oF.id;
                                oF.oEstatus.sEmpresa = oF.empresa.S();
                            }
                            else
                                Utils.GuardarBitacora("Pedido: " + oF.id.S() + ", no tiene lineas de detalle");
                        }

                        return oLst;
                    }
                    else
                        return new List<Factura>();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public bool SetActualizaFRenPedido(int iIdFR, int iIdPedido, int iIdPay)
        {
            try
            {
                object oRes = oBD_SP.EjecutarValor("[Principales].[spU_DI_ActualizaNumeroFacturaReservaEnPedido]", "@ID", iIdPedido,
                                                                                                                    "@IdSAP_FR", iIdFR,
                                                                                                                    "@IdSAP_PAY", iIdPay);

                return oRes.S().I() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public bool SetActualizaIdPagosEnPedido(int iIdSAPPay, int iIdPedido)
        //{
        //    try
        //    {
        //        object oRes = oBD_SP.EjecutarValor("[Principales].[spU_DI_ActualizaIDsPagos]", "@IdSAP_Pay", iIdSAPPay,
        //                                                                                        "@IdPedido", iIdPedido);

        //        return oRes.S().I() > 0 ? true : false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetFacturasAProcesar
        //{
        //    get
        //    {
        //        try
        //        {
        //            return oBD_SP.EjecutarDT("[Principales].[spS_DI_ConsultaFacturasGrabadasSAP]");
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //public bool GetVerificaSiFacturaCancelada(int iFolioSAP)
        //{
        //    try
        //    {
        //        string sCad = string.Empty;
        //        sCad = "SELECT COUNT(1) FROM ORDR WHERE DocEntry = " + iFolioSAP.S() + " AND CANCELED = 'Y'";

        //        DataTable dtRes = new DBSAP().oDB_SP.EjecutarDT_DeQuery(sCad);
        //        if(dtRes != null && dtRes.Rows.Count > 0)
        //        {
        //            if (dtRes.Rows[0][0].S() == "0")
        //                return false;
        //            else
        //                return true;
        //        }
        //        else 
        //            return false;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public int GetObtieneFolioFacturaEnSAP(int iFolioSAP)
        //{
        //    try
        //    {
        //        string sCad = string.Empty;
        //        sCad = "SELECT TOP 1 TrgetEntry FROM RDR1 WHERE DocEntry = " + iFolioSAP.S() + " AND ISNULL(TrgetEntry,0) > 0";

        //        DataTable dtRes = new DBSAP().oDB_SP.EjecutarDT_DeQuery(sCad);
        //        if (dtRes.Rows.Count > 0)
        //        {
        //            return dtRes.Rows[0][0].S().I();
        //        }
        //        else
        //            return 0;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public void SetActualizaEstatusFacturaEnIntegrator(int iIdFactura, int iStatus)
        //{
        //    try
        //    {
        //        string sCad = "UPDATE Principales.tbp_DI_Facturas SET Estatus = " + iStatus.S() + " WHERE IdFactura = " + iIdFactura.S();
        //        object oRes = oBD_SP.EjecutarValor_DeQuery(sCad);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public void SetActualizaNumeroFacturaSAPEnMexJet(string sNoDocumento, int iFolioMJ, int iTipoFact)
        //{
        //    try
        //    {
        //        new DBBaseMXJ().oDB_SP.EjecutarSP("[Principales].[spU_MXJ_ActualizaFolioFactura]", "@IdPrefactura", iFolioMJ,
        //                                                                                            "@FolioFactura", sNoDocumento,
        //                                                                                            "@TipoFactura", iTipoFact);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
