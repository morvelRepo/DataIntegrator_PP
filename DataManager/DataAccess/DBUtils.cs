using DataIntegrator.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NucleoBase.Core;

namespace DataIntegrator.DataAccess
{
    public class DBUtils : DBBase
    {
        public void GetLoadInitialValues()
        {
            try
            {
                Utils.GuardarBitacora("Consulta accesos a SAP");
                DataTable dt = oBD_SP.EjecutarDT("[Principales].[spS_DI_ConsultaAccesosSBO]");

                if (dt != null && dt.Rows.Count > 0)
                {
                    Utils.GuardarBitacora("Registros: " + dt.Rows.Count.S());

                    DataRow row = dt.Rows[0];
                    MyGlobals.oCompany = new SAPbobsCOM.Company();


                    MyGlobals.oCompany.Server = row["Servidor"].S();    // "SBO";     
                    MyGlobals.oCompany.CompanyDB = row["DBCompania"].S();  // "PRINCIPADO_PRO";
                    MyGlobals.oCompany.UserName = row["SBOUserName"].S(); // "manager";
                    MyGlobals.oCompany.Password = row["SBOPassword"].S(); // "najejoharo";
                    MyGlobals.oCompany.LicenseServer = row["Licencia"].S();    // "SBO";
                    MyGlobals.oCompany.DbUserName = row["DBUsuario"].S();   // "sa";
                    MyGlobals.oCompany.DbPassword = row["DBPassword"].S();  // "najejoharo";

                    //MyGlobals.oCompany.Server = "SBO";      //row["Servidor"].S();    // "SBO";     
                    //MyGlobals.oCompany.CompanyDB = "PRUEBAS2020"; // "PRINCIPADO_PRO";     //row["DBCompania"].S();  // "PRINCIPADO_PRO";
                    //MyGlobals.oCompany.UserName = "sistemas";      //row["SBOUserName"].S(); // "manager";
                    //MyGlobals.oCompany.Password = "temporal1";      //row["SBOPassword"].S(); // "najejoharo";
                    //MyGlobals.oCompany.LicenseServer = "SBO"; //row["Licencia"].S();    // "SBO";
                    //MyGlobals.oCompany.DbUserName = "sa";    //row["DBUsuario"].S();   // "sa";
                    //MyGlobals.oCompany.DbPassword = "najejoharo";    //row["DBPassword"].S();  // "najejoharo";


                    switch (row["TipoServidor"].S())
                    {
                        case "1":
                            MyGlobals.oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL;
                                break;
                        case "4":
                            MyGlobals.oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005;
                                break;
                        case "6":
                            MyGlobals.oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;
                                break;
                        case "7":
                            MyGlobals.oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
                                break;
                        case "8":
                            MyGlobals.oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;
                                break;
                        case "9":
                            MyGlobals.oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017;
                            break;
                    }

                    Utils.GuardarBitacora("Intenta conectar a SAP" );
                    int iError = MyGlobals.oCompany.Connect();
                    if (iError != 0)
                    {
                        string sError = string.Empty;
                        MyGlobals.oCompany.GetLastError(out iError, out sError);
                        MyGlobals.sStepLog = "Conectar: " + iError.S() + " Mensaje: " + sError;
                        throw new Exception(MyGlobals.sStepLog);
                    }
                    else
                        Utils.GuardarBitacora("conecto bien a SAP");
                }
            }
            catch (Exception ex)
            {
                Utils.GuardarBitacora("Error en DBUtils. " + ex.Message);
                throw ex;
            }
        }
    }
}
