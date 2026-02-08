using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace UserAutherization
{
    public partial class Registration2 : Form
    {
        Registration R = new Registration();
        public Registration2()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;
        DataAccess.clsCommon objclsCommon1 = new DataAccess.clsCommon();

        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // string name="";

            String S4 = "select RtKey from tblRtKeyData";// where UserID = '" + txtUserName.Text.ToString().Trim() + "'";
            SqlCommand cmd4 = new SqlCommand(S4);
            SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
            DataTable dt4 = new DataTable();
            da4.Fill(dt4);
            if (objclsCommon1.decryptPassword(Convert.ToString(dt4.Rows[0].ItemArray[0])) == "sage@pbssaddonn1983")
            {
                MessageBox.Show("You have Already Register the Product");
                return;
            }
            Registration3 R3 = new Registration3();
            //R3.Show();
            // this.Close();
            // R.Getname(name);
            R3.Show();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Registration2_Load(object sender, EventArgs e)
        {

        }
    }
}