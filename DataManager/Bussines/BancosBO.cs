using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Xml.Linq;
using DataIntegrator.Clases;
using System.Xml;
using DataIntegrator.DataAccess;
using DataIntegrator.Objetos;
using Newtonsoft.Json;
using NucleoBase.Core;

namespace DataIntegrator.Bussines
{
    public class BancosBO
    {
        public void ObtieneTipoCambioDia()
        {
            try
            {
                Utils.GuardarBitacora("Comienza: Actualizar Tipo de Cambio"); //#LOG
                if (!new DBBancos().ExisteTipoCambioDia())
                {
                    wsBanxico.DgieWS oB = new wsBanxico.DgieWS();
                    string sXML = oB.tiposDeCambioBanxico().ToString();

                    XmlDocument oXml = new XmlDocument();
                    oXml.LoadXml(sXML);

                    XmlNodeList ser = oXml.GetElementsByTagName("bm:DataSet");
                    XmlAttribute lista = ser[0].ChildNodes[1].FirstChild.Attributes["OBS_VALUE"];

                    SAPbobsCOM.SBObob oSBObob;
                    oSBObob = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);


                    double dbTipoCambio = lista.Value.S().Replace('.', ',').S().Db();
                    if (dbTipoCambio > 100)
                    {
                        dbTipoCambio = Convert.ToDouble(lista.Value);
                    }

                    if (!String.IsNullOrEmpty(lista.Value))
                        oSBObob.SetCurrencyRate("USD", DateTime.Now.AddDays(1), dbTipoCambio, true);

                    Utils.GuardarBitacora("Tipo de Cambio Actualizado en SAP"); //#LOG


                    if (Helpers.sActualizaTCtoFirebase == "1")
                    {
                        // Actualiza Tipo de Cambio para FireBase
                        string sPathWApp = Helpers.sUrlTipoCambioFB;

                        var request = new TipoCambio()
                        {
                            conversionRate = dbTipoCambio
                        };

                        string sCad = JsonConvert.SerializeObject(request).ToString();
                        string sPathWSApp = sPathWApp + ".json";

                        // Enviamos el JSON al proveedor
                        Utils.HttpPatch(sPathWSApp, sCad);

                        Utils.GuardarBitacora("Tipo de Cambio Actualizado en FireBase"); //#LOG
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




    }

}
