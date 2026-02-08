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
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmPackage : Form
    {
        DataTable dtUser = new DataTable();
        Class1 a = new Class1();
        public int flg = 0;
        public int flgTech = 0;
        public string StrFormType = string.Empty;
        public int CheckSearch = 0;//to avoid goto textchange event after find result

        public dsScanChanel dsscanchanel = new dsScanChanel();

        bool run = false;
        bool add = false;
        bool edit = false;
        bool delete = false;

        public static string ConnectionString;
        public static bool isedit = false;
        public frmPackage()
        {
            InitializeComponent();
            setConnectionString();
        }

        //Method to establish the connection
        //public void setConnectionString()
        //{
        //    try
        //    {
        //        TextReader tr = new StreamReader("Connection.txt");
        //        ConnectionString = tr.ReadLine();
        //        tr.Close();
        //    }
        //    catch { }
        //}

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch { }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                //if (add)
                // {
                Enable();
                user.isLabList = false;
                cmbPaymentMethod.Text = "Cash";

                A = 0;
                txtdisRate.Text = "0";
                txtDisAmount.Text = "0";
                txtnetTotal.Text = "0";
                txtTotal.Text = "0";
                dgvScanItems.Rows.Clear();
                string currenru1 = user.userName;
                ClearText();
                isedit = false;
                EnableObjects();
                getTokenNo();

                //string ConnString = ConnectionString;
                //string sql = "Select HospitalCharge from tblDefualtSetting";
                //SqlConnection Conn = new SqlConnection(ConnString);
                //SqlCommand cmd = new SqlCommand(sql);
                //SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                //DataSet ds = new DataSet();
                //adapter.Fill(ds);
                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    // txtHospitalFee.Text = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                //}

                btnEdit.Enabled = false;
                flg = 2;
                txtPatientNo.Enabled = true;
                cmbScanSerch.Enabled = false;
                txtConsultantFind.Enabled = false;
                txtPatientNo.Enabled = true;
                cmbPackageType.Enabled = true;
                ultcmbPriefert.Enabled = true;

                cmbPackageType.ResetText();
                txtPationType.Text = "";
                txtReceiptNo.Text = "";
                txtPatientNo.Text = "";
                ultcmbPriefert.Text = "";
                btnVoid.Enabled = false;
                toolStripButton4.Enabled = false;
                btnConfirm.Enabled = false;
                btnPrint.Enabled = false;
                cmbPackageType.Focus();
                cmbSex.Text = "";
                cmbSex.Enabled = true;
                txtAge.Enabled = true;
                txtAge.Text = "";
                dtpTime.Refresh();
                dtpTime.ResetText();
                //  }
                // else
                // {
                // MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        public void getTokenNo()
        {
            try
            {
                //string ConnString = ConnectionString;
                //string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
                //SqlConnection Conn = new SqlConnection(ConnString);
                //SqlCommand cmd = new SqlCommand(sql);
                //SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                //DataSet ds = new DataSet();
                //adapter.Fill(ds);

                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    int p = ds.Tables[0].Rows.Count - 1;
                //    // AppointmentNo = Convert.ToInt32(ds.Tables[0].Rows[p].ItemArray[0]);
                //    string AppointmentNo = ds.Tables[0].Rows[p].ItemArray[0].ToString();
                //    string NewID = getNextID(AppointmentNo);

                //    txtReceiptNo.Text = NewID;
                //}
                //else
                //{
                //    String S2 = "Select ReceiptsNo from tblDefaultSettings";
                //    SqlCommand cmd2 = new SqlCommand(S2);
                //    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                //    DataTable dt1 = new DataTable();
                //    da2.Fill(dt1);

                //    if (dt1.Rows.Count > 0)
                //    {
                //        string P1 = dt1.Rows[dt1.Rows.Count - 1].ItemArray[0].ToString().Trim();
                //        string NewID = getNextID(P1);

                //        txtReceiptNo.Text = NewID;
                //    }
                //}

            }
            catch { }
        }

        public void EnableObjects()
        {
            try
            {
                //txtTokenNo.ReadOnly = false;
               cbxInpatient.Enabled = true;
                // txtPatientNo.ReadOnly = false;
                txtReceiptNo.Enabled = true;
                dtpDate.Enabled = true;
                //txtScanName.ReadOnly = false;
                // txtConsultant.ReadOnly = false;
                txtFirstName.ReadOnly = false;
                txtContactNo.ReadOnly = false;
                txtRemarks.ReadOnly = false;
                ultcmbPriefert.Enabled = true;
                //  txtScanFee.ReadOnly = false;
                // txtHospitalFee.ReadOnly = false;
                // txtTotal.ReadOnly = false;
                cmbPaymentMethod.Enabled = true;
                txtCreditCardNo.ReadOnly = false;
                // txtTechnician.ReadOnly = false;
                dtpDueDate.Enabled = true;
                // dtpRepDate.Enabled = true;
                //dtpRepDate

               // btnConsultant.Enabled = true;
                btnNewn.Enabled = true;
                btnEdit.Enabled = true;
                toolStripButton2.Enabled = true;
                txtdisRate.ReadOnly = false;
                txtPatientNo.ReadOnly = false;
                // btnScanNames.Enabled = false;
                dtpRepDate.Enabled = true;
                dtpDate.Enabled = true;
                rbcash.Enabled = true;

            }
            catch { }
        }

        public void DisableObjects()
        {
            try
            {
                txtTokenNo.ReadOnly = true;
                dtpDate.Enabled = false;
               
                txtFirstName.ReadOnly = true;
                txtContactNo.ReadOnly = true;
                txtRemarks.ReadOnly = true;
                // txtScanFee.ReadOnly = true;
                // txtHospitalFee.ReadOnly = true;
                // txtTotal.ReadOnly = true;
                cmbPaymentMethod.Enabled = false;
                txtCreditCardNo.ReadOnly = true;
                dtpDueDate.Enabled = false;
                //txtTechnician.ReadOnly = true;
                btnNewn.Enabled = false;
                btnEdit.Enabled = false;
                toolStripButton2.Enabled = false;
            }
            catch { }
        }

        public void ClearText()
        {
            try
            {
                rbcash.Checked = false;
                ultcmbPriefert.Text = "";
                dtpDueDate.Text = "";
                // txtTechnician.Text = "";
                txtTokenNo.Text = "";
                dtpDate.Text = "";
               
                txtFirstName.Text = "";
                txtContactNo.Text = "";
                txtRemarks.Text = "";
                // txtScanFee.Text = "0";        
                // txtHospitalFee.Text = "0";
                // txtTotal.Text = "0";
                cmbPaymentMethod.Text = "";
                txtCreditCardNo.Text = "";
            }
            catch { }
        }

        private void frmLab_Load(object sender, EventArgs e)
        {
            getPackages();
            //DataSet dsCustomer = new DataSet();
            //try
            //{
            //    dsCustomer.Clear();
            //    //StrSql = "SELECT distinct([ItemType]) as[ItemType] FROM[HospitalSystem].[dbo].[tblConsultantItemFee]";
            //    StrSql = "SELECT [TypeName]FROM [tblPatientMasterType]";
            //    SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
            //    dAdapt.Fill(dsCustomer, "DtClient");

            //    cmbTestType.DataSource = dsCustomer.Tables["DtClient"];
            //    cmbTestType.DisplayMember = "TypeName";
            //    cmbTestType.ValueMember = "TypeName";

            //    cmbTestType.DisplayLayout.Bands["DtClient"].Columns["TypeName"].Width = 150;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            dtpTime.ResetText();

            getpriefertBy();

            getpaymentmode();


            //try
            //{
            //    dtUser.Clear();
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmLab");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //        edit = Convert.ToBoolean(dtUser.Rows[0].ItemArray[4].ToString());
            //        delete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());
            //    }
            //}
            //catch { }
        }
        public DataSet dsPack;
        private void getPackages()
        {
            dsPack = new DataSet();
            try
            {
                dsPack.Clear();


                String StrSql = "SELECT DISTINCT PackageName FROM tblLabPackage ORDER BY PackageName ";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsPack, "DtClient");

                cmbPackageType.DataSource = dsPack.Tables["DtClient"];
                cmbPackageType.DisplayMember = "PackageName";
                cmbPackageType.ValueMember = "PackageName";

                cmbPackageType.DisplayLayout.Bands["DtClient"].Columns["PackageName"].Width = 350;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void getpaymentmode()
        {

            DataSet dspaymetmode = new DataSet();
            try
            {
                dspaymetmode.Clear();
                //StrSql = "SELECT distinct([ItemType]) as[ItemType] FROM[HospitalSystem].[dbo].[tblConsultantItemFee]";
                StrSql = "SELECT [CardType] As PaymentMethod FROM [tblCreditData]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dspaymetmode, "DtClient");

                cmbPaymentMethod.DataSource = dspaymetmode.Tables["DtClient"];
                cmbPaymentMethod.DisplayMember = "PaymentMethod";
                cmbPaymentMethod.ValueMember = "PaymentMethod";


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void getpriefertBy()
        {
            DataSet dsPriefert = new DataSet();
            try
            {
                dsPriefert.Clear();
                //StrSql = "SELECT distinct([ItemType]) as[ItemType] FROM[HospitalSystem].[dbo].[tblConsultantItemFee]";
                StrSql = "SELECT [Name],[Type] FROM [tblDoctorMaster]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsPriefert, "DtClient");

                ultcmbPriefert.DataSource = dsPriefert.Tables["DtClient"];
                ultcmbPriefert.DisplayMember = "Name";
                ultcmbPriefert.ValueMember = "Name";

                ultcmbPriefert.DisplayLayout.Bands["DtClient"].Columns["Name"].Width = 250;
                ultcmbPriefert.DisplayLayout.Bands["DtClient"].Columns["Type"].Width = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       

       
        private void frmLab_Activated(object sender, EventArgs e)
        {

            if (dc == 1)
            { }
            else
            {
                if (A != 1)
                {
                    double ReceipTotal = 0.00;
                    double ScanTotal = 0.00;
                    try
                    {
                        

                       

                        if (Class1.flg3 == 1)
                        {

                            txtConsultantFind.Text = a.GetText3();
                            Class1.flg3 = 0;
                        }

                        //if (Class1.flg4 == 1)
                        //{
                        //    txtConsultant.Text = a.GetText4();
                        //    DataAccess.Access.Consultant = a.GetText4();
                        //    Class1.flg4 = 0;
                        //}

                        if (Class1.flg5 == 1)
                        {
                            // txtTechnician.Text = a.GetText5();
                            DataAccess.Access.Consultant = a.GetText5();
                            Class1.flg5 = 0;
                        }

                        //......................

                        for (int a = 0; a < dgvScanItems.Rows.Count - 1; a++)
                        {
                            if (dgvScanItems.Rows[a].Cells[6].Value != null)
                            {
                                ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8
                            }
                        }
                        txtTotal.Text = ReceipTotal.ToString("N2");
                        txtnetTotal.Text = txtTotal.Text.Trim();

                        for (int a = 0; a < dgvScanItems.Rows.Count - 1; a++)
                        {
                            if (dgvScanItems.Rows[a].Cells[5].Value != null)
                            {
                                ScanTotal = ScanTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[5].Value);// sanjeewa change cell value 7 into 8
                            }
                        }
                        // txtTotal.Text = ReceipTotal.ToString("N2");
                        txtScanTotal.Text = ScanTotal.ToString("N2");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void txtScanFee_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (txtHospitalFee.Text.ToString().Trim() != "")
            //    {
            //        txtTotal.Text = Convert.ToString(Convert.ToDouble(txtHospitalFee.Text.ToString().Trim()) + Convert.ToDouble(txtScanFee.Text.ToString().Trim()));
            //    }
            //    else
            //    {
            //        txtTotal.Text = Convert.ToString(Convert.ToDouble(txtScanFee.Text.ToString().Trim()));
            //    }
            //}
            //catch { }
        }

        private void txtHospitalFee_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (txtScanFee.Text.ToString().Trim() != "")
            //    {
            //        txtTotal.Text = Convert.ToString(Convert.ToDouble(txtHospitalFee.Text.ToString().Trim()) + Convert.ToDouble(txtScanFee.Text.ToString().Trim()));
            //    }
            //    else
            //    {
            //        txtTotal.Text = Convert.ToString(Convert.ToDouble(txtHospitalFee.Text.ToString().Trim()));
            //    }
            //}
            //catch { }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (edit)
                {
                    EnableObjects();
                    btnNewn.Enabled = false;
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = true;
                    dtpDate.Enabled = false;
                   // btnScanNames.Enabled = false;
                    dtpDueDate.Enabled = false;
                   // btnConsultant.Enabled = false;
                    dgvScanItems.Enabled = false;
                    //txtConsultant.Enabled = false;
                    //txtScanName.Enabled = false;
                    flg = 1;

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (delete)
            //    {
            //        String S = "delete from tblScanChannel WHERE (TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "') AND (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
            //        SqlCommand cmd = new SqlCommand(S);
            //        SqlConnection con = new SqlConnection(ConnectionString);
            //        SqlDataAdapter da = new SqlDataAdapter(S, con);
            //        DataTable dt = new DataTable();
            //        da.Fill(dt);
            //        MessageBox.Show("Deleted Successfully");
            //        ClearText();
            //        DisableObjects();
            //        btnDelete.Enabled = false;
            //        btnNewn.Enabled = true;
            //        btnEdit.Enabled = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //catch { }
        }

        //public void exporetSalesInvoice()
        //{
        //    //Create a Xmal File..................................................................................

        //    //try
        //    //{
        //    XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\Scan.xml", System.Text.Encoding.UTF8);
        //    Writer.Formatting = Formatting.Indented;
        //    Writer.WriteStartElement("PAW_Invoices");
        //    Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
        //    Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
        //    Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

        //    // string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count-1);

        //    int gridcount = dgvScanItems.Rows.Count;
        //    int ab = (gridcount * 2) - 2;



        //    Writer.WriteStartElement("PAW_Invoice");
        //    Writer.WriteAttributeString("xsi:type", "paw:Receipt");


        //    if (txtPationType.Text == "Cash")
        //    {
        //        Writer.WriteStartElement("Customer_ID");
        //        Writer.WriteAttributeString("xsi:type", "paw:id");
        //        Writer.WriteString(txtPatientNo.Text.ToString().Trim());//Customer ID should be here = Ptient No
        //        Writer.WriteEndElement();
        //    }
        //    else
        //    {


        //        Writer.WriteStartElement("Customer_ID");
        //        Writer.WriteAttributeString("xsi:type", "paw:id");
        //        Writer.WriteString("C0001");//Customer ID should be here = Ptient No
        //        Writer.WriteEndElement();
        //    }

        //    Writer.WriteStartElement("Date");
        //    Writer.WriteAttributeString("xsi:type", "paw:id");
        //    Writer.WriteString(dtpRepDate.Value.ToString("MM/dd/yyyy"));//Date 
        //    Writer.WriteEndElement();

        //    Writer.WriteStartElement("Invoice_Number");
        //    Writer.WriteString(txtReceiptNo.Text.ToString().Trim());
        //    Writer.WriteEndElement();

        //    Writer.WriteStartElement("Accounts_Receivable_Account");
        //    Writer.WriteAttributeString("xsi:type", "paw:id");
        //    Writer.WriteString("65000");//Cash Account
        //    Writer.WriteEndElement();//CreditMemoType

        //    Writer.WriteStartElement("CreditMemoType");
        //    Writer.WriteString("FALSE");
        //    Writer.WriteEndElement();

        //    Writer.WriteStartElement("Number_of_Distributions");
        //    Writer.WriteString(ab.ToString());
        //    Writer.WriteEndElement();

        //    Writer.WriteStartElement("SalesLines");
        //    for (int i = 0; i <= dgvScanItems.Rows.Count - 2; i++)
        //    {

        //        Writer.WriteStartElement("SalesLine");

        //        Writer.WriteStartElement("Quantity");
        //        Writer.WriteString("1");//Doctor Charge
        //        Writer.WriteEndElement();


        //        Writer.WriteStartElement("Item_ID");
        //        Writer.WriteString("D0002");
        //        Writer.WriteEndElement();

        //        Writer.WriteStartElement("Description");
        //        Writer.WriteString("Consultant Chargers Scan");
        //        Writer.WriteEndElement();

        //        Writer.WriteStartElement("GL_Account");
        //        Writer.WriteAttributeString("xsi:type", "paw:id");
        //        Writer.WriteString("85500");
        //        Writer.WriteEndElement();


        //        //========================================================
        //        Writer.WriteStartElement("Tax_Type");
        //        Writer.WriteString("1");//Doctor Charge
        //        Writer.WriteEndElement();

        //        Writer.WriteStartElement("Amount");
        //        Writer.WriteString("-" + dgvScanItems[4, i].Value.ToString().Trim());//HospitalCharge
        //        //Writer.WriteString("0");//HospitalCharge
        //        Writer.WriteEndElement();
        //        Writer.WriteEndElement();



        //        //===================================================
        //        Writer.WriteStartElement("SalesLine");

        //        // Writer.WriteString(dgvScanItems[3, i].Value.ToString().Trim());//Doctor Charge

        //        Writer.WriteStartElement("Quantity");
        //        Writer.WriteString("1");//Doctor Charge
        //        Writer.WriteEndElement();


        //        Writer.WriteStartElement("Item_ID");
        //        Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
        //        Writer.WriteEndElement();

        //        Writer.WriteStartElement("Description");
        //        Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
        //        Writer.WriteEndElement();

        //        Writer.WriteStartElement("GL_Account");
        //        Writer.WriteAttributeString("xsi:type", "paw:id");
        //        Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
        //        Writer.WriteEndElement();


        //        //========================================================
        //        Writer.WriteStartElement("Tax_Type");
        //        Writer.WriteString("1");//Doctor Charge
        //        Writer.WriteEndElement();


        //        Writer.WriteStartElement("Amount");
        //        Writer.WriteString("-" + dgvScanItems[5, i].Value.ToString().Trim());//HospitalCharge
        //        Writer.WriteEndElement();

        //        Writer.WriteEndElement();//LINE
        //    }
        //    Writer.WriteEndElement();//LINES

        //    Writer.WriteEndElement();

        //    Writer.Close();

        //    Connector abc = new Connector();//export to peach tree
        //    abc.ImportScanData();//ImportSalesInvice()
        //    //}

        //    //catch { }


        //}


        public int A = 0;
        public void UpdatePatientCodeByType(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;

                StrSql = "SELECT  TOP 1(TNo) FROM tblLabTestTransactionType where TypeName='LAB' ORDER BY TNo DESC";
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
                StrSql = "UPDATE tblLabTestTransactionType SET TNo='" + intInvNo + "' where TypeName='LAB' ";
                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }

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

                StrSql = "SELECT TPref, TPad, TNo FROM tblLabTestTransactionType where TypeName='LAB'";
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
        string StrReferencetNo = string.Empty;

        //private void EditEventNew()
        //{
        //    setConnectionString();
        //    SqlConnection myConnection = new SqlConnection(ConnectionString);
        //    SqlTransaction myTrans = null;

        //    try
        //    {
        //        DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.OKCancel);

        //        if (reply == DialogResult.Cancel)
        //        {
        //            return;
        //        }
        //        myConnection.Open();
        //        myTrans = myConnection.BeginTransaction();
        //        string IsExport = "False";
        //        int rowCount = GetFilledRows();
        //        for (int i = 0; i < rowCount; i++)
        //        {
        //            StrSql = "UPDATE [dbo].[tblScanChannel]SET [Date] = '" + dtpDate.Text.ToString().Trim() + "',[ItemID] =  '" + dgvScanItems[0, i].Value + "',[Consultant] = '" + dgvScanItems[7, i].Value + "',[PatientName] = '" + txtFirstName.Text.ToString().Trim() + "',[ContactNo] =  '" + txtContactNo.Text.ToString().Trim() + "',[Remarks] = '" + txtRemarks.Text.ToString().Trim() + "',[PaymentMethod] = '" + cmbPaymentMethod.Text.ToString().Trim() + "',[CreditCardNo] = '" + txtCreditCardNo.Text.ToString().Trim() + "',[ItemDescription] =  '" + dgvScanItems[1, i].Value + "',[GLAccount] =  '" + dgvScanItems[2, i].Value + "',[Duration] =  '" + dgvScanItems[3, i].Value + "',[ConsultFee] =  '" + dgvScanItems[4, i].Value + "',[HospitalFee] =  '" + dgvScanItems[5, i].Value + "',[TotalFee] =  '" + dgvScanItems[6, i].Value + "',[DueDate] = '" + dtpDueDate.Text.ToString().Trim() + "',[ReceiptTotal] = '" + txtTotal.Text.ToString().Trim() + "',[ItemType] = '" + cmbPackageType.Text.ToString() + "',[CurrentUser] = '" + user.userName.ToString().Trim() + "',[RepDate] = '" + dtpRepDate.Text.ToString().Trim() + "',[IsExport] = '" + IsExport + "',[DisRate] = '" + txtdisRate.Text.ToString().Trim() + "',[DisAmount] = '" + txtDisAmount.Text.ToString().Trim() + "',[NetTotal] = '" + txtnetTotal.Text.ToString().Trim() + "',[PatientNo] = '" + txtPatientNo.Text.ToString() + "',[PrifertBy]='" + ultcmbPriefert.Text.ToString() + "', WHERE ([ReceiptNo] = '" + txtReceiptNo.Text + "')AND([TokenNo] = '" + txtTokenNo.Text.ToString().Trim() + "')AND([PationType] = '" + txtPationType.Text.ToString() + "')";
        //            SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
        //            command.CommandType = CommandType.Text;
        //            command.ExecuteNonQuery();

        //        }
        //        myTrans.Commit();
        //        MessageBox.Show("Patient Records Successfuly Edited.", "Information", MessageBoxButtons.OK);


        //    }
        //    catch (Exception ex) { MessageBox.Show(ex.Message); }
        //}
        private void SaveEventNew()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                //DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.YesNo);

                //if (reply == DialogResult.No)
                //{
                //    return;
                //}
                //else if (reply == DialogResult.Yes)
                //{
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                StrReferencetNo = GetPatientCodeByType(myConnection, myTrans);
                UpdatePatientCodeByType(myConnection, myTrans);
                txtReceiptNo.Text = StrReferencetNo;

                int rowCount = GetFilledRows();
                string IsExport = "False";//check weather accountss update or not
                string IsReprint = "False";//determine weather lab report print or not

                for (int i = 0; i < rowCount; i++)
                {
                   


                    StrSql = "insert into tblScanChannel(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal,PatientNo,IsConfirm,PationType,ReferedDoctor,NumberOfDistribution,DistributionNumber,IsReportPrint,Gender,Age,CollectTime)" +
                      " values ('" + txtReceiptNo.Text + "','" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString()+ "', '" + dgvScanItems[0, i].Value.ToString() + "', '" + dgvScanItems[7, i].Value.ToString() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dgvScanItems[6, i].Value + "', '" + dtpDueDate.Text.ToString().Trim() + "','" + txtTotal.Text.ToString().Trim() + "','" + cmbPackageType.Text.ToString() + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" + txtPatientNo.Text.ToString() + "','false','" + txtPationType.Text.ToString() + "','" + ultcmbPriefert.Text.ToString() + "','" + rowCount + "','" + i + "','" + IsReprint + "','"+cmbSex.Text.ToString()+"','"+txtAge.Text.ToString()+ "','" + dtpTime.Value + "')";
                    SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                myTrans.Commit();
                // MessageBox.Show("Patient Records Successfuly Saved.", "Information", MessageBoxButtons.OK);

                //}
            }
            catch (Exception ex)
            {
                A = 0;
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }
        }

        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvScanItems.Rows.Count; i++)
                {
                    if (dgvScanItems.Rows[i].Cells[0].Value != null) //change cell value by 1                   
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

        //public rowCount=0;
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                A = 1;
                if (isedit == false)
                {
                    //=====================================
                    //if (dtpDate.Value < System.DateTime.Now)
                    //{
                    //    getChanalingNoAfter();
                    //}
                    //else
                    //{
                    getChanalingNo();
                    //}
                    //StrPatientNo = GetPatientCodeByType(myConnection, myTrans);
                    //UpdatePatientCodeByType(myConnection, myTrans);
                    //txtPatientNo.Text = StrPatientNo;


                    //ReceiptNo();
                }
                //===================================================   
                if (cmbPackageType.Text == string.Empty)
                {
                    MessageBox.Show("Please Select a Test Type");
                    A = 0;
                    return;
                }
                //if (comboBox1.Text == string.Empty)
                //{
                //    MessageBox.Show("Please Select a Pation Type");
                //    return;
                //}
                if (txtPatientNo.Text == "")
                {
                    MessageBox.Show("Please Enter a Patient No");
                    A = 0;
                    return;
                }
                if (ultcmbPriefert.Text == "")
                {
                    MessageBox.Show("Please Select a Refered By");
                    A = 0;
                    return;
                }
                if (txtFirstName.Text == "")
                {
                    MessageBox.Show("Please Enter a Patient Name");
                    A = 0;
                    return;
                }
                if (txtContactNo.Text == "")
                {
                    MessageBox.Show("Please Enter a Contact Number");
                    A = 0;
                    return;
                }
                if (cmbPaymentMethod.Text == "")
                {
                    MessageBox.Show("Please Select a Payment Method");
                    A = 0;
                    return;
                }
                if (cmbSex.Text == "")
                {
                    MessageBox.Show("Please Select a Gender");
                    A = 0;
                    return;
                }
                if (txtAge.Text == "")
                {
                    MessageBox.Show("Please Enter Age");
                    A = 0;
                    return;
                }

                if (dgvScanItems.Rows.Count < 2)
                {
                    MessageBox.Show("Please Select a Scan");
                    A = 0;
                    return;
                }
                var rowCount = GetFilledRows();
                //for (int i = 0; i < rowCount; i++)
                //{
                //    if (dgvScanItems[3, i].Value.ToString().Trim() == string.Empty)
                //    {
                //        MessageBox.Show("Please Enter Due Date for Test Name : " + dgvScanItems[1, i].Value.ToString().Trim() + " ");
                //        return;
                //    }
                //}

                if (isedit == false)
                {
                    DialogResult reply1 = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.YesNo);

                    if (reply1 == DialogResult.No)
                    {
                        return;
                    }
                    else if (reply1 == DialogResult.Yes)
                    {
                        SaveEventNew();
                        DialogResult reply2 = MessageBox.Show("Do you Want to Confirm this record ? ", "Information", MessageBoxButtons.YesNo);

                        if (reply2 == DialogResult.No)
                        {
                            btnNew_Click(sender, e);
                            return;
                        }
                        else if (reply2 == DialogResult.Yes)
                        {
                            ConfirmEventSave();
                            btnReprintScan_Click(sender, e);
                            btnNew_Click(sender, e);
                        }
                    }
                }
                if (isedit == true)
                {
                    DeleteAndUpdate();
                    DialogResult reply2 = MessageBox.Show("Do you Want to Confirm this record ? ", "Information", MessageBoxButtons.YesNo);

                    if (reply2 == DialogResult.No)
                    {
                        btnNew_Click(sender, e);
                        return;
                    }
                    else if (reply2 == DialogResult.Yes)
                    {
                        ConfirmEventSave();
                        btnReprintScan_Click(sender, e);
                        btnNew_Click(sender, e);
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void DeleteAndUpdate()
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
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                

                string S8 = " DELETE FROM tblScanChannel WHERE ReceiptNo='" + txtReceiptNo.Text.ToString() + "' ";
                SqlCommand cmd8 = new SqlCommand(S8, myConnection, myTrans);
                SqlDataAdapter da8 = new SqlDataAdapter(cmd8);
                DataTable dt8 = new DataTable();
                da8.Fill(dt8);


                int rowCount = GetFilledRows();
                string IsExport = "False";
                string IsReprint = "False";
                for (int i = 0; i < rowCount; i++)
                {
                    StrSql = "insert into tblScanChannel(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal,PatientNo,IsConfirm,PationType,ReferedDoctor,NumberOfDistribution,DistributionNumber,IsReportPrint,Gender,Age)" +
                      " values ('" + txtReceiptNo.Text + "','" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value + "', '" + dgvScanItems[7, i].Value.ToString() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dgvScanItems[6, i].Value + "', '" + dtpDueDate.Text.ToString().Trim() + "','" + txtTotal.Text.ToString().Trim() + "','" + cmbPackageType.Text.ToString() + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" + txtPatientNo.Text.ToString() + "','false','" + txtPationType.Text.ToString() + "','" + ultcmbPriefert.Text.ToString() + "','" + rowCount + "','" + i + "','"+ IsReprint + "','"+cmbSex.Text.ToString()+"','"+txtAge.Text.ToString()+"')";
                    SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                }
                myTrans.Commit();
                MessageBox.Show("Patient Records Successfuly Update.", "Information", MessageBoxButtons.OK);


            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                A = 0;
                MessageBox.Show(ex.Message);
            }
        }

        //private void UpdateEvent()
        //{
        //    for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
        //    {
        //        // string ItemType = "Scan";
        //        string IsExport = "False";
        //        string ConnString2 = ConnectionString;
        //        String S2 = "UPDATE [dbo].[tblScanChannel]SET [Date] = '" + dtpDate.Text.ToString().Trim() + "',[ItemID] =  '" + dgvScanItems[0, i].Value + "',[Consultant] = '" + txtConsultant.Text.ToString().Trim() + "',[PatientName] = '" + txtFirstName.Text.ToString().Trim() + "',[ContactNo] =  '" + txtContactNo.Text.ToString().Trim() + "',[Remarks] = '" + txtRemarks.Text.ToString().Trim() + "',[PaymentMethod] = '" + cmbPaymentMethod.Text.ToString().Trim() + "',[CreditCardNo] = '" + txtCreditCardNo.Text.ToString().Trim() + "',[ItemDescription] =  '" + dgvScanItems[1, i].Value + "',[GLAccount] =  '" + dgvScanItems[2, i].Value + "',[Duration] =  '" + dgvScanItems[3, i].Value + "',[ConsultFee] =  '" + dgvScanItems[4, i].Value + "',[HospitalFee] =  '" + dgvScanItems[5, i].Value + "',[TotalFee] =  '" + dgvScanItems[6, i].Value + "',[DueDate] = '" + dtpDueDate.Text.ToString().Trim() + "',[ReceiptTotal] = '" + txtTotal.Text.ToString().Trim() + "',[ItemType] = '" + cmbTestType.Text.ToString() + "',[CurrentUser] = '" + user.userName.ToString().Trim() + "',[RepDate] = '" + dtpRepDate.Text.ToString().Trim() + "',[IsExport] = '" + IsExport + "',[DisRate] = '" + txtdisRate.Text.ToString().Trim() + "',[DisAmount] = '" + txtDisAmount.Text.ToString().Trim() + "',[NetTotal] = '" + txtnetTotal.Text.ToString().Trim() + "',[PatientNo] = '" + txtPatientNo.Text.ToString() + "' WHERE ([ReceiptNo] = '" + txtReceiptNo.Text + "')AND([TokenNo] = '" + txtTokenNo.Text.ToString().Trim() + "')AND([PationType] = '" + txtPationType.Text.ToString() + "')";
        //        SqlCommand cmd2 = new SqlCommand(S2);
        //        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString2);
        //        DataSet ds2 = new DataSet();
        //        da2.Fill(ds2);
        //    }
        //    MessageBox.Show("Update Successfully");
        //}

        //private void SaveEvent()
        //{
        //    try
        //    {
        //        //if (flg == 2)
        //        //{
        //        for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
        //        {
        //            // string ItemType = "Scan";
        //            string IsExport = "False";
        //            string ConnString2 = ConnectionString;
        //            String S2 = "insert into tblScanChannel(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal,PatientNo,IsConfirm,PationType)" +
        //                " values ('" + txtReceiptNo.Text + "','" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value + "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dgvScanItems[6, i].Value + "', '" + dtpDueDate.Text.ToString().Trim() + "','" + txtTotal.Text.ToString().Trim() + "','" + cmbTestType.Text.ToString() + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" + txtPatientNo.Text.ToString() + "','false','" + txtPationType.Text.ToString() + "')";
        //            SqlCommand cmd2 = new SqlCommand(S2);
        //            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString2);
        //            DataSet ds2 = new DataSet();
        //            da2.Fill(ds2);
        //        }

        //        // flg = 0;



        //        MessageBox.Show("Saved Successfully");

        //        // btnPrint_Click(sender, e);

        //        //}
        //        //else
        //        //{
        //        //    for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
        //        //    {
        //        //        string ItemType = "Scan";
        //        //        string IsExport = "False";
        //        //        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + txtScanName.Text.ToString().Trim() + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[0, i].Value + "',GLAccount = '" + dgvScanItems[1, i].Value + "', Duration = '" + dgvScanItems[2, i].Value + "', ConsultFee = '" + dgvScanItems[3, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "'  where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
        //        //        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "'  where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "') AND (IsExport = '" + IsExport + "')";
        //        //        SqlCommand cmd = new SqlCommand(S);
        //        //        SqlConnection con = new SqlConnection(ConnectionString);
        //        //        SqlDataAdapter da = new SqlDataAdapter(S, con);
        //        //        DataTable dt = new DataTable();
        //        //        da.Fill(dt);
        //        //    }
        //        //    MessageBox.Show("Updated Successfully");

        //        //    flg = 0;

        //        //}
        //        // btnPrint_Click(sender, e);
        //        ClearText();
        //        txtTokenNo.Text = "";
        //        DisableObjects();
        //        dgvScanItems.Rows.Clear();
        //        btnNewn.Enabled = true;
        //        // btnEdit.Enabled = true;
        //    }
        //    catch { }
        //}

        //private void ReceiptNo()
        //{
        //    try
        //    {
        //        // string ItemT = "Scan";
        //        string ConnString = ConnectionString;
        //        string sql = "Select ReceiptNo from tblScanChannel Where ItemType='" + cmbPackageType.Text.ToString() + "' ORDER BY ReceiptNo";
        //        SqlConnection Conn = new SqlConnection(ConnString);
        //        SqlCommand cmd = new SqlCommand(sql);
        //        SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
        //        DataSet ds = new DataSet();
        //        adapter.Fill(ds);

        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            int p = ds.Tables[0].Rows.Count - 1;
        //            string AppointmentNo = ds.Tables[0].Rows[p].ItemArray[0].ToString();
        //            string NewID = getNextID(AppointmentNo);

        //            txtReceiptNo.Text = NewID;
        //        }
        //        else
        //        {
        //            String S2 = "Select ReceiptsNo from tblDefualtSetting";
        //            SqlCommand cmd2 = new SqlCommand(S2);
        //            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //            DataTable dt1 = new DataTable();
        //            da2.Fill(dt1);

        //            if (dt1.Rows.Count > 0)
        //            {
        //                string P1 = dt1.Rows[dt1.Rows.Count - 1].ItemArray[0].ToString().Trim();
        //                string NewID = getNextID(P1);

        //                txtReceiptNo.Text = NewID;
        //            }
        //        }


        //    }   //===========================================
        //    catch (Exception ex) { }
        //    //==================================================

        //}

        private void getChanalingNo()
        {

            try
            {


                String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = 'WINLANKA') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                int NoOfTokens = 0;
                if (dt.Rows.Count > 0)
                {

                    NoOfTokens = dt.Rows.Count + 1;

                }
                else
                {
                    NoOfTokens = 1;
                }
                txtTokenNo.Text = NoOfTokens.ToString();
            }
            catch { }
        }

        //private void getChanalingNoAfter()
        //{
        //    try
        //    {

        //        String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
        //        SqlCommand cmd1 = new SqlCommand(S1);
        //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
        //        DataTable dt = new DataTable();
        //        da1.Fill(dt);
        //        int NoOfTokens = 0;
        //        if (dt.Rows.Count > 0)
        //        {

        //            NoOfTokens = dt.Rows.Count + 1;

        //        }
        //        else
        //        {
        //            NoOfTokens = 1;
        //        }
        //        txtTokenNo.Text = NoOfTokens.ToString();
        //    }
        //    catch { }
        //}

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    //frmSearchScanChannel frm = new frmSearchScanChannel();
            //    //frm.Show();
            //    //String S1 = "Select * from tblScanChannel where (Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "') AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
            //    String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
            //    SqlCommand cmd1 = new SqlCommand(S1);
            //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //    DataTable dt = new DataTable();
            //    da1.Fill(dt);

            //    if (dt.Rows.Count > 0)
            //    {
            //        txtTokenNo.Text = txtTokenNoFind.Text;
            //        dtpDate.Text = dtpDateFind.Text;
            //        txtScanName.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
            //        txtConsultant.Text = txtConsultantFind.Text;
            //        txtFirstName.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
            //        txtContactNo.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
            //        txtRemarks.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
            //       // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
            //       // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
            //        txtTotal.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
            //        cmbPaymentMethod.Text = dt.Rows[0].ItemArray[10].ToString().Trim();
            //        txtCreditCardNo.Text = dt.Rows[0].ItemArray[11].ToString().Trim();

            //        dgvScanItems.DataSource = dt;
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //          //  dgvScanItems.Rows.Add();
            //            dgvScanItems[0,i].Value = dt.Rows[i].ItemArray[12].ToString();
            //            dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[13].ToString();
            //            dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[14].ToString();
            //            dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[15].ToString();
            //            dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[16].ToString();
            //        }
            //    }
            //}
            //catch { }
        }

        private void txtConsultant_TextChanged(object sender, EventArgs e)
        {
            if (CheckSearch != 2)
            {
                // public void getReceiptNo()      
                //try
                //{
                //    string ItemT = "Scan";
                //    string ConnString = ConnectionString;
                //    // string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
                //    string sql = "Select ReceiptNo from tblScanChannel Where ItemType='" + ItemT + "' ORDER BY ReceiptNo";
                //    SqlConnection Conn = new SqlConnection(ConnString);
                //    SqlCommand cmd = new SqlCommand(sql);
                //    SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                //    DataSet ds = new DataSet();
                //    adapter.Fill(ds);

                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        int p = ds.Tables[0].Rows.Count - 1;
                //        // AppointmentNo = Convert.ToInt32(ds.Tables[0].Rows[p].ItemArray[0]);
                //        string AppointmentNo = ds.Tables[0].Rows[p].ItemArray[0].ToString();
                //        string NewID = getNextID(AppointmentNo);

                //        txtReceiptNo.Text = NewID;
                //    }
                //    else
                //    {
                //        String S2 = "Select ReceiptsNo from tblDefaultSettings";
                //        SqlCommand cmd2 = new SqlCommand(S2);
                //        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                //        DataTable dt1 = new DataTable();
                //        da2.Fill(dt1);

                //        if (dt1.Rows.Count > 0)
                //        {
                //            string P1 = dt1.Rows[dt1.Rows.Count - 1].ItemArray[0].ToString().Trim();
                //            string NewID = getNextID(P1);

                //            txtReceiptNo.Text = NewID;
                //        }
                //    }


                //}
                //catch { }
                // }


                //...................................
                //    try
                //    {


                //        String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //        // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //        SqlCommand cmd1 = new SqlCommand(S1);
                //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                //        DataTable dt = new DataTable();
                //        da1.Fill(dt);
                //        int NoOfTokens = 1;
                //        if (dt.Rows.Count > 0)
                //        {
                //            //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                //            // {
                //            // NoOfTokens = 1;
                //            // }
                //            //else
                //            //{
                //            NoOfTokens = dt.Rows.Count + 2;
                //            // }
                //        }
                //        else
                //        {
                //            NoOfTokens = 2;
                //        }
                //        txtTokenNo.Text = NoOfTokens.ToString();
                //    }
                //    catch { }

            }
        }

        //get next iD

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

        //.............

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    flgTech = 1;
            //    //frmSelectConsult frm = new frmSelectConsult(this);
            //    //frm.Show();
            //}
            //catch { }
        }

        private void btnTechnician_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    flgTech = 3;
            //    //frmSelectConsult frm = new frmSelectConsult(this);
            //    //frm.Show();
            //}
            //catch { }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dgvScanItems_Leave(object sender, EventArgs e)
        {
            //double ReceiptTotal = 0.0;
            //ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems.Columns[4].ToString());
            //txtTotal.Text = Convert.ToString(ReceiptTotal);
        }

        private void label6_Enter(object sender, EventArgs e)
        {
            //for (int a = 0; a < dataGridView4.Rows.Count; a++)
            //{
            //    if (dataGridView4.Rows[a].Cells[8].Value != null)
            //    {
            //        POgrossAmt = POgrossAmt + Convert.ToDouble(dataGridView4.Rows[a].Cells[8].Value);// sanjeewa change cell value 7 into 8
            //    }
            //}
        }

        private void dgvScanItems_Enter(object sender, EventArgs e)
        {
            //double ReceipTotal = 0.00;
            //for (int a = 0; a < dgvScanItems.Rows.Count-1; a++)
            //{
            //    if (dgvScanItems.Rows[a].Cells[4].Value != null)
            //    {
            //        ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[4].Value);// sanjeewa change cell value 7 into 8
            //    }
            //}
            //txtTotal.Text = ReceipTotal.ToString("N2");
            //dgvScanItems
        }

        private void txtConsultantFind_TextChanged(object sender, EventArgs e)
        {
            //Class1.flg2 = 1;
            //CheckSearch = 2;
            //if (cmbScanSerch.Text == "Patient Name")
            //{
            //    string add = txtConsultantFind.Text;
            //    string ItemType = "Scan";
            //    if (add != "")
            //    {
            //        // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
            //        String S2 = "Select * from tblScanChannel where (PatientName LIKE  '" + add + "%')AND (ItemType='" + cmbTestType.Text.ToString() + "')";
            //        // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
            //        SqlCommand cmd2 = new SqlCommand(S2);
            //        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            //        DataTable dt = new DataTable();
            //        da2.Fill(dt);

            //        if (dt.Rows.Count > 0)
            //        {
            //            txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
            //            txtTokenNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
            //            dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

            //            txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
            //            txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
            //            txtFirstName.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
            //            txtContactNo.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
            //            txtRemarks.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
            //            ultcmbPriefert.Text = dt.Rows[0].ItemArray[37].ToString().Trim();
            //            // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
            //            // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
            //            txtTotal.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
            //            dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();//Receipt Date
            //            cmbPaymentMethod.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
            //            txtCreditCardNo.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
            //            txtdisRate.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
            //            txtDisAmount.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
            //            txtnetTotal.Text = dt.Rows[0].ItemArray[25].ToString().Trim();
            //            txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();

            //            dgvScanItems.DataSource = dt;
            //            for (int i = 0; i < dt.Rows.Count; i++)
            //            {
            //                //  dgvScanItems.Rows.Add();
            //                // dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[10].ToString();
            //                dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
            //                dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
            //                dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();
            //                dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();
            //                dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[13].ToString();
            //                dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
            //                dgvScanItems.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();
            //            }

            //        }
            //    }
            //    else
            //    {
            //        //MessageBox.Show("Receipt not found");
            //    }
        //    }

        //    //.............
        //    if (cmbScanSerch.Text == "ReceiptNo")
        //    {
        //        string add = txtConsultantFind.Text;
        //        string ItemType = "Scan";
        //        if (add != "")
        //        {
        //            // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
        //            String S2 = "Select * from tblScanChannel where (ReceiptNo LIKE  '" + add + "%')AND (ItemType='" + cmbTestType.Text.ToString() + "')";
        //            // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
        //            SqlCommand cmd2 = new SqlCommand(S2);
        //            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //            DataTable dt = new DataTable();
        //            da2.Fill(dt);

        //            if (dt.Rows.Count > 0)
        //            {
        //                dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();//Receipt Date
        //                txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
        //                txtTokenNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
        //                dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

        //                txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
        //                txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
        //                txtFirstName.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
        //                txtContactNo.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
        //                txtRemarks.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
        //                // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
        //                // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
        //                txtTotal.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
        //                cmbPaymentMethod.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
        //                txtCreditCardNo.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
        //                txtdisRate.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
        //                txtDisAmount.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
        //                txtnetTotal.Text = dt.Rows[0].ItemArray[25].ToString().Trim();
        //                txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();

        //                dgvScanItems.DataSource = dt;
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    //  dgvScanItems.Rows.Add();
        //                    dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
        //                    dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
        //                    dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();
        //                    dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();
        //                    dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[13].ToString();
        //                    dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
        //                    dgvScanItems.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();
        //                }

        //            }
        //        }
        //        else
        //        {
        //            //MessageBox.Show("Receipt not found");
        //        }
        //    }

        //    //..............
        //    if (cmbScanSerch.Text == "ContactNo")
        //    {
        //        string add = txtConsultantFind.Text;
        //        string ItemType = "Scan";
        //        if (add != "")
        //        {
        //            // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
        //            String S2 = "Select * from tblScanChannel where (ContactNo LIKE  '" + add + "%')AND (ItemType='" + cmbTestType.Text.ToString() + "')";
        //            // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
        //            SqlCommand cmd2 = new SqlCommand(S2);
        //            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //            DataTable dt = new DataTable();
        //            da2.Fill(dt);

        //            if (dt.Rows.Count > 0)
        //            {
        //                dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();//Receipt Date
        //                txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
        //                txtTokenNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
        //                dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

        //                txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
        //                txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
        //                txtFirstName.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
        //                txtContactNo.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
        //                txtRemarks.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
        //                // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
        //                // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
        //                txtTotal.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
        //                cmbPaymentMethod.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
        //                txtCreditCardNo.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
        //                txtdisRate.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
        //                txtDisAmount.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
        //                txtnetTotal.Text = dt.Rows[0].ItemArray[25].ToString().Trim();
        //                txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();

        //                dgvScanItems.DataSource = dt;
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    //  dgvScanItems.Rows.Add();
        //                    dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
        //                    dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
        //                    dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();
        //                    dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();
        //                    dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[13].ToString();
        //                    dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
        //                    dgvScanItems.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();
        //                }

        //            }
        //        }
        //        else
        //        {
        //            //MessageBox.Show("Receipt not found");
        //        }
        //    }

        //    btnEdit.Enabled = false;
        //    btnRefund.Enabled = true;
        //    btnReprintScan.Enabled = true;
        //    groupBox2.Enabled = true;
        //    // dgvScanItems.Enabled = false;
        //    // btnEdit.Enabled = true;


        //    //=====================

        //    //txtTokenNo.ReadOnly = false;
        //    btnScanNames.Enabled = false;

        //    txtReceiptNo.Enabled = false;
        //    dtpDate.Enabled = false;
        //    txtScanName.ReadOnly = true;
        //    txtConsultant.ReadOnly = true;
        //    txtFirstName.ReadOnly = true;
        //    txtContactNo.ReadOnly = true;
        //    txtRemarks.ReadOnly = true;
        //    //  txtScanFee.ReadOnly = false;
        //    // txtHospitalFee.ReadOnly = false;
        //    // txtTotal.ReadOnly = false;
        //    cmbPaymentMethod.Enabled = false;
        //    txtCreditCardNo.ReadOnly = false;
        //    // txtTechnician.ReadOnly = false;
        //    dtpDueDate.Enabled = false;
        //    Clear.Enabled = false;
        //    // dtpRepDate.Enabled = true;
        //    //dtpRepDate

        //    btnConsultant.Enabled = false;
        //    btnNewn.Enabled = false;
        //    // btnEdit.Enabled = true;
        //    toolStripButton2.Enabled = false;
        //    txtCreditCardNo.Enabled = false;

        //    //===============================
        //    // btnDelete.Enabled = true;
        //    //.....................
        //}
        ////private void DirectPrint()
        ////{
        ////    try
        ////    {
        ////        string Myfullpath;
        ////        ReportDocument crp = new ReportDocument();
        ////        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\rptScanReceipt.rpt") == true)
        ////        {
        ////            Myfullpath = Path.GetFullPath("rptScanReceipt.rpt");
        ////        }
        ////        else
        ////        {
        ////            MessageBox.Show("rptScanReceipt.rpt not Exists");
        ////            this.Close();
        ////            return;
        ////        }
        ////        crp.Load(Myfullpath);
        ////        crp.SetDataSource(DsScanReport);
        ////        crp.PrintToPrinter(1, true, 0, 0);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        MessageBox.Show("Error :" + ex.Message);
        ////    }
        }
        private void btnReprintScan_Click(object sender, EventArgs e)
        {
            try
            {
                dsscanchanel.tblScanChannel.Clear();
                String S3 = "Select * from [tblScanChannel] where ([ReceiptNo] = '" + txtReceiptNo.Text.ToString().Trim() + "')AND(Date='" + dtpDate.Text.ToString() + "')";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(dsscanchanel, "tblScanChannel");

                frmPackagePrint invprint = new frmPackagePrint(this);
                invprint.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    int N = dgvScanItems.Rows.Count - 2;
            //    dgvScanItems.Rows.RemoveAt(N);

            //}
            //catch { }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //string ReferenceNo = "Cash-" + txtReceiptNo.Text.ToString().Trim();
            //string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);

            //// for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            //// {

            //XmlTextWriter Writer = new XmlTextWriter(@"c:\Receipts.xml", System.Text.Encoding.UTF8);
            //Writer.Formatting = Formatting.Indented;
            //Writer.WriteStartElement("PAW_Receipts");
            //Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            //Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            //Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            //for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            //{

            //    //string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);
            //    //for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            //    //{
            //    if (i == 0)
            //    {
            //        Writer.WriteStartElement("PAW_Receipt");
            //        Writer.WriteAttributeString("xsi:type", "paw:Receipt");
            //    }
            //    //this is for the first record which is describe the hospital charge
            //    Writer.WriteStartElement("ReceiptNumber");
            //    Writer.WriteString(txtReceiptNo.Text.ToString().Trim());//Receipts Number Should be here 
            //    Writer.WriteEndElement();
            //    //=======================================================================
            //    string CustomerName = txtFirstName.Text;
            //    string A = CustomerName.Substring(0, 1);
            //    // if(txtFirstName.Text==
            //    if (A == "a" || A == "A")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "b" || A == "B")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "c" || A == "C")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "d" || A == "D")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "e" || A == "E")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "f" || A == "F")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "g" || A == "G")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "h" || A == "H")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "i" || A == "I")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "j" || A == "J")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "k" || A == "K")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "l" || A == "L")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "m" || A == "M")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "n" || A == "N")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "o" || A == "O")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "p" || A == "P")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "q" || A == "Q")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "r" || A == "R")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "s" || A == "S")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "t" || A == "T")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "u" || A == "U")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "v" || A == "V")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "w" || A == "W")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "x" || A == "X")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "y" || A == "Y")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "z" || A == "Z")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("C0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    //====================================================================================
            //    //.......................................
            //    Writer.WriteStartElement("Date ");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
            //    Writer.WriteString(dtpDate.Text.ToString().Trim());//Date 
            //    // 03/15/2007
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Reference");
            //    Writer.WriteString(ReferenceNo);//Reference
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Cash_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString("70010");
            //    // Writer.WriteString("70010");//Cash Account//10200-00
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Payment_Method");
            //    Writer.WriteString("Cash");//PayMethod
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("InvoicePaid");
            //    Writer.WriteString("");//PayMethod
            //    Writer.WriteEndElement();

            //    //......................................................
            //    if (dgvScanItems.Rows.Count == 2)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("2");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 3)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("4");
            //        Writer.WriteEndElement();
            //    }

            //    if (dgvScanItems.Rows.Count == 4)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("6");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 5)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("8");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 6)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("10");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 7)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("12");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 8)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("14");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 9)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("16");
            //        Writer.WriteEndElement();
            //    }

            //    //........................................................

            //    Writer.WriteStartElement("Transaction_Number");
            //    Writer.WriteString(Convert.ToString(i + 1));
            //    Writer.WriteEndElement();
            //    //.....................................
            //    Writer.WriteStartElement("ItemID");
            //    Writer.WriteString("");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Description");
            //    Writer.WriteString("Doctor Charges");
            //    Writer.WriteEndElement();


            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString("-" + dgvScanItems[4, i].Value.ToString().Trim());//Doctor Charge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("GL_Account ");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString("76006");//Doctor payable Account 
            //    // Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());//Doctor payable Account 
            //    Writer.WriteEndElement();

            //    //.......................................................................


            //    //Writer.WriteStartElement("ItemID");
            //    //Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    //Writer.WriteStartElement("Description");
            //    //Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    //Writer.WriteStartElement("Amount");
            //    //Writer.WriteString(dgvScanItems[6, i].Value.ToString().Trim());//HospitalCharge
            //    //Writer.WriteEndElement();

            //    //Writer.WriteStartElement("GL_Account ");
            //    //Writer.WriteAttributeString("xsi:type", "paw:id");
            //    //Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    //Writer.WriteEndElement();
            //    //.............................................................................................
            //    //this is for a Scanning account
            //    // Writer.WriteStartElement("PAW_Receipt");001
            //    //Writer.WriteAttributeString("xsi:type", "paw:Receipt");001

            //    //this is for the first record which is describe the hospital charge
            //    Writer.WriteStartElement("ReceiptNumber");
            //    Writer.WriteString(txtReceiptNo.Text.ToString().Trim());//Receipts Number Should be here 
            //    Writer.WriteEndElement();
            //    //=================================================
            //    // string CustomerName = txtFirstName.Text;
            //    //string A = CustomerName.Substring(0, 1);
            //    // if(txtFirstName.Text==
            //    if (A == "a" || A == "A")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "b" || A == "B")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "c" || A == "C")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "d" || A == "D")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "e" || A == "E")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "f" || A == "F")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "g" || A == "G")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "h" || A == "H")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "i" || A == "I")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "j" || A == "J")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "k" || A == "K")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "l" || A == "L")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "m" || A == "M")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "n" || A == "N")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "o" || A == "O")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "p" || A == "P")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "q" || A == "Q")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "r" || A == "R")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "s" || A == "S")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "t" || A == "T")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "u" || A == "U")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "v" || A == "V")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "w" || A == "W")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "x" || A == "X")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "y" || A == "Y")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "z" || A == "Z")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("C0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    //===================================================================
            //    //.......................................
            //    Writer.WriteStartElement("Date ");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
            //    Writer.WriteString(dtpDate.Text.ToString().Trim());//Date 
            //    // 03/15/2007
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Reference");
            //    Writer.WriteString(ReferenceNo);//Reference
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Cash_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    // Writer.WriteString("70010");//Cash Account
            //    Writer.WriteString("70010");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Payment_Method");
            //    Writer.WriteString("Cash");//PayMethod
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("InvoicePaid");
            //    Writer.WriteString("");//PayMethod
            //    Writer.WriteEndElement();
            //    //....................
            //    //Writer.WriteStartElement("Number_of_Distributions ");
            //    //Writer.WriteString(Convert.ToString(dgvScanItems.Rows.Count - 1));
            //    //Writer.WriteEndElement();
            //    if (dgvScanItems.Rows.Count == 2)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("2");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 3)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("4");
            //        Writer.WriteEndElement();
            //    }

            //    if (dgvScanItems.Rows.Count == 4)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("6");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 5)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("8");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 6)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("10");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 7)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("12");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 8)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("14");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 9)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("16");
            //        Writer.WriteEndElement();
            //    }


            //    Writer.WriteStartElement("Transaction_Number");
            //    Writer.WriteString(Convert.ToString(i + 2));
            //    Writer.WriteEndElement();






            //    Writer.WriteStartElement("ItemID");
            //    Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Description");
            //    Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString("-" + dgvScanItems[5, i].Value.ToString().Trim());//HospitalCharge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("GL_Account ");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    // Writer.WriteString("40000-00");
            //    Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
            //    Writer.WriteEndElement();

            //    //Writer.WriteEndElement();
            //    // Writer.Close();       
            //    //....................................................................................................
            //}
            //Writer.WriteEndElement();
            //Writer.Close();

        }

        private void button1_Click_2(object sender, EventArgs e)
        {

            //string ReferenceNo = "Cash-" + txtReceiptNo.Text.ToString().Trim();
            //string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);

            //// for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            //// {

            //XmlTextWriter Writer = new XmlTextWriter(@"c:\Receipts.xml", System.Text.Encoding.UTF8);
            //Writer.Formatting = Formatting.Indented;
            //Writer.WriteStartElement("PAW_Receipts");
            //Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            //Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            //Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            //for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            //{

            //    //string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);
            //    //for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            //    //{
            //    if (i == 0)
            //    {
            //        Writer.WriteStartElement("PAW_Receipt");
            //        Writer.WriteAttributeString("xsi:type", "paw:Receipt");
            //    }
            //    //this is for the first record which is describe the hospital charge
            //    Writer.WriteStartElement("ReceiptNumber");
            //    Writer.WriteString(txtReceiptNo.Text.ToString().Trim());//Receipts Number Should be here 
            //    Writer.WriteEndElement();
            //    //=======================================================================
            //    string CustomerName = txtFirstName.Text;
            //    string A = CustomerName.Substring(0, 1);
            //    // if(txtFirstName.Text==
            //    if (A == "a" || A == "A")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "b" || A == "B")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "c" || A == "C")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "d" || A == "D")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "e" || A == "E")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "f" || A == "F")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "g" || A == "G")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "h" || A == "H")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "i" || A == "I")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "j" || A == "J")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "k" || A == "K")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "l" || A == "L")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "m" || A == "M")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "n" || A == "N")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "o" || A == "O")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "p" || A == "P")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "q" || A == "Q")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "r" || A == "R")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "s" || A == "S")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "t" || A == "T")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "u" || A == "U")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "v" || A == "V")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "w" || A == "W")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "x" || A == "X")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "y" || A == "Y")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "z" || A == "Z")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("C0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    //====================================================================================
            //    //.......................................
            //    Writer.WriteStartElement("Date ");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
            //    Writer.WriteString(dtpDate.Text.ToString().Trim());//Date 
            //    // 03/15/2007
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Reference");
            //    Writer.WriteString(ReferenceNo);//Reference
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Cash_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString("70010");
            //    // Writer.WriteString("70010");//Cash Account//10200-00
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Payment_Method");
            //    Writer.WriteString("Cash");//PayMethod
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("InvoicePaid");
            //    Writer.WriteString("");//PayMethod
            //    Writer.WriteEndElement();

            //    //......................................................
            //    if (dgvScanItems.Rows.Count == 2)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("2");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 3)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("4");
            //        Writer.WriteEndElement();
            //    }

            //    if (dgvScanItems.Rows.Count == 4)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("6");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 5)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("8");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 6)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("10");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 7)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("12");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 8)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("14");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 9)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("16");
            //        Writer.WriteEndElement();
            //    }

            //    //........................................................

            //    Writer.WriteStartElement("Transaction_Number");
            //    Writer.WriteString(Convert.ToString(i + 1));
            //    Writer.WriteEndElement();
            //    //.....................................
            //    Writer.WriteStartElement("ItemID");
            //    Writer.WriteString("");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Description");
            //    Writer.WriteString("Doctor Charges");
            //    Writer.WriteEndElement();


            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString(dgvScanItems[4, i].Value.ToString().Trim());//Doctor Charge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("GL_Account ");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString("76006");//Doctor payable Account 
            //    // Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());//Doctor payable Account 
            //    Writer.WriteEndElement();

            //    //.......................................................................


            //    //Writer.WriteStartElement("ItemID");
            //    //Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    //Writer.WriteStartElement("Description");
            //    //Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    //Writer.WriteStartElement("Amount");
            //    //Writer.WriteString(dgvScanItems[6, i].Value.ToString().Trim());//HospitalCharge
            //    //Writer.WriteEndElement();

            //    //Writer.WriteStartElement("GL_Account ");
            //    //Writer.WriteAttributeString("xsi:type", "paw:id");
            //    //Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    //Writer.WriteEndElement();
            //    //.............................................................................................
            //    //this is for a Scanning account
            //    // Writer.WriteStartElement("PAW_Receipt");001
            //    //Writer.WriteAttributeString("xsi:type", "paw:Receipt");001

            //    //this is for the first record which is describe the hospital charge
            //    Writer.WriteStartElement("ReceiptNumber");
            //    Writer.WriteString(txtReceiptNo.Text.ToString().Trim());//Receipts Number Should be here 
            //    Writer.WriteEndElement();
            //    //=================================================
            //    // string CustomerName = txtFirstName.Text;
            //    //string A = CustomerName.Substring(0, 1);
            //    // if(txtFirstName.Text==
            //    if (A == "a" || A == "A")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "b" || A == "B")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "c" || A == "C")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "d" || A == "D")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "e" || A == "E")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "f" || A == "F")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "g" || A == "G")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "h" || A == "H")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "i" || A == "I")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "j" || A == "J")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "k" || A == "K")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "l" || A == "L")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "m" || A == "M")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "n" || A == "N")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "o" || A == "O")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "p" || A == "P")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "q" || A == "Q")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "r" || A == "R")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "s" || A == "S")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "t" || A == "T")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "u" || A == "U")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "v" || A == "V")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "w" || A == "W")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "x" || A == "X")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "y" || A == "Y")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else if (A == "z" || A == "Z")
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    else
            //    {
            //        Writer.WriteStartElement("Customer_ID");
            //        Writer.WriteAttributeString("xsi:type", "paw:id");
            //        Writer.WriteString("C0001");//Customer ID should be here = Ptient No
            //        Writer.WriteEndElement();
            //    }
            //    //===================================================================
            //    //.......................................
            //    Writer.WriteStartElement("Date ");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
            //    Writer.WriteString(dtpDate.Text.ToString().Trim());//Date 
            //    // 03/15/2007
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Reference");
            //    Writer.WriteString(ReferenceNo);//Reference
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Cash_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    // Writer.WriteString("70010");//Cash Account
            //    Writer.WriteString("70010");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Payment_Method");
            //    Writer.WriteString("Cash");//PayMethod
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("InvoicePaid");
            //    Writer.WriteString("");//PayMethod
            //    Writer.WriteEndElement();
            //    //....................
            //    //Writer.WriteStartElement("Number_of_Distributions ");
            //    //Writer.WriteString(Convert.ToString(dgvScanItems.Rows.Count - 1));
            //    //Writer.WriteEndElement();
            //    if (dgvScanItems.Rows.Count == 2)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("2");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 3)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("4");
            //        Writer.WriteEndElement();
            //    }

            //    if (dgvScanItems.Rows.Count == 4)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("6");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 5)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("8");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 6)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("10");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 7)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("12");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 8)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("14");
            //        Writer.WriteEndElement();
            //    }
            //    if (dgvScanItems.Rows.Count == 9)
            //    {
            //        Writer.WriteStartElement("Number_of_Distributions ");
            //        Writer.WriteString("16");
            //        Writer.WriteEndElement();
            //    }


            //    Writer.WriteStartElement("Transaction_Number");
            //    Writer.WriteString(Convert.ToString(i + 2));
            //    Writer.WriteEndElement();



            //    //...........................



            //    //Writer.WriteStartElement("Number_of_Distributions ");
            //    //Writer.WriteString("4");
            //    //Writer.WriteEndElement();

            //    //Writer.WriteStartElement("Transaction_Number");
            //    //Writer.WriteString("2");
            //    //Writer.WriteEndElement();
            //    ////.....................................

            //    //Writer.WriteStartElement("Description");
            //    //Writer.WriteString("Doctor Charges");
            //    //Writer.WriteEndElement();


            //    //Writer.WriteStartElement("Amount");
            //    //Writer.WriteString("-" + dgvScanItems[4, i].Value.ToString().Trim());//Doctor Charge
            //    //Writer.WriteEndElement();

            //    //Writer.WriteStartElement("GL_Account ");
            //    //Writer.WriteAttributeString("xsi:type", "paw:id");
            //    //Writer.WriteString("76005");//Doctor payable Account 
            //    //Writer.WriteEndElement();

            //    //.......................................................................


            //    Writer.WriteStartElement("ItemID");
            //    Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Description");
            //    Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString(dgvScanItems[5, i].Value.ToString().Trim());//HospitalCharge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("GL_Account ");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    // Writer.WriteString("40000-00");
            //    Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
            //    Writer.WriteEndElement();

            //    //Writer.WriteEndElement();
            //    // Writer.Close();       
            //    //....................................................................................................
            //}
            //Writer.WriteEndElement();
            //Writer.Close();

        }
        public string CheckPosted = "";
        private void btnRefund_Click(object sender, EventArgs e)
        {
            A = 1;
            //====================================================


            String S2 = "select IsExport,Refund from tblScanChannel  WHERE ReceiptNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
            SqlCommand cmd2 = new SqlCommand(S2);
            SqlConnection con2 = new SqlConnection(ConnectionString);
            SqlDataAdapter da2 = new SqlDataAdapter(S2, con2);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);

            CheckPosted = dt2.Rows[0].ItemArray[0].ToString();

            if (CheckPosted == "False")
            {
                if (rbtnboth.Checked == true)
                {
                    string Ref = "Refund";
                    string IsExport = "True";

                    for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                    {
                        string ItemType = "Scan";
                        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + IsExport + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',DisAmount='" + txtDisAmount.Text.ToString().Trim() + "',NetTotal='" + txtnetTotal.Text.ToString().Trim() + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";//before scanrefund18-06-2009
                        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = 'WINLANKA', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + IsExport + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',DisAmount='" + txtDisAmount.Text.ToString().Trim() + "',NetTotal='" + txtnetTotal.Text.ToString().Trim() + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[4, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = 'WINLANKA') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + cmbPackageType.Text.ToString() + "')";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlConnection con = new SqlConnection(ConnectionString);
                        SqlDataAdapter da = new SqlDataAdapter(S, con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                    }
                    MessageBox.Show("Updated Successfully");
                }
                else
                {
                    string IsExport = "False";
                    for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                    {
                        string ItemType = "Scan";
                        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + txtScanName.Text.ToString().Trim() + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[0, i].Value + "',GLAccount = '" + dgvScanItems[1, i].Value + "', Duration = '" + dgvScanItems[2, i].Value + "', ConsultFee = '" + dgvScanItems[3, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "'  where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        //String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',IsExport='" + IsExport + "', DisRate='" + txtdisRate.Text.ToString().Trim() + "', DisAmount='" + txtDisAmount.Text.ToString().Trim() + "', NetTotal='" + txtnetTotal.Text.ToString().Trim() + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = 'WINLANKA', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',IsExport='" + IsExport + "', DisRate='" + txtdisRate.Text.ToString().Trim() + "', DisAmount='" + txtDisAmount.Text.ToString().Trim() + "', NetTotal='" + txtnetTotal.Text.ToString().Trim() + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[4, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = 'WINLANKA') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + cmbPackageType.Text.ToString() + "')";

                        //NetTotalCal='" + txtnetTotal.Text.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[5, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlConnection con = new SqlConnection(ConnectionString);
                        SqlDataAdapter da = new SqlDataAdapter(S, con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                    }
                    MessageBox.Show("Updated Successfully");
                }

            }

            else
            {
                string postrefund = "True";

                //====================================
                if (rbtnboth.Checked == true)
                {
                    string Ref = "Refund";
                    string IsExport = "False";

                    for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                    {
                        string ItemType = "Scan";
                        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + IsExport + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',DisAmount='" + txtDisAmount.Text.ToString().Trim() + "',NetTotal='" + txtnetTotal.Text.ToString().Trim() + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";//before scanrefund18-06-2009
                        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = 'WINLANKA', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + IsExport + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',DisAmount='" + txtDisAmount.Text.ToString().Trim() + "',PostedRefund='" + postrefund + "',NetTotal='" + txtnetTotal.Text.ToString().Trim() + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[4, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = 'WINLANKA') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + cmbPackageType.Text.ToString() + "')";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlConnection con = new SqlConnection(ConnectionString);
                        SqlDataAdapter da = new SqlDataAdapter(S, con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                    }
                    MessageBox.Show("Updated Successfully");
                }
                else
                {
                    string IsExport = "False";
                    for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                    {
                        string ItemType = "Scan";
                        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + txtScanName.Text.ToString().Trim() + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[0, i].Value + "',GLAccount = '" + dgvScanItems[1, i].Value + "', Duration = '" + dgvScanItems[2, i].Value + "', ConsultFee = '" + dgvScanItems[3, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "'  where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        //String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',IsExport='" + IsExport + "', DisRate='" + txtdisRate.Text.ToString().Trim() + "', DisAmount='" + txtDisAmount.Text.ToString().Trim() + "', NetTotal='" + txtnetTotal.Text.ToString().Trim() + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = 'WINLANKA', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',IsExport='" + IsExport + "', DisRate='" + txtdisRate.Text.ToString().Trim() + "', DisAmount='" + txtDisAmount.Text.ToString().Trim() + "', NetTotal='" + txtnetTotal.Text.ToString().Trim() + "',PostedRefund='" + postrefund + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[4, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = 'WINLANKA') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + cmbPackageType.Text.ToString() + "')";

                        //NetTotalCal='" + txtnetTotal.Text.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[5, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlConnection con = new SqlConnection(ConnectionString);
                        SqlDataAdapter da = new SqlDataAdapter(S, con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                    }
                    MessageBox.Show("Updated Successfully");
                }

                //=======================================


            }
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            if (CheckSearch != 2)
            {

                //if (dtpDate.Value < System.DateTime.Now)
                //{
                //    //MessageBox.Show("You cant select previous Date");
                //    dtpDate.Text = Convert.ToString(System.DateTime.Now);
                //    // dtpDate.Focus();

                //    try
                //    {


                //        String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //        // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //        SqlCommand cmd1 = new SqlCommand(S1);
                //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                //        DataTable dt = new DataTable();
                //        da1.Fill(dt);
                //        int NoOfTokens = 1;
                //        if (dt.Rows.Count > 0)
                //        {
                //            //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                //            // {
                //            // NoOfTokens = 1;
                //            // }
                //            //else
                //            //{
                //            NoOfTokens = dt.Rows.Count + 2;
                //            // }
                //        }
                //        else
                //        {
                //            NoOfTokens = 2;
                //        }
                //        txtTokenNo.Text = NoOfTokens.ToString();
                //    }
                //    catch { }
                //}
                //else
                //{
                //    try
                //    {


                //        String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //        // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //        SqlCommand cmd1 = new SqlCommand(S1);
                //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                //        DataTable dt = new DataTable();
                //        da1.Fill(dt);
                //        int NoOfTokens = 1;
                //        if (dt.Rows.Count > 0)
                //        {
                //            //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                //            // {
                //            // NoOfTokens = 1;
                //            // }
                //            //else
                //            //{
                //            NoOfTokens = dt.Rows.Count + 2;
                //            // }
                //        }
                //        else
                //        {
                //            NoOfTokens = 2;
                //        }
                //        txtTokenNo.Text = NoOfTokens.ToString();
                //    }
                //    catch { }


                // }
            }
        }

        private void txtTokenNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtScanName_TextChanged(object sender, EventArgs e)
        {

        }
        //====================================
        public double NetTotalRefund = 0.00;
        // public double DoctorCGRefund = 0.00;
        // public double HosCGRefund = 0.00;


        public void getScanData()
        {
            string add = txtReceiptNo.Text;
            string ItemType = "Scan";
            if (add != "")
            {
                // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                String S2 = "Select * from tblScanChannel where (ReceiptNo LIKE  '" + add + "%')AND (ItemType='" + cmbPackageType.Text.ToString() + "')";
                // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt = new DataTable();
                da2.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();//Receipt Date
                    txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                    txtTokenNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                    dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

                    //txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                    //txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                   txtFirstName.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                    txtContactNo.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
                    txtRemarks.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
                    // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
                    // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                    txtTotal.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
                    cmbPaymentMethod.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                    txtCreditCardNo.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
                    txtdisRate.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
                    txtDisAmount.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
                    txtnetTotal.Text = dt.Rows[0].ItemArray[25].ToString().Trim();

                    NetTotalRefund = Convert.ToDouble(txtnetTotal.Text);


                    dgvScanItems.DataSource = dt;
                    dgvScanRefundSave.DataSource = dt;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //  dgvScanItems.Rows.Add();
                        dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
                        dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
                        dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();
                        dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();
                        dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[13].ToString();
                        dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
                        dgvScanItems.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();

                        //=========================

                        dgvScanRefundSave[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
                        dgvScanRefundSave.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
                        dgvScanRefundSave.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();
                        dgvScanRefundSave.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();
                        dgvScanRefundSave.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[13].ToString();
                        dgvScanRefundSave.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
                        dgvScanRefundSave.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();


                        //==============================================
                    }

                }


                for (int a = 0; a < dgvScanRefundSave.Rows.Count - 1; a++)
                {
                    if (dgvScanRefundSave.Rows[a].Cells[6].Value != null)
                    {
                        NetTotalRefund = NetTotalRefund + Convert.ToDouble(dgvScanRefundSave.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8
                    }
                }
            }
            else
            {
                //MessageBox.Show("Receipt not found");
            }




        }
        //==================================================
        public int dc = 1;
        private string StrSql;

        private void rbtnDoctorfee_CheckedChanged(object sender, EventArgs e)
        {
            // frmLab_Activated(sender,e);
            dc = 1;
            getScanData();
            try
            {
                for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                {
                    dgvScanItems.Rows[i].Cells[4].Value = "0";
                    dgvScanItems.Rows[i].Cells[6].Value = Convert.ToInt32(dgvScanItems.Rows[i].Cells[5].Value) + Convert.ToInt32(dgvScanItems.Rows[i].Cells[4].Value);
                }
            }
            catch { }

            //.........................
            double ReceipTotal = 0.00;
            for (int a = 0; a < dgvScanItems.Rows.Count - 1; a++)
            {
                if (dgvScanItems.Rows[a].Cells[6].Value != null)
                {
                    ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8
                }
            }
            txtTotal.Text = ReceipTotal.ToString("N2");
            txtdisRate_TextChanged(sender, e);
            //............................
        }

        private void rbtnHosfee_CheckedChanged(object sender, EventArgs e)
        {
            dc = 1;
            getScanData();
            try
            {
                for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                {
                    dgvScanItems.Rows[i].Cells[5].Value = "0";
                    dgvScanItems.Rows[i].Cells[6].Value = Convert.ToInt32(dgvScanItems.Rows[i].Cells[4].Value) + Convert.ToInt32(dgvScanItems.Rows[i].Cells[5].Value);
                }
            }
            catch { }

            //.........................
            double ReceipTotal = 0.00;
            for (int a = 0; a < dgvScanItems.Rows.Count - 1; a++)
            {
                if (dgvScanItems.Rows[a].Cells[6].Value != null)
                {
                    ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8
                }
            }
            txtTotal.Text = ReceipTotal.ToString("N2");
            txtdisRate_TextChanged(sender, e);
        }

        private void rbtnboth_CheckedChanged(object sender, EventArgs e)
        {
            getScanData();
            try
            {
                for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                {
                    dgvScanItems.Rows[i].Cells[4].Value = "0";
                    dgvScanItems.Rows[i].Cells[5].Value = "0";
                    dgvScanItems.Rows[i].Cells[6].Value = "0";

                }
            }
            catch { }

            //.........................
            double ReceipTotal = 0.00;
            for (int a = 0; a < dgvScanItems.Rows.Count - 1; a++)
            {
                if (dgvScanItems.Rows[a].Cells[6].Value != null)
                {
                    ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8
                }
            }
            txtTotal.Text = ReceipTotal.ToString("N2");
            txtdisRate_TextChanged(sender, e);
        }

        private void txtdisRate_TextChanged(object sender, EventArgs e)
        {
            double Rate = 0.00;
            double DiscountAmount = 0.00;
            double totBefoDiscount = 0.00;
            double scandiscount = 0.00;
            double NetTotal = 0.00;

            // if (once == 1)
            // {
            try
            {

                totBefoDiscount = Convert.ToDouble(txtTotal.Text.Trim());
                scandiscount = Convert.ToDouble(txtScanTotal.Text);

            }
            catch { }
            // once = 2;
            // }

            if (txtdisRate.Text != "")
            {
                Rate = (Convert.ToDouble(txtdisRate.Text.Trim())) / 100;
            }
            else
            {
                Rate = 0.00;
            }

            try
            {

                if (dc == 1)
                {
                    DiscountAmount = totBefoDiscount * Rate;
                }
                else
                {
                    DiscountAmount = scandiscount * Rate;
                }
                // txtDisAmount.Text = Convert.ToString(DiscountAmount);
                txtDisAmount.Text = DiscountAmount.ToString("N2");
                NetTotal = totBefoDiscount - DiscountAmount;
                // txtnetTotal.Text = Convert.ToString(NetTotal);
                txtnetTotal.Text = NetTotal.ToString("N2");
            }
            catch { MessageBox.Show("Enter Correct Discount Rate"); }
        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtFirstName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtPationType.Text == "Cash")
                {
                    string Code = txtFirstName.Text.ToString().Trim();
                    string A = Code.Substring(0, 1);
                    string B = A.ToUpper();
                    txtPatientNo.Text = "C" + B + "0001";
                    txtFirstName.Text = Code;
                }
            }
            catch { }

        }

        private void txtPatientNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadPation()
        {
           
                string s = "SELECT  [CutomerID],[CustomerName],[Phone1],[Cus_Type]FROM [tblCustomerMaster] WHERE CutomerID='" + Search.ScanPation + "' ";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtPatientNo.Text = dt.Rows[0].ItemArray[0].ToString();
                    txtFirstName.Text = dt.Rows[0].ItemArray[1].ToString();
                    txtContactNo.Text = dt.Rows[0].ItemArray[2].ToString();
                    txtPationType.Text = dt.Rows[0].ItemArray[3].ToString();
                }
            
        }
        private void cbxInpatient_CheckedChanged(object sender, EventArgs e)
        {

            //if (cbxInpatient.Checked == true)
            //{
            //    txtPatientNo.ReadOnly = false;
            //    txtPatientNo.Focus();
            //}
            //else
            //{
            //    txtPatientNo.ReadOnly = true;
            //}
        }

        private void rbtncash_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtnbht_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtnetuopd_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void lblFirstName_Click(object sender, EventArgs e)
        {

        }

        private void lblContactNo_Click(object sender, EventArgs e)
        {

        }

        private void lblRemarks_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void cbxInpatient_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            isedit = true;
            EnableForEdit();
        }

        private void Enable()
        {
            cmbPackageType.Enabled = true;
            //comboBox1.Enabled = true;
            txtReceiptNo.Enabled = true;
            txtPatientNo.Enabled = true;
            txtTokenNo.Enabled = true;
            txtFirstName.Enabled = true;
            txtContactNo.Enabled = true;
            txtRemarks.Enabled = true;
           dgvScanItems.ReadOnly = false;
            toolStripButton2.Enabled = true;
            btnConfirm.Enabled = true;
             dtpRepDate.Enabled = true;
            dtpDate.Enabled = true;
            dtpDueDate.Enabled = true;
            ultcmbPriefert.Enabled = true;
            txtCreditCardNo.Enabled = true;
            txtdisRate.Enabled = true;

        }
        private void EnableForEdit()
        {
            cmbPackageType.Enabled = false;
            //comboBox1.Enabled = true;
            txtReceiptNo.Enabled = false;
            txtPatientNo.Enabled = false;
            txtTokenNo.Enabled = false;
            txtFirstName.Enabled = true;
            txtContactNo.Enabled = true;
            txtRemarks.Enabled = true;
            dgvScanItems.ReadOnly = false;
            toolStripButton2.Enabled = true;
            btnConfirm.Enabled = true;
            dtpRepDate.Enabled = false;
            dtpDate.Enabled = false;
            dtpDueDate.Enabled = false;
            ultcmbPriefert.Enabled = true;
            txtCreditCardNo.Enabled = true;
            txtdisRate.Enabled = true;
            cmbSex.Enabled = true;
            txtAge.Enabled = true;

        }

        private void btnList_Click(object sender, EventArgs e)
        {
            user.isLabList = true;
            frmPackageList sl = new frmPackageList();
            sl.ShowDialog();
            if (Search.ScanList != "")
            {
                btnNew_Click(sender, e);
                LoadScan();
                disable();
                btnPrint.Enabled = true;
                if (Confirmchk == false)
                {
                    btnVoid.Enabled = false;
                    toolStripButton4.Enabled = true;
                    btnSave.Enabled = false;
                    btnConfirm.Enabled = true;

                }
                else
                if (Confirmchk == true && VoidChk == false)
                {
                    btnVoid.Enabled = true;
                    btnConfirm.Enabled = false;
                    toolStripButton4.Enabled = false;
                    btnSave.Enabled = false;

                }
                else
                if (VoidChk == true && Confirmchk == true)
                {
                    btnVoid.Enabled = false;
                    btnConfirm.Enabled = false;
                    toolStripButton4.Enabled = false;
                    btnSave.Enabled = false;

                }

            }
            Search.ScanList = "";
        }

        private void disable()
        {
            cmbPackageType.Enabled = false;
            // comboBox1.Enabled = false;
            txtReceiptNo.Enabled = false;
            txtPatientNo.Enabled = false;
            txtTokenNo.Enabled = false;
            txtFirstName.Enabled = false;
            txtContactNo.Enabled = false;
            txtRemarks.Enabled = false;
            dgvScanItems.ReadOnly = true;
            toolStripButton2.Enabled = false;
            btnConfirm.Enabled = false;
             dtpRepDate.Enabled = false;
            dtpDate.Enabled = false;
            dtpDueDate.Enabled = false;
            ultcmbPriefert.Enabled = false;
            txtCreditCardNo.Enabled = false;
            txtdisRate.Enabled = false;
            cmbSex.Enabled = false;
            txtAge.Enabled = false;


        }
        Boolean Confirmchk;
        Boolean VoidChk;
        private void LoadScan()
        {
            try
            {
               
                user.isLabList = false;
                String S2 = "Select * from tblScanChannel where (ReceiptNo =  '" + Search.ScanList + "')AND(Date='" + Search.ScanDate + "')AND(TokenNo='" + Search.tokenNo + "')";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);

                if (dt2.Rows.Count > 0)
                {
                    cmbPackageType.Text = dt2.Rows[0].ItemArray[18].ToString();
                    //comboBox1.Text = dt2.Rows[0].ItemArray[2].ToString();
                    txtPationType.Text = dt2.Rows[0].ItemArray[36].ToString();
                    if (txtPationType.Text.ToString()== "Cash")
                    {
                        rbcash.Checked = true;
                    }
                    txtReceiptNo.Text = dt2.Rows[0].ItemArray[0].ToString();
                    txtPatientNo.Text = dt2.Rows[0].ItemArray[30].ToString();
                    txtTokenNo.Text = dt2.Rows[0].ItemArray[1].ToString();
                    txtFirstName.Text = dt2.Rows[0].ItemArray[5].ToString();
                    txtContactNo.Text = dt2.Rows[0].ItemArray[6].ToString();
                    txtRemarks.Text = dt2.Rows[0].ItemArray[7].ToString();
                   // txtConsultant.Text = dt2.Rows[0].ItemArray[4].ToString();
                   // txtScanName.Text = dt2.Rows[0].ItemArray[3].ToString();
                    dtpDate.Text = dtpDueDate.Text = dtpRepDate.Text = dt2.Rows[0].ItemArray[2].ToString();
                    VoidChk = Convert.ToBoolean(dt2.Rows[0].ItemArray[37].ToString());
                    Confirmchk = Convert.ToBoolean(dt2.Rows[0].ItemArray[35].ToString());
                   
                    ultcmbPriefert.Text = dt2.Rows[0].ItemArray[33].ToString();
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        //dgvScanItems.Rows.Add();
                        dgvScanItems.Rows[i].Cells[0].Value = dt2.Rows[i].ItemArray[3].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[1].Value = dt2.Rows[i].ItemArray[10].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[2].Value = dt2.Rows[i].ItemArray[11].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[3].Value = dt2.Rows[i].ItemArray[12].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[4].Value = dt2.Rows[i].ItemArray[13].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[5].Value = dt2.Rows[i].ItemArray[14].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[6].Value = dt2.Rows[i].ItemArray[15].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[7].Value = dt2.Rows[i].ItemArray[4].ToString().Trim();

                    }
                    cmbPaymentMethod.Text = dt2.Rows[0].ItemArray[8].ToString();
                    txtCreditCardNo.Text = dt2.Rows[0].ItemArray[9].ToString();
                    txtTotal.Text = dt2.Rows[0].ItemArray[17].ToString();
                    txtdisRate.Text = dt2.Rows[0].ItemArray[23].ToString();
                    txtDisAmount.Text = dt2.Rows[0].ItemArray[24].ToString();
                    txtnetTotal.Text = dt2.Rows[0].ItemArray[25].ToString();
                    cmbSex.Text = dt2.Rows[0].ItemArray[41].ToString();
                    txtAge.Text = dt2.Rows[0].ItemArray[42].ToString();
                    if (dt2.Rows[0].ItemArray[48].ToString() != null)
                    {
                        dtpTime.Text = dt2.Rows[0].ItemArray[48].ToString();
                    }
                    //if (dt2.Rows[0].ItemArray[35].ToString() == "True")
                    //{
                    //    toolStripButton4.Enabled = false;
                    //}
                    //else
                    //{
                    //    toolStripButton4.Enabled = true;
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            ConfirmEvent();
            DialogResult reply = MessageBox.Show("Do You Want to Print this record ? ", "Information", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                btnNew_Click(sender, e);
                return;
            }
            else if (reply == DialogResult.OK)
            {
                btnReprintScan_Click(sender, e);
            }

        }

        private void ConfirmEventSave()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                {
                    // string ItemType = "Scan";
                    string IsExport = "true";

                    String S2 = "UPDATE [dbo].[tblScanChannel]SET [IsExport] = '" + IsExport + "',[IsConfirm]='true' WHERE ([ReceiptNo] = '" + txtReceiptNo.Text + "')AND([TokenNo] = '" + txtTokenNo.Text.ToString().Trim() + "')AND([PationType] = '" + txtPationType.Text.ToString() + "')";

                    //  String S2 = "UPDATE [dbo].[tblScanChannel]SET  WHERE ([ReceiptNo] = '" + txtReceiptNo.Text + "')AND([TokenNo] = '" + txtTokenNo.Text.ToString().Trim() + "')AND([PationType] = '" + comboBox1.Text.ToString() + "')";

                    SqlCommand cmd2 = new SqlCommand(S2, myConnection, myTrans);
                    cmd2.CommandType = CommandType.Text;
                    cmd2.ExecuteNonQuery();
                }

                myTrans.Commit();


            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }


        }


        private void ConfirmEvent()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to Confirm this record ? ", "Information", MessageBoxButtons.YesNo);

                if (reply == DialogResult.No)
                {
                    return;
                }
                else if (reply == DialogResult.Yes)
                {
                    myConnection.Open();
                    myTrans = myConnection.BeginTransaction();
                    for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                    {
                        // string ItemType = "Scan";
                        string IsExport = "true";

                        String S2 = "UPDATE [dbo].[tblScanChannel]SET [IsExport] = '" + IsExport + "',[IsConfirm]='true' WHERE ([ReceiptNo] = '" + txtReceiptNo.Text + "')AND([TokenNo] = '" + txtTokenNo.Text.ToString().Trim() + "')AND([PationType] = '" + txtPationType.Text.ToString() + "')";

                        //  String S2 = "UPDATE [dbo].[tblScanChannel]SET  WHERE ([ReceiptNo] = '" + txtReceiptNo.Text + "')AND([TokenNo] = '" + txtTokenNo.Text.ToString().Trim() + "')AND([PationType] = '" + comboBox1.Text.ToString() + "')";

                        SqlCommand cmd2 = new SqlCommand(S2, myConnection, myTrans);
                        cmd2.CommandType = CommandType.Text;
                        cmd2.ExecuteNonQuery();
                    }

                    myTrans.Commit();
                    MessageBox.Show("Confirm Successfully");
                }
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbcash.Checked)
            {
                txtPatientNo.Text = "";
                txtFirstName.Text = "";
                txtContactNo.Text = "";
                txtPatientNo.ReadOnly = true;
                txtFirstName.Focus();
                txtPationType.Text = "Cash";
                txtReceiptNo.Text = "Cash";

            }
            else
            {
                txtPatientNo.Text = "";
                txtPatientNo.ReadOnly = false;
                txtPatientNo.Focus();
                txtReceiptNo.Text = "";
            }
        }

        private void txtPatientNo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (rbcash.Checked == false)
                {
                    user.Ispackage = true;
                    frmScanPation fsp = new frmScanPation();
                    fsp.ShowDialog();
                    if (Search.ScanPation != "")
                    {
                        user.Ispackage = false;
                        LoadPation();
                    }
                    Search.ScanPation = "";
                }
            }
            catch
            {

            }
        }

        private void txtPatientNo_MouseEnter(object sender, EventArgs e)
        {
            //if (cbCash.Checked == false)
            //{
            //    frmLabPation fsp = new frmLabPation();
            //    fsp.ShowDialog();
            //    if (Search.ScanPation != "")
            //    {
            //        LoadPation();
            //    }
            //    Search.ScanPation = "";
            //}
        }

        private void txtPatientNo_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (rbcash.Checked == false)
                {
                    user.Ispackage = true;
                    frmScanPation fsp = new frmScanPation();
                    fsp.ShowDialog();
                    if (Search.ScanPation != "")
                    {
                        user.Ispackage = false;
                        LoadPation();
                    }
                    Search.ScanPation = "";
                }
            }
            catch
            {

            }
        }

        private void cmbTestType_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    //txtConsultant.Text = string.Empty;
            //    //txtScanName.Text = string.Empty;
            //    //// int N = dgvScanItems.RowCount-0 ;
            //    dgvScanItems.Rows.Clear();


            //}
            //catch { }
        }

        private void btnVoid_Click(object sender, EventArgs e)
        {
            if (cmbPackageType.Text == string.Empty)
            {
                MessageBox.Show("Please Select a Test Type");
                return;
            }
            if (txtReceiptNo.Text == "")
            {
                MessageBox.Show("Please Enter a Patient No");
                return;
            }
            if (txtFirstName.Text == "")
            {
                MessageBox.Show("Please Enter a Patient Name");
                return;
            }
            if (txtContactNo.Text == "")
            {
                MessageBox.Show("Please Enter a Contact Number");
                return;
            }
            if (cmbPaymentMethod.Text == "")
            {
                MessageBox.Show("Please Select a Payment Method");
                return;
            }
           
            if (dgvScanItems.Rows.Count < 2)
            {
                MessageBox.Show("Please Select a Scan");
                return;
            }
            voidEvent();
        }

        private void voidEvent()
        {
            for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            {
                // string ItemType = "Scan";

                string ConnString2 = ConnectionString;
                String S2 = "UPDATE [dbo].[tblScanChannel]SET [IsVoid] = 'True' WHERE ([ReceiptNo] = '" + txtReceiptNo.Text + "')AND([TokenNo] = '" + txtTokenNo.Text.ToString().Trim() + "')AND([PationType] = '" + txtPationType.Text.ToString() + "')";

                //  String S2 = "UPDATE [dbo].[tblScanChannel]SET  WHERE ([ReceiptNo] = '" + txtReceiptNo.Text + "')AND([TokenNo] = '" + txtTokenNo.Text.ToString().Trim() + "')AND([PationType] = '" + comboBox1.Text.ToString() + "')";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString2);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
            }
            MessageBox.Show("Void Successfully");
            btnVoid.Enabled = false;
        }

        private void cmbTestType_ValueChanged(object sender, EventArgs e)
        {
            //if (cmbPackageType.Text.ToString() == "WARD")
            //{
            //    rbcash.Enabled = false;
            //}
            //else
            //{
            //    rbcash.Enabled = true;
            //}
        }

        private void dgvScanItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if (e.ColumnIndex == 5 || e.ColumnIndex == 4)
                {
                    //int x = e.RowIndex;
                    //if (dgvScanItems[1, x].Value.ToString() == "DENTAL" || dgvScanItems[0, x].Value.ToString() == "FINAL" || dgvScanItems[0, x].Value.ToString() == "AMBULANCE" || dgvScanItems[0, x].Value.ToString() == "ENDOSCOPY")
                    //{
                    dgvScanItems.BeginEdit(true);
                    //}
                    //else if (!(dgvScanItems[1, x].Value.ToString() == "DENTAL"))
                    //{
                    //}
                }
                if (e.ColumnIndex == 3)
                {
                    dgvScanItems.BeginEdit(true);
                }
            }
            catch { }

        }
        double grosstotal = 0;
        double netToatal = 0;
        private void dgvScanItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvScanItems.CurrentCell.ColumnIndex == 4 || dgvScanItems.CurrentCell.ColumnIndex == 5)
            {
                grosstotal = 0;
                int x = dgvScanItems.CurrentCell.RowIndex;
                dgvScanItems[6, x].Value = Convert.ToDouble(dgvScanItems[4, x].Value) + Convert.ToDouble(dgvScanItems[5, x].Value);

                for (int y = 0; dgvScanItems.RowCount > y; y++)
                {
                    grosstotal += Convert.ToDouble(dgvScanItems[6, y].Value);
                }

                txtTotal.Text = grosstotal.ToString("N2");
                double Rate = 0.00;
                double DiscountAmount = 0.00;
                double totBefoDiscount = 0.00;
                double scandiscount = 0.00;
                double NetTotal = 0.00;
                //grosstotal = 0;
                //netToatal = 0;

                // if (once == 1)
                // {
                try
                {

                    totBefoDiscount = Convert.ToDouble(txtTotal.Text.Trim());
                    scandiscount = Convert.ToDouble(txtScanTotal.Text);

                }
                catch { }
                // once = 2;
                // }

                if (txtdisRate.Text != "")
                {
                    Rate = (Convert.ToDouble(txtdisRate.Text.Trim())) / 100;
                }
                else
                {
                    Rate = 0.00;
                }

                try
                {

                    if (dc == 1)
                    {
                        DiscountAmount = totBefoDiscount * Rate;
                    }
                    else
                    {
                        DiscountAmount = scandiscount * Rate;
                    }
                    // txtDisAmount.Text = Convert.ToString(DiscountAmount);
                    txtDisAmount.Text = DiscountAmount.ToString("N2");
                    NetTotal = totBefoDiscount - DiscountAmount;
                    // txtnetTotal.Text = Convert.ToString(NetTotal);
                    txtnetTotal.Text = NetTotal.ToString("N2");
                }
                catch { MessageBox.Show("Enter Correct Discount Rate"); }
            }
        }

        private void cmbPackageType_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                dgvScanItems.Rows.Clear();
                txtTotal.Text = "0";
                String S = "Select * from tblLabPackage WHERE PackageName='" + cmbPackageType.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                   
                  
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        dgvScanItems.Rows.Add();
                        dgvScanItems.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[6].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[3].Value = "";//Collect Date
                        dgvScanItems.Rows[i].Cells[4].Value = 0;
                        dgvScanItems.Rows[i].Cells[5].Value = 0;
                        dgvScanItems.Rows[i].Cells[6].Value = 0;
                        dgvScanItems.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[8].ToString().Trim();

                    }
                    txtTotal.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                    txtnetTotal.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }
    }
}