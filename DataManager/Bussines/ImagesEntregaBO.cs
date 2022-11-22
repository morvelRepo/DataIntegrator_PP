using DataIntegrator.Clases;
using DataIntegrator.DataAccess;
using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NucleoBase.Core;

namespace DataIntegrator.Bussines
{
    public class ImagesEntregaBO
    {
        public void Import()
        {
            try
            {
                List<ImagenesDV> ols = new DBImagenesDV().ObtieneImagenesPorDescargar();
                Utils.GuardarBitacora("Numero de imagenes a descargar" + ols.Count.S());
                foreach (ImagenesDV item in ols)
                {
                    string filePath = Helpers.sDirectorioImagenesConfirmacion + @"\" + item.Anio + @"\" + item.Mes + @"\" + item.Dia + @"\" + item.Ruta + @"\";

                    if(item.Codigo.S().Substring(0,1) == "E")
                        filePath += @"Entregas\";
                    if(item.Codigo.S().Substring(0, 1) == "F")
                        filePath += @"Facturas\";

                    Utils.GuardarBitacora(filePath);

                    string dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    else
                    {
                        Utils.GuardarBitacora("Ya existe la ruta");
                    }

                    item.UbicacionFinal = filePath;
                    item.nombreFinal = item.Codigo.S() + ".pdf";

                    Utils.GuardarBitacora(filePath + item.Codigo.S() + ".pdf");

                    try
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(item.Url, filePath + item.Codigo.S() + ".pdf");

                        new DBImagenesDV().DBSetActualizaEstatusDocumentoDescargado(item.idConf, item.Codigo);
                        ActualizaDocumentoSAP(item);
                    }
                    catch (Exception ex)
                    {
                        Utils.GuardarBitacora("Error al descargar la imagen: " + ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ActualizaDocumentoSAP(ImagenesDV oImagen)
        {
            SAPbobsCOM.Documents oPed;
            bool ban = false;
            try
            {
                SAPbobsCOM.Attachments2 oATT = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2) as SAPbobsCOM.Attachments2;

                oATT.Lines.Add();
                oATT.Lines.FileName = System.IO.Path.GetFileNameWithoutExtension(oImagen.nombreFinal);
                oATT.Lines.FileExtension = Path.GetExtension(oImagen.nombreFinal).Replace(".", "");
                oATT.Lines.SourcePath = System.IO.Path.GetDirectoryName(oImagen.UbicacionFinal + oImagen.nombreFinal);
                oATT.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;

                int res = oATT.Add();

                if (res == 0)
                {
                    Utils.GuardarBitacora("Se agregó correctamente el Att");

                    int iAttEntry = int.Parse(MyGlobals.oCompany.GetNewObjectKey());

                    string sCodigoFinal = oImagen.Codigo.S().Replace("E", "").Replace("F", "");
                    int IdSAP = new DBConfirmacionDv().ObtieneDocEntryDocumentoEntrega(sCodigoFinal);
                    oPed = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);

                    if (oPed.GetByKey(IdSAP))
                    {
                        oPed.AttachmentEntry = iAttEntry;

                        Utils.GuardarBitacora("Intenta relacionar el ATT al pedido");
                        int iRes = oPed.Update();

                        if (iRes == 0)
                        {
                            Utils.GuardarBitacora("Pedido actualizado cone exito");
                            ban = true;
                        }
                        else
                        {
                            string sMensaje = "Error al relacionar el pedido con el ATT.  [" + MyGlobals.oCompany.GetLastErrorCode().S() + "] - " + MyGlobals.oCompany.GetLastErrorDescription();
                            Utils.GuardarBitacora(sMensaje);
                        }
                    }
                }
                else
                {
                    string sMensaje = "Error al guardar crear el ATT en SAP.  [" + MyGlobals.oCompany.GetLastErrorCode().S() + "] - " + MyGlobals.oCompany.GetLastErrorDescription();
                    Utils.GuardarBitacora(sMensaje);
                }

                //    oPed = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);

                //if (oPed.GetByKey(IdSAP))
                //{
                //    oPed.AttachmentEntry = 

                //    oPed.UserFields.Fields.Item("U_QuienRecibio").Value = Confirmacion.recibio;
                //    oPed.UserFields.Fields.Item("U_OperadorEntrego").Value = Confirmacion.driver_name;
                //    oPed.UserFields.Fields.Item("U_Descripcion").Value = string.Empty;
                //    oPed.UserFields.Fields.Item("U_RumEnt").Value = Confirmacion.route_code;
                //    oPed.UserFields.Fields.Item("U_ECORutaEntrega").Value = Confirmacion.vehicle_code;
                //    oPed.UserFields.Fields.Item("U_CveDireccionEnvio").Value = Confirmacion.clave_envio;
                //    oPed.UserFields.Fields.Item("U_Estado").Value = Confirmacion.status;
                //    oPed.UserFields.Fields.Item("U_Comentario").Value = Confirmacion.comentarios;

                //    int iRes = oPed.Update();

                //    if (iRes == 0)
                //    {
                //        ban = true;
                //        Confirmacion.msg = "La entrega se actualizó correctamente";
                //    }
                //    else
                //    {
                //        string sMensaje = "Error al actualizar la entrega: " + Confirmacion.code + " [" + MyGlobals.oCompany.GetLastErrorCode().ToString() + "] - [" + MyGlobals.oCompany.GetLastErrorDescription().ToString() + "]";
                //        Confirmacion.msg = sMensaje;
                //        Utils.GuardarBitacora("[LN ConfirmacionEntrega] " + sMensaje);
                //    }
                //}
                //else
                //{
                //    Confirmacion.msg = "No se encontró la entrega en SAP, DocEntry: " + IdSAP.S();
                //    Utils.GuardarBitacora("[LN ConfirmacionEntrega] No se encontró la entrega en SAP DocEntry: " + IdSAP.S());
                //}

                //if (oPed != null)
                //{
                //    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPed);
                //    oPed = null;
                //    GC.Collect();
                //}

                return ban;
            }
            catch (Exception ex)
            {
                string sMensaje = "Error al guardar el documento en SAP.  [" + MyGlobals.oCompany.GetLastErrorCode().S() + "] - " + MyGlobals.oCompany.GetLastErrorDescription();
                throw ex;
            }
        }


        //public void AgregaAnexo()
        //{
        //    try
        //    {
        //        SAPbobsCOM.Attachments2 oATT = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2) as SAPbobsCOM.Attachments2;

        //        oATT.Lines.Add();
        //        oATT.Lines.FileName = System.IO.Path.GetFileNameWithoutExtension(file_attach);
        //        oATT.Lines.FileExtension = Path.GetExtension(file_attach).Replace(".", "");
        //        oATT.Lines.SourcePath = System.IO.Path.GetDirectoryName(upload_path);
        //        oATT.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;

        //        int res = oATT.Add();

        //        if (res == 0)
        //        {
        //            iAttEntry = int.Parse(oCompany.GetNewObjectKey());
        //            if (AtcEntry == 0)
        //            {
        //                oDocto.AttachmentEntry = iAttEntry;
        //            }
        //            flagAttach = true;
        //        }
        //        else
        //        {
        //            oCompany.GetLastError(out lErrCode, out sErrMsg);
        //            _message_error = "Attachments: " + lErrCode + " " + sErrMsg;
        //            resul = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
