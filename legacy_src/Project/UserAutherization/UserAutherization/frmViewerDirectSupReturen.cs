using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;


namespace UserAutherization
{
    public partial class frmViewerDirectSupReturen : Form
    {

        DataSet ds;
        public static string ConnectionString;

        public frmViewerDirectSupReturen(frmDirectSupplierReturn frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsCustomerReturn;
        }

        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        private void frmViewerDirectSupReturen_Load(object sender, EventArgs e)
        {
            try
            {
              
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptDirectSupReturn.rpt";
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CRVDirectsupReturn.ReportSource = crp;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void CRVDirectsupReturn_Load(object sender, EventArgs e)
        {

        }
    }
}