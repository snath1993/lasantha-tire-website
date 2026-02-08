using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//using System.Windows.Forms;

namespace PCMBeans
{
    public class clsBeansFind
    {
        private static DataTable _DataTable;
        public static DataTable DataTable
        {
            get { return _DataTable; }
            set { _DataTable = value; }
        }

        private static string _ReturnValue;
        public static string ReturnValue
        {
            get { return clsBeansFind._ReturnValue; }
            set { clsBeansFind._ReturnValue = value; }
        }

        private static string _ReturnValue2;

        public static string ReturnValue2
        {
            get { return clsBeansFind._ReturnValue2; }
            set { clsBeansFind._ReturnValue2 = value; }
        }

        

    }
}
