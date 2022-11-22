using DataIntegrator.Objetos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NucleoBase.Core;

namespace DataIntegrator.DataAccess
{
    public class DBImagenesDV : DBBase
    {
        public List<ImagenesDV> ObtieneImagenesPorDescargar()
        {
            List<ImagenesDV> olst = new List<ImagenesDV>();
            try
            {
                DataTable dt = oBD_SP.EjecutarDT("[Principales].[spS_ID_ConsultaDocumentsPorDescargar]");
                foreach (DataRow row in dt.Rows)
                {
                    ImagenesDV oImg = new ImagenesDV();
                    oImg.idConf = row.S("IdConf").I();
                    oImg.Codigo = row.S("Codigo");
                    oImg.name = row.S("Nombre_Document");
                    oImg.Url = row.S("Url_Document");
                    oImg.Anio = row.S("FechaCreacion").Dt().Year.S();
                    oImg.Mes = row.S("FechaCreacion").Dt().Month.S().PadLeft(2, '0');
                    oImg.Dia = row.S("FechaCreacion").Dt().Day.S().PadLeft(2, '0');
                    oImg.Ruta = row.S("Ruta");

                    olst.Add(oImg);
                }

                return olst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DBSetActualizaEstatusDocumentoDescargado(long IdConf, string Codigo)
        {
            try
            {
                object oRes = oBD_SP.EjecutarValor("[Principales].[spU_ID_ActualizaEstatusDocumentosDescargados]", "@IdConf", IdConf,
                                                                                                                    "@Codigo", Codigo);

                return oRes.S().I();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
