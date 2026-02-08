using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient ;
using Infragistics.Win.UltraWinGrid;  

namespace UserAutherization
{
    public partial class frmInvoiceARRtnSerial : Form
    {
        //Used to set SQL Statments
        string sSQL = string.Empty;
        string sMsg = "Peachtree - Credit Memo";
        UltraGridRow ugR;
        //Common Sql Database Parameters
        SqlConnection sqlCon;
        SqlCommand sqlCMD;
        SqlDataAdapter sqlDA;
        DataSet sqlDS;
        static string ConnectionString;
        frmInvoiceARRtn ObjInvoiceArRtn = new frmInvoiceARRtn();
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        public frmInvoiceARRtnSerial()
        {
            setConnectionString();
            InitializeComponent();
        }
        public int  GetSerials(string ItemID, string WH, string Status,frmInvoiceARRtn objInvoiceRtn,string DeliNoteNo)
        {
            try
            {
                ObjInvoiceArRtn = objInvoiceRtn;
                if (DeliNoteNo == string.Empty)
                {
                    sSQL = "Select SerialNO from tblSerialItemTransaction where ItemID ='" + ItemID + "'  and WareHouseID ='" + WH + "' and Status='" + Status + "'";
                }
                else
                {
                    sSQL = "SELECT SerialNO FROM tblInvoiceSerializeItem WHERE INVNO ='" + DeliNoteNo + "' AND ItemID ='" + ItemID + "' AND WLocation = '" + WH + "'";

                } sqlCon = new SqlConnection(ConnectionString);
                sqlDS = new DataSet();
                sqlCMD = new SqlCommand(sSQL, sqlCon);
                sqlDA = new SqlDataAdapter(sqlCMD);
                sqlCon.Open();
                sqlDA.Fill(sqlDS);
                sqlCon.Close();
                if (sqlDS.Tables[0].Rows.Count > 0)
                {
                    cbSerials.Items.Clear();
                    foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                    {
                        cbSerials.Items.Add(Dr["SerialNO"].ToString());
                    }
                }
                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            Int64 iQty = 0;
            foreach (UltraGridRow mUgR in ObjInvoiceArRtn.UGSerial.Rows.All)
            {
                if (Convert.ToInt64( mUgR.Cells["ID"].Value) ==Convert.ToInt64(  ObjInvoiceArRtn.UGItems.Selected.Rows[0].Cells["ID"].Value))
                {
                    mUgR.Delete(false);
                }
            }
            foreach (object oSelectedItems in cbSerials.CheckedItems)
            {
                iQty = iQty + 1;
            }
            if (iQty != Convert.ToInt64(txtQty.Value))
            {
                MessageBox.Show("Selected Serial number qty should equal to return qty", "Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            foreach (object oSelectedItems in cbSerials.CheckedItems )
            {
                ugR = ObjInvoiceArRtn.UGSerial.DisplayLayout.Bands[0].AddNew();
                ugR.Cells["ID"].Value = ObjInvoiceArRtn.UGItems.Selected.Rows[0].Cells["ID"].Value;
                ugR.Cells["Serials"].Value =oSelectedItems.ToString().Trim();    
            }
           
            this.Close();  
        }
        
    }
}