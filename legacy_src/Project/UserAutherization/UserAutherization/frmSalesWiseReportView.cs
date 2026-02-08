using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmSalesWiseReportView : Form
    {
        private frmSalesWiseReport frmSalesWiseReport;
        public int Rtype;
        public string FromDate, Todate,CName;
        public frmSalesWiseReportView()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;
        DataSet ds;
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

       
        public frmSalesWiseReportView(frmSalesWiseReport frmParent,int ReportType,string Fdate,string TDate,string cname)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
            Rtype = frmParent.ReportType;
            FromDate = Fdate;
            Todate = TDate;
            CName = cname;
        }

        private void frmSalesWiseReportView_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath ="";
                if (Rtype == 1)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptItemWiseSales.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                }

                else if(Rtype == 2)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptCategroyWiseSales.rpt";
                }
                else if(Rtype == 3)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptBrandWiseSales.rpt";
                }
                else if (Rtype == 5)
                {
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptSalesRepWise.rpt";
                }
                ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvMaxViewer.ReportSource = crp;

                ParameterFields form = new ParameterFields();

                ParameterField fd = new ParameterField();
                fd.Name = "FromDate";
                ParameterDiscreteValue val = new ParameterDiscreteValue();
                val.Value =FromDate.ToString();
                fd.CurrentValues.Add(val);
                form.Add(fd);
                crvMaxViewer.ParameterFieldInfo = form;

                ParameterField fd2 = new ParameterField();
                fd2.Name = "ToDate";
                ParameterDiscreteValue val2 = new ParameterDiscreteValue();
                val2.Value =Todate.ToString();
                fd2.CurrentValues.Add(val2);
                form.Add(fd2);
                crvMaxViewer.ParameterFieldInfo = form;


                ParameterField fd3 = new ParameterField();
                fd3.Name = "CName";
                ParameterDiscreteValue val3 = new ParameterDiscreteValue();
                val3.Value = CName;
                fd3.CurrentValues.Add(val3);
                form.Add(fd3);
                crvMaxViewer.ParameterFieldInfo = form;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
