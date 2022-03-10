using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NucleoBase.Core;

namespace DataIntegrator.Clases
{
    public static class Helpers
    {
        public const string sEmpresa = "1";
        public static string sSerie = ConfigurationManager.AppSettings["Serie"].S();
        public const string sSucursal = "1";
        public static string sOrigenDocumento = ConfigurationManager.AppSettings["OrigenDocumento"].S();
        public static string sUrlTipoCambioFB = ConfigurationManager.AppSettings["UrlTipoCambioFirebase"].S();
        public static string sActualizaTCtoFirebase = ConfigurationManager.AppSettings["ActualizaTCtoFirebase"].S();
    }
}
