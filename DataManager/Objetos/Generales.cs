using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataManager.Objetos
{
    //public class Params
    //{
    //    private string _sEmpresa = string.Empty;
    //    private string _sDbServer = string.Empty;
    //    private string _sDbPaso = string.Empty;
    //    private string _sDbUser = string.Empty;
    //    private string _sDbPass = string.Empty;


    //    public string sEmpresa { get { return _sEmpresa; } set { _sEmpresa = value; } }
    //    public string sDbServer { get { return _sDbServer; } set { _sDbServer = value; } }
    //    public string sDbPaso { get { return _sDbPaso; } set { _sDbPaso = value; } }
    //    public string sDbUser { get { return _sDbUser; } set { _sDbUser = value; } }
    //    public string sDbPass { get { return _sDbPass; } set { _sDbPass = value; } }

    //}

    public class EstatusDocumento
    {
        private int _iID = 0;
        private string _sEmpresa = string.Empty;
        private string _sTabla = string.Empty;
        private int _iEstatus = 0;
        private string _sMensaje = string.Empty;
        private int _iSapDoc = 0;

        public int iID { get { return _iID; } set { _iID = value; } }
        public string sEmpresa { get { return _sEmpresa; } set { _sEmpresa = value; } }
        public string sTabla { get { return _sTabla; } set { _sTabla = value; } }
        public int iEstatus { get { return _iEstatus; } set { _iEstatus = value; } }
        public string sMensaje { get { return _sMensaje; } set { _sMensaje = value; } }
        public int iSapDoc { get { return _iSapDoc; } set { _iSapDoc = value; } }
    }
}
