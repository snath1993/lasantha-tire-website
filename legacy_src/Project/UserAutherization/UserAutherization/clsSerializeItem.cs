using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace UserAutherization
{
    class clsSerializeItem
    {
        public static string TranScreen = "";

        //private static string _StrWarehouseID = "";
        //public static string StrWarehouseID
        //{
        //    get { return clsSerializeItem._StrWarehouseID; }
        //    set { clsSerializeItem._StrWarehouseID = value; }
        //}

        //private static string _StrWarehouseName = "";
        //public static string StrWarehouseName
        //{
        //    get { return clsSerializeItem._StrWarehouseName; }
        //    set { clsSerializeItem._StrWarehouseName = value; }
        //}

        //private static string _StrSerialItemID = "";
        //public static string StrSerialItemID
        //{
        //    get { return clsSerializeItem._StrSerialItemID; }
        //    set { clsSerializeItem._StrSerialItemID = value; }
        //}

        //public static string StrDescription = "";
        //public static double IntEnterdQty = 0;
        //public static int IntItemClass = 1;


        private static DataTable _tblSerialGen;
        public static DataTable TblSerialGen
        {
            get { return clsSerializeItem._tblSerialGen; }
            set { clsSerializeItem._tblSerialGen = value; }
        }

        private static DataTable _dtsSerialNoList;
        public static DataTable DtsSerialNoList
        {
            get { return clsSerializeItem._dtsSerialNoList; }
            set { clsSerializeItem._dtsSerialNoList = value; }
        }

        private static DataTable _tblLotNos;
        public static DataTable TblLotNos
        {
            get { return clsSerializeItem._tblLotNos; }
            set { clsSerializeItem._tblLotNos = value; }
        }
       
    }
}
