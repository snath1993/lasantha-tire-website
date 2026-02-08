using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Perfect_Hospital_Management_System
{
    public partial class frmChannelingList : Form
    {
        int flg3 = 0; // for consultant
        int flg4 = 0;
        public static string ConnectionString;
        Class1 a = new Class1();

        public frmChannelingList()
        {
            try
            {
                InitializeComponent();
                setConnectionString();

                string ConnString1 = ConnectionString;
                String S11 = "Select ConsultantCode, ConsultantName from tblConsultantMaster where Block = 'False'";
                SqlCommand cmd11 = new SqlCommand(S11);
                SqlDataAdapter da11 = new SqlDataAdapter(S11, ConnectionString);
                DataTable dt11 = new DataTable();
                da11.Fill(dt11);

                if (dt11.Rows.Count > 0)
                {
                    for (int i = 0; i < dt11.Rows.Count; i++)
                    {
                        ListViewItem item1 = this.lstvConsultant.Items.Add(dt11.Rows[i].ItemArray[0].ToString());
                        item1.SubItems.Add(dt11.Rows[i].ItemArray[1].ToString());
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

        private void btnConsultant_Click(object sender, EventArgs e)
        {
            try
            {
                if (flg3 == 0)
                {
                    lstvConsultant.BringToFront();
                    lstvConsultant.Height = 175;
                    this.lstvConsultant.Location = new System.Drawing.Point(73, 38);
                    lstvConsultant.Visible = true;
                    flg3 = 1;
                }
                else
                {
                    lstvConsultant.Visible = false;
                    flg3 = 0;
                }
            }
            catch { }
        }


        private void lstvConsultant_Click(object sender, EventArgs e)
        {
            try
            {
                string item = lstvConsultant.SelectedItems[0].Text;
                if (flg3 == 0)
                {
                    txtConsultant.Text = item;
                }
                if (flg3 == 1)
                {
                    txtConsultant.Text = item;
                }
                lstvConsultant.Visible = false;
                flg3 = 0;
            }
            catch { }
        }

        private void txtConsultant_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtRoom.Text.ToString().Trim() == "")
                {
                    String S1 = "Select ConsultantType from tblConsultantMaster where ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "' AND Block = 'False'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        txtRoom.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString();
                        flg4 = 1;
                    }
                }
            }
            catch { }
        }

        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (flg4 == 1)
                {
                    txtConsultant.Text = "";
                    // ClearText();
                    string ConnString = ConnectionString;
                    String S1 = "Select ConsultantCode, ConsultantName from tblConsultantMaster where Category = '" + txtRoom.Text.ToString().Trim() + "' AND Block = 'False'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);
                }
            }
            catch { }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                //04/01/2009

                string ConnString = ConnectionString;
               // String S1 = "Select TokenNo, PatientNo, PName, Status from tblChannelingList where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (CDate = '" + dtpDateFrom.Text.ToString().Trim() + "') AND (Session = '" + cmbSession.Text.ToString().Trim() + "') ORDER BY TokenNo";
                String S1 = "Select ReceiptsNo,ConsultantName,FirstName,TokenNo from tblChannelingDetails where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (RepDate = '" + dtpDateFrom.Text.ToString().Trim() + "') ORDER BY ReceiptsNo";
                SqlCommand cmd1 = new SqlCommand(S1);//tblChannelingDetails
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
  
                dgvChannelingList.DataSource = dt;
                dgvChannelingList.Columns[0].HeaderText = "ReceiptNo";
                dgvChannelingList.Columns[0].Width = 100;
                dgvChannelingList.Columns[1].HeaderText = "Consultant";
                dgvChannelingList.Columns[1].Width = 180;
                dgvChannelingList.Columns[2].HeaderText = "Patient Name";
                dgvChannelingList.Columns[2].Width = 250;
                dgvChannelingList.Columns[3].HeaderText = "Channeling NO";
                dgvChannelingList.Columns[3].Width = 150;

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvChannelingList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvChannelingList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvChannelingList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvChannelingList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                    }
                }
            }
            catch { }
        }



        private void LoadChannelingList()
        {
            try
            {
                //04/01/2009

                string ConnString = ConnectionString;
                // String S1 = "Select TokenNo, PatientNo, PName, Status from tblChannelingList where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (CDate = '" + dtpDateFrom.Text.ToString().Trim() + "') AND (Session = '" + cmbSession.Text.ToString().Trim() + "') ORDER BY TokenNo";
                String S1 = "Select ReceiptsNo,ConsultantName,FirstName,TokenNo from tblChannelingDetails where (ReceiptsNo !='CH-100000') ORDER BY ReceiptsNo";
                SqlCommand cmd1 = new SqlCommand(S1);//tblChannelingDetails
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                dgvChannelingList.DataSource = dt;
                dgvChannelingList.Columns[0].HeaderText = "ReceiptNo";
                dgvChannelingList.Columns[0].Width = 100;
                dgvChannelingList.Columns[1].HeaderText = "Consultant";
                dgvChannelingList.Columns[1].Width = 180;
                dgvChannelingList.Columns[2].HeaderText = "Patient Name";
                dgvChannelingList.Columns[2].Width = 250;
                dgvChannelingList.Columns[3].HeaderText = "Channeling NO";
                dgvChannelingList.Columns[3].Width = 150;

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i!=0)
                        {
                            dgvChannelingList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvChannelingList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dgvChannelingList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dgvChannelingList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                       }
                    }
                }
            }
            catch { }


        }


        private void btnConsultant_Click_1(object sender, EventArgs e)
        {
            try
            {
                //Class1.formto = "Form2";
                //Class1.textto = "textBox1.Text";

                frmSelectConsult frm = new frmSelectConsult();
                frm.Show();
            }
            catch { }
            //try
            //{
            //    if (flg3 == 0)
            //    {
            //        lstvConsultant.BringToFront();
            //        lstvConsultant.Height = 175;
            //        this.lstvConsultant.Location = new System.Drawing.Point(37, 51);
            //        lstvConsultant.Visible = true;
            //        flg3 = 1;
            //    }
            //    else
            //    {
            //        lstvConsultant.Visible = false;
            //        flg3 = 0;
            //    }
            //}
            //catch { }
        }

        private void lstvConsultant_Click_1(object sender, EventArgs e)
        {
            try
            {
                string item = lstvConsultant.SelectedItems[0].SubItems[1].Text;
                if (flg3 == 0)
                {
                    txtConsultant.Text = item;
                }
                if (flg3 == 1)
                {
                    txtConsultant.Text = item;
                }
                lstvConsultant.Visible = false;
                flg3 = 0;
            }
            catch { }
        }

        private void dtpDateFrom_ValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    cmbSession.Text = "";
            //    cmbSession.Items.Clear();
            //    DateTime dt = dtpDateFrom.Value;
            //    string ADay = dt.DayOfWeek.ToString();
            //    // MessageBox.Show(dt.DayOfWeek.ToString());

            //    string ConnString = ConnectionString;
            //    String S1 = "Select SessionTime from tblSchedulingDetails where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (ADay = '" + ADay + "')";
            //    SqlCommand cmd1 = new SqlCommand(S1);
            //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //    DataSet ds1 = new DataSet();
            //    da1.Fill(ds1);
            //    if (ds1.Tables[0].Rows.Count > 0)
            //    {
            //        for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            //        {
            //            cmbSession.Items.Add(ds1.Tables[0].Rows[i].ItemArray[0].ToString());
            //        }
            //    }
            //}
            //catch { }
            btnRefresh_Click(sender, e);

        }

        private void cmbSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    //Auto fill room no
            //    String S3 = "Select CRoomNo from tblChannelingRooms where (ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "') AND (SessionTime = '" + cmbSession.Text.ToString().Trim() + "')";
            //    SqlCommand cmd3 = new SqlCommand(S3);
            //    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
            //    DataSet ds3 = new DataSet();
            //    da3.Fill(ds3);
            //    if (ds3.Tables[0].Rows.Count > 0)
            //    {
            //        txtRoom.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString();
            //    }

            //    btnRefresh_Click_1(sender, e);
            //}
            //catch { }
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            try
            {
                btnRefresh_Click(sender, e);

               
            }
            catch { }
            
        }

        private void dgvtblChannelingList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                frmAdmissionDetails frmadd = new frmAdmissionDetails(this);
                frmadd.Show();
            }
            catch { }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch { }
        }

        private void frmtblChannelingList_Load(object sender, EventArgs e)
        {
            //DateTime ab = System.DateTime.Now;
            //int sec = ab.Second;
            //while (sec < 5)
            //{
                LoadChannelingList();
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //btnRefresh_Click(sender, e);

        }

        private void frmChannelingList_Activated(object sender, EventArgs e)
        {
            if (Class1.flg == 1)
            {
                //textBox1.Text = Class1.myvalue;
                txtConsultant.Text = a.GetText();
                Class1.flg = 0;
            }
        }

        private void txtRoom_TextChanged(object sender, EventArgs e)
        {

        }


    }
}