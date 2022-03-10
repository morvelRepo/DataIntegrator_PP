using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NucleoBase.Core;

namespace DataIntegrator.DataAccess
{
    public class DBBancos : DBSAP
    {
        public bool ExisteTipoCambioDia()
        {
            try
            {
                string sCad = "SELECT COUNT(1) FROM ORTT WHERE CONVERT(DATE,RateDate) = CONVERT(DATE,(DATEADD(DD,1,GETDATE()))) AND Currency = 'USD'";
                object oRes = oDB_SP.EjecutarValor_DeQuery(sCad);

                if (oRes != null)
                {
                    return oRes.S() == "1" ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
