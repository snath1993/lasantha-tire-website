using System;
using System.Collections.Generic;
using System.Text;

namespace UserAutherization
{
    sealed class ClassDriiDown
    {
        public static string strSupplierReturnDirectNo = "";

        public static string StrTrnasferNoteNO = "";
        public static string PrevGRNNO = "";
        public static string Delivery_note_No = "";
        public static string Invoice_No = "";

        public static bool  SerialIsSearch = false;
        public static string SerialGRNNO = "";
        public static string SerialItemID = "";
        public static string SerialDescription = "";
        public static double ReceivedQty = 0.0;

        public static string SerialLocation = "";
        public static bool IsInvSerch=false;
        //get SupINV No To Supply Returns
        public string GetPrevGRN(string val)
        {
            PrevGRNNO = val;
            return val;
        }
        //=======================================
        //metodfor get checkfor search
        public bool GetCheckSearch(bool val)
        {
            SerialIsSearch = val;
            return val;
        }
        //===================================

        //metod for getting GRn NO

        public string GetGRNNO(string val)
        {
            SerialGRNNO = val;
            return val;
        }

        //=====================================

        //mEtod for get titem iD
        public string GetItemId(string val)
        {
            SerialItemID = val;
            return val;
        }
        //===================================
        public double GetRewceivedQty(double val)
        {
            ReceivedQty = val;
            return val;
        }
        //==================================
        //get location Details=============
        public string GetLocation(string val)
        {
            SerialLocation = val;
            return val;
        }

        //==================================
        public bool  GetCheckSearchToForm()
        {
            return SerialIsSearch;
        }
        //===================================
        public string GetGRNNOToForm()
        {
            return SerialGRNNO;
        }
        //=================================

        public string GetItemIDToForm()
        {
            return SerialItemID;
        }
        //=====================================================
        //Methods for get desription
        public string GetItemDescription(string val)
        {
            SerialDescription = val;
            return val;
        }

        //get Qantity to form===================
        //==============get Prev GRn To Form============================
        public string GetPrevGRNToForm()
        {
            return PrevGRNNO;
        }
        //=========================================

      

        public string GetItemDescriptionToForm()
        {
            return SerialDescription;
        }
        //====================================================

        public double GetReceiveQtyToForm()
        {
            //  ReceivedQty = val;
            return ReceivedQty;
        }
        //======================================

        //Load aserial To the form======================
        public string GetLocationToForm()
        {
            return SerialLocation;
        }
        //==========================================





        public string GetNext2(string val)
        {
            Delivery_note_No = val;
            return val;
        }
        public string GetText2()
        {
            return Delivery_note_No;
        }

        //===========invoice_NO============
        public string GetNext1(string val1)
        {
            Delivery_note_No = val1;
            return val1;
        }

        public string GetText1()
        {
            return Invoice_No;
        }
        //==================================

      
    }
}
