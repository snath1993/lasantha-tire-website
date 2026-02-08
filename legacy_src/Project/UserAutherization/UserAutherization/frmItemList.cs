using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization

{
    public partial class frmItemList : Form
    {
        public static string ConnectionString;
        Class1 a = new Class1();

        public frmItemList()
        {
            InitializeComponent();
            setConnectionString();
        }

        //Method to establish the connection
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

        private void frmItemList_Load(object sender, EventArgs e)
        {
            try
            {
                string type = DataAccess.Access.type;
                if (!(type == "WARD" || type == "OPD" || type == "ETU"))
                {
                    string abc = DataAccess.Access.Consultant;
                    string ConnString = ConnectionString;
                    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (ItemType = '" + DataAccess.Access.type + "') AND (Consultant= '" + DataAccess.Access.Consultant + "')";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    dgvItemList.DataSource = ds1.Tables[0];
                    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    dgvItemList.Columns[0].Width = 200;
                    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    dgvItemList.Columns[1].Width = 200;
                    dgvItemList.Columns[2].HeaderText = "Duration";
                    dgvItemList.Columns[2].Width = 200;

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    }

                }
                else if (DataAccess.Access.type == "WARD" || DataAccess.Access.type == "OPD" || DataAccess.Access.type == "ETU")
                {
                    string abc = DataAccess.Access.Consultant;
                    string ConnString = ConnectionString;
                    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (Consultant= '" + DataAccess.Access.Consultant + "')";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    dgvItemList.DataSource = ds1.Tables[0];
                    dgvItemList.Columns[0].HeaderText ="Type No";
                    dgvItemList.Columns[0].Width = 200;
                    dgvItemList.Columns[1].HeaderText = "Type Name";
                    dgvItemList.Columns[1].Width = 200;
                    dgvItemList.Columns[2].HeaderText = "Duration";
                    dgvItemList.Columns[2].Width = 200;

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    }
                }
                    //if (DataAccess.Access.type == "SCAN")
                    //// string abc = DataAccess.Access.Consultant;
                    //{
                    //    string ConnString = ConnectionString;
                    //    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (ItemType = '" + DataAccess.Access.type + "') AND (Consultant= '" + DataAccess.Access.Consultant + "')";
                    //    SqlCommand cmd1 = new SqlCommand(S1);
                    //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    //    DataSet ds1 = new DataSet();
                    //    da1.Fill(ds1);

                    //    dgvItemList.DataSource = ds1.Tables[0];
                    //    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    //    dgvItemList.Columns[0].Width = 200;
                    //    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    //    dgvItemList.Columns[1].Width = 200;
                    //    dgvItemList.Columns[2].HeaderText = "Duration";
                    //    dgvItemList.Columns[2].Width = 200;

                    //    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    //    {
                    //        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    //        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    //        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    //    }
                    //}
                    //if (DataAccess.Access.type == "X-Ray")
                    //{

                    //    string ConnString = ConnectionString;
                    //    string sss = "Xray";
                    //    //String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where ItemType = '" + DataAccess.Access.type + "'";
                    //    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (ItemType = '" + DataAccess.Access.type + "') AND (Consultant= '" + DataAccess.Access.Consultant + "')";
                    //    SqlCommand cmd1 = new SqlCommand(S1);
                    //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    //    DataSet ds1 = new DataSet();
                    //    da1.Fill(ds1);

                    //    dgvItemList.DataSource = ds1.Tables[0];
                    //    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    //    dgvItemList.Columns[0].Width = 200;
                    //    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    //    dgvItemList.Columns[1].Width = 200;
                    //    dgvItemList.Columns[2].HeaderText = "Duration";
                    //    dgvItemList.Columns[2].Width = 200;

                    //    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    //    {
                    //        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    //        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    //        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    //    }


                    //}

                    ////..labtest

                    //if (DataAccess.Access.type == "Lab")
                    //{

                    //    string ConnString = ConnectionString;
                    //    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (ItemType = '" + DataAccess.Access.type + "') AND (Consultant= '" + DataAccess.Access.Consultant + "')";

                    //    // String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where ItemType = '" + DataAccess.Access.type + "'";
                    //    SqlCommand cmd1 = new SqlCommand(S1);
                    //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    //    DataSet ds1 = new DataSet();
                    //    da1.Fill(ds1);

                    //    dgvItemList.DataSource = ds1.Tables[0];
                    //    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    //    dgvItemList.Columns[0].Width = 200;
                    //    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    //    dgvItemList.Columns[1].Width = 200;
                    //    dgvItemList.Columns[2].HeaderText = "Duration";
                    //    dgvItemList.Columns[2].Width = 200;

                    //    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    //    {
                    //        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    //        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    //        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    //    }


                    //}
                    ////.............

                    ////..etu

                    //if (DataAccess.Access.type == "Etu")
                    //{

                    //    string ConnString = ConnectionString;
                    //    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (ItemType = '" + DataAccess.Access.type + "') AND (Consultant= '" + DataAccess.Access.Consultant + "')";

                    //    // String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where ItemType = '" + DataAccess.Access.type + "'";
                    //    SqlCommand cmd1 = new SqlCommand(S1);
                    //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    //    DataSet ds1 = new DataSet();
                    //    da1.Fill(ds1);

                    //    dgvItemList.DataSource = ds1.Tables[0];
                    //    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    //    dgvItemList.Columns[0].Width = 200;
                    //    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    //    dgvItemList.Columns[1].Width = 200;
                    //    dgvItemList.Columns[2].HeaderText = "Duration";
                    //    dgvItemList.Columns[2].Width = 200;

                    //    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    //    {
                    //        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    //        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    //        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    //    }


                    //}
                    ////.............



                }
            catch { }
        }

        private void dgvItemList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // if (ac != 1)
            // {
            string val = dgvItemList[0, dgvItemList.CurrentRow.Index].Value.ToString().Trim();
            Class1.myvalue = a.GetNext2(val);
            this.Close();


            // }
        }

        private void dgvItemList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyValue == 13)
                {
                    string val = dgvItemList[1, dgvItemList.CurrentRow.Index].Value.ToString().Trim();
                    Class1.myvalue = a.GetNext2(val);
                    this.Close();
                }
            }
            catch { }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    string val = dgvItemList[1, dgvItemList.CurrentRow.Index].Value.ToString().Trim();
            //    Class1.myvalue = a.GetNext2(val);
            this.Close();
            //}

            //catch 
            //{
            //    MessageBox.Show("Please set a Doctor from HouseKeeping");

            //}
        }

        private void dgvItemList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string abc = DataAccess.Access.Consultant;
                string add = txtSearch.Text;
                if (DataAccess.Access.type == "SCAN")
                // string abc = DataAccess.Access.Consultant;
                {
                    string ConnString = ConnectionString;
                    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (ItemType = '" + DataAccess.Access.type + "') AND (Consultant= '" + DataAccess.Access.Consultant + "') AND ItemDescription LIKE  '" + add + "%'";
                    // String S = "Select ConsultantCode,Title,ConsultantName from tblConsultantMaster where ConsultantName LIKE  '" + add + "%'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    dgvItemList.DataSource = ds1.Tables[0];
                    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    dgvItemList.Columns[0].Width = 200;
                    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    dgvItemList.Columns[1].Width = 200;
                    dgvItemList.Columns[2].HeaderText = "Duration";
                    dgvItemList.Columns[2].Width = 200;

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    }
                }
                if (DataAccess.Access.type == "XRAY")
                {

                    string ConnString = ConnectionString;

                    //String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where ItemType = '" + DataAccess.Access.type + "'";
                    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (ItemType = '" + DataAccess.Access.type + "') AND (Consultant= '" + DataAccess.Access.Consultant + "') AND ItemDescription LIKE  '" + add + "%'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    dgvItemList.DataSource = ds1.Tables[0];
                    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    dgvItemList.Columns[0].Width = 200;
                    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    dgvItemList.Columns[1].Width = 200;
                    dgvItemList.Columns[2].HeaderText = "Duration";
                    dgvItemList.Columns[2].Width = 200;

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    }


                }

                //..labtest

                if (DataAccess.Access.type == "LAB")
                {

                    string ConnString = ConnectionString;
                    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where (ItemType = '" + DataAccess.Access.type + "') AND (Consultant= '" + DataAccess.Access.Consultant + "') AND ItemDescription LIKE  '" + add + "%'";

                    // String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where ItemType = '" + DataAccess.Access.type + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    dgvItemList.DataSource = ds1.Tables[0];
                    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    dgvItemList.Columns[0].Width = 200;
                    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    dgvItemList.Columns[1].Width = 200;
                    dgvItemList.Columns[2].HeaderText = "Duration";
                    dgvItemList.Columns[2].Width = 200;

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    }


                }
                //.............

                if (DataAccess.Access.type == "WARD"|| DataAccess.Access.type == "Pharmacy"|| DataAccess.Access.type == "OPD" || DataAccess.Access.type == "ETU")
                {

                    string ConnString = ConnectionString;
                    String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where  (Consultant= '" + DataAccess.Access.Consultant + "') AND ItemID LIKE  '" + add + "%'";

                    // String S1 = "Select ItemID, ItemDescription,Duration from tblConsultantItemFee where ItemType = '" + DataAccess.Access.type + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    dgvItemList.DataSource = ds1.Tables[0];
                    dgvItemList.Columns[0].HeaderText = DataAccess.Access.type + " No";
                    dgvItemList.Columns[0].Width = 200;
                    dgvItemList.Columns[1].HeaderText = DataAccess.Access.type + " Name";
                    dgvItemList.Columns[1].Width = 200;
                    dgvItemList.Columns[2].HeaderText = "Duration";
                    dgvItemList.Columns[2].Width = 200;

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                        dgvItemList.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                    }


                }
            }
            catch { }
        }

        private void dgvItemList_Enter(object sender, EventArgs e)
        {
            //if (ac != 1)
            //{
            //    string val = dgvItemList[0, dgvItemList.CurrentRow.Index].Value.ToString().Trim();
            //    Class1.myvalue = a.GetNext2(val);
            //    this.Close();
            //}
        }
        public int ac = 1;
        private void txtSearch_TabIndexChanged(object sender, EventArgs e)
        {
            ac = 0;
        }
    }
}