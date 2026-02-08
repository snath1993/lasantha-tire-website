using System;
using System.Collections.Generic;
using System.Text;
using Interop.PeachwServer;
using System.IO;
using System.Windows.Forms;
using PCMBeans;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using DataAccess;
using Infragistics.Win.UltraWinProgressBar;



namespace PCMDBL
{
    public class clsDBLPTImport
    {
        public Interop.PeachwServer.Application app;

        public Interop.PeachwServer.Export ImportFromPeactree;
       Interop.PeachwServer.Export exporter;
       Interop.PeachwServer.Import importer;
        public Array coa;
        clsBeansPhases objclsBeansPhases;
        clsDBLPhases objclsDBLPhases;
        SqlParameter[] objParams;
        clsDataAccess objDataAccess;

        
        public void ImportPhases_List(UltraProgressBar up)
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword                

                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Phases.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPhaseList);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseHasCost);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_CostType);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_Inactive);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseHasCost);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Phases.xml");
                exporter.Export();

                fillID_Phases_list(up);

                //ImportVenders_List();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void ImportSubPhases_List(UltraProgressBar up)
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword                

                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SubPhases.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjCostCodeList);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCostCodeListField.peachwIEObjCostCodeListField_CostCodeId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCostCodeListField.peachwIEObjCostCodeListField_CostCodeDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCostCodeListField.peachwIEObjCostCodeListField_CostCodeType);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCostCodeListField.peachwIEObjCostCodeListField_Inactive);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SubPhases.xml");
                exporter.Export();

                fillID_SubPhases_list(up);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void ImportVenders_List()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword                

                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Venders.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjVendorList);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorAddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorAddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorBalance);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCategory);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCODTerms);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorContact);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCountry);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCreditLimit);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorDateAdded);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorDayIsActual);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorDiscountDays);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorDiscountPercent);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorDueDays);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorDueMonthEnd);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorEmail);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorFax);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorFormDeliveryMethod);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorInactive);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCategory);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorOurAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorPhone1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorPhone2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorPrePayTerms);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorPurchRepId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorState);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorTermsText);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorZip);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Venders.xml");
                exporter.Export();

                fillID_Vendor_list();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void fillID_Job_list(UltraProgressBar up)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            string _strSiteID = "";
            string _strDescription = "";
            bool _boolInactive = false;
            string _strSupervisor = "";
            string _strCustomer = "";
            string _dtmActualStartDate = "";
            string _dtmActualEndDate = "";
            string _strBillingMethod = "";
            double _dblStatus = 0;
            double _dblRetainPersentage = 0;
            double _dblPaymentTerms = 0;
            bool _boolUsePhases = false;
            string _JobStatus = "";
            string _Category = "";
            string _ProjEndDate = "";
            double _Labor_Burden_Percent = 0.00;



            try
            {
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Job");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                coa = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                up.Step = aLength;
                up.Minimum = 0;
                up.Maximum = aLength;

                for (int j = 0; j <= aLength - 1; j++)
                {
                    _strSiteID = "";
                    _strDescription = "";
                    _boolInactive = false;
                    _strSupervisor = "";
                    _strCustomer = "";
                    _dtmActualStartDate = "";
                    _dtmActualEndDate = "";
                    _strBillingMethod = "";
                    _dblStatus = 0;
                    _dblRetainPersentage = 0;
                    _dblPaymentTerms = 0;
                    _boolUsePhases = false;
                    _JobStatus = "";
                    _Category = "";
                    _ProjEndDate = "";
                    _Labor_Burden_Percent = 0.00;

                    node = reader[j];
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        if (node.ChildNodes[i].Name == "ID")
                        {
                            _strSiteID = node.ChildNodes[i].InnerText;
                        }//
                        else if (node.ChildNodes[i].Name == "Labor_Burden_Percent")
                        {
                            _Labor_Burden_Percent = Convert.ToDouble(node.ChildNodes[i].InnerText);
                        }
                        else if (node.ChildNodes[i].Name == "Category")
                        {
                            _Category = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Projected_End_Date")
                        {
                            _ProjEndDate = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Description")
                        {
                            _strDescription = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Job_Status")
                        {
                            _JobStatus = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "isInactive")
                        {
                            _boolInactive = Convert.ToBoolean(node.ChildNodes[i].InnerText);
                        }
                        else if (node.ChildNodes[i].Name == "Supervisor")
                        {
                            _strSupervisor = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Customer_ID")
                        {
                            _strCustomer = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Start_Date")
                        {
                            _dtmActualStartDate = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "End_Date")
                        {
                            _dtmActualEndDate = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Billing_Method")
                        {
                            _strBillingMethod = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Percent_Complete")
                        {
                            _dblStatus = double.Parse(node.ChildNodes[i].InnerText);
                        }
                        else if (node.ChildNodes[i].Name == "Retainage_Percent")
                        {
                            _dblRetainPersentage = double.Parse(node.ChildNodes[i].InnerText);
                        }
                        else if (node.ChildNodes[i].Name == "UsePhases")
                        {
                            _boolUsePhases = bool.Parse(node.ChildNodes[i].InnerText);
                        }
                    }
                    SaveJob_PeachtreeRecords(_strSiteID, _strDescription, _boolInactive, _strSupervisor, _strCustomer, _dtmActualStartDate, _dtmActualEndDate, _strBillingMethod, _dblStatus, _dblRetainPersentage, _dblPaymentTerms, _boolUsePhases, _JobStatus, _Category, _Labor_Burden_Percent, _ProjEndDate);
                    up.Value = aLength;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //                    SaveJob_PeachtreeRecords(_strSiteID, _strDescription, _boolInactive,
        //_strSupervisor, _strCustomer, _dtmActualStartDate, _dtmActualEndDate, _strBillingMethod, _dblStatus,
        //_dblRetainPersentage, _dblPaymentTerms, _boolUsePhases, _JobStatus,_Category,_Labor_Burden_Percent,_ProjEndDate);

        private void SaveJob_PeachtreeRecords(string _strSiteID, string _strDescription, bool _boolInactive,string _strSupervisor, string _strCustomer, string _dtmActualStartDate, string _dtmActualEndDate,string _strBillingMethod, double _dblStatus, double _dblRetainPersentage, double _dblPaymentTerms,bool _boolUsePhases, string _JobStatus, string _Category, double _Labor_Burden_Percent, string _ProjEndDate)
        {
            try
            {
                //string dtmActualStartDate = DateTime.MinValue.ToString("MM/dd/yyyy");
                //string dtmActualEndDate = DateTime.MinValue.ToString("MM/dd/yyyy");
                //string dtmProjEndDate = DateTime.MinValue.ToString("MM/dd/yyyy");

                //if (_dtmActualStartDate != string.Empty) dtmActualStartDate = (_dtmActualStartDate);
                //if (_dtmActualEndDate != string.Empty) dtmActualEndDate = (_dtmActualEndDate);
                //if (_ProjEndDate != string.Empty) dtmProjEndDate = (_ProjEndDate);

                objParams = new SqlParameter[16];
                objParams[0] = new SqlParameter("SiteID", _strSiteID);
                objParams[1] = new SqlParameter("Description", _strDescription);
                objParams[2] = new SqlParameter("Inactive", _boolInactive);
                objParams[3] = new SqlParameter("Supervisor", _strSupervisor);
                objParams[4] = new SqlParameter("Customer", _strCustomer);
                objParams[5] = new SqlParameter("ActualStartDate", _dtmActualStartDate);
                objParams[6] = new SqlParameter("ActualEndDate", _dtmActualEndDate);
                objParams[7] = new SqlParameter("BillingMethod", _strBillingMethod);
                objParams[8] = new SqlParameter("Status", _dblStatus);
                objParams[9] = new SqlParameter("RetainPersentage", _dblRetainPersentage);
                objParams[10] = new SqlParameter("PaymentTerms", _dblPaymentTerms);
                objParams[11] = new SqlParameter("UsePhases", _boolUsePhases);
                objParams[12] = new SqlParameter("JobStatus", _JobStatus);
                objParams[13] = new SqlParameter("Labor_Burden_Percent", _Labor_Burden_Percent);
                objParams[14] = new SqlParameter("EstEndDate", _ProjEndDate);
                objParams[15] = new SqlParameter("Category", _Category);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblJobHeader_Save_PeachtreeRecords", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void ImportJob_List(UltraProgressBar up)
        {
            try
            {
                // ExportJob_List();
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword                

                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjJobList);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_BillingMethod);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Category);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Commnent);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionCostCodeId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedExpense);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedRevenue);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionPhaseId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_EndDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Inactive);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobAddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobAddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobCity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobCountry);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobState);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobZip);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_LaborBurdenPercent);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_NumberOfUnits);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_PercentComplete);
              //  exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_ProjectedEndDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_PurchaseOrderNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_RetainagePercent);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_StartDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Supervisor);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_UsePhases);
              //  exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobStatus);
              //  exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_ProjectedEndDate);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml");
                exporter.Export();

                fillID_Job_list(up);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void ExportJob_List()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword                

                //File.Delete(System.Windows.Forms.Application.StartupPath + "\\Job.xml");
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjJobList);
                //importer.ClearImportFieldList();
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_BillingMethod);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Category);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Commnent);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionCostCodeId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedExpense);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedRevenue);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionPhaseId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_EndDate);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Inactive);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobAddressLine1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobAddressLine2);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobCity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobCountry);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobDescription);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobState);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobZip);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_LaborBurdenPercent);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_NumberOfUnits);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_PercentComplete);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_ProjectedEndDate);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_PurchaseOrderNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_RetainagePercent);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_StartDate);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Supervisor);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_UsePhases);
                // importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.
                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml");
                importer.Import();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// this method contains PhaseID, Descriptiion and Inactive status 
        /// insert statement amd update statement for update Inactive status
        /// </summary>
        public void fillID_Phases_list(UltraProgressBar up)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            string _strPhaseDesc = "";
            string _strPhaseID = "";
            bool _boolInactive = false;
            bool _boolUseCostCodes = false;
            string _strCostType = "";

            try
            {
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Phases.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Phase");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                coa = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                up.Step = aLength;
                up.Minimum = 0;
                up.Maximum = aLength;

                for (int j = 0; j <= aLength - 1; j++)
                {
                    _boolUseCostCodes = false;
                    node = reader[j];
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        if (node.ChildNodes[i].Name == "ID")
                        {
                            _strPhaseID = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Description")
                        {
                            _strPhaseDesc = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "isInactive")
                        {
                            _boolInactive = Convert.ToBoolean(node.ChildNodes[i].InnerText);
                        }
                        else if (node.ChildNodes[i].Name == "UseCostCodes")
                        {
                            _boolUseCostCodes = true;
                        }
                        else if (node.ChildNodes[i].Name == "CostType")
                        {
                            _strCostType = node.ChildNodes[i].InnerText;
                        }
                    }
                    SavePhases_PeachtreeRecords(_strPhaseID, _strPhaseDesc, _boolInactive, _boolUseCostCodes, _strCostType);
                    up.Value = aLength;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

        public void fillID_Vendor_list()
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();

            bool _boolInactive = false;
            string _strVendorID = "";
            string _strVendorName = "";
            string _strVContact = "";
            string _strVAddress1 = "";
            string _strVAddress2 = "";
            string _strCategory = "";

            try
            {
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Venders.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Vendor");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                coa = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                for (int j = 0; j <= aLength - 1; j++)
                {
                    _boolInactive = false;
                    _strVendorID = "";
                    _strVendorName = "";
                    _strVContact = "";
                    _strVAddress1 = "";
                    _strVAddress2 = "";
                    _strCategory = "";

                    node = reader[j];
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        if (node.ChildNodes[i].Name == "ID")
                        {
                            _strVendorID = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Name")
                        {
                            _strVendorName = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "VendorType")
                        {
                            _strCategory = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "ContactName")
                        {
                            _strVContact = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "RemitToAddress")
                        {
                            for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                            {
                                if (node.ChildNodes[i].ChildNodes[k].Name == "Line1")
                                {
                                    _strVAddress1 = node.ChildNodes[i].ChildNodes[k].InnerText;
                                }
                                else if (node.ChildNodes[i].ChildNodes[k].Name == "Line2")
                                {
                                    _strVAddress2 = node.ChildNodes[i].ChildNodes[k].InnerText;
                                }
                            }
                        }
                        else if (node.ChildNodes[i].Name == "isInactive")
                        {
                            _boolInactive = Convert.ToBoolean(node.ChildNodes[i].InnerText);
                        }
                    }
                    SaveVendors_PeachtreeRecords(_strVendorID, _strVendorName, _strVContact, _strVAddress1, _strVAddress2, _strCategory, _boolInactive);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SavePhases_PeachtreeRecords(string _strPhaseIDValue, string _strPhaseDescValue, bool _boolInactiveValue, bool _boolUseCostCodes, string _strCostType)
        {
            try
            {
                objParams = new SqlParameter[5];

                if (_boolUseCostCodes) _strCostType = "";
                objParams[0] = new SqlParameter("PhaseID", _strPhaseIDValue);
                objParams[1] = new SqlParameter("PhaseDesc", _strPhaseDescValue);
                objParams[2] = new SqlParameter("Inactive", _boolInactiveValue);
                objParams[3] = new SqlParameter("UseCostCode", _boolUseCostCodes);
                objParams[4] = new SqlParameter("CostType", _strCostType);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblPhases_Save_PeachtreeRecords", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void SaveVendors_PeachtreeRecords(string _strVendorID, string _strVendorName, string _strVContact, string _strVAddress1, string _strVAddress2, string _strCategory, bool _boolInactive)
        {
            try
            {
                objParams = new SqlParameter[7];
                objParams[0] = new SqlParameter("VendorID", _strVendorID);
                objParams[1] = new SqlParameter("VendorName", _strVendorName);
                objParams[2] = new SqlParameter("VContact", _strVContact);
                objParams[3] = new SqlParameter("VAddress1", _strVAddress1);
                objParams[4] = new SqlParameter("VAddress2", _strVAddress2);
                objParams[5] = new SqlParameter("Type", _strCategory);
                objParams[6] = new SqlParameter("Inactive", _boolInactive);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblVendorMaster_Save_PeachtreeRecords", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void fillID_SubPhases_list(UltraProgressBar up)
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsDBLPhases = new clsDBLPhases();
            string _strSubPhaseDesc = "";
            string _strSubPhaseID = "";
            bool _boolInactive = false;
            string _strCostType = "";

            try
            {
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SubPhases.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Cost");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                coa = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                up.Step = aLength;
                up.Minimum = 0;
                up.Maximum = aLength;

                for (int j = 0; j <= aLength - 1; j++)
                {
                    node = reader[j];
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        if (node.ChildNodes[i].Name == "ID")
                        {
                            _strSubPhaseID = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Description")
                        {
                            _strSubPhaseDesc = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "IsInactive")
                        {
                            _boolInactive = Convert.ToBoolean(node.ChildNodes[i].InnerText);
                        }
                        else if (node.ChildNodes[i].Name == "CostType")
                        {
                            _strCostType = node.ChildNodes[i].InnerText;
                        }
                    }
                    SaveSubPhases_PeachtreeRecords(_strSubPhaseID, _strSubPhaseDesc, _boolInactive, _strCostType);
                    up.Value = aLength;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveSubPhases_PeachtreeRecords(string _strSubPhaseID, string _strSubPhaseDesc, bool _boolInactive, string _strCostType)
        {
            try
            {
                objParams = new SqlParameter[4];
                objParams[0] = new SqlParameter("SubPhaseID", _strSubPhaseID);
                objParams[1] = new SqlParameter("SubPhaseDesc", _strSubPhaseDesc);
                objParams[2] = new SqlParameter("Inactive", _boolInactive);
                objParams[3] = new SqlParameter("CostType", _strCostType);

                objDataAccess = new clsDataAccess();
                objDataAccess.BeginTransaction();
                objDataAccess.ExecuteSPReturnObject("tblSubPhases_Save_PeachtreeRecords", objParams);

                objDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                objDataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void CreateXmlToExportBOQ(string SiteID, string Description, bool UsePhases, bool IsActive, string Supervisor, string CustomerID, DateTime StartDate, DateTime EndDate, string Category, string BillingMethos, double PercentCompleted, DataTable DtblDis)
        {
            //Create a Xmal File..................................................................................
            try
            {
                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\BOQ.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Jobs");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Job");
                Writer.WriteAttributeString("xsi:type", "paw:job");

                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(SiteID);//Customer ID should be here
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(Description);
                Writer.WriteEndElement();

                Writer.WriteStartElement("UsePhases");
                Writer.WriteString("TRUE");//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("isInactive");
                Writer.WriteString(IsActive.ToString().ToUpper());//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("Supervisor");
                Writer.WriteString(Supervisor);//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(CustomerID);//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("Start_Date");
                Writer.WriteAttributeString("xsi:type", "paw:date");
                Writer.WriteString(StartDate.ToString("MM/dd/yyyy"));//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("End_Date");
                Writer.WriteAttributeString("xsi:type", "paw:date");
                Writer.WriteString(EndDate.ToString("MM/dd/yyyy"));//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("Category");//Prepayment
                Writer.WriteString(Category);//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("Billing_Method");
                Writer.WriteString(BillingMethos);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Percent_Complete");
                Writer.WriteString(PercentCompleted.ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Distributions");

                foreach (DataRow dr in DtblDis.Rows)
                {

                    Writer.WriteStartElement("Distribution");

                    Writer.WriteStartElement("PhaseID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dr["PhaseID"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CostID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dr["SubPhaseID"].ToString());
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("EstimatedRevenue");
                    //Writer.WriteString(dr["Amount"].ToString());
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("EstimatedExpense");
                    Writer.WriteString(dr["Amount"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("NumberOfUnits");
                    Writer.WriteString(dr["Qty"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }

                Writer.WriteEndElement();

                Writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void CreateXmlToExportJob(bool IsProjEndDate, bool IsProjStartDate, bool IsActEndDate,string SiteID, string Description, bool IsActive,string Supervisor, string CustomerID, DateTime StartDate, DateTime EndDate,string Category, string BillingMethos, double RetainingPersentage, string Status,double CompletedPercentage, double Labor_Burden_Percent, DateTime ActualEndDate)
        {
            //Create a Xmal File..................................................................................
            try
            {
                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Jobs");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Job");
                Writer.WriteAttributeString("xsi:type", "paw:job");

                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(SiteID);//Customer ID should be here
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(Description);
                Writer.WriteEndElement();

                Writer.WriteStartElement("UsePhases");
                Writer.WriteString("TRUE");//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("isInactive");
                Writer.WriteString(IsActive.ToString().ToUpper());//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("Supervisor");
                Writer.WriteString(Supervisor);//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(CustomerID);//Cash Account
                Writer.WriteEndElement();

                if (IsProjStartDate)
                {
                    Writer.WriteStartElement("Start_Date");
                    Writer.WriteAttributeString("xsi:type", "paw:date");
                    Writer.WriteString(StartDate.ToString("MM/dd/yyyy"));//Cash Account
                    Writer.WriteEndElement();
                }

                if (IsProjEndDate)
                {
                    Writer.WriteStartElement("Projected_End_Date");
                    Writer.WriteAttributeString("xsi:type", "paw:date");
                    Writer.WriteString(EndDate.ToString("MM/dd/yyyy"));//Cash Account
                    Writer.WriteEndElement();
                }

                if (IsActEndDate)
                {
                    Writer.WriteStartElement("End_Date");
                    Writer.WriteAttributeString("xsi:type", "paw:date");
                    Writer.WriteString(ActualEndDate.ToString("MM/dd/yyyy"));//Cash Account
                    Writer.WriteEndElement();
                }

                Writer.WriteStartElement("Category");//Prepayment
                Writer.WriteString(Category);//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("Billing_Method");
                Writer.WriteString(BillingMethos);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Percent_Complete");
                Writer.WriteString(CompletedPercentage.ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Labor_Burden_Percent");
                Writer.WriteString(Labor_Burden_Percent.ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Retainage_Percent");
                Writer.WriteString(RetainingPersentage.ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Job_Status");
                Writer.WriteString(Status.ToString());
                Writer.WriteEndElement();

                Writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void CreateXmlToExportBOM(string SiteID, string Description, bool UsePhases, bool IsActive, string Supervisor, string CustomerID, DateTime StartDate, DateTime EndDate, string Category, string BillingMethos, double PercentCompleted, DataTable DtblDis)
        {
            //Create a Xmal File..................................................................................
            try
            {
                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\BOM.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Jobs");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Job");
                Writer.WriteAttributeString("xsi:type", "paw:job");

                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(SiteID);//Customer ID should be here
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(Description);
                Writer.WriteEndElement();

                Writer.WriteStartElement("UsePhases");
                Writer.WriteString(UsePhases.ToString().ToUpper());//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("isInactive");
                Writer.WriteString(IsActive.ToString().ToUpper());//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("Supervisor");
                Writer.WriteString(Supervisor);//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(CustomerID);//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("Start_Date");
                Writer.WriteAttributeString("xsi:type", "paw:date");
                Writer.WriteString(StartDate.ToString("MM/dd/yyyy"));//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("End_Date");
                Writer.WriteAttributeString("xsi:type", "paw:date");
                Writer.WriteString(EndDate.ToString("MM/dd/yyyy"));//Cash Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("Category");//Prepayment
                Writer.WriteString(Category);//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("Billing_Method");
                Writer.WriteString(BillingMethos);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Percent_Complete");
                Writer.WriteString(PercentCompleted.ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Distributions");

                foreach (DataRow dr in DtblDis.Rows)
                {
                    Writer.WriteStartElement("Distribution");

                    Writer.WriteStartElement("PhaseID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dr["PhaseID"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CostID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dr["SubPhaseID"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("EstimatedRevenue");
                    Writer.WriteString(dr["Amount"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("EstimatedExpense");
                    Writer.WriteString(dr["Amount"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("NumberOfUnits");
                    Writer.WriteString(dr["Qty"].ToString());
                    Writer.WriteEndElement();
                }

                Writer.WriteEndElement();

                Writer.WriteEndElement();

                Writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void JobExport(bool IsProjEndDate, bool IsProjStartDate, bool IsActEndDate)
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjJobList);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_BillingMethod);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Category);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_CustomerId);                
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Inactive);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobDescription);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_PercentComplete);
                
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Supervisor);

                if(IsProjStartDate)
                    importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_StartDate);
                if(IsProjEndDate)
                    //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_ProjectedEndDate);
                if(IsActEndDate)
                    importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_EndDate);

                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobStatus);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_RetainagePercent);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_LaborBurdenPercent);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.
                ////importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InventoryAccountId);
                //importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAmount);
                //importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DiscountAmount);


                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml");
                importer.Import();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BOMExport()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjJobList);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_BillingMethod);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Category);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_EndDate);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Inactive);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobDescription);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_PercentComplete);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_StartDate);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Supervisor);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_UsePhases);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionCostCodeId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedExpense);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedRevenue);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionPhaseId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionCostCodeId);

                ////importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InventoryAccountId);
                //importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAmount);
                //importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DiscountAmount);


                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\BOM.xml");
                importer.Import();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BOQExport()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjJobList);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_BillingMethod);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Category);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_EndDate);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Inactive);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobDescription);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_PercentComplete);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_StartDate);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Supervisor);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_UsePhases);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionCostCodeId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedExpense);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedRevenue);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionPhaseId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionCostCodeId);

                ////importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InventoryAccountId);
                //importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAmount);
                //importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DiscountAmount);


                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\BOQ.xml");
                importer.Import();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void JobActualExport()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjJobList);

                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_BillingMethod);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Category);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_CustomerId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_EndDate);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Inactive);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobDescription);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_PercentComplete);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_StartDate);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_Supervisor);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_UsePhases);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionCostCodeId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedExpense);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionEstimatedRevenue);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionPhaseId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_DistributionCostCodeId);

                ////importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InventoryAccountId);
                //importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAmount);
                //importer.AddToImportFieldList((short)PeachtreeAccounting.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DiscountAmount);


                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml");
                importer.Import();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double ImportItemUnitCost(string ItemID)
        {
            double _LastUnitCost = 0;
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();
                //  PeachtreeAccounting.Login login = new  PeachtreeAccounting.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword



                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsUnitCost.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjInventoryItemsList);
                // exporter = (PeachtreeAccounting.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);
                // exporter.SetFilterValue(

                exporter.SetFilterValue((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListFilter.peachwIEObjInventoryItemsListFilter_ItemId,
                PeachwIEFilterOperation.peachwIEFilterOperationRange, ItemID, ItemID);

                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_LaborCost);

                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsUnitCost.xml");
                exporter.Export();
                _LastUnitCost = fillLastUnitCost();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return _LastUnitCost;
        }

        public double fillLastUnitCost()
        {
            string ItemID = "";
            double LastUnitCost = 0.00;
            Array coa1;
            try
            {
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsUnitCost.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Item");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                coa1 = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                for (int i = 0; i <= aLength - 1; i++)
                {
                    node = reader[i];
                    ItemID = Convert.ToString(node.ChildNodes[0].InnerText);
                    LastUnitCost = Convert.ToDouble(node.ChildNodes[1].InnerText);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return LastUnitCost;

        }

        public void CreateXmlToExportInvAdjust(string Reason, string ItemClass, double IssueQty, string ItemCode, string IssueDate, string IssueNo, double UnitPrice, string InventoryAcc, string JobID)
        {

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\IssueAdjustment.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            if (IssueQty != 0)
            {
                Writer.WriteStartElement("PAW_Item");
                Writer.WriteAttributeString("xsi:type", "paw:item");


                //crate a ID element (tag)=====================(1)
                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(ItemCode);//dgvItem[0, c].Value
                Writer.WriteEndElement();

                //this sis crating tag for reference============(2)
                Writer.WriteStartElement("Reference");
                Writer.WriteString(IssueNo);
                Writer.WriteEndElement();

                //This is a Tag for Adjusment Date==============(3)
                Writer.WriteStartElement("Date");
                Writer.WriteAttributeString("xsi:type", "paw:date");
                Writer.WriteString(IssueDate.ToString());//Date format must be (MM/dd/yyyy)
                Writer.WriteEndElement();

                Writer.WriteStartElement("JobID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(JobID.ToString());//Date format must be (MM/dd/yyyy)
                Writer.WriteEndElement();

                //This is a Tag for numberof dsistribution=======(4)

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                //Adjustmne Lines=================================(5)
                Writer.WriteStartElement("AdjustmentItems");
                //Adjustmne Lines=================================(6)
                Writer.WriteStartElement("AdjustmentItem");

                //Gl ASccount======================================(7)
                Writer.WriteStartElement("GLSourceAccount");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(InventoryAcc);
                Writer.WriteEndElement();

                //Quantity========================================(9)
                if (Reason == "Job Issues")
                {
                    Writer.WriteStartElement("Quantity");
                    // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
                    Writer.WriteString("-" + IssueQty.ToString().Trim());
                    //Adjust_qty
                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement("Quantity");
                    // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
                    Writer.WriteString((Math.Abs(IssueQty)).ToString().Trim());
                    //Adjust_qty
                    Writer.WriteEndElement();
                }
                //Amount===========================================(10)              

                double Amount = IssueQty * UnitPrice;
                if (Reason == "Job Issues")
                {
                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + Math.Abs(Amount).ToString().Trim());
                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(Math.Abs(Amount).ToString().Trim());
                    Writer.WriteEndElement();
                }

                Writer.WriteStartElement("ReasonToAdjust");
                Writer.WriteString(Reason);
                Writer.WriteEndElement();

                Writer.WriteEndElement();//Adjustment Line
                Writer.WriteEndElement();//Adjustment lines

                Writer.WriteEndElement();//Item Closed Tag

                // Writer.WriteEndElement();//Items Closed Tag

                Writer.Close();//finishing writing xml file
                //}
            }
        }

        public void IssueAdjustmentExport()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();
                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword

                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjAdjustmentsJournal);

                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);
                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Reference);
                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Date);
                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_GLAccountId);
                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Quantity);
                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Amount);
                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ReasonToAdjust);
                importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_JobId);


                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\IssueAdjustment.xml");
                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);

                importer.Import();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void IssueAdjustmentImport()
        {
            try
            {

               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword                

                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\IssueNote.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjAdjustmentsJournal);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Reference);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Date);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_GLAccountId);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Quantity);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Amount);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ReasonToAdjust);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_JobId);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\IssueNote.xml");
                exporter.Export();




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateXmlToExportLabourItems_Return(double Qty, string ItemDesc, string ItemCode, string JobCode, string StrSalesGLAccount, string StrReference, int intRowCount, string StrCustId, DateTime InvDate, string StrARAccount)
        {
            int intItemClass;

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            Writer.WriteStartElement("PAW_Invoice");
            Writer.WriteAttributeString("xsi:type", "paw:Receipt");

            Writer.WriteStartElement("Customer_ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrCustId);//Customer ID should be here = Ptient No
            Writer.WriteEndElement();

            Writer.WriteStartElement("Date");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(InvDate.ToString("MM/dd/yyyy"));//Date 
            Writer.WriteEndElement();

            Writer.WriteStartElement("Invoice_Number");
            Writer.WriteString(StrReference);
            Writer.WriteEndElement();

            Writer.WriteStartElement("Accounts_Receivable_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrARAccount);//Cash Account
            Writer.WriteEndElement();//CreditMemoType

            Writer.WriteStartElement("CreditMemoType");
            Writer.WriteString("TRUE");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString(intRowCount.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("SalesLines");
            Writer.WriteStartElement("SalesLine");

            Writer.WriteStartElement("Quantity");
            Writer.WriteString(Qty.ToString("0.00"));//Doctor Charge
            Writer.WriteEndElement();

            Writer.WriteStartElement("Item_ID");
            Writer.WriteString(ItemCode.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Description");
            Writer.WriteString(ItemDesc.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrSalesGLAccount);
            Writer.WriteEndElement();
            //========================================================
            Writer.WriteStartElement("Tax_Type");
            Writer.WriteString("1");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Amount");
            Writer.WriteString("-" + 0);
            Writer.WriteEndElement();

            Writer.WriteStartElement("Job_ID");
            Writer.WriteString(JobCode.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteEndElement();//LINE
            Writer.WriteEndElement();//LINES

            Writer.WriteEndElement();
            Writer.Close();
        }


        public void CreateXmlToExportLabourItems(double BOMRate,double Qty, string ItemDesc, string ItemCode, string JobCode, string StrSalesGLAccount, string StrReference, int intRowCount, string StrCustId, DateTime InvDate, string StrARAccount,string _Reason)
        {
            int intItemClass;

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            Writer.WriteStartElement("PAW_Invoice");
            Writer.WriteAttributeString("xsi:type", "paw:Receipt");

            Writer.WriteStartElement("Customer_ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrCustId);//Customer ID should be here = Ptient No
            Writer.WriteEndElement();

            Writer.WriteStartElement("Date");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(InvDate.ToString("MM/dd/yyyy"));//Date 
            Writer.WriteEndElement();

            Writer.WriteStartElement("Invoice_Number");
            Writer.WriteString(StrReference);
            Writer.WriteEndElement();

            Writer.WriteStartElement("Accounts_Receivable_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrARAccount);//Cash Account
            Writer.WriteEndElement();//CreditMemoType

            Writer.WriteStartElement("CreditMemoType");
            Writer.WriteString("FALSE");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString(intRowCount.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("SalesLines");
            Writer.WriteStartElement("SalesLine");

            Writer.WriteStartElement("Quantity");
            //Writer.WriteString("1");
            Writer.WriteString(Qty.ToString("0.00"));//Doctor Charge
            Writer.WriteEndElement();

            Writer.WriteStartElement("Item_ID");
            Writer.WriteString(ItemCode.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Description");
            Writer.WriteString(ItemDesc.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrSalesGLAccount);
            Writer.WriteEndElement();
            //========================================================
            Writer.WriteStartElement("Tax_Type");
            Writer.WriteString("1");
            Writer.WriteEndElement();

            //Writer.WriteStartElement("Amount");
            //Writer.WriteString("-" + 0);
            //Writer.WriteEndElement();

            double Amount = Qty * ImportItemUnitCost(ItemCode);
            if (_Reason == "Job Issues")
            {
                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + Amount.ToString().Trim());
                Writer.WriteEndElement();
            }
            else
            {
                Writer.WriteStartElement("Amount");
                Writer.WriteString(Math.Abs(Amount).ToString().Trim());
                Writer.WriteEndElement();
            }

            Writer.WriteStartElement("Job_ID");
            Writer.WriteString(JobCode.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteEndElement();//LINE
            Writer.WriteEndElement();//LINES

            Writer.WriteEndElement();
            Writer.Close();
        }

        public void ImportLabourItems()
        {
           Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();
            //Interop.PeachwServer.Import importer;
            app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword
            // connnect with the correct journal
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerPurchaseOrder);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_JobId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAmount);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml");
            importer.Import();
        }

        public void CreateXmlToExportJobActuals(string BOMID, string SiteID, DataTable DtblDis, clsDataAccess objDataAccess)
        {
            double _EstRev = 0;
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml", System.Text.Encoding.UTF8);

            //Create a Xmal File..................................................................................
            try
            {
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Jobs");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Job");
                Writer.WriteAttributeString("xsi:type", "paw:job");

                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(SiteID);//Customer ID should be here
                Writer.WriteEndElement();

                Writer.WriteStartElement("Distributions");

                foreach (DataRow dr in DtblDis.Rows)
                {
                    objclsDBLPhases = new clsDBLPhases();
                    DataSet _dts = objclsDBLPhases.GetBOQBalance(BOMID, dr["PhaseID"].ToString(), dr["SubPhaseID"].ToString(), objDataAccess);
                    _EstRev = double.Parse(_dts.Tables[0].Rows[0]["IssuedQty"].ToString()) - double.Parse(_dts.Tables[0].Rows[0]["ReturnedQty"].ToString());
                    Writer.WriteStartElement("Distribution");

                    Writer.WriteStartElement("PhaseID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dr["PhaseID"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CostID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dr["SubPhaseID"].ToString());
                    Writer.WriteEndElement();

                    //if (DtblDis.Columns.Contains("Returned"))
                    //{
                    //    Writer.WriteStartElement("EstimatedRevenue");
                    //    Writer.WriteString(((double.Parse(dr["LastUnitCost"].ToString()) * double.Parse(dr["Returned"].ToString())) ).ToString());
                    //    Writer.WriteEndElement();
                    //}
                    //else
                    //{
                        Writer.WriteStartElement("EstimatedRevenue");
                        Writer.WriteString((double.Parse(dr["LastUnitCost"].ToString())*_EstRev).ToString());
                        Writer.WriteEndElement();
                    //}

                    Writer.WriteEndElement();
                }

                Writer.WriteEndElement();

                Writer.Close();
            }
            catch (Exception ex)
            {
                Writer.Close();
                throw ex;
            }
        }

        public void CreateXmlToDeleteJob(string SiteID)
        {
            //Create a Xmal File..................................................................................
            try
            {
                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Jobs");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Job");
                Writer.WriteAttributeString("xsi:type", "paw:job");

                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(SiteID);//Customer ID should be here
                Writer.WriteEndElement();

                Writer.WriteStartElement("UsePhases");
                Writer.WriteString("FALSE");//Date 
                Writer.WriteEndElement();

                Writer.WriteEndElement();

                Writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateXmlToPhases(string PhaseID, bool IsUseCostCodes)
        {
            //Create a Xmal File..................................................................................
            try
            {
                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Phases.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Phases");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Phase");
                Writer.WriteAttributeString("xsi:type", "paw:phase");

                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(PhaseID);//Customer ID should be here
                Writer.WriteEndElement();

                Writer.WriteStartElement("UseCostCodes");
                Writer.WriteString(IsUseCostCodes.ToString().ToUpper());//Date 
                Writer.WriteEndElement();

                Writer.WriteEndElement();

                Writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ImportPhases()
        {
           Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

            app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword                

            //File.Delete(System.Windows.Forms.Application.StartupPath + "\\Phases.xml");
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPhaseList);
            //importer.ClearImportFieldList();
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseHasCost);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_CostType);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_Inactive);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseDescription);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Phases.xml");
            importer.Import();
        }

        public void JobDeleteExport()
        {
            try
            {
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();

                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");//frmMain.sName,frmMain.sPassword
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjJobList);


                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_UsePhases);

                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.

                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Job.xml");
                importer.Import();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteTransaction(string IssueNo)
        {
            try
            {
                //Interop.PeachwServer.Application app;
               Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();
                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");
                string[] recToDel = new string[2];
                recToDel[0] = IssueNo;
                //(Interop.PeachwServer.PeachBusObjects.pboAccount,Interop.PeachwServer.PeachObjectKey.pboKey_ByID, ref recToDel);
                //    clearForm();
                app.DeleteRecord(Interop.PeachwServer.PeachBusObjects.pboInventoryAdjustment,Interop.PeachwServer.PeachObjectKey.pboKey_ByID, ref recToDel);
                //MessageBox.Show("Recorded Deleted Successfully",app.ProductName);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
