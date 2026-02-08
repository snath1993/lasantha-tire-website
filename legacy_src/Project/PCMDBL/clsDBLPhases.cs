using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DataAccess;
using PCMBeans;
using Interop.PeachwServer;

namespace PCMDBL
{
    public class clsDBLPhases
    {
        clsDataAccess objDataAccess;
        DataSet objDataSet;
        SqlParameter[] objParams;
        clsBeansPhases objclsBeansPhases;
        clsDBLPTImport objclsDBLPTImport;
        public Interop.PeachwServer.Application app;

        //tblVendorMaster_Select_By_ID]
        //@VendorID varchar(20)

        public clsBeansPhases GetSubcontractors_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("VendorID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataAccess.BeginTransaction();
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblVendorMaster_Select_By_ID", objParams);
                objDataAccess.CommitTransaction();

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_Subcontractors(dr);
                }
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetSubContractor_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblSubContractors_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetContractor_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblContractors_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }


        public DataSet GetJobStatus_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblJobStatus_SelectAll_Dropdown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetJobCategory_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblJobCategory_SelectAll_Dropdown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetJob_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblJobHeader_SelectAll", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public void SaveJobs(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[23];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.Code);
                objParams[1] = new SqlParameter("Description", _objclsBeansPhases.Description);
                objParams[2] = new SqlParameter("ActualStartDate", _objclsBeansPhases.ActualStartDate);
                objParams[3] = new SqlParameter("ActualEndDate", _objclsBeansPhases.ActualEndDate);
                objParams[4] = new SqlParameter("EstStartDate", _objclsBeansPhases.EstimateStartDate);
                objParams[5] = new SqlParameter("EstEndDate", _objclsBeansPhases.EstimateEndDate);
                objParams[6] = new SqlParameter("ActualAmount", _objclsBeansPhases.ActualAmt);
                objParams[7] = new SqlParameter("EstAmount", _objclsBeansPhases.EstimatedAmt);
                objParams[8] = new SqlParameter("Architecture", _objclsBeansPhases.Architecture);
                objParams[9] = new SqlParameter("Customer", _objclsBeansPhases.CustomerID);
                objParams[10] = new SqlParameter("Contractor", _objclsBeansPhases.Contractor);
                objParams[11] = new SqlParameter("ProjManager", _objclsBeansPhases.ProjectManager);
                objParams[12] = new SqlParameter("StructEngineer", _objclsBeansPhases.StructuralEngineer);
                objParams[13] = new SqlParameter("BillingMethod", _objclsBeansPhases.BillingMethodID);
                objParams[14] = new SqlParameter("RetainPersentage", _objclsBeansPhases.RetainingPersentage);
                objParams[15] = new SqlParameter("Inactive", _objclsBeansPhases.Inactive);
                objParams[16] = new SqlParameter("PaymentTerms", "0");
                objParams[17] = new SqlParameter("JobStatus", _objclsBeansPhases.StatusID);
                objParams[18] = new SqlParameter("Supervisor", _objclsBeansPhases.Supervisor);
                objParams[19] = new SqlParameter("Status", _objclsBeansPhases.CompletedPercentage);
                objParams[20] = new SqlParameter("Category", _objclsBeansPhases.Category);
                objParams[21] = new SqlParameter("Labor_Burden_Percent", _objclsBeansPhases.Labor_Burden_Percent);
                objParams[22] = new SqlParameter("UsePhases", _objclsBeansPhases.IsUsePhases);
                    
                

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblJobHeader_Save", objParams);

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.Code);
                objDataAccess.ExecuteSPReturnObject("tblJobDetails_SubContractors_Delete", objParams);

                foreach (DataRow dr in _objclsBeansPhases.Dtbl.Rows)
                {
                    objParams = new SqlParameter[6];
                    objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.Code);
                    objParams[1] = new SqlParameter("SubContractor", dr["ID"].ToString());
                    objParams[2] = new SqlParameter("Remarks", dr["Remarks"].ToString());
                    objParams[3] = new SqlParameter("Name", dr["Name"].ToString());
                    objParams[4] = new SqlParameter("Address", dr["Address"].ToString());
                    objParams[5] = new SqlParameter("Contact", dr["Contact"].ToString());

                    objDataAccess.ExecuteSPReturnObject("tblJobDetails_SubContractors_Save", objParams);
                }
                objclsDBLPTImport = new clsDBLPTImport();

                objclsDBLPTImport.CreateXmlToExportJob(_objclsBeansPhases.IsProjEndDate, _objclsBeansPhases.IsProjStartDate, _objclsBeansPhases.IsActEndDate,_objclsBeansPhases.Code, 
                    _objclsBeansPhases.Description, 
                    _objclsBeansPhases.Inactive, 
                    _objclsBeansPhases.Supervisor, 
                    _objclsBeansPhases.CustomerID, 
                    _objclsBeansPhases.EstimateStartDate, 
                    _objclsBeansPhases.EstimateEndDate,
                    _objclsBeansPhases.Category, 
                    _objclsBeansPhases.BillingMethodID, 
                    _objclsBeansPhases.RetainingPersentage, 
                    _objclsBeansPhases.StatusID,
                    _objclsBeansPhases.CompletedPercentage,
                    _objclsBeansPhases.Labor_Burden_Percent,
                    _objclsBeansPhases.ActualEndDate);

                objclsDBLPTImport.JobExport(_objclsBeansPhases.IsProjEndDate, _objclsBeansPhases.IsProjStartDate, _objclsBeansPhases.IsActEndDate);

                SaveDocumentList(_objclsBeansPhases, _objclsBeansPhases.Code, objDataAccess);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public DataSet GetBillingMethods_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblBillingMethods_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        

        public DataSet GetCustomer_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblCustomerMaster_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

       

        public DataSet GetJob_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblJobHeader_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetJob_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                //objDataAccess.BeginTransaction();
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblJobHeader_Select_By_ID", objParams);                

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_Jobs(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblJobDetails_SubContractors_Select_By_ID", objParams).Tables[0];
                objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
                //objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                //objDataAccess.RollBackTransaction();
                throw ex;
            }
            return objclsBeansPhases;
        }

        public void DeletePhases(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("PhaseID", _objclsBeansPhases.Code);

                objDataAccess = new clsDataAccess();

                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblPhases_Delete", objParams);
                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void DeleteSubPhases(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SubPhaseID", _objclsBeansPhases.Code);

                objDataAccess = new clsDataAccess();

                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblSubPhases_Delete", objParams);
                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public DataSet GetPhases_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblPhases_SelectAll", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetPhases_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblPhases_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetSubPhases_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblSubPhases_SelectAll", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetSubPhases_AllActive_ForDropDown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblSubPhases_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public void SaveSubPhases(clsBeansPhases _objclsBeansPhases)
        {
            try
            {                
                objParams = new SqlParameter[7];
                objParams[0] = new SqlParameter("PhaseID", _objclsBeansPhases.PhaseID);
                objParams[1] = new SqlParameter("SubPhaseID", _objclsBeansPhases.Code);
                objParams[2] = new SqlParameter("SubPhaseDesc", _objclsBeansPhases.Description);
                objParams[3] = new SqlParameter("Inactive", _objclsBeansPhases.Inactive);
                objParams[4] = new SqlParameter("Units", _objclsBeansPhases.Units);
                objParams[5] = new SqlParameter("Rate", _objclsBeansPhases.Rate);
                objParams[6] = new SqlParameter("CostType", _objclsBeansPhases.CostTypeID);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblSubPhases_Save", objParams);

                foreach (DataRow dr in _objclsBeansPhases.Dtbl.Rows)
                {
                    objParams = new SqlParameter[5];
                    objParams[0] = new SqlParameter("SubPhaseID", _objclsBeansPhases.Code);
                    objParams[1] = new SqlParameter("LocationCode", dr["LocCode"].ToString());
                    objParams[2] = new SqlParameter("Rate", dr["Rate"].ToString());
                    objParams[3] = new SqlParameter("Margine", dr["Margine"].ToString());
                    objParams[4] = new SqlParameter("Date", DateTime.Today.ToString("MM/dd/yyyy"));

                    objDataAccess.ExecuteSPReturnObject("tblSubPhasesDetails_Save", objParams);
                }

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }

        }

        public void UpdatePhasesRates(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[5];
                objParams[0] = new SqlParameter("Code", _objclsBeansPhases.Code);
                objParams[1] = new SqlParameter("IsApplyToAll", _objclsBeansPhases.IsApplyToAll);
                objParams[2] = new SqlParameter("Date", _objclsBeansPhases.Date);
                objParams[3] = new SqlParameter("Increment", _objclsBeansPhases.RateIncrease);
                objParams[4] = new SqlParameter("IsPhase", Boolean.TrueString);                

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblPriceDetails_Update", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void UpdateSubPhasesRates(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[5];
                objParams[0] = new SqlParameter("Code", _objclsBeansPhases.Code);
                objParams[1] = new SqlParameter("IsApplyToAll", _objclsBeansPhases.IsApplyToAll);
                objParams[2] = new SqlParameter("Date", _objclsBeansPhases.Date);
                objParams[3] = new SqlParameter("Increment", _objclsBeansPhases.RateIncrease);
                objParams[4] = new SqlParameter("IsPhase", Boolean.FalseString);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblPriceDetails_Update", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void SavePhases(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[7];
                objParams[0] = new SqlParameter("PhaseID", _objclsBeansPhases.Code);
                objParams[1] = new SqlParameter("PhaseDesc", _objclsBeansPhases.Description);
                objParams[2] = new SqlParameter("Inactive", _objclsBeansPhases.Inactive);
                objParams[3] = new SqlParameter("Units", _objclsBeansPhases.Units);
                objParams[4] = new SqlParameter("Rate", _objclsBeansPhases.Rate);
                objParams[5] = new SqlParameter("UseCostCode", _objclsBeansPhases.UseCostCode);
                objParams[6] = new SqlParameter("CostType", _objclsBeansPhases.CostTypeID);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblPhases_Save", objParams);

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("PhaseID", _objclsBeansPhases.Code);
                objDataAccess.ExecuteSPReturnObject("tblPhasesDetails_Delete", objParams);

                foreach (DataRow dr in _objclsBeansPhases.Dtbl.Rows)
                {
                    objParams = new SqlParameter[5];
                    objParams[0] = new SqlParameter("PhaseID", _objclsBeansPhases.Code);
                    objParams[1] = new SqlParameter("LocationCode", dr["LocCode"].ToString());
                    objParams[2] = new SqlParameter("Rate", dr["Rate"].ToString());
                    objParams[3] = new SqlParameter("Margine", dr["Margine"].ToString());
                    objParams[4] = new SqlParameter("Date", DateTime.Today.ToString("MM/dd/yyyy"));

                    objDataAccess.ExecuteSPReturnObject("tblPhasesDetails_Save", objParams);
                }
                clsDBLPTImport objclsDBLPTImport = new clsDBLPTImport();
                objclsDBLPTImport.CreateXmlToPhases(_objclsBeansPhases.Code, _objclsBeansPhases.UseCostCode);
                objclsDBLPTImport.ImportPhases();

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }

        }        

        public void SaveCostTypes(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[3];
                objParams[0] = new SqlParameter("CostTypeID", _objclsBeansPhases.CostTypeID);
                objParams[1] = new SqlParameter("CostTypeDesc", _objclsBeansPhases.CostTypeName);
                objParams[2] = new SqlParameter("Inactive", _objclsBeansPhases.Inactive);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblCostTypes_Save", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void DeleteCostTypes(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("CostTypeID", _objclsBeansPhases.CostTypeID);

                objDataAccess = new clsDataAccess();

                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblCostTypes_Delete", objParams);
                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void DeleteLocations(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("LocationID", _objclsBeansPhases.Code);

                objDataAccess = new clsDataAccess();

                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblLocations_Delete", objParams);
                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void DeleteJob(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.Code);

                objDataAccess = new clsDataAccess();

                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblJobDetails_SubContractors_Delete", objParams);
                objDataAccess.ExecuteSPReturnObject("tblCostTypes_Delete", objParams);
                objDataAccess.CommitTransaction();

                DeleteDocumentList(_objclsBeansPhases.Code, objDataAccess);
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public DataSet GetCostTypes_All_Dropdown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblCostTypes_SelectAll_Dropdown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        

        public DataSet GetCostTypes_AllActive()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblCostTypes_SelectAll_Active", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetCostTypes_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("CostTypeID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();
                
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblCostTypes_Select_By_ID", objParams);                

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_CostTypes(dr);
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return objclsBeansPhases;
        }

        public clsBeansPhases GetCustomer_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("CutomerID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblCustomerMaster_Select_By_ID", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_Customers(dr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        private clsBeansPhases dataRead_Customers(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (!string.IsNullOrEmpty(dr["CutomerID"].ToString()))
                    objclsBeansPhases.Code = dr["CutomerID"].ToString();
                if (!string.IsNullOrEmpty(dr["CustomerName"].ToString()))
                    objclsBeansPhases.Description = dr["CustomerName"].ToString();
                //if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                //    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetSubPhases_ByPhaseID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("PhaseID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblSubPhases_SelectAll_By_PhaseID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetJobs_AllActive_ByCustomerID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("Customer", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblJobHeader_SelectAll_Active_ForDropDown_By_CustomerID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetCustomer_AllActive_BySiteID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblCustomerMaster_SelectAll_Active_By_SiteID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        private clsBeansPhases dataRead_CostTypes(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (!string.IsNullOrEmpty(dr["CostTypeID"].ToString()))
                    objclsBeansPhases.CostTypeID = dr["CostTypeID"].ToString();
                if (!string.IsNullOrEmpty(dr["CostTypeDesc"].ToString()))
                    objclsBeansPhases.CostTypeName = dr["CostTypeDesc"].ToString();
                if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        private clsBeansPhases dataRead_Subcontractors(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                //, , , , , , Inactive

                if (!string.IsNullOrEmpty(dr["VendorID"].ToString()))
                    objclsBeansPhases.Code = dr["VendorID"].ToString();

                if (!string.IsNullOrEmpty(dr["VendorName"].ToString()))
                    objclsBeansPhases.Description = dr["VendorName"].ToString();

                if (!string.IsNullOrEmpty(dr["VContact"].ToString()))
                    objclsBeansPhases.ContactNo = dr["VContact"].ToString();

                if (!string.IsNullOrEmpty(dr["VAddress1"].ToString()))
                    objclsBeansPhases.Address1 = dr["VAddress1"].ToString();

                if (!string.IsNullOrEmpty(dr["VAddress2"].ToString()))
                    objclsBeansPhases.Address2 = dr["VAddress2"].ToString();

                if (!string.IsNullOrEmpty(dr["Type"].ToString()))
                    objclsBeansPhases.Type = dr["Type"].ToString();

                if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        private clsBeansPhases dataRead_Phases(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (!string.IsNullOrEmpty(dr["PhaseID"].ToString()))
                    objclsBeansPhases.Code = dr["PhaseID"].ToString();
                if (!string.IsNullOrEmpty(dr["PhaseDesc"].ToString()))
                    objclsBeansPhases.Description = dr["PhaseDesc"].ToString();
                if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());

                if (!string.IsNullOrEmpty(dr["Units"].ToString()))
                    objclsBeansPhases.Units = dr["Units"].ToString();
                if (!string.IsNullOrEmpty(dr["Rate"].ToString()))
                    objclsBeansPhases.Rate = double.Parse(dr["Rate"].ToString());
                if (!string.IsNullOrEmpty(dr["UseCostCode"].ToString()))
                    objclsBeansPhases.UseCostCode = bool.Parse(dr["UseCostCode"].ToString());
                if (!string.IsNullOrEmpty(dr["CostType"].ToString()))
                    objclsBeansPhases.CostTypeID = dr["CostType"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        private clsBeansPhases dataRead_SubPhases(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (!string.IsNullOrEmpty(dr["PhaseID"].ToString()))
                    objclsBeansPhases.PhaseID = dr["PhaseID"].ToString();
                if (!string.IsNullOrEmpty(dr["SubPhaseID"].ToString()))
                    objclsBeansPhases.Code = dr["SubPhaseID"].ToString();
                if (!string.IsNullOrEmpty(dr["SubPhaseDesc"].ToString()))
                    objclsBeansPhases.Description = dr["SubPhaseDesc"].ToString();
                if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());

                if (!string.IsNullOrEmpty(dr["Units"].ToString()))
                    objclsBeansPhases.Units = dr["Units"].ToString();
                if (!string.IsNullOrEmpty(dr["Rate"].ToString()))
                    objclsBeansPhases.Rate = double.Parse(dr["Rate"].ToString());
                //if (!string.IsNullOrEmpty(dr["UseCostCode"].ToString()))
                //    objclsBeansPhases.UseCostCode = bool.Parse(dr["UseCostCode"].ToString());
                if (!string.IsNullOrEmpty(dr["CostType"].ToString()))
                    objclsBeansPhases.CostTypeID = dr["CostType"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        private clsBeansPhases dataRead_Jobs(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (!string.IsNullOrEmpty(dr["JobID"].ToString()))
                    objclsBeansPhases.Code = dr["JobID"].ToString();

                if (!string.IsNullOrEmpty(dr["JobDescription"].ToString()))
                    objclsBeansPhases.Description = dr["JobDescription"].ToString();

                if (!string.IsNullOrEmpty(dr["ActualStartDate"].ToString()))
                    objclsBeansPhases.ActualStartDate = DateTime.Parse(dr["ActualStartDate"].ToString());

                if (!string.IsNullOrEmpty(dr["ActualEndDate"].ToString()))
                    objclsBeansPhases.ActualEndDate = DateTime.Parse(dr["ActualEndDate"].ToString());

                if (!string.IsNullOrEmpty(dr["EstStartDate"].ToString()))
                    objclsBeansPhases.EstimateStartDate = DateTime.Parse(dr["EstStartDate"].ToString());

                if (!string.IsNullOrEmpty(dr["EstEndDate"].ToString()))
                    objclsBeansPhases.EstimateEndDate = DateTime.Parse(dr["EstEndDate"].ToString());

                if (!string.IsNullOrEmpty(dr["ActualAmount"].ToString()))
                    objclsBeansPhases.ActualAmt = double.Parse(dr["ActualAmount"].ToString());

                if (!string.IsNullOrEmpty(dr["EstAmount"].ToString()))
                    objclsBeansPhases.EstimatedAmt = double.Parse(dr["EstAmount"].ToString());

                if (!string.IsNullOrEmpty(dr["Architecture"].ToString()))
                    objclsBeansPhases.Architecture = dr["Architecture"].ToString();

                if (!string.IsNullOrEmpty(dr["CustomerID"].ToString()))
                    objclsBeansPhases.CustomerID = dr["CustomerID"].ToString();

                if (!string.IsNullOrEmpty(dr["CustomerName"].ToString()))
                    objclsBeansPhases.CustomerName = dr["CustomerName"].ToString();

                if (!string.IsNullOrEmpty(dr["Contractor"].ToString()))
                    objclsBeansPhases.Contractor = dr["Contractor"].ToString();

                if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());

                if (!string.IsNullOrEmpty(dr["ProjManager"].ToString()))
                    objclsBeansPhases.ProjectManager = dr["ProjManager"].ToString();

                if (!string.IsNullOrEmpty(dr["StructEngineer"].ToString()))
                    objclsBeansPhases.StructuralEngineer = dr["StructEngineer"].ToString();

                if (!string.IsNullOrEmpty(dr["BillingMethod"].ToString()))
                    objclsBeansPhases.BillingMethodID = dr["BillingMethod"].ToString();

                if (!string.IsNullOrEmpty(dr["RetainPersentage"].ToString()))
                    objclsBeansPhases.RetainingPersentage = double.Parse(dr["RetainPersentage"].ToString());

                if (!string.IsNullOrEmpty(dr["UsePhases"].ToString()))
                    objclsBeansPhases.UseCostCode = bool.Parse(dr["UsePhases"].ToString());

                if (!string.IsNullOrEmpty(dr["Supervisor"].ToString()))
                    objclsBeansPhases.Supervisor = dr["Supervisor"].ToString();

                if (!string.IsNullOrEmpty(dr["JobStatus"].ToString()))
                    objclsBeansPhases.StatusID = dr["JobStatus"].ToString();

                if (!string.IsNullOrEmpty(dr["Category"].ToString()))
                    objclsBeansPhases.Category = dr["Category"].ToString();

                if (!string.IsNullOrEmpty(dr["Labor_Burden_Percent"].ToString()))
                    objclsBeansPhases.Labor_Burden_Percent = double.Parse(dr["Labor_Burden_Percent"].ToString());

                if (!string.IsNullOrEmpty(dr["Status"].ToString()))
                    objclsBeansPhases.CompletedPercentage = double.Parse(dr["Status"].ToString());

                if (!string.IsNullOrEmpty(dr["UsePhases"].ToString()))
                    objclsBeansPhases.IsUsePhases = bool.Parse(dr["UsePhases"].ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        

        public clsBeansPhases GetPhases_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("PhaseID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblPhases_Select_By_ID", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_Phases(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblPhasesDetails_Select_By_ID", objParams).Tables[0];
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public clsBeansPhases GetSubPhases_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SubPhaseID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblSubPhases_Select_By_ID", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_SubPhases(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblSubPhasesDetails_Select_By_ID", objParams).Tables[0];

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }


        

        

        public void SaveLocations(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[3];
                objParams[0] = new SqlParameter("LocationID", _objclsBeansPhases.Code);
                objParams[1] = new SqlParameter("LocationDesc", _objclsBeansPhases.Description);
                objParams[2] = new SqlParameter("Inactive", _objclsBeansPhases.Inactive);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblLocations_Save", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public DataSet GetLocations_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblLocations_SelectAll", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetLocations_AllActive_Dropdown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblLocations_SelectAll_Active_Dropdown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetLocations_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("LocationID", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataAccess.BeginTransaction();
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblLocations_Select_By_ID", objParams);
                objDataAccess.CommitTransaction();

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_Locations(dr);
                }
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetLocations_AllActive_Dropdown_By_SubPhaseID(string _ID)
        {
            objDataSet = new DataSet();
            objDataAccess = new clsDataAccess();

            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SubPhaseID", _ID);              

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblLocations_Select_By_SubPhaseID_Active_Dropdown", objParams);
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
            objDataAccess = new clsDataAccess();

            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("PhaseID", _ID);

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblLocations_Select_By_PhaseID_Active_Dropdown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        private clsBeansPhases dataRead_Locations(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (!string.IsNullOrEmpty(dr["LocationID"].ToString()))
                    objclsBeansPhases.Code = dr["LocationID"].ToString();
                if (!string.IsNullOrEmpty(dr["LocationDesc"].ToString()))
                    objclsBeansPhases.Description = dr["LocationDesc"].ToString();
                if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public string SaveBOQs(clsBeansPhases _objclsBeansPhases,bool Save)
        {
            string _BOQID = "";
            try
            {            
                objParams = new SqlParameter[14];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.SiteID);
                objParams[1] = new SqlParameter("BOQID", _objclsBeansPhases.BOQID);
                objParams[2] = new SqlParameter("RevNo", _objclsBeansPhases.RevisionNo);
                objParams[3] = new SqlParameter("EstimateAmount", _objclsBeansPhases.EstimatedAmt);
                objParams[4] = new SqlParameter("ActualAmount", _objclsBeansPhases.ActualAmt);
                objParams[5] = new SqlParameter("Description", _objclsBeansPhases.Description);
                objParams[6] = new SqlParameter("StartDate", _objclsBeansPhases.EstimateStartDate.ToString("MM/dd/yyyy"));
                objParams[7] = new SqlParameter("EndDate", _objclsBeansPhases.EstimateEndDate.ToString("MM/dd/yyyy"));
                objParams[8] = new SqlParameter("CreatedDate", _objclsBeansPhases.Date.ToString("MM/dd/yyyy"));                
                objParams[9] = new SqlParameter("Inactive", _objclsBeansPhases.Inactive);
                objParams[10] = new SqlParameter("Enteruser", clsBeansPhases.USERNAME);
                objParams[11] = new SqlParameter("EnterDate", clsBeansPhases.LOGINDATE);
                objParams[12] = new SqlParameter("UsePhases", _objclsBeansPhases.IsUsePhases);
                objParams[13] = new SqlParameter("IsPosted", _objclsBeansPhases.IsPosted);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                _BOQID = objDataAccess.ExecuteSPReturnObject("tblTransBOQHeader_Insert", objParams).ToString();

                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("BOQID", _BOQID);
                objParams[1] = new SqlParameter("RevNo", _objclsBeansPhases.RevisionNo);
                objDataAccess.ExecuteSPReturnInteger("tblTransBOQDetails_Delete", objParams);

                foreach (DataRow dr in _objclsBeansPhases.Dtbl.Rows)
                {                   
                    objParams = new SqlParameter[10];
                    objParams[0] = new SqlParameter("BOQID", _BOQID);
                    objParams[1] = new SqlParameter("RevNo", _objclsBeansPhases.RevisionNo);
                    objParams[2] = new SqlParameter("PhaseID", dr["PhaseID"].ToString());
                    objParams[3] = new SqlParameter("SubPhaseID", dr["SubPhaseID"].ToString());
                    objParams[4] = new SqlParameter("Activity", dr["Activity"].ToString());
                    objParams[5] = new SqlParameter("Location", dr["LocationID"].ToString());
                    objParams[6] = new SqlParameter("Rate", dr["Rate"].ToString());
                    objParams[7] = new SqlParameter("Amount", dr["Amount"].ToString());
                    objParams[8] = new SqlParameter("Units", dr["Units"].ToString());
                    objParams[9] = new SqlParameter("Qty", dr["Qty"].ToString());                    

                    objDataAccess.ExecuteSPReturnInteger("tblTransBOQDetails_Insert", objParams);
                }

                SaveDocumentList(_objclsBeansPhases, _BOQID, objDataAccess);

                if (Save)
                {
                    clsDBLPTImport objclsDBLPTImport = new clsDBLPTImport();
                    //objclsDBLPTImport.CreateXmlToDeleteJob(_objclsBeansPhases.SiteID);
                    //objclsDBLPTImport.JobDeleteExport();


                    objclsDBLPTImport.CreateXmlToExportBOQ(
                        _objclsBeansPhases.SiteID,
                        _objclsBeansPhases.SiteDescription,
                        _objclsBeansPhases.UseCostCode,
                        _objclsBeansPhases.Inactive,
                        _objclsBeansPhases.Supervisor,
                        _objclsBeansPhases.CustomerID,
                        _objclsBeansPhases.EstimateStartDate,
                        _objclsBeansPhases.EstimateEndDate,
                        _objclsBeansPhases.Category,
                        _objclsBeansPhases.BillingMethodID,
                        _objclsBeansPhases.RetainingPersentage,
                        _objclsBeansPhases.DtblPT);
                    //sda
                    objclsDBLPTImport.BOQExport();
                }

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.SiteID);
                objDataAccess.ExecuteSPReturnInteger("tblJobHeader_Update", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
            return _BOQID;
        }

        public void SaveDocumentList(clsBeansPhases _objclsBeansPhases, string _Code, clsDataAccess objDataAccess)
        {
            try
            {
                if (_objclsBeansPhases.DtblDocList != null)
                {
                    foreach (DataRow dr in _objclsBeansPhases.DtblDocList.Rows)
                    {
                        objParams = new SqlParameter[6];
                        objParams[0] = new SqlParameter("Code", _Code);
                        objParams[1] = new SqlParameter("Name", dr["Name"].ToString());
                        objParams[2] = new SqlParameter("Path", dr["Path"].ToString());
                        System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
                        dateInfo.ShortDatePattern = "dd/MM/yyyy";
                        DateTime validDate = Convert.ToDateTime(dr["Modified"].ToString(), dateInfo);
                        objParams[3] = new SqlParameter("Modified", validDate);
                        objParams[4] = new SqlParameter("Size", dr["Size"].ToString());
                        objParams[5] = new SqlParameter("Date", DateTime.Today);
                        objDataAccess.ExecuteSPReturnInteger("tblDocumentList_Insert", objParams);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void DeleteDocumentList(string _Code, clsDataAccess objDataAccess)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("Code", _Code);
                
                objDataAccess.ExecuteSPReturnInteger("tblDocumentList_Delete", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public void DeleteBOQs(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[2];               
                objParams[0] = new SqlParameter("BOQID", _objclsBeansPhases.BOQID);
                objParams[1] = new SqlParameter("RevNo", _objclsBeansPhases.RevisionNo);               

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblTransBOQHeader_Delete", objParams);                
                objDataAccess.ExecuteSPReturnObject("tblTransBOQDetails_Delete", objParams);

                DeleteDocumentList(_objclsBeansPhases.BOQID, objDataAccess);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public DataSet GetBOQs_AllActive_Dropdown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOQHeader_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetDocumentList(string _Code)
        {
            DataSet _objDataset = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("Code", _Code);

                _objDataset=objDataAccess.ExecuteSPReturnDataset("tblDocumentList_Select_ByCode", objParams);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return _objDataset;
        }

        public clsBeansPhases GetBOQs_ByID(string _ID,int _RevNo)
        {
            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("BOQID", _ID);
                objParams[1] = new SqlParameter("RevNo", _RevNo);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOQHeader_Select_By_ID", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_BOQs(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransBOQDetails_Select_By_ID", objParams).Tables[0];
                objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public clsBeansPhases GetBOQs_ByID_Max(string _ID, int _RevNo)
        {
            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("BOQID", _ID);
                objParams[1] = new SqlParameter("RevNo", _RevNo);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOQHeader_Select_By_ID_Max", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_BOQs(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransBOQDetails_Select_By_ID_Max", objParams).Tables[0];
                objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetBOQs_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("BOQID", _ID);
                //objParams[1] = new SqlParameter("RevNo", _RevNo);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOQHeader_Select_By_ID_ForDropDown", objParams);

                //foreach (DataRow dr in objDataSet.Tables[0].Rows)
                //{
                //    objclsBeansPhases = dataRead_BOQs(dr);
                //}
                //objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransBOQDetails_Select_By_ID_Max", objParams).Tables[0];
                //objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        private clsBeansPhases dataRead_BOQs(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                //ID, SiteID, , , , , , , , , Inactive, EnterUser, EnterDate, EditUser, EditDate
                if (!string.IsNullOrEmpty(dr["SiteID"].ToString()))
                    objclsBeansPhases.SiteID = dr["SiteID"].ToString();

                if (!string.IsNullOrEmpty(dr["BOQID"].ToString()))
                    objclsBeansPhases.BOQID = dr["BOQID"].ToString();

                if (!string.IsNullOrEmpty(dr["RevNo"].ToString()))
                    objclsBeansPhases.RevisionNo = int.Parse(dr["RevNo"].ToString());

                if (!string.IsNullOrEmpty(dr["EstimateAmount"].ToString()))
                    objclsBeansPhases.EstimatedAmt = double.Parse(dr["EstimateAmount"].ToString());

                if (!string.IsNullOrEmpty(dr["ActualAmount"].ToString()))
                    objclsBeansPhases.ActualAmt = double.Parse(dr["ActualAmount"].ToString());

                if (!string.IsNullOrEmpty(dr["Description"].ToString()))
                    objclsBeansPhases.Description = dr["Description"].ToString();

                if (!string.IsNullOrEmpty(dr["StartDate"].ToString()))
                    objclsBeansPhases.EstimateStartDate = DateTime.Parse(dr["StartDate"].ToString());

                if (!string.IsNullOrEmpty(dr["EndDate"].ToString()))
                    objclsBeansPhases.EstimateEndDate = DateTime.Parse(dr["EndDate"].ToString());

                if (!string.IsNullOrEmpty(dr["CreatedDate"].ToString()))
                    objclsBeansPhases.Date = DateTime.Parse(dr["CreatedDate"].ToString());              

                if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());

                if (!string.IsNullOrEmpty(dr["IsPosted"].ToString()))
                    objclsBeansPhases.IsPosted = bool.Parse(dr["IsPosted"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetBOQs_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOQHeader_SelectAll", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetBOMs_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOMHeader_SelectAll", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetPhases_ByID_Location(string _ID, string _Location)
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            objclsBeansPhases = new clsBeansPhases();

            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("PhaseID", _ID);
                objParams[1] = new SqlParameter("LocationCode", _Location);                

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblPhasesDetails_Select_By_ID_Location", objParams);                                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetSubPhases_ByID_Location(string _ID, string _Location)
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            objclsBeansPhases = new clsBeansPhases();

            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("SubPhaseID", _ID);
                objParams[1] = new SqlParameter("LocationCode", _Location);

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblSubPhasesDetails_Select_By_ID_Location", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetPhases_All_Dropdown_WithBreakdown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblPhases_SelectAll_Active_WBD_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetItems_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            objclsBeansPhases = new clsBeansPhases();

            try
            {
                objParams = new SqlParameter[0];             

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblItemWhse_select_All_Items", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }


        public DataSet GetWarehouses_By_ItemID(string _ID)
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            objclsBeansPhases = new clsBeansPhases();

            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("ItemId", _ID);

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblItemWhse_select_All_Warehouse_By_ItemID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetItem_By_ItemID(string _ID)
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            objclsBeansPhases = new clsBeansPhases();

            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("ItemId", _ID);

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblItemWhse_select_By_ItemID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetItemMaster_By_ItemID(string _ID)
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            objclsBeansPhases = new clsBeansPhases();

            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("ItemId", _ID);

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblItemMaster_select_By_ItemID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }



        //BOM
        public string SaveBOMs(clsBeansPhases _objclsBeansPhases)
        {
            string _BOMID = "";
            try
            {
                objParams = new SqlParameter[11];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.SiteID);
                objParams[1] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                objParams[2] = new SqlParameter("RevNo", _objclsBeansPhases.RevisionNo);
                objParams[3] = new SqlParameter("EstimatedAmount", _objclsBeansPhases.EstimatedAmt);
                objParams[4] = new SqlParameter("ActualAmount", _objclsBeansPhases.ActualAmt);
                objParams[5] = new SqlParameter("Description", _objclsBeansPhases.Description);
                objParams[6] = new SqlParameter("CreatedDate", _objclsBeansPhases.Date);
                objParams[7] = new SqlParameter("Inactive", _objclsBeansPhases.Inactive);
                objParams[8] = new SqlParameter("Enteruser", clsBeansPhases.USERNAME);
                objParams[9] = new SqlParameter("EnterDate", clsBeansPhases.LOGINDATE);
                objParams[10] = new SqlParameter("BOQNO", _objclsBeansPhases.BOQID);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                _BOMID = objDataAccess.ExecuteSPReturnObject("tblTransBOMHeader_Insert", objParams).ToString();

                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("BOMID", _BOMID);
                objParams[1] = new SqlParameter("RevNo", _objclsBeansPhases.RevisionNo);
                objDataAccess.ExecuteSPReturnInteger("tblTransBOMDetails_Delete", objParams);

                foreach (DataRow dr in _objclsBeansPhases.Dtbl.Rows)
                {
                    objParams = new SqlParameter[11];
                    objParams[0] = new SqlParameter("BOMID", _BOMID);
                    objParams[1] = new SqlParameter("RevNo", _objclsBeansPhases.RevisionNo);
                    objParams[2] = new SqlParameter("PhaseID", dr["PhaseID"].ToString());
                    objParams[3] = new SqlParameter("SubPhaseID", dr["SubPhaseID"].ToString());
                    objParams[4] = new SqlParameter("Activity", dr["Activity"].ToString());
                    objParams[5] = new SqlParameter("WareHouseID", dr["LocationID"].ToString());
                    objParams[6] = new SqlParameter("Rate", dr["Rate"].ToString());
                    objParams[7] = new SqlParameter("Amount", dr["Amount"].ToString());
                    objParams[8] = new SqlParameter("Units", dr["Units"].ToString());
                    objParams[9] = new SqlParameter("Qty", dr["Qty"].ToString());
                    objParams[10] = new SqlParameter("ItemID", dr["Item"].ToString());

                    objDataAccess.ExecuteSPReturnInteger("tblTransBOMDetails_Insert", objParams);
                }

                SaveDocumentList(_objclsBeansPhases, _BOMID, objDataAccess);

                //clsDBLPTImport objclsDBLPTImport = new clsDBLPTImport();
                //objclsDBLPTImport.CreateXmlToExportBOM(_objclsBeansPhases.SiteID, _objclsBeansPhases.Description, _objclsBeansPhases.UseCostCode, _objclsBeansPhases.Inactive, _objclsBeansPhases.Supervisor, _objclsBeansPhases.CustomerID,
                //    _objclsBeansPhases.StartDate, _objclsBeansPhases.EndDate, _objclsBeansPhases.Category, _objclsBeansPhases.BillingMethodID, _objclsBeansPhases.RetainingPersentage, _objclsBeansPhases.Dtbl);

                //objclsDBLPTImport.BOMExport();
                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
            return _BOMID;
        }       

        public void DeleteBOMs(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                objParams[1] = new SqlParameter("RevNo", _objclsBeansPhases.RevisionNo);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblTransBOMHeader_Delete", objParams);
                objDataAccess.ExecuteSPReturnObject("tblTransBOMDetails_Delete", objParams);

                DeleteDocumentList(_objclsBeansPhases.BOMID, objDataAccess);
            	
                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public DataSet GetBOMs_AllActive_Dropdown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOMHeader_SelectAll_Active_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetBOMs_ByID(string _ID, int _RevNo)
        {
            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("BOMID", _ID);
                objParams[1] = new SqlParameter("RevNo", _RevNo);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOMHeader_Select_By_ID", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_BOMs(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransBOMDetails_Select_By_ID", objParams).Tables[0];

                objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetBOMs_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("BOMID", _ID);
                //objParams[1] = new SqlParameter("RevNo", _RevNo);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOMHeader_Select_By_ID_ForDropDown", objParams);

                //foreach (DataRow dr in objDataSet.Tables[0].Rows)
                //{
                //    objclsBeansPhases = dataRead_BOMs(dr);
                //}
                //objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransBOMDetails_Select_By_ID", objParams).Tables[0];

                //objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public clsBeansPhases GetBOMs_ByID_Max(string _ID, int _RevNo)
        {
            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("BOMID", _ID);
                objParams[1] = new SqlParameter("RevNo", _RevNo);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransBOMHeader_Select_By_ID_Max", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_BOMs(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransBOMDetails_Select_By_ID_Max", objParams).Tables[0];

                objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        private clsBeansPhases dataRead_BOMs(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                //ID, SiteID, , , , , , , , , Inactive, EnterUser, EnterDate, EditUser, EditDate

                if (!string.IsNullOrEmpty(dr["BOMID"].ToString()))
                    objclsBeansPhases.BOMID = dr["BOMID"].ToString();

                if (!string.IsNullOrEmpty(dr["SiteID"].ToString()))
                    objclsBeansPhases.SiteID = dr["SiteID"].ToString();

                if (!string.IsNullOrEmpty(dr["BOQNo"].ToString()))
                    objclsBeansPhases.BOQID = dr["BOQNo"].ToString();

                if (!string.IsNullOrEmpty(dr["RevNo"].ToString()))
                    objclsBeansPhases.RevisionNo = int.Parse(dr["RevNo"].ToString());

                if (!string.IsNullOrEmpty(dr["EstimatedAmount"].ToString()))
                    objclsBeansPhases.EstimatedAmt = double.Parse(dr["EstimatedAmount"].ToString());

                if (!string.IsNullOrEmpty(dr["ActualAmount"].ToString()))
                    objclsBeansPhases.ActualAmt = double.Parse(dr["ActualAmount"].ToString());

                if (!string.IsNullOrEmpty(dr["Description"].ToString()))
                    objclsBeansPhases.Description = dr["Description"].ToString();               

                if (!string.IsNullOrEmpty(dr["CreatedDate"].ToString()))
                    objclsBeansPhases.Date = DateTime.Parse(dr["CreatedDate"].ToString());

                if (!string.IsNullOrEmpty(dr["Inactive"].ToString()))
                    objclsBeansPhases.Inactive = bool.Parse(dr["Inactive"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        private clsBeansPhases dataRead_Returns(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (!string.IsNullOrEmpty(dr["ReturnNo"].ToString()))
                    objclsBeansPhases.ReturnNo = dr["ReturnNo"].ToString();

                if (!string.IsNullOrEmpty(dr["BOMID"].ToString()))
                    objclsBeansPhases.BOMID = dr["BOMID"].ToString();

                if (!string.IsNullOrEmpty(dr["SiteID"].ToString()))
                    objclsBeansPhases.SiteID = dr["SiteID"].ToString();

                if (!string.IsNullOrEmpty(dr["IssueNo"].ToString()))
                    objclsBeansPhases.IssueNo = dr["IssueNo"].ToString();

                if (!string.IsNullOrEmpty(dr["TotalAmount"].ToString()))
                    objclsBeansPhases.ActualAmt = double.Parse(dr["TotalAmount"].ToString());

                if (!string.IsNullOrEmpty(dr["Description"].ToString()))
                    objclsBeansPhases.Description = dr["Description"].ToString();

                if (!string.IsNullOrEmpty(dr["Date"].ToString()))
                    objclsBeansPhases.Date = DateTime.Parse(dr["Date"].ToString());

                if (!string.IsNullOrEmpty(dr["EnterUser"].ToString()))
                    objclsBeansPhases.EnterUser = dr["EnterUser"].ToString();

                if (!string.IsNullOrEmpty(dr["EnterDate"].ToString()))
                    objclsBeansPhases.EnterDate = DateTime.Parse(dr["EnterDate"].ToString());

                if (!string.IsNullOrEmpty(dr["EditUser"].ToString()))
                    objclsBeansPhases.EditUser = dr["EditUser"].ToString();

                if (!string.IsNullOrEmpty(dr["EditDate"].ToString()))
                    objclsBeansPhases.EditDtae = DateTime.Parse(dr["EditDate"].ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        private clsBeansPhases dataRead_Issues(DataRow dr)
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (!string.IsNullOrEmpty(dr["BOMID"].ToString()))
                    objclsBeansPhases.BOMID = dr["BOMID"].ToString();

                if (!string.IsNullOrEmpty(dr["SiteID"].ToString()))
                    objclsBeansPhases.SiteID = dr["SiteID"].ToString();

                if (!string.IsNullOrEmpty(dr["IssueNo"].ToString()))
                    objclsBeansPhases.IssueNo = dr["IssueNo"].ToString();

                if (!string.IsNullOrEmpty(dr["TotalAmount"].ToString()))
                    objclsBeansPhases.ActualAmt = double.Parse(dr["TotalAmount"].ToString());               

                if (!string.IsNullOrEmpty(dr["Description"].ToString()))
                    objclsBeansPhases.Description = dr["Description"].ToString();

                 if (!string.IsNullOrEmpty(dr["Date"].ToString()))
                    objclsBeansPhases.Date = DateTime.Parse(dr["Date"].ToString());

                 if (!string.IsNullOrEmpty(dr["EnterUser"].ToString()))
                    objclsBeansPhases.EnterUser = dr["EnterUser"].ToString();

                if (!string.IsNullOrEmpty(dr["EnterDate"].ToString()))
                    objclsBeansPhases.EnterDate = DateTime.Parse(dr["EnterDate"].ToString());

                 if (!string.IsNullOrEmpty(dr["EditUser"].ToString()))
                    objclsBeansPhases.EditUser = dr["EditUser"].ToString();

                if (!string.IsNullOrEmpty(dr["EditDate"].ToString()))
                    objclsBeansPhases.EditDtae = DateTime.Parse(dr["EditDate"].ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public clsBeansPhases GetIssues_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("IssueNo", _ID);                

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransSiteIssuesHeader_Select_By_ID", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_Issues(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransSiteIssuesDetails_Select_By_ID", objParams).Tables[0];

                objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public clsBeansPhases GetReturns_ByID(string _ID)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("ReturnNo", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransSiteReturnsHeader_Select_By_ID", objParams);

                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    objclsBeansPhases = dataRead_Returns(dr);
                }
                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransSiteReturnDetails_Select_By_ID", objParams).Tables[0];

                objclsBeansPhases.DtblDocList = GetDocumentList(_ID).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }

        public DataSet GetIssues_All_Dropdown()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransSiteIssuesHeader_SelectAll_ForDropDown", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public string SaveIssues(clsBeansPhases _objclsBeansPhases,bool Save)
        {
            string _IssueID = "";
            try
            {
                DataSet _obj = new DataSet();              
                
                string SalesGLAccount = "";
                string ARAccount = "";

                clsDBLPTImport objclsDBLPTImport = new clsDBLPTImport();              

                _obj = GetDefualtSetting_All();

                ARAccount = _obj.Tables[0].Rows[0]["ArAccount"].ToString();
                SalesGLAccount = _obj.Tables[0].Rows[0]["SalesGLAccount"].ToString();

                objParams = new SqlParameter[8];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.SiteID);
                objParams[1] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                objParams[2] = new SqlParameter("IssueNo", _objclsBeansPhases.IssueNo);
                objParams[3] = new SqlParameter("TotalAmount", _objclsBeansPhases.ActualAmt);
                objParams[4] = new SqlParameter("Description", _objclsBeansPhases.Description);
                objParams[5] = new SqlParameter("Date", _objclsBeansPhases.Date);
                objParams[6] = new SqlParameter("Enteruser", clsBeansPhases.USERNAME);
                objParams[7] = new SqlParameter("EnterDate", clsBeansPhases.LOGINDATE);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                _IssueID = objDataAccess.ExecuteSPReturnObject("tblTransSiteIssuesHeader_Insert", objParams).ToString();

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("IssueNo", _IssueID);                
                objDataAccess.ExecuteSPReturnInteger("tblTransSiteIssuesDetails_Delete", objParams);

                foreach (DataRow dr in _objclsBeansPhases.Dtbl.Rows)
                {                  
                    objParams = new SqlParameter[12];
                    objParams[0] = new SqlParameter("IssueNo", _IssueID);
                    objParams[2] = new SqlParameter("PhaseID", dr["PhaseID"].ToString());
                    objParams[3] = new SqlParameter("SubPhaseID", dr["SubPhaseID"].ToString());
                    objParams[4] = new SqlParameter("WareHouseID", dr["LocationID"].ToString());
                    objParams[5] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[6] = new SqlParameter("Units", dr["Units"].ToString());
                    objParams[7] = new SqlParameter("IssuedQty", dr["Issued"].ToString());
                    objParams[8] = new SqlParameter("OnHandAtIssue", dr["OH"].ToString());
                    objParams[9] = new SqlParameter("BOMQty", dr["Estimated"].ToString());
                    objParams[1] = new SqlParameter("BOMBalanceQty", dr["Balance"].ToString());
                    objParams[10] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                    objParams[11] = new SqlParameter("LastUnitCost", dr["LastUnitCost"].ToString());
                    objDataAccess.ExecuteSPReturnInteger("tblTransSiteIssuesDetails_Insert", objParams);                    

                    objParams = new SqlParameter[3];
                    objParams[0] = new SqlParameter("WarehouseID", dr["LocationID"].ToString());
                    objParams[1] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[2] = new SqlParameter("IssuedQty", double.Parse(dr["Issued"].ToString()));
                    objDataAccess.ExecuteSPReturnInteger("tblItemWhse_Update", objParams);

                    objParams = new SqlParameter[11];
                    objParams[0] = new SqlParameter("DocType", "8");
                    objParams[2] = new SqlParameter("TranNo", _IssueID);
                    objParams[3] = new SqlParameter("TransDate", _objclsBeansPhases.Date);
                    objParams[4] = new SqlParameter("TranType", "JOBIS");
                    objParams[5] = new SqlParameter("DocReference", "0");
                    objParams[1] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[6] = new SqlParameter("Qty", dr["Issued"].ToString());
                    objParams[7] = new SqlParameter("UnitCost", dr["LastUnitCost"].ToString());
                    objParams[8] = new SqlParameter("TotalCost", double.Parse(dr["LastUnitCost"].ToString()) * double.Parse(dr["Issued"].ToString()));
                    objParams[9] = new SqlParameter("WarehouseID", dr["LocationID"].ToString());
                    objParams[10] = new SqlParameter("SellingPrice", dr["LastUnitCost"].ToString());
                    objDataAccess.ExecuteSPReturnInteger("tbItemlActivity_Insert", objParams);


                    objParams = new SqlParameter[6];
                    objParams[1] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                    objParams[2] = new SqlParameter("PhaseID", dr["PhaseID"].ToString());
                    objParams[3] = new SqlParameter("SubPhaseID", dr["SubPhaseID"].ToString());
                    objParams[4] = new SqlParameter("WareHouseID", dr["LocationID"].ToString());
                    objParams[5] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[0] = new SqlParameter("IssuedQty", dr["Issued"].ToString());
                    objDataAccess.ExecuteSPReturnInteger("tblTransBOMDetails_Update", objParams);

                }

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                objDataAccess.ExecuteSPReturnInteger("tblTransBOQHeader_Update", objParams);

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                objDataAccess.ExecuteSPReturnInteger("tblTransBOMHeader_Update", objParams);

                if(_objclsBeansPhases.DtblPT!=null)
                foreach (DataRow dr in _objclsBeansPhases.DtblPT.Rows)
                {
                    if (Save)
                    {
                        if (dr["ItemClass"].ToString() != "4" && dr["ItemClass"].ToString() != "5" && dr["ItemClass"].ToString() != "7")
                        {
                            objclsDBLPTImport.CreateXmlToExportInvAdjust("Job Issues", "", double.Parse(dr["Issued"].ToString()), 
                                dr["Item"].ToString(), _objclsBeansPhases.Date.ToString("MM/dd/yyyy"), _IssueID, 
                                double.Parse(dr["LastUnitCost"].ToString()), dr["GL"].ToString(), 
                                _objclsBeansPhases.SiteID + "," + dr["PhaseID"].ToString() + "," + dr["SubPhaseID"].ToString());

                            objclsDBLPTImport.IssueAdjustmentExport();
                        }

                        if (dr["ItemClass"].ToString() == "4" || dr["ItemClass"].ToString() == "5" || dr["ItemClass"].ToString() == "7")
                        {
                            objclsDBLPTImport.CreateXmlToExportLabourItems(double.Parse(dr["BOMRate"].ToString()), 
                                double.Parse(dr["Issued"].ToString()), dr["ItemDesc"].ToString(), dr["Item"].ToString(), 
                                _objclsBeansPhases.SiteID + "," + dr["PhaseID"].ToString() + "," + dr["SubPhaseID"].ToString(), 
                                dr["GL"].ToString(), _IssueID, _objclsBeansPhases.DtblPT.Rows.Count, _objclsBeansPhases.CustomerID, _objclsBeansPhases.Date, ARAccount,"Job Issues");
                            objclsDBLPTImport.ImportLabourItems();
                        }

                        //objclsDBLPTImport.CreateXmlToExportJobActuals(_objclsBeansPhases.BOMID,_objclsBeansPhases.SiteID, _objclsBeansPhases.DtblPT, objDataAccess);
                        //objclsDBLPTImport.JobActualExport();
                    }
                }

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.SiteID);
                objDataAccess.ExecuteSPReturnInteger("tblJobHeader_Update", objParams);

                SaveDocumentList(_objclsBeansPhases, _IssueID, objDataAccess);               

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
            return _IssueID;
        }

        public string SaveReturns(clsBeansPhases _objclsBeansPhases, bool Save)
        {
            string _ReturnID = "";
            try
            {
                DataSet _obj = new DataSet();

                string SalesGLAccount = "";
                string ARAccount = "";

                clsDBLPTImport objclsDBLPTImport = new clsDBLPTImport();

                _obj = GetDefualtSetting_All();

                ARAccount = _obj.Tables[0].Rows[0]["ArAccount"].ToString();
                SalesGLAccount = _obj.Tables[0].Rows[0]["SalesGLAccount"].ToString();

                objParams = new SqlParameter[9];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.SiteID);
                objParams[1] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                objParams[2] = new SqlParameter("IssueNo", _objclsBeansPhases.IssueNo);
                objParams[3] = new SqlParameter("TotalAmount", _objclsBeansPhases.ActualAmt);
                objParams[4] = new SqlParameter("Description", _objclsBeansPhases.Description);
                objParams[5] = new SqlParameter("Date", _objclsBeansPhases.Date);
                objParams[6] = new SqlParameter("Enteruser", clsBeansPhases.USERNAME);
                objParams[7] = new SqlParameter("EnterDate", clsBeansPhases.LOGINDATE);
                objParams[8] = new SqlParameter("ReturnNo", _objclsBeansPhases.ReturnNo);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                _ReturnID = objDataAccess.ExecuteSPReturnObject("tblTransSiteReturnsHeader_Insert", objParams).ToString();

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("ReturnNo", _ReturnID);
                objDataAccess.ExecuteSPReturnInteger("tblTransSiteReturnDetails_Delete", objParams);

                foreach (DataRow dr in _objclsBeansPhases.Dtbl.Rows)
                {
                    objParams = new SqlParameter[12];
                    objParams[0] = new SqlParameter("ReturnNo", _ReturnID);
                    objParams[2] = new SqlParameter("PhaseID", dr["PhaseID"].ToString());
                    objParams[3] = new SqlParameter("SubPhaseID", dr["SubPhaseID"].ToString());
                    objParams[4] = new SqlParameter("WareHouseID", dr["LocationID"].ToString());
                    objParams[5] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[6] = new SqlParameter("Units", dr["Units"].ToString());
                    objParams[7] = new SqlParameter("ReturnedQty", dr["Returned"].ToString());
                    objParams[8] = new SqlParameter("OnHandAtReturn", dr["OH"].ToString());
                    objParams[9] = new SqlParameter("BOMQty", dr["Estimated"].ToString());
                    objParams[1] = new SqlParameter("BOMBalanceQty", dr["Balance"].ToString());
                    objParams[10] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                    objParams[11] = new SqlParameter("LastUnitCost", dr["LastUnitCost"].ToString());
                    objDataAccess.ExecuteSPReturnInteger("tblTransSiteReturnDetails_Insert", objParams);

                    objParams = new SqlParameter[6];
                    objParams[0] = new SqlParameter("IssueNo", _objclsBeansPhases.IssueNo);
                    objParams[2] = new SqlParameter("PhaseID", dr["PhaseID"].ToString());
                    objParams[3] = new SqlParameter("SubPhaseID", dr["SubPhaseID"].ToString());
                    objParams[4] = new SqlParameter("WareHouseID", dr["LocationID"].ToString());
                    objParams[5] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[1] = new SqlParameter("ReturnedQty", dr["Returned"].ToString());
                    objDataAccess.ExecuteSPReturnInteger("tblTransSiteIssuesDetails_Update", objParams);                   

                    objParams = new SqlParameter[3];
                    objParams[0] = new SqlParameter("WarehouseID", dr["LocationID"].ToString());
                    objParams[1] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[2] = new SqlParameter("IssuedQty", (double.Parse(dr["Returned"].ToString()) * (-1)));
                    objDataAccess.ExecuteSPReturnInteger("tblItemWhse_Update", objParams);

                    objParams = new SqlParameter[11];
                    objParams[0] = new SqlParameter("DocType", "8");
                    objParams[2] = new SqlParameter("TranNo", _ReturnID);
                    objParams[3] = new SqlParameter("TransDate", _objclsBeansPhases.Date);
                    objParams[4] = new SqlParameter("TranType", "JOBRE");
                    objParams[5] = new SqlParameter("DocReference", "0");
                    objParams[1] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[6] = new SqlParameter("Qty", dr["Returned"].ToString());
                    objParams[7] = new SqlParameter("UnitCost", dr["LastUnitCost"].ToString());
                    objParams[8] = new SqlParameter("TotalCost", double.Parse(dr["LastUnitCost"].ToString()) * double.Parse(dr["Returned"].ToString()));
                    objParams[9] = new SqlParameter("WarehouseID", dr["LocationID"].ToString());
                    objParams[10] = new SqlParameter("SellingPrice", dr["LastUnitCost"].ToString());
                    objDataAccess.ExecuteSPReturnInteger("tbItemlActivity_Insert", objParams);

                    objParams = new SqlParameter[6];
                    objParams[1] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                    objParams[2] = new SqlParameter("PhaseID", dr["PhaseID"].ToString());
                    objParams[3] = new SqlParameter("SubPhaseID", dr["SubPhaseID"].ToString());
                    objParams[4] = new SqlParameter("WareHouseID", dr["LocationID"].ToString());
                    objParams[5] = new SqlParameter("ItemID", dr["Item"].ToString());
                    objParams[0] = new SqlParameter("IssuedQty", (double.Parse(dr["Returned"].ToString())*(-1)));
                    objDataAccess.ExecuteSPReturnInteger("tblTransBOMDetails_Update", objParams);
                }

                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("BOMID", _objclsBeansPhases.BOMID);
                objDataAccess.ExecuteSPReturnInteger("tblTransBOQHeader_Update", objParams);

                if (_objclsBeansPhases.DtblPT != null)
                    foreach (DataRow dr in _objclsBeansPhases.DtblPT.Rows)
                    {
                        if (Save)
                        {
                            if (dr["ItemClass"].ToString() != "4" && dr["ItemClass"].ToString() != "5" && dr["ItemClass"].ToString() != "7")
                            {
                                objclsDBLPTImport.CreateXmlToExportInvAdjust("Job Returns","", (double.Parse(dr["Returned"].ToString())*(-1)), dr["Item"].ToString(), _objclsBeansPhases.Date.ToString("MM/dd/yyyy"), _ReturnID, double.Parse(dr["LastUnitCost"].ToString()), dr["GL"].ToString(), _objclsBeansPhases.SiteID + "," + dr["PhaseID"].ToString() + "," + dr["SubPhaseID"].ToString());

                                objclsDBLPTImport.IssueAdjustmentExport();
                            }

                            if (dr["ItemClass"].ToString() == "4" || dr["ItemClass"].ToString() == "5" && dr["ItemClass"].ToString() != "7")
                            {
                                objclsDBLPTImport.CreateXmlToExportLabourItems_Return((double.Parse(dr["Returned"].ToString()) * (-1)), dr["ItemDesc"].ToString(), dr["Item"].ToString(), _objclsBeansPhases.SiteID + "," + dr["PhaseID"].ToString() + "," + dr["SubPhaseID"].ToString(), dr["GL"].ToString(), _ReturnID, _objclsBeansPhases.DtblPT.Rows.Count, _objclsBeansPhases.CustomerID, _objclsBeansPhases.Date, ARAccount);
                                objclsDBLPTImport.ImportLabourItems();
                            }

                            objclsDBLPTImport.CreateXmlToExportJobActuals(_objclsBeansPhases.BOMID,_objclsBeansPhases.SiteID, _objclsBeansPhases.DtblPT,objDataAccess);
                            objclsDBLPTImport.JobActualExport();
                        }
                    }

               objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _objclsBeansPhases.SiteID);
                objDataAccess.ExecuteSPReturnInteger("tblJobHeader_Update", objParams);

                SaveDocumentList(_objclsBeansPhases, _ReturnID, objDataAccess);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
            return _ReturnID;
        }

        public void DeleteIssues(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("IssueNo", _objclsBeansPhases.IssueNo);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblTransSiteIssuesHeader_Delete", objParams);
                objDataAccess.ExecuteSPReturnObject("tblTransSiteIssuesDetails_Delete", objParams);

                DeleteDocumentList(_objclsBeansPhases.IssueNo, objDataAccess);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void DeleteReturns(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("ReturnNo", _objclsBeansPhases.ReturnNo);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblTransSiteReturnsHeader_Delete", objParams);
                objDataAccess.ExecuteSPReturnObject("tblTransSiteReturnsDetails_Delete", objParams);

                DeleteDocumentList(_objclsBeansPhases.ReturnNo, objDataAccess);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }


        public DataSet GetToBeIssuedQty(string _BOMID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("BOMID", _BOMID);

                objDataAccess = new clsDataAccess();
                _dts = objDataAccess.ExecuteSPReturnDataset("GetToBeIssuedQty", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOQBalance(string _BOQID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("BOQID", _BOQID);

                objDataAccess = new clsDataAccess();
                _dts = objDataAccess.ExecuteSPReturnDataset("GetBOQ_Balances", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOQBalance(string _BOQID,string _PhaseID,string _SubPhaseID, clsDataAccess objDataAccess)
        {
            DataSet _dts = new DataSet();
            try
            {                
                objParams = new SqlParameter[3];
                objParams[0] = new SqlParameter("BOMID", _BOQID);
                objParams[1] = new SqlParameter("PhaseID", _PhaseID);
                objParams[2] = new SqlParameter("SubPhaseID", _SubPhaseID);

                //objDataAccess.BeginTransaction();
                _dts = objDataAccess.ExecuteSPReturnDataset_WithoutOpen("GetBOQ_Balances_By_Phase_SubPhase", objParams);
                //objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOMBalance(string _BOMID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("BOMID", _BOMID);

                objDataAccess = new clsDataAccess();
                _dts = objDataAccess.ExecuteSPReturnDataset("GetBOM_Balances", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOQs_BySubPhaseID(string _SubPhaseID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SubPhaseID", _SubPhaseID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOQDetails_Select_By_SubPhaseID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOQs_ByPhaseID(string _PhaseID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("PhaseID", _PhaseID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOQDetails_Select_By_PhaseID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOQs_BySubPhaseID_PhaseID(string _SubPhaseID,string _PhaseID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("SubPhaseID", _SubPhaseID);
                objParams[1] = new SqlParameter("PhaseID", _PhaseID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOQDetails_Select_By_SubPhaseID_PhaseID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetItems_By_WH_ItemCode(string _WHID, string _ItemID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("WhseId", _WHID);
                objParams[1] = new SqlParameter("ItemId", _ItemID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblItemWhse_By_WH_ITEMCODE", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }


        public string GetCodeforArrow(string Type, string CurrntCode, string Side)
        {
            string _tempCode="";
            DataSet _dts = new DataSet();
            try
            {
                objDataAccess = new clsDataAccess();
                objParams = new SqlParameter[2];

                if (Type == "Job")
                {
                    objParams[0] = new SqlParameter("SiteID", CurrntCode);
                    objParams[1] = new SqlParameter("Side", Side);
                    _dts = objDataAccess.ExecuteSPReturnDataset("tblJobHeader_Select_Arrow", objParams);

                    if(_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                        _tempCode = _dts.Tables[0].Rows[0][0].ToString();
                }
                else if (Type == "Phases")
                {
                    objParams[0] = new SqlParameter("PhaseID", CurrntCode);
                    objParams[1] = new SqlParameter("Side", Side);
                    _dts = objDataAccess.ExecuteSPReturnDataset("tblPhases_Select_Arrow", objParams);//.Tables[0].Rows[0][0].ToString();
                    if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                        _tempCode = _dts.Tables[0].Rows[0][0].ToString();
                }
                else if (Type == "SubPhase")
                {
                    objParams[0] = new SqlParameter("SubPhaseID", CurrntCode);
                    objParams[1] = new SqlParameter("Side", Side);
                    _dts = objDataAccess.ExecuteSPReturnDataset("tblSubPhases_Select_Arrow", objParams);//.Tables[0].Rows[0][0].ToString();
                    if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                        _tempCode = _dts.Tables[0].Rows[0][0].ToString();
                }
                else if (Type == "BOQ")
                {
                    objParams[0] = new SqlParameter("BOQID", CurrntCode);
                    objParams[1] = new SqlParameter("Side", Side);
                    _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOQHeader_Select_Arrow", objParams);//.Tables[0].Rows[0][0].ToString();
                    if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                        _tempCode = _dts.Tables[0].Rows[0][0].ToString();
                }
                else if (Type == "BOM")
                {
                    objParams[0] = new SqlParameter("BOMID", CurrntCode);
                    objParams[1] = new SqlParameter("Side", Side);
                    _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOMHeader_Select_Arrow", objParams);//.Tables[0].Rows[0][0].ToString();
                    if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                        _tempCode = _dts.Tables[0].Rows[0][0].ToString();
                }
                else if (Type == "Issue")
                {
                    objParams[0] = new SqlParameter("IssueNo", CurrntCode);
                    objParams[1] = new SqlParameter("Side", Side);
                    _dts = objDataAccess.ExecuteSPReturnDataset("tblTransSiteIssuesHeader_Select_Arrow", objParams);//.Tables[0].Rows[0][0].ToString();
                    if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                        _tempCode = _dts.Tables[0].Rows[0][0].ToString();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _tempCode;
        }

        public DataSet GetIssues_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransSiteIssuesHeader_SelectAll", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet GetReturns_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransSiteReturnsHeader_SelectAll", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public void SaveTemp(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                string Item = "";
                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();

                objParams = new SqlParameter[0];
                objDataAccess.ExecuteSPReturnObject("TEMP_sp_Delete", objParams);

                foreach (DataRow dr in _objclsBeansPhases.Dtbl.Rows)
                {

                    //if (dr["Item"].ToString().Trim().Length > 0)
                    //{
                        objParams = new SqlParameter[9];
                    if(_objclsBeansPhases.Dtbl.Columns.Contains("Item"))
                        Item=dr["Item"].ToString();

                        objParams[8] = new SqlParameter("Item", Item);
                    //}
                    //else
                    //{
                    //    objParams = new SqlParameter[8];
                    //}                 
                    objParams[2] = new SqlParameter("PhaseID", dr["PhaseID"].ToString());
                    objParams[3] = new SqlParameter("SubPhaseID", dr["SubPhaseID"].ToString());
                    objParams[4] = new SqlParameter("Activity", dr["Activity"].ToString());
                    objParams[5] = new SqlParameter("LocCode", dr["LocationID"].ToString());
                    objParams[6] = new SqlParameter("Rate", dr["Rate"].ToString());
                    objParams[7] = new SqlParameter("Amt", dr["Amount"].ToString());
                    objParams[0] = new SqlParameter("Units", dr["Units"].ToString());
                    objParams[1] = new SqlParameter("Qty", dr["Qty"].ToString());

                    objDataAccess.ExecuteSPReturnInteger("TEMP_sp_Insert", objParams);
                }               

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public DataSet GetTemp_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("TEMP_sp_Select", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public void DeleteTemp(clsBeansPhases _objclsBeansPhases)
        {
            try
            {
                objParams = new SqlParameter[1];
                //objParams[0] = new SqlParameter("IssueNo", _objclsBeansPhases.IssueNo);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("TEMP_sp_Delete", objParams);
                //objDataAccess.ExecuteSPReturnObject("tblTransSiteIssuesDetails_Delete", objParams);

                //DeleteDocumentList(_objclsBeansPhases.IssueNo, objDataAccess);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public DataSet GetBOQs_BySiteID(string _SiteID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _SiteID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOQDetails_Select_By_SiteID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }



        public DataSet GetBOMs_BySiteID(string _SiteID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _SiteID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOMDetails_Select_By_SiteID", objParams);
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
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("WhseId", _ID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblWhseMaster_select_By_ID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        //public DataSet GetWereHouse_By_ID(string _ID)
        //{
        //    DataSet _dts = new DataSet();
        //    try
        //    {
        //        objParams = new SqlParameter[1];
        //        objParams[0] = new SqlParameter("SiteID", _ID);

        //        objDataAccess = new clsDataAccess();
        //        objDataSet = new DataSet();
        //        objclsBeansPhases = new clsBeansPhases();

        //        _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOMHeader_SelectAll_Active_By_SiteID_ForDropDown", objParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return _dts;
        //}

        public DataSet GetDefualtSetting_All()
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[0];
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblDefualtSetting_Select", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }


        public DataSet GetIssues_BySiteID(string _SiteID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _SiteID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblTransSiteIssuesDetails_Select_By_SiteID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetBOM_By_ItemID_PhaseID_SubPhaseID(string _ItemID, string _PhaseID, string _SubPhaseID)
        {
            DataSet _dts = new DataSet();
            try
            {
                objParams = new SqlParameter[3];
                objParams[0] = new SqlParameter("ItemID", _ItemID);
                objParams[1] = new SqlParameter("PhaseID", _PhaseID);
                objParams[2] = new SqlParameter("SubPhaseID", _SubPhaseID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                _dts = objDataAccess.ExecuteSPReturnDataset("tblTransBOMDetails_Select_By_Item_Phase_SubPhase", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dts;
        }

        public DataSet GetExpences_By_SiteID(string _SiteID)
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("SiteID", _SiteID);
                objDataSet = objDataAccess.ExecuteSPReturnDataset("Get_Espences", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public DataSet getIssuedetails_BySiteID_Phase_SubPhase(string _SiteID,string _Phase,string _SubPhase)
        {
            objDataAccess = new clsDataAccess();
            objDataSet = new DataSet();
            try
            {
                objParams = new SqlParameter[3];
                objParams[0] = new SqlParameter("SiteID", _SiteID);
                objParams[1] = new SqlParameter("PhaseID", _Phase);
                objParams[2] = new SqlParameter("SubPhaseID", _SubPhase);
                objDataSet = objDataAccess.ExecuteSPReturnDataset("tblTransSiteIssuesDetails_Select_By_SiteID_PhaseID_SubPhaseID", objParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDataSet;
        }

        public void Save_PeachtreeCompany(string _PTCompany)
        {
            try
            {
                objParams = new SqlParameter[1];
                objParams[0] = new SqlParameter("CompanyName", _PTCompany);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblCompanyInformation_Insert", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void DIsconnectPeachTree()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();
                //  PeachtreeAccounting.Login login = new  PeachtreeAccounting.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword
                app.CloseCompany();

                
            }
            catch (Exception ex)
            {
                app.ExecuteCommand("File|Exit", null);
                //MessageBox.Show("Please Stop the Process peachw.exe from Task Manager First");
                throw ex;

            }
        }

        public string LinkPeachTree()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword

                return app.CurrentCompanyName;
                             
            }
            catch (Exception ex)
            {
                app.ExecuteCommand("File|Exit", null);
                throw ex;
            }
        }

        public clsBeansPhases GetBOMs_ByID_Max(string _ID, int _RevNo, string _PhaseID, string _SubPhaseID)
        {
            //            tblTransBOQDetails_Select_By_ID_Max_Phase_SubPhase]
            //@BOQID	varchar(20)	,
            //@RevNo	int		,
            //@PhaseID varchar(20),
            //@SubPhaseID varchar(20)

            try
            {
                objParams = new SqlParameter[4];
                objParams[0] = new SqlParameter("BOMID", _ID);
                objParams[1] = new SqlParameter("RevNo", _RevNo);
                objParams[2] = new SqlParameter("PhaseID", _PhaseID);
                objParams[3] = new SqlParameter("SubPhaseID", _SubPhaseID);

                objDataAccess = new clsDataAccess();
                objDataSet = new DataSet();
                objclsBeansPhases = new clsBeansPhases();

                objclsBeansPhases.Dtbl = objDataAccess.ExecuteSPReturnDataset("tblTransBOMDetails_Select_By_ID_Max_Phase_SubPhase", objParams).Tables[0];


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objclsBeansPhases;
        }
    }
}
