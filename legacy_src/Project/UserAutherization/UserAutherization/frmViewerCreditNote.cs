using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;


namespace UserAutherization
{
    public partial class frmViewerCreditNote : Form
    {
        DataSet ds = new DataSet();

        //public frmViewerCreditNote(frmWareHouseTrans frmParent)
        //{
        //    InitializeComponent();
        //    ds = frmParent.DsItemWise;
        //}

        public frmViewerCreditNote(frmReturnNotenew frmParent)
        {
            InitializeComponent();
            ds = frmParent.DsCustomerReturn;
        }

  

        private void frmViewerItemWiseSales_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptCreditNote.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvInvoice.ReportSource = crp;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }

        }

        private void crvInvoice_Load(object sender, EventArgs e)
        {

        }
    }
}