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
    public partial class frmPrintDailyReports : Form
    {
        public static string ConnectionString;
        DataSet ds;
        public frmPrintDailyReports(frmDailySales frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSDAILY;
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
        string Myfullpath;  
        private void crvDailySales_Load(object sender, EventArgs e)
        {
            ReportDocument crp = new ReportDocument();
            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\DailySalesCollection.rpt";
            crp.Load(Myfullpath);
            crp.SetDataSource(ds);
            crvDailySales.ReportSource = crp;
        }
    }
}