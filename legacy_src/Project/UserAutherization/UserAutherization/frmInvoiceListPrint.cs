using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;
using System.Xml;
using System.Collections;
using System.Threading;
using CrystalDecisions.Shared;

namespace UserAutherization
{
    public partial class frmInvoiceListPrint : Form
    {
        public int PrintType;
        public frmInvoiceListPrint()
        {

            InitializeComponent();
            setConnectionString();
        }
        clsCommon objclsCommon = new clsCommon();
        private string _msgTitle = "Report";

        public DataSet dsWarehouse;
        public DataSet dsCustomer;
        public DataSet dsItem;
        public DataSet dsItemType;
        public DataSet dsUser;

        public string StrSql;


        public static string ConnectionString;
        public DTInvoiceData DSInvoicing = new DTInvoiceData();
        public DSInvoiceSummary Dsinvc = new DSInvoiceSummary();
        public DSMultiPayment DSM = new DSMultiPayment();
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        public void GetCustomerDataset()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT CutomerID,CustomerName FROM tblCustomerMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtCustomer");

                cmbCustomer.DataSource = dsCustomer.Tables["DtCustomer"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CustomerName";
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
                StrSql = "SELECT UserID FROM Login";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsUser, "DtUser");

                cmbUser.DataSource = dsUser.Tables["DtUser"];
                cmbUser.DisplayMember = "UserID";
                cmbUser.ValueMember = "UserID";
                cmbUser.DisplayLayout.Bands["DtUser"].Columns["UserID"].Width = 150;
                //cmbUser.DisplayLayout.Bands["DtCustomer"].Columns["CustomerName"].Width = 300;
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
                StrSql = "SELECT distinct Categoty FROM tblItemMaster where [Categoty]<> ''";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItemType, "dtType");

                cmbItemType.DataSource = dsItemType.Tables["dtType"];
                cmbItemType.DisplayMember = "Categoty";
                cmbItemType.ValueMember = "Categoty";
                cmbItemType.DisplayLayout.Bands["dtType"].Columns["Categoty"].Width = 150;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void GetItemDataSet()
        {
            try
            {
                string StrSql = "SELECT ItemID,ItemDescription,Categoty FROM tblItemMaster order by ItemDescription";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbItem.DataSource = dt;
                cmbItem.DisplayMember = "ItemDescription";
                cmbItem.ValueMember = "ItemDescription";
                cmbItem.DisplayLayout.Bands[0].Columns["ItemID"].Width = 200;
                cmbItem.DisplayLayout.Bands[0].Columns["ItemDescription"].Width = 300;
                cmbItem.DisplayLayout.Bands[0].Columns["Categoty"].Width = 75;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetFromHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWH.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWH.DisplayMember = "WhseId";
                cmbWH.ValueMember = "WhseId";
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 50;
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 150;
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["ArAccount"].Hidden = true;
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["CashAccount"].Hidden = true;
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["SalesGLAccount"].Hidden = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void frmInvoiceListPrint_Load(object sender, EventArgs e)
        {
            GetFromHouseDataSet();
            GetItemDataSet();
            GetCustomerDataset();
            GetItemTypeDataSet();
            GetUserDataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Type");
            dt.Rows.Add("All");
            dt.Rows.Add("Save");
            dt.Rows.Add("Process");

            cmbType.Text = "All";
            cmbType.DataSource = dt;
         
        }


        private void btnView_Click(object sender, EventArgs e)
        {
            string PM = "";
            if (optCash.Checked == true)
            {
                PM = optCash.Text;
            }
            else if (optCredit.Checked == true)
            {
                PM = optCredit.Text;
            }
            else if (rdobtnCreditCard.Checked == true)
            {
                PM = rdobtnCreditCard.Text;
            }

            string status = "";
       
            if (cmbType.Text == "All")
            {
                status = "";
            }
            else if (cmbType.Text == "Save")
            {
                status = "0";
            }
            else if (cmbType.Text == "Process")
            {
                status = "1";
            }
            try
            {
                string Customer = "";
                string Item = "";
                string WareHouse = "";
                string StrUser = string.Empty;


                if (cmbItem.Value != null) Item = cmbItem.Value.ToString().Trim();
                if (cmbCustomer.Value != null) Customer = cmbCustomer.Value.ToString().Trim();
                if (cmbWH.Value != null) WareHouse = cmbWH.Value.ToString().Trim();
                if (cmbUser.Value != null) StrUser = cmbUser.Value.ToString().Trim();

                //String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                //SqlCommand cmd1 = new SqlCommand(S1);
                //SqlConnection con1 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                //da1.Fill(DSInvoicing, "DTCustomerMaster");


                //string sSQL = "Select * from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'"+
                //    " and ItemID like '%'+'" + Item + "'"+
                //    " and CustomerID like '%'+'" + Customer + "'" +
                //    " and Location like '%'+'" + WareHouse + "'";              

                //SqlCommand cmd3 = new SqlCommand(sSQL);
                //SqlConnection con3 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                //da3.Fill(DSInvoicing, "DTInvoiceTransaction");

            
                if (OptDetail.Checked == true)
                {
                    DSInvoicing.Clear();                   

                  
                    String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(DSInvoicing, "DTCustomerMaster");

                    string sSQL = "Select * from tblSalesInvoices  where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                        " and Description like '%'+'" + Item + "'" +
                        " and CusName like '%'+'" + Customer + "'" +
                       " and PaymentM like '%'+'" + PM + "'" +
                        " and WHID like '%" + WareHouse + "' and WHID like '" + WareHouse + "%' " +
                    " and Currentuser like '%'+'" + StrUser + "' and  Convert(varchar,IsConfirm) like  '%" + status + "%' and VehicleNo like  '%" + txtVehicleNo.Text.Trim() + "%' and IsVoid='False' and InvoiceNo not in(select InvoiceNo from tblInvoicePayTypes  group by InvoiceNo having count(*)>1)";

 


                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                    da3.Fill(DSInvoicing, "DTInvoiceTransaction");
                    PrintType = 1;
                }
                if (optsummary.Checked == true)
                {
                    Dsinvc.Clear();
                    string sSQL = "Select * from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                        " and Description like '%'+'" + Item + "'" +
                        " and CusName like '%'+'" + Customer + "'" +
                          " and PaymentM like '%'+'" + PM + "'" +
                        " and WHID like '%" + WareHouse + "' and WHID like '" + WareHouse + "%' " +
                    " and Currentuser like '%'+'" + StrUser + "' and Convert(varchar,IsConfirm) like  '%" + status + "%' and VehicleNo like  '%" + txtVehicleNo.Text.Trim() + "%' and IsVoid='False'";

                   

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                    da3.Fill(Dsinvc, "SalesInvoice");
                    PrintType = 2;
                }
                if (rbFOC.Checked == true)
                {
                    DSInvoicing.Clear();

                    String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(DSInvoicing, "DTCustomerMaster");

                    string sSQL = "Select * from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                        " and Description like '%'+'" + Item + "'" +
                        " and CusName like '%'+'" + Customer + "'" +
                          " and PaymentM like '%'+'" + PM + "'" +
                        " and WHID like '%" + WareHouse + "' and WHID like '" + WareHouse + "%' " +
                    " and Currentuser like '%'+'" + StrUser + "' and Convert(varchar,IsConfirm) like  '%" + status + "%' and VehicleNo like  '%" + txtVehicleNo.Text.Trim() + "%' and FOCQty >0 and IsVoid='False'";

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                    da3.Fill(DSInvoicing, "DTInvoiceTransaction");
                    PrintType = 3;
                }


                if(rbMuliplePayment.Checked ==true)
                {
                    DSM.Clear();


                    String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(DSM.DsCustomer);

                    string sSQL = "Select * from tblInvoicePayTypes  where [InvDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                         " and  InvoiceNo  in(select InvoiceNo from tblSalesInvoices where Description like '%'+'" + Item + "')"+
                        " and InvoiceNo  in(select InvoiceNo from tblSalesInvoices where CusName like '%' + '" + Customer + "')"+
                          " and  InvoiceNo  in(select InvoiceNo from tblSalesInvoices where PaymentM like '%' + '" + PM + "')"+
                        " and InvoiceNo  in(select InvoiceNo from tblSalesInvoices where WHID like '%" + WareHouse + "' and WHID like '" + WareHouse + "%')" +
                    " and InvoiceNo  in(select InvoiceNo from tblSalesInvoices where UserID like '%'+'" + StrUser + "') and InvoiceNo  in(select InvoiceNo from tblSalesInvoices where IsVoid='False') and InvoiceNo  in(select InvoiceNo from tblInvoicePayTypes group by InvoiceNo having count(*)>1)";


                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                    da3.Fill(DSM.dtMultiplePayment);
                    PrintType = 4;

                }

                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing, "DTCompany");
                da4.Fill(Dsinvc, "DTCompany");
                da4.Fill(DSM.DtCompaniInfo);


                string Cname = ""; 
                if(cmbWH.Text.ToString().Trim()== "Lasantha TYRE TRADERS")
                {
                    Cname = "LASANTHA TYRE TRADERS";
                }
                else if (cmbWH.Text.ToString().Trim() != "")
                {
                    Cname = "NEW LASANTHA TYRE TRADERS";
                }
                else
                {
                    Cname = "LASANTHA TYRE TRADERS AND NEW LASANTHA TYRE TRADERS";
                }


               

                frmMaxInvoicelistViewer ObjViwerInvPrint = new frmMaxInvoicelistViewer(this, Cname);
                ObjViwerInvPrint.Show();

            }



            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void chkcusAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkcusAll.Checked)
            {
                cmbCustomer.Text = "";
                cmbCustomer.Enabled = false;
            }
            else
            {
                cmbCustomer.Enabled = true;
            }
        }

        private void chkItemAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkItemAll.Checked)
            {
                cmbItem.Text = "";
                cmbItem.Enabled = false;
            }
            else
            {
                cmbItem.Enabled = true;
            }

        }

        private void chkWHAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWHAll.Checked)
            {
                cmbWH.Text = "";
                cmbWH.Enabled = false;
            }
            else
            {
                cmbWH.Enabled = true;
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked ==true)
            {
                optCash.Checked = false;
                optCredit.Checked = false;
                rdobtnCreditCard.Checked = false;
            }
        }

        private void optCredit_CheckedChanged(object sender, EventArgs e)
        {
            if(optCredit.Checked==true)
            {
                checkBox2.Checked = false;
            }

        }

        private void optCash_CheckedChanged(object sender, EventArgs e)
        {
            if (optCash.Checked == true)
            {
                checkBox2.Checked = false;
            }
        }

        private void rdobtnCreditCard_CheckedChanged(object sender, EventArgs e)
        {
            if (rdobtnCreditCard.Checked == true)
            {
                checkBox2.Checked = false;
            }
        }
    }
}