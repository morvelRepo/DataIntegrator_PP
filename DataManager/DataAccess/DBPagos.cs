using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NucleoBase.Core;
using System.Data;
using DataIntegrator.Objetos;

namespace DataIntegrator.DataAccess
{
    public class DBPagos : DBBase
    {
        public List<PagoPedidos> ObtienePagosPendientesConFR()
        {
            try
            {
                return oBD_SP.EjecutarDT("[Principales].[spS_DI_ObtienePagosPendientesDeProcesar]")
                    .AsEnumerable().Select(r => new PagoPedidos()
                    {
                        iIdPedido = r["IdPedido"].S(),
                        mMonto = r["MontoPago"].S(),
                        dtFechaCreacion = r["FechaPago"].S().Dt(),
                        sIdTransaccion = r["IdTransaccion"].S(),
                        sTipoPago = r["TipoPago"].S(),
                        sReferenciaPago = r["ReferenciaPago"].S(),
                        iIdSAP_FR = r["IdSAP_FR"].S().I(),
                        sCardCode = r["CardCode"].S()
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
