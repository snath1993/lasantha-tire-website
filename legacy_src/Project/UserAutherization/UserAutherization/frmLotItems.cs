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
using System.Xml;

namespace UserAutherization
{
    public partial class frmLotItems : Form
    {
        Controlers objControlers = new Controlers();
        public static DataTable _dataTable = new DataTable();

        public frmLotItems()
        {
            InitializeComponent();
            setConnectionString();
            cmbWH.Focus();
        }
        //Golbal variable define here======================
        private double unitCost = 0.00;
        private double quantity = 0.00;
        private double totalCost = 0.00;
        public static string ConnectionString;
        private bool IsItemSerial = false;
        CSInvoiceSerial ObjSerialInvoice = new CSInvoiceSerial();
        clsCommon objclsCommon = new clsCommon();
        public string StrSql;
        public DataSet dsItem;
        public DataSet dsWarehouse;
        public DataSet dsVendor;
        public DataSet dsAR;
        public string sMsg = "Lot Items";

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetItemDataset()
        {
            dsItem = new DataSet();
            try
            {

                dsItem.Clear();
                StrSql = "SELECT ItemID, ItemDescription FROM tblItemMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItem, "DtItem");

                ugCmbItem.DataSource = dsItem.Tables["DtItem"];
                ugCmbItem.DisplayMember = "ItemId";
                ugCmbItem.ValueMember = "ItemId";
                ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemID"].Width = 100;
                ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDescription"].Width = 200;
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["QTY"].Width = 75;
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["UnitCost"].Width = 75;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       

        private void innerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }       

        private void frmBeginingBalances_Load(object sender, EventArgs e)
        {
            try
            {               
                GetItemDataset();
                Load_InvetoryList();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
  
        private void Load_InvetoryList()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select ItemID,ItemDescription,isnull(IsLotItem,'False')  from tblItemMaster"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvInventoryBeginBal.Rows.Add();
                        dgvInventoryBeginBal.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInventoryBeginBal.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvInventoryBeginBal.Rows[i].Cells[2].Value = bool.Parse(dt.Rows[i].ItemArray[2].ToString().Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        private void btnSave_Click(object sender, EventArgs e)
        {
            //dgvInventoryBeginBal.CommitEdit(DataGridViewDataErrorContexts.LeaveControl);
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {                
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                foreach (DataGridViewRow dgvr in dgvInventoryBeginBal.Rows)
                {
                    if (dgvr.Cells[0].Value != null && dgvr.Cells[0].Value.ToString().Trim() != string.Empty)
                    {
                        StrSql = "UPDATE tblItemMaster SET IsLotItem='" + bool.Parse(dgvr.Cells[2].Value.ToString()) + "' WHERE ItemID='" + dgvr.Cells[0].Value.ToString().Trim() + "'";

                        SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }

                myTrans.Commit();

                MessageBox.Show("Items Saved Successfully..!", "Lot Items", MessageBoxButtons.OK, MessageBoxIcon.Information);

                GetItemDataset();
                Load_InvetoryList();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                throw ex;
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ugCmbItem_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (ugCmbItem.Value == null) return;
                               

                int rowCount = GetFilledRows();//get row count from data grid

                for (int i = 0; i < rowCount; i++)
                {
                    if (dgvInventoryBeginBal[0, i].Value.ToString().Contains(ugCmbItem.Value.ToString().Trim()))
                    {
                        dgvInventoryBeginBal.CurrentCell = dgvInventoryBeginBal[0, i];
                        break;
                    }
                }
                //SetSelectdRow();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private int GetFilledRows()
        {
            try
            {
                int RowCount = 0;
                for (int i = 0; i < dgvInventoryBeginBal.Rows.Count; i++)
                {
                    if (dgvInventoryBeginBal.Rows[i].Cells[0].Value != null) //change cell value by 1                   
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

        private void dgvInventoryBeginBal_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvInventoryBeginBal_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvInventoryBeginBal.IsCurrentCellDirty)
            {
                dgvInventoryBeginBal.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

       
        

       

        
        


        
      
    }
}