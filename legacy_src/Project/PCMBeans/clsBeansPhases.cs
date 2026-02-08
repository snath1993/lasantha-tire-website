using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace PCMBeans
{
    public class clsBeansPhases
    {
        private bool _IsProjStartDate;
        public bool IsProjStartDate
        {
            get { return _IsProjStartDate; }
            set { _IsProjStartDate = value; }
        }

        private bool _IsProjEndDate;
        public bool IsProjEndDate
        {
            get { return _IsProjEndDate; }
            set { _IsProjEndDate = value; }
        }

        private bool _IsActStartDate;
        public bool IsActStartDate
        {
            get { return _IsActStartDate; }
            set { _IsActStartDate = value; }
        }

        private bool _IsActEndDate;
        public bool IsActEndDate
        {
            get { return _IsActEndDate; }
            set { _IsActEndDate = value; }
        }

        private bool _IsPosted;
        public bool IsPosted
        {
            get { return _IsPosted; }
            set { _IsPosted = value; }
        }

        private string _JobDescription;
        public string JobDescription
        {
            get { return _JobDescription; }
            set { _JobDescription = value; }
        }

        private double _GLAccount;
        public double GLAccount
        {
            get { return _GLAccount; }
            set { _GLAccount = value; }
        }

        private string _SubPhaseID;
        public string SubPhaseID
        {
            get { return _SubPhaseID; }
            set { _SubPhaseID = value; }
        }

        private string _Category;
        public string Category
        {
            get { return _Category; }
            set { _Category = value; }
        }

        private DateTime _StartDate;
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        private DateTime _EndDate;
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }

        private DataTable _dtblDocList;
        public DataTable DtblDocList
        {
            get { return _dtblDocList; }
            set { _dtblDocList = value; }
        }

        private static DataTable _dtblAttachment;
        public static DataTable DtblAttachment
        {
            get { return clsBeansPhases._dtblAttachment; }
            set { clsBeansPhases._dtblAttachment = value; }
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _Code;
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _PhaseID;
        public string PhaseID
        {
            get { return _PhaseID; }
            set { _PhaseID = value; }
        }

        private string _PhaseDesc;
        public string PhaseDesc
        {
            get { return _PhaseDesc; }
            set { _PhaseDesc = value; }
        }

        private bool _Inactive;
        public bool Inactive
        {
            get { return _Inactive; }
            set { _Inactive = value; }
        }

        private string _Units;
        public string Units
        {
            get { return _Units; }
            set { _Units = value; }
        }

        private double _RateIncrease;
        public double RateIncrease
        {
            get { return _RateIncrease; }
            set { _RateIncrease = value; }
        }

        private DataTable _dtblPT;
        public DataTable DtblPT
        {
            get { return _dtblPT; }
            set { _dtblPT = value; }
        }

        private DataTable _dtbl;
        public DataTable Dtbl
        {
            get { return _dtbl; }
            set { _dtbl = value; }
        }

        private DateTime _Date;
        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

        private bool _IsUsePhases;
        public bool IsUsePhases
        {
            get { return _IsUsePhases; }
            set { _IsUsePhases = value; }
        }

        private bool _IsApplyToAll;
        public bool IsApplyToAll
        {
            get { return _IsApplyToAll; }
            set { _IsApplyToAll = value; }
        }
        private double _CompletedPercentage;

        public double CompletedPercentage
        {
            get { return _CompletedPercentage; }
            set { _CompletedPercentage = value; }
        }

        private double _Labor_Burden_Percent;
        public double Labor_Burden_Percent
        {
            get { return _Labor_Burden_Percent; }
            set { _Labor_Burden_Percent = value; }
        }


        private double _Margine;
        public double Margine
        {
            get { return _Margine; }
            set { _Margine = value; }
        }

        private double _Rate;
        public double Rate
        {
            get { return _Rate; }
            set { _Rate = value; }
        }

        private bool _UseCostCode;
        public bool UseCostCode
        {
            get { return _UseCostCode; }
            set { _UseCostCode = value; }
        }

        private string _CostTypeID;
        public string CostTypeID
        {
            get { return _CostTypeID; }
            set { _CostTypeID = value; }
        }

        private string _CostTypeName;
        public string CostTypeName
        {
            get { return _CostTypeName; }
            set { _CostTypeName = value; }
        }

        private string _Address1;
        public string Address1
        {
            get { return _Address1; }
            set { _Address1 = value; }
        }

        private string _Address2;
        public string Address2
        {
            get { return _Address2; }
            set { _Address2 = value; }
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private string _ContactNo;
        public string ContactNo
        {
            get { return _ContactNo; }
            set { _ContactNo = value; }
        }

        private string _StructuralEngineer;
        public string StructuralEngineer
        {
            get { return _StructuralEngineer; }
            set { _StructuralEngineer = value; }
        }

        private string _Architecture;
        public string Architecture
        {
            get { return _Architecture; }
            set { _Architecture = value; }
        }

        private string _BillingMethodID;
        public string BillingMethodID
        {
            get { return _BillingMethodID; }
            set { _BillingMethodID = value; }
        }

        private string _StatusID;
        public string StatusID
        {
            get { return _StatusID; }
            set { _StatusID = value; }
        }

        private string _CustomerID;
        public string CustomerID
        {
            get { return _CustomerID; }
            set { _CustomerID = value; }
        }

        private string _CustomerName;
        public string CustomerName
        {
            get { return _CustomerName; }
            set { _CustomerName = value; }
        }

        private string _Contractor;
        public string Contractor
        {
            get { return _Contractor; }
            set { _Contractor = value; }
        }

        private DateTime _ActualStartDate;
        public DateTime ActualStartDate
        {
            get { return _ActualStartDate; }
            set { _ActualStartDate = value; }
        }

        private DateTime _ActualEndDate;
        public DateTime ActualEndDate
        {
            get { return _ActualEndDate; }
            set { _ActualEndDate = value; }
        }

        private DateTime _EstimateStartDate;
        public DateTime EstimateStartDate
        {
            get { return _EstimateStartDate; }
            set { _EstimateStartDate = value; }
        }

        private DateTime _EstimateEndDate;
        public DateTime EstimateEndDate
        {
            get { return _EstimateEndDate; }
            set { _EstimateEndDate = value; }
        }

        private double _ActualAmt;
        public double ActualAmt
        {
            get { return _ActualAmt; }
            set { _ActualAmt = value; }
        }

        private double _EstimatedAmt;
        public double EstimatedAmt
        {
            get { return _EstimatedAmt; }
            set { _EstimatedAmt = value; }
        }

        private string _ProjectManager;
        public string ProjectManager
        {
            get { return _ProjectManager; }
            set { _ProjectManager = value; }
        }

        private double _RetainingPersentage;
        public double RetainingPersentage
        {
            get { return _RetainingPersentage; }
            set { _RetainingPersentage = value; }
        }

        private string _Supervisor;
        public string Supervisor
        {
            get { return _Supervisor; }
            set { _Supervisor = value; }
        }

        private string _SiteID;
        public string SiteID
        {
            get { return _SiteID; }
            set { _SiteID = value; }
        }

        private string _BOQID;
        public string BOQID
        {
            get { return _BOQID; }
            set { _BOQID = value; }
        }

        private int _RevisionNo;
        public int RevisionNo
        {
            get { return _RevisionNo; }
            set { _RevisionNo = value; }
        }

        private string _BOMID;
        public string BOMID
        {
            get { return _BOMID; }
            set { _BOMID = value; }
        }

        private string _IssueNo;
        public string IssueNo
        {
            get { return _IssueNo; }
            set { _IssueNo = value; }
        }

        private DateTime _EnterDate;
        public DateTime EnterDate
        {
            get { return _EnterDate; }
            set { _EnterDate = value; }
        }

        private string _EnterUser;
        public string EnterUser
        {
            get { return _EnterUser; }
            set { _EnterUser = value; }
        }

        private DateTime _EditDtae;
        public DateTime EditDtae
        {
            get { return _EditDtae; }
            set { _EditDtae = value; }
        }

        private string _EditUser;
        public string EditUser
        {
            get { return _EditUser; }
            set { _EditUser = value; }
        }

        private string _SiteDescription;        
        public string SiteDescription
        {
            get { return _SiteDescription; }
            set { _SiteDescription = value; }
        }

        private string _ReturnNo;
        public string ReturnNo
        {
            get { return _ReturnNo; }
            set { _ReturnNo = value; }
        }


        private static string _USERNAME;
        public static string USERNAME
        {
            get { return clsBeansPhases._USERNAME; }
            set { clsBeansPhases._USERNAME = value; }
        }

        private static DateTime _LOGINDATE;
        public static DateTime LOGINDATE
        {
            get { return clsBeansPhases._LOGINDATE; }
            set { clsBeansPhases._LOGINDATE = value; }
        }
    }
}
