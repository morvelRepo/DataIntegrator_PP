using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrator.Objetos
{
    public class ConfirmacionDV
    {
        public long idConf { set; get; }
        public string code { set; get; }
        public string visit_arrival { set; get; }
        public string tracked_arrival { set; get; }
        public string recibio { set; get; }
        public string driver_name { set; get; }
        public string route_code { set; get; }
        public string vehicle_code { set; get; }
        public string clave_envio { set; get; }
        public string status { set; get; }
        public string msg { set; get; }
        public string comentarios { set; get; }
    }
}
