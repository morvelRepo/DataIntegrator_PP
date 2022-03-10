using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegrator.Objetos
{
    [Serializable]
    public class Pedido : BaseObjeto
    {
        public Pedido()
        {
            oEstatus.sTabla = "tbp_DI_Pedidos";
        }

        private int _id = 0;
        private int _empresa = 0;
        private int _sucursal = 0;
        private DateTime _fecha = new DateTime();
        private string _cardcode = string.Empty;
        private string _referencia = string.Empty;
        private string _comentarios = string.Empty;
        private string _moneda = string.Empty;
        private decimal _descuento = 0;
        private decimal _tipocambio = 0;
        private int _autorizado = 0;
        private int _NumUsrCte = 0;
        private string _UsrCte = string.Empty;
        private string _NomUsuaCte = string.Empty;
        private string _NumCtroCosto = string.Empty;
        private string _DescripcionCC = string.Empty;
        private string _Area = string.Empty;
        private int _NumUsrMod = 0;
        private int _Autorizador = 0;
        private string _Autorizadores = string.Empty;
        private string _FechaAutoriza = string.Empty;
        private string _DireccionFactura = string.Empty;
        private string _ClaveDireccionEnvio = string.Empty;
        private string _DireccionEnvio = string.Empty;
        private string _IPSource = string.Empty;
        private string _FolioNR = string.Empty;
        private string _Pedido = string.Empty;
        private int _PedidoAutorizado = 0;
        private int _NumPeridoEBO = 0;
        private string _IdCarrito = string.Empty;
        private string _sCtoCostoCli = string.Empty;
        List<ConceptosPedido> _conceptos = new List<ConceptosPedido>();


        

        


        public int id { get { return _id; } set { _id = value; } }
        public int empresa { get { return _empresa; } set { _empresa = value; } }
        public int sucursal { get { return _sucursal; } set { _sucursal = value; } }
        public DateTime fecha { get { return _fecha; } set { _fecha = value; } }
        public string cardcode { get { return _cardcode; } set { _cardcode = value; } }
        public string referencia { get { return _referencia; } set { _referencia = value; } }
        public string comentarios { get { return _comentarios; } set { _comentarios = value; } }
        public string moneda { get { return _moneda; } set { _moneda = value; } }
        public decimal descuento { get { return _descuento; } set { _descuento = value; } }
        public decimal tipocambio { get { return _tipocambio; } set { _tipocambio = value; } }
        public int autorizado { get { return _autorizado; } set { _autorizado = value; } }
        public int NumUsrCte { get { return _NumUsrCte; } set { _NumUsrCte = value; } }
        public string UsrCte { get { return _UsrCte; } set { _UsrCte = value; } }
        public string NomUsuaCte { get { return _NomUsuaCte; } set { _NomUsuaCte = value; } }
        public string NumCtroCosto { get { return _NumCtroCosto; } set { _NumCtroCosto = value; } }
        public string DescripcionCC { get { return _DescripcionCC; } set { _DescripcionCC = value; } }
        public string Area { get { return _Area; } set { _Area = value; } }
        public int NumUsrMod { get { return _NumUsrMod; } set { _NumUsrMod = value; } }
        public int Autorizador { get { return _Autorizador; } set { _Autorizador = value; } }
        public string Autorizadores { get { return _Autorizadores; } set { _Autorizadores = value; } }
        public string FechaAutoriza { get { return _FechaAutoriza; } set { _FechaAutoriza = value; } }
        public string DireccionFactura { get { return _DireccionFactura; } set { _DireccionFactura = value; } }
        public string ClaveDireccionEnvio { get { return _ClaveDireccionEnvio; } set { _ClaveDireccionEnvio = value; } }
        public string DireccionEnvio { get { return _DireccionEnvio; } set { _DireccionEnvio = value; } }
        public string IPSource { get { return _IPSource; } set { _IPSource = value; } }
        public string FolioNR { get { return _FolioNR; } set { _FolioNR = value; } }
        public string sPedido { get { return _Pedido; } set { _Pedido = value; } }
        public int PedidoAutorizado { get { return _PedidoAutorizado; } set { _PedidoAutorizado = value; } }
        public int NumPeridoEBO { get { return _NumPeridoEBO; } set { _NumPeridoEBO = value; } }
        public List<ConceptosPedido> conceptos { get { return _conceptos; } set { _conceptos = value; } }
        public string IdCarrito { get { return _IdCarrito; } set { _IdCarrito = value; } }
        public string sCtoCostoCli { get { return _sCtoCostoCli; } set { _sCtoCostoCli = value; } }
    }

    [Serializable]
    public class ConceptosPedido
    {

        private int _linea = 0;
        private string _item = string.Empty;
        private string _descripcionusuario = string.Empty;
        private string _codbarras = string.Empty;
        private int _cantidad = 0;
        private decimal _precio = 0;
        private decimal _descuento = 0;
        private int _impuesto = 0;
        private string _codimpuesto = string.Empty;
        private decimal _totallinea = 0;
        private string _almacen = string.Empty;
        private string _proyecto = string.Empty;
        private string _dimension1 = string.Empty;
        private string _dimension2 = string.Empty;
        private string _dimension3 = string.Empty;
        private string _dimension4 = string.Empty;
        private string _dimension5 = string.Empty;
        private string _UnidadMedida = string.Empty;
        private string _ValorConcurso = string.Empty;
        private decimal _CostoSTD = 0;
        private decimal _CostoReposicion = 0;
        private decimal _CostoPromedio = 0;
        private string _WhsCode = string.Empty;
        private string _ClaveArticuloCliente = string.Empty;
        private string _DescripcionArticuloCliente = string.Empty;
        private string _DescripcionArticulo = string.Empty;
        private decimal _PrecioVentaFinal = 0;
        private int _BPLid = 0;


        public int linea { get { return _linea; } set { _linea = value; } }
        public string item { get { return _item; } set { _item = value; } }
        public string descripcionusuario { get { return _descripcionusuario; } set { _descripcionusuario = value; } }
        public string codbarras { get { return _codbarras; } set { _codbarras = value; } }
        public int cantidad { get { return _cantidad; } set { _cantidad = value; } }
        public decimal precio { get { return _precio; } set { _precio = value; } }
        public decimal descuento { get { return _descuento; } set { _descuento = value; } }
        public int impuesto { get { return _impuesto; } set { _impuesto = value; } }
        public string codimpuesto { get { return _codimpuesto; } set { _codimpuesto = value; } }
        public decimal totallinea { get { return _totallinea; } set { _totallinea = value; } }
        public string almacen { get { return _almacen; } set { _almacen = value; } }
        public string proyecto { get { return _proyecto; } set { _proyecto = value; } }
        public string dimension1 { get { return _dimension1; } set { _dimension1 = value; } }
        public string dimension2 { get { return _dimension2; } set { _dimension2 = value; } }
        public string dimension3 { get { return _dimension3; } set { _dimension3 = value; } }
        public string dimension4 { get { return _dimension4; } set { _dimension4 = value; } }
        public string dimension5 { get { return _dimension5; } set { _dimension5 = value; } }
        public string UnidadMedida { get { return _UnidadMedida; } set { _UnidadMedida = value; } }
        public string ValorConcurso { get { return _ValorConcurso; } set { _ValorConcurso = value; } }
        public decimal CostoSTD { get { return _CostoSTD; } set { _CostoSTD = value; } }
        public decimal CostoReposicion { get { return _CostoReposicion; } set { _CostoReposicion = value; } }
        public decimal CostoPromedio { get { return _CostoPromedio; } set { _CostoPromedio = value; } }
        public string WhsCode { get { return _WhsCode; } set { _WhsCode = value; } }
        public string ClaveArticuloCliente { get { return _ClaveArticuloCliente; } set { _ClaveArticuloCliente = value; } }
        public string DescripcionArticuloCliente { get { return _DescripcionArticuloCliente; } set { _DescripcionArticuloCliente = value; } }
        public string DescripcionArticulo { get { return _DescripcionArticulo; } set { _DescripcionArticulo = value; } }
        public decimal PrecioVentaFinal { get { return _PrecioVentaFinal; } set { _PrecioVentaFinal = value; } }
        public int BPLid { get { return _BPLid; } set { _BPLid = value; } }
    }


}
