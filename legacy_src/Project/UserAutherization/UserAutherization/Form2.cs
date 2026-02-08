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
    public partial class Form2 : Form
    {
        DataSet ds = new DataSet();
        public Form2(frmScan frmParent)
        {
            InitializeComponent();
            ds = frmParent.dsscanchanel;           

        }
        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\crScanChanel.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvInvoice.ReportSource = crp;
            }
            catch
            {

            }
        }
    }
}
