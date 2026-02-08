using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace UserAutherization
{
    public partial class frmViewerStockTaking : Form
    {
        DataSet ds;
        public static string ConnectionString;
        public frmViewerStockTaking(frmStockTakingVarience frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
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

        private void frmViewerStockTaking_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath = Path.GetFullPath("CRStockTaking.rpt");
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);

                crp.SetDataSource(ds);
                crvStockTaking.ReportSource = crp;
            }
            catch { }
        }
    }
}