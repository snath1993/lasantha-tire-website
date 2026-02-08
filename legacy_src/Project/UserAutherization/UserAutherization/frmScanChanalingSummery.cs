using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmScanChanalingSummery : Form
    {
        public frmScanChanalingSummery(frmScanInformationReport frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
            //ds1 = frmParent.Dsinvc;
            PrintOpt = frmParent.PrintType;
        }
        public static string ConnectionString;
        DataSet ds;
        DataSet ds1;
        int PrintOpt = 1;
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        private void frmScanChanalingSummery_Load(object sender, EventArgs e)
        {
            try
            {
                if (PrintOpt == 1)
                {
                    //summery report print here option 1
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRDailyIncomeSummery.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvMaxViewer.ReportSource = crp;
                }
                else
                {
                    //Detail report will print Option 2
                    string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRScanChanaling.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvMaxViewer.ReportSource = crp;

                }
            }
            catch { }
        }
    }
}