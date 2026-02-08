using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace UserAutherization
{
    public partial class frmImportCustomer : Form
    {
        public frmImportCustomer()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;
        Connector conn = new Connector();

        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
        }
       // Connector conn = new Connector();
        private void frmImportCustomer_Load(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            //===================================
            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(user.LoginDate)))
                    return;
                String S = "Select * from tblWhseMaster";//where TaxCode = '" + cmbtaxCode.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    //conn.ImportCustomer_MasterCSV();

                   // conn.ExportSaleaOrderFrmPeachtree(); // purchase invoice journal
                  //
                   
                    //conn.Upload_ReceiptJ_BatchMode();

                    ///////////////////////////////////-Import Invoice & Receipt-/////////////////////
                    
                    //conn.ExportSalesJournal_By_Period(dateTimePicker1.Value, dateTimePicker2.Value );
                    //conn.Insert_Invoice();


                    //conn.ExportEnterBillsFromP();

                    //conn.Export_Receipt_Journal(); 
                    //conn.Insert_Receipt();

                    ///////////////////////////////////-Import Invoice & Receipt-/////////////////////
                   // conn.ImportDirectSalesInvice(); //Invoice
                    //conn.ExportSupplierInvoice();

                    conn.ImportCustomer_Master();//sanjeewa comment on31082013
                    conn.fillID_Customer_listAll();
                    //conn.Insert_Customer1();


                    // conn.ExportInventoryAdjustment();
                    //conn.ExportPurchaseJournal();

                   // conn.ExportSalesJournal(); //receipt
                   // conn.Insert_Receipt(); //receipt insert

                    //conn.ExportSalesOrderJournal();

                    //conn.ExportCustomerReturn();
                    //ExportSalesOrderJournal
                      //conn.Insert_CustomerCSV();
                    MessageBox.Show("Customer Master file Successfully imported from Peachtree", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please add warehouses before you import the master files");
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
                return;
            }

            //====================================

            //try
            //{
            //   // Connector conn = new Connector();
            //   // conn.ImportCustomerInvoicesApply();
            //  // conn.ImportCustomer_Master();
            //    //conn.Insert_Customer();
            //    conn.ImportCustomer_MasterCSV();
            //    conn.Insert_CustomerCSV();
            //   MessageBox.Show("Customer Master file Successfully imported from Peachtree");
            //   this.Close();
            //}
            //catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}