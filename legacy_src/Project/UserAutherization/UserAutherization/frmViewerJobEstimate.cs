using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;


namespace UserAutherization
{
    public partial class frmViewerJobEstimate : Form
    {
        DataSet ds = new DataSet();

        public frmViewerJobEstimate(frmJobEstimate frmParent)
        {
            InitializeComponent();
            ds = frmParent.DsEst;

        }

        private void frmViewerJobEstimate_Load(object sender, EventArgs e)
        {
            try
            {
               
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                ParameterFields pfields = new ParameterFields();

                if (clsPara.intEstPrint == 1)
                {

                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CREstimate.rpt";
                   
                }
                else if (clsPara.intEstPrint == 2)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\RptMRN.rpt";
                   
                }
                else if (clsPara.intEstPrint == 3)
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\RptJobTiket.rpt";
                   
                }
                else
                {
                     Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CREstimate.rpt";
                   
                }

                ParameterField pfieldCompany = new ParameterField();
                ParameterDiscreteValue disValdCompany = new ParameterDiscreteValue();
                disValdCompany.Value = clsPara.StrComName;
                pfieldCompany.Name = "Company";
                pfieldCompany.CurrentValues.Add(disValdCompany);



                pfields.Add(pfieldCompany);

                crystalReportViewer1.ParameterFieldInfo = pfields;
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crystalReportViewer1.ReportSource = crp;

            }
            catch (Exception)
            {
                
                throw;
            }


        

        }

       

  

    
    }
}