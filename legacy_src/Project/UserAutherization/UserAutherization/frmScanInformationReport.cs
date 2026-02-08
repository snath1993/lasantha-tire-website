using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmScanInformationReport : Form
    {
        private string ConnectionString;
        private DataSet dsDr;
        private DataSet dsUser;
        private DataSet dsCustomer;
        private DataSet dsItemType;
        private DataSet dsItem;
        public int PrintType;

        public frmScanInformationReport()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void frmScanInformationReport_Load(object sender, EventArgs e)
        {
            GetUserDataSet();
            GetRefferedDr();
            GetCustomerDataset();
            GetItemTypeDataSet();

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


        public void GetRefferedDr()
        {
            dsDr = new DataSet();
            try
            {
                dsDr.Clear();
                string StrSql = "SELECT [Name],[Type] FROM [tblDoctorMaster]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsDr, "DtDr");

                cmbReferredDr.DataSource = dsDr.Tables["DtDr"];
                cmbReferredDr.DisplayMember = "Name";
                cmbReferredDr.ValueMember = "Name";

                cmbReferredDr.DisplayLayout.Bands["DtDr"].Columns["Name"].Width = 200;
                cmbReferredDr.DisplayLayout.Bands["DtDr"].Columns["Type"].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetItemTypeDataSet()
        {
            dsItemType = new DataSet();
            try
            {
                dsItemType.Clear();
                string StrSql = "SELECT [TypeName]FROM [tblPatientMasterType]";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItemType, "dtType");

                cmbTT.DataSource = dsItemType.Tables["dtType"];
                cmbTT.DisplayMember = "TypeName";
                cmbTT.ValueMember = "TypeName";
                cmbTT.DisplayLayout.Bands["dtType"].Columns["TypeName"].Width = 150;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetCustomerDataset()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                string StrSql = "SELECT CutomerID,CustomerName FROM tblCustomerMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtCustomer");

                cmbCustomer.DataSource = dsCustomer.Tables["DtCustomer"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CutomerID";
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["CutomerID"].Width = 150;
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["CustomerName"].Width = 300;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetUserDataSet()
        {
            dsUser = new DataSet();
            try
            {
                dsUser.Clear();
                string StrSql = "SELECT UserID FROM Login";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsUser, "DtUser");

                cmbUser.DataSource = dsUser.Tables["DtUser"];
                cmbUser.DisplayMember = "UserID";
                cmbUser.ValueMember = "UserID";
                cmbUser.DisplayLayout.Bands["DtUser"].Columns["UserID"].Width = 150;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public dsScanChanel DSInvoicing = new dsScanChanel();

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                string Customer = "";
                string Item = "";
                string WareHouse = "";
                string StrUser = string.Empty;
                string StrDr = string.Empty;

                if (cmbTT.Value != null) Item = cmbTT.Value.ToString().Trim();
                if (cmbCustomer.Value != null) Customer = cmbCustomer.Value.ToString().Trim();
                if (cmbUser.Value != null) StrUser = cmbUser.Value.ToString().Trim();
                if (cmbReferredDr.Value != null) StrDr = cmbReferredDr.Value.ToString().Trim();


              


                if (cbincludeTime.Checked == true)
                {
                    
                    if (OptDetail.Checked == true)
                    {
                        DSInvoicing.Clear();

                        string sSQL = "Select * from [tblScanChannel] where ([Date]+[CollectTime] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + " " + dtpfrom.Value.ToString("HH:mm:ss") + "')AND([Date]+[CollectTime]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + " " + dtpto.Value.ToString("HH:mm:ss") + "')" +
                                    " and [ItemType] like '%'+'" + Item + "'" +
                                    " and PatientNo like '%'+'" + Customer + "'" +
                                    " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                                    " and [CurrentUser] like '%'+'" + StrUser + "'";

                        SqlCommand cmd3 = new SqlCommand(sSQL);
                        SqlConnection con3 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                        da3.Fill(DSInvoicing, "tblScanChannel");

                        DSInvoicing.tblTempTime.Rows.Clear();
                        DSInvoicing.tblTempTime.Rows.Add(dtpfrom.Value.ToString("HH:mm:ss"), dtpto.Value.ToString("HH:mm:ss"),dtpFromDate.Value.ToShortDateString(),dtpToDate.Value.ToShortDateString());

                      

                        PrintType = 2;
                    }

                    if (optsummary.Checked == true)
                    {
                        DSInvoicing.Clear();

                        string sSQL = "Select * from [tblScanChannel] where ([Date]+[CollectTime] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + " " + dtpfrom.Value.ToString("HH:mm:ss") + "')AND([Date]+[CollectTime]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + " " + dtpto.Value.ToString("HH:mm:ss") + "')" +
                            " and [ItemType] like '%'+'" + Item + "'" +
                            " and PatientNo like '%'+'" + Customer + "'" +
                             " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                        " and [CurrentUser] like '%'+'" + StrUser + "'";

                        SqlCommand cmd3 = new SqlCommand(sSQL);
                        SqlConnection con3 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                        da3.Fill(DSInvoicing, "tblScanChannel");

                        DSInvoicing.tblTempTime.Rows.Clear();
                        DSInvoicing.tblTempTime.Rows.Add(dtpfrom.Value.ToString("HH:mm:ss"), dtpto.Value.ToString("HH:mm:ss"), dtpFromDate.Value.ToShortDateString(), dtpToDate.Value.ToShortDateString());

                     
                        PrintType = 1;
                    }
                }
                else
                {
                  

                    if (OptDetail.Checked == true)
                    {
                        DSInvoicing.Clear();

                        string sSQL = "Select * from [tblScanChannel] where [Date] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [Date]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                            " and [ItemType] like '%'+'" + Item + "'" +
                            " and PatientNo like '%'+'" + Customer + "'" +
                             " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                        " and [CurrentUser] like '%'+'" + StrUser + "'";

                        SqlCommand cmd3 = new SqlCommand(sSQL);
                        SqlConnection con3 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                        da3.Fill(DSInvoicing, "tblScanChannel");

                        DSInvoicing.tblTempTime.Rows.Clear();
                        DSInvoicing.tblTempTime.Rows.Add("00:00:00.0000", "00:00:00.0000",dtpFromDate.Value.ToShortDateString(), dtpToDate.Value.ToShortDateString());

                     
                        PrintType = 2;
                    }

                    if (optsummary.Checked == true)
                    {
                        DSInvoicing.Clear();

                        string sSQL = "Select * from [tblScanChannel] where [Date] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [Date]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                            " and [ItemType] like '%'+'" + Item + "'" +
                            " and PatientNo like '%'+'" + Customer + "'" +
                             " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                        " and [CurrentUser] like '%'+'" + StrUser + "'";

                        SqlCommand cmd3 = new SqlCommand(sSQL);
                        SqlConnection con3 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                        da3.Fill(DSInvoicing, "tblScanChannel");

                        DSInvoicing.tblTempTime.Rows.Clear();
                        DSInvoicing.tblTempTime.Rows.Add("00:00:00.0000", "00:00:00.0000",dtpFromDate.Value.ToShortDateString(), dtpToDate.Value.ToShortDateString());
                        
                        PrintType = 1;
                    }
                }
              

                frmScanChanalingSummery ObjViwerInvPrint = new frmScanChanalingSummery(this);
                ObjViwerInvPrint.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkWHAll_CheckedChanged(object sender, EventArgs e)
        {
            cmbTT.Text = string.Empty;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            cmbUser.Text = string.Empty;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbCustomer_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            chkcusAll.Checked = false;
        }

        private void cmbTT_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            chkWHAll.Checked = false;
        }

        private void cmbUser_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            checkBox1.Checked = false;
        }

        private void chkcusAll_CheckedChanged(object sender, EventArgs e)
        {
            cmbCustomer.Text = "";
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (cbincludeTime.Checked == true)
            {
                dtpfrom.Enabled = true;
                dtpto.Enabled = true;
            }
            else
            {
                dtpto.Enabled = false;
                dtpfrom.Enabled = false;
            }
        }
    }
}
