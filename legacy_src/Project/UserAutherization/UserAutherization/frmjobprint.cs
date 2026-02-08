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
    public partial class frmjobprint : Form
    {
        public static string ConnectionString;
        DataSet ds;
        string Myfullpath;
        public frmjobprint(frmjobNote frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsJob;
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

        private void crvsalesorder_Load(object sender, EventArgs e)
        {

        }

        private void frmjobprint_Load(object sender, EventArgs e)
        {
            try
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\RptJobEnter.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvsalesorder.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

    }
}
