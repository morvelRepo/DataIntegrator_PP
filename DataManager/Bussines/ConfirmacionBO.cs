using DataIntegrator.Clases;
using DataIntegrator.DataAccess;
using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NucleoBase.Core;

namespace DataIntegrator.Bussines
{
    class ConfirmacionBO
    {
        public bool Import()
        {
            try
            {
                bool ban = false;
                List<ConfirmacionDV> oLstConfirmaciones = new List<ConfirmacionDV>();
                MyGlobals.sStepLog = "Consulta Pedidos Pendientes";
                Utils.GuardarBitacora(MyGlobals.sStepLog);
                oLstConfirmaciones = new DBConfirmacionDv().ObtieneConfirmacionesPendientes();
                Utils.GuardarBitacora("Obtuvo las siguientes confirmaciones: " + oLstConfirmaciones.Count.S());

                foreach (ConfirmacionDV oF in oLstConfirmaciones)
                {
                    try
                    {
                        if (ActualizaDocumentoSAP(oF))
                        {
                            try
                            {
                                new DBConfirmacionDv().DBSetActualizaPedigoEntregado(oF.idConf, oF.code, 2, oF.msg);
                            }
                            catch (Exception ex)
                            {
                                Utils.GuardarBitacora("Error paso 2  desc:" + ex.Message);
                            }
                        }
                        else
                            new DBConfirmacionDv().DBSetActualizaPedigoEntregado(oF.idConf, oF.code, 1, oF.msg);
                    }
                    catch (Exception ex)
                    {
                        Utils.GuardarBitacora("CodigoEntrega: " + oF.code + ", U_RumEnt: " + oF.route_code + ", ClaveEnvio: "+ oF.clave_envio);
                        throw ex;
                    }
                }

                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ActualizaDocumentoSAP(ConfirmacionDV Confirmacion)
        {
            SAPbobsCOM.Documents oPed;
            bool ban = false;
            try
            {
                int IdSAP = new DBConfirmacionDv().ObtieneDocEntryDocumentoEntrega(Confirmacion.code);
    
                oPed = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);

                if (oPed.GetByKey(IdSAP))
                {
                    if (Confirmacion.tracked_arrival == string.Empty)
                    {
                        if (Confirmacion.visit_arrival == string.Empty)
                        {
                            oPed.UserFields.Fields.Item("U_FechaEntregaME").Value = DateTime.Now.ToString("dd/MM/yyyy");
                            oPed.UserFields.Fields.Item("U_HoraEntregaME").Value = DateTime.Now.Hour.S().PadLeft(2, '0') + ":" + DateTime.Now.Minute.S().PadLeft(2, '0');
                        }
                        else
                        {
                            oPed.UserFields.Fields.Item("U_FechaEntregaME").Value = Confirmacion.visit_arrival.Length > 19 ? Confirmacion.visit_arrival.Substring(0, 19).S().Dt().ToString("dd/MM/yyyy") : string.Empty;
                            oPed.UserFields.Fields.Item("U_HoraEntregaME").Value = Confirmacion.visit_arrival.Length > 19 ? Confirmacion.visit_arrival.Substring(11, 5) : string.Empty;
                        }
                    }
                    else
                    {
                        oPed.UserFields.Fields.Item("U_FechaEntregaME").Value = Confirmacion.tracked_arrival.Length > 19 ? Confirmacion.tracked_arrival.Substring(0, 19).S().Dt().ToString("dd/MM/yyyy") : string.Empty;
                        oPed.UserFields.Fields.Item("U_HoraEntregaME").Value = Confirmacion.tracked_arrival.Length > 19 ? Confirmacion.tracked_arrival.Substring(11, 5) : string.Empty;
                    }

                    oPed.UserFields.Fields.Item("U_QuienRecibio").Value = Confirmacion.recibio;
                    oPed.UserFields.Fields.Item("U_OperadorEntrego").Value = Confirmacion.driver_name;
                    oPed.UserFields.Fields.Item("U_Descripcion").Value = string.Empty;
                    oPed.UserFields.Fields.Item("U_RumEnt").Value = Confirmacion.route_code;
                    oPed.UserFields.Fields.Item("U_ECORutaEntrega").Value = Confirmacion.vehicle_code;
                    oPed.UserFields.Fields.Item("U_CveDireccionEnvio").Value = Confirmacion.clave_envio;
                    oPed.UserFields.Fields.Item("U_Estado").Value = Confirmacion.status;
                    oPed.UserFields.Fields.Item("U_Comentario").Value = Confirmacion.comentarios;

                    int iRes = oPed.Update();

                    if (iRes == 0)
                    {
                        ban = true;
                        Confirmacion.msg = "La entrega se actualizó correctamente";
                    }
                    else
                    {
                        string sMensaje = "Error al actualizar la entrega: " + Confirmacion.code + " [" + MyGlobals.oCompany.GetLastErrorCode().ToString() + "] - [" + MyGlobals.oCompany.GetLastErrorDescription().ToString() + "]";
                        Confirmacion.msg = sMensaje;
                        Utils.GuardarBitacora("[LN ConfirmacionEntrega] " + sMensaje);
                    }
                }
                else
                {
                    Confirmacion.msg = "No se encontró la entrega en SAP, DocEntry: "+ IdSAP.S();
                    Utils.GuardarBitacora("[LN ConfirmacionEntrega] No se encontró la entrega en SAP DocEntry: " + IdSAP.S());
                }

                if (oPed != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPed);
                    oPed = null;
                    GC.Collect();
                }

                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
