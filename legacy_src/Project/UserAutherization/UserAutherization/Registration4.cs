using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;

namespace UserAutherization
{

    public partial class Registration4 : Form
    {
        //this is the serial number that is user enter to the system

        public Registration4()
        {
            InitializeComponent();
        }
        public string ProducCode = "";
        public string BCompanyName = "";

        public static string ConnectionString;
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Registration3 R3 = new Registration3();
            R3.Show();
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {

            if (txtBaseSerialNO.Text.ToString().Trim() != "84DED-4D6A-65F5-69E7")
            {
                MessageBox.Show("The Product Code that you have Entered is not a Valid Sage 50 ProductCode");
                return;
            }
            if (txtBaseCompanyName.Text.ToString().Trim() ==String.Empty)
            {
                MessageBox.Show("Please Enter the Company Name");
                return;
            }

            //int a = 1;
            //try
            //{
            //    Convert.ToDouble(txtBaseSerialNO.Text);
            //}
            //catch 
            //{
            //    a = 2;
            //}

            //if (txtBaseCompanyName.Text == " "  || txtBaseSerialNO.Text == " " || a == 2 )
            //{
            //    //string name = "";
            //    //Registration5 R5 = new Registration5();
            //    //Registration R = new Registration();
            //    //GetCompanyName();
            //    //GeSerialNO();
            //    //UpdateCompanyName(companyname, serialno);
            //    //R.Getname(name);
            //    //R5.Show();
            //    //ProducCode = serialno;
            //    //BCompanyName = companyname;
            //    //this.Close();

            //    MessageBox.Show("please enter ProductCode and CompanyName");
            //    btnNext.Focus();
            //}
            //else
            //{
            try
            {
                string name = "";
                Registration5 R5 = new Registration5();
                Registration R = new Registration();
                GetCompanyName();
                GeSerialNO();
                UpdateCompanyName(companyname, serialno);
                R.Getname(name);
                R5.Show();
                ProducCode = serialno;
                BCompanyName = companyname;
                this.Close();
            }
            catch { }

                //MessageBox.Show("please enter ProductCode and CompanyName");
                //btnNext.Focus();
            //}
        }

        private void Registration4_Load(object sender, EventArgs e)
        {
            txtBaseCompanyName.Text = BCompanyName;
            txtBaseSerialNO.Text = ProducCode;

        }
        public string serialno = "";
        public string companyname = "";
        public string GeSerialNO()
        {
            serialno = txtBaseSerialNO.Text;
            return serialno;
        }

        public string GetCompanyName()
        {
            companyname = txtBaseCompanyName.Text;
            return companyname;
        }

        #region Update_CompanyName
        public void UpdateCompanyName(string BaseCompanyName, string BaseSerialNumber)
        {
            setConnectionString();
            try
            {
                string ConnString = ConnectionString;
                // SqlConnection 
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = Conn.CreateCommand();
                cmd.CommandText = "INSERT INTO tblTempComData (CompanyName,SerialNumber) VALUES ('" + BaseCompanyName + "','" + BaseSerialNumber + "')";
                // "INSERT INTO SalesOrder(SONO) VALUES('" + SONO + "')";
                Conn.Open();
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            catch
            { }
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {

            Registration3 R3 = new Registration3();
            R3.Show();
            this.Close();
        }


    }

}