using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using UserAutherization;
using DataAccess;
using System.Xml;
using System.Text.RegularExpressions;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class InpatientMaster : Form
    {
        public static bool isedit = false;
        public dsInpation ds = new dsInpation();
        public InpatientMaster()
        {
            InitializeComponent();
            setConnectionString();
            getpaymentmode();
        }
        public static string ConnectionString;
        public static DateTime UserWiseDate = System.DateTime.Now;


        //public EtuDetails AddF = new EtuDetails();
        private void getpaymentmode()
        {

            DataSet dspaymetmode = new DataSet();
            try
            {
                dspaymetmode.Clear();
                StrSql = "SELECT [CardType] As PaymentMethod,[GL_Account] FROM [tblCreditData]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dspaymetmode, "DtClient");

                cmbCashAccount.DataSource = dspaymetmode.Tables["DtClient"];
                cmbCashAccount.DisplayMember = "PaymentMethod";
                cmbCashAccount.ValueMember = "GL_Account";
                cmbCashAccount.DisplayLayout.Bands["DtClient"].Columns["GL_Account"].Hidden = true;


            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        public string sMsg = "Patient Registration";

        private void ClearData()
        {
            txtTime.Enabled = true;
            btnSave.Enabled = true;
            cmbPatientType.Enabled = true;
            txtSearch.Enabled = false;
            txtAddress1.Text = "";
            txtAddress1.Enabled = true;
            txtAddress2.Text = "";
            txtAddress2.Enabled = true;
            txtAge.Text = "";
            txtAge.Enabled = true;
            txtContactNO.Text = "";
            txtContactNO.Enabled = true;
            txtFirstName.Text = "";
            txtFirstName.Enabled = true;
            txtPatientNo.Text = "";
            cmbCinsultant.Text = "";
            cmbCinsultant.Enabled = true;
            cmbroom.Text = "";
            cmbroom.Enabled = true;
            btnAdmit.Enabled = true;
            txtRate.Text = "0.00";
            txtRate.Enabled = true;
            cmbSex.Text = "";
            cmbSex.Enabled = true;
            txtNIC.Text = "";
            txtNIC.Enabled = true;
            txtCompany.Text = "";
            txtCompany.Enabled = true;
            txtDeposit.Text = "0.00";
            txtDeposit.Enabled = true;
            txtFirstName.Focus();
            dtpAdmiDate.Enabled = true;
            dtpAdmiDate.Value = System.DateTime.Now;
            txtTime.Text = System.DateTime.Now.ToShortTimeString();
            cmbPatientType.ResetText();
            cmbCashAccount.Enabled = true;
            cmbCashAccount.Text = "";



        }



        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearData();
            btnEdit.Enabled = false;
            isedit = false;
        }

        public string getNextID(string s)
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
                    }
                }
                i--;


                return s + "1";
            }
            else
            {

                return null;
            }

        }


        //==========================================================================================


        //public void exporetReceipt()
        //{
        //    //Create a Xmal File..................................................................................

        //    try
        //    {
        //        //Receipts2.xml
        //        XmlTextWriter Writer = new XmlTextWriter(@"c:\\Receipts2.xml", System.Text.Encoding.UTF8);
        //        Writer.Formatting = Formatting.Indented;
        //        Writer.WriteStartElement("PAW_Receipts");
        //        Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
        //        Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
        //        Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

        //       // string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);
        //        string NoDistributions = "1";
        //       // double discountRate = Convert.ToDouble(txtdisRate.Text) / 100;

        //       // for (int i = 0; i <= dgvScanItems.Rows.Count - 2; i++)
        //       // {
        //            Writer.WriteStartElement("PAW_Receipt");
        //            Writer.WriteAttributeString("xsi:type", "paw:Receipt");


        //            Writer.WriteStartElement("Customer_ID");
        //            Writer.WriteAttributeString("xsi:type", "paw:id");
        //            Writer.WriteString(txtPatientNo.Text.ToString().Trim());//Customer ID should be here = Ptient No
        //            Writer.WriteEndElement();


        //            Writer.WriteStartElement("Reference");
        //            Writer.WriteString(txtPatientNo.Text.ToString().Trim() + "BHT");
        //            Writer.WriteEndElement();


        //            Writer.WriteStartElement("Date ");
        //            Writer.WriteAttributeString("xsi:type", "paw:id");
        //            Writer.WriteString(dtpAdmiDate.Text.ToString().Trim());//Date 
        //            Writer.WriteEndElement();


        //            Writer.WriteStartElement("Payment_Method");
        //            Writer.WriteString("Cash");//PayMethod
        //            Writer.WriteEndElement();

        //            Writer.WriteStartElement("Cash_Account");
        //            Writer.WriteAttributeString("xsi:type", "paw:id");
        //            Writer.WriteString("70010");//Cash Account
        //            Writer.WriteEndElement();



        //            Writer.WriteStartElement("Number_of_Distributions ");
        //            Writer.WriteString(NoDistributions);
        //            Writer.WriteEndElement();


        //            Writer.WriteStartElement("InvoicePaid");
        //            Writer.WriteString("");//PayMethod
        //            Writer.WriteEndElement();


        //            Writer.WriteStartElement("Quantity");
        //            Writer.WriteString("1");//Doctor Charge
        //            Writer.WriteEndElement();


        //            Writer.WriteStartElement("Item_ID");
        //            Writer.WriteString("D00001");
        //            Writer.WriteEndElement();

        //            Writer.WriteStartElement("Description");
        //            Writer.WriteString("Deposit Amount");
        //            Writer.WriteEndElement();

        //            Writer.WriteStartElement("GL_Account ");
        //            Writer.WriteAttributeString("xsi:type", "paw:id");
        //            Writer.WriteString("10008");
        //            Writer.WriteEndElement();


        //            //========================================================
        //            Writer.WriteStartElement("Tax_Type");
        //            Writer.WriteString("1");//Doctor Charge
        //            Writer.WriteEndElement();

        //            //double Amount = Convert.ToDouble(dgvScanItems[5, i].Value);
        //            //double DiscountAmount = Amount * discountRate;
        //            //double ItemAmount = Amount - DiscountAmount;

        //            Writer.WriteStartElement("Amount");
        //            Writer.WriteString("-" + txtDeposit.Text.ToString().Trim());//HospitalCharge
        //            Writer.WriteEndElement();


        //            Writer.WriteStartElement("Transaction_Number");
        //            Writer.WriteString("1");
        //            Writer.WriteEndElement();

        //            Writer.WriteStartElement("ReceiptNumber");
        //            Writer.WriteString(txtPatientNo.Text.ToString().Trim() + "R");
        //            Writer.WriteEndElement();

        //            Writer.WriteEndElement();
        //            // Writer.Close();
        //     //   }

        //        Writer.Close();

        //        Connector abc = new Connector();//export to peach tree
        //        abc.Import_Receipt_JournalOnline1();
        //    }

        //    catch { }


        //}

        //==========================================================================================

        public int dcheck = 1;
        public int ConCheck = 0;
        private void btnAdmit_Click(object sender, EventArgs e)
        {

            try
            {
                //  Connector impo1 = new Connector();
                //impo1.ExportCustomerList();
                //impo1.ExportCustomerListBHT();

                //===============================================

                try
                {
                    string abc = cmbCinsultant.Text.ToString().Trim();

                    string BlockC = "False";
                    //  string ctype = "opd";

                    String S = "Select ConsultantName from tblConsultantMaster Where (Block='" + BlockC + "') AND (ConsultantName='" + abc + "')";
                    SqlCommand cmd1 = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                        ConCheck = 1;
                    }
                    else
                    {
                        ConCheck = 0;
                    }
                }
                catch { }


                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    cmbCinsultant.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                //}
                ////=================================================




                string ConnString = ConnectionString;
                // string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
                string sql = "Select PatientNO from tblItemMaster2  ORDER BY PatientNO";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    int p = ds.Tables[0].Rows.Count - 1;
                    // AppointmentNo = Convert.ToInt32(ds.Tables[0].Rows[p].ItemArray[0]);
                    string AppointmentNo = ds.Tables[0].Rows[p].ItemArray[0].ToString();
                    string NewID = getNextID(AppointmentNo);

                    // txtReceiptNo.Text = NewID;
                    txtPatientNo.Text = NewID;
                }
                else
                {
                    // txtReceiptNo.Text = "ET-100000";
                    txtPatientNo.Text = "IUI-1000000";
                }
            }

            catch { }

            try
            {

                double Deposit = 0.00;
                Deposit = Convert.ToDouble(txtDeposit.Text);
                dcheck = 1;
            }

            catch
            {
                dcheck = 2;
                //btnAdmit.Focus();
            }




            //======================================================
            if (txtPatientNo.Text == "" || txtAddress2.Text == "" || txtAddress1.Text == "" || txtFirstName.Text == "" || txtAge.Text == "" || txtContactNO.Text == "" || cmbCinsultant.Text == "" || cmbroom.Text == "" || txtAge.Text == "" || txtCompany.Text == "" || txtNIC.Text == "" || cmbSex.Text == "" || txtRate.Text == "" || txtDeposit.Text == "" || dcheck == 2 || txtDeposit.Text == "0" || ConCheck == 1)
            {

                if (dcheck == 2)
                {
                    MessageBox.Show("Deposit Amount Must Be a Number");
                    btnAdmit.Focus();
                }
                else if (ConCheck == 1)
                {
                    MessageBox.Show("Select Valid Doctor");
                    btnAdmit.Focus();
                }
                else
                {

                    MessageBox.Show("Enter all Details");
                    btnAdmit.Focus();
                }

            }
            else
            {
                //==============================================================

                try
                {
                    XmlTextWriter Writer = new XmlTextWriter(@"c:\\pbss\\CustomerList1.xml", System.Text.Encoding.UTF8);
                    Writer.Formatting = Formatting.Indented;
                    Writer.WriteStartElement("PAW_Customers");
                    Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                    Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                    Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                    Writer.WriteStartElement("PAW_Customer");
                    Writer.WriteAttributeString("xsi:type", "paw:Customer");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(txtPatientNo.Text.ToString().Trim());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Name");
                    Writer.WriteString(txtFirstName.Text.ToString().Trim());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("BillToAddress");

                    Writer.WriteStartElement("Line1");
                    Writer.WriteString(txtAddress1.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Line2");
                    Writer.WriteString(txtAddress2.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();//end of addresses

                    //PhoneNumbers

                    Writer.WriteStartElement("PhoneNumbers");

                    Writer.WriteStartElement("PhoneNumber");
                    Writer.WriteAttributeString("Key", "1");
                    Writer.WriteString(txtContactNO.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Customer_Type");
                    Writer.WriteString("BHT");
                    Writer.WriteEndElement();

                    // CustomFields

                    Writer.WriteStartElement("CustomFields");

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "1");//Change time and date both
                    Writer.WriteString(dtpAdmiDate.Text.ToString().Trim() + "-" + txtTime.Text.ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index", "2");
                    Writer.WriteString(cmbCinsultant.Text.ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CustomField");
                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index", "3");
                    Writer.WriteString(cmbroom.Text.ToString().Trim());
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();


                    Writer.WriteEndElement();
                    Writer.WriteEndElement();
                    Writer.Close();

                }

                catch
                {

                }

                //Connector impo1 = new Connector();
                //impo1.ExportCustomerList();

                //===============================================================
                //string Refund = "No";
                string Export = "No";
                string CurrentUser = user.userName.ToString().Trim();
                string Type = "IUI";
                string DepsitPay = "False";
                // string Discharge = "No";

                try
                {
                    string S = "Insert into tblItemMaster2(Date,Time,PatientNO,Name,Age,Adderess1,Address2,ContactNO,Doctor,Room,PatientType,IsExport,CurrentUser,Sex,Company,NIC,RateInfo,Deposit,ISDepositPaid) values('" + dtpAdmiDate.Text.ToString().Trim() + "','" + txtTime.Text.ToString().Trim() + "','" + txtPatientNo.Text.ToString().Trim() + "','" + txtFirstName.Text.ToString().Trim() + "','" + txtAge.Text.ToString().Trim() + "','" + txtAddress1.Text.ToString().Trim() + "','" + txtAddress2.Text.ToString().Trim() + "','" + txtContactNO.Text.ToString().Trim() + "','" + cmbCinsultant.Text.ToString().Trim() + "','" + cmbroom.Text.ToString().Trim() + "','" + Type + "', '" + Export + "','" + CurrentUser + "','" + cmbSex.Text.ToString().Trim() + "','" + txtCompany.Text.ToString().Trim() + "','" + txtNIC.Text.ToString().Trim() + "','" + txtRate.Text.ToString().Trim() + "','" + txtDeposit.Text.ToString().Trim() + "','" + DepsitPay + "')";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt2 = new DataTable();
                    da.Fill(dt2);
                    // MessageBox.Show("Addmission done Successfully");
                    // }
                    //  catch { }

                    string CustomerType = "ETU Patient";
                    // try
                    // {
                    string S1 = "Insert into tblPatientsDetails(PatientNo,FirstName,Address,ContactNo,CustomerType) values('" + txtPatientNo.Text.ToString().Trim() + "','" + txtFirstName.Text.ToString().Trim() + "','" + txtAddress1.Text.ToString().Trim() + "','" + txtContactNO.Text.ToString().Trim() + "','" + CustomerType + "')";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt21 = new DataTable();
                    da1.Fill(dt21);


                    //Connector impo1 = new Connector();
                    //impo1.ExportCustomerListBHT();


                    // exporetReceipt();


                    MessageBox.Show("Addmission  Successfully Your IUI No is ='" + txtPatientNo.Text.ToString().Trim() + "'");
                    // btnReprint_Click(sender, e);
                    btnNew_Click(sender, e);

                }
                catch { }
                txtTime.Text = System.DateTime.Now.ToShortTimeString();
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        public void RoomDataload()
        {
            // string BlockC = "False";
            String S = "Select RoomNo from tblRoomDetails";// Where Block='" + BlockC + "'";

            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                cmbroom.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            }

        }
        public string StrSql = null;
        public DataSet dsPType;
        public DataSet dsConMaster;
        public void GetPtypeMaswterData()
        {
            dsPType = new DataSet();
            try
            {
                dsPType.Clear();
                StrSql = " SELECT TypeID, TypeName FROM tblPatientMasterType order by TypeID";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsPType, "dtType");

                cmbPatientType.DataSource = dsPType.Tables["dtType"];
                cmbPatientType.DisplayMember = "TypeName";
                cmbPatientType.ValueMember = "TypeName";
                cmbPatientType.DisplayLayout.Bands["dtType"].Columns["TypeID"].Width = 50;
                cmbPatientType.DisplayLayout.Bands["dtType"].Columns["TypeName"].Width = 100;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void GetConsultantMaster()
        {
            dsConMaster = new DataSet();
            try
            {
                
                dsConMaster.Clear();
                StrSql = " SELECT Name, Type FROM tblDoctorMaster WHere Name IS NOT NULL ORDER by Name";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsConMaster, "dtConsultant");

                cmbCinsultant.DataSource = dsConMaster.Tables["dtConsultant"];
                cmbCinsultant.DisplayMember = "Name";
                cmbCinsultant.ValueMember = "Name";
                cmbCinsultant.DisplayLayout.Bands["dtConsultant"].Columns["Name"].Width = 200;
                cmbCinsultant.DisplayLayout.Bands["dtConsultant"].Columns["Type"].Width = 100;
               // cmbCinsultant.DisplayLayout.Bands["dtConsultant"].Columns["ConsultantType"].Width = 150;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        //public void ConsultanLOad()
        //{
        //    string BlockC = "False";
        //    string ctype = "opd";
        //    String S = "Select ConsultantName from tblConsultantMaster Where Block='" + BlockC + "'";// AND (ConsultantType='" + ctype + "')";

        //    SqlCommand cmd = new SqlCommand(S);
        //    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //    DataSet dt = new DataSet();
        //    da.Fill(dt);

        //    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //    {
        //        cmbCinsultant.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //    }

        //}
        private void InpatientMaster_Load(object sender, EventArgs e)
        {
            try
            {


                dtpAdmiDate.Value = System.DateTime.Now;

                txtTime.Text = System.DateTime.Now.ToShortTimeString();
                GetPtypeMaswterData();
                GetConsultantMaster();
                //ConsultanLOad();
                RoomDataload();
                btnEdit.Enabled = false;

                DateTime a = System.DateTime.Now;
                string abc = a.Month + "/" + a.Day + "/" + a.Year;

            }
            catch { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    {
            //        AddF.Clear();

            //        //String S1 = "Select * from tblChannelingDetails where ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "' AND Refund !='" + Ref + "'";
            //        String S1 = "Select * from tblItemMaster2 where PatientNO = '" + txtPatientNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
            //        SqlCommand cmd1 = new SqlCommand(S1);
            //        SqlConnection con1 = new SqlConnection(ConnectionString);
            //        SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
            //        // da1.Fill(AddF, "dtAdminForm");//WardAddmin
            //        da1.Fill(AddF, "dtAdminForm");//WardAddmin

            //        frmETUAddForm frm2 = new frmETUAddForm(this);
            //        frm2.Show();
            //    }
            //catch { }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string add = txtSearch.Text.ToString().Trim();

            String S2 = "Select * from tblItemMaster2 where PatientNO ='" + add + "'";
            SqlCommand cmd2 = new SqlCommand(S2);
            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            DataTable dt = new DataTable();
            da2.Fill(dt);

            if (dt.Rows.Count > 0)
            {

                dtpAdmiDate.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                txtTime.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                txtPatientNo.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                txtFirstName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                txtAge.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                txtAddress1.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                txtAddress2.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
                txtContactNO.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
                cmbCinsultant.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                cmbroom.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
                cmbSex.Text = dt.Rows[0].ItemArray[13].ToString().Trim();
                txtCompany.Text = dt.Rows[0].ItemArray[14].ToString().Trim();
                txtNIC.Text = dt.Rows[0].ItemArray[15].ToString().Trim();
                txtRate.Text = dt.Rows[0].ItemArray[16].ToString().Trim();
                txtDeposit.Text = dt.Rows[0].ItemArray[17].ToString().Trim();

            }
            else

            {
                dtpAdmiDate.Text = "";
                txtTime.Text = "";
                txtPatientNo.Text = "";
                txtFirstName.Text = "";
                txtAge.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtContactNO.Text = "";
                cmbCinsultant.Text = "";
                cmbroom.Text = "";
                cmbSex.Text = "";
                txtCompany.Text = "";
                txtNIC.Text = "";
                txtRate.Text = "";
                txtDeposit.Text = "";

            }

            txtFirstName.Enabled = false;
            txtAge.Enabled = false;
            txtAddress1.Enabled = false;
            txtAddress2.Enabled = false;
            txtContactNO.Enabled = false;
            cmbCinsultant.Enabled = false;
            cmbroom.Enabled = false;
            cmbSex.Enabled = false;
            txtCompany.Enabled = false;
            txtNIC.Enabled = false;
            txtRate.Enabled = false;
            txtDeposit.Enabled = false;

        }

        private void txtRate_Leave(object sender, EventArgs e)
        {
            btnAdmit.Focus();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbCinsultant_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
                if (cmbPatientType.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please select Patient Type", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtFirstName.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Enter Patient Name", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtContactNO.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Enter Patient Contact No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbCinsultant.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Enter Select a Doctor Name", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtAge.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Enter Patient Age", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbSex.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Select Gender", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbPatientType.Text.ToString() == "WARD")
                {
                    if (txtAddress1.Text.ToString() == string.Empty)
                    {
                        MessageBox.Show("Please Enter Patient Address", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (txtCompany.Text.ToString() == string.Empty)
                    {
                        MessageBox.Show("Please Enter Patient Company Name", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (Convert.ToDouble(txtAge.Text.ToString().Trim()) > 18)
                    {
                        if (txtNIC.Text.ToString() == string.Empty)
                        {
                            MessageBox.Show("Please Enter Patient NIC # or Passport # or Driving Licence #", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (cmbroom.Text.ToString() == string.Empty)
                    {
                        MessageBox.Show("Please Select a Room ", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (Convert.ToDouble(txtRate.Text.ToString()) <=0)
                    {
                        MessageBox.Show("Please Enter Ward Rate", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (Convert.ToDouble(txtDeposit.Text.ToString()) <= 0)
                    {
                        MessageBox.Show("Please Enter Deposit amount", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (cmbCashAccount.Text.ToString() == string.Empty)
                    {
                        MessageBox.Show("Please Select a Payment Type ", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                }
                //Connector objConnector = new Connector();
                //if (!(objConnector.IsOpenPeachtree(dtpAdmiDate.Value)))
                //    return;
                if (isedit == false)
                {
                    SaveEvent();
                }
                else
                {
                    UpdateEvent();
                    isedit = false;
                }

            btnPrint_Click(null, null);
            ClearData();
        }
        private void SaveScanchanel(SqlConnection con, SqlTransaction trans)
        {
            String S12 = "insert into [tblScanChannel](ConsultFee,HospitalFee,TotalFee,ReceiptTotal,IsConfirm,IsExport,DueDate,GLAccount,ItemDescription,ItemID,Consultant,TokenNo,[ReceiptNo],[Date],[PatientName],[ContactNo],[PaymentMethod],[NetTotal],[PatientNo],[PationType],[IsVoid],DistributionNumber,CurrentUser,[ItemType],ReferedDoctor,CollectTime)" +
                   "values ('0','" + txtDeposit.Text.ToString() + "','" + txtDeposit.Text.ToString() + "','" + txtDeposit.Text.ToString() + "','TRUE','FALSE','" + dtpAdmiDate.Value.ToShortDateString() + "','10210','DEPOSIT AMOUNT','DPA','FINAL','1','" + txtPatientNo.Text.ToString() + "R','" + dtpAdmiDate.Value.ToShortDateString() + "', '" + txtFirstName.Text.ToString() + "','" + txtContactNO.Text.ToString() + "','" + cmbCashAccount.Text.ToString() + "','" + txtDeposit.Text.ToString() + "','" + txtPatientNo.Text.ToString() + "','WARD','False','1','" + user.userName + "','WARD','"+cmbCinsultant.Text.ToString()+"','"+System.DateTime.Now.ToString("HH:mm:ss")+"')";
            SqlCommand cmd12 = new SqlCommand(S12, con, trans);
            cmd12.ExecuteNonQuery();
        }
        private void dltScanchanel(SqlConnection con, SqlTransaction trans)
        {
            String S12 = "DELETE FROM [tblScanChannel] WHERE [ReceiptNo] ='" + txtPatientNo.Text.ToString() + "R'";
                SqlCommand cmd12 = new SqlCommand(S12, con, trans);
            cmd12.ExecuteNonQuery();
        }
        private void UpdateEvent()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to Update this record ? ", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                //DateTime Date1 = System.DateTime.Now;
                //string Dformat = "MM/dd/yyyy";
                //string GRNDate = dtpAdmiDate

                //string CurretDate = GRNDate;
                //string CurrentTime = System.DateTime.Now.ToShortTimeString();

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                string StrSql1 = "DELETE FROM tblCustomerMaster WHERE CutomerID='" + txtPatientNo.Text.ToString().Trim() + "'";
                SqlCommand command1 = new SqlCommand(StrSql1, myConnection, myTrans);
                command1.CommandType = CommandType.Text;
                command1.ExecuteNonQuery();


                StrSql = "INSERT INTO[dbo].[tblCustomerMaster]([CutomerID],[CustomerName],[Address1],[Address2],[Phone1],[Custom1],[Custom2],[Custom3],[Custom4],[Custom5],[Cus_Type]) " +
                       " VALUES ('" + txtPatientNo.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtAddress1.Text.ToString().Trim() + "', " +
                     " '" + txtAddress2.Text.ToString().Trim() + "', '" + txtContactNO.Text.ToString().Trim() + "','" + txtAge.Text.ToString().Trim() + "','" + txtNIC.Text.ToString().Trim() + "', " +
                    " '" + cmbSex.Text.ToString().Trim() + "','" + cmbCinsultant.Text.ToString().Trim() + "','" + txtCompany.Text.ToString().Trim() + "','" + cmbPatientType.Text.ToString().Trim() + "')";
                SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                if (cmbPatientType.Text == "WARD")
                {
                    string StrSql2 = "DELETE FROM tblPatientAddmission WHERE CustomerID='" + txtPatientNo.Text.ToString().Trim() + "'";
                    SqlCommand command2 = new SqlCommand(StrSql2, myConnection, myTrans);
                    command2.CommandType = CommandType.Text;
                    command2.ExecuteNonQuery();

                    StrSql = "INSERT INTO[dbo].[tblPatientAddmission]([CustomerID],[AdmitDate],[AdmitTime],[Name],[Address1],[Address2],[Company],[RefDoctor],[NIC]" +
                              " ,[Age],[ContactNo],[Gender],[Room],[RoomRate],[DepositAmount],[IsDischarge],PaymentType)" +
                                  " VALUES ('" + txtPatientNo.Text.ToString().Trim() + "', '" + dtpAdmiDate.Value.ToShortDateString() + "', '" + System.DateTime.Now.ToShortTimeString() + "', " +
                              " '" + txtFirstName.Text.ToString().Trim() + "', '" + txtAddress1.Text.ToString().Trim() + "','" + txtAddress2.Text.ToString().Trim() + "','" + txtCompany.Text.ToString().Trim() + "', " +
                             " '" + cmbCinsultant.Text.ToString().Trim() + "','" + txtNIC.Text.ToString().Trim() + "','" + double.Parse(txtAge.Text.ToString()) + "','" + txtContactNO.Text.ToString().Trim() + "','" + cmbSex.Text.ToString().Trim() + "','" + cmbroom.Text.ToString().Trim() + "','" + double.Parse(txtRate.Value.ToString()) + "','" + double.Parse(txtDeposit.Value.ToString()) + "','1','"+cmbCashAccount.Text.ToString()+"')";

                    command = new SqlCommand(StrSql, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    dltScanchanel(myConnection, myTrans);
                    SaveScanchanel(myConnection, myTrans);
                }
                
                myTrans.Commit();
                MessageBox.Show("Patient Records Successfuly Updated.", "Information", MessageBoxButtons.OK);
                //ClearData();
                // return;

            }
            catch (Exception ex) {
                myTrans.Rollback();
                MessageBox.Show(ex.Message); }
        }
        private void CreateCustomer()
        {
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\exportCustomerMaster.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Customers");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            Writer.WriteStartElement("PAW_Customer");
            Writer.WriteAttributeString("xsi:type", "paw:Customer");

            Writer.WriteStartElement("ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(txtPatientNo.Text.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Name");
            Writer.WriteString(txtFirstName.Text.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Line1");
            Writer.WriteString(txtAddress1.Text.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Line2");
            Writer.WriteString(txtAddress2.Text.ToString().Trim());
            Writer.WriteEndElement();

            //  PhoneNumbers>
            // <PhoneNumber Key="1">777801845</PhoneNumber> 
            // </PhoneNumbers>
            Writer.WriteStartElement("PhoneNumbers");

            Writer.WriteStartElement("PhoneNumber");
            // Writer.WriteStartElement("TelePhone_1");
            Writer.WriteString(txtContactNO.Text.ToString().Trim());
            Writer.WriteEndElement();
            Writer.WriteEndElement();
            // Writer.WriteStartElement("PhoneNumbers");

            Writer.WriteStartElement("Customer_Type");
            Writer.WriteString(cmbPatientType.Text.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Account_Number");
            Writer.WriteString(txtNIC.Text.ToString().Trim());
            Writer.WriteEndElement();

            //Writer.WriteStartElement("CustomFields");
            //Writer.WriteStartElement("CustomField");
            //Writer.WriteStartElement("Description");
            //Writer.WriteString("Second Contact");
            //Writer.WriteEndElement();
            //Writer.WriteStartElement("Value Index = 1");
            //Writer.WriteString("CREDIT");
            //Writer.WriteEndElement();
            //Writer.WriteEndElement();
            //Writer.WriteEndElement();

            //CustomFields
            Writer.WriteStartElement("CustomFields");
            // Writer.WriteStartElement("CustomField1");

            Writer.WriteStartElement("CustomField");
            Writer.WriteStartElement("Value");
            Writer.WriteAttributeString("Index ", "1");//Change time and date both
            Writer.WriteString(txtAge.Text.ToString().Trim());
            Writer.WriteEndElement();
            Writer.WriteEndElement();

            Writer.WriteEndElement();

            Writer.WriteEndElement();
            Writer.WriteEndElement();
            Writer.Close();
            Connector Conn = new Connector();
            Conn.ExportCustomer();
        }
        string StrPatientNo = string.Empty;
        private void SaveEvent()
        {
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

                //DateTime Date1 = System.DateTime.Now;
                //string Dformat = "MM/dd/yyyy";
                //string GRNDate = dtpAdmiDate

                //string CurretDate = GRNDate;
                //string CurrentTime = System.DateTime.Now.ToShortTimeString();

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                StrPatientNo = GetPatientCodeByType(myConnection, myTrans);
                UpdatePatientCodeByType(myConnection, myTrans);
                txtPatientNo.Text = StrPatientNo;


                StrSql = "INSERT INTO[dbo].[tblCustomerMaster]([CutomerID],[CustomerName],[Address1],[Address2],[Phone1],[Custom1],[Custom2],[Custom3],[Custom4],[Custom5],[Cus_Type]) " +
                       " VALUES ('" + txtPatientNo.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtAddress1.Text.ToString().Trim() + "', " +
                     " '" + txtAddress2.Text.ToString().Trim() + "', '" + txtContactNO.Text.ToString().Trim() + "','" + txtAge.Text.ToString().Trim() + "','" + txtNIC.Text.ToString().Trim() + "', " +
                    " '" + cmbSex.Text.ToString().Trim() + "','" + cmbCinsultant.Text.ToString().Trim() + "','" + txtCompany.Text.ToString().Trim() + "','" + cmbPatientType.Text.ToString().Trim() + "')";
                SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                if (cmbPatientType.Text == "WARD")
                {

                    StrSql = "INSERT INTO[dbo].[tblPatientAddmission]([CustomerID],[AdmitDate],[AdmitTime],[Name],[Address1],[Address2],[Company],[RefDoctor],[NIC]" +
                              " ,[Age],[ContactNo],[Gender],[Room],[RoomRate],[DepositAmount],[IsDischarge],PaymentType)" +
                                  " VALUES ('" + txtPatientNo.Text.ToString().Trim() + "', '" + dtpAdmiDate.Value.ToShortDateString() + "', '" + System.DateTime.Now.ToShortTimeString() + "', " +
                              " '" + txtFirstName.Text.ToString().Trim() + "', '" + txtAddress1.Text.ToString().Trim() + "','" + txtAddress2.Text.ToString().Trim() + "','" + txtCompany.Text.ToString().Trim() + "', " +
                             " '" + cmbCinsultant.Text.ToString().Trim() + "','" + txtNIC.Text.ToString().Trim() + "','" + double.Parse(txtAge.Text.ToString()) + "','" + txtContactNO.Text.ToString().Trim() + "','" + cmbSex.Text.ToString().Trim() + "','" + cmbroom.Text.ToString().Trim() + "','" + double.Parse(txtRate.Value.ToString()) + "','" + double.Parse(txtDeposit.Value.ToString()) + "','1','"+cmbCashAccount.Text.ToString()+"')";

                    command = new SqlCommand(StrSql, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                    SaveScanchanel(myConnection, myTrans);
                }
               
                myTrans.Commit();
               // CreateCustomer();
                MessageBox.Show("Patient Records Successfuly Saved.", "Information", MessageBoxButtons.OK);
               // ClearData();
               // return;

            }
            catch (Exception ex) {
                myTrans.Rollback();
                MessageBox.Show(ex.Message); }
        }

        public string GetPatientCodeByType(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT TPref, TPad, TNo FROM tblPatientMasterType where TypeName='" + cmbPatientType.Text.ToString().Trim() + "'";
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
        public void UpdatePatientCodeByType(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;

                StrSql = "SELECT  TOP 1(TNo) FROM tblPatientMasterType where TypeName='" + cmbPatientType.Text.ToString().Trim() + "' ORDER BY TNo DESC";
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
                StrSql = "UPDATE tblPatientMasterType SET TNo='" + intInvNo + "' where TypeName='" + cmbPatientType.Text.ToString().Trim() + "' ";
                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {

                Print(txtPatientNo.Text.ToString()+"R");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Print(String p)
        {
           
                // ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.Cancel)
                {
                    return;
                }


                ds.Clear();
                String S1 = "SELECT * FROM [View_Deposit] WHERE [ReceiptNo] = '" + p + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                //da1.Fill(ds, "DTReturn");
                da1.Fill(ds.tbldeposit);



                // DirectPrint();

                frmInpationViewer cusReturn = new frmInpationViewer(this);
                cusReturn.Show();
            }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                frmPatientList ObjPatinetList = new frmPatientList();
                ObjPatinetList.ShowDialog();
            }
            catch
            {

            }


        }

        private void btnList_Click_1(object sender, EventArgs e)
        {
            try
            {
                InpatientMasterList ipml = new InpatientMasterList();
                ipml.ShowDialog();
                if (Search.InpationNo != "")
                {
                    loadPation();
                    btnEdit.Enabled = true;
                    isedit = false;
                    cmbCashAccount.Enabled = false;
                }
                Search.InpationNo = "";
                btnSave.Enabled = false;
                buttonClear();
            }
            catch
            {

            }
        }
        private void buttonClear()
        {
            btnSave.Enabled = false;
            cmbPatientType.Enabled = false;
            txtSearch.Enabled = false;
            txtAddress1.Enabled = false;
            txtAddress2.Enabled = false;
            txtAge.Enabled = false;
            txtContactNO.Enabled = false;
            txtFirstName.Enabled = false;
            cmbCinsultant.Enabled = false;
            cmbroom.Enabled = false;
            btnAdmit.Enabled = false;
            txtRate.Enabled = false;
            cmbSex.Enabled = false;
            txtNIC.Enabled = false;
            txtCompany.Enabled = false;
            txtDeposit.Enabled = false;
            dtpAdmiDate.Enabled = false;
            txtTime.Enabled = false;
            txtFirstName.Focus();
        }
        private void loadPation()
        {
            string s = "SELECT*FROM [tblCustomerMaster] where [CutomerID]='"+Search.InpationNo + "'";
            SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string s1 = "SELECT*FROM [ViewPatientAddmissAndCustomer] where [CustomerID]='" + Search.InpationNo + "'";
            SqlDataAdapter da1 = new SqlDataAdapter(s1, ConnectionString);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            ClearData();
            if (dt.Rows.Count > 0)
            {
                cmbPatientType.Text= dt.Rows[0]["Cus_Type"].ToString().Trim();
                txtPatientNo.Text = dt.Rows[0]["CutomerID"].ToString().Trim();
                //dtpAdmiDate.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                txtFirstName.Text = dt.Rows[0]["CustomerName"].ToString().Trim();
                txtNIC.Text = dt.Rows[0]["Custom2"].ToString().Trim();
                txtAddress1.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                txtAddress2.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                txtAge.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                txtContactNO.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                txtCompany.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
                cmbSex.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
                cmbCinsultant.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                //  txtDeposit.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                //cmbroom.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                //txtRate.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                if (dt1.Rows.Count > 0)
                {
                    dtpAdmiDate.Text= dt1.Rows[0]["AdmitDate"].ToString().Trim();
                    txtTime.Text = dt1.Rows[0]["AdmitTime"].ToString().Trim();
                    cmbroom.Text = dt1.Rows[0]["Room"].ToString().Trim();
                    txtRate.Value = dt1.Rows[0]["RoomRate"];
                    txtDeposit.Value = dt1.Rows[0]["DepositAmount"];
                    cmbCashAccount.Text = dt1.Rows[0]["PaymentType"].ToString();
                }

                }

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void cmbroom_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRate.Text = "0.00";
            string s = "SELECT Rate FROM [tblRoomDetails] where [RoomNo]='" + cmbroom.Text.ToString() + "'";
            SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                txtRate.Text = dt.Rows[0]["Rate"].ToString().Trim();
            }
            }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            isedit = true;
            Enable();
            btnEdit.Enabled = false;
        }
        private void Enable()
        {
            dtpAdmiDate.Enabled = true;
            txtTime.Enabled = true;
            btnSave.Enabled = true;
            cmbPatientType.Enabled = true;
            txtSearch.Enabled = false;
            txtAddress1.Enabled = true;
            txtAddress2.Enabled = true;
            txtAge.Enabled = true;
            txtContactNO.Enabled = true;
            txtFirstName.Enabled = true;
            cmbCinsultant.Enabled = true;
            cmbroom.Enabled = true;
            btnAdmit.Enabled = true;
            txtRate.Enabled = true;
            cmbSex.Enabled = true;
            txtNIC.Enabled = true;
            txtCompany.Enabled = true;
            txtDeposit.Enabled = true;
            txtFirstName.Focus();
            cmbCashAccount.Enabled = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void txtContactNO_Leave(object sender, EventArgs e)
        {
            try
            {
                string s1 = "SELECT Name,Address1,Address2,Company,NIC,Age,Gender,PaymentType FROM tblPatientAddmission WHERE ContactNo LIKE '" + txtContactNO.Text + "%' ";
                SqlDataAdapter da1 = new SqlDataAdapter(s1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {

                    txtFirstName.Text = dt1.Rows[0].ItemArray[0].ToString();
                    txtAddress1.Text = dt1.Rows[0].ItemArray[1].ToString();
                    txtAddress2.Text = dt1.Rows[0].ItemArray[2].ToString();
                    txtCompany.Text= dt1.Rows[0].ItemArray[3].ToString();
                    txtNIC.Text = dt1.Rows[0].ItemArray[4].ToString();
                    txtAge.Text = dt1.Rows[0].ItemArray[5].ToString();
                    cmbSex.Text = dt1.Rows[0].ItemArray[6].ToString();
                    cmbCashAccount.Text = dt1.Rows[0].ItemArray[7].ToString();
                }
                else 
                {
                    string s2 = "SELECT CustomerName,Address1,Address2,Custom1,Custom2,Custom3,Custom5 FROM tblCustomerMaster WHERE Phone1 LIKE '" + txtContactNO.Text + "%' ";
                    SqlDataAdapter da2 = new SqlDataAdapter(s2, ConnectionString);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);

                    if (dt2.Rows.Count > 0)
                    {

                        txtFirstName.Text = dt2.Rows[0].ItemArray[0].ToString();
                        txtAddress1.Text = dt2.Rows[0].ItemArray[1].ToString();
                        txtAddress2.Text = dt2.Rows[0].ItemArray[2].ToString();
                        txtAge.Text = dt2.Rows[0].ItemArray[3].ToString();
                        txtNIC.Text = dt2.Rows[0].ItemArray[4].ToString();
                        cmbSex.Text = dt2.Rows[0].ItemArray[5].ToString();
                        txtCompany.Text = dt2.Rows[0].ItemArray[6].ToString();
                        
                    }
                    else
                    {

                        string s = "SELECT PatientName,Gender,Age FROM tblScanChannel WHERE ContactNo LIKE '" + txtContactNO.Text + "%' ";
                        SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {

                            txtFirstName.Text = dt.Rows[0].ItemArray[0].ToString();
                            cmbSex.Text = dt.Rows[0].ItemArray[1].ToString();
                            txtAge.Text = dt.Rows[0].ItemArray[2].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}