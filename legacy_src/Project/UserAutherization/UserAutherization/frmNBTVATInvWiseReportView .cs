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
    public partial class frmNBTVATInvWiseReportView : Form
    {
      
        public int Rtype;
        public string FromDate, Todate;
        public frmNBTVATInvWiseReportView()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;
        DataSet ds;
        private frmNBTVATReport frmNBTVATReport;
        private int reportType;
        public ReportDocument crp = new ReportDocument();
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

       
       

        public frmNBTVATInvWiseReportView(frmNBTVATReport frmParent, int ReportType, string fromDate, string todate)
        {
            InitializeComponent();
            setConnectionString();
            FromDate = fromDate;
            Todate = todate;
            Rtype = frmParent.ReportType;
            ds = frmParent.DSInvoicing;
        }

        //private void crvMaxViewer_DoubleClick(object sender,  CrystalDecisions.Windows.Forms.PageMouseEventArgs e)
        //{

        //    MessageBox.Show(e.ObjectInfo.Name);
        //    //for (int j =0; j<= crp.Database.Tables[0].Fields.Count -1;j++)
        //    //{
        //    //    MessageBox.Show(crp.Rows[1].DataRowView.Row.ItemArray.GetValue(j).ToString());
        //    //}
        //}



        private void frmSalesWiseReportView_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath ="";
                if (Rtype == 1)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptNBTVatInvWise.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                }

                else if(Rtype == 2)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptNBTVatItemWise.rpt";
                }
               
                
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvMaxViewer.ReportSource = crp;

                ParameterFields form = new ParameterFields();

                ParameterField fd = new ParameterField();
                fd.Name = "FromDate";
                ParameterDiscreteValue val = new ParameterDiscreteValue();
                val.Value =FromDate;
                fd.CurrentValues.Add(val);
                form.Add(fd);
                crvMaxViewer.ParameterFieldInfo = form;

                ParameterField fd2 = new ParameterField();
                fd2.Name = "ToDate";
                ParameterDiscreteValue val2 = new ParameterDiscreteValue();
                val2.Value = Todate;
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
