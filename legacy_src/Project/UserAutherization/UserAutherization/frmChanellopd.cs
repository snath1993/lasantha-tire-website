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

namespace UserAutherization
{
    public partial class frmChanellopd : Form
    {
        DataTable dtUser = new DataTable();
        Class1 a = new Class1();
        public int flg = 0;
        public int flgTech = 0;
        public int CheckSearch = 0;//to avoid goto textchange event after find result

        public DsScan  DsScanReport = new DsScan();

        //bool run = false;
        //bool add = false;
        //bool edit = false;
        //bool delete = false;

        public static string ConnectionString;

        public frmChanellopd()
        {
            InitializeComponent();
            setConnectionString();
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
        

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                txtSelectItem.Enabled = true;
                cmbCinsultant.Text = "";
                cmbroom.Text = "";
                txtReceiptNo.Text = "";
                txtTime.Text = "";

                rbtncash.Enabled = true;
               // rbtncash.Checked = true;

                rbtnbht.Enabled = true;
                rbtnetuopd.Enabled = true;

                rbtnetuopd.Checked = true;

                rbtnbht.Checked = false;
               // rbtnetuopd.Checked = false;

                txtAge.Text = "";
                txtAge.Enabled = true;

                txtAddress1.Text = "";
                txtAddress1.Enabled = true;

                cmbCinsultant.Enabled = true;
                cmbroom.Enabled = true;
               // if (add)
               // {
                    //dgvScanItems[0, x].ReadOnly = false;
                  //  dgvScanItems.Columns[3].ReadOnly = false;
                   A = 0;
                    cbxInpatient.Enabled = true;
                    txtdisRate.Text = "0";
                    txtDisAmount.Text = "0";
                    txtnetTotal.Text = "0";
                    txtTotal.Text = "0";
                    txtScanName.Text = "0";
                    Clear_Click(sender,e);
                    string currenru1 = user.userName;
                    ClearText();
                    EnableObjects();
                    getTokenNo();

                    string ConnString = ConnectionString;
                    string sql = "Select HospitalCharge from tblDefaultSettings";
                    SqlConnection Conn = new SqlConnection(ConnString);
                    SqlCommand cmd = new SqlCommand(sql);
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                       // txtHospitalFee.Text = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                   
                    }
                    btnNew.Enabled = false;
                    btnEdit.Enabled = false;
                    flg = 2;
                //}
                //else
                //{
                //    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
            }
            catch { }
            cmbScanSerch.Enabled = false;
            txtConsultantFind.Enabled = false;
            btnReprintScan.Enabled = false;
            btnPrintCus.Enabled = false;
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
                txtNightCharge.Enabled = true;
                txtNightCharge.ReadOnly = false;
                txtScanName.ReadOnly = false;
                txtPatientNo.ReadOnly = false;
                txtReceiptNo.Enabled = true;
                dtpDate.Enabled = true;
                txtScanName.ReadOnly = false;
                txtConsultant.ReadOnly = false;
                txtFirstName.ReadOnly = false;
                txtContactNo.ReadOnly = false;
                txtRemarks.ReadOnly = false;


                txtAddress1.ReadOnly = false;
                txtAge.ReadOnly = false;

              //  txtScanFee.ReadOnly = false;
               // txtHospitalFee.ReadOnly = false;
               // txtTotal.ReadOnly = false;
                cmbPaymentMethod.Enabled = true;
                txtCreditCardNo.ReadOnly = false;
               // txtTechnician.ReadOnly = false;
                dtpDueDate.Enabled = true;
               // dtpRepDate.Enabled = true;
                //dtpRepDate
                
                btnConsultant.Enabled = true;
                btnNew.Enabled = true;
                btnEdit.Enabled = true;
                btnSave.Enabled = true;
                txtdisRate.ReadOnly = false;
               // btnScanNames.Enabled = false;
             
            }
            catch { }
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

        public void ClearText()
        {
            try
            {
               
                dtpDueDate.Text = "";
               // txtTechnician.Text = "";
                txtTokenNo.Text = "";
                dtpDate.Text = "";
                txtScanName.Text = "0";
                txtConsultant.Text = "";
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
        private void frmScan_Load(object sender, EventArgs e)
        {
            //try
            //{
            //    dtUser.Clear();
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmScan");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //        edit = Convert.ToBoolean(dtUser.Rows[0].ItemArray[4].ToString());
            //        delete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());
            //    }
            //}
            //catch { }

            try
            {
                dtpRepDate.Text = System.DateTime.Now.ToString().Trim();
                txtTime.Text = System.DateTime.Now.ToShortTimeString();

                ConsultanLOad();
                RoomDataload();

            }
            catch { }
        }

        public void ConsultanLOad()
        {
            string BlockC = "False";
            string ctype = "opd";
           // String S = "Select ConsultantName from tblConsultantMaster Where (Block='" + BlockC + "') AND (ConsultantType='" + ctype + "')";
            String S = "Select ConsultantName from tblConsultantMaster Where (Block='" + BlockC + "')";

            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                cmbCinsultant.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            }

        } 

        private void btnScanNames_Click(object sender, EventArgs e)
        {
            try
            {
              //  DataAccess.Access.type = "Scan";
                frmSelectItems frmS = new frmSelectItems();
                frmS.Show();
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
        public int c = 0;
        private void frmScan_Activated(object sender, EventArgs e)
        {
            //if (kk == 1)
            //{ }
            //else
            //{
                c = 1;
                if (A != 1)
                {
                    double ReceipTotal = 0.00;
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
                            // txtScanName.Text = a.GetText2();
                            //dgvScanItems.Rows.Add();
                            int x = dgvScanItems.Rows.Count - 1;
                            dgvScanItems.Rows.Add();


                            string ConnString = ConnectionString;
                            string sql = "Select * from tblScanItemMaster where ItemID='" + a.GetText2().ToString().Trim() + "'";
                            // where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (ItemID = '" + txtScanName.Text.ToString().Trim() + "')";
                            SqlConnection Conn = new SqlConnection(ConnString);
                            SqlCommand cmd = new SqlCommand(sql);
                            SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvScanItems.Columns[0].ReadOnly = true;
                            dgvScanItems.Columns[1].ReadOnly = true;
                            dgvScanItems.Columns[2].ReadOnly = true;
                            //dgvScanItems.Columns[4].ReadOnly = true;
                            dgvScanItems.Columns[5].ReadOnly = true;

                            // dgvScanItems[0, x].Value = a.GetText2();
                            if (dt.Rows.Count > 0)
                            {
                                dgvScanItems[0, x].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                                dgvScanItems[1, x].Value = dt.Rows[0].ItemArray[1].ToString().Trim();
                                dgvScanItems[2, x].Value = dt.Rows[0].ItemArray[3].ToString().Trim();
                                dgvScanItems[3, x].Value = "0";
                                dgvScanItems[4, x].Value = Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString().Trim());
                                dgvScanItems[5, x].Value = "0";
                                dgvScanItems[6, x].Value = dt.Rows[0].ItemArray[2].ToString().Trim();

                                // dgvScanItems[4, x].Value = dt.Rows[0].ItemArray[3].ToString().Trim();
                                //dgvScanItems[5, x].Value = dt.Rows[0].ItemArray[4].ToString().Trim();
                                // dgvScanItems[6, x].Value = Convert.ToDouble(dt.Rows[0].ItemArray[3].ToString().Trim()) + Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString().Trim());
                                //ReceiptTotal = ReceiptTotal + Convert.ToDouble (dgvScanItems[4, x].Value);

                                // txtTotal.Text =Convert.ToString(Convert.ToDouble (dgvScanItems[5, x].Value));

                                txtTotal.Text = "0.00";
                                txtdisRate_TextChanged(sender, e);
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
                            if (dgvScanItems.Rows[a].Cells[5].Value != null)
                            {
                                ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[5].Value);// sanjeewa change cell value 7 into 8
                            }
                        }

                        // Dotorfee 
                        txtTotal.Text = ReceipTotal.ToString("N2");
                        txtnetTotal.Text = txtTotal.Text.Trim();
                        txtScanName_Leave(sender, e);
                        //...................

                    }
                    catch { }
                }
           // }
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
               // if (edit)
               // {
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

              //  }
               // else
                //{
                 //   MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // }
            }
            catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //if (delete)
               // {
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
              //  }
              //  else
               // {
                //    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // }
            }
            catch { }
        }
        public int A = 0;
        private void btnSave_Click(object sender, EventArgs e)
        {
           // A = 1;

           //=============================================================

            if (CheckSearch != 2)
            {
                try
                {
                    //A = 1;
                    // if (dgvScanItems.CurrentCell.ColumnIndex == 2)
                    // {
                    for (int a = 0; a < dgvScanItems.Rows.Count - 1; a++)
                    {
                        if (dgvScanItems.Rows[a].Cells[3].Value != null && dgvScanItems.Rows[a].Cells[4].Value != null)
                        {
                            dgvScanItems.Rows[a].Cells[5].Value = Convert.ToDouble(dgvScanItems.Rows[a].Cells[3].Value) * Convert.ToDouble(dgvScanItems.Rows[a].Cells[4].Value);
                            // ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[5].Value);// sanjeewa change cell value 7 into 8
                        }
                    }
                    ReceipTotal1 = 0.00;
                    //===================================
                    for (int b = 0; b < dgvScanItems.Rows.Count - 1; b++)
                    {
                        if (dgvScanItems.Rows[b].Cells[5].Value != null)
                        {
                            ReceipTotal1 = ReceipTotal1 + Convert.ToDouble(dgvScanItems.Rows[b].Cells[5].Value);// sanjeewa change cell value 7 into 8
                        }
                    }

                    // Dotorfee 
                    txtTotal.Text = (ReceipTotal1 + Dotorfee + NightCharg).ToString("N2");
                    // txtTotal.Text = ReceipTotal.ToString("N2");
                    txtnetTotal.Text = txtTotal.Text.Trim();
                    txtdisRate_TextChanged(sender, e);

                    //=====================================

                    //  }
                }
                catch { }
            }


            //=====================================================

                try
                {
                    string ItemT = "Opd";
                    string ConnString = ConnectionString;
                    // string sql = "Select ReceiptsNo from tblChannelingDetails ORDER BY ReceiptsNo";
                    string sql = "Select ReceiptNo from tblOpdTransaction Where ItemType='" + ItemT + "' ORDER BY ReceiptNo";
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
                        
                        txtReceiptNo.Text = "OP-100000";
                      
                    }
                  
                }
                catch { }



                //...................................






            if ((txtFirstName.Text == "") || (txtReceiptNo.Text == "") || cmbPaymentMethod.Text == "" || txtTotal.Text == "0" || txtTotal.Text == "" || txtnetTotal.Text == "0" || txtnetTotal.Text == "" || txtnetTotal.Text=="0.00" )
           
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

                if (txtTotal.Text == "0")
                {
                    MessageBox.Show("Please enter at least one transaction");
                }
                else
                {
                    MessageBox.Show("Please enter at least one transaction");
                }
               
            }
            else{
                A = 1;
                                //............................

                try
                {
                    if (flg == 2)
                    {
                        for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                        {
                            string ItemType = "Opd";
                            string IsExport = "False";
                            string ConnString2 = ConnectionString;
                            String S2 = "insert into tblOpdTransaction(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal,PatientNo,UrgentFee,ItemCategory,Room) values ('" + txtReceiptNo.Text + "','" + txtTime.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value + "', '" + cmbCinsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtAddress1.Text.ToString().Trim()  + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "','" + txtScanName.Text.ToString().Trim() + "','" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dtpDueDate.Text.ToString().Trim() + "','" + txtTotal.Text.ToString().Trim() + "','" + ItemType + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" + txtPatientNo.Text.ToString().Trim() + "','" + txtAge.Text.ToString().Trim() + "','" + dgvScanItems[6, i].Value + "','" + cmbroom.Text.ToString().Trim() + "')";
                            SqlCommand cmd2 = new SqlCommand(S2);
                            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString2);
                            DataSet ds2 = new DataSet();
                            da2.Fill(ds2);
                        }
                       // MessageBox.Show("Saved Successfully");
                        flg = 0;
                        //abc1.Import_Receipt_Journal();
                        if (rbtnetuopd.Checked == true || rbtncash.Checked == true)
                        {
                            exporetReceipt();
                        }
                        else 
                        {
                            exporetSalesInvoice();
                        }
                       // btnReprintScan_Click (sender, e);

                        if (rbtnbht.Checked == true)
                        {
                            btnReprintScan_Click(sender,e);
                        }
                        else
                        {
                            btnPrintCus_Click(sender, e);
                        }
                       // btnPrint_Click(sender, e);
                        //..........................

                    }
                    else
                    {
                     
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
           // btnNew_Click(sender, e);
           
        }

        //=====================================================================
        public void exporetReceipt()
        {
            //Create a Xmal File..................................................................................

            try
            {
                //Receipts2.xml
            XmlTextWriter Writer = new XmlTextWriter(@"c:\\pbss\\Receipts2.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);
            double  discountRate = Convert.ToDouble(txtdisRate.Text)/100;

            for (int i = 0; i <= dgvScanItems.Rows.Count-2; i++)
            {
                Writer.WriteStartElement("PAW_Receipt");
                Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                if (rbtncash.Checked)
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(txtPatientNo.Text.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                               
                Writer.WriteStartElement("Reference");
                Writer.WriteString(txtPatientNo.Text.ToString().Trim() + "R");
                Writer.WriteEndElement();

                          
                Writer.WriteStartElement("Date ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(dtpRepDate.Text.ToString().Trim());//Date 
                Writer.WriteEndElement();


                Writer.WriteStartElement("Payment_Method");
                Writer.WriteString(cmbPaymentMethod.Text.ToString().Trim());//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("Cash_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString("70010");//Cash Account
                Writer.WriteEndElement();

             

                Writer.WriteStartElement("Number_of_Distributions ");
                Writer.WriteString(NoDistributions);
                Writer.WriteEndElement();


                Writer.WriteStartElement("InvoicePaid");
                Writer.WriteString("");//PayMethod
                Writer.WriteEndElement();

               
                Writer.WriteStartElement("Quantity");
                Writer.WriteString(dgvScanItems[3, i].Value.ToString().Trim());//Doctor Charge
                Writer.WriteEndElement();

                             
                Writer.WriteStartElement("Item_ID");
                Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
                Writer.WriteEndElement();


                //========================================================
                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");//Doctor Charge
                Writer.WriteEndElement();

                double Amount = Convert.ToDouble(dgvScanItems[5, i].Value);
                double DiscountAmount = Amount * discountRate;
                double ItemAmount = Amount - DiscountAmount;

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + ItemAmount.ToString().Trim());//HospitalCharge
                Writer.WriteEndElement();


                Writer.WriteStartElement("Transaction_Number");
                Writer.WriteString((i + 1).ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("ReceiptNumber");
                Writer.WriteString(txtReceiptNo.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteEndElement();
               // Writer.Close();
            }

                 Writer.Close();

                Connector abc = new Connector();//export to peach tree
                abc.Import_Receipt_JournalOnline();
            }

            catch { }

          
        }

        //====================================================================


        //import invoice to BHT Ptient============================================

        public void exporetSalesInvoice()
        {
            //Create a Xmal File..................................................................................

            try
            {
                //Receipts2.xml
                XmlTextWriter Writer = new XmlTextWriter(@"c:\\pbss\\SalesInvice.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);
                double discountRate = Convert.ToDouble(txtdisRate.Text) / 100;

                for (int i = 0; i <= dgvScanItems.Rows.Count - 2; i++)
                {
                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");


                    if (rbtncash.Checked)
                    {
                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                        Writer.WriteEndElement();
                    }
                    else
                    {
                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(txtPatientNo.Text.ToString().Trim());//Customer ID should be here = Ptient No
                        Writer.WriteEndElement();
                    }

                    Writer.WriteStartElement("Date");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dtpRepDate.Text.ToString().Trim());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(txtReceiptNo.Text.ToString().Trim());
                    Writer.WriteEndElement();


                  


                    //Writer.WriteStartElement("Payment_Method");
                    //Writer.WriteString(cmbPaymentMethod.Text.ToString().Trim());//PayMethod
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("61000");//Cash Account
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
                    Writer.WriteString(dgvScanItems[3, i].Value.ToString().Trim());//Doctor Charge
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

                    double Amount = Convert.ToDouble(dgvScanItems[5, i].Value);
                    double DiscountAmount = Amount * discountRate;
                    double ItemAmount = Amount - DiscountAmount;

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + ItemAmount.ToString().Trim());//HospitalCharge
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
                }

                Writer.Close();

                Connector abc = new Connector();//export to peach tree
                abc.ImportSalesInvice();//ImportSalesInvice()
            }

            catch { }


        }
        //======================================================================

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
                //    string ItemT = "Opd";
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
                //    //AppointmentNo = AppointmentNo + 1;

                //    //...............................
                //    String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //    SqlCommand cmd1 = new SqlCommand(S1);
                //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                //    DataTable dt = new DataTable();
                //    da1.Fill(dt);
                //    int NoOfTokens = 0;
                //    if (dt.Rows.Count > 0)
                //    {
                //        //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                //        // {
                //        // NoOfTokens = 1;
                //        // }
                //        //else
                //        //{
                //        NoOfTokens = dt.Rows.Count + 1;
                //        // }
                //    }
                //    else
                //    {
                //        NoOfTokens = 1;
                //    }
                //    txtTokenNo.Text = NoOfTokens.ToString();
                //    //......................................

                //}
                //catch { }
                //// }


                ////...................................
                //try
                //{


                //    String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //    // String S1 = "Select COUNT(TokenNo) from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                //    SqlCommand cmd1 = new SqlCommand(S1);
                //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                //    DataTable dt = new DataTable();
                //    da1.Fill(dt);
                //    int NoOfTokens = 0;
                //    if (dt.Rows.Count > 0)
                //    {
                //        //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                //        // {
                //        // NoOfTokens = 1;
                //        // }
                //        //else
                //        //{
                //        NoOfTokens = dt.Rows.Count + 1;
                //        // }
                //    }
                //    else
                //    {
                //        NoOfTokens = 1;
                //    }
                //    txtTokenNo.Text = NoOfTokens.ToString();
                //}
                //catch { }

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
            try
            {
                flgTech = 1;
              //  frmSelectConsult frm = new frmSelectConsult(this);
                //frm.Show();
            }
            catch { } 
        }

        private void btnTechnician_Click(object sender, EventArgs e)
        {
            try
            {
                flgTech = 3;
                //frmSelectConsult frm = new frmSelectConsult(this);
                //frm.Show();
            }
            catch { } 
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dgvScanItems_Leave(object sender, EventArgs e)
        {


            if (CheckSearch != 2)
            {
                try
                {
                    //A = 1;
                    // if (dgvScanItems.CurrentCell.ColumnIndex == 2)
                    // {
                    for (int a = 0; a < dgvScanItems.Rows.Count - 1; a++)
                    {
                        if (dgvScanItems.Rows[a].Cells[3].Value != null && dgvScanItems.Rows[a].Cells[4].Value != null)
                        {
                            dgvScanItems.Rows[a].Cells[5].Value = Convert.ToDouble(dgvScanItems.Rows[a].Cells[3].Value) * Convert.ToDouble(dgvScanItems.Rows[a].Cells[4].Value);
                            // ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[5].Value);// sanjeewa change cell value 7 into 8
                        }
                    }
                    ReceipTotal1 = 0.00;
                    //===================================
                    for (int b = 0; b < dgvScanItems.Rows.Count - 1; b++)
                    {
                        if (dgvScanItems.Rows[b].Cells[5].Value != null)
                        {
                            ReceipTotal1 = ReceipTotal1 + Convert.ToDouble(dgvScanItems.Rows[b].Cells[5].Value);// sanjeewa change cell value 7 into 8
                        }
                    }

                    // Dotorfee 
                    txtTotal.Text = (ReceipTotal1 + Dotorfee + NightCharg).ToString("N2");
                    // txtTotal.Text = ReceipTotal.ToString("N2");
                    txtnetTotal.Text = txtTotal.Text.Trim();
                    txtdisRate_TextChanged(sender, e);

                    //=====================================

                    //  }
                }
                catch { }
            }

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
            try
            {
                CheckSearch = 2;
                if (cmbScanSerch.Text == "Patient Name")
                {
                    string add = txtConsultantFind.Text;
                    string ItemType = "Opd";
                    if (add != "")
                    {
                        // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                        String S2 = "Select * from tblOpdTransaction where (PatientName LIKE  '" + add + "%')AND (ItemType='" + ItemType + "')";
                        // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt = new DataTable();
                        da2.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();
                            txtAge.Text  = dt.Rows[0].ItemArray[31].ToString().Trim();
                            cmbroom.Text = dt.Rows[0].ItemArray[33].ToString().Trim();

                            txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                            txtTime.Text  = dt.Rows[0].ItemArray[1].ToString().Trim();
                            dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

                            txtScanName.Text = dt.Rows[0].ItemArray[13].ToString().Trim();
                            cmbCinsultant.Text  = dt.Rows[0].ItemArray[4].ToString().Trim();
                            txtFirstName.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                            txtContactNo.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
                            txtAddress1.Text  = dt.Rows[0].ItemArray[7].ToString().Trim();


                            // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
                            // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                            txtTotal.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
                            dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();//Receipt Date
                            cmbPaymentMethod.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                            txtCreditCardNo.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
                            txtdisRate.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
                            txtDisAmount.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
                            txtnetTotal.Text = dt.Rows[0].ItemArray[25].ToString().Trim();

                            dgvScanItems.DataSource = dt;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                //  dgvScanItems.Rows.Add();
                                // dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[10].ToString();
                                dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
                                dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
                                dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();
                                dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();
                                dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[14].ToString();
                                dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[15].ToString();
                                //dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
                                //dgvScanItems.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();
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
                    string ItemType = "Opd";
                    if (add != "")
                    {
                        // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                        String S2 = "Select * from tblOpdTransaction where (ReceiptNo = '" + add + "')AND (ItemType='" + ItemType + "')";
                        // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt = new DataTable();
                        da2.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();
                            txtAge.Text = dt.Rows[0].ItemArray[31].ToString().Trim();
                            cmbroom.Text = dt.Rows[0].ItemArray[33].ToString().Trim();

                            txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                            txtTime.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                            dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

                            txtScanName.Text = dt.Rows[0].ItemArray[13].ToString().Trim();
                            cmbCinsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                            txtFirstName.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                            txtContactNo.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
                            txtAddress1.Text  = dt.Rows[0].ItemArray[7].ToString().Trim();


                            // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
                            // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                            txtTotal.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
                            dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();//Receipt Date
                            cmbPaymentMethod.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                            txtCreditCardNo.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
                            txtdisRate.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
                            txtDisAmount.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
                            txtnetTotal.Text = dt.Rows[0].ItemArray[25].ToString().Trim();



                            dgvScanItems.DataSource = dt;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                //  dgvScanItems.Rows.Add();
                                dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
                                dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
                                dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();

                                dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();

                                dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[14].ToString();
                                dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[15].ToString();
                                // dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
                                //dgvScanItems.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();
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
                    string ItemType = "Opd";
                    if (add != "")
                    {
                        // String S2 = "Select * from tblChannelingDetails where ReceiptsNo LIKE  '" + add + "%'";
                        String S2 = "Select * from tblOpdTransaction where (ContactNo LIKE  '" + add + "%')AND (ItemType='" + ItemType + "')";
                        // String S1 = "Select * from tblScanChannel where Consultant = '" + txtConsultantFind.Text.ToString().Trim() + "'";//) AND (Date = '" + dtpDateFind.Text.ToString().Trim() + "') AND (TokenNo = '" + txtTokenNoFind.Text.ToString().Trim() + "')";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt = new DataTable();
                        da2.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {

                            txtPatientNo.Text = dt.Rows[0].ItemArray[30].ToString().Trim();
                            txtAge.Text = dt.Rows[0].ItemArray[31].ToString().Trim();
                            cmbroom.Text = dt.Rows[0].ItemArray[33].ToString().Trim();

                            txtReceiptNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                            txtTime.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                            dtpDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();

                            txtScanName.Text = dt.Rows[0].ItemArray[13].ToString().Trim();
                            cmbCinsultant.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                            txtFirstName.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                            txtContactNo.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
                            txtAddress1.Text  = dt.Rows[0].ItemArray[7].ToString().Trim();


                            // txtScanFee.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
                            // txtHospitalFee.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                            txtTotal.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
                            dtpRepDate.Text = dt.Rows[0].ItemArray[20].ToString().Trim();//Receipt Date
                            cmbPaymentMethod.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                            txtCreditCardNo.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
                            txtdisRate.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
                            txtDisAmount.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
                            txtnetTotal.Text = dt.Rows[0].ItemArray[25].ToString().Trim();


                            dgvScanItems.DataSource = dt;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                //  dgvScanItems.Rows.Add();
                                dgvScanItems[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
                                dgvScanItems.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString();
                                dgvScanItems.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[11].ToString();

                                dgvScanItems.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[12].ToString();

                                dgvScanItems.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[14].ToString();
                                dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[15].ToString();
                                //dgvScanItems.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[14].ToString();
                                //dgvScanItems.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[15].ToString();
                            }

                        }
                    }
                    else
                    {
                        //MessageBox.Show("Receipt not found");
                    }
                }

                btnEdit.Enabled = false;
                btnRefund.Enabled = true;
                btnReprintScan.Enabled = true;
                btnPrintCus.Enabled = true;
                groupBox2.Enabled = true;
                // dgvScanItems.Enabled = false;
                // btnEdit.Enabled = true;


                //=====================

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

                //===============================
                // btnDelete.Enabled = true;
                //.....................
            }
            catch { }
        }

        private void btnReprintScan_Click(object sender, EventArgs e)
        {
            try
            {
                DsScanReport.Clear();

                string ItemType = "Opd";
                String S1 = "Select * from tblOpdTransaction where (ReceiptNo = '" + txtReceiptNo.Text.ToString().Trim() + "') AND (ItemType='" + ItemType + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DsScanReport, "dtScanData");

                OPDReport   frmX = new  OPDReport (this);
                frmX.Show();
                             
            }
            catch { }
        }

        public int kk = 0;
        private void Clear_Click(object sender, EventArgs e)
        {
            try
            {
                //kk = 1;
                  int N=dgvScanItems.Rows.Count-2;
                   dgvScanItems.Rows.RemoveAt(N);
              
            }
            catch {}
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string ReferenceNo = "Cash-" + txtReceiptNo.Text.ToString().Trim();
            string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);

            // for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            // {

            XmlTextWriter Writer = new XmlTextWriter(@"c:\Receipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            {

                //string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);
                //for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                //{
                if (i == 0)
                {
                    Writer.WriteStartElement("PAW_Receipt");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");
                }
                //this is for the first record which is describe the hospital charge
                Writer.WriteStartElement("ReceiptNumber");
                Writer.WriteString(txtReceiptNo.Text.ToString().Trim());//Receipts Number Should be here 
                Writer.WriteEndElement();
                //=======================================================================
                string CustomerName = txtFirstName.Text;
                string A = CustomerName.Substring(0, 1);
                // if(txtFirstName.Text==
                if (A == "a" || A == "A")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "b" || A == "B")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "c" || A == "C")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "d" || A == "D")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "e" || A == "E")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "f" || A == "F")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "g" || A == "G")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "h" || A == "H")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "i" || A == "I")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "j" || A == "J")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "k" || A == "K")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "l" || A == "L")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "m" || A == "M")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "n" || A == "N")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "o" || A == "O")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "p" || A == "P")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "q" || A == "Q")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "r" || A == "R")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "s" || A == "S")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "t" || A == "T")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "u" || A == "U")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "v" || A == "V")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "w" || A == "W")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "x" || A == "X")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "y" || A == "Y")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "z" || A == "Z")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                //====================================================================================
                //.......................................
                Writer.WriteStartElement("Date ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
                Writer.WriteString(dtpDate.Text.ToString().Trim());//Date 
                // 03/15/2007
                Writer.WriteEndElement();

                Writer.WriteStartElement("Reference");
                Writer.WriteString(ReferenceNo);//Reference
                Writer.WriteEndElement();

                Writer.WriteStartElement("Cash_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString("70010");
                // Writer.WriteString("70010");//Cash Account//10200-00
                Writer.WriteEndElement();

                Writer.WriteStartElement("Payment_Method");
                Writer.WriteString("Cash");//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("InvoicePaid");
                Writer.WriteString("");//PayMethod
                Writer.WriteEndElement();

                //......................................................
                if (dgvScanItems.Rows.Count == 2)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("2");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 3)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("4");
                    Writer.WriteEndElement();
                }

                if (dgvScanItems.Rows.Count == 4)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("6");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 5)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("8");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 6)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("10");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 7)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("12");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 8)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("14");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 9)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("16");
                    Writer.WriteEndElement();
                }

                //........................................................

                Writer.WriteStartElement("Transaction_Number");
                Writer.WriteString(Convert.ToString(i + 1));
                Writer.WriteEndElement();
                //.....................................
                Writer.WriteStartElement("ItemID");
                Writer.WriteString("");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString("Doctor Charges");
                Writer.WriteEndElement();


                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + dgvScanItems[4, i].Value.ToString().Trim());//Doctor Charge
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString("76006");//Doctor payable Account 
                // Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());//Doctor payable Account 
                Writer.WriteEndElement();

                //.......................................................................


                //Writer.WriteStartElement("ItemID");
                //Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("Description");
                //Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("Amount");
                //Writer.WriteString(dgvScanItems[6, i].Value.ToString().Trim());//HospitalCharge
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("GL_Account ");
                //Writer.WriteAttributeString("xsi:type", "paw:id");
                //Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
                //Writer.WriteEndElement();

                //Writer.WriteEndElement();
                //.............................................................................................
                //this is for a Scanning account
                // Writer.WriteStartElement("PAW_Receipt");001
                //Writer.WriteAttributeString("xsi:type", "paw:Receipt");001

                //this is for the first record which is describe the hospital charge
                Writer.WriteStartElement("ReceiptNumber");
                Writer.WriteString(txtReceiptNo.Text.ToString().Trim());//Receipts Number Should be here 
                Writer.WriteEndElement();
                //=================================================
                // string CustomerName = txtFirstName.Text;
                //string A = CustomerName.Substring(0, 1);
                // if(txtFirstName.Text==
                if (A == "a" || A == "A")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "b" || A == "B")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "c" || A == "C")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "d" || A == "D")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "e" || A == "E")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "f" || A == "F")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "g" || A == "G")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "h" || A == "H")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "i" || A == "I")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "j" || A == "J")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "k" || A == "K")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "l" || A == "L")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "m" || A == "M")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "n" || A == "N")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "o" || A == "O")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "p" || A == "P")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "q" || A == "Q")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "r" || A == "R")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "s" || A == "S")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "t" || A == "T")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "u" || A == "U")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "v" || A == "V")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "w" || A == "W")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "x" || A == "X")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "y" || A == "Y")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "z" || A == "Z")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                //===================================================================
                //.......................................
                Writer.WriteStartElement("Date ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
                Writer.WriteString(dtpDate.Text.ToString().Trim());//Date 
                // 03/15/2007
                Writer.WriteEndElement();

                Writer.WriteStartElement("Reference");
                Writer.WriteString(ReferenceNo);//Reference
                Writer.WriteEndElement();

                Writer.WriteStartElement("Cash_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                // Writer.WriteString("70010");//Cash Account
                Writer.WriteString("70010");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Payment_Method");
                Writer.WriteString("Cash");//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("InvoicePaid");
                Writer.WriteString("");//PayMethod
                Writer.WriteEndElement();
                //....................
                //Writer.WriteStartElement("Number_of_Distributions ");
                //Writer.WriteString(Convert.ToString(dgvScanItems.Rows.Count - 1));
                //Writer.WriteEndElement();
                if (dgvScanItems.Rows.Count == 2)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("2");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 3)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("4");
                    Writer.WriteEndElement();
                }

                if (dgvScanItems.Rows.Count == 4)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("6");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 5)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("8");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 6)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("10");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 7)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("12");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 8)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("14");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 9)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("16");
                    Writer.WriteEndElement();
                }


                Writer.WriteStartElement("Transaction_Number");
                Writer.WriteString(Convert.ToString(i + 2));
                Writer.WriteEndElement();



              


                Writer.WriteStartElement("ItemID");
                Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + dgvScanItems[5, i].Value.ToString().Trim());//HospitalCharge
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                // Writer.WriteString("40000-00");
                Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
                Writer.WriteEndElement();

                //Writer.WriteEndElement();
                // Writer.Close();       
                //....................................................................................................
            }
            Writer.WriteEndElement();
            Writer.Close();

        }

        private void button1_Click_2(object sender, EventArgs e)
        {

            string ReferenceNo = "Cash-" + txtReceiptNo.Text.ToString().Trim();
            string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);

            // for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            // {

            XmlTextWriter Writer = new XmlTextWriter(@"c:\Receipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
            {

                //string NoDistributions = Convert.ToString(dgvScanItems.Rows.Count - 1);
                //for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                //{
                if (i == 0)
                {
                    Writer.WriteStartElement("PAW_Receipt");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");
                }
                //this is for the first record which is describe the hospital charge
                Writer.WriteStartElement("ReceiptNumber");
                Writer.WriteString(txtReceiptNo.Text.ToString().Trim());//Receipts Number Should be here 
                Writer.WriteEndElement();
                //=======================================================================
                string CustomerName = txtFirstName.Text;
                string A = CustomerName.Substring(0, 1);
                // if(txtFirstName.Text==
                if (A == "a" || A == "A")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "b" || A == "B")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "c" || A == "C")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "d" || A == "D")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "e" || A == "E")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "f" || A == "F")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "g" || A == "G")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "h" || A == "H")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "i" || A == "I")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "j" || A == "J")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "k" || A == "K")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "l" || A == "L")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "m" || A == "M")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "n" || A == "N")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "o" || A == "O")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "p" || A == "P")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "q" || A == "Q")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "r" || A == "R")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "s" || A == "S")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "t" || A == "T")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "u" || A == "U")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "v" || A == "V")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "w" || A == "W")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "x" || A == "X")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "y" || A == "Y")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "z" || A == "Z")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                //====================================================================================
                //.......................................
                Writer.WriteStartElement("Date ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
                Writer.WriteString(dtpDate.Text.ToString().Trim());//Date 
                // 03/15/2007
                Writer.WriteEndElement();

                Writer.WriteStartElement("Reference");
                Writer.WriteString(ReferenceNo);//Reference
                Writer.WriteEndElement();

                Writer.WriteStartElement("Cash_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString("70010");
                // Writer.WriteString("70010");//Cash Account//10200-00
                Writer.WriteEndElement();

                Writer.WriteStartElement("Payment_Method");
                Writer.WriteString("Cash");//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("InvoicePaid");
                Writer.WriteString("");//PayMethod
                Writer.WriteEndElement();

                //......................................................
                if (dgvScanItems.Rows.Count == 2)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("2");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 3)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("4");
                    Writer.WriteEndElement();
                }

                if (dgvScanItems.Rows.Count == 4)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("6");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 5)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("8");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 6)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("10");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 7)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("12");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 8)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("14");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 9)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("16");
                    Writer.WriteEndElement();
                }

                //........................................................

                Writer.WriteStartElement("Transaction_Number");
                Writer.WriteString(Convert.ToString(i + 1));
                Writer.WriteEndElement();
                //.....................................
                Writer.WriteStartElement("ItemID");
                Writer.WriteString("");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString("Doctor Charges");
                Writer.WriteEndElement();


                Writer.WriteStartElement("Amount");
                Writer.WriteString(dgvScanItems[4, i].Value.ToString().Trim());//Doctor Charge
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString("76006");//Doctor payable Account 
                // Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());//Doctor payable Account 
                Writer.WriteEndElement();

                //.......................................................................


                //Writer.WriteStartElement("ItemID");
                //Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("Description");
                //Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("Amount");
                //Writer.WriteString(dgvScanItems[6, i].Value.ToString().Trim());//HospitalCharge
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("GL_Account ");
                //Writer.WriteAttributeString("xsi:type", "paw:id");
                //Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
                //Writer.WriteEndElement();

                //Writer.WriteEndElement();
                //.............................................................................................
                //this is for a Scanning account
                // Writer.WriteStartElement("PAW_Receipt");001
                //Writer.WriteAttributeString("xsi:type", "paw:Receipt");001

                //this is for the first record which is describe the hospital charge
                Writer.WriteStartElement("ReceiptNumber");
                Writer.WriteString(txtReceiptNo.Text.ToString().Trim());//Receipts Number Should be here 
                Writer.WriteEndElement();
                //=================================================
                // string CustomerName = txtFirstName.Text;
                //string A = CustomerName.Substring(0, 1);
                // if(txtFirstName.Text==
                if (A == "a" || A == "A")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "b" || A == "B")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "c" || A == "C")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "d" || A == "D")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "e" || A == "E")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "f" || A == "F")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "g" || A == "G")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "h" || A == "H")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "i" || A == "I")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "j" || A == "J")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "k" || A == "K")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "l" || A == "L")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "m" || A == "M")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "n" || A == "N")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "o" || A == "O")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "p" || A == "P")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "q" || A == "Q")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "r" || A == "R")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "s" || A == "S")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "t" || A == "T")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "u" || A == "U")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "v" || A == "V")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "w" || A == "W")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "x" || A == "X")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "y" || A == "Y")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else if (A == "z" || A == "Z")
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("C0001");//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();
                }
                //===================================================================
                //.......................................
                Writer.WriteStartElement("Date ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
                Writer.WriteString(dtpDate.Text.ToString().Trim());//Date 
                // 03/15/2007
                Writer.WriteEndElement();

                Writer.WriteStartElement("Reference");
                Writer.WriteString(ReferenceNo);//Reference
                Writer.WriteEndElement();

                Writer.WriteStartElement("Cash_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                // Writer.WriteString("70010");//Cash Account
                Writer.WriteString("70010");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Payment_Method");
                Writer.WriteString("Cash");//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("InvoicePaid");
                Writer.WriteString("");//PayMethod
                Writer.WriteEndElement();
                //....................
                //Writer.WriteStartElement("Number_of_Distributions ");
                //Writer.WriteString(Convert.ToString(dgvScanItems.Rows.Count - 1));
                //Writer.WriteEndElement();
                if (dgvScanItems.Rows.Count == 2)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("2");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 3)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("4");
                    Writer.WriteEndElement();
                }

                if (dgvScanItems.Rows.Count == 4)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("6");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 5)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("8");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 6)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("10");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 7)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("12");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 8)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("14");
                    Writer.WriteEndElement();
                }
                if (dgvScanItems.Rows.Count == 9)
                {
                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("16");
                    Writer.WriteEndElement();
                }


                Writer.WriteStartElement("Transaction_Number");
                Writer.WriteString(Convert.ToString(i + 2));
                Writer.WriteEndElement();



                //...........................



                //Writer.WriteStartElement("Number_of_Distributions ");
                //Writer.WriteString("4");
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("Transaction_Number");
                //Writer.WriteString("2");
                //Writer.WriteEndElement();
                ////.....................................

                //Writer.WriteStartElement("Description");
                //Writer.WriteString("Doctor Charges");
                //Writer.WriteEndElement();


                //Writer.WriteStartElement("Amount");
                //Writer.WriteString("-" + dgvScanItems[4, i].Value.ToString().Trim());//Doctor Charge
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("GL_Account ");
                //Writer.WriteAttributeString("xsi:type", "paw:id");
                //Writer.WriteString("76005");//Doctor payable Account 
                //Writer.WriteEndElement();

                //.......................................................................


                Writer.WriteStartElement("ItemID");
                Writer.WriteString(dgvScanItems[0, i].Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(dgvScanItems[1, i].Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString(dgvScanItems[5, i].Value.ToString().Trim());//HospitalCharge
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                // Writer.WriteString("40000-00");
                Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());
                Writer.WriteEndElement();

                //Writer.WriteEndElement();
                // Writer.Close();       
                //....................................................................................................
            }
            Writer.WriteEndElement();
            Writer.Close();

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
                   string  Ref = "Refund";
                   string IsExport = "True";
                                      
                    for (int i = 0; i < dgvScanItems.Rows.Count - 1; i++)
                    {
                        string ItemType = "Scan";
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
                        string ItemType = "Scan";
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
                        string ItemType = "Scan";
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
                        string ItemType = "Scan";
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
                //        int NoOfTokens = 0;
                //        if (dt.Rows.Count > 0)
                //        {
                //            //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                //            // {
                //            // NoOfTokens = 1;
                //            // }
                //            //else
                //            //{
                //            NoOfTokens = dt.Rows.Count + 1;
                //            // }
                //        }
                //        else
                //        {
                //            NoOfTokens = 1;
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
                //        int NoOfTokens = 0;
                //        if (dt.Rows.Count > 0)
                //        {
                //            //if (dt.Rows[0].ItemArray[0].ToString() == "0")
                //            // {
                //            // NoOfTokens = 1;
                //            // }
                //            //else
                //            //{
                //            NoOfTokens = dt.Rows.Count + 1;
                //            // }
                //        }
                //        else
                //        {
                //            NoOfTokens = 1;
                //        }
                //        txtTokenNo.Text = NoOfTokens.ToString();
                //    }
                //    catch { }


                //}
            }
        }

        private void txtTokenNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtScanName_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    double grosstotal = 0.00;
            //    double Dotorfee = 0.00;
            //    grosstotal = Convert.ToDouble(txtTotal.Text);
            //    Dotorfee = Convert.ToDouble(txtScanName.Text);

            //    txtTotal.Text = (Dotorfee + grosstotal).ToString("N2");
            //}
            //catch { }
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
                    dgvScanRefundSave.DataSource=dt;
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

        private void rbtnDoctorfee_CheckedChanged(object sender, EventArgs e)
        {
           // frmScan_Activated(sender,e);
            getScanData();
            try
            {
                for (int i = 0; i < dgvScanItems.Rows.Count-1; i++)
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
                    txtScanName.Text = "0";

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
            txtdisRate_TextChanged(sender,e);
        }

        private void txtdisRate_TextChanged(object sender, EventArgs e)
        {
            double Rate = 0.00;
            double DiscountAmount = 0.00;
            double totBefoDiscount = 0.00;
            double NetTotal = 0.00;

            // if (once == 1)
            // {
            try
            {

                totBefoDiscount = Convert.ToDouble(txtTotal.Text.Trim());

            }
            catch {}
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

        private void txtPatientNo_TextChanged(object sender, EventArgs e)
        {
            if (rbtnetuopd.Checked==true)
            {
                if (CheckSearch != 2)
                {
                    try
                    {
                        //string cusType = "";
                        string add = txtPatientNo.Text;
                        if (add != "")
                        {
                            //tblETUOPD
                           // String S2 = "Select * from tblPatientsDetails where PatientNo LIKE  '" + add + "%'";
                            String S2 = "Select * from tblETUOPD where PatientNo LIKE  '" + add + "%'";
                            SqlCommand cmd2 = new SqlCommand(S2);
                            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);

                            if (dt2.Rows.Count > 0)
                            {

                                dtpRepDate.Value  = Convert.ToDateTime(dt2.Rows[0].ItemArray[0].ToString());
                                txtTime.Text = dt2.Rows[0].ItemArray[1].ToString();
                                txtFirstName.Text = dt2.Rows[0].ItemArray[4].ToString();
                                txtContactNo.Text = dt2.Rows[0].ItemArray[7].ToString();
                                txtAddress1.Text = dt2.Rows[0].ItemArray[6].ToString();
                                txtAge.Text = dt2.Rows[0].ItemArray[5].ToString();
                                cmbCinsultant.Text = dt2.Rows[0].ItemArray[8].ToString();
                                cmbroom.Text = dt2.Rows[0].ItemArray[9].ToString();

                                                          

                            }
                            else
                            {
                                txtPatientNo.Text = "";
                                txtFirstName.Text = "";
                                txtContactNo.Text = "";
                                dtpRepDate.Text= "";
                                txtTime.Text = "";
                                txtFirstName.Text = "";
                                //txtFirstName.Enabled = true;
                               // txtFirstName.ReadOnly = false;
                                txtContactNo.Text = "";
                                txtAddress1.Text = "";
                                txtAge.Text = "";
                                cmbCinsultant.Text = "";
                                cmbroom.Text = "";

                            }
                        }
                    }
                    catch { }
                }

                txtFirstName.Enabled = true;
                txtFirstName.ReadOnly = false;
                txtAddress1.Enabled = true;
                txtAddress1.ReadOnly = false;
                
            }

            else if (rbtnbht.Checked == true)
            {
                try
                {
                    //string cusType = "";
                    string add = txtPatientNo.Text;
                    if (add != "")
                    {

                        String S2 = "Select * from tblItemMaster2 where PatientNo LIKE  '" + add + "%'";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);

                        if (dt2.Rows.Count > 0)
                        {

                            dtpRepDate.Value = Convert.ToDateTime(dt2.Rows[0].ItemArray[0].ToString());
                            txtTime.Text = dt2.Rows[0].ItemArray[1].ToString();
                            txtFirstName.Text = dt2.Rows[0].ItemArray[3].ToString();
                            txtContactNo.Text = dt2.Rows[0].ItemArray[7].ToString();
                            txtAddress1.Text = dt2.Rows[0].ItemArray[5].ToString();
                            txtAge.Text = dt2.Rows[0].ItemArray[4].ToString();
                            cmbCinsultant.Text = dt2.Rows[0].ItemArray[8].ToString();
                            cmbroom.Text = dt2.Rows[0].ItemArray[9].ToString();
                           
                        }
                        else
                        {
                            txtPatientNo.Text = "";
                            txtFirstName.Text = "";
                            txtContactNo.Text = "";
                            dtpRepDate.Text = "";
                            txtTime.Text = "";
                            txtFirstName.Text = "";
                            txtContactNo.Text = "";
                            txtAddress1.Text = "";
                            txtAge.Text = "";
                            cmbCinsultant.Text = "";
                            cmbroom.Text = "";
                          
                        }
                    }
                }
                catch { }
            }
            else{}
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            if (rbtncash.Checked==true)
            {
                try
                {
                    if (txtPatientNo.Text == "")
                    {
                        string Code = txtFirstName.Text.ToString().Trim();
                        string A = Code.Substring(5, 1);
                        string B = A.ToUpper();
                        //  string ab = "C" + A + "0001";
                        // txtPatientNo 
                        txtPatientNo.Text = "C" + B + "0001";
                        txtFirstName.Text = Code;
                    }
                }
                catch { }
            }
          
        }
        public int k = 0;
        private void txtFirstName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (rbtnetuopd.Checked==false && rbtnbht.Checked==false)
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
        public  double Dotorfee = 0.00;
        private void txtScanName_Leave(object sender, EventArgs e)
        {
            try
            {
               // double grosstotal = 0.00;
               // double Dotorfee = 0.00;
               // grosstotal = Convert.ToDouble(txtTotal.Text);
                Dotorfee = Convert.ToDouble(txtScanName.Text);

               // txtTotal.Text = (Dotorfee + grosstotal).ToString("N2");
            }
            catch { }
        }

        public double ReceipTotal1 = 0.00;
        private void dgvScanItems_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (CheckSearch != 2)
            {
                try
                {
                    //A = 1;
                    // if (dgvScanItems.CurrentCell.ColumnIndex == 2)
                    // {
                    for (int a = 0; a < dgvScanItems.Rows.Count - 1; a++)
                    {
                        if (dgvScanItems.Rows[a].Cells[3].Value != null && dgvScanItems.Rows[a].Cells[4].Value != null)
                        {
                            dgvScanItems.Rows[a].Cells[5].Value = Convert.ToDouble(dgvScanItems.Rows[a].Cells[3].Value) * Convert.ToDouble(dgvScanItems.Rows[a].Cells[4].Value);
                            // ReceipTotal = ReceipTotal + Convert.ToDouble(dgvScanItems.Rows[a].Cells[5].Value);// sanjeewa change cell value 7 into 8
                        }
                    }
                    ReceipTotal1 = 0.00;
                    //===================================
                    for (int b = 0; b < dgvScanItems.Rows.Count - 1; b++)
                    {
                        if (dgvScanItems.Rows[b].Cells[5].Value != null)
                        {
                            ReceipTotal1 = ReceipTotal1 + Convert.ToDouble(dgvScanItems.Rows[b].Cells[5].Value);// sanjeewa change cell value 7 into 8
                        }
                    }

                    // Dotorfee 
                    txtTotal.Text = (ReceipTotal1 + Dotorfee + NightCharg).ToString("N2");
                    // txtTotal.Text = ReceipTotal.ToString("N2");
                    txtnetTotal.Text = txtTotal.Text.Trim();
                    txtdisRate_TextChanged(sender, e);

                    //=====================================

                    //  }
                }
                catch { }
            }
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

        private void txtTotal_TextChanged(object sender, EventArgs e)
        {

        }
        public double NightCharg = 0.00;
        private void txtNightCharge_Leave(object sender, EventArgs e)
        {
            try
            {

                NightCharg = Convert.ToDouble(txtNightCharge.Text);

               
            }
            catch { }
        }

        private void txtNightCharge_TextChanged(object sender, EventArgs e)
        {
           
          string ab=txtNightCharge.Text;
        // txtNightCharge.PasswordChar("*");
          for(int i=0;i<ab.Length ; i++)
          {
             // ab.Substring(0,i);
              if (char.IsNumber(ab, i) || char.IsPunctuation(ab, i)== true)
              {

              }
              else 
              {
                 // MessageBox.Show("Enter Number");
                  txtNightCharge.Text = "";
              }
          }
         // char.
        }

        private void cbxInpatient_CheckedChanged_1(object sender, EventArgs e)
        {
            if (cbxInpatient.Checked == true)
            {

                dtpRepDate.Enabled = false;
                txtTime.Enabled = false;
                txtFirstName.Enabled = false;
                txtContactNo.Enabled = false;
                txtAddress1.Enabled = false;
                txtAge.Enabled = false;
                cmbCinsultant.Enabled = false;
                cmbroom.Enabled = false;

            }
            else
            {
                dtpRepDate.Enabled = true;
                txtTime.Enabled = true;
                txtFirstName.Enabled = true;
                txtContactNo.Enabled = true;
                txtAddress1.Enabled = true;
                txtAge.Enabled = true;
                cmbCinsultant.Enabled = true;
                cmbroom.Enabled = true;
            }
        }

        private void dtpRepDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnPrintCus_Click(object sender, EventArgs e)
        {
                try
                {
                    DsScanReport.Clear();

                    string ItemType = "Opd";
                    String S1 = "Select * from tblOpdTransaction where (ReceiptNo = '" + txtReceiptNo.Text.ToString().Trim() + "') AND (ItemType='" + ItemType + "')";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(DsScanReport, "dtScanData");

                    frmOPDCus frmocus = new frmOPDCus(this);
                    frmocus.Show();

                   // OPDReport frmX = new OPDReport(this);
                   // frmX.Show();
                }
                catch { }
        }

        private void txtAddress1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtReceiptNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPatientNo_Leave(object sender, EventArgs e)
        {
            try
            {
                string ab = txtPatientNo.Text;
                string bc = ab.ToUpper();
                txtPatientNo.Text = bc;
            }
            catch { }

        }
        public int yy = 0;
        private void rbtnetuopd_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnetuopd.Checked == true)
            {
               //yy = 1;
                txtPatientNo.ReadOnly = false;
                txtPatientNo.Text = "";
               
                dtpRepDate.Enabled = false;
                txtTime.Enabled = false;
                txtFirstName.Enabled = false;
                txtContactNo.Enabled = false;
                txtAddress1.Enabled = false;
                txtAge.Enabled = false;
                cmbCinsultant.Enabled = false;
                cmbroom.Enabled = false;
                txtPatientNo.Focus();

            }
            else
            {
                dtpRepDate.Enabled = true;
                txtTime.Enabled = true;
                txtFirstName.Enabled = true;
                txtContactNo.Enabled = true;
                txtAddress1.Enabled = true;
                txtAge.Enabled = true;
                cmbCinsultant.Enabled = true;
                cmbroom.Enabled = true;
            }
        }

        private void rbtnbht_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnbht.Checked == true)
            {

                txtPatientNo.ReadOnly = false;
                txtPatientNo.Text = "";


                dtpRepDate.Enabled = false;
                txtTime.Enabled = false;
                txtFirstName.Enabled = false;
                txtContactNo.Enabled = false;
                txtAddress1.Enabled = false;
                txtAge.Enabled = false;
                cmbCinsultant.Enabled = false;
                cmbroom.Enabled = false;
                txtPatientNo.Focus();

            }
            else
            {
                dtpRepDate.Enabled = true;
                txtTime.Enabled = true;
                txtFirstName.Enabled = true;
                txtContactNo.Enabled = true;
                txtAddress1.Enabled = true;
                txtAge.Enabled = true;
                cmbCinsultant.Enabled = true;
                cmbroom.Enabled = true;
            }
        }

        private void rbtncash_CheckedChanged(object sender, EventArgs e)
        {
            txtPatientNo.Text = "";
            txtFirstName.Text = "";
            txtContactNo.Text = "";
            txtPatientNo.ReadOnly = true;
            txtFirstName.Focus();
        }

        private void txtSelectItem_Enter(object sender, EventArgs e)
        {
            //// txtScanName.Text = a.GetText2();
            ////dgvScanItems.Rows.Add();
            //int x = dgvScanItems.Rows.Count - 1;
            //try
            //{
            //    A = 1;
            //    dgvScanItems.Rows.Add();


            //    string ConnString = ConnectionString;
            //    string sql = "Select * from tblScanItemMaster where ItemID='" + txtSelectItem.Text.ToString().Trim() + "'";
            //    // where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (ItemID = '" + txtScanName.Text.ToString().Trim() + "')";
            //    SqlConnection Conn = new SqlConnection(ConnString);
            //    SqlCommand cmd = new SqlCommand(sql);
            //    SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
            //    DataTable dt = new DataTable();
            //    adapter.Fill(dt);
            //    dgvScanItems.Columns[0].ReadOnly = true;
            //    dgvScanItems.Columns[1].ReadOnly = true;
            //    dgvScanItems.Columns[2].ReadOnly = true;
            //    //dgvScanItems.Columns[4].ReadOnly = true;
            //    dgvScanItems.Columns[5].ReadOnly = true;

            //    // dgvScanItems[0, x].Value = a.GetText2();
            //    if (dt.Rows.Count > 0)
            //    {
            //        dgvScanItems[0, x].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
            //        dgvScanItems[1, x].Value = dt.Rows[0].ItemArray[1].ToString().Trim();
            //        dgvScanItems[2, x].Value = dt.Rows[0].ItemArray[3].ToString().Trim();
            //        dgvScanItems[3, x].Value = "0";
            //        dgvScanItems[4, x].Value = Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString().Trim());
            //        dgvScanItems[5, x].Value = "0";
            //        dgvScanItems[6, x].Value = dt.Rows[0].ItemArray[2].ToString().Trim();

            //        // dgvScanItems[4, x].Value = dt.Rows[0].ItemArray[3].ToString().Trim();
            //        //dgvScanItems[5, x].Value = dt.Rows[0].ItemArray[4].ToString().Trim();
            //        // dgvScanItems[6, x].Value = Convert.ToDouble(dt.Rows[0].ItemArray[3].ToString().Trim()) + Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString().Trim());
            //        //ReceiptTotal = ReceiptTotal + Convert.ToDouble (dgvScanItems[4, x].Value);

            //        // txtTotal.Text =Convert.ToString(Convert.ToDouble (dgvScanItems[5, x].Value));

            //        txtTotal.Text = "0.00";
            //        txtdisRate_TextChanged(sender, e);
            //    }
            //}
            //catch { }
            //ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems[4, x].Value);
            // ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems.Columns[4].ToString());
            // txtTotal.Text = Convert.ToString(ReceiptTotal);
           // Class1.flg2 = 0;
        }

        private void txtSelectItem_Leave(object sender, EventArgs e)
        {
            //if (yy == 1)
            //{ }
            //else 
            //{
            //    // txtScanName.Text = a.GetText2();
                //dgvScanItems.Rows.Add();
                int x = dgvScanItems.Rows.Count - 1;
                try
                {
                    A = 0;
                    dgvScanItems.Rows.Add();

                    string ItemID = txtSelectItem.Text.ToString().Trim() + "-2OP";

                    string ConnString = ConnectionString;
                    string sql = "Select * from tblScanItemMaster where ItemID = '" + ItemID + "'";//LIKE  '" + add + "%'
                    // where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (ItemID = '" + txtScanName.Text.ToString().Trim() + "')";
                    SqlConnection Conn = new SqlConnection(ConnString);
                    SqlCommand cmd = new SqlCommand(sql);
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvScanItems.Columns[0].ReadOnly = true;
                    dgvScanItems.Columns[1].ReadOnly = true;
                    dgvScanItems.Columns[2].ReadOnly = true;
                    //dgvScanItems.Columns[4].ReadOnly = true;
                    dgvScanItems.Columns[5].ReadOnly = true;

                    // dgvScanItems[0, x].Value = a.GetText2();
                    if (dt.Rows.Count > 0)
                    {
                        dgvScanItems[0, x].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                        dgvScanItems[1, x].Value = dt.Rows[0].ItemArray[1].ToString().Trim();
                        dgvScanItems[2, x].Value = dt.Rows[0].ItemArray[3].ToString().Trim();
                        dgvScanItems[3, x].Value = "0";
                        dgvScanItems[4, x].Value = Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString().Trim());
                        dgvScanItems[5, x].Value = "0";
                        dgvScanItems[6, x].Value = dt.Rows[0].ItemArray[2].ToString().Trim();

                        // dgvScanItems[4, x].Value = dt.Rows[0].ItemArray[3].ToString().Trim();
                        //dgvScanItems[5, x].Value = dt.Rows[0].ItemArray[4].ToString().Trim();
                        // dgvScanItems[6, x].Value = Convert.ToDouble(dt.Rows[0].ItemArray[3].ToString().Trim()) + Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString().Trim());
                        //ReceiptTotal = ReceiptTotal + Convert.ToDouble (dgvScanItems[4, x].Value);

                        // txtTotal.Text =Convert.ToString(Convert.ToDouble (dgvScanItems[5, x].Value));

                        txtTotal.Text = "0.00";
                        txtdisRate_TextChanged(sender, e);
                    }
                    else
                    {

                        int N = dgvScanItems.Rows.Count - 2;
                        dgvScanItems.Rows.RemoveAt(N);

                    }

                      txtSelectItem.Text="";
                      dgvScanItems.Focus();
                     
                     // txtSelectItem.Focus();
                }
              
                catch { }
              //  txtSelectItem.Focus();
          //  }
        }

        private void txtSelectItem_TextChanged(object sender, EventArgs e)
        {

        }

       
      
         }
}