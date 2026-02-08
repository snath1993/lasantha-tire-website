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
    public partial class frmImportHeadToBranch : Form
    {

        public DSGRN objGRN = new DSGRN();
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



        string StrTransferNo;

        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }
        public frmImportHeadToBranch()
        {
            setConnectionString();
            InitializeComponent();

        }


        private int ExportCreditNote()
        {
            try
            {
                //if (rbCreditNote.Checked == true)
                //{
                string InvCreditAcc = string.Empty;
                string InvDebitAcc = string.Empty;
                SQL = "Select CusretnDrAc,CusretnCrAc from tblDefualtSetting";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                // Con.Open();
                DA.Fill(DS);
                // Con.Close();

                foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    InvCreditAcc = Dr["CusretnDrAc"].ToString();
                    InvDebitAcc = Dr["CusretnCrAc"].ToString();
                }

                SQL = "SELECT DISTINCT CustomerID, CreditNo, ReturnDate, InvoiceNO, ARAccount, NoofDistribution,JobID FROM tblCutomerReturn WHERE ISExport = 0 order by CreditNo";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);


                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                //Writer.WriteStartElement("PAW_Invoice");
                //Writer.WriteAttributeString("xsi:type", "paw:invoice");

                foreach (DataRow Dr in DS.Tables[0].Rows)
                {

                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:invoice");

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["CustomerID"].ToString());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(Dr["CreditNo"].ToString());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Date");
                    Writer.WriteString(Dr["ReturnDate"].ToString());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteString(Dr["ARAccount"].ToString());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("TRUE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(Dr["NoofDistribution"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("SalesLines");

                    SQL = "SELECT DistributionNo, ItemID, InvoiceQty,ReturnQty, Description, UOM, UnitPrice, Discount, Amount, GL_Account, NBT, VAT,ISExport, Tax1ID,Tax2ID,JobID FROM tblCutomerReturn WHERE CreditNo ='" + Dr["CreditNo"].ToString().Trim() + "' AND Status=1";
                    // SQL = "SELECT DistributionNo, ItemID, InvoiceQty,ReturnQty, Description, UOM, UnitPrice, Discount, Amount, GL_Account, NBT, VAT,ISExport, Tax1ID,Tax2ID,JobID FROM tblCutomerReturn WHERE ISExport = 0 order by CreditNo";

                    //WHERE CreditNo ='" + Dr["CreditNo"].ToString().Trim() + "' AND Status=1
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);

                    foreach (DataRow Dr1 in DS.Tables[0].Rows)
                    {
                        //if (Convert.ToDouble(Dr1["ReturnQty"]) > 0)
                        //{
                        Writer.WriteStartElement("SalesLine");

                        Writer.WriteStartElement("InvoiceDistNum");
                        Writer.WriteString(Dr1["DistributionNo"].ToString());
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("-" + Dr1["ReturnQty"].ToString());//Doctor Charge
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(Dr1["ItemID"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(Dr1["Description"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteString(Dr1["GL_Account"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Unit_Price");
                        Writer.WriteString(Dr1["UnitPrice"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(Dr1["Amount"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Job_ID");
                        Writer.WriteString(Dr1["JobID"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AppliedToSO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();
                        //  }
                    }
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();
                }

                Writer.WriteEndElement();//last line
                Writer.Close();
                //  }


                return 1;
            }
            catch (Exception)
            {

                return 0;
            }

        }

        private int ExportCustomerMaster()
        {
            try
            {
                //SQL = "SELECT CutomerID CustomerName from tblCustomerMaster where Satus=1";
                SQL = "SELECT CutomerID,CustomerName from tblCustomerMaster";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerMaster.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;

                Writer.WriteStartElement("PAW_Customers");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    Writer.WriteStartElement("PAW_Customer");
                    Writer.WriteAttributeString("xsi:type", "paw:customer");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["CutomerID"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Name");
                    Writer.WriteString(Dr["CustomerName"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Customer_Type");
                    Writer.WriteString("Branch");
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();
                    // Writer.WriteEndElement();
                }
                Writer.WriteEndElement();
                Writer.Close();

                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }


        private int ExportInventoryAdjustments()
        {
            try
            {
                SQL = "SELECT AdjusmentId,Date,WarehouseId,WarehouseName,ItemID,Itemdescription,UnitCost,AdjustQty,ReasonToAdjust,GLAccount from tblInventoryAdjustment";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InventoyAdjustment.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;

                Writer.WriteStartElement("PAW_Items");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


                foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    Writer.WriteStartElement("PAW_Item");
                    Writer.WriteAttributeString("xsi:type", "paw:item");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["ItemID"].ToString().Trim());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(Dr["AdjusmentId"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date ");
                    Writer.WriteAttributeString("xsi:type", "paw:date");
                    Writer.WriteString(Dr["Date"].ToString());//Date format must be (MM/dd/yyyy)
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("AdjustmentItems");
                    Writer.WriteStartElement("AdjustmentItem");

                    Writer.WriteStartElement("GLSourceAccount ");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["GLAccount"].ToString().Trim());
                    Writer.WriteEndElement();



                    Writer.WriteStartElement("UnitCost");
                    Writer.WriteString(Dr["UnitCost"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString(Dr["AdjustQty"].ToString().Trim());
                    Writer.WriteEndElement();

                    double TotalAmount = 0.00;
                    TotalAmount = Convert.ToDouble(Dr["AdjustQty"]) * Convert.ToDouble(Dr["UnitCost"]);

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(TotalAmount.ToString());
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();//Adjustment Line
                    Writer.WriteEndElement();//Adjustment lines

                }
                Writer.WriteEndElement();
                Writer.Close();

                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }



        private int CreateGRNUploadFile()
        {
            try
            {

                SQL = "SELECT [GRN_NO],[VendorID],[GRNDate],[APAccount],[NoOfDis],[DistributionNo],[ItemID],[Description]" +
                      ",[OrderQty],[ReceiveQty],[GlAccount],[UnitPrice],[Amount],[UOM],[TotalAmount],[NetTotal],[TotalDisRate],[TotalDisAmount]" +
                     "[PODistributionNO] FROM [tblGRNTranTemp]";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);

                //DateTime DTP = Convert.ToDateTime(Dr["GRNDate"].ToString());
                //string Dformat = "MM/dd/yyyy";
                //string GRNDate = DTP.ToString(Dformat);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\GRNExport.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Purchases");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Purchase");
                Writer.WriteAttributeString("xsi:type", "paw:purchase");

                foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    DateTime DTP = Convert.ToDateTime(Dr["GRNDate"].ToString());
                    string Dformat = "MM/dd/yyyy";
                    string GRNDate = DTP.ToString(Dformat);

                    Writer.WriteStartElement("VendorID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["VendorID"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(Dr["GRN_NO"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    // Writer.WriteString(dtpDispatchDate.Value.ToString("MM/dd/yyyy"));//Date 
                    Writer.WriteString(GRNDate);//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(Dr["NoOfDis"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("AP_Account");
                    Writer.WriteString(Dr["APAccount"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("PurchaseLines");
                    Writer.WriteStartElement("PurchaseLine");


                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString(Dr["ReceiveQty"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(Dr["ItemID"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(Dr["Description"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteString("6000");//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Unit_Price");
                    Writer.WriteString(Dr["UnitPrice"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(Dr["Amount"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("AppliedToPO");
                    //Writer.WriteString("TRUE");
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("PO_DistNumber");
                    //Writer.WriteString(dgvSupInvoice.Rows[i].Cells[7].Value.ToString());
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("PO_Number");
                    //// Writer.WriteString(POQuantity.Tables[0].Rows[i].ItemArray[2].ToString().Trim());
                    //Writer.WriteString(AAA[1].ToString().Trim());
                    //Writer.WriteEndElement();
                    Writer.WriteEndElement();//end of line
                    Writer.WriteEndElement();//end of lines
                }
                Writer.WriteEndElement();//End OF Items tag
                Writer.WriteEndElement();//End OF Items tag
                Writer.Close();

                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }



        private int CreateItemUploadXMLFile()
        {
            try
            {

                //SQL = "SELECT dbo.tblItemMaster.ItemID, dbo.tblItemMaster.ItemDescription, dbo.tblItemMaster.ItemClass, dbo.tblItemMaster.UnitPrice, dbo.tblItemMaster.SalesGLAccount, " +
                //      "dbo.tblItemMaster.Categoty, dbo.tblItemMaster.UOM, dbo.tblItemMaster.UnitCost, dbo.tblItemMaster.VendorID, dbo.tblItemMaster.PriceLevel1, " +
                //      "dbo.tblItemMaster.PriceLevel2, dbo.tblItemMaster.PriceLevel3, dbo.tblItemMaster.PriceLevel4, dbo.tblItemMaster.PriceLevel5, dbo.tblItemMaster.PriceLevel6, " +
                //      "dbo.tblItemMaster.PriceLevel7, dbo.tblItemMaster.PriceLevel8, dbo.tblItemMaster.PriceLevel9, dbo.tblItemMaster.PriceLevel10, dbo.tblGRNTran.GRN_NO" +
                //      " FROM dbo.tblGRNTran RIGHT OUTER JOIN" +
                //      "dbo.tblItemMaster ON dbo.tblGRNTran.ItemID = dbo.tblItemMaster.ItemID" +
                //      " WHERE(dbo.tblGRNTran.GRN_NO = '" + GRN_No + "')";



                SQL = "SELECT [ItemID],[ItemDescription],[ItemClass],[UnitPrice],[SalesGLAccount],[Categoty],[UOM],[UnitCost],[VendorID],[PriceLevel1]" +
                     ",[PriceLevel2],[PriceLevel3],[PriceLevel4],[PriceLevel5],[PriceLevel6],[PriceLevel7],[PriceLevel8],[PriceLevel9]," +
                     "[PriceLevel10] FROM [tblTempItemMasterFileUpload]";


                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemMaster.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Items");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    Writer.WriteStartElement("PAW_Item");
                    Writer.WriteAttributeString("xsi:type", "paw:Item");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["ItemID"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(Dr["ItemDescription"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Class");
                    Writer.WriteString(Dr["ItemClass"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Prices");

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "1");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel1"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "2");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel2"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "3");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel3"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "4");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel4"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "5");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel5"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "6");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel6"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "7");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel7"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "8");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel8"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "9");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel9"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "10");
                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(Dr["PriceLevel10"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();// End of Sales Prices


                    Writer.WriteStartElement("Last_Unit_Cost");
                    Writer.WriteString(Dr["UnitCost"].ToString().Trim());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("GL_Sales_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["SalesGLAccount"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Inventory_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("6000-00");//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_COGSSalary_Acct");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("2000-00");//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();// End Item

                }
                Writer.WriteEndElement();//End OF Items tag
                Writer.Close();

                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to export transactions now ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Con = new SqlConnection(ConnectionString);
                //if (rbInvoices.Checked == true)
                //{
                SQL = "SELECT DISTINCT InvoiceNo, CustomerID, InvoiceDate, ARAccount, NoofDistributions, IsExport, CustomerPO, JobID, SONO, Location,Tax1Amount, Tax2Amount FROM tblSalesInvoices WHERE IsExport = 0";
                CMD = new SqlCommand(SQL, Con);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                Con.Open();
                DA.Fill(DS);
                Con.Close();

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerInvoice.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;

                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                foreach (DataRow Dr in DS.Tables[0].Rows)
                {

                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");
                    int NoofDis = 0;

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["CustomerID"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(Dr["InvoiceNo"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteString(Convert.ToDateTime(Dr["InvoiceDate"]).ToString("MM/dd/yyyy").Trim());//Date 
                    Writer.WriteEndElement();

                    NoofDis = Convert.ToInt32(Dr["NoofDistributions"]) + 2;
                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoofDis.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteString(Dr["ARAccount"].ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("FALSE");//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("SalesLines");

                    SQL = "SELECT DistributionNo, ItemID, Qty, Description, GLAcount, UnitPrice, Discount, Amount, DiscountAmount, Tax1Amount, Tax2Amount, GrossTotal, NetTotal, UOM, JobID, SONO, Location, TTType1, TTType2, IsReturn, TTType3, Tax3Amount, RemainQty, InvoiceNo FROM tblSalesInvoices WHERE InvoiceNo ='" + Dr["InvoiceNo"].ToString().Trim() + "'";
                    CMD = new SqlCommand(SQL, Con);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    Con.Open();
                    DA.Fill(DS);
                    Con.Close();
                    foreach (DataRow Dr1 in DS.Tables[0].Rows)
                    {
                        Writer.WriteStartElement("SalesLine");

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString(Dr1["Qty"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(Dr1["ItemID"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(Dr1["Description"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteString(Dr1["GLAcount"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Unit_Price");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(Dr1["UnitPrice"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + Dr1["Amount"].ToString());
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Job_ID");
                        //Writer.WriteString(Dr1["JobID"].ToString());
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Apply_To_Sales_Order");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("SalesOrderDistributionNumber");
                        Writer.WriteString(Dr1["DistributionNo"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Warehouse");
                        Writer.WriteString(Dr1["Location"].ToString());
                        Writer.WriteEndElement();//CreditMemoType

                        //Location
                        Writer.WriteEndElement();// end of sales line

                    }
                    Writer.WriteEndElement();//End oF the Sales lines
                    Writer.WriteEndElement();//End oF the invoice
                }
                Writer.WriteEndElement();//End oF the Sales invoices
                Writer.Close();
                //}
                //else if (rbCreditNote.Checked == true)
                //{
                //string InvCreditAcc = string.Empty;
                //string InvDebitAcc = string.Empty;

                //SQL = "Select CusretnDrAc,CusretnCrAc from tblDefualtSetting";

                //CMD = new SqlCommand(SQL, Con);
                //CMD.CommandType = CommandType.Text;
                //DA = new SqlDataAdapter(CMD);
                //DS = new DataSet();
                //Con.Open();
                //DA.Fill(DS);
                //Con.Close();

                //foreach (DataRow Dr in DS.Tables[0].Rows)
                //{
                //    InvCreditAcc = Dr["CusretnDrAc"].ToString();
                //    InvDebitAcc = Dr["CusretnCrAc"].ToString();
                //}


                //SQL = "SELECT CustomerID, CreditNo, ReturnDate, LocationID, IsApplyToInvoice, InvoiceNO, ARAccount, NoofDistribution, DistributionNo, ItemID, InvoiceQty,ReturnQty, Description, UOM, UnitPrice, Discount, Amount, GL_Account, NBT, VAT, GrossTotal, GrandTotal, ISExport, CurrenUser, IsFullInvReturn,JobID, Tax1ID, Tax2ID FROM tblCutomerReturn WHERE ISExport = 0";
                //CMD = new SqlCommand(SQL, Con);
                //CMD.CommandType = CommandType.Text;
                //DA = new SqlDataAdapter(CMD);
                //DS = new DataSet();
                //Con.Open();
                //DA.Fill(DS);
                //Con.Close();


                //XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml", System.Text.Encoding.UTF8);
                //Writer.Formatting = Formatting.Indented;
                //Writer.WriteStartElement("PAW_Invoices");
                //Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                //Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                //Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                //Writer.WriteStartElement("PAW_Invoice");
                //Writer.WriteAttributeString("xsi:type", "paw:invoice");

                //foreach (DataRow Dr in DS.Tables[0].Rows)
                //{
                //    Writer.WriteStartElement("Customer_ID");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(Dr["CustomerID"].ToString());//Vendor ID should be here = Ptient No
                //    Writer.WriteEndElement();

                //    //if (i == 0)
                //    //{
                //    Writer.WriteStartElement("Invoice_Number");
                //    //Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(Dr["InvoiceNO"].ToString());
                //    Writer.WriteEndElement();
                //    //}                       

                //    Writer.WriteStartElement("Date");
                //    //Writer.WriteAttributeString("xsi:type", "paw:id");  
                //    Writer.WriteString(Dr["ReturnDate"].ToString());//Date 
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Accounts_Receivable_Account");
                //    Writer.WriteString(Dr["ARAccount"].ToString());//Cash Account
                //    Writer.WriteEndElement();//CreditMemoType

                //    Writer.WriteStartElement("CreditMemoType");
                //    Writer.WriteString("TRUE");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Number_of_Distributions");
                //    Writer.WriteString(Dr["NoofDistribution"].ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("InvoiceDistNum");
                //    Writer.WriteString(Dr["DistributionNo"].ToString());
                //    Writer.WriteEndElement();


                //    Writer.WriteStartElement("Quantity");
                //    Writer.WriteString("-" + Dr["ReturnQty"].ToString());//Doctor Charge
                //    Writer.WriteEndElement();


                //    Writer.WriteStartElement("Item_ID");
                //    Writer.WriteString(Dr["ItemID"].ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Description");
                //    Writer.WriteString(Dr["Description"].ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("GL_Account");
                //    //Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(Dr["GL_Account"].ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Tax_Type");
                //    Writer.WriteString("1");//Doctor Charge
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Unit_Price");
                //    //Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(Dr["UnitPrice"].ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Amount");
                //    //Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(Dr["Amount"].ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Job_ID");
                //    Writer.WriteString(Dr["JobID"].ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("AppliedToSO");
                //    Writer.WriteString("FALSE");
                //    Writer.WriteEndElement();
                //}
                //Writer.WriteEndElement();//last line
                //Writer.Close();
                //}
                MessageBox.Show("Invoice Export Successfully Completed", "Invoice & Return Export");
            }
        }
        private int SaveCustomerInvoices(string FilePath)
        {

            try
            {
                Trans = Con.BeginTransaction();
                Con.Open();


                string strCustomerId = string.Empty;
                string strInvoiceNo = string.Empty;
                string strSalesOrderNo = string.Empty;
                DateTime dtDate = DateTime.MinValue;
                bool bClosed = false;
                string strCustomerPO = string.Empty;
                string strARAccount = string.Empty;
                int intNoOfDis = 0;
                int intDisNo = 0;
                double dblQuantity = 0;
                string strItemId = string.Empty;
                string strSODescription = string.Empty;
                string strGLAccount = string.Empty;
                double dblPrice = 0;
                string strTaxType = string.Empty;
                string strUM = string.Empty;
                string strJobID = string.Empty;
                double dblAmount = 0;
                //XmlTextReader xmlTxtReader = new XmlTextReader("C:\\PBSS\\CustomerInvoice.xml");
                XmlTextReader xmlTxtReader = new XmlTextReader(FilePath);
                while (xmlTxtReader.Read())
                {
                    if (xmlTxtReader.NodeType == XmlNodeType.Element)
                    {
                        string TagName = xmlTxtReader.Name;
                        switch (TagName)
                        {
                            case "Customer_ID":
                                strCustomerId = xmlTxtReader.ReadString();
                                break;
                            case "Invoice_Number":
                                strInvoiceNo = xmlTxtReader.ReadString();
                                break;
                            case "Date":
                                dtDate = Convert.ToDateTime(xmlTxtReader.ReadString());
                                break;
                            case "Number_of_Distributions":
                                intNoOfDis = Convert.ToInt32(xmlTxtReader.ReadString());
                                break;
                            case "Accounts_Receivable_Account":
                                strARAccount = xmlTxtReader.ReadString();
                                break;
                            case "SalesOrderDistributionNumber":
                                intDisNo = Convert.ToInt32(xmlTxtReader.ReadString());
                                break;
                            case "Quantity":
                                dblQuantity = Convert.ToDouble(xmlTxtReader.ReadString());
                                break;
                            case "Item_ID":
                                strItemId = Convert.ToString(xmlTxtReader.ReadString());
                                break;
                            case "Description":
                                strSODescription = Convert.ToString(xmlTxtReader.ReadString());
                                break;
                            case "GL_Account":
                                strGLAccount = Convert.ToString(xmlTxtReader.ReadString());
                                break;
                            case "Unit_Price":
                                dblPrice = Convert.ToDouble(xmlTxtReader.ReadString());
                                break;
                            case "Tax_Type":
                                strTaxType = Convert.ToString(xmlTxtReader.ReadString());
                                break;
                            case "UM_ID":
                                strUM = Convert.ToString(xmlTxtReader.ReadString());
                                break;
                            case "Amount":
                                dblAmount = Convert.ToDouble(xmlTxtReader.ReadString());
                                break;
                            case "Job_ID":
                                strJobID = Convert.ToString(xmlTxtReader.ReadString());
                                break;
                            default:
                                break;

                        }
                    }
                    else if (xmlTxtReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (xmlTxtReader.Name == "SalesLine")
                        {

                            //if (SaveSalesInvoice(strInvoiceNo, strCustomerId, strSalesOrderNo, dtDate, bClosed, strCustomerPO, strARAccount, intNoOfDis, intDisNo, dblQuantity, strItemId, strSODescription, strGLAccount, dblPrice, strTaxType, strUM, dblAmount) == 0)
                            //{
                            //    Trans.Rollback();
                            //    return 0;
                            //}

                        }

                    }
                }
                Trans.Commit();
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                return 1;
            }
            catch (Exception)
            {

                Trans.Rollback();
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                return 0;
            }

        }

        //sanjeewa added this code segments to check wether this invoice number is already exixts

        public void DeleteItemTemp()
        {
            try
            {
                SQL = "DELETE FROM [tblTempItemMasterFileUpload]";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteGRNTemp()
        {
            try
            {
                SQL = "DELETE FROM [tblGRNTranTemp]";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int IsItemExistsInItemWise(string StrItemID, string WarehouseID)
        {
            try
            {

                SQL = "Select ItemId from tblItemWhse where ItemId ='" + StrItemID + "'and WhseId='" + WarehouseID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                // Con.Open();
                DA.Fill(DS);
                // Con.Close();
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        public int IsItemExists(string StrItemID)
        {
            try
            {

                SQL = "Select ItemID from tblItemMaster where ItemID='" + StrItemID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                // Con.Open();
                DA.Fill(DS);
                // Con.Close();
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public int IsTransferExists(string StrItemID, double IntLineNo)
        {
            try
            {
                SQL = "Select GRN_NO from tblGRNTran where GRN_NO='" + StrItemID + "' and DistributionNo='" + IntLineNo + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                // Con.Open();
                DA.Fill(DS);
                // Con.Close();
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }


        public int IsCustomerExists(string CustomerID)
        {
            try
            {
                SQL = "Select CutomerID from tblCustomerMaster where CutomerID='" + CustomerID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }


        public int IsCreditNoteExists(string CreditNoteNo, string DistributionNo)
        {
            try
            {
                SQL = "Select CreditNo from tblCutomerReturn where CreditNo='" + CreditNoteNo + "' and DistributionNo='" + DistributionNo + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        public int SaveAdjustments(string StrAdjusmentId, DateTime DTInvDate, string StrWarehouseId, string StrWarehouseName, string StrItemID, string StrItemdescription, double dblUnitCost, double dblOnhandQty, double dblAdjustQty, double dblNewQty, string StrReasonToAdjust, string StrGLAccount)
        {
            try
            {
                string SQL = "INSERT INTO tblInventoryAdjustment(AdjusmentId,Date,WarehouseId,WarehouseName,ItemID,Itemdescription,UnitCost,OnhandQty,AdjustQty,NewQty,ReasonToAdjust,GLAccount) VALUES('" + StrAdjusmentId + "','" + DTInvDate + "','" + StrWarehouseId + "','" + StrWarehouseName + "','" + StrItemID + "','" + StrItemdescription + "','" + dblUnitCost + "','" + dblOnhandQty + "','" + dblAdjustQty + "','" + dblNewQty + "','" + StrReasonToAdjust + "','" + StrGLAccount + "')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        //===========================
        public int SaveCustomerMaster(string StrCusid, string StrCusname)
        {
            try
            {
                string SQL = "INSERT INTO tblCustomerMaster(CutomerID,CustomerName) VALUES('" + StrCusid + "','" + StrCusname + "')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        //============================================================

        public int SaveGRN(string StrTrnasferNo, string StrFromW, string StrToW, DateTime TRansDate, double dblNetValue, string StrItemID, string StrDescription, string StrGLAccount, double dblQuantity, double dblUnitcost, double dblLineNO, double NOOFDis)
        {
            try
            {
                string SQL = "INSERT INTO [tblGRNTran]([GRN_NO],[VendorID],[PONos],[GRNDate],[APAccount],[NoOfDis],[DistributionNo],[ItemID]" +
          " ,[Description],[OrderQty],[ReceiveQty],[GlAccount],[UnitPrice],[Amount],[UOM],[TotalAmount],[CurrentDate]" +
           ",[Time],[CurrentUser],[Dupliate],[ISGRNFinished],[CustomerSO],[WareHouseID],[LineDiscountRate]" +
           ",[NetTotal],[TotalDisRate],[TotalDisAmount],[Tax1],[Tax2],[Tax1Amount],[Tax2Amount],[TaxRate],[TaxRate1]" +
           ",[CurrencyName],[CreencyRate],[ConvertedAmount],[PODistributionNO])" +
            "VALUES('" + StrTrnasferNo + "'," +
                    " '00001','Direct','" + Convert.ToDateTime(TRansDate).ToString("MM/dd/yyyy") + "','8000-03'," +
                    " '"+ NOOFDis +"','" + dblLineNO + "','" + StrItemID + "','" + StrDescription + "'," +
                    " '" + dblQuantity + "','" + dblQuantity + "','" + StrGLAccount + "','" + dblUnitcost + "','" + dblQuantity * dblUnitcost + "'," +
                    " 'Pcs','" + dblNetValue + "','" + Convert.ToDateTime(TRansDate).ToString("MM/dd/yyyy") + "'," +
                    " 'Time','CurrentUser','0','1'," +
                    " 'NO','" + StrToW + "','0','" + dblNetValue + "','0','0'," +
                    " 'NO','NO','0','0','0','0','Rs','1','1','0')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }
        public int SaveGRNTemp(string StrTrnasferNo, string StrFromW, string StrToW, DateTime TRansDate, double dblNetValue, string StrItemID, string StrDescription,string StrGLAccount,double dblQuantity, double dblUnitcost, double dblLineNO,double NOOFDis)
        {
            try
            {
                string SQL = "INSERT INTO [tblGRNTranTemp]([GRN_NO],[VendorID],[PONos],[GRNDate],[APAccount],[NoOfDis],[DistributionNo],[ItemID]" +
          " ,[Description],[OrderQty],[ReceiveQty],[GlAccount],[UnitPrice],[Amount],[UOM],[TotalAmount],[CurrentDate]" +
           ",[Time],[CurrentUser],[Dupliate],[ISGRNFinished],[CustomerSO],[WareHouseID],[LineDiscountRate]" +
           ",[NetTotal],[TotalDisRate],[TotalDisAmount],[Tax1],[Tax2],[Tax1Amount],[Tax2Amount],[TaxRate],[TaxRate1]" +
           ",[CurrencyName],[CreencyRate],[ConvertedAmount],[PODistributionNO])" +
            "VALUES('" + StrTrnasferNo + "'," +
                    " '00001','Direct','" + Convert.ToDateTime(TRansDate).ToString("MM/dd/yyyy") + "','8000-03'," +
                    " '" + NOOFDis + "','" + dblLineNO + "','" + StrItemID + "','" + StrDescription + "'," +
                    " '" + dblQuantity + "','" + dblQuantity + "','" + StrGLAccount + "','" + dblUnitcost + "','" + dblQuantity * dblUnitcost + "'," +
                    " 'Pcs','" + dblNetValue + "','" + Convert.ToDateTime(TRansDate).ToString("MM/dd/yyyy") + "'," +
                    " 'Time','CurrentUser','0','1'," +
                    " 'NO','" + StrToW + "','0','" + dblNetValue + "','0','0'," +
                    " 'NO','NO','0','0','0','0','Rs','1','1','0')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }



        public int SaveItemMasterTemp(string StrItemID, string StrItemDescription, string StrItemClass, double dblUnitPrice, string StrSalesGLAccount, string StrCategoty,
          string StrUOM, double dblUnitCost, string StrVendorID, double dblPriceLevel1, double dblPriceLevel2, double dblPriceLevel3, double dblPriceLevel4,
          double dblPriceLevel5, double dblPriceLevel6, double dblPriceLevel7, double dblPriceLevel8, double dblPriceLevel9, double dblPriceLevel10)
        {
            try
            {
                string SQL = "INSERT INTO [tblTempItemMasterFileUpload]([ItemID],[ItemDescription],[ItemClass],[UnitPrice],[SalesGLAccount],[Categoty],[UOM]" +
                ",[UnitCost],[VendorID],[PriceLevel1],[PriceLevel2],[PriceLevel3],[PriceLevel4],[PriceLevel5],[PriceLevel6]" +
                ",[PriceLevel7],[PriceLevel8],[PriceLevel9],[PriceLevel10])VALUES('" + StrItemID + "','" + StrItemDescription + "'," +
                "'" + StrItemClass + "', '" + dblUnitPrice + "', '" + StrSalesGLAccount + "', '" + StrCategoty + "','" + StrUOM + "', '" + dblUnitCost + "'," +
                "'" + StrVendorID + "', '" + dblPriceLevel1 + "', '" + dblPriceLevel2 + "', '" + dblPriceLevel3 + "', '" + dblPriceLevel4 + "'," +
                "'" + dblPriceLevel5 + "', '" + dblPriceLevel6 + "', '" + dblPriceLevel7 + "', '" + dblPriceLevel8 + "', '" + dblPriceLevel9 + "', '" + dblPriceLevel10 + "')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }


        //===================================following code segment inser items into item master==================
        public int SaveItemMaster(string StrItemID, string StrItemDescription, string StrItemClass, double dblUnitPrice, string StrSalesGLAccount, string StrCategoty,
            string StrUOM, double dblUnitCost, string StrVendorID, double dblPriceLevel1, double dblPriceLevel2, double dblPriceLevel3, double dblPriceLevel4,
            double dblPriceLevel5, double dblPriceLevel6, double dblPriceLevel7, double dblPriceLevel8, double dblPriceLevel9, double dblPriceLevel10)
        {
            try
            {
                string SQL = "INSERT INTO [tblItemMaster]([ItemID],[ItemDescription],[ItemClass],[UnitPrice],[SalesGLAccount],[Categoty],[UOM]" +
                ",[UnitCost],[VendorID],[PriceLevel1],[PriceLevel2],[PriceLevel3],[PriceLevel4],[PriceLevel5],[PriceLevel6]" +
                ",[PriceLevel7],[PriceLevel8],[PriceLevel9],[PriceLevel10])VALUES('" + StrItemID + "','" + StrItemDescription + "'," +
                "'" + StrItemClass + "', '" + dblUnitPrice + "', '" + StrSalesGLAccount + "', '" + StrCategoty + "','" + StrUOM + "', '" + dblUnitCost + "'," +
                "'" + StrVendorID + "', '" + dblPriceLevel1 + "', '" + dblPriceLevel2 + "', '" + dblPriceLevel3 + "', '" + dblPriceLevel4 + "'," +
                "'" + dblPriceLevel5 + "', '" + dblPriceLevel6 + "', '" + dblPriceLevel7 + "', '" + dblPriceLevel8 + "', '" + dblPriceLevel9 + "', '" + dblPriceLevel10 + "')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }

        public int InsertItemwise(string StrItemID, string StrItemDescription, string StrItemClass, double dblUnitPrice, string StrSalesGLAccount, string StrCategoty,
         string StrUOM, double dblUnitCost, string StrVendorID, double dblPriceLevel1, double dblPriceLevel2, double dblPriceLevel3, double dblPriceLevel4,
         double dblPriceLevel5, double dblPriceLevel6, double dblPriceLevel7, double dblPriceLevel8, double dblPriceLevel9, double dblPriceLevel10, string WarehouseID)
        {
            try
            {

                string SQL = "INSERT INTO [tblItemWhse]([WhseId],[WhseName],[ItemId],[ItemDis],[QTY]" +
                          ",[UOM],[TraDate],[UnitCost],[TranType],[TotalCost],[OPBQtry])" +
                         " VALUES('" + WarehouseID + "','','" + StrItemID + "','" + StrItemDescription + "','0','" + StrUOM + "'," +
                          "'" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + dblUnitCost + "','OpbBal','0','0')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }


        public int UpdateItemMaster(string StrItemID, string StrItemDescription, string StrItemClass, double dblUnitPrice, string StrSalesGLAccount, string StrCategoty,
          string StrUOM, double dblUnitCost, string StrVendorID, double dblPriceLevel1, double dblPriceLevel2, double dblPriceLevel3, double dblPriceLevel4,
          double dblPriceLevel5, double dblPriceLevel6, double dblPriceLevel7, double dblPriceLevel8, double dblPriceLevel9, double dblPriceLevel10)
        {
            try
            {
                string SQL = "UPDATE [tblItemMaster] SET [ItemDescription] = '" + StrItemDescription + "',[ItemClass] = '" + StrItemClass + "'" +
             ",[UnitPrice] ='" + dblUnitPrice + "',[SalesGLAccount] = '" + StrSalesGLAccount + "' ,[Categoty] ='" + StrCategoty + "',[UOM] = '" + StrUOM + "'" +
             ",[UnitCost] = '" + dblUnitCost + "',[VendorID] = '" + StrVendorID + "',[PriceLevel1] = '" + dblPriceLevel1 + "',[PriceLevel2] = '" + dblPriceLevel2 + "'" +
             ",[PriceLevel3] = '" + dblPriceLevel3 + "',[PriceLevel4] = '" + dblPriceLevel4 + "',[PriceLevel5] = '" + dblPriceLevel5 + "'" +
            ",[PriceLevel6] = '" + dblPriceLevel6 + "',[PriceLevel7] = '" + dblPriceLevel7 + "',[PriceLevel8] = '" + dblPriceLevel8 + "',[PriceLevel9] = '" + dblPriceLevel9 + "'" +
              ",[PriceLevel10] = '" + dblPriceLevel10 + "' WHERE  [ItemID]='" + StrItemID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }
        public int SaveItemActivity(string TranNo, DateTime TransDate, string ItemID, double Qty, double Cost, string WH, double SellingPrice)
        {
            try
            {
                string SQL = "INSERT INTO tbItemlActivity(DocType, TranNo, TransDate, TranType, DocReference, ItemID, Qty, UnitCost," +
                    " TotalCost, WarehouseID, SellingPrice) VALUES('4', '" + TranNo + "','" + Convert.ToDateTime(TransDate).ToString("MM/dd/yyyy") + "'," +
                    " 'Invoice','0','" + ItemID + "'," + Qty + ", " + Cost + "," + Qty * Cost + ",'" + WH + "'," + SellingPrice + ")";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int SaveItemActivityAdjustment(string TranNo, DateTime TransDate, string ItemID, double Qty, double Cost, string WH)
        {
            try
            {


                bool DocRef = false;
                // int DocRef = 0;
                if (Qty > 0)
                {
                    DocRef = true;
                }
                string SQL = "INSERT INTO tbItemlActivity(DocType, TranNo, TransDate, TranType, DocReference, ItemID, Qty, UnitCost," +
                    " TotalCost, WarehouseID, SellingPrice) VALUES('6', '" + TranNo + "','" + Convert.ToDateTime(TransDate).ToString("MM/dd/yyyy") + "'," +
                    " 'InvAdjust','" + DocRef + "','" + ItemID + "'," + Qty + ", " + Cost + "," + Qty * Cost + ",'" + WH + "','0')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }


        public int SaveItemActivityCreditNote(string TranNo, DateTime TransDate, string ItemID, double Qty, double Cost, string WH)
        {
            try
            {
                string SQL = "INSERT INTO tbItemlActivity(DocType, TranNo, TransDate, TranType, DocReference, ItemID, Qty, UnitCost," +
                    " TotalCost, WarehouseID, SellingPrice) VALUES('2', '" + TranNo + "','" + Convert.ToDateTime(TransDate).ToString("MM/dd/yyyy") + "'," +
                    " 'Grn-Tran','1','" + ItemID + "'," + Qty + ", " + Cost + "," + Qty * Cost + ",'" + WH + "','0')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }
        public int UpdateOnHandQtyINV(string ItemID, double Qty, string WH)
        {
            try
            {
                string SQL = "UPDATE tblItemWhse SET QTY=QTY-" + Qty + " WHERE WhseId='" + WH + "' AND ItemId='" + ItemID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }


        public int UpdateOnHandAdjustment(string ItemID, double Qty, string WH)
        {
            try
            {
                if (Qty < 0)
                {
                    string SQL = "UPDATE tblItemWhse SET QTY=QTY -" + Math.Abs(Qty) + " WHERE WhseId='" + WH + "' AND ItemId='" + ItemID + "'";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    CMD.ExecuteNonQuery();
                    return 1;
                }

                if (Qty > 0)
                {
                    string SQL = "UPDATE tblItemWhse SET QTY=QTY+" + Qty + " WHERE WhseId='" + WH + "' AND ItemId='" + ItemID + "'";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    CMD.ExecuteNonQuery();
                    return 1;

                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int UpdateOnHandQtyCreditNote(string ItemID, double Qty, string WH)
        {
            try
            {
                string SQL = "UPDATE tblItemWhse SET QTY=QTY+" + Qty + " WHERE WhseId='" + WH + "' AND ItemId='" + ItemID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }



        //public int PrintTransferNote(string StrTransNo)
        //{
        //    try
        //    {

        //        objGRN.Clear();
        //        string SQL = "Select * from tblGRNTran where GRN_NO = '" + StrTransNo + "'";
        //        CMD = new SqlCommand(SQL, Con, Trans);
        //        CMD.CommandType = CommandType.Text;
        //        CMD.ExecuteNonQuery();


        //        string SQL = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
        //        CMD = new SqlCommand(SQL, Con, Trans);
        //        CMD.CommandType = CommandType.Text;
        //        CMD.ExecuteNonQuery();
        //        return 1;


        //        frmDeiveryNotePrint frm = new frmDeiveryNotePrint(this);
        //        frm.Show();
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //    }
        //}






        private DataSet Get_Invoices_From_CSV_To_Dataset(string FileName)
        {
            DataSet dsExcel = new DataSet();
            try
            {

                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\PBSS\\;Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT * FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dsExcel;

            }

        }

        private DataSet Get_Adjustments_From_CSV_To_Dataset(string FileName)
        {
            DataSet dsExcel = new DataSet();
            try
            {
                // FillGL(dsExcel);
                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\PBSS\\;Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT AdjusmentId,Date,WarehouseId as WarehouseId,WarehouseName,ItemID,Itemdescription,UnitCost,OnhandQty,AdjustQty,NewQty,ReasonToAdjust,GLAccount FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return dsExcel;
            }
        }


        private DataSet Get_CustomerMaster_From_CSV_To_Dataset(string FileName)
        {
            DataSet dsExcel = new DataSet();
            try
            {
                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\PBSS\\;Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT * FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dsExcel;

            }

        }

        private DataSet Get_CreditNotes_From_CSV_To_Dataset(string FileName, string StrFDirestory)
        {
            DataSet dsExcel = new DataSet();
            try
            {
                //string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\PBSS\\;Extended Properties='text;HDR=YES;FMT=Delimited'";
                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + StrFDirestory + ";Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT * FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dsExcel;

            }

        }
        private int UpdateStatus()
        {
            try
            {
                SQL = "UPDATE tblSalesInvoices SET Status = 2 WHERE Status = 1";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {

                return 0;
            }
        }

        private int UpdateStatusCredit()
        {
            try
            {
                SQL = "UPDATE    tblCutomerReturn SET Status = 2 WHERE Status = 1";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int UpdateCustomerMaster()
        {
            try
            {
                SQL = "UPDATE tblCustomerMaster SET Status = 2 WHERE Status = 1";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {

                return 0;
            }
        }


        private void btnImports_Click(object sender, EventArgs e)
        {

            DataSet dsInvoiceList = new DataSet();
            string FilePath = string.Empty;
            string FileName = string.Empty;
            string FDirectory = string.Empty;
            OFD.InitialDirectory = txtselect.Text.ToString().Trim();


            OFD.Filter = "csv files (*.csv)|*.csv";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FilePath = OFD.FileName;
                FileName = Path.GetFileName(FilePath);
                FDirectory = Path.GetDirectoryName(FilePath);
                dsInvoiceList = Get_CreditNotes_From_CSV_To_Dataset(FileName, FDirectory);
            }
            if (dsInvoiceList.Tables.Count == 0)
            {
                MessageBox.Show("can not file the invoice data", "No data in csv file");
                return;
            }
            if (MessageBox.Show("Do you want to update now ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Trans = Con.BeginTransaction();


                DeleteItemTemp();
                DeleteGRNTemp();
                foreach (DataRow Dr in dsInvoiceList.Tables[0].Rows)
                {
                    //Insert Item Master

                    //if(Dr["ToWhseId"].ToString().Replace("_", "").Trim()!="K2")
                    //{
                    //        MessageBox.Show("This", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //        return;
                    //}

                    //SaveItemMasterTemp

                    if (SaveItemMasterTemp(Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["ItemDis"].ToString().Replace("'", "").Trim(), Dr["ItemClass"].ToString(),
                     Convert.ToDouble(Dr["UnitPrice"]), Dr["SalesGLAccount"].ToString(), Dr["Categoty"].ToString(), Dr["UOM"].ToString(), Convert.ToDouble(Dr["UnitCost"]), Dr["VendorID"].ToString(), Convert.ToDouble(Dr["PriceLevel1"]),
                     Convert.ToDouble(Dr["PriceLevel2"]), Convert.ToDouble(Dr["PriceLevel3"]), Convert.ToDouble(Dr["PriceLevel4"]), Convert.ToDouble(Dr["PriceLevel5"]), Convert.ToDouble(Dr["PriceLevel6"]),
                     Convert.ToDouble(Dr["PriceLevel7"]), Convert.ToDouble(Dr["PriceLevel8"]), Convert.ToDouble(Dr["PriceLevel9"]), Convert.ToDouble(Dr["PriceLevel10"])) == 0)
                    {
                        MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Trans.Rollback();
                        return;
                    }


                    if (IsItemExists(Dr["ItemID"].ToString().Replace("_", "").Trim()) == 1)
                    {

                        if (UpdateItemMaster(Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["ItemDis"].ToString().Replace("'", "").Trim(), Dr["ItemClass"].ToString(),
                        Convert.ToDouble(Dr["UnitPrice"]), Dr["SalesGLAccount"].ToString(), Dr["Categoty"].ToString(), Dr["UOM"].ToString(),
                        Convert.ToDouble(Dr["UnitCost"]), Dr["VendorID"].ToString(), Convert.ToDouble(Dr["PriceLevel1"]),
                        Convert.ToDouble(Dr["PriceLevel2"]), Convert.ToDouble(Dr["PriceLevel3"]), Convert.ToDouble(Dr["PriceLevel4"]),
                        Convert.ToDouble(Dr["PriceLevel5"]), Convert.ToDouble(Dr["PriceLevel6"]),
                        Convert.ToDouble(Dr["PriceLevel7"]), Convert.ToDouble(Dr["PriceLevel8"]), Convert.ToDouble(Dr["PriceLevel9"]), Convert.ToDouble(Dr["PriceLevel10"])) == 0)
                        {
                            MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Trans.Rollback();
                            return;
                        }

                    }
                    else if (SaveItemMaster(Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["ItemDis"].ToString().Replace("'", "").Trim(), Dr["ItemClass"].ToString(),
                     Convert.ToDouble(Dr["UnitPrice"]), Dr["SalesGLAccount"].ToString(), Dr["Categoty"].ToString(), Dr["UOM"].ToString(), Convert.ToDouble(Dr["UnitCost"]), Dr["VendorID"].ToString(), Convert.ToDouble(Dr["PriceLevel1"]),
                     Convert.ToDouble(Dr["PriceLevel2"]), Convert.ToDouble(Dr["PriceLevel3"]), Convert.ToDouble(Dr["PriceLevel4"]), Convert.ToDouble(Dr["PriceLevel5"]), Convert.ToDouble(Dr["PriceLevel6"]),
                     Convert.ToDouble(Dr["PriceLevel7"]), Convert.ToDouble(Dr["PriceLevel8"]), Convert.ToDouble(Dr["PriceLevel9"]), Convert.ToDouble(Dr["PriceLevel10"])) == 0)
                    {
                        MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Trans.Rollback();
                        return;
                    }
                    if (IsItemExistsInItemWise(Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["ToWhseId"].ToString().Replace("_", "").Trim()) == 1)
                    {

                    }
                    else
                    {
                        if (InsertItemwise(Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["ItemDis"].ToString().Replace("'", "").Trim(), Dr["ItemClass"].ToString(),
                         Convert.ToDouble(Dr["UnitPrice"]), Dr["SalesGLAccount"].ToString(), Dr["Categoty"].ToString(), Dr["UOM"].ToString(), Convert.ToDouble(Dr["UnitCost"]), Dr["VendorID"].ToString(), Convert.ToDouble(Dr["PriceLevel1"]),
                         Convert.ToDouble(Dr["PriceLevel2"]), Convert.ToDouble(Dr["PriceLevel3"]), Convert.ToDouble(Dr["PriceLevel4"]), Convert.ToDouble(Dr["PriceLevel5"]), Convert.ToDouble(Dr["PriceLevel6"]),
                         Convert.ToDouble(Dr["PriceLevel7"]), Convert.ToDouble(Dr["PriceLevel8"]), Convert.ToDouble(Dr["PriceLevel9"]), Convert.ToDouble(Dr["PriceLevel10"]), Dr["ToWhseId"].ToString().Replace("_", "").Trim()) == 0)
                        {
                            MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Trans.Rollback();
                            return;
                        }

                    }



                    //------------------transferote---------------------------------------

                    if (IsTransferExists(Dr["WhseTransId"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["ItemLine"])) == 1)
                    {
                        MessageBox.Show("This GRN Number '" + Dr["WhseTransId"].ToString().Replace("'", "").Trim() + "' Already exists in the database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Trans.Rollback();
                        return;
                    }
                    else
                    {

                        if (SaveGRN(Dr["WhseTransId"].ToString().Replace("'", "").Trim(), Dr["FrmWhseId"].ToString().Replace("_", "").Trim(),
                          Dr["ToWhseId"].ToString().Replace("_", "").Trim(), Convert.ToDateTime(Dr["TDate"]), Convert.ToDouble(Dr["NetValue"]), Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["ItemDis"].ToString().Replace("_", "").Trim(), Dr["SalesGLAccount"].ToString(),
                           Convert.ToDouble(Dr["QTY"]), Convert.ToDouble(Dr["UnitCost"]), Convert.ToDouble(Dr["ItemLine"]),dsInvoiceList.Tables[0].Rows.Count) == 0)
                        {
                            MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Trans.Rollback();
                            return;

                        }
                        if (SaveGRNTemp(Dr["WhseTransId"].ToString().Replace("'", "").Trim(), Dr["FrmWhseId"].ToString().Replace("_", "").Trim(),
                          Dr["ToWhseId"].ToString().Replace("_", "").Trim(), Convert.ToDateTime(Dr["TDate"]), Convert.ToDouble(Dr["NetValue"]), Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["ItemDis"].ToString().Replace("_", "").Trim(), Dr["SalesGLAccount"].ToString(),
                           Convert.ToDouble(Dr["QTY"]), Convert.ToDouble(Dr["UnitCost"]), Convert.ToDouble(Dr["ItemLine"]), dsInvoiceList.Tables[0].Rows.Count) == 0)
                        {
                            MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Trans.Rollback();
                            return;

                        }
                        if (SaveItemActivityCreditNote(Dr["WhseTransId"].ToString().Replace("'", "").Trim(), Convert.ToDateTime(Dr["TDate"]),
                            Dr["ItemID"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["QTY"]), Convert.ToDouble(Dr["UnitCost"]), Dr["ToWhseId"].ToString().Replace("_", "").Trim()) == 0)
                        {
                            MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Trans.Rollback();
                            return;
                        }

                        if (UpdateOnHandQtyCreditNote(Dr["ItemID"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["QTY"]), Dr["ToWhseId"].ToString().Replace("_", "").Trim()) == 0)
                        {
                            MessageBox.Show("Error found while updating OnHand Table", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Trans.Rollback();
                            return;
                        }
                    }
                }
                //send GRN To Peachtree-------------------
                if (CreateItemUploadXMLFile() == 0)
                {
                    MessageBox.Show("Item Export Error", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Trans.Rollback();
                    Con.Close();
                    return;
                }

                //foreach (DataRow Dr in dsInvoiceList.Tables[0].Rows)
                //{
                if (CreateGRNUploadFile() == 0)
                {
                    MessageBox.Show("GRN Export Error", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Trans.Rollback();
                    Con.Close();
                    return;
                }
                //}

                Connector ImportsInvoices = new Connector();
                if (ImportsInvoices.ImportItemMasterToPeactree() == 0)
                {
                    MessageBox.Show("Error found while updating Item Master To Peachtree database", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Trans.Rollback();
                    Con.Close();
                    return;
                }
                if (ImportsInvoices.ImportGRNUpload() == 0)
                {
                    MessageBox.Show("Error found while updating GRN To Peachtree database", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Trans.Rollback();
                    Con.Close();
                    return;
                }
                //ImportsInvoices.ImportGRNUpload();

                Trans.Commit();
                Con.Close();
                MessageBox.Show("Insert Item Master is Successfull", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
        }


        private int GetTransferNote()
        {
            try
            {

                //SQL = "SELECT  dbo.tblWhseTransfer.WhseTransId, dbo.tblWhseTransfer.FrmWhseId, dbo.tblWhseTransfer.ToWhseId, dbo.tblWhseTransfer.TDate, dbo.tblWhseTransfer.NetValue, " +
                //     " dbo.tblWhseTransLine.ItemId, dbo.tblWhseTransLine.ItemDis, SUM(dbo.tblWhseTransLine.QTY) AS QTY, dbo.tblWhseTransLine.UnitCost, " +
                //     " dbo.tblItemMaster.ItemClass, dbo.tblItemMaster.UnitPrice, dbo.tblItemMaster.SalesGLAccount, dbo.tblItemMaster.Categoty, dbo.tblItemMaster.UOM, " +
                //     " dbo.tblItemMaster.VendorID, dbo.tblItemMaster.PriceLevel1, dbo.tblItemMaster.PriceLevel2, dbo.tblItemMaster.PriceLevel3, dbo.tblItemMaster.PriceLevel4, " +
                //     " dbo.tblItemMaster.PriceLevel5, dbo.tblItemMaster.PriceLevel6, dbo.tblItemMaster.PriceLevel7, dbo.tblItemMaster.PriceLevel8, dbo.tblItemMaster.PriceLevel9, " +
                //     " dbo.tblItemMaster.PriceLevel10 " +
                //      "  FROM         dbo.tblItemMaster RIGHT OUTER JOIN " +
                //      " dbo.tblWhseTransLine ON dbo.tblItemMaster.ItemID = dbo.tblWhseTransLine.ItemId LEFT OUTER JOIN " +
                //      " dbo.tblWhseTransfer ON dbo.tblWhseTransLine.WhseTransId = dbo.tblWhseTransfer.WhseTransId " +
                //     "  GROUP BY dbo.tblWhseTransfer.WhseTransId, dbo.tblWhseTransfer.FrmWhseId, dbo.tblWhseTransfer.ToWhseId, dbo.tblWhseTransfer.TDate, dbo.tblWhseTransfer.NetValue, " +
                //    "  dbo.tblWhseTransLine.ItemId, dbo.tblWhseTransLine.ItemDis, dbo.tblWhseTransLine.UnitCost, dbo.tblItemMaster.ItemClass, dbo.tblItemMaster.UnitPrice, " +
                //    "  dbo.tblItemMaster.SalesGLAccount, dbo.tblItemMaster.Categoty, dbo.tblItemMaster.UOM, dbo.tblItemMaster.VendorID, dbo.tblItemMaster.PriceLevel1, " +
                //     " dbo.tblItemMaster.PriceLevel2, dbo.tblItemMaster.PriceLevel3, dbo.tblItemMaster.PriceLevel4, dbo.tblItemMaster.PriceLevel5, dbo.tblItemMaster.PriceLevel6, " +
                //    "  dbo.tblItemMaster.PriceLevel7, dbo.tblItemMaster.PriceLevel8, dbo.tblItemMaster.PriceLevel9, dbo.tblItemMaster.PriceLevel10 " +
                //      "  HAVING      (dbo.tblWhseTransfer.WhseTransId = '" +StrTransferNo + "')";

                SQL = "SELECT tblWhseTransfer.WhseTransId, tblWhseTransfer.FrmWhseId, tblWhseTransfer.ToWhseId,tblWhseTransfer.TDate, tblWhseTransfer.NetValue, " +
                     " tblWhseTransLine.ItemLine,replace( tblWhseTransLine.ItemId,',','')as ItemId,replace(tblWhseTransLine.ItemDis,',','')as ItemDis,isnull( tblWhseTransLine.QTY,0)as QTY ,isnull( tblWhseTransLine.UnitCost,0)as UnitCost, tblItemMaster.ItemClass, " +
                    "   isnull(tblItemMaster.UnitPrice,0)as UnitPrice ,  isnull(tblItemMaster.SalesGLAccount,'')as SalesGLAccount , isnull(tblItemMaster.Categoty,'')as Categoty , isnull(tblItemMaster.UOM,'')as UOM ,isnull(tblItemMaster.VendorID,'')as VendorID , " +
                    "  isnull(tblItemMaster.PriceLevel1,0)as PriceLevel1, isnull(tblItemMaster.PriceLevel2,0)as PriceLevel2, isnull(tblItemMaster.PriceLevel3,0)as PriceLevel3, isnull(tblItemMaster.PriceLevel4,0)as PriceLevel4, isnull(tblItemMaster.PriceLevel5,0)as PriceLevel5, isnull(tblItemMaster.PriceLevel6,0)as PriceLevel6, " +
                    "  isnull(tblItemMaster.PriceLevel7,0)as PriceLevel7, isnull(tblItemMaster.PriceLevel8,0)as PriceLevel8, isnull(tblItemMaster.PriceLevel9,0)as PriceLevel9, isnull(tblItemMaster.PriceLevel10,0)as PriceLevel10 " +
                    "FROM  tblItemMaster RIGHT OUTER JOIN " +
                      "tblWhseTransLine ON tblItemMaster.ItemID = tblWhseTransLine.ItemId LEFT OUTER JOIN" +
                    "  tblWhseTransfer ON tblWhseTransLine.WhseTransId = tblWhseTransfer.WhseTransId" +
                    " WHERE (tblWhseTransfer.WhseTransId = '" + StrTransferNo + "')";

                // SQL = "SELECT tblWhseTransfer.WhseTransId AS TransferNO,tblWhseTransfer.FrmWhseId, tblWhseTransfer.ToWhseId, tblWhseTransfer.TDate, tblWhseTransfer.NetValue, " +
                //" replace(tblWhseTransLine.ItemId,',','')as ItemID,replace(tblWhseTransLine.ItemDis,',','')as ItemDescription, isnull(tblWhseTransLine.QTY,0)as QTY ,tblWhseTransLine.UnitCost,tblWhseTransLine.ItemLine FROM tblWhseTransLine RIGHT OUTER JOIN" +
                //      " tblWhseTransfer ON tblWhseTransLine.WhseTransId = tblWhseTransfer.WhseTransId WHERE(tblWhseTransfer.TDate >='" + dtpFromDate.Text.ToString().Trim() + "') AND (tblWhseTransfer.TDate <= '" + dtpToDate.Text.ToString().Trim() + "')";
                Con = new SqlConnection(ConnectionString);
                CMD = new SqlCommand(SQL, Con);
                CMD.CommandType = CommandType.Text;
                DS = new DataSet();
                DA = new SqlDataAdapter(CMD);
                Con.Open();
                DA.Fill(DS);
                Con.Close();
                //StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\TransferNotes.csv", false);
                StreamWriter sw = new StreamWriter(txtselect.Text.ToString().Trim(), false);
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
                                if (i == 6 || i == 2)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = dr[i].ToString();
                            }
                            else
                            {
                                if (i == 6 || i == 2)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = "'" + dr[i].ToString();
                            }
                            //sWriteValue = dr[i].ToString();
                        }
                        else
                        {
                            if (i == 6 || i == 2)
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
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        //private int GetAllCreditNotes()
        //{
        //    try
        //    {

        //        SQL = "SELECT tblWhseTransfer.WhseTransId AS TransferNO,tblWhseTransfer.FrmWhseId, tblWhseTransfer.ToWhseId, tblWhseTransfer.TDate, tblWhseTransfer.NetValue, " +
        //       " replace(tblWhseTransLine.ItemId,',','')as ItemID,replace(tblWhseTransLine.ItemDis,',','')as ItemDescription, isnull(tblWhseTransLine.QTY,0)as QTY ,tblWhseTransLine.UnitCost,tblWhseTransLine.ItemLine FROM tblWhseTransLine RIGHT OUTER JOIN" +
        //             " tblWhseTransfer ON tblWhseTransLine.WhseTransId = tblWhseTransfer.WhseTransId WHERE(tblWhseTransfer.TDate >='" + dtpFromDate.Text.ToString().Trim() + "') AND (tblWhseTransfer.TDate <= '" + dtpToDate.Text.ToString().Trim() + "')";
        //        Con = new SqlConnection(ConnectionString);
        //        CMD = new SqlCommand(SQL, Con);
        //        CMD.CommandType = CommandType.Text;
        //        DS = new DataSet();
        //        DA = new SqlDataAdapter(CMD);
        //        Con.Open();
        //        DA.Fill(DS);
        //        Con.Close();
        //        StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\TransferNotes.csv", false);
        //        int cCount = DS.Tables[0].Columns.Count;
        //        int iRow = 0;
        //        foreach (DataColumn dc in DS.Tables[0].Columns)
        //        {
        //            iRow = iRow + 1;
        //            sw.Write(dc.ColumnName);
        //            if (iRow != cCount)
        //            {
        //                sw.Write(",");
        //            }
        //        }
        //        sw.Write(sw.NewLine);
        //        foreach (DataRow dr in DS.Tables[0].Rows)
        //        {
        //            iRow = 0;
        //            for (int i = 0; i < cCount; i++)
        //            {
        //                iRow = iRow + 1;
        //                string s = dr[i].ToString(); //Get Current Datarow value
        //                long n = 0;
        //                bool b = long.TryParse(s, out n);
        //                string sWriteValue = string.Empty;

        //                if (b == true)
        //                {
        //                    //check value are equal '001=1
        //                    if (s == n.ToString())
        //                    {
        //                        if (i == 5 || i == 2)
        //                            sWriteValue = dr[i].ToString() + "_";
        //                        else
        //                            sWriteValue = dr[i].ToString();
        //                    }
        //                    else
        //                    {
        //                        if (i == 5 || i == 2)
        //                            sWriteValue = dr[i].ToString() + "_";
        //                        else
        //                            sWriteValue = "'" + dr[i].ToString();
        //                    }
        //                    //sWriteValue = dr[i].ToString();
        //                }
        //                else
        //                {
        //                    if (i == 5 || i == 2)
        //                        sWriteValue = dr[i].ToString() + "_";

        //                    else sWriteValue = dr[i].ToString();
        //                }
        //                sw.Write(sWriteValue);
        //                if (iRow != cCount)
        //                {
        //                    sw.Write(",");
        //                }
        //            }
        //            sw.Write(sw.NewLine);

        //        }
        //        sw.Close();
        //        return 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return 0;
        //    }
        //}
        private int GetAllInvoive()
        {
            try
            {
                // String S3 = "Select * from tblPurchaseOrder  where Date  >= '" + dtpFromDate.Text.ToString().Trim() + "' and  Date <='" + dtpToDate.Text.ToString().Trim() + "'";
                //replace(Description,',','-')as Description
                SQL = " SELECT  replace(ItemID,',','')as ItemID, replace(ItemDescription,',','')as ItemDescription, ItemClass,isnull(UnitPrice,'0.00')as UnitPrice, isnull(SalesGLAccount,'0.00')as SalesGLAccount, " +
                                " isnull(Categoty,'') as Categoty,isnull(UOM,'') as UOM,isnull(UnitCost,'0.00')as UnitCost, isnull(VendorID,'')as VendorID, isnull(PriceLevel1,'0.00')as PriceLevel1, isnull(PriceLevel2,'0.00')as PriceLevel2," +
                                " isnull(PriceLevel3,'0.00')as PriceLevel3, isnull(PriceLevel4,'0.00')as PriceLevel4, isnull(PriceLevel5,'0.00')as PriceLevel5, isnull(PriceLevel6,'0.00')as PriceLevel6, isnull(PriceLevel7,'0.00')as PriceLevel7, isnull(PriceLevel8,'0.00')as PriceLevel8, isnull(PriceLevel9,'0.00')as PriceLevel9, " +
                                " isnull(PriceLevel10,'0.00')as PriceLevel10 FROM tblItemMaster";// WHERE InvoiceDate>='" + dtpFromDate.Text.ToString().Trim() + "' and InvoiceDate<= '" + dtpToDate.Text.ToString().Trim() + "' and IsExport = 0 ORDER BY InvoiceNo";
                Con = new SqlConnection(ConnectionString);
                CMD = new SqlCommand(SQL, Con);
                CMD.CommandType = CommandType.Text;
                DS = new DataSet();
                DA = new SqlDataAdapter(CMD);
                Con.Open();
                DA.Fill(DS);
                Con.Close();

                StreamWriter sw = new StreamWriter(@"c:\\PBSS\\ItemMaster.csv", false);
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
                                if (i == 0)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = dr[i].ToString();
                            }
                            else
                            {
                                if (i == 0)
                                    sWriteValue = dr[i].ToString() + "_";
                                else
                                    sWriteValue = "'" + dr[i].ToString();
                            }
                            //sWriteValue = dr[i].ToString();
                        }
                        else
                        {
                            if (i == 0)
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
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;

            }




        }
        private void frmImportExport_Load(object sender, EventArgs e)
        {
            StrTransferNo = ClassDriiDown.StrTrnasferNoteNO;
            txtTrnsferNO.Text = StrTransferNo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (rbInvoices.Checked == true || rbCreditNote.Checked == true)
            //{
            try
            {
                if (txtselect.Text == "")
                {
                    MessageBox.Show("You must Select a File Path", "File Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //if (rbInvoices.Checked == true)//items
                //{
                //    GetAllInvoive();
                //    MessageBox.Show("Creating Item Master File is Successfull", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                //else if (rbCreditNote.Checked == true)//TransferNote
                //{
                GetTransferNote();
                // GetAllCreditNotes();
                MessageBox.Show("Creating TrnasferNote File is Successfull", "File Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
            }
            catch { }
            //}
            //else
            //{
            //    MessageBox.Show("Please Select a valid file type");
            //}

            //GetAllInvoive();
            //GetAllCreditNotes();
            //MessageBox.Show("Export has successfully completed");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //Connector ObjImportP = new Connector();
                //ObjImportP.ImportDirectSalesInvice1();
                impoLocaldb();
                MessageBox.Show("Item Insert Successfully Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void impoLocaldb()
        {
            try
            {
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerInvoice.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Invoice");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                coa = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                for (int j = 0; j <= aLength - 1; j++)
                {
                    StrInvoiceNo = "";
                    StrCustomerID = "";
                    DtInvoiceDate = DateTime.Today;
                    dblNoofDistributions = 0;
                    StrARAccount = "";
                    blIsReturn = false;

                    node = reader[j];
                    for (int i = 0; i < 12; i++)
                    {
                        if (node.ChildNodes[i] != null)
                        {
                            if (node.ChildNodes[i].Name == "Customer_ID")
                            {
                                StrCustomerID = node.ChildNodes[i].InnerText;
                            }
                            else if (node.ChildNodes[i].Name == "Invoice_Number")
                            {
                                StrInvoiceNo = node.ChildNodes[i].InnerText;
                            }
                            else if (node.ChildNodes[i].Name == "Date")
                            {
                                DtInvoiceDate = DateTime.Parse(node.ChildNodes[i].InnerText);
                            }
                            else if (node.ChildNodes[i].Name == "Number_of_Distributions")
                            {
                                dblNoofDistributions = double.Parse(node.ChildNodes[i].InnerText);
                            }
                            else if (node.ChildNodes[i].Name == "Accounts_Receivable_Account")
                            {
                                StrARAccount = node.ChildNodes[i].InnerText;
                            }
                            else if (node.ChildNodes[i].Name == "CreditMemoType")
                            {
                                blIsReturn = bool.Parse(node.ChildNodes[i].InnerText);
                            }
                            else if (node.ChildNodes[i].Name == "SalesLines")
                            {
                                for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                                {
                                    if (node.ChildNodes[i].ChildNodes[k].Name == "SalesLine")
                                    {
                                        dblQty = 0;
                                        StrItemID = "";
                                        StrDescription = "";
                                        StrGLAcount = "";
                                        dblAmount = 0.00;
                                        StrTTType1 = "";
                                        blIsApplyToSO = false;
                                        dblDistributionNo = 0;
                                        dblUnitPrice = 0;

                                        for (int l = 0; l <= node.ChildNodes[i].ChildNodes[k].ChildNodes.Count - 1; l++)
                                        {
                                            if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "Quantity")
                                            {
                                                dblQty = double.Parse(node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText);
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "Item_ID")
                                            {
                                                StrItemID = node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText;
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "Description")
                                            {
                                                StrDescription = node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText;
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "GL_Account")
                                            {
                                                StrGLAcount = node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText;
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "Unit_Price")
                                            {
                                                dblUnitPrice = double.Parse(node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText);
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "Amount")
                                            {
                                                dblAmount = double.Parse(node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText);
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "Tax_Type")
                                            {
                                                StrTTType1 = node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText;
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "Apply_To_Sales_Order")
                                            {
                                                blIsApplyToSO = bool.Parse(node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText);
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "SalesOrderDistributionNumber")
                                            {
                                                dblDistributionNo = double.Parse(node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText);
                                            }
                                            else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[l].Name == "Warehouse")
                                            {
                                                StrLocation = node.ChildNodes[i].ChildNodes[k].ChildNodes[l].InnerText;
                                            }
                                        }
                                    }
                                    Save(StrInvoiceNo, StrCustomerID, DtInvoiceDate, dblNoofDistributions, StrARAccount, blIsReturn, dblQty, StrItemID, StrDescription, StrGLAcount, dblAmount, StrTTType1, blIsApplyToSO, dblDistributionNo, dblUnitPrice, StrLocation);

                                }
                            }
                        }
                    }
                    //SaveItem(objclsBeansItem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //return objclsBeansItem;
        }

        private void Save(string StrInvoiceNo, string StrCustomerID, DateTime DtInvoiceDate, double dblNoofDistributions, string StrARAccount, bool blIsReturn, double dblQty, string StrItemID, string StrDescription, string StrGLAcount, double dblAmount, string StrTTType1, bool blIsApplyToSO, double dblDistributionNo, double dblUnitPrice, string StrLocation)
        {
            try
            {
                //setConnectionString();

                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Trans = Con.BeginTransaction();
                //-------------------------------------------
                SQL = "SELECT InvoiceNo, DistributionNo FROM tblSalesInvoices where InvoiceNo='" + StrInvoiceNo + "' and DistributionNo='" + dblDistributionNo + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
                if (DS.Tables[0].Rows.Count > 0)
                {

                    if (StrInvoiceNo == DS.Tables[0].Rows[0].ItemArray[0].ToString().Trim() && dblDistributionNo == Convert.ToDouble(DS.Tables[0].Rows[0].ItemArray[1]))
                    {
                        //MessageBox.Show("Invoice No '" + StrInvoiceNo + "' already exists in the database");
                        //return;
                    }
                    else
                    {


                        //}
                        //if (MessageBox.Show("Invoice No '" + StrInvoiceNo + "' already exists in the database, Do you want to Proceed", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        //{

                        //}
                        //else
                        //{

                        //}

                        //MessageBox.Show("Invoice No '" + StrInvoiceNo + "' already exists in the database");
                        //return;
                        //}
                        //else
                        //{


                        SQL = "INSERT INTO tblSalesInvoices (InvoiceNo, CustomerID, " +
                            " InvoiceDate, ARAccount, NoofDistributions, " +
                            " DistributionNo, ItemID, Qty, " +
                            " Description, GLAcount, UnitPrice, " +
                            " TTType1, IsReturn, Amount) " +
                            " VALUES ('" + StrInvoiceNo + "','" + StrCustomerID + "','" +
                            DtInvoiceDate + "','" + StrARAccount + "','" + dblNoofDistributions + "','" +
                            dblDistributionNo + "','" + StrItemID + "','" + dblQty + "','" +
                            StrDescription + "','" + StrGLAcount + "','" + dblUnitPrice + "','" +
                            StrTTType1 + "','" + blIsReturn + "','" + dblAmount + "','" + StrLocation + "')";

                        CMD = new SqlCommand(SQL, Con, Trans);
                        CMD.CommandType = CommandType.Text;
                        CMD.ExecuteNonQuery();


                        // InsertItemActivity(StrInvoiceNo, DtInvoiceDate, StrItemID, StrLocation, dblQty, dblUnitPrice, dblNetTotal);

                        //}
                    }
                }
                else
                {
                    SQL = "INSERT INTO tblSalesInvoices (InvoiceNo, CustomerID, " +
                                               " InvoiceDate, ARAccount, NoofDistributions, " +
                                               " DistributionNo, ItemID, Qty, " +
                                               " Description, GLAcount, UnitPrice, " +
                                               " TTType1, IsReturn, Amount) " +
                                               " VALUES ('" + StrInvoiceNo + "','" + StrCustomerID + "','" +
                                               DtInvoiceDate + "','" + StrARAccount + "','" + dblNoofDistributions + "','" +
                                               dblDistributionNo + "','" + StrItemID + "','" + dblQty + "','" +
                                               StrDescription + "','" + StrGLAcount + "','" + dblUnitPrice + "','" +
                                               StrTTType1 + "','" + blIsReturn + "','" + dblAmount + "')";

                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    CMD.ExecuteNonQuery();
                    InsertItemActivity(StrInvoiceNo, DtInvoiceDate, StrItemID, StrLocation, dblQty, dblUnitPrice, dblNetTotal);
                }

                Trans.Commit();
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                throw ex;
            }
        }

        private void InsertItemActivity(string strInvoiceNo, DateTime dtDate, String StrItemCode, string StrLocCode, double dblQuantity, double dblPrice, double dblLineNetAmt)
        {
            //DocType=4,TranType=BranchInvoice,DocReference=0
            //SQL = "SELECT [TranNo],[ItemID],[WarehouseID] FROM [tbItemlActivity] where [TranNo]='" + StrInvoiceNo + "' and [ItemID]='" + StrItemID + "' and [WarehouseID]='" + StrLocation + "'";
            //CMD = new SqlCommand(SQL, Con, Trans);
            //CMD.CommandType = CommandType.Text;
            //DA = new SqlDataAdapter(CMD);
            //DS = new DataSet();
            //DA.Fill(DS);
            //if (DS.Tables[0].Rows.Count > 0)
            //{


            //}

            SQL = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(4,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','BranchInvoice','false','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "','" + dblPrice + "')";
            CMD = new SqlCommand(SQL, Con, Trans);
            CMD.CommandType = CommandType.Text;
            CMD.ExecuteNonQuery();
        }
        public static string GetDateTime(DateTime DtGetDate)
        {
            DateTime DTP = Convert.ToDateTime(DtGetDate);
            string Dformat = "MM/dd/yyyy";
            return DTP.ToString(Dformat);

        }
        private int GetCustomer()
        {
            try
            {
                //SQL = " SELECT CutomerID, CustomerName, Address1, Address2, Phone1, " +
                //                " Custom1, Custom2, Custom3, Custom4,Custom5, ShipToAddress1, ShipToAddress2," +
                //                " Phone2, Fax, Email, DueDays from  tblCustomerMaster";

                SQL = " SELECT replace(CutomerID,',','-')as CutomerID,replace(CustomerName,',','-')as CustomerName  from  tblCustomerMaster";
                Con = new SqlConnection(ConnectionString);
                CMD = new SqlCommand(SQL, Con);
                CMD.CommandType = CommandType.Text;
                DS = new DataSet();
                DA = new SqlDataAdapter(CMD);
                Con.Open();
                DA.Fill(DS);
                Con.Close();

                StreamWriter sw = new StreamWriter(@"c:\\PBSS\\CustomerMaster.csv", false);
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
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;

            }
        }

        private int GetAdjustments()
        {
            try
            {
                SQL = " SELECT replace(AdjusmentId,',','-')as AdjusmentId,Date,cast(replace(WarehouseId,',','-') as varchar(100))as WarehouseId,WarehouseName, replace(ItemID,',','-')as ItemID,replace(Itemdescription,',','-')as Itemdescription,UnitCost,OnhandQty,AdjustQty,NewQty,ReasonToAdjust,GLAccount from  tblInventoryAdjustment";
                Con = new SqlConnection(ConnectionString);
                CMD = new SqlCommand(SQL, Con);
                CMD.CommandType = CommandType.Text;
                DS = new DataSet();
                DA = new SqlDataAdapter(CMD);
                Con.Open();
                DA.Fill(DS);
                Con.Close();

                StreamWriter sw = new StreamWriter(@"c:\\PBSS\\Adjustments.csv", false);
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
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;

            }
        }

        private void btnExportCustomer_Click(object sender, EventArgs e)
        {
            GetCustomer();
            MessageBox.Show("Create Customer Data File is Successfull");
        }

        //private void InsertAdjustments()
        //{
        //    //sanjeewa
        //    DataSet dsAdjustments = new DataSet();
        //    string FilePath = string.Empty;
        //    string FileName = string.Empty;
        //    OFD.InitialDirectory = "C:\\PBSS";
        //    OFD.Filter = "csv files (*.csv)|*.csv";
        //    if (OFD.ShowDialog() == DialogResult.OK)
        //    {
        //        FilePath = OFD.FileName;
        //        FileName = Path.GetFileName(FilePath);
        //        dsAdjustments = Get_Adjustments_From_CSV_To_Dataset(FileName);
        //    }
        //    if (dsAdjustments.Tables.Count == 0)
        //    {
        //        MessageBox.Show("can not file the Adjutments data", "No data in csv file");
        //        return;
        //    }
        //    if (MessageBox.Show("Do you want to update now ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //    {
        //        if (rbtBoth.Checked == true)
        //        {
        //            Con = new SqlConnection(ConnectionString);
        //            Con.Open();
        //            Trans = Con.BeginTransaction();
        //            //IsCustomerExists
        //            foreach (DataRow Dr in dsAdjustments.Tables[0].Rows)
        //            {
        //                if (SaveAdjustments(Dr["AdjusmentId"].ToString().Replace("'", "").Trim(), Convert.ToDateTime(Dr["Date"]), Dr["WarehouseId"].ToString().Replace("'", "").Trim(), Dr["WarehouseName"].ToString().Replace("'", "").Trim(), Dr["ItemID"].ToString().Replace("'", "").Trim(), Dr["Itemdescription"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["UnitCost"]), Convert.ToDouble(Dr["OnhandQty"]), Convert.ToDouble(Dr["AdjustQty"]), Convert.ToDouble(Dr["NewQty"]), Dr["ReasonToAdjust"].ToString().Replace("'", "").Trim(), Dr["GLAccount"].ToString().Replace("'", "").Trim()) == 0)
        //                {
        //                    MessageBox.Show("Error found while updating local database", "Inventory Adjustments", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }

        //                if (SaveItemActivityAdjustment(Dr["AdjusmentId"].ToString(), Convert.ToDateTime(Dr["Date"]),
        //                       Dr["ItemID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["AdjustQty"]), Convert.ToDouble(Dr["UnitCost"]), Dr["WarehouseId"].ToString().Replace("'", "").Trim()) == 0)
        //                {
        //                    MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }

        //                if (UpdateOnHandAdjustment(Dr["ItemID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["AdjustQty"]), Dr["WarehouseId"].ToString().Replace("'", "").Trim()) == 0)
        //                {
        //                    MessageBox.Show("Error found while updating OnHand Table", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }

        //            }
        //            //  Trans.Commit();

        //            if (ExportInventoryAdjustments() == 0)
        //            {
        //                MessageBox.Show("Invoice Export Error", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                Trans.Rollback();
        //                Con.Close();
        //                return;
        //            }
        //            Connector ImportsInvoices = new Connector();
        //            if (ImportsInvoices.InventoryExport() == 0)
        //            {
        //                MessageBox.Show("Error found while updating Peachtree database", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                Trans.Rollback();
        //                Con.Close();
        //                return;
        //            }
        //            //else
        //            //{
        //            //    if (UpdateCustomerMaster() == 0)
        //            //    {
        //            //        Trans.Rollback();
        //            //        Con.Close();
        //            //        return;
        //            //    }
        //            //    else
        //            //    {
        //            Trans.Commit();
        //            MessageBox.Show("Invoice Import is Sucessfull", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            Con.Close();
        //            //    }
        //            //}

        //        }
        //        else if (rbLocalDB.Checked == true)
        //        {

        //            Con = new SqlConnection(ConnectionString);
        //            Con.Open();
        //            Trans = Con.BeginTransaction();

        //            foreach (DataRow Dr in dsAdjustments.Tables[0].Rows)
        //            {
        //                if (SaveAdjustments(Dr["AdjusmentId"].ToString().Replace("'", "").Trim(), Convert.ToDateTime(Dr["Date"]), Dr["WarehouseId"].ToString().Replace("_", "").Trim(), Dr["WarehouseName"].ToString().Replace("'", "").Trim(), Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["Itemdescription"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["UnitCost"]), Convert.ToDouble(Dr["OnhandQty"]), Convert.ToDouble(Dr["AdjustQty"]), Convert.ToDouble(Dr["NewQty"]), Dr["ReasonToAdjust"].ToString().Replace("'", "").Trim(), Dr["GLAccount"].ToString().Replace("'", "").Trim()) == 0)
        //                {
        //                    MessageBox.Show("Error found while updating local database", "Inventory Adjustments", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }

        //                if (SaveItemActivityAdjustment(Dr["AdjusmentId"].ToString(), Convert.ToDateTime(Dr["Date"]),
        //                       Dr["ItemID"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["AdjustQty"]), Convert.ToDouble(Dr["UnitCost"]), Dr["WarehouseId"].ToString().Replace("_", "").Trim()) == 0)
        //                {
        //                    MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }

        //                if (UpdateOnHandAdjustment(Dr["ItemID"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["AdjustQty"]), Dr["WarehouseId"].ToString().Replace("_", "").Trim()) == 0)
        //                {
        //                    MessageBox.Show("Error found while updating OnHand Table", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }
        //            }
        //            Trans.Commit();
        //            MessageBox.Show("Invoice Import is Sucessfull", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            Con.Close();
        //        }
        //        else if (rbPeachTree.Checked == true)
        //        {
        //            Con = new SqlConnection(ConnectionString);
        //            Con.Open();
        //            Trans = Con.BeginTransaction();


        //            if (ExportInventoryAdjustments() == 0)
        //            {
        //                MessageBox.Show("Invoice Export Error", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                Trans.Rollback();
        //                Con.Close();
        //                return;
        //            }
        //            Connector ImportsInvoices = new Connector();
        //            if (ImportsInvoices.InventoryExport() == 0)
        //            {
        //                MessageBox.Show("Error found while updating Peachtree database", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                Trans.Rollback();
        //                Con.Close();
        //                return;
        //            }
        //            //else
        //            //{
        //            //    if (UpdateCustomerMaster() == 0)
        //            //    {
        //            //        Trans.Rollback();
        //            //        Con.Close();
        //            //        return;
        //            //    }
        //            //    else
        //            //    {
        //            Trans.Commit();
        //            MessageBox.Show("Invoice Import is Sucessfull", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            Con.Close();
        //            //    }
        //            //}
        //        }
        //    }
        //}
        //private void InsertCustomer()
        //{
        //    //sanjeewa
        //    DataSet dsCustomer = new DataSet();
        //    string FilePath = string.Empty;
        //    string FileName = string.Empty;
        //    OFD.InitialDirectory = "C:\\PBSS";
        //    OFD.Filter = "csv files (*.csv)|*.csv";
        //    if (OFD.ShowDialog() == DialogResult.OK)
        //    {
        //        FilePath = OFD.FileName;
        //        FileName = Path.GetFileName(FilePath);
        //        dsCustomer = Get_CustomerMaster_From_CSV_To_Dataset(FileName);
        //    }

        //    if (dsCustomer.Tables.Count == 0)
        //    {
        //        MessageBox.Show("can not file the Customer data", "No data in csv file");
        //        return;
        //    }
        //    if (MessageBox.Show("Do you want to update now ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //    {
        //        if (rbtBoth.Checked == true)
        //        {
        //            Con = new SqlConnection(ConnectionString);
        //            Con.Open();
        //            Trans = Con.BeginTransaction();
        //            //IsCustomerExists
        //            foreach (DataRow Dr in dsCustomer.Tables[0].Rows)
        //            {
        //                if (SaveCustomerMaster(Dr["CutomerID"].ToString().Replace("'", "").Trim(), Dr["CustomerName"].ToString().Replace("'", "").Trim()) == 0)
        //                {
        //                    MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }
        //            }
        //            if (ExportCustomerMaster() == 0)
        //            {
        //                MessageBox.Show("Invoice Export Error", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                Trans.Rollback();
        //                Con.Close();
        //                return;
        //            }
        //            Connector ImportsInvoices = new Connector();
        //            if (ImportsInvoices.ImportCustomerMaster() == 0)
        //            {
        //                MessageBox.Show("Error found while updating Peachtree database", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                Trans.Rollback();
        //                Con.Close();
        //                return;
        //            }
        //            //else
        //            //{
        //            //    if (UpdateCustomerMaster() == 0)
        //            //    {
        //            //        Trans.Rollback();
        //            //        Con.Close();
        //            //        return;
        //            //    }
        //            //    else
        //            //    {
        //            Trans.Commit();
        //            MessageBox.Show("Invoice Import is Sucessfull", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            Con.Close();
        //            //    }
        //            //}

        //        }
        //        else if (rbLocalDB.Checked == true)
        //        {

        //            Con = new SqlConnection(ConnectionString);
        //            Con.Open();
        //            Trans = Con.BeginTransaction();

        //            foreach (DataRow Dr in dsCustomer.Tables[0].Rows)
        //            {

        //                if (IsCustomerExists(Dr["CutomerID"].ToString().Replace("'", "").Trim()) == 1)
        //                {
        //                    MessageBox.Show("This Customer '" + Dr["CutomerID"].ToString().Trim() + "' Already exists in the database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }

        //                if (SaveCustomerMaster(Dr["CutomerID"].ToString().Replace("'", "").Trim(), Dr["CustomerName"].ToString().Replace("'", "").Trim()) == 0)
        //                {
        //                    MessageBox.Show("Error found while updating local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    Trans.Rollback();
        //                    return;
        //                }
        //            }
        //            Trans.Commit();
        //            MessageBox.Show("Invoice Import is Sucessfull", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            Con.Close();
        //        }
        //        else if (rbPeachTree.Checked == true)
        //        {
        //            Con = new SqlConnection(ConnectionString);
        //            Con.Open();
        //            Trans = Con.BeginTransaction();


        //            if (ExportCustomerMaster() == 0)
        //            {
        //                MessageBox.Show("Invoice Export Error", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                Trans.Rollback();
        //                Con.Close();
        //                return;
        //            }
        //            Connector ImportsInvoices = new Connector();
        //            if (ImportsInvoices.ImportCustomerMaster() == 0)
        //            {
        //                MessageBox.Show("Error found while updating Peachtree database", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                Trans.Rollback();
        //                Con.Close();
        //                return;
        //            }
        //            //else
        //            //{
        //            //    if (UpdateCustomerMaster() == 0)
        //            //    {
        //            //        Trans.Rollback();
        //            //        Con.Close();
        //            //        return;
        //            //    }
        //            //    else
        //            //    {
        //            Trans.Commit();
        //            MessageBox.Show("Invoice Import is Sucessfull", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            Con.Close();
        //            //    }
        //            //}
        //        }
        //    }
        //}

        //private void btnInsertCustomer_Click(object sender, EventArgs e)
        //{
        //    if (rbLocalDB.Checked == false && rbPeachTree.Checked == false && rbtBoth.Checked == false)
        //    {
        //        MessageBox.Show("Please select an import option");
        //        return;
        //    }
        //    InsertCustomer();
        //}

        private void btnCresteAdjustment_Click(object sender, EventArgs e)
        {
            GetAdjustments();
            MessageBox.Show("Create Adjustments Data File is Successfull");
        }

        //private void btnInsertAdjustment_Click(object sender, EventArgs e)
        //{
        //    if (rbLocalDB.Checked == false && rbPeachTree.Checked == false && rbtBoth.Checked == false)
        //    {
        //        MessageBox.Show("Please select an import option");
        //        return;
        //    }
        //    InsertAdjustments();
        //}

        private DataSet FillGL(DataSet objdtsGL)
        {

            DataSet _objdtsGL = objdtsGL;
            try
            {
                Microsoft.Office.Interop.Excel.Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                Microsoft.Office.Interop.Excel.Range range;//openFileDialog1.FileName

                string str;
                int rCnt = 0;
                int cCnt = 0;
                int _indexOfAdj = 0;
                int _indexOfDate = 0;
                int _indexOfWH = 0;
                int _indexOfWHN = 0;
                int _indexOfItem = 0;
                int _indexOfItemD = 0;
                int _IndexOfUC = 0;
                int _IndexOfOHQ = 0;
                int _IndexOfAQ = 0;
                int _IndexOfNQ = 0;
                int _IndexOfRToAdj = 0;
                int _IndexOfGLAcc = 0;
                int _IndexRowStartValues = 0;

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open("C:\\PBSS\\Adjustments.csv", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                for (rCnt = 1; rCnt <= range.Rows.Count; rCnt++)
                {
                    for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                    {


                        str = (string)(range.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Text;
                        //MessageBox.Show(str);
                        if (str == "AdjusmentId")
                        {
                            _IndexRowStartValues = rCnt;
                            _indexOfAdj = cCnt;
                        }
                        if (str == "Date")
                        {
                            _indexOfDate = cCnt;
                        }
                        if (str == "WarehouseId")
                        {
                            _indexOfWH = cCnt;
                        }
                        if (str == "WarehouseName")
                        {
                            _indexOfWHN = cCnt;
                        }
                        if (str == "ItemID")
                        {
                            _indexOfItem = cCnt;
                        }
                        if (str == "Itemdescription")
                        {
                            _indexOfItemD = cCnt;
                        }
                        if (str == "UnitCost")
                        {
                            _IndexOfUC = cCnt;
                        }
                        if (str == "OnhandQty")
                        {
                            _IndexOfOHQ = cCnt;
                        }
                        if (str == "AdjustQty")
                        {
                            _IndexOfAQ = cCnt;
                        }
                        if (str == "Itemdescription")
                        {
                            _indexOfItemD = cCnt;
                        }
                        if (str == "NewQty")
                        {
                            _IndexOfNQ = cCnt;
                        }
                        if (str == "ReasonToAdjust")
                        {
                            _IndexOfRToAdj = cCnt;
                        }
                        if (str == "GLAccount")
                        {
                            _IndexOfGLAcc = cCnt;
                        }

                    }
                    if (_IndexRowStartValues > 0) break;
                }

                double _Credit = 0.00;
                double _Debit = 0.00;
                string _CashbookAcc = "";


                for (int _rowIndex = _IndexRowStartValues + 1; _rowIndex <= range.Rows.Count - 1; _rowIndex++)
                {
                    _Credit = 0.00;
                    _Debit = 0.00;
                    //string d = (string)(range.Cells[_rowIndex, _indexOfLD] as Microsoft.Office.Interop.Excel.Range).Text;

                    //if ((range.Cells[_rowIndex, _indexOfCrdt] as Microsoft.Office.Interop.Excel.Range).Value2 != null && (range.Cells[_rowIndex, _indexOfCrdt] as Microsoft.Office.Interop.Excel.Range).Value2.ToString().Trim().Length > 0) _Credit = double.Parse((string)(range.Cells[_rowIndex, _indexOfCrdt] as Microsoft.Office.Interop.Excel.Range).Text.ToString());
                    //if ((range.Cells[_rowIndex, _indexOfDedt] as Microsoft.Office.Interop.Excel.Range).Value2 != null && (range.Cells[_rowIndex, _indexOfDedt] as Microsoft.Office.Interop.Excel.Range).Value2.ToString().Trim().Length > 0) _Debit = double.Parse((string)(range.Cells[_rowIndex, _indexOfDedt] as Microsoft.Office.Interop.Excel.Range).Text.ToString());

                    //if (_Credit != 0) _CashbookAcc = (string)(range.Cells[_rowIndex, _indexOfGL] as Microsoft.Office.Interop.Excel.Range).Text + " " + (string)(range.Cells[_rowIndex, _indexOfDesc] as Microsoft.Office.Interop.Excel.Range).Text;

                    //if (_Debit >= 0 && _Credit == 0)
                    //{
                    _objdtsGL.Tables["dtblGL"].Rows.Add(
                        (string)(range.Cells[_rowIndex, _indexOfAdj] as Microsoft.Office.Interop.Excel.Range).Text,
                        (string)(range.Cells[_rowIndex, _indexOfDate] as Microsoft.Office.Interop.Excel.Range).Text, //"",
                        (string)(range.Cells[_rowIndex, _indexOfWH] as Microsoft.Office.Interop.Excel.Range).Text,
                        (string)(range.Cells[_rowIndex, _indexOfWHN] as Microsoft.Office.Interop.Excel.Range).Text,
                        (string)(range.Cells[_rowIndex, _indexOfItem] as Microsoft.Office.Interop.Excel.Range).Text,
                        (string)(range.Cells[_rowIndex, _indexOfItemD] as Microsoft.Office.Interop.Excel.Range).Text,
                        (string)(range.Cells[_rowIndex, _IndexOfUC] as Microsoft.Office.Interop.Excel.Range).Text,
                        (string)(range.Cells[_rowIndex, _IndexOfOHQ] as Microsoft.Office.Interop.Excel.Range).Text,
                        (string)(range.Cells[_rowIndex, _IndexOfAQ] as Microsoft.Office.Interop.Excel.Range).Text,
                        (string)(range.Cells[_rowIndex, _IndexOfNQ] as Microsoft.Office.Interop.Excel.Range).Text,
                         (string)(range.Cells[_rowIndex, _IndexOfRToAdj] as Microsoft.Office.Interop.Excel.Range).Text,
                          (string)(range.Cells[_rowIndex, _IndexOfGLAcc] as Microsoft.Office.Interop.Excel.Range).Text);
                    //}
                }

                xlWorkBook.Close(true, null, null);
                xlApp.Quit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _objdtsGL;
        }

        private void rdoItem_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() + StrTransferNo;
            saveFileDialog1.Filter = "CSV File (*.CSV)|*.CSV|Excel Files (*.xls)|*.xls|XML Files (*.xml)|*.xml";  //C# Project (*.csproj)|*.csproj|C# Source Files (*.cs)|*.cs|Java File (*.java)|*.java"
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                txtselect.Text = saveFileDialog1.FileName;
        }

        private void txtTrnsferNO_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}