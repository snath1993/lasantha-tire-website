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
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Collections;
using System.Threading;


namespace UserAutherization
{
    public partial class frmjobNote : Form
    {
        public static string ConnectionString;
        Connector conn = new Connector();
        clsCommon objclsCommon = new clsCommon();
        public DsJobEnter DsJob = new DsJobEnter(); 
        public bool IsNew = false;
        public bool IsFind = false; 
        public string StrSql;
        public string sMsg = "Peachtree - Tour Information";
        public string StrReference="";
        public int intGrid;
        DataTable dtUser = new DataTable();

        bool isadd;
        bool isEdit;
        bool isfind;
        bool iscancel;
        bool isReset;

        public bool run = false;
        public bool add = false;
        public bool edit = false;
        public bool delete = false;

        public frmjobNote()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {

        }
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex) { throw ex; }
        }
        public void GetItemDataSet()
        {
            try
            {
                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2,Phone1 from tblCustomerMaster";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cmbCustomer.DataSource = dt;
                    cmbCustomer.ValueMember = "CutomerID";
                    cmbCustomer.DisplayMember = "CutomerID";

                    cmbCustomer.DisplayLayout.Bands[0].Columns[0].Width = 75;
                    cmbCustomer.DisplayLayout.Bands[0].Columns[1].Width = 250;
                    cmbCustomer.DisplayLayout.Bands[0].Columns[2].Width = 0;
                    cmbCustomer.DisplayLayout.Bands[0].Columns[3].Width = 0;
                    cmbCustomer.DisplayLayout.Bands[0].Columns[4].Width = 0;
                    cmbCustomer.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    cmbCustomer.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                    cmbCustomer.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                }
                else
                {
                    cmbCustomer.DataSource = dt;
                    cmbCustomer.ValueMember = "CutomerID";
                    cmbCustomer.DisplayMember = "CutomerID";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetItemDataSet1()
        {
            try
            {
                StrSql = "SELECT ItemID,ItemDescription from tblItemMaster";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";

                    ultraCombo1.DisplayLayout.Bands[0].Columns[0].Width = 75;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[1].Width = 250;

                    ultraCombo2.DataSource = dt;
                    ultraCombo2.ValueMember = "ItemDescription";
                    ultraCombo2.DisplayMember = "ItemDescription";

                    ultraCombo2.DisplayLayout.Bands[0].Columns[0].Width = 75;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[1].Width = 250;
                }
                else
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";

                    ultraCombo2.DataSource = dt;
                    ultraCombo2.ValueMember = "ItemDescription";
                    ultraCombo2.DisplayMember = "ItemDescription";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmjobNote_Load(object sender, EventArgs e)
        {
            GetItemDataSet();
            GetItemDataSet1();
            GetJONum(true);            
            ClearData();
            checkuserauthentication();
        }

        private void ug_Click(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow ugR;

                if (HeaderValidation() == false)
                {
                    return;
                }
                if (ug.Rows.Count == 0)
                {
                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["LineNo"].Value = ugR.Index + 1;
                    ugR.Cells["LineNo"].Selected = true;
                    ugR.Cells["LineNo"].Activated = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Boolean HeaderValidation()
        {
            try
            {
                if (txtjobNo.Text.Trim() == "")
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ug_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyValue)
                {
                    case 37:
                        {
                            ug.PerformAction(UltraGridAction.PrevCell);
                            break;
                        }
                    case 38:
                        {
                            break;
                        }
                    case 39:
                        {
                            ug.PerformAction(UltraGridAction.NextCell);
                            break;
                        }
                    case 40:
                        {
                            if (ug.ActiveRow.Index != 0)
                                ug.PerformAction(UltraGridAction.BelowCell);
                            break;
                        }

                    case 9:
                        {
                            if (ug.ActiveCell.Column.Key == "M-Type")
                            {
                                if (ug.ActiveRow.HasNextSibling() == false)
                                {
                                    if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                                    {
                                        UltraGridRow ugR;
                                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                        ugR.Cells["LineNo"].Selected = true;
                                        ugR.Cells["LineNo"].Activated = true;
                                    }
                                }
                            }
                            break;
                        }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public Boolean IsGridExitCode(String StrCode, string _Type)
        {
            try
            {
                if (_Type == "Item")
                {
                    foreach (UltraGridRow ugR in ultraCombo1.Rows)
                    {
                        if (ugR.Cells["ItemID"].Text == StrCode)
                        {
                            return true;
                        }
                    }
                }
                if (_Type == "Desc")
                {
                    foreach (UltraGridRow ugR in ultraCombo2.Rows)
                    {
                        if (ugR.Cells["ItemDescription"].Text == StrCode)
                        {
                            return true;
                        }
                    }
                }               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            try
            {               
                if (ug.ActiveCell.Column.Key == "ItemCode")
                {
                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        return;
                    }
                    //else
                    {
                        ug.ActiveCell.Value = ug.ActiveCell.Text;

                        if (IsGridExitCode(ug.ActiveCell.Row.Cells[1].Text, "Item") == false)
                        {
                            e.Cancel = true;
                        }

                        foreach (UltraGridRow ugR in ultraCombo2.Rows)
                        {
                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemID"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemID"].Value;
                                ug.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemID"].Value;
                                ug.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDescription"].Value;                               
                            }
                        }
                    }
                }

                if (ug.ActiveCell.Column.Key == "Description")
                {
                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        return;
                    }
                    else
                    {
                        ug.ActiveCell.Value = ug.ActiveCell.Text;
                        if (IsGridExitCode(ug.ActiveCell.Row.Cells[2].Text, "Desc") == false)
                        {
                            e.Cancel = true;
                        }
                        foreach (UltraGridRow ugR in ultraCombo2.Rows)
                        {
                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemDescription"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemID"].Value;                                
                                ug.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDescription"].Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbCustomer_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtCustomer.Text = cmbCustomer.ActiveRow.Cells[1].Value.ToString();
                        txtAddress1.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString() + ", " + cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                        txttel.Text = cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void DeleteRows()
        {
            try
            {
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    ugR.Delete(false);
                }                
            }           
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            IsNew = true;
            isEdit = false;
            iscancel = false;

            ClearData();
            GetJONum(IsNew);
            EnableHeader(true);

            btnNew.Enabled = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnList.Enabled = true;
            btnCancel.Enabled = false;
            btnreset.Enabled = true;

            checkuserauthentication();
        }
        private void checkuserauthentication()
        {            
            dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmjobNote");
            if (dtUser.Rows.Count > 0)
            {
                run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                edit = Convert.ToBoolean(dtUser.Rows[0].ItemArray[4].ToString());
                delete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());
            }
            if (run != true)          
            {
                MessageBox.Show("Access Denied.", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //return;
                this.Close();
            }
            if (add != true )
            {
                //MessageBox.Show("Access Denied.", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //return;
                btnSave.Enabled = false;
            }
            if (edit != true )
            {
                //MessageBox.Show("Access Denied.", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //return;
                btnEdit.Enabled = false;
            }
            if (delete != true  )
            {
                //MessageBox.Show("Access Denied.", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //return;
                btnCancel.Enabled = false;
            }
        }
        private void ClearData()
        {
            IsNew = false;
            isEdit = false;
            isfind = false;
            isReset = false;

            dtjobdate.Value = DateTime.Now;
            dtjobtime.Value = DateTime.Now;
            dtdiliverydate.Value = DateTime.Now;
            dtdiltime.Value = DateTime.Now;

            txtnaration.Text = "";
            cmbCustomer.Text = "";
            txtAddress1.Text = "";
            txttel.Text = "";
            txtCustomer.Text = ""; 

            chkpc.Checked = false;
            chlam.Checked = false;
            chprint.Checked = false;
            chbinding.Checked = false;
            chkauto.Checked = false;
            chklatex.Checked = false;
            optbw.Checked = false;
            optbwtp.Checked = false;
            optcolour.Checked = false;
            optdcopy.Checked = false;
            DeleteRows();
            txtjobNo.Text = "";
            IsNew = true;
            GetJONum(IsNew);
            DeleteRows();
            
        }
        public void GetJONum(bool IsNew)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrTourNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                if (IsNew == true)
                {
                    StrSql = "SELECT JobNoPref, JobNoPad, JobNo FROM tblDefualtSetting";
                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        StrTourNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                        intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                        intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());
                        intP = 1;
                        for (intI = 1; intI <= intX; intI++)
                        {
                            intP = intP * 10;
                        }
                        intP = intP + intZ;
                        StrInV = intP.ToString();
                        txtjobNo.Text = StrTourNo + StrInV.Substring(1, intX);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveItemDetails(string Jobref, Int32 LineNo, string ItemCode,string Description, string Colour, string Size, string MType, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "";
                StrSql = "INSERT INTO [tbl_jobDetails]([Job_No],[LinNo],[ItemCode],[Description],[Colour],[Size],[MType]) VALUES('" + Jobref + "','" + LineNo + "','" + ItemCode + "','" + Description + "','" + Colour + "','" + Size + "','" + MType + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetDateTime(DateTime DtGetDate)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(DtGetDate);
                string Dformat = "MM/dd/yyyy";
                return DTP.ToString(Dformat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetTime(DateTime DtGetDate)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(DtGetDate);
                string Dformat = "MM/dd/yyyy hh:mm";
                return DTP.ToString(Dformat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            checkuserauthentication();    

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            if (txtjobNo.Text == null)
            {
                MessageBox.Show("Incorrect Job Number", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbCustomer.Text == null)
            {
                MessageBox.Show("Incorrect Customer", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }            
            try
            {
                if (isEdit == true)
                {
                    DialogResult reply = MessageBox.Show("Are you sure, you want to Edit this Job ? ", "Information", MessageBoxButtons.OKCancel);
                    if (reply == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                else if (IsNew == true)
                {
                    DialogResult reply = MessageBox.Show("Are you sure, you want to Save this Job ? ", "Information", MessageBoxButtons.OKCancel);
                    if (reply == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
                DeleteEmpGrid();

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                GetJONum(IsNew);
                UpdateJobNo(IsNew,myConnection, myTrans);
                if (isEdit == true)
                {
                    StrReference = txtjobNo.Text.ToString();
                    try
                    {
                        StrSql = "";
                        StrSql = "DELETE FROM [tbl_jobmaster] WHERE  Job_No='" + StrReference + "'";
                        SqlCommand sqcom1 = new SqlCommand(StrSql, myConnection, myTrans);
                        sqcom1.CommandType = CommandType.Text;
                        sqcom1.ExecuteNonQuery();

                        StrSql = "";
                        StrSql = "DELETE FROM [tbl_jobDetails] WHERE  Job_No='" + StrReference + "'";
                        SqlCommand sqcom2 = new SqlCommand(StrSql, myConnection, myTrans);
                        sqcom2.CommandType = CommandType.Text;
                        sqcom2.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                if (IsNew  == true)
                {
                    SqlCommand myCommand = new SqlCommand("select * from tbl_jobmaster where Job_No='" + txtjobNo.Text.Trim() + "'", myConnection, myTrans);
                    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                    DataTable dt41 = new DataTable();
                    da41.Fill(dt41);
                    if (dt41.Rows.Count > 0)
                    {
                        MessageBox.Show("Job No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        myTrans.Rollback();
                        myConnection.Close();
                        return;
                    }
                    else
                    {
                        StrReference = txtjobNo.Text.ToString();
                    }
                }

                StrSql = "";
                StrSql = "INSERT INTO [tbl_jobmaster](Job_No,JobDate,JobTime,DilDate,DilTime,Cusid,Photocopy,Laminate,Binding,Printings,Colour,ColourTp,Bw,Bwtp,Dc,Narration,Name,Address1,tel,letex,autocad)  VALUES('" + StrReference + "','" + GetDateTime(dtjobdate.Value) + "','" + GetTime(dtjobtime.Value) + "','" + GetDateTime(dtdiliverydate.Value) + "','" + GetTime(dtdiltime.Value) + "','" + cmbCustomer.Text.ToString().Trim() + "','" + Convert.ToBoolean(chkpc.Checked.ToString().Trim()) + "','" + Convert.ToBoolean(chlam.Checked.ToString().Trim()) + "','" + Convert.ToBoolean(chbinding.Checked.ToString().Trim()) + "','" + Convert.ToBoolean(chprint.Checked.ToString().Trim()) + "','" + Convert.ToBoolean(optcolour.Checked.ToString()) + "','" + Convert.ToBoolean(opttp.Checked.ToString()) + "','" + Convert.ToBoolean(optbw.Checked.ToString()) + "','" + Convert.ToBoolean(optbwtp.Checked.ToString()) + "','" + Convert.ToBoolean(optdcopy.Checked.ToString()) + "' ,'" + txtnaration.Text.ToString().Trim() + "','" + txtCustomer.Text.ToString() + "','" + txtAddress1.Text.ToString() + "','" + txttel.Text.ToString() + "','" + Convert.ToBoolean(chklatex.Checked.ToString().Trim()) + "','" + Convert.ToBoolean(chkauto.Checked.ToString().Trim()) + "')";
                SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                 if (ug.Rows.Count > 0)
                {
                    for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                    {
                        SaveItemDetails(txtjobNo.Text.ToString(), Convert.ToInt16(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), ug.Rows[intGrid].Cells["Colour"].Value.ToString(), ug.Rows[intGrid].Cells["Size"].Value.ToString(), ug.Rows[intGrid].Cells["Colour"].Value.ToString(), myConnection, myTrans);
                    }
                }                
                
                myTrans.Commit();
                MessageBox.Show("Job Successfuly Saved.", "Information", MessageBoxButtons.OK);
                Print(txtjobNo.Text.ToString().Trim());
                ClearData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateJobNo(bool IsNew,SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                if (IsNew == true)
                {
                    int intCRNNo;
                    SqlCommand command;

                    StrSql = "SELECT  TOP 1(JobNo) FROM tblDefualtSetting ORDER BY JobNo DESC";
                    command = new SqlCommand(StrSql, con, Trans);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        intCRNNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                    }
                    else
                    {
                        intCRNNo = 1;
                    }
                    StrSql = "UPDATE tblDefualtSetting SET JobNo='" + intCRNNo + "'";
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteEmpGrid()
        {
            try
            {
                ug.PerformAction(UltraGridAction.CommitRow);
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    if (ugR.Cells["ItemCode"].Value.ToString().Trim().Length == 0)
                    {
                        ugR.Delete(false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                IsNew = false;
                isReset = true;
                ClearData();
                          
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        private void EnableHeader(Boolean blnEnable)
        {
            groupBox2.Enabled = blnEnable;
            groupBox3.Enabled = blnEnable;
            groupBox1.Enabled = blnEnable;
            ug.Enabled = blnEnable;
            txtnaration.Enabled = blnEnable;            
            
        }
        private void btnList_Click(object sender, EventArgs e)
        {
            if (frmMain.objjoblist == null || frmMain.objjoblist.IsDisposed)
            {
                frmMain.objjoblist = new frmjoblist(1);
            }
            //frmMain.objjobnote.TopMost = false;
            frmMain.objjoblist.ShowDialog();
            frmMain.objjoblist.TopMost = true;
            btnSave.Enabled = false;

            ClearData();
            DeleteRows();
            txtjobNo.Text = Search.searchIssueNoteNo;
            IsFind = true;
            IsNew = false;
            setValue();
            if (iscancel == true)
            {
                EnableHeader(false);
                btnNew.Enabled = false;
                btnSave.Enabled = false;
                btnEdit.Enabled = false;
                btnList.Enabled = true;
                btnCancel.Enabled = false;
                btnreset.Enabled = true;                
            }
            else
            {
                EnableHeader(true);
                btnNew.Enabled = false;
                btnSave.Enabled = false;
                btnEdit.Enabled = true;
                btnList.Enabled = true;
                btnCancel.Enabled = false;
                btnreset.Enabled = true;   
            }
            checkuserauthentication();
        }
        public void setValue()
        { 
            try
            {
                string strNo = (Search.searchIssueNoteNo);

                if (strNo == "")
                {
                    strNo = "";
                }
                else
                {
                    txtjobNo.Text = Search.searchIssueNoteNo;
                    ViewHeader(Search.searchIssueNoteNo);
                    ViewHotelDetails(Search.searchIssueNoteNo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ViewHeader(string StrTourRefNo)
        {
            StrSql = "SELECT     Job_No,JobDate,JobTime,DilDate,DilTime,Cusid,Photocopy,Laminate,Binding,Printings,Colour,ColourTp,Bw,Bwtp,Dc,Narration,Cancel,Name,Address1,tel,letex,autocad FROM tbl_jobmaster where Job_No='" + txtjobNo.Text + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtjobNo.Text = dt.Rows[0]["Job_No"].ToString();
                dtjobdate.Value  = Convert.ToDateTime(dt.Rows[0]["JobDate"].ToString());
                dtjobtime.Value = Convert.ToDateTime(dt.Rows[0]["JobTime"].ToString());
                dtdiltime.Value = Convert.ToDateTime(dt.Rows[0]["DilDate"].ToString());
                dtdiltime.Value = Convert.ToDateTime(dt.Rows[0]["DilTime"].ToString());
                cmbCustomer.Text = dt.Rows[0]["Cusid"].ToString();
                if (Convert.ToBoolean(dt.Rows[0]["Photocopy"].ToString()) == true) chkpc.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["Laminate"].ToString()) == true) chlam.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["Binding"].ToString()) == true) chbinding.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["Printings"].ToString()) == true) chprint.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["Colour"].ToString()) == true) optcolour.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["ColourTp"].ToString()) == true) opttp.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["Bw"].ToString()) == true) optbw.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["Bwtp"].ToString()) == true) optbwtp.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["Dc"].ToString()) == true) optdcopy.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["letex"].ToString()) == true) chklatex.Checked = true;
                if (Convert.ToBoolean(dt.Rows[0]["autocad"].ToString()) == true) chkauto.Checked = true; 

                txtnaration.Text = dt.Rows[0]["Narration"].ToString();
                txtCustomer.Text = dt.Rows[0]["Name"].ToString();
                txtAddress1.Text = dt.Rows[0]["Address1"].ToString();
                txttel.Text = dt.Rows[0]["tel"].ToString();

                if( Convert.ToInt16(dt.Rows[0]["Cancel"].ToString()) == 1)
                {
                    lblCancel.Visible=true;
                    iscancel = true;
                }
                else
                {
                     lblCancel.Visible=false;
                     iscancel = false;
                }
            }
            
        }
        public void ViewHotelDetails(string StrTourRefNo)
        {
            try
            {
                StrSql = "SELECT * from tbl_jobDetails  WHERE [Job_No]='" + StrTourRefNo + "'ORDER BY [id]";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = Dr["LinNo"];
                        ugR.Cells["ItemCode"].Value = Dr["ItemCode"];
                        ugR.Cells["Description"].Value = Dr["Description"];
                        ugR.Cells["Colour"].Value = Dr["Colour"];
                        ugR.Cells["Size"].Value = Dr["Size"];
                        ugR.Cells["M-Type"].Value = Dr["MType"];                                         
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsNew = false;
            isEdit = true;
            EnableHeader(true);
            btnNew.Enabled = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnList.Enabled = true;
            btnCancel.Enabled = false;
            btnreset.Enabled = true;
            
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Print(txtjobNo.Text.ToString().Trim());
        }
        private void Print(string JobNo)
        {
            try
            {
                DataSet ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print Job ?", "Print", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.OK)
                {
                    DsJob.Clear();
                    if (JobNo != "")
                    {
                        string StrSql = "";
                        StrSql = "SELECT * FROM tblCustomerMaster where CutomerID='" + cmbCustomer.Text.ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(StrSql);
                        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(DsJob.DtCustomer);

                        StrSql = "Select * from tbl_jobmaster WHERE Job_No = '" + txtjobNo.Text.ToString().Trim() + "'";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsJob.DtJobMaster);

                        StrSql = "Select * from tbl_jobDetails WHERE Job_No = '" + txtjobNo.Text.ToString().Trim() + "'";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsJob.DtJobDetails);

                        DirectPrint();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }
        private void DirectPrint()
        {
            try
            {
                frmjobprint printax = new frmjobprint(this);
                printax.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            iscancel = true;
            checkuserauthentication();
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            myConnection.Open();

            try
            {                
                myTrans = myConnection.BeginTransaction();
                DataSet ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Cancel this Job ?", "Print", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.OK)
                {
                    CancelJob(txtjobNo.Text.ToString(),myConnection, myTrans);                    
                }
                myTrans.Commit();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                return;
            }
        }
        public void CancelJob(string StrRefno, SqlConnection con, SqlTransaction Trans)
        {
            SqlCommand command;
            StrSql = "UPDATE tbl_jobmaster SET Cancel='1' where Job_No='" + StrRefno.ToString().Trim() + "'";
            command = new SqlCommand(StrSql, con, Trans);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }
        public void ValidateCancel(string StrRefno,SqlConnection con, SqlTransaction Trans)
        {
            StrSql = "";
            StrSql = "SELECT * from dbo.tbl_jobmaster where Job_No='" + StrRefno.ToString().Trim() + "'";
            SqlCommand myCommand1 = new SqlCommand(StrSql, con, Trans);
            SqlDataAdapter da411 = new SqlDataAdapter(myCommand1);
            DataTable dt411 = new DataTable();
            da411.Fill(dt411);
            if (dt411.Rows.Count == 0)
            {
               
            }
        }
    }

}
