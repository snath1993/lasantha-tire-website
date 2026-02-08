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
    public partial class frmViewerItemWiseSales : Form
    {
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvInvoice;
        DataSet ds = new DataSet();

        public frmViewerItemWiseSales(frmItemReport frmParent)
        {
            InitializeComponent();
            ds = frmParent.DsEst;

        }



        private void frmViewerItemWiseSales_Load(object sender, EventArgs e)
        {
            string Myfullpath;
            ReportDocument crp = new ReportDocument();
            ParameterFields pfields = new ParameterFields();

            try
            {

                switch (clsPara.RepNo)
                {
                    case 7001:

                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRJobVarience1.rpt";

                        break;

                    case 7002:

                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRJobProduct.rpt";

                        break;

                    case 7003:

                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRDailyProductionSales.rpt";

                        break;

                    case 7004:

                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\RptCustomerProfitability.rpt";


                        break;

                    case 7005:

                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\RptMaterialAvailableOutstanding.rpt";


                        break;

                    case 7006:

                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\RptDailyProductionValuation.rpt";

                        break;

                    default:

                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRJobProduct.rpt";

                        break;

                }
                crp.Load(Myfullpath);

                ParameterField pfieldFDate = new ParameterField();
                ParameterDiscreteValue disValFDate = new ParameterDiscreteValue();
                ParameterField pfieldTDate = new ParameterField();
                ParameterDiscreteValue disValTDate = new ParameterDiscreteValue();

                if (clsPara.ArrItem[0] == 1)
                {
                    disValFDate.Value = clsPara.dtFrom;
                    pfieldFDate.Name = "FromDate";
                    pfieldFDate.CurrentValues.Add(disValFDate);


                    disValTDate.Value = clsPara.dtTo;
                    pfieldTDate.Name = "ToDate";
                    pfieldTDate.CurrentValues.Add(disValTDate);


                }

                ParameterField pfieldFWarehouse = new ParameterField();
                ParameterDiscreteValue disValFWarehouse = new ParameterDiscreteValue();
                ParameterField pfieldTWarehouse = new ParameterField();
                ParameterDiscreteValue disValTWarehouse = new ParameterDiscreteValue();

                if (clsPara.ArrItem[1] == 1)
                {

                    disValFWarehouse.Value = clsPara.StrLocFromCode;
                    pfieldFWarehouse.Name = "FromWarehouse";
                    pfieldFWarehouse.CurrentValues.Add(disValFWarehouse);

                    disValTWarehouse.Value = clsPara.StrLocToCode;
                    pfieldTWarehouse.Name = "ToWarehouse";
                    pfieldTWarehouse.CurrentValues.Add(disValTWarehouse);
                }

                ParameterField pfieldFJob = new ParameterField();
                ParameterDiscreteValue disValFJob = new ParameterDiscreteValue();
                ParameterField pfieldTJob = new ParameterField();
                ParameterDiscreteValue disValTJob = new ParameterDiscreteValue();

                if (clsPara.ArrItem[2] == 1)
                {

                    disValFJob.Value = clsPara.StrFromJob;
                    pfieldFJob.Name = "FromJob";
                    pfieldFJob.CurrentValues.Add(disValFJob);

                    disValTJob.Value = clsPara.StrToJob;
                    pfieldTJob.Name = "ToJob";
                    pfieldTJob.CurrentValues.Add(disValTJob);
                }



                ParameterField pfieldFItem = new ParameterField();
                ParameterDiscreteValue disValFItem = new ParameterDiscreteValue();
                ParameterField pfieldTItem = new ParameterField();
                ParameterDiscreteValue disValTItem = new ParameterDiscreteValue();

                if (clsPara.ArrItem[3] == 1)
                {
                    disValFItem.Value = clsPara.StrItemFromCode;
                    pfieldFItem.Name = "ItemFrom";
                    pfieldFItem.CurrentValues.Add(disValFItem);

                    disValTItem.Value = clsPara.StrItemToCode;
                    pfieldTItem.Name = "ItemTo";
                    pfieldTItem.CurrentValues.Add(disValTItem);

                }

                ParameterField pfieldFCustomer = new ParameterField();
                ParameterDiscreteValue disValFCustomer = new ParameterDiscreteValue();
                ParameterField pfieldTCustomer = new ParameterField();
                ParameterDiscreteValue disValTCustomer = new ParameterDiscreteValue();

                if (clsPara.ArrItem[4] == 1)
                {
                    disValFCustomer.Value = clsPara.StrFromCustomer;
                    pfieldFCustomer.Name = "FromCustomer";
                    pfieldFCustomer.CurrentValues.Add(disValFCustomer);

                    disValTCustomer.Value = clsPara.StrToCustomer;
                    pfieldTCustomer.Name = "ToCustomer";
                    pfieldTCustomer.CurrentValues.Add(disValTCustomer);
                }



                ParameterField pfieldFirstGroup = new ParameterField();
                ParameterDiscreteValue disValFirstGroup = new ParameterDiscreteValue();
                ParameterField pfieldSecondGroup = new ParameterField();
                ParameterDiscreteValue disValSecondGroup = new ParameterDiscreteValue();
                ParameterField pfieldReportGroup2 = new ParameterField();
                ParameterDiscreteValue disValdReportGroup2 = new ParameterDiscreteValue();


                if (clsPara.ArrItem[5] == 1)
                {
                    if (clsPara.StrFirstGroup.ToString() == clsPara.StrSecondGroup.ToString())
                    {

                        crp.DataDefinition.Groups[0].ConditionField = crp.Database.Tables[clsPara.intIndex1].Fields[clsPara.StrFirstGroup];

                    }
                    else
                    {
                        crp.DataDefinition.Groups[1].ConditionField = crp.Database.Tables[clsPara.intIndex2].Fields[clsPara.StrSecondGroup];
                        crp.DataDefinition.Groups[0].ConditionField = crp.Database.Tables[clsPara.intIndex1].Fields[clsPara.StrFirstGroup];

                    }


                    disValFirstGroup.Value = clsPara.StrFirstGroup;
                    pfieldFirstGroup.Name = "FirstGroup";
                    pfieldFirstGroup.CurrentValues.Add(disValFirstGroup);
                    disValSecondGroup.Value = clsPara.StrSecondGroup;
                    pfieldSecondGroup.Name = "SecondGroup";
                    pfieldSecondGroup.CurrentValues.Add(disValSecondGroup);
                    disValdReportGroup2.Value = clsPara.intGroup2Hide;
                    pfieldReportGroup2.Name = "Group2Hide";
                    pfieldReportGroup2.CurrentValues.Add(disValdReportGroup2);
                }

                ParameterField pfieldSummary = new ParameterField();
                ParameterDiscreteValue disValdSummary = new ParameterDiscreteValue();

                if (clsPara.ArrItem[6] == 1)
                {
                    disValdSummary.Value = clsPara.intIsSummary;
                    pfieldSummary.Name = "IsSummary";
                    pfieldSummary.CurrentValues.Add(disValdSummary);
                }


                ParameterField pfieldCompany = new ParameterField();
                ParameterDiscreteValue disValdCompany = new ParameterDiscreteValue();
                disValdCompany.Value = clsPara.StrComName;
                pfieldCompany.Name = "Company";
                pfieldCompany.CurrentValues.Add(disValdCompany);

                ParameterField pfieldReport = new ParameterField();
                ParameterDiscreteValue disValdReport = new ParameterDiscreteValue();
                disValdReport.Value = clsPara.StrReportName;
                pfieldReport.Name = "ReportName";
                pfieldReport.CurrentValues.Add(disValdReport);

                ParameterField pfieldReport1 = new ParameterField();
                ParameterDiscreteValue disValdReport1 = new ParameterDiscreteValue();
                disValdReport1.Value = clsPara.StrReportName1;
                pfieldReport1.Name = "ReportName1";
                pfieldReport1.CurrentValues.Add(disValdReport1);




                if (clsPara.ArrItem[0] == 1)
                {
                    pfields.Add(pfieldFDate);
                    pfields.Add(pfieldTDate);
                }

                if (clsPara.ArrItem[1] == 1)
                {
                    pfields.Add(pfieldFWarehouse);
                    pfields.Add(pfieldTWarehouse);
                }

                if (clsPara.ArrItem[2] == 1)
                {
                    pfields.Add(pfieldFJob);
                    pfields.Add(pfieldTJob);
                }

                if (clsPara.ArrItem[3] == 1)
                {
                    pfields.Add(pfieldFItem);
                    pfields.Add(pfieldTItem);

                }

                if (clsPara.ArrItem[4] == 1)
                {
                    pfields.Add(pfieldFCustomer);
                    pfields.Add(pfieldTCustomer);

                }


                if (clsPara.ArrItem[5] == 1)
                {
                    pfields.Add(pfieldFirstGroup);
                    pfields.Add(pfieldSecondGroup);
                    pfields.Add(pfieldReportGroup2);

                }

                if (clsPara.ArrItem[6] == 1)
                {
                    pfields.Add(pfieldSummary);

                }


                pfields.Add(pfieldReport);
                pfields.Add(pfieldReport1);
                pfields.Add(pfieldCompany);

                crvInvoice.ParameterFieldInfo = pfields;
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvInvoice.ReportSource = crp;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }




        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewerItemWiseSales));
            this.crvInvoice = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvInvoice
            // 
            this.crvInvoice.ActiveViewIndex = -1;
            this.crvInvoice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvInvoice.Location = new System.Drawing.Point(0, 0);
            this.crvInvoice.Name = "crvInvoice";
            this.crvInvoice.Size = new System.Drawing.Size(833, 496);
            this.crvInvoice.TabIndex = 1;
            this.crvInvoice.Load += new System.EventHandler(this.crvInvoice_Load);
            // 
            // frmViewerItemWiseSales
            // 
            this.ClientSize = new System.Drawing.Size(833, 496);
            this.Controls.Add(this.crvInvoice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmViewerItemWiseSales";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmViewerItemWiseSales_Load);
            this.ResumeLayout(false);

        }

        private void crvInvoice_Load(object sender, EventArgs e)
        {

        }


    }
}