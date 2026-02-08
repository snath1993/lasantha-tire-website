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
    public partial class frmCustomerHistory : Form
    {
        public static string ConnectionString;
        public DTVehicleHistory DSInvoicing = new DTVehicleHistory();
        public int ReportType = 0;
        public string FromDate, Todate;
        public frmCustomerHistory()
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

        private void btnViewItemWise_Click(object sender, EventArgs e)
        {
           
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DSInvoicing.Clear();

            try
            {
                string sSQL = "Select ItemID,Description,Amount/Qty as UnitPrice,Qty,Amount,ItemClass,Mileage,InvoiceDate,VehicleNo,InvoiceNo from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                          "and  IsVoid='False' and VehicleNo = '" + txtVehicleNo.Text.ToString().Trim() + "' and Qty>0";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSInvoicing.tblVehicleHistoryDetail);

                string sSQL2 = "Select ItemID,Description,UnitPrice,Qty,Amount,ItemClass,Mileage,InvoiceDate,VehicleNo,InvoiceNo from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                        "and  IsVoid='False' and VehicleNo = '" + txtVehicleNo.Text.ToString().Trim() + "' and Qty=0";

                SqlCommand cmd33 = new SqlCommand(sSQL2);
                SqlConnection con33 = new SqlConnection(ConnectionString);
                SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
                da33.Fill(DSInvoicing.tblVehicleHistoryDetail);


                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

                ReportType = 1;
                DateTime DTP = Convert.ToDateTime(dtpFromDate.Text);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Text);
                string Dformat = "MM/dd/yyyy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);


                frmVehicleHistoryView WiseView = new frmVehicleHistoryView(this, ReportType, FromDate, Todate);
                WiseView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DSInvoicing.Clear();

            try
            {

              
                string sSQL = "Select ItemID,Description,Amount/Qty as UnitPrice,Qty,Amount,ItemClass,Mileage,InvoiceDate,VehicleNo,InvoiceNo from tblSalesInvoices where ([InvoiceDate] = (select max(InvoiceDate) from tblSalesInvoices where InvoiceDate>='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and ItemClass<>'7') or [InvoiceDate] = (select max(InvoiceDate) from tblSalesInvoices where InvoiceDate>='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and ItemClass='7'))" +
                          "and  IsVoid='False' and VehicleNo = '" + txtVehicleNo.Text.ToString().Trim() + "' and Qty>0";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSInvoicing.tblVehicleHistoryDetail);


                string sSQL2 = "Select ItemID,Description,UnitPrice,Qty,Amount,ItemClass,Mileage,InvoiceDate,VehicleNo,InvoiceNo from tblSalesInvoices where ([InvoiceDate] = (select max(InvoiceDate) from tblSalesInvoices where InvoiceDate>='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and ItemClass<>'7') or [InvoiceDate] = (select max(InvoiceDate) from tblSalesInvoices where InvoiceDate>='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and ItemClass='7'))" +
                      "and  IsVoid='False' and VehicleNo = '" + txtVehicleNo.Text.ToString().Trim() + "' and Qty=0";

                SqlCommand cmd33 = new SqlCommand(sSQL2);
                SqlConnection con33 = new SqlConnection(ConnectionString);
                SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
                da33.Fill(DSInvoicing.tblVehicleHistoryDetail);


                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

                ReportType = 2;
                DateTime DTP = Convert.ToDateTime(dtpFromDate.Text);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Text);
                string Dformat = "MM/dd/yyyy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);


                frmVehicleHistoryView WiseView = new frmVehicleHistoryView(this, ReportType, FromDate, Todate);
                WiseView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DSInvoicing.Clear();

            try
            {
                string sSQL = "Select ItemID,Description,UnitPrice,Qty,Qty*UnitPrice as Amount,ItemClass,Mileage,InvoiceDate,VehicleNo,InvoiceNo from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                          "and  IsVoid='False' and VehicleNo = '" + txtVehicleNo.Text.ToString().Trim() + "' and UnitPrice=0";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSInvoicing.tblVehicleHistoryDetail);


                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSInvoicing.DTCompany);

                ReportType = 3;
                DateTime DTP = Convert.ToDateTime(dtpFromDate.Text);
                DateTime DTP2 = Convert.ToDateTime(dtpToDate.Text);
                string Dformat = "MM/dd/yyyy";
                FromDate = DTP.ToString(Dformat);
                Todate = DTP2.ToString(Dformat);


                frmVehicleHistoryView WiseView = new frmVehicleHistoryView(this, ReportType, FromDate, Todate);
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

        private void frmCustomerHistory_Load(object sender, EventArgs e)
        {

        }
    }
}
