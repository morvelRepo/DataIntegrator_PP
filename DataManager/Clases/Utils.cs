using DataIntegrator.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NucleoBase.Core;

namespace DataIntegrator.Clases
{
    public static class Utils
    {
        public static void GuardarBitacora(string sMensaje)
        {
            try
            {
                string lsCarpeta = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) +
                                    "\\BitacorasApp\\";
                string lsArchivo = "Bitacora_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";

                //valida si ya existe la carpeta o no
                if (!Directory.Exists(lsCarpeta))
                    Directory.CreateDirectory(lsCarpeta);

                lsArchivo = lsCarpeta + lsArchivo;

                //valida si existe el archivo
                if (!File.Exists(lsArchivo))
                    File.CreateText(lsArchivo).Close();

                StreamWriter loSW = File.AppendText(lsArchivo);

                loSW.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + sMensaje);

                loSW.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static bool DestroyCOMObject(object oSapObject)
        {
            try
            {
                bool ban = false;

                if(oSapObject != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oSapObject);
                    oSapObject = null;
                    GC.Collect();
                }

                ban = true;

                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GetLoadInitialValues()
        {
            try
            {
                new DBUtils().GetLoadInitialValues();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool HttpPatch(string url, string content, string contentType = "application / x-www-form-urlencoded")
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var request = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(url));
            if (request == null)
                throw new ApplicationException(string.Format("Could not create the httprequest from the url:{0}", url));

            request.Method = "PATCH";

            UTF8Encoding encoding = new UTF8Encoding();
            var byteArray = Encoding.ASCII.GetBytes(content);

            request.ContentLength = byteArray.Length;
            request.ContentType = contentType;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode.S() == "OK" ? true : false;
            }
            catch (WebException)
            {
                return false;
            }
        }


    }
}
