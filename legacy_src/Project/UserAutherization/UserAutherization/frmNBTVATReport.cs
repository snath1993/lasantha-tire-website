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
    public partial class frmNBTVATReport : Form
    {
        public static string ConnectionString;
        public DataSet dsItem;
        public string StrSql;
        public DTNBTVAT DSInvoicing = new DTNBTVAT();
        public int ReportType = 0;
        public string FromDate, Todate;
        public frmNBTVATReport()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void frmNBTVATReport_Load(object sender, EventArgs e)
        {
            GetItemDateSet();
          
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
        private void GetItemDateSet()
        {
            dsItem = new DataSet();
            try
            {
                dsItem.Clear();
                if (chbServiceItems.Checked == true && chbStockcItems.Checked == true)
                {
                    StrSql = "SELECT ItemID,ItemDescription,Categoty FROM tblItemMaster";
                }
                else if(chbServiceItems.Checked ==true)
                {
                    StrSql = "SELECT ItemID,ItemDescription,Categoty FROM tblItemMaster where ItemClass='7'";
                }
                else if(chbStockcItems.Checked ==true)
                {
                    StrSql = "SELECT ItemID,ItemDescription,Categoty FROM tblItemMaster where ItemClass='1'";
                }

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItem, "DtItem");

                cmbItem.DataSource = dsItem.Tables["DtItem"];
                cmbItem.DisplayMember = "ItemDescription";
                cmbItem.ValueMember = "ItemDescription";
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemID"].Width = 200;
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDescription"].Width = 300;
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["Categoty"].Width = 75;
            }
            catch (Exception ex)
            {
                throw ex;
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

        private void chbStockcItems_CheckedChanged(object sender, EventArgs e)
        {
            GetItemDateSet();
        }

        private void chbServiceItems_CheckedChanged(object sender, EventArgs e)
        {
            GetItemDateSet();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked ==true)
            {
                groupBox5.Enabled = false;
            }
            else
            {
                groupBox5.Enabled = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                groupBox5.Enabled = true;
            }
            else
            {
                groupBox5.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnViewItemWise_Click(object sender, EventArgs e)
        {
            DSInvoicing.Clear();

            try
            {
                if (radioButton1.Checked == true)
                {
                    string sSQL = "Select distinct InvoiceNo,InvoiceDate,CusName,Tax1Amount,Tax2Amount,NetTotal,Comments as VatNo  from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                              "and  IsVoid='False' and Tax1Amount>0";

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                    da3.Fill(DSInvoicing.tblNBTVATInvWise);

                    ReportType = 1;
                }
                else
                {
                    if(chbServiceItems.Checked ==true && chbStockcItems.Checked ==true)
                    {
                        string sSQL = "Select  ItemID,Description,Qty,UnitPrice,InclusivePrice,Amount,ItemClass  from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                          "and  IsVoid='False' and   Description like '%'+'" + cmbItem.Text.ToString().Trim() + "' and Tax1Amount>0";

                        SqlCommand cmd3 = new SqlCommand(sSQL);
                        SqlConnection con3 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                        da3.Fill(DSInvoicing.tblNBTVATItemWise);
                    }
                    else if(chbServiceItems.Checked ==true)
                    {
                        string sSQL = "Select  ItemID,Description,Qty,UnitPrice,InclusivePrice,Amount,ItemClass  from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                        "and  IsVoid='False' and   Description like '%'+'" + cmbItem.Text.ToString().Trim() + "' and ItemClass ='7'";

                        SqlCommand cmd3 = new SqlCommand(sSQL);
                        SqlConnection con3 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                        da3.Fill(DSInvoicing.tblNBTVATItemWise);
                    }
                    else if(chbStockcItems.Checked ==true)
                    {
                        string sSQL = "Select  ItemID,Description,Qty,UnitPrice,InclusivePrice,Amount,ItemClass  from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                       "and  IsVoid='False' and   Description like '%'+'" + cmbItem.Text.ToString().Trim() + "' and ItemClass <>'7'";

                        SqlCommand cmd3 = new SqlCommand(sSQL);
                        SqlConnection con3 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                        da3.Fill(DSInvoicing.tblNBTVATItemWise);
                    }
                   

                 

                    ReportType = 2;
                }


                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

            
                DateTime DTP = Convert.ToDateTime(dtpFromDate.Text);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Text);
                string Dformat = "MM/dd/yyyy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);


                frmNBTVATInvWiseReportView WiseView = new frmNBTVATInvWiseReportView(this, ReportType, FromDate, Todate);
                WiseView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
