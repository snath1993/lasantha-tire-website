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
    public partial class frmPOsalesorderprint : Form
    {
        public static string ConnectionString;
        DataSet ds;
        bool vat;
        public frmPOsalesorderprint()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmPOsalesorderprint(frmpurchesorder frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsSalesOrder;
            vat = frmParent.vat;            

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

        private void frmPOsalesorderprint_QueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e)
        {

        }
        string Myfullpath;
        private void frmPOsalesorderprint_Load(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Maximized;
            try
            {
            ReportDocument crp = new ReportDocument();
                if (vat == true)
                {
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptQuotation.rpt";
                }
                else
                {
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptQuotationNonVat.rpt";
                }
            crp.Load(Myfullpath);
            crp.SetDataSource(ds);
            crvsalesorder.ReportSource = crp; //rptSalesOrder
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

    }
}