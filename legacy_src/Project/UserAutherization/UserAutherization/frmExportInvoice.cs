using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace UserAutherization
{
    public partial class frmExportInvoice : Form
    {
        public static string ConnectionString;
        clsCommon objclsCommon = new clsCommon();
        public static string LineDisitemid, LineDisitemdescription, LineDisGLAccount, SpecialDisItemid, SpecialDisItemdescription, SpecialDisGLAccount, Cashitemid, cashitemdis, cashGL, NBitemid, NBTitemDis, NBTitemGL, VATitemid, VATitemDis, VATGL, FreeValitemid, FreeValitemDis, FreeValitemGL;

        private void dgvInvoiceList_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || dgvInvoiceList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                return;
            }
            if (dgvInvoiceList.Rows.Count - 1 > 0)
            {

                if (dgvInvoiceList.CurrentRow.DefaultCellStyle.BackColor != System.Drawing.Color.LightBlue)
                {
                    dgvInvoiceList.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                }

                else
                {
                    dgvInvoiceList.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;

                }

            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        public frmExportInvoice()
        {
            InitializeComponent();
            setConnectionString();
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
        private void frmExportSupplierInvoice_Load(object sender, EventArgs e)
        {
            try
            {
             
                //DataTable dt = DBUtil.clsUtils.loadItems("Select distinct SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal from tblDirectSupplierInvoices group by SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal");
                //if ((dt != null) && (dt.Rows.Count > 0))
                //{
                //    dgvInvoiceList.DataSource = dt;
                //}
                GetChargeItems();
                Load_SuppInvList();
                cmbSearchby.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        private void GetChargeItems()
        {
            try
            {

              string  StrSql = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='1'";

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

                string StrSql6 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='6'";

                SqlCommand cmd6 = new SqlCommand(StrSql6);
                SqlDataAdapter da6 = new SqlDataAdapter(StrSql6, ConnectionString);
                DataTable dt6 = new DataTable();
                dt6.Clear();
                da6.Fill(dt6);
                {
                    FreeValitemid = dt6.Rows[0].ItemArray[0].ToString();
                    FreeValitemDis = dt6.Rows[0].ItemArray[1].ToString();
                    FreeValitemGL = dt6.Rows[0].ItemArray[2].ToString();
                }


                string StrSql5 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='2'";

                SqlCommand cmd5 = new SqlCommand(StrSql5);
                SqlDataAdapter da5 = new SqlDataAdapter(StrSql5, ConnectionString);
                DataTable dt5 = new DataTable();
                dt5.Clear();
                da5.Fill(dt5);
                {
                    SpecialDisItemid = dt5.Rows[0].ItemArray[0].ToString();
                    SpecialDisItemdescription = dt5.Rows[0].ItemArray[1].ToString();
                    SpecialDisGLAccount = dt5.Rows[0].ItemArray[2].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Load_SuppInvList()
        {
            try
            {
                dgvInvoiceList.Rows.Clear();
                DataTable dt = new DataTable();
                String S3 = "select  distinct(InvoiceNo) as InvoiceNo,VehicleNo,InvoiceDate,SONO,'',IsConfirm,IsVoid,IsExport from tblSalesInvoices where IsDirect='True' and IsExport = 'False' and IsVoid='False' order by  InvoiceNo ASC";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvInvoiceList.Rows.Add();
                        dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsConfirm"].ToString().Trim()) == false)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[5].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                            dgvInvoiceList.Rows[i].Cells[5].Value = "Process";

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchOption();
           
        }

        private void SearchOption()
        {
            try
            {
                string ware = "";
                if(cmbSearchby.Text.Trim()== "Invoice No")
                {
                    ware = "InvoiceNo";
                }
                else if(cmbSearchby.Text.Trim() == "Customer ID")
                {
                    ware = "CustomerID";
                }
                else if (cmbSearchby.Text.Trim() == "Vehicle No")
                {
                    ware = "VehicleNo";
                }
                else if (cmbSearchby.Text.Trim() == "Location")
                {
                    ware = "Location";
                }

                dgvInvoiceList.Rows.Clear();
                DataTable dt = new DataTable();
                String S3 = "select  distinct(InvoiceNo) as InvoiceNo,VehicleNo,InvoiceDate,SONO,'',IsConfirm,IsVoid,IsExport from tblSalesInvoices where IsDirect='True' and IsExport = 'False' and IsVoid='False'  and " + ware + " like '%" + txtSearch.Text.Trim() + "%' and InvoiceDate >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and InvoiceDate <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' order by  InvoiceNo ASC";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        dgvInvoiceList.Rows.Add();

                        dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsConfirm"].ToString().Trim()) == false)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[5].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                            dgvInvoiceList.Rows[i].Cells[5].Value = "Process";

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked ==true && dgvInvoiceList.Rows.Count-1>0)
            {
                for (int i = 0; i < dgvInvoiceList.Rows.Count-1; i++)
                {
                    if (dgvInvoiceList.Rows[i].Cells[0].Value != null)
                    {
                        dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;

                    }
                }
            }
            else
            {
                for (int i = 0; i < dgvInvoiceList.Rows.Count - 1; i++)
                {
                    if (dgvInvoiceList.Rows[i].Cells[0].Value != null)
                    {
                        dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;

                    }
                }
            }
        }

        public string invno = "";
        private void btnExportNow_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Export This?", "Message", MessageBoxButtons.OKCancel,MessageBoxIcon.Question);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

            int rows = 0;
            int Exportedrows = 0;
            for (int i = 0; i < dgvInvoiceList.Rows.Count - 1; i++)
            {
                if (dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor == System.Drawing.Color.LightBlue)
                {
                    rows++;
                    if (lblExportStatus.Visible == false)
                    {
                        lblExportStatus.Visible = true;
                        lblExportStatus.Text = "Exporting.... ";
                    }

                }
            }
            Connector conn = new Connector();
            DataTable dt = new DataTable();
            bool IsMpayCash = false;

            try
            {
                for (int i = 0; i < dgvInvoiceList.Rows.Count - 1; i++)
                {
                    dt.Clear();
                    if (dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor == System.Drawing.Color.LightBlue)
                    {
                        String StrSql = "SELECT * FROM [tblSalesInvoices] where InvoiceNo = '" + dgvInvoiceList.Rows[i].Cells[0].Value.ToString() + "' order by InvoiceNo";
                        SqlCommand cmd = new SqlCommand(StrSql);
                        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            if (ChechVendorExported(dt.Rows[0]["CustomerID"].ToString().Trim()) == false)
                            {
                                ExportVendor(dt.Rows[0]["CustomerID"].ToString().Trim());
                            }

                            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml", System.Text.Encoding.UTF8);
                            Writer.Formatting = Formatting.Indented;
                            Writer.WriteStartElement("PAW_Invoices");
                            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                            Writer.WriteStartElement("PAW_Invoice");
                            Writer.WriteAttributeString("xsi:type", "paw:Invoice");

                            Writer.WriteStartElement("Customer_ID");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(dt.Rows[0]["CustomerID"].ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Date");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(dt.Rows[0]["InvoiceDate"].ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Date_Due");
                            Writer.WriteString(dt.Rows[0]["InvoiceDate"].ToString().Trim());//Date 
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Invoice_Number");
                            Writer.WriteString(dt.Rows[0]["InvoiceNo"].ToString().Trim());
                            Writer.WriteEndElement();

                            invno = dt.Rows[0]["InvoiceNo"].ToString().Trim();

                            string crtype = null;
                            if (dt.Rows[0]["PaymentM"].ToString().Trim() == "Cash")
                            {
                                crtype = "Cash";
                            }
                            else
                            {
                                crtype = "Credit";
                            }

                            Writer.WriteStartElement("Ship_Via");
                            Writer.WriteString(crtype);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Sales_Representative_ID");
                            Writer.WriteAttributeString("xsi:type", "paw:id");

                            Writer.WriteString(dt.Rows[0]["SalesRep"].ToString().Trim());

                            //Writer.WriteString(cmbSalesRep.Value.ToString().Trim()); //(MergeUser.ToString());//

                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Accounts_Receivable_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(dt.Rows[0]["ARAccount"].ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("CreditMemoType");
                            Writer.WriteString("FALSE");
                            Writer.WriteEndElement();

                            int vatnbtC = 0;
                            if (Convert.ToDouble(dt.Rows[0]["Tax1Amount"].ToString().Trim()) > 0)
                            {
                                vatnbtC++;

                            }
                            if (Convert.ToDouble(dt.Rows[0]["Tax2Amount"].ToString().Trim()) > 0)
                            {
                                vatnbtC++;

                            }

                            if (Convert.ToDouble(dt.Rows[0]["TotalDiscountAmount"].ToString().Trim()) > 0)
                            {
                                vatnbtC++;

                            }


                            Writer.WriteStartElement("Number_of_Distributions");
                            Writer.WriteString((dt.Rows.Count+vatnbtC).ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("SalesLines");
                            for (int x = 0; x < dt.Rows.Count; x++)
                            {

                                Writer.WriteStartElement("SalesLine");
                                // Writer.WriteEndElement();

                                Writer.WriteStartElement("Quantity");
                                Writer.WriteString(dt.Rows[x]["Qty"].ToString().Trim());
                                Writer.WriteEndElement();



                                Writer.WriteStartElement("Item_ID");
                                Writer.WriteString(dt.Rows[x]["ItemID"].ToString().Trim());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Description");
                                Writer.WriteString(GetItemDescription(dt.Rows[x]["ItemID"].ToString().Trim()));
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("GL_Account");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(dt.Rows[x]["GLAcount"].ToString().Trim());
                                Writer.WriteEndElement();


                                Writer.WriteStartElement("Tax_Type");
                                Writer.WriteString("1");
                                Writer.WriteEndElement();

                                double Amt = 0;

                                if (Convert.ToDouble(dt.Rows[0]["Tax1Amount"].ToString().Trim()) > 0 || Convert.ToDouble(dt.Rows[0]["Tax2Amount"].ToString().Trim()) > 0)
                                {
                                    Amt = Math.Round(Convert.ToDouble(dt.Rows[x]["InclusivePrice"].ToString().Trim()) * Convert.ToDouble(dt.Rows[x]["Qty"].ToString().Trim()), 2, MidpointRounding.AwayFromZero);

                                }
                                else

                                {
                                    Amt = Convert.ToDouble(dt.Rows[x]["Amount"].ToString().Trim());
                                }
                                Writer.WriteStartElement("Amount");
                                Writer.WriteString("-" +Amt);
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();//End of sales line element sanjeewa edited thid code on 04/10/2013

                            }


                            if (Convert.ToDouble(dt.Rows[0]["TotalDiscountAmount"].ToString().Trim()) > 0)
                            {
                                Writer.WriteStartElement("SalesLine");
                                // Writer.WriteEndElement();

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
                                Writer.WriteString(dt.Rows[0]["TotalDiscountAmount"].ToString().Trim());
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();//End of sales line element sanjeewa edited thid code on 04/10/2013
                            }


                            if (Convert.ToDouble(dt.Rows[0]["Tax1Amount"].ToString().Trim()) > 0)
                            {
                                Writer.WriteStartElement("SalesLine");
                                // Writer.WriteEndElement();

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
                                Writer.WriteString("-" + dt.Rows[0]["Tax1Amount"].ToString().Trim());
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();//End of sales line element sanjeewa edited thid code on 04/10/2013
                            }


                            if (Convert.ToDouble(dt.Rows[0]["Tax2Amount"].ToString().Trim()) > 0)
                            {
                                Writer.WriteStartElement("SalesLine");
                                // Writer.WriteEndElement();

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


                                Writer.WriteStartElement("Tax_Type");
                                Writer.WriteString("1");
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Amount");
                                Writer.WriteString("-" + dt.Rows[0]["Tax2Amount"].ToString().Trim());
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();//End of sales line element sanjeewa edited thid code on 04/10/2013
                            }




                            Writer.WriteEndElement();
                            Writer.WriteEndElement();
                            Writer.Close();

                            IsMpayCash = false;


                            if (dt.Rows[0]["PaymentM"].ToString().Trim() == "Cash")
                            {
                                XmlTextWriter Writer2 = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml", System.Text.Encoding.UTF8);
                                Writer2.Formatting = Formatting.Indented;
                                Writer2.WriteStartElement("PAW_Receipts");
                                Writer2.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                                Writer2.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                                Writer2.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                                Writer2.WriteStartElement("PAW_Receipt");
                                Writer2.WriteAttributeString("xsi:type", "paw:Receipt");

                                Writer2.WriteStartElement("Customer_ID");
                                Writer2.WriteAttributeString("xsi:type", "paw:id");
                                Writer2.WriteString(dt.Rows[0]["CustomerID"].ToString().Trim());//Customer ID should be here = Ptient No
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("Reference");
                                Writer2.WriteString(dt.Rows[0]["InvoiceNo"].ToString().Trim() + "R");
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("Sales_Representative_ID");
                                Writer2.WriteAttributeString("xsi:type", "paw:id");
                                Writer2.WriteString(dt.Rows[0]["SalesRep"].ToString().Trim());
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("Date");
                                Writer2.WriteAttributeString("xsi:type", "paw:id");
                                Writer2.WriteString(dt.Rows[0]["InvoiceDate"].ToString().Trim());//Date 
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("Payment_Method");
                                Writer2.WriteString("Cash");//PayMethod
                                Writer2.WriteEndElement();

                                string cardAccount = "";
                                String S = "Select GL_Account from tblCreditData where CardType='Cash'";
                                SqlCommand cmd1 = new SqlCommand(S);
                                SqlDataAdapter da1 = new SqlDataAdapter(S, ConnectionString);
                                DataSet dt1 = new DataSet();
                                da1.Fill(dt1);
                                cardAccount = dt1.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                                Writer2.WriteStartElement("Cash_Account");
                                Writer2.WriteAttributeString("xsi:type", "paw:id");
                                Writer2.WriteString(cardAccount);//Cash Account
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("Total_Paid_On_Invoices");
                                Writer2.WriteString("-"+dt.Rows[0]["NetTotal"].ToString().Trim());//PayMethod
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("ReceiptNumber");
                                Writer2.WriteString("R" + dt.Rows[0]["InvoiceNo"].ToString().Trim());
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("Number_of_Distributions ");
                                Writer2.WriteString("1");
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("Distributions");
                                Writer2.WriteStartElement("Distribution");

                                Writer2.WriteStartElement("InvoicePaid");
                                Writer2.WriteString(dt.Rows[0]["InvoiceNo"].ToString().Trim());//PayMethod
                                Writer2.WriteEndElement();

                                Writer2.WriteStartElement("Amount");
                                Writer2.WriteString("-"+dt.Rows[0]["NetTotal"].ToString().Trim());//PayMethod
                                Writer2.WriteEndElement();

                                Writer2.WriteEndElement();
                                Writer2.WriteEndElement();


                                Writer2.WriteEndElement();

                                Writer2.WriteEndElement();

                                Writer2.Close();
                            }

                            else if(dt.Rows[0]["PaymentM"].ToString().Trim() == "CreditCard")
                            {
                                String S22 = "select * from tblInvoicePayTypes where InvoiceNo in (select InvoiceNo from tblInvoicePayTypes group by InvoiceNo having count(*) > 1) and InvoiceNo='"+ dt.Rows[0]["InvoiceNo"].ToString().Trim() + "' and CardType='CASH'";
                                SqlCommand cmd22 = new SqlCommand(S22);
                                SqlDataAdapter da22 = new SqlDataAdapter(S22, ConnectionString);
                                DataSet dt22 = new DataSet();
                                da22.Fill(dt22);
                                if(dt22.Tables[0].Rows.Count>0)
                                {
                                    if(dt22.Tables[0].Rows[0].ItemArray[0].ToString().Trim()!="")
                                    {
                                        IsMpayCash = true;
                                        XmlTextWriter Writer2 = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml", System.Text.Encoding.UTF8);
                                        Writer2.Formatting = Formatting.Indented;
                                        Writer2.WriteStartElement("PAW_Receipts");
                                        Writer2.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                                        Writer2.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                                        Writer2.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                                        Writer2.WriteStartElement("PAW_Receipt");
                                        Writer2.WriteAttributeString("xsi:type", "paw:Receipt");

                                        Writer2.WriteStartElement("Customer_ID");
                                        Writer2.WriteAttributeString("xsi:type", "paw:id");
                                        Writer2.WriteString(dt.Rows[0]["CustomerID"].ToString().Trim());//Customer ID should be here = Ptient No
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("Reference");
                                        Writer2.WriteString(dt.Rows[0]["InvoiceNo"].ToString().Trim() + "R");
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("Sales_Representative_ID");
                                        Writer2.WriteAttributeString("xsi:type", "paw:id");
                                        Writer2.WriteString(dt.Rows[0]["SalesRep"].ToString().Trim());
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("Date");
                                        Writer2.WriteAttributeString("xsi:type", "paw:id");
                                        Writer2.WriteString(dt.Rows[0]["InvoiceDate"].ToString().Trim());//Date 
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("Payment_Method");
                                        Writer2.WriteString("Cash");//PayMethod
                                        Writer2.WriteEndElement();

                                        string cardAccount = "";
                                        String S = "Select GL_Account from tblCreditData where CardType='Cash'";
                                        SqlCommand cmd1 = new SqlCommand(S);
                                        SqlDataAdapter da1 = new SqlDataAdapter(S, ConnectionString);
                                        DataSet dt1 = new DataSet();
                                        da1.Fill(dt1);
                                        cardAccount = dt1.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                                        Writer2.WriteStartElement("Cash_Account");
                                        Writer2.WriteAttributeString("xsi:type", "paw:id");
                                        Writer2.WriteString(cardAccount);//Cash Account
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("Total_Paid_On_Invoices");
                                        Writer2.WriteString("-" + dt22.Tables[0].Rows[0]["Amount"].ToString().Trim());//PayMethod
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("ReceiptNumber");
                                        Writer2.WriteString("R" + dt.Rows[0]["InvoiceNo"].ToString().Trim());
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("Number_of_Distributions ");
                                        Writer2.WriteString("1");
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("Distributions");
                                        Writer2.WriteStartElement("Distribution");

                                        Writer2.WriteStartElement("InvoicePaid");
                                        Writer2.WriteString(dt.Rows[0]["InvoiceNo"].ToString().Trim());//PayMethod
                                        Writer2.WriteEndElement();

                                        Writer2.WriteStartElement("Amount");
                                        Writer2.WriteString("-" + dt22.Tables[0].Rows[0]["Amount"].ToString().Trim());//PayMethod
                                        Writer2.WriteEndElement();

                                        Writer2.WriteEndElement();
                                        Writer2.WriteEndElement();


                                        Writer2.WriteEndElement();

                                        Writer2.WriteEndElement();

                                        Writer2.Close();
                                    }
                                }
                            }
                        }

                        conn.ImportDirectSalesInvice();

                        if (dt.Rows[0]["PaymentM"].ToString().Trim() == "Cash")
                        {
                            conn.Import_Receipt_Journal();
                        }

                        if (dt.Rows[0]["PaymentM"].ToString().Trim() == "CreditCard")
                        {
                            if (IsMpayCash)
                            {
                                conn.Import_Receipt_Journal();
                            }
                        }

                        UpdateExportStatusToSales(dt.Rows[0]["InvoiceNo"].ToString());
                        Exportedrows++;

                        lblExportStatus.Text = "Exporting.... " + Exportedrows.ToString() + "/" + rows.ToString();
                    }


                }
                MessageBox.Show("Sales Invoices file Successfully Exported to Peachtree", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnClear_Click(null, null);


            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SearchOption();
            }          

        }

        private string GetItemDescription(string ItemId)
        {

           
            try
            {
                String StrSql = "SELECT [ItemDescription] FROM [tblItemMaster] where  ItemID ='" + ItemId + "'";
                SqlCommand command = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
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

        private void ExportVendor(string vid)
        {
            String StrSql = "SELECT * FROM [tblCustomerMaster] where CutomerID ='" + vid + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                try
                {
                    XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Customer.xml", System.Text.Encoding.UTF8);
                    Writer.Formatting = Formatting.Indented;
                    Writer.WriteStartElement("PAW_Customers");
                    Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                    Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                    Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                    Writer.WriteStartElement("PAW_Customer");
                    Writer.WriteAttributeString("xsi:type", "paw:Customer");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dt.Rows[0]["CutomerID"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Name");
                    Writer.WriteString(dt.Rows[0]["CustomerName"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Line1");
                    Writer.WriteString(dt.Rows[0]["Address1"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Line2");
                    Writer.WriteString(dt.Rows[0]["Address2"].ToString().Trim());
                    Writer.WriteEndElement();

                    //  PhoneNumbers>
                    // <PhoneNumber Key="1">777801845</PhoneNumber> 
                    // </PhoneNumbers>
                    Writer.WriteStartElement("PhoneNumbers");

                    Writer.WriteStartElement("PhoneNumber");
                    Writer.WriteAttributeString("Key", "1");
                    // Writer.WriteStartElement("TelePhone_1");
                    Writer.WriteString(dt.Rows[0]["Phone1"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("PhoneNumber");
                    Writer.WriteAttributeString("Key", "2");
                    // Writer.WriteStartElement("TelePhone_1");
                    Writer.WriteString(dt.Rows[0]["Phone2"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    // Writer.WriteStartElement("PhoneNumbers");

                    Writer.WriteStartElement("Customer_Type");
                    Writer.WriteString(dt.Rows[0]["Cus_Type"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("FaxNumber");
                    Writer.WriteString(dt.Rows[0]["Fax"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("EMail_Address");
                    Writer.WriteString(dt.Rows[0]["Email"].ToString().Trim());
                    Writer.WriteEndElement();

                    //CustomFields
                    Writer.WriteStartElement("CustomFields");
                    // Writer.WriteStartElement("CustomField1");

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "1");//Change time and date both
                    Writer.WriteString(dt.Rows[0]["Cus_Type"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "2");//Change time and date both
                    Writer.WriteString(dt.Rows[0]["VATNo"].ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    Writer.WriteEndElement();
                    Writer.Close();
                    Connector Conn = new Connector();
                    Conn.ExportCustomerList();

                    UpdateVendorExportStatus(vid);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void UpdateVendorExportStatus(string vid)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            myConnection.Open();
            myTrans = myConnection.BeginTransaction();

            SqlCommand myCommand367 = new SqlCommand("update tblCustomerMaster set  IsActive = '" + false + "' where CutomerID='" + vid + "'", myConnection, myTrans);
            myCommand367.ExecuteNonQuery();
            myTrans.Commit();
        }

        private bool ChechVendorExported(string vid)
        {
            String StrSql = "SELECT * FROM [tblCustomerMaster] where CutomerID ='" + vid+"' and IsActive ='"+true+"'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if(dt.Rows.Count>0)
            {
                return false;
            }
            return true;
        }

        private void UpdateExportStatusToSales(string supinvno)
        {
            
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            myConnection.Open();
            myTrans = myConnection.BeginTransaction();
        
            SqlCommand myCommand367 = new SqlCommand("update tblSalesInvoices set IsExport = '" + true + "' , IsConfirm = '" + true + "' where InvoiceNo='"+supinvno+"'", myConnection, myTrans);
            myCommand367.ExecuteNonQuery();
            myTrans.Commit();
         


        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                //DataTable dt = DBUtil.clsUtils.loadItems("Select distinct SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal from tblDirectSupplierInvoices group by SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal");
                //if ((dt != null) && (dt.Rows.Count > 0))
                //{
                //    dgvInvoiceList.DataSource = dt;
                //}
                Load_SuppInvList();
                cmbSearchby.SelectedIndex = 0;
                checkBox1.Checked = false;
                lblExportStatus.Visible = false;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
       
              
    private void dgvInvoiceList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        
        }
    }
}
