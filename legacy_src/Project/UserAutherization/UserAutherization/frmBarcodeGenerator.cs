using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmBarcodeGenerator : Form
    {
        public frmBarcodeGenerator()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static string ConnectionString;
        public int Printype = 4;
        public DsSupplierInvoice ds = new DsSupplierInvoice();
        Connector conn = new Connector();
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        private void loadglcode()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "SELECT ItemID,ItemDescription from  tblItemMaster ORDER BY ItemID";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    dataGridView1.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    dataGridView1.Rows[i].Cells[2].Value = "0";
                }
            }
            catch (Exception ex)
            {
                return;
            }

        }

        private void ImportItems()
        {
            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(user.LoginDate)))
                    return;
                String S1 = "Select * from tblWhseMaster";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet dt1 = new DataSet();
                da1.Fill(dt1);

                if (dt1.Tables[0].Rows.Count > 0)
                {
                    conn.ImportItem_List();
                    conn.fillID_Item_list();
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
        private void frmBarcodeGenerator_Load(object sender, EventArgs e)
        {
            loadglcode();
        }
        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != null) //change cell value by 1                   
                    {
                        RowCount++;
                    }
                }
                return RowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void createBarCode()
        {
            try
            {
                int rowCount = GetFilledRows();
                double QtyCount = 0;
                double QtyIN = 0;
                string Itemcode = "";
                string Taxable = "";
                double TaxablePrice = 0;
                double UnitPrice = 0;
                string ItemDescription;

                SqlConnection myConnection = new SqlConnection(ConnectionString);
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();

                DeleteTable(myConnection, myTrans);
                for (int b = 0; b < rowCount; b++)
                {
                    if (Convert.ToDouble(dataGridView1.Rows[b].Cells[2].Value) > 0)
                    {
                        QtyIN = Convert.ToDouble(dataGridView1.Rows[b].Cells[2].Value);
                        Itemcode = dataGridView1.Rows[b].Cells[0].Value.ToString();
                        ItemDescription = dataGridView1.Rows[b].Cells[1].Value.ToString();
                        QtyCount = Convert.ToDouble(dataGridView1.Rows[b].Cells[2].Value);

                        for (int c = 0; c < QtyCount; c++)
                        {
                            String S = "Select TaxType,UnitPrice from tblItemMaster where ItemID  = '" + Itemcode.ToString() + "'";
                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            DataSet dt = new DataSet();
                            da.Fill(dt);
                            if (dt.Tables[0].Rows.Count > 0)
                            {
                                Taxable = dt.Tables[0].Rows[0].ItemArray[0].ToString();
                                UnitPrice = Convert.ToDouble(dt.Tables[0].Rows[0].ItemArray[1].ToString());
                            }
                            if (Taxable == "TAX")
                            {
                                TaxablePrice = (Convert.ToDouble(dt.Tables[0].Rows[0].ItemArray[1].ToString()) * 112) / 100;
                            }
                            else
                            {
                                TaxablePrice = UnitPrice;
                            }
                            SqlCommand myCommand2 = new SqlCommand("insert into Temp_Barcode(Itemcode,Description,Taxable,UnitPrice,TaxablePrice) Values ('" + Itemcode.ToString().Trim() + "','" + ItemDescription.ToString().Trim() + "','" + Taxable + "','" + UnitPrice + "','" + TaxablePrice.ToString().Trim() + "')", myConnection, myTrans);
                            myCommand2.ExecuteNonQuery();
                        }
                    }
                }
                myTrans.Commit();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
        }
        private void printbarcode()
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

            ds.Clear();

            try
            {
                Printype = 4;
                String S3V = "Select * from dbo.Temp_Barcode";// where GRN_NO = '" + txtGRn_NO.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3V = new SqlCommand(S3V);
                SqlConnection con3V = new SqlConnection(ConnectionString);
                SqlDataAdapter da3V = new SqlDataAdapter(S3V, con3V);
                da3V.Fill(ds, "DTBarCode");

                frmRepSupInvoice prininv = new frmRepSupInvoice(this);
                prininv.Show();
            }
            catch (Exception ex)
            {
                //objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
                throw ex;
            }
        }
        public void DeleteTable(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                string StrSql = "DELETE FROM Temp_Barcode";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            loadglcode();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            createBarCode();
            printbarcode();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);
            if (reply == DialogResult.Cancel)
            {
                return;
            }
            ImportItems();
        }


    }

        
}
