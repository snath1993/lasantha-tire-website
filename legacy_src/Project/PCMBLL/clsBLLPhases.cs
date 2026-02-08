using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using PCMDBL;
using PCMBeans;

namespace PCMBLL
{
    public class clsBLLPhases
    {
        clsDBLPhases objclsDBLPhases;
        DataSet objDataSet;
        clsBeansPhases objclsBeansPhases;

        public DataSet GetJobStatus_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetJobStatus_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetJobCategory_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetJobCategory_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetSubContractor_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetSubContractor_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetContractor_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetContractor_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetCustomer_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetCustomer_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetBillingMethods_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBillingMethods_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetSubcontractors_ByID(string _ID)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetSubcontractors_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }        

        public clsBeansPhases GetJob_ByID(string _ID)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetJob_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetJob_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetJob_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetJob_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetJob_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetSubPhases_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetSubPhases_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetSubPhases_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetSubPhases_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetPhases_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetPhases_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetPhases_AllActive_ForDropDown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetPhases_AllActive_ForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetCostTypes_AllActive()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetCostTypes_AllActive();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetLocations_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetLocations_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetLocations_AllActive_Dropdown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetLocations_AllActive_Dropdown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetLocations_AllActive_Dropdown_By_SubPhaseID(string _ID)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetLocations_AllActive_Dropdown_By_SubPhaseID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetLocations_AllActive_Dropdown_By_PhaseID(string _ID)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetLocations_AllActive_Dropdown_By_PhaseID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetCostTypes_All_Dropdown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetCostTypes_All_Dropdown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public void SaveCostTypes(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.SaveCostTypes(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCostTypes(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeleteCostTypes(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteLocations(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeleteLocations(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public clsBeansPhases GetCostTypes_ByID(string _ID)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetCostTypes_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public clsBeansPhases GetLocations_ByID(string _ID)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetLocations_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public void SaveLocations(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.SaveLocations(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SavePhases(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.SavePhases(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePhasesRates(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.UpdatePhasesRates(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateSubPhasesRates(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.UpdateSubPhasesRates(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveSubPhases(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.SaveSubPhases(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveJobs(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.SaveJobs(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public clsBeansPhases GetPhases_ByID(string _ID)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetPhases_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public void DeleteJob(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeleteJob(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public clsBeansPhases GetSubPhases_ByID(string _ID)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetSubPhases_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetSubPhases_ByPhaseID(string _ID)
        {
            
            //objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetSubPhases_ByPhaseID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetBOQs_ByID_Max(string _ID, int _RevNo)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetBOQs_ByID_Max(_ID, _RevNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetBOQs_ByID(string _ID)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOQs_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetBOMs_ByID(string _ID)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOMs_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetBOQs_ByID(string _ID, int _RevNo)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetBOQs_ByID(_ID, _RevNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public string SaveBOQs(clsBeansPhases objclsBeansPhases,bool Save)
        {
            string _ID = "";
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _ID = objclsDBLPhases.SaveBOQs(objclsBeansPhases,Save);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ID;
        }

        public DataSet GetBOQs_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOQs_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetBOMs_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOMs_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetBOQs_AllActive_Dropdown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOQs_AllActive_Dropdown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public void DeleteBOQs(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeleteBOQs(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteBOMs(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeleteBOMs(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetPhases_ByID_Location(string _ID, string _Location)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetPhases_ByID_Location( _ID,  _Location);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetSubPhases_ByID_Location(string _ID, string _Location)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetSubPhases_ByID_Location( _ID,  _Location);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetPhases_All_Dropdown_WithBreakdown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetPhases_All_Dropdown_WithBreakdown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetItems_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetItems_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetWarehouses_By_ItemID(string _ID)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetWarehouses_By_ItemID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetItem_By_ItemID(string _ID)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetItem_By_ItemID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetItemMaster_By_ItemID(string _ID)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetItemMaster_By_ItemID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetBOMs_ByID(string _ID, int _RevNo)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetBOMs_ByID(_ID, _RevNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public clsBeansPhases GetBOMs_ByID_Max(string _ID, int _RevNo)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetBOMs_ByID_Max(_ID, _RevNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetBOMs_AllActive_Dropdown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOMs_AllActive_Dropdown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetBOM_By_ItemID_PhaseID_SubPhaseID(string _ItemID, string _PhaseID, string _SubPhaseID)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOM_By_ItemID_PhaseID_SubPhaseID(_ItemID, _PhaseID, _SubPhaseID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetIssues_ByID(string _ID)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetIssues_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public clsBeansPhases GetReturns_ByID(string _ID)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetReturns_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetToBeIssuedQty(string _BOMNO)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetToBeIssuedQty(_BOMNO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetCustomer_ByID(string _ID)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetCustomer_ByID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetIssues_All_Dropdown()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetIssues_All_Dropdown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public string SaveBOMs(clsBeansPhases objclsBeansPhases)
        {
            string _ID = "";
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _ID = objclsDBLPhases.SaveBOMs(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ID;
        }

        public string SaveIssues(clsBeansPhases objclsBeansPhases,bool Save)
        {
            string _ID = "";
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _ID = objclsDBLPhases.SaveIssues(objclsBeansPhases, Save);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ID;
        }

        public string SaveReturns(clsBeansPhases objclsBeansPhases, bool Save)
        {
            string _ID = "";
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _ID = objclsDBLPhases.SaveReturns(objclsBeansPhases, Save);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ID;
        }

        public void DeleteIssues(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeleteIssues(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteReturns(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeleteReturns(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeletePhases(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeletePhases(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteSubPhases(clsBeansPhases objclsBeansPhases)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.DeleteSubPhases(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetBOQs_ByPhaseID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetBOQs_ByPhaseID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOQs_BySubPhaseID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetBOQs_BySubPhaseID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetItems_By_WH_ItemCode(string _WHID, string _ItemID)
        {

            DataSet _dts = new DataSet();// = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetItems_By_WH_ItemCode(_WHID, _ItemID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public string GetCodeforArrow(string Type, string CurrntCode, string Side)
        {
            objclsDBLPhases = new clsDBLPhases();
            string Code = "";
            try
            {
                Code = objclsDBLPhases.GetCodeforArrow(Type, CurrntCode, Side);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Code;
        }

        public DataSet GetIssues_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetIssues_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetReturns_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetReturns_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetJobs_AllActive_ByCustomerID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetJobs_AllActive_ByCustomerID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetCustomer_AllActive_BySiteID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetCustomer_AllActive_BySiteID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public void SaveTemp(clsBeansPhases objclsBeansPhases)
        {
            string _ID = "";
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.SaveTemp(objclsBeansPhases);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return _ID;
        }

        public DataSet GetTemp_All()
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetTemp_All();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetBOMs_BySiteID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetBOMs_BySiteID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOQs_BySiteID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetBOQs_BySiteID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetWereHouse_By_ID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetWereHouse_By_ID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetIssues_BySiteID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetIssues_BySiteID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetExpences_By_SiteID(string _ID)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.GetExpences_By_SiteID(_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet getIssuedetails_BySiteID_Phase_SubPhase(string _ID,string Phase,string SubPhase)
        {
            DataSet _dts = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                _dts = objclsDBLPhases.getIssuedetails_BySiteID_Phase_SubPhase(_ID, Phase,SubPhase);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOMBalance(string _BOMNO)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOMBalance(_BOMNO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetBOQBalance(string _BOQNO)
        {
            objDataSet = new DataSet();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objDataSet = objclsDBLPhases.GetBOQBalance(_BOQNO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public void Save_PeachtreeCompany(string _PTCompany)
        {
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsDBLPhases.Save_PeachtreeCompany(_PTCompany);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string LinkPeachTree()
        {
            string _CompName = string.Empty;
            try
            {
                objclsDBLPhases = new clsDBLPhases();

                _CompName = objclsDBLPhases.LinkPeachTree();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _CompName;
        }

        public void DIsconnectPeachTree()
        {
            try
            {
                objclsDBLPhases = new clsDBLPhases();

                objclsDBLPhases.DIsconnectPeachTree();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return _dataTable;
        }

        public clsBeansPhases GetBOMs_ByID_Max(string _ID, int _RevNo, string _PhaseID, string _SubPhaseID)
        {

            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            try
            {
                objclsBeansPhases = objclsDBLPhases.GetBOMs_ByID_Max(_ID, _RevNo, _PhaseID, _SubPhaseID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }
    }
}
