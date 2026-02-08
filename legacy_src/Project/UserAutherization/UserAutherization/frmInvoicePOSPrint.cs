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
    public partial class frmInvoicePosPrint : Form
    {
        public static string ConnectionString;
        DataSet ds;
        int InvType = 1;

        //public frmInvoicePosPrint(frmInvoiceARList frmParent)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    ds = frmParent.DSInvoicing;

        //}

        public frmInvoicePosPrint(frmInvoicing frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsItemWise; ;
            InvType = frmParent.InvoiceType;
        }

        //public frmInvoicePosPrint(frmInvoicing frmParent)
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
        private void frmInvoicePosPrint_Load(object sender, EventArgs e)
        {
            try
            {
               
                ReportDocument crp = new ReportDocument();
                if (InvType == 1)
                {
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoice.rpt";
                } if (InvType == 2)
                {
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRTaxInvoice.rpt";
                } if (InvType == 3)
                {
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRSVATInvoice.rpt";
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
    }
}