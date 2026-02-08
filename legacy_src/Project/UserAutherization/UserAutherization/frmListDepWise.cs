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

namespace UserAutherization
{
    public partial class frmListDepWise : Form
    {
        public static string ConnectionString;
        DataSet ds;
        DataSet dp;
        public int PrnType;
        public frmListDepWise(frmDepartmentWiseSales frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsDipDaily;
            dp = frmParent.DsVatpay;
            PrnType = frmParent.Reptype; 
        }
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        string Myfullpath;

        private void frmListDepWise_Load(object sender, EventArgs e)
        {
            if (PrnType == 0)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRDipWiseSales.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 1)
            { 
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRDateWiseSales.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CryDaillyColView.ReportSource = crp;                
            }
            if (PrnType == 2)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceWiseSales.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 3)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRDipWiseSalesSummary.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 4)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRIndividualDipWiseSalesSummary.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 5)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRDipWiseSalesDetails.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 6)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRYVatReportInvoiceWise.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 7)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRYVatReportInvoiceWiseDetails.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 8)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptVatPayanble-Raw.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(dp);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 9)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptVatPayanble.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(dp);
                CryDaillyColView.ReportSource = crp;
            }
            if (PrnType == 10)
            {
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptwherehousewiseqty.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(dp);
                CryDaillyColView.ReportSource = crp;
            }
        }
    }
}
