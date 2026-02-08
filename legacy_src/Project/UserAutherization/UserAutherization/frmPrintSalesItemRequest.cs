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
    public partial class frmPrintSalesItemRequest : Form
    {
        public static string ConnectionString;
        DataSet ds;
        public frmPrintSalesItemRequest()
        {
            InitializeComponent();
            setConnectionString();
        }
        public frmPrintSalesItemRequest(frmOrder frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsSalesItemRequest ;            

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

        private void frmPrintSalesItemRequest_Load(object sender, EventArgs e)
        {
            try
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptItemsRequest.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvsalesrequest.ReportSource = crp; //rptSalesOrder
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
        
    }
}