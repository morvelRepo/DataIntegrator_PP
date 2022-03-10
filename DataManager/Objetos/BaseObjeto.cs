using DataManager.Objetos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DataIntegrator.Objetos
{
     [Serializable, Bindable(BindableSupport.Yes)]
    public class BaseObjeto
    {
        private EstatusDocumento _oEstatus = new EstatusDocumento();
        private bool bDisposed = false;

        ~BaseObjeto()
        {
            Dispose(false);
        }

        [Browsable(false)]
        public EstatusDocumento oEstatus { get { return _oEstatus; } set { _oEstatus = value; } }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            try
            {
                if (!bDisposed)
                {
                    if (bDisposing)
                    {

                    }
                    bDisposed = true;
                }
            }
            catch
            {
            }
        }
    }
}
