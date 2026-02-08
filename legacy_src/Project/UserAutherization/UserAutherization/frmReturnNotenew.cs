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
using System.Collections;
using System.Threading;


namespace UserAutherization
{
    public partial class frmReturnNotenew : Form
    {
        clsCommon objclsCommon = new clsCommon();
        public DSEstimate DsEst = new DSEstimate();
        bool run;
        bool add;
        bool edit;
        bool delete;
        DataTable dtUser = new DataTable();

        public string sMsg = "Peachtree - Direct Return";
        public static string LineDisitemid, LineDisitemdescription, LineDisGLAccount, SpecialDisItemid, SpecialDisItemdescription, SpecialDisGLAccount, Cashitemid, cashitemdis, cashGL, NBitemid, NBTitemDis, NBTitemGL, VATitemid, VATitemDis, VATGL, SERID, SERDIS, SERGL;
        public static string vatpr, nbtpr, vat, nbt;
        public string StrSql;
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
        public static string ConnectionString;
        int intEstomateProcode;
        public Boolean blnEdit;
        public int intProcess;
        public string StrPaymmetM;
        public string StrARAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;

        public DataSet dsWarehouse;
        public DataSet dsReturn;
        public DsItemWiseSales DsItemWise = new DsItemWiseSales();
        public DSCustomerReturn DsCustomerReturn = new DSCustomerReturn();
        public static DateTime UserWiseDate = System.DateTime.Now;
        DataSet dsCustomer;
        DataSet dsSalesRep;
        DataSet ds;
        bool IsFind = false;
        public string Tax1ID = "";
        public double Tax1Rate = 0.0;
        public string Tax1Name = "";
        public double Tax1Amount = 0.0;
        public string Tax1GLAccount = "";
        public string Tax2ID = "";
        public double Tax2Rate = 0.0;
        public string Tax2Name = "";
        public double Tax2Amount = 0.0;
        public string Tax2GLAccount = "";

        public frmReturnNotenew()
        {
            InitializeComponent();
            setConnectionString();
            IsFind = false;
        }

        public frmReturnNotenew(string intNo)
        {
            try
            {
                InitializeComponent();
                setConnectionString();
                IsFind = true;
                setValue();
            }
            catch (Exception ex)
            {
                throw ex;
                //objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public frmReturnNotenew(int intNo)
        {
            try
            {
                InitializeComponent();
                setConnectionString();

                if (intNo == 0)
                {
                    intEstomateProcode = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void setValue()
        {
            try
            {
                string strNo = (Search.searchIssueNoteNo);

                if (strNo == "")
                {
                    strNo = "";
                }
                else
                {
                    ClearHeader();
                    DeleteRows();

                    EnableHeader(true);
                    EnableFoter(true);
                    txtCreditNo.Text = strNo;
                    //ViewHeader(strNo);
                    //ViewDetails(strNo);
                    EnableHeader(false);
                    EnableFoter(false);
                    GrandTotal();
                }
            }
            catch { }
        }   
        public void InvoiceCalculation(Int64 iInvoiceType)
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

                    //double dblTotNBT = 0;
                    //double dblInvTotal = 0;

                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = 0;
                    dblVATPer = Convert.ToDouble(txtVatPer.Text);  //12%
                    double dblNBTPer = 0;
                    dblNBTPer = Convert.ToDouble(txtNBTPer.Text);  //2%
                    double TotalAmount = 0;


                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (UltraGridRow dgvr in ug.Rows)
                        {
                            //ug.ActiveCell.Row.Cells

                            if (dgvr.Cells["ItemCode"].Value == null || dgvr.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString().Trim() == string.Empty) dgvr.Cells["DiscLineAmt"].Value = 0.00;
                            if (dgvr.Cells["UnitPrice"].Value == null || dgvr.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Disc %"].Value = 0.00;
                            if (dgvr.Cells["Quantity"].Value == null || dgvr.Cells["Quantity"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Quantity"].Value = 0.00;
                            if (dgvr.Cells["UnitPrice(Incl)"].Value == null || dgvr.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["TotalPrice(Incl)"].Value == null || dgvr.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["LineTax"].Value == null || dgvr.Cells["LineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineTax"].Value = 0.00;

                            dblLinePrice = Convert.ToDouble(dgvr.Cells["UnitPrice"].Value);//here price in inclusive eg-114.24
                            dbdiscountPer = Convert.ToDouble(dgvr.Cells["Disc %"].Value) / 100;
                            dblDiscountedPrice = Math.Round(dblLinePrice - (dblLinePrice * dbdiscountPer), 2, MidpointRounding.AwayFromZero);
                            dblLinediscountAmount = Math.Round(dblLinePrice - dblDiscountedPrice, 2, MidpointRounding.AwayFromZero);

                            //dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 2, MidpointRounding.AwayFromZero);//price without vat=102
                            dblPriceWithoutVat = ((dblDiscountedPrice * 100) / (100 + dblVATPer));//price without vat=102
                            dblLineQty = Convert.ToDouble(dgvr.Cells["Quantity"].Value);

                            dblLineVAT = Math.Round(((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            //dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            dblExcluisvePrice = ((dblPriceWithoutVat * 100) / (100 + dblNBTPer));//exclusive price here eg 100
                            dblLineNBT = Math.Round(((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dgvr.Cells["UnitPrice(Incl)"].Value = dblExcluisvePrice;//100
                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;
                            dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal;
                            dgvr.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice)- ((dblLineQty * dblLinePrice)* dbdiscountPer), 2, MidpointRounding.AwayFromZero);

                            dgvr.Cells["DiscLineAmt"].Value = dblLinediscountAmount;
                            //VAT
                            dblLineTax = Math.Round((dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);

                            //dblLineTax = Math.Round(Convert.ToDouble(dgvr.Cells["TotalPrice"].Value) - Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                            //txtGridTotalExcl.Text = TotalAmount.ToString();

                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();
                        }
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetValue.Text = (Math.Round((dblSubTotal + dblTotalVAT + dblTotalNBT), 2, MidpointRounding.AwayFromZero)).ToString();
                        txtpaid.Text = txtNetValue.Text.ToString();

                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (UltraGridRow dgvr in ug.Rows)
                        {

                            if (dgvr.Cells["UnitPrice"].Value == null || dgvr.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Disc %"].Value = 0.00;
                            if (dgvr.Cells["Quantity"].Value == null || dgvr.Cells["Quantity"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Quantity"].Value = 0.00;
                            if (dgvr.Cells["UnitPrice(Incl)"].Value == null || dgvr.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["TotalPrice(Incl)"].Value == null || dgvr.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["LineTax"].Value == null || dgvr.Cells["LineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineTax"].Value = 0.00;
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString().Trim() == string.Empty) dgvr.Cells["DiscLineAmt"].Value = 0.00;


                            if (dgvr.Cells["ItemCode"].Value == null || dgvr.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["Quantity"].Value);

                            //dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dblInclusiveLineTotal = (dblLinePrice * dblLineQty);
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString().Trim() == string.Empty)
                                dbdiscountPer = 0;
                            else
                                dbdiscountPer = Convert.ToDouble(dgvr.Cells["Disc %"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblDiscountedLineTotal = dblInclusiveLineTotal - dblLinediscountAmount;

                            dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            dgvr.Cells["DiscLineAmt"].Value = dblLinediscountAmount;

                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);

                            //dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            //dbdiscountPer = Convert.ToDouble(dgvr.Cells["Disc %"].Value) / 100;
                            //dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            //dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            dgvr.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice) - ((dblLineQty * dblLinePrice) * dbdiscountPer), 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            //dblLineTax = Math.Round(Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value) - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value), 3, MidpointRounding.AwayFromZero);

                            dgvr.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();
                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetValue.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
                        txtpaid.Text = txtNetValue.Text.ToString();

                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (UltraGridRow dgvr in ug.Rows)
                        {
                            if (dgvr.Cells["UnitPrice"].Value == null || dgvr.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Disc %"].Value = 0.00;
                            if (dgvr.Cells["Quantity"].Value == null || dgvr.Cells["Quantity"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Quantity"].Value = 0.00;
                            if (dgvr.Cells["UnitPrice(Incl)"].Value == null || dgvr.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["TotalPrice(Incl)"].Value == null || dgvr.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) dgvr.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (dgvr.Cells["LineTax"].Value == null || dgvr.Cells["LineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["LineTax"].Value = 0.00;
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString().Trim() == string.Empty) dgvr.Cells["DiscLineAmt"].Value = 0.00;

                            if (dgvr.Cells["ItemCode"].Value == null || dgvr.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(dgvr.Cells["Disc %"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;

                            dgvr.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            // dgvr.Cells["TotalPrice(Incl)"].Value = Math.Round(((dblLineQty * dblLinePrice)), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["DiscLineAmt"].Value = dblLinediscountAmount;
                            dgvr.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice) - ((dblLineQty * dblLinePrice) * dbdiscountPer), 2, MidpointRounding.AwayFromZero);

                            // dblLineVAT = Math.Round(Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value) - Convert.ToDouble(dgvr.Cells["TotalPrice"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["LineTax"].Value = Math.Round(dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            // dblTotVAT = Math.Round(dblTotVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            //dgvr.Activated = true;
                            //ug.PerformAction(UltraGridAction.CommitRow);
                            //ug.PerformAction(UltraGridAction.ExitEditMode);  
                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["TotalPrice"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();
                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetValue.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
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



                    dblGrossTotalF = Convert.ToDouble(txtSubValue.Text.ToString().Trim());
                    dblServiceChgF = Convert.ToDouble(txtServicecharge.Text.ToString().Trim());
                    dblDiscountF = (dblGrossTotalF) * Convert.ToDouble(txtDiscPer.Text.ToString().Trim()) / 100;
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;

                    txtDiscAmount.Text = (dblTotalDiscount +
                        (double.Parse(txtServicecharge.Text.ToString()) * double.Parse(txtDiscPer.Text.ToString()) / 100)).ToString();

                    double _AmountWithoutDisc = double.Parse(txtSubValue.Text.ToString()) + //102
                        double.Parse(txtDiscLineTot.Text) -
                        double.Parse(txtDiscAmount.Text.ToString()) +
                        double.Parse(txtServicecharge.Text.ToString());

                    if (cmbInvoiceType.Value.ToString() != "3")
                    {
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

                            txtVat.Value = Convert.ToDouble((_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) *
                            double.Parse(txtVatPer.Value.ToString()) / 100).ToString("0.00");

                            //txtVat.Text = ((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                            //  double.Parse(txtVatPer.Text.ToString()) / 100).ToString();
                        }

                    }

                    txtNBT.Text = (_AmountWithoutDisc * double.Parse(txtNBTPer.Text.ToString()) / 100).ToString();
                    txtVat.Text = ((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                        double.Parse(txtVatPer.Text.ToString()) / 100).ToString();
                    dblNBTF = Convert.ToDouble(txtNBT.Text.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVat.Text.ToString().Trim());

                    dblNetTotalF = Math.Round(_AmountWithoutDisc + dblNBTF + dblVATF, 2, MidpointRounding.AwayFromZero);// +double.Parse(txtDiscAmount.Value.ToString());

                    txtNetValue.Text = dblNetTotalF.ToString("N2"); //127.99
                    //Asanga
                    txtpaid.Text = dblNetTotalF.ToString("N2");

                    if (cmbInvoiceType.Value.ToString() == "1")
                        txtDiscAmount.Text = ((Convert.ToDouble(txtGridTotalExcl.Text) -
                            Convert.ToDouble(txtDiscLineTot.Text)) *
                            Convert.ToDouble(txtDiscPer.Text) / 100).ToString();

                    else
                        txtDiscAmount.Text = (Convert.ToDouble(txtSubValue.Text.Trim()) * Convert.ToDouble(txtDiscPer.Text) / 100).ToString();
                    txtNBT.Text = Convert.ToDouble(txtNBT.Text.ToString().Trim()).ToString("0.00"); //txtNBT.Text.ToString("N2");
                    txtVat.Text = Convert.ToDouble(txtVat.Text.ToString().Trim()).ToString("0.00");  //txtVat.Text.ToString("N2");

                   

                }
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
            catch (Exception ex) { throw ex; }
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
                StrSql = "SELECT TrnPref, TrnPad, TrnNum FROM tblDefualtSetting";
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
                int intCRNNo;
                SqlCommand command;

                StrSql = "SELECT  TOP 1(CRNNum) FROM tblDefualtSetting ORDER BY CRNNum DESC";

                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intCRNNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                }
                else
                {
                    intCRNNo = 1;
                }
                StrSql = "UPDATE tblDefualtSetting SET CRNNum='" + intCRNNo + "'";

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
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

                StrSql = "SELECT CRNPref, CRNPad, CRNNum FROM tblDefualtSetting";

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
                throw ex;
            }
        }

        public void GetCRNNo()
        {
            try
            {
                if (user.IsCRTNNoAutoGen)
                {
                    Int32 intX;
                    Int32 intZ;
                    string StrInvNo;
                    Int32 intP;
                    Int32 intI;
                    String StrInV;

                    StrSql = "SELECT CRNPref, CRNPad, CRNNum FROM tblDefualtSetting";

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

                        txtCreditNo.Text = StrInvNo + StrInV.Substring(1, intX);
                    }
                }
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

        public void GetItemDataSet()
        {
            try
            {
                if (cmbWarehouse.Text == "")
                {
                    return;
                }

                StrSql = "SELECT                 tblItemWhse.ItemId,    tblItemWhse.ItemDis,        tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";
                StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)";

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
                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[3].Hidden = true;

                    ultraCombo1.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    ultraCombo1.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Width = 200;
                }
                else
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetCustomer()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2 FROM tblCustomerMaster order by CutomerID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");

                cmbCustomer.DataSource = dsCustomer.Tables["DtClient"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CutomerID";

                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address1"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address2"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CutomerID"].Width = 100;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CustomerName"].Width = 150;
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
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    ugR.Delete(false);
                }
                GrandTotal();
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

        private void GrandTotal()
        {
            try
            {
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();   

                //CalculateGridAmounts();
                //CalculateTaxAmounts();
                //dblGrandTot = 0;
                //dblGrocessAmt = 0;
                //dblSubTot = 0;
                //dblVatAmount = 0;
                //dblNbtAmount = 0;

                //int intGridRow;

                //for (intGridRow = 0; intGridRow < ug.Rows.Count; intGridRow++)
                //{
                //    dblSubTot += double.Parse(ug.Rows[intGridRow].Cells[5].Value.ToString());
                //}
                //dblDiscPer = double.Parse(txtDiscPer.Value.ToString());
                //if (double.Parse(txtDiscPer.Value.ToString()) > 0)
                //{
                //    dblDiscAmt = (dblSubTot * dblDiscPer) / 100;
                //}
                //else
                //{
                //    dblDiscAmt = 0;
                //}

                //dblGrocessAmt = dblSubTot - dblDiscAmt;

                //if (double.Parse(txtNBTPer.Value.ToString()) > 0)
                //{
                //    dblNbtAmount = ((dblGrocessAmt * double.Parse(txtNBTPer.Value.ToString())) / 100);
                //}
                //else
                //{
                //    dblNbtAmount = 0;
                //}
                //if (double.Parse(txtVatPer.Value.ToString()) > 0)
                //{
                //    dblVatAmount = (((dblGrocessAmt + dblNbtAmount) * double.Parse(txtVatPer.Value.ToString())) / 100);
                //}
                //else
                //{
                //    dblVatAmount = 0;
                //}

                //dblNetAmount = dblGrocessAmt + dblNbtAmount + dblVatAmount;

                //txtSubValue.Value = dblSubTot;
                //txtDiscAmount.Value = dblDiscAmt;
                //txtGrossValue.Value = dblGrocessAmt;
                //txtNBT.Value = dblNbtAmount;
                //txtVat.Value = dblVatAmount;
                //txtNetValue.Value = dblNetAmount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CalculateGridAmounts()
        {
            double _UnitPrice = 0;
            double _UnitPriceIncl = 0;
            double _LineTax = 0;
            double _DiscountP = 0;
            double _LineDiscount = 0;
           // double _AmountIncl = 0;
            double _Amount = 0;
            double _AmountAfterDisc = 0;
            double _TotalAmountIncl = 0;
            double _TotalAmount = 0;
            double _Qty = 0;
            double _TempTax1 = 0;
            double _TempTax2 = 0;
            double _DiscountTotal = 0;
            double _Tax1Total = 0;
            double _Tax2Total = 0;
            double _servCharg = 0;
            double _servChargTax1 = 0;
            double _servChargTax2 = 0;
            double _TempTaxRate = 0;
            double _footerDiscP = 0;

            try
            {
                Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
                Tax2Rate = double.Parse(txtVatPer.Text.Trim());
                _footerDiscP = double.Parse(txtDiscPer.Text.Trim());

                if (rbtExclusive.Checked || (ClassDriiDown.IsInvSerch == true))
                {
                    foreach (UltraGridRow dgvr in ug.Rows)
                    {
                        if (dgvr.Cells["ItemCode"].Value != null && dgvr.Cells["ItemCode"].Value.ToString() != string.Empty)
                        {
                            if (dgvr.Cells["Quantity"].Value == null || dgvr.Cells["Quantity"].Value.ToString() == string.Empty) dgvr.Cells["Quantity"].Value = "0";
                            if (dgvr.Cells["UnitPrice"].Value == null || dgvr.Cells["UnitPrice"].Value.ToString() == string.Empty) dgvr.Cells["UnitPrice"].Value = "0";
                            if (dgvr.Cells["UnitPrice(Incl)"].Value == null || dgvr.Cells["UnitPrice(Incl)"].Value.ToString() == string.Empty) dgvr.Cells["UnitPrice(Incl)"].Value = "0";
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString() == string.Empty) dgvr.Cells["Disc %"].Value = "0";
                            if (dgvr.Cells["TotalPrice"].Value == null || dgvr.Cells["TotalPrice"].Value.ToString() == string.Empty) dgvr.Cells["TotalPrice"].Value = "0";
                            if (dgvr.Cells["DiscLineAmt"].Value == null || dgvr.Cells["DiscLineAmt"].Value.ToString() == string.Empty) dgvr.Cells["DiscLineAmt"].Value = "0";
                            if (dgvr.Cells["TotalPrice(Incl)"].Value == null || dgvr.Cells["TotalPrice(Incl)"].Value.ToString() == string.Empty) dgvr.Cells["TotalPrice(Incl)"].Value = "0";
                            if (dgvr.Cells["LineTax"].Value == null || dgvr.Cells["LineTax"].Value.ToString() == string.Empty) dgvr.Cells["LineTax"].Value = "0";

                            _Qty = double.Parse(dgvr.Cells["Quantity"].Value.ToString());
                            _UnitPrice = double.Parse(dgvr.Cells["UnitPrice"].Value.ToString());
                            _DiscountP = double.Parse(dgvr.Cells["Disc %"].Value.ToString());
                            _LineDiscount = _UnitPrice * _Qty * _DiscountP / 100;

                            //Incl Unit Price without Discount
                            //_Amount = (_UnitPrice * _Qty);
                            _TempTax1 = _UnitPrice * Tax1Rate / 100;
                            _TempTax2 = (_UnitPrice + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;
                            dgvr.Cells["UnitPrice(Incl)"].Value = (_UnitPrice + _LineTax).ToString("0.00");


                            //Incl Unit Price with Discount
                            _Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            _TempTax1 = _Amount * Tax1Rate / 100;
                            _TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;

                            //dgvr.Cells["UnitPrice(Incl)"].Value = (_UnitPrice + _LineTax).ToString("0.00");
                            dgvr.Cells["DiscLineAmt"].Value = _LineDiscount.ToString("0.00");
                            dgvr.Cells["TotalPrice"].Value = (_Amount).ToString("0.00");
                            dgvr.Cells["LineTax"].Value = (_LineTax).ToString("0.00");
                            dgvr.Cells["TotalPrice(Incl)"].Value = (_Amount + _LineTax).ToString("0.00");

                            //after total discount
                            //_LineDiscount = _UnitPrice * _Qty * (_DiscountP+double.Parse(txtPersntage.Text.Trim())) / 100;
                            //_Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            //_TempTax1 = _Amount * Tax1Rate / 100;
                            //_TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            //_LineTax = _TempTax1 + _TempTax2;

                            _Tax1Total = _Tax1Total + _TempTax1;
                            _Tax2Total = _Tax2Total + _TempTax2;

                            _TotalAmount = _TotalAmount + _Amount;


                            //_TotalAmountIncl = _TotalAmountIncl + _Amount + _TempTax1 + _TempTax1;
                        }
                    }
                }

                else if (rbtInclusive.Checked)
                {
                    foreach (UltraGridRow dgvr in ug.Rows)
                    {
                        if (dgvr.Cells["ItemCode"].Value != null && dgvr.Cells["ItemCode"].Value.ToString() != string.Empty)
                        {
                            if (dgvr.Cells["Quantity"].Value == null || dgvr.Cells["Quantity"].Value.ToString() == string.Empty) dgvr.Cells["Quantity"].Value = "0";
                            if (dgvr.Cells["UnitPrice"].Value == null || dgvr.Cells["UnitPrice"].Value.ToString() == string.Empty) dgvr.Cells["UnitPrice"].Value = "0";
                            if (dgvr.Cells["UnitPrice(Incl)"].Value == null || dgvr.Cells["UnitPrice(Incl)"].Value.ToString() == string.Empty) dgvr.Cells["UnitPrice(Incl)"].Value = "0";
                            if (dgvr.Cells["Disc %"].Value == null || dgvr.Cells["Disc %"].Value.ToString() == string.Empty) dgvr.Cells["Disc %"].Value = "0";
                            if (dgvr.Cells["TotalPrice"].Value == null || dgvr.Cells["TotalPrice"].Value.ToString() == string.Empty) dgvr.Cells["TotalPrice"].Value = "0";
                            if (dgvr.Cells["DiscLineAmt"].Value == null || dgvr.Cells["DiscLineAmt"].Value.ToString() == string.Empty) dgvr.Cells["DiscLineAmt"].Value = "0";
                            if (dgvr.Cells["TotalPrice(Incl)"].Value == null || dgvr.Cells["TotalPrice(Incl)"].Value.ToString() == string.Empty) dgvr.Cells["TotalPrice(Incl)"].Value = "0";
                            if (dgvr.Cells["LineTax"].Value == null || dgvr.Cells["LineTax"].Value.ToString() == string.Empty) dgvr.Cells["LineTax"].Value = "0";

                            _TempTaxRate = Temp_getTaxRate();

                            _Qty = double.Parse(dgvr.Cells["Quantity"].Value.ToString());
                            _UnitPriceIncl = double.Parse(dgvr.Cells["UnitPrice(Incl)"].Value.ToString());
                            _DiscountP = double.Parse(dgvr.Cells["Disc %"].Value.ToString());
                            _UnitPrice = _UnitPriceIncl * 100 / (100 + _TempTaxRate);

                            _DiscountP = double.Parse(dgvr.Cells["Disc %"].Value.ToString());
                            _LineDiscount = _UnitPrice * _Qty * _DiscountP / 100;
                            _Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            _TempTax1 = _Amount * Tax1Rate / 100;
                            _TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;

                            dgvr.Cells["UnitPrice"].Value = (_UnitPrice).ToString("0.00");
                            dgvr.Cells["DiscLineAmt"].Value = _LineDiscount.ToString("0.00");
                            dgvr.Cells["TotalPrice"].Value = (_Amount).ToString("0.00");
                            dgvr.Cells["LineTax"].Value = (_LineTax).ToString("0.00");
                            dgvr.Cells["TotalPrice(Incl)"].Value = (_Amount + _LineTax).ToString("0.00");

                            //after total discount
                            //_LineDiscount = _UnitPrice * _Qty * (_DiscountP + double.Parse(txtPersntage.Text.Trim())) / 100;
                            //_Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            //_TempTax1 = _Amount * Tax1Rate / 100;
                            //_TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            //_LineTax = _TempTax1 + _TempTax2;

                            _Tax1Total = _Tax1Total + _TempTax1;
                            _Tax2Total = _Tax2Total + _TempTax2;

                            _TotalAmount = _TotalAmount + _Amount;
                            //_TotalAmountIncl = _Amount + _TotalAmountIncl + _Tax1Total + _Tax2Total;
                        }
                    }
                }

                txtSubValue.Text = _TotalAmount.ToString("0.00");
                txtGridTotalIncl.Text = (_TotalAmount + _Tax1Total + _Tax2Total).ToString("0.00");

                if (txtServicecharge.Text.Trim() == null || txtServicecharge.Text.Trim() == string.Empty)
                    txtServicecharge.Text = "0.00";

                _servCharg = double.Parse(txtServicecharge.Text.Trim());
                _servChargTax1 = _servCharg * Tax1Rate / 100;
                _servChargTax2 = (_servChargTax1 + _servCharg) * Tax2Rate / 100;

                txtNBT.Text = (_Tax1Total + _servChargTax1).ToString("0.00");
                txtVat.Text = (_Tax2Total + _servChargTax2).ToString("0.00");

                //txtNetTotal.Text = (_TotalAmount + _servCharg - double.Parse(txtValueDiscount.Text.Trim()) - (double.Parse(txtDiscLineTot.Text.Trim())) + (_Tax1Total + _servChargTax1) + (_Tax2Total + _servChargTax2)).ToString("0.00");

                txtNetValue.Text = (_TotalAmount + _servCharg - double.Parse(txtDiscAmount.Text.Trim()) - (double.Parse(txtDiscLineTot.Text.Trim())) + (_Tax1Total + _servChargTax1) + (_Tax2Total + _servChargTax2)).ToString("0.00");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private double Temp_getTaxRate()
        {
            double _TempRate = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            try
            {
                _Tax1 = Tax1Rate;
                _Tax2 = (Tax1Rate + 100) * Tax2Rate / 100;

                _TempRate = _Tax2 + _Tax1;

                //_Rate=100+(100*Tax1Rate/100)+(
                return _TempRate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CalculateTaxAmounts()
        {
            double _subTot = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
            Tax2Rate = double.Parse(txtVatPer.Text.Trim());

            if (txtDiscAmount.Text.Trim() == string.Empty || txtDiscAmount.Text == null) txtDiscAmount.Text = "0.00";

            _subTot = double.Parse(txtSubValue.Text.Trim()) + double.Parse(txtServicecharge.Text.Trim());

            txtDiscAmount.Text = ((_subTot * double.Parse(txtDiscPer.Text.Trim())) / 100).ToString("0.00");

            _subTot = _subTot - double.Parse(txtDiscAmount.Text.Trim()) - double.Parse(txtDiscLineTot.Text.Trim());
            //_subTot = _subTot - double.Parse(txtValueDiscount.Text.Trim()) - double.Parse(txtDiscLineTot.Text.Trim());

            _Tax1 = _subTot * Tax1Rate / 100;
            _Tax2 = (_Tax1 + _subTot) * Tax2Rate / 100;

            txtNBT.Text = _Tax1.ToString("0.00");
            txtVat.Text = _Tax2.ToString("0.00");

            txtNetValue.Text = (_subTot + _Tax1 + _Tax2).ToString("0.00");
        }

        private void ug_KeyDown(object sender, KeyEventArgs e)
            {
            try
            {
                switch (e.KeyValue)
                {
                    case 37:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ug.PerformAction(UltraGridAction.PrevCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 38:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ug.PerformAction(UltraGridAction.AboveCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 39:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ug.PerformAction(UltraGridAction.NextCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 40:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ug.PerformAction(UltraGridAction.BelowCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }

                    case 9:
                        {
                            if (ug.ActiveCell.Column.Key == "Disc %")
                            {
                                if (ug.ActiveRow.HasNextSibling() == false)
                                {
                                    if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                                    {
                                        UltraGridRow ugR;
                                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                        ugR.Cells["LineNo"].Selected = true;
                                        ugR.Cells["LineNo"].Activated = true;
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double LineCalculation(double UnitCost, double Quantity)
        {
            try
            {
                dblLineTot = 0;
                double lineTotal = 0;
                dblLineTot = UnitCost * Quantity;
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public Boolean IsGridValidation()
        //{
        //    try
        //    {
        //        if (ug.Rows.Count == 0)
        //        {
        //            return false;
        //        }

        //        foreach (UltraGridRow ugR in ug.Rows)
        //        {
        //            if (IsGridExitCode(ugR.Cells["ItemCode"].Text) == false)
        //            {
        //                MessageBox.Show("Invalid Item Code.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return false;
        //            }
        //            if (double.Parse(ugR.Cells["Quantity"].Value.ToString()) <= 0)
        //            {
        //                MessageBox.Show("Quantity Should be Greater than zero.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public Boolean HeaderValidation()
        {
            try
            {
                if (cmbWarehouse.Text.Trim() == "")
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
                StrSql = "SELECT ItemLine,ItemId,ItemDis,Qty,UnitCost,LineTotal FROM tblWhseTransLine   WHERE WhseTransId='" + StrInvoiceNo + "' ORDER BY ItemLine";

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
                        ugR.Cells["LineNo"].Value = Dr["ItemLine"];
                        ugR.Cells["ItemCode"].Value = Dr["ItemId"];
                        ugR.Cells["Description"].Value = Dr["ItemDis"];
                        ugR.Cells["UnitPrice"].Value = Dr["UnitCost"];
                        ugR.Cells["Quantity"].Value = Dr["Qty"];
                        ugR.Cells["TotalPrice"].Value = Dr["LineTotal"];
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
                StrSql = "SELECT WhseTransId,FrmWhseId,ToWhseId,TDate,NetValue,Description FROM tblWhseTransfer WHERE WhseTransId='" + StrInvoiceNo + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtCreditNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbWarehouse.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0].ItemArray[3].ToString().Trim());
                    txtNetValue.Value = double.Parse(dt.Rows[0].ItemArray[4].ToString().Trim());
                    txtDescription.Text = (dt.Rows[0].ItemArray[5].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        private void frmWareHouseTrans_Load(object sender, EventArgs e)
        {
            GridKeyActionMapping m = new GridKeyActionMapping(Keys.Enter, UltraGridAction.NextCellByTab, (UltraGridState)0, UltraGridState.Cell, SpecialKeys.All, (SpecialKeys)0);
            this.ug.KeyActionMappings.Add(m);

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
                        GetCurrentUserDate();

                        btnSave.Enabled = false;
                        btnPrint.Enabled = false;
                        btnSearch.Enabled = true;
                        btnReset.Enabled = true;
                        btnNew.Enabled = true;
                        btnEdit.Enabled = false;
                        dtpDate.Enabled = false;

                       
                        GetWareHouseDataSet();
                        GetCustomer();
                        GetSalesRep();
                        loadChartofAcount();
                        LoadDefualtAccount();
                        loadDefaltOption();
                        GetItemDataSet();
                        ClearHeader();
                        DeleteRows();
                        GetChargeItems();
                        EnableHeader(false);
                        EnableFoter(false);                        
                        GetCRNNo();
                        btnNew_Click(sender, e);                       
                        
                        
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
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
        private void loadDefaltOption()
        {
            try
            {
                //StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='PAY' and UserName='" + user.userName.ToString().Trim() + "'";
                //SqlCommand cmd = new SqlCommand(StrSql);
                //SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                //DataTable dt = new DataTable();
                //da.Fill(dt);
                //if (dt.Rows.Count > 0)
                //{
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        if (bool.Parse(dt.Rows[0].ItemArray[2].ToString()) == true)
                //        {
                //            grpamounttypes.Enabled = false;
                //        }
                //        if (dt.Rows[0]["TAXID"].ToString() == "Cash")
                //        {
                //            optCash.Checked = true;
                //        }
                //        else if (dt.Rows[0]["TAXID"].ToString() == "Credit")
                //        {
                //            optCredit.Checked = true;
                //        }
                //        else if (dt.Rows[0]["TAXID"].ToString() == "Other")
                //        {
                //            rdobtnCreditCard.Checked = true;
                //        }
                //    }
                //}

                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='TAX' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(StrSql);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        if (dt1.Rows[i]["Tid"].ToString() == "1")
                        {
                            rbtInclusive.Checked = true;
                            if (bool.Parse(dt1.Rows[i]["locked"].ToString()) == true)
                            {
                                rbtInclusive.Enabled = false;
                            }
                            else
                            {
                                rbtInclusive.Enabled = true ;
                            }
                        }
                        else if (dt1.Rows[i]["Tid"].ToString() == "2")
                        {
                            rbtExclusive.Checked = true;
                            if (bool.Parse(dt1.Rows[i]["locked"].ToString()) == true)
                            {
                                rbtExclusive.Enabled = false;
                            }
                            else
                            {
                                rbtExclusive.Enabled = true;
                            }
                        }
                        else
                        {
                            rbtExclusive.Checked = true;                        
                            if (bool.Parse(dt1.Rows[i]["locked"].ToString()) == true)
                            {
                                rbtExclusive.Enabled = false;
                            }
                            else
                            {
                                rbtExclusive.Enabled = true;
                            }
                        }

                    }
                }
                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='REP' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(StrSql);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    cmbSalesRep.Enabled = true;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        cmbSalesRep.Value = dt2.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt2.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbSalesRep.Enabled = false; 
                        }
                    }
                }

                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='WEH' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd3 = new SqlCommand(StrSql);
                SqlDataAdapter da3 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
                if (dt2.Rows.Count > 0)
                {
                    cmbWarehouse.Enabled = true;
                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        cmbWarehouse.Value = dt3.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt3.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbWarehouse.Enabled = false;
                        }
                    }
                }

                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='TAX' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd5 = new SqlCommand(StrSql);
                SqlDataAdapter da5 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt5 = new DataTable();
                da5.Fill(dt5);
                if (dt5.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        cmbInvoiceType.Value = dt5.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt5.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbInvoiceType.Enabled = false;
                        }
                    }
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
                //String S = "Select CurrentDate from tblUserWiseDate where UserName='" + UserAutherization.user.userName.ToString() + "'";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataSet dt = new DataSet();
                //da.Fill(dt);

                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                //    dtpDate.Value = UserWiseDate;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cmbWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                String S2 = "select * from tblWhseMaster where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    txtWarehouseName.Text = dt2.Rows[0].ItemArray[1].ToString();
                    StrARAccount = dt2.Rows[0].ItemArray[3].ToString();
                    StrCashAccount = dt2.Rows[0].ItemArray[4].ToString();
                    StrSalesGLAccount = dt2.Rows[0].ItemArray[5].ToString();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {                
                btnSave.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = false;
                btnSNO.Enabled = false;
                btnSearch.Enabled = true;
                btnReset.Enabled = true;
                btnEdit.Enabled = false;
                EnableHeader(true);
                EnableFoter(true);

                ClearHeader();
                DeleteRows();
                GetCRNNo();
                GetCustomer();
                GetSalesRep();
                loadChartofAcount();
                LoadDefualtAccount();
                cmbCustomer.Focus();
                GetWareHouseDataSet();
                GetItemDataSet();
           
                GetTaxDeails();
                loadDefaltOption();
                if (user.IsCRTNNoAutoGen) txtCreditNo.ReadOnly = true;
                else txtCreditNo.ReadOnly = false;

                clsSerializeItem.DtsSerialNoList.Rows.Clear();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
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

        public Boolean IsGridValidation()
        {
            if (ug.Rows.Count == 0)
            {
                MessageBox.Show("No Items In the Gird....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            foreach (UltraGridRow ugR in ug.Rows)
            {
                if (ugR.Cells[1].Text.Trim() != string.Empty)
                {
                    if (IsGridExitCode(ugR.Cells[1].Text) == false)
                    {
                        MessageBox.Show("Invalid Item Code.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                if (ugR.Cells[1].Text.Trim() != string.Empty)
                {
                    if (double.Parse(ugR.Cells["Quantity"].Value.ToString()) <= 0)
                    {
                        MessageBox.Show("Quantity Should be Greater than zero.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }           
            return true;
        }

        private bool IsThisItemSerial(string _ItemCode)
        {
            try
            {
                //if (dgvGRNTransaction.CurrentRow.Cells[0].Value == null) return false;
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

        public bool IsSerialNoCorrect()
        {
            try
            {
                int _Count = 0;
                string expression;
                foreach (UltraGridRow dgvr in ug.Rows)
                {
                    if (dgvr.Cells["ItemCode"].Value != null)
                    {
                        if (IsThisItemSerial(dgvr.Cells["ItemCode"].Value.ToString().Trim()) && double.Parse(dgvr.Cells["Quantity"].Value.ToString())>0)
                        {
                            if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemCode"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            _Count = 0;
                            expression = "ItemCode = '" + dgvr.Cells["ItemCode"].Value.ToString().Trim() + "'";
                            DataRow[] foundRows;

                            foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

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

        private void SaveEvent()
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


            if (!user.IsCRTNNoAutoGen)
            {
                if (txtCreditNo.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Enter CRN No....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

            if (cmbCustomer.Value == null)
            {
                MessageBox.Show("Incorrect Customer", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!IsGridValidation()) return;

            if (!IsSerialNoCorrect())
                return;

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
                DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                DeleteEmpGrid();

                //if (IsGridValidation() == false)
                //{
                //    return;
                //}
                if (HeaderValidation() == false)
                {
                    return;
                }
                //if (optCash.Checked == true)
                //{
                //    StrPaymmetM = "Cash";
                //}
                //else if (optCredit.Checked == true)
                //{
                //    StrPaymmetM = "Credit";
                //}

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                if (user.IsCRTNNoAutoGen)
                {
                    StrReference = GetInvNoField(myConnection, myTrans);
                    UpdatePrefixNo(myConnection, myTrans);
                    txtCreditNo.Text=StrReference;
                }
                else
                {
                    //tblCutomerReturn(CustomerID,CreditNo
                    SqlCommand myCommand = new SqlCommand("select * from tblCutomerReturn where CreditNo='" + txtCreditNo.Text.Trim() + "'", myConnection, myTrans);
                    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                    DataTable dt41 = new DataTable();
                    da41.Fill(dt41);

                    if (dt41.Rows.Count > 0)
                    {
                        MessageBox.Show("CRN No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        myTrans.Rollback();
                        myConnection.Close();//
                        return;
                    }
                }

                StrReference = txtCreditNo.Text.Trim();

                int InvType = 1;

                if (rdoSVat.Checked) InvType = 3;
                else if (rdoTax.Checked) InvType = 2;

                int _LineCount = CalculateLines();
                //chamila
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    SaveDetails(InvType,double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()), _LineCount, Int16.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), StrPaymmetM,
                        ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                        ug.Rows[intGrid].Cells["Description"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(),
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                        ug.Rows[intGrid].Cells["GL"].Value.ToString(), myConnection, myTrans);

                    //--------Check Stock Item-------------
                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {
                        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);

                        //if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > dblAvailableQty)
                        //{
                        //    MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                        //    myTrans.Rollback();
                        //    return;
                        //}

                        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);
                        InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), myConnection, myTrans);
                    }
                    //---------------------------------------
                }

                foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                {
                    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                        " TranType='CreditNote',Status='Available' " +
                        " where ItemID='" +
                        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                    myCommandSe1.ExecuteNonQuery();
                }

                frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "CreditNote", cmbWarehouse.Text.ToString(), txtCreditNo.Text.ToString().Trim(), dtpDate.Value, false, "");
                  

                //--End PH3 Posting--------------------
                CreatePurchaseJXML(myTrans, myConnection);

                Connector ObjImportP = new Connector();
                ObjImportP.ImportCustomerReturn();

                myTrans.Commit();
                MessageBox.Show("CreditNote Successfuly Saved.", "Information", MessageBoxButtons.OK);

                Print(StrReference);

                ButtonClear();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();                
                //btnSave.Focus();
                throw ex;
            }
        }

        public void CreatePurchaseJXMLOld(SqlTransaction tr, SqlConnection con)
        {

            //string INvCreditAcc = "";
            //string InvDebitAcc = "";

            //SqlCommand myCommand4 = new SqlCommand("Select CusretnDrAc,CusretnCrAc from tblDefualtSetting", con, tr);
            //// SqlCommand cmd = new SqlCommand(S);
            //SqlDataAdapter da = new SqlDataAdapter(myCommand4);
            //DataSet dt = new DataSet();
            //da.Fill(dt);

            //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //{
            //    // APAccount    = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //    INvCreditAcc = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //    InvDebitAcc = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //}

            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            int DistributionNumber = 0;
            int rowCount1 = CalculateLines();
            string NoDistributions = Convert.ToString(rowCount1);



            for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
            {
                if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
                {
                    if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
                    {
                        Writer.WriteStartElement("PAW_Invoice");
                        Writer.WriteAttributeString("xsi:type", "paw:invoice");

                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Vendor ID should be here = Ptient No
                        Writer.WriteEndElement();

                        //if (i == 0)
                        //{
                        Writer.WriteStartElement("Invoice_Number");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(txtCreditNo.Text.ToString().Trim());
                        Writer.WriteEndElement();
                        //}                       

                        Writer.WriteStartElement("Date");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");  
                        Writer.WriteString(dtpDate.Value.ToString("MM/dd/yyyy").Trim());//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Sales_Representative_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbSalesRep.Text.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Accounts_Receivable_Account");
                        Writer.WriteString(StrARAccount);//Cash Account
                        Writer.WriteEndElement();//CreditMemoType

                        Writer.WriteStartElement("CreditMemoType");
                        Writer.WriteString("TRUE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString((intGrid + 1).ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("SalesLines");

                        Writer.WriteStartElement("SalesLine");

                        Writer.WriteStartElement("InvoiceDistNum");
                        Writer.WriteString(DistributionNumber.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("-" + ug.Rows[intGrid].Cells["Quantity"].Value.ToString());//Doctor Charge
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(StrSalesGLAccount);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Unit_Price");
                        ////Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString(ug.Rows[i].Cells[5].Value.ToString());
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                        Writer.WriteEndElement();
                        //========================================================                        
                        Writer.WriteEndElement();
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AppliedToSO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();
                    }
                }
            }
            //********************
            Writer.WriteEndElement();//last line
            Writer.Close();                        
        }

        public void CreatePurchaseJXML(SqlTransaction tr, SqlConnection con)
        {            
            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);
            double _Doscount = 0;
            double _Amount = 0;
            _Doscount = double.Parse(txtDiscAmount.Value.ToString());

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            int DistributionNumber = 0;
            int rowCount1 = CalculateLines();
            string NoDistributions = Convert.ToString(rowCount1);

            double TotalLineDiscount = 0;

            for (int i = 0; i < ug.Rows.Count; i++)
            {
                try
                {
                    
                       TotalLineDiscount = TotalLineDiscount + (double.Parse(ug.Rows[i].Cells["UnitPrice"].Value.ToString()) * double.Parse(ug.Rows[i].Cells["Quantity"].Value.ToString())) - double.Parse(ug.Rows[i].Cells["TotalPrice"].Value.ToString());
                }
                catch (Exception ex)
                {

                }

            }

            if (TotalLineDiscount > 0)
            {
                NoDistributions = (Convert.ToInt32(NoDistributions) + 1).ToString();
            }
            Writer.WriteStartElement("PAW_Invoice");
            Writer.WriteAttributeString("xsi:type", "paw:invoice");

            Writer.WriteStartElement("Customer_ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Vendor ID should be here = Ptient No
            Writer.WriteEndElement();

            //if (i == 0)
            //{
            Writer.WriteStartElement("Invoice_Number");
            //Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(txtCreditNo.Text.ToString().Trim());
            Writer.WriteEndElement();
            //}                       

            Writer.WriteStartElement("Date");
            //Writer.WriteAttributeString("xsi:type", "paw:id");  
            Writer.WriteString(GRNDate);//Date 
            Writer.WriteEndElement();

            Writer.WriteStartElement("Sales_Representative_ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbSalesRep.Text.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Accounts_Receivable_Account");
            Writer.WriteString(StrARAccount);//Cash Account
            Writer.WriteEndElement();//CreditMemoType

            Writer.WriteStartElement("CreditMemoType");
            Writer.WriteString("TRUE");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString(NoDistributions);
            Writer.WriteEndElement();

            Writer.WriteStartElement("SalesLines");

            for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
            {
                if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
                {
                    if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
                    {
                        if (IsThisItemSerial(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString()))
                        {
                            foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                            {
                                if (ug.Rows[intGrid].Cells["ItemCode"].Value.ToString() == dr["ItemCode"].ToString())
                                {
                                    Writer.WriteStartElement("SalesLine");

                                    Writer.WriteStartElement("Quantity");
                                    Writer.WriteString("-" + ug.Rows[intGrid].Cells["Quantity"].Value.ToString());//Doctor Charge
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Item_ID");
                                    Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Description");
                                    Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("GL_Account");
                                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                                    Writer.WriteString(StrSalesGLAccount);
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Tax_Type");
                                    Writer.WriteString("1");//Doctor Charge
                                    Writer.WriteEndElement();

                                    //Writer.WriteStartElement("Unit_Price");
                                    ////Writer.WriteAttributeString("xsi:type", "paw:id");
                                    //Writer.WriteString(ug.Rows[intGrid].Cells[5].Value.ToString());
                                    //Writer.WriteEndElement();
                                    double _GRAM = 0;
                                    double dblcolUnitPriceExport = 0;
                                    double Disam = 0.00;
                                    double vatper = double.Parse(txtVatPer.Value.ToString());
                                    double Taxam = 0.00;
                                    if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                                    {
                                        _Amount = 0;
                                        _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());

                                        if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                        {
                                            _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()));
                                            _Amount = (_Amount / (vatper + 100)) * 100;
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                        {
                                            Taxam = double.Parse(ug.Rows[intGrid].Cells["Linetax"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                            Disam = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) - Taxam) * double.Parse(txtDiscPer.Text.ToString()) / 100;
                                            _Amount = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) - Taxam) - Disam;
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                        {
                                            _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["Linetax"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                        {
                                            //Discount 1
                                            _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()));
                                            _Amount = (_Amount / (vatper + 100)) * 100;
                                            //Discount 2
                                            _Amount = _Amount - (_Amount * double.Parse(txtDiscPer.Text.ToString()) / 100);

                                        }
                                        dblcolUnitPriceExport = _Amount;
                                    }
                                    if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                                    {
                                        _Amount = 0;
                                        _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                        if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                        {
                                            Disam = double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                            _Amount = _Amount - Disam;
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                        {
                                            _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                        {
                                            _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                        {
                                            _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                            _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                        }
                                        dblcolUnitPriceExport = _Amount;
                                    }
                                    else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                                    {
                                        _Amount = 0;
                                        _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                        if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                        {
                                            Disam = double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                            _Amount = _Amount - Disam;
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                        {
                                            _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                        {
                                            _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                        }
                                        else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                        {
                                            _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                            _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                        }
                                        dblcolUnitPriceExport = _Amount;

                                    }

                                    dblcolUnitPriceExport = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) * double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));

                                    Writer.WriteStartElement("Amount");
                                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                                    Writer.WriteString(dblcolUnitPriceExport.ToString());//(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Serial_Number");
                                    Writer.WriteString(dr["SerialNo"].ToString());
                                    Writer.WriteEndElement();
                                    //========================================================                        
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("AppliedToSO");
                                    Writer.WriteString("FALSE");
                                    Writer.WriteEndElement();                                    
                                }
                            }
                        }
                        else
                        {
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("-" + ug.Rows[intGrid].Cells["Quantity"].Value.ToString());//Doctor Charge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Item_ID");
                            Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Description");
                            Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteString(ug.Rows[intGrid].Cells["GL"].Value.ToString());// (StrSalesGLAccount);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Job_ID");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");//Doctor Charge
                            Writer.WriteEndElement();

                            double _GRAM = 0;
                            double dblcolUnitPriceExport = 0;
                            double Disam = 0.00;
                            double vatper = double.Parse(txtVatPer.Value.ToString());
                            double Taxam = 0.00;
                            if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                            {
                                _Amount = 0;
                                _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());

                                if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                {
                                    _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()));
                                    _Amount = (_Amount / (vatper + 100)) * 100;
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                {
                                    Taxam = double.Parse(ug.Rows[intGrid].Cells["Linetax"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                    Disam = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) - Taxam) * double.Parse(txtDiscPer.Text.ToString()) / 100;
                                    _Amount = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) - Taxam) - Disam;
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                {
                                    _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["Linetax"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                {
                                    //Discount 1
                                    _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()));
                                    _Amount = (_Amount / (vatper + 100)) * 100;
                                    //Discount 2
                                    _Amount = _Amount - (_Amount * double.Parse(txtDiscPer.Text.ToString()) / 100);

                                }
                                dblcolUnitPriceExport = _Amount;
                            }
                            if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                            {
                                _Amount = 0;
                                _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                {
                                    Disam = double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                    _Amount = _Amount - Disam;
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                {
                                    _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                {
                                    _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                {
                                    _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                    _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                }
                                dblcolUnitPriceExport = _Amount;
                            }
                            else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                            {
                                _Amount = 0;
                                _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                {
                                    Disam = double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());
                                    _Amount = _Amount - Disam;
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                {
                                    _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                {
                                    _Amount = double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                                }
                                else if (double.Parse(ug.Rows[intGrid].Cells["Disc %"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                {
                                    _Amount = _Amount - (double.Parse(ug.Rows[intGrid].Cells["DiscLineAmt"].Value.ToString()) / double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));
                                    _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                }
                                dblcolUnitPriceExport = _Amount;
                            }
                            dblcolUnitPriceExport = (double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) * double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()));

                            Writer.WriteStartElement("Amount");
                            //Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(dblcolUnitPriceExport.ToString());// (ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                            Writer.WriteEndElement();
                            //========================================================                        
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("AppliedToSO");
                            Writer.WriteString("FALSE");
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("Serial_Number");
                            //Writer.WriteString(dr["SerialNo"].ToString());
                            //Writer.WriteEndElement();

                        }
                    }
                }
            }

            if (double.Parse(txtNBT.Value.ToString().Trim()) > 0)
            {

                Writer.WriteStartElement("SalesLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("-1");
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
                Writer.WriteString(txtNBT.Text.ToString());
                Writer.WriteEndElement();


                Writer.WriteStartElement("AppliedToSO");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }

            if (double.Parse(txtVat.Value.ToString().Trim()) > 0)
            {

                Writer.WriteStartElement("SalesLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("-1");
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

                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString(txtVat.Text.ToString());
                Writer.WriteEndElement();


                Writer.WriteStartElement("AppliedToSO");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }

            //if (double.Parse(TotalLineDiscount.ToString().Trim()) > 0)
            //{
            //    Writer.WriteStartElement("SalesLine");
            //    Writer.WriteStartElement("Quantity");
            //    Writer.WriteString("-1");
            //    Writer.WriteEndElement();


            //    Writer.WriteStartElement("Item_ID");
            //    Writer.WriteString(LineDisitemid);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Description");
            //    Writer.WriteString(LineDisitemdescription);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("GL_Account");
            //    Writer.WriteString(LineDisGLAccount);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Job_ID");
            //    Writer.WriteString(cmbjob.Text.ToString().Trim());
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Tax_Type");
            //    Writer.WriteString("1");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString(TotalLineDiscount.ToString());
            //    Writer.WriteEndElement();



            //    Writer.WriteEndElement();
            //}

            if (double.Parse(txtDiscAmount.Text.ToString().Trim()) > 0)
            {
                Writer.WriteStartElement("SalesLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("-1");
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
                Writer.WriteString("-" + txtDiscAmount.Text.ToString());
                Writer.WriteEndElement();


                Writer.WriteStartElement("AppliedToSO");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }

            if (double.Parse(txtServicecharge.Text.ToString().Trim()) > 0)
            {
                Writer.WriteStartElement("SalesLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("-1");
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
                Writer.WriteString(txtServicecharge.Text.ToString());
                Writer.WriteEndElement();


                Writer.WriteStartElement("AppliedToSO");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }

            if ((TotalLineDiscount) > 0)
            {
                Writer.WriteStartElement("SalesLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("-1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Item_ID");
                Writer.WriteString(LineDisitemid);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(LineDisitemdescription);
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(LineDisGLAccount);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + TotalLineDiscount.ToString());
                Writer.WriteEndElement();


                Writer.WriteStartElement("AppliedToSO");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }


            //********************
            Writer.WriteEndElement();//last line
            Writer.Close();

            //Connector ObjImportP = new Connector();
            //ObjImportP.ImportCustomerReturn();
        }

        //private void ImportCreditNote()
        //{
        //    try
        //    {
        //        jkhjkhj



        //        XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CreditNote.xml", System.Text.Encoding.UTF8);
        //        Writer.Formatting = Formatting.Indented;
        //        Writer.WriteStartElement("PAW_Invoices");
        //        Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
        //        Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
        //        Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");
        //        // --PH3 Posting--------------------
        //        for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
        //        {
        //            if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
        //            {
        //                if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
        //                {
        //                    DateTime DTP = Convert.ToDateTime(dtpDate.Text);
        //                    string Dformat = "MM/dd/yyyy";
        //                    string InvDate = DTP.ToString(Dformat);

        //                    if (intGrid < ug.Rows.Count)
        //                    {
        //                        Writer.WriteStartElement("PAW_Invoice");
        //                        Writer.WriteAttributeString("xsi:type", "paw:Receipt");

        //                        Writer.WriteStartElement("Customer_ID");
        //                        Writer.WriteAttributeString("xsi:type", "paw:id");
        //                        Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Date");
        //                        Writer.WriteAttributeString("xsi:type", "paw:id");
        //                        Writer.WriteString(GetDateTime(dtpDate.Value));//Date 
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Invoice_Number");
        //                        Writer.WriteString(txtCreditNo.Text.Trim());
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Sales_Representative_ID");
        //                        Writer.WriteAttributeString("xsi:type", "paw:id");
        //                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Accounts_Receivable_Account");
        //                        Writer.WriteAttributeString("xsi:type", "paw:id");
        //                        Writer.WriteString(cmbARAccount.Text.ToString().Trim());//Cash Account
        //                        Writer.WriteEndElement();//CreditMemoType

        //                        Writer.WriteStartElement("CreditMemoType");
        //                        Writer.WriteString("TRUE");
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Number_of_Distributions");
        //                        Writer.WriteString((ug.Rows.Count).ToString());
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("SalesLines");

        //                            Writer.WriteStartElement("SalesLine");

        //                            Writer.WriteStartElement("Quantity");
        //                            Writer.WriteString(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());//Doctor Charge
        //                            Writer.WriteEndElement();

        //                            Writer.WriteStartElement("Item_ID");
        //                            Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
        //                            Writer.WriteEndElement();

        //                            Writer.WriteStartElement("Description");
        //                            Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
        //                            Writer.WriteEndElement();

        //                            Writer.WriteStartElement("GL_Account");
        //                            Writer.WriteAttributeString("xsi:type", "paw:id");
        //                            Writer.WriteString(StrSalesGLAccount);
        //                            Writer.WriteEndElement();
        //                            //========================================================
        //                            Writer.WriteStartElement("Tax_Type");
        //                            Writer.WriteString("1");//Doctor Charge
        //                            Writer.WriteEndElement();

        //                            Writer.WriteStartElement("Amount");
        //                            Writer.WriteString("-" + ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());//HospitalCharge
        //                            Writer.WriteEndElement();

        //                            Writer.WriteEndElement();//LINE
        //                            Writer.WriteEndElement();//LINES

        //                        Writer.WriteEndElement();
        //                    }
        //                }
        //            }
        //        }
        //        Writer.Close();

        //        Connector ObjImportP = new Connector();
        //        ObjImportP.ImportDirectSalesInvice();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private int CalculateLines()
        {
            int _Count = 0;
            try
            {
                for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
                    {
                        if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
                            _Count = _Count + 1;
                    }
                }

                if (double.Parse(txtNBT.Value.ToString().Trim()) > 0) _Count = _Count + 1;
                if (double.Parse(txtVat.Value.ToString().Trim()) > 0) _Count = _Count + 1;
                if (double.Parse(txtServicecharge.Text.ToString().Trim()) > 0) _Count = _Count + 1;
                if ((double.Parse(txtDiscAmount.Text.ToString().Trim()) + double.Parse(txtDiscLineTot.Text.Trim())) > 0) _Count = _Count + 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _Count;
            //SaveDetails(ug.Rows.Count, intGrid + 1, StrPaymmetM, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()),
            //ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(),
            //double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
        }

        private void UpdateItemWarehouse(string StrItemCode, string StrWarehouse, double dblQty, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblItemWhse SET QTY=QTY+" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveDetails(int InvType,double LineDiscP, int intLineCount, int intLineNo, string StrPayMethod, string StrItemCode, double dblQuantity, string StrItemDescription, string StrUOM, double dblPrice, double dblLineNetAmt, string StrGLAccount, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO tblCutomerReturn(CustomerID,CreditNo,ReturnDate, " +
                    " LocationID,IsApplyToInvoice,InvoiceNO, " +
                    " ARAccount,NoofDistribution,DistributionNo, " +
                    " ItemID,ReturnQty, " +
                    " Description,UOM,UnitPrice, " +
                    " Discount,Amount,GL_Account, " +
                    " NBT,VAT,GrossTotal, " +
                    " GrandTotal,ISExport,CurrenUser, " +
                    " Type ,SalesRep,InvType,IsInclusive,ServCharg,LineDiscP,NBTP,VATP) " +
                         " VALUES ('" + cmbCustomer.Value.ToString() + "','" + txtCreditNo.Text.Trim() + "','" + (dtpDate.Value.ToString("MM/dd/yyyy")) + "','" +
                         cmbWarehouse.Value.ToString() + "','0','DirectReturn','" +
                         cmbARAccount.Text.Trim() + "','" + intLineCount + "','" + intLineNo + "','" +
                         StrItemCode + "','" + dblQuantity + "','" +
                         StrItemDescription + "','" + StrUOM + "','" + dblPrice + "','" +
                         txtDiscPer.Value.ToString().Trim() + "','" + dblLineNetAmt + "','" + StrGLAccount + "','" +
                         txtNBT.Value.ToString().Trim() + "','" + txtVat.Value.ToString().Trim() + "','" + txtSubValue.Value.ToString() + "','" +
                         txtNetValue.Value.ToString().Trim() + "','False','" + user.userName.ToString().Trim() + "','DirectReurn','" + cmbSalesRep.Value.ToString().Trim() + "','" +
                         InvType+"','"+rbtInclusive.Checked+"','"+txtServicecharge.Text.Trim()+"','"+ LineDiscP +"','"+txtNBTPer.Value.ToString().Trim()+"','"+txtVatPer.Value.ToString().Trim()+"')";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveEvent();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double CheckWarehouseItem(string StrItemCode, string StrWarehouseCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT QTY FROM tblItemWhse WHERE ItemId='" + StrItemCode + "' AND WhseId='" + StrWarehouseCode + "'";

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
            try
            {
                DateTime DTP = Convert.ToDateTime(DtGetDate);
                string Dformat = "MM/dd/yyyy";
                return DTP.ToString(Dformat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        private void InvTransaction(string strCRNNo, DateTime dtDate, String StrItemCode, string StrLocCode, double dblQuantity, double dblPrice, double dblLineNetAmt, double SellingPrice, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],                [UnitCost],         [TotalCost],        [WarehouseID],[SellingPrice]) " +
                    " VALUES(5,'" + strCRNNo + "','" + GetDateTime(dtDate) + "','CreditNote','True','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "','" + SellingPrice + "')";

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
            try
            {
                blnEdit = true;
                btnEdit.Enabled = true;
                btnPrint.Enabled = true;

                if (frmMain.objfrmReturnNotenewSearch == null || frmMain.objfrmReturnNotenewSearch.IsDisposed)
                {
                    frmMain.objfrmReturnNotenewSearch = new frmReturnNotenewSearch(1);
                }
                //frmMain.ObjDirectR.TopMost = false;
                frmMain.objfrmReturnNotenewSearch.ShowDialog();
                setValue();
                //frmMain.objfrmReturnNotenewSearch.TopMost = true;               

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
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

        private void Print(string strCRNNo)
        {
            try
            {
                ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                if (strCRNNo != "")
                {
                    DsCustomerReturn.Clear();
                    String S1 = "Select * from tblCutomerReturn WHERE CreditNo = '" + txtCreditNo.Text.Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    //da1.Fill(ds, "DTReturn");
                    da1.Fill(DsCustomerReturn.DTReturn);

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    //da4.Fill(ds, "dt_CompanyDetails");
                    da4.Fill(DsCustomerReturn.dt_CompanyDetails);

                   // DirectPrint();

                    frmViewerCreditNote cusReturn = new frmViewerCreditNote(this);
                    cusReturn.Show();
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void DirectPrint()
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptCreditNote.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(DsCustomerReturn);
                crp.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception ex)
            {
               // throw ex;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Print(txtCreditNo.Text);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ClearFooter()
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ClearHeader()
        {
            try
            {
                txtCreditNo.Text = "";
                cmbWarehouse.Text = user.StrDefaultWH;
                txtDescription.Text = "";
                dtpDate.Value = user.LoginDate;
               // txtWarehouseName.Text = "";
                cmbCustomer.Text = "";
                txtCustomer.Text = "";
                cmbSalesRep.Text = "";
                txtSalesRep.Text = "";
                txtAddress1.Text = "";

                //txtNBTPer.Value = 0;
                //txtVatPer.Value = 0;
                txtDiscPer.Value = 0;
                txtSubValue.Value = 0;
                txtDiscAmount.Value = 0;
                txtGrossValue.Value = 0;
                //txtNBT.Value = 0;
                //txtVat.Value = 0;
                txtNetValue.Value = 0;
                rdoNonVat.Checked = true;
                rbtExclusive.Checked = true;
                txtDiscLineTot.Text = "0.00";
                txtServicecharge.Text = "0.00";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnableFoter(Boolean blnEnable)
        {
            try
            {
                txtVatPer.Enabled = blnEnable;
                txtNBTPer.Enabled = blnEnable;
                txtDescription.Enabled = blnEnable;
                txtDiscPer.Enabled = blnEnable;
                //ug.Enabled = tr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnableHeader(Boolean blnEnable)
        {
            try
            {
                cmbWarehouse.Enabled = blnEnable;
                dtpDate.Enabled = blnEnable;
                txtWarehouseName.Enabled = false;

                cmbCustomer.Enabled = blnEnable;
                txtCustomer.Enabled = false;

                cmbSalesRep.Enabled = blnEnable;
                txtSalesRep.Enabled = false;

                cmbARAccount.Enabled = blnEnable;
                optCash.Enabled = blnEnable;
                optCredit.Enabled = blnEnable;
                btnSave.Enabled = blnEnable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ButtonClear()
        {
            try
            {
                btnSave.Enabled = false;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
                btnSearch.Enabled = true;
                btnReset.Enabled = true;
                btnEdit.Enabled = false;

                ClearHeader();
                EnableHeader(false);
                EnableFoter(false);
                DeleteRows();
                GetCRNNo();
                ug.Enabled = true;
                intEstomateProcode = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonClear();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
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
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double ChangePrice2(double dblPriceList2)
        {
            return dblPriceList2;
        }

        private void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {                
                if (e.Cell == null) return;

//                
//
//
//
//

                //if (e.Cell.Column.Key == "ItemCode" || e.Cell.Column.Key == "UnitPrice" ||
                //    e.Cell.Column.Key == "UnitPrice(Incl)" || e.Cell.Column.Key == "TotalPrice" ||
                //    e.Cell.Column.Key == "TotalPrice(Incl)" || e.Cell.Column.Key == "Quantity" || e.Cell.Column.Key == "Disc %")
                //{
                //    //int rowCount = GetFilledRows();
                //    //NAmountDC = 0.0;
                //    double DispatchQty = 0.00;
                //    double unitprice = 0.00;
                //    double unitprice_Incl = 0.00;
                //    double DiscountRate = 0.00;
                //    double DiscountAmount = 0.00;
                //    double Amount = 0.00;
                //    double Amount1 = 0.00;
                //    double TotalAmount = 0.00;
                //    double TotalDiscAmount = 0.00;
                //    //GetTaxDeails();

                //    for (int a = 0; a < ug.Rows.Count; a++)
                //    {
                //        if (ug.Rows[a].Cells["ItemCode"].Value != null && ug.Rows[a].Cells["Quantity"].Value != null && ug.Rows[a].Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                //        {
                //            if (rbtExclusive.Checked)
                //            {
                //                DispatchQty = Convert.ToDouble(ug.Rows[a].Cells["Quantity"].Value);
                //                unitprice = Convert.ToDouble(ug.Rows[a].Cells["UnitPrice"].Value);
                //                DiscountRate = Convert.ToDouble(ug.Rows[a].Cells["Disc %"].Value) / 100;
                //                Amount = (DispatchQty * unitprice);
                //                TotalAmount = TotalAmount + Amount;
                //                DiscountAmount = Amount * DiscountRate;
                //                Amount1 = Amount - DiscountAmount;
                //                ug.Rows[a].Cells["DiscLineAmt"].Value = DiscountAmount.ToString("0.00");
                //                ug.Rows[a].Cells["TotalPrice"].Value = Amount1.ToString("0.00");
                //                TotalDiscAmount = TotalDiscAmount + Convert.ToDouble(ug.Rows[a].Cells["DiscLineAmt"].Value);
                //                unitprice_Incl = unitprice + (unitprice * Tax1Amount / 100);
                //                unitprice_Incl = unitprice_Incl + (unitprice_Incl * Tax2Amount / 100);
                //                ug.Rows[a].Cells["UnitPrice(Incl)"].Value = unitprice_Incl;
                //            }
                //        }
                //    }

                //    //txtDiscLineTot.Text = TotalDiscAmount.ToString();
                //    //txtSubTot.Text = Amount1.ToString("N2");
                //    txtSubValue.Text = (Amount1 + double.Parse(txtServicecharge.Text.Trim())).ToString("N2");
                //    txtNetValue.Text = TotalAmount.ToString("N2");

                //    GrandTotal();
                //}
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
            //try
            //{
            //    if (e.Cell.Column.Key == "UnitPrice" || e.Cell.Column.Key == "Quantity" || e.Cell.Column.Key == "UnitPrice(Incl)" || e.Cell.Column.Key == "TotalPrice(Incl)" || e.Cell.Column.Key == "Disc %")
            //    {
            //        e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
            //        GrandTotal();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            //}
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
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtDiscPer.Text.Trim(), out amount))
                {
                    txtDiscPer.Text = "0.00";
                    return;

                }

                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtNBTPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtNBTPer.Text.Trim(), out amount))
                {
                    txtNBTPer.Text = "0.00";
                    return;
                }

                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        private void cmbWarehouse_Leave(object sender, EventArgs e)
        {
            try
            {
                DeleteRows();
                ClearFooter();
                GetItemDataSet();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            try
            {
                if (ug.ActiveCell.Column.Key == "ItemCode")
                {
                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        return;
                    }
                    else
                    {
                        ug.ActiveCell.Value = ug.ActiveCell.Text;
                        if (IsGridExitCode(ug.ActiveCell.Row.Cells[1].Text) == false)
                        {
                            e.Cancel = true;
                        }
                        foreach (UltraGridRow ugR in ultraCombo1.Rows)
                        {
                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemId"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDis"].Value;
                                ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
                                ug.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ug.ActiveCell.Row.Cells["CostPrice"].Value = ugR.Cells["UnitCost"].Value;
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
                                ug.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                                ug.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;


                                ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;

                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["UnitCost"].Value;
                            }
                        }
                        HideSelectedRow();
                    }
                }                
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        private void HideSelectedRow()
        {
            for (int x = 0; x < ultraCombo1.Rows.Count; x++)
            {
                ultraCombo1.Rows[x].Hidden = false;
            }
            for (int i = 0; i < ultraCombo1.Rows.Count; i++)
            {
                for (int v = 0; v < ug.Rows.Count; v++)
                {
                    if (ultraCombo1.Rows[i].Cells[0].Value.ToString() == ug.Rows[v].Cells["ItemCode"].Value.ToString())
                    {
                        ultraCombo1.Rows[i].Hidden = true;
                    }

                }
            }
        }
        private void optSerialTwo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GetCRNNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void optSerialOne_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GetCRNNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                        cmbARAccount.Text = StrARAccount;
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
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
                cmbSalesRep.DisplayMember = "RepCode";
                cmbSalesRep.ValueMember = "RepCode";
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepCode"].Width = 75;
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepName"].Width = 125;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnclose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ug_AfterRowsDeleted(object sender, EventArgs e)
        {
            HideSelectedRow();
            try
            {
                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                        txtAddress1.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString() + ", " + cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public void LoadDefualtAccount()
        {
            try
            {
                String S = "Select CusretnDrAc from tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbARAccount.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                    // cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public void loadChartofAcount()
        {
            try
            {
                String S = "Select * from tblChartofAcounts";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex) { throw ex; }
        }       

        private void cmbSalesRep_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtSalesRep.Text = cmbSalesRep.ActiveRow.Cells[1].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscPer_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                //btnSave.Focus();
        }
        bool search = false;

        private void txtDiscAmount_ValueChanged_1(object sender, EventArgs e)
        {

        }

        private void txtCreditNo_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                //ug.DataSource = null;
                string ConnString1 = ConnectionString;
                String S2 = "SELECT Reason,InvType from tblCutomerReturn   where CreditNo='" + txtCreditNo.Text.Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {                    
                    cmbInvoiceType.Value = dt2.Rows[0]["InvType"].ToString().Trim();
                }

                string ConnString = ConnectionString;
                String S1 = "SELECT tblCutomerReturn.*, tblItemMaster.ItemClass, tblItemMaster.SalesGLAccount, tblItemMaster.Categoty, " +
                    " tblItemMaster.PriceLevel1, tblItemMaster.PriceLevel2, tblItemMaster.PriceLevel3, tblItemMaster.PriceLevel4, " +
                    " tblItemMaster.PriceLevel5, tblItemMaster.UnitCost,isnull(tblItemWhse.QTY,0)as QTY" +
                    " FROM            tblCutomerReturn LEFT OUTER JOIN " +
                    " tblItemMaster ON tblCutomerReturn.ItemID = tblItemMaster.ItemID LEFT OUTER JOIN " +
                    " tblItemWhse ON tblCutomerReturn.ItemID = tblItemWhse.ItemId AND tblCutomerReturn.LocationID = tblItemWhse.WhseId where CreditNo='" + txtCreditNo.Text.Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cmbCustomer.Text = dt.Rows[0]["CustomerID"].ToString();
                    cmbSalesRep.Text = dt.Rows[0]["SalesRep"].ToString();
                    cmbWarehouse.Text = dt.Rows[0]["LocationID"].ToString();
                    cmbARAccount.Text = dt.Rows[0]["ARAccount"].ToString();
                   //txtDiscAmount.Value = double.Parse(dt.Rows[0]["Discount"].ToString()).ToString("0.00");                    

                    foreach (DataRow dr in dt.Rows)
                    {
                        UltraGridRow ugR;
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                        ugR.Cells["ItemCode"].Value = dr["ItemID"].ToString();
                        ugR.Cells["Description"].Value = dr["Description"].ToString();
                        ugR.Cells["UnitPrice"].Value = dr["UnitPrice"].ToString();
                        ugR.Cells["Quantity"].Value = dr["ReturnQty"].ToString();
                        ugR.Cells["TotalPrice"].Value = dr["Amount"].ToString();
                        ugR.Cells["OnHand"].Value = Convert.ToDouble(dr["QTY"]);
                        ugR.Cells["ItemClass"].Value = dr["ItemClass"].ToString();
                        ugR.Cells["GL"].Value = dr["SalesGLAccount"].ToString();
                        ugR.Cells["UOM"].Value = dr["UOM"].ToString();
                        ugR.Cells["Categoty"].Value = dr["Categoty"].ToString();
                        ugR.Cells["CostPrice"].Value = dr["UnitCost"].ToString();
                        ugR.Cells["PriceLevel1"].Value = dr["PriceLevel1"].ToString();
                        ugR.Cells["PriceLevel2"].Value = dr["PriceLevel2"].ToString();
                        ugR.Cells["PriceLevel3"].Value = dr["PriceLevel3"].ToString();
                        ugR.Cells["PriceLevel4"].Value = dr["PriceLevel4"].ToString();
                        ugR.Cells["PriceLevel5"].Value = dr["PriceLevel5"].ToString();
                        ugR.Cells["Disc %"].Value = dr["LineDiscP"].ToString();


                      

                    }
                    txtDiscPer.Value = double.Parse(dt.Rows[0]["Discount"].ToString()).ToString("0.00");
                    // txtDiscPer.Value = ((double.Parse(dt.Rows[0]["Disc %"].ToString())) * 100 / (double.Parse(dt.Rows[0]["GrossTotal"].ToString()))).ToString("0.00");
                    txtSubValue.Value = double.Parse(dt.Rows[0]["GrossTotal"].ToString()).ToString("0.00"); 
                    txtGrossValue.Value = double.Parse(dt.Rows[0]["GrandTotal"].ToString()).ToString("0.00"); 
                    txtNetValue.Value = double.Parse(dt.Rows[0]["GrandTotal"].ToString()).ToString("0.00"); 
                    dtpDate.Value = DateTime.Parse(dt.Rows[0]["ReturnDate"].ToString());
                    txtServicecharge.Text = double.Parse(dt.Rows[0]["ServCharg"].ToString()).ToString("0.00"); 
                    txtNBT.Value = double.Parse(dt.Rows[0]["NBT"].ToString()).ToString("0.00"); 
                    txtVat.Value = double.Parse(dt.Rows[0]["VAT"].ToString()).ToString("0.00"); 
                    txtNBTPer.Value = double.Parse(dt.Rows[0]["NBTP"].ToString()).ToString("0.00");
                    txtVatPer.Value = double.Parse(dt.Rows[0]["VATP"].ToString()).ToString("0.00");



                    rbtExclusive.Checked = true;
                    rbtInclusive.Checked = bool.Parse(dt.Rows[0]["IsInclusive"].ToString());

                    if (int.Parse(dt.Rows[0]["InvType"].ToString()) == 1)
                        rdoNonVat.Checked = true;
                    if (int.Parse(dt.Rows[0]["InvType"].ToString()) == 2)
                        rdoTax.Checked = true;
                    if (int.Parse(dt.Rows[0]["InvType"].ToString()) == 3)
                        rdoSVat.Checked = true;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }

          
        }

        private void ug_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void txtVat_ValueChanged(object sender, EventArgs e)
        {

        }

        private void GetTaxDeails()
        {
            try
            {
                //                SELECT     TaxID, TaxName, Rate, Rank, Account, IsActive, IsTaxOnTax
                //FROM         tblTaxApplicable
                String S1 = "Select * from tblTaxApplicable order by Rank";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                //txtNBTPer.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
                //txtVatPer.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");

                //String S1 = "Select * from tblTaxApplicable order by Rank";
                //SqlCommand cmd1 = new SqlCommand(S1);
                //SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                //DataTable dt = new DataTable();
                //da1.Fill(dt);

               // txtNBTPer.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
              //  txtVatPer.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");

                Tax1ID = dt.Rows[0]["TaxID"].ToString();
                Tax2ID = dt.Rows[1]["TaxID"].ToString();

                Tax1Name = dt.Rows[0]["TaxName"].ToString();
                Tax2Name = dt.Rows[1]["TaxName"].ToString();

                Tax1Rate = double.Parse(dt.Rows[0]["Rate"].ToString());
                Tax2Rate = double.Parse(dt.Rows[1]["Rate"].ToString());

                Tax1GLAccount = user.TaxPayGL1;
                Tax2GLAccount = user.TaxPayGL2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillInvoiceType()
        {
            try
            {
                if (rdoNonVat.Checked)
                {
                    //txtNBT.Text = "0.00";
                    //txtNBTPer.Text = "0.00";
                    //txtVatPer.Text = "0.00";
                    //txtVat.Text = "0.00";

                    txtNBT.ReadOnly = true;
                    txtNBTPer.ReadOnly = true;
                    txtVat.ReadOnly = true;
                    txtVatPer.ReadOnly = true;
                }
                else if (rdoTax.Checked)
                {
                    //txtNBT.Text = "0.00";
                    //txtNBTPer.Text = "0.00";
                    //txtVatPer.Text = "0.00";
                    //txtVat.Text = "0.00";

                    //txtNBT.ReadOnly = false;
                    txtNBTPer.ReadOnly = false;
                    //txtVAT.ReadOnly = false;
                    txtVatPer.ReadOnly = false;
                   // GetTaxDeails();
                }
                else if (rdoSVat.Checked)
                {
                    //txtNBT.Text = "0.00";
                    //txtNBTPer.Text = "0.00";
                    //txtVatPer.Text = "0.00";
                    //txtVat.Text = "0.00";

                    //txtNBT.ReadOnly = false;
                    txtNBTPer.ReadOnly = false;
                    //txtVAT.ReadOnly = false;
                    txtVatPer.ReadOnly = false;
                  //  GetTaxDeails();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void rdoNonVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoNonVat.Checked)
                {
                    FillInvoiceType();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoTax_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoTax.Checked)
                {
                    FillInvoiceType();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoSVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoSVat.Checked)
                {
                    FillInvoiceType();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rbtExclusive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbtExclusive.Checked)
                {
                    //CalculateGridAmounts();
                    //CalculateTaxAmounts();

                    InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                    FooterCalculation();    

                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice(Incl)"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice(Incl)"].Hidden = true;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rbtInclusive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoNonVat.Checked)
                {
                    if (rbtInclusive.Checked)
                    {
                        rbtInclusive.Checked = false;
                        rbtExclusive.Checked = true;
                        //MessageBox.Show("Invalid Tax Type....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                    FooterCalculation();    

                   // CalculateGridAmounts();
                   // CalculateTaxAmounts();
                }

                if (rbtInclusive.Checked)
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice(Incl)"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice(Incl)"].Hidden = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_AfterExitEditMode(object sender, EventArgs e)
        {
            try
            {
                if (ug.ActiveCell.Column.Key == "ItemCode" || ug.ActiveCell.Column.Key == "UnitPrice" ||
                        ug.ActiveCell.Column.Key == "UnitPrice(Incl)" || ug.ActiveCell.Column.Key == "TotalPrice" ||
                        ug.ActiveCell.Column.Key == "TotalPrice(Incl)" || ug.ActiveCell.Column.Key == "Quantity" || ug.ActiveCell.Column.Key == "Disc %")
                {                    
                    double DispatchQty = 0.00;
                    double unitprice = 0.00;
                    double unitprice_Incl = 0.00;
                    double DiscountRate = 0.00;
                    double DiscountAmount = 0.00;
                    double Amount = 0.00;
                    double Amount1 = 0.00;
                    double TotalAmount = 0.00;
                    double TotalDiscAmount = 0.00;
                    //GetTaxDeails();

                    for (int a = 0; a < ug.Rows.Count; a++)
                    {
                        if (ug.Rows[a].Cells["ItemCode"].Value != null && ug.Rows[a].Cells["Quantity"].Value != null && ug.Rows[a].Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                        {
                            if (rbtExclusive.Checked)
                            {
                                DispatchQty = Convert.ToDouble(ug.Rows[a].Cells["Quantity"].Value);
                                unitprice = Convert.ToDouble(ug.Rows[a].Cells["UnitPrice"].Value);
                                DiscountRate = Convert.ToDouble(ug.Rows[a].Cells["Disc %"].Value) / 100;
                                Amount = (DispatchQty * unitprice);
                                TotalAmount = TotalAmount + Amount;
                                DiscountAmount = Amount * DiscountRate;
                                Amount1 = Amount - DiscountAmount;
                                ug.Rows[a].Cells["DiscLineAmt"].Value = DiscountAmount.ToString("0.00");
                                ug.Rows[a].Cells["TotalPrice"].Value = Amount1.ToString("0.00");
                                TotalDiscAmount = TotalDiscAmount + Convert.ToDouble(ug.Rows[a].Cells["DiscLineAmt"].Value);
                                unitprice_Incl = unitprice + (unitprice * Tax1Amount / 100);
                                unitprice_Incl = unitprice_Incl + (unitprice_Incl * Tax2Amount / 100);
                                ug.Rows[a].Cells["UnitPrice(Incl)"].Value = unitprice_Incl;

                            }
                        }
                    }
                    txtDiscLineTot.Text = TotalDiscAmount.ToString();
                    //txtSubTot.Text = Amount1.ToString("N2");
                    txtSubValue.Text = (Amount1 + double.Parse(txtServicecharge.Text.Trim())).ToString("N2");
                    txtNetValue.Text = TotalAmount.ToString("N2");

                    GrandTotal();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtServicecharge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtServicecharge.Text.Trim(), out amount))
                {
                    txtServicecharge.Text = "0.00";
                    return;
                }

                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscAmount_ValueChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select To Warehouse First");
                    return;
                }

                if (Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" + ug.ActiveRow.Cells["Quantity"].Value.ToString() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            ug.ActiveRow.Cells["Quantity"].Selected = true;
                        }
                    }
                }
                else
                {
                    frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("CreditNote", cmbWarehouse.Text.ToString().Trim(),
                       ug.ActiveRow.Cells["ItemCode"].Value.ToString(),
                       Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()),
                       txtCreditNo.Text.Trim(), blnEdit, clsSerializeItem.DtsSerialNoList, null, true, blnEdit);
                    ObjfrmSerialSubCommon.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (IsThisItemSerial(ug.ActiveRow.Cells[1].Value.ToString().Trim()))
                btnSNO.Enabled = true;
            else
                btnSNO.Enabled = false;
        }

        private void cmbInvoiceType_ValueChanged(object sender, EventArgs e)
        {
            double Tax1Rate = double.Parse(txtNBTPer.Value.ToString().Trim());
            double Tax2Rate = double.Parse(txtVatPer.Value.ToString().Trim());
            InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
            FooterCalculation(); 
        }

        private void txtNBTPer_ValueChanged_1(object sender, EventArgs e)
        {
            InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
            FooterCalculation();

        }

        private void txtDiscPer_ValueChanged_1(object sender, EventArgs e)
        {
            InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
            FooterCalculation();
        }

        private void txtServicecharge_ValueChanged(object sender, EventArgs e)
        {
            InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
            FooterCalculation();
        }

        private void txtVatPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtVatPer.Text.Trim(), out amount))
                {
                    txtVatPer.Text = "0.00";
                    return;
                }

                GrandTotal();
                FooterCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

       

       
























    }
}