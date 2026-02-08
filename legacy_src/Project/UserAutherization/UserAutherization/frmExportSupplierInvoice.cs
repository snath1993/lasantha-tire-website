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
    public partial class frmExportSupplierInvoice : Form
    {
        public static string ConnectionString;
        clsCommon objclsCommon = new clsCommon();
        public static string LineDisitemid, LineDisitemdescription, LineDisGLAccount, SpecialDisItemid, SpecialDisItemdescription, SpecialDisGLAccount, Cashitemid, cashitemdis, cashGL, NBitemid, NBTitemDis, NBTitemGL, VATitemid, VATitemDis, VATGL, FreeValitemid, FreeValitemDis, FreeValitemGL;

        public frmExportSupplierInvoice()
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

                string StrSql = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='1'";

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
                String S3 = "Select distinct SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblDirectSupplierInvoices where IsExport ='" + false + "' group by SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal,GRNNos,IsActive order by SupInvoiceNo ASC";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(dt);


                String S33 = "Select distinct SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where IsExport ='" + false + "' group by SupInvoiceNo, CustomerID, InvoiceDate, Location, NetTotal, GRNNos, IsActive order by SupInvoiceNo ASC";
                SqlCommand cmd33 = new SqlCommand(S33);
                SqlConnection con33 = new SqlConnection(ConnectionString);
                SqlDataAdapter da33 = new SqlDataAdapter(S33, con33);
                da33.Fill(dt);



                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        dgvInvoiceList.Rows.Add();

                        dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

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
                if (cmbSearchby.Text.Trim() == "Invoice No")
                {
                    ware = "SupInvoiceNo";
                }
                else if (cmbSearchby.Text.Trim() == "VendorID")
                {
                    ware = "CustomerID";
                }
                else if (cmbSearchby.Text.Trim() == "GRN NO")
                {
                    ware = "GRNNos";
                }
                else if (cmbSearchby.Text.Trim() == "Location")
                {
                    ware = "Location";
                }

                dgvInvoiceList.Rows.Clear();
                DataTable dt = new DataTable();
                String S3 = "Select distinct SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblDirectSupplierInvoices where IsExport ='" + false + "' and " + ware + " like '%" + txtSearch.Text.Trim() + "%' and InvoiceDate >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and InvoiceDate <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' group by SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal,GRNNos,IsActive order by SupInvoiceNo ASC";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(dt);


                String S33 = "Select distinct SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where IsExport ='" + false + "' and " + ware + " like '%" + txtSearch.Text.Trim() + "%' and InvoiceDate >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and InvoiceDate <= '" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' group by SupInvoiceNo, CustomerID, InvoiceDate, Location, NetTotal, GRNNos, IsActive  order by SupInvoiceNo ASC";
                SqlCommand cmd33 = new SqlCommand(S33);
                SqlConnection con33 = new SqlConnection(ConnectionString);
                SqlDataAdapter da33 = new SqlDataAdapter(S33, con33);
                da33.Fill(dt);



                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        dgvInvoiceList.Rows.Add();

                        dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

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
            if (checkBox1.Checked == true && dgvInvoiceList.Rows.Count - 1 > 0)
            {
                for (int i = 0; i < dgvInvoiceList.Rows.Count - 1; i++)
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
            try
            {
                for (int i = 0; i < dgvInvoiceList.Rows.Count - 1; i++)
                {
                    dt.Clear();
                    if (dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor == System.Drawing.Color.LightBlue)
                    {
                        String StrSql = "SELECT * FROM [tblDirectSupplierInvoices] where InvReferenceNo = '" + dgvInvoiceList.Rows[i].Cells[0].Value.ToString() + "' order by InvReferenceNo";
                        SqlCommand cmd = new SqlCommand(StrSql);
                        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                        da.Fill(dt);

                        //String StrSql2 = "SELECT * FROM [tblSupplierInvoices] where InvReferenceNo = '" + dgvInvoiceList.Rows[i].Cells[0].Value.ToString() + "'  order by InvReferenceNo";
                        //SqlCommand cmd2 = new SqlCommand(StrSql2);
                        //SqlDataAdapter da2 = new SqlDataAdapter(StrSql2, ConnectionString);
                        //da2.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            if (ChechVendorExported(dt.Rows[0]["CustomerID"].ToString().Trim()) == false)
                            {
                                ExportVendor(dt.Rows[0]["CustomerID"].ToString().Trim());
                            }


                            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierInvoice.xml", System.Text.Encoding.UTF8);
                            Writer.Formatting = Formatting.Indented;
                            Writer.WriteStartElement("PAW_Purchases");
                            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                            int DistributionNumber = 0;

                            Writer.WriteStartElement("PAW_Purchase");
                            Writer.WriteAttributeString("xsi:type", "paw:purchase");


                            Writer.WriteStartElement("VendorID");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(dt.Rows[0]["CustomerID"].ToString().Trim());//Vendor ID should be here = Ptient No
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Invoice_Number");
                            Writer.WriteString(dt.Rows[0]["CustomerSO"].ToString().Trim());
                            Writer.WriteEndElement();

                            invno = dt.Rows[0]["InvReferenceNo"].ToString().Trim();

                            Writer.WriteStartElement("Date");
                            Writer.WriteString(dt.Rows[0]["InvoiceRecivedDate"].ToString().Trim());//Date 
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Date_Due");
                            Writer.WriteString(dt.Rows[0]["InvoiceRecivedDate"].ToString().Trim());//Date 
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("CustomerInvoiceNumber");
                            Writer.WriteString(dt.Rows[0]["InvReferenceNo"].ToString().Trim());//Supinv 
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("AP_Account");
                            Writer.WriteString(dt.Rows[0]["APAccount"].ToString().Trim());//Cash Account
                            Writer.WriteEndElement();//CreditMemoType

                            int rowCnt = 0;
                            if (double.Parse(dt.Rows[0]["Tax1Amount"].ToString()) > 0)
                            {
                                rowCnt++;
                            }
                            if (double.Parse(dt.Rows[0]["Tax2Amount"].ToString()) > 0)
                            {
                                rowCnt++;
                            }
                            string NoDistributions = Convert.ToString(dt.Rows.Count + rowCnt);

                            Writer.WriteStartElement("Number_of_Distributions");
                            Writer.WriteString(NoDistributions);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("PurchaseLines");
                            for (int x = 0; x < dt.Rows.Count; x++)
                            {

                                double ConvertedQty = 0;
                                ConvertedQty = Convert.ToDouble(dt.Rows[x]["ConvertedQty"].ToString().Trim()) + Convert.ToDouble(dt.Rows[x]["FreeQty"].ToString().Trim());

                                Writer.WriteStartElement("PurchaseLine");

                                Writer.WriteStartElement("Quantity");
                                Writer.WriteString(ConvertedQty.ToString());
                                Writer.WriteEndElement();


                                Writer.WriteStartElement("Item_ID");
                                Writer.WriteString(dt.Rows[x]["ItemID"].ToString().Trim());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Description");
                                Writer.WriteString(dt.Rows[x]["Description"].ToString().Trim());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("GL_Account");
                                Writer.WriteString(dt.Rows[x]["GLAcount"].ToString().Trim());
                                Writer.WriteEndElement();

                                double lineAMT = 0;
                                double UnitPrice = 0;
                                //  lineAMT = (Convert.ToDouble(dt.Rows[x]["Amount"].ToString()) * (100 - (Convert.ToDouble(dt.Rows[x]["DisCountRate"].ToString()))) / 100) * (100 - (Convert.ToDouble(dt.Rows[x]["DisCountRate1"].ToString()))) / 100;
                              //  double newCost = ((Convert.ToDouble(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()) / dblConverteQty) * (100 - (Convert.ToDouble(txtSpecialDisPer.Value))) / 100) * (100 - (Convert.ToDouble(txtCashDisPer.Value))) / 100 * (100 - (Convert.ToDouble(txtAddiDisPer.Value))) / 100;
                                lineAMT = (Convert.ToDouble(dt.Rows[x]["Amount"].ToString()) * (100 - (Convert.ToDouble(dt.Rows[x]["DisCountRate"].ToString()))) / 100) * (100 - (Convert.ToDouble(dt.Rows[x]["DisCountRate1"].ToString()))) / 100 * (100 - (Convert.ToDouble(dt.Rows[x]["AdditionalDisPer"].ToString()))) / 100;
                                UnitPrice = lineAMT / ConvertedQty;
                                //AdditionalDisPer
                                Writer.WriteStartElement("Unit_Price");
                                Writer.WriteString(UnitPrice.ToString());
                                Writer.WriteEndElement();

                                // double  lineAMT = (Convert.ToDouble(dt.Rows[x]["Amount"].ToString()) * (100 - (Convert.ToDouble(dt.Rows[x]["DisCountRate"].ToString()))) / 100) * (100 - (Convert.ToDouble(dt.Rows[x]["DisCountRate1"].ToString()))) / 100;

                                Writer.WriteStartElement("Amount");
                                Writer.WriteString(lineAMT.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();
                            }


                            if (double.Parse(dt.Rows[0]["Tax1Amount"].ToString()) > 0)
                            {
                                Writer.WriteStartElement("PurchaseLine");

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
                                Writer.WriteString("1");//Doctor Charge
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Amount");
                                Writer.WriteString((Convert.ToDouble(dt.Rows[0]["Tax1Amount"].ToString())).ToString());//tax amount1
                                Writer.WriteEndElement();


                                Writer.WriteEndElement();

                            }


                            if (double.Parse(dt.Rows[0]["Tax2Amount"].ToString()) > 0)
                            {
                                Writer.WriteStartElement("PurchaseLine");

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
                                Writer.WriteString("1");//Doctor Charge
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Amount");
                                Writer.WriteString((Convert.ToDouble(dt.Rows[0]["Tax2Amount"].ToString())).ToString());//tax amount1
                                Writer.WriteEndElement();



                                Writer.WriteEndElement();
                            }

                            Writer.WriteEndElement();
                            Writer.WriteEndElement();
                            Writer.WriteEndElement();
                            Writer.Close();




                        }

                        conn.ImportSupplierInvoice();
                        UpdateExportStatusToSales(dt.Rows[0]["SupInvoiceNo"].ToString());

                        Exportedrows++;
                        lblExportStatus.Text = "Exporting.... " + Exportedrows.ToString() + "/" + rows.ToString();
                    }


                }
                MessageBox.Show("Supplier Invoice file Successfully Exported to Peachtree", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnClear_Click(null, null);

            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ExportVendor(string vid)
        {
            String StrSql = "SELECT * FROM [tblVendorMaster] where VendorID ='" + vid + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                try
                {
                    XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Vendor.xml", System.Text.Encoding.UTF8);
                    Writer.Formatting = Formatting.Indented;
                    Writer.WriteStartElement("PAW_Vendors");
                    Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                    Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                    Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                    Writer.WriteStartElement("PAW_Vendor");
                    Writer.WriteAttributeString("xsi:type", "paw:vendor");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(vid);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Name");
                    Writer.WriteString(dt.Rows[0]["VendorName"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Line1");
                    Writer.WriteString(dt.Rows[0]["VAddress1"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Line2");
                    Writer.WriteString(dt.Rows[0]["VAddress2"].ToString());
                    Writer.WriteEndElement();

                    //  PhoneNumbers>
                    // <PhoneNumber Key="1">777801845</PhoneNumber> 
                    // </PhoneNumbers>
                    Writer.WriteStartElement("PhoneNumbers");

                    Writer.WriteStartElement("PhoneNumber");
                    Writer.WriteAttributeString("Key", "1");
                    // Writer.WriteStartElement("TelePhone_1");
                    Writer.WriteString(dt.Rows[0]["VContact"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("PhoneNumber");
                    Writer.WriteAttributeString("Key", "2");
                    // Writer.WriteStartElement("TelePhone_1");
                    Writer.WriteString(dt.Rows[0]["VContact2"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    // Writer.WriteStartElement("PhoneNumbers");

                    Writer.WriteStartElement("FaxNumber");
                    Writer.WriteString(dt.Rows[0]["Fax"].ToString());
                    Writer.WriteEndElement();

                    //CustomFields
                    Writer.WriteStartElement("CustomFields");
                    // Writer.WriteStartElement("CustomField1");

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "1");//Change time and date both
                    Writer.WriteString(dt.Rows[0]["CustomField1"].ToString());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "2");//Change time and date both
                    Writer.WriteString(dt.Rows[0]["CustomField2"].ToString());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "3");//Change time and date both
                    Writer.WriteString(dt.Rows[0]["CustomField3"].ToString());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "4");//Change time and date both
                    Writer.WriteString(dt.Rows[0]["CustomField4"].ToString());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "5");//Change time and date both
                    Writer.WriteString(dt.Rows[0]["CustomField5"].ToString());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    Writer.WriteEndElement();
                    Writer.Close();

                    Connector Conn = new Connector();
                    Conn.ExportVendorList();

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

            SqlCommand myCommand367 = new SqlCommand("update tblVendorMaster set  IsActive = '" + false + "' where VendorID='" + vid + "'", myConnection, myTrans);
            myCommand367.ExecuteNonQuery();
            myTrans.Commit();
        }

        private bool ChechVendorExported(string vid)
        {
            String StrSql = "SELECT * FROM [tblVendorMaster] where VendorID ='" + vid + "' and IsActive ='" + true + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
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

            SqlCommand myCommand367 = new SqlCommand("update tblDirectSupplierInvoices set IsExport = '" + true + "' , IsActive = '" + false + "' where SupInvoiceNo='" + supinvno + "'", myConnection, myTrans);
            myCommand367.ExecuteNonQuery();

            SqlCommand myCommand368 = new SqlCommand("update tblSupplierInvoices set IsExport = '" + true + "' , IsActive = '" + false + "' where SupInvoiceNo='" + supinvno + "'", myConnection, myTrans);
            myCommand368.ExecuteNonQuery();

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
    }
}
