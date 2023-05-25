using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrator.Objetos
{
    [Serializable]
    public class PedidoAriba : BaseObjeto
    {
        public PedidoAriba()
        {
            oEstatus.sTabla = "tbp_AB_PedidosRecibidos";
        }

        public int IdPedido { set; get; }
        public int IdEmpresa { set; get; }
        public string CredencialNetwork { set; get; }
        public string CredencialSystem { set; get; }
        public string CredencialEndpoint { set; get; }
        public string ContactName { set; get; }
        public string ContactStreet { set; get; }
        public string ContactCity { set; get; }
        public string ContactIsoCountry { set; get; }
        public string ContactEmailName { set; get; }
        public string ContactEmail { set; get; }

        public string SenderIdentity { set; get; }
        public string SenderSharedSecret { set; get; }
        public string SenderDomain { set; get; }
        public string SenderUserAgent { set; get; }
        public string OrderRHTotal { set; get; }
        public string OrderRHCurrency { set; get; }
        public string OrderRHComments { set; get; }  //--> FALTA INSERTAR EN EL MAPEO
        public string OrderRHShipName { set; get; }

        public string OrderRHShipDeliverContac { set; get; }
        public string OrderRHShipDeliverName { set; get; }
        public string OrderRHShipDeliverStreet { set; get; }
        public string OrderRHShipDeliverCity { set; get; }
        public string OrderRHShipDeliverMunicipality { set; get; }

        public string OrderRHShipDeliverState { set; get; }
        public string OrderRHShipDeliverPostalCode { set; get; }
        public string OrderRHShipDeliverCountryCode { set; get; }
        public string OrderRHShipDeliverAddressID { set; get; }

        public string OrderRHBillName { set; get; }
        public string OrderRHBillStreet { set; get; }
        public string OrderRHBillCity { set; get; }
        public string OrderRHBillMunicipality { set; get; }
        public string OrderRHBillState { set; get; }

        public string OrderRHBillPostalCode { set; get; }
        public string OrderRHBillIsoCountryCode { set; get; }
        public string OrderRHBillPhoneIsoCountry { set; get; }
        public string OrderRHBillPhoneAreaOrCity { set; get; }
        public string OrderRHBillPhoneNumber { set; get; }

        public string OrderRHBillFaxAddressID { set; get; }
        public string OrderRHBillFaxIsoCountry { set; get; }
        public string OrderRHBillContactPhoneAddressId { set; get; }
        public string OrderRHBillContactPhoneRole { set; get; }

        public string OrderRHBillExtrinOrderDate { set; get; }
        public string OrderRHBillExtrinOrderID { set; get; }
        public string OrderRHBillExtrinOrderType { set; get; }
        public string OrderRHBillExtrintype { set; get; }
        public string RequestDeliveryDate { set; get; }

        public string OrderPayload { set; get; }
        public string OrderTimeStamp { set; get; }
        public string OrderVersion { set; get; }

        public List<ConceptosPedidoAriba> olsConceptos { set; get; }
    }

    public class ConceptosPedidoAriba
    {
        public int IdPedido { set; get; }
        public string SupplierPartID { set; get; }
        public decimal UnitPrice { set; get; }
        public string CurrencyPrice { set; get; }
        public string Descripcion { set; get; }
        public string UnitOfMeasure { set; get; }

        public int Quantity { set; get; }
        public decimal ConversorFactor { set; get; }
        public string Domain { set; get; }
        public string Texto { set; get; }
        public string ExtriPurcharseGroup { set; get; }
        public string ExtriItemCategory { set; get; }
        public string ExtriItemRequester { set; get; }
        public string ExtriItemStorageLoc { set; get; }

        public string ExtriItemPurcharseOrg { set; get; }
        public string ExtriItemPrintPrice { set; get; }
        public string ExtriItemDeliverTo { set; get; }
        public string ScheItemType { set; get; }
        public decimal ScheQuantity { set; get; }
        public decimal ScheRequestDelivery { set; get; }
        public string ScheLineNumber { set; get; }
    }
}
