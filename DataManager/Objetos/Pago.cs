using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegrator.Objetos
{
    public class Pago
    {
        private string _sEmpresa = string.Empty;
        private int _iId = 0;
        private string _sSucursal = string.Empty;
        private DateTime _dtFecha = DateTime.Now;
        private string _sCliente = string.Empty;
        private string _sTipoPago = string.Empty;
        private decimal _dImporte = 0;
        private int _iIdFactura = 0;
        private string _sSerie = string.Empty;
        private string _sMoneda = string.Empty;
        private string _sReferencia = string.Empty;
        private string _sComentarios = string.Empty;
        private string _sProyecto1 = string.Empty;
        private string _sDimension1 = string.Empty;


        public string sEmpresa { set { _sEmpresa = value; } get { return _sEmpresa; } }
        public int iId { set { _iId = value; } get { return _iId; } }
        public string sSucursal { set { _sSucursal = value; } get { return _sSucursal; } }
        public DateTime dtFecha { set { _dtFecha = value; } get { return _dtFecha; } }
        public string sCliente { set { _sCliente = value; } get { return _sCliente; } }
        public string sTipoPago { set { _sTipoPago = value; } get { return _sTipoPago; } }
        public decimal dImporte { set { _dImporte = value; } get { return _dImporte; } }
        public int iIdFactura { set { _iIdFactura = value; } get { return _iIdFactura; } }
        public string sSerie { set { _sSerie = value; } get { return _sSerie; } }
        public string sMoneda { set { _sMoneda = value; } get { return _sMoneda; } }
        public string sReferencia { set { _sReferencia = value; } get { return _sReferencia; } }
        public string sComentarios { set { _sComentarios = value; } get { return _sComentarios; } }
        public string sProyecto1 { set { _sProyecto1 = value; } get { return _sProyecto1; } }
        public string sDimension1 { set { _sDimension1 = value; } get { return _sDimension1; } }
    }

    public class PagoPedidos
    {
        private string _sCardCode = string.Empty;
        private string _iIdPedido = string.Empty;
        private string _mMonto = string.Empty;
        private string _dFechaCreacion = string.Empty;
        private DateTime _dtFechaCreacion = DateTime.Now;
        private string _sIdTransaccion = string.Empty;
        private string _sTipoPago = string.Empty;
        private string _sReferenciaPago = string.Empty;
        private int _iIdSAP_FR = 0;


        public string sCardCode
        {
            set { _sCardCode = value; }
            get { return _sCardCode; }
        }
        public string iIdPedido
        {
            set { _iIdPedido = value; }
            get { return _iIdPedido; }
        }
        public string mMonto
        {
            set { _mMonto = value; }
            get { return _mMonto; }
        }
        public string dFechaCreacion
        {
            set { _dFechaCreacion = value; }
            get { return _dFechaCreacion; }
        }
        public DateTime dtFechaCreacion
        {
            set { _dtFechaCreacion = value; }
            get { return _dtFechaCreacion; }
        }
        public string sIdTransaccion
        {
            set { _sIdTransaccion = value; }
            get { return _sIdTransaccion; }
        }
        public string sTipoPago
        {
            set { _sTipoPago = value; }
            get { return _sTipoPago; }
        }
        public string sReferenciaPago
        {
            set { _sReferenciaPago = value; }
            get { return _sReferenciaPago; }
        }
        public int iIdSAP_FR
        {
            set { _iIdSAP_FR = value; }
            get { return _iIdSAP_FR; }
        }
        
    }
}
