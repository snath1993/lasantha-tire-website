using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;
using System.Xml;
using System.Collections;
using System.Threading;

namespace UserAutherization
{
    public partial class frmSerialSubCommon : Form
    {
        public frmSerialSubCommon(string _TranType, string _StrWH, string _StrItemID,
            double _dblEnteredQty, string _strRefNo, bool _IsEditMode, DataTable _Datatable,
            DataTable _dtblRefNo, bool _IsDirect, bool _IsEnable)
        {
            InitializeComponent();
            setConnectionString();

            TranType = _TranType;
            StrWH = _StrWH;
            StrItemID = _StrItemID;
            dblEnteredQty = _dblEnteredQty;
            strRefNo = _strRefNo;
            IsEditMode = _IsEditMode;
            dblDatatable = _Datatable;
            dtblRefNo = _dtblRefNo;
            IsDirect = _IsDirect;

            if (_IsEditMode)
            {
                dgvSerialNum.Enabled = false;
                btn_Add.Enabled = false;
                btn_Clear.Enabled = false;
            }
        }

        public static string ConnectionString;

        public string StrSql;
        //public static SqlConnection con;
        //public static SqlTransaction Trans;

        //define global variable here --------------------
        public static string TranType;
        public static string Status;

        public static string StrWH;
        public static string StrWHName;
        public static string StrItemID;
        public static string StrDescription;
        public static double dblEnteredQty;
        public static string dblSerialAddQty;
        public static bool IsEditMode;
        public static string strRefNo;
        public DataTable dblDatatable;
        public DataTable dtblRefNo;
        public bool IsDirect;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;

            }
            catch { }
        }

        private void frmSerialSubCommon_Load(object sender, EventArgs e)
        {
            try
            {
                string SSql = string.Empty;
                txtWarehouseID.Text = StrWH;
                txtItemID.Text = StrItemID;
                txtDescription.Text = StrDescription;
                dgvSerialNum.Rows.Clear();                

                if (IsEditMode)
                {
                    if (TranType == "Tran" || TranType == "Issue" || TranType == "Return" || TranType == "Sup-Return" ||
                        TranType == "Del-Note" || TranType == "Invoice" || TranType=="CreditNote")
                    {
                        SSql = " select 'True' as Selected , tblSerialTransfer.ItemID, tblSerialTransfer.SerialNO, tblSerialTransfer.TransactionType, tblSerialTransfer.LocationID, " +
                        " tblSerialTransfer.Status " +
                        " FROM tblSerialTransfer " +
                        " where tblSerialTransfer.ItemID='" + StrItemID + "' and tblSerialTransfer.LocationID='" + StrWH + "' and (tblSerialTransfer.TRNNO='" + strRefNo + "' )";
                    }
                    else if (TranType == "Sup-Invoice")
                    {
                        SSql = "SELECT 'True' as Selected, TRNNO,ItemID as ItemCode,SerialNO,TransactionType,Status,LocationID as WHCode,TransDate,IsOut,Status2 FROM tblSerialTransfer " +
                                         " WHERE (tblSerialTransfer.ItemID = '" + StrItemID + "') and Status<>'Invoiced' and   (";

                        for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                        {
                            SSql = SSql + " tblSerialTransfer.TRNNO='" + dtblRefNo.Rows[Index]["GRN"].ToString().Trim() + "' ";

                            if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                                SSql = SSql + " or ";
                        }
                        SSql = SSql + ")";
                    }
                    else
                    {
                        SSql = " select 'True' as Selected , tblSerialTransfer.TRNNO, tblSerialTransfer.ItemID, tblSerialTransfer.SerialNO, tblSerialTransfer.TransactionType, tblSerialTransfer.LocationID, " +
                            " tblSerialTransfer.TransDate, tblSerialTransfer.IsOut, tblSerialOpBalTemp.Status " +
                            " FROM tblSerialTransfer INNER JOIN " +
                            " tblSerialOpBalTemp ON tblSerialTransfer.ItemID = tblSerialOpBalTemp.ItemID AND tblSerialTransfer.SerialNO = tblSerialOpBalTemp.SerialNO " +
                            " where tblSerialTransfer.ItemID='" + StrItemID + "' and tblSerialTransfer.LocationID='" + StrWH + "' and tblSerialTransfer.TRNNO='" + strRefNo + "'";
                    }
                }
                else
                {
                    if (TranType == "Tran" || TranType == "OUT-InvAdjust" || TranType=="Issue")
                    {
                        SSql = " SELECT 'False' as Selected, WareHouseID, ItemID, SerialNO, TranType, Status FROM tblSerialItemTransaction " +
                                " WHERE WareHouseID='" + StrWH + "' and ItemID = '" + StrItemID + "' and Status='Available'";                        
                    }
                    else if (TranType == "Invoice")
                    {
                        if (!IsDirect)
                        {
                            SSql = "SELECT 'True' as Selected, TRNNO,ItemID,SerialNO,TransactionType,Status,LocationID,TransDate,IsOut,Status2 FROM tblSerialTransfer " +
                           " WHERE (tblSerialTransfer.ItemID = '" + StrItemID + "') and Status<>'Invoiced' and   (";

                            for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                            {
                                SSql = SSql + " tblSerialTransfer.TRNNO='" + dtblRefNo.Rows[Index]["DNote"].ToString().Trim() + "' ";

                                if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                                    SSql = SSql + " or ";
                            }
                            SSql = SSql + ")";
                        }
                        else
                            SSql = " select *,'False' as Selected from tblSerialItemTransaction Where ItemID='" + StrItemID + "' and WareHouseID='" + StrWH + "' and ( Status='Available')";
                    }
                    else if (TranType == "Del-Note" || TranType == "InvAdjust")
                    {
                        SSql = " SELECT 'False' as Selected, WareHouseID, ItemID, SerialNO, TranType, Status FROM tblSerialItemTransaction Where ItemID='" + StrItemID + "' and WareHouseID='" + StrWH + "' and ( Status='Available')";
                    }
                    else if (TranType == "Sup-Invoice")
                    {
                        SSql = "SELECT 'True' as Selected, TRNNO,ItemID,SerialNO,TransactionType,Status,LocationID,TransDate,IsOut,Status2 FROM tblSerialTransfer " +
                            " WHERE (tblSerialTransfer.ItemID = '" + StrItemID + "') and Status<>'Invoiced' and   (";

                        for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                        {
                            SSql = SSql + " tblSerialTransfer.TRNNO='" + dtblRefNo.Rows[Index]["GRN"].ToString().Trim() + "' ";

                            if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                                SSql = SSql + " or ";
                        }
                        SSql = SSql + ")";
                    }
                    else if (TranType == "Sup-Return")
                    {
                        if (!IsDirect)
                        {
                            SSql = "SELECT 'False' as Selected,tblSerialTransfer.TRNNO, tblSerialTransfer.ItemID, tblSerialTransfer.SerialNO, tblSerialTransfer.TransactionType, tblSerialTransfer.Status, "+
                                " tblSerialTransfer.LocationID, tblSerialTransfer.TransDate, tblSerialTransfer.IsOut, tblSerialTransfer.Status2, tblSupplierInvoices.SupInvoiceNo "+
                                " FROM tblSerialTransfer INNER JOIN tblSupplierInvoices ON tblSerialTransfer.TRNNO = tblSupplierInvoices.SupInvoiceNo AND tblSerialTransfer.ItemID = tblSupplierInvoices.ItemID " +
                                " WHERE (tblSerialTransfer.ItemID = '" + StrItemID + "') and Status<>'Sup-Returned' and   (";

                            for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                            {
                                SSql = SSql + " tblSupplierInvoices.SupInvoiceNo='" + dtblRefNo.Rows[Index]["GRN"].ToString() + "' ";

                                if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                                    SSql = SSql + " or ";
                            }
                            SSql = SSql + ")";
                        }
                        else
                            SSql = "SELECT  distinct   'False' AS Selected,  tblSerialTransfer.ItemID, "+
                                " tblSerialTransfer.SerialNO,tblSerialItemTransaction.Status FROM tblSerialTransfer INNER JOIN " +
                                " tblSerialItemTransaction ON tblSerialTransfer.ItemID = tblSerialItemTransaction.ItemID AND "+
                                " tblSerialTransfer.SerialNO = tblSerialItemTransaction.SerialNO "+
                                " WHERE (tblSerialTransfer.ItemID = '" + StrItemID + 
                                "') and (tblSerialTransfer.TransactionType <> 'Sup-Invoice') AND (tblSerialItemTransaction.Status = 'Available')";
                    }
                    else if (TranType == "CreditNote")
                    {
                        if (!IsDirect)
                        {
                            SSql = "SELECT 'False' as Selected,tblSerialTransfer.TRNNO, tblSerialTransfer.ItemID, tblSerialTransfer.SerialNO, " +
                                " tblSerialTransfer.TransactionType, tblSerialTransfer.Status, tblSerialTransfer.LocationID, tblSerialTransfer.TransDate, " +
                                " tblSerialTransfer.IsOut, tblSerialTransfer.Status2, tblSalesInvoices.InvoiceNo  " +
                                " FROM         tblSerialTransfer INNER JOIN "+
                                " tblSalesInvoices ON tblSerialTransfer.TRNNO = tblSalesInvoices.InvoiceNo AND tblSerialTransfer.ItemID = tblSalesInvoices.ItemID INNER JOIN "+
                                " tblSerialItemTransaction ON tblSerialTransfer.ItemID = tblSerialItemTransaction.ItemID AND tblSerialTransfer.SerialNO = tblSerialItemTransaction.SerialNO "+
                                " WHERE (tblSerialTransfer.ItemID = '" + StrItemID + "')  and tblSerialItemTransaction.Status='OutOfStock'  and ";  
                            
                            for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                            {
                                SSql = SSql + " ( tblSalesInvoices.InvoiceNo='" + dtblRefNo.Rows[Index]["RefNo"].ToString() + "' ";

                                if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                                    SSql = SSql + " or ";
                            }
                            SSql = SSql + ")";
                        }
                        else
                        
                        {
                            SSql =  " SELECT  distinct   'False' AS Selected,  tblSerialTransfer.ItemID, " +
                                    " tblSerialTransfer.SerialNO,tblSerialItemTransaction.Status FROM tblSerialTransfer INNER JOIN " +
                                    " tblSerialItemTransaction ON tblSerialTransfer.ItemID = tblSerialItemTransaction.ItemID AND " +
                                    " tblSerialTransfer.SerialNO = tblSerialItemTransaction.SerialNO " +
                                    " WHERE (tblSerialTransfer.ItemID = '" + StrItemID + "')  AND (tblSerialItemTransaction.Status = 'OutOfStock')";
                        }
                    }
                }
               
                SqlCommand cmd = new SqlCommand(SSql);
                SqlDataAdapter da = new SqlDataAdapter(SSql, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int k = 0; k < dt.Tables[0].Rows.Count; k++)
                {
                    dgvSerialNum.Rows.Add(dt.Tables[0].Rows[k]["Selected"].ToString().Trim(),dt.Tables[0].Rows[k]["SerialNo"].ToString().Trim(),
                        dt.Tables[0].Rows[k]["Status"].ToString().Trim());
                }

                foreach (DataRow dr in dblDatatable.Rows)
                {
                    if (dr["WHCode"].ToString().Trim() == StrWH&& dr["ItemCode"].ToString().Trim() == StrItemID)
                    {
                        foreach (DataGridViewRow dgvr in dgvSerialNum.Rows)
                        {
                            if (dgvr.Cells[1].Value.ToString().Trim() == dr["SerialNo"].ToString().Trim())
                            {
                                dgvSerialNum.Rows.Remove(dgvr);
                            }
                        }
                        dgvSerialNum.Rows.Add(bool.TrueString, dr["SerialNo"].ToString().Trim(), dr["Status"].ToString().Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvSerialNum.Rows.Count; i++)
            {
                if (dgvSerialNum[1, i].Value.ToString().Contains(txtSerialEnter.Text.ToString().Trim()))
                {
                    dgvSerialNum.CurrentCell = dgvSerialNum[0, i];
                    break;
                }
            }
        }

        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvSerialNum.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dgvSerialNum.Rows[i].Cells[0].Value) == true ) //change cell value by 1                   
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

        private void save_Click(object sender, EventArgs e)
        {
            int RowCount = GetFilledRows();
            if (RowCount == 0)
            {
                MessageBox.Show("Please Enter '" + dblEnteredQty + "' Serial numbers for the Item ID '" + txtItemID.Text.ToString().Trim() + "'");
                return;
            }

            if (RowCount != Math.Abs(dblEnteredQty))
            {
                MessageBox.Show("Please Enter '" + dblEnteredQty + "' Serial numbers for the Item ID '" + txtItemID.Text.ToString().Trim() + "'");
                return;
            }
         
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            //myConnection.Open();
            //SqlTransaction myTrans = myConnection.BeginTransaction();
            try
            {
                for (int i = dblDatatable.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dblDatatable.Rows[i];
                    if (dr["ItemCode"].ToString() == StrItemID &&
                    dr["WHCode"].ToString() == StrWH)
                        dr.Delete();
                }
                //dblDatatable.Rows.Count;

                for (int i = 0; i < dgvSerialNum.Rows.Count; i++)
                {
                    if (dgvSerialNum.Rows[i].Cells[1].Value != null && dgvSerialNum.Rows[i].Cells[1].Value.ToString() != string.Empty
                        && bool.Parse(dgvSerialNum.Rows[i].Cells[0].Value.ToString().Trim())==true)
                    {
                        DataRow drow = dblDatatable.NewRow();

                        if (dblDatatable.Columns.Count == 0)
                        {
                            dblDatatable.Columns.Add("ItemCode");
                            dblDatatable.Columns.Add("WHCode");
                            dblDatatable.Columns.Add("SerialNo");
                            dblDatatable.Columns.Add("Status");
                            //dblDatatable.Columns.Add("Selected");
                        }                        

                        drow["SerialNo"] = dgvSerialNum.Rows[i].Cells[1].Value;
                        drow["Status"] = dgvSerialNum.Rows[i].Cells[2].Value;
                        drow["ItemCode"] = StrItemID;
                        drow["WHCode"] = StrWH;
                        //drow["Selected"] = true;
                        dblDatatable.Rows.Add(drow);
                    }
                }

                clsSerializeItem.DtsSerialNoList = dblDatatable;
                this.Close();
            }
            catch (Exception ex)
            {
                //myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //myConnection.Close();
            }
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvSerialNum.Rows.Count; i++)
            {
                dgvSerialNum.Rows[i].Cells["colSelect"].Value = false;
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}