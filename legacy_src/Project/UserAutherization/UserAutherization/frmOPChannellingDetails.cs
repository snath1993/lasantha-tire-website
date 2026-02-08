using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using UserAutherization;
using DataAccess;
using System.Xml;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmOPChannellingDetails : Form
    {
        
        bool run = false;
        bool add = false;
        bool edit = false;
        bool delete = false;

        public static string ConnectionString;
        public static bool isedit = false;
        public frmOPChannellingDetails()
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FillGridConsultant();
            Clear();

            ConsutHave = 0;
        }

        private void Clear()
        {
            try
            {
                ConsultantCode = string.Empty;
                label3.Visible = true;
                txtconsulttitle.Visible = true;
                txtconsulttitle.Text = string.Empty;
                txtConsutSearch.Text = string.Empty;
                txtConsultant.Text = string.Empty;
                txtCategory.Text= string.Empty;

                txtMondayS1.Text= string.Empty;
                txtMondayS2.Text = string.Empty;
                txtMondayS3.Text = string.Empty;

                txtThursdayS1.Text = string.Empty;
                txtThursdayS2.Text = string.Empty;
                txtThursdayS3.Text = string.Empty;

                txtTuesdayS1.Text = string.Empty;
                txtTuesdayS2.Text = string.Empty;
                txtTuesdayS3.Text = string.Empty;

                txtWedenesdayS1.Text = string.Empty;
                txtWednesdayS2.Text = string.Empty;
                txtWednesdayS3.Text = string.Empty;

                txtSaturdayS1.Text = string.Empty;
                txtSaturdayS2.Text = string.Empty;
                txtSaturdayS3.Text = string.Empty;

                txtSundayS1.Text = string.Empty;
                txtSundayS2.Text = string.Empty;
                txtSundayS3.Text = string.Empty;

                txtFridayS1.Text = string.Empty;
                txtFridayS2.Text = string.Empty;
                txtFridayS3.Text = string.Empty;

                flag = false;
                txtConsultant.ReadOnly = false;

                txtconsulttitle.Text = string.Empty;
                txtConsultant.Text = string.Empty;
                txtCategory.Text = string.Empty;
                txtAddress.Text = string.Empty;
                txtContactNo.Text = string.Empty;
                txtQualifications.Text = string.Empty;
                txtInterval.Text = string.Empty;
                txtOtherHospitals.Text = string.Empty;
                ultrType.Text = string.Empty;
                chkBlock.Checked = false;
                ChkLeve.Checked = false;
                //txtCategory.ReadOnly = false;
                txtconsulttitle.Enabled = true;
                txtConsultant.Enabled = true;
                txtCategory.Enabled = true;
                txtAddress.Enabled = true;
                txtContactNo.Enabled = true;
                txtQualifications.Enabled = true;
                txtInterval.Enabled = true;
                txtOtherHospitals.Enabled = true;
                chkBlock.Enabled = true;
                ChkLeve.Enabled = true;
                ultrType.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmScan_Load(object sender, EventArgs e)
        {
            FillGridConsultant();
            GetCategory();
            GetTypes();
        }

        private void GetTypes()
        {
            DataSet dstype = new DataSet();
            try
            {
                dstype.Clear();

                String StrSql = "SELECT DISTINCT Type FROM tblConsultantMaster WHERE  Type<>'' ";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dstype, "DtClient");

                ultrType.DataSource = dstype.Tables["DtClient"];
                ultrType.DisplayMember = "Type";
                ultrType.ValueMember = "Type";

                ultrType.DisplayLayout.Bands["DtClient"].Columns["Type"].Width = 170;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void GetCategory()
        {
            DataSet dscatagory = new DataSet();
            try
            {
                dscatagory.Clear();

                String StrSql = "SELECT DISTINCT ConsultantType FROM tblConsultantMaster  ";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dscatagory, "DtClient");

                txtCategory.DataSource = dscatagory.Tables["DtClient"];
                txtCategory.DisplayMember = "ConsultantType";
                txtCategory.ValueMember = "ConsultantType";

                txtCategory.DisplayLayout.Bands["DtClient"].Columns["ConsultantType"].Width = 100;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public void FillGridConsultant()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode as ConsultantID, ConsultantName from tblConsultantMaster ";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                dgvConsultantRoom.DataSource = ds1.Tables[0];
                dgvConsultantRoom.Columns[0].Width = 80;
                dgvConsultantRoom.Columns[1].Width = 250;
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    dgvConsultantRoom.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    dgvConsultantRoom.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

      

        private void frmScan_Activated(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult reply1 = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.YesNo);

                if (reply1 == DialogResult.No)
                {
                    return;
                }
                else if (reply1 == DialogResult.Yes)
                {

                    if (flag == false)
                    {
                        SaveNewConsultant();
                    }
                    else
                    {
                        updateconsultan();
                    }
                    SaveEventNew();
                    btnNew_Click(sender, e);



                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateconsultan()
        {
            string LeaveFrom = "";
            string LeaveTo = "";
            String Block = "";
            if (chkBlock.Checked == true)
            {
                Block = "True";

            }
            else
            {
                Block = "False";
            }
            if (ChkLeve.Checked == true)
            {
                LeaveFrom = dtpFromLeave.Value.ToString(@"MM/dd/yyy");
                LeaveTo = dtpToLeave.Value.ToString(@"MM/dd/yyy");

            }
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            try
            {
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                string S6 = " DELETE FROM tblConsultantMaster WHERE ConsultantCode='" + ConsultantCode + "' ";
                SqlCommand cmd6 = new SqlCommand(S6, myConnection, myTrans);
                SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                DataTable dt6 = new DataTable();
                da6.Fill(dt6);


                String S1 = "INSERT INTO tblConsultantMaster( [ConsultantCode],[Title],[ConsultantName] , [ConsultantType],[ConsultantCharges],[Block],[HspitalCharges] ,[Address],[ContactNO],[Qualifications],[OtherHospitals],[LeaveFromDate],[LeaveToDate],[Interval],[Type]) values ('" + ConsultantCode + "','" + txtconsulttitle.Text.ToString().Trim() + "' ,'" + txtConsultant.Text.ToString().Trim() + "' ,'" + txtCategory.Text.ToString().Trim() + "' ,'0' ,'" + Block + "','0' ,'" + txtAddress.Text.ToString().Trim() + "','" + txtContactNo.Text.ToString() + "','" + txtQualifications.Text.ToString().Trim() + "','" + txtOtherHospitals.Text.ToString().Trim() + "','" + LeaveFrom+ "','" + LeaveTo + "','" + txtInterval.Text.ToString().Trim() + "','" + ultrType.Text.ToString().Trim() + "')";
                SqlCommand command = new SqlCommand(S1, myConnection, myTrans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
               
                myTrans.Commit();


            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveNewConsultant()
        {
            string LeaveFrom = "";
            string LeaveTo = "";
            String Block = "";
           if(chkBlock.Checked == true)
            {
                Block ="True";

            }
            else
            {
                Block = "False";
            }
            if (ChkLeve.Checked==true)
            {
                LeaveFrom = dtpFromLeave.Value.ToString();
                LeaveTo= dtpToLeave.Value.ToString();

            }
            if (txtConsultant.Text == string.Empty)
            {
                MessageBox.Show("Please Type Consultant Name");
            }
            else
           if (txtconsulttitle.Text == string.Empty)
            {
                MessageBox.Show("Please Select Consultant Title ");
            }
            else
           if (txtCategory.Text == string.Empty)
            {
                MessageBox.Show("Please Select Consultant Category");
            }
            else
            {
                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                SqlTransaction myTrans = null;
                try
                {
                    myConnection.Open();
                    myTrans = myConnection.BeginTransaction();
                    ConsultantCode = GetDoctorNo(myConnection, myTrans);

                    String S1 = "INSERT INTO tblConsultantMaster( [ConsultantCode],[Title],[ConsultantName] , [ConsultantType],[ConsultantCharges],[Block],[HspitalCharges] ,[Address],[ContactNO],[Qualifications],[OtherHospitals],[LeaveFromDate],[LeaveToDate],[Interval],[Type]) values ('" + ConsultantCode + "','" + txtconsulttitle.Text.ToString().Trim() + "' ,'" + txtConsultant.Text.ToString().Trim() + "' ,'" + txtCategory.Text.ToString().Trim() + "' ,'0' ,'" + Block + "','0' ,'" + txtAddress.Text.ToString().Trim() + "','" + txtContactNo.Text.ToString() + "','" + txtQualifications.Text.ToString().Trim() + "','" + txtOtherHospitals.Text.ToString().Trim() + "','" + LeaveFrom + "','" + LeaveTo + "','" + txtInterval.Text.ToString().Trim() + "','"+ultrType.Text.ToString().Trim()+"')";
                    SqlCommand command = new SqlCommand(S1, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    UpdateDoctorNo(myConnection, myTrans);
                    myTrans.Commit();


                }
                catch (Exception ex)
                {
                    myTrans.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void UpdateDoctorNo(SqlConnection con, SqlTransaction trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;


                String StrSql = "SELECT  TOP 1(DNo) FROM tblDoctorNumberget ORDER BY DNo DESC";


                command = new SqlCommand(StrSql, con, trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intInvNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 0001;
                }
                else
                {
                    intInvNo = 1;
                }


                StrSql = "UPDATE tblDoctorNumberget SET DNo='" + intInvNo + "'";


                command = new SqlCommand(StrSql, con, trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void UpdateEventNew1()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            try
            {
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                string[,] scores = new string[21, 3] { { txtSaturdayS1.Text.ToString(), "Saturday", "1" }, { txtSaturdayS2.Text.ToString(), "Saturday", "2" }, { txtSaturdayS3.Text.ToString(), "Saturday", "3" }, { txtSundayS1.Text.ToString(), "Sunday", "1" }, { txtSundayS2.Text.ToString(), "Sunday", "2" }, { txtSundayS3.Text.ToString(), "Sunday", "3" }, { txtMondayS1.Text.ToString(), "Monday", "1" }, { txtMondayS2.Text.ToString(), "Monday", "2" }, { txtMondayS3.Text.ToString(), "Monday", "3" }, { txtThursdayS1.Text.ToString(), "Thursday", "1" }, { txtThursdayS2.Text.ToString(), "Thursday", "2" }, { txtThursdayS3.Text.ToString(), "Thursday", "3" }, { txtTuesdayS1.Text.ToString(), "Tuesday", "1" }, { txtTuesdayS2.Text.ToString(), "Tuesday", "2" }, { txtTuesdayS3.Text.ToString(), "Tuesday", "3" }, { txtWedenesdayS1.Text.ToString(), "Wednesday", "1" }, { txtWednesdayS2.Text.ToString(), "Wednesday", "2" }, { txtWednesdayS3.Text.ToString(), "Wednesday", "3" }, { txtFridayS1.Text.ToString(), "Friday", "1" }, { txtFridayS2.Text.ToString(), "Friday", "2" }, { txtFridayS3.Text.ToString(), "Friday", "3" } };

                for (int i = 0; i < 21; i++)
                {

                    String S1 = "UPDATE tblSchedulingDetails SET  [SessionTime]='" + scores[i, 0] + "'  WHERE ([ConsultantCode]='" + ConsultantCode + "' )AND ([ConsultantType]='" + txtCategory.Text.ToString().Trim() + "') AND ([Aday]='" + scores[i, 1] + "') AND ( [Session]='" + scores[i, 2] + "') ";
                    SqlCommand command = new SqlCommand(S1, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                myTrans.Commit();
                MessageBox.Show("Details Successfully Updated");

            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }
            
        }

        private void SaveEventNew()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            try {
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                  string[,] scores = new string[21, 3] { { txtSaturdayS1.Text.ToString(), "Saturday", "1" }, { txtSaturdayS2.Text.ToString(), "Saturday", "2" }, { txtSaturdayS3.Text.ToString(), "Saturday", "3" }, { txtSundayS1.Text.ToString(), "Sunday", "1" }, { txtSundayS2.Text.ToString(), "Sunday", "2" }, { txtSundayS3.Text.ToString(), "Sunday", "3" }, { txtMondayS1.Text.ToString(), "Monday", "1" }, { txtMondayS2.Text.ToString(), "Monday", "2" }, { txtMondayS3.Text.ToString(), "Monday", "3" }, { txtThursdayS1.Text.ToString(), "Thursday", "1" }, { txtThursdayS2.Text.ToString(), "Thursday", "2" }, { txtThursdayS3.Text.ToString(), "Thursday", "3" }, { txtTuesdayS1.Text.ToString(), "Tuesday", "1" }, { txtTuesdayS2.Text.ToString(), "Tuesday", "2" }, { txtTuesdayS3.Text.ToString(), "Tuesday", "3" }, { txtWedenesdayS1.Text.ToString(), "Wednesday", "1" }, { txtWednesdayS2.Text.ToString(), "Wednesday", "2" }, { txtWednesdayS3.Text.ToString(), "Wednesday", "3" }, { txtFridayS1.Text.ToString(), "Friday", "1" }, { txtFridayS2.Text.ToString(), "Friday", "2" }, { txtFridayS3.Text.ToString(), "Friday", "3" } };


                String S = "Select * from tblSchedulingDetails where (ConsultantCode = '"+ ConsultantCode+ "' AND ConsultantName='"+ txtConsultant.Text.ToString().Trim()+"') ";
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < 21; i++)
                    {

                        String S1 = "UPDATE tblSchedulingDetails SET  [SessionTime]='" + scores[i, 0] + "'  WHERE ([ConsultantCode]='" + ConsultantCode + "' )AND ([ConsultantType]='" + txtCategory.Text.ToString().Trim() + "') AND ([Aday]='" + scores[i, 1] + "') AND ( [Session]='" + scores[i, 2] + "') ";
                        SqlCommand command = new SqlCommand(S1, myConnection, myTrans);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
                else
                {

                    for (int i = 0; i < 21; i++)
                    {

                        String S1 = "INSERT INTO tblSchedulingDetails( [ConsultantCode],[ConsultantName],[ConsultantType] , [Aday], [Session],[SessionTime] ) values ('" + ConsultantCode + "','" + txtConsultant.Text.ToString().Trim() + "' ,'" + txtCategory.Text.ToString().Trim() + "' ,'" + scores[i, 1] + "' ,'" + scores[i, 2] + "' ,'" + scores[i, 0] + "' )";
                        SqlCommand command = new SqlCommand(S1, myConnection, myTrans);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
                myTrans.Commit();
                MessageBox.Show("Details Successfully Saved");
                
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }
        }
        public string GetDoctorNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

               String StrSql = "SELECT DPref, DPad, DNo FROM tblDoctorNumberget ";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    return StrInvNo + StrInV.Substring(1, intX);

                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                return null;
                MessageBox.Show(ex.Message);
            }
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

       
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
 
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode as ConsultantID, ConsultantName from tblConsultantMaster where Block = 'False' AND ConsultantName LikE '%"+txtConsutSearch.Text.ToString()+"%'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                dgvConsultantRoom.DataSource = ds1.Tables[0];
                dgvConsultantRoom.Columns[0].Width = 80;
                dgvConsultantRoom.Columns[1].Width = 200;
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    dgvConsultantRoom.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    dgvConsultantRoom.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        string ConsultantCode = "";
        Boolean flag = false;
        private void dgvConsultantRoom_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                clearcon();
                ConsutHave = 0;
                txtConsultant.Text = dgvConsultantRoom[1, dgvConsultantRoom.CurrentRow.Index].Value.ToString().Trim();
                ConsultantCode= dgvConsultantRoom[0, dgvConsultantRoom.CurrentRow.Index].Value.ToString().Trim();
                //if (txtCategory.Text.ToString().Trim() == "")
                //{
                    string ConnString = ConnectionString;
                    String S1 = "Select * from tblConsultantMaster where ConsultantCode = '" + dgvConsultantRoom.Rows[dgvConsultantRoom.CurrentRow.Index].Cells[0].Value + "' AND Block = 'False'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);
                    txtCategory.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();
                    txtConsultant.Text = dgvConsultantRoom[1, dgvConsultantRoom.CurrentRow.Index].Value.ToString().Trim();
                    txtconsulttitle.Text= ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                    txtAddress.Text= ds1.Tables[0].Rows[0].ItemArray[4].ToString();
                    txtContactNo.Text= ds1.Tables[0].Rows[0].ItemArray[5].ToString();
                    txtQualifications.Text= ds1.Tables[0].Rows[0].ItemArray[6].ToString();
                    ultrType.Text = ds1.Tables[0].Rows[0].ItemArray[14].ToString();
                
                    if (ds1.Tables[0].Rows[0].ItemArray[9].ToString() == "True")
                    {
                        chkBlock.Checked = true;
                    }
                    else
                    {
                        chkBlock.Checked = false;

                    }
                    
                if (ds1.Tables[0].Rows[0].ItemArray[10].ToString() != "")
                {
                    ChkLeve.Checked = true;
                    dtpFromLeave.Value = Convert.ToDateTime(ds1.Tables[0].Rows[0].ItemArray[10].ToString());
                    dtpToLeave.Value = Convert.ToDateTime(ds1.Tables[0].Rows[0].ItemArray[11].ToString());
                }
                else
                {
                    ChkLeve.Checked = false;
                }
                txtOtherHospitals.Text= ds1.Tables[0].Rows[0].ItemArray[7].ToString();
                txtInterval.Text = ds1.Tables[0].Rows[0].ItemArray[12].ToString();

                //}
                FillData();
                flag = true;
                txtConsultant.ReadOnly = true;
                
               // label3.Visible = false;
                //txtconsulttitle.Visible = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void clearcon()
        {
            ultrType.Text = string.Empty;
            txtMondayS1.Text = string.Empty;
            txtMondayS2.Text = string.Empty;
            txtMondayS3.Text = string.Empty;

            txtThursdayS1.Text = string.Empty;
            txtThursdayS2.Text = string.Empty;
            txtThursdayS3.Text = string.Empty;

            txtTuesdayS1.Text = string.Empty;
            txtTuesdayS2.Text = string.Empty;
            txtTuesdayS3.Text = string.Empty;

            txtWedenesdayS1.Text = string.Empty;
            txtWednesdayS2.Text = string.Empty;
            txtWednesdayS3.Text = string.Empty;

            txtSaturdayS1.Text = string.Empty;
            txtSaturdayS2.Text = string.Empty;
            txtSaturdayS3.Text = string.Empty;

            txtSundayS1.Text = string.Empty;
            txtSundayS2.Text = string.Empty;
            txtSundayS3.Text = string.Empty;

            txtFridayS1.Text = string.Empty;
            txtFridayS2.Text = string.Empty;
            txtFridayS3.Text = string.Empty;

            txtconsulttitle.Text= string.Empty;
            txtConsultant.Text= string.Empty;
            txtCategory.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtContactNo.Text = string.Empty;
            txtQualifications.Text = string.Empty;
            txtInterval.Text = string.Empty;
            txtOtherHospitals.Text = string.Empty;
            chkBlock.Checked = false;
            ChkLeve.Checked = false;
        }
        int ConsutHave = 0;
        public void FillData()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select ADay , Session, SessionTime from tblSchedulingDetails where ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "'AND ConsultantType='"+ txtCategory.Text.ToString() + "' ORDER BY ADay";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                ConsutHave = dt1.Rows.Count;
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Sunday") && (dt1.Rows[i].ItemArray[1].ToString() == "1"))
                    {
                        txtSundayS1.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Sunday") && (dt1.Rows[i].ItemArray[1].ToString() == "2"))
                    {
                        txtSundayS2.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Sunday") && (dt1.Rows[i].ItemArray[1].ToString() == "3"))
                    {
                        txtSundayS3.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    //------------------
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Monday") && (dt1.Rows[i].ItemArray[1].ToString() == "1"))
                    {
                        txtMondayS1.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Monday") && (dt1.Rows[i].ItemArray[1].ToString() == "2"))
                    {
                        txtMondayS2.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Monday") && (dt1.Rows[i].ItemArray[1].ToString() == "3"))
                    {
                        txtMondayS3.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    //--------------------
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Tuesday") && (dt1.Rows[i].ItemArray[1].ToString() == "1"))
                    {
                        txtTuesdayS1.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Tuesday") && (dt1.Rows[i].ItemArray[1].ToString() == "2"))
                    {
                        txtTuesdayS2.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Tuesday") && (dt1.Rows[i].ItemArray[1].ToString() == "3"))
                    {
                        txtTuesdayS3.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }

                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Wednesday") && (dt1.Rows[i].ItemArray[1].ToString() == "1"))
                    {
                        txtWedenesdayS1.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Wednesday") && (dt1.Rows[i].ItemArray[1].ToString() == "2"))
                    {
                        txtWednesdayS2.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Wednesday") && (dt1.Rows[i].ItemArray[1].ToString() == "3"))
                    {
                        txtWednesdayS3.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    // -------------
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Thursday") && (dt1.Rows[i].ItemArray[1].ToString() == "1"))
                    {
                        txtThursdayS1.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Thursday") && (dt1.Rows[i].ItemArray[1].ToString() == "2"))
                    {
                        txtThursdayS2.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Thursday") && (dt1.Rows[i].ItemArray[1].ToString() == "3"))
                    {
                        txtThursdayS3.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }

                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Friday") && (dt1.Rows[i].ItemArray[1].ToString() == "1"))
                    {
                        txtFridayS1.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Friday") && (dt1.Rows[i].ItemArray[1].ToString() == "2"))
                    {
                        txtFridayS2.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Friday") && (dt1.Rows[i].ItemArray[1].ToString() == "3"))
                    {
                        txtFridayS3.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }

                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Saturday") && (dt1.Rows[i].ItemArray[1].ToString() == "1"))
                    {
                        txtSaturdayS1.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Saturday") && (dt1.Rows[i].ItemArray[1].ToString() == "2"))
                    {
                        txtSaturdayS2.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                    if ((dt1.Rows[i].ItemArray[0].ToString() == "Saturday") && (dt1.Rows[i].ItemArray[1].ToString() == "3"))
                    {
                        txtSaturdayS3.Text = dt1.Rows[i].ItemArray[2].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtconsulttitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {

                    e.Handled = true;
                    return;
                }
                if (txtContactNo.Text.Length == 10)
                {
                    e.Handled = true;

                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ChkLeve_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkLeve.Checked == true)
                {
                    dtpFromLeave.Enabled = true;
                    dtpToLeave.Enabled = true;
                }
                else
                {
                    dtpFromLeave.Enabled = false;
                    dtpToLeave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}