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
    public partial class frmChannellig : Form
    {
        DataTable dtUser = new DataTable();
        Class1 a = new Class1();
       // public int flg = 0;
        public int flgTech = 0;
        public string StrFormType = string.Empty;
        public int CheckSearch = 0;//to avoid goto textchange event after find result

        public dsChannelling dsChannelling = new dsChannelling();

        bool run = false;
        bool add = false;
        bool edit = false;
        bool delete = false;

        public static string ConnectionString;
        public static bool isedit = false;
        public frmChannellig()
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
                MessageBox.Show(ex.Message);
            }
        }

       

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
               FlgChangeNotTokenNo = false;
                dtpTime.Enabled = true;
                cmbSession.Enabled = true;
                dtpTime.Value = DateTime.Now;
                cmbSession.SelectedIndex=-1;
                txtconsultIntime.Text = "";
               
                Enable();
                cmbPaymentMethod.Text = "Cash";
               
                txtdisRate.Text = "0";
                txtDisAmount.Text = "0";
                txtnetTotal.Text = "0";
                txtTotal.Text = "0";
                dgvScanItems.Rows.Clear();
                string currenru1 = user.userName;
                ClearText();
                isedit = false;
                EnableObjects();
                          
               // flg = 2;
                txtPatientNo.Enabled = true;
                txtPatientNo.Enabled = true;
                txtTestType.Enabled = true;

                A = 0;
                txtTestType.Text = "";
                txtPationType.Text = "";
                txtReceiptNo.Text = "";
                txtPatientNo.Text = "";
               
                btnVoid.Enabled = false;
                toolStripButton4.Enabled = false;
                btnConfirm.Enabled = false;
                btnPrint.Enabled = false;
                txtTestType.Focus();
                txtTestType.Text = "CHANNELLING";
                
                rbcash.Enabled = true;

                cbNormalBill.Enabled = true;
                cbNormalBill.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }
        public void EnableObjects()
        {
            try
            {
                
                txtReceiptNo.Enabled = true;
                dtpDate.Enabled = true;
                  txtFirstName.ReadOnly = false;
                UltrInsuranceCampany.ReadOnly = false;
                txtContactNo.ReadOnly = false;
                txtRemarks.ReadOnly = false;
                cmbPaymentMethod.Enabled = true;
                txtCreditCardNo.ReadOnly = false;
               
                btnConsultant.Enabled = true;
                btnNewn.Enabled = true;
                btnSave.Enabled = true;
                txtdisRate.ReadOnly = false;
                txtPatientNo.ReadOnly = false;
                dtpRepDate.Enabled = true;
                dtpDate.Enabled = true;
                rbcash.Enabled = true;

            }
            catch { }
        }

        public void ClearText()
        {
            try
            {
                rbcash.Checked = false;
                
                txtTokenNo.Text = "";
                dtpDate.Text = "";
                txtConsultant.Text = "";
                txtFirstName.Text = "";
                UltrInsuranceCampany.Text = "";
                txtContactNo.Text = "";
                txtRemarks.Text = "";
                cmbPaymentMethod.Text = "";
                txtCreditCardNo.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }
        private void Enable()
        {
           
            txtTestType.Enabled = true;
            txtReceiptNo.Enabled = true;
            txtPatientNo.Enabled = true;
            txtTokenNo.Enabled = true;
            txtFirstName.Enabled = true;
            UltrInsuranceCampany.Enabled = true;
            txtContactNo.Enabled = true;
            txtRemarks.Enabled = true;
            txtConsultant.Enabled = true;
            dgvScanItems.ReadOnly = false;
            btnSave.Enabled = true;
            btnConfirm.Enabled = true;
            Clear.Enabled = true;
            btnConsultant.Enabled = true;
            dtpRepDate.Enabled = true;
            dtpDate.Enabled = true;
           
            txtCreditCardNo.Enabled = true;
            txtdisRate.Enabled = true;

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
       
       
        private void btnConsultant_Click(object sender, EventArgs e)
        {
            try {
                StrFormType = txtTestType.Text.ToString();
                 frmChannellingConsultantList frm = new frmChannellingConsultantList(this);
                frm.Show();

            }
            catch { }

        }
        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {


        }
        private bool validateInsurenceCompany()
        {

            String S1 = "SELECT * FROM [tblInsurence] WHERE [InsurenceName]='" + UltrInsuranceCampany.Text.ToString() + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        
        private void frmScan_Load(object sender, EventArgs e)
        {
            getinsurance();
            getpaymentmode();

        }


        private void getinsurance()
        {
            DataSet dsInsurance = new DataSet();
            try
            {
                dsInsurance.Clear();
                String StrSql4 = "SELECT InsurenceName FROM tblInsurence ";

                SqlDataAdapter dAdapt4 = new SqlDataAdapter(StrSql4, ConnectionString);
                dAdapt4.Fill(dsInsurance, "DtClient");

                UltrInsuranceCampany.DataSource = dsInsurance.Tables["DtClient"];
                UltrInsuranceCampany.DisplayMember = "InsurenceName";
                UltrInsuranceCampany.ValueMember = "InsurenceName";




                UltrInsuranceCampany.DisplayLayout.Bands["DtClient"].Columns["InsurenceName"].Width = 300;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void frmScan_Activated(object sender, EventArgs e)
        {
            
                if (Class1.flg == 1)
                {
                    Class1.flg = 0;
                    txtConsultant.Text = a.GetText7();
                    //dgvScanItems.Rows.Add();
                    int x = dgvScanItems.Rows.Count - 1;
                    dgvScanItems.Rows.Clear();
                    // dgvScanItems.Rows.Add();

                    String itemid = a.GetText();
                    string itemtype = a.GetText6();
                    string ConnString = ConnectionString;
                    string sql = "Select * from tblConsultantItemFee where  (ItemID = '" + itemid + "')AND (ItemType='" + itemtype + "')";
                    SqlConnection Conn = new SqlConnection(ConnString);
                    SqlCommand cmd = new SqlCommand(sql);
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // dgvScanItems[0, x].Value = a.GetText2();
                    if (dt.Rows.Count > 0)
                    {
                        dgvScanItems[0, x].Value = dt.Rows[0].ItemArray[1].ToString().Trim();
                        dgvScanItems[1, x].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                        dgvScanItems[2, x].Value = dt.Rows[0].ItemArray[6].ToString().Trim();
                        dgvScanItems[3, x].Value = dt.Rows[0].ItemArray[5].ToString().Trim();
                        dgvScanItems[4, x].Value = dt.Rows[0].ItemArray[3].ToString().Trim();
                        dgvScanItems[5, x].Value = dt.Rows[0].ItemArray[4].ToString().Trim();
                        dgvScanItems[6, x].Value = Convert.ToDouble(dt.Rows[0].ItemArray[3].ToString().Trim()) + Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString().Trim());
                        // ReceiptTotal = ReceiptTotal + Convert.ToDouble (dgvScanItems[4, x].Value);
                        txtTotal.Text = Convert.ToString(Convert.ToDouble(dgvScanItems[6, x].Value));
                        // txtScanTotal.Text = Convert.ToString(Convert.ToDouble(dgvScanItems[5, x].Value));
                    }
                    txtconsultIntime.Text = string.Empty;
                    GetConsultIntime();
                    getChanalingNo();
                    //ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems[4, x].Value);
                    // ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems.Columns[4].ToString());
                    // txtTotal.Text = Convert.ToString(ReceiptTotal);

                }
            
        }

       


        private void getChanalingNo()
        {

            try
            {
                string StrCode = "";
                switch (cmbSession.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "1";

                            break;
                        }
                    case 1:
                        {
                            StrCode = "2";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "3";
                            break;
                        }




                }

                String S1 = "Select DISTINCT(ReceiptNo)from tblScanChannel where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "') AND Session='"+ StrCode + "'";
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        public int A = 0;
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                A = 1;
                if (isedit == false)
                {
                    
                    getChanalingNo();
                   
                }
                //===================================================   
                if (txtTestType.Text == string.Empty)
                {
                    MessageBox.Show("Please Select a Test Type");
                    A = 0;
                    return;
                }
                
                if (txtPatientNo.Text == "")
                {
                    MessageBox.Show("Please Enter a Patient No");
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
                if (txtConsultant.Text == "")
                {
                    MessageBox.Show("Please Select a Consultant");
                    A = 0;
                    return;
                }

                if (cbNormalBill.Checked == false)
                {
                    if (validateInsurenceCompany())
                    {
                        MessageBox.Show("Please Select a Valied Insurence Company");
                        A = 0;
                        return;
                    }
                }
                if (dgvScanItems.Rows.Count < 1)
                {
                    MessageBox.Show("Please Select a Consultant");
                    A = 0;
                    return;
                }
                if (txtconsultIntime.Text=="")
                {
                    MessageBox.Show("Please Select a Correct Sesstion");
                    A = 0;
                    return;
                }


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
        string StrReferencetNo = string.Empty;
        private void ConfirmEventSave()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                for (int i = 0; i < dgvScanItems.Rows.Count ; i++)
                {
                    // string ItemType = "Scan";
                    string IsExport = "False";

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
        private void DeleteAndUpdate()
        {
            string StrCode = "";
            switch (cmbSession.SelectedIndex)
            {
                case 0:
                    {
                        StrCode = "1";

                        break;
                    }
                case 1:
                    {
                        StrCode = "2";
                        break;
                    }
                case 2:
                    {
                        StrCode = "3";
                        break;
                    }




            }
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

                //StrReferencetNo = GetPatientCodeByType(myConnection, myTrans);
                //UpdatePatientCodeByType(myConnection, myTrans);
                //txtReceiptNo.Text = StrReferencetNo;

                string S8 = " DELETE FROM tblScanChannel WHERE ReceiptNo='" + txtReceiptNo.Text.ToString() + "' ";
                SqlCommand cmd8 = new SqlCommand(S8, myConnection, myTrans);
                SqlDataAdapter da8 = new SqlDataAdapter(cmd8);
                DataTable dt8 = new DataTable();
                da8.Fill(dt8);


                int rowCount = GetFilledRows();
                string IsExport = "False";
                string IsReprint = "False";
                if (cbNormalBill.Checked == true)
                {
                    Insurence = "Normal Bill";
                }
                else
                {
                    Insurence = UltrInsuranceCampany.Text.ToString();
                }
                for (int i = 0; i < rowCount; i++)
                {
                    string StrSql = "insert into tblScanChannel(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal,PatientNo,IsConfirm,PationType,ReferedDoctor,NumberOfDistribution,DistributionNumber,IsReportPrint,ReciptTime,Session,ConsultantInTime,InsuranceCampany)" +
                    " values ('" + txtReceiptNo.Text + "','" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value.ToString() + "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dgvScanItems[6, i].Value + "', '','" + txtTotal.Text.ToString().Trim() + "','" + txtTestType.Text.ToString() + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" + txtPatientNo.Text.ToString() + "','false','" + txtPationType.Text.ToString() + "','','" + rowCount + "','" + i + "','" + IsReprint + "','" + dtpTime.Value + "','" + StrCode + "','" + txtconsultIntime.Text.ToString() + "','"+ Insurence + "')";
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
        private void SaveEventNew()
        {
            string StrCode = "";
            switch (cmbSession.SelectedIndex)
            {
                case 0:
                    {
                        StrCode = "1";

                        break;
                    }
                case 1:
                    {
                        StrCode = "2";
                        break;
                    }
                case 2:
                    {
                        StrCode = "3";
                        break;
                    }




            }
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                StrReferencetNo = GetPatientCodeByType(myConnection, myTrans);
                UpdatePatientCodeByType(myConnection, myTrans);
                txtReceiptNo.Text = StrReferencetNo;

                int rowCount = GetFilledRows();
                string IsExport = "False";
                string IsReprint = "False";
                if (cbNormalBill.Checked == true)
                {
                    Insurence = "Normal Bill";
                }
                else
                {
                    Insurence = UltrInsuranceCampany.Text.ToString();
                }
                for (int i = 0; i < rowCount; i++)
                {
                   string StrSql = "insert into tblScanChannel(ReceiptNo,TokenNo, Date, ItemID, Consultant, PatientName, ContactNo, Remarks,PaymentMethod, CreditCardNo, ItemDescription,GLAccount,Duration, ConsultFee, HospitalFee, TotalFee, DueDate,ReceiptTotal,ItemType,CurrentUser,RepDate,IsExport,DisRate,DisAmount,NetTotal,PatientNo,IsConfirm,PationType,ReferedDoctor,NumberOfDistribution,DistributionNumber,IsReportPrint,ReciptTime,Session,ConsultantInTime,InsuranceCampany)" +
                      " values ('" + txtReceiptNo.Text + "','" + txtTokenNo.Text.ToString().Trim() + "', '" + dtpDate.Text.ToString().Trim() + "', '" + dgvScanItems[0, i].Value.ToString() + "', '" + txtConsultant.Text.ToString().Trim() + "', '" + txtFirstName.Text.ToString().Trim() + "', '" + txtContactNo.Text.ToString().Trim() + "', '" + txtRemarks.Text.ToString().Trim() + "','" + cmbPaymentMethod.Text.ToString().Trim() + "', '" + txtCreditCardNo.Text.ToString().Trim() + "', '" + dgvScanItems[1, i].Value + "', '" + dgvScanItems[2, i].Value + "', '" + dgvScanItems[3, i].Value + "', '" + dgvScanItems[4, i].Value + "', '" + dgvScanItems[5, i].Value + "', '" + dgvScanItems[6, i].Value + "', '','" + txtTotal.Text.ToString().Trim() + "','" + txtTestType.Text.ToString() + "','" + user.userName.ToString().Trim() + "','" + dtpRepDate.Text.ToString().Trim() + "','" + IsExport + "','" + txtdisRate.Text.ToString().Trim() + "','" + txtDisAmount.Text.ToString().Trim() + "','" + txtnetTotal.Text.ToString().Trim() + "','" + txtPatientNo.Text.ToString() + "','false','" + txtPationType.Text.ToString() + "','','" + rowCount + "','" + i + "','" + IsReprint + "','"+dtpTime.Value+"','"+ StrCode + "','"+txtconsultIntime.Text.ToString()+"','"+ Insurence + "')";
                    SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                myTrans.Commit();
                
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
        public void UpdatePatientCodeByType(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;

               string StrSql = "SELECT  TOP 1(TNo) FROM tblLabTestTransactionType where TypeName='CHANNELLING' ORDER BY TNo DESC";
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
                StrSql = "UPDATE tblLabTestTransactionType SET TNo='" + intInvNo + "' where TypeName='CHANNELLING' ";
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

               string StrSql = "SELECT TPref, TPad, TNo FROM tblLabTestTransactionType where TypeName='CHANNELLING'";
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
        
        private void txtConsultant_TextChanged(object sender, EventArgs e)
        {
           
        }

        
               

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dgvScanItems_Leave(object sender, EventArgs e)
        {
           
        }

        private void label6_Enter(object sender, EventArgs e)
        {
        }

        private void dgvScanItems_Enter(object sender, EventArgs e)
        {
            
        }

       
        private void btnReprintScan_Click(object sender, EventArgs e)
        {
            try
            {
                dsChannelling.tblScanChannel.Clear();
                String S3 = "Select * from [tblScanChannel] where ([ReceiptNo] = '" + txtReceiptNo.Text.ToString().Trim() + "')AND(Date='" + dtpDate.Text.ToString() + "')";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(dsChannelling, "tblScanChannel");

                frmChannellingPrint invprint = new frmChannellingPrint(this);
                invprint.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            try
            {
                
                dgvScanItems.Rows.Clear();

            }
            catch { }
        }

        

       private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            if (FlgChangeNotTokenNo == false)
            {
                txtconsultIntime.Text = string.Empty;
                GetConsultIntime();
                getChanalingNo();
            }
        }

        private void GetConsultIntime()
        {
            string code = "";
            try
            {
                //string date11 = dtpDate.Value.ToString(@"MM/dd/yyy");
                String StrSql11 = "Select ConsultantCode from tblConsultantMaster where ConsultantName = '" + txtConsultant.Text.ToString() + "'";
                SqlCommand com11 = new SqlCommand(StrSql11);
                SqlDataAdapter da11 = new SqlDataAdapter(StrSql11, ConnectionString);
                DataTable dt11 = new DataTable();
                da11.Fill(dt11);
                if (dt11.Rows.Count > 0)
                {
                    code = dt11.Rows[0].ItemArray[0].ToString();
                }
                if (!((cmbSession.Text.ToString() == string.Empty) || (txtConsultant.Text.ToString() == string.Empty)))
                {
                    string date = dtpDate.Value.ToString(@"MM/dd/yyy");
                    String StrSql = "Select * from tblConsultantMaster where ConsultantName = '" + txtConsultant.Text.ToString() + "' AND( LeaveFromDate<='" + date + "' AND '" + date + "'<=LeaveToDate)";
                    SqlCommand com = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("This Appointment Date, Doctor's Leave Day");
                    }
                    else
                    {
                        string StrCode = "";
                        switch (cmbSession.SelectedIndex)
                        {
                            case 0:
                                {
                                    StrCode = "1";

                                    break;
                                }
                            case 1:
                                {
                                    StrCode = "2";
                                    break;
                                }
                            case 2:
                                {
                                    StrCode = "3";
                                    break;
                                }




                        }
                        DateTime y = Convert.ToDateTime(dtpDate.Value);
                        string day = y.ToString(@"dddd");

                        string ConnString = ConnectionString;
                        String S1 = "Select SessionTime from tblSchedulingDetails where ConsultantName = '" + txtConsultant.Text.ToString() + "' AND  ADay ='" + day + "'  AND Session='" + StrCode + "' ORDER BY ADay";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                        if (dt1.Rows.Count > 0)
                        {
                            txtconsultIntime.Text = dt1.Rows[0].ItemArray[0].ToString();
                            if (dt1.Rows[0].ItemArray[0].ToString() == "")
                            {
                                MessageBox.Show("The Doctor is not allocated in this Session");
                            }
                        }
                        else
                        {
                            MessageBox.Show("The Doctor is not allocated in this Session");
                        }
                           
                        

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtTokenNo_TextChanged(object sender, EventArgs e)
        {

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
                txtDisAmount.Text = DiscountAmount.ToString("N2");
                NetTotal = totBefoDiscount - DiscountAmount;
                // txtnetTotal.Text = Convert.ToString(NetTotal);
                txtnetTotal.Text = NetTotal.ToString("N2");
            }
            catch { MessageBox.Show("Enter Correct Discount Rate"); }

        }

        private void txtPatientNo_TextChanged(object sender, EventArgs e)
        {

        }

       
       private void label3_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
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

        

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                isedit = true;
                EnableForEdit();
            }
            catch
            {

            }
        }
        private void EnableForEdit()
        {

            dtpTime.Enabled = true;
            cmbSession.Enabled = true;
            btnConfirm.Enabled = false;
            txtTestType.Enabled = false;
            //comboBox1.Enabled = true;
            txtReceiptNo.Enabled = false;
            txtPatientNo.Enabled = false;
            txtTokenNo.Enabled = false;
            txtFirstName.Enabled = true;
            if (cbNormalBill.Checked == true)
            {
                UltrInsuranceCampany.Enabled = false;
            }
            else
            {
                UltrInsuranceCampany.Enabled = true;
            }

            txtContactNo.Enabled = true;
            txtRemarks.Enabled = true;
            txtConsultant.Enabled = true;
            
            dgvScanItems.ReadOnly = false;
            
        
            Clear.Enabled = true;

            btnConsultant.Enabled = true;
            btnSave.Enabled = true;
            dtpRepDate.Enabled = false;
            dtpDate.Enabled = false;
           
           
            txtCreditCardNo.Enabled = true;
            txtdisRate.Enabled = true;

        }

        Boolean Confirmchk;
        Boolean VoidChk;
        private void disable()
        {
            txtconsultIntime.Enabled = false;
            dtpTime.Enabled = false;
            cmbSession.Enabled = false;
            txtTestType.Enabled = false;
         
            txtReceiptNo.Enabled = false;
            txtPatientNo.Enabled = false;
            txtTokenNo.Enabled = false;
            txtFirstName.Enabled = false;
            UltrInsuranceCampany.Enabled = false;
            txtContactNo.Enabled = false;
            txtRemarks.Enabled = false;
            txtConsultant.Enabled = false;
            rbcash.Enabled = false;
            dgvScanItems.ReadOnly = true;
            btnSave.Enabled = false;
            btnConfirm.Enabled = false;
            Clear.Enabled = false;
            btnConsultant.Enabled = false;
           
            dtpRepDate.Enabled = false;
            dtpDate.Enabled = false;
           
            txtCreditCardNo.Enabled = false;
            txtdisRate.Enabled = false;


        }
        private void btnList_Click(object sender, EventArgs e)
        {
           
            ChannellingList sl = new ChannellingList();
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
       Boolean FlgChangeNotTokenNo = false;
        private string Insurence;

        private void LoadScan()
        {
            try
            {
               
                String S2 = "Select * from tblScanChannel where (ReceiptNo =  '" + Search.ScanList + "')AND(Date='" + Search.ScanDate + "')AND(TokenNo='" + Search.tokenNo + "')";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);

                if (dt2.Rows.Count > 0)
                {
                    if (txtPationType.Text.ToString() == "Cash")
                    {
                        rbcash.Checked = true;
                    }
                    txtTestType.Text = dt2.Rows[0].ItemArray[18].ToString();
                   
                  txtPationType.Text = dt2.Rows[0].ItemArray[36].ToString();
                    txtReceiptNo.Text = dt2.Rows[0].ItemArray[0].ToString();
                    txtPatientNo.Text = dt2.Rows[0].ItemArray[30].ToString();
                   
                    txtFirstName.Text = dt2.Rows[0].ItemArray[5].ToString();
                    txtContactNo.Text = dt2.Rows[0].ItemArray[6].ToString();
                    txtRemarks.Text = dt2.Rows[0].ItemArray[7].ToString();
                    txtConsultant.Text = dt2.Rows[0].ItemArray[4].ToString();
                  
                    dtpDate.Text =  dt2.Rows[0].ItemArray[2].ToString();
                    VoidChk = Convert.ToBoolean(dt2.Rows[0].ItemArray[37].ToString());
                    Confirmchk = Convert.ToBoolean(dt2.Rows[0].ItemArray[35].ToString());
                    dtpRepDate.Text = dt2.Rows[0].ItemArray[20].ToString();
                    dtpTime.Text = dt2.Rows[0].ItemArray[44].ToString();
                    txtconsultIntime.Text = dt2.Rows[0].ItemArray[46].ToString();
                    if (dt2.Rows[0].ItemArray[47].ToString() == "Normal Bill")
                    {
                        cbNormalBill.Checked = true;
                    }
                    else
                    {
                        cbNormalBill.Checked = false;
                        UltrInsuranceCampany.Text = dt2.Rows[0].ItemArray[47].ToString();
                    }

                    if (dt2.Rows[0].ItemArray[45].ToString() == "1")
                    {
                        cmbSession.SelectedIndex=0;
                    }
                    else if (dt2.Rows[0].ItemArray[45].ToString() == "2")
                    {
                        cmbSession.SelectedIndex = 1;
                    }
                    else if (dt2.Rows[0].ItemArray[45].ToString() == "3")
                    {
                        cmbSession.SelectedIndex = 2;
                    }
                   
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        dgvScanItems.Rows[i].Cells[0].Value = dt2.Rows[i].ItemArray[3].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[1].Value = dt2.Rows[i].ItemArray[10].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[2].Value = dt2.Rows[i].ItemArray[11].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[3].Value = dt2.Rows[i].ItemArray[12].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[4].Value = dt2.Rows[i].ItemArray[13].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[5].Value = dt2.Rows[i].ItemArray[14].ToString().Trim();
                        dgvScanItems.Rows[i].Cells[6].Value = dt2.Rows[i].ItemArray[15].ToString().Trim();
                    }
                    cmbPaymentMethod.Text = dt2.Rows[0].ItemArray[8].ToString();
                    txtCreditCardNo.Text = dt2.Rows[0].ItemArray[9].ToString();
                    txtTotal.Text = dt2.Rows[0].ItemArray[17].ToString();
                    txtdisRate.Text = dt2.Rows[0].ItemArray[23].ToString();
                    txtDisAmount.Text = dt2.Rows[0].ItemArray[24].ToString();
                    txtnetTotal.Text = dt2.Rows[0].ItemArray[25].ToString();

                     FlgChangeNotTokenNo = true;
                    txtTokenNo.Text = dt2.Rows[0].ItemArray[1].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                    for (int i = 0; i < dgvScanItems.Rows.Count; i++)
                    {
                        // string ItemType = "Scan";
                        string IsExport = "False";

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
        private void txtPatientNo_MouseEnter(object sender, EventArgs e)
        {
        }
        private void getpaymentmode()
        {

            DataSet dspaymetmode = new DataSet();
            try
            {
                dspaymetmode.Clear();
                //StrSql = "SELECT distinct([ItemType]) as[ItemType] FROM[HospitalSystem].[dbo].[tblConsultantItemFee]";
               string StrSql = "SELECT [CardType] As PaymentMethod FROM [tblCreditData]";
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
        private void txtPatientNo_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (rbcash.Checked == false)
                {
                    DataAccess.Access.type = txtTestType.Text.ToString();
                    frmScanPation fsp = new frmScanPation();
                    fsp.ShowDialog();
                    if (Search.ScanPation != "")
                    {
                        LoadPation();
                    }
                    Search.ScanPation = "";
                }
            }
            catch
            {

            }
          
        }




        private void btnVoid_Click(object sender, EventArgs e)
        {
            if (txtTestType.Text == string.Empty)
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
            if (txtConsultant.Text == "")
            {
                MessageBox.Show("Please Select a Consultant");
                return;
            }
            if (dgvScanItems.Rows.Count < 1)
            {
                MessageBox.Show("Please Select a Consultant");
                return;
            }
            voidEvent();
            btnNew_Click(sender, e);
        }

        private void voidEvent()
        {
            for (int i = 0; i < dgvScanItems.Rows.Count; i++)
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

       

        

        private void dgvScanItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
       
        private void dgvScanItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
           
           
        }

        private void txtTotal_TextChanged(object sender, EventArgs e)
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
                txtDisAmount.Text = DiscountAmount.ToString("N2");
                NetTotal = totBefoDiscount - DiscountAmount;
                // txtnetTotal.Text = Convert.ToString(NetTotal);
                txtnetTotal.Text = NetTotal.ToString("N2");
            }
            catch { }
        }

        private void cmbSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FlgChangeNotTokenNo == false)
            {
                txtconsultIntime.Text = string.Empty;
                GetConsultIntime();
                getChanalingNo();
            }
        }

        private void txtContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {

                    e.Handled = true;
                    return;
                }
                if (txtContactNo.Text.Length == 10)
                {
                    e.Handled = true;

                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbNormalBill_CheckedChanged(object sender, EventArgs e)
        {
            if (cbNormalBill.Checked == true)
            {
                UltrInsuranceCampany.Enabled = false;
                UltrInsuranceCampany.Text = "";
                cmbPaymentMethod.Text = "CASH";
            }
            else
            {
                UltrInsuranceCampany.Enabled = true;
            }
        }

        private void txtContactNo_Leave(object sender, EventArgs e)
        {
            try
            {
                string contactnum = "";
                string s = "SELECT PatientName FROM tblScanChannel WHERE ContactNo = '" + txtContactNo.Text + "' ";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                contactnum = txtContactNo.Text.ToString();
                if (dt.Rows.Count > 0)
                {
                    rbcash.Checked = true;
                    txtFirstName.Text = dt.Rows[0].ItemArray[0].ToString();
                    txtContactNo.Text = contactnum;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}