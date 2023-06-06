using DataManager.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.Core;
using DataIntegrator.Clases;
using System.Data;

namespace DataIntegrator.DataAccess
{
    public class DBMetodos : DBBase
    {
        public void ActualizaRegistro(EstatusDocumento oCarrier)
        {
            StringBuilder sQ = new StringBuilder();
            sQ.AppendLine("UPDATE [Principales].[" + oCarrier.sTabla + "] ");
            sQ.AppendLine("SET [FechaImp] = GETDATE(), [Estatus]=" + oCarrier.iEstatus.S() + ", ");

            if (oCarrier.iEstatus.S().I() == 0)
                sQ.AppendLine("[Msg]='" + oCarrier.sMensaje.Replace("'","") + "' ");
            else
                sQ.AppendLine("[Msg]=NULL, [SapDoc]='" + oCarrier.iSapDoc + "' ");
            
            sQ.AppendLine("WHERE [ID]=" + oCarrier.iID + " ");
            sQ.AppendLine("AND [Empresa]=" + oCarrier.sEmpresa + " ");

            Utils.GuardarBitacora("ActualizarRegistro: " + Environment.NewLine + sQ.S()); //#LOG

            try
            {
                object oRes = oBD_SP.EjecutarValor_DeQuery(sQ.S());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetValueByQuery(string sQ)
        {
            SAPbobsCOM.Recordset oRS = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            object sRes = null;

            try
            {
                oRS.DoQuery(sQ);
                if (oRS.RecordCount > 0)
                    sRes = oRS.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oRS != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
                    oRS = null;
                    GC.Collect();
                }
            }

            return sRes;
        }

        public int GetObtieneConsecutivoPorClave(string sClave)
        {
            try
            {
                object oRes = oBD_SP.EjecutarValor("[Principales].[spS_DI_ObtieneConsecutivoPorClave]", "@Clave", sClave);

                return oRes != null ? oRes.S().I() : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable DBGetObtieneInfoArticulo(string sItem, string sCardCode)
        {
            try
            {
                string query = "SELECT a.U_DescArtCli, b.BcdCode " +
                                " FROM OSCN a " +
                                " inner join OBCD b on convert(varchar, a.U_DescArtCli) = convert(varchar, b.UomEntry) and b.ItemCode = '" + sItem + "' " +
                                " WHERE a.ItemCode = '" + sItem + "' " +
                                    " and CardCode = '" + sCardCode + "'";

                DataTable dt = new DBSAP_PRO().oBD_SP.EjecutarDT_DeQuery(query);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    //Protected Function GetValueByQuery(ByVal sQ As String)
    //    Dim oRS As SAPbobsCOM.Recordset = Me.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    //    Dim sRes As Object = Nothing
    //    Try
    //        oRS.DoQuery(sQ)
    //        If oRS.RecordCount > 0 Then
    //            sRes = oRS.Fields.Item(0).Value
    //        End If
    //    Catch ex As Exception
    //        Throw New Exception("GetValueByQuery: " & ex.Message)
    //    Finally
    //        Me.oCommon.DestroyCOMObject(oRS)
    //    End Try
    //    Return sRes
    //End Function
    }
}
