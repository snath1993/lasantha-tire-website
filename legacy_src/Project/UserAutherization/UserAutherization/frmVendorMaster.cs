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
    public partial class frmVendorMaster : Form
    {
        private string ConnectionString;
        public frmVendorMaster()
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
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        bool _IsEdit = false;
        private void frmVendorMaster_Load(object sender, EventArgs e)
        {
            _IsEdit = false;
        }
        private void LoadCustomFields()
        {
            try
            {

                String StrSql = "SELECT * FROM tbl_VendorMasterCostomizeFields";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        lblCustom1.Text = dt.Rows[i].ItemArray[0].ToString().Trim();
                        lblCustom2.Text = dt.Rows[i].ItemArray[1].ToString().Trim();
                        lblCustom3.Text = dt.Rows[i].ItemArray[2].ToString().Trim();
                        lblCustom4.Text = dt.Rows[i].ItemArray[3].ToString().Trim();
                        lblCustom5.Text = dt.Rows[i].ItemArray[4].ToString().Trim();
                       

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmItemMasterCustomFields imcf = new frmItemMasterCustomFields();
            frmMain.CostomizeFormName = "VendorMaster";
            imcf.ShowDialog();
            LoadCustomFields();
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
                if (_IsEdit)
                {
                    string StrSql = "DELETE FROM [tblVendorMaster] WHERE VendorID='" + txtVendorID.Text.ToString().Trim() + "'";
                    SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                    command1.CommandType = CommandType.Text;
                    command1.ExecuteNonQuery();
                }


                string StrSql2 = "Insert into tblVendorMaster (VendorID,VendorName,VAddress1,VAddress2,VContact,VContact2,Fax,CustomField1,CustomField2,CustomField3,CustomField4,CustomField5,IsActive) values ('" + txtVendorID.Text.ToString().Trim() + "','" + txtVendorName.Text.ToString().Trim() + "','" + txtAdress1.Text.ToString().Trim() + "','" + txtAddress2.Text.ToString().Trim() + "','" + txtTel1.Text.ToString().Trim() + "','" + txtTel2.Text.ToString().Trim() + "','" + txtFax.Text.ToString().Trim() + "','" + txtC1.Text.ToString().Trim() + "','" + txtc2.Text.ToString().Trim() + "','" + txtC3.Text.ToString().Trim() + "','" + txtC4.Text.ToString().Trim() + "','" + txtC5.Text.ToString().Trim() + "','"+true+"')";

                SqlCommand command = new SqlCommand(StrSql2, myConnection, myTrans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                myTrans.Commit();
                MessageBox.Show("Successfully Saved");
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }
            finally
            {
                myConnection.Close();
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            EnableFiedl(true);
            _IsEdit = false;
            btnedit.Enabled = false;
            btnProcess.Enabled = false;
            btnSave.Enabled = true;
            ClearDetails();
            txtVendorID.Enabled = true;
        }

        private void ClearDetails()
        {
            txtVendorID.Text = "";
            txtVendorName.Text = "";
            txtAdress1.Text = "";
            txtAddress2.Text = "";
            txtTel1.Text = "";
            txtTel2.Text = "";
            txtFax.Text = "";
            txtC1.Text = "";
            txtc2.Text = "";
            txtC3.Text = "";
            txtC4.Text = "";
            txtC5.Text = "";
        }

        private void EnableFiedl(bool v)
        {
            //txtVendorID.Enabled = v;
            txtVendorName.Enabled = v;
            txtAdress1.Enabled = v;
            txtAddress2.Enabled = v;
            txtTel1.Enabled = v;
            txtTel2.Enabled = v;
            txtFax.Enabled = v;
            txtC1.Enabled = v;
            txtc2.Enabled = v;
            txtC3.Enabled = v;
            txtC4.Enabled = v;
            txtC5.Enabled = v;
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            btnedit.Enabled = false;
            btnSave.Enabled = true;
            btnProcess.Enabled = false;
            _IsEdit = true;
            EnableFiedl(true);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            frmVendorMasterList cl = new frmVendorMasterList();
            cl.ShowDialog();

            if (Search.VendorSearch != "" || Search.VendorSearch != null)
            {
                btnedit.Enabled = true;
                btnSave.Enabled = false;
                btnProcess.Enabled = true;
                txtVendorID.Enabled = false;
                SetValue();
            }
        }

        private void SetValue()
        {

            EnableFiedl(false);
            string ConnString = ConnectionString;
            String S1 = "Select VendorID,VendorName,VAddress1,VAddress2,VContact,VContact2,Fax,CustomField1,CustomField5,CustomField3,CustomField4,CustomField5,IsActive from tblVendorMaster  where VendorID='" + Search.VendorSearch + "'";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                                                                                                                                                                                                    // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                txtVendorID.Text = dt.Rows[0].ItemArray[0].ToString();
                txtVendorName.Text = dt.Rows[0].ItemArray[1].ToString();
                txtAdress1.Text = dt.Rows[0].ItemArray[2].ToString();
                txtAddress2.Text = dt.Rows[0].ItemArray[3].ToString();
                txtTel1.Text = dt.Rows[0].ItemArray[4].ToString();
                txtTel2.Text = dt.Rows[0].ItemArray[5].ToString();
                txtFax.Text = dt.Rows[0].ItemArray[6].ToString();
                txtC1.Text = dt.Rows[0].ItemArray[7].ToString();
                txtc2.Text = dt.Rows[0].ItemArray[8].ToString();
                txtC3.Text = dt.Rows[0].ItemArray[9].ToString();
                txtC4.Text = dt.Rows[0].ItemArray[10].ToString();
                txtC5.Text = dt.Rows[0].ItemArray[11].ToString();

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
        //    SqlConnection myConnection = new SqlConnection(ConnectionString);
        //    SqlTransaction myTrans = null;

        //    myConnection.Open();
            //myTrans = myConnection.BeginTransaction();
            try
            {
                //string StrSql = "DELETE FROM [tblVendorMaster] WHERE VendorID='" + txtVendorID.Text.ToString().Trim() + "'";
                //SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                //command1.CommandType = CommandType.Text;
                //command1.ExecuteNonQuery();

                //StrSql = "Insert into tblVendorMaster (VendorID,VendorName,VAddress1,VAddress2,VContact,VContact2,Fax,CustomField1,CustomField2,CustomField3,CustomField4,CustomField5,IsActive) values ('" + txtVendorID.Text.ToString().Trim() + "','" + txtVendorName.Text.ToString().Trim() + "','" + txtAdress1.Text.ToString().Trim() + "','" + txtAddress2.Text.ToString().Trim() + "','" + txtTel1.Text.ToString().Trim() + "','" + txtTel2.Text.ToString().Trim() + "','" + txtFax.Text.ToString().Trim() + "','" + txtC1.Text.ToString().Trim() + "','" + txtc2.Text.ToString().Trim() + "','" + txtC3.Text.ToString().Trim() + "','" + txtC4.Text.ToString().Trim() + "','" + txtC5.Text.ToString().Trim() + "','" + false + "')";

                //SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                //command.CommandType = CommandType.Text;
                //command.ExecuteNonQuery();

                //myTrans.Commit();

                CreateVendor();
                Connector Conn = new Connector();
                Conn.ExportVendorList();

                MessageBox.Show("Successfully Processed");
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateVendor()
        {
            try
            {
                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\Vendor.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Vendor");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Vendor");
                Writer.WriteAttributeString("xsi:type", "paw:vendor");

                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(txtVendorID.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Name");
                Writer.WriteString(txtVendorName.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Line1");
                Writer.WriteString(txtAdress1.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Line2");
                Writer.WriteString(txtAddress2.Text.ToString().Trim());
                Writer.WriteEndElement();

                //  PhoneNumbers>
                // <PhoneNumber Key="1">777801845</PhoneNumber> 
                // </PhoneNumbers>
                Writer.WriteStartElement("PhoneNumbers");

                Writer.WriteStartElement("PhoneNumber");
                Writer.WriteAttributeString("Key", "1");
                // Writer.WriteStartElement("TelePhone_1");
                Writer.WriteString(txtTel1.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("PhoneNumber");
                Writer.WriteAttributeString("Key", "2");
                // Writer.WriteStartElement("TelePhone_1");
                Writer.WriteString(txtTel2.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteEndElement();
                // Writer.WriteStartElement("PhoneNumbers");

                Writer.WriteStartElement("FaxNumber");
                Writer.WriteString(txtFax.Text.ToString().Trim());
                Writer.WriteEndElement();

                //CustomFields
                Writer.WriteStartElement("CustomFields");
                // Writer.WriteStartElement("CustomField1");

                Writer.WriteStartElement("CustomField");
                Writer.WriteStartElement("Value");
                Writer.WriteAttributeString("Index ", "1");//Change time and date both
                Writer.WriteString(txtC1.Text.ToString().Trim());
                Writer.WriteEndElement();
                Writer.WriteEndElement();

                Writer.WriteStartElement("CustomField");
                Writer.WriteStartElement("Value");
                Writer.WriteAttributeString("Index ", "2");//Change time and date both
                Writer.WriteString(txtc2.Text.ToString().Trim());
                Writer.WriteEndElement();
                Writer.WriteEndElement();

                Writer.WriteStartElement("CustomField");
                Writer.WriteStartElement("Value");
                Writer.WriteAttributeString("Index ", "3");//Change time and date both
                Writer.WriteString(txtC3.Text.ToString().Trim());
                Writer.WriteEndElement();
                Writer.WriteEndElement();

                Writer.WriteStartElement("CustomField");
                Writer.WriteStartElement("Value");
                Writer.WriteAttributeString("Index ", "4");//Change time and date both
                Writer.WriteString(txtC4.Text.ToString().Trim());
                Writer.WriteEndElement();
                Writer.WriteEndElement();

                Writer.WriteStartElement("CustomField");
                Writer.WriteStartElement("Value");
                Writer.WriteAttributeString("Index ", "5");//Change time and date both
                Writer.WriteString(txtC5.Text.ToString().Trim());
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
