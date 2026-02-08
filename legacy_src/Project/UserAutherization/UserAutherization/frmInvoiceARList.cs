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
    public partial class frmInvoiceARList : Form
    {
        string sSQL = string.Empty;
        string sMsg = "Peachtree - Customer Invoice List";
        SqlConnection sqlCon;
        SqlTransaction sqlTrans;
        SqlCommand sqlCMD;
        SqlDataAdapter sqlDA;
        DataSet sqlDS;
        static string ConnectionString;
        public DSInvoice DSInvoicing = new DSInvoice();
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        public frmInvoiceARList()
        {
            setConnectionString();
            InitializeComponent();
        }
        private void GetInvoiceList()
        {
            sSQL = "SELECT DISTINCT InvoiceNo, CustomerID, DeliveryNoteNos, InvoiceDate, ARAccount," +
                " Tax1Amount, Tax2Amount,  NetTotal, CustomerPO, JobID, SONO,Location," +
                " SalesRep,InvType, CASE InvType WHEN 1 THEN 'Inclusive' WHEN 2 THEN 'Exclusive' WHEN 3 THEN 'NON-VAT' END As InvType FROM tblSalesInvoices";
            sqlCon = new SqlConnection(ConnectionString);  
            sqlCMD = new SqlCommand(sSQL, sqlCon);
            sqlCMD.CommandType = CommandType.Text;
            sqlDA = new SqlDataAdapter(sqlCMD);
            sqlDS = new DataSet();
            sqlCon.Open(); 
            sqlDA.Fill(sqlDS);
            sqlCon.Close();
            
            UG.DataSource = sqlDS.Tables[0];

            UG.DisplayLayout.Bands[0].Columns["NetTotal"].Format = "0.00";
            UG.DisplayLayout.Bands[0].Columns["NetTotal"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right ;   
            UG.DisplayLayout.Bands[0].Columns["Tax1Amount"].Format = "0.00";
            UG.DisplayLayout.Bands[0].Columns["Tax1Amount"].Header.Caption = "NBT"; 
            UG.DisplayLayout.Bands[0].Columns["Tax1Amount"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            UG.DisplayLayout.Bands[0].Columns["Tax2Amount"].Format = "0.00";
            UG.DisplayLayout.Bands[0].Columns["Tax2Amount"].Header.Caption = "VAT"; 
            UG.DisplayLayout.Bands[0].Columns["Tax2Amount"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            UG.DisplayLayout.Bands[0].Columns["InvoiceDate"].Format = "dd/MM/yyyy";
            UG.DisplayLayout.Bands[0].Columns["InvType"].Hidden = true;
             
        }
        private void frmInvoiceARList_Load(object sender, EventArgs e)
        {
            GetInvoiceList(); 
        }

        private void tsbReprint_Click(object sender, EventArgs e)
        {
            UltraGridRow ugR;
            if (UG.Selected.Rows.Count == 0 || UG.Selected.Rows.Count > 1 )
            {
                return;
            }
            else
            {
                ugR = UG.ActiveRow;
                if (ugR == null)
                {
                    return;
                }
            }
            ClassDriiDown.IsInvSerch = false;
            DSInvoicing.Clear();
            sqlCon = new SqlConnection(ConnectionString);
            Int64  iInvType = 0;
            if (ugR.Cells["InvoiceType"].Value.ToString () == "")
            {
                iInvType = 0;
            }
            else
            {
                iInvType = Convert.ToInt64(ugR.Cells["InvoiceType"].Value);
            }

            if (Convert.ToInt64(iInvType) == 1 || Convert.ToInt64(iInvType) == 3)
            {
                try
                {
                    sSQL = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD  = new SqlCommand(sSQL,sqlCon  );
                    sqlDA  = new SqlDataAdapter(sqlCMD );
                    sqlDA.Fill(DSInvoicing, "CustomerMaster1");

                }

                catch { }

                //Added by Chathura on 20/04/2011
                //Added New Table into DSInvoice : tblItemMaster
                try
                {
                    sSQL = "SELECT    * FROM         tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "tblItemMaster");

                }
                catch { }
                //---------------------

                try
                {
                    sSQL = "Select * from tblSalesInvoices where InvoiceNo = '" + ugR.Cells["InvoiceNo"].Value.ToString().Trim()     + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "dtInvoiceData");

                    //frmInvoicePrint prininv = new frmInvoicePrint(this);
                    //prininv.Show();

                  
                }
                catch { }
            }
            else
            {
                try
                {
                    sSQL  = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "CustomerMaster1");

                }

                catch { }
                //Added by Chathura on 20/04/2011
                //Added New Table into DSInvoice : tblItemMaster
                try
                {
                    sSQL  = "SELECT    * FROM         tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "tblItemMaster");

                }
                catch { }
                //---------------------


                try
                {
                    sSQL = "Select * from tblSalesInvoices where InvoiceNo = '" + ugR.Cells["InvoiceNo"].Value.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "dtInvoiceData");

                    sSQL  = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "dt_CompanyDetails");



                    frmPrintTaxInvoice printax = new frmPrintTaxInvoice(this);
                    printax.Show();
                }
                catch { }
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void tsbExport_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog fdlg=new SaveFileDialog();
            fdlg.Title ="Export to Excel";
            fdlg.InitialDirectory ="C:\\";
            fdlg.Filter = "MS Excel (*.xls)|*.xls";
            fdlg.RestoreDirectory = true;

            string myFineName = string.Empty ;

            if (fdlg.ShowDialog() == DialogResult.OK )  
            {
                myFineName = fdlg.FileName;
            }



            if (myFineName == String.Empty)
            {
                MessageBox.Show("Please enter a file name","Export to excel", MessageBoxButtons.OK,  MessageBoxIcon.Information );
                return;
            }

         
            ugExcelExporter.Export(UG , myFineName);

            System.Diagnostics.Process Process=new System.Diagnostics.Process();
            Process.StartInfo.FileName = myFineName;
            Process.Start();
        
        }
    }
}