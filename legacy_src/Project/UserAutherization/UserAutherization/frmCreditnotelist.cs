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

namespace UserAutherization
{
    public partial class frmCreditnotelist : Form
    {
        public frmCreditnotelist()
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

        public string StrSql;


        public static string ConnectionString;
        public DTInvoiceData dsCreditNote = new DTInvoiceData();
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
                cmbCustomer.ValueMember = "CutomerID";
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["CutomerID"].Width = 150;
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["CustomerName"].Width = 300;
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
        private void frmCreditnotelist_Load(object sender, EventArgs e)
        {
            try
            {
                GetCustomerDataset();
                GetFromHouseDataSet();
            }
            catch { }

        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                string Customer = "";
                string Item = "";
                string WareHouse = "";


                if (cmbCustomer.Value != null) Customer = cmbCustomer.Value.ToString().Trim();
                if (cmbWH.Value != null) WareHouse = cmbWH.Value.ToString().Trim();

                dsCreditNote.Clear();

                String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(dsCreditNote, "DTCustomerMaster");

                String StrSql = "SELECT * FROM tblCompanyInformation";
                SqlDataAdapter da251 = new SqlDataAdapter(StrSql, ConnectionString);
                da251.Fill(dsCreditNote, "DTCompany");

                string sSQL = "Select DISTINCT CreditNo, ReturnDate, LocationID, InvoiceNO, NBT, VAT, GrossTotal, GrandTotal, CustomerID  from tblCutomerReturn where [ReturnDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [ReturnDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +                    
                    " and CustomerID like '%'+'" + Customer + "'" +
                    " and LocationID like '%" + WareHouse + "' and LocationID like '" + WareHouse + "%'";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(dsCreditNote, "DtsCreditNote");

                frmVieverCreditNotelList ObjfrmVieverCreditNotelList = new frmVieverCreditNotelList(this);
                ObjfrmVieverCreditNotelList.Show();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
    }
}