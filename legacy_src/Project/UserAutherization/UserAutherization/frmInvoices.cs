using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Net;
using System.Net.Mail;
using System.Collections.Specialized;
using sendmail.CommunicationManager;
using sendmail.DTOs;



namespace UserAutherization
{
    public partial class frmInvoices : Form
    {
        public static bool GoEdit = false;
        public static bool Close = false;
        Controlers objControlers = new Controlers();
        clsCommon objclsCommon = new clsCommon();
        public DSEstimate DsEst = new DSEstimate();
        bool run;
        bool add;
        bool edit;
        bool delete;
        DataTable dtUser = new DataTable();
        public string StrSql = null;
        public string StrSql1 = null;
        public double dblLineTot;
        public double dblSubTot;
        public double dblGrocessAmt;
        public double dblGrandTot;
        public double dblDiscAmt;
        public double dblDiscPer;
        public double dblVatAmount;
        public double dblNbtAmount;
        public double dblNetAmount;
        public double dblVat;
        public double dblNbt;
        public double dblServiceAmount;

        public static string ConnectionString;
        public static string SmsMassage;


        public Boolean PayValChecked;
        int intEstomateProcode;
        public Boolean blnEdit;
        public int intProcess;
        public string StrPaymmetM;
        public string StrRetailCustomer;
        public Boolean blnRetailsCustomer;
        //Following Code Segment Define GL Accounts
        public string sMsg = "Warehouse Management - Invoice";
        public string StrARAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;
        public double dblQtyLevel1;
        public double dblQtyLevel2;
        public double dblQtyLevel3;
        public bool IsEdit;
        public DataSet dsCustomer;
        public DataSet dsWarehouse;
        public DataSet dsSalesRep;
        public DataSet dsAR;
        bool IsFind = false;
        UltraGridRow ugR;
        public int TaxINCType = 1;
        public int InvoiceType = 1;
        public int dblCusPriceLevel = 0;
        public DsItemWiseSales DsItemWise = new DsItemWiseSales();
        public Boolean IsGLok = false;
        public static DateTime UserWiseDate = System.DateTime.Now;

        public static string LineDisitemid, LineDisitemdescription, LineDisGLAccount, SpecialDisItemid, SpecialDisItemdescription, SpecialDisGLAccount, Cashitemid, cashitemdis, cashGL, NBitemid, NBTitemDis, NBTitemGL, VATitemid, VATitemDis, VATGL, SERID, SERDIS, SERGL;
        public static bool PDFViewer;
        public frmInvoices(string intNo)
        {
            try
            {
                InitializeComponent();
                setConnectionString();
                setSMsString();
                IsFind = true;
                txtInvoiceNo.Text = intNo;
                GetChargeItems();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public frmInvoices(int intNo)
        {
            try
            {
                InitializeComponent();
                setConnectionString();

                if (intNo == 0)
                {
                    intEstomateProcode = 0;
                }
                IsFind = false;
                GetChargeItems();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void setValue()
        {
            try
            {
                string strNo = (Search.searchIssueNoteNo);
                Search.searchIssueNoteNo = "";
                LoadCreditcardData();

                if (strNo == "")
                {
                    strNo = "";
                }
                else
                {
                    StrSql = "SELECT tblSalesInvoices.DistributionNo,tblSalesInvoices.ItemID,tblSalesInvoices.Description,tblSalesInvoices.Qty,tblSalesInvoices.UnitPrice,tblSalesInvoices.Amount,tblItemWhse.QTY as WHQTY,tblSalesInvoices.ItemClass,tblSalesInvoices.ItemType,tblSalesInvoices.GLAcount,tblSalesInvoices.UOM,tblSalesInvoices.CostPrrice FROM tblSalesInvoices INNER JOIN tblItemWhse ON tblItemWhse.ItemID=tblSalesInvoices.ItemID AND tblItemWhse.WhseId=tblSalesInvoices.Location  WHERE tblSalesInvoices.InvoiceNo='" + strNo + "' ORDER BY tblSalesInvoices.DistributionNo";

                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        GetCustomer();
                        GetWareHouseDataSet();

                        ClearHeader();
                        DeleteRows();
                        EnableHeader(true);
                        EnableFoter(true);
                        ViewHeader(strNo);
                        ViewDetails(strNo);
                        ViewHeader(strNo);
                        viewCardHistory(strNo);
                        viewinvpayhistory(strNo);
                        EnableHeader(false);
                        EnableFoter(false);
                        ValidateInvoiceSetting();
                        //EditValidtion();
                        btnSave.Enabled = false;
                        SetReadOnly(true);

                        txtInvoiceNo.ReadOnly = true;
                        dtpDate.Enabled = false;
                        cmbSalesRep.Enabled = false;
                        cmbInvoiceType.Enabled = false;
                        combMode.Enabled = false;
                        ug.Enabled = false;


                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EditValidtion()
        {
            throw new NotImplementedException();
        }

        public void GetCustomer()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2,Pricing_Level,Custom2 as VATNo FROM tblCustomerMaster   order by CutomerID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbCustomer.DataSource = dsCustomer.Tables["DtClient"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CutomerID";

                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address1"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address2"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CutomerID"].Width = 100;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CustomerName"].Width = 150;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Pricing_Level"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["VATNo"].Hidden = true;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void setSMsString()
        {
            try
            {
                TextReader tr = new StreamReader("SmsCustom.txt");
                SmsMassage = tr.ReadLine();
                tr.Close();
            }
            catch { }
        }

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

        private void ValidateInvoiceSetting()
        {

            String S1 = "Select Locked from tblTax_Default where flg ='PRL' and UserName='" + user.userName.ToString().Trim() + "'";
            SqlCommand cmd = new SqlCommand(S1);
            SqlDataAdapter da = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Boolean Isok = false;
            if (dt.Rows.Count > 0)
            {
                Isok = bool.Parse(dt.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].CellActivation = Activation.NoEdit;
                }
                else
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].CellActivation = Activation.AllowEdit;
                }
            }
            String S2 = "Select Locked from tblTax_Default where flg ='QTY' and UserName='" + user.userName.ToString().Trim() + "'";
            SqlCommand cmd1 = new SqlCommand(S2);
            SqlDataAdapter da1 = new SqlDataAdapter(S2, ConnectionString);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                Isok = bool.Parse(dt1.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    ug.DisplayLayout.Bands[0].Columns["Quantity"].CellActivation = Activation.NoEdit;
                }
                else
                {
                    ug.DisplayLayout.Bands[0].Columns["Quantity"].CellActivation = Activation.AllowEdit;
                }
            }
            String S3 = "Select Locked from tblTax_Default where flg ='DST' and UserName='" + user.userName.ToString().Trim() + "'";
            SqlCommand cmd2 = new SqlCommand(S3);
            SqlDataAdapter da2 = new SqlDataAdapter(S3, ConnectionString);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            if (dt2.Rows.Count > 0)
            {
                Isok = bool.Parse(dt2.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    ug.DisplayLayout.Bands[0].Columns["Discount"].CellActivation = Activation.NoEdit;
                }
                else
                {
                    ug.DisplayLayout.Bands[0].Columns["Discount"].CellActivation = Activation.AllowEdit;
                }
            }
        }

        public void UpdateInvoiceNo(Int32 intInvoiceNo, SqlConnection con, SqlTransaction Trans)
        {
            SqlCommand command;
            Int32 intX;
            Int32 intZ;
            string StrInvNo;
            Int32 intP;
            Int32 intI;
            String StrInV;
            string StrUpdateInvNo;

            try
            {
                StrSql = "SELECT JobActPref, JobActPad, JobActNum FROM tblDefualtSetting";
                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = (int.Parse(dt.Rows[0].ItemArray[1].ToString().Trim()));
                    intZ = (int.Parse(dt.Rows[0].ItemArray[2].ToString().Trim()));

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();
                    StrUpdateInvNo = StrInvNo + StrInV.Substring(1, intX);

                    StrSql = "UPDATE EST_HEADER SET  EstimateNo='" + StrUpdateInvNo + "' WHERE AutoIndex=" + intInvoiceNo + "";
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void GetQuantityMatrix()
        {
            try
            {
                StrSql = "SELECT  QTY1,QTY2,QTY3 FROM tblQuantityMatrix";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();

                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dblQtyLevel1 = double.Parse(dt.Rows[0].ItemArray[0].ToString().Trim());
                    dblQtyLevel2 = double.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    dblQtyLevel3 = double.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetRetailCustomer()
        {
            try
            {
                StrSql = "SELECT  CutomerID FROM tblDefualtSetting";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();

                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrRetailCustomer = dt.Rows[0].ItemArray[0].ToString().Trim();
                }
                else
                {
                    StrRetailCustomer = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;
                StrSql = "SELECT  TOP 1(InvNum) FROM tblDefualtSetting ORDER BY InvNum DESC";//TAX            
                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intInvNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                }
                else
                {
                    intInvNo = 1;
                }
                StrSql = "UPDATE tblDefualtSetting SET InvNum='" + intInvNo + "'";//tax    
                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Boolean CheckInvoiceNo(string INVNO)
        {
            try
            {
                StrSql = "SELECT InvoiceNo FROM tblSalesInvoices where InvoiceNo='" + INVNO + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        public void UpdatePrefixNonew(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;

                if (optSerialOne.Checked == true)
                {
                    StrSql = "SELECT  TOP 1(InvNum) FROM tblDefualtSetting ORDER BY InvNum DESC";
                }
                else
                {
                    StrSql = "SELECT  TOP 1(InvNum1) FROM tblDefualtSetting ORDER BY InvNum1 DESC";
                }

                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intInvNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                }
                else
                {
                    intInvNo = 1;
                }

                if (optSerialOne.Checked == true)
                {
                    StrSql = "UPDATE tblDefualtSetting SET InvNum='" + intInvNo + "'";
                }
                else
                {
                    StrSql = "UPDATE tblDefualtSetting SET InvNum1='" + intInvNo + "'";
                }

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                con.Close();
            }

        }

        public string GetInvNoFieldNew(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                if (optSerialOne.Checked == true)
                {
                    StrSql = "SELECT InvPref, InvPad, InvNum FROM tblDefualtSetting";
                }
                else
                {
                    StrSql = "SELECT InvPref1, InvPad1, InvNum1 FROM tblDefualtSetting";
                }

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    return StrInvNo + StrInV.Substring(1, intX);

                }
                else
                {
                    return null;
                }
            }

            catch (Exception)
            {
                return null;
                throw;
            }
            finally
            {
                con.Close();
            }


        }
        public string GetInvNoField_CashOR_Credit(SqlConnection con, SqlTransaction Trans, string WH)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                //if (user.InvPrefexDir == true)
                //{
                //    StrSql = "SELECT InvPref, InvPad, InvNum FROM tblDefualtSetting";
                //}
                //else if (user.InvPrefexDir == false)
                //{
                //    if (combMode.Value.ToString() == "1")
                //    {
                //        StrSql = "SELECT InvPref, InvPad, InvNum FROM tblDefualtSetting";
                //    }
                //    else if (combMode.Value.ToString() == "2")
                //    {
                //        StrSql = "SELECT InvDRCreditPref, InvDRCreditPad, InvDRCreditNum FROM tblDefualtSetting";
                //    }
                //}

                //----------------------
                //if (user.InvPrefexDir == true)
                //{
                //    StrSql = "SELECT InvPref, InvPad, InvNum FROM tblDefualtSetting";
                //}
                //else if (user.InvPrefexDir == false)
                //{
                if (WH == "NEW Lasantha TYRE TRADERS")
                {
                    StrSql = "SELECT InvPref, InvPad, InvNum FROM tblDefualtSetting";
                }
                else
                {
                    StrSql = "SELECT InvDRCreditPref, InvDRCreditPad, InvDRCreditNum FROM tblDefualtSetting";
                }
                //}




                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    return StrInvNo + StrInV.Substring(1, intX);

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public void UpdatePrefixCashCreditNonew(SqlConnection con, SqlTransaction Trans, string WH)
        {
            try
            {
                int intInvNo;
                SqlCommand command;

                //if (user.InvPrefexDir == true)
                //{
                //    StrSql = "SELECT  TOP 1(InvNum) FROM tblDefualtSetting ORDER BY InvNum DESC";
                //}
                //else
                //{
                if (WH == "NEW Lasantha TYRE TRADERS")
                {
                    StrSql = "SELECT  TOP 1(InvNum) FROM tblDefualtSetting ORDER BY InvNum DESC";
                }
                else
                {
                    StrSql = "SELECT  TOP 1(InvDRCreditNum) FROM tblDefualtSetting ORDER BY InvDRCreditNum DESC";
                }
                //}
                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intInvNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                }
                else
                {
                    intInvNo = 1;
                }

                //if (user.InvPrefexDir == true)
                //{
                //    StrSql = "UPDATE tblDefualtSetting SET InvNum='" + intInvNo + "'";

                //}
                //else
                //{
                //    if (combMode.Value.ToString() == "1")
                //    {
                //        StrSql = "UPDATE tblDefualtSetting SET InvNum='" + intInvNo + "'";
                //    }
                //    else if (combMode.Value.ToString() == "2")
                //    {
                //        StrSql = "UPDATE tblDefualtSetting SET InvDRCreditNum='" + intInvNo + "'";
                //    }
                //}

                if (WH == "NEW Lasantha TYRE TRADERS")
                {
                    StrSql = "UPDATE tblDefualtSetting SET InvNum='" + intInvNo + "'";
                }
                else
                {
                    StrSql = "UPDATE tblDefualtSetting SET InvDRCreditNum='" + intInvNo + "'";
                }

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }

        }
        public string GetInvNoField(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;
                StrSql = "SELECT InvPref, InvPad, InvNum FROM tblDefualtSetting";//HO0001              

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    return StrInvNo + StrInV.Substring(1, intX);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        public void GetAR()
        {



        }

        public void GetSalesRep()
        {
            dsSalesRep = new DataSet();
            try
            {
                dsSalesRep.Clear();
                StrSql = " SELECT RepCode, RepName FROM tblSalesRep order by RepCode";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsSalesRep, "DtSalesRep");

                cmbSalesRep.DataSource = dsSalesRep.Tables["DtSalesRep"];
                cmbSalesRep.DisplayMember = "RepName";
                cmbSalesRep.ValueMember = "RepCode";
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepCode"].Width = 75;
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepName"].Width = 125;
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
                StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster";

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

        //===================================

        //=======================================

        public void GetItemDataSet()
        {
            try
            {
                if (cmbWarehouse.Text == "")
                {
                    return;
                }
                //sanjeewa

                //if (CheckBox == "" || CheckBox == null)
                //{
                StrSql = "SELECT  tblItemWhse.ItemId, tblItemWhse.ItemDis as ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1 as ListPrice,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost,tblItemMaster.TaxType FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID where tblItemWhse.QTY>0";
                // StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1 as ListPrice,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost,tblItemMaster.TaxType  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)  and tblItemWhse.QTY>0 order by tblItemWhse.ItemDis";
                StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1 as ListPrice,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost,tblItemMaster.TaxType  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)   order by tblItemWhse.ItemDis";
                //}
                //else
                //{
                //    StrSql = "SELECT  tblItemWhse.ItemId, tblItemWhse.ItemDis as ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1 as ListPrice,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost,tblItemMaster.TaxType FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID where tblItemWhse.QTY>0";
                //    // StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1 as ListPrice,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost,tblItemMaster.TaxType  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)  and tblItemWhse.QTY>0 order by tblItemWhse.ItemDis";
                //    StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1 as ListPrice,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost,tblItemMaster.TaxType  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)   order by tblItemWhse.ItemDis";

                //}

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";


                    ultraCombo1.DisplayLayout.Bands[0].Columns[0].Width = 120;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[1].Width = 200;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Width = 60;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[3].Width = 100;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[4].Width = 70;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[5].Width = 70;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[6].Width = 70;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[7].Width = 70;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[8].Width = 70;


                    // ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[14].Hidden = true;


                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    //  ultraCombo1.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[7].Hidden = true;


                    ultraCombo2.DataSource = dt;
                    ultraCombo2.ValueMember = "ItemDescription";
                    ultraCombo2.DisplayMember = "ItemDescription";

                    ultraCombo2.DisplayLayout.Bands[0].Columns[0].Width = 120;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[1].Width = 320;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[2].Width = 60;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[3].Width = 100;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[4].Width = 70;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[5].Width = 70;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[6].Width = 70;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[7].Width = 70;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[8].Width = 70;


                    // ultraCombo2.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[14].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[14].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                    //  ultraCombo2.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[7].Hidden = true;

                    // ultraCombo1.DisplayLayout.Bands[0].Columns[2].dis
                }
                else
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";

                    // ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[14].Hidden = true;


                    // ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[7].Hidden = true;

                    ultraCombo2.DataSource = dt;
                    ultraCombo2.ValueMember = "ItemDescription";
                    ultraCombo2.DisplayMember = "ItemDescription";

                    // ultraCombo2.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[14].Hidden = true;


                    // ultraCombo2.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteRows()
        {
            try
            {
                if (ug.Rows == null) return;
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    ugR.Delete(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteEmpGrid()
        {
            try
            {
                ug.PerformAction(UltraGridAction.CommitRow);
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    if (ugR.Cells[1].Value.ToString().Trim().Length == 0)
                    {
                        ugR.Delete(false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.ShiftKey)
            {
                txtpaidAmount.Focus();
            }

            switch (e.KeyValue)
            {

                case 37:
                    {
                        ug.PerformAction(UltraGridAction.PrevCell);
                        break;
                    }
                case 38:
                    {
                        if (ug.ActiveCell.Column.Key != "ItemCode" && ug.ActiveCell.Column.Key != "Description")
                            ug.PerformAction(UltraGridAction.AboveCell);
                        break;
                    }
                case 39:
                    {

                        ug.PerformAction(UltraGridAction.NextCell);
                        break;
                    }
                case 40:
                    {
                        if (ug.ActiveCell.Column.Key == "ItemCode")
                        {
                            ug.PerformAction(UltraGridAction.EnterEditModeAndDropdown, false, true);
                        }
                        break;
                    }

                case 9:
                    {
                        if (ug.ActiveCell.Column.Key == "Quantity")
                        {

                            ug.PerformAction(UltraGridAction.NextCell);

                            if (ug.ActiveRow.Cells["Quantity"].Value == null || ug.ActiveRow.Cells["Quantity"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["Quantity"].Value = "0";
                            if (ug.ActiveRow.Cells["UnitPrice"].Value == null || ug.ActiveRow.Cells["UnitPrice"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["UnitPrice"].Value = "0";
                            if (ug.ActiveRow.Cells["UnitPrice(Incl)"].Value == null || ug.ActiveRow.Cells["UnitPrice(Incl)"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["UnitPrice(Incl)"].Value = "0";
                            if (ug.ActiveRow.Cells["Discount"].Value == null || ug.ActiveRow.Cells["Discount"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["Discount"].Value = "0";
                            if (ug.ActiveRow.Cells["TotalPrice"].Value == null || ug.ActiveRow.Cells["TotalPrice"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["TotalPrice"].Value = "0";
                            if (ug.ActiveRow.Cells["LineDisc"].Value == null || ug.ActiveRow.Cells["LineDisc"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["LineDisc"].Value = "0";
                            if (ug.ActiveRow.Cells["TotalPrice(Incl)"].Value == null || ug.ActiveRow.Cells["TotalPrice(Incl)"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["TotalPrice(Incl)"].Value = "0";
                            if (ug.ActiveRow.Cells["LineTax"].Value == null || ug.ActiveRow.Cells["LineTax"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["LineTax"].Value = "0";
                            ug.PerformAction(UltraGridAction.PrevCell);

                        }


                        else if (ug.ActiveCell.Column.Key == "TotalPrice(Incl)")
                        {

                            ug.PerformAction(UltraGridAction.NextCell);
                        }

                        else if (ug.ActiveCell.Column.Key == "Discount")
                        {

                        }

                        else if (ug.ActiveCell.Column.Key == "TotalPrice")
                        {
                            if (ug.ActiveRow.Cells["Description"].Value != null && ug.ActiveRow.Cells["Description"].Value.ToString() != string.Empty)
                            {
                                if (ug.ActiveRow.HasNextSibling() == false)
                                {
                                    ug.PerformAction(UltraGridAction.BelowRow);
                                    UltraGridRow ugR;
                                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                                    ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                    ugR.Cells["LineNo"].Selected = true;
                                    ugR.Cells["LineNo"].Activated = true;
                                }
                            }
                        }
                        else if (ug.ActiveCell.Column.Key == "LineNo")
                        {
                            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                            {
                                ug.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, true);
                            }


                        }
                        else if (ug.ActiveCell.Column.Key == "ItemCode")
                        {

                        }

                        break;
                    }

                case 13:
                    {
                        if (ug.ActiveCell.Column.Key == "Quantity")
                        {

                            ug.PerformAction(UltraGridAction.NextCell);

                            if (ug.ActiveRow.Cells["Quantity"].Value == null || ug.ActiveRow.Cells["Quantity"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["Quantity"].Value = "0";
                            if (ug.ActiveRow.Cells["UnitPrice"].Value == null || ug.ActiveRow.Cells["UnitPrice"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["UnitPrice"].Value = "0";
                            if (ug.ActiveRow.Cells["UnitPrice(Incl)"].Value == null || ug.ActiveRow.Cells["UnitPrice(Incl)"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["UnitPrice(Incl)"].Value = "0";
                            if (ug.ActiveRow.Cells["Discount"].Value == null || ug.ActiveRow.Cells["Discount"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["Discount"].Value = "0";
                            if (ug.ActiveRow.Cells["TotalPrice"].Value == null || ug.ActiveRow.Cells["TotalPrice"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["TotalPrice"].Value = "0";
                            if (ug.ActiveRow.Cells["LineDisc"].Value == null || ug.ActiveRow.Cells["LineDisc"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["LineDisc"].Value = "0";
                            if (ug.ActiveRow.Cells["TotalPrice(Incl)"].Value == null || ug.ActiveRow.Cells["TotalPrice(Incl)"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["TotalPrice(Incl)"].Value = "0";
                            if (ug.ActiveRow.Cells["LineTax"].Value == null || ug.ActiveRow.Cells["LineTax"].Value.ToString() == string.Empty) ug.ActiveRow.Cells["LineTax"].Value = "0";
                            ug.PerformAction(UltraGridAction.PrevCell);

                        }


                        else if (ug.ActiveCell.Column.Key == "TotalPrice(Incl)")
                        {

                            ug.PerformAction(UltraGridAction.NextCell);
                        }

                        else if (ug.ActiveCell.Column.Key == "Discount")
                        {

                        }

                        else if (ug.ActiveCell.Column.Key == "TotalPrice")
                        {
                            if (ug.ActiveRow.Cells["Description"].Value != null && ug.ActiveRow.Cells["Description"].Value.ToString() != string.Empty)
                            {
                                if (ug.ActiveRow.HasNextSibling() == false)
                                {
                                    ug.PerformAction(UltraGridAction.BelowRow);
                                    UltraGridRow ugR;
                                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                                    ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                    ugR.Cells["LineNo"].Selected = true;
                                    ugR.Cells["LineNo"].Activated = true;
                                }
                            }
                        }
                        else if (ug.ActiveCell.Column.Key == "LineNo")
                        {
                            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                            {
                                ug.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, true);
                            }


                        }
                        else if (ug.ActiveCell.Column.Key == "ItemCode")
                        {

                        }

                        break;
                    }


                case 10:
                    {

                        break;
                    }

            }

        }

        private double LineCalculation(double UnitPrice, double Quantity)
        {
            try
            {
                dblLineTot = 0;
                double lineTotal = 0;
                dblLineTot = UnitPrice * Quantity;
                lineTotal = dblLineTot;
                return lineTotal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_Click(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow ugR;

                if (HeaderValidation() == false)
                {
                    return;
                }

                if (ug.Rows.Count == 0)
                {
                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["LineNo"].Value = ugR.Index + 1;
                    ugR.Cells["LineNo"].Selected = true;
                    ugR.Cells["LineNo"].Activated = true;
                }
                if (ug.ActiveCell != null)
                {

                    if (ug.ActiveCell.Column.Key == "ItemCode")
                    {
                        {


                            StrSql = " SELECT   tblItemWhse.ItemID,  tblItemWhse.ItemDis, tblItemWhse.QTY, tblItemMaster.ItemClass, tblItemMaster.SalesGLAccount, tblItemWhse.UOM, tblItemMaster.Categoty, " +
                                " tblItemMaster.PriceLevel1 as ListPrice, tblItemMaster.PriceLevel2, tblItemMaster.PriceLevel3, tblItemMaster.PriceLevel4, tblItemMaster.PriceLevel6, tblItemMaster.PriceLevel5, " +
                                " tblItemMaster.PriceLevel7, tblItemMaster.PriceLevel8, tblItemMaster.PriceLevel9, tblItemMaster.PriceLevel10, isnull( tblItemWhse.UnitCost,0) as UnitCost " +
                                " FROM         tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID where  tblItemWhse.ItemID='" + ug.ActiveCell.Text + "'";

                            SqlCommand cmd = new SqlCommand(StrSql);
                            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            foreach (DataRow dr in dt.Rows)
                            {
                                if (ug.ActiveCell.Value.ToString() == dr["ItemId"].ToString())
                                {

                                    ug.ActiveCell.Row.Cells["Description"].Value = dr["ItemDis"].ToString();
                                    ug.ActiveCell.Row.Cells["OnHand"].Value = dr["QTY"].ToString();
                                    ug.ActiveCell.Row.Cells["ItemClass"].Value = dr["ItemClass"].ToString();
                                    ug.ActiveCell.Row.Cells["GL"].Value = dr["SalesGLAccount"].ToString();
                                    ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
                                    ug.ActiveCell.Row.Cells["UOM"].Value = dr["UOM"].ToString();
                                    ug.ActiveCell.Row.Cells["Categoty"].Value = dr["Categoty"].ToString();
                                    ug.ActiveCell.Row.Cells["CostPrice"].Value = dr["UnitCost"].ToString();
                                    ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr["ListPrice"].ToString();
                                    ug.ActiveCell.Row.Cells["TotalPrice(Incl)"].Value = dr["ListPrice"].ToString();
                                    ug.ActiveCell.Row.Cells["PriceLevel1"].Value = dr["ListPrice"].ToString();
                                    ug.ActiveCell.Row.Cells["PriceLevel2"].Value = dr["PriceLevel2"].ToString();
                                    ug.ActiveCell.Row.Cells["PriceLevel3"].Value = dr["PriceLevel3"].ToString();
                                    ug.ActiveCell.Row.Cells["PriceLevel4"].Value = dr["PriceLevel4"].ToString();
                                    ug.ActiveCell.Row.Cells["PriceLevel5"].Value = dr["PriceLevel5"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsWarehouseHaveStock()
        {
            try
            {
                double _RtnStock = 0;
                double _IssQty = 0;
                int intItemClass;
                foreach (UltraGridRow dr in ug.Rows)
                {
                    SqlConnection con = new SqlConnection(ConnectionString);
                    string s = "select QTY from tblItemWhse where ItemID = '" + dr.Cells[1].Value.ToString().Trim() + "' and WhseId='" + dr.Cells["WH"].Value.ToString().Trim() + "'";
                    SqlDataAdapter da = new SqlDataAdapter(s, con);
                    DataTable _dtbl = new DataTable();
                    da.Fill(_dtbl);

                    _RtnStock = double.Parse(_dtbl.Rows[0][0].ToString());

                    _IssQty = 0;
                    foreach (UltraGridRow udr in ug.Rows)
                    {
                        if (dr.Cells[1].ToString() == udr.Cells[1].Value.ToString())
                        {
                            _IssQty = _IssQty + double.Parse(udr.Cells["Quantity"].Value.ToString().Trim()); //
                        }
                        intItemClass = int.Parse(udr.Cells["ItemClass"].Value.ToString().Trim());
                        if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                        {
                            if (_RtnStock < _IssQty)
                            {
                                MessageBox.Show("Not Enough Qty for " + dr.Cells[1].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        public Boolean IsGridValidation()
        {
            try
            {
                if (ug.Rows.Count == 0)
                {
                    MessageBox.Show("Please Select an item", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return false;
                }

                foreach (UltraGridRow ugR in ug.Rows)
                {
                    double _qty = double.Parse(ugR.Cells["Quantity"].Value.ToString()) + double.Parse(ugR.Cells["FOCQty"].Value.ToString());
                    if (_qty <= 0)
                    {
                        MessageBox.Show("Quantity Should be Greater than zero.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                if (!IsWarehouseHaveStock())
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Boolean HeaderValidation()
        {
            try
            {
                if (cmbWarehouse.Text.Trim() == "")
                {
                    return false;
                }

                if (cmbSalesRep.Text.Trim() == "")
                {
                    return false;
                }
                if (cmbCustomer.Text.Trim() == "")
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ViewDetails(string StrInvoiceNo)
        {
            try
            {
                StrSql = "SELECT LineTax,LineDiscountAmount,LineDiscountPercentage,tblSalesInvoices.DistributionNo,tblSalesInvoices.ItemID,tblSalesInvoices.Description,tblSalesInvoices.Qty,tblSalesInvoices.UnitPrice,tblSalesInvoices.Amount,tblSalesInvoices.InclusivePrice,tblItemWhse.QTY as WHQTY,tblSalesInvoices.ItemClass,tblSalesInvoices.ItemType,tblSalesInvoices.GLAcount,tblSalesInvoices.UOM,tblSalesInvoices.CostPrrice,tblSalesInvoices.FOCQty,tblSalesInvoices.Location FROM tblSalesInvoices INNER JOIN tblItemWhse ON tblItemWhse.ItemID=tblSalesInvoices.ItemID   WHERE tblSalesInvoices.InvoiceNo='" + StrInvoiceNo + "' ORDER BY tblSalesInvoices.DistributionNo";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = Dr["DistributionNo"];
                        ugR.Cells["ItemCode"].Value = Dr["ItemID"];
                        ugR.Cells["Description"].Value = Dr["Description"];
                        ugR.Cells["UnitPrice"].Value = Dr["UnitPrice"];
                        ugR.Cells["UnitPrice(Incl)"].Value = Dr["InclusivePrice"];
                        ugR.Cells["Quantity"].Value = Dr["Qty"];
                        ugR.Cells["TotalPrice"].Value = Dr["Amount"];
                        ugR.Cells["TotalPrice(Incl)"].Value = Convert.ToDouble(Dr["InclusivePrice"]) * Convert.ToDouble(Dr["Qty"]);
                        ugR.Cells["OnHand"].Value = Dr["WHQTY"];
                        ugR.Cells["ItemClass"].Value = Dr["ItemClass"];
                        ugR.Cells["GL"].Value = Dr["GLAcount"];
                        ugR.Cells["WH"].Value = Dr["Location"];
                        ugR.Cells["UOM"].Value = Dr["UOM"];
                        ugR.Cells["Categoty"].Value = Dr["ItemType"];
                        ugR.Cells["CostPrice"].Value = Dr["CostPrrice"];
                        ugR.Cells["Discount"].Value = Dr["LineDiscountPercentage"];
                        ugR.Cells["LineDisc"].Value = Dr["LineDiscountAmount"];
                        ugR.Cells["LineTax"].Value = Dr["LineTax"];
                        ugR.Cells["FOCQty"].Value = Dr["FOCQty"];
                        ugR.Cells["TotalQty"].Value = Convert.ToDouble(Dr["FOCQty"]) + Convert.ToDouble(Dr["Qty"]);
                        ugR.Cells["PriceLevel1"].Value = 0.00;
                        ugR.Cells["PriceLevel2"].Value = 0.00;
                        ugR.Cells["PriceLevel3"].Value = 0.00;
                        ugR.Cells["PriceLevel4"].Value = 0.00;
                        ugR.Cells["PriceLevel5"].Value = 0.00;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void ViewHeader(string StrInvoiceNo)
        {
            try
            {
                StrSql = "SELECT     InvoiceNo, InvoiceDate, CustomerID, Location, Comments, ARAccount, " +
                    " SalesRep, PaymentM, " +
                    " Tax1Rate, Tax2Rate, Tax1Amount, Tax2Amount, " +
                    " ISNULL(SubValue, 0) AS Expr2, GrossTotal, NetTotal, " +
                    " DeliveryNoteNos, NoofDistributions, DistributionNo, ItemID, Qty,  " +
                    " Description, GLAcount, UnitPrice, TotalDiscountPercen, Amount, TotalDiscountAmount,  " +
                    " CurrentDate, Time, Currentuser, IsExport, CustomerPO, UOM, JobID, SONO, " +
                    " TTType1, TTType2, IsReturn, TTType3, Tax3Amount, RemainQty, CostPrrice, ItemClass,  " +
                    " ItemType, IsVoid, VoidReson, VoidUser, SubValue, Tax3Rate, InvType,  " +
                    " WHID, ServiceCharge, LineDiscountAmount, LineDiscountPercentage, IsInclusive,PaymentM,CustomerName,ContactNo,PaidAmount,CusPoNum,Mileage,JobDoneBy,IsConfirm,VehicleNo,Comments,WarrentyKm,WarrentyYear,Remarks" +
                    " FROM tblSalesInvoices where InvoiceNo='" + StrInvoiceNo + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    lblCompany.Text = dt.Rows[0]["WHID"].ToString();

                    if (dt.Rows[0]["IsConfirm"].ToString() == "True")
                    {
                        btnEditer.Enabled = false;
                        btnConfirm.Enabled = false;
                        btnVoid.Enabled = false;
                    }
                    else
                    {
                        btnEditer.Enabled = true;
                        btnConfirm.Enabled = true;
                    }

                    if (dt.Rows[0]["IsVoid"].ToString() == "True")
                    {

                        btnVoid.Enabled = false;
                        btnEditer.Enabled = false;
                        btnConfirm.Enabled = false;
                        lblVoid.Visible = true;
                    }
                    else
                    {
                        btnVoid.Enabled = true;
                        lblVoid.Visible = false;
                    }

                    txtTyreSize.Text = dt.Rows[0]["JobID"].ToString();
                    txtVehicleNo.Text = dt.Rows[0]["VehicleNo"].ToString();
                    txtInvoiceNo.Text = dt.Rows[0]["InvoiceNo"].ToString();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0]["InvoiceDate"].ToString().Trim());
                    cmbCustomer.Value = dt.Rows[0]["CustomerID"].ToString();
                    cmbWarehouse.Text = dt.Rows[0]["Location"].ToString().Trim();
                    txtDescription.Text = dt.Rows[0]["Comments"].ToString().Trim();
                    //cmbAR.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                    cmbSalesRep.Value = dt.Rows[0]["SalesRep"].ToString().Trim();
                    TxtPatientName.Text = dt.Rows[0]["CustomerName"].ToString().Trim();
                    txtPatientContactNo.Text = dt.Rows[0]["ContactNo"].ToString().Trim();
                    txtCusPONum.Text = dt.Rows[0]["CusPoNum"].ToString().Trim();
                    txtMileage.Text = dt.Rows[0]["Mileage"].ToString().Trim();
                    combMode.Value = dt.Rows[0]["PaymentM"].ToString().Trim();
                    cmbJobDone.Text = dt.Rows[0]["JobDoneBy"].ToString().Trim();
                    txtWarrentyKm.Text = dt.Rows[0]["WarrentyKm"].ToString().Trim();
                    txtWarrentyYear.Text = dt.Rows[0]["WarrentyYear"].ToString().Trim();
                    txtRemarks.Text = dt.Rows[0]["Remarks"].ToString().Trim();
                    if (Convert.ToInt32(dt.Rows[0]["InvType"]) == 1)
                    {
                        rbtNoVat.Checked = true;
                    }
                    if (Convert.ToInt32(dt.Rows[0]["InvType"]) == 2)
                    {
                        rbtVAT.Checked = true;
                    }
                    if (Convert.ToInt32(dt.Rows[0]["InvType"]) == 3)
                    {
                        rbtSVAT.Checked = true;
                    }

                    if (dt.Rows[0]["PaymentM"].ToString().Trim() == "Cash")
                    {
                        optCash.Checked = true;
                    }
                    if (dt.Rows[0]["PaymentM"].ToString().Trim() == "Credit")
                    {
                        optCredit.Checked = true;
                    }
                    if (dt.Rows[0]["PaymentM"].ToString().Trim() == "CreditCard")
                    {
                        rdobtnCreditCard.Checked = true;
                    }


                    txtNBTPer.Value = double.Parse(dt.Rows[0]["Tax1Rate"].ToString().Trim());
                    txtVatPer.Value = double.Parse(dt.Rows[0]["Tax2Rate"].ToString().Trim());
                    txtNBT.Value = double.Parse(dt.Rows[0]["Tax1Amount"].ToString().Trim());
                    txtVat.Value = double.Parse(dt.Rows[0]["Tax2Amount"].ToString().Trim());
                    txtDiscPer.Value = double.Parse(dt.Rows[0]["TotalDiscountPercen"].ToString().Trim());
                    txtDiscAmount.Value = double.Parse(dt.Rows[0]["TotalDiscountAmount"].ToString().Trim());

                    txtServicecharge.Value = Convert.ToDouble(dt.Rows[0]["ServiceCharge"].ToString());
                    txtSubValue.Value = double.Parse(dt.Rows[0]["SubValue"].ToString().Trim());
                    txtGrossValue.Value = double.Parse(dt.Rows[0]["GrossTotal"].ToString().Trim());
                    txtNetValue.Value = double.Parse(dt.Rows[0]["NetTotal"].ToString().Trim());

                    txtpaidAmount.Text = dt.Rows[0]["PaidAmount"].ToString();
                    txtpaid.Text = dt.Rows[0]["PaidAmount"].ToString();

                    txtDescription.Text = dt.Rows[0]["Comments"].ToString();

                    if (Convert.ToInt32(dt.Rows[0]["IsInclusive"]) == 1)
                    {
                        cmbInvoiceType.Value = 1; //1-Inclusive,2-Exclusive,3-non VAT 
                    }
                    if (Convert.ToInt32(dt.Rows[0]["IsInclusive"]) == 2)
                    {
                        cmbInvoiceType.Value = 2; //1-Inclusive,2-Exclusive,3-non VAT 
                    }
                    if (Convert.ToInt32(dt.Rows[0]["IsInclusive"]) == 3)
                    {
                        cmbInvoiceType.Value = 3; //1-Inclusive,2-Exclusive,3-non VAT 
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void LoadCreditcardData()
        {
            try
            {
                cblcname.Items.Clear();
                String S = "Select CardType from tblCreditData order by CardType";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                dt.Clear();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cblcname.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch { }

        }

        private void CheckGL()
        {
            string StrSql1 = "SELECT GL_True from  tblDefualtSetting";
            SqlCommand cmd1 = new SqlCommand(StrSql1);
            SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                IsGLok = Convert.ToBoolean(dt1.Rows[0].ItemArray[0].ToString().Trim());
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    return base.ProcessDialogKey(Keys.Tab);
            }
            return base.ProcessDialogKey(keyData);
        }

        public string CheckBox;
        private void frmInvoices_Load(object sender, EventArgs e)
        {
            //Connector c = new Connector();
            // c.Export_Receipt_Journal();

            GridKeyActionMapping m = new GridKeyActionMapping(Keys.Enter, UltraGridAction.NextCellByTab, (UltraGridState)0, UltraGridState.Cell, SpecialKeys.All, (SpecialKeys)0);
            this.ug.KeyActionMappings.Add(m);

            this.ActiveControl = cmbCustomer;
            try
            {
                if (!IsFind)
                {
                    //----------------user----------

                    intEstomateProcode = 0;

                    if (intEstomateProcode == 0)
                    {
                        run = false;
                        add = false;
                        edit = false;
                        delete = false;

                        dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmInvoices");
                        if (dtUser.Rows.Count > 0)
                        {
                            run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                            add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                            edit = Convert.ToBoolean(dtUser.Rows[0].ItemArray[4].ToString());
                            delete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());
                        }

                        run = true;
                        add = true;
                        edit = true;
                        delete = true;
                        //---------------------------------
                        IsEdit = false;
                        GetCurrentUserDate();

                        btnSave.Enabled = false;
                        btnPrint.Enabled = false;
                        btnSearch.Enabled = true;
                        btnReset.Enabled = true;
                        btnNew.Enabled = true;
                        btnEditer.Enabled = false;
                        dtpDate.Enabled = false;

                        CheckGL();

                        GetWareHouseDataSet();
                        GetCustomer();
                        GetJobDoneby();
                        GetSalesRep();
                        GetAR();
                        LoadCreditcardData();
                        ClearHeader();
                        DeleteRows();
                        GetChargeItems();
                        EnableHeader(false);
                        EnableFoter(false);
                        //  SetDefaultWarehouse();

                        loadDefaltOption();

                        GetItemDataSet();
                        ValidateInvoiceSetting();
                        if (user.IsSINVNoAutoGen) txtInvoiceNo.ReadOnly = true;
                        else txtInvoiceNo.ReadOnly = false;

                        LoadINVDefault();
                        btnNew_Click(sender, e);

                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void LoadINVDefault()
        {
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT * FROM InvoiceDefault";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    PDFViewer = Convert.ToBoolean(dt.Rows[0].ItemArray[0].ToString());
                    // ToolStripViewer = Convert.ToBoolean(dt.Rows[0].ItemArray[1].ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetChargeItems()
        {
            try
            {

                StrSql = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='1'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    LineDisitemid = dt.Rows[0].ItemArray[0].ToString();
                    LineDisitemdescription = dt.Rows[0].ItemArray[1].ToString();
                    LineDisGLAccount = dt.Rows[0].ItemArray[2].ToString();
                }

                string StrSql2 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='3'";

                SqlCommand cmd2 = new SqlCommand(StrSql2);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql2, ConnectionString);
                DataTable dt2 = new DataTable();
                dt2.Clear();
                da2.Fill(dt2);
                {
                    Cashitemid = dt2.Rows[0].ItemArray[0].ToString();
                    cashitemdis = dt2.Rows[0].ItemArray[1].ToString();
                    cashGL = dt2.Rows[0].ItemArray[2].ToString();
                }
                string StrSql3 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='4'";

                SqlCommand cmd3 = new SqlCommand(StrSql3);
                SqlDataAdapter da3 = new SqlDataAdapter(StrSql3, ConnectionString);
                DataTable dt3 = new DataTable();
                dt3.Clear();
                da3.Fill(dt3);
                {
                    NBitemid = dt3.Rows[0].ItemArray[0].ToString();
                    NBTitemDis = dt3.Rows[0].ItemArray[1].ToString();
                    NBTitemGL = dt3.Rows[0].ItemArray[2].ToString();
                }

                string StrSql4 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='5'";

                SqlCommand cmd4 = new SqlCommand(StrSql4);
                SqlDataAdapter da4 = new SqlDataAdapter(StrSql4, ConnectionString);
                DataTable dt4 = new DataTable();
                dt4.Clear();
                da4.Fill(dt4);
                {
                    VATitemid = dt4.Rows[0].ItemArray[0].ToString();
                    VATitemDis = dt4.Rows[0].ItemArray[1].ToString();
                    VATGL = dt4.Rows[0].ItemArray[2].ToString();
                }


                string StrSql5 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='6'";

                SqlCommand cmd5 = new SqlCommand(StrSql5);
                SqlDataAdapter da5 = new SqlDataAdapter(StrSql5, ConnectionString);
                DataTable dt5 = new DataTable();
                dt5.Clear();
                da5.Fill(dt5);
                {
                    SERID = dt5.Rows[0].ItemArray[0].ToString();
                    SERDIS = dt5.Rows[0].ItemArray[1].ToString();
                    SERGL = dt5.Rows[0].ItemArray[2].ToString();
                }






            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetJobDoneby()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT Description FROM tbl_ItemCustom6 order by ID";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cmbJobDone.DataSource = dt;
                    cmbJobDone.ValueMember = "Description";
                    cmbJobDone.DisplayMember = "Description";
                    cmbJobDone.DisplayLayout.Bands[0].Columns[0].Width = 120;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetCurrentUserDate()
        {
            try
            {
                dtpDate.Value = user.LoginDate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cmbWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetDefaultWarehouse()
        {

        }

        private void clearControls()
        {
            try
            {
                lblCompany.Text = "";
                ug.PerformAction(UltraGridAction.ExitEditMode);
                txtVehicleNo.Text = "";
                txtTyreSize.Text = "";
                lblVoid.Visible = false;
                //1-Inclusive,2-Exclusive,3-non VAT 
                txtInvoiceNo.Text = "";
                IsEdit = false;
                txtInvoiceNo.Enabled = true;
                btnDelete.Enabled = false;
                btnVoid.Enabled = false;
                btnSave.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = false;
                btnSNO.Enabled = false;
                btnSearch.Enabled = true;
                btnReset.Enabled = true;
                btnEditer.Enabled = false;
                EnableHeader(true);
                EnableFoter(true);
                txtbalance.Text = "0.00";
                txtpaid.Text = "0.00"; txtpaidAmount.Text = "0.00";
                ClearHeader();
                DeleteRows();
                dataGridView1.Rows.Clear();
                cmbCustomer.Focus();
                //SetDefaultWarehouse();
                GetItemDataSet();
                if (user.IsCINVNoAutoGen) txtInvoiceNo.ReadOnly = true;
                else txtInvoiceNo.ReadOnly = false;
                optCredit.Checked = true;
                LoadCreditcardData();
                cmbInvoiceType.Value = 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetTaxDeails()
        {
            try
            {
                String S1 = "Select * from tblTaxApplicable order by Rank";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtNBTPer.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
                    txtVatPer.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void New()
        {
            try
            {
                IsFind = false;
                GetWareHouseDataSet();
                GetCustomer();
                GetSalesRep();
                clearControls();
                GetTaxDeails();
                loadDefaltOption();
                ValidateInvoiceSetting();
                clsSerializeItem.DtsSerialNoList.Rows.Clear();
                if (user.IsSINVNoAutoGen) txtInvoiceNo.ReadOnly = true;
                else txtInvoiceNo.ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch.Enabled = true;
                clearControls();
                SetReadOnly(false);
                POSPrint = false;
                btnSave.Enabled = true;
                GetJobDoneby();
                ClearHeader();
                IsFind = false;
                GetWareHouseDataSet();
                GetSalesRep();
                Close = false;
                GetTaxDeails();
                loadDefaltOption();
                ValidateInvoiceSetting();
                clsSerializeItem.DtsSerialNoList.Rows.Clear();
                if (user.IsSINVNoAutoGen) txtInvoiceNo.ReadOnly = true;
                else txtInvoiceNo.ReadOnly = false;
                txtCusPONum.Text = string.Empty;
                btnConfirm.Enabled = false;
                btnEditer.Enabled = false;
                IsEdit = false;
                GetItemDataSet();

                txtNBTPer.Enabled = false;
                txtVatPer.Enabled = false;

                SetReadOnly(false);
                ug.Enabled = true;
                GetCustomer();

                cmbCustomer.Focus();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private int GetEstimateCode(string strJobID, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT AutoIndex FROM tblJobMaster WHERE JobID='" + strJobID + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //============================================
        private void ImportSalesInvoice_Old(int intGrid, string StrReference)
        {

            double _Doscount = 0;
            double _Amount = 0;
            _Doscount = double.Parse(txtDiscAmount.Value.ToString());

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            int _ItemsRowCount = ug.Rows.Count;
            int _TaxLines = 0;

            if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
                _TaxLines = _TaxLines + 1;

            if (txtVat.Text.Trim() != string.Empty && double.Parse(txtVat.Text.Trim()) > 0)
                _TaxLines = _TaxLines + 1;

            int NoDistributions = 0;

            if (txtServicecharge.Text.Trim() != string.Empty && double.Parse(txtServicecharge.Text.Trim()) > 0)
            {
                NoDistributions = _ItemsRowCount + _TaxLines + 1;
            }
            else
            {
                NoDistributions = _ItemsRowCount + _TaxLines;
            }

            if (_Doscount > 0)
            {
                NoDistributions = NoDistributions + 1;
            }


            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);

            Writer.WriteStartElement("PAW_Invoice");
            Writer.WriteAttributeString("xsi:type", "paw:Invoice");

            Writer.WriteStartElement("Customer_ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbCustomer.Text.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Date");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(dtpDate.Value.ToString("MM/dd/yyyy"));
            Writer.WriteEndElement();

            Writer.WriteStartElement("Invoice_Number");
            Writer.WriteString(StrReference);
            Writer.WriteEndElement();



            Writer.WriteStartElement("Sales_Representative_ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            if (user.MergeAccUser)
            {
                Writer.WriteString(user.userName.ToString().Trim());
            }
            else
            {
                Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
            }
            //Writer.WriteString(cmbSalesRep.Value.ToString().Trim()); //(MergeUser.ToString());//

            Writer.WriteEndElement();

            Writer.WriteStartElement("Accounts_Receivable_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrARAccount);
            Writer.WriteEndElement();

            Writer.WriteStartElement("CreditMemoType");
            Writer.WriteString("FALSE");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString((NoDistributions).ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("SalesLines");


            for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
            {
                if (IsThisItemSerial(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString()))
                {
                    foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                    {
                        if (dr["ItemCode"].ToString() == ug.Rows[intGrid].Cells["ItemCode"].Value.ToString())
                        {
                            double dblUnitPriceExport = 0;
                            if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                            {
                                dblUnitPriceExport = double.Parse(ug.Rows[intGrid].Cells["UnitPrice(Incl)"].Value.ToString());
                            }
                            else
                            {
                                dblUnitPriceExport = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                            }


                            string a = ug.Rows[intGrid].Cells["GL"].Value.ToString();
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                            Writer.WriteEndElement();



                            Writer.WriteStartElement("Item_ID");
                            Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Description");
                            Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(ug.Rows[intGrid].Cells["GL"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Serial_Number");
                            Writer.WriteString(dr["SerialNo"].ToString());
                            Writer.WriteEndElement();

                            _Amount = double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) * dblUnitPriceExport;

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + _Amount);//HospitalCharge
                            Writer.WriteEndElement();

                        }
                    }
                }
                else
                {

                    double dblUnitPriceExport = 0;
                    if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                    {
                        dblUnitPriceExport = double.Parse(ug.Rows[intGrid].Cells["UnitPrice(Incl)"].Value.ToString());
                    }
                    else
                    {
                        dblUnitPriceExport = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                    }


                    string a = ug.Rows[intGrid].Cells["GL"].Value.ToString();

                    Writer.WriteStartElement("SalesLine");
                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(ug.Rows[intGrid].Cells["GL"].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    _Amount = double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) * dblUnitPriceExport;

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + _Amount);//HospitalCharge
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }
            }
            Writer.WriteEndElement();
            if (double.Parse(txtNBT.Value.ToString()) > 0)
            {
                Writer.WriteStartElement("SalesLine");
                Writer.WriteStartElement("Quantity");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Item_ID");
                Writer.WriteString("NBTR");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString("NBT");
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(user.tax1GL);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + txtNBT.Value.ToString());
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }
            if (double.Parse(txtVat.Value.ToString()) > 0)
            {
                Writer.WriteStartElement("SalesLine");
                Writer.WriteStartElement("Quantity");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Item_ID");
                Writer.WriteString("VATR");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString("VAT Receivable");
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(user.tax2GL);
                Writer.WriteEndElement();
                //========================================================
                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + txtVat.Value.ToString());
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }
            if (_Doscount > 0)
            {
                Writer.WriteStartElement("SalesLine");
                Writer.WriteStartElement("Quantity");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Item_ID");
                Writer.WriteString(user.DiscountItemID);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString("Discount");
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(user.DiscountGL);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString(_Doscount.ToString());
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }
            if (double.Parse(txtServicecharge.Value.ToString()) > 0)
            {
                Writer.WriteStartElement("SalesLine");
                Writer.WriteStartElement("Quantity");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Item_ID");
                Writer.WriteString(user.ServiceChargesItemID);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString("Service Charges");
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(user.ServiceChargesGL);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + txtServicecharge.Value.ToString());
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
            Writer.WriteEndElement();
            Writer.Close();

            Connector ObjImportP = new Connector();
            ObjImportP.ImportDirectSalesInvice();
        }
        private void ImportSalesInvoice(int intGrid, string StrReference, SqlConnection con, SqlTransaction trans)
        {

            try
            {
                double _Doscount = 0;
                double _Amount = 0;
                _Doscount = double.Parse(txtDiscAmount.Value.ToString());

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                int _ItemsRowCount = ug.Rows.Count;
                int _TaxLines = 0;

                if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
                    _TaxLines = _TaxLines + 1;

                if (txtVat.Text.Trim() != string.Empty && double.Parse(txtVat.Text.Trim()) > 0)
                    _TaxLines = _TaxLines + 1;

                int NoDistributions = 0;

                if (txtServicecharge.Text.Trim() != string.Empty && double.Parse(txtServicecharge.Text.Trim()) > 0)
                {
                    NoDistributions = _ItemsRowCount + _TaxLines + 1;
                }
                else
                {
                    NoDistributions = _ItemsRowCount + _TaxLines;
                }

                if (txtDiscAmount.Text.Trim() != string.Empty && double.Parse(txtDiscAmount.Text.Trim()) > 0)
                {
                    NoDistributions++;
                }



                double TotalLineDiscount = 0;

                for (int i = 0; i < ug.Rows.Count; i++)
                {
                    try
                    {
                        TotalLineDiscount = TotalLineDiscount + double.Parse(ug.Rows[i].Cells["LineDisc"].Value.ToString());

                    }
                    catch (Exception ex)
                    {

                    }

                }

                //if (TotalLineDiscount > 0)
                //{
                //   NoDistributions++;
                //}

                //if (_Doscount > 0)
                //{
                //    NoDistributions = NoDistributions + 1;
                //}

                double QTY = 0.00;
                DateTime DTP = Convert.ToDateTime(dtpDate.Text);
                string Dformat = "MM/dd/yyyy";
                string InvDate = DTP.ToString(Dformat);

                Writer.WriteStartElement("PAW_Invoice");
                Writer.WriteAttributeString("xsi:type", "paw:Invoice");

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomer.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Date");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(InvDate);
                Writer.WriteEndElement();


                Writer.WriteStartElement("Date_Due");
                Writer.WriteString(InvDate);//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(StrReference);
                Writer.WriteEndElement();

                string crtype = null;
                if (optCredit.Text.ToString() == "Credit" && combMode.Text.ToString() == "Credit")
                {
                    crtype = "Credit";
                }
                else
                {
                    crtype = "Cash";
                }

                Writer.WriteStartElement("Ship_Via");
                Writer.WriteString(crtype);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Representative_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                if (user.MergeAccUser)
                {
                    Writer.WriteString(user.userName.ToString().Trim());
                }
                else
                {
                    Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                }
                //Writer.WriteString(cmbSalesRep.Value.ToString().Trim()); //(MergeUser.ToString());//

                Writer.WriteEndElement();

                Writer.WriteStartElement("Accounts_Receivable_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(StrARAccount);
                Writer.WriteEndElement();

                Writer.WriteStartElement("CreditMemoType");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString((NoDistributions).ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("SalesLines");


                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {

                    if (IsThisItemSerial(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString()))
                    {
                        foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                        {
                            if (dr["ItemCode"].ToString() == ug.Rows[intGrid].Cells["ItemCode"].Value.ToString())
                            {
                                double dblUnitPriceExport = 0;
                                double Disam = 0.00;
                                double vatper = double.Parse(txtVatPer.Value.ToString());
                                double Taxam = 0.00;
                                if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                                {
                                    _Amount = 0;
                                    _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());

                                    if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()));
                                        _Amount = (_Amount / (vatper + 100)) * 100;
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        Taxam = double.Parse(ug.Rows[intGrid].Cells["LineTax"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                        Disam = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) - Taxam) * double.Parse(txtDiscPer.Text.ToString()) / 100;
                                        _Amount = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) - Taxam) - Disam;
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineTax"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        //Discount 1
                                        _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()));
                                        _Amount = (_Amount / (vatper + 100)) * 100;
                                        //Discount 2
                                        _Amount = _Amount - (_Amount * double.Parse(txtDiscPer.Text.ToString()) / 100);

                                    }
                                    dblUnitPriceExport = _Amount;
                                }
                                if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                                {
                                    _Amount = 0;
                                    _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                    if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        Disam = double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                        _Amount = _Amount - Disam;
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                    }
                                    dblUnitPriceExport = _Amount;
                                }
                                else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                                {
                                    _Amount = 0;
                                    _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                    if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        Disam = double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                        _Amount = _Amount - Disam;
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                    }
                                    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                    }
                                    dblUnitPriceExport = _Amount;
                                }
                                string a = ug.Rows[intGrid].Cells["GL"].Value.ToString();
                                Writer.WriteStartElement("SalesLine");
                                // Writer.WriteEndElement();

                                QTY = double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) + double.Parse(ug.Rows[intGrid].Cells["FOCQty"].Value.ToString());
                                Writer.WriteStartElement("Quantity");
                                Writer.WriteString(QTY.ToString());
                                Writer.WriteEndElement();



                                Writer.WriteStartElement("Item_ID");
                                Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Description");
                                Writer.WriteString(GetItemDescription(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString().Trim(), con, trans));
                                Writer.WriteEndElement();

                                string GL_code = ug.Rows[intGrid].Cells["GL"].Value.ToString();
                                if (IsGLok == true)
                                {
                                    GL_code = StrSalesGLAccount;
                                }

                                Writer.WriteStartElement("GL_Account");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(GL_code);
                                Writer.WriteEndElement();


                                Writer.WriteStartElement("Tax_Type");
                                Writer.WriteString("1");
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Serial_Number");
                                Writer.WriteString(dr["SerialNo"].ToString());
                                Writer.WriteEndElement();

                                dblUnitPriceExport = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());

                                _Amount = double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                                if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) == 0.00)
                                {
                                    _Amount = 0.00;
                                }
                                if (rbtVAT.Checked == true)
                                {
                                    _Amount = double.Parse(ug.Rows[intGrid].Cells["TotalPrice(Incl)"].Value.ToString());

                                }
                                Writer.WriteStartElement("Amount");
                                Writer.WriteString("-" + _Amount);
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();//End of sales line element sanjeewa edited thid code on 04/10/2013

                            }
                        }
                    }
                    else
                    {

                        double dblUnitPriceExport = 0;
                        double Disam = 0.00;
                        double vatper = double.Parse(txtVatPer.Value.ToString());
                        double NBTper = double.Parse(txtNBTPer.Value.ToString());
                        double _NBTAm = 0.00;
                        double Taxam = 0.00;
                        if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                        {
                            //double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString())
                            _Amount = 0;
                            _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());

                            //if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                            //{
                            //    _Amount = _Amount - (_Amount * (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) / 100));
                            //    _Amount = (_Amount / (NBTper + 100)) * 100;
                            //    _Amount = (_Amount / (vatper + 100)) * 100;
                            //}
                            //else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                            //{
                            //    Taxam = double.Parse(ug.Rows[intGrid].Cells["LineTax"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                            //    Disam = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) - Taxam) * double.Parse(txtDiscPer.Text.ToString()) / 100;
                            //    _Amount = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) - Taxam) - Disam;
                            //}
                            //else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                            //{
                            //    _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineTax"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                            //}
                            //else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                            //{
                            //    //Discount 1
                            //    //_Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()));
                            //    _Amount = _Amount - (_Amount * (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) / 100));
                            //    _Amount = (_Amount / (vatper + 100)) * 100;
                            //    //Discount 2
                            //    _Amount = _Amount - (_Amount * double.Parse(txtDiscPer.Text.ToString()) / 100);

                            //}
                            dblUnitPriceExport = _Amount;
                        }
                        if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                        {
                            _Amount = 0;
                            _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                            //if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                            //{
                            //    Disam = double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                            //    _Amount = _Amount - Disam;
                            //}
                            //else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                            //{
                            //    _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                            //}
                            //else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                            //{
                            //    _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                            //}
                            //else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                            //{
                            //    _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                            //    _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                            //}
                            dblUnitPriceExport = _Amount;
                        }
                        //else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                        //{
                        //    _Amount = 0;
                        //    _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                        //    if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                        //    {
                        //        Disam = double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                        //        _Amount = _Amount - Disam;
                        //    }
                        //    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                        //    {
                        //        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                        //    }
                        //    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                        //    {
                        //        _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                        //    }
                        //    else if (double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                        //    {
                        //        _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                        //        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                        //    }
                        //    dblUnitPriceExport = _Amount;
                        //}

                        string a = ug.Rows[intGrid].Cells["GL"].Value.ToString();

                        Writer.WriteStartElement("SalesLine");
                        QTY = double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) + double.Parse(ug.Rows[intGrid].Cells["FOCQty"].Value.ToString());

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString(QTY.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(GetItemDescription(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString().Trim(), con, trans));
                        Writer.WriteEndElement();

                        string GL_code = ug.Rows[intGrid].Cells["GL"].Value.ToString();
                        if (IsGLok == true)
                        {
                            GL_code = StrSalesGLAccount;
                        }
                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(GL_code);
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");
                        Writer.WriteEndElement();

                        dblUnitPriceExport = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                        //  _Amount = double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) * dblUnitPriceExport;
                        ///   _Amount = double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                        ///   
                        _Amount = double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                        if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) == 0.00)
                        {
                            _Amount = 0.00;
                        }

                        if (rbtVAT.Checked == true)
                        {
                            if (rbtVAT.Checked == true)
                            {
                                _Amount = double.Parse(ug.Rows[intGrid].Cells["TotalPrice(Incl)"].Value.ToString());

                            }

                        }
                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + _Amount);//HospitalCharge
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();
                    }
                }
                //  Writer.WriteEndElement();//End OF sales Lines 
                if (double.Parse(txtNBT.Value.ToString()) > 0)
                {
                    Writer.WriteStartElement("SalesLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(NBitemid);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(NBTitemDis);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(NBTitemGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + txtNBT.Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }
                if (double.Parse(txtVat.Value.ToString()) > 0)
                {
                    Writer.WriteStartElement("SalesLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(VATitemid);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(VATitemDis);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(VATGL);
                    Writer.WriteEndElement();
                    //========================================================
                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + txtVat.Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }
                if (_Doscount > 0)
                {
                    Writer.WriteStartElement("SalesLine");
                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(Cashitemid);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(cashitemdis);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cashGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(_Doscount.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }
                if (double.Parse(txtServicecharge.Value.ToString()) > 0)
                {
                    Writer.WriteStartElement("SalesLine");
                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(SERID);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(SERDIS);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(SERGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + txtServicecharge.Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }

                //if (double.Parse(TotalLineDiscount.ToString()) > 0)
                //{
                //    Writer.WriteStartElement("SalesLine");
                //    Writer.WriteStartElement("Quantity");
                //    Writer.WriteString("1");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Item_ID");
                //    Writer.WriteString(LineDisitemid);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Description");
                //    Writer.WriteString(LineDisitemdescription);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("GL_Account");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(LineDisGLAccount);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Tax_Type");
                //    Writer.WriteString("1");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Amount");
                //    Writer.WriteString(TotalLineDiscount.ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteEndElement();
                //}

                Writer.WriteEndElement();
                Writer.WriteEndElement();
                Writer.Close();

                Connector ObjImportP = new Connector();
                ObjImportP.ImportDirectSalesInvice();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetItemDescription(string ItemId, SqlConnection con, SqlTransaction trans)
        {
            try
            {
                String StrSql = "SELECT [ItemDescription] FROM [tblItemMaster] where  ItemID ='" + ItemId + "'";
                SqlCommand command = new SqlCommand(StrSql, con, trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0 && dt.Rows[0].ItemArray[0].ToString().Trim() != "")
                {

                    return dt.Rows[0].ItemArray[0].ToString().Trim();


                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return "";
        }

        private bool IsThisItemSerial(string _ItemCode)
        {
            try
            {
                //if (ug.ActiveRow == null ) return false;
                //if (ug.ActiveRow.Cells[0].Value == null) return false;
                //mmm
                bool IsThisItemSerial = false;
                string ItemClass = "";
                //================================
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

        //following code segment export receipts to Peachtree=========

        public void exporetReceipt_Cash(string StrReference)
        {

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);

            int rowCount1 = 1;

            int _ItemsRowCount = ug.Rows.Count;
            //int _TaxLines = 0;

            //if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
            //    _TaxLines = _TaxLines + 1;

            //if (txtVat.Text.Trim() != string.Empty && double.Parse(txtVat.Text.Trim()) > 0)
            //    _TaxLines = _TaxLines + 1;

            int NoDistributions = 1;

            //if (txtServicecharge.Text.Trim() != string.Empty && double.Parse(txtServicecharge.Text.Trim()) > 0)
            //{
            //    NoDistributions = _ItemsRowCount + _TaxLines + 1;
            //}
            //else
            //{
            //    NoDistributions = _ItemsRowCount + _TaxLines;
            //}

            //if (txtDiscAmount.Text.Trim() != string.Empty && double.Parse(txtDiscAmount.Text.Trim()) > 0)
            //{
            //    NoDistributions++;
            //}



            //double TotalLineDiscount = 0;

            //for (int i = 0; i < ug.Rows.Count; i++)
            //{
            //    try
            //    {
            //        TotalLineDiscount = TotalLineDiscount + double.Parse(ug.Rows[i].Cells["LineDisc"].Value.ToString());
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //}

            //if (TotalLineDiscount > 0)
            //{
            //    NoDistributions++;
            //}



            for (int i = 0; i < rowCount1; i++)
            {
                if (double.Parse(txtNetValue.Text.ToString()) > 0)
                {
                    Writer.WriteStartElement("PAW_Receipt");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(StrReference + "R");
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    if (user.MergeAccUser)
                    {
                        Writer.WriteString(user.userName.ToString().Trim());
                    }
                    else
                    {
                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                    }
                    //Writer.WriteString(cmbSalesRep.Value.ToString().Trim()); //(MergeUser.ToString());//

                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(InvDate);//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Payment_Method");
                    Writer.WriteString("Cash");//PayMethod
                    Writer.WriteEndElement();

                    string cardAccount = "";
                    String S = "Select GL_Account from tblCreditData where CardType='Cash'";
                    SqlCommand cmd1 = new SqlCommand(S);
                    SqlDataAdapter da1 = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt1 = new DataSet();
                    da1.Fill(dt1);
                    cardAccount = dt1.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                    Writer.WriteStartElement("Cash_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cardAccount);//Cash Account
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Total_Paid_On_Invoices");
                    Writer.WriteString(txtNetValue.Text.ToString().Trim());//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("ReceiptNumber");
                    Writer.WriteString("R" + StrReference);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString(NoDistributions.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Distributions");
                    Writer.WriteStartElement("Distribution");

                    Writer.WriteStartElement("InvoicePaid");
                    Writer.WriteString(StrReference);//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(txtNetValue.Text.ToString().Trim());//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    // Writer.Close();
                }
            }

            Writer.Close();
            Connector ObjReceiptP = new Connector();
            ObjReceiptP.Import_Receipt_Journal();


        }

        public void exporetReceipt_Credit(string StrReference)
        {
            int intGrid;
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);

            int rowCount1 = 1;

            for (int i = 0; i < rowCount1; i++)
            {
                if (rdobtnCreditCard.Checked)
                {
                    if (double.Parse(txtcardamount.Text.ToString()) > 0)
                    {
                        for (intGrid = 0; intGrid < dataGridView1.Rows.Count - 1; intGrid++)
                        {
                            if (dataGridView1.Rows[intGrid].Cells[0].Value != null && dataGridView1.Rows[intGrid].Cells[0].Value.ToString().Trim() != string.Empty)
                            {
                                Writer.WriteStartElement("PAW_Receipt");
                                Writer.WriteAttributeString("xsi:type", "paw:receipt");

                                Writer.WriteStartElement("Customer_ID");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Reference");
                                //(dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString() == "Cash")
                                if (dataGridView1.Rows[intGrid].Cells[0].Value.ToString().ToUpper() == "CASH")
                                {
                                    Writer.WriteString(StrReference + "R".ToString());
                                    Writer.WriteEndElement();
                                }
                                else if (dataGridView1.Rows[intGrid].Cells[0].Value.ToString().ToUpper() == "ADVANCE")
                                {
                                    Writer.WriteString(StrReference + "ADV".ToString());
                                    Writer.WriteEndElement();
                                }
                                else if (dataGridView1.Rows[intGrid].Cells[0].Value.ToString() == "Check")
                                {
                                    Writer.WriteString(dataGridView1.Rows[intGrid].Cells[1].Value.ToString());
                                    Writer.WriteEndElement();
                                }
                                else
                                {
                                    Writer.WriteString(dataGridView1.Rows[intGrid].Cells[1].Value.ToString() + "R");
                                    Writer.WriteEndElement();
                                }

                                Writer.WriteStartElement("Sales_Representative_ID");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                if (user.MergeAccUser)
                                {
                                    Writer.WriteString(user.userName.ToString().Trim());
                                }
                                else
                                {
                                    Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                                }
                                //Writer.WriteString(cmbSalesRep.Value.ToString().Trim()); //(MergeUser.ToString());//

                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Date ");
                                Writer.WriteAttributeString("xsi:type", "paw:Date");
                                Writer.WriteString(InvDate);//(DateTime.Parse(InvDate).ToString("MM/dd/yyyy"));//Date 
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Payment_Method");
                                Writer.WriteString(dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString());//PayMethod
                                Writer.WriteEndElement();
                                string cardAccount = "";
                                String S = "Select GL_Account from tblCreditData where CardType='" + dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString() + "'";
                                SqlCommand cmd2 = new SqlCommand(S);
                                SqlDataAdapter da2 = new SqlDataAdapter(S, ConnectionString);
                                DataSet dt2 = new DataSet();
                                da2.Fill(dt2);
                                cardAccount = dt2.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                                Writer.WriteStartElement("Cash_Account");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(cardAccount);//Cash Account
                                Writer.WriteEndElement();
                            }


                            Writer.WriteStartElement("Total_Paid_On_Invoices");
                            Writer.WriteString(double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString().Trim()).ToString("0.00"));//PayMethod
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("ReceiptNumber");
                            Writer.WriteString("R" + StrReference);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Number_of_Distributions ");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("Distributions");
                            Writer.WriteStartElement("Distribution");

                            Writer.WriteStartElement("InvoicePaid");
                            Writer.WriteString(StrReference);//PayMethod
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString(double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString().Trim()).ToString("0.00"));//PayMethod
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();

                        }
                    }
                }
            }

            Writer.Close();
            Connector ObjReceiptP = new Connector();
            ObjReceiptP.Import_Receipt_Journal();

        }
        //==============================================================
        private void DeleteEvent()
        {
            int intGrid;
            int intAutoIndex;
            double dblAvailableQty;
            string StrReference = null;
            int intItemClass;
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            try
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to Edit this record ? ", "Information", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.Cancel)
                {
                    return;
                }
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                StrReference = txtInvoiceNo.Text.ToString().Trim();
                GetInvNoField(myConnection, myTrans);
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    DeleteInvoice(StrReference, myConnection, myTrans);
                    //  DeleteFromItemWisetbl(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);
                    //  DeleteFromActivity(StrReference, myConnection, myTrans);
                }
                myTrans.Commit();
                ButtonClear();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                throw ex;
            }
        }
        //============================================================
        //private void EditEvent()
        //{
        //    int IntInvoicetype = 1;
        //    if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
        //    {
        //        IntInvoicetype = 1;
        //    }
        //    if (Convert.ToInt64(cmbInvoiceType.Value) == 2)//inclusive
        //    {
        //        IntInvoicetype = 2;
        //    }
        //    if (Convert.ToInt64(cmbInvoiceType.Value) == 3)//inclusive
        //    {
        //        IntInvoicetype = 3;
        //    }
        //    if (cmbWarehouse.Value == null)
        //    {
        //        MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return;
        //    }

        //    if (cmbSalesRep.Value == null)
        //    {
        //        MessageBox.Show("Incorrect SalesRep", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return;
        //    }

        //    if (cmbCustomer.Value == null)
        //    {
        //        MessageBox.Show("Incorrect Customer", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return;
        //    }

        //    int intGrid;
        //    int intAutoIndex;
        //    double dblAvailableQty;
        //    string StrReference = null;

        //    int intItemClass;

        //    setConnectionString();
        //    SqlConnection myConnection = new SqlConnection(ConnectionString);
        //    SqlTransaction myTrans = null;

        //    try
        //    {
        //        int INVTYPE = 1;
        //        if (rbtNoVat.Checked == true)
        //        {
        //            INVTYPE = 1;
        //        }
        //        if (rbtVAT.Checked == true)
        //        {
        //            INVTYPE = 2;
        //        }
        //        if (rbtSVAT.Checked == true)
        //        {
        //            INVTYPE = 3;
        //        }

        //        DialogResult reply = MessageBox.Show("Are you sure, you want to Edit this record ? ", "Information", MessageBoxButtons.OKCancel);

        //        if (reply == DialogResult.Cancel)
        //        {
        //            return;
        //        }
        //        DeleteEmpGrid();

        //        if (IsGridValidation() == false)
        //        {
        //            return;
        //        }
        //        if (HeaderValidation() == false)
        //        {
        //            return;
        //        }
        //        if (optCash.Checked == true)
        //        {
        //            StrPaymmetM = "Cash";
        //        }
        //        else if (optCredit.Checked == true)
        //        {
        //            StrPaymmetM = "Credit";
        //        }

        //        myConnection.Open();
        //        myTrans = myConnection.BeginTransaction();
        //        StrReference = txtInvoiceNo.Text.ToString().Trim();
        //        DeleteInvoice(StrReference, myConnection, myTrans);

        //        for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
        //        {
        //            SaveDetails(INVTYPE, double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()), StrReference, ug.Rows.Count, intGrid + 1, StrPaymmetM, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), ug.Rows[intGrid].Cells["ItemClass"].Value.ToString(), ug.Rows[intGrid].Cells["GL"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(), ug.Rows[intGrid].Cells["Categoty"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), IntInvoicetype, double.Parse(ug.Rows[intGrid].Cells["UnitPrice(Incl)"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["LineTax"].Value.ToString()), txtCustomer.ToString(),txtCusPONum.Text.ToString(),txtMileage.Text.ToString(),txtJobdoneBy.Text.ToString(), myConnection, myTrans);
        //            intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());
        //            if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
        //            {
        //                dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);
        //                if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > dblAvailableQty)
        //                {
        //                    MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
        //                    myTrans.Rollback();
        //                    return;
        //                }

        //                UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);
        //                InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
        //            }
        //            //---------------------------------------
        //        }


        //        //--End PH3 Posting--------------------

        //        myTrans.Commit();
        //        MessageBox.Show("Invoice Successfuly Edited.", "Information", MessageBoxButtons.OK);
        //        //Print(StrReference);
        //        ButtonClear();
        //    }
        //    catch (Exception ex)
        //    {
        //        myTrans.Rollback();
        //        throw ex;

        //    }
        //}
        //==============================================
        private async void UpdateEvent()
        {

            int IntInvoicetype = 1;
            if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
            {
                IntInvoicetype = 1;
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 2)//inclusive
            {
                IntInvoicetype = 2;
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 3)//inclusive
            {
                IntInvoicetype = 3;
            }
            int INVTYPE = 1;
            if (rbtNoVat.Checked == true)
            {
                INVTYPE = 1;
            }
            if (rbtVAT.Checked == true)
            {
                INVTYPE = 2;
            }
            if (rbtSVAT.Checked == true)
            {
                INVTYPE = 3;
            }

            if (!user.IsSINVNoAutoGen)
            {
                if (txtInvoiceNo.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Enter Invoice No....!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbWarehouse.Focus();
                return;
            }
            if (cmbSalesRep.Value == null)
            {
                MessageBox.Show("Incorrect SalesRep", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbSalesRep.Focus();
                return;
            }


            //if ((txtpaid.Text == null || (Convert.ToDouble(txtNetValue.Value) > Convert.ToDouble(txtpaidAmount.Text))) && optCash.Checked == true)
            //    {
            //    MessageBox.Show("Invoice Paid Amount Incorrect or Blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    txtpaidAmount.Focus();
            //    return;
            //}
            //if (txtpaid.Text == null)
            //{
            //    MessageBox.Show("Invoice Paid Amount Incorrect or Blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}

            //if (double.Parse(txtpaidAmount.Text.ToString()) == 0)
            //{
            //    MessageBox.Show("Please Enter Paid Amount", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}


            //if (cmbJobDone.Text.ToString() == "" || cmbJobDone.Text.ToString() == string.Empty)
            //{
            //    MessageBox.Show("Please Select Job done by", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    cmbJobDone.Focus();
            //    return;
            //}

            if (cmbCustomer.Text != "OTC")
            {
                //if (TxtPatientName.Text == string.Empty)
                //{
                //    MessageBox.Show("Please Enter Name", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}
                //if (txtPatientContactNo.Text == string.Empty)
                //{
                //    MessageBox.Show("Please Enter Contact No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}

                //if (txtVehicleNo.Text == string.Empty)
                //{
                //    MessageBox.Show("Please Enter Vehicle No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}
            }


            double CreditCAm;
            double Netam;
            double PaidAmount;
            PaidAmount = 0;
            CreditCAm = 0;
            Netam = 0;
            if (txtpaid.Text != null)
            {
                PaidAmount = double.Parse(txtpaid.Text.ToString());
            }
            //if (PaidAmount < 0)
            //{
            //    MessageBox.Show("Paid Amount is Zero....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //if (rdobtnCreditCard.Checked == true)
            //{
            //    if (txtNetValue.Text != null)
            //    {
            //        CreditCAm = double.Parse(txtcardamount.Text.ToString());
            //    }
            //    Netam = double.Parse(txtNetValue.Text.ToString());
            //    if (CreditCAm != Netam)
            //    {
            //        DialogResult reply1 = MessageBox.Show("You Have a Advance Rs. " + double.Parse(txtcardamount.Text.ToString()) + " . You want to Proceed This..? ", "Information", MessageBoxButtons.OKCancel);
            //        if (reply1 != DialogResult.Cancel)
            //        {

            //        }
            //        else
            //        {
            //            MessageBox.Show("Invalid Payment....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            return;
            //        }

            //    }

            //}

            int intGrid;
            double dblAvailableQty;
            string StrReference = null;

            int intItemClass;

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;



            try
            {
                //DialogResult reply = MessageBox.Show("Are you sure, you want to Update this record ? ", "Information", MessageBoxButtons.OKCancel);

                //if (reply == DialogResult.Cancel)
                //{
                //    return;
                //}
                DeleteEmpGrid();
                if (IsGridValidationItemIdandDis() == false)
                {

                    return;
                }


                if (GrideValidation == true)
                {
                    return;
                }
                CheckRowsValidation();

                if (PayValChecked == true)
                {
                    MessageBox.Show("Invalid Card Amount or Pay Type", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (IsGridValidation() == false)
                {
                    return;
                }



                if (HeaderValidation() == false)
                {
                    return;
                }

                if (optCash.Checked == true)
                {
                    StrPaymmetM = "Cash";

                }
                else if (optCredit.Checked == true)
                {
                    StrPaymmetM = "Credit";

                }
                else if (rdobtnCreditCard.Checked)
                {
                    StrPaymmetM = "CreditCard";
                }

                //if (!IsSerialNoCorrect())
                //    return;

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                //ALIGNMENT//WHEEL ALIGNMENT
                IsWheelAlignemntDone = "No";
                //====================
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Categoty"].Value.ToString() == "WHEEL ALIGNMENT")
                    {
                        IsWheelAlignemntDone = "Yes";
                    }
                }


                //if (txtPatientContactNo.Text.ToString().Trim() != "")
                //{
                //    if (IsWheelAlignemntDone == "Yes")
                //    {
                //        if (txtMileage.Text.ToString() != string.Empty)
                //        {
                //            double nextmileAge = 0;
                //            nextmileAge = double.Parse(txtMileage.Text.ToString()) + 3000;

                //            var smsManager = new SmsManager();
                //            var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                //            var sms1 = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,\nWe would like to inform you that the first wheel alignment for your vehicle (" + txtVehicleNo.Text.ToString().Trim() + ") was completed on " + dtpDate.Value.ToString("dd/MM/yyyy") + ". \nYou are entitled to a free wheel alignment at " + nextmileAge.ToString().Trim() + "Km. \nFor any assistance, feel free to contact our Hotline at 0773131883. \nThank you for choosing Lasantha Tyre Traders. We look forward to serving you again!");
                //            await smsManager.SendSms(sms);
                //            await smsManager.SendSms(sms1);
                //        }
                //    }
                //    else
                //    {
                //        var smsManager = new SmsManager();
                //        var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                //        await smsManager.SendSms(sms);
                //    }
                //}
                //==============================
                DELETEINSERTITEM(myConnection, myTrans);
                DeleteFromItemWisetbl(txtInvoiceNo.Text.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);
                DeleteFromActivity(txtInvoiceNo.Text.ToString(), myConnection, myTrans);

                //if (user.IsSINVNoAutoGen)
                //{
                //    //StrReference = GetInvNoFieldNew(myConnection, myTrans);
                //    // UpdatePrefixNonew(myConnection, myTrans);

                //    StrReference = GetInvNoField_CashOR_Credit(myConnection, myTrans);
                //    UpdatePrefixCashCreditNonew(myConnection, myTrans);
                //    txtInvoiceNo.Text = StrReference;
                //    DELETEINSERTITEM(myConnection, myTrans);
                //}
                //else
                //{
                //    SqlCommand myCommand = new SqlCommand("select * from tblSalesInvoices where InvoiceNo='" + txtInvoiceNo.Text.Trim() + "'", myConnection, myTrans);
                //    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                //    DataTable dt41 = new DataTable();
                //    da41.Fill(dt41);

                //    if (dt41.Rows.Count > 0)
                //    {
                //        MessageBox.Show("Invoice No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        myTrans.Rollback();
                //        myConnection.Close();//
                //        return;
                //    }
                //    else
                //    {
                //        StrReference = txtInvoiceNo.Text.ToString();
                //    }
                //}
                StrReference = txtInvoiceNo.Text.ToString();
                double Paid_Am;
                double AM_bal;
                Paid_Am = 0;
                AM_bal = 0;
                if (txtpaid.Text != null)
                {
                    Paid_Am = double.Parse(txtpaid.Text.ToString());
                }
                if (txtbalance.Text != null)
                {
                    AM_bal = double.Parse(txtbalance.Text.ToString());
                }
                if (txtpaid.Text != null)
                {
                    SaveInvBalance(StrReference, Netam, Paid_Am, AM_bal, StrPaymmetM, myConnection, myTrans);
                }
                string CRCardNo = "";
                string TransactionType = "INV";

                if (StrPaymmetM == "CreditCard")
                {
                    for (intGrid = 0; intGrid < dataGridView1.Rows.Count - 1; intGrid++)
                    {

                        if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value != null && double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()) > 0)
                        {
                            if (dataGridView1.Rows[intGrid].Cells["colcardno"].Value == null && dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString() != "CASH" && dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString() != "Advance")
                            {
                                MessageBox.Show("Please Enter Card No", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                myTrans.Rollback();
                                return;
                            }
                            if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString().ToUpper() == "CASH")
                            {
                                CRCardNo = "Cash";
                            }
                            else if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString().ToUpper() == "ADVANCE")
                            {
                                CRCardNo = "Advance";
                            }
                            else
                            {
                                CRCardNo = dataGridView1.Rows[intGrid].Cells["colcardno"].Value.ToString();
                            }
                            SaveCardDetailsWinlanka(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), TransactionType, cmbWarehouse.Value.ToString(), cmbCustomer.Value.ToString(), myConnection, myTrans);

                            //SaveCardDetails(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), myConnection, myTrans);
                        }

                    }
                }
                else
                {
                    SaveCardDetailsWinlanka(StrReference, StrPaymmetM, StrPaymmetM, double.Parse(txtNetValue.Value.ToString()), TransactionType, cmbWarehouse.Value.ToString(), cmbCustomer.Value.ToString(), myConnection, myTrans);
                    // SaveCardDetails(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), myConnection, myTrans);

                }

                String WareHose = "Lasantha TYRE TRADERS";

                if (rbtVAT.Checked == false)
                {
                    for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                    {

                        if (ug.Rows[intGrid].Cells["WH"].Value.ToString() == "NEW Lasantha TYRE TRADERS")
                        {
                            WareHose = ug.Rows[intGrid].Cells["WH"].Value.ToString();
                        }
                    }
                }

                if (lblCompany.Text.ToString().Trim() != WareHose)
                {
                    MessageBox.Show("Cannot Change Default Company This Invoice Already Contain " + lblCompany.Text + " Serial Invoice Number");
                    return;
                }
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    Boolean Confirm = false;
                    SaveDetails(INVTYPE, ug.Rows[intGrid].Cells["Discount"].Value.ToString(),
                        double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()), StrReference, ug.Rows.Count, intGrid + 1,
                        StrPaymmetM, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(),
                        ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                        ug.Rows[intGrid].Cells["ItemClass"].Value.ToString(), ug.Rows[intGrid].Cells["GL"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(),
                        ug.Rows[intGrid].Cells["Categoty"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), IntInvoicetype,
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice(Incl)"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["LineTax"].Value.ToString()),
                        txtCustomer.Text.ToString(), txtCusPONum.Text.ToString(), txtMileage.Text.ToString(), cmbJobDone.Text.ToString(), Confirm, double.Parse(ug.Rows[intGrid].Cells["FOCQty"].Value.ToString()), ug.Rows[intGrid].Cells["WH"].Value.ToString(), WareHose, myConnection, myTrans);

                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());
                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {
                        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), myConnection, myTrans);
                        double QTY = double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) + double.Parse(ug.Rows[intGrid].Cells["FOCQty"].Value.ToString());
                        if (QTY > dblAvailableQty)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;
                        }

                        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), QTY, myConnection, myTrans);

                        InvTransaction(txtInvoiceNo.Text.Trim(), dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), QTY, double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
                    }
                }

                //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                //{
                //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                //        " TranType='Invoice',Status='OutOfStock' " +
                //        " where ItemID='" +
                //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                //    myCommandSe1.ExecuteNonQuery();
                //}

                //frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                //objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans,
                //    clsSerializeItem.DtsSerialNoList, "Invoice", cmbWarehouse.Text.ToString(),
                //    txtInvoiceNo.Text.ToString().Trim(), dtpDate.Value, true, "Invoiced");

                //if (optCash.Checked == true)
                //{
                //    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim());
                //    exporetReceipt_Cash(txtInvoiceNo.Text.Trim());
                //}
                //else if (rdobtnCreditCard.Checked == true)
                //{
                //    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim());
                //    exporetReceipt_Credit(txtInvoiceNo.Text.Trim());
                //}
                //else
                //{
                //    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim());
                //}
                myTrans.Commit();

                //DialogResult reply2 = MessageBox.Show("Are you sure, you want to Print this record ? ", "Information", MessageBoxButtons.OKCancel);

                //     if (reply2 == DialogResult.Cancel)
                //     {
                //         // btnNew_Click(null, null);
                //         return;
                //     }
                //     else
                //     {
                //         Print(txtInvoiceNo.Text.Trim());
                //     }

                POSPrint = false;
                DirectNormalPrint = false;
                btnConfirm.Enabled = true;
                btnSave.Enabled = false;
                btnEditer.Enabled = true;



                EnableHeader(false);
                EnableFoter(false);
                cmbWarehouse.Enabled = false;
                ug.Enabled = false;

                DialogResult reply = MessageBox.Show("Are you sure, you want to Print this record ? ", "Information", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    if (txtPatientContactNo.Text.ToString().Trim() != "")
                    {
                        if (IsWheelAlignemntDone == "Yes")
                        {
                            if (txtMileage.Text.ToString() != string.Empty)
                            {
                                double nextmileAge = 0;
                                nextmileAge = double.Parse(txtMileage.Text.ToString()) + 3000;

                                var smsManager = new SmsManager();
                                var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                                var sms1 = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,\nWe would like to inform you that the first wheel alignment for your vehicle (" + txtVehicleNo.Text.ToString().Trim() + ") was completed on " + dtpDate.Value.ToString("dd/MM/yyyy") + ". \nYou are entitled to a free wheel alignment at " + nextmileAge.ToString().Trim() + "Km. \nFor any assistance, feel free to contact our Hotline at 0773131883. \nThank you for choosing Lasantha Tyre Traders. We look forward to serving you again!");
                                await smsManager.SendSms(sms);
                                await smsManager.SendSms(sms1);
                            }
                        }
                        else
                        {
                            var smsManager = new SmsManager();
                            var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                            await smsManager.SendSms(sms);
                        }
                    }

                    Print(txtInvoiceNo.Text);

                }




              //  Print(txtInvoiceNo.Text);


                // btnNew_Click(null,null);
            }

            catch (Exception ex)
            {
                myTrans.Rollback();
                throw ex;
            }
        }
        string InvoiceNo;



        public Boolean IsGridValidationItemIdandDis()
        {
            try
            {

                if (ug.Rows.Count == 0)
                {
                    return false;
                }

                foreach (UltraGridRow ugR in ug.Rows)
                {

                    //string b = ugR.Cells["ItemCode"].Text;
                    //if (IsGridExitCode(ugR.Cells["ItemCode"].Text) == false)
                    //{
                    //    MessageBox.Show("Invalid Item Code In Line No :- " + ugR.Cells["LineNo"].Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return false;
                    //}

                    //if (ugR.Cells["Categoty"].Text != "Customize")
                    //{
                    //    if (IsGridExitCodeDes(ugR.Cells["Description"].Text) == false)
                    //    {
                    //        MessageBox.Show("Invalid Item Description In Line No :- " + ugR.Cells["LineNo"].Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //        return false;
                    //    }
                    //}

                    if ((ugR.Cells["Categoty"].Text) == "WHEEL ALIGNMENT")
                    {
                        if (txtVehicleNo.Text.ToString() == "")
                        {
                            MessageBox.Show("Please Enter VehicleNo", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtVehicleNo.Focus();
                            return false;
                        }

                        if (txtMileage.Text.ToString() == "")
                        {
                            MessageBox.Show("Please Enter Mileage", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMileage.Focus();
                            return false;
                        }


                    }

                    //if (ugR.Cells["ItemClass"].Value.ToString() != "7" && Convert.ToDouble(ugR.Cells["UnitPrice"].Value) == 0.00)
                    //{
                    //    MessageBox.Show("ListPrice is '0' in Line No :- " + ugR.Cells["LineNo"].Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return false;
                    //}
                }
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsGridExitCodeDes(string StrCode)
        {
            try
            {

                foreach (UltraGridRow ugR in ultraCombo2.Rows)
                {


                    if (ugR.Cells["ItemDescription"].Text == StrCode)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        public string IsWheelAlignemntDone = "No";
        private async void SaveEvents()
        {


            int IntInvoicetype = 1;
            if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
            {
                IntInvoicetype = 1;
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 2)//inclusive
            {
                IntInvoicetype = 2;
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 3)//inclusive
            {
                IntInvoicetype = 3;
            }
            int INVTYPE = 1;
            if (rbtNoVat.Checked == true)
            {
                INVTYPE = 1;
            }
            if (rbtVAT.Checked == true)
            {
                INVTYPE = 2;
            }
            if (rbtSVAT.Checked == true)
            {
                INVTYPE = 3;
            }

            if (!user.IsSINVNoAutoGen)
            {
                if (txtInvoiceNo.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Enter Invoice No....!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbWarehouse.Focus();
                return;
            }
            if (cmbSalesRep.Value == null)
            {
                MessageBox.Show("Incorrect SalesRep", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbSalesRep.Focus();
                return;
            }

            //if ((txtpaid.Text == null || (Convert.ToDouble(txtNetValue.Value) > Convert.ToDouble(txtpaidAmount.Text)))&&optCash.Checked==true)
            //{
            //    MessageBox.Show("Invoice Paid Amount Incorrect or Blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    txtpaidAmount.Focus();
            //    return;
            //}
            //if (txtpaid.Text == null)
            //{
            //    MessageBox.Show("Invoice Paid Amount Incorrect or Blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}

            //if (double.Parse(txtpaidAmount.Text.ToString()) == 0)
            //{
            //    MessageBox.Show("Please Enter Paid Amount", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}


            //if (cmbJobDone.Text.ToString() == "" || cmbJobDone.Text.ToString() == string.Empty)
            //{
            //    MessageBox.Show("Please Select Job done by", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    cmbJobDone.Focus();
            //    return;
            //}

            if (cmbCustomer.Text != "OTC")
            {
                //if (TxtPatientName.Text == string.Empty)
                //{
                //    MessageBox.Show("Please Enter Name", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    TxtPatientName.Focus();
                //    return;
                //}
                //if (txtPatientContactNo.Text == string.Empty)
                //{
                //    MessageBox.Show("Please Enter Contact No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    txtPatientContactNo.Focus();
                //    return;
                //}

                //if (txtVehicleNo.Text == string.Empty)
                //{
                //    MessageBox.Show("Please Enter Vehicle No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    txtVehicleNo.Focus();
                //    return;
                //}
            }


            double CreditCAm;
            double Netam;
            double PaidAmount;
            PaidAmount = 0;
            CreditCAm = 0;
            Netam = 0;
            if (txtpaid.Text != null)
            {
                PaidAmount = double.Parse(txtpaid.Text.ToString());
            }
            //if (PaidAmount < 0)
            //{
            //    MessageBox.Show("Paid Amount is Zero....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //if (rdobtnCreditCard.Checked == true)
            //{
            //    if (txtNetValue.Text != null)
            //    {
            //        CreditCAm = double.Parse(txtcardamount.Text.ToString());
            //    }
            //    Netam = double.Parse(txtNetValue.Text.ToString());
            //    if (CreditCAm != Netam)
            //    {
            //        DialogResult reply1 = MessageBox.Show("You Have a Advance Rs. " + double.Parse(txtcardamount.Text.ToString()) + " . You want to Proceed This..? ", "Information", MessageBoxButtons.OKCancel);
            //        if (reply1 != DialogResult.Cancel)
            //        {

            //        }
            //        else
            //        {
            //            MessageBox.Show("Invalid Payment....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            return;
            //        }

            //    }

            //}

            int intGrid;
            double dblAvailableQty;
            string StrReference = null;

            int intItemClass;

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;


            try
            {
                DeleteEmpGrid();
                //DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.OKCancel);

                //if (reply == DialogResult.Cancel)
                //{
                //    return;


                //}
                if (IsGridValidationItemIdandDis() == false)
                {

                    return;
                }

                if (GrideValidation == true)
                {
                    return;
                }
                CheckRowsValidation();

                if (PayValChecked == true)
                {
                    MessageBox.Show("Invalid Card Amount or Pay Type", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (IsGridValidation() == false)
                {
                    return;
                }
                if (HeaderValidation() == false)
                {
                    return;
                }

                if (optCash.Checked == true)
                {
                    StrPaymmetM = "Cash";

                }
                else if (optCredit.Checked == true)
                {
                    StrPaymmetM = "Credit";

                }
                else if (rdobtnCreditCard.Checked)
                {
                    StrPaymmetM = "CreditCard";
                }

                //if (!IsSerialNoCorrect())
                //    return;

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                //ALIGNMENT//WHEEL ALIGNMENT
                IsWheelAlignemntDone = "No";
                //====================
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Categoty"].Value.ToString() == "WHEEL ALIGNMENT")
                    {
                        IsWheelAlignemntDone = "Yes";
                    }
                }
                //==============================
                
                if(IsWheelAlignemntDone== "Yes")
                {
                   if(cmbJobDone.Text.ToString().Trim()==string.Empty)
                    {
                        MessageBox.Show("Please Select Job Doing Person");
                        return;
                    }

                }

                String WareHose = "Lasantha TYRE TRADERS";

                if (rbtVAT.Checked == false)
                {
                    for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                    {

                        if (ug.Rows[intGrid].Cells["WH"].Value.ToString() == "NEW Lasantha TYRE TRADERS")
                        {
                            WareHose = ug.Rows[intGrid].Cells["WH"].Value.ToString();
                        }
                    }
                }
                //==============================
                //if (txtPatientContactNo.Text.ToString().Trim() != "")
                //{
                //    if (IsWheelAlignemntDone == "Yes")
                //    {
                //        if (txtMileage.Text.ToString() != string.Empty)
                //        {
                //            double nextmileAge = 0;
                //            nextmileAge = double.Parse(txtMileage.Text.ToString()) + 3000;
                          
                //            var smsManager = new SmsManager();
                //            var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                //            var sms1 = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,\nWe would like to inform you that the first wheel alignment for your vehicle (" + txtVehicleNo.Text.ToString().Trim() + ") was completed on " + dtpDate.Value.ToString("dd/MM/yyyy") + ". \nYou are entitled to a free wheel alignment at " + nextmileAge.ToString().Trim() + "Km. \nFor any assistance, feel free to contact our Hotline at 0773131883. \nThank you for choosing Lasantha Tyre Traders. We look forward to serving you again!");
                //            await smsManager.SendSms(sms);
                //            await smsManager.SendSms(sms1);
                //        }
                //    }
                //    else
                //    {
                //        var smsManager = new SmsManager();
                //        var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                //        await smsManager.SendSms(sms);
                //    }
                //}
                //===============================

                if (user.IsSINVNoAutoGen)
                {
                    StrReference = GetInvNoField_CashOR_Credit(myConnection, myTrans, WareHose);
                    UpdatePrefixCashCreditNonew(myConnection, myTrans, WareHose);
                    txtInvoiceNo.Text = StrReference;
                    //StrReference = txtInvoiceNo.Text;
                    InvoiceNo = StrReference;

                    txtInvoiceNo.Text = StrReference;
                }
                //else
                //{
                //    SqlCommand myCommand = new SqlCommand("select * from tblSalesInvoices where InvoiceNo='" + txtInvoiceNo.Text.Trim() + "'", myConnection, myTrans);
                //    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                //    DataTable dt41 = new DataTable();
                //    da41.Fill(dt41);

                //    if (dt41.Rows.Count > 0)
                //    {
                //        MessageBox.Show("Invoice No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        myTrans.Rollback();
                //        myConnection.Close();//
                //        return;
                //    }
                //    else
                //    {
                //        StrReference = txtInvoiceNo.Text.ToString();
                //    }
                //}

                double Paid_Am;
                double AM_bal;
                Paid_Am = 0;
                AM_bal = 0;
                if (txtpaid.Text != null)
                {
                    Paid_Am = double.Parse(txtpaid.Text.ToString());
                }
                if (txtbalance.Text != null)
                {
                    AM_bal = double.Parse(txtbalance.Text.ToString());
                }
                if (txtpaid.Text != null)
                {
                    SaveInvBalance(StrReference, Netam, Paid_Am, AM_bal, StrPaymmetM, myConnection, myTrans);
                }
                string CRCardNo = "";
                string TransactionType = "INV";

                if (StrPaymmetM == "CreditCard")
                {
                    for (intGrid = 0; intGrid < dataGridView1.Rows.Count - 1; intGrid++)
                    {

                        if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value != null && double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()) > 0)
                        {
                            if (dataGridView1.Rows[intGrid].Cells["colcardno"].Value == null && dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString() != "CASH" && dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString() != "Advance")
                            {
                                MessageBox.Show("Please Enter Card No", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                myTrans.Rollback();
                                return;
                            }
                            if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString().ToUpper() == "CASH")
                            {
                                CRCardNo = "Cash";
                            }
                            else if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString().ToUpper() == "ADVANCE")
                            {
                                CRCardNo = "Advance";
                            }
                            else
                            {
                                CRCardNo = dataGridView1.Rows[intGrid].Cells["colcardno"].Value.ToString();
                            }
                            SaveCardDetailsWinlanka(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), TransactionType, cmbWarehouse.Value.ToString(), cmbCustomer.Value.ToString(), myConnection, myTrans);

                            //SaveCardDetails(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), myConnection, myTrans);
                        }

                    }
                }
                else
                {
                    SaveCardDetailsWinlanka(StrReference, StrPaymmetM, StrPaymmetM, double.Parse(txtNetValue.Value.ToString()), TransactionType, cmbWarehouse.Value.ToString(), cmbCustomer.Value.ToString(), myConnection, myTrans);
                    // SaveCardDetails(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), myConnection, myTrans);

                }



                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    Boolean Confirm = false;
                    SaveDetails(INVTYPE, ug.Rows[intGrid].Cells["Discount"].Value.ToString(),
                        double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()), StrReference, ug.Rows.Count, intGrid + 1,
                        StrPaymmetM, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(),
                        ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                        ug.Rows[intGrid].Cells["ItemClass"].Value.ToString(), ug.Rows[intGrid].Cells["GL"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(),
                        ug.Rows[intGrid].Cells["Categoty"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), IntInvoicetype,
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice(Incl)"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["LineTax"].Value.ToString()),
                        txtCustomer.Text.ToString(), txtCusPONum.Text.ToString(), txtMileage.Text.ToString(), cmbJobDone.Text.ToString(), Confirm, double.Parse(ug.Rows[intGrid].Cells["FOCQty"].Text.ToString()), ug.Rows[intGrid].Cells["WH"].Value.ToString(), WareHose, myConnection, myTrans);

                    double QTY = double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) + double.Parse(ug.Rows[intGrid].Cells["FOCQty"].Value.ToString());

                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());
                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {
                        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), myConnection, myTrans);

                        if (QTY > dblAvailableQty)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;
                        }

                        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), QTY, myConnection, myTrans);
                        InvTransaction(txtInvoiceNo.Text.Trim(), dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), QTY, double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
                    }
                }

                //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                //{
                //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                //        " TranType='Invoice',Status='OutOfStock' " +
                //        " where ItemID='" +
                //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                //    myCommandSe1.ExecuteNonQuery();
                //}

                //frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                //objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans,
                //    clsSerializeItem.DtsSerialNoList, "Invoice", cmbWarehouse.Text.ToString(),
                //    txtInvoiceNo.Text.ToString().Trim(), dtpDate.Value, true, "Invoiced");

                //if (optCash.Checked == true)
                //{
                //    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim());
                //    exporetReceipt_Cash(txtInvoiceNo.Text.Trim());
                //}
                //else if (rdobtnCreditCard.Checked == true)
                //{
                //    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim());
                //    exporetReceipt_Credit(txtInvoiceNo.Text.Trim());
                //}
                //else
                //{
                //    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim());
                //}
                myTrans.Commit();

                lblCompany.Text = WareHose;
                btnConfirm.Enabled = true;
                btnSave.Enabled = false;
                btnEditer.Enabled = true;

                //DirectNormalPrint=true;
                //Print(txtInvoiceNo.Text.Trim());

                POSPrint = false;
                DirectNormalPrint = false;




                EnableHeader(false);
                EnableFoter(false);
                cmbWarehouse.Enabled = false;
                ug.Enabled = false;



                DialogResult reply = MessageBox.Show("Are you sure, you want to Print this record ? ", "Information", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    if (txtPatientContactNo.Text.ToString().Trim() != "")
                    {
                        if (IsWheelAlignemntDone == "Yes")
                        {
                            if (txtMileage.Text.ToString() != string.Empty)
                            {
                                double nextmileAge = 0;
                                nextmileAge = double.Parse(txtMileage.Text.ToString()) + 3000;

                                var smsManager = new SmsManager();
                                var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                                var sms1 = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,\nWe would like to inform you that the first wheel alignment for your vehicle (" + txtVehicleNo.Text.ToString().Trim() + ") was completed on " + dtpDate.Value.ToString("dd/MM/yyyy") + ". \nYou are entitled to a free wheel alignment at " + nextmileAge.ToString().Trim() + "Km. \nFor any assistance, feel free to contact our Hotline at 0773131883. \nThank you for choosing Lasantha Tyre Traders. We look forward to serving you again!");
                                await smsManager.SendSms(sms);
                                await smsManager.SendSms(sms1);
                            }
                        }
                        else
                        {
                            var smsManager = new SmsManager();
                            var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                            await smsManager.SendSms(sms);
                        }
                    }

                    Print(txtInvoiceNo.Text);

                }

                //ButtonClear();
                //clearControls();
                // btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                throw ex;
            }

        }

        private void DELETEINSERTITEM(SqlConnection myConnection, SqlTransaction myTrans)
        {
            string S7 = " DELETE FROM tblInvoicePaymentHistory WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString() + "' ";
            SqlCommand cmd7 = new SqlCommand(S7, myConnection, myTrans);
            SqlDataAdapter da7 = new SqlDataAdapter(cmd7);
            DataTable dt7 = new DataTable();
            da7.Fill(dt7);
            string Sq = " DELETE FROM tblInvoicePayTypes WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString() + "' ";
            SqlCommand cmdq = new SqlCommand(Sq, myConnection, myTrans);
            SqlDataAdapter daq = new SqlDataAdapter(cmdq);
            DataTable dtq = new DataTable();
            daq.Fill(dtq);
            string So = " DELETE FROM tblSalesInvoices WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString() + "' ";
            SqlCommand cmdo = new SqlCommand(So, myConnection, myTrans);
            SqlDataAdapter dao = new SqlDataAdapter(cmdo);
            DataTable dto = new DataTable();
            dao.Fill(dto);
        }


        private void ConfirmEvent()
        {

            int IntInvoicetype = 1;

            Connector objConnector = new Connector();
            if (!(objConnector.IsOpenPeachtree(dtpDate.Value)))
                return;
            if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
            {
                IntInvoicetype = 1;
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 2)//inclusive
            {
                IntInvoicetype = 2;
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 3)//inclusive
            {
                IntInvoicetype = 3;
            }
            int INVTYPE = 1;
            if (rbtNoVat.Checked == true)
            {
                INVTYPE = 1;
            }
            if (rbtVAT.Checked == true)
            {
                INVTYPE = 2;
            }
            if (rbtSVAT.Checked == true)
            {
                INVTYPE = 3;
            }

            if (!user.IsSINVNoAutoGen)
            {
                if (txtInvoiceNo.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Enter Invoice No....!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }
            }
            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }
            if (cmbSalesRep.Value == null)
            {
                MessageBox.Show("Incorrect SalesRep", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            if ((txtpaid.Text == null || (Convert.ToDouble(txtNetValue.Value) > Convert.ToDouble(txtpaidAmount.Text)) || Convert.ToDouble(txtNetValue.Value) == 0) && optCash.Checked == true)
            {
                MessageBox.Show("Invoice Paid Amount Incorrect or Blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            //if (double.Parse(txtpaidAmount.Text.ToString()) == 0)
            //{
            //    MessageBox.Show("Please Enter Paid Amount", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //    return;

            //}


            //if (cmbJobDone.Text.ToString() == "" || cmbJobDone.Text.ToString() == string.Empty)
            //{
            //    MessageBox.Show("Please Select Job done by", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //    return;
            //}

            //if (cmbCustomer.Text != "OTC")
            //{
            //    if (TxtPatientName.Text == string.Empty)
            //    {
            //        MessageBox.Show("Please Enter Name", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //        return;
            //    }
            //    if (txtPatientContactNo.Text == string.Empty)
            //    {
            //        MessageBox.Show("Please Enter Contact No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //        return;
            //    }

            //    if (txtVehicleNo.Text == string.Empty)
            //    {
            //        MessageBox.Show("Please Enter Vehicle No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        return;
            //    }
            //}


            double CreditCAm;
            double Netam;
            double PaidAmount;
            PaidAmount = 0;
            CreditCAm = 0;
            Netam = 0;
            //if (txtpaid.Text != null)
            //{
            //    PaidAmount = double.Parse(txtpaid.Text.ToString());
            //}
            //if (PaidAmount < 0)
            //{
            //    MessageBox.Show("Paid Amount is Zero....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //if (rdobtnCreditCard.Checked == true)
            //{
            //    if (txtNetValue.Text != null)
            //    {
            //        CreditCAm = double.Parse(txtcardamount.Text.ToString());
            //    }
            //    Netam = double.Parse(txtNetValue.Text.ToString());
            //    if (CreditCAm != Netam)
            //    {
            //        DialogResult reply1 = MessageBox.Show("You Have a Advance Rs. " + double.Parse(txtcardamount.Text.ToString()) + " . You want to Proceed This..? ", "Information", MessageBoxButtons.OKCancel);
            //        if (reply1 != DialogResult.Cancel)
            //        {

            //        }
            //        else
            //        {
            //            MessageBox.Show("Invalid Payment....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            return;
            //        }

            //    }

            //}

            int intGrid;
            double dblAvailableQty;
            string StrReference = null;

            StrReference = txtInvoiceNo.Text;
            if (StrReference == null || StrReference == "")
            {
                StrReference = InvoiceNo;
            }
            int intItemClass;

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {


                DialogResult reply = MessageBox.Show("Are you sure, you want to Process this record ? ", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;

                }

                DeleteEmpGrid();
                //if (IsGridValidationItemIdandDis() == false)
                //{

                //    return;
                //}

                if (GrideValidation == true)
                {
                    return;
                }
                CheckRowsValidation();

                if (PayValChecked == true)
                {
                    MessageBox.Show("Invalid Card Amount or Pay Type", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (IsGridValidation() == false)
                {
                    return;
                }
                if (HeaderValidation() == false)
                {
                    return;
                }

                if (optCash.Checked == true)
                {
                    StrPaymmetM = "Cash";

                }
                else if (optCredit.Checked == true)
                {
                    StrPaymmetM = "Credit";

                }
                else if (rdobtnCreditCard.Checked)
                {
                    StrPaymmetM = "CreditCard";
                }

                //if (!IsSerialNoCorrect())
                //    return;

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                DELETEINSERTITEM(myConnection, myTrans);

                StrSql = "select ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster where WhseId='" + cmbWarehouse.Text.ToString() + "'";
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    StrARAccount = dt.Rows[0].ItemArray[0].ToString();
                    StrCashAccount = dt.Rows[0].ItemArray[1].ToString();
                    StrSalesGLAccount = dt.Rows[0].ItemArray[2].ToString();
                }

                double Paid_Am;
                double AM_bal;
                Paid_Am = 0;
                AM_bal = 0;
                if (txtpaid.Text != null)
                {
                    Paid_Am = double.Parse(txtpaid.Text.ToString());
                }
                if (txtbalance.Text != null)
                {
                    AM_bal = double.Parse(txtbalance.Text.ToString());
                }
                if (txtpaid.Text != null)
                {
                    SaveInvBalance(StrReference, Netam, Paid_Am, AM_bal, StrPaymmetM, myConnection, myTrans);
                }
                string CRCardNo = "";
                string TransactionType = "INV";

                if (StrPaymmetM == "CreditCard")
                {
                    for (intGrid = 0; intGrid < dataGridView1.Rows.Count - 1; intGrid++)
                    {

                        if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value != null && double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()) > 0)
                        {
                            if (dataGridView1.Rows[intGrid].Cells["colcardno"].Value == null && dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString() != "CASH" && dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString() != "Advance")
                            {
                                MessageBox.Show("Please Enter Card No", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                myTrans.Rollback();
                                return;
                            }
                            if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString().ToUpper() == "CASH")
                            {
                                CRCardNo = "Cash";
                            }
                            else if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString().ToUpper() == "ADVANCE")
                            {
                                CRCardNo = "Advance";
                            }
                            else
                            {
                                CRCardNo = dataGridView1.Rows[intGrid].Cells["colcardno"].Value.ToString();
                            }
                            SaveCardDetailsWinlanka(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), TransactionType, cmbWarehouse.Value.ToString(), cmbCustomer.Value.ToString(), myConnection, myTrans);

                            //SaveCardDetails(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), myConnection, myTrans);
                        }

                    }
                }
                else
                {
                    SaveCardDetailsWinlanka(StrReference, StrPaymmetM, StrPaymmetM, double.Parse(txtNetValue.Value.ToString()), TransactionType, cmbWarehouse.Value.ToString(), cmbCustomer.Value.ToString(), myConnection, myTrans);
                    // SaveCardDetails(StrReference, dataGridView1.Rows[intGrid].Cells["cblcname"].Value.ToString(), CRCardNo, double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()), myConnection, myTrans);

                }

                String WareHose = "Lasantha TYRE TRADERS";

                if (rbtVAT.Checked == false)
                {
                    for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                    {

                        if (ug.Rows[intGrid].Cells["WH"].Value.ToString() == "NEW Lasantha TYRE TRADERS")
                        {
                            WareHose = ug.Rows[intGrid].Cells["WH"].Value.ToString();
                        }
                    }
                }


                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    Boolean Confirm = true;
                    SaveDetails(INVTYPE, ug.Rows[intGrid].Cells["Discount"].Value.ToString(),
                        double.Parse(ug.Rows[intGrid].Cells["LineDisc"].Value.ToString()), StrReference, ug.Rows.Count, intGrid + 1,
                        StrPaymmetM, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(),
                        ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                        ug.Rows[intGrid].Cells["ItemClass"].Value.ToString(), ug.Rows[intGrid].Cells["GL"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(),
                        ug.Rows[intGrid].Cells["Categoty"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), IntInvoicetype,
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice(Incl)"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["LineTax"].Value.ToString()),
                        txtCustomer.Text.ToString(), txtCusPONum.Text.ToString(), txtMileage.Text.ToString(), cmbJobDone.Text.ToString(), Confirm, double.Parse(ug.Rows[intGrid].Cells["FOCQty"].Text.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), WareHose, myConnection, myTrans);

                    //intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());
                    //if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    //{
                    //    dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);
                    //    double QTY = double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) + double.Parse(ug.Rows[intGrid].Cells["FOCQty"].Value.ToString());
                    //    if (QTY > dblAvailableQty)
                    //    {
                    //        MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                    //        myTrans.Rollback();
                    //        return;
                    //    }
                    //    UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), QTY, myConnection, myTrans);
                    //    InvTransaction(txtInvoiceNo.Text.Trim(), dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), QTY, double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
                    //}
                }

                foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                {
                    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                        " TranType='Invoice',Status='OutOfStock' " +
                        " where ItemID='" +
                        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                    myCommandSe1.ExecuteNonQuery();
                }

                frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans,
                    clsSerializeItem.DtsSerialNoList, "Invoice", cmbWarehouse.Text.ToString(),
                    txtInvoiceNo.Text.ToString().Trim(), dtpDate.Value, true, "Invoiced");

                if (optCash.Checked == true)
                {
                    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim(), myConnection, myTrans);
                    exporetReceipt_Cash(txtInvoiceNo.Text.Trim());
                }
                else if (rdobtnCreditCard.Checked == true)
                {
                    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim(), myConnection, myTrans);
                    exporetReceipt_Credit(txtInvoiceNo.Text.Trim());
                }
                else
                {
                    ImportSalesInvoice(intGrid, txtInvoiceNo.Text.Trim(), myConnection, myTrans);
                }
                myTrans.Commit();
                MessageBox.Show("Invoice Successfuly Processed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnNew_Click(null, null);
            }

            catch (Exception ex)
            {
                myTrans.Rollback();
                throw ex;
            }
        }
        public bool IsSerialNoCorrect()
        {
            try
            {

                int _Count = 0;
                // Presuming the DataTable has a column named Date. 
                string expression;
                foreach (UltraGridRow dgvr in ug.Rows)
                {
                    if (dgvr.Cells["ItemCode"].Value != null)
                    {
                        if (IsThisItemSerial(dgvr.Cells["ItemCode"].Value.ToString().Trim()) && double.Parse(dgvr.Cells["Quantity"].Value.ToString()) > 0)
                        {
                            if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemCode"].Value.ToString().Trim(),
                                    "Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            _Count = 0;
                            expression = "ItemCode = '" + dgvr.Cells["ItemCode"].Value.ToString().Trim() + "'";
                            DataRow[] foundRows;

                            // Use the Select method to find all rows matching the filter.
                            foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                            // Print column 0 of each returned row. 
                            for (int i = 0; i < foundRows.Length; i++)
                            {
                                _Count = i + 1;
                            }

                            if (_Count > 0 && double.Parse(dgvr.Cells["Quantity"].Value.ToString()) == 0)
                            {
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                                }
                            }

                            if (_Count != double.Parse(dgvr.Cells["Quantity"].Value.ToString()))
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemCode"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        public void CheckRowsValidation()
        {
            try
            {
                PayValChecked = false;
                int intGrid = 0;
                for (intGrid = 0; intGrid < dataGridView1.Rows.Count - 1; intGrid++)
                {
                    string CRCardNo = "";
                    if (dataGridView1.Rows[intGrid].Cells["cblcname"].Value == null || dataGridView1.Rows[intGrid].Cells["Amount"].Value == null || double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()) < 0)
                    {
                        PayValChecked = true;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //above code segment import invoice to Pechtree

        private void btnSave_Click(object sender, EventArgs e)
        {

            GrideValidation = false;
            try
            {


                if (dtpDate.Value < user.Period_begin_Date)
                {
                    MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (dtpDate.Value > user.Period_End_Date)
                {
                    MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }



                //if (txtVehicleNo.Text == "")IsEdit
                //{
                //    MessageBox.Show("Please Enter Vehicle No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}
                if (IsEdit == false)
                {
                    SaveEvents();
                    //if (SaveComplete == true)
                    //{
                    //    btnSave.Enabled = false;

                    //    DialogResult reply = MessageBox.Show("Are you sure, you want to Process this record ? ", "Information", MessageBoxButtons.OKCancel);

                    //    if (reply == DialogResult.Cancel)
                    //    {
                    //        ButtonClear();
                    //        clearControls();
                    //        btnNew_Click(sender, e);
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        ConfirmEvent();

                    //    }
                    //  }
                    //if(ConfirmComplete ==true)
                    //{
                    //DialogResult reply = MessageBox.Show("Are you sure, you want to Print this record ? ", "Information", MessageBoxButtons.OKCancel);

                    //if (reply == DialogResult.Cancel)
                    //{
                    //    ButtonClear();
                    //    clearControls();
                    //    btnNew_Click(sender, e);
                    //    return;
                    //}
                    //else
                    //{
                    //    Print(txtInvoiceNo.Text.Trim());
                    //}

                    //}
                    //if(PrintComplete)
                    //{
                    //ButtonClear();
                    //    clearControls();
                    //    btnNew_Click(sender, e);
                    //    return;
                    //}

                }
                else
                {
                    UpdateEvent();
                    //if (Update == true)
                    //{
                    //    ButtonClear();
                    //    clearControls();
                    //    btnNew_Click(sender, e);
                    //}
                }

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }

            //SaveComplete = false;
            //ConfirmComplete = false;
            //PrintComplete = false;
        }

        private double CheckWarehouseItem(string StrItemCode, string StrWarehouseCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT QTY FROM tblItemWhse where WhseId = '" + StrWarehouseCode + "' and ItemId='" + StrItemCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return double.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
                throw ex;
            }
        }

        public static string GetDateTime(DateTime DtGetDate)
        {
            DateTime DTP = Convert.ToDateTime(DtGetDate);
            string Dformat = "MM/dd/yyyy";
            return DTP.ToString(Dformat);
        }

        private void DeleteDetails(int intEstomateProcode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "DELETE FROM [EST_DETAILS] WHERE AutoIndex=" + intEstomateProcode + "";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteFromItemWisetbl(string INVNo, string StrWarehouse, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                string s = "";
                StrSql = "SELECT ItemID,Qty,WarehouseID FROM tbItemlActivity WHERE TranNo='" + INVNo + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    s = " UPDATE tblItemWhse SET QTY=QTY+'" + Convert.ToDouble(dt.Rows[i].ItemArray[1].ToString()) + "'  where WhseId='" + dt.Rows[i].ItemArray[2].ToString() + "' and ItemId='" + dt.Rows[i].ItemArray[0].ToString() + "'";
                    SqlCommand command2 = new SqlCommand(s, con, Trans);
                    command2.CommandType = CommandType.Text;
                    command2.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateItemWarehouse(string StrItemCode, string StrWarehouse, double dblQty, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblItemWhse SET QTY=QTY-" + dblQty + " WHERE WhseId ='" + StrWarehouse + "' and ItemId='" + StrItemCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateEstimateVsActualHeader(int intAutoIndex, double dblActualAmount, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE EST_HEADER SET ActHed_NetAmt=ActHed_NetAmt +  " + dblActualAmount + " WHERE AutoIndex=" + intAutoIndex + " ";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int GetLastTransactionNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT TOP 1 [AutoIndex] FROM EST_HEADER ORDER BY [AutoIndex] DESC";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteFromActivity(string strInvoiceNo, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "DELETE [tbItemlActivity] WHERE [TranNo]='" + strInvoiceNo + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InvTransaction(string strInvoiceNo, DateTime dtDate, String StrItemCode, string StrLocCode, double dblQuantity, double dblPrice, double dblLineNetAmt, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                if (dblQuantity != 0)
                {
                    StrSql =
                         "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse  where WhseId ='" + StrLocCode + "' and ItemId='" + StrItemCode + "') " +
                        " INSERT INTO [tbItemlActivity](OHQTY,[DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES( @OHQTY,4,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Invoice','false','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "','" + dblPrice + "')";
                    SqlCommand command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteInvoice(string StrInvoiceNo, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = " declare @ItemCode varchar(20),@Qty numeric(18,2),@WHID varchar(20) " +
                    " DECLARE db_cursorA CURSOR FOR  SELECT    tblSalesInvoices.ItemID, tblSalesInvoices.Qty,WHID " +
                    " FROM         tblSalesInvoices LEFT OUTER JOIN " +
                    " tblItemWhse ON tblSalesInvoices.ItemID = tblItemWhse.ItemId AND tblSalesInvoices.WHID = tblItemWhse.WhseId " +
                    " where tblSalesInvoices.InvoiceNo ='" + StrInvoiceNo + "' " +
                    " OPEN db_cursorA FETCH NEXT FROM db_cursorA INTO @ItemCode,@Qty,@WHID " +
                    " WHILE @@FETCH_STATUS = 0   " +
                    " BEGIN  " +
                    " UPDATE tblItemWhse SET QTY=QTY+@Qty WHERE WhseId=@WHID AND ItemId=@ItemCode	" +
                    " FETCH NEXT FROM db_cursorA INTO @ItemCode,@Qty,@WHID " +
                    " END   " +
                    " CLOSE db_cursorA   " +
                    " DEALLOCATE db_cursorA " +
                    " delete from tblSalesInvoices where InvoiceNo='" + StrInvoiceNo + "'" +
                    " delete from dbo.tbItemlActivity where TranNo='" + StrInvoiceNo + "' ";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveDetails(int InvType, string LineDiscPerc, double LineDiscAmount, string StrInvoiceNo, int intLineCount, int intLineNo, String StrPayMethod, int intLineId, String StrItemCode, string StrItemDescription, double dblQuantity, double dblPrice, double dblLineNetAmt, string StrItemClass, string StrGLAccount, string StrUOM, string StrItemType, double dblCostPrice, Int64 IntInvoicetype, double dblInclusivePrice, double dblLineTax, string cuname, string CusPoNum, string Mileage, string Jobdoneby, Boolean Confirm, double FOCQty, string WH, string DefaultWH, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO tblSalesInvoices(IsDirect,InvType,LineDiscountAmount,LineDiscountPercentage,IsInclusive,InvoiceNo,WHID,CustomerID,DeliveryNoteNos,InvoiceDate,PaymentM, " +
                         " SalesRep,ARAccount,NoofDistributions,DistributionNo, " +
                         " ItemID,Description,Qty,UnitPrice,Amount,GLAcount,UOM,TotalDiscountPercen,TotalDiscountAmount,GrossTotal,NetTotal, " +
                         " CurrentUser,CurrentDate,Time,ItemClass,ItemType,IsVoid,VoidReson,VoidUser,CostPrrice,RemainQty,CustomerPO,SONO,IsExport,Location,IsReturn,Tax1Rate,Tax2Rate,Tax1Amount,Tax2Amount,TTTYPE1,TTTYPE2,JobID,SubValue,Comments,ServiceCharge,InclusivePrice,LineTax,CusName,CustomerName,ContactNo,PaidAmount,CusPoNum,Mileage,JobDoneBy,IsConfirm,VehicleNo,FOCQty,WarrentyKm,WarrentyYear,Remarks) " +
                         " VALUES ('True','" + InvType + "','" + LineDiscAmount + "','" + LineDiscPerc + "','" + IntInvoicetype + "','" + StrInvoiceNo + "','" + DefaultWH + "','" + cmbCustomer.Value.ToString() + "','','" + GetDateTime(dtpDate.Value) + "','" + StrPayMethod + "', " +
                         " '" + cmbSalesRep.Value.ToString() + "','" + StrARAccount + "','" + intLineCount.ToString() + "','" + intLineNo.ToString() + "' " +
                         " ,'" + StrItemCode + "','" + StrItemDescription + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + "," +
                         " '" + StrGLAccount + "','" + StrUOM + "'," + double.Parse(txtDiscPer.Value.ToString()) + "," + double.Parse(txtDiscAmount.Value.ToString()) + "," +
                         " " + double.Parse(txtGrossValue.Value.ToString()) + "," + double.Parse(txtNetValue.Value.ToString()) + "," +
                         " '" + user.userName.ToString().Trim() + "','" + GetDateTime(DateTime.Now) + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "'," +
                         " '" + StrItemClass + "','" + StrItemType + "',0,'','" + user.userName.ToString().Trim() + "'," + dblCostPrice + "," + dblQuantity + ",'','',0,'" + WH + "',0," + txtNBTPer.Value.ToString() + "," + txtVatPer.Value.ToString() + "," + txtNBT.Value.ToString() + "," + txtVat.Value.ToString() + ",'NBT','VAT','" + txtTyreSize.Text.ToString().Trim() + "'," + txtSubValue.Value.ToString() + ",'" + txtDescription.Text.Trim() + "','" + Convert.ToDouble(txtServicecharge.Value.ToString()) + "','" + dblInclusivePrice + "','" + dblLineTax + "','" + cuname + "','" + TxtPatientName.Text.ToString().Trim() + "','" + txtPatientContactNo.Text.ToString().Trim() + "','" + double.Parse(txtpaidAmount.Text.ToString()) + "','" + CusPoNum + "','" + Mileage + "','" + Jobdoneby + "','" + Confirm + "','" + txtVehicleNo.Text.ToString().Trim() + "','" + FOCQty + "','" + txtWarrentyKm.Text.ToString().Trim() + "','" + txtWarrentyYear.Text.ToString().Trim() + "','" + txtRemarks.Text.ToString().Trim() + "')";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            btnSearch.Enabled = false;
            try
            {


                if (frmMain.objfrmInvoiceSearch == null || frmMain.objfrmInvoiceSearch.IsDisposed)
                {
                    frmMain.objfrmInvoiceSearch = new frmInvoiceSearch(1);
                }

                frmMain.ObjInvoices.TopMost = false;
                frmMain.objfrmInvoiceSearch.Show();
                //this.Close();
                //frmMain.objfrmInvoiceSearch.TopMost = true;

                if (Search.searchIssueNoteNo != "0" && Search.searchIssueNoteNo != "")
                {
                    txtInvoiceNo.Text = Search.searchIssueNoteNo;

                }

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
            btnSearch.Enabled = true;
        }

        private void DataSetHeader(string StrInvoiceNo)
        {
            try
            {
                StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + StrInvoiceNo + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsEst.DtEstimateHeader);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void DataSetDetails(int intEstimateNo)
        {
            try
            {
                StrSql = "SELECT * FROM EST_DETAILS WHERE AutoIndex=" + intEstimateNo + "";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsEst.DtEstimateDETAILS);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void Print(string strInvoiceNo)
        {

            //DialogResult reply = MessageBox.Show("Are you sure, you want to Print this record ? ", "Information", MessageBoxButtons.OKCancel);
            //if (reply == DialogResult.Cancel)
            //{
            //    return;
            //}
            //else
            //{

                if (rbtNoVat.Checked == true)
                {
                    InvoiceType = 1;
                }
                else if (rbtVAT.Checked == true)
                {
                    InvoiceType = 2;
                }
                else if (rbtVAT.Checked == true)
                {
                    InvoiceType = 3;
                }

                try
                {


                    //DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

                    //if (reply == DialogResult.Cancel)
                    //{
                    //    //New();
                    //    return;
                    //}


                    if (strInvoiceNo != "")
                    {
                        //DsItemWise.Clear();

                        //StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + strInvoiceNo + "'";
                        //SqlCommand cmd = new SqlCommand(StrSql);
                        //SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                        //DataTable dt = new DataTable();

                        //da.Fill(DsItemWise.dtSalesInvoice);

                        //StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
                        //cmd = new SqlCommand(StrSql);
                        //da = new SqlDataAdapter(StrSql, ConnectionString);
                        //dt = new DataTable();
                        //da.Fill(DsItemWise.DsItem);

                        //StrSql = "SELECT CutomerID,CustomerName,Address1,Address2 FROM tblCustomerMaster";
                        //cmd = new SqlCommand(StrSql);
                        //da = new SqlDataAdapter(StrSql, ConnectionString);
                        //dt = new DataTable();
                        //da.Fill(DsItemWise.DsCustomer);


                        //StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
                        //cmd = new SqlCommand(StrSql);
                        //da = new SqlDataAdapter(StrSql, ConnectionString);
                        //dt = new DataTable();
                        //da.Fill(DsItemWise.DsWarehouse);
                        DsItemWise.Clear();
                        StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(StrSql);
                        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(DsItemWise.dtSalesInvoice);

                        StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.DsItem);

                        StrSql = "SELECT CutomerID,CustomerName,Address1,Address2,Custom1,Custom2,Custom3,Custom4,Fax,Phone1 FROM tblCustomerMaster where CutomerID='" + cmbCustomer.Text.ToString().Trim() + "'";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.DsCustomer);

                        StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.DsWarehouse);

                        StrSql = "SELECT * FROM tblInvoicePaymentHistory WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.tblInvoicePaymentHistory);

                        StrSql = "SELECT * FROM tblInvoicePayTypes WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.tblpaytran);

                        StrSql = "SELECT * FROM tblSalesRep ";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.tblSalesRep);

                        StrSql = "SELECT * FROM tblCompanyInformation ";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.DtCompaniInfo);

                        if (DirectNormalPrint == false)
                        {
                            InDirectPrint();
                        }
                        else
                        {

                            DirectPrint();
                        }

                    }

                    //if (txtPatientContactNo.Text.ToString().Trim() != "")
                    //{
                    //    if (IsWheelAlignemntDone == "Yes")
                    //    {
                    //        if (txtMileage.Text.ToString() != string.Empty)
                    //        {
                    //            double nextmileAge = 0;
                    //            nextmileAge = double.Parse(txtMileage.Text.ToString()) + 3000;
                    //            //  if(double.Parse(txtMileage.Text.ToString())
                    //            var smsManager = new SmsManager();
                    //            var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");

                    //            //Thank you for your recent purchase. We are honored to gain you as a customer and hope to serve you for a long time.
                    //            //  var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear " + TxtPatientName.Text.ToString().Trim() + ", Thank you for your recent purchase. We are honored to gain you as a customer and hope to serve you for a long time. - Team Lasantha Tyre Traders");
                    //            var sms1 = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,\nWe would like to inform you that the first wheel alignment for your vehicle (" + txtVehicleNo.Text.ToString().Trim() + ") was completed on " + dtpDate.Value.ToString("dd/MM/yyyy") + ". \nYou are entitled to a free wheel alignment at " + nextmileAge.ToString().Trim() + "Km. \nFor any assistance, feel free to contact our Hotline at 0773131883. \nThank you for choosing Lasantha Tyre Traders. We look forward to serving you again!");
                    //            // var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear " + TxtPatientName.Text.ToString().Trim() + ", Your Bill No is " + txtInvoiceNo.Text.ToString().Trim() + "  & Amount is '" + txtNetValue.Value.ToString().Trim() + "  Thank you & Come Again - Team Lasantha Tyre Traders");
                    //            await smsManager.SendSms(sms);
                    //            await smsManager.SendSms(sms1);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var smsManager = new SmsManager();
                    //        //  var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), SmsMassage);
                    //        //  var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Customer ,\n chamila");


                    //        var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Sir/Madam,Thank you for being a valued customer of Lasantha Tyre Traders!\nPlease rate us on Google (https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9) or Facebook (https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL).\nFor any tyre services, call us at 0112773232 or our hotline at 0773131883. \nWe are open every day from 6:30 AM to 9:00 PM (wheel alignment services: 7:30 AM to 6:00 PM). \n Go smart with Lasantha Tyre Traders!");
                    //        //var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear Customer,\n Thank you for beeng valued customer for Lasantha Tyre Traders.\n" +
                    //        //                                                                          "Pls rate us on \n" +
                    //        //                                                                          "Google - https://maps.app.goo.gl/FAgQGmmMWL4jDwHF9 \n " +
                    //        //                                                                          "Facebook - https://www.facebook.com/LasanthaTyres?mibextid=ZbWKwL \n" +
                    //        //                                                                          "call us for any tyre service \n" +
                    //        //                                                                          "0112773232 \n" +
                    //        //                                                                          "24 / 7 Hotline 0777078700 \n" +
                    //        //                                                                          "We are open 365 Days from 6:30 to 21:00 For your service \n" +
                    //        //                                                                          "(Wheel alignment only on 07:30 to 18:00) \n" +
                    //        //                                                                          "Go smart with Lasantha Tyre Traders \n");

                    //        // var sms = new SendSmsDto(txtPatientContactNo.Text.ToString().Trim(), "Dear " + TxtPatientName.Text.ToString().Trim() + ", Your Bill No is " + txtInvoiceNo.Text.ToString().Trim() + "  & Amount is '" + txtNetValue.Value.ToString().Trim() + "  Thank you & Come Again - Team Lasantha Tyre Traders");
                    //        await smsManager.SendSms(sms);
                    //    }
                    //}

                }
                catch (Exception ex)
                {
                    throw ex;
                }

          //  }
        }

        private void DirectPrint()
        {
            try
            {
                //if (rbtVAT.Checked == true)
                //{
                //tax invoice
                //Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoice.rpt";
                string Myfullpath = string.Empty;
                if (POSPrint == true)
                {
                    if (chbWithDis.Checked == true)
                    {

                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRPOSInvoicePRINTWithDis.rpt";
                        ReportDocument crp = new ReportDocument();
                        crp.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
                        System.Drawing.Printing.PrinterSettings printersettings = new System.Drawing.Printing.PrinterSettings();
                        printersettings.DefaultPageSettings.Landscape = false;
                        crp.Load(Myfullpath);
                        crp.SetDataSource(DsItemWise);
                        crp.PrintToPrinter(1, true, 0, 0);
                    }
                    else
                    {
                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRPOSInvoicePRINT.rpt";
                        ReportDocument crp = new ReportDocument();
                        crp.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
                        System.Drawing.Printing.PrinterSettings printersettings = new System.Drawing.Printing.PrinterSettings();
                        printersettings.DefaultPageSettings.Landscape = false;
                        crp.Load(Myfullpath);
                        crp.SetDataSource(DsItemWise);
                        crp.PrintToPrinter(1, true, 0, 0);
                    }

                    POSPrint = false;
                }
                else
                {
                    if (chbWithDis.Checked == true)
                    {

                        if (rbtNoVat.Checked == true)
                        {
                            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceWithDiscountNonVat.rpt";
                        }
                        else
                        {
                            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceWithDiscount.rpt";
                        }
                        ReportDocument crp = new ReportDocument();
                        crp.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
                        System.Drawing.Printing.PrinterSettings printersettings = new System.Drawing.Printing.PrinterSettings();
                        printersettings.DefaultPageSettings.Landscape = false;
                        crp.Load(Myfullpath);
                        crp.SetDataSource(DsItemWise);
                        crp.PrintToPrinter(1, true, 0, 0);
                    }
                    else
                    {
                        if (rbtNoVat.Checked == true)
                        {
                            if (optCredit.Checked == true)
                            {
                                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceCreditNonVat.rpt";
                            }
                            else
                            {
                                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceNonVat.rpt";
                            }
                        }
                        else
                        {


                            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoice.rpt";

                        }

                        ReportDocument crp = new ReportDocument();
                        crp.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
                        System.Drawing.Printing.PrinterSettings printersettings = new System.Drawing.Printing.PrinterSettings();
                        printersettings.DefaultPageSettings.Landscape = false;
                        crp.Load(Myfullpath);
                        crp.SetDataSource(DsItemWise);
                        crp.PrintToPrinter(1, true, 0, 0);
                    }
                }

                //if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoice.rpt") == true)
                //{
                //    Myfullpath = Path.GetFullPath("CRInvoice.rpt");
                //}
                //else
                //{
                //    MessageBox.Show("CRTaxInvoice.rpt not Exists.");
                //    this.Close();
                //    return;
                //}


                //}
                //if (rbtSVAT.Checked == true)
                //{

                //    //SVAT invoice
                //    string Myfullpath;
                //    ReportDocument crp = new ReportDocument();
                //    if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRSVATInvoice.rpt") == true)
                //    {
                //        Myfullpath = Path.GetFullPath("CRSVATInvoice.rpt");
                //    }
                //    else
                //    {
                //        MessageBox.Show("CRSVATInvoice.rpt not Exists.");
                //        this.Close();
                //        return;
                //    }

                //crp.Load(Myfullpath);
                //crp.SetDataSource(DsItemWise);
                //crp.PrintToPrinter(1, true, 0, 0);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool WithDisPrint = false;
        public bool Credit = false;
        public bool Card = false;
        private void InDirectPrint()
        {
            if (optCredit.Checked == true)
            {
                Credit = true;
            }
            else
            {
                Credit = false;
            }

            if (rdobtnCreditCard.Checked == true)
            {
                Card = true;
            }
            else
            {
                Card = false;
            }

            if (chbWithDis.Checked == true)
            {
                WithDisPrint = true;
            }
            else
            {
                WithDisPrint = false;
            }
            try
            {

                bool pdf = PDFViewer;
                if (rbtNoVat.Checked)
                {
                    InvoiceType = 1;
                    frmPrintTaxInvoice printax = new frmPrintTaxInvoice(this, pdf);
                    printax.ShowDialog();
                    if (Close == true)
                    {
                        btnNew_Click(null, null);
                    }
                }
                if (rbtVAT.Checked)
                {
                    if (Convert.ToInt64(cmbInvoiceType.Value) == 1)
                        TaxINCType = 1;
                    else if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                        TaxINCType = 2;

                    InvoiceType = 2;
                    frmPrintTaxInvoice printax = new frmPrintTaxInvoice(this, pdf);
                    printax.ShowDialog();
                    if (Close == true)
                    {
                        btnNew_Click(null, null);
                    }
                }
                if (rbtSVAT.Checked)
                {
                    InvoiceType = 2;
                    frmPrintTaxInvoice printax = new frmPrintTaxInvoice(this, pdf);
                    printax.ShowDialog();
                    if (Close == true)
                    {
                        btnNew_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DirectNormalPrint = false;
        private void btnPrint_Click(object sender, EventArgs e)
        {
            DirectNormalPrint = true;
            POSPrint = false;
            try
            {

                IsWheelAlignemntDone = "No";
                //====================
                for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Categoty"].Value.ToString() == "WHEEL ALIGNMENT")
                    {
                        IsWheelAlignemntDone = "Yes";
                    }
                }
                //==============================

                Print(txtInvoiceNo.Text);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ClearFooter()
        {
            txtNBTPer.Value = 0;
            txtVatPer.Value = 0;
            txtDiscPer.Value = 0;
            txtSubValue.Value = 0;
            txtDiscAmount.Value = 0;
            txtGrossValue.Value = 0;
            txtNBT.Value = 0;
            txtVat.Value = 0;
            txtNetValue.Value = 0;

        }

        private void ClearHeader()
        {
            try
            {
                // chkQtyWithZero.Checked = false;
                chbWithDis.Checked = false;
                cmbJobDone.Text = string.Empty;
                txtMileage.Text = string.Empty;
                txtCusPONum.Text = string.Empty;
                cmbWarehouse.Value = user.StrDefaultWH;
                txtDescription.Text = "";
                //  dtpDate.Value = System.DateTime.Now;
                GetCurrentUserDate();
                cmbCustomer.Text = "";
                txtCustomer.Text = "";
                cmbSalesRep.Text = "";
                txtWarehouseAddress.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtNBTPer.Value = 0;
                txtVatPer.Value = 0;
                txtDiscPer.Value = 0;
                txtSubValue.Value = 0;
                txtDiscAmount.Value = 0;
                txtGrossValue.Value = 0;
                txtNBT.Value = 0;
                txtVat.Value = 0;
                txtNetValue.Value = 0;
                txtpaidAmount.Text = "0.00";
                rbtNoVat.Checked = true;
                txtcardamount.Text = "0.00";
                txtcardamount.Text = "0.00";
                txtServicecharge.Value = "0.00";
                txtpaid.Text = "0.00";
                txtbalance.Text = "0.00";
                txtBalanceAmt.Text = "0.00";
                optCredit.Checked = true;
                optSerialTwo.Checked = true;

                txtPatientContactNo.Text = "";
                TxtPatientName.Text = "";
                txtRemarks.Text = "";
                txtWarrentyKm.Text = "";
                txtWarrentyYear.Text = "";


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnableFoter(Boolean blnEnable)
        {
            //txtVatPer.Enabled = blnEnable;
            //txtNBTPer.Enabled = blnEnable;
            //txtDescription.Enabled = blnEnable;
            //txtDiscPer.Enabled = blnEnable;
            //txtServicecharge.Enabled = blnEnable;
            //ug.Enabled = blnEnable;


        }

        private void SetReadOnly(Boolean blnEnable)
        {


            txtVehicleNo.ReadOnly = blnEnable;
            txtCusPONum.ReadOnly = blnEnable;
            txtMileage.ReadOnly = blnEnable;
            TxtPatientName.ReadOnly = blnEnable;
            txtPatientContactNo.ReadOnly = blnEnable;
            txtDescription.ReadOnly = blnEnable;

            txtServicecharge.ReadOnly = blnEnable;
            txtDiscPer.ReadOnly = blnEnable;
            txtNBTPer.ReadOnly = blnEnable;
            txtVatPer.ReadOnly = blnEnable;
            txtpaidAmount.ReadOnly = blnEnable;

        }

        private void EnableHeader(Boolean blnEnable)
        {
            cmbCustomer.Enabled = blnEnable;
            cmbJobDone.Enabled = blnEnable;
            cmbWarehouse.Enabled = blnEnable;
            grpPayment.Enabled = blnEnable;
            groupBox1.Enabled = blnEnable;
            dtpDate.Enabled = blnEnable;
        }

        private void ButtonClear()
        {
            btnSave.Enabled = false;
            btnNew.Enabled = true;
            btnPrint.Enabled = true;
            btnSearch.Enabled = true;
            btnReset.Enabled = true;
            btnEditer.Enabled = false;

            ClearHeader();
            EnableHeader(false);
            EnableFoter(false);
            DeleteRows();
            //GetInvNo();
            ug.Enabled = false;
            intEstomateProcode = 0;
            // optCash.Checked = true;
            optCredit.Checked = true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnNew_Click(sender, e);
            ButtonClear();

        }

        private void DatasetWhse()
        {
            try
            {
                StrSql = "SELECT * FROM tblWhseMaster";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsEst.DtWhseMaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double ChangePrice(string ItemCode, double dblGridQty)
        {
            try
            {
                double _Price = 0.00;
                string sSQL = " select isnull((select isnull((SELECT (viewItemPriceSelect.Unitprice) as Unitprice " +
                    " FROM tblPriceMatrix INNER JOIN viewItemPriceSelect ON " +
                    " tblPriceMatrix.ItemID = viewItemPriceSelect.ItemID AND tblPriceMatrix.PriceLevel = viewItemPriceSelect.PriceLevel " +
                    " WHERE     (tblPriceMatrix.MinQty < " + dblGridQty + ") AND (tblPriceMatrix.MaxQty >= " + dblGridQty + ") AND (tblPriceMatrix.ItemID = '" + ItemCode + "')), " +
                    " (SELECT     viewItemPriceSelect.Unitprice " +
                    " FROM         viewItemPriceSelect INNER JOIN " +
                    " viewItemDefault ON viewItemPriceSelect.PriceLevel = viewItemDefault.PriceLevel AND " +
                    " viewItemPriceSelect.ItemID = viewItemDefault.ItemID " +
                    " WHERE (viewItemDefault.ItemID = '" + ItemCode + "'))) as UnitPrice),0) as UnitPrice";

                SqlCommand cmd = new SqlCommand(sSQL);
                SqlDataAdapter da = new SqlDataAdapter(sSQL, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null)
                {
                    _Price = double.Parse(dt.Rows[0]["UnitPrice"].ToString());
                }

                return _Price;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private double ChangePrice2(double dblPriceList2)
        {
            return dblPriceList2;
        }

        private void txtDiscPer_Leave(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(txtDiscPer.Value.ToString()) > 100)
                {
                    MessageBox.Show("Invalid Discount Percentage", "Information", MessageBoxButtons.OK);
                    txtDiscPer.Focus();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscPer_ValueChanged(object sender, EventArgs e)
        {
            //try
            //{               
            //    FooterCalculation();                
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
            try
            {
                decimal amount = 0;
                //   InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                CalPaidAmt();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtNBTPer_ValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    FooterCalculation();
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
            try
            {

                txtVat.Value = 0;

                FooterCalculation();
                CalPaidAmt();


            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtVatPer_ValueChanged(object sender, EventArgs e)
        {
            //try
            //{              
            //    FooterCalculation();
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
            try
            {


                decimal amount = 0;
                //  InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                CalPaidAmt();




            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void Edit()
        {
            try
            {
                EnableHeader(false);
                EnableFoter(false);
                dtpDate.Enabled = true;
                txtDescription.Enabled = true;
                txtDiscPer.Enabled = true;
                txtNBTPer.Enabled = true;
                txtServicecharge.Enabled = true;
                txtVatPer.Enabled = true;
                btnReset.Enabled = true;
                btnSave.Enabled = true;
                ug.Enabled = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Boolean IsGridExitCode(String StrCode)
        {
            try
            {
                foreach (UltraGridRow ugR in ultraCombo1.Rows)
                {
                    if (ugR.Cells["ItemID"].Text == StrCode)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cmbWarehouse_Leave(object sender, EventArgs e)
        {
            try
            {
                string Code;
                GetItemDataSet();
                if (ug.Rows.Count > 0)
                {
                    DialogResult reply = MessageBox.Show("Are you sure, you want to channge Warehouse?", "Information", MessageBoxButtons.OK);

                    if (reply == DialogResult.OK)
                    {
                        DeleteRows();
                        ClearFooter();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }






        private void CalculateTaxAmounts()
        {
            double _subTot = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            double Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
            double Tax2Rate = double.Parse(txtVatPer.Text.Trim());

            if (txtDiscAmount.Text.Trim() == string.Empty || txtDiscAmount.Text == null) txtDiscAmount.Text = "0.00";

            _subTot = double.Parse(txtGridTotalExcl.Text.Trim()) + double.Parse(txtServicecharge.Text.Trim());

            _subTot = _subTot - double.Parse(txtDiscAmount.Text.Trim());// - double.Parse(txtDiscLineTot.Text.Trim());

            _Tax1 = _subTot * Tax1Rate / 100;
            _Tax2 = (_Tax1 + _subTot) * Tax2Rate / 100;

            if (rbtNoVat.Checked)
            {
                txtNBT.Text = "0.00";
                txtVat.Text = "0.00";
            }
            else
            {
                txtNBT.Text = _Tax1.ToString("0.00");
                txtVat.Text = _Tax2.ToString("0.00");
            }



            txtNetValue.Text = _subTot.ToString("0.00");

            txtpaid.Text = _subTot.ToString("0.00");


        }

        private double Temp_getTaxRate()
        {
            double _TempRate = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            try
            {
                double Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
                double Tax2Rate = double.Parse(txtVatPer.Text.Trim());

                _Tax1 = Math.Round(Tax1Rate, 2);
                _Tax2 = Math.Round((Tax1Rate + 100) * Tax2Rate / 100, 2);

                _TempRate = Math.Round(_Tax2 + _Tax1, 2);

                return _TempRate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cmbCustomer_Leave(object sender, EventArgs e)
        {
            try
            {
                if (cmbCustomer.Value == null) cmbCustomer.Value = "";
                if (StrRetailCustomer == cmbCustomer.Text.ToString())
                {
                    blnRetailsCustomer = true;
                }
                else
                {
                    blnRetailsCustomer = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbCustomer_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtCustomer.Text = cmbCustomer.ActiveRow.Cells[1].Value.ToString();
                        txtAddress1.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString();
                        txtAddress2.Text = cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                        dblCusPriceLevel = int.Parse(cmbCustomer.ActiveRow.Cells[4].Value.ToString());
                        txtDescription.Text = cmbCustomer.ActiveRow.Cells[5].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbWarehouse_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtWarehouseName.Text = cmbWarehouse.ActiveRow.Cells[1].Value.ToString();

                        StrARAccount = cmbWarehouse.ActiveRow.Cells[2].Value.ToString();
                        StrCashAccount = cmbWarehouse.ActiveRow.Cells[3].Value.ToString();
                        StrSalesGLAccount = cmbWarehouse.ActiveRow.Cells[4].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmInvoices_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                rbtSVAT.Checked = true;
            }
            if (e.KeyCode == Keys.Menu)
            {
                button1_Click(null, null);
            }
            if (btnSave.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
                {
                    btnSave_Click(null, null);

                }
            }

            if (btnSearch.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L)
                {
                    btnSearch_Click(null, null);
                }
            }


            if (btnEditer.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E)
                {
                    toolStripButton4_Click(null, null);
                }
            }



            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
            {
                btnNew_Click(null, null);
            }


            if (btnConfirm.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                {
                    btnConfirm_Click(null, null);
                }
            }


            if (toolStripButton3.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
                {
                    toolStripButton3.ShowDropDown();
                }
            }


            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
            {
                this.Close();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Down)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void btnSNO_Click(object sender, EventArgs e)
        {

            btnSave.Enabled = true;
            EnableHeader(true);
            EnableFoter(true);
        }

        private void txtServicecharge_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                FooterCalculation();
                CalPaidAmt();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(txtInvoiceNo, cmbCustomer, e);

            if (e.KeyCode == Keys.Down)
            {
                if (cmbCustomer.Enabled == true)
                {
                    cmbCustomer.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void txtInvoiceNo_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmbSalesRep_KeyDown(object sender, KeyEventArgs e)
        {
            cmbWarehouse.PerformAction(UltraComboAction.Dropdown, false, false);
        }

        private void txtInvoiceNo_TextChanged(object sender, EventArgs e)
        {
            try
            {

                setValue();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rbtExculsive_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtInclusive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        private void FillInvoiceType()
        {
            try
            {
                if (rbtNoVat.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTPer.Text = "0.00";
                    txtVatPer.Text = "0.00";
                    txtVat.Text = "0.00";
                    txtNBTPer.ReadOnly = true;
                    txtVatPer.ReadOnly = true;

                }
                else if (rbtVAT.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTPer.Text = "0.00";
                    txtVat.Text = "0.00";
                    txtVatPer.Text = "0.00";

                    txtNBTPer.ReadOnly = false;
                    txtVatPer.ReadOnly = false;
                    GetTaxDeails();
                }
                else if (rbtSVAT.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTPer.Text = "0.00";
                    txtVatPer.Text = "0.00";
                    txtVat.Text = "0.00";

                    txtNBTPer.ReadOnly = false;
                    txtVatPer.ReadOnly = false;
                    GetTaxDeails();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void rbtNoVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbtNoVat.Checked == true)
                {
                    txtDescription.Visible = false;
                    label4.Visible = false;

                    txtNBTPer.Enabled = false;
                    txtVatPer.Enabled = false;

                    if (ug.Rows.Count > 0)
                    {
                        FooterCalculation();
                        CalPaidAmt();
                    }
                }

                else
                {
                    txtDescription.Visible = true;
                    label4.Visible = true;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rbtVAT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbtVAT.Checked == true)
                {

                    txtNBTPer.Enabled = true;
                    txtVatPer.Enabled = true;

                    txtDescription.Visible = true;
                    label4.Visible = true;


                    if (ug.Rows.Count > 0)
                    {
                        FooterCalculation();
                        CalPaidAmt();
                    }
                }
                else
                {
                    txtDescription.Visible = false;
                    label4.Visible = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rbtSVAT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                FillInvoiceType();
                if (rbtVAT.Checked == true)
                {

                    txtNBTPer.Enabled = true;
                    txtVatPer.Enabled = true;
                    txtDescription.Visible = true;
                    label4.Visible = true;


                    if (ug.Rows.Count > 0)
                    {
                        InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                        FooterCalculation();
                        CalPaidAmt();
                    }
                }
                else
                {
                    txtDescription.Visible = false;
                    label4.Visible = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscAmount_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int intGridRow = 0;
                double NewSubTotal = 0;
                for (intGridRow = 0; intGridRow < ug.Rows.Count; intGridRow++)
                {
                    if (ug.Rows[intGridRow].Cells[8].Value != null && ug.Rows[intGridRow].Cells[8].Value.ToString() != string.Empty)
                    {
                        NewSubTotal += double.Parse(ug.Rows[intGridRow].Cells[8].Value.ToString());
                    }
                }
                double TotalDis = Convert.ToDouble(txtDiscAmount.Text.ToString());
                double TotalVat = Convert.ToDouble(txtServicecharge.Text.ToString());
                txtNetValue.Text = ((NewSubTotal - TotalDis) + TotalVat).ToString("N2");
                txtpaid.Text = ((NewSubTotal - TotalDis) + TotalVat).ToString("N2");

            }
            catch
            {

            }
        }

        private void txtGridTotalExcl_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtGridTotalExcl.Text != null && txtGridTotalExcl.Text.Trim() != string.Empty)
                {
                    txtSubValue.Value = txtGridTotalExcl.Text.ToString();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscLineTot_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtGrossValue.Value = double.Parse(txtSubValue.Value.ToString()) - (double.Parse(txtDiscAmount.Value.ToString()) + double.Parse(txtDiscLineTot.Text));
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtSubValue_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtGrossValue.Value = double.Parse(txtSubValue.Value.ToString()) - (double.Parse(txtDiscAmount.Value.ToString()) + double.Parse(txtDiscLineTot.Text));
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        bool GrideValidation = false;
        private void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {

                if (ug.ActiveCell == null)
                    return;

                if (e.Cell.Column.Key == "ItemCode")
                {
                    StrSql = " SELECT   tblItemWhse.ItemID,  tblItemWhse.ItemDis, tblItemWhse.QTY, tblItemMaster.ItemClass, tblItemMaster.SalesGLAccount, tblItemWhse.UOM, tblItemMaster.Categoty, " +
                                " tblItemMaster.PriceLevel1 as ListPrice, tblItemMaster.PriceLevel2, tblItemMaster.PriceLevel3, tblItemMaster.PriceLevel4, tblItemMaster.PriceLevel6, tblItemMaster.PriceLevel5, " +
                                " tblItemMaster.PriceLevel7, tblItemMaster.PriceLevel8, tblItemMaster.PriceLevel9, tblItemMaster.PriceLevel10, isnull(tblItemWhse.UnitCost,0) as UnitCost,tblItemWhse.WhseId " +
                                " FROM         tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID where  tblItemWhse.ItemID='" + ug.Rows[e.Cell.Row.Index].Cells["ItemCode"].Value.ToString() + "'";

                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (ug.ActiveCell.Value.ToString() == dr["ItemId"].ToString())
                        {
                            ug.ActiveCell.Row.Cells["Description"].Value = dr["ItemDis"].ToString();
                            ug.ActiveCell.Row.Cells["OnHand"].Value = dr["QTY"].ToString();
                            ug.ActiveCell.Row.Cells["ItemClass"].Value = dr["ItemClass"].ToString();
                            ug.ActiveCell.Row.Cells["GL"].Value = dr["SalesGLAccount"].ToString();
                            ug.ActiveCell.Row.Cells["Quantity"].Value = "1.00";
                            ug.ActiveCell.Row.Cells["UOM"].Value = dr["UOM"].ToString();
                            ug.ActiveCell.Row.Cells["Categoty"].Value = dr["Categoty"].ToString();
                            ug.ActiveCell.Row.Cells["CostPrice"].Value = dr["UnitCost"].ToString();
                            ug.ActiveCell.Row.Cells["UnitPrice(Incl)"].Value = dr["ListPrice"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel1"].Value = dr["ListPrice"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel2"].Value = dr["PriceLevel2"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel3"].Value = dr["PriceLevel3"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel4"].Value = dr["PriceLevel4"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel5"].Value = dr["PriceLevel5"].ToString();

                            ug.ActiveCell.Row.Cells["WH"].Value = dr["WhseId"].ToString();

                            ug.ActiveCell.Row.Cells["TotalPrice"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["TotalPrice(Incl)"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["Discount"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["LineTax"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["LineDisc"].Value = "0.00";

                            LoadLineDiscount(ug.ActiveRow.Cells["ItemCode"].Value.ToString());
                            ug.ActiveCell.Row.Cells["Discount"].Value = cmbLineDiscount.Rows[0].Cells[0].Value.ToString();


                            if (dblCusPriceLevel == 0)
                            {
                                // dblCusPriceLevel = 1;
                                string PriceLevel = "PriceLevel" + "1";
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr["ListPrice"].ToString();
                            }
                            else
                            {
                                string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr["ListPrice"].ToString();
                            }



                        }

                    }
                    HideSelectedRow();
                }

                if (e.Cell.Column.Key == "Description")
                {

                    //if (ug.ActiveCell.Row.Cells["Categoty"].Value != null)
                    //{
                    //    if (ug.ActiveCell.Row.Cells["Categoty"].Value.ToString() == "Customize")
                    //    {
                    //        return;
                    //    }
                    //}

                    StrSql = " SELECT   tblItemWhse.ItemID,  tblItemWhse.ItemDis, tblItemWhse.QTY, tblItemMaster.ItemClass, tblItemMaster.SalesGLAccount, tblItemWhse.UOM, tblItemMaster.Categoty, " +
                                " tblItemMaster.PriceLevel1 as ListPrice, tblItemMaster.PriceLevel2, tblItemMaster.PriceLevel3, tblItemMaster.PriceLevel4, tblItemMaster.PriceLevel6, tblItemMaster.PriceLevel5, " +
                                " tblItemMaster.PriceLevel7, tblItemMaster.PriceLevel8, tblItemMaster.PriceLevel9, tblItemMaster.PriceLevel10, isnull(tblItemWhse.UnitCost,0) as UnitCost,tblItemWhse.WhseId " +
                                " FROM         tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID where  tblItemWhse.ItemDis='" + ug.Rows[e.Cell.Row.Index].Cells["Description"].Value.ToString() + "'";

                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (ug.ActiveCell.Value.ToString() == dr["ItemDis"].ToString())
                        {
                            ug.ActiveCell.Row.Cells["ItemCode"].Value = dr["ItemID"].ToString();
                            ug.ActiveCell.Row.Cells["OnHand"].Value = dr["QTY"].ToString();
                            ug.ActiveCell.Row.Cells["ItemClass"].Value = dr["ItemClass"].ToString();
                            ug.ActiveCell.Row.Cells["GL"].Value = dr["SalesGLAccount"].ToString();
                            ug.ActiveCell.Row.Cells["Quantity"].Value = "1.00";
                            ug.ActiveCell.Row.Cells["UOM"].Value = dr["UOM"].ToString();
                            ug.ActiveCell.Row.Cells["Categoty"].Value = dr["Categoty"].ToString();
                            ug.ActiveCell.Row.Cells["CostPrice"].Value = dr["UnitCost"].ToString();

                            ug.ActiveCell.Row.Cells["WH"].Value = dr["WhseId"].ToString();

                            ug.ActiveCell.Row.Cells["UnitPrice(Incl)"].Value = dr["ListPrice"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel1"].Value = dr["ListPrice"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel2"].Value = dr["PriceLevel2"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel3"].Value = dr["PriceLevel3"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel4"].Value = dr["PriceLevel4"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel5"].Value = dr["PriceLevel5"].ToString();
                            ug.ActiveCell.Row.Cells["TotalPrice"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["TotalPrice(Incl)"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["Discount"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["LineTax"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["LineDisc"].Value = "0.00";
                            LoadLineDiscount(ug.ActiveRow.Cells["ItemCode"].Value.ToString());

                            ug.ActiveCell.Row.Cells["Discount"].Value = cmbLineDiscount.Rows[0].Cells[0].Value.ToString();


                            if (dblCusPriceLevel == 0)
                            {
                                // dblCusPriceLevel = 1;
                                string PriceLevel = "PriceLevel" + "1";
                                // string PriceLevel = "PriceLevel" + (dblCusPriceLevel).ToString().Trim();
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr["ListPrice"].ToString();
                            }
                            else
                            {
                                string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr["ListPrice"].ToString();
                            }


                        }
                    }
                    HideSelectedRow();
                }




                if (e.Cell.Column.Key == "Discount")
                {
                    //setConnectionString();
                    //SqlConnection myConnection = new SqlConnection(ConnectionString);
                    //SqlCommand myCommand = new SqlCommand("select Discount+Profit from tblItemMaster where ItemID='" + ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString() + "'", myConnection);
                    //SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                    //DataTable dt41 = new DataTable();
                    //da41.Fill(dt41);

                    if (ug.ActiveCell.Row.Cells["Discount"].Value.ToString() != "40+4")
                    {

                        if (Convert.ToDouble(ug.ActiveCell.Row.Cells["Discount"].Value) > 99)
                        {
                            MessageBox.Show("Invalid Line Discount in Line no:-" + ug.ActiveCell.Row.Cells["LineNo"].Value.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            ug.ActiveCell.Row.Cells["Discount"].Value = 0.00;
                            ug.ActiveCell.Row.Cells["LineDisc"].Value = 0.00;
                            ug.ActiveCell.Row.Cells["TotalPrice"].Value = Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) * Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value);
                            GrideValidation = true;
                            return;
                        }
                    }


                    //if (dt41.Rows.Count > 0)
                    //{
                    //    if (ug.ActiveCell.Row.Cells["Discount"].Value.ToString() != "40+4")
                    //    {
                    //        if (Convert.ToDouble(ug.ActiveCell.Row.Cells["Discount"].Value) > Convert.ToDouble(dt41.Rows[0].ItemArray[0]))
                    //        {
                    //            MessageBox.Show("Invalid Line Discount in Line no:-" + ug.ActiveCell.Row.Cells["LineNo"].Value.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //            ug.ActiveCell.Row.Cells["Discount"].Value = 0.00;
                    //            ug.ActiveCell.Row.Cells["LineDisc"].Value = 0.00;
                    //            ug.ActiveCell.Row.Cells["TotalPrice"].Value = Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) * Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value);
                    //            GrideValidation = true;
                    //            return;
                    //        }

                    //    }
                    //}
                }

                if (e.Cell.Column.Key == "TotalPrice")
                {
                    if (Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) * Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value) != Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value))
                    {
                        ug.ActiveCell.Row.Cells["LineDisc"].Value = (Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) * Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value)) - Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value);

                        // dgvr.Cells["TotalPrice"].Value = dgvr.Cells["TotalPrice"].Value;
                    }
                }

                if (e.Cell.Column.Key == "UnitPrice")
                {

                    if (Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) == 0.00 && e.Cell.Row.Cells["ItemCode"].Value != null && e.Cell.Row.Cells["Description"].Value != null)
                    {
                        if (e.Cell.Row.Cells["ItemCode"].Value.ToString() != "" && e.Cell.Row.Cells["Description"].Value.ToString() != "")
                        {
                            ug.ActiveCell.Row.Cells["Quantity"].Value = 0;
                            ug.ActiveCell.Row.Cells["FOCQty"].Value = 1;

                            ug.ActiveCell.Row.Cells["Quantity"].Activation = Activation.NoEdit;
                            ug.ActiveCell.Row.Cells["FOCQty"].Activation = Activation.AllowEdit;
                        }
                    }


                    else
                    {
                        if (e.Cell.Row.Cells["ItemCode"].Value.ToString() != "" && e.Cell.Row.Cells["Description"].Value.ToString() != "")
                        {
                            ug.ActiveCell.Row.Cells["FOCQty"].Value = 0;
                            ug.ActiveCell.Row.Cells["Quantity"].Activation = Activation.AllowEdit;
                            ug.ActiveCell.Row.Cells["FOCQty"].Activation = Activation.AllowEdit;
                        }

                    }



                }


                if (e.Cell.Column.Key == "Quantity" || e.Cell.Column.Key == "FOCQty")
                {
                    ug.ActiveCell.Row.Cells["TotalQty"].Value = Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value) + Convert.ToDouble(ug.ActiveCell.Row.Cells["FOCQty"].Value);
                }


                if ((e.Cell.Column.Key == "TotalPrice" || e.Cell.Column.Key == "LineDisc") && Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) > 0)
                {


                    if (Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) * Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value) < Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value))
                    {
                        MessageBox.Show("Invalid Line Total in Line no:-" + ug.ActiveCell.Row.Cells["LineNo"].Value.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        ug.ActiveCell.Row.Cells["Discount"].Value = 0.00;
                        GrideValidation = true;
                        return;
                    }

                    if (ug.ActiveCell.Row.Cells["Discount"].Value.ToString() == "40+4")
                    {
                        double DisPrice1 = ((Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) * Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value)) * 60 / 100);
                        double DisPrice2 = DisPrice1 * 96 / 100;

                        if (Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value) != DisPrice2)
                        {
                            double dis = Math.Round(((Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) - (Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value) / Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value))) / Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value)) * 100, 2, MidpointRounding.AwayFromZero);
                            if (ug.ActiveCell.Row.Cells["Discount"].Value.ToString() != dis.ToString())
                            {
                                ug.ActiveCell.Row.Cells["Discount"].Value = Math.Round(((Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) - (Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value) / Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value))) / Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value)) * 100, 2, MidpointRounding.AwayFromZero);//Calculate DisPer according to Line Total
                            }
                        }

                    }
                    else if (Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value) > 0)
                    {

                        double dis = Math.Round(((Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) - (Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value) / Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value))) / Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value)) * 100, 2, MidpointRounding.AwayFromZero);
                        if (Convert.ToDouble(ug.ActiveCell.Row.Cells["Discount"].Value) != dis)
                        {
                            ug.ActiveCell.Row.Cells["Discount"].Value = Math.Round(((Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) - (Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value) / Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value))) / Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value)) * 100, 2, MidpointRounding.AwayFromZero);//Calculate DisPer according to Line Total
                        }

                        if (ug.ActiveCell.Row.Cells["TotalPrice"].Value != null && ug.ActiveCell.Row.Cells["TotalPrice"].Value.ToString().Trim() != string.Empty && Convert.ToDouble(ug.ActiveCell.Row.Cells["LineDisc"].Value) != 0.00)
                        {
                            if (Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) * Convert.ToDouble(ug.ActiveCell.Row.Cells["Quantity"].Value) == Convert.ToDouble(ug.ActiveCell.Row.Cells["TotalPrice"].Value))
                            {
                                ug.ActiveCell.Row.Cells["LineDisc"].Value = 0.00;
                            }
                        }
                    }
                }



                if (e.Cell.Column.Key != "LineNo" && e.Cell.Column.Key != "TotalPrice")
                    InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                CalPaidAmt();


            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        private void HideSelectedRow()
        {
            for (int x = 0; x < ultraCombo1.Rows.Count; x++)
            {
                ultraCombo1.Rows[x].Hidden = false;
                ultraCombo2.Rows[x].Hidden = false;
            }
            for (int i = 0; i < ultraCombo1.Rows.Count; i++)
            {
                for (int v = 0; v < ug.Rows.Count; v++)
                {
                    if (ultraCombo1.Rows[i].Cells[0].Value.ToString() == ug.Rows[v].Cells["ItemCode"].Value.ToString())
                    {
                        ultraCombo1.Rows[i].Hidden = true;
                        ultraCombo2.Rows[i].Hidden = true;
                    }

                }
            }
        }
        public void FooterCalculation_1()
        {
            try
            {
                if (IsFind != true)
                {
                    double dblGrossTotalF = 0;
                    double dblNetTotalF = 0;
                    double dblNBTF = 0;
                    double dblVATF = 0;
                    double dblDiscountF = 0;
                    double dblServiceChgF = 0;
                    double dblDiscountLineTot = 0;
                    double dblTotalDiscount = 0;
                    double dblTotalDiscountTemp = 0;

                    dblGrossTotalF = Convert.ToDouble(txtSubValue.Value.ToString().Trim());
                    dblServiceChgF = Convert.ToDouble(txtServicecharge.Value.ToString().Trim());
                    dblDiscountF = (dblGrossTotalF) * Convert.ToDouble(txtDiscPer.Value.ToString().Trim()) / 100;
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;

                    dblTotalDiscountTemp = dblTotalDiscount +
                    (double.Parse(txtServicecharge.Value.ToString()) * double.Parse(txtDiscPer.Value.ToString()) / 100);
                    double _AmountWithoutDisc = double.Parse(txtSubValue.Value.ToString()) + double.Parse(txtDiscLineTot.Text) - dblTotalDiscountTemp + double.Parse(txtServicecharge.Value.ToString());

                    if (cmbInvoiceType.Value.ToString() != "3")
                    {
                        txtNBT.Value = _AmountWithoutDisc * double.Parse(txtNBTPer.Value.ToString()) / 100;
                        txtVat.Value = (_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) *
                        double.Parse(txtVatPer.Value.ToString()) / 100;
                    }
                    dblNBTF = Convert.ToDouble(txtNBT.Value.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVat.Value.ToString().Trim());
                    dblNetTotalF = _AmountWithoutDisc + dblNBTF + dblVATF;
                    txtNetValue.Value = dblNetTotalF.ToString("N2");
                    //Asanga
                    txtpaid.Text = dblNetTotalF.ToString("N2");

                    //if (cmbInvoiceType.Value.ToString() == "1")
                    // //   txtDiscAmount.Text = (Convert.ToDouble(txtGridTotalExcl.Text) *
                    //  //      Convert.ToDouble(txtDiscPer.Text) / 100).ToString();
                    //else
                    // //   txtDiscAmount.Text = (Convert.ToDouble(txtSubValue.Text.Trim()) * Convert.ToDouble(txtDiscPer.Text) / 100).ToString();


                }
            }
            catch { }
        }

        public void InvoiceCalculation_1(Int64 iInvoiceType)
        {
            try
            {
                if (IsFind != true)
                {

                    double dblSubTotal = 0;
                    double dblTotVAT = 0;
                    double dblDiscountedPrice = 0;

                    double dblPriceWithoutVat = 0;
                    double dblExcluisvePrice = 0;

                    double dbdiscountPer = 0;
                    double dblLinediscountAmount = 0;

                    double dblInclusiveLineTotal = 0;
                    double dblDiscountedLineTotal = 0;

                    double dblLineVAT = 0;
                    double dblLineNBT = 0;

                    double dblTotalVAT = 0;
                    double dblTotalNBT = 0;
                    double dblTotalLinedDiscount = 0;

                    double dblLineTax = 0;
                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = Convert.ToDouble(txtVatPer.Value);  //12%
                    double dblNBTPer = Convert.ToDouble(txtNBTPer.Value);  //2%

                    txtGridTotalExcl.Text = "0";

                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            //ug.ActiveCell.Row.Cells
                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);//here price in inclusive eg-114.24
                            dbdiscountPer = Math.Round(Convert.ToDouble(ugR.Cells["Discount"].Value) / 100, 2, MidpointRounding.AwayFromZero);
                            dblDiscountedPrice = Math.Round(dblLinePrice - (dblLinePrice * dbdiscountPer), 2, MidpointRounding.AwayFromZero);
                            dblLinediscountAmount = Math.Round(dblLinePrice - dblDiscountedPrice, 2, MidpointRounding.AwayFromZero);
                            txtGridTotalExcl.Text = (double.Parse(txtGridTotalExcl.Text) + dblDiscountedPrice).ToString();
                            dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 2, MidpointRounding.AwayFromZero);//price without vat=102
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);
                            dblLineVAT = Math.Round(((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            dblLineNBT = Math.Round(((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            ugR.Cells["UnitPrice(Incl)"].Value = dblExcluisvePrice;//100
                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;
                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal;
                            ugR.Cells["TotalPrice"].Value = Math.Round(dblLineQty * dblLinePrice, 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = Math.Round((dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);

                        }
                        txtSubValue.Value = dblSubTotal;
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //    txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = Math.Round((dblSubTotal), 2, MidpointRounding.AwayFromZero);
                        txtpaid.Text = txtNetValue.Value.ToString();
                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty)
                                dblLinePrice = 0;
                            else
                                dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty)
                                dblLineQty = 0;
                            else
                                dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty)
                                dbdiscountPer = 0;
                            else
                                dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblDiscountedLineTotal = dblInclusiveLineTotal - dblLinediscountAmount;

                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);

                            ugR.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);


                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //   txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtSubValue.Value = dblSubTotal;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = (dblSubTotal);
                        txtpaid.Text = txtNetValue.Value.ToString();
                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            //error
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            //error
                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;

                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            ugR.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT                            
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineVAT, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);

                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //  txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtSubValue.Value = dblSubTotal;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = (dblSubTotal);
                        txtpaid.Text = txtNetValue.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InvoiceCalculation(Int64 iInvoiceType)
        {
            try
            {
                if (IsFind != true || IsFind == true)
                {
                    double dblSubTotal = 0;
                    double dblTotVAT = 0;
                    double dblDiscountedPrice = 0;

                    double dblPriceWithoutVat = 0;
                    double dblExcluisvePrice = 0;

                    double dbdiscountPer = 0;
                    double dblLinediscountAmount = 0;

                    double dblInclusiveLineTotal = 0;
                    double dblDiscountedLineTotal = 0;

                    double dblLineVAT = 0;
                    double dblLineNBT = 0;

                    double dblTotalVAT = 0;
                    double dblTotalNBT = 0;
                    double dblTotalLinedDiscount = 0;

                    double dblLineTax = 0;

                    //double dblTotNBT = 0;
                    //double dblInvTotal = 0;
                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = 0;
                    double dblNBTPer = 0;
                    double TotalAmount = 0;


                    try
                    {

                        dblVATPer = Convert.ToDouble(txtVatPer.Value);  //12%                     
                        dblNBTPer = Convert.ToDouble(txtNBTPer.Value);  //2%

                    }
                    catch (Exception ex)
                    {
                        dblVATPer = 0;
                        dblNBTPer = 0;
                    }




                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (UltraGridRow dgvr in ug.Rows)
                        {
                            //ug.ActiveCell.Row.Cells



                            if (dgvr.Cells["ItemCode"].Value == null || dgvr.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineDisc"].Value = 0.00;
                            if (dgvr.Cells["UnitPrice"].Value == null || dgvr.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Discount"].Value = 0.00;
                            if (dgvr.Cells["Quantity"].Value == null || dgvr.Cells["Quantity"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Quantity"].Value = 0.00;
                            if (dgvr.Cells["UnitPrice(Incl)"].Value == null || dgvr.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["TotalPrice(Incl)"].Value == null || dgvr.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["LineTax"].Value == null || dgvr.Cells["LineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineTax"].Value = 0.00;

                            dblLinePrice = Convert.ToDouble(dgvr.Cells["UnitPrice"].Value);//here price in inclusive eg-114.24
                            dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            dblDiscountedPrice = dblLinePrice - (dblLinePrice * dbdiscountPer);
                            dblLinediscountAmount = dblLinePrice - dblDiscountedPrice;

                            //dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 2, MidpointRounding.AwayFromZero);//price without vat=102
                            dblPriceWithoutVat = ((dblDiscountedPrice * 100) / (100 + dblVATPer));//price without vat=102
                            dblLineQty = Convert.ToDouble(dgvr.Cells["Quantity"].Value);

                            //                            dblLineVAT = Math.Round(((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = ((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty);
                            //dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            dblExcluisvePrice = ((dblPriceWithoutVat * 100) / (100 + dblNBTPer));//exclusive price here eg 100
                                                                                                 //                            dblLineNBT = Math.Round(((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dblLineNBT = ((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty);
                            dgvr.Cells["UnitPrice(Incl)"].Value = dblExcluisvePrice;//100
                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;
                            if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                            {
                                double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);
                                dgvr.Cells["TotalPrice"].Value = DisPrice1 * 96 / 100;


                                dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - (dblLinePrice * dblLineQty - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value));
                            }
                            else
                            {
                                if (ug.ActiveCell.Column.Key != "TotalPrice")
                                {
                                    dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;
                                }
                                else
                                {
                                    dgvr.Cells["TotalPrice(Incl)"].Value = dgvr.Cells["TotalPrice"].Value;
                                }
                            }



                            if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                            {
                                double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);
                                dgvr.Cells["TotalPrice"].Value = DisPrice1 * 96 / 100;
                                dgvr.Cells["LineDisc"].Value = dblLinePrice * dblLineQty - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                            }
                            else
                            {


                                if (ug.ActiveCell.Column.Key != "TotalPrice")
                                {
                                    dgvr.Cells["LineDisc"].Value = dblLinediscountAmount * dblLineQty;
                                    dgvr.Cells["TotalPrice"].Value = (dblLineQty * dblLinePrice) - Math.Round(Convert.ToDouble(dgvr.Cells["LineDisc"].Value), 2, MidpointRounding.AwayFromZero);
                                }
                                else if (dblLinePrice * dblLineQty != Convert.ToDouble(dgvr.Cells["TotalPrice"].Value))
                                {
                                    dgvr.Cells["LineDisc"].Value = (dblLinePrice * dblLineQty) - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                                    // dgvr.Cells["TotalPrice"].Value = dgvr.Cells["TotalPrice"].Value;
                                }

                            }



                            if (Convert.ToDouble(dgvr.Cells["UnitPrice"].Value) < 1)
                            {
                                dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                                dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            }
                            if (Convert.ToDouble(dgvr.Cells["UnitPrice"].Value) > 0 && Convert.ToDouble(dgvr.Cells["Quantity"].Value) > 0)
                            {
                                if (dgvr.Cells["ItemClass"].Value.ToString() == "7")
                                {
                                    dgvr.Cells["UnitPrice(Incl)"].Value = Math.Round(((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / dblLineQty) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                    //  dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round(Convert.ToDouble(dgvr.Cells["UnitPrice(Incl)"].Value) * dblLineQty, 2, MidpointRounding.AwayFromZero);
                                    dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / 108 * 100), 2, MidpointRounding.AwayFromZero);



                                }
                                else
                                {

                                    dgvr.Cells["UnitPrice(Incl)"].Value = Math.Round(((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / dblLineQty) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                    //     dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round(Convert.ToDouble(dgvr.Cells["UnitPrice(Incl)"].Value) * dblLineQty, 2, MidpointRounding.AwayFromZero);

                                    dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                }
                            }
                            //dgvr.Cells["TotalPrice"].Value = Math.Round(dblLineQty * dblLinePrice, 2, MidpointRounding.AwayFromZero);

                            //                            dgvr.Cells["LineDisc"].Value = Math.Round(dblLinediscountAmount * dblLineQty, 2, MidpointRounding.AwayFromZero); 



                            //VAT
                            // dblLineTax = Math.Round((dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            dblLineTax = (dblLineNBT + dblLineVAT);
                            //dblLineTax = Math.Round(Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) - Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            //                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            dblSubTotal = dblSubTotal + Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            //dblTotalVAT = dblTotalVAT + dblLineVAT;
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            //dblTotalNBT = dblTotalNBT + dblLineNBT;
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                            //txtGridTotalExcl.Text = TotalAmount.ToString();

                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();


                        }
                        dblSubTotal = dblSubTotal - Convert.ToDouble(txtDiscAmount.Text.ToString());
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //     txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetValue.Text = (Math.Round((dblSubTotal), 2, MidpointRounding.AwayFromZero)).ToString();
                        txtpaid.Text = txtNetValue.Text.ToString();

                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (UltraGridRow dgvr in ug.Rows)
                        {


                            if (dgvr.Cells["UnitPrice"].Value == null || dgvr.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Discount"].Value = 0.00;
                            if (dgvr.Cells["Quantity"].Value == null || dgvr.Cells["Quantity"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Quantity"].Value = 0.00;
                            if (dgvr.Cells["UnitPrice(Incl)"].Value == null || dgvr.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["TotalPrice(Incl)"].Value == null || dgvr.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["LineTax"].Value == null || dgvr.Cells["LineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineTax"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineDisc"].Value = 0.00;


                            if (dgvr.Cells["ItemCode"].Value == null || dgvr.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["Quantity"].Value);

                            //dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dblInclusiveLineTotal = (dblLinePrice * dblLineQty);
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty)
                                dbdiscountPer = 0;
                            else
                                try
                                {
                                    dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                                    dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                                    dblDiscountedLineTotal = dblInclusiveLineTotal - dblLinediscountAmount;
                                }
                                catch
                                { }


                            if (ug.ActiveCell.Column.Key == "Discount")
                            {
                                if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                                {
                                    double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);


                                    dgvr.Cells["TotalPrice"].Value = DisPrice1 * 96 / 100;
                                    dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - (dblLinePrice * dblLineQty - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value));
                                }
                                else
                                {

                                    dgvr.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero) - Math.Round(dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                                    dgvr.Cells["LineDisc"].Value = dblLinediscountAmount;

                                }
                            }



                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);

                            //dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            //dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            //dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            //dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;


                            if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                            {
                                double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);
                                dgvr.Cells["TotalPrice"].Value = DisPrice1 * 96 / 100;
                                dgvr.Cells["LineDisc"].Value = dblLinePrice * dblLineQty - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                            }
                            else
                            {


                                dgvr.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero) - Math.Round(dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);



                            }

                            if (Convert.ToDouble(dgvr.Cells["UnitPrice"].Value) < 1)
                            {
                                dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                                dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            }
                            if (Convert.ToDouble(dgvr.Cells["UnitPrice"].Value) > 0 && Convert.ToDouble(dgvr.Cells["Quantity"].Value) > 0)
                            {
                                if (dgvr.Cells["ItemClass"].Value.ToString() == "7")
                                {
                                    dgvr.Cells["UnitPrice(Incl)"].Value = Math.Round(((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / dblLineQty) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                    //  dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round(Convert.ToDouble(dgvr.Cells["UnitPrice(Incl)"].Value) * dblLineQty, 2, MidpointRounding.AwayFromZero);
                                    dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / 108 * 100) / 102 * 100, 2, MidpointRounding.AwayFromZero);



                                }
                                else
                                {

                                    dgvr.Cells["UnitPrice(Incl)"].Value = Math.Round(((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / dblLineQty) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                    //     dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round(Convert.ToDouble(dgvr.Cells["UnitPrice(Incl)"].Value) * dblLineQty, 2, MidpointRounding.AwayFromZero);

                                    dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                }
                            }

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            //dblLineTax = Math.Round(Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value) - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value), 3, MidpointRounding.AwayFromZero);

                            dgvr.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["TotalPrice"].Value), 2, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();


                        }
                        dblSubTotal = dblSubTotal - Convert.ToDouble(txtDiscAmount.Text.ToString());
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //    txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetValue.Text = (dblSubTotal).ToString();
                        txtpaid.Text = txtNetValue.Text.ToString();

                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (UltraGridRow dgvr in ug.Rows)
                        {
                            if (dgvr.Cells["UnitPrice"].Value == null || dgvr.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Discount"].Value = 0.00;
                            if (dgvr.Cells["Quantity"].Value == null || dgvr.Cells["Quantity"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Quantity"].Value = 0.00;
                            if (dgvr.Cells["UnitPrice(Incl)"].Value == null || dgvr.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["TotalPrice(Incl)"].Value == null || dgvr.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["LineTax"].Value == null || dgvr.Cells["LineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineTax"].Value = 0.00;
                            if (dgvr.Cells["LineDisc"].Value == null || dgvr.Cells["LineDisc"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineDisc"].Value = 0.00;

                            if (dgvr.Cells["TotalPrice"].Value == null || dgvr.Cells["TotalPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["TotalPrice"].Value = 0.00;

                            if (dgvr.Cells["ItemCode"].Value == null || dgvr.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);


                            if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                            {
                                double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);
                                dgvr.Cells["TotalPrice"].Value = DisPrice1 * 96 / 100;
                                //   dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - (dblLinePrice * dblLineQty - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value));

                            }
                            else
                            {
                                dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                                dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                                // dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;
                            }

                            // dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round(((dblLineQty * dblLinePrice)), 3, MidpointRounding.AwayFromZero);
                            //dgvr.Cells["LineDisc"].Value = dblLinediscountAmount;

                            //dgvr.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero) - Convert.ToDouble(dgvr.Cells["LineDisc"].Value);


                            if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                            {
                                double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);
                                dgvr.Cells["TotalPrice"].Value = DisPrice1 * 96 / 100;
                                dgvr.Cells["LineDisc"].Value = dblLinePrice * dblLineQty - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);

                            }
                            else
                            {

                                dgvr.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero) - Math.Round(dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                                dgvr.Cells["LineDisc"].Value = dblLinediscountAmount;

                                //else if (dblLinePrice * dblLineQty != Convert.ToDouble(dgvr.Cells["TotalPrice"].Value))
                                //{
                                //    dgvr.Cells["LineDisc"].Value = (dblLinePrice * dblLineQty) - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);

                                //    //  dgvr.Cells["TotalPrice"].Value = dgvr.Cells["TotalPrice"].Value;
                                //}

                            }



                            if (Convert.ToDouble(dgvr.Cells["UnitPrice"].Value) < 1)
                            {
                                dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                                dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            }
                            if (Convert.ToDouble(dgvr.Cells["UnitPrice"].Value) > 0 && Convert.ToDouble(dgvr.Cells["Quantity"].Value) > 0)
                            {
                                if (dgvr.Cells["ItemClass"].Value.ToString() == "7")
                                {
                                    dgvr.Cells["UnitPrice(Incl)"].Value = Math.Round(((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / dblLineQty) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                    //  dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round(Convert.ToDouble(dgvr.Cells["TotalPrice"].Value), 2, MidpointRounding.AwayFromZero);
                                    dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / 108 * 100), 2, MidpointRounding.AwayFromZero);



                                }
                                else
                                {

                                    dgvr.Cells["UnitPrice(Incl)"].Value = Math.Round(((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / dblLineQty) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                    //   dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round(Convert.ToDouble(dgvr.Cells["TotalPrice"].Value), 2, MidpointRounding.AwayFromZero);

                                    dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round((Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) / 108 * 100), 2, MidpointRounding.AwayFromZero);
                                }
                            }
                            //VAT
                            // dblLineVAT = Math.Round(Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value) - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["LineTax"].Value = Math.Round(dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            if (rbtVAT.Checked == false)
                            {
                                dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["TotalPrice"].Value), 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            }
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            // dblTotVAT = Math.Round(dblTotVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            //dgvr.Activated = true;
                            //ug.PerformAction(UltraGridAction.CommitRow);
                            //ug.PerformAction(UltraGridAction.ExitEditMode);  
                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();


                        }
                        dblSubTotal = dblSubTotal - Convert.ToDouble(txtDiscAmount.Text.ToString());
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //     txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetValue.Text = (dblSubTotal).ToString();
                        txtpaid.Text = txtNetValue.Text.ToString();



                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void FooterCalculation()
        {
            try
            {
                if (IsFind != true || IsFind == true)
                {
                    double dblGrossTotalF = 0;
                    double dblNetTotalF = 0;
                    double dblNBTF = 0;
                    double dblVATF = 0;
                    double dblDiscountF = 0;
                    double dblServiceChgF = 0;
                    double dblDiscountLineTot = 0;
                    double dblTotalDiscount = 0;
                    double InsideVAT = 0; double InsideNBT = 0;
                    double InsideVatTotal = 0; double InsideNbtTotal = 0; double grossTotal = 0;


                    dblGrossTotalF = Convert.ToDouble(txtSubValue.Text.ToString().Trim());
                    dblServiceChgF = Convert.ToDouble(txtServicecharge.Text.ToString().Trim());
                    //    dblDiscountF = (dblGrossTotalF+dblServiceChgF) * Convert.ToDouble(txtDiscPer.Text.ToString().Trim()) / 100;
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;

                    //    txtDiscAmount.Text = (dblDiscountF).ToString();
                    dblDiscountF = double.Parse(txtDiscAmount.Text.ToString());
                    double _AmountWithoutDisc = double.Parse(txtSubValue.Text.ToString()) - //102
                       dblDiscountF
                    +
                        double.Parse(txtServicecharge.Text.ToString());


                    if (cmbInvoiceType.Value.ToString() == "2")
                    {
                        txtNBT.Text = (_AmountWithoutDisc * double.Parse(txtNBTPer.Text.ToString()) / 100).ToString();
                        txtVat.Text = ((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                            double.Parse(txtVatPer.Text.ToString()) / 100).ToString();
                    }
                    else
                    {
                        txtNBT.Text = (_AmountWithoutDisc * double.Parse(txtNBTPer.Text.ToString()) / 100).ToString();
                        // txtVat.Value = Math.Round(((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                        //double.Parse(txtVatPer.Text.ToString()) / 100), 1, MidpointRounding.AwayFromZero);
                        string sss = null;
                        sss = Convert.ToDouble((_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) *
                         double.Parse(txtVatPer.Value.ToString()) / 100).ToString("");
                        txtVat.Value = Convert.ToDouble((_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) *
                        double.Parse(txtVatPer.Value.ToString()) / 100).ToString("");

                        //txtVat.Text = ((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                        //  double.Parse(txtVatPer.Text.ToString()) / 100).ToString();
                    }
                    int intGridRow;
                    dblSubTot = 0;
                    for (intGridRow = 0; intGridRow < ug.Rows.Count; intGridRow++)
                    {


                        if (ug.Rows[intGridRow].Cells["TotalPrice"].Value != null || ug.Rows[intGridRow].Cells["TotalPrice"].Value.ToString().Trim() != string.Empty)
                        {
                            if (ug.Rows[intGridRow].Cells["ItemClass"].Value.ToString() == "7")
                            {

                                InsideNBT = 0;
                                InsideVAT = Math.Round((Convert.ToDouble(ug.Rows[intGridRow].Cells["TotalPrice(Incl)"].Value)) * 0.08, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {

                                InsideNBT = 0;
                                InsideVAT = Math.Round((Convert.ToDouble(ug.Rows[intGridRow].Cells["TotalPrice(Incl)"].Value)) * 0.08, 2, MidpointRounding.AwayFromZero);
                            }
                        }

                        //double P1 = Math.Round((Convert.ToDouble(ug.Rows[intGridRow].Cells["TotalPrice"].Value) - (InsideNBT + InsideVAT)), 2, MidpointRounding.AwayFromZero);
                        //double P2 = Math.Round(Convert.ToDouble(ug.Rows[intGridRow].Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                        //double ajs = 0;
                        //if (P1 > P2)
                        //{
                        //    ajs = Math.Round(P1 - Convert.ToDouble(ug.Rows[intGridRow].Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                        //    InsideVAT = InsideVAT + ajs;
                        //}
                        //else
                        //{
                        //    ajs = Math.Round(Convert.ToDouble(ug.Rows[intGridRow].Cells["TotalPrice(Incl)"].Value) - P1, 2, MidpointRounding.AwayFromZero);
                        //    InsideVAT = InsideVAT - ajs;
                        //}








                        InsideNbtTotal += InsideNBT;
                        InsideVatTotal += InsideVAT;
                        if (rbtVAT.Checked == true)
                        {
                            dblSubTot = dblSubTot + (double.Parse(ug.Rows[intGridRow].Cells["TotalPrice(Incl)"].Value.ToString()));
                        }
                        else
                        {
                            dblSubTot = dblSubTot + double.Parse(ug.Rows[intGridRow].Cells["TotalPrice"].Value.ToString());
                        }
                    }

                    if (rbtVAT.Checked == true)
                    {
                        dblNbtAmount = InsideNbtTotal;
                        dblVatAmount = InsideVatTotal;
                        _AmountWithoutDisc = ((dblSubTot - dblDiscountF) + double.Parse(txtServicecharge.Text.ToString())) + InsideNbtTotal + InsideVatTotal;

                    }
                    else
                    {
                        dblNbtAmount = 0;
                        dblVatAmount = 0;
                        _AmountWithoutDisc = ((dblSubTot - dblDiscountF) + double.Parse(txtServicecharge.Text.ToString()));

                    }


                    txtNBT.Text = dblNbtAmount.ToString().Trim();
                    txtVat.Text = dblVatAmount.ToString().Trim();

                    dblNetTotalF = Math.Round(_AmountWithoutDisc, 2, MidpointRounding.AwayFromZero);// +double.Parse(txtDiscAmount.Value.ToString());
                                                                                                    //28.125+3.375
                    txtNetValue.Text = dblNetTotalF.ToString("N2"); //127.99
                                                                    //Asanga
                    txtpaid.Text = dblNetTotalF.ToString("N2");





                }


                //CalPaidAmt();
            }
            catch { }
        }

        private void cmbInvoiceType_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                double Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
                double Tax2Rate = double.Parse(txtVatPer.Text.Trim());
                if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice(Incl)"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice(Incl)"].Hidden = true;
                }
                else if (Convert.ToInt64(cmbInvoiceType.Value) == 2)//Exclusive
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice(Incl)"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice(Incl)"].Hidden = true;
                }
                else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice(Incl)"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice(Incl)"].Hidden = true;
                    Tax1Rate = 0.00;
                    Tax2Rate = 0.00;

                }
                //Invoice calculation
                //InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                //FooterCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        public void FooterCalculation_test()
        {
            try
            {
                if (IsFind != true)
                {
                    double dblGrossTotalF = 0;
                    double dblNetTotalF = 0;
                    double dblNBTF = 0;
                    double dblVATF = 0;
                    double dblDiscountF = 0;
                    double dblServiceChgF = 0;
                    double dblDiscountLineTot = 0;
                    double dblTotalDiscount = 0;

                    dblGrossTotalF = Convert.ToDouble(txtSubValue.Value.ToString().Trim());//subtotal
                    dblServiceChgF = Convert.ToDouble(txtServicecharge.Value.ToString().Trim());//Service Charge

                    foreach (UltraGridRow ugR in ug.Rows)
                    {
                        if (ug.ActiveCell.Value != null && ug.ActiveCell.Value.ToString().Trim() != string.Empty)
                        {
                            ug.ActiveCell.Row.Cells["LineDisc"].Value = double.Parse(ug.ActiveCell.Row.Cells["TotalPrice"].Value.ToString()) *
                                (double.Parse(ug.ActiveCell.Row.Cells["Discount"].Value.ToString()) + double.Parse(txtDiscPer.Value.ToString())) / 100;

                        }
                    }

                    dblDiscountLineTot = (dblGrossTotalF + dblServiceChgF) *
                        Convert.ToDouble(txtDiscPer.Value.ToString().Trim()) / 100;

                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;

                    //   txtDiscAmount.Value = dblTotalDiscount;

                    double _SubTotal = (double.Parse(txtSubValue.Value.ToString()) +
                            double.Parse(txtServicecharge.Value.ToString()));

                    if (cmbInvoiceType.Text != "Non VAT")
                    {
                        txtNBT.Value = (double.Parse(txtNBTPer.Value.ToString()) *
                           (_SubTotal - double.Parse(txtDiscAmount.Text)) / 100);

                        txtVat.Value = (double.Parse(txtVatPer.Value.ToString()) *
                            (_SubTotal - double.Parse(txtDiscAmount.Text) + double.Parse(txtNBT.Value.ToString()))
                            / 100);

                    }
                    else
                    {
                        txtNBT.Value = "0.00";
                        txtVat.Value = "0.00";
                    }
                    dblNBTF = Convert.ToDouble(txtNBT.Value.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVat.Value.ToString().Trim());

                    dblNetTotalF = (dblGrossTotalF + dblServiceChgF) - (dblTotalDiscount);
                    txtNetValue.Value = dblNetTotalF.ToString("N2");

                    //Asanga                                            
                    txtpaid.Text = dblNetTotalF.ToString("N2").Trim();

                }
            }
            catch { }
        }
        public void InvoiceCalculation_test(Int64 iInvoiceType)
        {
            try
            {
                if (IsFind != true)
                {
                    double dblSubTotal = 0;
                    double dblTotVAT = 0;
                    double dblDiscountedPrice = 0;

                    double dblPriceWithoutVat = 0;
                    double dblExcluisvePrice = 0;

                    double dbdiscountPer = 0;
                    double dblLinediscountAmount = 0;

                    double dblInclusiveLineTotal = 0;
                    double dblDiscountedLineTotal = 0;

                    double dblLineVAT = 0;
                    double dblLineNBT = 0;

                    double dblTotalVAT = 0;
                    double dblTotalNBT = 0;
                    double dblTotalLinedDiscount = 0;

                    double dblLineTax = 0;
                    double netvalue = 0;

                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = Convert.ToDouble(txtVatPer.Value);  //12%
                    double dblNBTPer = Convert.ToDouble(txtNBTPer.Value);  //2%

                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            //ug.ActiveCell.Row.Cells
                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;

                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;


                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);//here price in inclusive eg-114.24
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblDiscountedPrice = Math.Round(dblLinePrice - (dblLinePrice * dbdiscountPer), 3, MidpointRounding.AwayFromZero);
                            dblLinediscountAmount = Math.Round(dblLinePrice - dblDiscountedPrice, 3, MidpointRounding.AwayFromZero);

                            dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 3, MidpointRounding.AwayFromZero);//price without vat=102
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblLineVAT = Math.Round(((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty), 3, MidpointRounding.AwayFromZero);
                            dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 3, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            dblLineNBT = Math.Round(((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty), 3, MidpointRounding.AwayFromZero);

                            ugR.Cells["UnitPrice(Incl)"].Value = dblExcluisvePrice;//100

                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;


                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal;
                            ugR.Cells["TotalPrice"].Value = Math.Round(dblLineQty * dblLinePrice, 3, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = Math.Round((dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineTax, 3, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 3, MidpointRounding.AwayFromZero);

                            //Inv TAX Total                            
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 3, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 3, MidpointRounding.AwayFromZero);

                        }
                        txtSubValue.Value = dblSubTotal;
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //  txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = Math.Round((dblSubTotal), 2, MidpointRounding.AwayFromZero);

                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 4, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblDiscountedLineTotal = dblInclusiveLineTotal - dblLinediscountAmount;


                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;



                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 3, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 3, MidpointRounding.AwayFromZero);
                            ugR.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 3, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineTax, 3, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 3, MidpointRounding.AwayFromZero);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 3, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 3, MidpointRounding.AwayFromZero);

                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //  txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtSubValue.Value = dblSubTotal;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = (dblSubTotal);

                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;

                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;
                            ugR.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 3, MidpointRounding.AwayFromZero);

                            //VAT                            
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineVAT, 3, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 3, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 3, MidpointRounding.AwayFromZero);

                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        //  txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtSubValue.Value = dblSubTotal;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = (dblSubTotal);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void ultraGroupBox1_Click(object sender, EventArgs e)
        {

        }
        //Asanga
        private void rdobtnCreditCard_CheckedChanged(object sender, EventArgs e)
        {
            cmbCardType.Visible = false;
            lblSelectcard.Visible = false;
            grpamounttypes.Enabled = true;
            //if (rdobtnCreditCard.Checked == true)
            combMode.Value = 1;

        }
        //Asanga
        private void optCredit_CheckedChanged(object sender, EventArgs e)
        {
            cmbCardType.Visible = false;
            lblSelectcard.Visible = false;
            grpamounttypes.Enabled = false;
            //if (optCredit.Checked== true )
            combMode.Value = 2;

        }
        //Asanga
        private void optCash_CheckedChanged(object sender, EventArgs e)
        {
            cmbCardType.Visible = false;
            lblSelectcard.Visible = false;
            grpamounttypes.Enabled = false;
            //if (optCash.Checked== true )
            combMode.Value = 1;
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }
        //Asanga
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            for (int j = 0; j < dataGridView1.Rows.Count; j++)
            {
                if (dataGridView1.Rows[j].Cells["cblcname"].Value != null && dataGridView1.Rows[j].Cells["Amount"].Value == null)
                {
                    if (e.ColumnIndex == dataGridView1.Columns["Amount"].Index)
                    {
                        int i;
                        if (!int.TryParse(Convert.ToString(e.FormattedValue), out i))
                        {
                            e.Cancel = true;
                            MessageBox.Show("Amount Must be Numeric");
                        }
                    }
                }
            }

        }

        //Asanga
        private void SaveInvBalance(string InvoiceNo, double InvAmount, double InvPaid, double InvBalance, string Paytype, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "Insert into  tblInvoicePaymentHistory(InvoiceNo,InvAmount,InvPaid,InvBalance,PayType)" +
                    "VALUES ('" + InvoiceNo + "','" + InvAmount + "','" + InvPaid + "'," + InvBalance + ",'" + Paytype + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // objclsCommon.ErrorLog("Invoice ", ex.Message, sender.ToString(), ex.StackTrace);
                throw ex;
            }
        }
        //Asanga
        private void SaveCardDetailsWinlanka(string InvoiceNo, string CardType, string CardNo, double Amount, string InvType, string WH, string CusID, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "Insert into  tblInvoicePayTypes(InvoiceNo,CardType,CardNo,Amount,InvDate,UserID,InvType,WH,CusId,CurrentDate,PaymentMode)" +
                    "VALUES ('" + InvoiceNo + "','" + CardType + "','" + CardNo + "'," + Amount + ",'" + GetDateTime(dtpDate.Value) + "','" + user.userName.ToString().Trim() + "','" + InvType + "','" + WH + "','" + CusID + "','" + System.DateTime.Now.ToString("MM/dd/yyyy") + "','" + StrPaymmetM + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // objclsCommon.ErrorLog("Invoice ", ex.Message, sender.ToString(), ex.StackTrace);
                throw ex;
            }
        }

        private void SaveCardDetails(string InvoiceNo, string CardType, string CardNo, double Amount, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "Insert into  tblInvoicePayTypes(InvoiceNo,CardType,CardNo,Amount)" +
                    "VALUES ('" + InvoiceNo + "','" + CardType + "','" + CardNo + "'," + Amount + ")";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // objclsCommon.ErrorLog("Invoice ", ex.Message, sender.ToString(), ex.StackTrace);
                throw ex;
            }
        }
        //Asanga
        private void viewCardHistory(string StrInvoiceNo)
        {
            try
            {
                dataGridView1.Rows.Clear();
                string ConnString = ConnectionString;
                String S1 = "SELECT CardType,CardNo,Amount from  tblInvoicePayTypes  WHERE InvoiceNo='" + StrInvoiceNo + "'  and PaymentMode='CreditCard' ORDER BY CardType";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    dataGridView1.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    dataGridView1.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Asanga
        private void viewinvpayhistory(string StrInvoiceNo)
        {
            try
            {
                string StrSql = "SELECT InvPaid,InvBalance from  tblInvoicePaymentHistory  WHERE InvoiceNo='" + StrInvoiceNo + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtpaid.Text = Convert.ToDouble(dt.Rows[0]["InvPaid"]).ToString("N2").Trim();
                    txtbalance.Text = dt.Rows[0]["InvBalance"].ToString().Trim();
                    this.txtpaid.Focus();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Asanga
        public void PaycardCalculation()
        {
            try
            {
                double MultyAmount = 0;
                double NetAmount = 0;
                double Total = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    //Amount   
                    if (dataGridView1.Rows[i].Cells["Amount"].Value == null || dataGridView1.Rows[i].Cells["Amount"].Value.ToString().Trim() == string.Empty)
                        break;
                    Total += Convert.ToDouble(dataGridView1.Rows[i].Cells["Amount"].Value);
                }
                txtcardamount.Text = Total.ToString("0.00");
                txtpaid.Text = Total.ToString("0.00");

                if (txtcardamount.Text != null)
                {
                    MultyAmount = double.Parse(txtcardamount.Text.ToString());

                }
                if (txtNetValue.Text != null)
                {
                    NetAmount = double.Parse(txtNetValue.Text.ToString());
                }
                if (MultyAmount > NetAmount)
                {
                    MessageBox.Show("Invoice Amount & Multipayment Amount Not Match....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }
            catch { }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select a Warehouse First");
                    return;
                }

                if (Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a Quantity before trying to add serial numbers for Item ID '" +
                        ug.ActiveRow.Cells["Quantity"].Value.ToString() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            ug.ActiveRow.Cells["Quantity"].Selected = true;
                        }
                    }
                }
                else
                {
                    frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Invoice", cmbWarehouse.Text.ToString().Trim(),
                    ug.ActiveRow.Cells["ItemCode"].Value.ToString(),
                    Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()),
                    txtInvoiceNo.Text.Trim(), IsFind, clsSerializeItem.DtsSerialNoList, null, true, true);
                    ObjfrmSerialSubCommon.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtpaid_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //Asanga

        private void CalPaidAmt()
        {
            try
            {
                double dblpaidamount = 0;
                double dblInvoiceTotal = 0;
                double dblBalanceAmt = 0;

                dblpaidamount = double.Parse(txtpaidAmount.Text.ToString());
                dblInvoiceTotal = double.Parse(txtNetValue.Text.ToString());
                dblBalanceAmt = dblInvoiceTotal - dblpaidamount;
                txtBalanceAmt.Text = dblBalanceAmt.ToString("N2");
            }
            catch { }
        }
        private void txtpaid_TextChanged(object sender, EventArgs e)
        {
            try
            {
                float NetAm;
                float PaidAm;
                float BalAm;
                NetAm = 0;
                BalAm = 0;
                PaidAm = 0;
                NetAm = float.Parse(txtNetValue.Value.ToString());
                PaidAm = float.Parse(txtpaid.Text.ToString());
                BalAm = NetAm - PaidAm;
                txtbalance.Text = BalAm.ToString("0.00");
                // CalPaidAmt();
            }
            catch
            {

            }
        }
        //Asanga
        private void dataGridView1_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {
            PaycardCalculation();
        }
        //Asanga
        //private void loadDefaltOption()
        //{
        //    try
        //    {
        //        StrSql = "Select Tid,TAXID from tblTax_Default where Flg='PAY'";
        //        SqlCommand cmd = new SqlCommand(StrSql);
        //        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);
        //        if (dt.Rows.Count > 0)
        //        {
        //            if (dt.Rows[0]["TAXID"].ToString() == "Cash")
        //            {
        //                optCash.Checked = true; 
        //            }
        //            else if (dt.Rows[0]["TAXID"].ToString() == "Credit")
        //            {
        //                optCredit.Checked = true;
        //            }
        //            else if (dt.Rows[0]["TAXID"].ToString() == "Other")
        //            {
        //                rdobtnCreditCard.Checked = true;
        //            }

        //        }                

        //        StrSql1 = "Select Tid,TAXID from tblTax_Default where Flg='TAX'";
        //        SqlCommand cmd1 = new SqlCommand(StrSql1);
        //        SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
        //        DataTable dt1 = new DataTable();
        //        da1.Fill(dt1);
        //        if (dt1.Rows.Count > 0)
        //        {
        //            cmbInvoiceType.Value = dt1.Rows[0]["Tid"].ToString();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        private void loadDefaltOption()
        {
            try
            {
                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='PAY' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (bool.Parse(dt.Rows[0].ItemArray[2].ToString()) == true)
                        {
                            grpamounttypes.Enabled = false;
                        }
                        if (dt.Rows[0]["TAXID"].ToString() == "Cash")
                        {
                            optCash.Checked = true;
                        }
                        else if (dt.Rows[0]["TAXID"].ToString() == "Credit")
                        {
                            optCredit.Checked = true;
                        }
                        else if (dt.Rows[0]["TAXID"].ToString() == "Other")
                        {
                            rdobtnCreditCard.Checked = true;
                        }
                    }
                }

                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='TAX' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(StrSql1);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    cmbInvoiceType.Enabled = true;
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        if (IsEdit == false)
                        {
                            cmbInvoiceType.Value = dt1.Rows[i]["Tid"].ToString();
                        }
                        if (bool.Parse(dt1.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbInvoiceType.Enabled = false;
                        }
                    }
                }
                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='REP' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(StrSql1);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    cmbSalesRep.Enabled = true;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        if (IsEdit == false)
                        {
                            cmbSalesRep.Value = dt2.Rows[i]["Tid"].ToString();
                        }
                        if (bool.Parse(dt2.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbSalesRep.Enabled = false;
                        }
                    }
                }

                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='WEH' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd3 = new SqlCommand(StrSql1);
                SqlDataAdapter da3 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
                if (dt2.Rows.Count > 0)
                {
                    cmbWarehouse.Enabled = true;
                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        if (IsEdit == false)
                        {
                            cmbWarehouse.Value = dt3.Rows[i]["Tid"].ToString();
                        }

                        if (bool.Parse(dt3.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbWarehouse.Enabled = false;
                        }
                    }
                }

                //if (IsEdit == false)
                //{
                //    dsCustomer.Clear();
                //}
                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2,Pricing_Level FROM tblCustomerMaster where CutomerID='CASH'";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                //if (IsEdit == false)
                //{
                //    dAdapt.Fill(dsCustomer, "DtClient");
                //}
                DataTable dt10 = new DataTable();
                dAdapt.Fill(dt10);
                if (dt10.Rows.Count > 0)
                {
                    if (IsEdit == false)
                    {
                        cmbCustomer.Value = dt10.Rows[0]["CutomerID"].ToString();
                    }
                }


                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='INV' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd4 = new SqlCommand(StrSql1);
                SqlDataAdapter da4 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt4 = new DataTable();
                da4.Fill(dt4);
                if (dt4.Rows.Count > 0)
                {
                    combMode.Enabled = true;
                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        if (IsEdit == false)
                        {
                            combMode.Value = dt4.Rows[i]["Tid"].ToString();
                        }
                        if (bool.Parse(dt4.Rows[i]["locked"].ToString()) == true)
                        {
                            combMode.Enabled = false;
                        }
                        else
                        {
                            combMode.Enabled = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void txtpaid_Leave(object sender, EventArgs e)
        {

            string s = string.Format("{0:c}", txtpaid.Text);
            txtpaid.Text = s;
        }

        private void txtpaid_Enter(object sender, EventArgs e)
        {

        }

        private void ultraCombo1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void txtpaid_Click(object sender, EventArgs e)
        {

        }

        private void txtNetValue_ValueChanged(object sender, EventArgs e)
        {
            CalPaidAmt();
        }

        private void txtNetValue_Leave(object sender, EventArgs e)
        {

        }

        private void txtDiscLineTot_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txtNBT_ValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    int intGridRow = 0;
            //    double NewSubTotal = 0;
            //    for (intGridRow = 0; intGridRow < ug.Rows.Count; intGridRow++)
            //    {
            //        if (ug.Rows[intGridRow].Cells[8].Value != null)
            //        {
            //            NewSubTotal += double.Parse(ug.Rows[intGridRow].Cells[8].Value.ToString());
            //        }
            //    }
            //    double TotalDis = Convert.ToDouble(txtDiscAmount.Text.ToString());
            //    double TotalVat =  Convert.ToDouble(txtServicecharge.Text.ToString());
            //    txtNetValue.Text = ((NewSubTotal - TotalDis) + TotalVat).ToString("N2");
            //    txtpaid.Text = ((NewSubTotal - TotalDis) + TotalVat).ToString("N2");

            //}
            //catch
            //{

            //}
        }

        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtVat_ValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    int intGridRow = 0;
            //    double NewSubTotal = 0;
            //    for (intGridRow = 0; intGridRow < ug.Rows.Count; intGridRow++)
            //    {
            //        if (ug.Rows[intGridRow].Cells[8].Value != null)
            //        {
            //            NewSubTotal += double.Parse(ug.Rows[intGridRow].Cells[8].Value.ToString());
            //        }
            //    }
            //    double TotalDis = Convert.ToDouble(txtDiscAmount.Text.ToString());
            //    double TotalVat =  Convert.ToDouble(txtServicecharge.Text.ToString());
            //    txtNetValue.Text = ((NewSubTotal - TotalDis) + TotalVat).ToString("N2");
            //    txtpaid.Text = ((NewSubTotal - TotalDis) + TotalVat).ToString("N2");

            //}
            //catch
            //{

            //}
        }

        private void ug_ClickCell(object sender, ClickCellEventArgs e)
        {
            //foreach (UltraGridRow ugR in ug.Rows)
            try
            {
                if (ug.ActiveRow.Cells["Quantity"].Value != null || ug.ActiveRow.Cells["Quantity"].Value.ToString().Trim() != string.Empty)
                {
                    if (IsThisItemSerial(ug.ActiveRow.Cells["ItemCode"].Value.ToString()))
                        toolStripButton1.Enabled = true;
                    else
                        toolStripButton1.Enabled = false;
                }


                LoadLineDiscount(ug.ActiveRow.Cells["ItemCode"].Value.ToString());


            }
            catch
            {

            }



        }

        private void LoadLineDiscount(string v)
        {
            try
            {

                StrSql = "SELECT  convert(varchar(10),Discount) as Discount FROM tblItemMaster where ItemID ='" + v + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                string StrSql2 = " SELECT * FROM tblItemMaster where ItemID ='" + v + "' and Custom3 ='MAXXIES'";
                SqlCommand cmd2 = new SqlCommand(StrSql);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    dt.Rows.Add("40.00");
                    dt.Rows.Add("40+4");
                }


                if (dt.Rows.Count > 0)
                {
                    cmbLineDiscount.DataSource = dt;
                    cmbLineDiscount.ValueMember = "Discount";
                    cmbLineDiscount.DisplayMember = "Discount";

                    cmbLineDiscount.DisplayLayout.Bands[0].Columns[0].Width = 75;
                }
                else
                {
                    ug.ActiveCell.Row.Cells["Discount"].Value = "0.00";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbWarehouse_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void txtPatientContactNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtPatientName_TextChanged(object sender, EventArgs e)
        {


        }

        private void ug_AfterRowsDeleted_1(object sender, EventArgs e)
        {
            HideSelectedRow();
            FooterCalculation();
            CalPaidAmt();
        }

        public bool POSPrint = false;
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            POSPrint = true;
            DirectNormalPrint = true;
            try
            {
                IsWheelAlignemntDone = "No";
                //====================
                for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Categoty"].Value.ToString() == "WHEEL ALIGNMENT")
                    {
                        IsWheelAlignemntDone = "Yes";
                    }
                }

                Print(txtInvoiceNo.Text);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_Validating(object sender, CancelEventArgs e)
        {

        }

        private void ug_TextChanged(object sender, EventArgs e)
        {
            if (ug.ActiveCell.Column.Key == "FOC" && Convert.ToDouble(ug.ActiveCell.Row.Cells["UnitPrice"].Value) != 0.00)
            {

                if (Convert.ToBoolean(ug.ActiveCell.Row.Cells["FOC"].Value) == true)
                {
                    ug.ActiveCell.Row.Cells["UnitPrice"].Value = 0.00;

                }
                else
                {

                }

            }


        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void normalPrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POSPrint = false;
            DirectNormalPrint = false;


            try
            {

                IsWheelAlignemntDone = "No";
                //====================
                for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Categoty"].Value.ToString() == "WHEEL ALIGNMENT")
                    {
                        IsWheelAlignemntDone = "Yes";
                    }
                }
                Print(txtInvoiceNo.Text);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void pOSPrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POSPrint = true;
            DirectNormalPrint = false;
            try
            {

                IsWheelAlignemntDone = "No";
                //====================
                for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Categoty"].Value.ToString() == "WHEEL ALIGNMENT")
                    {
                        IsWheelAlignemntDone = "Yes";
                    }
                }
                Print(txtInvoiceNo.Text);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void ug_FilterCellValueChanged(object sender, FilterCellValueChangedEventArgs e)
        {

        }


        private void ug_CellChange(object sender, CellEventArgs e)
        {


        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {

        }

        private void txtDescription_Leave(object sender, EventArgs e)
        {

        }

        private void txtDescription_Leave_1(object sender, EventArgs e)
        {
            ug.Focus();
            if (ug.Enabled == true)
            {
                ug_Click(null, null);
            }
        }

        private void ug_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void grpPayment_Enter(object sender, EventArgs e)
        {

        }

        private void optCredit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                optCredit.Checked = true;
            }
        }

        private void optCash_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                optCash.Checked = true;
            }
        }

        private void rdobtnCreditCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rdobtnCreditCard.Checked = true;
            }
        }

        private void optSerialOne_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                optSerialOne.Checked = true;
            }
        }

        private void optSerialTwo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                optSerialTwo.Checked = true;
            }
        }

        private void rbtNoVat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rbtNoVat.Checked = true;
            }
        }

        private void rbtVAT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rbtVAT.Checked = true;
            }
        }

        private void rbtSVAT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rbtSVAT.Checked = true;
            }
        }

        private void cmbWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbWarehouse.Enabled == true)
                {
                    cmbWarehouse.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void cmbJobDone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbJobDone.Enabled == true)
                {
                    cmbJobDone.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void ultraCombo1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (ultraCombo1.Enabled == true)
                {
                    ultraCombo1.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void ultraCombo2_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmbLineDiscount_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void cmbLineDiscount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbLineDiscount.Enabled == true)
                {
                    cmbLineDiscount.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Class1.ItemSearchDesc = "";
            try
            {
                frmItemMasterList fl = new frmItemMasterList();
                fl.ShowDialog();

                if (Class1.ItemSearchDesc == "" || Class1.ItemSearchDesc == null)
                {
                    return;
                }
                if (ug.Enabled == false)
                {
                    return;
                }
                else
                {
                    if (ug.ActiveCell == null)
                        ug_Click(null, null);
                    ug.Focus();

                    ug.ActiveRow.Cells["Description"].Activated = true;
                    for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                    {
                        if (ug.Rows[intGrid].Cells["Description"].Value.ToString() == Class1.ItemSearchDesc)
                        {
                            return;
                        }
                    }


                    foreach (UltraGridRow ugR in ug.Rows)
                    {
                        if (IsGridExitCodeDes(Class1.ItemSearchDesc) == false)
                        {
                            return;
                        }
                    }
                    ug.ActiveCell.Row.Cells["Description"].Value = Class1.ItemSearchDesc;
                }
            }
            catch
            {

            }
        }

        private void btnVoid_Click(object sender, EventArgs e)
        {

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            myConnection.Open();
            myTrans = myConnection.BeginTransaction();
            try
            {
                UpdateQTY(txtInvoiceNo.Text.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);
                myTrans.Commit();
                MessageBox.Show("Successfully Voided");
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }

        private void UpdateQTY(string INVNo, string StrWarehouse, SqlConnection con, SqlTransaction Trans)
        {

            string s = "";
            StrSql = "SELECT ItemID,Qty,UnitCost,TotalCost,SellingPrice,WarehouseID FROM tbItemlActivity WHERE TranNo='" + INVNo + "'";

            SqlCommand command = new SqlCommand(StrSql, con, Trans);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                s = " UPDATE tblItemWhse SET QTY=QTY+'" + Convert.ToDouble(dt.Rows[i].ItemArray[1].ToString()) + "' WHERE WhseId='" + dt.Rows[i].ItemArray[5].ToString() + "' AND ItemId='" + dt.Rows[i].ItemArray[0].ToString() + "'";
                SqlCommand command2 = new SqlCommand(s, con, Trans);
                command2.CommandType = CommandType.Text;
                command2.ExecuteNonQuery();


                StrSql =
                    "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + dt.Rows[i].ItemArray[0].ToString() + "') " +
                   " INSERT INTO [tbItemlActivity](OHQTY,[DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES( @OHQTY,5,'" + INVNo + "Void" + "','" + GetDateTime(System.DateTime.Today) + "','CreditNote','true','" + dt.Rows[i].ItemArray[0].ToString() + "'," + Convert.ToDouble(dt.Rows[i].ItemArray[1].ToString()) + "," + Convert.ToDouble(dt.Rows[i].ItemArray[2].ToString()) + "," + Convert.ToDouble(dt.Rows[i].ItemArray[3].ToString()) + ",'" + StrWarehouse + "','" + Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString()) + "')";
                SqlCommand command3 = new SqlCommand(StrSql, con, Trans);
                command3.CommandType = CommandType.Text;
                command3.ExecuteNonQuery();
            }

            s = " UPDATE tblSalesInvoices SET IsVoid='" + true + "' where InvoiceNo='" + INVNo + "'";
            SqlCommand command4 = new SqlCommand(s, con, Trans);
            command4.CommandType = CommandType.Text;
            command4.ExecuteNonQuery();

        }

        private void ultraLabel2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Leave(object sender, EventArgs e)
        {
            if (txtDescription.Visible == true)
            {
                return;
            }
            ug.Focus();
            if (ug.Enabled == true)
            {
                ug_Click(null, null);
            }
        }

        private void txtDiscAmount_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                txtpaidAmount.Focus();
            }
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            frmCustomerHistory ch = new frmCustomerHistory();
            ch.Show();
        }

        private void txtVehicleNo_Leave(object sender, EventArgs e)
        {

        }

        private void txtMileage_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtVehicleNo.Text.ToString().Trim() != "")
                {
                    var StrSql = "SELECT [Mileage],Description,InvoiceNo,InvoiceDate FROM [tblSalesInvoices] WHERE [VehicleNo]='" + txtVehicleNo.Text.ToString() + "' and Description like '%" + "WHEEL ALIGNMENT" + "%' and Mileage = (Select max(Mileage) from tblSalesInvoices WHERE [VehicleNo]='" + txtVehicleNo.Text.ToString() + "')";

                    SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    dAdapt.Fill(dt);
                    if (dt.Rows.Count > 0 && dt.Rows[0].ItemArray[0].ToString() != "")
                    {
                        if ((Convert.ToDouble(txtMileage.Text.ToString().Trim()) - Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString())) < 3000)
                        {
                            MessageBox.Show("This Vehicle Already Done '" + dt.Rows[0].ItemArray[1].ToString() + "' Within 3000KM \n" + " Invoice No:- " + dt.Rows[0].ItemArray[2].ToString() + "\n InvoiceDate:- " + (Convert.ToDateTime(dt.Rows[0].ItemArray[3].ToString())).ToShortDateString() + "\n Mileage:- " + dt.Rows[0].ItemArray[0].ToString(), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                }
            }
            catch
            {

            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void lblSelectcard_Click(object sender, EventArgs e)
        {

        }

        private void chbWithDis_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void chkQtyWithZero_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkQtyWithZero.Checked == true)
            //{
            //    CheckBox = "0.00";
            //}
            //else
            //{
            //    CheckBox = "";
            //}
            //GetItemDataSet();
        }

        private async void btnSMS_Click(object sender, EventArgs e)
        {
            var smsManager = new SmsManager();
            var sms = new SendSmsDto("94766877828", "Thank you for being our valued customer. We are so greatful for the pleasure of serving you and hope we met your expectations - Team Lasantha Tyretraders");
            await smsManager.SendSms(sms);
            MessageBox.Show("Sucessfull");




        }

        private void grpSerial_Enter(object sender, EventArgs e)
        {

        }

        private void combMode_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbInvoiceType_Click(object sender, EventArgs e)
        {

        }

        private void cmbInvoiceType_MouseLeave(object sender, EventArgs e)
        {

        }

        private void combMode_Validated(object sender, EventArgs e)
        {

        }

        private void combMode_Validating(object sender, CancelEventArgs e)
        {
            if (Convert.ToInt32(combMode.Value) == 2)
            {
                optCredit.Checked = true;
            }
            else
            {
                optCash.Checked = true;
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtbalance_TextChanged(object sender, EventArgs e)
        {

        }

        private void ug_AfterRowsDeleted(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtpaidAmount_TextChanged(object sender, EventArgs e)
        {
            CalPaidAmt();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            IsEdit = true;
            btnEditer.Enabled = false;
            ug.Enabled = true;
            btnConfirm.Enabled = false;
            btnSave.Enabled = true;

            EnableHeader(true);
            EnableFoter(true);

            //loadDefaltOption();

            SetReadOnly(false);
            if (rbtVAT.Checked == true || rbtSVAT.Checked == true)
            {
                txtNBTPer.Enabled = true;
                txtVatPer.Enabled = true;
            }
            try
            {

                GetJobDoneby();
                GetItemDataSet();
                HideSelectedRow();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            txtServicecharge.Enabled = true;
            txtDiscPer.Enabled = true;

        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {

                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(dtpDate.Value)))
                    return;

                if (dtpDate.Value < user.Period_begin_Date)
                {
                    MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (dtpDate.Value > user.Period_End_Date)
                {
                    MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {

                    ConfirmEvent();



                }


            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtVehicleNo_TextChanged(object sender, EventArgs e)
        {
            if (txtVehicleNo.Text != "")
            {
                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                SqlCommand myCommand = new SqlCommand("select CustomerName,ContactNo from tblSalesInvoices where VehicleNo='" + txtVehicleNo.Text.Trim() + "'", myConnection);
                SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                DataTable dt41 = new DataTable();
                da41.Fill(dt41);

                if (dt41.Rows.Count > 0)
                {
                    TxtPatientName.Text = dt41.Rows[0].ItemArray[0].ToString();
                    txtPatientContactNo.Text = dt41.Rows[0].ItemArray[1].ToString();
                }
                else
                {
                    TxtPatientName.Text = "";
                    txtPatientContactNo.Text = "";
                }
            }

        }

        private void ultraCombo1_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {

                StrSql = "SELECT  convert(varchar(10),Discount) as Discount FROM tblItemMaster where ItemID ='" + ultraCombo1.Text + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                string StrSql2 = " SELECT * FROM tblItemMaster where ItemID ='" + ultraCombo1.Text + "' and Custom3 ='MAXXIES'";
                SqlCommand cmd2 = new SqlCommand(StrSql);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    dt.Rows.Add("40.00");
                    dt.Rows.Add("40+4");
                }


                if (dt.Rows.Count > 0)
                {
                    cmbLineDiscount.DataSource = dt;
                    cmbLineDiscount.ValueMember = "Discount";
                    cmbLineDiscount.DisplayMember = "Discount";

                    cmbLineDiscount.DisplayLayout.Bands[0].Columns[0].Width = 75;
                }
                else
                {
                    //  ug.ActiveCell.Row.Cells["Discount"].Value = "0.00";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void ultraCombo2_RowSelected(object sender, RowSelectedEventArgs e)
        {


            try
            {

                StrSql = " SELECT convert(varchar(10),Discount) as Discount FROM tblItemMaster where ItemDescription ='" + ultraCombo2.Text + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                string StrSql2 = " SELECT * FROM tblItemMaster where ItemDescription ='" + ultraCombo2.Text + "' and Custom3 ='MAXXIES'";
                SqlCommand cmd2 = new SqlCommand(StrSql);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    dt.Rows.Add("40.00");
                    dt.Rows.Add("40+4");
                }


                if (dt.Rows.Count > 0)
                {
                    cmbLineDiscount.DataSource = dt;
                    cmbLineDiscount.ValueMember = "Discount";
                    cmbLineDiscount.DisplayMember = "Discount";

                    cmbLineDiscount.DisplayLayout.Bands[0].Columns[0].Width = 75;
                }

                else
                {
                    //  ug.ActiveCell.Row.Cells["Discount"].Value = "0.00";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}