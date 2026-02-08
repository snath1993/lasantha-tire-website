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
    public partial class Registration5 : Form
    {
       // public string  Islogin = "";
        public string companyname1 = "";
        public string Registrationcode = "";
        public string SerialNumber2 = "";
        public string SerialNumber3 = "";//this is the one customer sent to us

        public Registration5()
        {
            InitializeComponent();
        }
        public static string ConnectionString;
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            
        }




        private void Registration5_Load(object sender, EventArgs e)
        {

            GetSerialNO();
            GetRegCode();
           // Registrationcode;
           // txtReistrationCode.Text = Registrationcode;
            lblAcessCode.Text = SerialNumber3;
            
        }


        private void btnBack_Click(object sender, EventArgs e)
        {

            Registration4 R4 = new Registration4();
            R4.BCompanyName =companyname1;
            R4.ProducCode = SerialNumber2;
            R4.Show();
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Regisration6 R6 = new Regisration6();
           // R6.Show();
           // this.Close();
            Registrationcode = "A19830429M2345P2B32S";
            if (txtReistrationCode.Text == Registrationcode)
            {
                // MessageBox.Show("Incorect Registration Code");
                R6.Show();
                this.Close();
                //TrialVersion.TrialFailed = false;
                //Login Userlog = new Login();
                //Userlog.Show();

              //  Islogin =" true";
                 updateTrial();
            }

            else
            {
                MessageBox.Show("Incorect Registration Code");
                return;
            }
           // R6.Show();
            //this.Close();

        }

        //.......................................

        //.................................................

        #region UpdateTrial
        public void updateTrial()
        {
            setConnectionString();
            try
            {
                string RTKey = "Iv0ys3FzCODs6aoawXFX03CgIimGmjSV";
                string ConnString = ConnectionString;// @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ImportCosting.mdb";
                SqlConnection Conn = new   SqlConnection(ConnString);
                SqlCommand cmd = Conn.CreateCommand();
                // cmd.CommandText = "INSERT INTO Trial (Trial) VALUES ('" + Islogin + "','" + BaseSerialNumber + "')";
                //cmd.CommandText="update  Trial set Trial=('" + true + "')";
                  //select RtKey from tblRtKeyData
                cmd.CommandText = "update tblRtKeyData set RtKey='" + RTKey  + "'";//     '0 ' where Trial='1'"; // or Trial='true'"; //and  Trial=('" + Islogin + "')";
                Conn.Open();
                // "INSERT INTO SalesOrder(SONO) VALUES('" + SONO + "')";
                // Conn.Open();
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            catch
            { }
        }
        #endregion

        ////.......................................................




        #region Getserial number
        public void GetSerialNO()
        {
            setConnectionString();
            try
            {
                string ConnString = ConnectionString;
                string sql = "select CompanyName,SerialNumber from tblTempComData"; //where Company_Name ='" + CompanyName + "'";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        companyname1 = (reader.GetValue(0).ToString().Trim());
                        SerialNumber2 = (reader.GetValue(1).ToString().Trim());
                    }
                }
                reader.Close();
                Conn.Close();

            }
            catch
            { }
            // }
            //catch
            //{ }
        }
        #endregion


        //.........................................

       
        // get reistration code


        #region Get registration code
        public void GetRegCode()
        {
            setConnectionString();
            try
            {
                string ConnString = ConnectionString;
                string sql = "select RegCode,SerialNumber from RT"; //where Company_Name ='" + CompanyName + "'";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Registrationcode = (reader.GetValue(0).ToString().Trim());
                        SerialNumber3 = (reader.GetValue(1).ToString().Trim());                       
                    }
                }
                reader.Close();
                Conn.Close();

            }
            catch
            { }
            // }
            //catch
            //{ }
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {


            Registration4 R4 = new Registration4();
            R4.Close();
            this.Show();
        }



        //...................................................






    }
}