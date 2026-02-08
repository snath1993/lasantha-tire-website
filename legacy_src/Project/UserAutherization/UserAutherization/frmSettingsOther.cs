using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace UserAutherization
{
    public partial class frmSettingsOther : Form
    {
        //clsBLLPhases objclsBLLPhases = null;
        clsDataAccess objclsDataAccess = null;
        SqlParameter[] objParams;
        clsCommon objclsCommon = new clsCommon();
        private string _msgTitle = "Settings";
        public static string ConnectionString;
        SqlConnection myConnection = new SqlConnection(ConnectionString);
        SqlTransaction myTrans = null;  
        public frmSettingsOther()
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
                throw ex;
            }
        }
        private void frmSettingsAccounts_Load(object sender, EventArgs e)
        {
            try
            {
                viewinvdfhistory();
                objclsDataAccess = new clsDataAccess();
                objParams = new SqlParameter[0];
                DataSet _dts = objclsDataAccess.ExecuteSPReturnDataset("tblDefaultOtherSettings_select", objParams);

                foreach(DataRow dr in _dts.Tables[0].Rows)
                {
                    chkDirCustInv.Checked =bool.Parse(dr["IsDirectINVEnbl"].ToString());
                    chkIncExcInv.Checked = bool.Parse(dr["IsInclTaxINVEnbl"].ToString());
                    chkIndiCustInv.Checked = bool.Parse(dr["IsIndrctINVEnbl"].ToString());
                    chkCustInvoicePriceMet.Checked = bool.Parse(dr["IsPMINVEnbl"].ToString());
                    chkDirCustRet.Checked = bool.Parse(dr["IsDirectRtnEnbl"].ToString());
                    chkInclExclCustRet.Checked = bool.Parse(dr["IsIncluRtnEnbl"].ToString());
                    chkIndCustRetrn.Checked = bool.Parse(dr["IsIndRtnEnbl"].ToString());
                    chkFGT.Checked = bool.Parse(dr["IsFGRtransferEnbl"].ToString());
                    chkJobCost.Checked = bool.Parse(dr["IsJobCosting"].ToString());
                    chkgrn.Checked = bool.Parse(dr["IsGRN"].ToString());
                    chkDelNote.Checked = bool.Parse(dr["IsDelNote"].ToString());
                    chkSupRet.Checked = bool.Parse(dr["IsSupRet"].ToString());
                    chkSupInv.Checked = bool.Parse(dr["IsSupInv"].ToString());
                    chkInvBegBal.Checked = bool.Parse(dr["IsInvBegBal"].ToString());
                    chkInvWHTrans.Checked = bool.Parse(dr["IsWHTrans"].ToString());
                    chkInvAdj.Checked = bool.Parse(dr["IsInvAdj"].ToString());
                    chkBranchSynk.Checked = bool.Parse(dr["IsBranchSynk"].ToString());
                    chkInvIsRt.Checked = bool.Parse(dr["IsInvIssueReturn1"].ToString());
                    
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAccDilivery();
                objclsDataAccess = new clsDataAccess();
                objclsDataAccess.BeginTransaction();

                objParams = new SqlParameter[18];
                objParams[17] = new SqlParameter("IsInvIssueReturn1", chkInvIsRt.Checked);
                objParams[0] = new SqlParameter("IsDirectINVEnbl", chkDirCustInv.Checked);
                objParams[1] = new SqlParameter("IsInclTaxINVEnbl", chkIncExcInv.Checked);
                objParams[2] = new SqlParameter("IsIndrctINVEnbl", chkIndiCustInv.Checked);
                objParams[3] = new SqlParameter("IsPMINVEnbl", chkCustInvoicePriceMet.Checked);
                objParams[4] = new SqlParameter("IsDirectRtnEnbl", chkDirCustRet.Checked);
                objParams[5] = new SqlParameter("IsIncluRtnEnbl", chkInclExclCustRet.Checked);
                objParams[6] = new SqlParameter("IsIndRtnEnbl", chkIndCustRetrn.Checked);
                objParams[7] = new SqlParameter("IsFGRtransferEnbl", chkFGT.Checked);
                objParams[8] = new SqlParameter("IsJobCosting", chkJobCost.Checked);
                objParams[9] = new SqlParameter("IsGRN", chkgrn.Checked);
                objParams[10] = new SqlParameter("IsDelNote", chkDelNote.Checked);
                objParams[11] = new SqlParameter("IsSupRet", chkSupRet.Checked);
                objParams[12] = new SqlParameter("IsSupInv", chkSupInv.Checked);
                objParams[13] = new SqlParameter("IsInvBegBal", chkInvBegBal.Checked);
                objParams[14] = new SqlParameter("IsWHTrans", chkInvWHTrans.Checked);
                objParams[15] = new SqlParameter("IsInvAdj", chkInvAdj.Checked);
                objParams[16] = new SqlParameter("IsBranchSynk", chkBranchSynk.Checked);

                objclsDataAccess.ExecuteSPReturnObject("tblDefaultOtherSettings_Insert", objParams);

                objParams = new SqlParameter[6];
                objParams[0] = new SqlParameter("IsMinusAllow", chkWOutStock.Checked);
                objParams[1] = new SqlParameter("OverGRN", chkOverPO.Checked);
                objParams[2] = new SqlParameter("OverSO", chkOverSO.Checked);
                objParams[3] = new SqlParameter("ReturnOverSupInv", chkBranchSynk.Checked);
                objParams[4] = new SqlParameter("ImportBegBal", chkBranchSynk.Checked);
                objParams[5] = new SqlParameter("ReturnOverCustInv", chkBranchSynk.Checked);

                objclsDataAccess.ExecuteSPReturnObject("tblDefualtSetting_Insert_Validations", objParams);
               
                objclsDataAccess.CommitTransaction();
                
                MessageBox.Show("Saved Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                MessageBox.Show("Restart the System to Apply Changes...!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                objclsDataAccess.RollBackTransaction();
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        private void viewinvdfhistory()
        {
            try
            {
                string StrSql = "SELECT SalesWithDiliveryOrder,Merge,GL_True from  tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Boolean Isok=false ;
                Boolean IsChk = false;
                Boolean IsGlChk = false;
                if (dt.Rows.Count > 0)
                {
                    Isok = Convert.ToBoolean(dt.Rows[0].ItemArray[0].ToString().Trim());
                    if (Isok == true)
                    {
                        chkaccessdilnote.Checked = true;
                    }
                    else if (Isok == false)
                    {
                        chkaccessdilnote.Checked = false;
                    }
                    IsChk = Convert.ToBoolean(dt.Rows[0].ItemArray[1].ToString().Trim());
                    if (IsChk == true)
                    {
                        chkMerge.Checked = true;
                    }
                    else if (IsChk == false)
                    {
                        chkMerge.Checked = false;
                    }
                    IsGlChk = Convert.ToBoolean(dt.Rows[0].ItemArray[2].ToString().Trim());
                    if (IsGlChk == true)
                    {
                        chkaccess.Checked = true;
                    }
                    else if (IsGlChk == false)
                    {
                        chkaccess.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveAccDilivery()
        {
            try
            {
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();
                double OptYN = 0;
                if (chkaccessdilnote.Checked == true)
                {
                    OptYN = 1;
                }
                else if (chkaccessdilnote.Checked == true)
                {
                    OptYN = 0;
                }

                SqlCommand cmd1 = new SqlCommand("UPDATE tblDefualtSetting SET SalesWithDiliveryOrder='" + OptYN.ToString() + "',Merge='" + chkMerge.Checked.ToString() + "',GL_True='" + chkaccess.Checked.ToString() + "' select SalesWithDiliveryOrder,GL_True from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daSupIn = new SqlDataAdapter(cmd1);
                DataTable dtSupIn = new DataTable();
                daSupIn.Fill(dtSupIn);
                if (dtSupIn.Rows.Count <= 0)
                {
                    SqlCommand cmd2 = new SqlCommand("Insert into tblDefualtSetting (SalesWithDiliveryOrder,Merge,GL_True) values ('" + OptYN.ToString() + "','" + chkMerge.Checked.ToString() + "','" + chkaccess.Checked.ToString() + "')", myConnection, myTrans);
                    cmd2.ExecuteNonQuery();
                }
                myTrans.Commit();

            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                myConnection.Close();
            }
            
        }
    }
}