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
    public partial class frmViewerCustomerReturn : Form
    {
        int invType = 1;
        int ExInType = 5;
        DataSet ds;
        public static string ConnectionString;

        public frmViewerCustomerReturn(frmInvoiceARRtn frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
        }

        public frmViewerCustomerReturn(frmCustomerReturns frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
            invType = frmParent.INVTYPE;
            ExInType = frmParent.INVEXINC;
            
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
            try
            {
                if (invType == 1)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRCustomerReturn.rpt";
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvCustomerReturn.ReportSource = crp;
                }
                if (invType == 2)
                {
                    if (ExInType == 5)
                    {
                        string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRCustomerReturn.rpt";
                        ReportDocument crp = new ReportDocument();
                        crp.Load(Myfullpath);
                        crp.SetDataSource(ds);
                        crvCustomerReturn.ReportSource = crp;
                    }
                    else
                    {
                        string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRCustomerReturn.rpt";
                        ReportDocument crp = new ReportDocument();
                        crp.Load(Myfullpath);
                        crp.SetDataSource(ds);
                        crvCustomerReturn.ReportSource = crp;
                    }
                    
                }
                if (invType == 3)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRCustomerReturn.rpt";
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvCustomerReturn.ReportSource = crp;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
    }
}