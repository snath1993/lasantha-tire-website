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
    public partial class frmViwerAdjustmetnsListPrint : Form
    {
        public frmViwerAdjustmetnsListPrint(frmadustmentsListPrint frmParent)
        {
            InitializeComponent();
            ds = frmParent.dsObjAdjustments;
        }

        public frmViwerAdjustmetnsListPrint(frmInventotyAdjustment frmParent)
        {
            InitializeComponent();
            ds = frmParent.dsObjAdjustments;
        }

        DataSet ds = new DataSet();
        private frmInventotyAdjustment frmInventotyAdjustment;

        private void frmViwerAdjustmetnsListPrint_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRAdjutmentsList.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CRVAdjustmentList.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
    }
}