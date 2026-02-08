using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DataAccess;
using CrystalDecisions.CrystalReports.Engine;
using Infragistics.Win.UltraWinGrid;

namespace UserAutherization
{
    
    public partial class frmInvoiceSearch : Form
    {
        int IsFind = 0;
        clsCommon objclsCommon = new clsCommon();
        public static string ConnectionString;
        public DsItemWiseSales DsItemWise = new DsItemWiseSales();

        public static frmMasterInventory objfrmMasterInventory;


        public DataSet dsWarehouse;
        public DataSet dsCustomer;
        public DataSet dsItem;
        public DataSet dsItemType;
        public DataSet dsUser;

       public string CRInvoice = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoice.rpt";
        public string CRInvoiceNonVat = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceNonVat.rpt";
        public string CRInvoiceCreditNonVat = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceCreditNonVat.rpt";
        ReportDocument crp = new ReportDocument();
        
        public string StrSql;
        public frmInvoiceSearch()
        {
            InitializeComponent();
            setConnectionString();
          //  LoadDefault();
        }

        public frmInvoiceSearch(int _IsFind)
        {
            InitializeComponent();
            setConnectionString();
            IsFind = _IsFind;
          //  LoadDefault();
        }

        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }

        }

        private void frmInvoiceSearch_Load(object sender, EventArgs e)
        {

            try
            {
                txtVehicleNo.Focus();
                Load_IssueList();
                CRVCreditNoteList.Width = this.Width - (dgvSearchIssue.Width + 40);
                GetFromHouseDataSet();
                GetItemDataSet();
                SetItemBrand();
                SetItemType();
                GetJobDoneby();
                DataTable dt = new DataTable();
                dt.Columns.Add("Type");
                dt.Rows.Add("Save");
                dt.Rows.Add("Process");

                cmbType.DataSource = dt;


            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void GetJobDoneby()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT Description FROM tbl_ItemCustom6 order by ID";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cmbJobDone.DataSource = dt;
                    cmbJobDone.ValueMember = "Description";
                    cmbJobDone.DisplayMember = "Description";
                    cmbJobDone.DisplayLayout.Bands[0].Columns[0].Width = 120;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetItemType()
        {
            string StrSql = "select  Description from tbl_ItemType";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbItemType.DataSource = dt;
                cmbItemType.ValueMember = "Description";
                cmbItemType.DisplayMember = "Description";
                cmbItemType.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }
        }


        private void SetItemBrand()
        {
            string StrSql = "select  Description from tbl_ItemCustom3";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbBrand.DataSource = dt;
                cmbBrand.ValueMember = "Description";
                cmbBrand.DisplayMember = "Description";

                cmbBrand.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }

        }


        public void GetItemDataSet()
        {
            dsItem = new DataSet();
            try
            {
                dsItem.Clear();
                StrSql = "SELECT ItemID,ItemDescription,Categoty FROM tblItemMaster order by ItemID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItem, "DtItem");

                cmbItem.DataSource = dsItem.Tables["DtItem"];
                cmbItem.DisplayMember = "ItemDescription";
                cmbItem.ValueMember = "ItemDescription";
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemID"].Hidden =true;
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["Categoty"].Hidden = true;
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDescription"].Width = 800;
                //cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDescription"].Width = 300;
                //cmbItem.DisplayLayout.Bands["DtItem"].Columns["Categoty"].Width = 75;
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
        private void LoadDefault()
        {
            try
            {

                Load_IssueList();

                string StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo=(select max(InvoiceNo) from tblSalesInvoices)";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                     // LoadLastReport(dt.Rows[0].ItemArray[0].ToString());

                }

                string StrSql2 = "SELECT min(InvoiceDate),max(InvoiceDate) FROM tblSalesInvoices";
                SqlCommand cmd2 = new SqlCommand(StrSql2);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    dtpFromDate.Value = Convert.ToDateTime(dt2.Rows[0].ItemArray[0].ToString());
                    dtpSearchDate.Value = Convert.ToDateTime(dt2.Rows[0].ItemArray[1].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Customer Invoice List", ex.Message);
            }
        }

        private void LoadLastReport(string invno)
        {
         

            string StrSql;
            DsItemWise.Clear();
            StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + invno + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsItemWise.dtSalesInvoice);

            StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.DsItem);

            StrSql = "SELECT CutomerID,CustomerName,Address1,Address2,Custom1,Custom2,Custom3,Custom4,Fax,Phone1  FROM tblCustomerMaster where CutomerID='" + DsItemWise.Tables["dtSalesInvoice"].Rows[0]["CustomerID"].ToString() + "'";// where CutomerID='" + cmbCustomer.Text.ToString().Trim() + "'";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.DsCustomer);

            StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.DsWarehouse);

            StrSql = "SELECT * FROM tblInvoicePaymentHistory WHERE InvoiceNo='" + invno + "'";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.tblInvoicePaymentHistory);

            StrSql = "SELECT * FROM tblInvoicePayTypes WHERE InvoiceNo='" + invno + "'";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.tblpaytran);

            StrSql = "SELECT * FROM tblSalesRep ";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.tblSalesRep);

            StrSql = "SELECT * FROM tblCompanyInformation ";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.DtCompaniInfo);


            if (Convert.ToDouble(DsItemWise.dtSalesInvoice.Rows[0].ItemArray[15].ToString()) > 0)
            {
                crp.Load(CRInvoice);
            }
            else if (DsItemWise.dtSalesInvoice.Rows[0].ItemArray[36].ToString()!= "Cash")
            {
                crp.Load(CRInvoiceNonVat);
            }
            else
            {
                crp.Load(CRInvoiceCreditNonVat);
            }
            crp.SetDataSource(DsItemWise);
            CRVCreditNoteList.ReportSource = crp;
            CRVCreditNoteList.Zoom(70);
        }
        private void LoadReport(string invno)
        {


            string StrSql;
            DsItemWise.Clear();
            StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + invno + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsItemWise.dtSalesInvoice);

            StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.DsItem);

            StrSql = "SELECT CutomerID,CustomerName,Address1,Address2,Custom1,Custom2,Custom3,Custom4,Fax,Phone1  FROM tblCustomerMaster  where CutomerID='" + DsItemWise.Tables["dtSalesInvoice"].Rows[0]["CustomerID"].ToString() + "'";// where CutomerID='" + cmbCustomer.Text.ToString().Trim() + "'";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.DsCustomer);

            StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.DsWarehouse);

            StrSql = "SELECT * FROM tblInvoicePaymentHistory WHERE InvoiceNo='" + invno + "'";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.tblInvoicePaymentHistory);

            StrSql = "SELECT * FROM tblInvoicePayTypes WHERE InvoiceNo='" + invno + "'";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.tblpaytran);

            StrSql = "SELECT * FROM tblSalesRep ";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.tblSalesRep);

            StrSql = "SELECT * FROM tblCompanyInformation ";
            cmd = new SqlCommand(StrSql);
            da = new SqlDataAdapter(StrSql, ConnectionString);
            dt = new DataTable();
            da.Fill(DsItemWise.DtCompaniInfo);


            if (Convert.ToDouble(DsItemWise.dtSalesInvoice.Rows[0].ItemArray[15].ToString()) > 0)
            {
                crp.Load(CRInvoice);
            }
            else if (DsItemWise.dtSalesInvoice.Rows[0].ItemArray[36].ToString() == "Cash")
            {
                crp.Load(CRInvoiceNonVat);
            }
            else
            {
                crp.Load(CRInvoiceCreditNonVat);
            }

            crp.SetDataSource(DsItemWise);
            CRVCreditNoteList.ReportSource = crp;
            CRVCreditNoteList.Zoom(70);
        }

        private void Load_IssueList()
        {
            try
            {
                string s = "select distinct   top 50 InvoiceNo,VehicleNo,InvoiceDate,SONO,'',IsConfirm,IsVoid from tblSalesInvoices where IsDirect='True' order by  InvoiceDate desc,InvoiceNo DESC";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSearchIssue.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchIssue.Rows.Add();
                        dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvSearchIssue.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsConfirm"].ToString().Trim()) == false)
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvSearchIssue.Rows[i].Cells[5].Value = "Save";
                        }
                        else
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            dgvSearchIssue.Rows[i].Cells[5].Value = "Process";

                        }

                        if (Convert.ToBoolean(dt.Rows[i]["IsVoid"].ToString().Trim()) == true)
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SeachOption()
        {
            try
            {
                dgvSearchIssue.Rows.Clear();
                string status = "";

              
                 if (cmbType.Text == "Save")
                {
                    status = "0";
                }
                else if (cmbType.Text == "Process")
                {
                    status = "1";
                }
                double option;
                if(checkBox1.Checked == true)
                {
                    option = 1;
                }
                else
                {
                    option = 0;
                }

                string PM="";
                if(optCash.Checked ==true)
                {
                    PM = optCash.Text;
                }
                else if(optCredit.Checked ==true)
                {
                    PM = optCredit.Text;
                }
                else if(rdobtnCreditCard.Checked ==true)
                {
                    PM = rdobtnCreditCard.Text;
                }

                string Vat = "";
                if(rbtVAT.Checked==true)
                {
                    Vat = "2";
                }
                else if(rbtNoVat.Checked ==true)
                {
                    Vat = "1";
                }
                string s = "";
                if (cmbBrand.Text.Trim() == "" && cmbItemType.Text.Trim() == "")
                {
                     s = "select  distinct top 1000 InvoiceNo,VehicleNo,InvoiceDate,SONO,'',IsConfirm,IsVoid from tblSalesInvoices where  IsDirect='True' and InvType like '" + Vat + "%' and  VehicleNo like '" + txtVehicleNo.Text.Trim() + "%' and  CustomerName like '" + txtCustomerName.Text.Trim() + "%' and Location like '" + cmbWH.Text.Trim() + "%' and Location like '%" + cmbWH.Text.Trim() + "' and Description like '%" + cmbItem.Text.Trim() + "%' and    Convert(varchar,IsConfirm) like  '%" + status + "%' and JobDoneBy like  '%" + cmbJobDone.Text.Trim() + "%'   and  InvoiceDate <= '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' and InvoiceDate >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and   FOCQty >='" + option + "' and PaymentM like '"+PM+ "%' and   InvoiceNo like '%" + txtTotal.Text + "' order by InvoiceDate desc,InvoiceNo DESC";
                }
                else if (cmbItemType.Text.Trim()!=""&&cmbBrand.Text.Trim()!="")
                {
                     s = "select  distinct top 1000 InvoiceNo,VehicleNo,InvoiceDate,SONO,'',IsConfirm,IsVoid from tblSalesInvoices where  IsDirect='True' and InvType like '" + Vat + "%' and  VehicleNo like '" + txtVehicleNo.Text.Trim() + "%' and  CustomerName like '" + txtCustomerName.Text.Trim() + "%' and Location like '" + cmbWH.Text.Trim() + "%' and Location like '%" + cmbWH.Text.Trim() + "' and Description like '%" + cmbItem.Text.Trim() + "%' and    Convert(varchar,IsConfirm) like  '%" + status + "%' and JobDoneBy like  '%" + cmbJobDone.Text.Trim() + "%'  and  InvoiceDate <= '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' and InvoiceDate >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and FOCQty >='" + option + "' and ItemID  in(select ItemID from tblItemMaster where Custom3 = '" + cmbBrand.Text.ToString() + "') and ItemID  in(select ItemID from tblItemMaster where Categoty = '" + cmbItemType.Text.ToString() + "') and PaymentM like '" + PM + "%' and  InvoiceNo like '%" + txtTotal.Text + "' order by InvoiceDate desc,InvoiceNo DESC";
                }
                else if(cmbBrand.Text!="")
                {
                     s = "select distinct   top 1000 InvoiceNo,VehicleNo,InvoiceDate,SONO,'',IsConfirm,IsVoid from tblSalesInvoices where  IsDirect='True' and InvType like '" + Vat + "%' and  VehicleNo like '" + txtVehicleNo.Text.Trim() + "%' and  CustomerName like '" + txtCustomerName.Text.Trim() + "%' and Location like '" + cmbWH.Text.Trim() + "%' and Location like '%" + cmbWH.Text.Trim() + "' and Description like '%" + cmbItem.Text.Trim() + "%' and    Convert(varchar,IsConfirm) like  '%" + status + "%' and JobDoneBy like  '%" + cmbJobDone.Text.Trim() + "%' and  InvoiceDate <= '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' and InvoiceDate >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and FOCQty  >='" + option + "' and ItemID  in(select ItemID from tblItemMaster where Custom3 = '" + cmbBrand.Text.ToString() + "') and PaymentM like '" + PM + "%' and  InvoiceNo like '%" + txtTotal.Text + "' order by InvoiceDate desc,InvoiceNo DESC";

                }
                else if(cmbItemType.Text !="")
                {
                     s = "select  distinct top 1000 InvoiceNo,VehicleNo,InvoiceDate,SONO,'',IsConfirm,IsVoid from tblSalesInvoices where  IsDirect='True' and InvType like '" + Vat + "%' and  VehicleNo like '" + txtVehicleNo.Text.Trim() + "%' and  CustomerName like '" + txtCustomerName.Text.Trim() + "%' and Location like '" + cmbWH.Text.Trim() + "%' and Location like '%" + cmbWH.Text.Trim() + "' and Description like '%" + cmbItem.Text.Trim() + "%' and    Convert(varchar,IsConfirm) like  '%" + status + "%' and JobDoneBy like  '%" + cmbJobDone.Text.Trim() + "%' and  InvoiceDate <= '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' and InvoiceDate >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  FOCQty >='" + option + "' and ItemID  in(select ItemID from tblItemMaster where Categoty = '" + cmbItemType.Text.ToString() + "') and PaymentM like '" + PM + "%' and InvoiceNo like '%" + txtTotal.Text + "' order by InvoiceDate desc,InvoiceNo DESC";

                }
                dgvSearchIssue.Rows.Clear();
                        SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);                        
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvSearchIssue.Rows.Add();
                                dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvSearchIssue.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            if (Convert.ToBoolean(dt.Rows[i]["IsConfirm"].ToString().Trim()) == false)
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                dgvSearchIssue.Rows[i].Cells[5].Value = "Save";
                            }
                            else
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                dgvSearchIssue.Rows[i].Cells[5].Value = "Process";

                            }

                        if (Convert.ToBoolean(dt.Rows[i]["IsVoid"].ToString().Trim()) == true)
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        }
                    }
                                          
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                SeachOption();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvSearchIssue.Rows.Clear();
                Load_IssueList();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

       

        private void dgvSearchIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
          



        }

      

        private void dgvSearchIssue_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void printPreviewControl1_Click(object sender, EventArgs e)
        {

        }

        private void dgvSearchIssue_Click(object sender, EventArgs e)
        {

           
        }

        private void printPreviewControl1_Click_1(object sender, EventArgs e)
        {

        }

        private void dgvSearchIssue_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvSearchIssue_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSearchIssue_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dgvSearchIssue_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSearchIssue_CellContentClick_3(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dgvSearchIssue_CellContentClick_4(object sender, DataGridViewCellEventArgs e)
        {
           
             
            
        }

        private void dgvSearchIssue_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            try
            {

                if(e.RowIndex==-1||dgvSearchIssue.Rows[e.RowIndex].Cells[e.ColumnIndex].Value==null)
                {
                    return;
                }
                if (e.Button == MouseButtons.Left)
                {
                    Point pt = dgvSearchIssue.PointToScreen(dgvSearchIssue.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location);
                    cmbDirInvoiceCS.Show(pt);
                }
            }
            catch
            {

            }


           
               
            

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
           
           
            

        }

        private void cmbDirInvoiceCS_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "View")
            {
                if ((dgvSearchIssue["InvoiceNo", dgvSearchIssue.CurrentRow.Index].Value.ToString().Trim()) != "")
                {
                    string searchIssueNoteNo = dgvSearchIssue["InvoiceNo", dgvSearchIssue.CurrentRow.Index].Value.ToString().Trim();
                    LoadReport(searchIssueNoteNo);
                }
            }

            else if (e.ClickedItem.Text == "Open")
            {
                try
                {
                    if ((dgvSearchIssue["InvoiceNo", dgvSearchIssue.CurrentRow.Index].Value.ToString().Trim()) != "")
                    {
                        Search.searchIssueNoteNo = dgvSearchIssue["InvoiceNo", dgvSearchIssue.CurrentRow.Index].Value.ToString().Trim();
                    }

                    if (user.IsDirectINVEnbl)
                    {
                        if (IsFind == 1)
                        {
                            this.Close();
                            frmMain.ObjInvoices.Close();
                            frmMain.ObjInvoices = new frmInvoices(Search.searchIssueNoteNo);
                            frmMain.ObjInvoices.Show();
                            //frmMain.ObjInvoices.TopMost = true;
                        }
                        else
                        {
                            this.Close();
                            if (frmMain.ObjInvoices == null || frmMain.ObjInvoices.IsDisposed)
                            {
                                frmMain.ObjInvoices = new frmInvoices(Search.searchIssueNoteNo);
                            }
                            frmMain.ObjInvoices.Show();
                            //frmMain.ObjInvoices.TopMost = true;
                            frmMain.ObjInvoices.WindowState = FormWindowState.Normal;
                            frmMain.objfrmInvoiceSearch.TopMost = false;
                        }
                    }
                    else if (user.IsPMINVEnbl)
                    {
                        if (IsFind == 1)
                        {
                            this.Close();
                            frmMain.objfrmInvoiceMetrics.Close();
                            frmMain.objfrmInvoiceMetrics = new frmInvoiceMetrics(Search.searchIssueNoteNo);
                            frmMain.objfrmInvoiceMetrics.Show();
                        }
                        else
                        {
                            this.Close();
                            if (frmMain.objfrmInvoiceMetrics == null || frmMain.objfrmInvoiceMetrics.IsDisposed)
                            {
                                frmMain.objfrmInvoiceMetrics = new frmInvoiceMetrics(Search.searchIssueNoteNo);
                            }
                            frmMain.objfrmInvoiceMetrics.Show();
                            frmMain.objfrmInvoiceMetrics.TopMost = true;
                            frmMain.objfrmInvoiceMetrics.WindowState = FormWindowState.Normal;
                            frmMain.objfrmInvoiceSearch.TopMost = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
                }
            }
        }

        private void grpPayment_Enter(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SeachOption();
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {

            try
            {

                txtVehicleNo.Text = "";
                txtCustomerName.Text = "";
                cmbWH.Text = "";
                cmbBrand.Text = "";
                cmbItem.Text = "";
                cmbItemType.Text = "";
                cmbType.Text = "";
                cmbJobDone.Text = "";
                txtTotal.Text = "";

                rdobtnCreditCard.Checked = false;
                optCash.Checked = false;
                optCredit.Checked = false;
                checkBox1.Checked = false;


                LoadDefault();
                Load_IssueList();           

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvSearchIssue_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dgvSearchIssue_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                try
                {

                    if (dgvSearchIssue.CurrentRow.Index == -1 || dgvSearchIssue.Rows[dgvSearchIssue.CurrentRow.Index].Cells[dgvSearchIssue.CurrentCell.ColumnIndex].Value == null)
                    {
                        return;
                    }
                  
                        Point pt = dgvSearchIssue.PointToScreen(dgvSearchIssue.GetCellDisplayRectangle(dgvSearchIssue.CurrentCell.ColumnIndex, dgvSearchIssue.CurrentRow.Index, false).Location);
                        cmbDirInvoiceCS.Show(pt);
                    
                }
                catch
                {

                }
            }
        }

        private void btnSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(null, null);

            }
        }

        private void btnClear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnClear_Click_1(null, null);

            }
        }

        private void checkBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if(checkBox1.Checked ==false)
                {
                    checkBox1.Checked = true;
                }
                else
                {
                    checkBox1.Checked = false;
                }
            }
        }

        private void cmbWH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbWH.Enabled == true)
                {
                    cmbWH.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void cmbItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbItem.Enabled == true)
                {
                    cmbItem.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void cmbBrand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbBrand.Enabled == true)
                {
                    cmbBrand.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void cmbItemType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbItemType.Enabled == true)
                {
                    cmbItemType.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void cmbType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbType.Enabled == true)
                {
                    cmbType.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void cmbJobDone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbJobDone.Enabled == true)
                {
                    cmbJobDone.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void frmInvoiceSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
            {
                this.Close();

            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Down)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

   
    }
}