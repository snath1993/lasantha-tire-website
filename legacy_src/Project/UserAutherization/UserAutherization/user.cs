using System;
using System.Collections.Generic;
using System.Text;

namespace UserAutherization
{
    public sealed class user
    {
        public static string userName = "";
        public static DateTime LoginDate = DateTime.Today;
        public static bool isLabList;
        public static bool isChannelligList;
        public static bool isAct;
        public static bool isRun;
        public static bool isAdd;
        public static bool isEdit;
        public static bool isDelete;
        public static bool IsGRNNoAutoGen;
        public static bool IsDNNoAutoGen;
        public static bool IsCINVNoAutoGen;
        public static bool IsSINVNoAutoGen;
        public static bool IsCRTNNoAutoGen;
        public static bool IsSRTNNoAutoGen;
        public static bool IsTaxApplicable;
        public static bool IsTaxOnTax;
        public static bool IsOverGRNQty;
        public static bool IsOverSOQty;
        public static bool IsMinusAllow;
        public static bool IsMultiWhseAllow;
        public static bool IsJOBIssueNOAutoGen;
        public static bool IsJOBReturnNOAutoGen;
        public static bool IsBOQNOAutoGen;
        public static bool IsBOMNOAutoGen;
        public static bool IsGRNEnbl;
        public static bool IsDelNoteEnbl;
        public static bool IsSupRetEnbl;
        public static bool IsSupInvEnbl;
        public static bool IsInvBegBalEnbl;
        public static bool IsWHTransEnbl;
        public static bool IsInvAdjEnbl;
        public static bool IsBranchSynkEnbl;
        public static bool IsInvIssueReturnEnbl;

        public static bool IsDirectINVEnbl;
        public static bool IsInclTaxINVEnbl;
        public static bool IsIndrctINVEnbl;
        public static bool IsPMINVEnbl;
        public static bool IsDirectRtnEnbl;
        public static bool IsIndRtnEnbl;
        public static bool IsJobPreFormsEnbl;
        public static bool IsJobEstimateEnbl;
        public static bool IsJobActualEnbl;
        public static bool IsJobReturnEnbl;
        public static bool IsFGRtransferEnbl;
        public static bool IsJobCloseEnbl;
        public static bool IsBOQEnbl;
        public static bool IsBOMEnbl;
        public static bool IsJobIssueNoteEnbl;
        public static bool IsIncluRtnEnbl;
        public static bool IsMoreThanBOMQty;
        public static bool IsReturnOverSupInv;
        public static bool IsImportBegBal;
        public static bool IsReturnOverCustInv;

        public static DateTime Period_begin_Date;
        public static DateTime Period_End_Date;

        public static string StrDefaultWH;


        public static string ArAccount;
        public static string ApAccount;
        public static string tax1GL;
        public static string tax2GL;
        public static string DiscountGL;
        public static string GLJob;
        public static string SalesGLAccount;
        public static string IssueNoteCurrentAC;
        public static string TaxPayGL1;
        public static string LabChargGL;
        public static string TaxPayGL2;
        public static string AdjustGL;
        public static string RetrnIssuGL;
        public static string FTransGL;
        public static string TransportGL;
        public static string TransporItemID;
        public static string DiscountItemID;
        public static string TaxRec1ID;
        public static string TaxRec2ID;
        public static string TaxPay1ID;
        public static string TaxPay2ID;
        public static string ServiceChargesItemID;
        public static string ServiceChargesGL;
        public static bool  MergeAccUser;
        public static bool IsCostHide;
        public static string ReportPath;
        public static bool InvPrefex;
        public static bool InvPrefexDir;
        internal static bool Ispackage;

        //  IsMinusAllow, MultiWhse, OverGRN, IsTaxApplicable, IsTaxOnTax, OverSO, IsAssignItem

    }
}
