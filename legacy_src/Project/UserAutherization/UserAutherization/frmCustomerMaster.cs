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
    public partial class frmCustomerMaster : Form
    {
        private string ConnectionString;
        private DataSet dsGL;
        public frmCustomerMaster()
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
        private void label2_Click(object sender, EventArgs e)
        {

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
        private void frmCustomerMaster_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtCreditNo;
            //  GetCRNNo();
        }
        public void GetCRNNo()
        {

            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

               string StrSql = "SELECT TheaterPref, TheaterPad, TheaterNo FROM tblDefualtSetting";

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
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
      
        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            myConnection.Open();
            myTrans = myConnection.BeginTransaction();
            try
            {
               


           //     UpdatePrefixNo(myConnection, myTrans);

                string StrSql = "DELETE FROM [tblCustomerMaster] WHERE CutomerID='" + txtCreditNo.Text.ToString().Trim() + "'";
                SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                command1.CommandType = CommandType.Text;
                command1.ExecuteNonQuery();



                StrSql = "Insert into tblCustomerMaster (CutomerID,CustomerName,Address1,Address2,Phone1,Phone2,Fax,Email,Custom2,Cus_Type,IsActive) values ('" + txtCreditNo.Text.ToString().Trim() + "','" + txtCustomer.Text.ToString().Trim() + "','" + txtad1.Text.ToString().Trim() + "','" + txtad2.Text.ToString().Trim() + "','" + txtTel.Text.ToString().Trim() + "','" + Tel2.Text.ToString().Trim() + "','" + txtFax.Text.ToString().Trim() + "','" + txtEmail.Text.ToString().Trim() + "','" + txtVatNo.Text.ToString().Trim() + "','" + cmbpaytype.Text.ToString().Trim()  + "','"+true+"')";

                SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                myTrans.Commit();
                MessageBox.Show("Successfully Saved");
                btnNew_Click(null, null);
            }
            catch(Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }
            finally
            {
                myConnection.Close();
            }
           
           
        }

        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intCRNNo;
                string StrSql;
                SqlCommand command;

                StrSql = "SELECT  TOP 1(TheaterNo) FROM tblDefualtSetting ORDER BY TheaterNo DESC";

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
                StrSql = "UPDATE tblDefualtSetting SET TheaterNo='" + intCRNNo + "'";

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        bool EditClick = false;
        private void btnedit_Click(object sender, EventArgs e)
        {
            EditClick = true;
            btnSave.Enabled = true;
            btnProcess.Enabled = true;
            EnableFiedl(true);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            EditClick = false;
            GetCRNNo();
            EnableFiedl(true);
            btnedit.Enabled = false;
            btnProcess.Enabled = false;
            btnSave.Enabled = true;
            ClearDetails();
        }

        private void ClearDetails()
        {
            txtCustomer.Text = "";
            txtad1.Text = "";
            txtad2.Text = "";
            txtTel.Text = "";
            Tel2.Text = "";
            txtFax.Text = "";
            txtEmail.Text = "";
            txtVatNo.Text = "";
            cmbpaytype.Text = "";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            frmCustomerList cl = new frmCustomerList();
            cl.ShowDialog();

            if (Search.searchCustomer != "" || Search.searchCustomer != null)
            {
                btnedit.Enabled = true;
                SetValue();
            }
        }

        private void SetValue()
        {

            EnableFiedl(false);
            string ConnString = ConnectionString;
            String S1 = "Select CutomerID,CustomerName,Address1,Address2,Phone1,Phone2,Fax,Email,Custom2,Cus_Type,IsActive from tblCustomerMaster  where CutomerID='" + Search.searchCustomer + "'";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                                                                                                                         // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                txtCreditNo.Text = dt.Rows[0].ItemArray[0].ToString();
                txtCustomer.Text = dt.Rows[0].ItemArray[1].ToString();
                txtad1.Text = dt.Rows[0].ItemArray[2].ToString();
                txtad2.Text = dt.Rows[0].ItemArray[3].ToString();
                txtTel.Text = dt.Rows[0].ItemArray[4].ToString();
                Tel2.Text = dt.Rows[0].ItemArray[5].ToString();
                txtFax.Text = dt.Rows[0].ItemArray[6].ToString();
                txtEmail.Text = dt.Rows[0].ItemArray[7].ToString();
                txtVatNo.Text = dt.Rows[0].ItemArray[8].ToString();
                cmbpaytype.Text = dt.Rows[0].ItemArray[9].ToString();

                if (Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString().Trim()) == true)
                {
                    btnProcess.Enabled = true;
                }
                else
                {
                    btnProcess.Enabled = false;
                }

            }
        }

        private void EnableFiedl(bool v)
        {
            txtCreditNo.Enabled = v;
            txtCustomer.Enabled = v;
            txtad1.Enabled = v;
            txtad2.Enabled = v;
            txtTel.Enabled = v;
            Tel2.Enabled = v;
            txtFax.Enabled = v;
            txtEmail.Enabled = v;
            txtVatNo.Enabled = v;
            cmbpaytype.Enabled = v;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            myConnection.Open();
            myTrans = myConnection.BeginTransaction();
            try
            {
                string StrSql = "DELETE FROM [tblCustomerMaster] WHERE CutomerID='" + txtCreditNo.Text.ToString().Trim() + "'";
                SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                command1.CommandType = CommandType.Text;
                command1.ExecuteNonQuery();



                StrSql = "Insert into tblCustomerMaster (CutomerID,CustomerName,Address1,Address2,Phone1,Phone2,Fax,Email,Custom2,Cus_Type,IsActive) values ('" + txtCreditNo.Text.ToString().Trim() + "','" + txtCustomer.Text.ToString().Trim() + "','" + txtad1.Text.ToString().Trim() + "','" + txtad2.Text.ToString().Trim() + "','" + txtTel.Text.ToString().Trim() + "','" + Tel2.Text.ToString().Trim() + "','" + txtFax.Text.ToString().Trim() + "','" + txtEmail.Text.ToString().Trim() + "','" + txtVatNo.Text.ToString().Trim() + "','" + cmbpaytype.Text.ToString().Trim() + "','"+false+"')";

                SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                myTrans.Commit();

                CreateCustomer();
                Connector Conn = new Connector();
                Conn.ExportCustomerList();

                MessageBox.Show("Successfully Processed");
                btnNew_Click(null, null);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateCustomer()
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
                Writer.WriteString(txtCreditNo.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Name");
                Writer.WriteString(txtCustomer.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Line1");
                Writer.WriteString(txtad1.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Line2");
                Writer.WriteString(txtad2.Text.ToString().Trim());
                Writer.WriteEndElement();

                //  PhoneNumbers>
                // <PhoneNumber Key="1">777801845</PhoneNumber> 
                // </PhoneNumbers>
                Writer.WriteStartElement("PhoneNumbers");

                Writer.WriteStartElement("PhoneNumber");
                Writer.WriteAttributeString("Key", "1");
                // Writer.WriteStartElement("TelePhone_1");
                Writer.WriteString(txtTel.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("PhoneNumber");
                Writer.WriteAttributeString("Key", "2");
                // Writer.WriteStartElement("TelePhone_1");
                Writer.WriteString(Tel2.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteEndElement();
                // Writer.WriteStartElement("PhoneNumbers");

                Writer.WriteStartElement("Customer_Type");
                Writer.WriteString(cmbpaytype.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("FaxNumber");
                Writer.WriteString(txtFax.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("EMail_Address");
                Writer.WriteString(txtEmail.Text.ToString().Trim());
                Writer.WriteEndElement();

                //CustomFields
                Writer.WriteStartElement("CustomFields");
                // Writer.WriteStartElement("CustomField1");

                Writer.WriteStartElement("CustomField");
                Writer.WriteStartElement("Value");
                Writer.WriteAttributeString("Index ", "1");//Change time and date both
                Writer.WriteString(cmbpaytype.Text.ToString().Trim());
                Writer.WriteEndElement();
                Writer.WriteEndElement();

                Writer.WriteStartElement("CustomField");
                Writer.WriteStartElement("Value");
                Writer.WriteAttributeString("Index ", "2");//Change time and date both
                Writer.WriteString(txtVatNo.Text.ToString().Trim());
                Writer.WriteEndElement();
                Writer.WriteEndElement();


                Writer.WriteEndElement();

                Writer.WriteEndElement();
                Writer.WriteEndElement();
                Writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


       


    }
}
