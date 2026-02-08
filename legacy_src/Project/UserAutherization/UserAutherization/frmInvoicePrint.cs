using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmInvoicePrint : Form
    {
        public static string ConnectionString;
        DataSet ds;
        int InvType = 1;
        int PosTypeNUmber = 0;
        public int InvTAXType=1;

        //public frmInvoicePrint(frmInvoiceARList frmParent)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    ds = frmParent.DSInvoicing;

        //}

        public frmInvoicePrint(frmInvoicing frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsItemWise; ;
            InvType = frmParent.InvoiceType;
            PosTypeNUmber = frmParent.PosType;
            InvTAXType = frmParent.TaxINCType;
        }

        public frmInvoicePrint()
        {
        }

        //public frmInvoicePrint(frmInvoicing frmParent)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    ds = frmParent.DsItemWise; ;
        //    InvType = frmParent.InvoiceType;
        //}

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        string Myfullpath;
        private void frmInvoicePrint_Load(object sender, EventArgs e)
        {
            try
            {
               
                ReportDocument crp = new ReportDocument();
                if (PosTypeNUmber == 1)
                {
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRPOSInvoice.rpt";
                }
                else
                {
                    if (InvType == 1)
                    {
                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoice.rpt";
                    } if (InvType == 2)
                    {
                       // Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRTaxInvoice.rpt";
                        if (InvTAXType == 1)
                        {
                            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\VATINCCRInvoice.rpt";
                        }
                        else if (InvTAXType == 2)
                        {
                            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\VATEXCCRInvoice.rpt";
                        }

                    } if (InvType == 3)
                    {
                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRSVATInvoice.rpt";
                    }
                }
                
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvInvoicePrint.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void crvInvoicePrint_Load(object sender, EventArgs e)
        {

        }

        
    }
}