using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegrator.Objetos
{
    public class Gasto : BaseObjeto
    {
        public Gasto()
        {
            oEstatus.sTabla = "tbp_DI_Gastos";
        }

        private string _sEmpresa = string.Empty;
        private int _iId = 0;
        private string _sSucursal = string.Empty;
        private DateTime _dtFecha = DateTime.Now;
        private string _sEmpleado = string.Empty;
        private string _sNoReporte = string.Empty;
        private string _sFormaPago = string.Empty;
        private string _sComentarios = string.Empty;
        private string _sSerie = string.Empty;
        private string _sMoneda = string.Empty;
        private decimal _dDescuento = 0;
        private decimal _dTipoCambio = 0;
        private int _iTimbrar = 0;
        private string _sUID = string.Empty;
        private string _sMsg = string.Empty;
        private List<ConceptosGasto> _oLstConceptos = new List<ConceptosGasto>();

        public string sEmpresa { get { return _sEmpresa; } set { _sEmpresa = value; } }
        public int iId { get { return _iId; } set { _iId = value; } }
        public string sSucursal { get { return _sSucursal; } set { _sSucursal = value; } }
        public DateTime dtFecha { get { return _dtFecha; } set { _dtFecha = value; } }
        public string sEmpleado { get { return _sEmpleado; } set { _sEmpleado = value; } }
        public string sNoReporte { get { return _sNoReporte; } set { _sNoReporte = value; } }
        public string sFormaPago { get { return _sFormaPago; } set { _sFormaPago = value; } }
        public string sComentarios { get { return _sComentarios; } set { _sComentarios = value; } }
        public string sSerie { get { return _sSerie; } set { _sSerie = value; } }
        public string sMoneda { get { return _sMoneda; } set { _sMoneda = value; } }
        public decimal dDescuento { get { return _dDescuento; } set { _dDescuento = value; } }
        public decimal dTipoCambio { get { return _dTipoCambio; } set { _dTipoCambio = value; } }
        public int iTimbrar { get { return _iTimbrar; } set { _iTimbrar = value; } }
        public string sUID { get { return _sUID; } set { _sUID = value; } }
        public string sMsg { get { return _sMsg; } set { _sMsg = value; } }
        public List<ConceptosGasto> oLstConceptos { get { return _oLstConceptos; } set { _oLstConceptos = value; } }

    }

    public class ConceptosGasto
    {
        private long _lIdGasto = 0;
        private string _sEmpresa = string.Empty;
        private int _iId = 0;
        private int _iLinea = 0;
        private string _sItem = string.Empty;
        private string _sDescripcionUsuario = string.Empty;
        private int _iCantidad = 0;
        private decimal _dPrecio = 0;
        private decimal _dImpuesto = 0;
        private string _sCodigoImpuesto = string.Empty;
        private decimal _dDescuento = 0;
        private decimal _dTotalLinea = 0;
        private string _sCuenta = string.Empty;
        private string _sAlmacen = string.Empty;
        private string _sProyecto = string.Empty;
        private string _sDimension1 = string.Empty;
        private string _sDimension2 = string.Empty;
        private string _sDimension3 = string.Empty;
        private string _sDimension4 = string.Empty;
        private string _sDimension5 = string.Empty;
        private string _sDescripcionAmpliada = string.Empty;


        public long lIdGasto { get { return _lIdGasto; } set { _lIdGasto = value; } }
        public string sEmpresa { get { return _sEmpresa; } set { _sEmpresa = value; } }
        public int iId { get { return _iId; } set { _iId = value; } }
        public int iLinea { get { return _iLinea; } set { _iLinea = value; } }
        public string sItem { get { return _sItem; } set { _sItem = value; } }
        public string sDescripcionUsuario { get { return _sDescripcionUsuario; } set { _sDescripcionUsuario = value; } }
        public int iCantidad { get { return _iCantidad; } set { _iCantidad = value; } }
        public decimal dPrecio { get { return _dPrecio; } set { _dPrecio = value; } }
        public decimal dImpuesto { get { return _dImpuesto; } set { _dImpuesto = value; } }
        public string sCodigoImpuesto { get { return _sCodigoImpuesto; } set { _sCodigoImpuesto = value; } }
        public decimal dDescuento { get { return _dDescuento; } set { _dDescuento = value; } }
        public decimal dTotalLinea { get { return _dTotalLinea; } set { _dTotalLinea = value; } }
        public string sCuenta { get { return _sCuenta; } set { _sCuenta = value; } }
        public string sAlmacen { get { return _sAlmacen; } set { _sAlmacen = value; } }
        public string sProyecto { get { return _sProyecto; } set { _sProyecto = value; } }
        public string sDimension1 { get { return _sDimension1; } set { _sDimension1 = value; } }
        public string sDimension2 { get { return _sDimension2; } set { _sDimension2 = value; } }
        public string sDimension3 { get { return _sDimension3; } set { _sDimension3 = value; } }
        public string sDimension4 { get { return _sDimension4; } set { _sDimension4 = value; } }
        public string sDimension5 { get { return _sDimension5; } set { _sDimension5 = value; } }
        public string sDescripcionAmpliada { get { return _sDescripcionAmpliada; } set { _sDescripcionAmpliada = value; } }
        
    }
}
