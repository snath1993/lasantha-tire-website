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
    public partial class frmPurchaseAndSoldPrint : Form
    {
      

        public frmPurchaseAndSoldPrint()
        {
            InitializeComponent();
          
        }

      

        public static string ConnectionString;
      
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        DataSet ds1 = new DataSet();
        DataSet ds2 = new DataSet();
        private int rT;
        public frmPurchaseAndSoldPrint(ItemPurchaseAndSold frmParent, int RT)
        {
            InitializeComponent();
            setConnectionString();
            rT = RT;
            ds1 = frmParent.DsItemWise;
            ds2 = frmParent.ds;
        }
        //public frmPurchaseAndSoldPrint(ItemPurchaseAndSold frmParent, int RT)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    rT = RT;
        //    ds1 = frmParent.DsItemWise;
        //    ds2 = frmParent.ds;
        //}
        DataSet ds3 = new DataSet();
        private ItemPurchaseAndSold itemPurchaseAndSold;

        private void frmPurchaseAndSoldPrint_Load(object sender, EventArgs e)
        {
           
            ds3.Clear();
            string Myfullpath;
            if (rT == 2)
            {
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptSupplierInvoiceSearch.rpt";
                ds3 = ds2;
            }
            else
            {
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceNonVatSearch.rpt";
                ds3 = ds1;
            }

            ReportDocument crp = new ReportDocument();
            crp.Load(Myfullpath);
            crp.SetDataSource(ds3);
            crvMaxViewer.ReportSource = crp;
        }
    }
}
