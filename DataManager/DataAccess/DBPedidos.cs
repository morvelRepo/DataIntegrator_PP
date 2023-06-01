using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NucleoBase.Core;
using System.Data.SqlClient;
using DataIntegrator.Clases;

namespace DataIntegrator.DataAccess
{
    public class DBPedidos : DBBase
    {
        public void TransfierePedidosAntiguoCarrito()
        {
            try
            {
                Utils.GuardarBitacora("Arma script para consultar pedidos del carrito anterior.");
                
                DataSet ds = new DBSAP_PRO().oBD_SP.EjecutarDS("[dbo].[spS_MIT_ObtienePedidosCarritoViejo]");

                if (ds != null && ds.Tables.Count > 0)
                {
                    Utils.GuardarBitacora("Se transferiran: " + ds.Tables[0].Rows.Count.S() + " pedidos.");
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string sFolio = row["NSFT_ID"].S();

                        DataRow[] drConceptos = ds.Tables[1].Select("ID = " + sFolio);
                        if (drConceptos != null && drConceptos.Length > 0)
                        {
                            int iIdPedido = InsertaHeaderCarritoAnterior(row);

                            if (iIdPedido.S().I() > 0)
                            {
                                InsertaConceptoCarritoAnterior(iIdPedido, drConceptos);
                                Utils.GuardarBitacora("Se insertó al integrator el Pedido con ID:" + iIdPedido.S());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataSet DBGetObtieneDatosDeQuery(string sQuery, string sCad)
        {
            try
            {
                SqlConnection conn = new SqlConnection();

                try
                {
                    DataSet dsRes = new DataSet();
                    conn.ConnectionString = sCad;
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sQuery, conn);
                    cmd.CommandType = CommandType.Text;
                    
                    cmd.CommandTimeout = 0;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dsRes);
                    conn.Close();

                    return dsRes;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Dispose();
                        conn = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizaDocNum(int iIdPedido, int DocNum)
        {
            try
            {
                string sQuery = "UPDATE [dbo].[NSFT_HPEDIDOS] SET NSFT_DocNum = " + DocNum.S() + ", NSFT_IDSBO = " + DocNum.S() + ", NSFT_DocDate = CONVERT(VARCHAR(10), GETDATE(), 103) WHERE NSFT_ID = " + iIdPedido.S();
                new DBSAP().oDB_SP.EjecutarValor_DeQuery(sQuery);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertaHeaderCarritoAnterior(DataRow Row)
        {
            try
            {
                object oRes = oBD_SP.EjecutarValor("[Principales].[spI_DI_InsertaHeaderPedidoCarritoAnt]", "@Empresa", 1,
                                                                                                            "@Sucursal", "1",
                                                                                                            "@Fecha", Row["NSFT_DocDate"].S(),
                                                                                                            "@CardCode", Row["NSFT_CardCode"].S(),
                                                                                                            "@Referencia", Row["NSFT_NumReferencia"].S(),
                                                                                                            "@Comentarios", Row["NSFT_Observaciones"].S(),
                                                                                                            "@Moneda", "$",
                                                                                                            "@Descuento", 0,
                                                                                                            "@TipoCambio", 1,
                                                                                                            "@Autorizado", Row["NSFT_Autorizado"].S().I(),
                                                                                                            "@FechaExp", Row["NSFT_DocDate"].S(),
                                                                                                            "@NumUsrCte", Row["NSFT_NumUsrCte"].S(),
                                                                                                            "@UsrCte", Row["NSFT_UsrCte"].S(),
                                                                                                            "@NomUsuaCliente", Row["NSFT_NomUsuaCliente"].S(),
                                                                                                            "@NumCtroCosto", Row["NSFT_NumCtroCosto"].S(),
                                                                                                            "@DescripcionCC", Row["NSFT_DescripcionCC"].S(),
                                                                                                            "@Area", Row["NSFT_Area"].S(),
                                                                                                            "@NumUsrMod", Row["NSFT_NumUsrMod"].S().I(),
                                                                                                            "@Autorizador", Row["NSFT_Autorizador"].S().I(),
                                                                                                            "@Autorizadores", Row["NSFT_Autorizadores"].S(),
                                                                                                            "@FechaAutoriza", Row["NSFT_FechaAutoriza"].S().Dt(),
                                                                                                            "@DireccionFactura", Row["NSFT_DireccionFactura"].S(),
                                                                                                            "@ClaveDireccionEnvio", Row["NSFT_ClaveDireccionEnvio"].S(),
                                                                                                            "@DireccionEnvio", Row["NSFT_ClaveDireccionEnvio"].S(),
                                                                                                            "@IPSource", Row["NSFT_IP_Source"].S(),
                                                                                                            "@FolioNR", Row["NSFT_FolioNR"].S(),
                                                                                                            "@Pedido", Row["NSFT_Pedido"].S(),
                                                                                                            "@PedidoAutorizado", Row["NSFT_PedidoAutorizado"].S().I(),
                                                                                                            "@NumPedidoEBO", Row["NSFT_NumPedidoEBO"].S().I(),
                                                                                                            "@IdCarrito", Row["NSFT_ID"].S(),
                                                                                                            "@CenCostoCli", Row["NSFT_CtoCostoCte"].S());
                return oRes.S() != string.Empty ? oRes.S().I() : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertaConceptoCarritoAnterior(int iIdPedido, DataRow[] rows)
        {
            try
            {
                foreach (DataRow row in rows)
                {
                    string sItemName = DBGetObtieneNombreArticulo(row["ItemCode"].S());

                    oBD_SP.EjecutarSP("[Principales].[spI_DI_InsertaConceptosCarritoAnt]", "@ID", iIdPedido,
                                                                                            "@Linea", row["LineNum"].S().I(),
                                                                                            "@Item", row["ItemCode"].S(),
                                                                                            "@DescripcionUsuario", sItemName,
                                                                                            "@CodBarras", string.Empty,
                                                                                            "@Cantidad", row["Quantity"].S().D(),
                                                                                            "@Precio", row["PrecioVentaFinal"].S().D(),
                                                                                            "@Descuento", 0,
                                                                                            "@Impuesto", row["U_IvaV"].S() == "Exenta" ? 0 : 16,
                                                                                            "@CodigoImpuesto", row["U_IVA"].S(),
                                                                                            "@TotalLinea", row["PrecioVentaFinal"].S().D() * row["Quantity"].S().D(),
                                                                                            "@Almacen", string.Empty,
                                                                                            "@Proyecto", string.Empty,
                                                                                            "@Dimension1", string.Empty,
                                                                                            "@Dimension2", string.Empty,
                                                                                            "@Dimension3", string.Empty,
                                                                                            "@Dimension4", string.Empty,
                                                                                            "@Dimension5", string.Empty,
                                                                                            "@UnidadMedida", row["UnidadDeMedida"].S(),
                                                                                            "@ValorConcurso", row["ValorConcurso"].S(),
                                                                                            "@CostoSTD", row["CostoSTD"].S().D(),
                                                                                            "@CostoReposicion", row["CostoReposicion"].S().D(),
                                                                                            "@CostoPromedio", row["CostoPromedio"].S().D(),
                                                                                            "@WhsCode", row["WhsCode"].S(),
                                                                                            "@ClaveArticuloCliente", row["ClaveArticuloCliente"].S(),
                                                                                            "@DescripcionArticuloCliente", row["DescripcionArticuloCliente"].S(),
                                                                                            "@DescripcionArticulo", row["DescripcionArticulo"].S(),
                                                                                            "@PrecioVentaFinal", row["PrecioVentaFinal"].S().D(),
                                                                                            "@BPLid", 0); //row["BPLid"].S().I());

                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Pedido> GetPedidoPendientes
        {
            get
            {
                int iIdSAPTemp = 0;
                try
                {
                    Utils.GuardarBitacora("Inicia consulta de pedidos pendientes");
                    DataSet dsRes = oBD_SP.EjecutarDS_DeQuery("EXEC [Principales].[spS_DI_ConsultaPedidosPendientesProcesar]");
                    Utils.GuardarBitacora("Termino de consultar pedidos");
                    if (dsRes != null && dsRes.Tables[0].Rows.Count > 0)
                    {
                        Utils.GuardarBitacora("Header: " + dsRes.Tables[0].Rows.Count.S());
                        List<Pedido> oLst = dsRes.Tables[0].AsEnumerable().Select(r => new Pedido()
                        {
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
                            sCtoCostoCli = r["CenCostoCli"].S()
                        }).ToList();

                        Utils.GuardarBitacora("Lista header terminada");

                        foreach (Pedido oF in oLst)
                        {
                            iIdSAPTemp = oF.id;
                            DataTable dtDetalle = dsRes.Tables[1].Select("ID = " + oF.id.S()).CopyToDataTable();
                            Utils.GuardarBitacora("Detalle: " + dsRes.Tables[1].Rows.Count.S() + ", ID_PEDIDO: " + oF.id.S() + ", No. Detalles: " + dtDetalle.Rows.Count.S());
                            if (dtDetalle.Rows.Count > 0)
                            {
                                // Obtiene conceptos de la factura
                                oF.conceptos = dtDetalle.AsEnumerable().Select(r => new ConceptosPedido()
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
                                    PrecioVentaFinal = r["PrecioVentaFinal"].S().D(),
                                    BPLid = r["BPLid"].S().I()
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
                        return new List<Pedido>();
                }
                catch (Exception ex)
                {
                    Utils.GuardarBitacora("Desc error (" + iIdSAPTemp.S() + "): " + ex.Message);
                    throw ex;
                }
            }
        }

        public string DBGetObtieneNombreArticulo(string sItemCode)
        {
            try
            {
                string sQuery = "SELECT ItemName FROM OITM (NOLOCK) WHERE ItemCode = '" + sItemCode + "'";
                DataSet ds = DBGetObtieneDatosDeQuery(sQuery, new DBSAP_PRO().oBD_SP.sConexionSQL);
                
                if(ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].S();
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PagoPedidos> GetPagosPendientes
        {
            get
            {
                try
                {
                    List<PagoPedidos> oLstPagos = new List<PagoPedidos>();
                    DataTable dt = oBD_SP.EjecutarDT("[Principales].[spS_DI_ObtienePagosPendientesProcesar]");
                    foreach (DataRow row in dt.Rows)
                    {
                        PagoPedidos oPP = new PagoPedidos();
                        oPP.iIdPedido = row.S("IdSapDoc");
                        oPP.mMonto = row.S("MontoPago");
                        oPP.dtFechaCreacion = row.S("FechaPago").Dt();
                        oPP.sIdTransaccion = row.S("IdTransaccion");
                        oPP.sTipoPago = row.S("TipoPago");
                        oPP.sReferenciaPago = row.S("ReferenciaPago");
                    }

                    return oLstPagos;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public DataTable DBGetObtieneClientesConUM()
        {
            try
            {
                return oBD_SP.EjecutarDT("[Principales].[spS_DI_ConsultaClientesConUM]");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
