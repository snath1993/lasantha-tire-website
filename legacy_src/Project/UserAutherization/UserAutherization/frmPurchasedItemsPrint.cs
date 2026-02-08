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
    public partial class frmPurchasedItemsPrint : Form
    {
        private frmViewPurchasedItems frmViewPurchasedItems;

        public frmPurchasedItemsPrint()
        {
            InitializeComponent();
        }
        DataSet ds;
        public frmPurchasedItemsPrint(frmViewPurchasedItems frmParent)
        {
            InitializeComponent();
            ds = frmParent.DSPurchasedItems;
           
        }

        private void frmPurchasedItemsPrint_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            try
            {
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptPurchasedItems.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvValuation.ReportSource = crp;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
