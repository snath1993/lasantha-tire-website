using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;


namespace UserAutherization
{
    public partial class frmItemReport : Form
    {
        public DataSet dsWarehouse;
        public DataSet dsItem;
        public DataSet dsCustomer;
        public DataSet dsJob;
        string StrSql;
        public static string ConnectionString;

        public DSEstimate DsEst = new DSEstimate();


        public frmItemReport()
        {
            InitializeComponent();
            setConnectionString();
            FillItems();

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


        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void FillItems()
        {

            dsWarehouse = new DataSet();
            dsItem = new DataSet();
            dsCustomer = new DataSet();
            dsJob = new DataSet();

            try
            {
                dtpDateFrom.Value = DateTime.Now;
                dtpDateTo.Value = DateTime.Now;

                dsWarehouse.Clear();
                dsItem.Clear();
                dsCustomer.Clear();
                dsJob.Clear();

                StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster  order by WhseId";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                da.Fill(dsWarehouse, "dsWarehouse");

                StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster ORDER BY ItemID";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                da.Fill(dsItem, "dsItem");

                StrSql = "SELECT CutomerID,CustomerName FROM tblCustomerMaster ORDER BY CutomerID";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                da.Fill(dsCustomer, "dsCustomer");

                StrSql = "SELECT JobID,JobDescription FROM tblJobMaster order by JobID";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                da.Fill(dsJob, "dsJob");



                cmbJobFrom.DataSource = dsJob.Tables["dsJob"];
                cmbJobFrom.DisplayMember = "JobID";
                cmbJobFrom.ValueMember = "JobID";

                cmbJobTo.DataSource = dsJob.Tables["dsJob"];
                cmbJobTo.DisplayMember = "JobID";
                cmbJobTo.ValueMember = "JobID";



                cmbFromWarehouce.DataSource = dsWarehouse.Tables["dsWarehouse"];
                cmbFromWarehouce.DisplayMember = "WhseName";
                cmbFromWarehouce.ValueMember = "WhseId";

                cmbToWarehouce.DataSource = dsWarehouse.Tables["dsWarehouse"];
                cmbToWarehouce.DisplayMember = "WhseName";
                cmbToWarehouce.ValueMember = "WhseId";

                cmbItemFrom.DataSource = dsItem.Tables["dsItem"];
                cmbItemFrom.DisplayMember = "ItemDescription";
                cmbItemFrom.ValueMember = "ItemID";

                cmbItemTo.DataSource = dsItem.Tables["dsItem"];
                cmbItemTo.DisplayMember = "ItemDescription";
                cmbItemTo.ValueMember = "ItemID";

                
                cmbCustomerFrom.DataSource = dsCustomer.Tables["dsCustomer"];
                cmbCustomerFrom.DisplayMember = "CustomerName";
                cmbCustomerFrom.ValueMember = "CutomerID";

                cmbCustomerTo.DataSource = dsCustomer.Tables["dsCustomer"];
                cmbCustomerTo.DisplayMember = "CustomerName";
                cmbCustomerTo.ValueMember = "CutomerID";

                cmbGroup1.Items.Clear();
                cmbGroup1.Items.Add("Warehouse");
                cmbGroup1.Items.Add("Job");
                cmbGroup1.Items.Add("Item");
                //cmbGroup1.Items.Add("Customer");
                
                cmbGroup2.Items.Clear();
                cmbGroup2.Items.Add("(None)");
                cmbGroup2.Items.Add("Warehouse");
                cmbGroup2.Items.Add("Job");
                cmbGroup2.Items.Add("Item");
                //cmbGroup2.Items.Add("Customer");
            
             

                cmbGroup1.SelectedItem = "Warehouse";
                cmbGroup2.SelectedItem = "(None)";

                cmbFromWarehouce.SelectedIndex = 0;
                cmbToWarehouce.SelectedIndex = 0;

                cmbJobFrom.SelectedIndex = 0;
                cmbJobTo.SelectedIndex = 0;

                cmbItemFrom.SelectedIndex = 0;
                cmbItemTo.SelectedIndex = 0;

                cmbCustomerFrom.SelectedIndex = 0;
                cmbCustomerTo.SelectedIndex = 0;

            }
            catch (Exception)
            {

                throw;
            }
        }


      

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void EnableControl()
        {
            try
            {

                foreach (Control ctrl in this.Controls)
                {
                    foreach (Control ctrl2 in ctrl.Controls)
                    {
                        if (ctrl2 is DateTimePicker)
                        {
                            ctrl2.Enabled = true;
                        }
                    }

                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void frmItemReport_Load(object sender, EventArgs e)
        {
            EnableControl();
            FillItems();

            try 
            {
                switch (clsPara.RepNo)
                {
                    case 7001:
                        this.Text = "Job Varience";
                        VisibleLocation(1, 1, 1, 1, 1, 1, 1, 1);
                        clsPara.ArrItem[0] = 1;
                        clsPara.ArrItem[1] = 1;
                        clsPara.ArrItem[2] = 1;
                        clsPara.ArrItem[3] = 1;
                        clsPara.ArrItem[4] = 1;
                        clsPara.ArrItem[5] = 1;
                        clsPara.ArrItem[6] = 1;
                        clsPara.ArrItem[7] = 1;
                        break;

                    case 7002:
                        this.Text = "Job Production";
                        VisibleLocation(1, 0, 1, 0, 1, 0, 0, 1);
                        clsPara.ArrItem[0] = 1;
                        clsPara.ArrItem[1] = 0;
                        clsPara.ArrItem[2] = 1;
                        clsPara.ArrItem[3] = 0;
                        clsPara.ArrItem[4] = 1;
                        clsPara.ArrItem[5] = 0;
                        clsPara.ArrItem[6] = 1;
                        clsPara.ArrItem[7] = 1;
                        dtpDateFrom.Value = dtpDateFrom.Value.AddYears(-100);
                        dtpDateFrom.Enabled = false;
                        break;
                    case 7003:
                        this.Text = "Job Production,Sales And Return";
                        VisibleLocation(1, 0, 1, 0, 1, 0, 0, 1);
                        clsPara.ArrItem[0] = 1;
                        clsPara.ArrItem[1] = 0;
                        clsPara.ArrItem[2] = 1;
                        clsPara.ArrItem[3] = 0;
                        clsPara.ArrItem[4] = 1;
                        clsPara.ArrItem[5] = 0;
                        clsPara.ArrItem[6] = 1;
                        clsPara.ArrItem[7] = 1;
                        dtpDateFrom.Value = dtpDateFrom.Value.AddYears(-100);
                        //dtpDateFrom.Enabled = false;
                        break;
                    case 7004:
                        this.Text = "Job Profitability";
                        VisibleLocation(1, 0, 1, 0, 1, 0, 0, 1);
                        clsPara.ArrItem[0] = 1;
                        clsPara.ArrItem[1] = 0;
                        clsPara.ArrItem[2] = 1;
                        clsPara.ArrItem[3] = 0;
                        clsPara.ArrItem[4] = 1;
                        clsPara.ArrItem[5] = 0;
                        clsPara.ArrItem[6] = 1;
                        clsPara.ArrItem[7] = 1;
                        dtpDateFrom.Value = dtpDateFrom.Value.AddYears(-100);
                        //dtpDateFrom.Enabled = false;
                        break;
                    case 7005:
                        this.Text = "Material Available For Outstanding Order";
                        VisibleLocation(1, 1, 1, 1, 1, 1, 1, 1);
                        clsPara.ArrItem[0] = 1;
                        clsPara.ArrItem[1] = 1;
                        clsPara.ArrItem[2] = 1;
                        clsPara.ArrItem[3] = 1;
                        clsPara.ArrItem[4] = 1;
                        clsPara.ArrItem[5] = 1;
                        clsPara.ArrItem[6] = 1;
                        clsPara.ArrItem[7] = 1;
                        break;

                    case 7006:
                        this.Text = "Daily Finished Goods Valuation";
                        VisibleLocation(1, 0, 0, 0, 0, 1, 1, 1);
                        clsPara.ArrItem[0] = 1;
                        clsPara.ArrItem[1] = 0;
                        clsPara.ArrItem[2] = 0;
                        clsPara.ArrItem[3] = 0;
                        clsPara.ArrItem[4] = 0;
                        clsPara.ArrItem[5] = 1;
                        clsPara.ArrItem[6] = 1;
                        clsPara.ArrItem[7] = 1;
                        break;

                    default:
                        break;

                }

            }
            catch (Exception)
            {

            throw;
            }
              
        }

        private void VisibleLocation(int intDate, int intWarehouse, int intJob, int intItem, int intCustomer, int intGroup, int intOption, int intPrint)
        {
            try
            {
                int intTop = 12;

                if (intDate == 1)
                {
                    grpDate.Top = intTop;
                    intTop = intTop + 52;
                    grpDate.Visible = true;
                }
                else
                {
                    grpDate.Visible = false;

                }

                if (intWarehouse == 1)
                {
                    grpWarehouse.Top = intTop;
                    intTop = intTop +52;
                    grpWarehouse.Visible = true;
                }
                else
                {
                    grpWarehouse.Visible = false;

                }

                if (intJob == 1)
                {
                    grpJob.Top = intTop;
                    intTop = intTop + 52;
                    grpJob.Visible = true;
                }
                else
                {
                    grpJob.Visible = false;

                }

                if (intItem == 1)
                {
                    grpItem.Top = intTop;
                    intTop = intTop + 52;
                    grpItem.Visible = true;
                }
                else
                {
                    grpItem.Visible = false;

                }

                if (intCustomer == 1)
                {
                    grpCustomer.Top = intTop;
                    intTop = intTop + 52;
                    grpCustomer.Visible = true;
                }
                else
                {
                    grpCustomer.Visible = false;

                }

                if (intGroup == 1)
                {
                    grpGroup.Top = intTop;
                    intTop = intTop + 52;
                    grpGroup.Visible = true;
                }
                else
                {
                    grpGroup.Visible = false;

                }

                if (intOption == 1)
                {
                    grpOption.Top = intTop;
                    intTop = intTop + 52;
                    grpOption.Visible = true;
                }
                else
                {
                    grpOption.Visible = false;

                }

                if (intPrint == 1)
                {
                    grpPrint.Top = intTop;
                    intTop = intTop + 52;
                    grpPrint.Visible = true;
                }
                else
                {
                    grpPrint.Visible = false;

                }

                intTop = intTop + 50;

                this.Height = intTop;


            }
            catch (Exception)
            {
                
                throw;
            }

        }

       

        public static string GetDateTime(DateTime DtGetDate)
        {
            DateTime DTP = Convert.ToDateTime(DtGetDate);
            string Dformat = "MM/dd/yyyy";
            return DTP.ToString(Dformat);

        }

        private void  PrintAndFill(int intDate, int intWarehouse, int intJob, int intItem, int intCustomer, int intGroup, int intOption, int intPrint)
        {
            try
            {
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                DsEst.Clear();
                clsPara.dtFrom = dtpDateFrom.Value;
                clsPara.dtTo = dtpDateTo.Value;

                if (chkWarehouse.Checked == true)
                {
                    clsPara.StrLocFromCode = "All";
                    clsPara.StrLocToCode = "All";

                }
                else
                {
                    clsPara.StrLocFromCode = cmbFromWarehouce.Text.Trim();
                    clsPara.StrLocToCode = cmbToWarehouce.Text.Trim();
                }

                if (chkItem.Checked == true)
                {
                    clsPara.StrItemFromCode = "All";
                    clsPara.StrItemToCode = "All";
                }
                else
                {
                    clsPara.StrItemFromCode = cmbItemFrom.Text.Trim();
                    clsPara.StrItemToCode = cmbItemTo.Text.Trim();
                }

                if (chkCustomer.Checked == true)
                {
                    clsPara.StrFromCustomer = "All";
                    clsPara.StrToCustomer = "All";
                }
                else
                {
                    clsPara.StrFromCustomer = cmbCustomerFrom.Text.Trim();
                    clsPara.StrToCustomer = cmbCustomerTo.Text.Trim();
                }

                if (chkJob.Checked == true)
                {
                    clsPara.StrFromJob = "All";
                    clsPara.StrToJob = "All";
                }
                else
                {
                    clsPara.StrFromJob = cmbJobFrom.Text.Trim();
                    clsPara.StrToJob = cmbJobTo.Text.Trim();
                }

                switch (clsPara.RepNo)
                {
                    case 7001:
                        {
                            switch (cmbGroup1.SelectedIndex)
                            {

                                case 0: { clsPara.StrFirstGroup = "WarehouseCode"; clsPara.StrReportName = "Warehousewise"; clsPara.intIndex1 = 1; break; }
                                case 1: { clsPara.StrFirstGroup = "JobCode"; clsPara.StrReportName = "Jobwise"; clsPara.intIndex1 = 1; break; }
                                case 2: { clsPara.StrFirstGroup = "Item_ID"; clsPara.StrReportName = "Itemwise"; clsPara.intIndex1 = 0; break; }
                                case 3: { clsPara.StrFirstGroup = "CustomerID"; clsPara.StrReportName = "Customerwise"; clsPara.intIndex1 = 2; break; }

                            }

                            switch (cmbGroup2.SelectedIndex)
                            {
                                case 0: { clsPara.StrSecondGroup = clsPara.StrFirstGroup; clsPara.StrReportName1 = ""; clsPara.intIndex2 = clsPara.intIndex1; break; }
                                case 1: { clsPara.StrSecondGroup = "WarehouseCode"; clsPara.StrReportName1 = "Warehousewise"; clsPara.intIndex2 = 1; break; }
                                case 2: { clsPara.StrSecondGroup = "JobCode"; clsPara.StrReportName1 = "Jobwise"; clsPara.intIndex2 = 1; break; }
                                case 3: { clsPara.StrSecondGroup = "Item_ID"; clsPara.StrReportName1 = "Itemwise"; clsPara.intIndex2 = 0; break; }
                                case 4: { clsPara.StrSecondGroup = "CustomerID"; clsPara.StrReportName1 = "Customerwise"; clsPara.intIndex2 = 3; break; }

                            }


                            break;
                        }
                    case 7002:
                        {
                            switch (cmbGroup1.SelectedIndex)
                            {
                                
                                case 0: { clsPara.StrFirstGroup = "AutoIndex"; clsPara.StrReportName = "Warehousewise"; clsPara.intIndex1 = 1; break; }
                                case 1: { clsPara.StrFirstGroup = "JobCode"; clsPara.StrReportName = "Jobwise"; clsPara.intIndex1 = 1; break; }
                                case 2: { clsPara.StrFirstGroup = "Item_ID"; clsPara.StrReportName = "Itemwise"; clsPara.intIndex1 = 0; break; }
                                case 3: { clsPara.StrFirstGroup = "CustomerID"; clsPara.StrReportName = "Customerwise"; clsPara.intIndex1 = 2; break; }

                            }

                            switch (cmbGroup2.SelectedIndex)
                            {
                                case 0: { clsPara.StrSecondGroup = clsPara.StrFirstGroup; clsPara.StrReportName1 = ""; clsPara.intIndex2 = clsPara.intIndex1; break; }
                                case 1: { clsPara.StrSecondGroup = "WarehouseCode"; clsPara.StrReportName1 = "Warehousewise"; clsPara.intIndex2 = 1; break; }
                                case 2: { clsPara.StrSecondGroup = "JobCode"; clsPara.StrReportName1 = "Jobwise"; clsPara.intIndex2 = 1; break; }
                                case 3: { clsPara.StrSecondGroup = "Item_ID"; clsPara.StrReportName1 = "Itemwise"; clsPara.intIndex2 = 0; break; }
                                case 4: { clsPara.StrSecondGroup = "CustomerID"; clsPara.StrReportName1 = "Customerwise"; clsPara.intIndex2 = 3; break; }

                            }
                            
                            break;
                        }

                    case 7003:
                        {
                            switch (cmbGroup1.SelectedIndex)
                            {

                                case 0: { clsPara.StrFirstGroup = "AutoIndex"; clsPara.StrReportName = "Warehousewise"; clsPara.intIndex1 = 1; break; }
                                case 1: { clsPara.StrFirstGroup = "JobCode"; clsPara.StrReportName = "Jobwise"; clsPara.intIndex1 = 1; break; }
                                case 2: { clsPara.StrFirstGroup = "Item_ID"; clsPara.StrReportName = "Itemwise"; clsPara.intIndex1 = 0; break; }
                                case 3: { clsPara.StrFirstGroup = "CustomerID"; clsPara.StrReportName = "Customerwise"; clsPara.intIndex1 = 2; break; }

                            }

                            switch (cmbGroup2.SelectedIndex)
                            {
                                case 0: { clsPara.StrSecondGroup = clsPara.StrFirstGroup; clsPara.StrReportName1 = ""; clsPara.intIndex2 = clsPara.intIndex1; break; }
                                case 1: { clsPara.StrSecondGroup = "WarehouseCode"; clsPara.StrReportName1 = "Warehousewise"; clsPara.intIndex2 = 1; break; }
                                case 2: { clsPara.StrSecondGroup = "JobCode"; clsPara.StrReportName1 = "Jobwise"; clsPara.intIndex2 = 1; break; }
                                case 3: { clsPara.StrSecondGroup = "Item_ID"; clsPara.StrReportName1 = "Itemwise"; clsPara.intIndex2 = 0; break; }
                                case 4: { clsPara.StrSecondGroup = "CustomerID"; clsPara.StrReportName1 = "Customerwise"; clsPara.intIndex2 = 3; break; }

                            }

                            break;
                        }

                    case 7004:
                        {
                            switch (cmbGroup1.SelectedIndex)
                            {

                                case 0: { clsPara.StrFirstGroup = "WarehouseCode"; clsPara.StrReportName = "Warehousewise"; clsPara.intIndex1 = 1; break; }
                                case 1: { clsPara.StrFirstGroup = "JobCode"; clsPara.StrReportName = "Jobwise"; clsPara.intIndex1 = 1; break; }
                                case 2: { clsPara.StrFirstGroup = "Item_ID"; clsPara.StrReportName = "Itemwise"; clsPara.intIndex1 = 0; break; }
                                case 3: { clsPara.StrFirstGroup = "CustomerID"; clsPara.StrReportName = "Customerwise"; clsPara.intIndex1 = 2; break; }

                            }

                            switch (cmbGroup2.SelectedIndex)
                            {
                                case 0: { clsPara.StrSecondGroup = clsPara.StrFirstGroup; clsPara.StrReportName1 = ""; clsPara.intIndex2 = clsPara.intIndex1; break; }
                                case 1: { clsPara.StrSecondGroup = "WarehouseCode"; clsPara.StrReportName1 = "Warehousewise"; clsPara.intIndex2 = 1; break; }
                                case 2: { clsPara.StrSecondGroup = "JobCode"; clsPara.StrReportName1 = "Jobwise"; clsPara.intIndex2 = 1; break; }
                                case 3: { clsPara.StrSecondGroup = "Item_ID"; clsPara.StrReportName1 = "Itemwise"; clsPara.intIndex2 = 0; break; }
                                case 4: { clsPara.StrSecondGroup = "CustomerID"; clsPara.StrReportName1 = "Customerwise"; clsPara.intIndex2 = 3; break; }

                            }

                            break;


                        }

                    case 7005:
                        {
                            switch (cmbGroup1.SelectedIndex)
                            {

                                case 0: { clsPara.StrFirstGroup = "WarehouseCode"; clsPara.StrReportName = "Warehousewise"; clsPara.intIndex1 = 1; break; }
                                case 1: { clsPara.StrFirstGroup = "JobCode"; clsPara.StrReportName = "Jobwise"; clsPara.intIndex1 = 1; break; }
                                case 2: { clsPara.StrFirstGroup = "Item_ID"; clsPara.StrReportName = "Itemwise"; clsPara.intIndex1 = 0; break; }
                                case 3: { clsPara.StrFirstGroup = "CustomerID"; clsPara.StrReportName = "Customerwise"; clsPara.intIndex1 = 2; break; }

                            }

                            switch (cmbGroup2.SelectedIndex)
                            {
                                case 0: { clsPara.StrSecondGroup = clsPara.StrFirstGroup; clsPara.StrReportName1 = ""; clsPara.intIndex2 = clsPara.intIndex1; break; }
                                case 1: { clsPara.StrSecondGroup = "WarehouseCode"; clsPara.StrReportName1 = "Warehousewise"; clsPara.intIndex2 = 1; break; }
                                case 2: { clsPara.StrSecondGroup = "JobCode"; clsPara.StrReportName1 = "Jobwise"; clsPara.intIndex2 = 1; break; }
                                case 3: { clsPara.StrSecondGroup = "Item_ID"; clsPara.StrReportName1 = "Itemwise"; clsPara.intIndex2 = 0; break; }
                                case 4: { clsPara.StrSecondGroup = "CustomerID"; clsPara.StrReportName1 = "Customerwise"; clsPara.intIndex2 = 3; break; }

                            }

                            break;
                        }
                    case 7006:
                        {
                            switch (cmbGroup1.SelectedIndex)
                            {

                                case 0: { clsPara.StrFirstGroup = "WarehouseCode"; clsPara.StrReportName = "Warehousewise"; clsPara.intIndex1 = 1; break; }
                                case 1: { clsPara.StrFirstGroup = "JobCode"; clsPara.StrReportName = "Jobwise"; clsPara.intIndex1 = 1; break; }
                                case 2: { clsPara.StrFirstGroup = "Item_ID"; clsPara.StrReportName = "Itemwise"; clsPara.intIndex1 = 0; break; }
                                case 3: { clsPara.StrFirstGroup = "CustomerID"; clsPara.StrReportName = "Customerwise"; clsPara.intIndex1 = 2; break; }

                            }

                            switch (cmbGroup2.SelectedIndex)
                            {
                                case 0: { clsPara.StrSecondGroup = clsPara.StrFirstGroup; clsPara.StrReportName1 = ""; clsPara.intIndex2 = clsPara.intIndex1; break; }
                                case 1: { clsPara.StrSecondGroup = "WarehouseCode"; clsPara.StrReportName1 = "Warehousewise"; clsPara.intIndex2 = 1; break; }
                                case 2: { clsPara.StrSecondGroup = "JobCode"; clsPara.StrReportName1 = "Jobwise"; clsPara.intIndex2 = 1; break; }
                                case 3: { clsPara.StrSecondGroup = "Item_ID"; clsPara.StrReportName1 = "Itemwise"; clsPara.intIndex2 = 0; break; }
                                case 4: { clsPara.StrSecondGroup = "CustomerID"; clsPara.StrReportName1 = "Customerwise"; clsPara.intIndex2 = 3; break; }

                            }

                            break;
                        }
                    default :
                        {
                            break;
                        }
                }

                
                if (clsPara.StrFirstGroup == clsPara.StrSecondGroup)
                {
                    clsPara.intGroup2Hide = 1;
                }
                else
                {
                    clsPara.intGroup2Hide = 0;
                }

                if (optSummery.Checked == true)
                {
                    clsPara.intIsSummary = 1;
                }
                else
                {
                    clsPara.intIsSummary = 0;
                }

                switch (clsPara.RepNo)
                {
                    case 7001:
                        StrSql = "SELECT * FROM EST_HEADER WHERE (Job_Status=2) AND (DocType in (1))";
                        break;
                    case 7002:
                        StrSql = "SELECT * FROM EST_HEADER WHERE (DocType in (1)) AND (Job_Status=2)";
                        break;
                    case 7003:
                        StrSql = "SELECT * FROM EST_HEADER WHERE (DocType in (1)) AND (Job_Status=2)";
                        break;
                    case 7004:
                        StrSql = "SELECT * FROM EST_HEADER WHERE (DocType in (1)) AND (Job_Status=2)";
                        break;
                    case 7005:
                        StrSql = "SELECT * FROM EST_HEADER WHERE (Job_Status=2) AND (DocType in (1,2,3))";
                        break;
                    case 7006:
                        StrSql = "SELECT * FROM EST_HEADER WHERE (DocType in (4)) AND (EstHed_Process=1)";
                        break;
                    default :
                        StrSql = "SELECT * FROM EST_HEADER WHERE (Job_Status=2) AND (DocType in (1,2,3))";
                        break;
                }

                

                if (intDate == 1)
                {
                    StrSql = StrSql + "AND (EstDate between '" + GetDateTime(dtpDateFrom.Value) + "' AND '" + GetDateTime(dtpDateTo.Value) + "')";
                }

                if (intJob == 1)
                {
                    if (chkJob.Checked == false)
                    {
                        StrSql = StrSql + "AND (JobCode BETWEEN '" + cmbJobFrom.Value.ToString() + "' AND '" + cmbJobTo.Value.ToString() + "' )";
                    }

                }
                
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();

                da.Fill(DsEst.DtEstimateHeader);

                StrSql = "SELECT * FROM EST_DETAILS";

                if (intItem == 1)
                {
                    if (chkItem.Checked == false)
                    {
                        StrSql = StrSql + " WHERE (Item_ID BETWEEN '" + cmbItemFrom.Value.ToString() + "' AND '" + cmbItemTo.Value.ToString() + "' )";
                    }
                }
                

                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsEst.DtEstimateDETAILS);


                StrSql = "SELECT CutomerID,CustomerName FROM tblCustomerMaster";

                if (intCustomer == 1)
                {
                    if (chkCustomer.Checked == false)
                    {
                        StrSql = StrSql + " WHERE (CutomerID BETWEEN '" + cmbCustomerFrom.Value.ToString() + "' AND '" + cmbCustomerTo.Value.ToString() + "' )";
                    }
                }
               

                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsEst.DtCustomer);


                StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";

                if (intWarehouse == 1)
                {
                    if (chkWarehouse.Checked == false)
                    {
                        StrSql = StrSql + " WHERE (WhseId BETWEEN '" + cmbFromWarehouce.Value.ToString() + "' and '" + cmbToWarehouce.Value.ToString() + "')";
                    }
                }
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsEst.DtWhseMaster);

                StrSql = "SELECT CustomerID,JobID,JobDescription FROM tblJobMaster";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsEst.DtJob);


                StrSql = "SELECT JobID,ItemID,Qty,Amount FROM View_SalesInvoice";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsEst.DtInvoice);

                StrSql = "SELECT JobID,Quantity,UnitPrice FROM View_SalesOrder";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsEst.DtOrder);

                StrSql = "SELECT JobID,ItemID,Qty as ReturnQty,Amount FROM View_InvoiceReturn";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsEst.DtInvoiceReturn);

                StrSql = "SELECT ItemID,UOM FROM tblItemMaster";

                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsEst.DtItem);

                frmViewerItemWiseSales frmviewer = new frmViewerItemWiseSales(this);
                frmviewer.Show();


            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private Boolean ReportValidation(int intWarehouse,int intItem,int intCustomer,int intJob)
        {
            try
            {

                if (intWarehouse == 1)
                {
                    if (chkWarehouse.Checked == false)
                    {
                        if (cmbFromWarehouce.Text == string.Empty || cmbToWarehouce.Text == string.Empty)
                        {
                            return false;

                        }

                    }
                }

                if (intItem == 1)
                {
                    if (chkItem.Checked == false)
                    {
                        if (cmbItemFrom.Text == string.Empty || cmbItemTo.Text == string.Empty)
                        {
                            return false;
                        }

                    }
                }

                if (intCustomer == 1)
                {
                    if (chkCustomer.Checked == false)
                    {
                        if (cmbCustomerFrom.Text == string.Empty || cmbCustomerTo.Text == string.Empty)
                        {
                            return false;
                        }
                    }
                }

                if (intJob == 1)
                {
                    if (chkJob.Checked == false)
                    {
                        if (cmbJobFrom.Text == string.Empty || cmbJobTo.Text == string.Empty)
                        {
                            return false;
                        }
                    }
                }
              
                return true;

            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Error :" + ex.Message);
                return false;
            }
        }

        private void Print()
        {
                try
                {

                    switch (clsPara.RepNo)
                    {
                        case 7001:
                            if (ReportValidation(1, 1, 1, 1) == false)
                            {
                                return;
                            }
                            else
                            {
                                PrintAndFill(1, 1, 1, 1, 1, 1, 1, 1);
                            }
                            break;
                        
                        case 7002:
                            if (ReportValidation(0, 0, 1, 1) == false)
                            {
                                return;
                            }
                            else
                            {
                                PrintAndFill(1, 1, 1, 1, 1, 1, 1, 1);
                            }
                            break;

                        case 7003:
                            if (ReportValidation(0, 0, 1, 1) == false)
                            {
                                return;
                            }
                            else
                            {
                                PrintAndFill(1, 1, 1, 1, 1, 1, 1, 1);
                            }
                            break;
                        case 7004:
                            if (ReportValidation(0, 0, 1, 1) == false)
                            {
                                return;
                            }
                            else
                            {
                                PrintAndFill(1, 1, 1, 1, 1, 1, 1, 1);
                            }
                            break;
                        case 7005:
                            if (ReportValidation(1, 1, 1, 1) == false)
                            {
                                return;
                            }
                            else
                            {
                                PrintAndFill(1, 1, 1, 1, 1, 1, 1, 1);
                            }
                            break;
                        case 7006:
                            if (ReportValidation(0, 0, 0, 0) == false)
                            {
                                return;
                            }
                            else
                            {
                                PrintAndFill(1, 0, 0, 0, 0, 0, 1, 1);
                            }
                            break;

                        default :
                            {
                                return;
                            }

                    }

                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error :" + ex.Message);
                }

           
        }

        

       

        
       
       

    }
}