using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataIntegrator.Clases;
using DataIntegrator.DataAccess;
using DataIntegrator.Objetos;
using NucleoBase.Core;
using System.Data;

namespace DataIntegrator.Bussines
{
    public class PagosBO
    {
        public bool Import()
        {
            try
            {
                bool ban = false;
                List<PagoPedidos> oLstPagos = new List<PagoPedidos>();
                oLstPagos = new DBPagos().ObtienePagosPendientesConFR();
                foreach (PagoPedidos oP in oLstPagos)
                {
                    if(CreateSapDoc(oP))
                    {

                    }
                }


                return ban;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public bool CreateSapDoc(Pago oP)
        //{
        //    bool ban = false;


        //    SAPbobsCOM.Payments downPayment = (SAPbobsCOM.Payments)MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
        //    downPayment.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;

        //    downPayment.CardCode = oP.sCliente;
        //    downPayment.DocDate = oP.dtFecha;
        //    downPayment.DocType = SAPbobsCOM.BoRcptTypes.rSupplier;
        //    downPayment.DocCurrency = oP.sMoneda;

        //    downPayment.JournalRemarks = "PAGO " + socioNegocio.CardName;
        //    downPayment.Remarks = socioNegocio.CardName;

        //    //La variable *total*, contiene la suma de los importes que se aplicará a las facturas en el pago menos el importe de las notas de credito que se creen

        //    //if (oP.sTipoPago == TipoPagoPago.Efectivo)
        //    //{
        //    //    downPayment.CashSum = (double)total;
        //    //}
        //    //else if (oP.sTipoPago == TipoPagoPago.Cheque)
        //    //{
        //    //    downPayment.Checks.CheckSum = (double)total;
        //    //    downPayment.Checks.AccounttNum = pago.CodigoCuenta;
        //    //    downPayment.Checks.BankCode = pago.CodigoBanco;
        //    //    downPayment.Checks.CheckNumber = (int)pago.NumeroCheque.Value;
        //    //    downPayment.Checks.DueDate = pago.Fecha;
        //    //}
        //    //else if (oP.sTipoPago == TipoPagoPago.Deposito || oP.sTipoPago == TipoPagoPago.Transferencia)
        //    //{
        //    downPayment.TransferAccount = pago.BancoCuenta.GLAccount;
        //    downPayment.TransferSum = (double)total;
        //    //}

        //    var line = 0;
        //    foreach (var pagoFactura in pago.Detalles)
        //    {

        //        var folio = pagoFactura.Folio;
        //        var monto = pagoFactura.Abono;  //El monto a aplicar de cada factura
        //        var importeNC = pagoFactura.ImporteNC; //El monto de la nota de credito que se creará

        //        if (monto > 0)
        //        {
        //            if (line != 0)
        //            {
        //                downPayment.Invoices.Add();
        //            }

        //            //Se anexa el folio de la factura y su monto

        //            downPayment.Invoices.DocEntry = folio;
        //            downPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_PurchaseInvoice;
        //            downPayment.Invoices.SumApplied = (double)monto;

        //            line++;
        //        }
        //        if (importeNC > 0)
        //        {
        //            if (line != 0)
        //            {
        //                downPayment.Invoices.Add();
        //            }

        //            var nota = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes) as Documents;

        //            var fecha = pago.Fecha;

        //            nota.CardCode = socio;
        //            nota.DocDate = fecha;
        //            nota.DocDueDate = fecha;
        //            nota.TaxDate = fecha;
        //            nota.DocCurrency = pago.CodigoMoneda;
        //            nota.Comments = "Nota de Credito de la factura " + folio.Split('-')[1];
        //            nota.DocType = BoDocumentTypes.dDocument_Service;

        //            nota.Lines.AccountCode = cuentaContableNC;
        //            nota.Lines.ItemDescription = "DESCUENTO A PROVEEDORES";
        //            nota.Lines.Quantity = 1;
        //            nota.Lines.TaxCode = "TAX0";
        //            nota.Lines.Price = (double)importeNC;

        //            var statusNC = nota.Add();
        //            if (statusNC != 0)
        //            {
        //                log.ErrorFormat("Error registering Nota credito {0} - {1} - {2}", pagoFactura.Codigo, statusNC, company.GetLastErrorDescription());
        //                break;
        //            }
        //            var codigo = MyGlobals.oCompany.GetNewObjectKey();
        //            nota.GetByKey(int.Parse(codigo));

        //            //Una vez creada la nota, se anexa al pago con el monto de la nota

        //            downPayment.Invoices.DocEntry = int.Parse(codigo);
        //            downPayment.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseCreditNote;
        //            downPayment.Invoices.SumApplied = (double)importeNC;
        //            line++;

        //        }
        //    }

        //    var status = downPayment.Add();
        //    //Aqui es donde company.GetLastErrorDescription() me devuelve el mensaje de error, el codigo que me regresa es -10

        //}


        public bool CreateSapDoc(PagoPedidos oP)
        {
            bool ban = false;
            try
            {

                SAPbobsCOM.Payments oPmt = MyGlobals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
                oPmt.Invoices.DocEntry = oP.iIdSAP_FR;
                oPmt.CardCode = oP.sCardCode;
                oPmt.DocDate = DateTime.Now;
                oPmt.DocTypte = SAPbobsCOM.BoRcptTypes.rCustomer;
                oPmt.CashSum = oP.mMonto.S().Db();

                

                if (oPmt.Add() != 0)
                {
                    Utils.GuardarBitacora(MyGlobals.oCompany.GetLastErrorDescription());
                }
                {
                    Utils.GuardarBitacora("Pago creado correctamente.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ban;
        }
    }
}
