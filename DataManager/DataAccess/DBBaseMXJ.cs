using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.BaseDeDatos;
using NucleoBase.Core;

namespace DataIntegrator.DataAccess
{
    public class DBBaseMXJ
    {
        public BD_SP oDB_SP = new BD_SP();
        private bool bDisposed = false;

        public DBBaseMXJ()
        {
            oDB_SP.sConexionSQL = Globales.GetConfigConnection("SqlDefaulMXJ");
        }
    }
}
