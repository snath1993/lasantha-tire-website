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


namespace UserAutherization
{
    public partial class frmadustmentsListPrint : Form
    {
        public frmadustmentsListPrint()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;
        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        public dsAPCommon dsObjAdjustments = new dsAPCommon();

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DateTime DTP = Convert.ToDateTime(dateTimePicker1.Text);
            string Dformat = "MM/dd/yyyy";
            string WHTDate = DTP.ToString(Dformat);
            DateTime DTP1 = Convert.ToDateTime(dateTimePicker2.Text);
            string Dformat1 = "MM/dd/yyyy";
            string WHTDate1 = DTP1.ToString(Dformat1);

            dsObjAdjustments.Clear();

            String StrSql = "SELECT * FROM tblCompanyInformation";
            SqlDataAdapter da251 = new SqlDataAdapter(StrSql, ConnectionString);
            da251.Fill(dsObjAdjustments, "DtCompaniInfo");




            String S25;
            if (cmbBrand.Text.ToString()!="")
            {
                 S25 = "Select * from tblInventoryAdjustment where Date between '" + WHTDate + "' and '" + WHTDate1 + "' and WarehouseId like'%" + cmbWH.Text.ToString().Trim() + "' and WarehouseId like'" + cmbWH.Text.ToString().Trim() + "%' and ItemID like'" + cmbItem.Text.ToString().Trim() + "%' and ItemID  in(select ItemID from tblItemMaster where Custom3 = '" + cmbBrand.Text.ToString() + "') ";//

            }
            else
            {
                 S25 = "Select * from tblInventoryAdjustment where Date between '" + WHTDate + "' and '" + WHTDate1 + "' and WarehouseId like'%" + cmbWH.Text.ToString().Trim() + "' and WarehouseId like'" + cmbWH.Text.ToString().Trim() + "%' and ItemID like'" + cmbItem.Text.ToString().Trim() + "%'";//

            }

            SqlDataAdapter da25 = new SqlDataAdapter(S25, ConnectionString);
            da25.Fill(dsObjAdjustments, "dt_Adjustments");

          if(dsObjAdjustments.dt_Adjustments.Rows.Count>0)
            {
                frmViwerAdjustmetnsListPrint objAdjustmentList = new frmViwerAdjustmetnsListPrint(this);
                objAdjustmentList.Show();
            }
          else
            {
                MessageBox.Show("No Data Found");
              
            }

       
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkWHAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWHAll.Checked)
            {
                cmbWH.Text = "";
                cmbWH.Enabled = false;
            }
            else
            {
                cmbWH.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                cmbBrand.Text = "";
                cmbBrand.Enabled = false;
            }
            else
            {
                cmbBrand.Enabled = true;
            }
        }

        private void frmadustmentsListPrint_Load(object sender, EventArgs e)
        {
            try
            {
                SetItemBrand();
                GetWareHouseDataSet();
                GetItemDataSet();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetItemDataSet()
        {          
            try
            {
               
              string   StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster order by ItemDescription";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                cmbItem.DataSource = dt;
                cmbItem.DisplayMember = "ItemID";
                cmbItem.ValueMember = "ItemID";
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetItemBrand()
        {
            string StrSql = "select  Description from tbl_ItemCustom3";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbBrand.DataSource = dt;
                cmbBrand.ValueMember = "Description";
                cmbBrand.DisplayMember = "Description";

                cmbBrand.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }

        }

        public void GetWareHouseDataSet()
        {
         
               string StrSql = " SELECT WhseId, WhseName FROM tblWhseMaster";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                cmbWH.DataSource = dt;
                cmbWH.DisplayMember = "WhseId";
                cmbWH.ValueMember = "WhseId";
             
            }
         
             
         
        }

        private void chkItemAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkItemAll.Checked)
            {
                cmbItem.Text = "";
                cmbItem.Enabled = false;
            }
            else
            {
                cmbItem.Enabled = true;
            }
        }
    }
}