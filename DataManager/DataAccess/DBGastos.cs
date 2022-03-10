using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.Core;
using DataIntegrator.Objetos;
using System.Data;

namespace DataIntegrator.DataAccess
{
    public class DBGastos : DBBase
    {
        public List<Gasto> GetGastosPendientes
        {
            get
            {
                try
                {
                    DataTable dtRes = oBD_SP.EjecutarDT("[Principales].[spS_DI_ConsultaGastosPendientes]");
                    if (dtRes != null && dtRes.Rows.Count > 0)
                    {
                        List<Gasto> oLst = dtRes.AsEnumerable().Select(r => new Gasto()
                        {
                            sEmpresa = r["Empresa"].S(),
                            iId = r["ID"].S().I(),
                            sSucursal = r["Sucursal"].S(),
                            dtFecha = r["Fecha"].S().Dt(),
                            sEmpleado = r["Empleado"].S(),
                            sNoReporte = r["Referencia"].S(),
                            sFormaPago = r["FormaPago"].S(),
                            sComentarios = r["Comentarios"].S(),
                            sSerie = r["Serie"].S(),
                            sMoneda = r["Moneda"].S(),
                            dDescuento = r["Descuento"].S().D(),
                            dTipoCambio = r["TipoCambio"].S().D(),
                            iTimbrar = r["Timbrar"].S().I(),
                            sUID = r["UUID"].S()
                        }).ToList();

                        foreach (Gasto oF in oLst)
                        {
                            // Obtiene conceptos de la factura
                            oF.oLstConceptos = oBD_SP.EjecutarDT("[Principales].[spS_DI_ConsultaGastoDetalle]", "@ID", oF.iId).AsEnumerable().
                                    Select(r => new ConceptosGasto()
                                    {
                                        sEmpresa = r["Empresa"].S(),
                                        iId = r["ID"].S().I(),
                                        iLinea = r["Linea"].S().I(),
                                        sItem = r["Item"].S(),
                                        sDescripcionUsuario = r["DescripcionUsuario"].S(),
                                        iCantidad = Convert.ToInt32(r["Cantidad"].S().D()),
                                        dPrecio = r["Precio"].S().D(),
                                        dImpuesto = r["Impuesto"].S().D(),
                                        sCodigoImpuesto = r["CodigoImpuesto"].S(),
                                        dDescuento = r["Descuento"].S().D(),
                                        sCuenta = r["Cuenta"].S(),
                                        sAlmacen = r["Almacen"].S(),
                                        sProyecto = r["Proyecto"].S(),
                                        sDimension1 = r["Dimension1"].S(),
                                        sDimension2 = r["Dimension2"].S(),
                                        sDimension3 = r["Dimension3"].S(),
                                        sDimension4 = r["Dimension4"].S(),
                                        sDimension5 = r["Dimension5"].S(),
                                        sDescripcionAmpliada = r["DescripcionAmpliada"].S()
                                    }).ToList();

                            oF.oEstatus.iID = oF.iId;
                            oF.oEstatus.sEmpresa = oF.sEmpresa;
                        }

                        return oLst;
                    }
                    else
                        return new List<Gasto>();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public DataSet DBGetObtieneGastosConcur
        {
            get
            {
                try
                {
                    return oBD_SP.EjecutarDS("[Principales].[spS_DI_ConsultaGastosConcur]");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public string DBGetObtieneClaveArticulo(string sName)
        {
            try
            {
                string sCad = string.Empty;
                sCad = "SELECT ISNULL(ItemCode,'GT99') FROM OITM (NOLOCK) WHERE ItemName = '" + sName + "'";

                object oRes = new DBSAP().oDB_SP.EjecutarValor_DeQuery(sCad);

                return oRes != null ? oRes.S() : string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string DBGetObtieneFlotaMatricula(string sMatricula)
        {
            try
            {
                DataTable dtRes = new DBBaseMXJ().oDB_SP.EjecutarDT("[Catalogos].[spS_MXJ_ConsultaAeronave]", "@Serie", string.Empty,
                                                                                                    "@Matricula", sMatricula,
                                                                                                    "@estatus", 1);
                if (dtRes != null && dtRes.Rows.Count > 0)
                {
                    return dtRes.Rows[0]["DescripcionFlota"].S();
                }
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int DBSetInsertaHeaderGasto(Gasto oGasto)
        {
            try
            {
                object oRes = oBD_SP.EjecutarValor("[Principales].[spI_DI_InsertaHeaderGasto]", "@Empresa", oGasto.sEmpresa.S().I(),
                                                                                                "@ID", oGasto.iId,
                                                                                                "@Sucursal", oGasto.sSucursal,
                                                                                                "@Fecha", oGasto.dtFecha,
                                                                                                "@Empleado", oGasto.sEmpleado,
                                                                                                "@Referencia", oGasto.sNoReporte,
                                                                                                "@FormaPago", oGasto.sFormaPago,
                                                                                                "@Comentarios", oGasto.sComentarios,
                                                                                                "@Serie", oGasto.sSerie,
                                                                                                "@Moneda", oGasto.sMoneda,
                                                                                                "@Descuento", oGasto.dDescuento,
                                                                                                "@TipoCambio", oGasto.dTipoCambio,
                                                                                                "@Msg", oGasto.sMsg);

                return oRes != null ? oRes.S().I() : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int DBSetInsertaDetalleGasto(ConceptosGasto oC)
        {
            try
            {
                object oRes = oBD_SP.EjecutarValor("[Principales].[spI_DI_InsertaConceptosGasto]", "@Empresa", oC.sEmpresa.S().I(),
                                                                                                    "@ID", oC.iId,
                                                                                                    "@Linea", oC.iLinea,
                                                                                                    "@Item", oC.sItem,
                                                                                                    "@DescripcionUsuario", oC.sDescripcionUsuario,
                                                                                                    "@Precio", oC.dPrecio,
                                                                                                    "@Descuento", oC.dDescuento,
                                                                                                    "@Impuesto", oC.dImpuesto,
                                                                                                    "@CodigoImpuesto", oC.sCodigoImpuesto,
                                                                                                    "@TotalLinea", oC.dTotalLinea,
                                                                                                    "@Cuenta", oC.sCuenta,
                                                                                                    "@Almacen", oC.sAlmacen,
                                                                                                    "@Proyecto", oC.sProyecto,
                                                                                                    "@Dimension1", oC.sDimension1,
                                                                                                    "@Dimension2", oC.sDimension2,
                                                                                                    "@Dimension3", oC.sDimension3,
                                                                                                    "@Dimension4", oC.sDimension4,
                                                                                                    "@Dimension5", oC.sDimension5,
                                                                                                    "@DescripcionAmpliada", oC.sDescripcionAmpliada,
                                                                                                    "@IdGastoOrigen", oC.lIdGasto);

                return oRes != null ? oRes.S().I() : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void DBSetMarcaProcesadoGasto(int iIdGasto, string sMsg)
        {
            try
            {
                oBD_SP.EjecutarSP("[Principales].[spU_DI_MarcaProcesadoGasto]", "@IdGasto", iIdGasto,
                                                                                "@Msg", sMsg);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DBGetObtieneDimensionPorCodigo(string sCodigo)
        {
            try
            {
                string sCad = string.Empty;
                sCad = "SELECT DimCode FROM dbo.OOCR (NOLOCK) WHERE OcrCode = '" + sCodigo + "'";

                return new DBSAP().oDB_SP.EjecutarValor_DeQuery(sCad).S();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DBSetRegresaGastosConcurPasoAnterior()
        {
            try
            {
                oBD_SP.EjecutarSP("[Concur].[spU_DI_RegresaGastosConcurPasoAnterior]");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
