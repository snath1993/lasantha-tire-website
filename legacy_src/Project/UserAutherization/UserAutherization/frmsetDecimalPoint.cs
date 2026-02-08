using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;
using System.Xml;
using System.Collections;
using System.Threading;

namespace UserAutherization
{
    public partial class frmsetDecimalPoint : Form
    {
        public frmsetDecimalPoint()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;

            }
            catch { }
        }

        private void frmsetDecimalPoint_Load(object sender, EventArgs e)
        {
            try
            {
                string Ftype = "Quantity";
                String S = "Select CurentDecimal from tblDecimal where FieldType='" + Ftype + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                       // btnSet.Enabled = false;
                        lbcurrenrDpoint.Text = "No of Decimal to be Assigned";
                    }
                    else
                    {
                        // cmbDecimal.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                        btnSet.Enabled = false;
                        lbcurrenrDpoint.Text = dt.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                    }
                //}

                //===========================

                    string Ftype1 = "Price";
                    String S1 = "Select CurentDecimal from tblDecimal where FieldType='" + Ftype1 + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet dt1 = new DataSet();
                    da1.Fill(dt1);

                    //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    //{
                    if (dt1.Tables[0].Rows.Count == 0)
                    {
                        // btnSet.Enabled = false;
                        lblCurentforPrice.Text = "No of Decimal to be Assigned";
                    }
                    else
                    {
                        // cmbDecimal.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                        btnsetforPrice.Enabled = false;
                        lblCurentforPrice.Text = dt1.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                    }
            }
            catch { }

        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (cmbDecimal.Text == "")
            {
                MessageBox.Show("You must Select a Value");
            }
            else
            {
                string Ftype = "Quantity";
                string ConnString = ConnectionString;
                String S2 = "insert into tblDecimal(FieldType,CurentDecimal) values ('" + Ftype + "','" + cmbDecimal.Text.ToString().Trim() + "')";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
                MessageBox.Show("Set Successfully");
                frmsetDecimalPoint_Load(sender, e);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (cmbDecimal.Text == "")
            {
                MessageBox.Show("You must Select a Value");
            }
            else
            {
                string Ftype = "Quantity";
                string ConnString = ConnectionString;
                String S2 = "Update tblDecimal SET CurentDecimal='" + cmbDecimal.Text.ToString().Trim() + "' where FieldType='" + Ftype + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
                MessageBox.Show("Update Successfully");
                frmsetDecimalPoint_Load(sender, e);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnsetforPrice_Click(object sender, EventArgs e)
        {
            if (cmbDecimal1.Text == "")
            {
                MessageBox.Show("You must Select a Value");
            }
            else
            {
                string Ftype = "Price";
                string ConnString = ConnectionString;
                String S2 = "insert into tblDecimal(FieldType,CurentDecimal) values ('" + Ftype + "','" + cmbDecimal1.Text.ToString().Trim() + "')";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
                MessageBox.Show("Set Successfully");
                frmsetDecimalPoint_Load(sender, e);
            }
        }

        private void btnChangeforPrice_Click(object sender, EventArgs e)
        {
            if (cmbDecimal1.Text == "")
            {
                MessageBox.Show("You must Select a Value");
            }
            else
            {
                string Ftype = "Price";
                string ConnString = ConnectionString;
                String S2 = "Update tblDecimal SET CurentDecimal='" + cmbDecimal1.Text.ToString().Trim() + "' where FieldType='" + Ftype + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
                MessageBox.Show("Update Successfully");
                frmsetDecimalPoint_Load(sender, e);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}