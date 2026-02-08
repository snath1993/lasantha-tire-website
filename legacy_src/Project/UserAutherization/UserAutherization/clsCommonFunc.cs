using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using PCMBLL;
using PCMBeans;
using System.Data;
using System.Windows.Forms;

namespace UserAutherization
{
    public class clsCommonFunc
    {
        clsBLLPhases objclsBLLPhases;

        public void fillSites(UltraCombo _ucmbSiteID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbSiteID.DataSource = null;
                _ucmbSiteID.DataSource = objclsBLLPhases.GetJob_AllActive_ForDropDown();
                _ucmbSiteID.DisplayMember = "JobID";
                _ucmbSiteID.ValueMember = "JobID";
                _ucmbSiteID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbSiteID.DisplayLayout.Bands[0].Columns[1].Width = 100;
                _ucmbSiteID.DisplayLayout.Bands[0].Columns[2].Width = 80;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillSubContractors(UltraCombo _ucmbSubConID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbSubConID.DataSource = objclsBLLPhases.GetSubContractor_AllActive_ForDropDown();
                _ucmbSubConID.DisplayMember = "ID";
                _ucmbSubConID.ValueMember = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillCustomer(UltraCombo _ucmbCustomerID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbCustomerID.DataSource = objclsBLLPhases.GetCustomer_AllActive_ForDropDown();
                _ucmbCustomerID.DisplayMember = "ID";
                _ucmbCustomerID.ValueMember = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillJobs(UltraCombo _ucmbSiteID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbSiteID.DataSource = objclsBLLPhases.GetJob_All();
                _ucmbSiteID.DisplayMember = "JobID";
                _ucmbSiteID.ValueMember = "JobID";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillJobs_Active(UltraCombo _ucmbSiteID, UltraCombo _ucmbCustomerID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                if(_ucmbCustomerID.Text.Trim().Length > 0)
                    _ucmbSiteID.DataSource = objclsBLLPhases.GetJobs_AllActive_ByCustomerID(_ucmbCustomerID.Text.Trim());
                else
                    _ucmbSiteID.DataSource = objclsBLLPhases.GetJob_All();

                _ucmbSiteID.DisplayMember = "JobID";
                _ucmbSiteID.ValueMember = "JobID";


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillBillingMethod(UltraCombo _ucmbBillingMethod)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbBillingMethod.DataSource = objclsBLLPhases.GetBillingMethods_AllActive_ForDropDown();
                _ucmbBillingMethod.DisplayMember = "Description";
                _ucmbBillingMethod.ValueMember = "Code";
                _ucmbBillingMethod.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                _ucmbBillingMethod.DisplayLayout.Bands[0].Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillJobStatus(UltraCombo _ucmbStatus)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbStatus.DataSource = objclsBLLPhases.GetJobStatus_AllActive_ForDropDown();
                _ucmbStatus.DisplayMember = "Description";
                _ucmbStatus.ValueMember = "Description";
                //_ucmbStatus.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                //_ucmbStatus.DisplayLayout.Bands[0].Columns[1].Width = 300;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillJobCategory(UltraCombo _ucmbStatus)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbStatus.DataSource = objclsBLLPhases.GetJobCategory_AllActive_ForDropDown();
                _ucmbStatus.DisplayMember = "Description";
                _ucmbStatus.ValueMember = "Description";
                //_ucmbStatus.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                //_ucmbStatus.DisplayLayout.Bands[0].Columns[1].Width = 300;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillBOQs(UltraCombo _ucmbBOQID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbBOQID.DataSource = null;
                _ucmbBOQID.DataSource = objclsBLLPhases.GetBOQs_All();
                _ucmbBOQID.DisplayMember = "BOQID";
                _ucmbBOQID.ValueMember = "BOQID";
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[1].Width = 40;
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[2].Width = 100;
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "SiteID";
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Revision No";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillBOQs_Active(UltraCombo _ucmbBOQID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbBOQID.DataSource = null;
                _ucmbBOQID.DataSource = objclsBLLPhases.GetBOQs_AllActive_Dropdown();
                _ucmbBOQID.DisplayMember = "BOQID";
                _ucmbBOQID.ValueMember = "BOQID";
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[1].Width = 40;
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[2].Width = 100;
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "SiteID";
                _ucmbBOQID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Revision No";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void fillBOQs_Active_Max(UltraCombo _ucmbBOQID)
        //{
        //    objclsBLLPhases = new clsBLLPhases();
        //    try
        //    {
        //        _ucmbBOQID.DataSource = null;
        //        _ucmbBOQID.DataSource = objclsBLLPhases.GetBOQs_AllActive_Dropdown();
        //        _ucmbBOQID.DisplayMember = "BOQID";
        //        _ucmbBOQID.ValueMember = "BOQID";
        //        _ucmbBOQID.DisplayLayout.Bands[0].Columns[0].Width = 40;
        //        _ucmbBOQID.DisplayLayout.Bands[0].Columns[1].Width = 40;
        //        _ucmbBOQID.DisplayLayout.Bands[0].Columns[2].Width = 100;
        //        _ucmbBOQID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
        //        _ucmbBOQID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "SiteID";
        //        _ucmbBOQID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Revision No";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void fillLocations(UltraCombo _ucmbLocCode)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbLocCode.DataSource = null;
                _ucmbLocCode.DataSource = objclsBLLPhases.GetLocations_AllActive_Dropdown();
                _ucmbLocCode.DisplayMember = "ID";
                _ucmbLocCode.ValueMember = "ID";
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Width = 100;
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet fillLocations_By_SubPhaseID(UltraCombo _ucmbLocCode,string _SubPhaseID)
        {
            objclsBLLPhases = new clsBLLPhases();
            DataSet _objDataset = new DataSet();
            try
            {
                _ucmbLocCode.DataSource = null;
                _objDataset = objclsBLLPhases.GetLocations_AllActive_Dropdown_By_SubPhaseID(_SubPhaseID);
                _ucmbLocCode.DataSource = _objDataset;
                _ucmbLocCode.DisplayMember = "ID";
                _ucmbLocCode.ValueMember = "ID";
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Width = 100;
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _objDataset;
        }

        public DataSet fillLocations_By_PhaseID(UltraCombo _ucmbLocCode, string _PhaseID)
        {
            objclsBLLPhases = new clsBLLPhases();
            DataSet _objDataset = new DataSet();
            try
            {
                _ucmbLocCode.DataSource = null;
                _objDataset = objclsBLLPhases.GetLocations_AllActive_Dropdown_By_PhaseID(_PhaseID);
                _ucmbLocCode.DataSource = _objDataset;
                _ucmbLocCode.DisplayMember = "ID";
                _ucmbLocCode.ValueMember = "ID";
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Width = 100;
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                _ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _objDataset;
        }

        public void fillPhases(UltraCombo _ucmbPhaseID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbPhaseID.DataSource = null;
                _ucmbPhaseID.DataSource = objclsBLLPhases.GetPhases_AllActive_ForDropDown();
                _ucmbPhaseID.DisplayMember = "PhaseID";
                _ucmbPhaseID.ValueMember = "PhaseID";
                _ucmbPhaseID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbPhaseID.DisplayLayout.Bands[0].Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillSubPhases(UltraCombo _ucmbSubPhaseID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbSubPhaseID.DataSource = null;
                _ucmbSubPhaseID.DataSource = objclsBLLPhases.GetSubPhases_AllActive_ForDropDown();
                _ucmbSubPhaseID.DisplayMember = "SubPhaseID";
                _ucmbSubPhaseID.ValueMember = "SubPhaseID";
                _ucmbSubPhaseID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbSubPhaseID.DisplayLayout.Bands[0].Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillBOMs(UltraCombo _ucmbBOMID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbBOMID.DataSource = null;
                _ucmbBOMID.DataSource = objclsBLLPhases.GetBOMs_AllActive_Dropdown();
                _ucmbBOMID.DisplayMember = "BOMID";
                _ucmbBOMID.ValueMember = "BOMID";
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[1].Width = 40;
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[2].Width = 100;
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "SiteID";
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Description";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillBOMs_Active(UltraCombo _ucmbBOMID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbBOMID.DataSource = null;
                _ucmbBOMID.DataSource = objclsBLLPhases.GetBOMs_All();
                _ucmbBOMID.DisplayMember = "BOMID";
                _ucmbBOMID.ValueMember = "BOMID";
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[1].Width = 40;
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[2].Width = 100;
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "SiteID";
                _ucmbBOMID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Description";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillItems(UltraCombo _ucmbItems)
        {
            objclsBLLPhases = new clsBLLPhases();
           try
            {
                _ucmbItems.DataSource = null;
                _ucmbItems.DataSource = objclsBLLPhases.GetItems_All();
                _ucmbItems.DisplayMember = "ItemId";
                _ucmbItems.ValueMember = "ItemId";
                _ucmbItems.DisplayLayout.Bands[0].Columns[0].Width = 100;
                _ucmbItems.DisplayLayout.Bands[0].Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillContractr(UltraCombo _ucmbContractr)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbContractr.DataSource = null;
                _ucmbContractr.DataSource = objclsBLLPhases.GetContractor_AllActive_ForDropDown();
                _ucmbContractr.DisplayMember = "ID";
                _ucmbContractr.ValueMember = "Name";
                _ucmbContractr.DisplayLayout.Bands[0].Columns[0].Width = 100;
                _ucmbContractr.DisplayLayout.Bands[0].Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillWarehouse_By_Item(UltraCombo ucmbLocCode, string _ItemID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbLocCode.DataSource = null;
                ucmbLocCode.DataSource = objclsBLLPhases.GetWarehouses_By_ItemID(_ItemID);
                ucmbLocCode.DisplayMember = "WhseId";
                ucmbLocCode.ValueMember = "WhseId";
                ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Width = 100;
                ucmbLocCode.DisplayLayout.Bands[0].Columns[2].Width = 100;
                ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
                ucmbLocCode.DisplayLayout.Bands[0].Columns[2].Header.Caption = "OnHand Qty.";
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillIssues(UltraCombo _ucmbIssueID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbIssueID.DataSource = null;
                _ucmbIssueID.DataSource = objclsBLLPhases.GetIssues_All();
                _ucmbIssueID.DisplayMember = "IssueNo";
                _ucmbIssueID.ValueMember = "IssueNo";
                _ucmbIssueID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                _ucmbIssueID.DisplayLayout.Bands[0].Columns[1].Width = 40;
                _ucmbIssueID.DisplayLayout.Bands[0].Columns[2].Width = 100;
                _ucmbIssueID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                _ucmbIssueID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "SiteID";
                _ucmbIssueID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Description";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillReturns(UltraCombo _ucmbReturnID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                _ucmbReturnID.DataSource = null;
                _ucmbReturnID.DataSource = objclsBLLPhases.GetReturns_All();
                _ucmbReturnID.DisplayMember = "ReturnNo";
                _ucmbReturnID.ValueMember = "ReturnNo";
                _ucmbReturnID.DisplayLayout.Bands[0].Columns[0].Width = 60;
                _ucmbReturnID.DisplayLayout.Bands[0].Columns[1].Width = 60;
                _ucmbReturnID.DisplayLayout.Bands[0].Columns[2].Width = 60;
                _ucmbReturnID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ReturnNo";
                _ucmbReturnID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "IssueNo";
                _ucmbReturnID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "SiteID";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillCustomer(UltraCombo _ucmbCustomerID, UltraCombo _ucmbSiteID)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                if (_ucmbSiteID.Value != null && _ucmbSiteID.Value.ToString().Trim().Length > 0)
                {
                    _ucmbCustomerID.DataSource = null;
                    _ucmbCustomerID.DataSource = objclsBLLPhases.GetCustomer_AllActive_BySiteID(_ucmbSiteID.Value.ToString().Trim());
                    _ucmbCustomerID.DisplayMember = "ID";
                    _ucmbCustomerID.ValueMember = "Name";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public double GetColumnTotalsFor_DuplicatedRowsInGrid(DataGridView dgvData, string KeyColumnName, string ValueColumnName, string KeyColumnValue, double ValueColumnValue)
        //{
        //    string _KeyCode = "";
        //    double _KeyValue = 0;
        //    try
        //    {
        //        _KeyCode = KeyColumnValue;
        //        //_KeyValue = ValueColumnValue;

        //        foreach (DataGridViewRow dgvr in dgvData.Rows)
        //        {
        //            if (dgvr.Cells[KeyColumnName].Value != null && dgvr.Cells[KeyColumnName].Value.ToString().Trim().Length > 0)
        //            {
        //                if (_KeyCode == dgvr.Cells[KeyColumnName].Value.ToString())
        //                    _KeyValue = _KeyValue + double.Parse(dgvr.Cells[ValueColumnName].Value.ToString());
        //            }

        //            //_KeyCode = dgvr.Cells[KeyColumnName].Value.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return _KeyValue;
        //}


        private void IsAutoGenNo(string Type)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
