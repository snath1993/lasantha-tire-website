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
    public partial class frmXray : Form
    {
        public frmXray()
        {
            InitializeComponent();
            setConnectionString();
        }
        Class1 a = new Class1();
        DataTable dtUser = new DataTable();
        public int flgTech = 0;
        public int flg = 0;
        public int CheckSearch = 0;//to avoid goto textchange event after find result

        public DsScan DsScanReport = new DsScan();


        bool run = false;
        bool add = false;
        bool edit = false;
        bool delete = false;
 

        public static string ConnectionString;

        public void setConnectionString()
        {
            try
            {
                TextReader tr = new StreamReader("Connection.txt");
                ConnectionString = tr.ReadLine();
                tr.Close();
            }
            catch { }
        }

        private void btnConsultant_Click(object sender, EventArgs e)
        {
            try
            {
                flgTech = 2;
                frmSelectConsult frm = new frmSelectConsult(this);
                frm.Show();
            }
            catch { } 
        }

        private void frmXray_Activated(object sender, EventArgs e)
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
                        if (Class1.flg == 1)
                        {
                            txtConsultant.Text = a.GetText();
                            DataAccess.Access.Consultant = a.GetText();
                            Class1.flg = 0;
                        }

                        if (Class1.flg2 == 1)
                        {
                            txtScanName.Text = a.GetText2();
                            //dgvScanItems.Rows.Add();
                            int x = dgvScanItems.Rows.Count - 1;
                            dgvScanItems.Rows.Add();


                            string ConnString = ConnectionString;
                            string sql = "Select * from tblConsultantItemFee where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (ItemID = '" + txtScanName.Text.ToString().Trim() + "')";
                            SqlConnection Conn = new SqlConnection(ConnString);
                            SqlCommand cmd = new SqlCommand(sql);
                            SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // dgvScanItems[0, x].Value = a.GetText2();
                            if (dt.Rows.Count > 0)
                            {
                                dgvScanItems[0, x].Value = dt.Rows[0].ItemArray[1].ToString().Trim();
                                dgvScanItems[1, x].Value = dt.Rows[0].ItemArray[2].ToString().Trim();
                                dgvScanItems[2, x].Value = dt.Rows[0].ItemArray[6].ToString().Trim();
                                dgvScanItems[3, x].Value = dt.Rows[0].ItemArray[5].ToString().Trim();
                                dgvScanItems[4, x].Value = dt.Rows[0].ItemArray[3].ToString().Trim();
                                dgvScanItems[5, x].Value = dt.Rows[0].ItemArray[4].ToString().Trim();
                                dgvScanItems[6, x].Value = Convert.ToDouble(dt.Rows[0].ItemArray[3].ToString().Trim()) + Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString().Trim());
                                // ReceiptTotal = ReceiptTotal + Convert.ToDouble (dgvScanItems[4, x].Value);
                                txtTotal.Text = Convert.ToString(Convert.ToDouble(dgvScanItems[4, x].Value));
                                // txtScanTotal.Text = Convert.ToString(Convert.ToDouble(dgvScanItems[5, x].Value));
                            }
                            //ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems[4, x].Value);
                            // ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems.Columns[4].ToString());
                            // txtTotal.Text = Convert.ToString(ReceiptTotal);
                            Class1.flg2 = 0;
                        }

                        if (Class1.flg3 == 1)
                        {

                            txtConsultantFind.Text = a.GetText3();
                            Class1.flg3 = 0;
                        }

                        if (Class1.flg4 == 1)
                        {
                            txtConsultant.Text = a.GetText4();
                            DataAccess.Access.Consultant = a.GetText4();
                            Class1.flg4 = 0;
                        }

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
                    catch { }
                }
            }
        }

        private void txtConsultant_TextChanged(object sender, EventArgs e)
        {
            if (CheckSearch != 2)
            {
                //try
                //{
                //    string ItemT = "Xray";
                //    string ConnString = ConnectionString;
                //    //string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
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
                //    //AppointmentNo = AppointmentNo + 1;

                //}
                //catch { }


                //...................................
                //try
                //{


                //    String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //    SqlCommand cmd1 = new SqlCommand(S1);
                //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                //    DataTable dt = new DataTable();
                //    da1.Fill(dt);
                //    int NoOfTokens = 1;
                //    if (dt.Rows.Count > 0)
                //    {
                //        //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                //        // {
                //        // NoOfTokens = 1;
                //        // }
                //        //else
                //        //{
                //        NoOfTokens = dt.Rows.Count + 2;
                //        // }
                //    }
                //    else
                //    {
                //        NoOfTokens = 2;
                //    }
                //    txtTokenNo.Text = NoOfTokens.ToString();
                //}
                //catch { }
            }
        }

        //.................

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

        private void btnScanNames_Click(object sender, EventArgs e)
        {
            DataAccess.Access.type = "Xray";
            frmItemList frm = new frmItemList();
            frm.Show();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (add)
                {
                    rbtncash.Enabled = true;
                    rbtnetuopd.Enabled = true;
                    rbtnbht.Enabled = true;


                    rbtncash.Checked = true;

                    cmbPaymentMethod.Text = "Cash";
                    A = 0;
                    //................
                    txtdisRate.Text = "0";
                    txtDisAmount.Text = "0";
                    txtnetTotal.Text = "0";
                    txtTotal.Text = "0";
                    Clear_Click(sender, e);
                    //................


                    txtdisRate.ReadOnly = false;
                    ClearText();
                    EnableObjects();
                   // getTokenNo();

                    string ConnString = ConnectionString;
                    string sql = "Select HospitalCharge from tblDefaultSettings";
                    SqlConnection Conn = new SqlConnection(ConnString);
                    SqlCommand cmd = new SqlCommand(sql);
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                      //  txtHospitalFee.Text = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                    }
                    btnNew.Enabled = false;
                    btnEdit.Enabled = false;
                    flg = 2;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }

            cmbScanSerch.Enabled = false;
            txtConsultantFind.Enabled = false;

        }

        //.......................
        public void ClearText()
        {
            try
            {
                dtpDueDate.Text = "";
                //txtTechnician.Text = "";
                txtTokenNo.Text = "";
                dtpDate.Text = "";
                txtScanName.Text = "";
                txtConsultant.Text = "";
                txtFirstName.Text = "";
                txtContactNo.Text = "";
                txtRemarks.Text = "";
               // txtScanFee.Text = "0";
               // txtHospitalFee.Text = "0";
                //txtTotal.Text = "0";
                cmbPaymentMethod.Text = "";
                txtCreditCardNo.Text = "";
            }
            catch { }
        }
        //..............
        public void EnableObjects()
        {
            try
            {
                //txtTokenNo.ReadOnly = false;
                cbxInpatient.Enabled = true;
                //txtPatientNo.ReadOnly = false;
                txtReceiptNo.Enabled = true;
                dtpDate.Enabled = true;
                txtScanName.ReadOnly = false;
                txtConsultant.ReadOnly = false;
                txtFirstName.ReadOnly = false;
                txtContactNo.ReadOnly = false;
                txtRemarks.ReadOnly = false;
              //  txtScanFee.ReadOnly = false;
               // txtHospitalFee.ReadOnly = false;
               // txtTotal.ReadOnly = false;
                cmbPaymentMethod.Enabled = true;
                txtCreditCardNo.ReadOnly = false;
               // txtTechnician.ReadOnly = false;
                dtpDueDate.Enabled = true;
              //  dtpRepDate.Enabled = true;
               // dtpRepDate

                btnConsultant.Enabled = true;
                btnNew.Enabled = true;
                btnEdit.Enabled = true;
                btnSave.Enabled = true;
                btnDelete.Enabled = true;

            }
            catch { }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (edit)
                {
                    EnableObjects();
                    btnNew.Enabled = false;
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = true;
                    dtpDate.Enabled = false;
                    btnScanNames.Enabled = false;
                    dtpDueDate.Enabled = false;
                    btnConsultant.Enabled = false;
                    dgvScanItems.Enabled = false;
                    txtConsultant.Enabled = false;
                    txtScanName.Enabled = false;
                    flg = 1;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void frmXray_Load(object sender, EventArgs e)
        {
            try
            {
                dtUser.Clear();
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmXray");
                if (dtUser.Rows.Count > 0)
                {

                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    edit = Convert.ToBoolean(dtUser.Rows[0].ItemArray[4].ToString());
                    delete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());

                }
            }
            catch { }
        }



        //========================================
        public void exporetSalesInvoice()
        {
            //Create a Xmal File..................................................................................

            try
            {
                XmlTextWriter Writer = new XmlTextWriter(@"c:\\pbss\\Xray.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count-1);

               // int gridcount = dgvScanItems.Rows.Count;
                //int ab = (gridcount * 2) - 2;



                Writer.WriteStartElement("PAW_Invoice");
                Writer.WriteAttributeString("xsi:type", "paw:Receipt");


                if (rbtnbht.Checked)
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(txtPatientNo.Text.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else
                {


                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }

                Writer.WriteStartElement("Date");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(dtpRepDate.Text.ToString().Trim());//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(txtReceiptNo.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Accounts_Receivable_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString("65000");//Cash Account
                Writer.WriteEndElement();//CreditMemoType

                Writer.WriteStartElement("CreditMemoType");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString(NoDistributions);//no of distributors
                Writer.WriteEndElement();

                Writer.WriteStartElement("SalesLines");
                for (int i = 0; i <= dgvScanItems.Rows.Count - 2; i++)
                {

                    //Writer.WriteStartElement("SalesLine");

                    //Writer.WriteStartElement("Quantity");
                    //Writer.WriteString("1");//Doctor Charge
                    //Writer.WriteEndElement();


                    //Writer.WriteStartElement("Item_ID");
                    //Writer.WriteString("D0001");
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Description");
                    //Writer.WriteString("Doctor Charges");
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("GL_Account");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    //Writer.WriteString("10008");
                    //Writer.WriteEndElement();


                    ////========================================================
                    //Writer.WriteStartElement("Tax_Type");
                    //Writer.WriteString("1");//Doctor Charge
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Amount");
                    //Writer.WriteString("-" + dgvScanItems[4, i].Value.ToString().Trim());//HospitalCharge
                    //Writer.WriteEndElement();
                    //Writer.WriteEndElement();



                    //===================================================
                    Writer.WriteStartElement("SalesLine");

                    // Writer.WriteString(dgvScanItems[3, i].Value.ToString().Trim());//Doctor Charge

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
                    Writer.WriteEndElement();


                    //========================================================
                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + dgvScanItems[5, i].Value.ToString().Trim());//HospitalCharge
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();//LINE
                }
                Writer.WriteEndElement();//LINES

                Writer.WriteEndElement();

                Writer.Close();

                Connector abc = new Connector();//export to peach tree
                abc.ImportSalesInvice();//ImportSalesInvice()
            }

            catch { }


        }
        //======================================
        public int A = 0;
        private void btnSave_Click(object sender, EventArgs e)
        {
            A = 1;
            //=============================================================
            if (dtpDate.Value < System.DateTime.Now)
            {
                //MessageBox.Show("You cant select previous Date");
                dtpDate.Text = Convert.ToString(System.DateTime.Now);
                // dtpDate.Focus();

                try
                {


                    String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);
                    int NoOfTokens = 1;
                    if (dt.Rows.Count > 0)
                    {
                        //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                        // {
                        // NoOfTokens = 1;
                        // }
                        //else
                        //{
                        NoOfTokens = dt.Rows.Count + 2;
                        // }
                    }
                    else
                    {
                        NoOfTokens = 2;
                    }
                    txtTokenNo.Text = NoOfTokens.ToString();
                }
                catch { }
            }
            else
            {
                try
                {


                    String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);
                    int NoOfTokens = 1;
                    if (dt.Rows.Count > 0)
                    {
                        //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                        // {
                        // NoOfTokens = 1;
                        // }
                        //else
                        //{
                        NoOfTokens = dt.Rows.Count + 2;
                        // }
                    }
                    else
                    {
                        NoOfTokens = 2;
                    }
                    txtTokenNo.Text = NoOfTokens.ToString();
                }
                catch { }


            }

            //==============================================================

            try
            {
                string ItemT = "Xray";
                string ConnString = ConnectionString;
                //string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
                string sql = "Select ReceiptNo from tblScanChannel Where ItemType='" + ItemT + "' ORDER BY ReceiptNo";
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

                    txtReceiptNo.Text = NewID;
                }
                else
                {
                    String S2 = "Select ReceiptsNo from tblDefaultSettings";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da2.Fill(dt1);

                    if (dt1.Rows.Count > 0)
                    {
                        string P1 = dt1.Rows[dt1.Rows.Count - 1].ItemArray[0].ToString().Trim();
                        string NewID = getNextID(P1);

                        txtReceiptNo.Text = NewID;
                    }
                }


            }
            catch { }

            //==============================================================


            if ((txtFirstName.Text == "") || (txtReceiptNo.Text == "") || cmbPaymentMethod.Text == "")
            {
                if (txtReceiptNo.Text == "")
                {
                    MessageBox.Show("Please Enter a Patient No");
                }

                if (txtFirstName.Text == "")
                {

                    MessageBox.Show("Please Enter a Patient Name");
                }
                if (cmbPaymentMethod.Text == "")

                { MessageBox.Show("Please Select a Payment Method"); }
            }
            else
            {
                //try
                //{
                //    string ItemT = "Xray";
                //    string ConnString = ConnectionString;
                //    //string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
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


//===========================================

             

                try
                {
                    if (flg == 2)
                    {
                        for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                        {
                            string ItemType = "Xray";
                            string IsExport ="False";
                            string ConnString2 = ConnectionString;
                           // String S2 = "insert into tblScanChannel(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal) values ('" + txtReceiptNo.Text + "','" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value + "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dgvScanItems[6, i].Value + "', '" + dtpDueDate.Text.ToString().Trim() + "','" + txtTotal.Text.ToString().Trim() + "','" + ItemType + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "')";
                            //String S2 = "insert into tblScanChannel(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal) values ('" + txtReceiptNo.Text + "','" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value + "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dgvScanItems[6, i].Value + "', '" + dtpDueDate.Text.ToString().Trim() + "','" + txtTotal.Text.ToString().Trim() + "','" + ItemType + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "')";
                            String S2 = "insert into tblScanChannel(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal,PatientNo,UrgentFee) values ('" + txtReceiptNo.Text + "','" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value + "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dgvScanItems[6, i].Value + "', '" + dtpDueDate.Text.ToString().Trim() + "','" + txtTotal.Text.ToString().Trim() + "','" + ItemType + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" + txtPatientNo.Text.ToString() + "','0')";
                            SqlCommand cmd2 = new SqlCommand(S2);
                            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString2);
                            DataSet ds2 = new DataSet();
                            da2.Fill(ds2);
                        }
                        //MessageBox.Show("Saved Successfully");
                        flg = 0;

                        //.....................
                        //string ConnString3 = ConnectionString;
                        //String S3 = "insert into tblChannelingDetails(ReceiptsNo)values ('" + txtReceiptNo.Text + "')";//,'" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + txtScanName.Text.ToString().Trim() + "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "' , '" + txtTotal.Text.ToString().Trim() + "', '" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dtpDueDate.Text.ToString().Trim() + "', '" + txtTechnician.Text.ToString().Trim() + "')";
                        //SqlCommand cmd3 = new SqlCommand(S3);
                        //SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnString3);
                        //DataSet ds3 = new DataSet();
                        //da3.Fill(ds3);

                       // Connector abc = new Connector();
                       // abc.Import_Receipt_Journal();
                        //if (rbtnbht.Checked == true)
                        //{
                        //    exporetSalesInvoice();
                        //    btnReprintXray_Click(sender, e);
                        //}
                        //else
                        //{
                        //    btnReprintXray_Click(sender, e);
                        //}
                        //btnReprintScan_Click(sender, e);
                        //..........................
                        exporetSalesInvoice();

                        MessageBox.Show("Saved Successfully");
                        btnReprintXray_Click(sender, e);


                    }
                    else
                    {
                        for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                        {
                            string ItemType = "Xray";
                            // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + txtScanName.Text.ToString().Trim() + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[0, i].Value + "',GLAccount = '" + dgvScanItems[1, i].Value + "', Duration = '" + dgvScanItems[2, i].Value + "', ConsultFee = '" + dgvScanItems[3, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "'  where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                            String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='"+user.userName.ToString().Trim()+"',RepDate='"+dtpRepDate.Text.ToString().Trim() +"'  where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                            SqlCommand cmd = new SqlCommand(S);
                            SqlConnection con = new SqlConnection(ConnectionString);
                            SqlDataAdapter da = new SqlDataAdapter(S, con);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                        }
                        MessageBox.Show("Updated Successfully");

                        flg = 0;

                    }
                    // btnPrint_Click(sender, e);
                    ClearText();
                    txtTokenNo.Text = "";
                    DisableObjects();

                    btnNew.Enabled = true;
                   // btnEdit.Enabled = true;
                }
                catch { }
            }
        }


        public void DisableObjects()
        {
            try
            {
                txtTokenNo.ReadOnly = true;
                dtpDate.Enabled = false;
                txtScanName.ReadOnly = true;
                txtConsultant.ReadOnly = true;
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
                btnNew.Enabled = false;
                btnEdit.Enabled = false;
                btnSave.Enabled = false;
            }
            catch { }
        }


       

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (delete)
                {
                    String S = "delete from tblScanChannel WHERE (TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "') AND (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlConnection con = new SqlConnection(ConnectionString);
                    SqlDataAdapter da = new SqlDataAdapter(S, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    MessageBox.Show("Deleted Successfully");
                    ClearText();
                    DisableObjects();
                    btnDelete.Enabled = false;
                    btnNew.Enabled = true;
                    btnEdit.Enabled = true;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        //=======================================================================

        public double NetTotalRefund = 0.00;
        // public double DoctorCGRefund = 0.00;
        // public double HosCGRefund = 0.00;


        public void getScanData()
        {
            string add = txtReceiptNo.Text;
            string ItemType = "Xray";
            if (add != "")
            {
                // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                String S2 = "Select * from tblScanChannel where (ReceiptNo LIKE  '" + add + "%')AND (ItemType='" + ItemType + "')";
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

                    txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                    txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
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

        //=======================================================================
        private void txtConsultantFind_TextChanged(object sender, EventArgs e)
        {
            CheckSearch = 2;
            if (cmbScanSerch.Text == "Patient Name")
            {
                string add = txtConsultantFind.Text;
                string ItemType = "Xray";
                if (add != "")
                {
                    // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                    String S2 = "Select * from tblScanChannel where (PatientName LIKE  '" + add + "%')AND (ItemType='" + ItemType + "')";
                    // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    DataTable dt = new DataTable();
                    da2.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();
                        txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                        txtTokenNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

                        txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                        txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
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

                        txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();

                        dgvScanItems.DataSource = dt;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //  dgvScanItems.Rows.Add();
                            // dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[10].ToString();
                            dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
                            dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
                            dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();
                            dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();
                            dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[13].ToString();
                            dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
                            dgvScanItems.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();
                        }

                    }
                }
                else
                {
                    //MessageBox.Show("Receipt not found");
                }
            }

            //.............
            if (cmbScanSerch.Text == "ReceiptNo")
            {
                string add = txtConsultantFind.Text;
                string ItemType = "Xray";
                if (add != "")
                {
                    // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                    String S2 = "Select * from tblScanChannel where (ReceiptNo LIKE  '" + add + "%')AND (ItemType='" + ItemType + "')";
                    // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    DataTable dt = new DataTable();
                    da2.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();
                        txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                        txtTokenNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

                        txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                        txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
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

                        txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();

                        dgvScanItems.DataSource = dt;
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
                        }

                    }
                }
                else
                {
                    //MessageBox.Show("Receipt not found");
                }
            }

            //..............
            if (cmbScanSerch.Text == "ContactNo")
            {
                string add = txtConsultantFind.Text;
                string ItemType = "Xray";
                if (add != "")
                {
                    // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                    String S2 = "Select * from tblScanChannel where (ContactNo LIKE  '" + add + "%')AND (ItemType='" + ItemType + "')";
                    // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    DataTable dt = new DataTable();
                    da2.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();
                        txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                        txtTokenNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

                        txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                        txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
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

                        txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();

                        dgvScanItems.DataSource = dt;
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
                        }

                    }
                }
                else
                {
                    //MessageBox.Show("Receipt not found");
                }
            }
            groupBox3.Enabled = true;
            btnNew.Enabled = true;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnDelete.Enabled = true;
            btnReprintXray.Enabled = true;
            btnRefund.Enabled = true;


            //txtTokenNo.ReadOnly = false;
            btnScanNames.Enabled = false;

            txtReceiptNo.Enabled = false;
            dtpDate.Enabled = false;
            txtScanName.ReadOnly = true;
            txtConsultant.ReadOnly = true;
            txtFirstName.ReadOnly = true;
            txtContactNo.ReadOnly = true;
            txtRemarks.ReadOnly = true;
            //  txtScanFee.ReadOnly = false;
            // txtHospitalFee.ReadOnly = false;
            // txtTotal.ReadOnly = false;
            cmbPaymentMethod.Enabled = false;
            txtCreditCardNo.ReadOnly = false;
            // txtTechnician.ReadOnly = false;
            dtpDueDate.Enabled = false;
            Clear.Enabled = false;
            // dtpRepDate.Enabled = true;
            //dtpRepDate

            btnConsultant.Enabled = false;
            btnNew.Enabled = false;
            // btnEdit.Enabled = true;
            btnSave.Enabled = false;
            txtCreditCardNo.Enabled = false;


        }

        private void DirectPrint()
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\rptXray.rpt") == true)
                {
                    Myfullpath = Path.GetFullPath("rptXray.rpt");
                }
                else
                {
                    MessageBox.Show("rptXray.rpt not Exists");
                    this.Close();
                    return;
                }
                crp.Load(Myfullpath);
                crp.SetDataSource(DsScanReport);
                crp.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
        private void btnReprintXray_Click(object sender, EventArgs e)
        {
            try
            {
                DsScanReport.Clear();

                string ItemType = "Xray";
                String S1 = "Select * from tblScanChannel where (ReceiptNo = '" + txtReceiptNo.Text.ToString().Trim() + "') AND (ItemType='" + ItemType + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DsScanReport, "dtScanData");

                //frmScanReport frms = new frmScanReport(this);
               // frms.Show();

                XrayReport frms = new XrayReport(this);
                frms.Show();
             //  DirectPrint();
                //frmChannelingReceipt frm = new frmChannelingReceipt(this);
                // frm. Show();
            }
            catch { }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
          //  dgvScanItems.Rows.Clear();
            try
            {
                int N = dgvScanItems.Rows.Count - 2;
                dgvScanItems.Rows.RemoveAt(N);

            }
            catch { }
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
                        string ItemType = "Xray";
                        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + IsExport + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',DisAmount='" + txtDisAmount.Text.ToString().Trim() + "',NetTotal='" + txtnetTotal.Text.ToString().Trim() + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";//before scanrefund18-06-2009
                        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + IsExport + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',DisAmount='" + txtDisAmount.Text.ToString().Trim() + "',NetTotal='" + txtnetTotal.Text.ToString().Trim() + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[4, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
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
                        string ItemType = "Xray";
                        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + txtScanName.Text.ToString().Trim() + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[0, i].Value + "',GLAccount = '" + dgvScanItems[1, i].Value + "', Duration = '" + dgvScanItems[2, i].Value + "', ConsultFee = '" + dgvScanItems[3, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "'  where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        //String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',IsExport='" + IsExport + "', DisRate='" + txtdisRate.Text.ToString().Trim() + "', DisAmount='" + txtDisAmount.Text.ToString().Trim() + "', NetTotal='" + txtnetTotal.Text.ToString().Trim() + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',IsExport='" + IsExport + "', DisRate='" + txtdisRate.Text.ToString().Trim() + "', DisAmount='" + txtDisAmount.Text.ToString().Trim() + "', NetTotal='" + txtnetTotal.Text.ToString().Trim() + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[4, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";

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
                        string ItemType = "Xray";
                        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + IsExport + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',DisAmount='" + txtDisAmount.Text.ToString().Trim() + "',NetTotal='" + txtnetTotal.Text.ToString().Trim() + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";//before scanrefund18-06-2009
                        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + IsExport + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',DisAmount='" + txtDisAmount.Text.ToString().Trim() + "',PostedRefund='" + postrefund + "',NetTotal='" + txtnetTotal.Text.ToString().Trim() + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[4, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
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
                        string ItemType = "Xray";
                        // String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + txtScanName.Text.ToString().Trim() + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[0, i].Value + "',GLAccount = '" + dgvScanItems[1, i].Value + "', Duration = '" + dgvScanItems[2, i].Value + "', ConsultFee = '" + dgvScanItems[3, i].Value + "', HospitalFee = '" + dgvScanItems[4, i].Value + "', TotalFee = '" + dgvScanItems[5, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "'  where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        //String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',IsExport='" + IsExport + "', DisRate='" + txtdisRate.Text.ToString().Trim() + "', DisAmount='" + txtDisAmount.Text.ToString().Trim() + "', NetTotal='" + txtnetTotal.Text.ToString().Trim() + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";
                        String S = "Update tblScanChannel SET Date  = '" + dtpDate.Text.ToString().Trim() + "', ItemID = '" + dgvScanItems[0, i].Value + "', Consultant  = '" + txtConsultant.Text.ToString().Trim() + "', PatientName  = '" + txtFirstName.Text.ToString().Trim() + "', ContactNo = '" + txtContactNo.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', ItemDescription = '" + dgvScanItems[1, i].Value + "',GLAccount = '" + dgvScanItems[2, i].Value + "', Duration = '" + dgvScanItems[3, i].Value + "', ConsultFee = '" + dgvScanItems[4, i].Value + "', HospitalFee = '" + dgvScanItems[5, i].Value + "', TotalFee = '" + dgvScanItems[6, i].Value + "',ReceiptTotal='" + txtTotal.Text.ToString().Trim() + "',TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "',CurrentUser='" + user.userName.ToString().Trim() + "',RepDate='" + dtpRepDate.Text.ToString().Trim() + "',IsExport='" + IsExport + "', DisRate='" + txtdisRate.Text.ToString().Trim() + "', DisAmount='" + txtDisAmount.Text.ToString().Trim() + "', NetTotal='" + txtnetTotal.Text.ToString().Trim() + "',PostedRefund='" + postrefund + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + dgvScanRefundSave[4, i].Value + "',HosCal='" + dgvScanRefundSave[5, i].Value + "' where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')AND (ReceiptNo = '" + txtReceiptNo.Text.ToString() + "')AND (ItemType = '" + ItemType + "')";

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


                //}
            }
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

        private void txtScanName_TextChanged(object sender, EventArgs e)
        {

        }
        //========================
        //public void getScanData()
        //{
        //    string add = txtReceiptNo.Text;
        //    string ItemType = "Xray";
        //    if (add != "")
        //    {
        //        // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
        //        String S2 = "Select * from tblScanChannel where (ReceiptNo LIKE  '" + add + "%')AND (ItemType='" + ItemType + "')";
        //        // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
        //        SqlCommand cmd2 = new SqlCommand(S2);
        //        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //        DataTable dt = new DataTable();
        //        da2.Fill(dt);

        //        if (dt.Rows.Count > 0)
        //        {
        //            dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();//Receipt Date
        //            txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
        //            txtTokenNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
        //            dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

        //            txtScanName.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
        //            txtConsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
        //            txtFirstName.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
        //            txtContactNo.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
        //            txtRemarks.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
        //            // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
        //            // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
        //            txtTotal.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
        //            cmbPaymentMethod.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
        //            txtCreditCardNo.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
        //            txtdisRate.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
        //            txtDisAmount.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
        //            txtnetTotal.Text = dt.Rows[0].ItemArray[25].ToString().Trim();


        //            dgvScanItems.DataSource = dt;
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                //  dgvScanItems.Rows.Add();
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




        //}
        //================================
        public int dc = 0;
        private void rbtnDoctorfee_CheckedChanged(object sender, EventArgs e)
        {
            // frmScan_Activated(sender,e);
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
            dc = 0;
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

        private void txtPatientNo_TextChanged(object sender, EventArgs e)
        {
            if (rbtnbht.Checked == true || rbtnetuopd.Checked == true)
            {
                if (CheckSearch != 2)
                {
                    try
                    {
                        //string cusType = "";
                        string add = txtPatientNo.Text;
                        if (add != "")
                        {

                            String S2 = "Select * from tblPatientsDetails where PatientNo LIKE  '" + add + "%'";
                            SqlCommand cmd2 = new SqlCommand(S2);
                            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);

                            if (dt2.Rows.Count > 0)
                            {

                                txtFirstName.Text = dt2.Rows[0].ItemArray[2].ToString();
                                txtContactNo.Text = dt2.Rows[0].ItemArray[16].ToString();

                                // dt2.Rows[0].ItemArray[28].ToString();
                                if (dt2.Rows[0].ItemArray[28].ToString() == "Member")
                                {
                                    // lblCustomerType.Visible = true;
                                    // txtCType.Text = dt2.Rows[0].ItemArray[28].ToString();
                                    // txtCType.Visible = true;

                                }
                                else
                                {
                                    // lblCustomerType.Visible = false ;
                                    //txtCType.Visible  = false;
                                }

                                if (dt2.Rows[0].ItemArray[28].ToString() == "Deposit Holder")
                                {
                                    // lblCustomerType.Visible = true;
                                    // txtCType.Text = dt2.Rows[0].ItemArray[28].ToString();
                                    //txtCType.Visible = true;

                                }
                                else
                                {
                                    //lblCustomerType.Visible = false;
                                    // txtCType.Visible = false; 
                                }






                            }
                            else
                            {
                                txtPatientNo.Text = "";
                                txtFirstName.Text = "";
                                txtContactNo.Text = "";
                                // lblCustomerType.Visible = false;
                                // txtCType.Visible = false;
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private void txtFirstName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (rbtncash.Checked == true)
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

        private void cbxInpatient_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxInpatient.Checked == true)
            {
                txtPatientNo.ReadOnly = false;
                txtPatientNo.Focus();
            }
            else
            {
                txtPatientNo.ReadOnly = true;
            }
        }

        private void txtRemarks_TextChanged(object sender, EventArgs e)
        {

        }

        private void rbtncash_CheckedChanged(object sender, EventArgs e)
        {
            txtPatientNo.Text = "";
            txtFirstName.Text = "";
            txtContactNo.Text = "";
            txtPatientNo.ReadOnly = true;
            txtFirstName.Focus();
        }

        private void rbtnetuopd_CheckedChanged(object sender, EventArgs e)
        {
            txtPatientNo.Text = "";
            txtPatientNo.ReadOnly = false;
            txtPatientNo.Focus();
        }

        private void rbtnbht_CheckedChanged(object sender, EventArgs e)
        {
            txtPatientNo.Text = "";
            txtPatientNo.ReadOnly = false;
            txtPatientNo.Focus();
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {

        }
        //............

    }
}