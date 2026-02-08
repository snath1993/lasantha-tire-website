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
    public partial class frmImportUnitofmesureData : Form
    {
        public frmImportUnitofmesureData()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
        }
        Connector conn = new Connector();
        private void ImportUnitofmesureData_Load(object sender, EventArgs e)
        {

        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(user.LoginDate)))
                    return;
                String S1 = "Select * from tblWhseMaster";//where TaxCode = '" + cmbtaxCode.Text.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet dt1 = new DataSet();
                da1.Fill(dt1);

                if (dt1.Tables[0].Rows.Count > 0)
                {
                    conn.ImportUnitsMeasureData();
                    //conn.fillID_Item_list();
                    MessageBox.Show("Item Master file Successfully imported from Peachtree", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please add warehouses before you import the master files");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}