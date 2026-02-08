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
    public partial class frmViewerInvoiceWiseSales : Form
    {
        DataSet ds;
        public static string ConnectionString;
        public frmViewerInvoiceWiseSales(frmInvoiceWiseSales frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
        }
        private void setConnectionString()
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

        private void frmViewerInvoiceWiseSales_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath = Path.GetFullPath("CRInvoceWiseSales.rpt");
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);

                crp.SetDataSource(ds);
                crvInvoiceWiseSales.ReportSource = crp;
            }
            catch { }
        }
    }
}