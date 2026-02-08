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
    public partial class frmAccountLink : Form
    {
        private string ConnectionString;
        private DataSet DSInvoicing;
        private DataSet dsDr;
        private DataSet dsUser;
        private DataSet dsCustomer;
        private DataSet dsItemType;
        private DataSet dsItem;
        public frmAccountLink()
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
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkWHAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkcusAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DataTable dt;
            dt = LoadDataTable();
            if (dt == null)
            {
                return;
            }
            LoadGrid(dt);
        }

        private void LoadGrid(DataTable dt)
        {
            try
            {
                dgvUpdateAcc.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvUpdateAcc.Rows.Add();
                        dgvUpdateAcc.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[6].ToString().Trim();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DataTable LoadDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                string Customer = "";
                string Item = "";
                string WareHouse = "";
                string StrUser = string.Empty;
                string StrDr = string.Empty;

                if (cmbTT.Value != null) Item = cmbTT.Value.ToString().Trim();
                if (cmbCustomer.Value != null) Customer = cmbCustomer.Value.ToString().Trim();
                if (cmbUser.Value != null) StrUser = cmbUser.Value.ToString().Trim();
                if (cmbReferredDr.Value != null) StrDr = cmbReferredDr.Value.ToString().Trim();


                if (cbincludeTime.Checked == true)
                {


                    string sSQL = "SELECT[ReceiptNo],[ItemType],[Date],[ConsultFee],[HospitalFee],[TotalFee],[GLAccount],[PaymentMethod] FROM[tblScanChannel] where ([Date]+[CollectTime] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + " " + dtpfrom.Value.ToString("HH:mm:ss") + "')AND([Date]+[CollectTime]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + " " + dtpto.Value.ToString("HH:mm:ss") + "')" +
                                " and [ItemType] like '%'+'" + Item + "'" +
                                " and PatientNo like '%'+'" + Customer + "'" +
                                " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                                " and [CurrentUser] like '%'+'" + StrUser + "'and IsExport ='False'";

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);

                    da3.Fill(dt);


                }

                if (cbincludeTime.Checked == false)
                {


                    string sSQL = "SELECT [ReceiptNo],[ItemType],[Date],[ConsultFee],[HospitalFee],[TotalFee],[GLAccount],[PaymentMethod] FROM [tblScanChannel] where [Date] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [Date]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                                " and [ItemType] like '%'+'" + Item + "'" +
                                " and PatientNo like '%'+'" + Customer + "'" +
                                " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                                " and [CurrentUser] like '%'+'" + StrUser + "' and IsExport ='False'";

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                    da3.Fill(dt);

                }
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private double getRowCount()
        {
            DataTable dt = new DataTable();

            try
            {
                string Customer = "";
                string Item = "";
                string WareHouse = "";
                string StrUser = string.Empty;
                string StrDr = string.Empty;

                if (cmbTT.Value != null) Item = cmbTT.Value.ToString().Trim();
                if (cmbCustomer.Value != null) Customer = cmbCustomer.Value.ToString().Trim();
                if (cmbUser.Value != null) StrUser = cmbUser.Value.ToString().Trim();
                if (cmbReferredDr.Value != null) StrDr = cmbReferredDr.Value.ToString().Trim();


                if (cbincludeTime.Checked == true)
                {


                    string sSQL = "SELECT distinct( [ItemType]) FROM[tblScanChannel] where ([Date]+[CollectTime] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + " " + dtpfrom.Value.ToString("HH:mm:ss") + "')AND([Date]+[CollectTime]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + " " + dtpto.Value.ToString("HH:mm:ss") + "')" +
                                " and [ItemType] like '%'+'" + Item + "'" +
                                " and PatientNo like '%'+'" + Customer + "'" +
                                " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                                " and [CurrentUser] like '%'+'" + StrUser + "'and IsExport ='False'";

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);

                    da3.Fill(dt);


                }

                if (cbincludeTime.Checked == false)
                {


                    string sSQL = "SELECT distinct( [ItemType]) FROM [tblScanChannel] where [Date] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [Date]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                                " and [ItemType] like '%'+'" + Item + "'" +
                                " and PatientNo like '%'+'" + Customer + "'" +
                                " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                                " and [CurrentUser] like '%'+'" + StrUser + "' and IsExport ='False'";

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                    da3.Fill(dt);

                }
                double count = dt.Rows.Count;
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        private DataTable LoadDataTable1()
        {
            DataTable dt = new DataTable();

            try
            {
                string Customer = "";
                string Item = "";
                string WareHouse = "";
                string StrUser = string.Empty;
                string StrDr = string.Empty;

                if (cmbTT.Value != null) Item = cmbTT.Value.ToString().Trim();
                if (cmbCustomer.Value != null) Customer = cmbCustomer.Value.ToString().Trim();
                if (cmbUser.Value != null) StrUser = cmbUser.Value.ToString().Trim();
                if (cmbReferredDr.Value != null) StrDr = cmbReferredDr.Value.ToString().Trim();


                if (cbincludeTime.Checked == true)
                {


                    string sSQL = "SELECT distinct( [ItemType]) FROM[tblScanChannel] where ([Date]+[CollectTime] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + " " + dtpfrom.Value.ToString("HH:mm:ss") + "')AND([Date]+[CollectTime]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + " " + dtpto.Value.ToString("HH:mm:ss") + "')" +
                                " and [ItemType] like '%'+'" + Item + "'" +
                                " and PatientNo like '%'+'" + Customer + "'" +
                                " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                                " and [CurrentUser] like '%'+'" + StrUser + "'and IsExport ='False'";

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);

                    da3.Fill(dt);


                }

                if (cbincludeTime.Checked == false)
                {


                    string sSQL = "SELECT distinct( [ItemType]) FROM [tblScanChannel] where [Date] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [Date]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                                " and [ItemType] like '%'+'" + Item + "'" +
                                " and PatientNo like '%'+'" + Customer + "'" +
                                " and (ReferedDoctor like '%'+'" + StrDr + "' OR ReferedDoctor IS NULL)" +
                                " and [CurrentUser] like '%'+'" + StrUser + "' and IsExport ='False'";

                    SqlCommand cmd3 = new SqlCommand(sSQL);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                    da3.Fill(dt);

                }

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public void GetPatientCodeByType()
        {

          
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                string StrSql = "SELECT AcclinkPref, AcclinkPad, AcclinkNo FROM tblDefualtSetting";
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

                    txtAccountLinkNo.Text = StrInvNo + StrInV.Substring(1, intX);

                }

           
        }
        private void frmAccountLink_Load(object sender, EventArgs e)
        {
            GetUserDataSet();
            GetRefferedDr();
            GetCustomerDataset();
            GetItemTypeDataSet();
            GetPatientCodeByType();
        }
        public void GetRefferedDr()
        {
            dsDr = new DataSet();
            try
            {
                dsDr.Clear();
                string StrSql = "SELECT [Name],[Type] FROM [tblDoctorMaster]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsDr, "DtDr");

                cmbReferredDr.DataSource = dsDr.Tables["DtDr"];
                cmbReferredDr.DisplayMember = "Name";
                cmbReferredDr.ValueMember = "Name";

                cmbReferredDr.DisplayLayout.Bands["DtDr"].Columns["Name"].Width = 200;
                cmbReferredDr.DisplayLayout.Bands["DtDr"].Columns["Type"].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetItemTypeDataSet()
        {
            dsItemType = new DataSet();
            try
            {
                dsItemType.Clear();
                string StrSql = "SELECT [TypeName]FROM [tblPatientMasterType]";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItemType, "dtType");

                cmbTT.DataSource = dsItemType.Tables["dtType"];
                cmbTT.DisplayMember = "TypeName";
                cmbTT.ValueMember = "TypeName";
                cmbTT.DisplayLayout.Bands["dtType"].Columns["TypeName"].Width = 150;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetCustomerDataset()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                string StrSql = "SELECT CutomerID,CustomerName FROM tblCustomerMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtCustomer");

                cmbCustomer.DataSource = dsCustomer.Tables["DtCustomer"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CutomerID";
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["CutomerID"].Width = 150;
                cmbCustomer.DisplayLayout.Bands["DtCustomer"].Columns["CustomerName"].Width = 300;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetUserDataSet()
        {
            dsUser = new DataSet();
            try
            {
                dsUser.Clear();
                string StrSql = "SELECT UserID FROM Login";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsUser, "DtUser");

                cmbUser.DataSource = dsUser.Tables["DtUser"];
                cmbUser.DisplayMember = "UserID";
                cmbUser.ValueMember = "UserID";
                cmbUser.DisplayLayout.Bands["DtUser"].Columns["UserID"].Width = 150;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cbincludeTime_CheckedChanged(object sender, EventArgs e)
        {
            if (cbincludeTime.Checked == true)
            {
                dtpfrom.Enabled = true;
                dtpto.Enabled = true;
            }
            else
            {
                dtpto.Enabled = false;
                dtpfrom.Enabled = false;
            }
        }
        private void JournalEntrie(SqlConnection con, SqlTransaction myTrans)
        {





            string date = (System.DateTime.Now).ToShortDateString();

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\Journal_Entrie.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_GLJournal_Entries");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");
            Writer.WriteStartElement("PAW_GLJournal_Entry");
            Writer.WriteAttributeString("xsi:type", "paw:gljournal_entry");



            Writer.WriteStartElement("Date");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(date);//Date 
            Writer.WriteEndElement();

            Writer.WriteStartElement("Reference");
            Writer.WriteString(txtAccountLinkNo.Text.ToString());
            Writer.WriteEndElement();


            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString((daAccount.Data.Rows.Count * 2).ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Transactions");
            for (int i = 0; i < daAccount.Data.Rows.Count; i++)
            {

                Writer.WriteStartElement("Transaction");

                Writer.WriteStartElement("GL_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");

                SqlCommand command1;
                String StrSql1 = "SELECT [GL_Account] FROM [tblCreditData] where [CardType]='" + daAccount.Data.Rows[i].ItemArray[2].ToString() + "'";
                command1 = new SqlCommand(StrSql1, con, myTrans);
                SqlDataAdapter da1 = new SqlDataAdapter(command1);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    Writer.WriteString(dt1.Rows[0].ItemArray[0].ToString().Trim());
                }

                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(daAccount.Data.Rows[i].ItemArray[2].ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString( daAccount.Data.Rows[i].ItemArray[1].ToString());
                Writer.WriteEndElement();

                Writer.WriteEndElement();


                Writer.WriteStartElement("Transaction");

                Writer.WriteStartElement("GL_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");

                SqlCommand command;
                String StrSql = "SELECT GlAccount FROM tblGLAccount where ItemType='" + daAccount.Data.Rows[i].ItemArray[0].ToString() + "'";
                command = new SqlCommand(StrSql, con, myTrans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Writer.WriteString(dt.Rows[0].ItemArray[0].ToString().Trim());
                }
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(daAccount.Data.Rows[i].ItemArray[0].ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + daAccount.Data.Rows[i].ItemArray[1].ToString());
                Writer.WriteEndElement();

                Writer.WriteEndElement();














            }
            Writer.WriteEndElement();

            Writer.WriteEndElement();
            Writer.WriteEndElement();
            Writer.Close();
            Connector abc = new Connector();//export to peach tree
            abc.ImportGeneralJournaFromPeachtreeee();//ImportSalesInvice()


        }
        public dsAccountLink daAccount = new dsAccountLink();

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            SqlConnection con3 = new SqlConnection(ConnectionString);
            double count = 0;
            DataTable dt;
            DataTable dt1;
            DataTable dt2;
            dt = LoadDataTable();
            dt1 = LoadDataTable1();
            dt2 = Loaddatatabl2();
            count = getRowCount();
            if (dt == null)
            {
                return;
            }

            Fillds(count, dt, dt1, dt2);
          //  JournalEntrie(con3);


        }

        private DataTable Loaddatatabl2()
        {
            DataTable dt = new DataTable();
            try
            {

                string StrSql = "SELECT [CardType] As PaymentMethod FROM [tblCreditData]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void Fillds(double count, DataTable dt, DataTable dt1, DataTable dt2)
        {
            daAccount.Clear();
            for (int k = 0; k < dt2.Rows.Count; k++)
            {
                for (int i = 0; i < count; i++)
                {
                    double _Total = 0;
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        if (dt2.Rows[k].ItemArray[0].ToString() == dt.Rows[j].ItemArray[7].ToString())
                        {
                            if (dt1.Rows[i].ItemArray[0].ToString() == dt.Rows[j].ItemArray[1].ToString())
                            {
                                _Total += Convert.ToDouble(dt.Rows[j].ItemArray[5].ToString());
                            }
                        }
                    }
                    if (_Total != 0)
                    {
                        daAccount.Data.Rows.Add(dt1.Rows[i].ItemArray[0].ToString(), _Total.ToString(), dt2.Rows[k].ItemArray[0].ToString());
                    }
                }
            }
        }

        public void UpdatePatientCodeByType(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;

                string StrSql = "SELECT  TOP 1(AcclinkNo) FROM tblDefualtSetting";
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
                StrSql = "UPDATE tblDefualtSetting SET AcclinkNo='" + intInvNo + "'";
                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }

        }
        private void btnSave_Click(object sender, EventArgs e)
        {

           
            SaveEvent();
            btnNewn_Click(sender, e);


        }


        private void SaveEvent()
        {
            double count = 0;
            DataTable dt;
            DataTable dt1;
            DataTable dt2;
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            try
            {
                DialogResult reply1 = MessageBox.Show("Are you sure, you want to Save And Upload this record ? ", "Information", MessageBoxButtons.YesNo);

                if (reply1 == DialogResult.No)
                {
                    return;
                }

                dt = LoadDataTable();
                dt1 = LoadDataTable1();
                dt2 = Loaddatatabl2();
                count = getRowCount();

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                GetPatientCodeByType();
                UpdatePatientCodeByType(myConnection, myTrans);

                for (int i = 0; i < dgvUpdateAcc.RowCount; i++)
                {
                    if (!((String)dgvUpdateAcc[0, i].Value == null || (String)dgvUpdateAcc[0, i].Value.ToString() == string.Empty || (String)dgvUpdateAcc[0, i].Value == ""))
                    {
                        string StrSql = "insert into tblAccountLinkDetail(AccLinDeNo,ReferanceNo,IncomeType,Date,DoctorCharge,HospitalCharge,Amount,GLAcount,SaveDate)" +
                      " values ('" + txtAccountLinkNo.Text.ToString() + "','" + dgvUpdateAcc[0, i].Value.ToString() + "', '" + dgvUpdateAcc[1, i].Value.ToString() + "', '" + dgvUpdateAcc[2, i].Value + "', '" + dgvUpdateAcc[3, i].Value + "', '" + dgvUpdateAcc[4, i].Value + "', '" + dgvUpdateAcc[5, i].Value + "', '" + dgvUpdateAcc[6, i].Value.ToString() + "','" + DateTime.Now + "')";
                        SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
                UpdateScanChanel(myTrans, myConnection);
                // btnUpdate_Click(null, null);



                if (dt == null)
                {
                    return;
                }

                Fillds(count, dt, dt1, dt2);

                //the follwing code segment create xml file and export to peachtee as agernalentry
                JournalEntrie(myConnection, myTrans);

               


                MessageBox.Show("Details Successfuly Save.", "Information", MessageBoxButtons.OK);
                myTrans.Commit();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);

            }

        }

        private void UpdateScanChanel(SqlTransaction myTrans, SqlConnection myConnection)
        {




            for (int i = 0; i < dgvUpdateAcc.RowCount; i++)
            {
                if (!((String)dgvUpdateAcc[0, i].Value == null || (String)dgvUpdateAcc[0, i].Value.ToString() == string.Empty || (String)dgvUpdateAcc[0, i].Value == ""))
                {
                    string StrSql = "UPDATE tblScanChannel SET IsExport='True' WHERE ReceiptNo='" + dgvUpdateAcc[0, i].Value.ToString() + "'";
                    SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }



        }

        private void btnNewn_Click(object sender, EventArgs e)
        {
            dgvUpdateAcc.Rows.Clear();
            cbincludeTime.Checked = false;
            chkcusAll.Checked = false;
            chkWHAll.Checked = false;
            checkBox2.Checked = false;
            checkBox1.Checked = false;
            dtpFromDate.Value = DateTime.Now;
            dtpToDate.Value = DateTime.Now;
            dtpfrom.Value = DateTime.Now;
            dtpto.Value = DateTime.Now;
            cmbCustomer.Text = "";
            cmbTT.Text = "";
            cmbReferredDr.Text = "";
            cmbUser.Text = "";
            txtAccountLinkNo.Text = "";
            Eneble();
            GetPatientCodeByType();

        }

        private void btnList_Click(object sender, EventArgs e)
        {
            frmAccountLinkList AccLinkList = new frmAccountLinkList();
            AccLinkList.ShowDialog();
            if (Search.AccountLinkNo != "")
            {
                btnNewn_Click(null, null);
                Disable();
                LoadData();
            }
        }

        private void Disable()
        {
            btnSave.Enabled = false;
            btnUpdate.Enabled = false;
            btnLoad.Enabled = false;
            cbincludeTime.Enabled = false;
            groupBox1.Enabled = false;
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
            groupBox5.Enabled = false;
            groupBox7.Enabled = false;
            txtAccountLinkNo.ReadOnly = true;

        }
        private void Eneble()
        {
            btnSave.Enabled = true;
            btnUpdate.Enabled = true;
            btnLoad.Enabled = true;
            cbincludeTime.Enabled = true;
            groupBox1.Enabled = true;
            groupBox3.Enabled = true;
            groupBox4.Enabled = true;
            groupBox5.Enabled = true;
            groupBox7.Enabled = true;
            txtAccountLinkNo.ReadOnly = true;

        }

        private void LoadData()
        {
            try
            {
                String S2 = "Select * from tblAccountLinkDetail where AccLinDeNo='" + Search.AccountLinkNo + "' ";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);

                dgvUpdateAcc.Rows.Clear();
                if (dt2.Rows.Count > 0)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        dgvUpdateAcc.Rows.Add();
                        txtAccountLinkNo.Text = dt2.Rows[i].ItemArray[0].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[0].Value = dt2.Rows[i].ItemArray[1].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[1].Value = dt2.Rows[i].ItemArray[2].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[2].Value = dt2.Rows[i].ItemArray[3].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[3].Value = dt2.Rows[i].ItemArray[4].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[4].Value = dt2.Rows[i].ItemArray[5].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[5].Value = dt2.Rows[i].ItemArray[6].ToString().Trim();
                        dgvUpdateAcc.Rows[i].Cells[6].Value = dt2.Rows[i].ItemArray[7].ToString().Trim();

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
