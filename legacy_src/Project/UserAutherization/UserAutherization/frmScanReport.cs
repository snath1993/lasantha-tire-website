using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace UserAutherization
{
    public partial class frmScanReport : Form
    {
        public static string ConnectionString;
        public int flg = 0;
        DataSet ds;


        //public frmScanReport(frmScan  frmParent)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    ds = frmParent.DsScanReport;
        //}

        public frmScanReport(frmXray  frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsScanReport;
        }

        public frmScanReport(frmLabTest frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsScanReport;
        }


        public void setConnectionString()
        {
            try
            {
                TextReader tr = new StreamReader("Connection.txt");
                ConnectionString = tr.ReadLine();
                tr.Close();
            }
            catch { }
        }

        private void frmScanReport_Load(object sender, EventArgs e)
        {
            rptScanReceipt cr = new rptScanReceipt();
            cr.SetDataSource(ds);
            crvScanReport.ReportSource = cr;
        }
    }
}