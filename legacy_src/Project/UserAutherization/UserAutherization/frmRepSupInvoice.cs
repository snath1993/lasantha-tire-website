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
    public partial class frmRepSupInvoice : Form
    {
        DataSet ds;
        DataSet dp;
        DataSet dk;
        DataSet sk;
        public static string ConnectionString;
        public int TypeofPrint = 0;
        public frmRepSupInvoice(frmSupInvoice frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
            dp = frmParent.ds;
            dk = frmParent.ds;
            sk = frmParent.ds;
            TypeofPrint = frmParent.Printype;
            //  ConnectionString = DBUtil.clsUtils.conStringSqlServer();
        }

        public frmRepSupInvoice(frmDirectSupInvoice frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
            dp = frmParent.ds;
            dk = frmParent.ds;
            sk = frmParent.ds;
            TypeofPrint = frmParent.Printype;
            //  ConnectionString = DBUtil.clsUtils.conStringSqlServer();
        }
        public frmRepSupInvoice(frmGRN frmParent)
        {
            InitializeComponent();
            setConnectionString();
            dk = frmParent.ds;
            TypeofPrint = frmParent.Printype;
            //  ConnectionString = DBUtil.clsUtils.conStringSqlServer();
        }
        public frmRepSupInvoice(frmBarcodeGenerator frmParent)
        {
            InitializeComponent();
            setConnectionString();
            sk = frmParent.ds;
            TypeofPrint = frmParent.Printype;
            //  ConnectionString = DBUtil.clsUtils.conStringSqlServer();
        }

        public frmRepSupInvoice()
        {
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

        private void frmRepSupInvoice_Load(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Maximized;
            try
            {
                // string Myfullpath = Path.GetFullPath("rptSupplierInvoice.rpt");

                if (TypeofPrint == 0)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptSupplierInvoice.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvSupInvoice.ReportSource = crp;
                }
                else if (TypeofPrint == 1)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptBarcode.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(dp);
                    crvSupInvoice.ReportSource = crp;
                }
                else if (TypeofPrint == 3)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptBarcode.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(dk);
                    crvSupInvoice.ReportSource = crp;
                }
                else if (TypeofPrint == 4)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptBarcode.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(sk);
                    crvSupInvoice.ReportSource = crp;
                }
                //CRSupplierInvoice cr = new CRSupplierInvoice();
                //cr.SetDataSource(ds);
                //crvSupInvoice.ReportSource = cr;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}