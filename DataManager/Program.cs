using DataIntegrator.Bussines;
using DataIntegrator.Clases;
using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DataIntegrator
{
    class Program
    {
        private static Mutex mutex = null;

        static void Main(string[] args)
        {
            //const string appName = "DataIntegratorDev";
            const string appName = "DataIntegrator";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                return;
            }
            else
            {
                try
                {
                    Console.WriteLine("Prod");
                    Utils.GuardarBitacora("Inicia DataIntegrator");
                    DateTime dtIni = DateTime.Now;
                    LoadInitialValues();

                    Utils.GuardarBitacora("Inicia pedidos");
                    MyGlobals.sStepLog = "Pedidos";
                    PedidosBO oPed = new PedidosBO();
                    oPed.Import();

                    Utils.GuardarBitacora("Inicia Confirmacion Pedidos DrivIn");
                    MyGlobals.sStepLog = "Confirmacion DrivIn";
                    ConfirmacionBO oConf = new ConfirmacionBO();
                    oConf.Import();

                    // SE DEJÓ DE PASAR LAS FACTURAS DE RESERVA

                    //List<Factura> oLstFacturas = new List<Factura>();
                    //Utils.GuardarBitacora("Inicia facturas de reserva");
                    //MyGlobals.sStepLog = "Pago facturas reserva";
                    //FacturasBO oFac = new FacturasBO();
                    //oFac.Import(oLstFacturas);

                    if (MyGlobals.oCompany.Connected)
                        MyGlobals.oCompany.Disconnect();

                    DateTime dtFin = DateTime.Now;
                    TimeSpan ts = dtFin - dtIni;
                    Utils.GuardarBitacora("Total de tiempo: " + ts.ToString());
                }
                catch (Exception ex)
                {
                    if (MyGlobals.oCompany.Connected)
                        MyGlobals.oCompany.Disconnect();

                    Utils.GuardarBitacora("Error en paso " + MyGlobals.sStepLog + ": " + ex.Message);
                }
            }
        }

        private static void LoadInitialValues()
        {
            try
            {
                MyGlobals.sStepLog = "Carga valores iniciales";
                Utils.GetLoadInitialValues();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
