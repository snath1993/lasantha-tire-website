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
    public partial class frmSettingsTax : Form
    {
        clsBLLPhases objclsBLLPhases = null;
        clsDataAccess objclsDataAccess = null;
        SqlParameter[] objParams;
        clsCommon objclsCommon = new clsCommon();
        private string _msgTitle = "Settings";

        public frmSettingsTax()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            try
            {
                objclsDataAccess = new clsDataAccess();
                objParams = new SqlParameter[0];
                DataSet _dts = objclsDataAccess.ExecuteSPReturnDataset("tblTaxApplicable_Select", objParams);

                cmbTaxID.DataSource = _dts.Tables[0];
                cmbTaxID.DisplayMember = "TaxID";
                cmbTaxID.ValueMember = "TaxID";
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
                objclsDataAccess = new clsDataAccess();
                objclsDataAccess.BeginTransaction();

                objParams = new SqlParameter[6];
                objParams[0] = new SqlParameter("TaxID", cmbTaxID.Text.Trim());
                objParams[1] = new SqlParameter("TaxName", txttaxName.Text.Trim());
                objParams[2] = new SqlParameter("Rate", txtrate.Text.Trim());
                objParams[3] = new SqlParameter("Rank", txtRank.Text.Trim());
                objParams[4] = new SqlParameter("IsActive", chkIsActive.Checked);
                objParams[5] = new SqlParameter("IsTaxOnTax", rbttaxOnTax.Checked);              
                objclsDataAccess.ExecuteSPReturnObject("tblTaxApplicable_insert", objParams);                     
                objclsDataAccess.CommitTransaction();
                MessageBox.Show("Saved Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                objclsDataAccess.RollBackTransaction();
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbTaxID_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                objclsDataAccess = new clsDataAccess();
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("TaxID", cmbTaxID.Text.Trim());
                DataSet _dts = objclsDataAccess.ExecuteSPReturnDataset("tblTaxApplicable_select_By_ID", objParams);
                foreach (DataRow dr in _dts.Tables[0].Rows)
                {
                    cmbTaxID.Text = dr["TaxID"].ToString();
                    txttaxName.Text = dr["TaxName"].ToString();
                    txtrate.Text = dr["Rate"].ToString();
                    txtRank.Text = dr["Rank"].ToString();
                    chkIsActive.Checked =bool.Parse(dr["IsActive"].ToString());
                    rbttaxOnTax.Checked =bool.Parse(dr["IsTaxOnTax"].ToString());
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rbttaxOnTax_CheckedChanged(object sender, EventArgs e)
        {
            if (rbttaxOnTax.Checked)
                rbtIndTax.Checked = false;
        }

        private void rbtIndTax_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtIndTax.Checked)
                rbttaxOnTax.Checked = false;
        }

        

        
    }
}