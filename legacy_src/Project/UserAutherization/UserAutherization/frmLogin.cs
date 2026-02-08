using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;
using Interop.PeachwServer;
using System.Xml;
using PCMBeans;
using DataAccess;


namespace UserAutherization
{
    public partial class frmLogin : Form
    {
        clsCommon objclsCommon = new clsCommon();
        //Connector objConnector = new Connector();
        public static string CurrentUser;
        public static string ConnectionString;
        clsBeansPhases objclsBeansPhases;
        public Interop.PeachwServer.Application app;
        public string StrComName;
        public string StrSql;
        Controlers objControlers = new Controlers();
        DataAccess.clsCommon objclsCommon1 = new DataAccess.clsCommon();
        public string Merge1AccUser;
        public static bool InvPrefex;
        public static bool InvPrefexDir;

        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }


        public frmLogin()
        {
            InitializeComponent();
            setConnectionString();
            txtUserName.Focus();
        }
        public void GetInvoicePrefex()
        {
            StrSql = "SELECT InvPref,InvDRCreditPref,InvCreditPref,InvoicePrefix,InvCreditPref FROM tblDefualtSetting";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0].ItemArray[0].ToString().Trim() == dt.Rows[0].ItemArray[1].ToString().Trim())
                {
                    user.InvPrefexDir = true;
                }
                else
                {
                    user.InvPrefexDir = false;
                }
                if (dt.Rows[0].ItemArray[2].ToString().Trim() == dt.Rows[0].ItemArray[3].ToString().Trim())
                {
                    user.InvPrefex = true;
                }
                else
                {
                    user.InvPrefex = false;
                }

            }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                setConnectionString();
                if (ConnectionString == "")
                {
                    MessageBox.Show("You must attach the database follow the link given below");
                    return;
                }
                else
                {
                    if ((txtUserName.Text.ToString().Trim() != "") && (txtPassWord.Text.ToString().Trim() != ""))
                    {
                        String S = "select PassWord from Login where UserID = '" + txtUserName.Text.ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string a = Convert.ToString(dt.Rows[0].ItemArray[0].ToString().Trim());
                            if (a == txtPassWord.Text)
                            {
                                user.userName = txtUserName.Text.ToString().Trim();//take the system user

                                String S4 = "select RtKey from tblRtKeyData";// where UserID = '" + txtUserName.Text.ToString().Trim() + "'";
                                SqlCommand cmd4 = new SqlCommand(S4);
                                SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
                                DataTable dt4 = new DataTable();
                                da4.Fill(dt4);
                                //if (objclsCommon1.decryptPassword(Convert.ToString(dt4.Rows[0].ItemArray[0])) == "sage@pbssaddonn1983")
                                if ("sage@pbssaddonn1983" == "sage@pbssaddonn1983")
                                {
                                    String S5 = "select Count(Program_name) from Sys.SysProcesses where Program_name='AAAA'";
                                    SqlCommand cmd5 = new SqlCommand(S5);
                                    SqlDataAdapter da5 = new SqlDataAdapter(S5, ConnectionString);
                                    DataTable dt5 = new DataTable();
                                    da5.Fill(dt5);
                                    if (Convert.ToDouble(dt5.Rows[0].ItemArray[0]) > 1)
                                    {
                                        MessageBox.Show("Maximaum user count exeeded");
                                    }
                                    else
                                    {
                                        //following code segment insert the systen date or specific dste for transaction
                                        if (rdobtnUseCurrentDate.Checked == true || rdobtnSpecificDate.Checked == true)
                                        {
                                            try
                                            {
                                                string Dformat = "MM/dd/yyyy";
                                                string LoginDate = "";
                                                string SystemDate = "";
                                                if (rdobtnUseCurrentDate.Checked == true)
                                                {
                                                    DateTime DTP = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                                                    DateTime DTP1 = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                                                    LoginDate = DTP.ToString(Dformat);
                                                    SystemDate = DTP1.ToString(Dformat);
                                                }
                                                else
                                                {
                                                    DateTime DTP = Convert.ToDateTime(dtplogindate.Text);
                                                    DateTime DTP1 = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                                                    LoginDate = DTP.ToString(Dformat);
                                                    SystemDate = DTP1.ToString(Dformat);
                                                }

                                                String S3 = "select * from tblUserWiseDate where UserName = '" + user.userName.ToString().Trim() + "'";
                                                SqlCommand cmd3 = new SqlCommand(S3);
                                                SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                                                DataTable dt3 = new DataTable();
                                                da3.Fill(dt3);

                                                if (dt3.Rows.Count == 0)
                                                {
                                                    String S2 = "insert into tblUserWiseDate(UserName,CurrentDate,SystemDate) values ('" + user.userName.ToString().Trim() + "','" + SystemDate + "','" + LoginDate + "')";
                                                    SqlCommand cmd2 = new SqlCommand(S2);
                                                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                                                    DataSet ds2 = new DataSet();
                                                    da2.Fill(ds2);
                                                }
                                                else
                                                {
                                                    String SS12 = "Update tblUserWiseDate SET CurrentDate = '" + LoginDate + "', SystemDate = '" + SystemDate + "' where UserName='" + user.userName.ToString().Trim() + "'";
                                                    SqlCommand cmdS12 = new SqlCommand(SS12);
                                                    SqlDataAdapter daS12 = new SqlDataAdapter(SS12, ConnectionString);
                                                    DataSet dsS12 = new DataSet();
                                                    daS12.Fill(dsS12);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message);
                                            }

                                            //=============================================================
                                            //sanjeewa
                                            //Connector objConnector = new Connector();
                                            //if (objConnector.IsOpenPeachtree(dtplogindate.Value) == false)
                                            //{
                                            //    System.Windows.Forms.Application.Exit();
                                            //}
                                            //else
                                            //{
                                            String SSql = " exec tblDefaultSettingCodeGen_get ";
                                            SqlCommand cmdS2 = new SqlCommand(SSql);
                                            SqlDataAdapter daS2 = new SqlDataAdapter(SSql, ConnectionString);
                                            DataSet dsS2 = new DataSet();
                                            daS2.Fill(dsS2);

                                            foreach (DataRow dr in dsS2.Tables[0].Rows)
                                            {
                                                if (dr["TransType"].ToString() == "GRN")
                                                    user.IsGRNNoAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "DN")
                                                    user.IsDNNoAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "CINV")
                                                    user.IsCINVNoAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "SINV")
                                                    user.IsSINVNoAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "CRTN")
                                                    user.IsCRTNNoAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "SRTN")
                                                    user.IsSRTNNoAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "JISN")
                                                    user.IsJOBIssueNOAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "JRTN")
                                                    user.IsJOBReturnNOAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "BOQ")
                                                    user.IsBOQNOAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                                if (dr["TransType"].ToString() == "BOM")
                                                    user.IsBOMNOAutoGen = bool.Parse(dr["IsAuto"].ToString());

                                            }

                                            clsDataAccess objclsDataAccess = new clsDataAccess();
                                            SqlParameter[] objParams = new SqlParameter[0];
                                            dsS2 = objclsDataAccess.ExecuteSPReturnDataset("tblDefualtSetting_SelectAll", objParams);

                                            user.IsMoreThanBOMQty = bool.Parse(dsS2.Tables[0].Rows[0]["IsMinusAllow"].ToString());
                                            user.IsMinusAllow = bool.Parse(dsS2.Tables[0].Rows[0]["IsMinusAllow"].ToString());
                                            user.IsMultiWhseAllow = bool.Parse(dsS2.Tables[0].Rows[0]["MultiWhse"].ToString());
                                            user.IsOverGRNQty = bool.Parse(dsS2.Tables[0].Rows[0]["OverGRN"].ToString());
                                            user.IsTaxApplicable = bool.Parse(dsS2.Tables[0].Rows[0]["IsTaxApplicable"].ToString());
                                            user.IsTaxOnTax = bool.Parse(dsS2.Tables[0].Rows[0]["IsTaxOnTax"].ToString());
                                            user.IsOverSOQty = bool.Parse(dsS2.Tables[0].Rows[0]["OverSO"].ToString());
                                            user.IsReturnOverSupInv = bool.Parse(dsS2.Tables[0].Rows[0]["ReturnOverSupInv"].ToString());
                                            user.IsImportBegBal = bool.Parse(dsS2.Tables[0].Rows[0]["ImportBegBal"].ToString());
                                            user.IsReturnOverCustInv = bool.Parse(dsS2.Tables[0].Rows[0]["ReturnOverCustInv"].ToString());
                                            user.ArAccount = dsS2.Tables[0].Rows[0]["ArAccount"].ToString();
                                            user.ApAccount = dsS2.Tables[0].Rows[0]["APAccount"].ToString();
                                            user.tax1GL = dsS2.Tables[0].Rows[0]["tax1GL"].ToString();
                                            user.tax2GL = dsS2.Tables[0].Rows[0]["tax2GL"].ToString();
                                            user.DiscountGL = dsS2.Tables[0].Rows[0]["DiscountGL"].ToString();
                                            user.GLJob = dsS2.Tables[0].Rows[0]["GLJob"].ToString();
                                            user.SalesGLAccount = dsS2.Tables[0].Rows[0]["SalesGLAccount"].ToString();
                                            user.IssueNoteCurrentAC = dsS2.Tables[0].Rows[0]["IssueNoteCurrentAC"].ToString();
                                            user.TaxPayGL1 = dsS2.Tables[0].Rows[0]["TaxPayGL1"].ToString();
                                            user.LabChargGL = dsS2.Tables[0].Rows[0]["LabChargGL"].ToString();
                                            user.TaxPayGL2 = dsS2.Tables[0].Rows[0]["TaxPayGL2"].ToString();
                                            user.AdjustGL = dsS2.Tables[0].Rows[0]["AdjustGL"].ToString();
                                            user.RetrnIssuGL = dsS2.Tables[0].Rows[0]["RetrnIssuGL"].ToString();
                                            user.FTransGL = dsS2.Tables[0].Rows[0]["FTransGL"].ToString();
                                            user.TransportGL = dsS2.Tables[0].Rows[0]["TransportGL"].ToString();
                                            user.TransporItemID = dsS2.Tables[0].Rows[0]["TransportItemID"].ToString();
                                            user.DiscountItemID = dsS2.Tables[0].Rows[0]["DiscountID"].ToString();

                                            user.TaxPay1ID = dsS2.Tables[0].Rows[0]["TaxPayID1"].ToString();
                                            user.TaxPay2ID = dsS2.Tables[0].Rows[0]["TaxPayID2"].ToString();
                                            user.TaxRec1ID = dsS2.Tables[0].Rows[0]["tax1ID"].ToString();
                                            user.TaxRec2ID = dsS2.Tables[0].Rows[0]["tax2ID"].ToString();

                                            user.ServiceChargesGL = dsS2.Tables[0].Rows[0]["ServChargGL"].ToString();
                                            user.ServiceChargesItemID = dsS2.Tables[0].Rows[0]["ServChargID"].ToString();
                                            user.MergeAccUser = bool.Parse(dsS2.Tables[0].Rows[0]["Merge1"].ToString());

                                            SSql = " select * from tblDefaultOtherSettings ";
                                            cmdS2 = new SqlCommand(SSql);
                                            daS2 = new SqlDataAdapter(SSql, ConnectionString);
                                            dsS2 = new DataSet();
                                            daS2.Fill(dsS2);

                                            user.IsDirectINVEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsDirectINVEnbl"].ToString());
                                            user.IsInclTaxINVEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsInclTaxINVEnbl"].ToString());
                                            user.IsIndrctINVEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsIndrctINVEnbl"].ToString());
                                            user.IsPMINVEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsPMINVEnbl"].ToString());
                                            user.IsDirectRtnEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsDirectRtnEnbl"].ToString());
                                            user.IsIndRtnEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsIndRtnEnbl"].ToString());
                                            user.IsJobPreFormsEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsJobCosting"].ToString());
                                            user.IsGRNEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsGRN"].ToString());
                                            user.IsDelNoteEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsDelNote"].ToString());
                                            user.IsSupRetEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsSupRet"].ToString());
                                            user.IsSupInvEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsSupInv"].ToString());
                                            user.IsInvBegBalEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsInvBegBal"].ToString());
                                            user.IsWHTransEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsWHTrans"].ToString());
                                            user.IsInvAdjEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsInvAdj"].ToString());
                                            user.IsBranchSynkEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsBranchSynk"].ToString());
                                            user.IsFGRtransferEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsFGRtransferEnbl"].ToString());
                                            //user.IsBOQEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsBOQEnbl"].ToString());
                                            //user.IsBOMEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsBOMEnbl"].ToString());
                                            user.IsInvIssueReturnEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsInvIssueReturn1"].ToString());
                                            user.IsIncluRtnEnbl = bool.Parse(dsS2.Tables[0].Rows[0]["IsIncluRtnEnbl"].ToString());

                                            user.Period_begin_Date = DateTime.Parse(dsS2.Tables[0].Rows[0]["Period_begin_Date"].ToString());
                                            user.Period_End_Date = DateTime.Parse(dsS2.Tables[0].Rows[0]["Period_End_Date"].ToString());



                                            clsSerializeItem.DtsSerialNoList = new DataTable();
                                            clsSerializeItem.TblLotNos = new DataTable();
                                            try
                                            {
                                                SSql = "select * from tblWhseMaster WHERE IsDefault='true'";
                                                cmdS2 = new SqlCommand(SSql);
                                                daS2 = new SqlDataAdapter(SSql, ConnectionString);
                                                dsS2 = new DataSet();
                                                daS2.Fill(dsS2);
                                                user.StrDefaultWH = dsS2.Tables[0].Rows[0]["WhseId"].ToString();
                                            }
                                            catch
                                            {

                                            }

                                            //CurrentUser = user.userName;
                                            //string StrSql = null;    
                                            //string StrSql = "SELECT Merge1 from  tblDefualtSetting";
                                            //SqlCommand cmd4 = new SqlCommand(StrSql);
                                            //SqlDataAdapter da4 = new SqlDataAdapter(StrSql, ConnectionString);
                                            //DataTable dt4 = new DataTable();
                                            //da4.Fill(dt4);
                                            //Boolean IsChk = false;
                                            //if (dt4.Rows.Count > 0)
                                            //{
                                            //    IsChk = Convert.ToBoolean(dt4.Rows[0].ItemArray[0].ToString().Trim());
                                            //    if (IsChk == true)
                                            //    {                                                        
                                            //        Merge1AccUser = user.userName.ToString();
                                            //    }
                                            //}
                                            GetInvoicePrefex();
                                            user.LoginDate = dtplogindate.Value;
                                            clsBeansPhases.LOGINDATE = dtplogindate.Value;
                                            clsBeansPhases.USERNAME = CurrentUser;
                                            frmMain inv = new frmMain();
                                            inv.Show();
                                            this.Hide();

                                            //}
                                        }
                                        else
                                        {
                                            MessageBox.Show("You must Specify the Transaction Date");
                                        }
                                    }
                                }
                                else
                                {

                                    //=======================================================
                                    String S1 = "select LoginCount from tbllc";// where UserID = '" + txtUserName.Text.ToString().Trim() + "'";
                                    SqlCommand cmd1 = new SqlCommand(S1);
                                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                                    DataTable dt1 = new DataTable();
                                    da1.Fill(dt1);
                                    if (dt1.Rows.Count > 0)
                                    {
                                        if (Convert.ToDouble(dt1.Rows[0].ItemArray[0]) < 10)
                                        {

                                            CurrentUser = user.userName;
                                            MessageBox.Show("You are using a trial version Please register the product", "Please register the product", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                            String S3 = "Update tbllc SET LoginCount = LoginCount + 1";
                                            SqlCommand cmd3 = new SqlCommand(S3);
                                            SqlConnection con3 = new SqlConnection(ConnectionString);
                                            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                                            DataTable dt3 = new DataTable();
                                            da3.Fill(dt3);

                                            frmMain inv = new frmMain();
                                            inv.Show();
                                            this.Hide();


                                            // MessageBox.Show("You are using a trial version", "Please register the product", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                        {
                                            DialogResult reply = MessageBox.Show("Your Evaluation logins has been exceeded....  Do you want to Register the Product ?",
                                            "Perfect Distribution System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                                            if (reply == DialogResult.Yes)
                                            {

                                                Registration2 r2 = new Registration2();
                                                r2.Show();
                                                //IsRestart = true;
                                                //System.Windows.Forms.Application.Restart();
                                                //// Environment.Exit(0);
                                            }
                                            else if (reply == DialogResult.No)
                                            {
                                                // e.Cancel = true; //  state = 1;
                                                return;
                                            }
                                            //  MessageBox.Show("Please Register the product", "Evaluation Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }

                                    }
                                    else
                                    {
                                        //insert into the table
                                        int logincount = 1;
                                        String S2 = "insert into tbllc(UserName,LoginCount) values ('" + user.userName.ToString().Trim() + "','" + logincount + "')";
                                        SqlCommand cmd2 = new SqlCommand(S2);
                                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                                        DataSet ds2 = new DataSet();
                                        da2.Fill(ds2);

                                        CurrentUser = user.userName;
                                        frmMain inv = new frmMain();
                                        inv.Show();
                                        this.Hide();


                                    }
                                }
                                //kjjkjkjkjk

                                //======================================================



                                // user.userName = txtUserName.Text.ToString().Trim();//take the system user
                                // CurrentUser = user.userName;
                                // frmMain inv = new frmMain();
                                //// frmInvoice inv = new frmInvoice();
                                // inv.Show();
                                // this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Please Enter the Valid Password", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtPassWord.Text = "";
                                txtPassWord.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please Enter the Valid Data", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtUserName.Text = "";
                            txtPassWord.Text = "";
                            txtUserName.Focus();
                        }
                    }
                    else
                    {
                        if (txtUserName.Text == "" && txtPassWord.Text == "")
                        {
                            MessageBox.Show("Please Enter The Username And Password", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtUserName.Focus();
                        }
                        else
                        {
                            if (txtUserName.Text == "")
                            {
                                MessageBox.Show("Please Enter The User Name", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtUserName.Focus();
                            }
                            if (txtPassWord.Text == "")
                            {
                                MessageBox.Show("Please Enter The Password", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtPassWord.Focus();
                            }
                        }
                    }
                }//==============
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Login", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Application.Exit();
            }
            catch { }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            frmAddUser au = new frmAddUser();
            au.ShowDialog();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            //frmLogin abc = new frmLogin();

            //abc.txtUserName.Enabled = true;
            txtUserName.Focus();
        }

        private void lnkReg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                frmRegSettings objfrmRegSettings = new frmRegSettings();
                objfrmRegSettings.ShowDialog();
            }
            catch { }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(txtPassWord, txtUserName, e);
        }

        private void txtPassWord_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(btnLogin, txtUserName, e);
        }

        private void gradientWaitingBar2_Click(object sender, EventArgs e)
        {

        }

        private void rdobtnSpecificDate_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}