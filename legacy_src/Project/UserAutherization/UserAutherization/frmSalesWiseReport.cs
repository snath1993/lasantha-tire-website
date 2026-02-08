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
    public partial class frmSalesWiseReport : Form
    {
        public static string ConnectionString;
        public DataSet dsItem, dsSerItem, dsCustomer,dsSalesRep;
        public string StrSql;
        public DataSet dsWarehouse;
        public DTSalesWise DSInvoicing = new DTSalesWise();
        public frmSalesWiseReport()
        {
            InitializeComponent();
            setConnectionString();
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void frmSalesWiseReport_Load(object sender, EventArgs e)
        {
            GetFromHouseDataSet();
            GetItemDateSet();
            SetItemBrand();
            SetItemCategory();
            SetItemCategory1();
            GetSerItemDateSet();
            GetJobDoneBy();
            GetSalesRep();
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
        private void GetJobDoneBy()
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

        private void GetSalesRep()
        {
            dsSalesRep = new DataSet();
            try
            {
                dsSalesRep.Clear();
              //  StrSql = "SELECT RepCode FROM tblSalesRep order by RepCode";
                StrSql = "SELECT distinct  [SalesRep] FROM [tblSalesInvoices]";
                //SELECT distinct  [SalesRep] FROM [tblSalesInvoices]

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cmbSalesRep.DataSource = dt;
                    cmbSalesRep.ValueMember = "SalesRep";
                    cmbSalesRep.DisplayMember = "SalesRep";
                    cmbSalesRep.DisplayLayout.Bands[0].Columns[0].Width = 120;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void GetSerItemDateSet()
        {
            try
            {
                string StrSql = "SELECT ItemID,ItemDescription,Categoty FROM tblItemMaster  where ItemClass='7' order by ItemDescription";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbServiceItem.DataSource = dt;
                cmbServiceItem.DisplayMember = "ItemDescription";
                cmbServiceItem.ValueMember = "ItemDescription";
                cmbServiceItem.DisplayLayout.Bands[0].Columns["ItemID"].Width = 200;
                cmbServiceItem.DisplayLayout.Bands[0].Columns["ItemDescription"].Width = 300;
                cmbServiceItem.DisplayLayout.Bands[0].Columns["Categoty"].Width = 75;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetItemCategory()
        {
            string StrSql = "select  Description from tbl_ItemType";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbCategory.DataSource = dt;
                cmbCategory.ValueMember = "Description";
                cmbCategory.DisplayMember = "Description";

                cmbCategory.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }
        }


        private void SetItemCategory1()
        {
            string StrSql = "select  Description from tbl_ItemType";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbItemTye.DataSource = dt;
                cmbItemTye.ValueMember = "Description";
                cmbItemTye.DisplayMember = "Description";

                cmbItemTye.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }
        }
        private void GetItemDateSet()
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

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (chbCategory.Checked)
            {
                cmbCategory.Text = "";
                cmbCategory.Enabled = false;
            }
            else
            {
                cmbCategory.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                cmbBrand.Text = "";
                cmbBrand.Enabled = false;
            }
            else
            {
                cmbBrand.Enabled = true;
            }
        }
        public int ReportType = 0;
        public string FromDate, Todate;
        private void btnViewItemWise_Click(object sender, EventArgs e)
        {
            DSInvoicing.Clear();

            try
            {
                string sSQL = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description,dbo.tblSalesInvoices.Amount/dbo.tblSalesInvoices.Qty as UnitPrice, dbo.tblSalesInvoices.Qty, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                        " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                          "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblSalesInvoices.Description like '%'+'" + cmbItem.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty>0  and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%'";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSInvoicing.tblSalesWise);

                string sSQL2 = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description,dbo.tblSalesInvoices.Amount as UnitPrice, dbo.tblSalesInvoices.FOCQty as Qty, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                       " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                         "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblSalesInvoices.Description like '%'+'" + cmbItem.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty=0 and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%' and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%'";

                SqlCommand cmd33 = new SqlCommand(sSQL2);
                SqlConnection con33 = new SqlConnection(ConnectionString);
                SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
                da33.Fill(DSInvoicing.tblSalesWise);




                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

                ReportType = 1;
                DateTime DTP = Convert.ToDateTime(dtpFromDate.Value);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Value);
                string Dformat = "dd-MMM-yy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);

                string Cname = "";
                if (cmbWH.Text.ToString().Trim() == "Lasantha TYRE TRADERS")
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

                frmSalesWiseReportView WiseView = new frmSalesWiseReportView(this, ReportType, FromDate, Todate, Cname);
                WiseView.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DSInvoicing.Clear();

            try
            {
                string sSQL = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description,dbo.tblSalesInvoices.Amount/dbo.tblSalesInvoices.Qty as UnitPrice, dbo.tblSalesInvoices.Qty, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                        " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                          "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblSalesInvoices.Description like '%'+'" + cmbServiceItem.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty>0  and JobDoneBy like '%'+'" + cmbJobDone.Text.ToString().Trim() + "' and WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%'";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSInvoicing.tblSalesWise);

                string sSQL2 = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description,dbo.tblSalesInvoices.Amount as UnitPrice, dbo.tblSalesInvoices.FOCQty as Qty, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                       " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                         "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblSalesInvoices.Description like '%'+'" + cmbServiceItem.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty=0 and JobDoneBy like '%'+'" + cmbJobDone.Text.ToString().Trim() + "' and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%'";

                SqlCommand cmd33 = new SqlCommand(sSQL2);
                SqlConnection con33 = new SqlConnection(ConnectionString);
                SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
                da33.Fill(DSInvoicing.tblSalesWise);




                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

                ReportType = 1;
                DateTime DTP = Convert.ToDateTime(dtpFromDate.Value);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Value);
                string Dformat = "dd-MMM-yy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);


                string Cname = "";
                if (cmbWH.Text.ToString().Trim() == "Lasantha TYRE TRADERS")
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

                frmSalesWiseReportView WiseView = new frmSalesWiseReportView(this, ReportType, FromDate, Todate, Cname);
                WiseView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                cmbServiceItem.Text = "";
                cmbServiceItem.Enabled = false;
            }
            else
            {
                cmbServiceItem.Enabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                cmbJobDone.Text = "";
                cmbJobDone.Enabled = false;
            }
            else
            {
                cmbJobDone.Enabled = true;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

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

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DSInvoicing.Clear();

            try
            {

                string sSQL = "SELECT [InvoiceNo],[InvoiceDate],[NoofDistributions],[DistributionNo],[ItemID],[Qty],[Description],[UnitPrice],[TotalDiscountPercen],[Amount],[TotalDiscountAmount]" +
      ",[GrossTotal],[NetTotal],[CurrentDate],[Time],[Currentuser],[Location],[SalesRep],[CostPrrice],[ItemClass],[ItemType]" +
      ",[IsVoid],[SubValue],[WHID],[ContactNo],[PaidAmount],[CusPoNum],[Mileage],[JobDoneBy],[IsConfirm],[VehicleNo],[FOCQty] FROM [dbo].[tblSalesInvoices] " +
       "  where InvoiceDate>='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and InvoiceDate<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and [ItemType] ='"+ cmbItemTye.Text.ToString().Trim() +"' and [Currentuser] ='"+ cmbSalesRep.Text.ToString().Trim() +"' order by [InvoiceNo] ";



        //string sSQL = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description,dbo.tblSalesInvoices.Amount/dbo.tblSalesInvoices.Qty as UnitPrice, dbo.tblSalesInvoices.Qty,dbo.tblSalesInvoices.SalesRep, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
        //                " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
        //                  "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblSalesInvoices.Description like '%'+'" + cmbServiceItem.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty>0  and JobDoneBy like '%'+'" + cmbJobDone.Text.ToString().Trim() + "' and WHID like '%" + cmbWH.Text.ToString().Trim() + "' and  SalesRep like '" + cmbSalesRep.Text.ToString().Trim() + "%'";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSInvoicing.dtSalesCommision);

                //string sSQL2 = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description,dbo.tblSalesInvoices.Amount as UnitPrice, dbo.tblSalesInvoices.FOCQty as Qty,dbo.tblSalesInvoices.SalesRep, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                //       " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                //         "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblSalesInvoices.Description like '%'+'" + cmbServiceItem.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty=0 and JobDoneBy like '%'+'" + cmbJobDone.Text.ToString().Trim() + "' and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and SalesRep like '" + cmbSalesRep.Text.ToString().Trim() + "%'";

                //SqlCommand cmd33 = new SqlCommand(sSQL2);
                //SqlConnection con33 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
                //da33.Fill(DSInvoicing.tblSalesWise);




                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

                ReportType = 5;
                DateTime DTP = Convert.ToDateTime(dtpFromDate.Value);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Value);
                string Dformat = "dd-MMM-yy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);


                string Cname = "";
                if (cmbWH.Text.ToString().Trim() == "Lasantha TYRE TRADERS")
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

                frmSalesWiseReportView WiseView = new frmSalesWiseReportView(this, ReportType, FromDate, Todate, Cname);
                WiseView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnViewCategoryWise_Click(object sender, EventArgs e)
        {
            DSInvoicing.Clear();

            try
            {

                string sSQL = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description, dbo.tblSalesInvoices.Amount/dbo.tblSalesInvoices.Qty as UnitPrice, dbo.tblSalesInvoices.Qty, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                      " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                        "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblItemMaster.Categoty like '%'+'" + cmbCategory.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty>0 and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%'";

              
                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSInvoicing.tblSalesWise);

                string sSQL2 = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description, dbo.tblSalesInvoices.Amount as UnitPrice, dbo.tblSalesInvoices.FOCQty as Qty, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                  "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblItemMaster.Categoty like '%'+'" + cmbCategory.Text.ToString().Trim() + "'and  dbo.tblSalesInvoices.Qty=0 and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%'";


                SqlCommand cmd33 = new SqlCommand(sSQL2);
                SqlConnection con33 = new SqlConnection(ConnectionString);
                SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
                da33.Fill(DSInvoicing.tblSalesWise);


                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

                ReportType = 2;

                DateTime DTP = Convert.ToDateTime(dtpFromDate.Value);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Value);
                string Dformat = "dd-MMM-yy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);

                string Cname = "";
                if (cmbWH.Text.ToString().Trim() == "Lasantha TYRE TRADERS")
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

                frmSalesWiseReportView WiseView = new frmSalesWiseReportView(this, ReportType, FromDate, Todate, Cname);
                WiseView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnViewBrandWise_Click(object sender, EventArgs e)
        {
            DSInvoicing.Clear();

            try
            {
                string sSQL = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description, dbo.tblSalesInvoices.Amount/dbo.tblSalesInvoices.Qty as UnitPrice, dbo.tblSalesInvoices.Qty, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                                     " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                                       "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblItemMaster.Custom3 like '%'+'" + cmbBrand.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty>0 and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%'";


                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSInvoicing.tblSalesWise);

                string sSQL2 = "SELECT dbo.tblSalesInvoices.ItemID, dbo.tblSalesInvoices.Description, dbo.tblSalesInvoices.Amount as UnitPrice, dbo.tblSalesInvoices.FOCQty as Qty, dbo.tblItemMaster.Categoty as Category, dbo.tblItemMaster.Custom3 as Brand,dbo.tblSalesInvoices.Amount as LineTotal FROM  dbo.tblItemMaster INNER JOIN" +
                                   " dbo.tblSalesInvoices ON dbo.tblItemMaster.ItemID = dbo.tblSalesInvoices.ItemID where dbo.tblSalesInvoices.[InvoiceDate] >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and dbo.tblSalesInvoices.[InvoiceDate] <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                                     "and  dbo.tblSalesInvoices.IsVoid='False' and dbo.tblItemMaster.Custom3 like '%'+'" + cmbBrand.Text.ToString().Trim() + "' and dbo.tblSalesInvoices.Qty=0 and  WHID like '%" + cmbWH.Text.ToString().Trim() + "' and WHID like '" + cmbWH.Text.ToString().Trim() + "%'";


                SqlCommand cmd33 = new SqlCommand(sSQL2);
                SqlConnection con33 = new SqlConnection(ConnectionString);
                SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
                da33.Fill(DSInvoicing.tblSalesWise);


                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

                ReportType = 3;

                DateTime DTP = Convert.ToDateTime(dtpFromDate.Value);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Value);
                string Dformat = "dd-MMM-yy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);
                string Cname = "";
                if (cmbWH.Text.ToString().Trim() == "Lasantha TYRE TRADERS")
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

                frmSalesWiseReportView WiseView = new frmSalesWiseReportView(this, ReportType, FromDate, Todate, Cname);
                WiseView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
