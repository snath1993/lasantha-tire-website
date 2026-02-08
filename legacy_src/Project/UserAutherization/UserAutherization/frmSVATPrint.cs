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
    public partial class frmSVATPrint : Form
    {
        public static string ConnectionString;
        DataSet ds;
              
        //public frmSVATPrint(frmInvoicing1 frmParent)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    ds = frmParent.DsItemWise;
        //}

        public frmSVATPrint(frmInvoices frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsItemWise;
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

        private void frmSVATPrint_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRSVATInvoice.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CRSVATPrint.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
    }
}