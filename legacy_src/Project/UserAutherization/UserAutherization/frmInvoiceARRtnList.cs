using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;


namespace UserAutherization
{
    public partial class frmInvoiceARRtnList : Form
    {
        string sSQL = string.Empty;
        string sMsg = "Peachtree - Customer Invoice List";
        SqlConnection sqlCon;
        clsCommon objclsCommon = new clsCommon();      
        SqlCommand sqlCMD;
        SqlDataAdapter sqlDA;
        DataSet sqlDS;
        public DSCustomerReturn ds = new DSCustomerReturn();
        static string ConnectionString;
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        public frmInvoiceARRtnList()
        {
            setConnectionString();
            InitializeComponent();
        }

        private void tsbExport_Click(object sender, EventArgs e)
        {
            //try
            //{
                SaveFileDialog fdlg = new SaveFileDialog();
                fdlg.Title = "Export to Excel";
                fdlg.InitialDirectory = "C:\\";
                fdlg.Filter = "MS Excel (*.xls)|*.xls";
                fdlg.RestoreDirectory = true;

                string myFineName = string.Empty;

                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    myFineName = fdlg.FileName;
                }

                if (myFineName == String.Empty)
                {
                    MessageBox.Show("Please enter a file name", "Export to excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ugExcelExporter.Export(UG, myFineName);

                System.Diagnostics.Process Process = new System.Diagnostics.Process();
                Process.StartInfo.FileName = myFineName;
                Process.Start();
            //}
            
        }
        
        private void GetInvoiceList()
        {
            try
            {
                sSQL = "SELECT     DISTINCT (CreditNo), ReturnDate, LocationID, InvoiceNO, VAT, GrandTotal, SalesRep, CustomerPO,InvType, " +
                    " CASE InvType WHEN 1 THEN 'Invoice Rtn' WHEN 2 THEN 'General Rtn' END AS RtnType FROM tblCutomerReturn";
                sqlCon = new SqlConnection(ConnectionString);
                sqlCMD = new SqlCommand(sSQL, sqlCon);
                sqlCMD.CommandType = CommandType.Text;
                sqlDA = new SqlDataAdapter(sqlCMD);
                sqlDS = new DataSet();
                sqlCon.Open();
                sqlDA.Fill(sqlDS);
                sqlCon.Close();

                UG.DataSource = sqlDS.Tables[0];

                UG.DisplayLayout.Bands[0].Columns["GrandTotal"].Format = "0.00";
                UG.DisplayLayout.Bands[0].Columns["GrandTotal"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                UG.DisplayLayout.Bands[0].Columns["VAT"].Format = "0.00";
                UG.DisplayLayout.Bands[0].Columns["VAT"].Header.Caption = "VAT";
                UG.DisplayLayout.Bands[0].Columns["VAT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                UG.DisplayLayout.Bands[0].Columns["ReturnDate"].Format = "dd/MM/yyyy";
                UG.DisplayLayout.Bands[0].Columns["InvType"].Hidden = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void frmInvoiceARRtnList_Load(object sender, EventArgs e)
        {
            try
            {
                GetInvoiceList();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            } 
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
        private void InvoicePrint(string InvNo)
        {


            if (InvNo.Length > 0)
            {
                ds.Clear();

                try
                {
                    String S1 = "Select * from tblCutomerReturn WHERE CreditNo = '" + InvNo + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(ds, "DTReturn");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    //----------------------- Added on 23/04/2011
                    try
                    {
                        String S2 = "Select * from tblCustomerMaster";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlConnection con2 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, con2);
                        da2.Fill(ds, "CustomerMaster");
                    }

                    catch { }
                    try
                    {
                        String S3 = "Select * from tblSalesInvoices";
                        SqlCommand cmd3 = new SqlCommand(S3);
                        SqlConnection con3 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                        da3.Fill(ds, "dtInvoiceData");

                    }
                    catch { }
                    //-----------------------

                    frmViewerCustomerReturnReprint cusReturn = new frmViewerCustomerReturnReprint(this);
                    cusReturn.Text = "Reprint Credit Note No : " + InvNo;  
                    cusReturn.Show();
                }
                catch { }
            }
            else
            {
                MessageBox.Show("Please select a customer return and try again");
            }
        }
        private void tsbReprint_Click(object sender, EventArgs e)
        {
            try
            {
                InvoicePrint(UG.ActiveRow.Cells["CreditNo"].Value.ToString().Trim());       
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Credit Note Reprint", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return ;
            }
          
        }
    }
}