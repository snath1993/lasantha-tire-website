using System;
using System.Collections.Generic;
using System.Text;
using PCMDBL;
using Infragistics.Win.UltraWinProgressBar;

namespace PCMBLL
{
    public class clsBBLPTImport
    {
        clsDBLPTImport objclsDBLPTImport;

        public void ImportPhases_List(UltraProgressBar up)
        {
            try
            {
                objclsDBLPTImport = new clsDBLPTImport();
                objclsDBLPTImport.ImportPhases_List(up);
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
                objclsDBLPTImport = new clsDBLPTImport();
                objclsDBLPTImport.ImportVenders_List();
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
                objclsDBLPTImport = new clsDBLPTImport();
                objclsDBLPTImport.ImportSubPhases_List(up);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public void ImportJob_List(UltraProgressBar up)
        {
            try
            {
                objclsDBLPTImport = new clsDBLPTImport();
                objclsDBLPTImport.ImportJob_List(up);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public double ImportItemUnitCost(string ItemID)
        {
            objclsDBLPTImport = new clsDBLPTImport();
            double _LastUnitCost = 0;
            try
            {
                _LastUnitCost = objclsDBLPTImport.ImportItemUnitCost(ItemID);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            return _LastUnitCost;
        }

    }
}
