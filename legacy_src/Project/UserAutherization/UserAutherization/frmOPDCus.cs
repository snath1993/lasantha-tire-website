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
    public partial class frmOPDCus : Form
    {
        public static string ConnectionString;
        public int flg = 0;
        DataSet ds;


        public frmOPDCus()
        {
            InitializeComponent();
            setConnectionString();
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

       public frmOPDCus(frmChanellopd frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsScanReport;
        }

        private void frmOPDCus_Load(object sender, EventArgs e)
        {
            CRopdCustomer cr = new CRopdCustomer();
            cr.SetDataSource(ds);
            crvopdcus.ReportSource = cr;
        }
    }
}