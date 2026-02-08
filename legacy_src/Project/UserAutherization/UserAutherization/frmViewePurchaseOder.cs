using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmViewePurchaseOder : Form
    {
        DataSet ds;
        public static string ConnectionString;

        public frmViewePurchaseOder(frmPurchaseOder frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSPurchaseOder;
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

        private void CRVPurchaseOder_Load(object sender, EventArgs e)
        {
            try
            {

                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptPurchaseOder.rpt";
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CRVPurchaseOder.ReportSource = crp;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void frmViewePurchaseOder_Load(object sender, EventArgs e)
        {

        }
    }
}
