using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;
using System.Xml;
using System.Collections;
using System.Threading;
using Interop.PeachwServer;

namespace UserAutherization
{
    public partial class frmDeliveryNote : Form
    {
        clsCommon objclsCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        bool IsFind = false;

        public frmDeliveryNote()
        {
            setConnectionString();
            InitializeComponent();
            IsFind = false;
        }

        public frmDeliveryNote(string DelNoteNo)
        {
            setConnectionString();
            InitializeComponent();
            IsFind = true;
            flag = true;
        }

        //  Connector conn = new Connector();
        public CSInvoiceSerial ObjSerialInvoice = new CSInvoiceSerial();
        public DSDeliveryNotes DSDispatch = new DSDeliveryNotes();
        public static DateTime UserWiseDate = System.DateTime.Now;
        public static string ConnectionString;
        public string sMsg = "MultiWarehouse Module - Delivery Note";
        public string StrARAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;
        public DataSet dsCustomer;
        public DataSet dsWarehouse;
        public DataSet dsSalesRep;
        public DataSet dsAR;
        public string StrAP = null;
        public string StrSql;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int Decimalpoint = 2;//validate for price
        public int DecimalpointQuantity = 2;//validate for quabtity
        public void load_Decimal()
        {
            try
            {
                string FType = "Price";
                String S = "Select CurentDecimal from tblDecimal where FieldType='" + FType + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                Decimalpoint = Convert.ToInt16(dt.Tables[0].Rows[0].ItemArray[0]);
                //==========================================================================
                string FType1 = "Quantity";
                String S1 = "Select CurentDecimal from tblDecimal where FieldType='" + FType1 + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet dt1 = new DataSet();
                da1.Fill(dt1);
                DecimalpointQuantity = Convert.ToInt16(dt1.Tables[0].Rows[0].ItemArray[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //***************************************************
        public void GetCurrentUserDate()
        {
            try
            {
                dtpDispatchDate.Value = user.LoginDate;
                //String S = "Select CurrentDate from tblUserWiseDate where UserName='" + user.userName.ToString().Trim() + "'";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataSet dt = new DataSet();
                //da.Fill(dt);

                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                //    dtpDispatchDate.Value = UserWiseDate;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //*******************************************************
        private void CreateXmlToExportGRNAdjust(SqlTransaction tr, SqlConnection con)
        {
            // double Qty = 0.0;
            // double Unitprice=0.00;;
            //double PQty = 0;
            //double SysQty = 0;
            //double Adjust_qty = 0;

            string ClosingStockAcc = "";
            SqlCommand myCommand4 = new SqlCommand("Select DelNoteDrAc from tblDefualtSetting", con, tr);
            // SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(myCommand4);
            DataSet dt = new DataSet();
            da.Fill(dt);
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                ClosingStockAcc = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            }

            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
            // string reference = "DelNote" + "-" + DTP.Day + DTP.Month + DTP.Year;
            string reference = txtDeliveryNoteNo.Text.ToString().Trim();
            string Dformat = "MM/dd/yyyy";
            string AdjustmentDate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustmentGRN.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            int GridrowCount = GetFilledRows();
            //create a start elemet=========================

            for (int k = 0; k < GridrowCount; k++)
            {
                //if (Convert.ToDouble(dgvItem[4, k].Value) != 0)
                //{
                // PQty = Convert.ToDouble(dgvItem[4, k].Value);
                // SysQty = Convert.ToDouble(dgvItem[3, k].Value);
                // Adjust_qty = PQty - SysQty;
                double ReceivedQty = Convert.ToDouble(dgvdispactApplytoSales[2, k].Value);
                if (ReceivedQty != 0)
                {
                    Writer.WriteStartElement("PAW_Item");
                    Writer.WriteAttributeString("xsi:type", "paw:item");

                    //crate a ID element (tag)=====================(1)
                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dgvdispactApplytoSales[0, k].Value.ToString().Trim());//dgvItem[0, c].Value
                    Writer.WriteEndElement();

                    //this sis crating tag for reference============(2)
                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(reference);
                    Writer.WriteEndElement();

                    //This is a Tag for Adjusment Date==============(3)
                    Writer.WriteStartElement("Date ");
                    Writer.WriteAttributeString("xsi:type", "paw:date");
                    Writer.WriteString(AdjustmentDate);//Date format must be (MM/dd/yyyy)
                    Writer.WriteEndElement();

                    //This is a Tag for numberof dsistribution=======(4)

                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    //Adjustmne Lines=================================(5)
                    Writer.WriteStartElement("AdjustmentItems");
                    //Adjustmne Lines=================================(6)
                    Writer.WriteStartElement("AdjustmentItem");

                    //Gl ASccount======================================(7)
                    Writer.WriteStartElement("GLSourceAccount ");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(ClosingStockAcc);
                    Writer.WriteEndElement();

                    //Unit Cost========================================(8)
                    //Writer.WriteStartElement("UnitCost");
                    //Writer.WriteString(dgvdispactApplytoSales[5, k].Value.ToString().Trim());
                    //Writer.WriteEndElement();

                    //Quantity========================================(9)

                    Writer.WriteStartElement("Quantity");
                    // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
                    Writer.WriteString("-" + ReceivedQty.ToString().Trim());
                    //Adjust_qty
                    Writer.WriteEndElement();

                    //Amount===========================================(10)
                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(dgvdispactApplytoSales[6, k].Value.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("ReasonToAdjust");
                    Writer.WriteString("Good Delivered");
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();//Adjustment Line
                    Writer.WriteEndElement();//Adjustment lines

                    Writer.WriteEndElement();//Item Closed Tag
                    // Writer.WriteEndElement();//Items Closed Tag
                    //   Writer.Close();//finishing writing xml file
                }
            }

            Writer.Close();//finishing writing xml file
        }

        private void datagridCellEndEditevent()
        {
            try
            {
                int rowCount = GetFilledRows();
                double DispatchQty = 0.0;
                // double RemQty = 0.0;
                double unitprice = 0.00;
                double Amount = 0.00;
                double TotalAmount = 0.00;

                double PreDQty = 0;//previous dispatch quantity
                double NowDQuanty = 0;// changing dispatch quantity
                double DispatchNew = 0;//new dispatch qty


                for (int a = 0; a < rowCount; a++)
                {
                    if (dgvdispactApplytoSales.Rows[a].Cells[2].Value != null && dgvdispactApplytoSales.Rows[a].Cells[5].Value != null)
                    {
                        DispatchQty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[2].Value);
                        unitprice = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[5].Value);
                        Amount = DispatchQty * unitprice;

                        if (Decimalpoint == 0)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString();
                        }
                        else if (Decimalpoint == 1)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N5");
                        }
                        //dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N2");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8

                        ////=========================================
                        PreDQty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[9].Value);
                        NowDQuanty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[2].Value);
                        DispatchNew = NowDQuanty - PreDQty;
                        dgvdispactApplytoSales.Rows[a].Cells[10].Value = DispatchNew;
                        ////if (PreDQty > NowDQuanty)
                        ////{
                        ////    DispatchNew = PreDQty - NowDQuanty;
                        ////}
                        ////else if (PreDQty < NowDQuanty)
                        ////{
                        ////    DispatchNew = NowDQuanty - PreDQty;
                        ////}
                        ////=========================================================================
                    }
                }
                if (Decimalpoint == 0)
                {
                    txtTotalAmount.Text = TotalAmount.ToString();
                }
                else if (Decimalpoint == 1)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N1");
                }
                else if (Decimalpoint == 2)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N2");
                }
                else if (Decimalpoint == 3)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N3");
                }
                else if (Decimalpoint == 4)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N4");
                }
                else if (Decimalpoint == 5)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N5");
                }
                //txtTotalAmount.Text = TotalAmount.ToString("N2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //******************************************************

        public void GetCustomerDataSet()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = " SELECT CutomerID,CustomerName,Address1,Address2,Phone1 FROM tblCustomerMaster order by CutomerID";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtCustomer");

                cmbCustomer.DataSource = dsCustomer.Tables["DtCustomer"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CutomerID";
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["CutomerID"].Width = 75;
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["CustomerName"].Width = 250;
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["Address1"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["Address2"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["Phone1"].Hidden = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetWareHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster order by IsDefault";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 75;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 125;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["ArAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["CashAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["SalesGLAccount"].Hidden = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetARAccount()
        {
            dsAR = new DataSet();
            try
            {
                dsAR.Clear();
                StrSql = " SELECT AcountID, AccountDescription FROM tblChartofAcounts order by AcountID";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsAR, "DtAR");
                cmbAR.DataSource = dsAR.Tables["DtAR"];
                cmbAR.DisplayMember = "AcountID";
                cmbAR.ValueMember = "AcountID";
                cmbAR.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                cmbAR.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 150;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetDefaultAPAccount()
        {
            try
            {
                StrSql = "SELECT [CusretnDrAc] FROM tblDefualtSetting";
                SqlCommand command = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    StrAP = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbAR.Text = StrAP.Trim();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool flag = false;
        private void frmInvoice_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsFind)
                {
                    GetCustomerDataSet();//infragistics
                    dgvdispactApplytoSales.Rows.Clear();
                    GetWareHouseDataSet();//infragistics
                    GetARAccount();//infragistics
                    GetDefaultAPAccount();//infragistics
                    GetCurrentUserDate();
                    load_Decimal();
                    Disable();
                    btnNew_Click(sender, e);
                    clsSerializeItem.DtsSerialNoList.Rows.Clear();
                }

                if (user.IsDNNoAutoGen) txtDeliveryNoteNo.ReadOnly = true;
                else txtDeliveryNoteNo.ReadOnly = false;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
         }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Load_salesOrder()
        {
            try
            {

                dgvdispactApplytoSales.Rows.Clear();
                txtTotalAmount.Text = "0.00";


                clistbxSalesOrder.Items.Clear();
                bool dispatch = false;
                String S = "Select distinct(SalesOrderNo) from tblSalesOrderTemp where CustomerID='" + cmbCustomer.Value.ToString().Trim() + "' and IsfullDispatch='" + dispatch + "' and RemainQty!=0";// and IsSoClosed='" + ISSoClosed + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    clistbxSalesOrder.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void dgvdispactApplytoSales_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int rowCount = GetFilledRows();
                double DispatchQty = 0.0;
                // double RemQty = 0.0;
                double unitprice = 0.00;
                double Amount = 0.00;
                double TotalAmount = 0.00;

                double PreDQty = 0;//previous dispatch quantity
                double NowDQuanty = 0;// changing dispatch quantity
                double DispatchNew = 0;//new dispatch qty

                for (int a = 0; a < rowCount; a++)
                {
                    if (dgvdispactApplytoSales.Rows[a].Cells[2].Value != null && dgvdispactApplytoSales.Rows[a].Cells[5].Value != null)
                    {
                        DispatchQty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[2].Value);
                        unitprice = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[5].Value);
                        Amount = DispatchQty * unitprice;

                        if (Decimalpoint == 0)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString();
                        }
                        else if (Decimalpoint == 1)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N5");
                        }
                        //dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount.ToString("N2");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8

                        ////=========================================
                        PreDQty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[9].Value);
                        NowDQuanty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[2].Value);
                        DispatchNew = NowDQuanty - PreDQty;
                        dgvdispactApplytoSales.Rows[a].Cells[10].Value = DispatchNew;
                        ////if (PreDQty > NowDQuanty)
                        ////{
                        ////    DispatchNew = PreDQty - NowDQuanty;
                        ////}
                        ////else if (PreDQty < NowDQuanty)
                        ////{
                        ////    DispatchNew = NowDQuanty - PreDQty;
                        ////}
                        ////=========================================================================
                    }
                }
                if (Decimalpoint == 0)
                {
                    txtTotalAmount.Text = TotalAmount.ToString();
                }
                else if (Decimalpoint == 1)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N1");
                }
                else if (Decimalpoint == 2)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N2");
                }
                else if (Decimalpoint == 3)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N3");
                }
                else if (Decimalpoint == 4)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N4");
                }
                else if (Decimalpoint == 5)
                {
                    txtTotalAmount.Text = TotalAmount.ToString("N5");
                }
                //txtTotalAmount.Text = TotalAmount.ToString("N2");
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        public void LoadSalesLines(string CheckItem)
        {
            try
            {
                //  String S = "Select * from tblSalesOrder where SalesOrderNo = '" + clistbxSalesOrder.SelectedItem.ToString().Trim() + "'";
                String S = "Select * from tblSalesOrder where SalesOrderNo = '" + CheckItem + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // String S1 = "Select ItemID,Quantity,Description,GLAccount,UnitPrice from tblSalesOrder where SalesOrderNo = '" + clistbxSalesOrder.SelectedItem.ToString().Trim() + "'";
                String S1 = "Select ItemID,Quantity,Description,GLAccount,UnitPrice from tblSalesOrder where SalesOrderNo = '" + CheckItem + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        // dgvdispactApplytoSales[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
                        dgvdispactApplytoSales.Rows.Add();
                        dgvdispactApplytoSales.Rows[i].Cells[0].Value = dt1.Rows[i].ItemArray[0].ToString();
                        dgvdispactApplytoSales.Rows[i].Cells[1].Value = dt1.Rows[i].ItemArray[1].ToString();
                        dgvdispactApplytoSales.Rows[i].Cells[2].Value = "0.00";
                        dgvdispactApplytoSales.Rows[i].Cells[3].Value = dt1.Rows[i].ItemArray[2].ToString();
                        dgvdispactApplytoSales.Rows[i].Cells[4].Value = dt1.Rows[i].ItemArray[3].ToString();
                        dgvdispactApplytoSales.Rows[i].Cells[5].Value = dt1.Rows[i].ItemArray[4].ToString();
                        dgvdispactApplytoSales.Rows[i].Cells[6].Value = "0.00";
                    }
                }
                else
                {
                    //dgvdispactApplytoSales.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void clistbxSalesOrder_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                if (clistbxSalesOrder.CheckedItems.Count >= 1 && e.CurrentValue != CheckState.Checked)
                {
                    e.NewValue = e.CurrentValue;
                    MessageBox.Show("You can only check one item");
                    // AmountCalculation();
                    txtTotalAmount.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        public ArrayList ArraySONO = new ArrayList();//Sales orders related to a custmer

        public string SelectSONums1 = "";//saving purpose


        private void clistbxSalesOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectSONums1 = "";
                string SelectSONums = "";
                dgvdispactApplytoSales.Rows.Clear();
                int step = 0;
                int i = 0;
                ArraySONO.Clear();

                while (i < clistbxSalesOrder.Items.Count)
                {
                    if (clistbxSalesOrder.GetItemChecked(i) == true)
                    {
                        step++;
                        string[] SOIDs = new string[2];
                        SOIDs = clistbxSalesOrder.Items[i].ToString().Split('*');
                        string So_No = SOIDs[0].ToString();

                        string So_No1 = SOIDs[0].ToString();//saving code

                        ArraySONO.Add(So_No);
                        So_No = "'" + So_No + "'";

                        So_No = So_No + ",";
                        SelectSONums = SelectSONums + So_No;

                        So_No1 = So_No1 + " ";//savins purpose
                        SelectSONums1 = SelectSONums1 + So_No1;//saving purpose
                    }
                    i++;
                }

                if (SelectSONums.Length != 0)
                {
                    SelectSONums = SelectSONums.Substring(0, SelectSONums.Length - 1);
                    DataSet ds = new DataSet();

                    ds = ReturnSOList(SelectSONums);

                    string CusPO = ReturnCusPO(SelectSONums);
                    txtCustomerPO.Text = CusPO;

                    //==========================================
                    //try
                    //{
                    //    string jobID = ReturnJobID(SelectSONums);
                    //    txtJob.Text = jobID;
                    //   // txtCustomerPO.Text = CusPO;
                    //}
                    //catch { }
                    //===========================================

                    for (int k = 0; k < ds.Tables[0].Rows.Count - 1; k++)
                    {
                        dgvdispactApplytoSales.Rows.Add();
                    }


                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        //dgvdispactApplytoSales.Rows.Add();
                        dgvdispactApplytoSales.Rows[j].Cells[0].Value = ds.Tables[0].Rows[j].ItemArray[0].ToString().Trim();//ItemID


                        if (DecimalpointQuantity == 0)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString();//Quantity
                            dgvdispactApplytoSales.Rows[j].Cells[3].Value = 0;//Quantity
                        }
                        else if (DecimalpointQuantity == 1)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N1");//Quantity
                            dgvdispactApplytoSales.Rows[j].Cells[3].Value = 0.0;//Quantity
                        }
                        else if (DecimalpointQuantity == 2)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N2");//Quantity
                            dgvdispactApplytoSales.Rows[j].Cells[3].Value = 0.00;//Quantity
                        }
                        else if (DecimalpointQuantity == 3)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N3");//Quantity
                            dgvdispactApplytoSales.Rows[j].Cells[3].Value = 0.000;//Quantity
                        }
                        else if (DecimalpointQuantity == 4)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N4");//Quantity
                            dgvdispactApplytoSales.Rows[j].Cells[3].Value = 0.0000;//Quantity
                        }
                        else if (DecimalpointQuantity == 5)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N5");//Quantity
                            dgvdispactApplytoSales.Rows[j].Cells[3].Value = 0.00000;//Quantity
                        }
                        //dgvdispactApplytoSales.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j].ItemArray[1].ToString().Trim();//Quantity
                        //dgvdispactApplytoSales.Rows[j].Cells[2].Value = "0";//delivery Qty

                        dgvdispactApplytoSales.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j].ItemArray[2].ToString().Trim();
                        dgvdispactApplytoSales.Rows[j].Cells[4].Value = ds.Tables[0].Rows[j].ItemArray[3].ToString().Trim();

                        //dgvdispactApplytoSales.Rows[j].Cells[5].Value = ds.Tables[0].Rows[j].ItemArray[4].ToString().Trim();
                        //dgvdispactApplytoSales.Rows[j].Cells[6].Value = "0";

                        if (Decimalpoint == 0)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString();
                            dgvdispactApplytoSales.Rows[j].Cells[6].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]);
                        }
                        else if (Decimalpoint == 1)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N1");
                            dgvdispactApplytoSales.Rows[j].Cells[6].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N2");
                            dgvdispactApplytoSales.Rows[j].Cells[6].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N3");
                            dgvdispactApplytoSales.Rows[j].Cells[6].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N4");
                            dgvdispactApplytoSales.Rows[j].Cells[6].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            dgvdispactApplytoSales.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N5");
                            dgvdispactApplytoSales.Rows[j].Cells[6].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N5");
                        }
                        //dgvdispactApplytoSales.Rows[j].Cells[8].Value = "";//joid
                        //dgvdispactApplytoSales.Rows[j].Cells[7].Value = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();//Job
                        dgvdispactApplytoSales.Rows[j].Cells[7].Value = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();//UnitofMesure
                        dgvdispactApplytoSales.Rows[j].Cells[8].Value = ds.Tables[0].Rows[j].ItemArray[7].ToString().Trim();//JobID
                        dgvdispactApplytoSales.Rows[j].Cells[11].Value = ds.Tables[0].Rows[j].ItemArray[6].ToString().Trim();//UnitofMesure
                        // txtCustomerPO.Text = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();
                    }
                }

                dgvdispactApplytoSales.Columns[0].ReadOnly = true;
                dgvdispactApplytoSales.Columns[1].ReadOnly = true;
                dgvdispactApplytoSales.Columns[2].ReadOnly = true;
                dgvdispactApplytoSales.Columns[4].ReadOnly = true;
                dgvdispactApplytoSales.Columns[5].ReadOnly = true;
                dgvdispactApplytoSales.Columns[6].ReadOnly = true;
                dgvdispactApplytoSales.Columns[7].ReadOnly = true;
                dgvdispactApplytoSales.Columns[8].ReadOnly = true;

                txtTotalAmount.Text = "";
                //autocalculatedatagridcellendeditevent
                //====================================
                datagridCellEndEditevent();
                //  txtCustomerPO.Text = "";                
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        public DataSet ReturnSOList(string SO_No)
        {
            DataSet ds = new DataSet();

            try
            {
                //group by Item_ID
                // String S = "Select ItemID,Quantity,Description,GLAccount,UnitPrice from tblSalesOrder where SalesOrderNo in (" + SO_No + ")";
                // String S = "Select ItemID,Sum(RemainQty),Description,GLAccount,UnitPrice from tblSalesOrder where SalesOrderNo in (" + SO_No + ")  group by ItemID,Description,GLAccount,UnitPrice";
                //String S = "Select ItemID,Sum(RemainQty),Description,GLAccount,UnitPrice,UOM,JobID from tblSalesOrderTemp where SalesOrderNo in (" + SO_No + ")and IsfullDispatch='false'  group by ItemID,Description,GLAccount,UnitPrice,UOM,JobID";
                // SELECT ItemID, SUM(RemainQty) AS Expr1, Description, GLAccount, UnitPrice, UOM, DisNumber FROM tblSalesOrderTemp WHERE (SalesOrderNo IN ('20002')) AND (IsfullDispatch = 'false') GROUP BY ItemID, Description, GLAccount, UnitPrice, UOM, DisNumber ORDER BY DisNumber
                String S = "SELECT ItemID,SUM(RemainQty) AS Expr1, Description, GLAccount, UnitPrice, UOM, DisNumber,JobID FROM tblSalesOrderTemp WHERE (SalesOrderNo IN (" + SO_No + ")) AND (IsfullDispatch = 'false') GROUP BY ItemID, Description, GLAccount, UnitPrice, UOM, DisNumber,JobID ORDER BY DisNumber";
                //String S = "Select ItemID,Sum(RemainQty),Description,GLAccount,UnitPrice,UOM,DisNumber from tblSalesOrderTemp where SalesOrderNo in (" + SO_No + ")and IsfullDispatch='false'  group by ItemID,Description,GLAccount,UnitPrice,UOM,DisNumber";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                da.Fill(ds, "SO");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public string ReturnCusPO(string SO_No)
        {
            string CusPO = "";

            try
            {
                String S1 = "Select CustomerPO from tblSalesOrderTemp where SalesOrderNo in (" + SO_No + ")";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {

                        CusPO = dt1.Rows[i].ItemArray[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CusPO;
        }

        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvdispactApplytoSales.Rows.Count; i++)
                {
                    if (dgvdispactApplytoSales.Rows[i].Cells[0].Value != null) //change cell value by 1                   
                    {
                        RowCount++;
                    }
                }
                return RowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int ChechDQty = 0;

        public bool IsValidControl(int _RowCount)
        {
            try
            {
                if (!user.IsDNNoAutoGen)
                {
                    if (txtDeliveryNoteNo.Text.Trim() == string.Empty)
                    {
                        MessageBox.Show("Enter Delivery Note No...!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                if (cmbWarehouse.Value == null)
                {
                    MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                if (cmbCustomer.Value == null)
                {
                    MessageBox.Show("Incorrect Customer", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                if (cmbAR.Value == null)
                {
                    MessageBox.Show("Incorrect AR Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                if (_RowCount == 0)
                {
                    MessageBox.Show("Please enter transaction data", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                foreach (DataGridViewRow dr in dgvdispactApplytoSales.Rows)
                {
                    if (dr.Cells[3].Value != null && dr.Cells[3].Value.ToString().Trim().Length > 0 && double.Parse(dr.Cells[3].Value.ToString().Trim()) > 0)
                        if (dr.Cells[3].Value == null || dr.Cells[3].Value.ToString().Trim().Length == 0)
                        {
                            MessageBox.Show("Please select the Job In Sales Order");
                            return false;
                        }
                }
                for (int a = 0; a < _RowCount; a++)
                {
                    // string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + dgvdispactApplytoSales.Rows[a].Cells[0].Value.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);

                    if (dt.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("Item Is not In Item Master File.Please Import the Item from Peachtree.....!");
                        return false;
                    }
                    dt = null;
                    //SqlConnection con = new SqlConnection(ConnectionString);
                    S = "select ISNULL(QTY,0) AS QTY from tblItemWhse where ItemID = '" + dgvdispactApplytoSales.Rows[a].Cells[0].Value.ToString().Trim() + "' and WhseId='" + cmbWarehouse.Text.Trim() + "'";
                    cmd = new SqlCommand(S);
                    da = new SqlDataAdapter(S, ConnectionString);
                    dt = new DataSet();
                    da.Fill(dt);

                    if (!user.IsMinusAllow)
                    {
                        if (double.Parse(dgvdispactApplytoSales.Rows[a].Cells[3].Value.ToString().Trim()) > 0)
                        {
                            if (dt.Tables[0].Rows.Count == 0)
                            {
                                MessageBox.Show("Not Enough Stock to Deliver Item " + dgvdispactApplytoSales.Rows[a].Cells[0].Value, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                            else if (double.Parse(dt.Tables[0].Rows[0][0].ToString()) < double.Parse(dgvdispactApplytoSales.Rows[a].Cells[3].Value.ToString().Trim()))
                            {
                                MessageBox.Show("Not Enough Stock to Deliver Item " + dgvdispactApplytoSales.Rows[a].Cells[0].Value, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }

                    ChechDQty = 0;
                    double ChechDQty1 = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[3].Value);
                    if (Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[3].Value) > Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[2].Value))
                    {
                        if (!user.IsOverSOQty)
                        {
                            MessageBox.Show("Deliver Quantity can not be grater than the Sales Order quantity");
                            return false;
                        }
                    }
                }
                if (!IsSerialNoCorrect())
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Save(int rowCount)
        {
            int DocType = 4;//deliveryNote
            bool QtyIN = false;//reduceing from the stock
            string TranType = "Del-Note";
            //int rowCount = GetFilledRows();
            flag = false;
            bool isminusAllow = true;
            string DeliveryNoteNo = "";

            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvTranDate = DTP.ToString(Dformat);

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction(); ;
            bool IsfullShipment = false;
            bool Isinvoice = false;

            try
            {
                SqlCommand myCommand = null;
                if (user.IsDNNoAutoGen)
                {
                    myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET DeliveryNoteNo = DeliveryNoteNo + 1 select DeliveryNoteNo, DeliveryNotePrefix from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                    DataTable dt41 = new DataTable();
                    da41.Fill(dt41);

                    if (dt41.Rows.Count > 0)
                    {
                        DeliveryNoteNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                        DeliveryNoteNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + DeliveryNoteNo;
                    }
                    txtDeliveryNoteNo.Text = DeliveryNoteNo;
                }
                else
                {
                    myCommand = new SqlCommand("select * from tblDispatchOrder where DeliveryNoteNo='" + txtDeliveryNoteNo.Text.Trim() + "'", myConnection, myTrans);
                    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                    DataTable dt41 = new DataTable();
                    da41.Fill(dt41);

                    if (dt41.Rows.Count > 0)
                    {
                        MessageBox.Show("Delivery Note No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        myTrans.Rollback();
                        myConnection.Close();
                        return;
                    }
                }

                for (int i = 0; i < rowCount; i++)
                {
                    bool Duplicate = true;
                    string NoOfDis = Convert.ToString(dgvdispactApplytoSales.Rows.Count - 1);
                    if (Updateflag == true)
                    {
                        myCommand.CommandText = "Update tblDispatchOrder SET DispatchQty = '" + Convert.ToDouble(dgvdispactApplytoSales[3, i].Value) + "' where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";
                        myCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        myCommand.CommandText = " if(" + Convert.ToDouble(dgvdispactApplytoSales[3, i].Value) + "<>0) insert into tblDispatchOrder(DeliveryNoteNo,CustomerID,SONos,DispatchDate,ArAccount,NoOfDistributions,DistributionNo,ItemID,Description,OrderQty,DispatchQty,GL_Account,UnitPrice,Amount,TotalAmount,CurrentDate,Time,CurrentUser,Duplicate,IsInvoce,CustomePO,UOM,JobID,WareHouseID,SODistributionNO) values ('" + txtDeliveryNoteNo.Text.ToString().Trim() + "','" + cmbCustomer.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + dtpDispatchDate.Value.ToString("MM/dd/yyyy").Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvdispactApplytoSales[0, i].Value + "','" + dgvdispactApplytoSales[1, i].Value + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "','" + Convert.ToDouble(dgvdispactApplytoSales[3, i].Value) + "','" + dgvdispactApplytoSales[4, i].Value + "','" + Convert.ToDouble(dgvdispactApplytoSales[5, i].Value) + "','" + Convert.ToDouble(dgvdispactApplytoSales[6, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy").Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Duplicate + "','" + Isinvoice + "','" + txtCustomerPO.Text.ToString().Trim() + "','" + dgvdispactApplytoSales[7, i].Value + "','" + dgvdispactApplytoSales[8, i].Value.ToString() + "','" + cmbWarehouse.Value.ToString().Trim() + "','" + Convert.ToDouble(dgvdispactApplytoSales[11, i].Value) + "')";
                        myCommand.ExecuteNonQuery();
                        double ItemCost = 0.00;
                        SqlCommand cmd34 = new SqlCommand("select UnitCost from tblItemMaster where ItemID='" + dgvdispactApplytoSales[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                        SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                        DataTable dt34 = new DataTable();
                        da34.Fill(dt34);
                        if (dt34.Rows.Count > 0)
                        {
                            ItemCost = Convert.ToDouble(dt34.Rows[0].ItemArray[0]);
                        }

                        SqlCommand myCommand2 = new SqlCommand("Select * from  tblItemWhse where ItemId='" + dgvdispactApplytoSales[0, i].Value + "' and WhseId='" + cmbWarehouse.Value.ToString().Trim() + "'", myConnection, myTrans);
                        SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);

                        if (dt1.Rows.Count > 0)
                        {
                            myCommand.CommandText = "update tblItemWhse set QTY = QTY - '" + Convert.ToDouble(dgvdispactApplytoSales[3, i].Value) + "' where ItemId='" + dgvdispactApplytoSales[0, i].Value + "' and WhseId='" + cmbWarehouse.Value.ToString().Trim() + "'";
                            myCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            myCommand.CommandText = "insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbWarehouse.Value.ToString().Trim() + "','" + Convert.ToString(dgvdispactApplytoSales[0, i].Value) + "','" + Convert.ToString(dgvdispactApplytoSales[1, i].Value) + "','" + Convert.ToDouble(dgvdispactApplytoSales[3, i].Value) + "','" + Convert.ToString(dgvdispactApplytoSales[7, i].Value) + "','" + InvTranDate + "')";
                            myCommand.ExecuteNonQuery();
                        }

                        SqlCommand cmd11 = new SqlCommand(
                            " declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + cmbWarehouse.Text.ToString().Trim() + "' AND ItemId='" + dgvdispactApplytoSales[0, i].Value + "') " +
                            "if(" + Convert.ToDouble(dgvdispactApplytoSales[3, i].Value) + "<>0) Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY,'" + DocType + "','" + txtDeliveryNoteNo.Text.ToString().Trim() + "','" + dtpDispatchDate.Value.ToString("MM/dd/yyyy").Trim() + "','" + TranType + "','" + QtyIN + "','" + dgvdispactApplytoSales[0, i].Value + "','" + Convert.ToDouble(dgvdispactApplytoSales[3, i].Value) + "','" + ItemCost + "','" + Convert.ToDouble(dgvdispactApplytoSales[3, i].Value) * ItemCost + "','" + cmbWarehouse.Value.ToString().Trim() + "','" + Convert.ToDouble(dgvdispactApplytoSales[5, i].Value) + "')", myConnection, myTrans);
                        cmd11.ExecuteNonQuery();
                        
                        bool IsGRNProcess = true;
                        string Stutas = "Sold";

                        //========================================================================
                    }
                    //==========================================setfull dispath
                    if (Updateflag == true)
                    {
                        string UpdateSO = clistbxSalesOrder.Items[0].ToString();
                        ArraySONO.Add(UpdateSO);
                    }
                    if (Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells[2].Value.ToString()) == Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells[3].Value.ToString()))//changed cell value
                    {
                        IsfullShipment = true;
                        SetFullDispatch(ArraySONO, dgvdispactApplytoSales.Rows[i].Cells[0].Value.ToString(), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells[3].Value.ToString()), IsfullShipment, int.Parse(dgvdispactApplytoSales.Rows[i].Cells["SODistributionNO"].Value.ToString()));
                    }
                    else
                    {
                        if (Updateflag == true)
                        {
                            UpdateSoTemptbl(ArraySONO, dgvdispactApplytoSales.Rows[i].Cells[0].Value.ToString(), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells[10].Value.ToString()), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells[5].Value.ToString()), int.Parse(dgvdispactApplytoSales.Rows[i].Cells["SODistributionNO"].Value.ToString()));
                        }
                        else
                        {
                            UpdateSoTemptbl(ArraySONO, dgvdispactApplytoSales.Rows[i].Cells[0].Value.ToString(), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells[3].Value.ToString()), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells[5].Value.ToString()), int.Parse(dgvdispactApplytoSales.Rows[i].Cells["SODistributionNO"].Value.ToString()));
                        }
                    }
                }//End of the for loop datagrid

                foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                {
                    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                        " TranType='Del-Note',Status='OutOfStock' " +
                        " where ItemID='" +
                        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                    myCommandSe1.ExecuteNonQuery();
                }

                frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Del-Note", cmbWarehouse.Text.ToString(), txtDeliveryNoteNo.Text.ToString().Trim(), dtpDispatchDate.Value, true, string.Empty);
                  
                
                myTrans.Commit();

                MessageBox.Show("Delivery Note Successfully Saved");

            }//end of the trnasaction scope 
            catch (Exception ex)
            {
                myTrans.Rollback();
                throw ex;
            }
        }

        public Boolean IsGridValidation()
        {
            try
            {
                double dilqty = 0;
                if (dgvdispactApplytoSales.Rows.Count == 0)
                {
                    return false;
                }

                foreach (DataGridViewRow ugR in dgvdispactApplytoSales.Rows)
                {
                    if (ugR.Cells["DeliverQty"].Value != null)
                    {
                        dilqty = dilqty + double.Parse(ugR.Cells["DeliverQty"].Value.ToString());

                        if (double.Parse(ugR.Cells["DeliverQty"].Value.ToString()) < 0)
                        {
                            MessageBox.Show("Enter Valid Quantity of Item ID '" + ugR.Cells["ItemID"].Value.ToString() + "'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                if (dilqty <= 0)
                {
                    MessageBox.Show("Invalid Delivery Quantity", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }
        }





        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dgvdispactApplytoSales.EndEdit();
                if (dtpDispatchDate.Value < user.Period_begin_Date)
                {
                    MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (dtpDispatchDate.Value > user.Period_End_Date)
                {
                    MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }


                if (!objControlers.HeaderValidation_Customer(cmbCustomer.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_Warehouse(cmbWarehouse.Text, sMsg)) return;

                int rowCount = GetFilledRows();
                if (IsGridValidation() == false)
                {
                    return;
                }

                if (IsValidControl(rowCount))
                {
                    Save(rowCount);
                    btnNew_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        //get order qtry fro so temp table=======================

        public Double GetOrdQTY1(string poid, string ItemID, double UPrice, int DisNo)
        {
            try
            {
                string POid = poid.Trim();
                setConnectionString();
                Double OrdQty = 0;
                string ConnString = ConnectionString;
                string sql = "select RemainQty from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' AND DisNumber='" + DisNo + "'";// and UnitPrice=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrdQty = reader.GetDouble(0);
                }
                reader.Close();
                Conn.Close();
                return OrdQty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==============================================================

        //==============================================================
        public Double GetPrevOrdQTY(string poid, string ItemID, double UPrice, int DisNo)
        {
            try
            {
                string POid = poid.Trim();
                setConnectionString();
                Double OrdQty = 0;
                string ConnString = ConnectionString;
                // string sql = "select QTY from PO_Temp where PO_NO='" + POid + "' and Item_ID='" + ItemID + "' and Unit_Price=" + UPrice + "";
                string sql = "select RemainQty from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' AND DisNumber='" + DisNo + "'";// and UnitPrice=" + UPrice + "";
                // string sql = "select Quantity from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrdQty = reader.GetDouble(0);
                }
                reader.Close();
                Conn.Close();
                return OrdQty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================================================================
        //======================================================

        public bool GetIem(string poid, string ItemID)
        {
            try
            {
                string POid = poid.Trim();
                bool IsAvalble = false;
                setConnectionString();
                //  Double OrdQty = 0;
                string ConnString = ConnectionString;
                // string sql = "select QTY from PO_Temp where PO_NO='" + POid + "' and Item_ID='" + ItemID + "' and Unit_Price=" + UPrice + "";
                string sql = "select ItemID from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "'";
                // string sql = "select Quantity from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd2 = new SqlCommand(sql);
                SqlDataAdapter da2 = new SqlDataAdapter(sql, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    IsAvalble = true;
                }
                return IsAvalble;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=========================================================



        public Double GetPrevOrginalQty(string poid, string ItemID, double UPrice)
        {
            try
            {
                string POid = poid.Trim();
                setConnectionString();
                Double DispatchQ = 0;
                string ConnString = ConnectionString;
                // string sql = "select QTY from PO_Temp where PO_NO='" + POid + "' and Item_ID='" + ItemID + "' and Unit_Price=" + UPrice + "";
                string sql = "select DispatchQty from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
                // string sql = "select Quantity from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DispatchQ = reader.GetDouble(0);
                }
                reader.Close();
                Conn.Close();
                return DispatchQ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //=================================================================


        public void SetDispatchFOR_Partial(string SOID, string ItemID, bool FullShip, double DispatchQty, int DisNo)
        {
            try
            {
                bool fullshipment = FullShip;
                double OrginalQty = 0;
                double UpdateDispatchQty = 0;
                double RemainQty = 0;

                String S = "Select Quantity,DispatchQty,RemainQty from tblSalesOrderTemp where SalesOrderNo = '" + SOID + "' and ItemID='" + ItemID + "' AND DisNumber='" + DisNo + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt1 = new DataTable();
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int k = 0; k < dt1.Rows.Count; k++)
                    {
                        OrginalQty = Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                        UpdateDispatchQty = Convert.ToDouble(dt1.Rows[k].ItemArray[1].ToString());
                        RemainQty = Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                    }
                }

                UpdateDispatchQty = UpdateDispatchQty + DispatchQty;
                RemainQty = OrginalQty - UpdateDispatchQty;
                if (RemainQty <= 0)
                {
                    RemainQty = 0;
                    UpdateDispatchQty = OrginalQty;
                    fullshipment = true;
                }

                string ConnString = ConnectionString;
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd1 = Conn.CreateCommand();
                Conn.Open();
                cmd1.CommandText = "UPDATE tblSalesOrderTemp SET IsfullDispatch = '" + fullshipment + "',DispatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SalesOrderNo = '" + SOID + "' and ItemID='" + ItemID + "' AND DisNumber='" + DisNo + "'";

                cmd1.ExecuteNonQuery();
                Conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=========================================================

        public void UpdateSOTemp(string PO_NO_update, string ItemID_update, double ReceivableQty_update, double DispatchQ, double UnitPrice_update, int DisNo)
        {
            try
            {
                bool fullDispath = false;
                double updateQty = 0;
                double OriginalQty = 0;
                double RemainQty = 0;

                if (ReceivableQty_update == 0)
                {
                    fullDispath = true;
                }
                else
                {
                    fullDispath = false;
                }

                String S = "Select DispatchQty,Quantity from tblSalesOrderTemp where SalesOrderNo = '" + PO_NO_update + "' and ItemID='" + ItemID_update + "' AND DisNumber='" + DisNo + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt1 = new DataTable();
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int k = 0; k < dt1.Rows.Count; k++)
                    {
                        updateQty = Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                        OriginalQty = Convert.ToDouble(dt1.Rows[k].ItemArray[1].ToString());
                    }
                }

                updateQty = updateQty + DispatchQ;
                RemainQty = OriginalQty - updateQty;

                string ConnString = ConnectionString;
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd1 = Conn.CreateCommand();
                cmd1.CommandText = "UPDATE tblSalesOrderTemp SET RemainQty=" + RemainQty + ",DispatchQty='" + updateQty + "',IsfullDispatch='" + fullDispath + "' where SalesOrderNo='" + PO_NO_update + "'and ItemID='" + ItemID_update + "' AND DisNumber='" + DisNo + "'";
                Conn.Open();
                cmd1.ExecuteNonQuery();
                Conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //=========================================================
        public void UpdateSoTemptbl(ArrayList SOIDs, string ItemID, double dispatchQty, double UPrice, int DisNo)
        {
            try
            {
                bool FullDispatch = true;
                double DispathQty = 0;
                double Extra_qty = 0;
                double Extra_qty1 = 0;
                double ParialDispatchQty = dispatchQty;
                double ParialDispatchQty1 = dispatchQty;
                double DispatchableQty = 0;
                double PrevOrderdqty = 0;//quantity of the pevious line
                double prevqtyTemp = 0;
                bool checkItem = false;
                bool ABC = false;
                bool PQR = false;
                for (int i = 0; i < SOIDs.Count; i++)
                {
                    string SOID = SOIDs[i].ToString();
                    string Item_ID = ItemID;
                    double orderdqty = GetOrdQTY1(SOID, ItemID, UPrice, DisNo);//order quantity mean Actual Remain Qty
                    if (orderdqty != 0)
                    {
                        if (orderdqty <= dispatchQty)
                        {
                            Extra_qty1 = ParialDispatchQty1 - orderdqty;
                            Extra_qty = dispatchQty - orderdqty;
                            dispatchQty = Extra_qty;
                            if (Extra_qty == 0)
                            {
                                if (ABC == false)
                                {
                                    SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty1, DisNo);
                                }
                            }
                            else if (Extra_qty > 0)
                            {
                                ParialDispatchQty = ParialDispatchQty1 - Extra_qty1;

                                if (i == 0)
                                {
                                    prevqtyTemp = Extra_qty1;
                                    SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty, DisNo);
                                    string SOID3 = SOIDs[i].ToString();
                                    checkItem = GetIem(SOID3, ItemID);
                                    if (checkItem == true)
                                    {
                                        // SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty);
                                        ABC = true;

                                    }
                                }
                                else
                                {
                                    string SOID2 = SOIDs[i - 1].ToString();
                                    PrevOrderdqty = GetPrevOrdQTY(SOID2, ItemID, UPrice, DisNo);
                                    if (PrevOrderdqty == 0)
                                    {
                                        DispatchableQty = dispatchQty;
                                        FullDispatch = false;
                                        SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty, DisNo);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (PQR == true)
                            {
                                ABC = true;
                            }
                            else
                            {
                                ABC = false;
                            }

                            if (i == 0)
                            {
                                ABC = true;
                                DispathQty = orderdqty - dispatchQty;
                                {
                                    UpdateSOTemp(SOIDs[i].ToString(), ItemID, DispathQty, dispatchQty, UPrice, DisNo);
                                    PQR = true;
                                }
                            }
                            else
                            {
                                string SOID1 = SOIDs[i - 1].ToString();
                                PrevOrderdqty = GetPrevOrdQTY(SOID1, ItemID, UPrice, DisNo);

                                if (ABC == false)
                                {
                                    if (PrevOrderdqty == 0)
                                    {
                                        DispathQty = orderdqty - prevqtyTemp;
                                        {
                                            UpdateSOTemp(SOIDs[i].ToString(), ItemID, DispathQty, dispatchQty, UPrice, DisNo);
                                            ABC = true;
                                            PQR = true;
                                        }
                                    }
                                }
                            }
                        }
                    }//if(orderQty==0)               
                }
                //========================================
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SetFullDispatch(ArrayList SOID, string ItemID, double DispatchQty, bool FullDispatch, int DisNo)
        {
            try
            {
                //try
                //{
                double OrginalQty = 0;
                double UpdateDispatchQty = 0;
                double RemainQty = 0;
                // bool   FullDispatch = true;
                //DataSet ds = new DataSet();

                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                SqlCommand myCommand = new SqlCommand();
                myConnection.Open();
                myCommand.Connection = myConnection;
                for (int i = 0; i < SOID.Count; i++)
                {
                    //================================================


                    //try
                    //{
                    String S = "Select Quantity,DispatchQty,RemainQty from tblSalesOrderTemp where SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "' AND DisNumber='" + DisNo + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        for (int k = 0; k < dt1.Rows.Count; k++)
                        {
                            OrginalQty = Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                            UpdateDispatchQty = Convert.ToDouble(dt1.Rows[k].ItemArray[1].ToString());
                            RemainQty = Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                        }
                    }

                    //}
                    //catch
                    //{

                    //}
                    UpdateDispatchQty = UpdateDispatchQty + DispatchQty;
                    RemainQty = OrginalQty - UpdateDispatchQty;
                    if (RemainQty <= 0)
                    {
                        // UpdateDispatchQty
                        RemainQty = 0;
                        UpdateDispatchQty = OrginalQty;
                        // FullDispatch = true;

                    }
                    //kjkjkjkfjfjf
                    //=====================================


                    // myCommand.CommandText = "UPDATE tblSalesOrder SET IsfullDispatch = '" + FullDispatch + "',DispatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SONumber = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    //// myCommand.CommandText = "UPDATE tblSalesOrder SET IsfullDispatch = '" + FullDispatch + "' WHERE SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    // myCommand.ExecuteNonQuery();

                    myCommand.CommandText = "UPDATE tblSalesOrderTemp SET IsfullDispatch = '" + FullDispatch + "',DispatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "' and DisNumber='" + DisNo + "'";
                    // myCommand.CommandText = "UPDATE tblSalesOrderTemp SET IsfullDispatch = '" + FullDispatch + "' WHERE SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    myCommand.ExecuteNonQuery();




                    // myCommand.CommandText = "UPDATE tblSalesOrder SET Quantity = '" + OrginalQty  + "',DispatchQty='"+ DispatchQty  +"',RemainQty='"+ DispatchQty +"' WHERE SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    // cmd.CommandText = "UPDATE Purchase_Order SET IsFullShipment = '" + FullShip + "' WHERE PO_NO = '" + POID[i].ToString() + "' and Item_ID='" + ItemID + "';";
                    // cmd1.CommandText = "UPDATE PO_Temp SET IsFullShipment = '" + FullShip + "' WHERE PO_NO = '" + POID[i].ToString() + "' and Item_ID='" + ItemID + "';";
                    //cmd.ExecuteNonQuery();
                    //cmd1.ExecuteNonQuery();
                }
                myConnection.Close();
                //}
                //catch { }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //=============================================================




        public int getNexNo()//get the next dispatch link number
        {
            int NDispachLink = 0;
            try
            {
                String S1 = "Select max(DispachLink) from tblDispatchHeader";// where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                // int CDispachLink = 0;

                if (dt.Rows.Count > 0)
                {
                    NDispachLink = Convert.ToInt32(dt.Rows[0].ItemArray[0]) + 1;
                }
                else
                {
                    NDispachLink = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NDispachLink;
        }

        //======================================================================


        public int getNextDisTrans()//get the next dispatch transaction number link number
        {
            int NDispachTranLink = 0;

            try
            {
                String S1 = "Select max(DispatchTranLink) from tblDispatchTransaction";// where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                // int CDispachLink = 0;

                if (dt.Rows.Count > 0)
                {
                    NDispachTranLink = Convert.ToInt32(dt.Rows[0].ItemArray[0]) + 1;
                }
                else
                {
                    NDispachTranLink = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NDispachTranLink;
        }


        //======================================================================

        public string NextDeliveryNoteNo()//get the next dispatch link number
        {
            string NextDNoteNo = "";

            try
            {
                string ConnString = ConnectionString;
                string sql = "Select DeliveryNoteNo from tblDispatchOrder ORDER BY DeliveryNoteNo";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    // NextDNoteNo =ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    //   int p = ds.Tables[0].Rows.Count - 1;
                    NextDNoteNo = getNextID(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1].ItemArray[0].ToString());

                    //  txtReceiptNo.Text = NewID;
                }
                else
                {
                    //String S2 = "Select DeliveryNoteNo from tblDefaultSetting";
                    //SqlCommand cmd2 = new SqlCommand(S2);
                    //SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    //DataTable dt1 = new DataTable();
                    //da2.Fill(dt1);

                    //if (dt1.Rows.Count > 0)
                    //{
                    //  //  NextDNoteNo = dt1.Rows[0].ItemArray[0].ToString().Trim();
                    NextDNoteNo = "D0-100000";
                    //}
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NextDNoteNo;
        }

        //=====================================GEt next ID===================


        public string getNextID(string s)
        {
            try
            {
                int i = 0;
                string nextID = "";
                while (i < s.Length - 1)
                {
                    if ((Char.IsDigit(s[i]) && Char.IsLetter(s[i + 1])) || (Char.IsLetter(s[i]) && Char.IsDigit(s[i + 1]) || ((s[i] == '-')) || ((s[i] == ' '))))
                    {
                        s = s.Insert(i + 1, "*");
                    }
                    i++;
                }
                bool Islarge = false;
                string[] arr = s.Split('*');
                i = arr.Length - 1;
                for (int no = i; no >= 0; no--)
                {
                    if (arr[i].Length > 19)
                    {
                        Islarge = true;
                    }
                    else
                    {
                        Islarge = false;
                    }
                }
                if (Islarge == false)
                {
                    ///'''''''''''''''''''''''''''''''''
                    while (i >= 0)
                    {
                        try
                        {
                            //if (arr[i].Length<=19)
                            //{
                            long no = long.Parse(arr[i]);
                            i = 0;
                            while (i < arr.Length)
                            {
                                if (arr[i] == no.ToString())
                                {
                                    no++;
                                    arr[i] = no.ToString();
                                }
                                nextID = nextID + arr[i];
                                i++;
                            }
                            return nextID;

                        }
                        catch { }


                        if (i != 0)
                        {
                            i--;
                        }
                    }


                    return s + "1";
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Enable()
        {
            btnNew.Enabled = true;
            btnSave.Enabled = true;
            btnPrint.Enabled = false;
            btnclose.Enabled = true;
            btnList.Enabled = true;

            dgvdispactApplytoSales.Enabled = true;
            clistbxSalesOrder.Enabled = true;
            dtpDispatchDate.Enabled = true;
        }

        public void Disable()
        {
            txtcusName.Enabled = false;
            txtcusCity.Enabled = false;
            txtCusAdd1.Enabled = false;
            txtCusAdd2.Enabled = false;
            txtlocName.Enabled = false;
            //txtDeliveryNoteNo.Enabled = false;
            txtTotalAmount.Enabled = false;
            dgvdispactApplytoSales.Enabled = true;
            clistbxSalesOrder.Enabled = false;
            dtpDispatchDate.Enabled = false;
            txtCustomerPO.Enabled = false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                clsSerializeItem.DtsSerialNoList.Rows.Clear();
                GetWareHouseDataSet();
                GetCustomerDataSet();
                GetARAccount();
                flag = false;
                cmbCustomer.Text = "";
                dgvdispactApplytoSales.Rows.Clear();
                clistbxSalesOrder.Items.Clear();
                txtCustomerPO.Text = "";
                dgvdispactApplytoSales.Columns[3].ReadOnly = false;
                txtcusName.Text = "";
                txtcusCity.Text = "";
                txtCusAdd1.Text = "";
                txtCusAdd2.Text = "";
                txtDeliveryNoteNo.Text = "";
                txtTotalAmount.Text = "0.00";
                dtpDispatchDate.Value = user.LoginDate;

                cmbWarehouse.Text = user.StrDefaultWH;
                //  txtlocName.Text = "";
                Enable();

                if (user.IsDNNoAutoGen) txtDeliveryNoteNo.ReadOnly = true;
                else txtDeliveryNoteNo.ReadOnly = false;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

            flag = false;
            DSDispatch.Clear();

            try
            {
                String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DSDispatch, "CustomerMaster");

                // DSDispatch.Clear();

                String S2 = "Select SalesOrderNo,CustomerPO from tblSalesOrderTemp";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlConnection con2 = new SqlConnection(ConnectionString);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, con2);
                da2.Fill(DSDispatch, "dtSalesOrder");

                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(DSDispatch, "dt_CompanyDetails");

                //String S2 = "Select * from tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                // SqlCommand cmd2 = new SqlCommand(S2);
                //SqlConnection con2 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(S2, con2);
                // da2.Fill(DSDispatch, "ItemMaster");



                String S3 = "Select * from tblDispatchOrder where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(DSDispatch, "DtDeliveryNote");

                frmDNotePrint dnp = new frmDNotePrint(this);
                dnp.Show();

                // frmDeiveryNotePrint frm = new frmDeiveryNotePrint(this);
                // frm.Show();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                flag = true;
                //frmDNoteList ObjDNoteList = new frmDNoteList();
                //ObjDNoteList.ShowDialog();ObjDNoteList = new frmDNoteList();

                if (frmMain.ObjDNoteList == null || frmMain.ObjDNoteList.IsDisposed)
                {
                    frmMain.ObjDNoteList = new frmDNoteList(1);
                }
                frmMain.ObjDeliveryNote.TopMost = false;
                frmMain.ObjDNoteList.ShowDialog();
                frmMain.ObjDNoteList.TopMost = true;
                //ObjDeliveryNote = new frmDeliveryNote();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        ClassDriiDown ab = new ClassDriiDown();
        public bool Updateflag = false;
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = true;
                Updateflag = true;
                dgvdispactApplytoSales.Columns[2].ReadOnly = false;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmDeliveryNote_Activated(object sender, EventArgs e)
        {
            try
            {
                if (flag == true)
                {
                    btnSave.Enabled = false;
                    clistbxSalesOrder.Items.Clear();
                    // string DeliveryNoteNo = ab.GetText2();
                    string SerchText = ab.GetText2();

                    string ConnString = ConnectionString;
                    String S1 = "Select * from tblDispatchOrder where DeliveryNoteNo='" + SerchText + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        cmbWarehouse.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
                        txtDeliveryNoteNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                        cmbCustomer.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        dtpDispatchDate.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                        cmbAR.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                        txtTotalAmount.Text = dt.Rows[0].ItemArray[13].ToString().Trim();
                        clistbxSalesOrder.Items.Add(dt.Rows[0].ItemArray[2].ToString().Trim());
                        txtCustomerPO.Text = dt.Rows[0].ItemArray[20].ToString().Trim();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            //txtDeliveryNoteNo.Enabled = false;

                            dtpDispatchDate.Enabled = false;
                            txtTotalAmount.Enabled = false;
                            clistbxSalesOrder.Enabled = false;
                            txtCustomerPO.Enabled = false;
                            dgvdispactApplytoSales.Columns[0].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[1].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[2].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[3].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[4].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[5].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[6].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[7].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[8].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[9].ReadOnly = true;
                            dgvdispactApplytoSales.Columns[10].ReadOnly = true;

                            txtcusName.Enabled = false;
                            txtCusAdd1.Enabled = false;
                            txtCusAdd2.Enabled = false;

                            dgvdispactApplytoSales.Rows.Add();

                            dgvdispactApplytoSales.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                            dgvdispactApplytoSales.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[8].ToString().Trim();

                            // DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvdispactApplytoSales.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[9].ToString().Trim();
                            dgvdispactApplytoSales.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                            dgvdispactApplytoSales.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[11].ToString().Trim();
                            dgvdispactApplytoSales.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[12].ToString().Trim();
                            dgvdispactApplytoSales.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[13].ToString().Trim();
                            dgvdispactApplytoSales.Rows[i].Cells[8].Value = dt.Rows[i].ItemArray[22].ToString().Trim();
                            dgvdispactApplytoSales.Rows[i].Cells[9].Value = dt.Rows[i].ItemArray[10].ToString().Trim();//updatedQty
                            dgvdispactApplytoSales.Rows[i].Cells[10].Value = "0";
                        }
                    }
                    btnPrint.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnSNO_Click(object sender, EventArgs e)
        {
            // CheckAcivated = false;
            try
            {
                int rowCount = GetFilledRows();

                dgvdispactApplytoSales.CommitEdit(DataGridViewDataErrorContexts.Commit);

                if (rowCount == 0 || Convert.ToDouble(dgvdispactApplytoSales[3, dgvdispactApplytoSales.CurrentRow.Index].Value) == 0 || cmbWarehouse.Text == "")
                {
                    if (rowCount == 0)
                    {
                        MessageBox.Show("please Select a line That contain Serialized Item");
                    }
                    if (Convert.ToDouble(dgvdispactApplytoSales[3, dgvdispactApplytoSales.CurrentRow.Index].Value) == 0)
                    {
                        MessageBox.Show("please Enter Despatch Quantity");
                    }
                    if (cmbWarehouse.Text == "")
                    {
                        MessageBox.Show("please Select a Warehouse");
                    }
                }
                else
                {
                    string ItemID = dgvdispactApplytoSales[0, dgvdispactApplytoSales.CurrentRow.Index].Value.ToString().Trim();
                    //check wether this item is serial ior not  by classs
                    string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + ItemID + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);

                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                    }

                    if (ItemClass == "10" || ItemClass == "11")
                    {
                        frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Del-Note", cmbWarehouse.Text.ToString().Trim(),
                       ItemID, Convert.ToDouble(dgvdispactApplytoSales[3, dgvdispactApplytoSales.CurrentRow.Index].Value),
                       txtDeliveryNoteNo.Text.Trim(), flag, clsSerializeItem.DtsSerialNoList, null, false,true);
                        ObjfrmSerialSubCommon.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("You must select a Serialize Stock Item");
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbCustomer_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtcusName.Text = cmbCustomer.ActiveRow.Cells[1].Value.ToString();
                        txtCusAdd1.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString();
                        txtCusAdd2.Text = cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                        Load_salesOrder();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbWarehouse_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtlocName.Text = cmbWarehouse.ActiveRow.Cells[1].Value.ToString();
                        StrARAccount = cmbWarehouse.ActiveRow.Cells[2].Value.ToString();
                        StrCashAccount = cmbWarehouse.ActiveRow.Cells[3].Value.ToString();
                        StrSalesGLAccount = cmbWarehouse.ActiveRow.Cells[4].Value.ToString();
                        cmbAR.Text = StrARAccount;
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                Connector conn = new Connector();
                conn.ImportSalesOrderListD();
                conn.InsertSOD();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ultraPanel1_PaintClient(object sender, PaintEventArgs e)
        {

        }

        private void dgvdispactApplytoSales_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
           // MessageBox.Show("abcd");
        }

        private void dgvdispactApplytoSales_Leave(object sender, EventArgs e)
        {
           // MessageBox.Show("abcd");
        }

        public bool IsSerialNoCorrect()
        {
            try
            {
                //DataTable table = DataSet1.Tables["Orders"];
                int _Count = 0;
                // Presuming the DataTable has a column named Date. 
                string expression;
                foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                {
                    if (dgvr.Cells[0].Value != null)
                    {
                        if (IsThisItemSerial(dgvr.Cells[0].Value.ToString().Trim()) && double.Parse(dgvr.Cells["DeliverQty"].Value.ToString())>0)
                        {
                            if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0 )
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells[0].Value.ToString().Trim(), "Delivery Note", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            _Count = 0;
                            expression = "ItemCode = '" + dgvr.Cells[0].Value.ToString().Trim() + "'";
                            DataRow[] foundRows;

                            // Use the Select method to find all rows matching the filter.
                            foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                            // Print column 0 of each returned row. 
                            for (int i = 0; i < foundRows.Length; i++)
                            {
                                _Count = i + 1;
                            }

                            if (_Count > 0 && double.Parse(dgvr.Cells["DeliverQty"].Value.ToString()) == 0)
                            {
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                                }
                            }

                            if (_Count != double.Parse(dgvr.Cells["DeliverQty"].Value.ToString()))
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells[0].Value.ToString().Trim(), "Delivery Note", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsThisItemSerial(string _ItemCode)
        {
            try
            {
                bool IsThisItemSerial = false;
                string ItemClass = "";
                String S = "Select * from tblItemMaster where ItemID  = '" + _ItemCode + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                }
                if (ItemClass == "10" || ItemClass == "11")
                {
                    IsThisItemSerial = true;
                }
                return IsThisItemSerial;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}