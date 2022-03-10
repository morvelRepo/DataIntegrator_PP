using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.BaseDeDatos;
using NucleoBase.Core;

namespace DataIntegrator.DataAccess
{
    public class DBBase
    {
        public BD_SP oBD_SP = new BD_SP();
        public DBBase()
        {
            oBD_SP.sConexionSQL = Globales.GetConfigConnection("SqlDefault");
        }
    }
}
