using Infragistics.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmItemMasterList : Form
    {
        public static string ConnectionString;
        public string CheckBox;
        int IsFind = 0;
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




        private string were;

        public frmItemMasterList()
        {
            InitializeComponent();
            setConnectionString();
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }

        public frmItemMasterList(int _IsFind)
        {
            InitializeComponent();
            setConnectionString();

            IsFind = _IsFind;
        }
        private const int cGrip = 16;
        private const int cCaption = 32;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Class1.ItemSearch = "";
            Class1.ItemSearchDesc = "";
            this.Close();
        }

        private void frmCustomerList_Load(object sender, EventArgs e)
        {
            //Search();

            // CheckBox = "";
            ItemMasterSearch();
            SETSerchCombo();
            SetCustomFields();

            // SetFrontSize();

            this.ActiveControl = txtDescriptionSearch;
        }

        //private void SetFrontSize()
        //{
        //    String S1 = "select * from tblFrontEdit where FromName='"+this.Name+"'";
        //    SqlCommand cmd1 = new SqlCommand(S1);
        //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
        //    DataTable dt1 = new DataTable();
        //    da1.Fill(dt1);

        //    if (dt1.Rows.Count > 0)
        //    {
        //        List<Control> allControls = GetAllControls(this);
        //        string frontstyle = dt1.Rows[0].ItemArray[1].ToString();
        //        int frontsize = Convert.ToInt32(dt1.Rows[0].ItemArray[2]);
        //        int frontsize2 = Convert.ToInt32(dt1.Rows[0].ItemArray[3]);
        //        allControls.ForEach(k => k.Font = new System.Drawing.Font(frontstyle,frontsize));

        //        dgvSearchCustomer.ColumnHeadersDefaultCellStyle.Font = new Font(frontstyle, frontsize2);
        //        this.dgvSearchCustomer.DefaultCellStyle.Font = new Font(frontstyle, frontsize2);
        //    }
        //}
        private List<Control> GetAllControls(Control container, List<Control> list)
        {
            foreach (Control c in container.Controls)
            {

                if (c.Controls.Count > 0)
                    list = GetAllControls(c, list);
                else
                    list.Add(c);
            }

            return list;
        }
        private List<Control> GetAllControls(Control container)
        {
            return GetAllControls(container, new List<Control>());
        }
        private void SetCustomFields()
        {
            try
            {
                String StrSql = "SELECT * FROM [tbl_ItemMasterCostomizeFields]";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        lblCustom1.Text = dt.Rows[i].ItemArray[0].ToString().Trim() + " :";
                        lblCustom2.Text = dt.Rows[i].ItemArray[1].ToString().Trim() + " :";
                        lblCustom3.Text = dt.Rows[i].ItemArray[2].ToString().Trim() + " :";
                        lblCustom4.Text = dt.Rows[i].ItemArray[3].ToString().Trim() + " :";
                        lblCustom5.Text = dt.Rows[i].ItemArray[4].ToString().Trim() + " :";


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SETSerchCombo()
        {
            try
            {
                SetItemBrand();
                SetItemCategory();
                SetItemCountry();
                SetItemType();
                SetItemWhiteWall();

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void SetItemWhiteWall()
        {
            string StrSql = "select  Description from tbl_ItemCustom7";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbWhiteWall.DataSource = dt;
                cmbWhiteWall.ValueMember = "Description";
                cmbWhiteWall.DisplayMember = "Description";
                cmbWhiteWall.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }
        }

        private void SetItemType()
        {
            string StrSql = "select  Description from tbl_ItemCustom4";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbItemType.DataSource = dt;
                cmbItemType.ValueMember = "Description";
                cmbItemType.DisplayMember = "Description";
                cmbItemType.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }
        }

        private void SetItemCountry()
        {
            string StrSql = "select  Description from tbl_ItemCustom2";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbCountry.DataSource = dt;
                cmbCountry.ValueMember = "Description";
                cmbCountry.DisplayMember = "Description";

                cmbCountry.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }
        }

        private void SetItemCategory()
        {
            string StrSql = "select  Description from tbl_ItemCustom5";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbCategory.DataSource = dt;
                cmbCategory.ValueMember = "Description";
                cmbBrand.DisplayMember = "Description";

                cmbCategory.DisplayLayout.Bands[0].Columns[0].Width = 120;
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

        private void Search()
        {
            String StrSql = "SELECT [ItemID],[ItemDescription],[Categoty],Custom3,[PriceLevel1],[Discount],OHQ FROM [view_ItemMaster]";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvSearchCustomer.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dgvSearchCustomer.Rows.Add();
                    dgvSearchCustomer.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                    dgvSearchCustomer.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                    dgvSearchCustomer.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                    dgvSearchCustomer.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                    dgvSearchCustomer.Rows[i].Cells[4].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[6].ToString())).ToString("N2");
                    dgvSearchCustomer.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString())).ToString("N2");
                    dgvSearchCustomer.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                    dgvSearchCustomer.Rows[i].Cells[7].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * (100 - Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()))).ToString("N2");
                    if (dt.Rows[i].ItemArray[3].ToString().Trim() == "MAXXIES")
                    {
                        dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N2");
                        dgvSearchCustomer.Rows[i].Cells[9].Value = (((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60) / 100 * 96).ToString("N2");
                    }
                    else
                    {
                        dgvSearchCustomer.Rows[i].Cells[8].Value = "0.00";
                        dgvSearchCustomer.Rows[i].Cells[9].Value = "0.00";
                    }
                    //dgvSearchCustomer.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                    //dgvSearchCustomer.Rows[i].Cells[8].Value = dt.Rows[i].ItemArray[8].ToString().Trim();

                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            ItemMasterSearch();

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //Search();
            txtItemIDSearch.Text = "";
            txtDescriptionSearch.Text = "";
            cmbCountry.Text = "";
            cmbBrand.Text = "";
            cmbCategory.Text = "";
            txtTyerSizeSearch.Text = "";
            txtUnitCost.Text = "";
            cmbItemType.Text = "";
            cmbWhiteWall.Text = "";
            txtUnitPrice.Text = "";
            checkBox1.Checked = false;
            ItemMasterSearch();
        }

        private void dgvSearchCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                string val = dgvSearchCustomer[0, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
                Class1.ItemSearch = val;
                Class1.ItemSearchDesc = dgvSearchCustomer[1, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
                this.Close();
            }
            catch { }


            //frmMain.ObjGRN.frmGRN_Activated



        }

        private void txtCategorySearch_TextChanged(object sender, EventArgs e)
        {

            //  ItemMasterSearch();
        }

        private void txtCountrySearch_TextChanged(object sender, EventArgs e)
        {

            //  ItemMasterSearch();
        }

        private void txtCompanySearch_TextChanged(object sender, EventArgs e)
        {

            //  ItemMasterSearch();
        }

        private void txtDescriptionSearch_TextChanged(object sender, EventArgs e)
        {

            ItemMasterSearch();
        }

        private void txtTyerSize_TextChanged(object sender, EventArgs e)
        {

            ItemMasterSearch();
        }
        String StrSql;
        private void ItemMasterSearch()
        {
            try
            {

                if (CheckBox == "" || CheckBox == null)
                {
                    StrSql = "SELECT [ItemID],[ItemDescription],[Categoty],Custom3,[PriceLevel1],convert(double precision,[Discount]),ISNULL(OHQ,0),convert(double precision,[Discount2]),convert(double precision,[Discount3]) FROM [view_ItemMaster] WHERE ItemID like'" + txtItemIDSearch.Text.ToString().Trim() + "%' and ItemDescription like'" + txtDescriptionSearch.Text.ToString().Trim() + "%' and Custom1 like'" + txtTyerSizeSearch.Text.ToString().Trim() + "%' and Custom2 like'" + cmbCountry.Text.ToString().Trim() + "%' and Categoty like'" + cmbCategory.Text.ToString().Trim() + "%' and Custom3 like'" + cmbBrand.Text.ToString().Trim() + "%' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "' and PriceLevel1  like'" + txtUnitPrice.Text.ToString().Trim() + "%' and UnitCost like'" + txtUnitCost.Text.ToString().Trim() + "%' and OHQ !=0 and Custom4 like'" + cmbItemType.Text.ToString().Trim() + "%' and Custom5 like'" + cmbWhiteWall.Text.ToString().Trim() + "%'";
                    //  StrSql = "SELECT [ItemID],[ItemDescription],[Categoty],Custom3,[PriceLevel1],convert(double precision,[Discount]),ISNULL(OHQ,0),convert(double precision,[Discount2]),convert(double precision,[Discount3]) FROM [view_ItemMaster] WHERE ItemID like'" + txtItemIDSearch.Text.ToString().Trim() + "%' and ItemDescription like'" + txtDescriptionSearch.Text.ToString().Trim() + "%' and Custom1 like'" + txtTyerSizeSearch.Text.ToString().Trim() + "%' and Custom2 like'" + cmbCountry.Text.ToString().Trim() + "%' and Categoty like'" + cmbCategory.Text.ToString().Trim() + "%' and Custom3 like'" + cmbBrand.Text.ToString().Trim() + "%' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "'  and PriceLevel1  like'" + txtUnitPrice.Text.ToString().Trim() + "%' and UnitCost like'" + txtUnitCost.Text.ToString().Trim() + "%' and Custom4 like'" + cmbItemType.Text.ToString().Trim() + "%' and Custom5 like'" + cmbWhiteWall.Text.ToString().Trim() + "%'";

                }
                else
                {
                    //  StrSql = "SELECT [ItemID],[ItemDescription],[Categoty],Custom3,[PriceLevel1],convert(double precision,[Discount]),ISNULL(OHQ,0),convert(double precision,[Discount2]),convert(double precision,[Discount3]) FROM [view_ItemMaster] WHERE ItemID like'" + txtItemIDSearch.Text.ToString().Trim() + "%' and ItemDescription like'" + txtDescriptionSearch.Text.ToString().Trim() + "%' and Custom1 like'" + txtTyerSizeSearch.Text.ToString().Trim() + "%' and Custom2 like'" + cmbCountry.Text.ToString().Trim() + "%' and Categoty like'" + cmbCategory.Text.ToString().Trim() + "%' and Custom3 like'" + cmbBrand.Text.ToString().Trim() + "%' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "' and PriceLevel1  like'" + txtUnitPrice.Text.ToString().Trim() + "%' and UnitCost like'" + txtUnitCost.Text.ToString().Trim() + "%' and OHQ !='" + CheckBox + "' and Custom4 like'" + cmbItemType.Text.ToString().Trim() + "%' and Custom5 like'" + cmbWhiteWall.Text.ToString().Trim() + "%'";
                    StrSql = "SELECT [ItemID],[ItemDescription],[Categoty],Custom3,[PriceLevel1],convert(double precision,[Discount]),ISNULL(OHQ,0),convert(double precision,[Discount2]),convert(double precision,[Discount3]) FROM [view_ItemMaster] WHERE ItemID like'" + txtItemIDSearch.Text.ToString().Trim() + "%' and ItemDescription like'" + txtDescriptionSearch.Text.ToString().Trim() + "%' and Custom1 like'" + txtTyerSizeSearch.Text.ToString().Trim() + "%' and Custom2 like'" + cmbCountry.Text.ToString().Trim() + "%' and Categoty like'" + cmbCategory.Text.ToString().Trim() + "%' and Custom3 like'" + cmbBrand.Text.ToString().Trim() + "%' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "'  and PriceLevel1  like'" + txtUnitPrice.Text.ToString().Trim() + "%' and UnitCost like'" + txtUnitCost.Text.ToString().Trim() + "%' and Custom4 like'" + cmbItemType.Text.ToString().Trim() + "%' and Custom5 like'" + cmbWhiteWall.Text.ToString().Trim() + "%'";

                }
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSearchCustomer.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchCustomer.Rows.Add();
                        dgvSearchCustomer.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchCustomer.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvSearchCustomer.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvSearchCustomer.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvSearchCustomer.Rows[i].Cells[4].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[6].ToString())).ToString("N0");
                        dgvSearchCustomer.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString())).ToString("N0");

                        dgvSearchCustomer.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[5].ToString().Trim() + "%";
                        dgvSearchCustomer.Rows[i].Cells[7].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * (100 - Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()))).ToString("N0");

                        if (double.Parse(dt.Rows[i].ItemArray[5].ToString().Trim()) == 0)
                        {
                            dgvSearchCustomer.Rows[i].Cells[8].Value = "0%";
                            dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                            dgvSearchCustomer.Rows[i].Cells[10].Value = "0%";
                            dgvSearchCustomer.Rows[i].Cells[11].Value = "0";

                        }
                        else
                        {

                            dgvSearchCustomer.Rows[i].Cells[8].Value = dt.Rows[i].ItemArray[7].ToString().Trim() + "%";
                            if (double.Parse(dt.Rows[i].ItemArray[7].ToString().Trim()) != 0)
                            {
                                dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * (100 - Convert.ToDouble(dt.Rows[i].ItemArray[7].ToString().Trim()))).ToString("N0");
                            }

                            else
                            {
                                dgvSearchCustomer.Rows[i].Cells[9].Value = "0";

                            }
                            dgvSearchCustomer.Rows[i].Cells[10].Value = dt.Rows[i].ItemArray[8].ToString().Trim() + "%";
                            if (double.Parse(dt.Rows[i].ItemArray[8].ToString().Trim()) != 0)
                            {
                                dgvSearchCustomer.Rows[i].Cells[11].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * (100 - Convert.ToDouble(dt.Rows[i].ItemArray[8].ToString().Trim()))).ToString("N0");
                            }
                            else
                            {
                                dgvSearchCustomer.Rows[i].Cells[11].Value = "0";

                            }
                        }
                        //  dgvSearchCustomer.Columns[9].HeaderText = "35%";


                        //dgvSearchCustomer.Rows[i].Cells[8].Value = 
                        //dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * (100 - Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()))).ToString("N0");

                        //dgvSearchCustomer.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[5].ToString().Trim() + "%";
                        //dgvSearchCustomer.Rows[i].Cells[7].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * (100 - Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()))).ToString("N0");

                        // LasanthaTire





                        //if ((dt.Rows[i].ItemArray[3].ToString().Trim() == "MAXXIES") && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim())>0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //}
                        //else if(dt.Rows[i].ItemArray[3].ToString().Trim() == "HIFLY" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}

                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "ROADSTON" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 62.5).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "KUMHO" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "CONTINENTAL" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 80).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "CEAT" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 72.5).ToString("N0"); 
                        //}

                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "GT" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //   // dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "GITI" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 55).ToString("N0");
                        //    // dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "LEAO" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //    // dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}

                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "TRATMAX" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //    // dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}
                        ////TRATMAX

                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "GT CHINA" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //  //  dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "MATRAX" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 80).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //}

                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "BRIDGESTONE" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "FERENTINO" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = "0";
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "DURATURN" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "NEXEN" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "TOYO" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 80).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "YOKOHAMA" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 80).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "PIRELLI" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}

                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "LANVIGATOR" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "POWERTRAC" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "LING LONG" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "ARPICO" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 80).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 85).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "MICHELIN" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "FEDERAL" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "GOODYEAR" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 80).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "PRESA" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "CHENSHIN" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "HANKOOK" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}
                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "OVATION" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 75).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 70).ToString("N0");
                        //}

                        //else if (dt.Rows[i].ItemArray[3].ToString().Trim() == "TRACMAX" && (Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Trim()) > 0))
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 65).ToString("N0");
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = ((Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Trim()) / 100) * 60).ToString("N0");
                        //}
                        //else
                        //{
                        //    dgvSearchCustomer.Rows[i].Cells[8].Value = "0";
                        //    dgvSearchCustomer.Rows[i].Cells[9].Value = "0";
                        //}



                    }

                    //  LoadDiscount(dt.Rows[0].ItemArray[3].ToString().Trim(), Convert.ToDouble(dt.Rows[0].ItemArray[5].ToString().Trim()));

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private object GetOHQTY(string v)
        {
            String StrSql = "SELECT sum(QTY) FROM [tblItemWhse] WHERE ItemId ='" + txtItemIDSearch.Text.ToString().Trim() + "' group by ItemId";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray[0].ToString().Trim();
            }
            else
            {
                return "0.00";
            }

        }

        private void txtBrand_TextChanged(object sender, EventArgs e)
        {
            // ItemMasterSearch();
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {
            ItemMasterSearch();
        }

        private void txtUnitCost_TextChanged(object sender, EventArgs e)
        {
            ItemMasterSearch();
        }

        private void cmbCategory_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            ItemMasterSearch();
        }

        private void cmbCountry_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            ItemMasterSearch();
        }

        private void cmbBrand_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            ItemMasterSearch();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbItemType_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            ItemMasterSearch();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                CheckBox = "0.00";
            }
            else
            {
                CheckBox = "";
            }
            ItemMasterSearch();
        }

        private void cmbItemType_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void cmbWhiteWall_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            ItemMasterSearch();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ItemMasterSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvSearchCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    string val = dgvSearchCustomer[0, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
            //    Class1.ItemSearch = val;
            //    Class1.ItemSearchDesc= dgvSearchCustomer[1, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
            //    this.Close();
            //}
            //catch { }
        }

        private void dgvSearchCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    string val = dgvSearchCustomer[0, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
                    Class1.ItemSearch = val;
                    Class1.ItemSearchDesc = dgvSearchCustomer[1, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
                    this.Close();


                }

            }
            catch { }

        }

        private void cmbBrand_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void frmItemMasterList_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtTyerSizeSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
        }

        private void txtDescriptionSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
        }

        private void txtItemIDSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
        }

        private void txtUnitCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
        }

        private void txtUnitPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
        }

        private void cmbBrand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }

        }

        private void cmbCategory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }

        }

        private void cmbCountry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }

        }

        private void cmbItemType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }

        }

        private void checkBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }

            else if (e.KeyCode == Keys.Down)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
        }

        private void cmbWhiteWall_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }

        }

        private void btnClear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (dgvSearchCustomer.Focus() == false)
                {
                    dgvSearchCustomer.Focus();
                }
            }
        }

        private void dgvSearchCustomer_RowEnter(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void dgvSearchCustomer_TabIndexChanged(object sender, EventArgs e)
        {

        }

        private void dgvSearchCustomer_SelectionChanged(object sender, EventArgs e)
        {


            if (dgvSearchCustomer[7, dgvSearchCustomer.CurrentRow.Index].Value == null)
            {
                return;
            }

            for (int x = 0; x < dgvSearchCustomer.Rows.Count; x++)
            {
                dgvSearchCustomer.Rows[x].DefaultCellStyle.BackColor = Color.White;

            }
            dgvSearchCustomer.CurrentRow.DefaultCellStyle.BackColor = Color.CornflowerBlue;
            //  LoadDiscount(dgvSearchCustomer[3, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim(), Convert.ToDouble(dgvSearchCustomer[7, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim()));


        }

        //private void LoadDiscount(string Brand,double MasterDiscount)
        //{
        //    //OVATION
        //    if ((Brand == "MAXXIES") && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "35%";
        //    }
        //    else if ((Brand == "FEDERAL") && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "35%";
        //    }
        //    else if ((Brand == "ARPICO") && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "20%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "25%";
        //    }
        //    else if ((Brand == "OVATION") && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "25%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if ((Brand == "TRACMAX") && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "35%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    }
        //    else if (Brand == "HIFLY" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "25%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "DURATURN" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "35%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    }

        //    else if (Brand == "ROADSTON" && (MasterDiscount > 0))
        //    {

        //        dgvSearchCustomer.Columns[8].HeaderText = "37.5%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    }
        //    else if (Brand == "GITI" && (MasterDiscount > 0))
        //    {

        //        dgvSearchCustomer.Columns[8].HeaderText = "40%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "45%";
        //    }
        //    //else if (Brand == "KUMHO" && (MasterDiscount > 0))
        //    //{
        //    //    dgvSearchCustomer.Columns[8].HeaderText = "37.5%";
        //    //    dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    //}

        //    else if (Brand == "CONTINENTAL" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "20%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "25%";
        //    }

        //    else if (Brand == "CEAT" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "25%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "27.5%";
        //    }
        //    else if (Brand == "GT" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "GT CHINA" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "BRIDGESTONE" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "35%";
        //    }
        //    else if (Brand == "PIRELLI" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "FERENTINO" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "0%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "NEXEN" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "35%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    }
        //    else if (Brand == "TOYO" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "20%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "25%";
        //    }
        //    else if (Brand == "YOKOHAMA" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "20%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "25%";
        //    }
        //    else if (Brand == "KUMHO" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "35%";
        //    }
        //    else if (Brand == "LANVIGATOR" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "25%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "POWERTRAC" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "25%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "LING LONG" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "25%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "MICHELIN" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "25%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "30%";
        //    }
        //    else if (Brand == "CHENSHIN" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "35%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    }
        //    else if (Brand == "GOODYEAR" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "20%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "25%";
        //    }
        //    else if (Brand == "GOOD YEAR" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "20%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "25%";
        //    }
        //    else if (Brand == "HANKOOK" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }
        //    else if (Brand == "PRESA" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "30%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "35%";
        //    }
        //    else if (Brand == "YOKOHAMA" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "20%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "35%";
        //    }
        //    else if (Brand == "LIYO" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "35%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    }
        //    else if (Brand == "LEAO" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "35%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    }
        //    else if (Brand == "MATRAX" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "20%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "25%";
        //    }
        //    else if (Brand == "TRATMAX" && (MasterDiscount > 0))
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "35%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "40%";
        //    }

        //    else
        //    {
        //        dgvSearchCustomer.Columns[8].HeaderText = "0%";
        //        dgvSearchCustomer.Columns[9].HeaderText = "0%";
        //    }

        //}

        private void dgvSearchCustomer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void dgvSearchCustomer_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string val = dgvSearchCustomer[0, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
                Class1.ItemSearch = val;
                Class1.ItemSearchDesc = dgvSearchCustomer[1, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
                this.Close();
            }
            catch { }
        }

        private void btnViewUnitCost_Click(object sender, EventArgs e)
        {

            try
            {
                frmItemUnitCost ObjItemUnitCost = new frmItemUnitCost(txtDescriptionSearch.Text.ToString().Trim());
                ObjItemUnitCost.Show();
            }

            catch (Exception ex)

            { MessageBox.Show(ex.Message); }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                frmMobileApp ObjItemUnitCost = new frmMobileApp();
                ObjItemUnitCost.Show();
            }

            catch (Exception ex)

            { MessageBox.Show(ex.Message); }
        }
    }
}
