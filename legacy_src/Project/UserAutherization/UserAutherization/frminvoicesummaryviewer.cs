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
    public partial class frminvoicesummaryviewer : Form
    {
        public frminvoicesummaryviewer(frmSalesSummary  frmParent) 
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
        }
        public static string ConnectionString;
        DataSet ds;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        private void crvMaxViewer_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CrSalesSUmmary.rpt";                
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvMaxViewer.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
