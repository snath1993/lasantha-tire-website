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
    public partial class OPDReport : Form
    {

        public static string ConnectionString;
        public int flg = 0;
        DataSet ds;

        public OPDReport()
        {
            InitializeComponent();
            setConnectionString();
        }



        public OPDReport(frmChanellopd frmParent)
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

        private void OPDReport_Load(object sender, EventArgs e)
        {
            rptOpdReceipt cr = new rptOpdReceipt();
            cr.SetDataSource(ds);
            crvopdReport.ReportSource = cr;
        }
    }
}