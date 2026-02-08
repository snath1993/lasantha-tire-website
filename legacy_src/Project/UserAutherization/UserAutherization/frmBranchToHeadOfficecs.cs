using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;
using Interop.PeachwServer;
using System.IO;
using System.Data.OleDb;
//using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

namespace UserAutherization
{
       
    public partial class frmBranchToHeadOfficecs : Form
    {
        SqlCommand CMD;
        SqlConnection Con;
        SqlDataAdapter DA;
        DataSet DS;
        SqlTransaction Trans;
        string SQL;



        public static string ConnectionString;
        public Array coa;
        string StrInvoiceNo;
        string StrCustomerID;
        string StrDeliveryNoteNos;
        DateTime DtInvoiceDate;
        string StrARAccount;
        double dblNoofDistributions;
        double dblDistributionNo;
        string StrItemID;
        double dblQty;
        string StrDescription;
        string StrGLAcount;
        double dblUnitPrice;
        double dblDiscount;
        double dblAmount;
        double dblDiscountAmount;
        double dblTax1Amount;
        double dblTax2Amount;
        double dblGrossTotal;
        double dblNetTotal;
        DateTime dtCurrentDate;
        string StrTime;
        string StrCurrentuser;
        bool blIsExport;
        string StrCustomerPO;
        string StrUOM;
        string StrJobID;
        string StrSONO;
        string StrLocation;
        string StrTTType1;
        string StrTTType2;
        bool blIsReturn;
        string StrTTType3;
        double dblTax3Amount;
        double dblRemainQty;
        string StrSalesRep;
        double dblCostPrrice;
        string StrPaymentM;
        string StrItemClass;
        string StrItemType;
        bool blIsVoid;
        string StrVoidReson;
        string StrVoidUser;
        string StrComments;
        double dblTax1Rate;
        double dblTax2Rate;
        double dblSubValue;
        bool blIsApplyToSO;


        public frmBranchToHeadOfficecs()
        {
            InitializeComponent();
            setConnectionString();
        }

        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }

        private void frmBranchToHeadOfficecs_Load(object sender, EventArgs e)
        {

        }
        private void  GetAdjustments(SqlConnection Con, SqlTransaction Trans)
        {
            try
            {
                
                //SQL = "SELECT replace(AdjusmentId,',','-')as AdjusmentId,Date,cast(replace(WarehouseId,',','-') as varchar(100))as WarehouseId,WarehouseName, replace(ItemID,',','-')as ItemID,replace(Itemdescription,',','-')as Itemdescription,UnitCost,OnhandQty,AdjustQty,NewQty,ReasonToAdjust,GLAccount from  tblInventoryAdjustment  WHERE  IsExport = 0"+
                //      "update tblInventoryAdjustment set IsExport=1 where IsExport = 0";

                SQL = "SELECT replace(AdjusmentId,',','-')as AdjusmentId,Date,cast(replace(WarehouseId,',','-') as varchar(100))as WarehouseId,WarehouseName, replace(ItemID,',','-')as ItemID,replace(Itemdescription,',','-')as Itemdescription,UnitCost,OnhandQty,AdjustQty,NewQty,ReasonToAdjust,GLAccount from  tblInventoryAdjustment  WHERE Date>='" + dtpFromDate.Text.ToString().Trim() + "' and Date<= '" + dtpToDate.Text.ToString().Trim() + "'";
                //WHERE InvoiceDate>='" + dtpFromDate.Text.ToString().Trim() + "' and InvoiceDate<= '" + dtpToDate.Text.ToString().Trim() + "'";

                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DS = new DataSet();
                DA = new SqlDataAdapter(CMD);
                DA.Fill(DS);
                //Con.Close();

                string FileName = System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Hour.ToString() + "ADJ.csv";
                StreamWriter sw = new StreamWriter(txtselect.Text.ToString() + "\\" + FileName, false);
                int cCount = DS.Tables[0].Columns.Count;
                int iRow = 0;
                foreach (DataColumn dc in DS.Tables[0].Columns)
                {
                    iRow = iRow + 1;
                    sw.Write(dc.ColumnName);
                    if (iRow != cCount)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);


                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    iRow = 0;
                    for (int i = 0; i < cCount; i++)
                    {
                        iRow = iRow + 1;
                        string s = dr[i].ToString(); //Get Current Datarow value
                        long n = 0;
                        bool b = long.TryParse(s, out n);
                        string sWriteValue = string.Empty;
                        if (b == true)
                        {
                            //check value are equal '001=1
                            if (s == n.ToString())
                            {
                                if (i == 4)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = dr[i].ToString();
                            }
                            else
                            {
                                if (i == 2 || i == 4)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = "'" + dr[i].ToString();
                            }
                            //sWriteValue = dr[i].ToString();
                        }
                        else
                        {
                            if (i == 2 || i == 4)
                                sWriteValue = dr[i].ToString() + "_";

                            else sWriteValue = dr[i].ToString();
                        }
                        sw.Write(sWriteValue);
                        if (iRow != cCount)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                }
                sw.Close();
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void GetCustomer(SqlConnection Con, SqlTransaction Trans)
        {
            try
            {
               
               
                //SQL = " SELECT replace(CutomerID,',','-')as CutomerID,replace(CustomerName,',','-')as CustomerName  from  tblCustomerMaster where IsExport = 0"+
                //"update tblCustomerMaster set IsExport=1 where IsExport = 0";

                SQL = " SELECT replace(CutomerID,',','-')as CutomerID,replace(CustomerName,',','-')as CustomerName  from  tblCustomerMaster";
                CMD = new SqlCommand(SQL, Con,Trans);
                CMD.CommandType = CommandType.Text;
                DS = new DataSet();
                DA = new SqlDataAdapter(CMD);
                DA.Fill(DS);

                string FileName = System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Hour.ToString() + "Customer.csv";


                StreamWriter sw = new StreamWriter(txtselect.Text.ToString() + "\\" + FileName, false);

               // StreamWriter sw = new StreamWriter(txtselect.Text.ToString()+"\\"+"CustomerMaster.csv", false);
                int cCount = DS.Tables[0].Columns.Count;
                int iRow = 0;
                foreach (DataColumn dc in DS.Tables[0].Columns)
                {
                    iRow = iRow + 1;
                    sw.Write(dc.ColumnName);
                    if (iRow != cCount)
                    {
                        sw.Write(",");
                    }


                }
                sw.Write(sw.NewLine);


                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    iRow = 0;
                    for (int i = 0; i < cCount; i++)
                    {
                        iRow = iRow + 1;
                        string s = dr[i].ToString(); //Get Current Datarow value
                        long n = 0;
                        bool b = long.TryParse(s, out n);
                        string sWriteValue = string.Empty;
                        if (b == true)
                        {
                            //check value are equal '001=1
                            if (s == n.ToString())
                            {
                                sWriteValue = dr[i].ToString();
                            }
                            else
                            {
                                sWriteValue = "'" + dr[i].ToString();
                            }
                            //sWriteValue = dr[i].ToString();
                        }
                        else
                        {
                            sWriteValue = dr[i].ToString();
                        }
                        sw.Write(sWriteValue);
                        if (iRow != cCount)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                }
                sw.Close();
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void GetAllInvoive(SqlConnection Con, SqlTransaction Trans)
        {
            try
            {
               
                //SQL = " SELECT InvoiceNo, CustomerID, DeliveryNoteNos, InvoiceDate, ARAccount, " +
                //                " NoofDistributions, DistributionNo, ItemID, Qty, replace(Description,',','-')as Description, GLAcount, UnitPrice," +
                //                " Discount, Amount, DiscountAmount, Tax1Amount, Tax2Amount, GrossTotal, NetTotal, " +
                //                " CurrentDate, Time, Currentuser, IsExport, CustomerPO, UOM, " +
                //                " JobID, SONO, Location, TTType1, TTType2, IsReturn, TTType3, Tax3Amount," +
                //                " RemainQty, Tax1Rate, Tax2Rate, SalesRep, CostPrrice, Tax3Rate FROM tblSalesInvoices WHERE IsExport = 0 ORDER BY InvoiceNo "+
                //                " update tblSalesInvoices set IsExport=1 where IsExport = 0";


                 SQL = " SELECT InvoiceNo, CustomerID, DeliveryNoteNos, InvoiceDate, ARAccount, " +
                                " NoofDistributions,DistributionNo,ItemID,isnull(Qty,0) as Qty,replace(Description,',','-')as Description, GLAcount,isnull(UnitPrice,0) as UnitPrice," +
                                " Discount, isnull(Amount,0) as Amount,isnull(DiscountAmount,0) as DiscountAmount,isnull(Tax1Amount,0) as Tax1Amount,isnull(Tax2Amount,0) as Tax2Amount,isnull(GrossTotal,0) as GrossTotal,isnull(NetTotal,0) as NetTotal, " +
                                " CurrentDate, Time, Currentuser, IsExport, CustomerPO, UOM, " +
                                " JobID, SONO, Location, TTType1, TTType2, IsReturn, TTType3,isnull(Tax3Amount,0) as Tax3Amount," +
                                " isnull(RemainQty,0) as RemainQty,isnull(Tax1Rate,0) as Tax1Rate,isnull(Tax2Rate,0) as Tax2Rate, SalesRep,  isnull(CostPrrice,0) as CostPrrice,  isnull(Tax3Rate,0) as Tax3Rate,PaymentM FROM tblSalesInvoices WHERE InvoiceDate>='" + dtpFromDate.Text.ToString().Trim() + "' and InvoiceDate<= '" + dtpToDate.Text.ToString().Trim() + "'";
                CMD = new SqlCommand(SQL, Con,Trans);
                CMD.CommandType = CommandType.Text;
                DS = new DataSet();
                DA = new SqlDataAdapter(CMD);
                DA.Fill(DS);

                string FileName = System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Hour.ToString() + "INV.csv";
                StreamWriter sw = new StreamWriter(txtselect.Text.ToString() + "\\" + FileName, false);


                //StreamWriter sw = new StreamWriter(txtselect.Text.ToString() + "\\" + "CustomerInvoice.csv", false);
                int cCount = DS.Tables[0].Columns.Count;
                int iRow = 0;
                foreach (DataColumn dc in DS.Tables[0].Columns)
                {
                    iRow = iRow + 1;
                    sw.Write(dc.ColumnName);
                    if (iRow != cCount)
                    {
                        sw.Write(",");
                    }


                }
                sw.Write(sw.NewLine);


                //foreach (DataRow dr in DS.Tables[0].Rows)
                //{
                //    iRow = 0;
                //    for (int i = 0; i < cCount; i++)
                //    {
                //        iRow = iRow + 1;
                //        string s = dr[i].ToString(); //Get Current Datarow value
                //        long n = 0;
                //        bool b = long.TryParse(s, out n);
                //        string sWriteValue = string.Empty;
                //        if (b == true)
                //        {
                //            //check value are equal '001=1
                //            if (s == n.ToString())
                //            {
                //                sWriteValue = dr[i].ToString();
                //            }
                //            else
                //            {
                //                sWriteValue = "'" + dr[i].ToString();
                //            }
                //            //sWriteValue = dr[i].ToString();
                //        }
                //        else
                //        {
                //            sWriteValue = dr[i].ToString();
                //        }
                //        sw.Write(sWriteValue);
                //        if (iRow != cCount)
                //        {
                //            sw.Write(",");
                //        }
                //    }
                //    sw.Write(sw.NewLine);

                //}


                //----------------------------
                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    iRow = 0;
                    for (int i = 0; i < cCount; i++)
                    {
                        iRow = iRow + 1;
                        string s = dr[i].ToString(); //Get Current Datarow value
                        long n = 0;
                        bool b = long.TryParse(s, out n);
                        string sWriteValue = string.Empty;
                        if (b == true)
                        {
                            //check value are equal '001=1
                            if (s == n.ToString())
                            {
                                if (i == 27 || i == 36)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = dr[i].ToString();
                            }
                            else
                            {
                                if (i == 27 || i == 36)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = "'" + dr[i].ToString();
                            }
                            //sWriteValue = dr[i].ToString();
                        }
                        else
                        {
                            if (i == 27 || i == 36)
                                sWriteValue = dr[i].ToString() + "_";

                            else sWriteValue = dr[i].ToString();
                        }
                        sw.Write(sWriteValue);
                        if (iRow != cCount)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                }
                //------------------------------
                sw.Close();
              
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void  GetAllCreditNotes(SqlConnection Con, SqlTransaction Trans)
        {
            try
            {
                
                //SQL = " SELECT CustomerID, CreditNo, ReturnDate, LocationID, IsApplyToInvoice, " +
                //                " InvoiceNO, ARAccount, NoofDistribution, DistributionNo, ItemID, isnull(InvoiceQty,0)as InvoiceQty, isnull(ReturnQty,0) as ReturnQty," +
                //                " replace(Description,',','-')as Description , isnull(UOM,'Pcs')as UOM, isnull(UnitPrice,0) as UnitPrice, isnull(Discount,0)as Discount, isnull(Amount,0) as Amount, GL_Account, NBT, " +
                //                " VAT, isnull(GrossTotal,0) as GrossTotal, isnull(GrandTotal,0) as GrandTotal, isnull(ISExport,0)as ISExport, CurrenUser, isnull(IsFullInvReturn,0)as IsFullInvReturn, " +
                //                " JobID, Tax1ID, Tax2ID,Type,SalesRep FROM tblCutomerReturn where ISExport = 0" +
                //                " update tblCutomerReturn set ISExport=1 where ISExport = 0";


                SQL = " SELECT CustomerID, CreditNo, ReturnDate, LocationID, IsApplyToInvoice, " +
                                " InvoiceNO, ARAccount, NoofDistribution, DistributionNo, ItemID, isnull(InvoiceQty,0)as InvoiceQty, isnull(ReturnQty,0) as ReturnQty," +
                                " replace(Description,',','-')as Description , isnull(UOM,'Pcs')as UOM, isnull(UnitPrice,0) as UnitPrice, isnull(Discount,0)as Discount, isnull(Amount,0) as Amount, GL_Account, NBT, " +
                                " VAT, isnull(GrossTotal,0) as GrossTotal, isnull(GrandTotal,0) as GrandTotal, isnull(ISExport,0)as ISExport, CurrenUser, isnull(IsFullInvReturn,0)as IsFullInvReturn, " +
                                " JobID, Tax1ID, Tax2ID,Type,SalesRep FROM tblCutomerReturn where ReturnDate  >= '" + dtpFromDate.Text.ToString().Trim() + "' and  ReturnDate <='" + dtpToDate.Text.ToString().Trim() + "'";

                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DS = new DataSet();
                DA = new SqlDataAdapter(CMD);
                DA.Fill(DS);


                string FileName = System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Hour.ToString() + "CreditNote.csv";
                StreamWriter sw = new StreamWriter(txtselect.Text.ToString() + "\\" + FileName, false);

               // StreamWriter sw = new StreamWriter(txtselect.Text.ToString() + "\\" + "CustomerReturn.csv", false);
                int cCount = DS.Tables[0].Columns.Count;
                int iRow = 0;
                foreach (DataColumn dc in DS.Tables[0].Columns)
                {
                    iRow = iRow + 1;
                    sw.Write(dc.ColumnName);
                    if (iRow != cCount)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
                //foreach (DataRow dr in DS.Tables[0].Rows)
                
                //{
                //    iRow = 0;
                //    for (int i = 0; i < cCount; i++)
                //    {
                //        iRow = iRow + 1;
                //        string s = dr[i].ToString(); //Get Current Datarow value
                //        long n = 0;
                //        bool b = long.TryParse(s, out n);
                //        string sWriteValue = string.Empty;
                //        if (b == true)
                //        {
                //            //check value are equal '001=1
                //            if (s == n.ToString())
                //            {
                //                sWriteValue = dr[i].ToString();
                //            }
                //            else
                //            {
                //                sWriteValue = "'" + dr[i].ToString();
                //            }
                //            //sWriteValue = dr[i].ToString();
                //        }
                //        else
                //        {
                //            sWriteValue = dr[i].ToString();
                //        }
                //        sw.Write(sWriteValue);
                //        if (iRow != cCount)
                //        {
                //            sw.Write(",");
                //        }
                //    }
                //    sw.Write(sw.NewLine);

                //}

                //-------------------------
                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    iRow = 0;
                    for (int i = 0; i < cCount; i++)
                    {
                        iRow = iRow + 1;
                        string s = dr[i].ToString(); //Get Current Datarow value
                        long n = 0;
                        bool b = long.TryParse(s, out n);
                        string sWriteValue = string.Empty;
                        if (b == true)
                        {
                            //check value are equal '001=1
                            if (s == n.ToString())
                            {
                                if (i == 3)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = dr[i].ToString();
                            }
                            else
                            {
                                if (i == 3)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = "'" + dr[i].ToString();
                            }
                            //sWriteValue = dr[i].ToString();
                        }
                        else
                        {
                            if (i == 3)
                                sWriteValue = dr[i].ToString() + "_";

                            else sWriteValue = dr[i].ToString();
                        }
                        sw.Write(sWriteValue);
                        if (iRow != cCount)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                }
                //-----------------------------
                sw.Close();
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnCreateFiles_Click(object sender, EventArgs e)
        {
            if (txtselect.Text.ToString() == string.Empty)
            {
                MessageBox.Show("Please select a file Path");
                return;
            }

            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Trans = Con.BeginTransaction();

                if (chkInvAdjustments.Checked == true)
                {
                    GetAdjustments(Con, Trans);
                }
                if (chkTransfers.Checked == true)
                {

                }
                if (chkCustomerMasters.Checked == true)
                {
                    GetCustomer(Con, Trans);
                }
                if (chkInvoices.Checked == true)
                {
                    GetAllInvoive(Con, Trans);
                }
                if (chkCreditNote.Checked == true)
                {
                    GetAllCreditNotes(Con, Trans);
                }
                if (chkReceipts.Checked == true)
                {
                    Connector ObjConnector = new Connector();
                    ObjConnector.GET_Receipt_JournalOnline(txtselect.Text.ToString(),dtpFromDate.Value,dtpToDate.Value);
                }

                Trans.Commit();
                Con.Close();
                MessageBox.Show("File Generation is Successfull");
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                MessageBox.Show(ex.Message);
                Con.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtselect.Text = folderBrowserDialog1.SelectedPath;
            }
            //saveFileDialog1.RestoreDirectory = true;
            //saveFileDialog1.InitialDirectory = @"c:\";
            //saveFileDialog1.FileName = System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString();
            //saveFileDialog1.Filter = "CSV File (*.CSV)|*.CSV|Excel Files (*.xls)|*.xls|XML Files (*.xml)|*.xml";  //C# Project (*.csproj)|*.csproj|C# Source Files (*.cs)|*.cs|Java File (*.java)|*.java"
            //if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            //    txtselect.Text = saveFileDialog1.FileName;
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
                    }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkTransfers_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}