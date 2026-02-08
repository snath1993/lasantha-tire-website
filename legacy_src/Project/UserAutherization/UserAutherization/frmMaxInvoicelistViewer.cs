using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace UserAutherization
{
    public partial class frmMaxInvoicelistViewer : Form
    {
        public int Prtype;
        public string Cname;
        public frmMaxInvoicelistViewer(frmInvoiceListPrint frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
            ds1 = frmParent.Dsinvc;
            ds2 = frmParent.DSM;
            Prtype = frmParent.PrintType; 
        }

        public frmMaxInvoicelistViewer(frmInvoiceListPrint frmParent, string cname) 
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
            ds1 = frmParent.Dsinvc;
            ds2 = frmParent.DSM;
            Prtype = frmParent.PrintType;
            Cname = cname;
        }

        public static string ConnectionString;
        DataSet ds;
        DataSet ds1;
        DataSet ds2;
        private string cname;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        private void frmMaxInvoicelistViewer_Load(object sender, EventArgs e)
        {
            try
            {
                ParameterFields form = new ParameterFields();

                ParameterField fd = new ParameterField();
                fd.Name = "CName";
                ParameterDiscreteValue val = new ParameterDiscreteValue();
                val.Value = Cname;
                fd.CurrentValues.Add(val);
                form.Add(fd);
                crvMaxViewer.ParameterFieldInfo = form;
                //CRMaxInvoiceSummaryList.rpt
                //string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRMaxInvoiceSummaryList.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                if (Prtype == 1)
                {

                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRMaxInvoiceList.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvMaxViewer.ReportSource = crp;
                }
                else if (Prtype == 2)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRSalesInvoiceSummaryList.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds1);
                    crvMaxViewer.ReportSource = crp;
                }

                else if (Prtype == 3)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRFOCMaxInvoiceList.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvMaxViewer.ReportSource = crp;
                }

                else if (Prtype == 4)
                {
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRMultiINVDetails.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds2);
                    crvMaxViewer.ReportSource = crp;
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}