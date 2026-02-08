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
using UserAutherization;

namespace WarehouseTransfer
{
    public partial class frmFGTransfer : Form
    {
       // public DSTransNoteList a = new DSTransNoteList();
        //public DSReturnNote ObjRetrnNoteDS = new DSReturnNote();
        public DSFGTRanfer ObjRetrnNoteDS = new DSFGTRanfer();
        bool run = false;
        bool add = false;
        bool edit = false;
        bool delete = false;
        DataTable dtUser = new DataTable();
        string issueNoteNo = "";
        int newIssueNoteNo = 0;
        public static string ConnectionString;

        public static DateTime UserWiseDate = System.DateTime.Now;

        public frmFGTransfer()
        {
            InitializeComponent();
            setConnectionString();
        }
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;

            }
            catch { }
        }

        private void frmIssueNote_Load(object sender, EventArgs e)
        {

            try
            {
                //----------------user----------
                try
                {
                    dtUser.Clear();
                    dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmIssueNote");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                        edit = Convert.ToBoolean(dtUser.Rows[0].ItemArray[4].ToString());
                        delete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());
                    }
                }
                catch { }
                //---------------------------------
                GetCurrentUserDate();
                btnSave.Enabled = false;
                btnPrint.Enabled = false;
                btnSearch.Enabled = true;
                btnReset.Enabled = true;
                btnNew.Enabled = true;
                //  cmbWarehouse.Enabled = false;
                dtpDate.Enabled = false;
                dgvIssueNote.Enabled = false;
                // cmbWarehouse.Enabled = true;
                //--Load warehouse ID
                String S1 = "Select WhseId,WhseName from tblWhseMaster";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    cmbWarehouse.Items.Clear();
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        cmbWarehouse.Items.Add(dt1.Rows[i].ItemArray[0].ToString().Trim());
                    }
                }
                ////-----load the Item Codes--------------------
                //string sql2 = "SELECT DISTINCT ItemID,ItemDescription FROM tblItemMaster";
                //SqlCommand cmd2 = new SqlCommand(sql2);
                //SqlDataAdapter da2 = new SqlDataAdapter(sql2,ConnectionString);
                //da2.Fill(dSWhuseAndItemList, "Dt_Item");

                //databindCombobox_Purchase();
                // GetCurrentUserDate();
                Filljob();
                databindCombobox_Purchase();
            }
            catch { }
        }
        //=====================
        //fill job list combo box with job
        private void Filljob()
        {
            String S11 = "Select JobID from tblJobMaster";
            SqlCommand cmd11 = new SqlCommand(S11);
            SqlDataAdapter da11 = new SqlDataAdapter(S11, ConnectionString);
            DataTable dt11 = new DataTable();
            da11.Fill(dt11);
            if (dt11.Rows.Count > 0)
            {
               //cmbjob 
                cmbjob.Items.Clear();
                for (int i = 0; i < dt11.Rows.Count; i++)
                {
                    cmbjob.Items.Add(dt11.Rows[i].ItemArray[0].ToString().Trim());
                }
            }
        }
        //=================================

        public void GetCurrentUserDate()
        {
            //string aa= UserAutherization.user.userName.ToString();
            try
            {
                String S = "Select CurrentDate from tblUserWiseDate where UserName='" + UserAutherization.user.userName.ToString() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                    dtpDate.Value = UserWiseDate;
                    //.ToString().Trim();
                    // cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch { }

        }

        private void databindCombobox_Purchase()
        {
            DataTable S_data2 = new DataTable();
            try
            {
                S_data2 = this.filldata();
                if (S_data2.Rows.Count > 0)
                {
                    mltcmbboxItemSelect.DataSource = S_data2;
                    mltcmbboxItemSelect.DisplayMember = "ItemID";
                    mltcmbboxItemSelect.ValueMember = "Description";
                    mltcmbboxItemSelect.Text = "";
                }

            }
            catch { }
        }

        public DataTable filldata()
        {
            DataTable dataTable = new DataTable("Item");
            string ConnString = ConnectionString;
            string sql = "select ItemID,ItemDescription,UOM from tblItemMaster where ItemClass!='8' and  ItemClass!='5'  and ItemClass!='3'";
           // string sql = "select ItemId,ItemDis,UOM from tblItemWhse where WhseId='" + WID + "'";
            //string sql = "select ItemID,Item_Description,ItemClass,PartNumber,LastUnitCost,PriceLevel from Item where ItemClass!='8' and  ItemClass!='5'  and ItemClass!='3'";
            SqlConnection Conn = new SqlConnection(ConnString);
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = Conn;
            dataTable.Columns.Add("ItemID", typeof(String));
            dataTable.Columns.Add("Description", typeof(String));
            // dataTable.Columns.Add("UOM", typeof(String));
            //dataTable.Columns.Add("UnitPrice", typeof(String));

            Conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            // dataTable.Rows.Add(new String[] { "Item ID","Description"});//, null,""  });//, null
            if (reader.HasRows == true)
            {
                while (reader.Read())
                {
                    try
                    {

                        dataTable.Rows.Add(new String[] { reader.GetString(0).Trim(), reader.GetString(1).Trim() });//, reader.GetString(3).ToString().Trim(),reader.GetString(4).ToString().Trim()});//, reader.GetValue(3).ToString(), reader.GetValue(4).ToString()
                    }
                    catch
                    { }
                }
            }
            reader.Close();
            Conn.Close();
            return dataTable;
        }

        private void dgvIssueNote_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    if (dgvIssueNote.CurrentCell.ColumnIndex == 1)
            //    {
            //        //Fill Item Description to grid
            //        String S = "select ItemDescription from tblItemMaster where ItemID = '" + dgvIssueNote[0, dgvIssueNote.CurrentRow.Index].Value + "' ";
            //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //        DataTable dt = new DataTable();
            //        da.Fill(dt);
            //        if (dt.Rows.Count > 0)
            //        {
            //            dgvIssueNote[1, dgvIssueNote.CurrentRow.Index].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
            //        }

            //        //Fill QOH
            //        String S1 = "select QTY,UOM from tblItemWhse where ItemId = '" + dgvIssueNote[0, dgvIssueNote.CurrentRow.Index].Value + "' and WhseId = '" + cmbWarehouse.Text.ToString().Trim() + "' ";
            //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //        DataTable dt1 = new DataTable();
            //        da1.Fill(dt1);
            //        if (dt1.Rows.Count > 0)
            //        {
            //            dgvIssueNote[2, dgvIssueNote.CurrentRow.Index].Value = dt1.Rows[0].ItemArray[0].ToString().Trim();
            //            dgvIssueNote[4, dgvIssueNote.CurrentRow.Index].Value = dt1.Rows[0].ItemArray[1].ToString().Trim();

            //        }
            //    }
            //    if (dgvIssueNote.CurrentCell.ColumnIndex == 4)
            //    {
            //        int onHandQty = Convert.ToInt32(dgvIssueNote[2, dgvIssueNote.CurrentRow.Index].Value.ToString());
            //        int issueQty = Convert.ToInt32(dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value.ToString());
            //        int varience = onHandQty - issueQty;
            //        if (varience < 0)
            //        {
            //            string s = "select IsMinusAllow from tblDefualtSetting";
            //            SqlDataAdapter da = new SqlDataAdapter(s,ConnectionString);
            //            DataTable dt = new DataTable();
            //            da.Fill(dt);
            //            if (dt.Rows[0].ItemArray[0].ToString().Trim() == "True")
            //            { }
            //            else if (dt.Rows[0].ItemArray[0].ToString().Trim() == "False")
            //            {
            //                MessageBox.Show("Tranfer quantity exceed than quantity on hand","Issue Note",MessageBoxButtons.OK,MessageBoxIcon.Error);

            //                if (onHandQty == 0)
            //                {
            //                    dgvIssueNote.Rows.RemoveAt(dgvIssueNote.CurrentRow.Index);
            //                    //dgvIssueNote.Rows.Remove(dgvIssueNote.CurrentRow.Index);
            //                }
            //                else if (onHandQty > 0)
            //                {
            //                    dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value = "";
            //                }
            //            }
            //        }
            //    }
            //}
            //catch { }
        }


        private void cmbWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                dgvIssueNote.Rows.Clear();
                String S2 = "select * from tblWhseMaster where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    txtWarehouseName.Text = dt2.Rows[0].ItemArray[1].ToString();
                    txtWarehouseAddress.Text = dt2.Rows[0].ItemArray[2].ToString();
                }
                if (checkActivate == false)
                {
                  //  databindCombobox_Purchase(cmbWarehouse.Text.ToString().Trim());
                }
            }
            catch { }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {

            try
            {
                String S3 = "delete from tblSerialIssueNoteTemp";// where ItemID = '" + Search.ItemId.ToString().Trim() + "'";
                SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
            }
            catch { }
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (add)
            {
                //========================
               // SqlCommand myCommand21 = new SqlCommand("delete from tblSerialIssueNoteTemp", myConnection, myTrans);
                try
                {
                    String S3 = "delete from tblSerialIssueNoteTemp";// where ItemID = '" + Search.ItemId.ToString().Trim() + "'";
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                    DataTable dt3 = new DataTable();
                    da3.Fill(dt3);
                }
                catch { }

                //====================================
                // btnNew.Enabled = false;
                //  checkActivate = false;
                btnSave.Enabled = true;
                // cmbWarehouse.Enabled = true;
                checkActivate = false;
                dtpDate.Enabled = true;
                dgvIssueNote.Enabled = true;
                mltcmbboxItemSelect.Visible = false;

                cmbWarehouse.Text = "";
                cmbWarehouse.SelectedIndex = 0;
                txtWarehouseName.Text = "";
                txtWarehouseAddress.Text = "";
                dgvIssueNote.Rows.Clear();
                txtIssueNoteId.Text = "";
                mltcmbboxItemSelect.Location = new System.Drawing.Point(10, 145);
               // mltcmbboxItemSelect.Visible = true;
            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public int ChechDQty = 0;
        public bool CheckNull = false;
        public double CheckNull1 = 0;

        public int GetFilledRows()
        {
            int RowCount = 0;

            for (int i = 0; i < dgvIssueNote.Rows.Count; i++)
            {
                if (dgvIssueNote.Rows[i].Cells[0].Value != null) //change cell value by 1
                {
                    RowCount++;
                }

            }
            return RowCount;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            int rowCount = GetFilledRows();//get row count from data grid
            bool checkZeroIssue = false;
            for (int b = 0; b < rowCount; b++)
            {
                if (Convert.ToDouble(dgvIssueNote.Rows[b].Cells[3].Value) == 0)
                {
                    checkZeroIssue = true;
                }

            }
            //==========================================================

            bool IsItemSerial = false;
            //check wether this item is serialized or not=======================
            try
            {
                for (int a = 0; a < rowCount; a++)
                {
                    string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + dgvIssueNote.Rows[a].Cells[0].Value.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);

                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                    }
                    if (ItemClass == "10" || ItemClass == "11")
                    {
                        //======================

                        IsItemSerial = true;
                        if (Convert.ToDouble(dgvIssueNote.Rows[a].Cells[3].Value) > 0)
                        {
                          //  String S1 = "Select SerialNO from tblSerialTransferTemp where ItemID  = '" + dgvItem.Rows[a].Cells[0].Value.ToString().Trim() + "'";
                            String S1 = "Select SerialNO from tblSerialIssueNoteTemp where ItemID  = '" + dgvIssueNote.Rows[a].Cells[0].Value.ToString().Trim() + "'";
                            SqlCommand cmd1 = new SqlCommand(S1);
                            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                            DataSet dt1 = new DataSet();
                            da1.Fill(dt1);
                            if (Convert.ToDouble(dgvIssueNote.Rows[a].Cells[3].Value) == dt1.Tables[0].Rows.Count)
                            {
                                IsItemSerial = false;
                            }
                        }
                        else
                        {
                            IsItemSerial = false;
                        }

                        //=========================


                        //IsItemSerial = true;
                        //String S1 = "Select SerialNO from tblSerialIssueNoteTemp where ItemID  = '" + dgvIssueNote.Rows[a].Cells[0].Value.ToString().Trim() + "'";
                        //SqlCommand cmd1 = new SqlCommand(S1);
                        //SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        //DataSet dt1 = new DataSet();
                        //da1.Fill(dt1);
                        //if (Convert.ToDouble(dgvIssueNote.Rows[a].Cells[3].Value) == dt1.Tables[0].Rows.Count)
                        //{
                        //    IsItemSerial = false;
                        //}
                    }

                }
            }
            catch { }
            //======================================================================================================
            DataSet ds = new DataSet();
            bool CheckMinusQty = false;
            bool IsMinusAllow = false;
            //=========================================
            try
            {
                String S3 = "select IsMinusAllow from tblDefualtSetting";// where ItemID = '" + Search.ItemId.ToString().Trim() + "'";
                SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
                if (dt3.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt3.Rows[0].ItemArray[0]) == true)
                    {
                        IsMinusAllow = true;
                    }
                    else
                    {
                        IsMinusAllow = false;
                    }

                }
            }
            catch { }
            //============================================
            try
            {
                String S1 = "delete from TrnQtyValidation";// where TaxCode = '" + txtTaxCode.Text.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                for (int b = 0; b < rowCount; b++)
                {
                    string ConnString = ConnectionString;
                    String S2 = "insert into TrnQtyValidation(TranitemID,TranOnhanQty,TranTranQty) values ('" + dgvIssueNote.Rows[b].Cells[0].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvIssueNote.Rows[b].Cells[2].Value) + "','" + Convert.ToDouble(dgvIssueNote.Rows[b].Cells[3].Value) + "')";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                }
            }
            catch
            {
                // chkNumber = 1;
            }
            //=================================================================
            try
            {
                for (int k = 0; k < rowCount; k++)
                {
                    String S = "Select TranOnhanQty,sum(TranTranQty) from TrnQtyValidation where TranitemID ='" + dgvIssueNote.Rows[k].Cells[0].Value.ToString().Trim() + "' group by TranitemID,TranOnhanQty";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    da.Fill(ds, "SO");
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        if (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]) > Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[0]))//Description )
                        {
                            CheckMinusQty = true;
                            if (IsMinusAllow == true)
                            {
                                CheckMinusQty = false;
                            }

                        }
                        else
                        {
                        }
                    }
                }
            }
            catch { }


            //if (CheckMinusQty == false)
            //{
                //===================================================

                if (dgvIssueNote.Rows.Count > 1)
                {
                    //----check the numaric convertions
                    try
                    {
                        for (int a = 0; a < rowCount; a++)
                        {
                            ChechDQty = 0;
                            Convert.ToDouble(dgvIssueNote.Rows[a].Cells[3].Value);
                            Convert.ToDouble(dgvIssueNote.Rows[a].Cells[5].Value);

                        }
                    }
                    catch
                    {
                        ChechDQty = 1;//if this flag is 1 the violate the number format
                    }

                    //---check wether have unfilld cells IN GRID
                    try
                    {
                        for (int k = 0; k < rowCount; k++)
                        {
                            CheckNull1 = Convert.ToDouble(dgvIssueNote.Rows[k].Cells[3].Value);
                            CheckNull = false;
                        }
                    }
                    catch
                    {
                        CheckNull = true;
                    }

                    if (ChechDQty == 0 && CheckNull == false && IsItemSerial == false && checkZeroIssue ==false && cmbjob.Text !="")//if number conversions are correct and not unfilld cells
                    {
                        int newItemQty = 0;

                        DateTime DTP = Convert.ToDateTime(dtpDate.Text);
                        string Dformat = "MM/dd/yyyy";
                        string issueDate = DTP.ToString(Dformat);

                        //save in tblIssueNote
                        int noOfDistribution = dgvIssueNote.Rows.Count - 1;


                        //********************
                        setConnectionString();
                        SqlConnection myConnection = new SqlConnection(ConnectionString);
                        SqlCommand myCommand = new SqlCommand();
                        SqlTransaction myTrans;
                        myConnection.Open();
                        myCommand.Connection = myConnection;

                        myTrans = myConnection.BeginTransaction();
                        myCommand.Transaction = myTrans;//start the trasaction scope

                        //********************
                        try
                        {
                            //****************
                            //SqlCommand cmd2 = new SqlCommand("UPDATE tblItemWhse  set QTY = QTY -'" + curbal + "' where  WhseId= '" + valFrom + "' and ItemId='" + dgvItem[0, c].Value + "' select QTY from tblItemWhse  where  WhseId= '" + valFrom + "' and ItemId='" + dgvItem[0, c].Value + "'", myConnection, myTrans);
                            //SqlDataAdapter da41 = new SqlDataAdapter(cmd2);
                            //DataTable dt41 = new DataTable();
                            //da41.Fill(dt41);


                            //myCommand.CommandText = "update tblDefualtSetting with(rowlock) set IssueNoteNo = IssueNoteNo + 1 select IssueNoteNo,IssuNotePrefix from tblDefualtSetting with(rowlock)";
                            SqlCommand cmd = new SqlCommand("update tblDefualtSetting with(rowlock) set FGTransNo = FGTransNo + 1 select FGTransNo,FGTransPrefix from tblDefualtSetting with(rowlock)", myConnection, myTrans);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                issueNoteNo = dt.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                                issueNoteNo = dt.Rows[0].ItemArray[1].ToString().Trim() + "-" + issueNoteNo;
                                txtIssueNoteId.Text = issueNoteNo;
                            }
                            //*************************
                            for (int i = 0; i < rowCount; i++)
                            {
                                int distributionNo = i + 1;
                                string UOM = "";
                                //try
                                //{
                                if (dgvIssueNote.Rows[i].Cells[4].Value.Equals(null))
                                {
                                    UOM = "-";
                                }
                                else
                                {
                                    UOM = dgvIssueNote.Rows[i].Cells[4].Value.ToString().Trim();
                                }
                                //}
                                //catch
                                //{
                                //    UOM = "-";
                                //}
                                string Towarehouse = "UnKnown";
                                string TranType = "FGTransfer";
                                //myCommand.CommandText = "insert into tblIssueNote(IssueNoteId,FrmWhseId,IDate,ItemId,ItemDis,QTY,UOM,DistributionNo,NoOfDistribution,WhseQty) values ('" + issueNoteNo + "','" + cmbWarehouse.Text.ToString().Trim() + "','" + issueDate + "','" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "','" + dgvIssueNote.Rows[i].Cells[1].Value.ToString().Trim() + "','" + dgvIssueNote.Rows[i].Cells[3].Value.ToString().Trim() + "','" + UOM + "','" + distributionNo + "','" + noOfDistribution + "','" + dgvIssueNote.Rows[i].Cells[2].Value.ToString().Trim() + "')";
                                SqlCommand cmd1 = new SqlCommand("insert into tblFGTransfer(FGTransNo,WarehouseID,TransferDate,ItemID,ItemDescription,Quantity,UOM,DistributionNo,NoOfDistribution,WarehouseQty,Job,UnitCost) values ('" + issueNoteNo + "','" + cmbWarehouse.Text.ToString().Trim() + "','" + issueDate + "','" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "','" + dgvIssueNote.Rows[i].Cells[1].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvIssueNote.Rows[i].Cells[3].Value) + "','" + UOM + "','" + distributionNo + "','" + noOfDistribution + "','" + Convert.ToDouble(dgvIssueNote.Rows[i].Cells[2].Value) + "','" + cmbjob.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvIssueNote.Rows[i].Cells[5].Value) + "')", myConnection, myTrans);
                                //SqlCommand cmd1 = new SqlCommand("insert into tblReturnNote(ReturnNoteNo,FrmWarehouseID,ReturnDate,ItemID,ItemDescription,Quantity,UOM,Distributionno,NoofDistribution,WarehouseQty,Job) values ('" + issueNoteNo + "','" + cmbWarehouse.Text.ToString().Trim() + "','" + issueDate + "','" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "','" + dgvIssueNote.Rows[i].Cells[1].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvIssueNote.Rows[i].Cells[3].Value) + "','" + UOM + "','" + distributionNo + "','" + noOfDistribution + "','" + Convert.ToDouble(dgvIssueNote.Rows[i].Cells[2].Value) + "','" + cmbjob.Text.ToString().Trim() + "')", myConnection, myTrans);

                                SqlDataAdapter dai = new SqlDataAdapter(cmd1);
                                DataTable dti = new DataTable();
                                dai.Fill(dti);

                                SqlCommand cmd7 = new SqlCommand("Insert into tblInvTransaction (TDate,ItemId,FrmWhseId,ToWhseId,QTY,TransType) values ('" + issueDate + "','" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "','" + cmbWarehouse.Text.ToString().Trim() + "','" + Towarehouse + "','" + dgvIssueNote.Rows[i].Cells[3].Value.ToString().Trim() + "','" + TranType + "')", myConnection, myTrans);
                                cmd7.ExecuteNonQuery();


                                SqlCommand myCommand5 = new SqlCommand("select QTY,UOM from tblItemWhse where ItemId = '" + mltcmbboxItemSelect.Text.Trim() + "' and WhseId = '" + cmbWarehouse.Text.ToString().Trim() + "'", myConnection, myTrans);
                                //myCommand2.ExecuteNonQuery();
                                SqlDataAdapter da5 = new SqlDataAdapter(myCommand5);
                                DataTable dt5 = new DataTable();
                                da5.Fill(dt5);
                                if (dt5.Rows.Count > 0)
                                {
                                    SqlCommand cmd2 = new SqlCommand("update tblItemWhse set QTY = QTY +'" + Convert.ToDouble(dgvIssueNote.Rows[i].Cells[3].Value) + "' where ItemId = '" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "' and WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'", myConnection, myTrans);
                                    cmd2.ExecuteNonQuery();
                                }
                                else
                                {
                                    SqlCommand cmd75 = new SqlCommand("Insert into tblItemWhse(WhseId,WhseName,ItemId,ItemDis,QTY,TraDate) values ('" + cmbWarehouse.Text.ToString().Trim() + "','" + txtWarehouseName.Text.ToString().Trim() + "','" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "','" + dgvIssueNote.Rows[i].Cells[1].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvIssueNote.Rows[i].Cells[3].Value) + "','" + issueDate + "')", myConnection, myTrans);
                                    cmd75.ExecuteNonQuery();
                                }

                                //update the Serial no Table of Issue Note==============================

                                SqlCommand myCommandSe = new SqlCommand("Select * from  tblSerialIssueNoteTemp where ItemID='" + dgvIssueNote[0, i].Value + "'", myConnection, myTrans);
                                //myCommand2.ExecuteNonQuery();
                                SqlDataAdapter daSe = new SqlDataAdapter(myCommandSe);
                                DataTable dtSe = new DataTable();
                                daSe.Fill(dtSe);
                                //Insert to serial numbers table to serial numbers whic are taken from the serialisetemp table============

                              //  string TranType = "IssueNote";
                                string Status = "Issue";
                                bool IsGRNProcess = true;
                                for (int j = 0; j < dtSe.Rows.Count; j++)
                                {
                                    // SqlCommand cmd6 = new SqlCommand("insert into tblWhseTransLine (WhseTransId,ItemId,ItemDis,QTY,UOM,FrmWhseQTY,ToWhseQTY) values ('" + textTrans.Text.ToString().Trim() + "','" + dgvItem[0, c].Value + "','" + dgvItem[1, c].Value + "','" + dgvItem[3, c].Value + "','" + dgvItem[5, c].Value + "','" + dgvItem[2, c].Value + "','" + dgvItem[4, c].Value + "')", myConnection, myTrans);

                                    //SqlCommand myCommandSe12 = new SqlCommand("insert into tblWareHouseSerialNO(TranNote,WarehouseID,TransDate,ItemID,SerialNO,TranType,Status)values ('" + textTrans.Text.ToString().Trim() + "','" + valTo + "','" + WHTDate + "','" + dgvItem[0, c].Value.ToString() + "','" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "','" + TranType + "','" + Status + "')", myConnection, myTrans);
                                    //myCommandSe12.ExecuteNonQuery();

                                    SqlCommand myCommandSe13 = new SqlCommand("insert into tblSerialIssueNote(ISNNO,WareHouseID,ItemID,SerialNO,TransactionType)values ('" + txtIssueNoteId.Text.ToString().Trim() + "','" + cmbWarehouse.Text.ToString().Trim() + "','" + dgvIssueNote[0, i].Value.ToString() + "','" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "','" + TranType + "')", myConnection, myTrans);
                                    myCommandSe13.ExecuteNonQuery();


                                    myCommand.CommandText = "Update tblSerialItemTransaction SET Status = '" + Status + "' where ItemID = '" + dgvIssueNote[0, i].Value.ToString() + "' and SerialNO='" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "'";
                                    myCommand.ExecuteNonQuery();



                                }

                                //======================================================================================




                                //update the item quantity in warehouse
                                //SqlCommand cmd5 = new SqlCommand("select QTY from tblItemWhse where ItemId = '" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "' ", myConnection, myTrans);

                                ////myCommand.CommandText = "select QTY from tblItemWhse where ItemId = '" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "' ";
                                //SqlDataAdapter da11 = new SqlDataAdapter(cmd5);
                                //DataTable dt11 = new DataTable();
                                //da11.Fill(dt11);
                                //if (dt11.Rows.Count > 0)
                                //{
                                //    int WhQty = Convert.ToInt32(dt11.Rows[0].ItemArray[0].ToString().Trim());
                                //    int IsQty = Convert.ToInt32(dgvIssueNote.Rows[i].Cells[3].Value.ToString().Trim());
                                //    newItemQty = WhQty - IsQty;
                                //}

                                //myCommand.CommandText = "update tblItemWhse set QTY = '" + newItemQty + "' where ItemId = '" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "'";
                                //SqlCommand cmd2 = new SqlCommand("update tblItemWhse set QTY = QTY -'" + Convert.ToDouble(dgvIssueNote.Rows[i].Cells[3].Value) + "' where ItemId = '" + dgvIssueNote.Rows[i].Cells[0].Value.ToString().Trim() + "' and WhseName='" + cmbWarehouse.Text.ToString().Trim() + "'", myConnection, myTrans);
                                //SqlDataAdapter da1 = new SqlDataAdapter(cmd2);
                                //DataTable dt1 = new DataTable();
                                //da1.Fill(dt1);
                                Search.searchIssueNoteNo = "0";
                            }
                            //dddd

                            //temporory stop tsending dta into peachtree
                            CreateXmlToExportInvAdjust(myTrans, myConnection);


                            //UserAutherization.Connector conn  = new Connector();
                            //conn.i
                            //Connector Conn = new Connector();
                            //Conn.IssueAdjustmentExport();
                          // Conn.im
                            // Conn.IssueAdjustmentExport();//call to connector class to import to adjustment
                            //Conn.inv
                            SqlCommand myCommand21 = new SqlCommand("delete from tblSerialIssueNoteTemp", myConnection, myTrans);
                            myCommand21.ExecuteNonQuery();
                            myTrans.Commit();
                            MessageBox.Show("Finished Goods Transfer successfully saved", "FG Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnPrint_Click(sender, e);
                            btnNew_Click(sender, e);

                            btnNew.Enabled = true;
                            Search.searchIssueNoteNo = "0";
                            //updateIssueNoteNo();
                            btnSave.Enabled = false;
                            btnPrint.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            myTrans.Rollback();
                            MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            btnSave.Enabled = true;

                        }
                        finally
                        {
                            myConnection.Close();
                        }


                    }
                    else if (ChechDQty == 1)
                    {
                        MessageBox.Show("Enter Valid Quantity or Cost");
                        btnSave.Focus();
                    }
                    else if (CheckNull == true)
                    {
                        MessageBox.Show("Please enter the Transfer Quantity", "FG Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (IsItemSerial == true)
                    {
                        //MessageBox.Show("You have not entered serial numbers for this items");
                        MessageBox.Show("There are Serialize Stock Items you have not entered serial numbers");
                        btnSave.Focus();
                    }
                    else if(checkZeroIssue==true)
                    {
                       MessageBox.Show("There are one or more item with Transfer quantity = 0");
                    }
                    else if (cmbjob.Text=="")
                    {
                        MessageBox.Show("You must select a Job");
                    }
                }
                else
                {
                    MessageBox.Show("Pleasen enter Transfer Note details for transaction", "FG Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            //}
            //else
            //{
            //    MessageBox.Show("Return Quantity canot be grater than the on hand quantity");

            //}
        }

        private void CreateXmlToExportInvAdjust(SqlTransaction myTrans, SqlConnection myConnection)
        {

            Connector Conn = new Connector();
           // Conn.i
           // object.
          //  conn
         // Conn.ImportItemUnitCost 
          // Conn.impo
            //string GLSourceAccount = "";
            //SqlCommand myCommand4 = new SqlCommand("Select SalesGLAccount,UnitPrice from tblItemMaster", myConnection, myTrans);
            //SqlDataAdapter da = new SqlDataAdapter(myCommand4);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //if (dt.Rows.Count > 0)
            //{
            //    GLSourceAccount = dt.Rows[0].ItemArray[0].ToString().Trim();
            //}

            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string reference = txtIssueNoteId.Text.ToString().Trim();
            string Dformat = "MM/dd/yyyy";
            string IssueDate = DTP.ToString(Dformat);


            XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\IssueAdjustment.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


            //XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\IssueAdjustment.xml", System.Text.Encoding.UTF8);
            //Writer.Formatting = Formatting.Indented;


//            <PAW_Items xmlns:paw="urn:schemas-peachtree-com/paw8.02-datatypes"
//xmlns:xsi="http://www.w3.org/2000/10/XMLSchema-instance"
//xmlns:xsd="http://www.w3.org/2000/10/XMLSchema-datatypes">

            //Writer.WriteStartElement("PAW_Items");
            //Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            //Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            //Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


            int GridrowCount = GetFilledRows();//get row count from data grid

            //create a start elemet=========================

            for (int k = 0; k < GridrowCount; k++)
            {


                //SqlCommand myCommand45 = new SqlCommand("Select SerialNO from tblSerialIssueNote where ISNNO='" + txtIssueNoteId.Text.ToString().Trim() + "' and ItemID='" + dgvIssueNote.Rows[k].Cells[0].Value.ToString() + "'", myConnection, myTrans);
                //// SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da1 = new SqlDataAdapter(myCommand45);
                //DataSet dt1 = new DataSet();
                //da1.Fill(dt1);


                //if (dt1.Tables[0].Rows.Count > 0)
                //{
                //    for (int j = 0; j < dt1.Tables[0].Rows.Count; j++)
                //    {
                        double IssQty = Convert.ToDouble(dgvIssueNote[3, k].Value);
                        if (IssQty != 0)
                        {
                            Writer.WriteStartElement("PAW_Item");
                            Writer.WriteAttributeString("xsi:type", "paw:item");
                           // Writer.WriteEndElement();//get Last Unit Cost
                            //===================================================
                            string GLSourceAccount = "";
                            SqlCommand myCommand4 = new SqlCommand("Select SalesGLAccount,UnitPrice from tblItemMaster where ItemID='" + dgvIssueNote[0, k].Value.ToString().Trim() + "'", myConnection, myTrans);
                            SqlDataAdapter da = new SqlDataAdapter(myCommand4);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            //==================================
                            if (dt.Rows.Count > 0)
                            {
                                GLSourceAccount = dt.Rows[0].ItemArray[0].ToString().Trim();
                            }


                            Conn.ImportItemUnitCost(dgvIssueNote[0, k].Value.ToString().Trim());
                           // Writer.WriteEndElement();//get Last Unit Cost

                            //crate a ID element (tag)=====================(1)
                            Writer.WriteStartElement("ID");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(dgvIssueNote[0, k].Value.ToString().Trim());//dgvItem[0, c].Value
                            Writer.WriteEndElement();


                            //this sis crating tag for reference============(2)
                            Writer.WriteStartElement("Reference");
                            Writer.WriteString(reference);
                            Writer.WriteEndElement();

                            //This is a Tag for Adjusment Date==============(3)
                            Writer.WriteStartElement("Date ");
                            Writer.WriteAttributeString("xsi:type", "paw:date");
                            Writer.WriteString(IssueDate);//Date format must be (MM/dd/yyyy)
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("JobID");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(cmbjob.Text.ToString().Trim());
                            //Writer.WriteString(Amount.ToString().Trim());
                            Writer.WriteEndElement();

                                                      

                           

                            //This is a Tag for numberof dsistribution=======(4)

                            Writer.WriteStartElement("Number_of_Distributions ");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            //Adjustmne Lines=================================(5)
                            Writer.WriteStartElement("AdjustmentItems");
                            //Adjustmne Lines=================================(6)
                            Writer.WriteStartElement("AdjustmentItem");


                            //Gl ASccount======================================(7)
                            Writer.WriteStartElement("GLSourceAccount ");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            //Writer.WriteString(GLSourceAccount);
                            Writer.WriteString("60100");
                            Writer.WriteEndElement();


                            //Unit Cost========================================(8)
                            //Writer.WriteStartElement("UnitCost");
                            //Writer.WriteString(dgvGRNTransaction[5, k].Value.ToString().Trim());
                            //Writer.WriteEndElement();

                            //string LastUnitCost = "0";
                            ////String S1 = "Select LastUnitCost from tblLastUnitCost where ItemID='" + dgvItemList[0, k].Value.ToString().Trim() + "'";
                            //SqlCommand cmd1 = new SqlCommand("Select LastUnitCost from tblLastUnitCost where ItemID='" + dgvIssueNote[0, k].Value.ToString().Trim() + "'", myConnection, myTrans);
                            //SqlDataAdapter da12 = new SqlDataAdapter(cmd1);
                            //DataSet dt12= new DataSet();
                            //da12.Fill(dt12);
                           // LastUnitCost = dt12.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                           // LastUnitCost = dt12.Tables[0].Rows[0].ItemArray[0].ToString().Trim();

                            Writer.WriteStartElement("UnitCost");
                            Writer.WriteString(dgvIssueNote[5, k].Value.ToString().Trim());
                            //Writer.WriteString(LastUnitCost);
                            Writer.WriteEndElement();




                            Writer.WriteStartElement("Quantity");
                            // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
                            //Writer.WriteString(IssQty.ToString().Trim());
                            Writer.WriteString(IssQty.ToString().Trim());
                            //Adjust_qty
                            Writer.WriteEndElement();

                            double Amount = IssQty * Convert.ToDouble(dgvIssueNote[5, k].Value);


                            Writer.WriteStartElement("Amount");
                            Writer.WriteString(Amount.ToString().Trim());
                            Writer.WriteEndElement();


                            //Quantity========================================(9)
                            //if (IssQty >= 0)
                            //{
                            //    Writer.WriteStartElement("Quantity");
                            //    // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
                            //    Writer.WriteString("-" + IssQty.ToString().Trim());
                            //    //Adjust_qty
                            //    Writer.WriteEndElement();
                            //}
                            //else
                            //{
                            //    Writer.WriteStartElement("Quantity");
                            //    // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
                            //    Writer.WriteString((IssQty * -1).ToString().Trim());
                            //    //Adjust_qty
                            //    Writer.WriteEndElement();
                            //}
                            //Amount===========================================(10)
                            //double unitPrice = 0.00;
                            //if (dt.Rows.Count > 0)
                            //{
                            //    if (dt.Rows[0].ItemArray[1].ToString() != "")
                            //    {
                            //        unitPrice = Convert.ToDouble(dt.Rows[0].ItemArray[1].ToString().Trim());
                            //    }
                            //}
                            //double Amount = IssQty * unitPrice;
                            //if (Amount >= 0)
                            //{
                            //    Writer.WriteStartElement("Amount");
                            //    Writer.WriteString("-" + Amount.ToString().Trim());
                            //    Writer.WriteEndElement();
                            //}
                            //if (Amount < 0)
                            //{
                            //    Writer.WriteStartElement("Amount");
                            //    Writer.WriteString(Amount.ToString().Trim());
                            //    Writer.WriteEndElement();
                            //}

                            //Writer.WriteStartElement("ReasonToAdjust");
                            //Writer.WriteString("Issue");
                            //Writer.WriteEndElement();


                            //===============================================

                            //Writer.WriteStartElement("Serial_Number");
                            //// Writer.WriteString(POQuantity.Tables[0].Rows[i].ItemArray[2].ToString().Trim());
                            //Writer.WriteString(dt1.Tables[0].Rows[j].ItemArray[0].ToString().Trim());
                            //Writer.WriteEndElement();
                            //================================================


                            Writer.WriteEndElement();//Adjustment Line
                            Writer.WriteEndElement();//Adjustment lines

                            //Writer.WriteStartElement("Serial_Number");
                            //// Writer.WriteString(POQuantity.Tables[0].Rows[i].ItemArray[2].ToString().Trim());
                            ////Writer.WriteString(dt1.Tables[0].Rows[j].ItemArray[0].ToString().Trim());
                            //Writer.WriteString("");
                            //Writer.WriteEndElement();


                            Writer.WriteEndElement();//Item Closed Tag

                            // Writer.WriteEndElement();//Items Closed Tag

                            //   Writer.Close();//finishing writing xml file
                        }
                //    }
                //}

            }

            Writer.Close();//finishing writing xml file

           //Conno
           // Connector Conn = new Connector();
            Conn.IssueAdjustmentExport();
           // ObjConn.IssueAdjustmentExport();
        }

        private void updateIssueNoteNo()
        {
            try
            {
                //update default settings table
                string s1 = "update tblDefualtSetting set IssueNoteNo = '" + newIssueNoteNo + "'";
                SqlDataAdapter da1 = new SqlDataAdapter(s1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }
        }

        private void getIssueNoteNo()
        {
            ////"UPDATE tblDefualtSetting with (rowlock) SET SupplierReturnNo = SupplierReturnNo + 1 select suplierReturnNo, SupReturnPrefix from tblDefualtSetting with (rowlock)";
            //try
            //{
            //    string s = "update IssueNoteNo with(rowlock) set IssueNoteNo = IssueNoteNo + 1 select IssueNoteNo from tblDefualtSetting with(rowlock)";
            //    //string s = "select IssueNoteNo from tblDefualtSetting";
            //    SqlDataAdapter da41 = new SqlDataAdapter(myCommand.CommandText,ConnectionString);
            //    SqlDataAdapter da = new SqlDataAdapter(s,ConnectionString);
            //    DataTable dt = new DataTable();
            //    da.Fill(dt);
            //    if (dt.Rows.Count > 0)
            //    {
            //        issueNoteNo = Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString().Trim());
            //        txtIssueNoteId.Text = issueNoteNo.ToString();
            //        newIssueNoteNo = issueNoteNo + 1;
            //    }
            //}
            //catch {}
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
               // frmReturnNoteList 


                frmFGTransferList ObjFGTranslist = new frmFGTransferList();
                ObjFGTranslist.Show();


                //frmReturnNoteList ObjRnotelist = new frmReturnNoteList();
                //ObjRnotelist.ShowDialog();
                //frmIssueNoteSearch issueSearch = new frmIssueNoteSearch();
                //issueSearch.ShowDialog();
            }
            catch { }
        }

        private void dgvIssueNote_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

            //    try
            //    {
            //        if (dgvIssueNote.CurrentCell.ColumnIndex == 3)
            //        {
            //            double onHandQty = Convert.ToDouble(dgvIssueNote[2, dgvIssueNote.CurrentRow.Index].Value);
            //            double issueQty = Convert.ToDouble(dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value);
            //            //int issueQty = Convert.ToInt32(dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value.ToString());
            //            if (issueQty > onHandQty)
            //            {
            //                string s1 = "select IsMinusAllow from tblDefualtSetting";
            //                 SqlDataAdapter da1 = new SqlDataAdapter(s1, ConnectionString);
            //                DataTable dt1 = new DataTable();
            //                da1.Fill(dt1);
            //                if (dt1.Rows[0].ItemArray[0].ToString().Trim() == "True")
            //                {
            //                    double  varience = onHandQty - issueQty;
            //                }
            //                else
            //                {
            //                    MessageBox.Show("System does not allow minus quantity");
            //                    dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value = "";
            //                }
            //            }
            //        }
            //    }
            //    catch { }
        }
        
      

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ObjRetrnNoteDS.Clear();
               // a.Dt_IssueNote.Clear();
                string s5 = "Select * from tblFGTransfer where FGTransNo = '" + txtIssueNoteId.Text.ToString().Trim() + "'";
                SqlConnection conn5 = new SqlConnection(ConnectionString);
                SqlDataAdapter da5 = new SqlDataAdapter(s5, conn5);
                da5.Fill(ObjRetrnNoteDS, "DTFGTansfer");


                frmFGTransferPrint objFGtransPrint = new frmFGTransferPrint(this);
                objFGtransPrint.Show();

                //frmPrintReturnNote ObjPrintRetrnNote = new frmPrintReturnNote(this);
                //ObjPrintRetrnNote.Show();

                //frmReportViewerIssueNote issueNote = new frmReportViewerIssueNote(this);
                //issueNote.Show();
            }
            catch { }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {

            //Connector conn = new Connector();
            //// conn.IssueAdjustmentExportGetFromP();
            //conn.IssueAdjustmentExport();
            //cmbWarehouse.SelectedItem = null;
            //dgvIssueNote.Rows.Clear();
            //txtWarehouseName.Text = "";
            //txtWarehouseAddress.Text = "";
        }

        public bool ISSAVE = false;//if save this variable ge true
        bool formload = false;
        // bool formload = true;
        int currentRowIndex1;
        int k = 0;

        private void mltcmbboxItemSelect_KeyDown(object sender, KeyEventArgs e)
        {
            //string[] CsFrom1 = combo_Whse_from.SelectedItem.ToString().Trim().Split('-');
            //string valFrom1 = CsFrom1[0];
            //string[] CsTo1 = combo_Whse_to.SelectedItem.ToString().Trim().Split('-');
            //string valTo1 = CsTo1[0];
            if (e.KeyCode == Keys.Enter)
            {
                if (formload == false)
                {
                    k++;
                    try
                    {
                        if (ISSAVE != true)
                        {
                            if (k >= 1)
                            {
                                if (currentRowIndex1 - 1 != -1)
                                {
                                    if (dgvIssueNote[0, currentRowIndex1 - 1].Value != null)
                                    {
                                        dgvIssueNote.Rows.Add();
                                        dgvIssueNote[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
                                        dgvIssueNote[1, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

                                        //Fill Item Description to grid
                                        String S = "select ItemDescription from tblItemMaster where ItemID = '" + mltcmbboxItemSelect.Text.Trim() + "' ";
                                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                        DataTable dt = new DataTable();
                                        da.Fill(dt);
                                        if (dt.Rows.Count > 0)
                                        {
                                            dgvIssueNote[1, currentRowIndex1].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                        }

                                        //Fill QOH
                                        String S1 = "select QTY,UOM from tblItemWhse where ItemId = '" + mltcmbboxItemSelect.Text.Trim() + "' and WhseId = '" + cmbWarehouse.Text.ToString().Trim() + "' ";
                                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                                        DataTable dt1 = new DataTable();
                                        da1.Fill(dt1);

                                        if (dt.Rows.Count > 0)
                                        {
                                            dgvIssueNote[1, currentRowIndex1].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                        }

                                        if (dt1.Rows.Count > 0)
                                        {
                                            if (dt1.Rows[0].ItemArray[0] == "")
                                            {
                                                dgvIssueNote[2, currentRowIndex1].Value = "0";
                                            }
                                            else
                                            {
                                                dgvIssueNote[2, currentRowIndex1].Value = dt1.Rows[0].ItemArray[0].ToString().Trim();
                                            }
                                            dgvIssueNote[3, currentRowIndex1].Value = "0";
                                            dgvIssueNote[4, currentRowIndex1].Value = dt1.Rows[0].ItemArray[1].ToString().Trim();

                                        }
                                        else
                                        {
                                            dgvIssueNote[2, currentRowIndex1].Value = "0";
                                            dgvIssueNote[3, currentRowIndex1].Value = "0";
                                            dgvIssueNote[4, currentRowIndex1].Value = "";
                                        }


                                        //String S = "Select UOM from tblItemMaster where ItemID='" + mltcmbboxItemSelect.Text.Trim() + "'";
                                        //SqlCommand cmd = new SqlCommand(S);
                                        //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                        //DataSet dt = new DataSet();
                                        //da.Fill(dt);
                                        //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                                        //{
                                       // dgvIssueNote[4, currentRowIndex1].Value = dt.Rows[0].ItemArray[1].ToString().Trim();
                                      //  dgvIssueNote[3, currentRowIndex1].Value = "0";
                                        //String S6 = "select * from tblItemWhse where WhseId='" + valFrom1.Trim() + "' and ItemId='" + dgvItem[0, i].Value + "'";
                                        //SqlDataAdapter da6 = new SqlDataAdapter(S6, ConnectionString);
                                        //DataTable dt6 = new DataTable();
                                        //da6.Fill(dt6);

                                        //String S611 = "select * from tblItemWhse where WhseId='" + valTo1.Trim() + "' and ItemId='" + dgvItem[0, i].Value + "'";
                                        //SqlDataAdapter da611 = new SqlDataAdapter(S611, ConnectionString);
                                        //DataTable dt611 = new DataTable();
                                        //da611.Fill(dt611);

                                        //if (dt6.Rows.Count > 0)
                                        //{
                                        //    dgvItem[2, currentRowIndex1].Value = dt6.Rows[i].ItemArray[4].ToString().Trim();
                                        //}
                                        //else
                                        //{
                                        //    dgvItem[2, currentRowIndex1].Value = 0;
                                        //}

                                        //if (dt611.Rows.Count > 0)
                                        //{
                                        //    dgvItem[4, currentRowIndex1].Value = dt611.Rows[0].ItemArray[4].ToString().Trim();
                                        //}
                                        //else
                                        //{
                                        //    dgvItem[4, currentRowIndex1].Value = 0;
                                        //}
                                        //}
                                        mltcmbboxItemSelect.Visible = false;
                                        if (k > 4)
                                        {
                                            dgvIssueNote.Rows.Add();
                                        }

                                    }
                                }
                                else
                                {
                                    dgvIssueNote.Rows.Add();
                                    dgvIssueNote[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
                                    dgvIssueNote[1, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

                                    //Fill Item Description to grid
                                    String S = "select ItemDescription from tblItemMaster where ItemID = '" + mltcmbboxItemSelect.Text.Trim() + "' ";
                                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                    DataTable dt = new DataTable();
                                    da.Fill(dt);
                                    if (dt.Rows.Count > 0)
                                    {
                                        dgvIssueNote[1, currentRowIndex1].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                    }

                                    //Fill QOH
                                    String S1 = "select QTY,UOM from tblItemWhse where ItemId = '" + mltcmbboxItemSelect.Text.Trim() + "' and WhseId = '" + cmbWarehouse.Text.ToString().Trim() + "' ";
                                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                                    DataTable dt1 = new DataTable();
                                    da1.Fill(dt1);

                                    if (dt.Rows.Count > 0)
                                    {
                                        dgvIssueNote[1, currentRowIndex1].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                    }

                                    if (dt1.Rows.Count > 0)
                                    {
                                        if (dt1.Rows[0].ItemArray[0] == "")
                                        {
                                            dgvIssueNote[2, currentRowIndex1].Value = "0";
                                        }
                                        else
                                        {
                                            dgvIssueNote[2, currentRowIndex1].Value = dt1.Rows[0].ItemArray[0].ToString().Trim();
                                        }
                                        dgvIssueNote[3, currentRowIndex1].Value = "0";
                                        dgvIssueNote[4, currentRowIndex1].Value = dt1.Rows[0].ItemArray[1].ToString().Trim();

                                    }
                                    else
                                    {
                                        dgvIssueNote[2, currentRowIndex1].Value = "0";
                                        dgvIssueNote[3, currentRowIndex1].Value = "0";
                                        dgvIssueNote[4, currentRowIndex1].Value = "";
                                    }
                                  //  dgvIssueNote[3, currentRowIndex1].Value = "0";


                                    //String S = "Select UOM from tblItemMaster where ItemID='" + mltcmbboxItemSelect.Text.Trim() + "'";
                                    //SqlCommand cmd = new SqlCommand(S);
                                    //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                    //DataSet dt = new DataSet();
                                    //da.Fill(dt);
                                    //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                                    //{
                                    //dgvIssueNote[4, currentRowIndex1].Value = dt.Rows[0].ItemArray[1].ToString().Trim();

                                    //String S6 = "select * from tblItemWhse where WhseId='" + valFrom1.Trim() + "' and ItemId='" + dgvItem[0, i].Value + "'";
                                    //SqlDataAdapter da6 = new SqlDataAdapter(S6, ConnectionString);
                                    //DataTable dt6 = new DataTable();
                                    //da6.Fill(dt6);

                                    //String S611 = "select * from tblItemWhse where WhseId='" + valTo1.Trim() + "' and ItemId='" + dgvItem[0, i].Value + "'";
                                    //SqlDataAdapter da611 = new SqlDataAdapter(S611, ConnectionString);
                                    //DataTable dt611 = new DataTable();
                                    //da611.Fill(dt611);

                                    //if (dt6.Rows.Count > 0)
                                    //{
                                    //    dgvItem[2, currentRowIndex1].Value = dt6.Rows[i].ItemArray[4].ToString().Trim();
                                    //}
                                    //else
                                    //{
                                    //    dgvItem[2, currentRowIndex1].Value = 0;
                                    //}

                                    //if (dt611.Rows.Count > 0)
                                    //{
                                    //    dgvItem[4, currentRowIndex1].Value = dt611.Rows[0].ItemArray[4].ToString().Trim();
                                    //}
                                    //else
                                    //{
                                    //    dgvItem[4, currentRowIndex1].Value = 0;
                                    //}
                                    //}
                                    mltcmbboxItemSelect.Visible = false;
                                    if (k > 4)
                                    {
                                        dgvIssueNote.Rows.Add();
                                    }


                                    mltcmbboxItemSelect.Visible = false;
                                    if (k > 5)// old value is 4 change in to  as 5
                                    {
                                        dgvIssueNote.Rows.Add();
                                    }
                                }
                            }
                        }
                        else
                        { }
                        dgvIssueNote.Focus();
                        dgvIssueNote.CurrentCell = dgvIssueNote[3, currentRowIndex1];

                       // dgvIssueNote.Focus();

                    }
                    catch
                    { }
                }
            }

            try
            {
                int rowCount = GetFilledRows();
                for (int a = rowCount; a < dgvIssueNote.Rows.Count; a++)
                {
                    dgvIssueNote.Rows.RemoveAt(a);
                }
            }
            catch { }
        }

        private void mltcmbboxItemSelect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (formload == false)
            {
                k++;
                try
                {
                    if (ISSAVE != true)
                    {
                        if (k >= 1)
                        {
                            if (currentRowIndex1 - 1 != -1)
                            {
                                if (dgvIssueNote[0, currentRowIndex1 - 1].Value != null)
                                {
                                    dgvIssueNote.Rows.Add();
                                    dgvIssueNote[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
                                    dgvIssueNote[1, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

                                    //Fill Item Description to grid
                                    String S = "select ItemDescription from tblItemMaster where ItemID = '" + mltcmbboxItemSelect.Text.Trim() + "' ";
                                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                    DataTable dt = new DataTable();
                                    da.Fill(dt);
                                    if (dt.Rows.Count > 0)
                                    {
                                        dgvIssueNote[1, currentRowIndex1].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                    }

                                    //Fill QOH
                                    String S1 = "select QTY,UOM from tblItemWhse where ItemId = '" + mltcmbboxItemSelect.Text.Trim() + "' and WhseId = '" + cmbWarehouse.Text.ToString().Trim() + "' ";
                                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                                    DataTable dt1 = new DataTable();
                                    da1.Fill(dt1);

                                    if (dt.Rows.Count > 0)
                                    {
                                        dgvIssueNote[1, currentRowIndex1].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                    }

                                    //if (dt1.Rows.Count > 0)
                                    //{
                                    //    dgvIssueNote[2, currentRowIndex1].Value = dt1.Rows[0].ItemArray[0].ToString().Trim();
                                    //    dgvIssueNote[4, currentRowIndex1].Value = dt1.Rows[0].ItemArray[1].ToString().Trim();

                                    //}
                                    //else
                                    //{
                                    //    dgvIssueNote[2, currentRowIndex1].Value = "";
                                    //    dgvIssueNote[4, currentRowIndex1].Value = "";
                                    //}
                                    if (dt1.Rows.Count > 0)
                                    {
                                        if (dt1.Rows[0].ItemArray[0] == "")
                                        {
                                            dgvIssueNote[2, currentRowIndex1].Value = "0";
                                        }
                                        else
                                        {
                                            dgvIssueNote[2, currentRowIndex1].Value = dt1.Rows[0].ItemArray[0].ToString().Trim();
                                        }
                                        dgvIssueNote[3, currentRowIndex1].Value = "0";
                                        dgvIssueNote[4, currentRowIndex1].Value = dt1.Rows[0].ItemArray[1].ToString().Trim();

                                    }
                                    else
                                    {
                                        dgvIssueNote[2, currentRowIndex1].Value = "0";
                                        dgvIssueNote[3, currentRowIndex1].Value = "0";
                                        dgvIssueNote[4, currentRowIndex1].Value = "";
                                    }


                                    dgvIssueNote[4, currentRowIndex1].Value = dt.Rows[0].ItemArray[1].ToString().Trim();
                                    dgvIssueNote[3, currentRowIndex1].Value = "0";
                                    mltcmbboxItemSelect.Visible = false;
                                    if (k > 4)
                                    {
                                        dgvIssueNote.Rows.Add();
                                    }

                                }
                            }
                            else
                            {
                                dgvIssueNote.Rows.Add();
                                dgvIssueNote[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
                                dgvIssueNote[1, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

                                //Fill Item Description to grid
                                String S = "select ItemDescription from tblItemMaster where ItemID = '" + mltcmbboxItemSelect.Text.Trim() + "' ";
                                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    dgvIssueNote[1, currentRowIndex1].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                }
                                //Fill QOH
                                String S1 = "select QTY,UOM from tblItemWhse where ItemId = '" + mltcmbboxItemSelect.Text.Trim() + "' and WhseId = '" + cmbWarehouse.Text.ToString().Trim() + "' ";
                                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                                DataTable dt1 = new DataTable();
                                da1.Fill(dt1);

                                //if (dt.Rows.Count > 0)
                                //{
                                //    dgvIssueNote[1, currentRowIndex1].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                //}

                                //if (dt1.Rows.Count > 0)
                                //{
                                //    dgvIssueNote[2, currentRowIndex1].Value = dt1.Rows[0].ItemArray[0].ToString().Trim();
                                //    dgvIssueNote[4, currentRowIndex1].Value = dt1.Rows[0].ItemArray[1].ToString().Trim();

                                //}
                                if (dt1.Rows.Count > 0)
                                {
                                    if (dt1.Rows[0].ItemArray[0] == "")
                                    {
                                        dgvIssueNote[2, currentRowIndex1].Value = "0";
                                    }
                                    else
                                    {
                                        dgvIssueNote[2, currentRowIndex1].Value = dt1.Rows[0].ItemArray[0].ToString().Trim();
                                    }
                                    dgvIssueNote[3, currentRowIndex1].Value = "0";
                                    dgvIssueNote[4, currentRowIndex1].Value = dt1.Rows[0].ItemArray[1].ToString().Trim();

                                }
                                else
                                {
                                    dgvIssueNote[2, currentRowIndex1].Value = "0";
                                    dgvIssueNote[3, currentRowIndex1].Value = "0";
                                    dgvIssueNote[4, currentRowIndex1].Value = "";
                                }
                                dgvIssueNote[3, currentRowIndex1].Value = "0";
                                mltcmbboxItemSelect.Visible = false;
                                if (k > 4)
                                {
                                    dgvIssueNote.Rows.Add();
                                }


                                mltcmbboxItemSelect.Visible = false;
                                if (k > 5)// old value is 4 change in to  as 5
                                {
                                    dgvIssueNote.Rows.Add();
                                }
                            }
                        }
                    }
                    else
                    { }
                    dgvIssueNote.Focus();

                }
                catch
                { }
                mltcmbboxItemSelect.Visible = false;
                dgvIssueNote.CurrentCell = dgvIssueNote[3, currentRowIndex1];
            }

            try
            {
                int rowCount = GetFilledRows();
                for (int a = rowCount; a < dgvIssueNote.Rows.Count; a++)
                {
                    dgvIssueNote.Rows.RemoveAt(a);
                }
            }
            catch { }
            //}
        }


        int clickCount = 0;
        Point defaultLocation1;
        private void dgvIssueNote_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            mltcmbboxItemSelect.Visible = false;
            clickCount++;
            try
            {
                if (clickCount >= 1)//previous value is > now >=
                {
                    int y;
                    //defaultLocation1 = ;
                    Rectangle rect = this.dgvIssueNote.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    //mltcmbboxItemSelect.Location = rect.Location;
                    mltcmbboxItemSelect.Location = new System.Drawing.Point(rect.Location.X + 8, rect.Location.Y + 123);
                    //this.mltcmbboxItemSelect.Location = new System.Drawing.Point(defaultLocation1.X, defaultLocation1.Y);

                    if (e.ColumnIndex == 0 && e.RowIndex != -1)
                    {
                        currentRowIndex1 = e.RowIndex;
                        mltcmbboxItemSelect.Visible = true;
                        {
                            //y = mltcmbboxItemSelect.Location.Y + calculateRowsHeightForTRN();
                            //this.mltcmbboxItemSelect.Location = new System.Drawing.Point(10, y);
                            Rectangle rect2 = this.dgvIssueNote.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                            // mltcmbboxItemSelect.Location = new System.Drawing.Point(rect2.Location.X + 25, rect2.Location.Y + 52);
                            mltcmbboxItemSelect.Location = new System.Drawing.Point(10, rect2.Location.Y + 123);
                            //  mltcmbboxItemSelect.Location = rect2.Location;
                            //this.mltcmbboxItemSelect.Location = new System.Drawing.Point(12, y);

                        }
                    }

                    else if (dgvIssueNote[0, currentRowIndex1 - 1].Value != null)
                    {
                        //this.mltcmbboxItemSelect.Location = new System.Drawing.Point(defaultLocation1.X, defaultLocation1.Y);
                        Rectangle rect1 = this.dgvIssueNote.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        mltcmbboxItemSelect.Location = new System.Drawing.Point(rect1.Location.X + 8, rect1.Location.Y + 123);
                        //mltcmbboxItemSelect.Location = rect1.Location;
                        mltcmbboxItemSelect.Visible = false;
                    }
                    else
                        mltcmbboxItemSelect.Visible = false;

                }
            }
            catch
            {
            }
        }

        private void mltcmbboxItemSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            //mltcmbboxItemSelect.Visible = false;
            //mltcmbboxItemSelect.Focus();
        }

        private void dgvIssueNote_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvIssueNote.CurrentCell.ColumnIndex == 3)
                {
                    string validateC = "";
                    int valid = 0;
                    //*******Enter only numbers

                    if (dgvIssueNote[3, dgvIssueNote.CurrentCell.RowIndex].Value != null)
                    {
                        validateC = dgvIssueNote[3, dgvIssueNote.CurrentCell.RowIndex].Value.ToString();

                        Regex regex = new Regex("[0-9]");
                        if (regex.IsMatch(validateC))
                        {
                            valid = 1;
                        }
                        else
                        {
                            valid = 0;
                            MessageBox.Show("Enter Only Numbers", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value = "";
                        }
                    }
                    //*************************



                    double onHandQty = Convert.ToDouble(dgvIssueNote[2, dgvIssueNote.CurrentRow.Index].Value);
                    double issueQty = Convert.ToDouble(dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value);
                    //int issueQty = Convert.ToInt32(dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value.ToString());
                    //if (issueQty > onHandQty)
                    //{
                    //    string s1 = "select IsMinusAllow from tblDefualtSetting";
                    //    SqlDataAdapter da1 = new SqlDataAdapter(s1, ConnectionString);
                    //    DataTable dt1 = new DataTable();
                    //    da1.Fill(dt1);
                    //    if (dt1.Rows[0].ItemArray[0].ToString().Trim() == "True")
                    //    {
                    //        double varience = onHandQty - issueQty;
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("System does not allow minus quantity");
                    //        dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value = "";
                    //    }
                    //}
                    if (issueQty < 0)
                    {
                        string s11 = "select IsMinusAllow from tblDefualtSetting";
                        SqlDataAdapter da11 = new SqlDataAdapter(s11, ConnectionString);
                        DataTable dt11 = new DataTable();
                        da11.Fill(dt11);
                        if (dt11.Rows[0].ItemArray[0].ToString().Trim() == "False")
                        {
                            MessageBox.Show("System does not allow minus quantity");
                            dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value = "";
                        }
                    }


                    //--------------

                    //                    double out1 = 0.0;
                    // if ((dgvIssueNote.Columns(e.ColumnIndex).Name == "Column2") && (!double.TryParse(dgvIssueNote[3,dgvIssueNote.CurrentRow.Index].Value.ToString(), out1)))

                    //{
                    ////not a double value. Tell the user.
                    //                dgvIssueNote(e.ColumnIndex, e.RowIndex).Value = 0;
                    //                MessageBox.Show("Please enter numeric values");
                    //        }
                    //---------------
                }
            }
            catch { }
        }

        private void cmbWarehouse_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dgvIssueNote.Rows.Clear();
                String S2 = "select * from tblWhseMaster where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    txtWarehouseName.Text = dt2.Rows[0].ItemArray[1].ToString();
                    txtWarehouseAddress.Text = dt2.Rows[0].ItemArray[2].ToString();
                }
            }
            catch { }
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            Search.searchIssueNoteNo = "0";
        }
        public bool flag = false;
        public CSInvoiceSerial ObjSerialInvoice = new CSInvoiceSerial();

        private void btnSNO_Click(object sender, EventArgs e)
        {

           // ConnIssueAdjustmentExportGetFromP();
            try
            {
                int rowCount = GetFilledRows();
                if (rowCount == 0 || Convert.ToDouble(dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value) == 0)
                {
                    if (rowCount == 0)
                    {
                        MessageBox.Show("please Select a line That contain Serialized Item");
                    }
                    if (Convert.ToDouble(dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value) == 0)
                    {
                        MessageBox.Show("please Enter return Quantity");
                    }
                }
                else
                {
                    // string[] AAA = new string[2];
                    // string AAA = "";
                    // AAA = chkSupplierInvoices.SelectedItems[0].ToString().Split(':');

                    string ItemID = dgvIssueNote[0, dgvIssueNote.CurrentRow.Index].Value.ToString().Trim();
                    //check wether this item is serial ior not  by classs
                    string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + ItemID + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);

                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                    }

                    if (ItemClass == "10" || ItemClass == "11")
                    {
                        string ItemDescription = dgvIssueNote[1, dgvIssueNote.CurrentRow.Index].Value.ToString().Trim();
                        double ReceivedQty = Convert.ToDouble(dgvIssueNote[3, dgvIssueNote.CurrentRow.Index].Value);
                        string Location = cmbWarehouse.Text.ToString().Trim();
                        bool IsSearch = flag;
                        //CSInvoiceSerial 
                        ClassDriiDown.SerialIsSearch = ObjSerialInvoice.GetCheckSearch(IsSearch);
                        // textTrans.Text 
                        if (txtIssueNoteId.Text != "")
                        {
                            string GRNNO = txtIssueNoteId.Text.ToString().Trim();
                            ClassDriiDown.SerialGRNNO = ObjSerialInvoice.GetGRNNO(GRNNO);
                        }
                        //GetGRNNO
                        // ClassDriiDown.PrevGRNNO = ObjSerialInvoice.GetPrevGRN(PrevGRN);
                        ClassDriiDown.SerialItemID = ObjSerialInvoice.GetItemId(ItemID);
                        ClassDriiDown.SerialDescription = ObjSerialInvoice.GetItemDescription(ItemDescription);
                        ClassDriiDown.ReceivedQty = ObjSerialInvoice.GetRewceivedQty(ReceivedQty);
                        ClassDriiDown.SerialLocation = ObjSerialInvoice.GetLocation(Location);


                        //frmSerialTransfer 
                        // frmSerialIssue
                        frmSerialIssue ObjInvSerial = new frmSerialIssue();
                        ObjInvSerial.Show();

                        //Add_Serial_Numbers ObjAddSerial = new Add_Serial_Numbers();
                        //ObjAddSerial.Show();
                    }
                    else
                    {
                        MessageBox.Show("You must select a Serialize Stock Item");
                    }
                }

            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int N = dgvIssueNote.CurrentRow.Index;
                dgvIssueNote.Rows.RemoveAt(N);
                // mltcmbboxItemSelect.Visible = false;

            }
            catch { }
        }
        private bool checkActivate = false;
        private void frmReturnNote_Activated(object sender, EventArgs e)
        {
            try
            {
                if (Search.searchIssueNoteNo == "0")
                { }
                else
                {
                    checkActivate = true;
                    btnPrint.Enabled = true;
                    cmbWarehouse.Enabled = false;
                    dgvIssueNote.Enabled = false;
                    dtpDate.Enabled = false;

                    string s = "select * from tblFGTransfer where FGTransNo = '" + Search.searchIssueNoteNo + "'";
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        txtIssueNoteId.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                        cmbWarehouse.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                        cmbjob.Text = dt.Rows[0].ItemArray[10].ToString().Trim();
                        dgvIssueNote.Rows.Clear();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvIssueNote.Rows.Add();
                            dgvIssueNote.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            dgvIssueNote.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvIssueNote.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[9].ToString().Trim();
                            dgvIssueNote.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            dgvIssueNote.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[6].ToString().Trim();
                        }
                    }
                    btnSave.Enabled = false;
                }
            }
            catch { }
        }

    }
}