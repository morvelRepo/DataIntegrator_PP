using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NucleoBase.Core;
using System.Data;

namespace DataIntegrator.DataAccess
{
    public class DBConfirmacionDv : DBBase
    {
        public List<ConfirmacionDV> ObtieneConfirmacionesPendientes()
        {
            List<ConfirmacionDV>  olst = new List<ConfirmacionDV>();
            try
            {
                DataTable dt = oBD_SP.EjecutarDT("[Principales].[spS_ID_ConsultaEntregasPendientesConfirmar]");
                foreach (DataRow row in dt.Rows)
                {
                    ConfirmacionDV oconf = new ConfirmacionDV();
                    oconf.idConf = row.S("idConf").L();
                    oconf.code = row.S("codigo");
                    oconf.visit_arrival = row.S("visit_arrival");
                    oconf.tracked_arrival = row.S("tracked_arrival");
                    oconf.recibio = row.S("recibio");
                    oconf.driver_name = row.S("driver_name");
                    oconf.route_code = row.S("route_code");
                    oconf.vehicle_code = row.S("vehicle_code");
                    oconf.clave_envio = row.S("clave_Envio");
                    oconf.status = row.S("estatus");

                    olst.Add(oconf);
                }

                return olst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DBGetObtieneEstatusEquivalencia(string Estatus)
        {
            try
            {
                return new DBBase().oBD_SP.EjecutarValor("[Principales].[spS_ID_ObtieneEquivalenciaEstatusDrivIn]", "@Estatus", Estatus).S();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DBSetActualizaPedigoEntregado(long IdConf, string DocNum, int iEstatus, string sMensaje)
        {
            try
            {
                object oRes = oBD_SP.EjecutarValor("[Principales].[spU_ID_ActualizaDetConfirmacionPedido]", "@IdConf", IdConf,
                                                                                                            "@Codigo", DocNum,
                                                                                                            "@ActualizadoSAP", iEstatus,
                                                                                                            "@Msg", sMensaje);

                return oRes.S().I();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ObtieneDocEntryDocumentoEntrega(string DocNum)
        {
            try
            {
                object oRes = new DBSAP_PRO().oBD_SP.EjecutarValor_DeQuery("SELECT TOP 1 DocEntry FROM ODLN (NOLOCK) WHERE DocNum = " + DocNum);

                return oRes.S().I();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
