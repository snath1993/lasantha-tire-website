using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmViewerCustomerReturnReprint : Form
    {
        DataSet ds;
        public static string ConnectionString;
        public frmViewerCustomerReturnReprint(frmInvoiceARRtnList  frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
        }
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
            }
            catch { }
        }

        private void frmViewerCustomerReturn_Load(object sender, EventArgs e)
        {
            //try
            //{
             
            //    CRCustomerReturn2  crp = new CRCustomerReturn2  ();
            //    crp.SetDataSource(ds);
            //    crvCustomerReturn.ReportSource = crp;
               
            //}
            //catch { }
        }
    }
}