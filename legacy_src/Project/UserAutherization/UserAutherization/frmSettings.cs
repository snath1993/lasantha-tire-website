using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DataAccess;
using PCMBLL;
using System.Data.SqlClient;

namespace UserAutherization
{
    public partial class frmSettings : Form
    {
        clsBLLPhases objclsBLLPhases = null;
        clsDataAccess objclsDataAccess = null;
        SqlParameter[] objParams;
        clsCommon objclsCommon = new clsCommon();
        private string _msgTitle = "Settings";
        DataSet dataset = new DataSet();

        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            try
            {
                objclsDataAccess = new clsDataAccess();
                objParams = new SqlParameter[0];
                DataSet _dts = objclsDataAccess.ExecuteSPReturnDataset("tblDefualtSetting_SelectAll", objParams);
                dataset = _dts;
                foreach (DataRow dr in _dts.Tables[0].Rows)
                {
                    txtPGRN.Text = dr["GRNPrefix"].ToString();
                    txtPGRNNo.Text = dr["GRNNo"].ToString();
                    txtPSupInvNo.Text = dr["SupplierInvoiceNo"].ToString();
                    txtPSupInv.Text = dr["SupInvoicePrefix"].ToString();
                    txtPSupRetNo.Text = dr["SupplierReturnNo"].ToString();
                    txtPSupRet.Text = dr["SupReturnPrefix"].ToString();
                    txtPDelNoNo.Text = dr["DeliveryNoteNo"].ToString();
                    txtPDelNo.Text = dr["DeliveryNotePrefix"].ToString();
                    txtPCustInvNoDir.Text = dr["InvNum"].ToString();
                    txtPCustInvDir.Text = dr["InvPref"].ToString();
                    txtPCustInvNo.Text = dr["InvoiceNo"].ToString();
                    txtPCustInv.Text = dr["InvoicePrefix"].ToString();
                    txtPAdgNo.Text = dr["AdjustNum"].ToString();
                    txtPAdg.Text = dr["AdjustPref"].ToString();
                    txtPIssNotNo.Text = dr["IssueNoteNo"].ToString();
                    txtPIssNot.Text = dr["IssueNotePref"].ToString();
                    txtPRetNoteNo.Text = dr["ReturnNoteNo"].ToString();
                    txtPRetNote.Text = dr["ReturnNotePrefix"].ToString();
                    txtPTransNoNo.Text = dr["TrnNum"].ToString();
                    txtPTransNo.Text = dr["TrnPref"].ToString();
                    txtBOM.Text = dr["BOMPref"].ToString();
                    txtBOMNo.Text = dr["BOMNum"].ToString();
                    txtPBOQ.Text = dr["BOQPref"].ToString();
                    txtPBOQNo.Text = dr["BOQNum"].ToString();
                    txtPJobIss.Text = dr["SiteIssuePref"].ToString();
                    txtPJobIssNo.Text = dr["SiteIssueNum"].ToString();
                    txtPJobRet.Text = dr["SiteReturnsPref"].ToString();
                    txtPJobRetNo.Text = dr["SiteReturnsNum"].ToString();
                    txtPGRNPad.Text = dr["GRNPad"].ToString();
                    txtPSupInvPad.Text = dr["SupInvoicePad"].ToString();
                    txtPSupRetPad.Text = dr["SupReturnPad"].ToString();
                    txtPDelNoPad.Text = dr["DeliveryNotePad"].ToString();
                    txtPCustInvPadDir.Text = dr["InvPad"].ToString();
                    txtPCustInvPad.Text = dr["InvoicePad"].ToString();
                    txtPAdgPad.Text = dr["AdjustPad"].ToString();
                    txtPIssNotPad.Text = dr["IssueNotePad"].ToString();
                    txtPRetNotePad.Text = dr["ReturnNotePad"].ToString();
                    txtPTransNoPad.Text = dr["TrnPad"].ToString();
                    txtPBOQPad.Text = dr["BOQPad"].ToString();
                    txtBOMPad.Text = dr["BOMPad"].ToString();
                    txtPJobIssPad.Text = dr["SiteIssuePad"].ToString();
                    txtPJobRetPad.Text = dr["SiteReturnsPad"].ToString();    
                    txtPCustRetNo.Text = dr["CusReturnNo"].ToString();
                    txtPCustRet.Text = dr["CusReturnPrefix"].ToString();
                    txtPCustRetPad.Text = dr["CusReturnPad"].ToString();
                    txtPCustRetNoDir.Text = dr["CRNNum"].ToString();
                    txtPCustRetDir.Text = dr["CRNPref"].ToString();
                    txtPCustRetPadDir.Text = dr["CRNPad"].ToString();
                    txtSOdg.Text = dr["SOPref"].ToString();
                    txtSONO.Text = dr["SONum"].ToString();
                    txtSONOPad.Text = dr["SOPad"].ToString();
                    txtInvCreditPref.Text = dr["InvCreditPref"].ToString();
                    txtInvCreditNum.Text = dr["InvCreditNum"].ToString();
                    txtInvCreditPad.Text = dr["InvCreditPad"].ToString();
                    txtInvDRCreditPref.Text = dr["InvDRCreditPref"].ToString();
                    txtInvDRCreditNum.Text = dr["InvDRCreditNum"].ToString();
                    txtInvDRCreditPad.Text = dr["InvDRCreditPad"].ToString();	

                }

                _dts = objclsDataAccess.ExecuteSPReturnDataset("tblDefualtSettingCodeGen_Select", objParams);
                
                foreach (DataRow dr in _dts.Tables[0].Rows)
                {
                    chkPGRN.Checked = bool.Parse(dr["GRN"].ToString());
                    chkPDelNo.Checked = bool.Parse(dr["DN"].ToString());
                    chkPCustInv.Checked = bool.Parse(dr["CINV"].ToString());
                    chkPSupInv.Checked = bool.Parse(dr["SINV"].ToString());
                    chkPCustRet.Checked = bool.Parse(dr["CRTN"].ToString());
                    chkPSupRet.Checked = bool.Parse(dr["SRTN"].ToString());
                    chkPTrans.Checked = bool.Parse(dr["TN"].ToString());
                    chkPIssNot.Checked = bool.Parse(dr["ISN"].ToString());
                    chkPRetNote.Checked = bool.Parse(dr["RTN"].ToString());
                    chkPJobIss.Checked = bool.Parse(dr["JISN"].ToString());
                    chkPJobRet.Checked = bool.Parse(dr["JRTN"].ToString());
                    chkPBOQ.Checked = bool.Parse(dr["BOQ"].ToString());
                    chkBOM.Checked = bool.Parse(dr["BOM"].ToString());
                    chkPCustInvDir.Checked = bool.Parse(dr["CINVD"].ToString());
                    chkPCustRetDir.Checked = bool.Parse(dr["CRTND"].ToString());
                    chkauto.Checked = bool.Parse(dr["SOD"].ToString());
                    chkindc.Checked = bool.Parse(dr["INVC"].ToString());
                    chkindvc.Checked = bool.Parse(dr["INVDC"].ToString());
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

                if (int.Parse(dataset.Tables[0].Rows[0]["GRNNo"].ToString()) > int.Parse(txtPGRNNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence for...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPGRNNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["SupplierInvoiceNo"].ToString()) > int.Parse(txtPSupInvNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPSupInvNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["SupplierReturnNo"].ToString()) > int.Parse(txtPSupRetNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPSupRetNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["DeliveryNoteNo"].ToString()) > int.Parse(txtPDelNoNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPDelNoNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["InvNum"].ToString()) > int.Parse(txtPCustInvNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPCustInvNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["AdjustNum"].ToString()) > int.Parse(txtPAdgNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPAdgNo.Focus();
                    return;
                }



                if (int.Parse(dataset.Tables[0].Rows[0]["IssueNoteNo"].ToString()) > int.Parse(txtPIssNotNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPIssNotNo.Focus();
                    return;
                }



                if (int.Parse(dataset.Tables[0].Rows[0]["ReturnNoteNo"].ToString()) > int.Parse(txtPRetNoteNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPRetNoteNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["TrnNum"].ToString()) > int.Parse(txtPTransNoNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPTransNoNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["BOQNum"].ToString()) > int.Parse(txtPBOQNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPBOQNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["BOMNum"].ToString()) > int.Parse(txtBOMNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtBOMNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["SiteIssueNum"].ToString()) > int.Parse(txtPJobIssNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPJobIssNo.Focus();
                    return;
                }
                if (int.Parse(dataset.Tables[0].Rows[0]["SiteReturnsNum"].ToString()) > int.Parse(txtPJobRetNo.Text.Trim()))
                {
                    MessageBox.Show("Unable to decrease the Sequence...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPJobRetNo.Focus();
                    return;
                }
                objParams = new SqlParameter[57];                
                objParams[0] = new SqlParameter("GRNPrefix", txtPGRN.Text.Trim());
                objParams[1] = new SqlParameter("GRNNo", txtPGRNNo.Text.Trim());
                objParams[2] = new SqlParameter("SiteReturnsPad", txtPJobRetPad.Text.Trim());
                objParams[3] = new SqlParameter("GRNPad", txtPGRNPad.Text.Trim());
                objParams[4] = new SqlParameter("SupplierInvoiceNo", txtPSupInvNo.Text.Trim());
                objParams[5] = new SqlParameter("SupInvoicePrefix", txtPSupInv.Text.Trim());
                objParams[6] = new SqlParameter("SupInvoicePad", txtPSupInvPad.Text.Trim());
                objParams[7] = new SqlParameter("SupplierReturnNo", txtPSupRetNo.Text.Trim());
                objParams[8] = new SqlParameter("SupReturnPrefix", txtPSupRet.Text.Trim());
                objParams[9] = new SqlParameter("SupReturnPad", txtPSupRetPad.Text.Trim());
                objParams[10] = new SqlParameter("DeliveryNoteNo", txtPDelNoNo.Text.Trim());
                objParams[11] = new SqlParameter("DeliveryNotePrefix", txtPDelNo.Text.Trim());
                objParams[12] = new SqlParameter("DeliveryNotePad", txtPDelNoPad.Text.Trim()); 
                objParams[13] = new SqlParameter("InvNum", txtPCustInvNoDir.Text.Trim());
                objParams[14] = new SqlParameter("InvPref", txtPCustInvDir.Text.Trim());
                objParams[15] = new SqlParameter("InvoiceNo", txtPCustInvNo.Text.Trim());
                objParams[16] = new SqlParameter("InvoicePrefix", txtPCustInv.Text.Trim());
                objParams[17] = new SqlParameter("InvPad", txtPCustInvPadDir.Text.Trim());
                objParams[18] = new SqlParameter("AdjustNum", txtPAdgNo.Text.Trim());
                objParams[19] = new SqlParameter("AdjustPref", txtPAdg.Text.Trim());
                objParams[20] = new SqlParameter("AdjustPad", txtPAdgPad.Text.Trim());
                objParams[21] = new SqlParameter("IssueNoteNo", txtPIssNotNo.Text.Trim());
                objParams[22] = new SqlParameter("IssueNotePref", txtPIssNot.Text.Trim());
                objParams[23] = new SqlParameter("IssueNotePad", txtPIssNotPad.Text.Trim());
                objParams[24] = new SqlParameter("ReturnNoteNo", txtPRetNoteNo.Text.Trim());
                objParams[25] = new SqlParameter("ReturnNotePrefix", txtPRetNote.Text.Trim());
                objParams[26] = new SqlParameter("ReturnNotePad", txtPRetNotePad.Text.Trim());
                objParams[27] = new SqlParameter("TrnNum", txtPTransNoNo.Text.Trim());
                objParams[28] = new SqlParameter("TrnPref", txtPTransNo.Text.Trim());
                objParams[29] = new SqlParameter("TrnPad", txtPTransNoPad.Text.Trim());
                objParams[30] = new SqlParameter("BOQNum", txtPBOQNo.Text.Trim());
                objParams[31] = new SqlParameter("BOQPref", txtPBOQ.Text.Trim());
                objParams[32] = new SqlParameter("BOQPad", txtPBOQPad.Text.Trim());
                objParams[33] = new SqlParameter("BOMNum", txtBOMNo.Text.Trim());
                objParams[34] = new SqlParameter("BOMPref", txtBOM.Text.Trim());
                objParams[35] = new SqlParameter("BOMPad", txtBOMPad.Text.Trim());    
                objParams[36] = new SqlParameter("SiteIssueNum", txtPJobIssNo.Text.Trim());
                objParams[37] = new SqlParameter("SiteIssuePref", txtPJobIss.Text.Trim());
                objParams[38] = new SqlParameter("SiteIssuePad", txtPJobIssPad.Text.Trim());  
                objParams[39] = new SqlParameter("SiteReturnsNum", txtPJobRetNo.Text.Trim());
                objParams[40] = new SqlParameter("SiteReturnsPref", txtPJobRet.Text.Trim());
                objParams[41] = new SqlParameter("CusReturnNo", txtPCustRetNo.Text.Trim());
                objParams[42] = new SqlParameter("CusReturnPrefix", txtPCustRet.Text.Trim());
                objParams[43] = new SqlParameter("CusReturnPad", txtPCustRetPad.Text.Trim());
                objParams[44] = new SqlParameter("InvoicePad", txtPCustInvPad.Text.Trim());
                objParams[45] = new SqlParameter("CRNPref", txtPCustRetDir.Text.Trim());
                objParams[46] = new SqlParameter("CRNPad", txtPCustRetPadDir.Text.Trim());
                objParams[47] = new SqlParameter("CRNNum", txtPCustRetNoDir.Text.Trim()); 
                objParams[48] = new SqlParameter("SOPref", txtSOdg.Text.Trim());
                objParams[49] = new SqlParameter("SONum", txtSONO.Text.Trim());
                objParams[50] = new SqlParameter("SOPad", txtSONOPad.Text.Trim());
                objParams[51] = new SqlParameter("InvCreditPref", txtInvCreditPref.Text.Trim());
                objParams[52] = new SqlParameter("InvCreditNum", txtInvCreditNum.Text.Trim());
                objParams[53] = new SqlParameter("InvCreditPad", txtInvCreditPad.Text.Trim());
                objParams[54] = new SqlParameter("InvDRCreditPref", txtInvDRCreditPref.Text.Trim());
                objParams[55] = new SqlParameter("InvDRCreditNum", txtInvDRCreditNum.Text.Trim());
                objParams[56] = new SqlParameter("InvDRCreditPad", txtInvDRCreditPad.Text.Trim()); 

                objclsDataAccess = new clsDataAccess();
                objclsDataAccess.BeginTransaction();
                objclsDataAccess.ExecuteSPReturnObject("tblDefualtSetting_Insert", objParams);

                objParams = new SqlParameter[18];
                objParams[0] = new SqlParameter("GRN", chkPGRN.Checked);
                objParams[1] = new SqlParameter("DN", chkPDelNo.Checked);
                objParams[2] = new SqlParameter("CINV", chkPCustInv.Checked);
                objParams[3] = new SqlParameter("SINV", chkPCustInvDir.Checked);
                objParams[4] = new SqlParameter("CRTN", chkPCustRet.Checked);
                objParams[5] = new SqlParameter("SRTN", chkPSupRet.Checked);
                objParams[6] = new SqlParameter("TN", chkPTrans.Checked);
                objParams[7] = new SqlParameter("ISN", chkPIssNot.Checked);
                objParams[8] = new SqlParameter("RTN", chkPRetNote.Checked);
                objParams[9] = new SqlParameter("JISN", chkPJobIss.Checked);
                objParams[10] = new SqlParameter("JRTN", chkPJobRet.Checked);
                objParams[11] = new SqlParameter("BOQ", chkPBOQ.Checked);
                objParams[12] = new SqlParameter("BOM", chkBOM.Checked);
                objParams[13] = new SqlParameter("CINVD", chkPJobIss.Checked);
                objParams[14] = new SqlParameter("CRTND", chkPCustRetDir.Checked);
                objParams[15] = new SqlParameter("SOD", chkauto.Checked);
                objParams[16] = new SqlParameter("INVC", chkindc.Checked);
                objParams[17] = new SqlParameter("INVDC", chkindvc.Checked);

                objclsDataAccess.ExecuteSPReturnObject("tblDefualtSettingCodeGen_Insert", objParams);
                
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

        private void txtPGRNNo_TextChanged(object sender, EventArgs e)
        {
            erpMaster.Dispose();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        
    }
}