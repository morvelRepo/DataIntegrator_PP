using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NucleoBase.Core;
using System.Data;
using DataIntegrator.Objetos;
using DataIntegrator.Clases;

namespace DataIntegrator.DataAccess
{
    public class DBPedidosAriba : DBBase
    {
        public List<PedidoAriba> GetPedidoPendientes
        {
            get {
                    int iIdSAPTemp = 0;
                    try
                    {
                        Utils.GuardarBitacora("Inicia consulta de pedidos de ARIBA pendientes");
                        DataSet dsRes = oBD_SP.EjecutarDS("[Ariba].[spS_AB_ConsultaPedidosPorProcesar]");
                        if (dsRes != null && dsRes.Tables[0].Rows.Count > 0)
                        {
                            Utils.GuardarBitacora("Header: " + dsRes.Tables[0].Rows.Count.S());
                            List<PedidoAriba> oLst = dsRes.Tables[0].AsEnumerable().Select(r => new PedidoAriba()
                            {
                                IdPedido = r["IdPedido"].S().I(),
                                CredencialNetwork = r["CredencialNetwork"].S(),
                                CredencialSystem = r["CredencialSystem"].S(),
                                CredencialEndpoint = r["CredencialEndpoint"].S(),
                                ContactName = r["ContactName"].S(),
                                ContactStreet = r["ContactStreet"].S(),
                                ContactCity = r["ContactCity"].S(),
                                ContactIsoCountry = r["ContactIsoCountry"].S(),
                                ContactEmailName = r["ContactEmailName"].S(),
                                ContactEmail = r["ContactEmail"].S(),
                                SenderIdentity = r["SenderIdentity"].S(),
                                SenderSharedSecret = r["SenderSharedSecret"].S(),
                                SenderDomain = r["SenderDomain"].S(),
                                SenderUserAgent = r["SenderUserAgent"].S(),
                                OrderRHTotal = r["OrderRHTotal"].S(),
                                OrderRHComments = r["OrderRHComments"].S(),
                                OrderRHCurrency = r["OrderRHCurrency"].S(),
                                OrderRHShipName = r["OrderRHShipName"].S(),
                                OrderRHShipDeliverContac = r["OrderRHShipDeliverContac"].S(),
                                OrderRHShipDeliverName = r["OrderRHShipDeliverName"].S(),
                                OrderRHShipDeliverStreet = r["OrderRHShipDeliverStreet"].S(),
                                OrderRHShipDeliverCity = r["OrderRHShipDeliverCity"].S(),
                                OrderRHShipDeliverMunicipality = r["OrderRHShipDeliverMunicipality"].S(),
                                OrderRHShipDeliverState = r["OrderRHShipDeliverState"].S(),
                                OrderRHShipDeliverPostalCode = r["OrderRHShipDeliverPostalCode"].S(),
                                OrderRHShipDeliverCountryCode = r["OrderRHShipDeliverIsoCountryCode"].S(),
                                OrderRHShipDeliverAddressID = r["OrderRHShipDeliverAddressID"].S(),
                                OrderRHBillName = r["OrderRHBillName"].S(),
                                OrderRHBillStreet = r["OrderRHBillStreet"].S(),
                                OrderRHBillCity = r["OrderRHBillCity"].S(),
                                OrderRHBillMunicipality = r["OrderRHBillMunicipality"].S(),

                                OrderRHBillState = r["OrderRHBillState"].S(),
                                OrderRHBillPostalCode = r["OrderRHBillPostalCode"].S(),
                                OrderRHBillIsoCountryCode = r["OrderRHBillIsoCountryCode"].S(),
                                OrderRHBillPhoneIsoCountry = r["OrderRHBillPhoneIsoCountry"].S(),
                                OrderRHBillPhoneAreaOrCity = r["OrderRHBillPhoneAreaOrCity"].S(),
                                OrderRHBillPhoneNumber = r["OrderRHBillPhoneNumber"].S(),
                                OrderRHBillFaxAddressID = r["OrderRHBillFaxAddressID"].S(),
                                OrderRHBillFaxIsoCountry = r["OrderRHBillFaxIsoCountry"].S(),
                                OrderRHBillContactPhoneAddressId = r["OrderRHBillContactPhoneAddressId"].S(),

                                OrderRHBillContactPhoneRole = r["OrderRHBillContactPhoneRole"].S(),
                                OrderRHBillExtrinOrderDate = r["OrderRHBillExtrinOrderDate"].S(),
                                OrderRHBillExtrinOrderID = r["OrderRHBillExtrinOrderID"].S(),
                                OrderRHBillExtrinOrderType = r["OrderRHBillExtrinOrderType"].S(),
                                OrderRHBillExtrintype = r["OrderRHBillExtrinType"].S(),
                                OrderPayload = r["OrderPayload"].S(),
                                OrderTimeStamp = r["OrderTimeStamp"].S(),
                                OrderVersion = r["OrderVersion"].S()
                            }).ToList();

                            Utils.GuardarBitacora("Lista header terminada");

                            foreach (PedidoAriba oF in oLst)
                            {
                                iIdSAPTemp = oF.IdPedido;
                                DataTable dtDetalle = dsRes.Tables[1].Select("IdPedido = " + oF.IdPedido.S()).CopyToDataTable();
                                Utils.GuardarBitacora("Detalle: " + dsRes.Tables[1].Rows.Count.S() + ", ID_PEDIDO: " + oF.IdPedido.S() + ", No. Detalles: " + dtDetalle.Rows.Count.S());
                                if (dtDetalle.Rows.Count > 0)
                                {
                                    // Obtiene conceptos de la factura
                                    oF.olsConceptos = dtDetalle.AsEnumerable().Select(r => new ConceptosPedidoAriba()
                                    {
                                        IdPedido = r["IdPedido"].S().I(),
                                        SupplierPartID = r["SupplierPartID"].S(),
                                        UnitPrice = r["UnitPrice"].S().D(),
                                        CurrencyPrice = r["CurrencyPrice"].S(),
                                        Descripcion = r["Descripcion"].S(),
                                        UnitOfMeasure = r["UnitOfMeasure"].S(),
                                        Quantity = r["Quantity"].S().I(),
                                        ConversorFactor = r["ConversorFactor"].S().I(),
                                        Domain = r["Domain"].S(),
                                        Texto = r["Texto"].S(),
                                        ExtriPurcharseGroup = r["ExtriPurcharseGroup"].S(),
                                        ExtriItemCategory = r["ExtriItemCategory"].S(),
                                        ExtriItemRequester = r["ExtriItemRequester"].S(),
                                        ExtriItemStorageLoc = r["ExtriItemStorageLoc"].S(),
                                        ExtriItemPurcharseOrg = r["ExtriItemPurcharseOrg"].S(),
                                        ExtriItemPrintPrice = r["ExtriItemPrintPrice"].S(),
                                        ExtriItemDeliverTo = r["ExtriItemDeliverTo"].S(),
                                        ScheItemType = r["ScheItemType"].S(),
                                        ScheQuantity = r["ScheQuantity"].S().D(),
                                        ScheRequestDelivery = r["ScheRequestDelivery"].S().D(),
                                        ScheLineNumber = r["ScheLineNumber"].S()
                                    }).ToList();
                                }
                                else
                                    Utils.GuardarBitacora("Pedido: " + oF.IdPedido.S() + ", no tiene lineas de detalle");
                            }

                            return oLst;
                        }
                        else
                            return new List<PedidoAriba>();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
        
        }

        public void DBSetActualizaEstatusPedido(PedidoAriba oP, int iEstatus)
        {
            try
            {
                oBD_SP.EjecutarSP("[Ariba].[spU_AB_ActualizaEstatusPedido]", "@IdPedido", oP.IdPedido,
                                                                            "@IdEstatus", iEstatus,
                                                                            "@SapDoc", oP.oEstatus.iSapDoc,
                                                                            "@Msg", oP.oEstatus.sMensaje);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
