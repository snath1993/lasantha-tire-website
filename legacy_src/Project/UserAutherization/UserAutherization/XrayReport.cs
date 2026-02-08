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
    public partial class XrayReport : Form
    {
        public static string ConnectionString;
        public int flg = 0;
        DataSet ds;

        public XrayReport(frmXray frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsScanReport;
        }


        //public XrayReport(frmChanellopd  frmParent)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    ds = frmParent.DsScanReport;
        //}

        public XrayReport(frmLabTest frmParent)
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


        public XrayReport()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void XrayReport_Load(object sender, EventArgs e)
        {
            rptXray cr = new rptXray();
            cr.SetDataSource(ds);
            crvXrayReport.ReportSource = cr;
        }
    }
}