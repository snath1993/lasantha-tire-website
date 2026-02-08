using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;


namespace UserAutherization
{
    public partial class frmInvPrint1 : Form
    {
        public static string ConnectionString;
        DataSet ds;
        public frmInvPrint1(frmInvoiceAR frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
        }
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
             
            }
            catch { }
        }


        private void frmInvPrint1_Load(object sender, EventArgs e)
        {

            string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoice.rpt";
            ReportDocument crp = new ReportDocument();
            crp.Load(Myfullpath);
            crp.SetDataSource(ds);
            crvInvoicePrint.ReportSource = crp;

            //CRInvoice cr = new CRInvoice();
            //cr.SetDataSource(ds);
            //crvInvoicePrint.ReportSource = cr;
        }
    }
}