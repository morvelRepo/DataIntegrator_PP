using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DataIntegrator.Objetos
{
    public class EstatusRes
    {
        private bool _success = false;
        private string _message = string.Empty;

        public bool success
        {
            set { _success = value; }
            get { return (bool)_success; }
        }
        public string message
        {
            set { _message = value; }
            get { return (string)_message; }
        }
    }

    public class ActualizacionCarrito
    {
        private int _idSAP = 0;

        public int idSAP
        {
            get { return _idSAP; }
            set { _idSAP = value; }
        }

        public string ToJson()
        {
            string json = string.Empty;
            JavaScriptSerializer js = new JavaScriptSerializer();
            json = js.Serialize(this);
            js = null;
            return json;
        }
    }
}
