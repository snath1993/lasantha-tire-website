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
using System.Text.RegularExpressions;

namespace UserAutherization
{
    public partial class frmSerialAddCommon : Form
    {
        public DataTable dblDatatable;
        public DataGridView dgvSerial;

        public frmSerialAddCommon()
        {
            InitializeComponent();
            setConnectionString();            
        }

        public frmSerialAddCommon(string _TranType, string _StrWH, string _StrItemID,
            double _dblEnteredQty, string _strRefNo, bool _IsEditMode, string _StrDescription, DataTable _Datatable)
        {
            //,DataGridView _dgvSerial
            InitializeComponent();
            setConnectionString();
            TranType = _TranType;
            StrWH = _StrWH;
            StrItemID = _StrItemID;
            dblEnteredQty = _dblEnteredQty;
            strRefNo = _strRefNo;
            IsEditMode = _IsEditMode;
            StrDescription = _StrDescription;
            dblDatatable = _Datatable;
            if (IsEditMode)
                btn_Add.Enabled = false;
            else
                btn_Add.Enabled = true;
            //_dgvSerial=_dgvSerial;
        }

        //frmBeginingBalances objfrmBeginingBalances = new frmBeginingBalances();
        public static string ConnectionString;

        public string StrSql;
        //public static SqlConnection con;
        //public static SqlTransaction Trans;

        //define global variable here --------------------
        public static string TranType;
        public static string StrWH;
        public static string StrWHName;
        public static string StrItemID;
        public static string StrDescription;
        public static double dblEnteredQty;
        public static string dblSerialAddQty;
        public static string strRefNo;
        public static bool IsEditMode;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;

            }
            catch { }
        }

        private void frmSerialAddCommon_Load(object sender, EventArgs e)
        {
            try
            {
                string SSql=string.Empty;
                
                txtItemID.Text = StrItemID;               
                txtDescription.Text = StrDescription;
                dgvSerialNum.Rows.Clear();
                DataSet dt = new DataSet();
                DataTable dtbl = new DataTable();

                if (TranType == "Grn-Tran" || TranType == "Sup-Return" ||
                    TranType == "Invoice" || TranType == "IN-InvAdjust" || TranType == "Return" ||
                    TranType == "Sup - Invoice")
                {
                    SSql = " SELECT     tblSerialTransfer.TRNNO, tblSerialTransfer.ItemID, tblSerialTransfer.SerialNO, tblSerialTransfer.TransactionType, tblSerialTransfer.LocationID, " +
                    " tblSerialTransfer.TransDate, tblSerialTransfer.IsOut, tblSerialTransfer.Status " +
                        " FROM         tblSerialTransfer " +
                    " Where tblSerialTransfer.LocationID='" + StrWH + "' and tblSerialTransfer.TransactionType='" + TranType + 
                    "' and tblSerialTransfer.TRNNO='" + strRefNo + "'";

                    SqlCommand cmd = new SqlCommand(SSql);
                    SqlDataAdapter da = new SqlDataAdapter(SSql, ConnectionString);
                    da.Fill(dt);
                }

                else if (TranType == "OpbBal")
                {
                    SSql = " SELECT     tblSerialTransfer.TRNNO, tblSerialTransfer.ItemID, tblSerialTransfer.SerialNO, tblSerialTransfer.TransactionType, tblSerialTransfer.LocationID, " +
                                        " tblSerialTransfer.TransDate, tblSerialTransfer.IsOut, tblSerialTransfer.Status " +
                                            " FROM         tblSerialTransfer " +
                                        " Where tblSerialTransfer.ItemID='"+StrItemID+"' and tblSerialTransfer.LocationID='" + StrWH + "' and tblSerialTransfer.TransactionType='" + TranType +
                                        "' and tblSerialTransfer.TRNNO='" + strRefNo + "'";
                    SqlCommand cmd = new SqlCommand(SSql);
                    SqlDataAdapter da = new SqlDataAdapter(SSql, ConnectionString);
                    da.Fill(dt);

                    if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                    {
                        clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                        clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                        clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                        clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                    }

                    clsSerializeItem.DtsSerialNoList.Rows.Clear();

                    foreach (DataRow dr in dt.Tables[0].Rows)
                    {                        
                        if (dr["Status"].ToString() == "Available")
                        {
                            clsSerializeItem.DtsSerialNoList.Rows.Add(dr["ItemID"].ToString(), dr["LocationID"].ToString(), dr["SerialNO"].ToString(), dr["Status"].ToString());
                        }
                    }
                }
                if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
                {
                    if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                    {
                        clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                        clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                        clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                        clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                    }

                    foreach (DataRow dr in dt.Tables[0].Rows)
                    {
                        if (StrWH == dr["LocationID"].ToString().Trim() && dr["ItemID"].ToString().Trim() == StrItemID)
                        {
                            if (dr["Status"].ToString() == "Available")
                            {
                                dgvSerialNum.Rows.Add(dr["SerialNo"].ToString().Trim(), dr["Status"].ToString().Trim());
                            }
                        }
                        if (dr["Status"].ToString() == "Available")
                        {
                            clsSerializeItem.DtsSerialNoList.Rows.Add(dr["ItemID"].ToString(), dr["LocationID"].ToString(), dr["SerialNO"].ToString(), dr["Status"].ToString());
                        }
                    }
                }             
                else
                {
                    foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                    {
                        if (StrWH == dr["WHCode"].ToString().Trim() && dr["ItemCode"].ToString().Trim() == StrItemID)
                        {
                            if (dr["Status"].ToString() == "New" || dr["Status"].ToString() == "Available")
                                dgvSerialNum.Rows.Add(dr["SerialNo"].ToString().Trim(), dr["Status"].ToString().Trim());
                        }
                    }
                }

                if(int.Parse(dblEnteredQty.ToString())!=GetNoOfRows())
                {
                    dgvSerialNum.Rows.Clear();

                    string expression = "ItemCode = '" + StrItemID + "'";
                    DataRow[] foundRows;

                    foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                    for (int i = 0; i < foundRows.Length; i++)
                    {
                        clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private int GetNoOfRows()
        {
            int _RowCount = 0;

            try
            {               
                foreach (DataGridViewRow dgvr in dgvSerialNum.Rows)
                {
                    if (dgvr.Cells[0].Value != null && dgvr.Cells[0].Value.ToString().Trim() != string.Empty)
                    {
                        _RowCount = _RowCount + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _RowCount;
        }

    
        


        //public string getNextID(string s)
        //{






        //    int i = 0;
        //    string nextID = "";
        //    while (i < s.Length - 1)
        //    {
        //        if ((Char.IsDigit(s[i]) && Char.IsLetter(s[i + 1])) || (Char.IsLetter(s[i]) && Char.IsDigit(s[i + 1]) || ((s[i] == '-')) || ((s[i] == ' '))))
        //        {
        //            s = s.Insert(i + 1, "*");
        //        }
        //        i++;
        //    }
        //    bool Islarge = false;
        //    string[] arr = s.Split('*');
        //    i = arr.Length - 1;
        //    for (int no = i; no >= 0; no--)
        //    {
        //        if (arr[i].Length > 19)
        //        {
        //            Islarge = true;
        //        }
        //        else
        //        {
        //            Islarge = false;
        //        }
        //    }
        //    if (Islarge == false)
        //    {

        //        while (i >= 0)
        //        {
        //            try
        //            {
        //                //if (arr[i].Length<=19)
        //                //{
        //                long no = long.Parse(arr[i]);
        //                i = 0;
        //                while (i < arr.Length)
        //                {
        //                    if (arr[i] == no.ToString())
        //                    {
        //                        no++;
        //                        arr[i] = no.ToString();
        //                    }
        //                    nextID = nextID + arr[i];
        //                    i++;
        //                }
        //                return nextID;

        //            }
        //            catch { }
        //            i--;
        //        }
        //        return s + "1";
        //    }
        //    else
        //    {
        //        //  MessageBox.Show(" Please Enter Number of digit less than 20");
        //        return null;
        //    }

        //}

        public static int NoOfItems = 0;
        private void btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (numericUpDown1.Enabled == false)
                {
                    if (txtSerialEnter.Text == String.Empty)
                    {
                        MessageBox.Show("Please Enter Serial Number");
                        txtSerialEnter.Focus();
                        return;
                    }

                    for (int j = 0; j < dgvSerialNum.Rows.Count; j++)
                    {
                        if (txtSerialEnter.Text == dgvSerialNum.Rows[j].Cells[0].Value.ToString())
                        {
                            MessageBox.Show("The Serial Number '" + dgvSerialNum.Rows[j].Cells[0].Value.ToString() + "' Already in the List");
                            txtSerialEnter.Focus();
                            return;
                        }
                    }
                    int NumberOfSno = Convert.ToInt32(numericUpDown1.Value);
                    string SerialNumber = txtSerialEnter.Text.Trim();
                    dgvSerialNum.Rows.Add();
                    int x = dgvSerialNum.Rows.Count - 1;

                    if (numericUpDown1.Value > NoOfItems || numericUpDown1.Enabled == false)
                    {
                        if (txtSerialEnter.Text != "")
                        {
                            dgvSerialNum[0, x].Value = SerialNumber;
                            dgvSerialNum[1, x].Value = "New";
                            NoOfItems++;
                            txtSerialEnter.Text = "";
                        }

                    }
                    lblSerialCount.Text = "Added: Serial Number '" + dgvSerialNum.Rows.Count + "' of '" + dblEnteredQty + "' for Item '" + StrItemID + "'";
                    txtSerialEnter.Focus();

                }
                if (numericUpDown1.Enabled == true)
                {
                    //ListBox1.Items.Clear();
                    //arrSNumbers.Clear();

                    if (txtSerialEnter.Text == String.Empty)
                    {
                        MessageBox.Show("Please Enter Serial Number");
                        txtSerialEnter.Focus();
                        return;
                    }
                    if (numericUpDown1.Value == 0)
                    {
                        MessageBox.Show("Please Enter Numer of Serial Numbers you want");
                        numericUpDown1.Focus();
                        return;
                    }
                    for (int j = 0; j < dgvSerialNum.Rows.Count; j++)
                    {
                        if (txtSerialEnter.Text == dgvSerialNum.Rows[j].Cells[0].Value.ToString())
                        {
                            MessageBox.Show("The Serial Number '" + dgvSerialNum.Rows[j].Cells[0].Value.ToString() + "' Already in the List");
                            txtSerialEnter.Focus();
                            return;
                        }
                    }
                    NoOfItems = 0;
                    int NumberOfSno = Convert.ToInt32(numericUpDown1.Value);
                    string SerialNumber = txtSerialEnter.Text.Trim();


                    if (numericUpDown1.Value > NoOfItems)
                    {
                        dgvSerialNum.Rows.Add();
                        int x = dgvSerialNum.Rows.Count - 1;

                        dgvSerialNum[0, x].Value = SerialNumber;
                        dgvSerialNum[1, x].Value = "New";
                        for (int i = x + 1; i < NumberOfSno + x; i++)
                        {
                            dgvSerialNum.Rows.Add();

                            string _NuemPart = string.Empty;
                            int _IntValue = 0;
                            string _StrPart = string.Empty;
                            //int _IntValueLength = 0;
                            //int _StrPartLength = 0;
                            //int __NuemPartLength = 0;

                            Regex re = new Regex(@"\d+", RegexOptions.RightToLeft);
                            Match m = re.Match(SerialNumber);
                            
                            _NuemPart = m.Value;
                            
                            _IntValue = int.Parse(_NuemPart);
                            _IntValue = _IntValue + 1;

                            if (m.Index > 0)
                            {
                                _StrPart = SerialNumber.Remove(m.Index);
                                if (_StrPart.Length > 0 && _NuemPart.Length != _IntValue.ToString().Length)
                                    SerialNumber = _StrPart.PadRight(SerialNumber.Length - _IntValue.ToString().Length, char.Parse("0")) + (_IntValue);
                                else
                                    SerialNumber = _StrPart.PadRight(SerialNumber.Length - _IntValue.ToString().Length, char.Parse("0")) + (_IntValue);
                            }
                            else
                            {
                                _StrPart = SerialNumber.Remove(m.Index,_NuemPart.Length);
                                if (_StrPart.Length > 0)
                                    SerialNumber = (_IntValue) + _StrPart.PadLeft(_NuemPart.Length - _IntValue.ToString().Length + 1, char.Parse("0"));
                                else
                                {
                                    if (_NuemPart.Length < _IntValue.ToString().Length)
                                        SerialNumber = (_IntValue) + _StrPart;
                                    else
                                        SerialNumber = _StrPart.PadLeft(_NuemPart.Length - _IntValue.ToString().Length, char.Parse("0")) + (_IntValue);
                                }
                            }
                            //SerialNumber = getNextID(SerialNumber);
                            dgvSerialNum[0, i].Value = SerialNumber;
                            dgvSerialNum[1, i].Value = "New";
                            NoOfItems++;
                        }
                    }

                    lblSerialCount.Text = "Added: Serial Number '" + dgvSerialNum.Rows.Count + "' of '" + dblEnteredQty + "' for Item '" + StrItemID + "'";
                    txtSerialEnter.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                numericUpDown1.Enabled = true;
                numericUpDown1.Value =int.Parse(dblEnteredQty.ToString());
            }
            else
            {
                numericUpDown1.Enabled = false;
            }
        }



        private void dgvSerialNum_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        private void dgvSerialNum_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            btn_Delete.Enabled = true;
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (dgvSerialNum.Rows.Count == 0)
            {
                //dgvItemList.Rows.Clear();
            }
            else
            {
                int N = dgvSerialNum.CurrentRow.Index;
                dgvSerialNum.Rows.RemoveAt(N);
            }
            lblSerialCount.Text = "Available: Serial Number '" + dgvSerialNum.Rows.Count + "' of '" + dblEnteredQty + "' for Item '" + StrItemID + "'";
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            dgvSerialNum.Rows.Clear();
            lblSerialCount.Text = "Available: Serial Number '" + dgvSerialNum.Rows.Count + "' of '" + dblEnteredQty + "' for Item '" + StrItemID + "'";
            txtSerialEnter.Focus();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvSerialNum.Rows.Count; i++)
                {
                    if (dgvSerialNum.Rows[i].Cells[0].Value != null) //change cell value by 1                   
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
            if (dgvSerialNum.Rows.Count == 0)
            {
                MessageBox.Show("Please Enter '" + dblEnteredQty + "' Serial numbers for the Item ID '" + txtItemID.Text.ToString().Trim() + "'");
                return;
            }

            if (GetFilledRows() != dblEnteredQty)
            {
                MessageBox.Show("Please Enter '" + dblEnteredQty + "' Serial numbers for the Item ID '" + txtItemID.Text.ToString().Trim() + "'");
                return;
            }
            int RowCount = GetFilledRows();            
            try
            {

                //if (int.Parse(numericUpDown1 .ToString()) != GetNoOfRows())
                //{
                //    dgvSerialNum.Rows.Clear();

                //    string expression = "ItemCode = '" + StrItemID + "'";
                //    DataRow[] foundRows;

                //    foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                //    for (int i = 0; i < foundRows.Length; i++)
                //    {
                //        clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                //    }
                //}

                int _rc = clsSerializeItem.DtsSerialNoList.Rows.Count;
                //DataTable _dt = clsSerializeItem.DtsSerialNoList;
                //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)

                DataRow[] foundRows = new DataRow[1];
                //if (dr["ItemCode"].ToString() == StrItemID && StrWH == dr["WHCode"].ToString().Trim())
                foundRows = clsSerializeItem.DtsSerialNoList.Select("ItemCode ='" + StrItemID + "'");
                for(int r=0;r<foundRows.Length;r++)
                {
                    clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[r]);
                }

                //for (int i = 0; i < clsSerializeItem.DtsSerialNoList.Rows.Count; i++)
                //{
                //    if (clsSerializeItem.DtsSerialNoList.Rows[i]["ItemCode"].ToString() == StrItemID && StrWH == clsSerializeItem.DtsSerialNoList.Rows[i]["WHCode"].ToString().Trim())
                //    {
                //        clsSerializeItem.DtsSerialNoList.Rows.RemoveAt(i);
                //    }
                //}
                //clsSerializeItem.DtsSerialNoList = _dt;

               
                for (int i = 0; i < RowCount; i++)
                {
                    if (dgvSerialNum.Rows[i].Cells[0].Value != null && dgvSerialNum.Rows[i].Cells[0].Value.ToString() != string.Empty)
                    {
                        DataRow drow = clsSerializeItem.DtsSerialNoList.NewRow();
                        DataColumn[] keys = new DataColumn[1];

                        if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                        {
                            clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                            clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                            clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                            clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                        }

                        drow["SerialNo"] = dgvSerialNum.Rows[i].Cells[0].Value;
                        drow["Status"] = dgvSerialNum.Rows[i].Cells[1].Value;
                        drow["ItemCode"] = StrItemID;
                        drow["WHCode"] = StrWH;
                        clsSerializeItem.DtsSerialNoList.Rows.Add(drow);                        
                    }
                }

                //for (int i = clsSerializeItem.DtsSerialNoList.Rows.Count - 1; i >= 0; i--)
                //{
                //    DataRow dr = clsSerializeItem.DtsSerialNoList.Rows[i];
                //    DataRow[] foundRows = new DataRow[1];
                //    if (dr["ItemCode"].ToString() == StrItemID && StrWH == dr["WHCode"].ToString().Trim())
                //        foundRows = clsSerializeItem.DtsSerialNoList.Select("ItemCode ='" + StrItemID + "'");
                //    if (foundRows.Length > 0)
                //    {
                //        clsSerializeItem.DtsSerialNoList.Rows.Remove(dr);
                //    }
                //}

                //foreach (DataRow dr in dblDatatable.Rows)
                //{
                //    if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                //    {
                //        clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                //        clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                //        clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                //        clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                //        //dblDatatable.PrimaryKey = "ItemCode";
                //    }

                //    DataRow drow = clsSerializeItem.DtsSerialNoList.NewRow();
                //    drow["SerialNo"] = dr["SerialNo"];
                //    drow["Status"] = dr["Status"];
                //    drow["ItemCode"] = dr["ItemCode"];
                //    drow["WHCode"] = dr["WHCode"];
                //    clsSerializeItem.DtsSerialNoList.Rows.Add(drow);  
                //    //clsSerializeItem.DtsSerialNoList = dblDatatable;
                //}
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }


        //public void SaveSerialNos(SqlConnection _myConnection, SqlTransaction _myTrans, DataTable _dtblSerial, string _TransType, bool _IsOut, string _WHCode)
        //{
        //    string Status=string.Empty;

        //    if(_TransType=="OpbBal") Status="Available"; //Av,New
        //    else if(_TransType=="Tran") 
        //    {
        //        if(_IsOut) Status="Tran";//av,new
        //        else Status = "Available";//
        //    }
        //    else if(_TransType=="Invoice") Status="Sold";
        //    else if(_TransType=="Issue") Status="Issue";
        //    else if (_TransType == "Return") Status = "Available";
        //    else if (_TransType == "Grn-Tran") Status = "Available";
        //    else if (_TransType == "CreditNote") Status = "Available";
        //    else if (_TransType == "Sup-Return") Status = "Return";
        //    else if (_TransType == "Del-Note") Status = "Sold";
        //    //foreach(DataGridViewRow dgvr in _dgview.Rows)
        //    {
        //        foreach(DataRow dr in _dtblSerial.Rows)
        //        {
        //            if (dr["ItemCode"].ToString().Trim() != string.Empty)
        //            {
        //                SqlCommand myCommand = new SqlCommand(
        //                    "if not exists (select * from tblSerialOpBalTemp where ItemID='" + dr["ItemCode"].ToString() + "' and SerialNO='" + dr["SerialNo"].ToString() + "' and LocationID='" + _WHCode + "') " +
        //                    " insert into tblSerialOpBalTemp(ItemID, Description, SerialNO, TransactionType, Status, LocationID) " +
        //                    " values('" + dr["ItemCode"].ToString() + "','','" + dr["SerialNo"].ToString() + "','" + _TransType + "','" + Status + "','" + _WHCode + "')  " +
        //                    " else " +
        //                    " update     tblSerialOpBalTemp set " +
        //                    " Description='', " +
        //                    " TransactionType='" + _TransType + "', " +
        //                    " Status='" + Status + "' " +
        //                    " where ItemID='" + dr["ItemCode"].ToString() + "' and SerialNO='" + dr["SerialNo"].ToString() + "' and LocationID='" + _WHCode + "'",
        //                _myConnection, _myTrans);
        //                SqlDataAdapter daTemSerial = new SqlDataAdapter(myCommand);
        //                DataTable dtTempSerial = new DataTable();
        //                daTemSerial.Fill(dtTempSerial);
        //            }
        //        }
        //    }
        //}


        public void SaveSerialNos_Activity(SqlConnection _myConnection, SqlTransaction _myTrans, 
            DataTable _dtblSerial, string _TransType, string _WHCode,string _RefNo,DateTime _Date,bool _IsOut,string _Status2)
        {

            string Status = string.Empty;

            if (_TransType == "OpbBal")
            {
                Status = "Available"; //Av,New
                _RefNo = "OpbBal";
            }
            else if (_TransType == "Tran")
            {
                if (_IsOut) Status = "Adjusted";//av,new
                else Status = "Available";//
            }

            else if (_TransType == "IN-InvAdjust")
                Status = "Available";

            else if (_TransType == "OUT-InvAdjust")
                Status = "Adjusted";

            else if (TranType == "Sup-Invoice")
                Status = "Available";

            else if (_TransType == "Invoice") Status = "Sold";
            else if (_TransType == "Issue") Status = "Issue";
            else if (_TransType == "Return") Status = "Available";
            else if (_TransType == "Grn-Tran") Status = "Available";
            else if (_TransType == "CreditNote") Status = "Available";
            else if (_TransType == "Sup-Return") Status = "Return";
            else if (_TransType == "Del-Note") Status = "Sold";

            if (_TransType == "OpbBal" || _TransType == "Grn-Tran" )
            {
                foreach (DataRow dr in _dtblSerial.Rows)
                {
                    if (dr["ItemCode"].ToString().Trim() != string.Empty && dr["Status"].ToString() == "New")
                    {
                        SqlCommand myCommand = new SqlCommand(
                            " insert into tblSerialTransfer(TRNNO, ItemID, SerialNO, TransactionType, Status, LocationID, TransDate,IsOut,sTATUS2) " +
                            " values('" + _RefNo + "','" + dr["ItemCode"].ToString() + "','" + dr["SerialNo"].ToString() + "','" + _TransType + "','" + Status + "','" + _WHCode + "','" + _Date + "','" + _IsOut + "','" + _Status2 + "')  ",
                        _myConnection, _myTrans);
                        SqlDataAdapter daTemSerial = new SqlDataAdapter(myCommand);
                        DataTable dtTempSerial = new DataTable();
                        daTemSerial.Fill(dtTempSerial);
                    }
                }
            }
            else
            {
                foreach (DataRow dr in _dtblSerial.Rows)
                {
                    if (dr["ItemCode"].ToString().Trim() != string.Empty)
                    {
                        SqlCommand myCommand = new SqlCommand(
                            " insert into tblSerialTransfer(TRNNO, ItemID, SerialNO, TransactionType, Status, LocationID, TransDate,IsOut,Status2) " +
                            " values('" + _RefNo + "','" + dr["ItemCode"].ToString() + "','" + dr["SerialNo"].ToString() + "','" + _TransType + "','" + Status + "','" + _WHCode + "','" + _Date + "','" + _IsOut + "','"+_Status2+"')  ",
                        _myConnection, _myTrans);
                        SqlDataAdapter daTemSerial = new SqlDataAdapter(myCommand);
                        DataTable dtTempSerial = new DataTable();
                        daTemSerial.Fill(dtTempSerial);
                    }
                }
            }
        }    
    }
}