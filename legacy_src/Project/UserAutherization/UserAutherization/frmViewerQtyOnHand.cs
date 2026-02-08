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
    public partial class frmViewerQtyOnHand : Form
    {
        DataSet ds;
        public static string ConnectionString;

        public frmViewerQtyOnHand(frmQtyOnHand frmParent)
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

        private void frmValuations_Load(object sender, EventArgs e)
        {
            try
            {
               // string Myfullpath = Path.GetFullPath("CRWHOnHand.rpt");

               // string Myfullpath = Application.StartupPath + "\\CRWHOnHand.rpt";
                string  Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRWHOnHand.rpt";
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);

                crp.SetDataSource(ds);
                crvValuation.ReportSource = crp;
            }
            catch { }
        }
    }
}