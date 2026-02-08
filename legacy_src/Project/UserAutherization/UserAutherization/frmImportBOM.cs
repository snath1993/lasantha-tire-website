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
    public partial class frmImportBOM : Form
    {
        public frmImportBOM()
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

                   // conn.AssemblyJournal();
                    //conn.ImportComponentMaster();
                    conn.ImportBOMMaster();
                    conn.InserBOMData();

                   // conn.ImportCustomer_Master();
                    //conn.Insert_Customer();
                    MessageBox.Show("BOM Master file Successfully imported from Peachtree", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please add warehouses before you import the master files");
                }
            }
            catch { }

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