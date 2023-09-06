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
                    // ------------------------------------------------------------------------------------------------------------------------
                    try
                    {
                        Utils.GuardarBitacora("Inicia Confirmacion Pedidos DrivIn");
                        MyGlobals.sStepLog = "Confirmacion DrivIn";
                        ConfirmacionBO oConf = new ConfirmacionBO();
                        oConf.Import();
                    }
                    catch (Exception ex)
                    {
                        Utils.GuardarBitacora("Error en confirmaciones --> Error en paso " + MyGlobals.sStepLog + ": " + ex.Message);
                    }
                    // ------------------------------------------------------------------------------------------------------------------------
                    //try
                    //{
                    //    Utils.GuardarBitacora("Inicia procesamiento de Pedidos Ariba");
                    //    MyGlobals.sStepLog = "Pedidos ARIBA";
                    //    AribaBO oPedAriba = new AribaBO();
                    //    oPedAriba.Import();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Utils.GuardarBitacora("Error en pedidos de ARIBA --> Error en paso " + MyGlobals.sStepLog + ": " + ex.Message);
                    //}

                    //// ------------------------------------------------------------------------------------------------------------------------
                    //try
                    //{
                    //    Utils.GuardarBitacora("Inicia descarga de imagenes de la confirmacion");
                    //    MyGlobals.sStepLog = "";
                    //    ImagesEntregaBO oImg = new ImagesEntregaBO();
                    //    oImg.Import();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Utils.GuardarBitacora("Error en descarga de Imagenes --> Error en paso " + MyGlobals.sStepLog + ": " + ex.Message);
                    //}
                    //// ------------------------------------------------------------------------------------------------------------------------

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
