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
using System.Xml;

namespace UserAutherization
{
    public partial class frmPrintTotalDailyCollection : Form
    {
        public static string ConnectionString;
        DataSet ds;        
        public frmPrintTotalDailyCollection()
        {
            InitializeComponent();
        }
        public frmPrintTotalDailyCollection(frmDaillyCollection  frmParent)
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

        private void CryDaillyColView_Load(object sender, EventArgs e)
        {
            ReportDocument crp = new ReportDocument();
            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\Rpt_DaillyCollectionSummary_new.rpt";
            crp.Load(Myfullpath);
            crp.SetDataSource(ds);
            CryDaillyColView.ReportSource = crp;
        }
       
    }
}