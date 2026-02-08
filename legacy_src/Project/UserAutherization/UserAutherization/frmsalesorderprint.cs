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
    public partial class frmsalesorderprint : Form
    {
        public static string ConnectionString;
        DataSet ds;

        public frmsalesorderprint()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmsalesorderprint(frmOrder frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsSalesOrder;            

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

        private void frmsalesorderprint_QueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e)
        {

        }
        string Myfullpath;
        private void frmsalesorderprint_Load(object sender, EventArgs e)
        {
            try
            {
            ReportDocument crp = new ReportDocument();
            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptNewSalesOrder.rpt";
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