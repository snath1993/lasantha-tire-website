using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using UserAutherization;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;


namespace UserAutherization
{
    public partial class frmChanneling : Form
    {
        Class1 a = new Class1();
        DataTable dtUser = new DataTable();

        public string Discountval = "";

        bool run = false;
        bool add = false;
        bool edit = false;
        bool delete = false;

        public int flg = 0;
        int flg2 = 0; // for category
        //int flg3 = 0; // for consultant
        int flgNew = 0; // for New No
        int flg4 = 0;
        public DsChanneling dsAll = new DsChanneling();
        public static string ConnectionString;
        public int CheckSearch = 0;


        public frmChanneling(frmDisountValidate frmParent)
        {

            InitializeComponent();
            setConnectionString();

            //  ds = frmParent.dsAll;

        }



        public frmChanneling()
        {
            try
            {
                InitializeComponent();
                setConnectionString();

                string ConnString = ConnectionString;
                String S1 = "Select Type from tblConsultantType";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        ListViewItem item = this.lstvCategory.Items.Add(dt1.Rows[i].ItemArray[0].ToString());
                        //item.SubItems.Add(dt1.Rows[i].ItemArray[1].ToString());
                        //item.SubItems.Add(dt1.Rows[i].ItemArray[2].ToString());
                    }
                }
            }
            catch { }
        }

        //Method to establish the connection
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch { }
        }

        public void EnableObjects()
        {
            try
            {
                txtReceiptNo.Text = "";
                // txtTokenNo.ReadOnly = false;
                // txtHospitalCharge.ReadOnly = false;
                cbxInpatient.Enabled = true;
                dtpDateFrom.Enabled = true;
                // txtPatientNo.ReadOnly = false;
                txtFirstName.ReadOnly = false;
                // txtConsultant.ReadOnly = false;
                //txtCategory.ReadOnly = false;
                //dtpReceiptDate.Enabled = true;
                // dtpReceiptDate
                //cmbSession.Enabled = true;
                //txtRoom.ReadOnly = false;
                txtContactNo.ReadOnly = false;
                txtRemarks.ReadOnly = false;
                // txtHospitalCharge.ReadOnly = false;
                // txtConsultantCharge.ReadOnly = false;
                // txtTotal.ReadOnly = false;
                // txtCHosCharge.ReadOnly = false;
                //txtCConsCharge.ReadOnly = false;
                txtCTotal.ReadOnly = false;
                cmbPaymentMethod.Enabled = true;
                btnCategory.Enabled = true;
                btnConsultant.Enabled = true;
                btnNewNo.Enabled = true;
                btnSave.Enabled = true;
                //txtCreditCardNo.ReadOnly = false;
                cmbCurrency.Enabled = true;
                txtRoom.ReadOnly = false;
                // txtTokenNo.ReadOnly = false;
                txtdisRate.ReadOnly = false;
            }
            catch { }
        }

        public void DisableObjects()
        {
            try
            {
                txtReceiptNo.ReadOnly = true;
                txtTokenNo.ReadOnly = true;
                txtPatientNo.ReadOnly = true;
                txtFirstName.ReadOnly = true;
                txtConsultant.ReadOnly = true;
                txtCategory.ReadOnly = true;
                dtpDateFrom.Enabled = false;
                cmbSession.Enabled = false;
                txtRoom.ReadOnly = true;
                txtContactNo.ReadOnly = true;
                txtRemarks.ReadOnly = true;
                txtHospitalCharge.ReadOnly = true;
                txtConsultantCharge.ReadOnly = true;
                txtTotal.ReadOnly = true;
                txtCHosCharge.ReadOnly = true;
                txtCConsCharge.ReadOnly = true;
                txtCTotal.ReadOnly = true;
                cmbPaymentMethod.Enabled = false;
                btnCategory.Enabled = false;
                btnConsultant.Enabled = false;
                btnNewNo.Enabled = false;
                btnSave.Enabled = false;
                txtCreditCardNo.ReadOnly = true;
            }
            catch { }
        }
        public int newClick = 0;
        public int checkreset = 0;

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (add)
                {

                    cbxPhoneBooking.Checked = false;
                    if (cbxPhoneBooking.Checked == true)
                    {
                        cmbPaymentMethod.Text = "Phone Booking";
                    }
                    else
                    {
                        cmbPaymentMethod.Text = "Cash";

                    }

                    checkreset = 1;
                    cmbSearchReceipt.Enabled = false;
                    btnclear.Enabled = true;
                    rbtncash.Enabled = true;
                    rbtnetuopd.Enabled = true;
                    rbtnbht.Enabled = true;


                    rbtncash.Checked = true;


                    newClick = 1;
                    EnableObjects();
                    ClearText();
                    // getReceiptNo();
                    txtDocDisRate.Text = "0";
                    txtDocDisRate.Enabled = false;

                    //string ConnString = ConnectionString;
                    //string sql = "Select HospitalCharge from tblDefaultSettings";
                    //SqlConnection Conn = new SqlConnection(ConnString);
                    //SqlCommand cmd = new SqlCommand(sql);
                    //SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                    //DataSet ds = new DataSet();
                    //adapter.Fill(ds);
                    //if (ds.Tables[0].Rows.Count > 0)
                    //{
                    //    txtHospitalCharge.Text = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    //    txtCHosCharge.Text = txtHospitalCharge.Text;
                    //}
                    btnNew.Enabled = false;
                    btnEdit.Enabled = false;

                    txtFind.Enabled = false;
                    cmbSearchReceipt.Enabled = false;
                    txtdisRate.Enabled = true;
                    txtdisRate.ReadOnly = false;
                    btnDiscount.Enabled = true;
                    cbxPhoneBooking.Enabled = true;
                    dtpDateFrom.Focus();


                    // txtPatientNo.Focus();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
            //txtFind.Enabled = false;
            //cmbSearchReceipt.Enabled = false;
            //txtdisRate.Enabled = true;
            //txtdisRate.ReadOnly = false;
            //btnDiscount.Enabled = true;
            //cbxPhoneBooking.Enabled = true;
        }

        //public void getReceiptNo()
        //{
        //    try
        //    {
        //        string type = "Channel";
        //        string ConnString = ConnectionString;
        //        string sql = "Select ReceiptsNo from tblChannelingDetails where Type='" + type + "' ORDER BY ReceiptsNo";
        //        SqlConnection Conn = new SqlConnection(ConnString);
        //        SqlCommand cmd = new SqlCommand(sql);
        //        SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
        //        DataSet ds = new DataSet();
        //        adapter.Fill(ds);

        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            int p = ds.Tables[0].Rows.Count - 1;
        //           // AppointmentNo = Convert.ToInt32(ds.Tables[0].Rows[p].ItemArray[0]);
        //            string AppointmentNo = ds.Tables[0].Rows[p].ItemArray[0].ToString();
        //            string NewID = getNextID(AppointmentNo);

        //            txtReceiptNo.Text = NewID;
        //        }
        //        else
        //        {
        //            String S2 = "Select ReceiptsNo from tblDefaultSettings";
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
        //        //AppointmentNo = AppointmentNo + 1;

        //    }
        //    catch { }
        //}

        private void btnCategory_Click(object sender, EventArgs e)
        {
            try
            {
                if (flg2 == 0)
                {
                    lstvCategory.BringToFront();
                    lstvCategory.Height = 175;
                    this.lstvCategory.Location = new System.Drawing.Point(403, 128);
                    lstvCategory.Visible = true;
                    flg2 = 1;
                }
                else
                {
                    lstvCategory.Visible = false;
                    flg2 = 0;
                }
            }
            catch { }
        }

        private void lstvCategory_Click(object sender, EventArgs e)
        {
            try
            {
                string item = lstvCategory.SelectedItems[0].Text;
                if (flg2 == 0)
                {
                    txtCategory.Text = item;
                }
                if (flg2 == 1)
                {
                    txtCategory.Text = item;
                }
                lstvCategory.Visible = false;
                flg2 = 0;
            }
            catch { }
        }

        private void frmAppointment_Load(object sender, EventArgs e)
        {
            try
            {
                dtUser.Clear();
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmChanneling");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    edit = Convert.ToBoolean(dtUser.Rows[0].ItemArray[4].ToString());
                    delete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());
                }
                FillGridConsultant();
                fillCurrency();
            }
            catch { }
        }
        public void FillGridConsultant()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode, ConsultantName,ConsultantType from tblConsultantMaster where Block = 'False'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                dgvConsultantRoom.DataSource = ds1.Tables[0];
                dgvConsultantRoom.Columns[0].Visible = false;
                dgvConsultantRoom.Columns[1].HeaderText = "Consultant Name";
                dgvConsultantRoom.Columns[1].Width = 123;
                dgvConsultantRoom.Columns[2].HeaderText = "Consultant Type";
                dgvConsultantRoom.Columns[2].Width = 123;
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvConsultantRoom.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvConsultantRoom.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                        dgvConsultantRoom.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();

                    }
                }
            }
            catch { }
        }



        public void fillCurrency()

        {
            cmbCurrency.Items.Clear();

            String S = "Select * from tblCurrency";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            String S1 = "Select BaseCurrency from tblDefaultSettings";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataSet dt1 = new DataSet();
            da1.Fill(dt1);

            // dataGridViewPatientHistory.DataSource = dt.Tables[0];
            if (dt.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {

                    cmbCurrency.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
                if (dt1.Tables[0].Rows.Count > 0)
                {
                    string currency = dt1.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                    cmbCurrency.SelectedItem = currency;
                }
            }




        }
        private void dgvConsultantRoom_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //txtConsultant.Text = "";
                // txtCategory.Text = "";
                flg4 = 1;
                txtConsultant.Text = dgvConsultantRoom[1, dgvConsultantRoom.CurrentRow.Index].Value.ToString().Trim();
                if (txtCategory.Text.ToString().Trim() != "")
                {
                    String S2 = "Select ConsultantCharges from tblConsultantMaster where ConsultantCode = '" + dgvConsultantRoom.Rows[dgvConsultantRoom.CurrentRow.Index].Cells[0].Value + "' AND Block = 'False'";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        txtConsultantCharge.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString();
                    }
                }
                if (txtCategory.Text.ToString().Trim() == "")
                {
                    String S1 = "Select ConsultantType, ConsultantCharges from tblConsultantMaster where ConsultantCode = '" + dgvConsultantRoom.Rows[dgvConsultantRoom.CurrentRow.Index].Cells[0].Value + "' AND Block = 'False'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        txtCategory.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString();
                        txtConsultantCharge.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                        txtConsultant.Text = dgvConsultantRoom[1, dgvConsultantRoom.CurrentRow.Index].Value.ToString().Trim();
                    }
                }
                if (flg4 == 1)
                {
                    String S1 = "Select ConsultantType, ConsultantCharges from tblConsultantMaster where ConsultantCode = '" + dgvConsultantRoom.Rows[dgvConsultantRoom.CurrentRow.Index].Cells[0].Value + "' AND Block = 'False'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        txtCategory.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString();
                        txtConsultantCharge.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                        txtConsultant.Text = dgvConsultantRoom[1, dgvConsultantRoom.CurrentRow.Index].Value.ToString().Trim();
                    }
                }
            }
            catch { }
        }

        public void ClearText()
        {
            try
            {
                txtTokenNo.Text = "";
                cmbSession.Text = "";
                dtpDateFrom.Text = "";
                txtCategory.Text = "";
                txtPatientNo.Text = "";
                txtConsultant.Text = "";
                txtFirstName.Text = "";
                txtContactNo.Text = "";
                txtRemarks.Text = "";
                txtConsultantCharge.Text = "0";
                txtHospitalCharge.Text = "0";
                txtTotal.Text = "0";
                txtRoom.Text = "";
                cmbPaymentMethod.Text = "";
                txtRate.Text = "1";
                txtCHosCharge.Text = "0";
                txtCConsCharge.Text = "0";
                txtCTotal.Text = "0";
                txtDisAmount.Text = "0";
                txtdisRate.Text = "0";
              
            }
            catch { }
        }

        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //txtConsultant.Text = "";
                ClearText();
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode, ConsultantName from tblConsultantMaster where Category = '" + txtCategory.Text.ToString().Trim() + "' AND Block = 'False'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                dgvConsultantRoom.DataSource = ds1.Tables[0];
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvConsultantRoom.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvConsultantRoom.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    }
                }
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
                    flg = 1;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        public void DisplayCHList()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "insert into tblChannelingList(ConsultantName, ConsultantType, CDate, Session, TokenNo, PatientNo, PName, Status) values ('" + txtConsultant.Text.ToString().Trim() + "', '" + txtCategory.Text.ToString().Trim() + "', '" + dtpDateFrom.Text.ToString().Trim() + "', '" + cmbSession.Text.ToString().Trim() + "', '" + txtTokenNo.Text.ToString().Trim() + "', '" + txtPatientNo.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + "Pending" + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);
            }
            catch { }
        }
        //public string IsPost = "";

        //=====================================================================================


        //=====================================================================================


        private void btnSave_Click(object sender, EventArgs e)
        {
            //=====================================
            DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.YesNo);

            if (reply == DialogResult.Yes)
            {
                try
                {
                    //===================================
                    string type = "Channel";
                    string ConnString = ConnectionString;
                    // string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
                    // string sql = "Select ReceiptsNo from tblChannelingDetails  ORDER BY ReceiptsNo";
                    string sql = "Select ReceiptsNo from tblChannelingDetails where Type='" + type + "' ORDER BY ReceiptsNo";
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


                try
                {

                    String S3 = "Select DISTINCT(ReceiptsNo)from tblChannelingDetails  where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDateFrom.Text.ToString().Trim() + "')";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                    DataTable dt = new DataTable();
                    da3.Fill(dt);
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
                //=========================================

                double Duration = 0.0;
                //string   abc = System.DateTime.Now.ToShortTimeString();

                // DateTime.Now.ToShortTimeString();


                //  TimeInterval = Convert.ToDateTime().Second;

                //=================================================================
                try
                {
                    DateTime GetSTime = Convert.ToDateTime(dtpDateFrom.Text.ToString().Trim());

                    double Token = 0.0;
                    Token = Convert.ToDouble(txtTokenNo.Text);

                    DateTime GetInterval = System.DateTime.Now;
                    DateTime newTime = System.DateTime.Now;
                    DateTime DComeTime = System.DateTime.Now;

                    string dd = Convert.ToString(GetInterval.Day);
                    string mm = Convert.ToString(GetInterval.Month);
                    string yyyy = Convert.ToString(GetInterval.Year);


                    string T1 = "";
                    string T2 = "";
                    string T3 = "";




                    string GetDay = GetSTime.DayOfWeek.ToString().Trim();
                    //10/23/2009 12:00:00 AM

                    //  string TimeForCahnel = "";


                    String S3 = "Select SessionTime,MaxBooking from tblSchedulingDetails  where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Aday = '" + GetDay + "')";
                    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                    DataTable dt = new DataTable();
                    da3.Fill(dt);

                    if (dt.Rows[0].ItemArray[0].ToString().Trim() == "")
                    {

                    }
                    else
                    {
                        //10/23/2009 12:00:00 AM

                        // GetInterval

                        // "04:30 PM"

                        if (Token == 2)
                        {
                            Duration = 1.0;
                        }
                        else
                        {
                            Duration = Convert.ToDouble(dt.Rows[0].ItemArray[1].ToString()) * Token;
                        }

                        cmbSession.Text = dt.Rows[0].ItemArray[0].ToString();

                        T1 = dt.Rows[0].ItemArray[0].ToString();
                        T2 = T1.Substring(0, 5);
                        T3 = T1.Substring(6, 2);


                        // newTime ="10/23/2009 12:00:00 AM

                        newTime = Convert.ToDateTime(mm + "/" + dd + "/" + yyyy + " " + T2 + " " + T3);
                        DComeTime = newTime.AddMinutes(Duration);
                        txtChannelingTime.Text = DComeTime.ToShortTimeString();
                        // abc = Convert.ToShortTimeString(dt.Rows[0].ItemArray[0].ToString());


                    }


                }
                catch { }
                //======================================================================

                string IsPBooking = "No";
                if (cmbPaymentMethod.Text == "Phone Booking")
                {
                    IsPBooking = "Yes";
                }

                //new one
                // IsPost = "Yes";
                string ReferenceNo = "Cash-" + txtReceiptNo.Text.ToString().Trim();
                try
                {
                    //(txtTokenNo.Text.ToString().Trim() == "") || sanjeewa removed this part to avoid compulsury token no
                    //(txtPatientNo.Text.ToString().Trim() == "") 
                    if ((txtPatientNo.Text.ToString().Trim() == "") || (txtTokenNo.Text.ToString().Trim() == "") || (txtFirstName.Text.ToString().Trim() == "") || (cmbSession.Text.ToString().Trim() == "") || (txtConsultant.Text.ToString().Trim() == "") || (cmbPaymentMethod.Text.ToString().Trim() == "") || (txtPatientNo.Text.ToString().Trim() == ""))
                    {
                        if (txtFirstName.Text == "")
                        {
                            MessageBox.Show("You must Enter Patient Name");
                            return;
                        }
                        else if (cmbSession.Text == "")
                        {
                            MessageBox.Show("You must Shedule this Doctor to get time");
                            return;
                        }
                        else if (cmbPaymentMethod.Text == "")
                        {
                            MessageBox.Show("You must Select a Payment Method");
                            return;
                        }
                        else if (txtPatientNo.Text == "")
                        {
                            MessageBox.Show("You must Enter a Pation Number");
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Please fill the empty fields");
                            return;
                        }
                    }
                    else
                    {

                        string CU = user.userName;
                        string Export = "False";
                        string type = "Channel";

                        //try
                        //{
                        //    //===================================
                        //    // string type = "Channel";
                        //    string ConnString = ConnectionString;
                        //    // string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
                        //    // string sql = "Select ReceiptsNo from tblChannelingDetails  ORDER BY ReceiptsNo";
                        //    string sql = "Select ReceiptsNo from tblChannelingDetails where Type='" + type + "' ORDER BY ReceiptsNo";
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

                        //========================================


                        // string Dateset = dtpDateFrom.Text.ToString();
                        //string CDay= Dateset.Substring(0, 2);
                        //string CMonth = Dateset.Substring(3, 2);
                        //string CYear=Dateset.Substring(6,4);
                        //string newDate = CMonth + "/" + CDay + "/" + CYear;//this is anew date that will update SQLQuary
                        //"29/03/2009"
                        try
                        {
                            //exporetSalesInvoice();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }

                        if (flg == 0)
                        {
                            //DisplayCHList();

                            string ConnString1 = ConnectionString;//txtChannelingTime.Text

                            String S1 = "insert into tblChannelingDetails(ReceiptsNo, TokenNo, PatientNo, ConsultantName, " +
                            " FirstName, ConsultantType, Date, Session, Room, ContactNO, HospitalFee, DoctorCharge, Total, " +
                            " PaymentMethod, Remarks, CreditCardNo, CHosCharge, CConCharge, CTotal, Rate,CurrentUser,RepDate," +
                            "IsExport,DiscountAmt,TotalAmt,DisRate,IsPBooking,disRateD,DocDiscount,HosDiscount,Type) values ('" +
                            txtReceiptNo.Text.ToString().Trim() +
                                "', '" + txtTokenNo.Text.ToString().Trim() + "', '" + txtPatientNo.Text.ToString().Trim() +
                                "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() +
                                "', '" + txtCategory.Text.ToString().Trim() + "', '" + dtpDateFrom.Text.ToString() + "', '" +
                                txtChannelingTime.Text.ToString().Trim() + "', '" + txtRoom.Text.ToString().Trim() + "', '" +
                                txtContactNo.Text.ToString().Trim() + "', '" + txtHospitalCharge.Text.ToString().Trim() +
                                "', '" + txtConsultantCharge.Text.ToString().Trim() + "', '" +
                                txtTotal.Text.ToString().Trim() + "', '" + cmbPaymentMethod.Text.ToString().Trim() + "', '" +
                                txtRemarks.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" +
                                txtCHosCharge.Text.ToString().Trim() + "', '" + txtCConsCharge.Text.ToString().Trim() + "', '" +
                                txtCTotal.Text.ToString().Trim() + "', '" + txtRate.Text.ToString().Trim() + "','" + CU + "','" +
                                dtpReceiptDate.Text.ToString().Trim() + "','" + Export + "','" +
                                txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" +
                                txtdisRate.Text.ToString().Trim() + "','" + IsPBooking + "','" +
                                txtDocDisRate.Text.ToString().Trim() + "','" + txtdocdiscount.Text.ToString().Trim() + "','" +
                                txthosDiscount.Text.ToString().Trim() + "','" + type + "')";//CU
                                                                                            // String S1 = "insert into tblChannelingDetails(ReceiptsNo, TokenNo, PatientNo, ConsultantName, FirstName, ConsultantType, Date, Session, Room, ContactNO, HospitalFee, DoctorCharge, Total, PaymentMethod, Remarks, CreditCardNo, CHosCharge, CConCharge, CTotal, Rate,CurrentUser,RepDate,IsExport,DiscountAmt,TotalAmt,DisRate,IsPBooking,disRateD,DocDiscount,HosDiscount,Type) values ('" + txtReceiptNo.Text.ToString().Trim() + "', '" + txtTokenNo.Text.ToString().Trim() + "', '" + txtPatientNo.Text.ToString().Trim() + "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtCategory.Text.ToString().Trim() + "', '" + dtpDateFrom.Text.ToString() + "', '" + cmbSession.Text.ToString().Trim() + "', '" + txtRoom.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtHospitalCharge.Text.ToString().Trim() + "', '" + txtConsultantCharge.Text.ToString().Trim() + "', '" + txtTotal.Text.ToString().Trim() + "', '" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + txtCHosCharge.Text.ToString().Trim() + "', '" + txtCConsCharge.Text.ToString().Trim() + "', '" + txtCTotal.Text.ToString().Trim() + "', '" + txtRate.Text.ToString().Trim() + "','" + CU + "','" + dtpReceiptDate.Text.ToString().Trim() + "','" + Export + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" + txtdisRate.Text.ToString().Trim() + "','" + IsPBooking + "','" + txtDocDisRate.Text.ToString().Trim() + "','" + txtdocdiscount.Text.ToString().Trim() + "','" + txthosDiscount.Text.ToString().Trim() + "','" + type + "')";//CU
                            SqlCommand cmd1 = new SqlCommand(S1);
                            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                            DataSet ds1 = new DataSet();
                            da1.Fill(ds1);
                            string aa = "True";
                            //....................................
                            String s = "update tblChannelingRooms set TokenNO ='" + txtTokenNo.Text.ToString().Trim() + "' where ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "'AND ConsultantType = '" + txtCategory.Text.ToString().Trim() + "' AND SessionType = '" + aa + "' AND SessionTime = '" + cmbSession.Text.ToString().Trim() + "' AND Date ='" + dtpDateFrom.Text.ToString() + "'";
                            //(,,,,,,)
                            SqlCommand cmd2 = new SqlCommand(s);
                            SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                            DataSet ds2 = new DataSet();
                            da.Fill(ds1);
                            //..............................................

                            if (flgNew == 1)
                            {
                                string ConnString2 = ConnectionString;
                                String S2 = "insert into tblPatientsDetails(PatientNo, FirstName, ContactNo) values ('" + txtPatientNo.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "')";
                                SqlCommand cmd3 = new SqlCommand(S2);
                                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString2);
                                DataSet ds3 = new DataSet();
                                da2.Fill(ds2);
                            }

                            //This is peachtree upload part

                            //Connector abc = new Connector();
                            //abc.Import_Receipt_Journal();
                            // MessageBox.Show("Saved Successfully");


                            if (cmbPaymentMethod.Text != "Phone Booking")
                            {
                                //btnPrint_Click(sender, e);
                                //exporetSalesInvoice();
                                //MessageBox.Show("Saved Successfully");

                                MessageBox.Show("Channeling is  Successfull Your Reference No ='" + txtReceiptNo.Text.ToString().Trim() + "'" + " Appintment NO is='" + txtTokenNo.Text.ToString().Trim() + "'");

                                btnPrint_Click(sender, e);


                                //if (rbtnbht.Checked == true)
                                //{
                                //    exporetSalesInvoice();
                                //    btnPrint_Click(sender, e);

                                //}
                                //else
                                //{
                                //    btnPrint_Click(sender, e);

                                //}
                            }
                            else
                            {
                                //MessageBox.Show("Phone Booking is  Successfull Your Reference No ='" + txtReceiptNo.Text.ToString().Trim() + "'");
                                MessageBox.Show("Your Reference No ='" + txtReceiptNo.Text.ToString().Trim() + "'" + " Appintment NO is='" + txtTokenNo.Text.ToString().Trim() + "'");
                            }
                            // btnPrint_Click(sender, e);
                            //if (rbtnbht.Checked == true)
                            //{
                            //    exporetSalesInvoice();

                            //}

                        }
                        else
                        {
                            // String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() +"', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='"+dtpReceiptDate.Text.ToString().Trim()+"' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                            String S = "Update tblChannelingDetails SET PaymentMethod = 'Cash',RepDate = '" + dtpReceiptDate.Text.ToString().Trim() + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                            SqlCommand cmd1 = new SqlCommand(S);
                            SqlConnection con = new SqlConnection(ConnectionString);
                            SqlDataAdapter da = new SqlDataAdapter(S, con);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            MessageBox.Show("Updated Successfully");

                            flg = 0;

                        }
                        // btnPrint_Click(sender, e);

                        ClearText();
                        txtReceiptNo.Text = "";
                        DisableObjects();
                        FillGridConsultant();
                    }
                    btnNew.Enabled = true;
                    // btnEdit.Enabled = true;
                    flgNew = 0;
                }
                catch
                {
                    MessageBox.Show("Another user is Associating with this transaction");
                    btnNew_Click(sender, e);
                }
            }
            else
            {

                return;
            }
        }

        //=================================================
        public void exporetSalesInvoice()
        {
            //Create a Xmal File..................................................................................


            //Receipts2.xml
            XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\Channeling.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            string NoDistributions = "2";
            //  double discountRate = Convert.ToDouble(txtdisRate.Text) / 100;

            //  for (int i = 0; i <= 2; i++)
            // {
            Writer.WriteStartElement("PAW_Invoice");
            Writer.WriteAttributeString("xsi:type", "paw:Receipt");

            if (rbtnbht.Checked)
            {
                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                //C0001
                //Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                Writer.WriteString(txtPatientNo.Text.ToString().Trim());//Customer ID should be here = Ptient No
                Writer.WriteEndElement();
            }
            else
            {

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                //C0001
                Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                                            //Writer.WriteString(txtPatientNo.Text.ToString().Trim());//Customer ID should be here = Ptient No
                Writer.WriteEndElement();
            }

            Writer.WriteStartElement("Date");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(dtpReceiptDate.Value.ToString("MM/dd/yyyy"));//Date 
            Writer.WriteEndElement();

            Writer.WriteStartElement("Invoice_Number");
            Writer.WriteString(txtReceiptNo.Text.ToString().Trim());
            Writer.WriteEndElement();





            //Writer.WriteStartElement("Payment_Method");
            //Writer.WriteString(cmbPaymentMethod.Text.ToString().Trim());//PayMethod
            //Writer.WriteEndElement();

            Writer.WriteStartElement("Accounts_Receivable_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString("65000");//Cash Account
            Writer.WriteEndElement();//CreditMemoType

            Writer.WriteStartElement("CreditMemoType");
            Writer.WriteString("FALSE");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString(NoDistributions);
            Writer.WriteEndElement();


            //Writer.WriteStartElement("InvoicePaid");
            //Writer.WriteString("");//PayMethod
            //Writer.WriteEndElement();//SalesLines

            Writer.WriteStartElement("SalesLines");

            Writer.WriteStartElement("SalesLine");

            Writer.WriteStartElement("Quantity");
            Writer.WriteString("1");//Doctor Charge
            Writer.WriteEndElement();


            Writer.WriteStartElement("Item_ID");
            Writer.WriteString("D0001");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Description");
            Writer.WriteString("Consultant Chargers");
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString("85500");
            Writer.WriteEndElement();


            //========================================================
            Writer.WriteStartElement("Tax_Type");
            Writer.WriteString("1");//Doctor Charge
            Writer.WriteEndElement();

            //double Amount = Convert.ToDouble(dgvScanItems[5, i].Value);
            //double DiscountAmount = Amount * discountRate;
            //double ItemAmount = Amount - DiscountAmount;

            Writer.WriteStartElement("Amount");
            Writer.WriteString("-" + txtConsultantCharge.Text.ToString().Trim());//HospitalCharge
                                                                                 // Writer.WriteString("0");//HospitalCharge
            Writer.WriteEndElement();

            Writer.WriteEndElement();


            //===================================================
            Writer.WriteStartElement("SalesLine");

            Writer.WriteStartElement("Quantity");
            Writer.WriteString("1");//Doctor Charge
            Writer.WriteEndElement();


            Writer.WriteStartElement("Item_ID");
            Writer.WriteString("H0001");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Description");
            Writer.WriteString("Hospital Charges");
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString("10000");
            Writer.WriteEndElement();


            //========================================================
            Writer.WriteStartElement("Tax_Type");
            Writer.WriteString("1");//Doctor Charge
            Writer.WriteEndElement();

            //double Amount = Convert.ToDouble(dgvScanItems[5, i].Value);
            //double DiscountAmount = Amount * discountRate;
            //double ItemAmount = Amount - DiscountAmount;

            Writer.WriteStartElement("Amount");
            Writer.WriteString("-" + txtHospitalCharge.Text.ToString());//HospitalCharge
            Writer.WriteEndElement();

            //Writer.WriteStartElement("Transaction_Number");
            //Writer.WriteString((i + 1).ToString().Trim());
            //Writer.WriteEndElement();

            //Writer.WriteStartElement("ReceiptNumber");
            //Writer.WriteString(txtReceiptNo.Text.ToString().Trim());
            //Writer.WriteEndElement();
            Writer.WriteEndElement();//LINE
            Writer.WriteEndElement();//LINES

            Writer.WriteEndElement();
            // Writer.Close();
            //  }

            Writer.Close();

            Connector abc = new Connector();//export to peach tree
            abc.ImportChannelingData();//ImportSalesInvice()



        }

        //===========================================================

        private void dtpDateFrom_ValueChanged(object sender, EventArgs e)
        {
            if (AB != 2)
            {
                try
                {
                    cmbSession.Items.Clear();

                }
                catch { }
                //===================================================
                try
                {
                    //{
                    //   // string sql = "Select ReceiptsNo from tblChannelingDetails where Type='" + type + "' ORDER BY ReceiptsNo";
                    //    String S3 = "Select DISTINCT(ReceiptsNo)from tblChannelingDetails  where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDateFrom.Text.ToString().Trim() + "')";
                    //    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    //    SqlCommand cmd3 = new SqlCommand(S3);
                    //    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                    //    DataTable dt = new DataTable();
                    //    da3.Fill(dt);
                    //    int NoOfTokens = 1;
                    //    if (dt.Rows.Count > 0)
                    //    {

                    //            NoOfTokens = dt.Rows.Count + 2;

                    //    }
                    //    else
                    //    {
                    //        NoOfTokens = 2;
                    //    }
                    //    txtTokenNo.Text = NoOfTokens.ToString();
                    //......................................

                    //=================================================
                    //      }
                    //catch { }




                    DateTime GetSTime = Convert.ToDateTime(dtpDateFrom.Text.ToString().Trim());
                    string GetDay = GetSTime.DayOfWeek.ToString().Trim();
                    //  string TimeForCahnel = "";


                    String S4 = "Select SessionTime from tblSchedulingDetails  where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Aday = '" + GetDay + "')";
                    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da4.Fill(dt1);

                    if (dt1.Rows[0].ItemArray[0].ToString().Trim() == "")
                    {
                        // MessageBox.Show("Please Schedule this Consultant");                
                    }
                    else
                    {
                        cmbSession.Text = dt1.Rows[0].ItemArray[0].ToString();
                    }


                }
                catch { }
            }
        }

        private void btnConsultant_Click(object sender, EventArgs e)
        {
            try
            {
                //Class1.formto = "Form2";
                //Class1.textto = "textBox1.Text";

                frmSelectConsult frm = new frmSelectConsult();
                frm.Show();
            }
            catch { }
        }



        private void txtConsultant_TextChanged(object sender, EventArgs e)
        {
            if (AB != 2)
            {
                try
                {

                    txtTokenNo.Text = "";
                    txtRoom.Text = "";
                    cmbSession.Text = "";
                    cmbSession.Items.Clear();
                    txtCategory.Text = "";
                    if (txtCategory.Text.ToString().Trim() != "")
                    {
                        String S2 = "Select ConsultantCharges,HspitalCharges from tblConsultantMaster where ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "' AND Block = 'False'";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataSet ds2 = new DataSet();
                        da2.Fill(ds2);
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            txtConsultantCharge.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString();
                            txtCConsCharge.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                            txtHospitalCharge.Text = ds2.Tables[0].Rows[0].ItemArray[1].ToString().Trim();


                            txtdisRate_TextChanged(sender, e);
                        }


                    }
                    if (txtCategory.Text.ToString().Trim() == "")
                    {
                        String S1 = "Select ConsultantType, ConsultantCharges,HspitalCharges from tblConsultantMaster where ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "' AND Block = 'False'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataSet ds1 = new DataSet();
                        da1.Fill(ds1);
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            txtCategory.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString();
                            txtConsultantCharge.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                            txtCConsCharge.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                            txtConsultant.Text = txtConsultant.Text.ToString().Trim();
                            txtHospitalCharge.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString().Trim();

                            txtTempDocFee.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                            txtTempHosFee.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString().Trim();

                            //txtdisRate_TextChanged(sender, e);
                        }


                    }
                }
                catch { }

                //======================================================


                //try
                //{

                //    String S3 = "Select DISTINCT(ReceiptsNo)from tblChannelingDetails  where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDateFrom.Text.ToString().Trim() + "')";
                //    SqlCommand cmd3 = new SqlCommand(S3);
                //    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                //    DataTable dt = new DataTable();
                //    da3.Fill(dt);
                //    int NoOfTokens = 1;
                //    if (dt.Rows.Count > 0)
                //    {
                //        NoOfTokens = dt.Rows.Count + 2;
                //    }
                //    else
                //    {
                //        NoOfTokens = 2;
                //    }
                //    txtTokenNo.Text = NoOfTokens.ToString();

                //}
                //catch { }

                //===================================================



                try
                {
                    DateTime GetSTime = Convert.ToDateTime(dtpDateFrom.Text.ToString().Trim());
                    string GetDay = GetSTime.DayOfWeek.ToString().Trim();
                    //  string TimeForCahnel = "";


                    String S3 = "Select SessionTime from tblSchedulingDetails  where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Aday = '" + GetDay + "')";
                    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                    DataTable dt = new DataTable();
                    da3.Fill(dt);

                    if (dt.Rows[0].ItemArray[0].ToString().Trim() == "")
                    {

                    }
                    else
                    {
                        cmbSession.Text = dt.Rows[0].ItemArray[0].ToString();
                    }


                }
                catch { }
            }
            //==============================================================
        }

        //====================================================
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
        //======================================================

        private void btnNewNo_Click(object sender, EventArgs e)
        {
            try
            {
                flgNew = 1;
                String S1 = "Select PatientNo from tblPatientsDetails ORDER BY PatientNo";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //int PatientNo = Convert.ToInt32(dt.Rows[dt.Rows.Count-1].ItemArray[0].ToString().Trim()) + 1;
                    //txtPatientNo.Text = PatientNo.ToString().PadLeft(8,'0');
                    string PatientNo = dt.Rows[dt.Rows.Count - 1].ItemArray[0].ToString().Trim();
                    string NewID = getNextID(PatientNo);
                    txtPatientNo.Text = NewID;
                }
                else
                {
                    //int PatientNo =1;
                    //txtPatientNo.Text = PatientNo.ToString().PadLeft(8, '0');
                    String S2 = "Select PatientNo from tblDefaultSettings";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da2.Fill(dt1);

                    if (dt1.Rows.Count > 0)
                    {
                        string P1 = dt1.Rows[dt1.Rows.Count - 1].ItemArray[0].ToString().Trim();
                        string NewID = getNextID(P1);
                        txtPatientNo.Text = NewID;
                    }
                }
            }
            catch { }
        }


        //Get the Next ID
        //public string getNextID(string s)
        //{
        //    int i = 0;
        //    string nextID = "";
        //    while (i < s.Length - 1)
        //    {
        //        if ((Char.IsDigit(s[i]) && Char.IsLetter(s[i + 1])) || (Char.IsLetter(s[i]) && Char.IsDigit(s[i + 1]) || ((s[i] == '-')) || ((s[i] == ' '))))
        //        {
        //            s = s.Insert(i + 1, "*");
        //        }
        //        i++;
        //    }
        //    bool Islarge = false;
        //    string[] arr = s.Split('*');
        //    i = arr.Length - 1;
        //    for (int no = i; no >= 0; no--)
        //    {
        //        if (arr[i].Length > 19)
        //        {
        //            Islarge = true;
        //        }
        //        else
        //        {
        //            Islarge = false;
        //        }
        //    }
        //    if (Islarge == false)
        //    {
        //        ///'''''''''''''''''''''''''''''''''
        //        while (i >= 0)
        //        {
        //            try
        //            {
        //                //if (arr[i].Length<=19)
        //                //{
        //                long no = long.Parse(arr[i]);
        //                i = 0;
        //                while (i < arr.Length)
        //                {
        //                    if (arr[i] == no.ToString())
        //                    {
        //                        no++;
        //                        arr[i] = no.ToString();
        //                    }
        //                    nextID = nextID + arr[i];
        //                    i++;
        //                }
        //                return nextID;

        //            }
        //            catch { }


        //            if (i != 0)
        //            {
        //                i--;
        //            }
        //        }


        //        return s + "1";
        //    }
        //    else
        //    {

        //        return null;
        //    }

        //}


        //.............

        private void txtPatientNo_Leave(object sender, EventArgs e)
        {

            // string abc = txtPatientNo.Text.ToUpper;
            try
            {
                txtPatientNo.Text = txtPatientNo.Text.ToUpper();
            }
            catch { }

            //try
            //{
            //    if (flgNew != 1)
            //    {
            //        String S1 = "Select FirstName, ContactNo, Title from tblPatientsDetails where PatientNo = '" + txtPatientNo.Text.ToString().Trim() + "'";
            //        SqlCommand cmd1 = new SqlCommand(S1);
            //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //        DataTable dt = new DataTable();
            //        da1.Fill(dt);
            //        if (dt.Rows.Count > 0)
            //        {
            //            txtFirstName.Text = dt.Rows[0].ItemArray[0].ToString();
            //            txtContactNo.Text = dt.Rows[0].ItemArray[3].ToString();
            //        }
            //    }
            //}
            //catch { }

            // if (cmbSearchReceipt.Text == "Receipts Number")
            // {

            // string Code = txtFirstName.Text.ToString().Trim();

            //try
            //{
            //    //string cusType = "";
            //    string add = txtPatientNo.Text;
            //    if (add != "")
            //    {

            //        String S2 = "Select * from tblPatientsDetails where PatientNo LIKE  '" + add + "%'";
            //        SqlCommand cmd2 = new SqlCommand(S2);
            //        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            //        DataTable dt2 = new DataTable();
            //        da2.Fill(dt2);

            //        if (dt2.Rows.Count > 0)
            //        {

            //            txtFirstName.Text = dt2.Rows[0].ItemArray[2].ToString();
            //            txtContactNo.Text = dt2.Rows[0].ItemArray[17].ToString();

            //            // dt2.Rows[0].ItemArray[28].ToString();
            //            if (dt2.Rows[0].ItemArray[28].ToString() == "Member")
            //            {
            //                lblCustomerType.Visible = true;
            //                txtCType.Text = dt2.Rows[0].ItemArray[28].ToString();
            //                txtCType.Visible = true;

            //            }
            //            else
            //            {
            //                // lblCustomerType.Visible = false ;
            //                //txtCType.Visible  = false;
            //            }

            //            if (dt2.Rows[0].ItemArray[28].ToString() == "Deposit Holder")
            //            {
            //                lblCustomerType.Visible = true;
            //                txtCType.Text = dt2.Rows[0].ItemArray[28].ToString();
            //                txtCType.Visible = true;

            //            }
            //            else
            //            {
            //                //lblCustomerType.Visible = false;
            //                // txtCType.Visible = false; 
            //            }





            //            //if (dt2.Rows[0].ItemArray[35].ToString() == "Yes")
            //            //{
            //            //    cbxPhoneBooking.Checked = true;
            //            //    if (dt2.Rows[0].ItemArray[16].ToString() == "Phone Booking")
            //            //    {
            //            //        // btnp.Enabled = true;
            //            //        btnPay.Enabled = true;
            //            //        cbxnotpaid.Visible = true;
            //            //        cbxnotpaid.Checked = true;
            //            //    }

            //            //}
            //            //else
            //            //{
            //            //    cbxPhoneBooking.Checked = false;
            //            //    cbxnotpaid.Visible = false;
            //            //}
            //        }
            //        else
            //        {
            //            txtFirstName.Text = "";
            //            txtContactNo.Text = "";
            //            lblCustomerType.Visible = false;
            //            txtCType.Visible = false;
            //        }
            //    }
            //}
            //catch { }
            ////  }
            //// else
            //// {
            ////MessageBox.Show("Receipt not found");
            ////}
            ////  }
        }

        private void cmbSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckSearch != 2)
            {
                try
                {
                    txtRoom.Text = "";
                    txtTokenNo.Text = "";

                    //Auto fill room no
                    // String S3 = "Select CRoomNo from tblChannelingRooms where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (SessionTime = '" + cmbSession.Text.ToString().Trim() + "')";
                    String S3 = "Select CRoomNo from tblChannelingRooms where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (SessionTime = '" + cmbSession.Text.ToString().Trim() + "') AND(Date='" + dtpDateFrom.Text.ToString().Trim() + "')";
                    SqlCommand cmd3 = new SqlCommand(S3);//Date
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                    DataSet ds3 = new DataSet();
                    da3.Fill(ds3);
                    if (ds3.Tables[0].Rows.Count > 0)
                    {
                        txtRoom.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString();
                    }

                    DateTime a = dtpDateFrom.Value;
                    //int amonth = a.Month;
                    //int aYear = a.Year;
                    //int aDay = a.Day;

                    // //01/04/2009 12:00:00 AM

                    // string NewDate=

                    //Auto fill token no, time
                    // String S1 = "Select COUNT(TokenNo) from tblChannelingDetails where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Session = '" + cmbSession.Text.ToString().Trim() + "') AND (Date = '" + dtpDateFrom.Text.ToString().Trim() + "')";
                    //String S1 = "Select COUNT(TokenNo) from tblChannelingDetails where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Session = '" + cmbSession.Text.ToString().Trim() + "') AND (Date = '" + dtpDateFrom.Text.ToString().Trim() + "')";
                    String S1 = "Select TokenNo from tblChannelingRooms where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (SessionTime = '" + cmbSession.Text.ToString().Trim() + "')AND (CRoomNo = '" + txtRoom.Text + "') AND (Date = '" + dtpDateFrom.Text.ToString().Trim() + "')";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);
                    int NoOfTokens = 0;
                    if (dt.Rows.Count >= 0)
                    {
                        if (dt.Rows[0].ItemArray[0].ToString() == "0")
                        {
                            NoOfTokens = 1;
                        }
                        else
                        {
                            //NoOfTokens = Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString()) + 1;
                            NoOfTokens = Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString());
                        }
                    }
                    else
                    {
                        NoOfTokens = 1;
                    }
                    txtTokenNo.Text = NoOfTokens.ToString();

                    //if (NoOfTokens == 1)
                    //{
                    //    if ((Convert.ToInt32(cmbSession.Text.Substring(0, 1)) == 00) || ((Convert.ToInt32(cmbSession.Text.Substring(0, 1)) >= 01) && (Convert.ToInt32(cmbSession.Text.Substring(0, 1)) < 12)))
                    //    {
                    //        txtTime.Text = cmbSession.Text.Substring(0, 5) + " AM";
                    //    }
                    //    else
                    //    {
                    //        txtTime.Text = cmbSession.Text.Substring(0, 5) + " PM";
                    //    }
                    //    DateTime dtt = Convert.ToDateTime("00/00/2009 " + txtTime.Text.ToString() + ":00 PM");
                    //  //  MessageBox.Show(dtt.ToString());
                    //}
                    //else
                    //{
                    //    String S2 = "Select Interval from tblConsultantMaster where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "')";
                    //    SqlCommand cmd2 = new SqlCommand(S2);
                    //    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    //    DataTable dt2 = new DataTable();
                    //    da2.Fill(dt2);

                    //    //if (dt2.Rows.Count > 0)
                    //    //{
                    //    //    //  MessageBox.Show(dtpDateFrom.Text);
                    //    //    txtTime.Text = dtpDateFrom.Value.ToString();
                    //    //    DateTime dtt = dtpDateFrom.Value;
                    //    //    if ((Convert.ToInt32(cmbSession.Text.Substring(0, 1)) == 00) || ((Convert.ToInt32(cmbSession.Text.Substring(0, 1)) >= 01) && (Convert.ToInt32(cmbSession.Text.Substring(0, 1)) < 12)))
                    //    //    {
                    //    //         dtt = Convert.ToDateTime("01/01/2009 " + cmbSession.Text.Substring(0, 5) + ":00 AM");
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //         dtt = Convert.ToDateTime("01/01/2009 " + cmbSession.Text.Substring(0, 5) + ":00 PM");

                    //    //    }
                    //    //    double interval = Convert.ToDouble(dt2.Rows[0].ItemArray[0].ToString().Trim());
                    //    //    double tokens = Convert.ToDouble(txtTokenNo.Text.ToString().Trim()) - 1;
                    //    //   // double totalint = interval * Convert.ToDouble(txtTokenNo.Text.ToString().Trim());
                    //    //    double totalint = interval * tokens;

                    //    //    txtTime.Text = dtt.AddMinutes(totalint).ToShortTimeString().ToString();
                    //    //    //string ADay = dtt.DayOfWeek.ToString();
                    //    //    //MessageBox.Show(dt.DayOfWeek.ToString());
                    //    //    //MessageBox.Show(dtt.ToString());
                    //    //    //MessageBox.Show(dtt.AddMinutes(125).ToString());
                    //    //}
                    //}

                    DateTime dat = dtpDateFrom.Value;
                    //MessageBox.Show(dt.DayOfWeek.ToString());
                    string ADay = dat.DayOfWeek.ToString();

                    //check in default settings for minus allow

                    String S5 = "Select MinusAllow from tblDefaultSettings";
                    SqlCommand cmd5 = new SqlCommand(S5);
                    SqlDataAdapter da5 = new SqlDataAdapter(S5, ConnectionString);
                    DataTable dt5 = new DataTable();
                    da5.Fill(dt5);

                    //get maximum booking amount for the consultant
                    String S4 = "Select MaxBookings from tblSchedulingDetails where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (ADay = '" + ADay + "') AND (SessionTime = '" + cmbSession.Text.ToString().Trim() + "')";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
                    DataTable dt4 = new DataTable();
                    da4.Fill(dt4);
                    if (dt4.Rows.Count > 0)
                    {
                        if (NoOfTokens > Convert.ToInt32(dt4.Rows[0].ItemArray[0].ToString()))
                        {
                            if (dt5.Rows.Count > 0)
                            {
                                if (dt5.Rows[0].ItemArray[0].ToString() == "False")
                                {
                                    MessageBox.Show("Tokens exceed maximum bookings, please use another session");
                                    cmbSession.Text = "";
                                    txtTokenNo.Text = "";
                                }
                                else
                                {
                                    MessageBox.Show("Tokens exceed maximum bookings");
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    String S2 = "Select * from tblChannelingDetails where (ReceiptsNo = '" + txtFind.Text.ToString().Trim()+"')";  //.PadLeft(8,'0') + "')";
            //    SqlCommand cmd2 = new SqlCommand(S2);
            //    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            //    DataTable dt2 = new DataTable();
            //    da2.Fill(dt2);

            //    if (dt2.Rows.Count > 0)
            //    {

            //        txtReceiptNo.Text = dt2.Rows[0].ItemArray[0].ToString();

            //        txtPatientNo.Text = dt2.Rows[0].ItemArray[2].ToString();
            //        txtCategory.Text = dt2.Rows[0].ItemArray[7].ToString();
            //        txtConsultant.Text = dt2.Rows[0].ItemArray[3].ToString();
            //        txtTokenNo.Text = dt2.Rows[0].ItemArray[1].ToString();
            //        txtFirstName.Text = dt2.Rows[0].ItemArray[4].ToString();
            //        dtpDateFrom.Text = dt2.Rows[0].ItemArray[8].ToString();
            //        cmbSession.Text = dt2.Rows[0].ItemArray[9].ToString();
            //        txtRoom.Text = dt2.Rows[0].ItemArray[10].ToString();
            //        txtContactNo.Text = dt2.Rows[0].ItemArray[11].ToString();          
            //        txtHospitalCharge.Text = dt2.Rows[0].ItemArray[12].ToString();
            //        txtConsultantCharge.Text = dt2.Rows[0].ItemArray[13].ToString();
            //        txtTotal.Text = dt2.Rows[0].ItemArray[15].ToString();              
            //        cmbPaymentMethod.Text = dt2.Rows[0].ItemArray[16].ToString();
            //        txtRemarks.Text = dt2.Rows[0].ItemArray[17].ToString();
            //        txtCreditCardNo.Text = dt2.Rows[0].ItemArray[19].ToString();
            //        txtCHosCharge.Text = dt2.Rows[0].ItemArray[20].ToString();
            //        txtCConsCharge.Text = dt2.Rows[0].ItemArray[21].ToString();
            //        txtCTotal.Text = dt2.Rows[0].ItemArray[22].ToString();
            //        txtRate.Text = dt2.Rows[0].ItemArray[23].ToString();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Receipt not found");
            //    }
            //    txtFind.Text = "";
            //    btnNew.Enabled = true;
            //    btnEdit.Enabled = true;
            //    btnSave.Enabled = false;
            //    button1.Enabled = true;
            //    btnPrint.Enabled = true;
            //    btnDelete.Enabled = true;
            //    btnEdit.Enabled = true;
            //}
            //catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    String S = "DELETE FROM tblChannelingDetails WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
            //    SqlCommand cmd = new SqlCommand(S);
            //    SqlConnection con = new SqlConnection(ConnectionString);
            //    SqlDataAdapter da = new SqlDataAdapter(S, con);
            //    DataTable dt = new DataTable();
            //    da.Fill(dt);
            //    MessageBox.Show("Deleted Successfully");
            //    ClearText();
            //}
            //catch { }
        }

        private void txtHospitalCharge_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    txtdisRate_TextChanged(sender, e);
            //}
            //catch { }

            try
            {
                if (txtConsultantCharge.Text.ToString().Trim() != "")
                {
                    txtTotal.Text = (Convert.ToDouble(txtHospitalCharge.Text.ToString().Trim()) + Convert.ToDouble(txtConsultantCharge.Text.ToString().Trim())).ToString("N2");
                    // txtnetTotal.Text = txtTotal.Text;
                    txtnetTotal.Text = (Convert.ToDouble(txtHospitalCharge.Text.ToString().Trim()) + Convert.ToDouble(txtConsultantCharge.Text.ToString().Trim())).ToString("N2");
                }
                else
                {
                    txtTotal.Text = Convert.ToString(Convert.ToDouble(txtHospitalCharge.Text.ToString().Trim()));
                    txtnetTotal.Text = txtTotal.Text;
                }
            }
            catch { }
        }

        private void txtConsultantCharge_TextChanged(object sender, EventArgs e)
        {
            if (CheckSearch != 2)
            {
                try
                {
                    if (txtHospitalCharge.Text.ToString().Trim() != "")
                    {
                        txtTotal.Text = (Convert.ToDouble(txtHospitalCharge.Text.ToString().Trim()) + Convert.ToDouble(txtConsultantCharge.Text.ToString().Trim())).ToString("N2");
                        // txtnetTotal.Text = txtTotal.Text.Trim();
                        txtnetTotal.Text = (Convert.ToDouble(txtHospitalCharge.Text.ToString().Trim()) + Convert.ToDouble(txtConsultantCharge.Text.ToString().Trim())).ToString("N2");
                    }
                    else
                    {
                        txtTotal.Text = Convert.ToString(Convert.ToDouble(txtConsultantCharge.Text.ToString().Trim()));
                        // txtnetTotal.Text = txtTotal.Text.Trim();
                        txtnetTotal.Text = txtTotal.Text.Trim();
                    }

                }
                catch { }
            }
        }

        private void txtCategory_TextChanged_1(object sender, EventArgs e)
        {
            //try
            //{
            //    if (flg4 == 0)
            //    {
            //       // txtConsultant.Text = "";
            //        //dgvAvailability.Rows.Clear();
            //        //ClearText();
            //        string ConnString = ConnectionString;
            //        String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where ConsultantType = '" + txtCategory.Text.ToString().Trim() + "' AND Block = 'False'";
            //        SqlCommand cmd1 = new SqlCommand(S1);
            //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //        DataSet ds1 = new DataSet();
            //        da1.Fill(ds1);

            //        dgvConsultantRoom.DataSource = ds1.Tables[0];
            //        dgvConsultantRoom.Columns[0].Visible = false;
            //        dgvConsultantRoom.Columns[1].HeaderText = "Consultant Name";
            //        dgvConsultantRoom.Columns[1].Width = 123;
            //        dgvConsultantRoom.Columns[2].HeaderText = "Consultant Type";
            //        dgvConsultantRoom.Columns[2].Width = 123;

            //        for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            //        {
            //            dgvConsultantRoom.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
            //            dgvConsultantRoom.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
            //            dgvConsultantRoom.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
            //       }
            //    }
            //    if (flg4 == 1)
            //    {
            //       // dgvAvailability.Rows.Clear();
            //        //ClearText();
            //        string ConnString = ConnectionString;
            //        String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where ConsultantType = '" + txtCategory.Text.ToString().Trim() + "' AND Block = 'False'";
            //        SqlCommand cmd1 = new SqlCommand(S1);
            //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //        DataSet ds1 = new DataSet();
            //        da1.Fill(ds1);

            //        dgvConsultantRoom.DataSource = ds1.Tables[0];
            //        dgvConsultantRoom.Columns[0].Visible = false;
            //        dgvConsultantRoom.Columns[1].HeaderText = "Consultant Name";
            //        dgvConsultantRoom.Columns[1].Width = 123;
            //        dgvConsultantRoom.Columns[2].HeaderText = "Consultant Type";
            //        dgvConsultantRoom.Columns[2].Width = 123;

            //        for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            //        {
            //            dgvConsultantRoom.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
            //            dgvConsultantRoom.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
            //            dgvConsultantRoom.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
            //        }
            //    }
            //}
            //catch { }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {

                txtCategory.Clear();
                txtConsultant.Clear();
                FillGridConsultant();
            }
            catch { }
        }
        private void DirectPrint()
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\CRChannelingrpt.rpt") == true)
                {
                    Myfullpath = Path.GetFullPath("CRChannelingrpt.rpt");
                }
                else
                {
                    MessageBox.Show("CRChannelingrpt.rpt not Exists");
                    this.Close();
                    return;
                }
                crp.Load(Myfullpath);
                crp.SetDataSource(dsAll);
                crp.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            //if (IsPost == "Yes")
            // {
            string Ref = " Refund";
            try
            {
                dsAll.Clear();

                //String S1 = "Select * from tblChannelingDetails where ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "' AND Refund !='" + Ref + "'";
                String S1 = "Select * from tblChannelingDetails where ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(dsAll, "DtChanneling");

                // DirectPrint();

                frmChannelingReceipt frm = new frmChannelingReceipt(this);
                frm.Show();
            }
            catch { }
            // }
            // else 
            // {
            //   MessageBox.Show("This is not Posted Post First");
            // }
        }

        private void dtpDateFrom_Leave(object sender, EventArgs e)
        {
            dtpDateFrom_ValueChanged(sender, e);
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            // cmbCurrency.Items.Clear();
            try
            {
                string SelectCurrency = cmbCurrency.SelectedItem.ToString();

                String S = "Select Rate from tblCurrency where Currency='" + SelectCurrency + "'";

                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    {

                        double HC = Convert.ToDouble(txtHospitalCharge.Text);
                        double ConC = Convert.ToDouble(txtConsultantCharge.Text);
                        double TC = Convert.ToDouble(txtTotal.Text);

                        double CurrencyRate = Convert.ToDouble(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                        txtRate.Text = CurrencyRate.ToString();
                        double HCN = HC * CurrencyRate;
                        double ConCN = ConC * CurrencyRate;
                        double TCN = TC * CurrencyRate;

                        txtCHosCharge.Text = Convert.ToString(HCN);
                        txtCConsCharge.Text = Convert.ToString(ConCN);
                        txtCTotal.Text = Convert.ToString(TCN);
                        //cmbCurrency.Text = "Mrf";
                    }
                }

            }
            catch { }

            //// dataGridViewPatientHistory.DataSource = dt.Tables[0];
            //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //{

            //    cmbCurrency.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            //}




        }

        private void txtCHosCharge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtCConsCharge.Text.ToString().Trim() != "")
                {
                    txtCTotal.Text = Convert.ToString(Convert.ToDouble(txtCHosCharge.Text.ToString().Trim()) + Convert.ToDouble(txtCConsCharge.Text.ToString().Trim()));
                }
                else
                {
                    txtCTotal.Text = Convert.ToString(Convert.ToDouble(txtCHosCharge.Text.ToString().Trim()));
                }
            }
            catch { }
        }

        private void txtCConsCharge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtCHosCharge.Text.ToString().Trim() != "")
                {
                    txtCTotal.Text = Convert.ToString(Convert.ToDouble(txtCHosCharge.Text.ToString().Trim()) + Convert.ToDouble(txtCConsCharge.Text.ToString().Trim()));
                }
                else
                {
                    txtCTotal.Text = Convert.ToString(Convert.ToDouble(txtCConsCharge.Text.ToString().Trim()));
                }
            }
            catch { }
        }

        private void frmChanneling_Activated(object sender, EventArgs e)
        {
            //  txtDocDisRate.Enabled = false ;
            // txtDocDisRate.ReadOnly =true 

            if (Class1.IsReceiptSearch == 1)
            {
                ClearText();
                SetReceiptNO();
            }

            if (Class1.flg == 1)
            {
                //textBox1.Text = Class1.myvalue;
                if (Discountval != "2")
                {
                    txtConsultant.Text = a.GetText();
                }
                //Discountval = a.GetText3();
                //if (Discountval == "2")
                //{
                //    txtDocDisRate.Enabled = true;
                //    txtDocDisRate.ReadOnly = false;
                //    Class1.flg = 0;
                //}
                // Class1.flg = 0;
            }
            if (Class1.flgdiscount == 1)
            {
                Discountval = a.GetText3();
                if (Discountval == "2")
                {
                    txtDocDisRate.Enabled = true;
                    txtDocDisRate.ReadOnly = false;
                    Class1.flgdiscount = 0;
                }
            }
        }
        public string CheckPosted = "";
        private void button1_Click(object sender, EventArgs e)
        {

            //check wehter this receiptposted or no================
            try
            {
                String S2 = "select IsExport,Refund from tblChannelingDetails  WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlConnection con2 = new SqlConnection(ConnectionString);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, con2);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);



                CheckPosted = dt2.Rows[0].ItemArray[0].ToString();
                if (CheckPosted == "False")
                {
                    string Ref = "Refund";
                    string Export = "True";
                    string CU = user.userName;

                    if (rbtnboth.Checked == true)
                    {
                        DialogResult reply = MessageBox.Show("Do you Really want to refund this Receipts",
                                                 "Warnning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (reply == DialogResult.Yes)
                        {
                            //..............
                            try
                            {
                                String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + Export + "',DiscountAmt='" + txtDisAmount.Text.ToString().Trim() + "',TotalAmt='" + txtnetTotal.Text.ToString().Trim() + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + DoctorCGRefund + "',HosCal='" + HosCGRefund + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                                SqlCommand cmd = new SqlCommand(S);
                                SqlConnection con = new SqlConnection(ConnectionString);
                                SqlDataAdapter da = new SqlDataAdapter(S, con);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                MessageBox.Show("Refund Successfully");
                                // MessageBox.Show("Updated Successfully");
                            }
                            catch { }

                        }
                        else if (reply == DialogResult.No)
                        {
                            button1.Focus();
                        }

                    }
                    else
                    {
                        Export = "False";
                        DialogResult reply = MessageBox.Show("Do you Really want to refund this Receipts",
                                                    "Warnning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (reply == DialogResult.Yes)
                        {
                            //..............

                            try
                            {
                                // String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',IsExport='" + Export + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                                String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',IsExport='" + Export + "',DiscountAmt='" + txtDisAmount.Text.ToString().Trim() + "',TotalAmt='" + txtnetTotal.Text.ToString().Trim() + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + DoctorCGRefund + "',HosCal='" + HosCGRefund + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";


                                // String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + Export + "',DiscountAmt='" + txtDisAmount.Text.ToString().Trim() + "',TotalAmt='" + txtnetTotal.Text.ToString().Trim() + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                                SqlCommand cmd = new SqlCommand(S);
                                SqlConnection con = new SqlConnection(ConnectionString);
                                SqlDataAdapter da = new SqlDataAdapter(S, con);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                MessageBox.Show("Refund Successfully");
                                // MessageBox.Show("Updated Successfully");
                            }
                            catch { }


                        }
                        else if (reply == DialogResult.No)
                        {
                            button1.Focus();
                        }

                    }
                }
                else //if this receipt already posted
                {
                    string postrefund = "True";
                    string Ref = "Refund";
                    string Export = "False";
                    string CU = user.userName;

                    if (rbtnboth.Checked == true)
                    {
                        DialogResult reply = MessageBox.Show("Do you Really want to refund this Receipts",
                                                 "Warnning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (reply == DialogResult.Yes)
                        {
                            //..............
                            try
                            {
                                String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',Refund='" + Ref + "',IsExport='" + Export + "',DiscountAmt='" + txtDisAmount.Text.ToString().Trim() + "',TotalAmt='" + txtnetTotal.Text.ToString().Trim() + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',PostedRefund='" + postrefund + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + DoctorCGRefund + "',HosCal='" + HosCGRefund + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                                SqlCommand cmd = new SqlCommand(S);
                                SqlConnection con = new SqlConnection(ConnectionString);
                                SqlDataAdapter da = new SqlDataAdapter(S, con);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                MessageBox.Show("Refund Successfully");
                                // MessageBox.Show("Updated Successfully");
                            }
                            catch { }

                        }
                        else if (reply == DialogResult.No)
                        {
                            button1.Focus();
                        }

                    }
                    else
                    {
                        // Export = "False";
                        DialogResult reply = MessageBox.Show("Do you Really want to refund this Receipts",
                                                    "Warnning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (reply == DialogResult.Yes)
                        {
                            //..............

                            try
                            {
                                // String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',IsExport='" + Export + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                                String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',IsExport='" + Export + "',DiscountAmt='" + txtDisAmount.Text.ToString().Trim() + "',TotalAmt='" + txtnetTotal.Text.ToString().Trim() + "',DisRate='" + txtdisRate.Text.ToString().Trim() + "',PostedRefund='" + postrefund + "',NetTotalCal='" + NetTotalRefund.ToString().Trim() + "',DocCal='" + DoctorCGRefund + "',HosCal='" + HosCGRefund + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                                SqlCommand cmd = new SqlCommand(S);
                                SqlConnection con = new SqlConnection(ConnectionString);
                                SqlDataAdapter da = new SqlDataAdapter(S, con);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                MessageBox.Show("Refund Successfully");
                                // MessageBox.Show("Updated Successfully");
                            }
                            catch { }


                        }
                        else if (reply == DialogResult.No)
                        {
                            button1.Focus();
                        }

                    }


                }//if posted ths record



            }
            catch { }

            //================================        
        }
        public double NetTotalRefund = 0.00;
        public double DoctorCGRefund = 0.00;
        public double HosCGRefund = 0.00;
        public int AB = 0;
        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            //.................................
            AB = 2;
            btnEdit.Enabled = false;
            CheckSearch = 2;
            try
            {
                if (cmbSearchReceipt.Text == "Receipts Number")
                {
                    string add = txtFind.Text;
                    if (add != "")
                    {
                        String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);

                        if (dt2.Rows.Count > 0)
                        {

                            txtReceiptNo.Text = dt2.Rows[0].ItemArray[0].ToString();

                            txtPatientNo.Text = dt2.Rows[0].ItemArray[2].ToString();
                            txtCategory.Text = dt2.Rows[0].ItemArray[7].ToString();
                            txtConsultant.Text = dt2.Rows[0].ItemArray[3].ToString();
                            txtTokenNo.Text = dt2.Rows[0].ItemArray[1].ToString();
                            txtFirstName.Text = dt2.Rows[0].ItemArray[4].ToString();
                            dtpDateFrom.Text = dt2.Rows[0].ItemArray[8].ToString();
                            cmbSession.Text = dt2.Rows[0].ItemArray[9].ToString();
                            txtRoom.Text = dt2.Rows[0].ItemArray[10].ToString();
                            txtContactNo.Text = dt2.Rows[0].ItemArray[11].ToString();
                            txtHospitalCharge.Text = dt2.Rows[0].ItemArray[12].ToString();

                            txtTemHos.Text = dt2.Rows[0].ItemArray[38].ToString();//temperaly allocated data
                            txtTempDoc.Text = dt2.Rows[0].ItemArray[37].ToString();//temperaly allocated data

                            txtConsultantCharge.Text = dt2.Rows[0].ItemArray[13].ToString();



                            txtTotal.Text = dt2.Rows[0].ItemArray[15].ToString();
                            cmbPaymentMethod.Text = dt2.Rows[0].ItemArray[16].ToString();
                            txtRemarks.Text = dt2.Rows[0].ItemArray[17].ToString();
                            txtCreditCardNo.Text = dt2.Rows[0].ItemArray[19].ToString();
                            txtCHosCharge.Text = dt2.Rows[0].ItemArray[20].ToString();
                            txtCConsCharge.Text = dt2.Rows[0].ItemArray[21].ToString();


                            txtCTotal.Text = dt2.Rows[0].ItemArray[22].ToString();
                            txtRate.Text = dt2.Rows[0].ItemArray[23].ToString();
                            dtpReceiptDate.Text = dt2.Rows[0].ItemArray[25].ToString();
                            txtDisAmount.Text = dt2.Rows[0].ItemArray[28].ToString();
                            txtnetTotal.Text = dt2.Rows[0].ItemArray[29].ToString();
                            txtdisRate.Text = dt2.Rows[0].ItemArray[30].ToString();
                            NetTotalRefund = Convert.ToDouble(txtnetTotal.Text);

                            DoctorCGRefund = Convert.ToDouble(txtConsultantCharge.Text);
                            HosCGRefund = Convert.ToDouble(txtHospitalCharge.Text);
                            txtDocDisRate.Text = dt2.Rows[0].ItemArray[36].ToString();
                            //========================
                            if (dt2.Rows[0].ItemArray[2].ToString().Substring(0, 1) == "B")
                            {
                                rbtnbht.Checked = true;
                            }
                            else if (dt2.Rows[0].ItemArray[2].ToString().Substring(0, 1) == "E")
                            {
                                rbtnetuopd.Checked = true;
                            }
                            else { }
                            //===========================================



                            if (dt2.Rows[0].ItemArray[35].ToString() == "Yes")
                            {
                                cbxPhoneBooking.Checked = true;
                                if (dt2.Rows[0].ItemArray[16].ToString() == "Phone Booking")
                                {
                                    // btnp.Enabled = true;
                                    btnPay.Enabled = true;
                                    cbxnotpaid.Visible = true;
                                    cbxnotpaid.Checked = true;
                                }

                            }
                            else
                            {
                                cbxPhoneBooking.Checked = false;
                                cbxnotpaid.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Receipt not found");
                    }

                    txtFirstName.ReadOnly = false;
                    txtContactNo.ReadOnly = false;
                    txtRemarks.ReadOnly = false;


                }

                //SearchBy Patient Name
                if (cmbSearchReceipt.Text == "Patient Name")
                {
                    string add = txtFind.Text;
                    if (add != "")
                    {
                        String S2 = "Select * from tblChannelingDetails where FirstName LIKE  '" + add + "%'";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);

                        if (dt2.Rows.Count > 0)
                        {

                            txtReceiptNo.Text = dt2.Rows[0].ItemArray[0].ToString();

                            txtPatientNo.Text = dt2.Rows[0].ItemArray[2].ToString();
                            txtCategory.Text = dt2.Rows[0].ItemArray[7].ToString();
                            txtConsultant.Text = dt2.Rows[0].ItemArray[3].ToString();
                            txtTokenNo.Text = dt2.Rows[0].ItemArray[1].ToString();
                            txtFirstName.Text = dt2.Rows[0].ItemArray[4].ToString();
                            dtpDateFrom.Text = dt2.Rows[0].ItemArray[8].ToString();
                            cmbSession.Text = dt2.Rows[0].ItemArray[9].ToString();
                            txtRoom.Text = dt2.Rows[0].ItemArray[10].ToString();
                            txtContactNo.Text = dt2.Rows[0].ItemArray[11].ToString();
                            txtHospitalCharge.Text = dt2.Rows[0].ItemArray[12].ToString();
                            txtConsultantCharge.Text = dt2.Rows[0].ItemArray[13].ToString();
                            txtTotal.Text = dt2.Rows[0].ItemArray[15].ToString();
                            cmbPaymentMethod.Text = dt2.Rows[0].ItemArray[16].ToString();
                            txtRemarks.Text = dt2.Rows[0].ItemArray[17].ToString();
                            txtCreditCardNo.Text = dt2.Rows[0].ItemArray[19].ToString();
                            txtCHosCharge.Text = dt2.Rows[0].ItemArray[20].ToString();
                            txtCConsCharge.Text = dt2.Rows[0].ItemArray[21].ToString();
                            txtCTotal.Text = dt2.Rows[0].ItemArray[22].ToString();
                            txtRate.Text = dt2.Rows[0].ItemArray[23].ToString();
                            dtpReceiptDate.Text = dt2.Rows[0].ItemArray[25].ToString();
                            NetTotalRefund = Convert.ToDouble(txtnetTotal.Text);
                            txtDocDisRate.Text = dt2.Rows[0].ItemArray[36].ToString();

                            txtTemHos.Text = dt2.Rows[0].ItemArray[38].ToString();//temperaly allocated data
                            txtTempDoc.Text = dt2.Rows[0].ItemArray[37].ToString();//temperaly allocated data

                            if (dt2.Rows[0].ItemArray[35].ToString() == "Yes")
                            {
                                cbxPhoneBooking.Checked = true;
                                if (dt2.Rows[0].ItemArray[16].ToString() == "Phone Booking")
                                {
                                    btnPay.Enabled = true;
                                    cbxnotpaid.Visible = true;
                                    cbxnotpaid.Checked = true;
                                }
                            }
                            {
                                cbxPhoneBooking.Checked = false;
                                cbxnotpaid.Visible = false;
                                //cbxnotpaid.Checked = true;
                            }

                            DoctorCGRefund = Convert.ToDouble(txtConsultantCharge.Text);
                            HosCGRefund = Convert.ToDouble(txtHospitalCharge.Text);
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Receipt not found");
                    }

                    txtFirstName.ReadOnly = false;
                    txtContactNo.ReadOnly = false;
                    txtRemarks.ReadOnly = false;
                }
                //..........................


                //SearchBy ConTactNumber
                if (cmbSearchReceipt.Text == "Contact No")
                {
                    string add = txtFind.Text;
                    if (add != "")
                    {
                        String S2 = "Select * from tblChannelingDetails where ContactNO LIKE  '" + add + "%'";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);

                        if (dt2.Rows.Count > 0)
                        {

                            txtReceiptNo.Text = dt2.Rows[0].ItemArray[0].ToString();

                            txtPatientNo.Text = dt2.Rows[0].ItemArray[2].ToString();
                            txtCategory.Text = dt2.Rows[0].ItemArray[7].ToString();
                            txtConsultant.Text = dt2.Rows[0].ItemArray[3].ToString();
                            txtTokenNo.Text = dt2.Rows[0].ItemArray[1].ToString();
                            txtFirstName.Text = dt2.Rows[0].ItemArray[4].ToString();
                            dtpDateFrom.Text = dt2.Rows[0].ItemArray[8].ToString();
                            cmbSession.Text = dt2.Rows[0].ItemArray[9].ToString();
                            txtRoom.Text = dt2.Rows[0].ItemArray[10].ToString();
                            txtContactNo.Text = dt2.Rows[0].ItemArray[11].ToString();
                            txtHospitalCharge.Text = dt2.Rows[0].ItemArray[12].ToString();
                            txtConsultantCharge.Text = dt2.Rows[0].ItemArray[13].ToString();
                            txtTotal.Text = dt2.Rows[0].ItemArray[15].ToString();
                            cmbPaymentMethod.Text = dt2.Rows[0].ItemArray[16].ToString();
                            txtRemarks.Text = dt2.Rows[0].ItemArray[17].ToString();
                            txtCreditCardNo.Text = dt2.Rows[0].ItemArray[19].ToString();
                            txtCHosCharge.Text = dt2.Rows[0].ItemArray[20].ToString();
                            txtCConsCharge.Text = dt2.Rows[0].ItemArray[21].ToString();
                            txtCTotal.Text = dt2.Rows[0].ItemArray[22].ToString();
                            txtRate.Text = dt2.Rows[0].ItemArray[23].ToString();
                            dtpReceiptDate.Text = dt2.Rows[0].ItemArray[25].ToString();
                            NetTotalRefund = Convert.ToDouble(txtnetTotal.Text);
                            txtDocDisRate.Text = dt2.Rows[0].ItemArray[36].ToString();

                            txtTemHos.Text = dt2.Rows[0].ItemArray[38].ToString();//temperaly allocated data
                            txtTempDoc.Text = dt2.Rows[0].ItemArray[37].ToString();//temperaly allocated data


                            if (dt2.Rows[0].ItemArray[35].ToString() == "Yes")
                            {
                                cbxPhoneBooking.Checked = true;
                                if (dt2.Rows[0].ItemArray[16].ToString() == "Phone Booking")
                                {
                                    btnPay.Enabled = true;
                                    cbxnotpaid.Visible = true;
                                    cbxnotpaid.Checked = true;
                                }
                            }
                            {
                                cbxPhoneBooking.Checked = false;
                                cbxnotpaid.Visible = false;
                                //cbxnotpaid.Checked = true;
                            }

                            DoctorCGRefund = Convert.ToDouble(txtConsultantCharge.Text);
                            HosCGRefund = Convert.ToDouble(txtHospitalCharge.Text);
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Receipt not found");
                    }
                }

                txtFirstName.ReadOnly = false;
                txtContactNo.ReadOnly = false;
                txtRemarks.ReadOnly = false;
                //.....................
            }
            catch { }

            // txtFind.Text = "";
            btnNew.Enabled = false; ;
            // btnEdit.Enabled = true;
            btnSave.Enabled = false;
            button1.Enabled = true;
            btnPrint.Enabled = true;
            //btnDelete.Enabled = true;
            //btnEdit.Enabled = true;
            dtpDateFrom.Enabled = false;

            rbtnHosfee.Enabled = true;
            rbtnDoctorfee.Enabled = true;
            rbtnboth.Enabled = true;

            //.....................................

        }

        private void lstvCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rbtnDoctorfee_CheckedChanged(object sender, EventArgs e)
        {
            txtFind_TextChanged(sender, e);
            txtConsultantCharge.Text = "0";// +txtConsultantCharge.Text.ToString();
            double a = Convert.ToDouble(txtConsultantCharge.Text);
            double b = Convert.ToDouble(txtHospitalCharge.Text);

            double dishos = Convert.ToDouble(txtTemHos.Text);

            txtTotal.Text = Convert.ToString(b + a);
            //txtnetTotal.Text = (b + a).ToString("N2");

            txtDisAmount.Text = dishos.ToString("N2");

            txtnetTotal.Text = ((b + a) - dishos).ToString("N2");

            // btnPrint_Click(sender, e);
            // txtdisRate_TextChanged(sender, e);
            // txtDocDisRate_TextChanged(sender, e);

            //txtFind_TextChanged(sender, e);
            //txtHospitalCharge.Text = "0";
            //// txtConsultantCharge.Text = "0";
            //int c = Convert.ToInt32(txtHospitalCharge.Text);
            //int d = Convert.ToInt32(txtConsultantCharge.Text);
            //txtTotal.Text = Convert.ToString(d + c);
            //txtdisRate_TextChanged(sender, e);
        }

        private void rbtnHosfee_CheckedChanged(object sender, EventArgs e)
        {

            //txtFind_TextChanged(sender, e);
            //txtConsultantCharge.Text = "0";// +txtConsultantCharge.Text.ToString();
            //int a = Convert.ToInt32(txtConsultantCharge.Text);
            //int b = Convert.ToInt32(txtHospitalCharge.Text);
            //txtTotal.Text = Convert.ToString(b + a);
            //// btnPrint_Click(sender, e);
            //txtdisRate_TextChanged(sender, e);


            txtFind_TextChanged(sender, e);

            txtHospitalCharge.Text = "0";
            // txtConsultantCharge.Text = "0";
            double c = Convert.ToDouble(txtHospitalCharge.Text);
            double d = Convert.ToDouble(txtConsultantCharge.Text);

            double d2 = Convert.ToDouble(txtTempDoc.Text);

            // double dd = 0.00;
            double Rate1 = Convert.ToDouble(txtDocDisRate.Text);

            txtTotal.Text = Convert.ToString(d + c);
            // txtdisRate_TextChanged(sender, e);

            // dd = d2 * 100 / Rate1;
            txtDisAmount.Text = d2.ToString("N2");
            txtnetTotal.Text = ((d + c) - d2).ToString("N2");
            // txtDisAmount.Text =( d2 * Rate1 / 100).ToString("N2"); 
            //txtDisAmount.Text = (txthosDiscount.Text).ToString("N2");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // public static  int once = 1;
        private void txtdisRate_TextChanged(object sender, EventArgs e)
        {
            if (CheckSearch != 2)
            {
                // double hospitalcharge = 0.00;
                double Rate = 0.00;
                double DiscountAmount = 0.00;
                double totBefoDiscount = 0.00;
                double NetTotal = 0.00;
                double DoctorCharge = 0.00;

                double AfterDHC = 0.00;

                // if (once == 1)
                // {
                try
                {


                    // totBefoDiscount = Convert.ToDouble(txtTotal.Text.Trim());//txtHospitalCharge
                    totBefoDiscount = Convert.ToDouble(txtTempHosFee.Text.Trim());//txtHospitalCharge
                    DoctorCharge = Convert.ToDouble(txtTempDocFee.Text.Trim());//Doctprcharge

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
                    DiscountAmount = totBefoDiscount * Rate;
                    // txtDisAmount.Text = Convert.ToString(DiscountAmount);
                    txthosDiscount.Text = DiscountAmount.ToString("N2");
                    txtDisAmount.Text = (Convert.ToDouble(txthosDiscount.Text) + Convert.ToDouble(txtdocdiscount.Text)).ToString("N2");
                    // txtDisAmount.Text = DiscountAmount.ToString("N2");
                    NetTotal = (totBefoDiscount + DoctorCharge) - (Convert.ToDouble(txthosDiscount.Text) + Convert.ToDouble(txtdocdiscount.Text));
                    // txtnetTotal.Text = Convert.ToString(NetTotal);
                    // txtnetTotal.Text = NetTotal.ToString("N2");
                    txtnetTotal.Text = NetTotal.ToString("N2");
                    AfterDHC = totBefoDiscount - DiscountAmount;

                    //  txtnetTotal.Text = (Convert.ToDouble(txtnetTotal.Text) - Convert.ToDouble(txtDisAmount.Text)).ToString("N2");//.Tostring("N2");

                    //txtHospitalCharge.Text = AfterDHC.ToString("N2"); 

                    // txtDocDisRate_TextChanged(sender, e);


                    //   = Convert.ToString(Convert.ToInt32(txtHospitalCharge.Text) - DiscountAmount);

                }
                catch { MessageBox.Show("Enter Correct Discount Rate"); }
            }


        }

        private void rbtnboth_CheckedChanged(object sender, EventArgs e)
        {
            txtFind_TextChanged(sender, e);
            txtHospitalCharge.Text = "0";
            txtConsultantCharge.Text = "0";
            double c = Convert.ToDouble(txtHospitalCharge.Text);
            double d = Convert.ToDouble(txtConsultantCharge.Text);
            txtTotal.Text = Convert.ToString(d + c);
            txtnetTotal.Text = (d + c).ToString("N2");
            txtDisAmount.Text = "0.00";
            //txtdisRate_TextChanged(sender, e);
        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMethod.SelectedItem.ToString().Trim() == "Credit Card")
            {
                txtCreditCardNo.ReadOnly = false;
            }
        }

        private void txtConsultant_Enter(object sender, EventArgs e)
        {
            //            try
            //            {
            //                String S3 = "Select DISTINCT(ReceiptsNo)from tblChannelingDetails  where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDateFrom.Text.ToString().Trim() + "')";
            //                // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
            //                SqlCommand cmd3 = new SqlCommand(S3);
            //                SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
            //                DataTable dt = new DataTable();
            //                da3.Fill(dt);
            //                int NoOfTokens = 0;
            //                if (dt.Rows.Count > 0)
            //                {
            //                    //if (dt.Rows[0].ItemArray[0].ToString() == "0")
            //                    // {
            //                    // NoOfTokens = 1;
            //                    // }
            //                    //else
            //                    //{
            //                    NoOfTokens = dt.Rows.Count + 1;
            //                    // }
            //                }
            //                else
            //                {
            //                    NoOfTokens = 1;
            //                }
            //                txtTokenNo.Text = NoOfTokens.ToString();
            //                //......................................

            ////=================================================
            //            //      }
            //            //catch { }




            //                DateTime GetSTime = Convert.ToDateTime(dtpDateFrom.Text.ToString().Trim());
            //                string GetDay = GetSTime.DayOfWeek.ToString().Trim();
            //              //  string TimeForCahnel = "";


            //                String S4 = "Select SessionTime from tblSchedulingDetails  where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (Aday = '" + GetDay + "')";
            //                // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
            //                SqlCommand cmd4 = new SqlCommand(S4);
            //                SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
            //                DataTable dt1 = new DataTable();
            //                da4.Fill(dt1);

            //                if (dt1.Rows[0].ItemArray[0].ToString().Trim() == "")
            //                {
            //                   // MessageBox.Show("Please Schedule this Consultant");                
            //                }
            //                else
            //                {
            //                    cmbSession.Text = dt1.Rows[0].ItemArray[0].ToString();
            //                }


            //            }
            //            catch { }
        }

        private void grpChanneling_Enter(object sender, EventArgs e)
        {

        }

        private void btnDiscount_Click(object sender, EventArgs e)
        {


            frmDisountValidate disval = new frmDisountValidate();
            disval.Show();

        }

        private void txtDocDisRate_TextChanged(object sender, EventArgs e)
        {
            if (CheckSearch != 2)
            {
                // double hospitalcharge = 0.00;
                double Rate = 0.00;
                double DiscountAmount = 0.00;
                double totBefoDiscount = 0.00;
                double NetTotal = 0.00;
                double DoctorCharge = 0.00;

                double AfterDHC = 0.00;

                // if (once == 1)
                // {
                try
                {


                    // totBefoDiscount = Convert.ToDouble(txtTotal.Text.Trim());//txtHospitalCharge
                    totBefoDiscount = Convert.ToDouble(txtTempDocFee.Text.Trim());//txtHospitalCharge
                    DoctorCharge = Convert.ToDouble(txtTempHosFee.Text.Trim());//Doctprcharge//txtTempHosFee.Text.Trim()

                }
                catch { }
                // once = 2;
                // }

                if (txtDocDisRate.Text != "")
                {
                    Rate = (Convert.ToDouble(txtDocDisRate.Text.Trim())) / 100;
                }
                else
                {
                    Rate = 0.00;
                }

                try
                {
                    DiscountAmount = totBefoDiscount * Rate;
                    // txtDisAmount.Text = Convert.ToString(DiscountAmount);
                    txtdocdiscount.Text = DiscountAmount.ToString("N2");


                    txtDisAmount.Text = (Convert.ToDouble(txthosDiscount.Text) + Convert.ToDouble(txtdocdiscount.Text)).ToString("N2");
                    NetTotal = (totBefoDiscount + DoctorCharge) - (Convert.ToDouble(txthosDiscount.Text) + Convert.ToDouble(txtdocdiscount.Text));
                    // txtnetTotal.Text = Convert.ToString(NetTotal);
                    // txtnetTotal.Text = NetTotal.ToString("N2");
                    txtnetTotal.Text = NetTotal.ToString("N2");
                    AfterDHC = totBefoDiscount - DiscountAmount;

                    // txtConsultantCharge.Text  = AfterDHC.ToString("N2");

                    // txtdisRate_TextChanged(sender, e);
                    //   = Convert.ToString(Convert.ToInt32(txtHospitalCharge.Text) - DiscountAmount);

                    //txtnetTotal.Text = (Convert.ToDouble(txtnetTotal.Text) - Convert.ToDouble(txtDisAmount.Text)).ToString("N2");
                }
                catch { MessageBox.Show("Enter Correct Discount Rate"); }
            }
        }

        private void cbxPhoneBooking_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckSearch != 2)
            {
                if (cbxPhoneBooking.Checked == true)
                {
                    cmbPaymentMethod.SelectedItem = "Phone Booking";
                    cmbPaymentMethod.Enabled = false;
                }
                else
                {
                    cmbPaymentMethod.SelectedItem = "Cash";
                    cmbPaymentMethod.Enabled = true;
                }
            }
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {

                Class1.IsReceiptSearch = 0;
                DialogResult reply = MessageBox.Show("Are you sure, you want to Pay this Bill ? ", "Information", MessageBoxButtons.YesNo);

                if (reply == DialogResult.Yes)
                {
                    String S = "Update tblChannelingDetails SET PaymentMethod = 'Cash',RepDate = '" + dtpReceiptDate.Text.ToString().Trim() + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlConnection con = new SqlConnection(ConnectionString);
                    SqlDataAdapter da = new SqlDataAdapter(S, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    MessageBox.Show("Paid Successfully");
                    btnPrint_Click(sender, e);
                    btnNewNo_Click(sender,e);
                }
                else
                {
                    return;
                }


            }
            catch (Exception ex)

            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtPatientNo_TextChanged(object sender, EventArgs e)
        {
            //if (k != 1)
            // {
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
                                // txtPatientNo.Text = "";
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
            // }
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            Class1.IsReceiptSearch = 0;
        }
        // public string PationNo = "";
        // public int k = 0;
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

        private void txtFirstName_Enter(object sender, EventArgs e)
        {

        }

        private void btnNew_TabIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (add)
            //    {


            //        EnableObjects();
            //        ClearText();
            //        getReceiptNo();
            //        txtDocDisRate.Text = "0";
            //        txtDocDisRate.Enabled = false;

            //        btnNew.Enabled = false;
            //        btnEdit.Enabled = false;

            //        txtFind.Enabled = false;
            //        cmbSearchReceipt.Enabled = false;
            //        txtdisRate.Enabled = true;
            //        txtdisRate.ReadOnly = false;
            //        btnDiscount.Enabled = true;
            //        cbxPhoneBooking.Enabled = true;

            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //catch { }
        }

        private void btnNew_Leave(object sender, EventArgs e)
        {
            //try
            //{
            //    if (add)
            //    {


            //        EnableObjects();
            //        ClearText();
            //        getReceiptNo();
            //        txtDocDisRate.Text = "0";
            //        txtDocDisRate.Enabled = false;

            //        btnNew.Enabled = false;
            //        btnEdit.Enabled = false;

            //        txtFind.Enabled = false;
            //        cmbSearchReceipt.Enabled = false;
            //        txtdisRate.Enabled = true;
            //        txtdisRate.ReadOnly = false;
            //        btnDiscount.Enabled = true;
            //        cbxPhoneBooking.Enabled = true;

            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //catch { }
        }

        private void cmbSearchReceipt_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        private void cbxToken2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtnetuopd_CheckedChanged(object sender, EventArgs e)
        {
            txtPatientNo.Text = "";
            txtFirstName.Text = "";
            txtContactNo.Text = "";
            txtPatientNo.ReadOnly = false;
            txtPatientNo.Focus();
            cmbPaymentMethod.Text = "Credit";

            cbxPhoneBooking.Checked = false;
            cbxPhoneBooking.Enabled = false;
        }

        private void rbtnbht_CheckedChanged(object sender, EventArgs e)
        {
            txtPatientNo.Text = "";
            txtFirstName.Text = "";
            txtContactNo.Text = "";
            txtPatientNo.ReadOnly = false;
            txtPatientNo.Focus();
            cmbPaymentMethod.Text = "Credit";
            cbxPhoneBooking.Checked = false;
            cbxPhoneBooking.Enabled = false;
        }

        private void rbtncash_CheckedChanged(object sender, EventArgs e)
        {
            txtPatientNo.Text = "";
            txtFirstName.Text = "";
            txtContactNo.Text = "";
            txtPatientNo.ReadOnly = true;
            txtFirstName.Focus();
            cbxPhoneBooking.Enabled = true;
            cbxPhoneBooking.Checked = false;
            cmbPaymentMethod.Text = "Cash";

        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            // frmAppointment_Load(sender, e);

            cmbSearchReceipt.Enabled = true;
            txtFind.Enabled = true;

            txtReceiptNo.Enabled = false;

            txtPatientNo.Enabled = false;
            txtCategory.Enabled = false;
            txtConsultant.Enabled = false;
            txtTokenNo.Enabled = false;
            txtFirstName.ReadOnly = true;

            dtpDateFrom.Enabled = false;

            cmbSession.Enabled = false;
            txtRoom.ReadOnly = true;

            txtContactNo.ReadOnly = true;
            txtHospitalCharge.Enabled = false;

            txtTemHos.Enabled = false;
            txtTempDoc.Enabled = false;
            txtConsultantCharge.Enabled = false;



            txtTotal.Enabled = false;
            cmbPaymentMethod.Enabled = false;
            txtRemarks.Enabled = false;
            txtCreditCardNo.Enabled = false;
            txtCHosCharge.Enabled = false;
            txtCConsCharge.Enabled = false;


            txtCTotal.Enabled = false;
            txtRate.Enabled = false;
            dtpReceiptDate.Enabled = false;
            txtDisAmount.Enabled = false;
            txtnetTotal.Enabled = false;
            txtdisRate.Enabled = false;

            btnNew.Enabled = true;




        }

        private void txtChannelingTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                frmInvoiceList ObjInvList = new frmInvoiceList();
                ObjInvList.Show();

            }
            catch
            {

            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {

                SetReceiptNO();
            }
            catch
            {

            }
        }

        private void SetReceiptNO()
        {
            try
            {
                //string ConnString = ConnectionString;
                //String S1 = "Select Date,ReceiptsNo,FirstName,ContactNO,ConsultantName,PaymentMethod,PatientNo,Room,TokenNo,Remarks from tblChannelingDetails  where ReceiptsNo='" + a.GetInvoice().ToString().Trim() + "'";
                ////String S1 = "Select distinct(InvoiceNo),CustomerID,JobID,ItemID,Description,InvoiceQty,UnitPrice,Amount,GrandTotal from tblCustomerInvoice"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                //SqlCommand cmd1 = new SqlCommand(S1);
                //SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                //DataTable dt = new DataTable();
                //da1.Fill(dt);
                //// dgvItemList.Rows.Clear();
                ////dgvItemList.Rows.Add();
                //if (dt.Rows.Count > 0)
                //{
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        txtReceiptNo.Text = dt.Rows[i].ItemArray[1].ToString().Trim();
                //        dtpDateFrom.Text = dt.Rows[i].ItemArray[0].ToString().Trim();
                //        txtFirstName.Text = dt.Rows[i].ItemArray[2].ToString().Trim();
                //        txtContactNo.Text = dt.Rows[i].ItemArray[3].ToString().Trim();
                //        txtConsultant.Text = dt.Rows[i].ItemArray[4].ToString().Trim();
                //        cmbPaymentMethod.Text = dt.Rows[i].ItemArray[5].ToString().Trim();
                //        txtPatientNo.Text = dt.Rows[i].ItemArray[6].ToString().Trim();
                //        txtRoom.Text = dt.Rows[i].ItemArray[7].ToString().Trim();
                //        txtTokenNo.Text = dt.Rows[i].ItemArray[8].ToString().Trim();
                //        txtRemarks.Text = dt.Rows[i].ItemArray[9].ToString().Trim();
                //    }
                //}
                //..............................................

                //if (cmbSearchReceipt.Text == "Receipts Number")
                //{
                //string add = txtFind.Text;
                //if (add != "")
                //{
                String S2 = "Select * from tblChannelingDetails where ReceiptsNo='" + a.GetInvoice().ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);

                if (dt2.Rows.Count > 0)
                {

                    txtReceiptNo.Text = dt2.Rows[0].ItemArray[0].ToString();

                    txtPatientNo.Text = dt2.Rows[0].ItemArray[2].ToString();
                    txtCategory.Text = dt2.Rows[0].ItemArray[7].ToString();
                    txtConsultant.Text = dt2.Rows[0].ItemArray[3].ToString();
                    txtTokenNo.Text = dt2.Rows[0].ItemArray[1].ToString();
                    txtFirstName.Text = dt2.Rows[0].ItemArray[4].ToString();
                    dtpDateFrom.Text = dt2.Rows[0].ItemArray[8].ToString();
                    cmbSession.Text = dt2.Rows[0].ItemArray[9].ToString();
                    txtRoom.Text = dt2.Rows[0].ItemArray[10].ToString();
                    txtContactNo.Text = dt2.Rows[0].ItemArray[11].ToString();
                    txtHospitalCharge.Text = dt2.Rows[0].ItemArray[12].ToString();

                    txtTemHos.Text = dt2.Rows[0].ItemArray[38].ToString();//temperaly allocated data
                    txtTempDoc.Text = dt2.Rows[0].ItemArray[37].ToString();//temperaly allocated data

                    txtConsultantCharge.Text = dt2.Rows[0].ItemArray[13].ToString();



                    txtTotal.Text = dt2.Rows[0].ItemArray[15].ToString();
                    cmbPaymentMethod.Text = dt2.Rows[0].ItemArray[16].ToString();
                    txtRemarks.Text = dt2.Rows[0].ItemArray[17].ToString();
                    txtCreditCardNo.Text = dt2.Rows[0].ItemArray[19].ToString();
                    txtCHosCharge.Text = dt2.Rows[0].ItemArray[20].ToString();
                    txtCConsCharge.Text = dt2.Rows[0].ItemArray[21].ToString();


                    txtCTotal.Text = dt2.Rows[0].ItemArray[22].ToString();
                    txtRate.Text = dt2.Rows[0].ItemArray[23].ToString();
                    dtpReceiptDate.Text = dt2.Rows[0].ItemArray[25].ToString();
                    txtDisAmount.Text = dt2.Rows[0].ItemArray[28].ToString();
                    txtnetTotal.Text = dt2.Rows[0].ItemArray[29].ToString();
                    txtdisRate.Text = dt2.Rows[0].ItemArray[30].ToString();
                    NetTotalRefund = Convert.ToDouble(txtnetTotal.Text);

                    DoctorCGRefund = Convert.ToDouble(txtConsultantCharge.Text);
                    HosCGRefund = Convert.ToDouble(txtHospitalCharge.Text);
                    txtDocDisRate.Text = dt2.Rows[0].ItemArray[36].ToString();
                    //========================
                    if (dt2.Rows[0].ItemArray[2].ToString().Substring(0, 1) == "B")
                    {
                        rbtnbht.Checked = true;
                    }
                    else if (dt2.Rows[0].ItemArray[2].ToString().Substring(0, 1) == "E")
                    {
                        rbtnetuopd.Checked = true;
                    }
                    else { }
                    //===========================================



                    if (dt2.Rows[0].ItemArray[35].ToString() == "Yes")
                    {
                        cbxPhoneBooking.Checked = true;
                        if (dt2.Rows[0].ItemArray[16].ToString() == "Phone Booking")
                        {
                            // btnp.Enabled = true;
                            btnPay.Enabled = true;
                            cbxnotpaid.Visible = true;
                            cbxnotpaid.Checked = true;
                        }

                    }
                    else
                    {
                        cbxPhoneBooking.Checked = false;
                        cbxnotpaid.Visible = false;
                    }
                }
                //}
                //else
                //{
                //    //MessageBox.Show("Receipt not found");
                //}

                txtFirstName.ReadOnly = false;
                txtContactNo.ReadOnly = false;
                txtRemarks.ReadOnly = false;


                //}
            }
            catch
            {

            }
        }

        private void txtContactNo_TextChanged(object sender, EventArgs e)
        {
            Class1.IsReceiptSearch = 0;
        }

        private void txtRemarks_TextChanged(object sender, EventArgs e)
        {
            Class1.IsReceiptSearch = 0;
        }
    }
}