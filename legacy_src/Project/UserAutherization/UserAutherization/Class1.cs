using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace UserAutherization
{
    sealed class Class1
    {
        public static string myvalue = "";
        public static string myvalue1 = "";
        public static string myvalue2 = "";//INVOICE nO
        public static string myvalue3 = "";//TempID
        public static string myvalue4 = "";
        public static string myvalue5 = "";
        public static string myvalue11 = "";
        //public static string formto = "";
        //public static string textto = "";
        public static int flg = 0;
        public static int flgdiscount = 0;
        public static int flg2 = 0;
        public static int flg3 = 0;
        public static int flg4 = 0;
        public static int flg5 = 0;

        public static bool IsInv = false;
        public static bool IsTemp = false;
        internal static string ItemSearch;
        internal static string ItemSearchDesc;


        public string GetText()
        {
            return myvalue;
         
        }
        public string GetText6()
        {
           
            return myvalue4;
        }
        public string GetText7()
        {

            return myvalue5;
        }
        public string GetText11()
        {

            return myvalue11;
        }
        public string GetText2()
        {
            return myvalue;
        }

        public string GetInvoice()
        {
            return myvalue2;
        }

        public string GetTempID()
        {
            return myvalue3;
        }
        public string GetText3()
        {
            return myvalue1;
        }
       
        public string GetText4()
        {
            return myvalue;
        }
        public string GetText5()
        {
            return myvalue;
        }
        public string GetNext(string val)
        {
            myvalue = val;
          
            // this.GetSelected("","");
            flg = 1;
            return val;
        }
        public string GetNext6(string val, string val1, string val2)
        {
            myvalue = val;
            myvalue4 = val1;
            myvalue5 = val2;
            // this.GetSelected("","");
            flg = 1;
            return val;
        }

        public string DicountV(string val)
        {
            myvalue1 = val;
            // this.GetSelected("","");
            flgdiscount = 1;
            return val;
        }
        public string GetInvoiceNo(string ChannelingNO)
        {
            ChannelingNO = ChannelingNO;
            IsInv = true;
            //IsReceiptSearch = 1;
            return ChannelingNO;
        }
        //public string GetText3()
        //{
        //    return myvalue1;
        //}
        //public string GetText4()
        //    {
        //        return myvalue;
        //    }
        //    public string GetText5()
        //    {
        //        return myvalue;
        //    } 
        //public datat GetText2()
        //{
        //    return myvalue;
        //}
        //public string GetSelected(string FocusTo, string ParentForm, string s)
        //{
        //    return s;
        //}

      

        //public string DicountV(string val)
        //{
        //    myvalue1 = val;
        //    // this.GetSelected("","");
        //    flgdiscount = 1;
        //    return val;
        //}



        public string GetNext2(string val)
        {
            myvalue = val;
            flg2 = 1;
            IsInv = false;
            return val;
        }

     

        public string GetTemplateID(string TempID)
        {
            myvalue2 = TempID;
            IsTemp = true;
            return TempID;
        }


        public string GetNext3(string val1)
        {
            myvalue1 = val1;
            flg3 = 1;
            return val1;
        }
        public string GetNext4(string val)
        {
            myvalue = val;
            // this.GetSelected("","");
            flg4 = 1;
            return val;
        }

        public string GetNext5(string val)
        {
            myvalue = val;
            // this.GetSelected("","");
            flg5 = 1;
            return val;
        }
        public string GetNext11(string val)
        {
            myvalue11 = val;
            // this.GetSelected("","");
            
            return val;
        }


        //public string GetNext3(string val)
        //{
        //    myvalue = val;
        //    // this.GetSelected("","");
        //    flg3 = 1;
        //    return val;
        //}

        //public string GetNext4(string val)
        //{
        //    myvalue = val;
        //    // this.GetSelected("","");
        //    flg4 = 1;
        //    return val;
        //}

        //public string GetNext5(string val)
        //{
        //    myvalue = val;
        //    // this.GetSelected("","");
        //    flg5 = 1;
        //    return val;
        //}
    }
}
