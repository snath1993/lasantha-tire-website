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
    public partial class frmNewInvoicePrint : Form
    {
        public static string ConnectionString;
        DataSet ds; 
        public frmNewInvoicePrint()
        {
            InitializeComponent();
        }

        public frmNewInvoicePrint(frmNewInvoice  frmParent)
        {
             InitializeComponent();
             setConnectionString();
             ds = frmParent.objGRN;

        }

        public void setConnectionString()
        {
            try
            {
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        private void frmNewInvoicePrint_Load(object sender, EventArgs e)
        {
            CRNewInvoice cr = new CRNewInvoice();
            cr.SetDataSource(ds);
            CrvNewInvoice.ReportSource = cr;

        }
    }
}