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
    public partial class frmVehicleHistoryView : Form
    {
      
        public int Rtype;
        public string FromDate, Todate;
        public frmVehicleHistoryView()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;
        DataSet ds;
        private frmCustomerHistory frmCustomerHistory;
        private int reportType;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

       
      

        public frmVehicleHistoryView(frmCustomerHistory frmParent, int reportType, string fromDate, string todate)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
            Rtype = frmParent.ReportType;
            FromDate = fromDate;
            Todate = todate;
        }

        private void frmSalesWiseReportView_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath ="";
                if (Rtype == 1)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptVehicleDetailedHistory.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                }

                else if(Rtype == 2)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptVehicleDetailedHistory.rpt";
                }
                else if(Rtype == 3)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptVehicleDetailedHistory.rpt";
                }
                    ReportDocument crp = new ReportDocument();
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvMaxViewer.ReportSource = crp;

                ParameterFields form = new ParameterFields();

                ParameterField fd = new ParameterField();
                fd.Name = "FromDate";
                ParameterDiscreteValue val = new ParameterDiscreteValue();
                val.Value =Convert.ToDateTime(FromDate).ToString("dd/MMM/yyy");
                fd.CurrentValues.Add(val);
                form.Add(fd);
                crvMaxViewer.ParameterFieldInfo = form;

                ParameterField fd2 = new ParameterField();
                fd2.Name = "ToDate";
                ParameterDiscreteValue val2 = new ParameterDiscreteValue();
                val2.Value = Convert.ToDateTime(Todate).ToString("dd/MMM/yyy");
                fd2.CurrentValues.Add(val2);
                form.Add(fd2);
                crvMaxViewer.ParameterFieldInfo = form;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
