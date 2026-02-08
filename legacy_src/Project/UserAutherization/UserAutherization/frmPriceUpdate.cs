using Infragistics.Win.UltraWinGrid;
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
    public partial class frmPriceUpdate : Form
    {
        internal int flgTech = 10;
        private string ConnectionString;
        private int intcon = 0;
        Class1 a = new Class1();
        public string CheckBox;

        public frmPriceUpdate()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void btnConsultant_Click(object sender, EventArgs e)
        {
            try
            {
                flgTech = 10;
                //StrFormType = cmbTestType.Text.ToString();
                //DataAccess.Access.type = cmbTestType.Text.ToString();
                frmSelectConsult frm = new frmSelectConsult(this);
                frm.ShowDialog();
                intcon = 1;
            }
            catch { }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
        private void btnNewn_Click(object sender, EventArgs e)
        {
            edit = false;
            txtConFee.Enabled = true;
            txtConFee.Text = "0.00";
            // txtConsultant.Enabled = true;
            txtConsultant.Text = "";
            txtDuration.Enabled = true;
            txtDuration.Text = "";
            txtGL.Enabled = true;
            txtGL.Text = "";
            txtHosFee.Enabled = true;
            txtHosFee.Text = "0.00";
            txtItemDescription.Enabled = true;
            txtItemDescription.Text = "";
            txtItemId.Enabled = true;
            txtItemId.Text = "";
            cmbTestType.Enabled = true;
            cmbTestType.Text = "";
            btnConsultant.Enabled = true;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            dgvPriceUpdate.Enabled = false;
          
            btnClear.Enabled = false;

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txtConFee.Enabled = false;
            txtConFee.Text = "0.00";
            //  txtConsultant.Enabled = false;
            txtConsultant.Text = "";
            txtDuration.Enabled = false;
            txtDuration.Text = "";
            txtGL.Enabled = false;
            txtGL.Text = "";
            txtHosFee.Enabled = false;
            txtHosFee.Text = "0.00";
            txtItemDescription.Enabled = false;
            txtItemDescription.Text = "";
            txtItemId.Enabled = false;
            txtItemId.Text = "";
            cmbTestType.Enabled = false;
            cmbTestType.Text = "";
            btnConsultant.Enabled = false;
            btnSave.Enabled = false;
            btnUpdate.Enabled = true;
            dgvPriceUpdate.Enabled = true;

            // dgvPriceUpdate.BeginEdit(true);
         
            btnClear.Enabled = true;
            edit = true;
        }
        Boolean edit = false;
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation())
                {
                    return;
                }

                SaveEvent();
                btnNewn_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveEvent()
        {

            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            try
            {
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                string StrSql = "INSERT INTO [tblConsultantItemFee]([Consultant],[ItemID],[ItemDescription],[ConsultFee],[HospitalFee],[Duration],[GLAccount],[ItemType])" +
                                "VALUES('" + txtConsultant.Text.ToString() + "','" + txtItemId.Text.ToString() + "','" + txtItemDescription.Text.ToString() + "','" + txtConFee.Text.ToString() + "','" + txtHosFee.Text.ToString() + "','" + txtDuration.Text.ToString() + "','" + txtGL.Text.ToString() + "','" + cmbTestType.Text.ToString() + "')";
                SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                myTrans.Commit();
                MessageBox.Show("Scan\\Lab Test\\ Channelling Added SuccessFully");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool validation()
        {
            if (txtConsultant.Text == "")
            {
                MessageBox.Show("Please Select Consaltent");
                return true;
            }
            if (txtItemId.Text == "")
            {
                MessageBox.Show("Please Insert Item ID");
                return true;
            }
            if (txtItemDescription.Text == "")
            {
                MessageBox.Show("Please Insert Item Description");
                return true;
            }
            if (cmbTestType.Text == "")
            {
                MessageBox.Show("Please Select Item Type");
                return true;
            }
            if (txtItemId.Text == "")
            {
                MessageBox.Show("Please Insert Item Type");
                return true;
            }
            if (txtGL.Text == "")
            {
                MessageBox.Show("Please Insert GL Account ");
                return true;
            }
            if (txtHosFee.Text.Trim() == "")
            {
                MessageBox.Show("Please Insert Hospital Fee");
                return true;
            }
            if (txtConFee.Text.Trim() == "")
            {
                MessageBox.Show("Please Insert Consaltent Fee");
                return true;
            }

            String S2 = "SELECT [ItemID] FROM [tblConsultantItemFee] WHERE ItemID='" + txtItemId.Text.ToString().Trim() + "' ";
            SqlCommand cmd2 = new SqlCommand(S2);
            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            if (dt2.Rows.Count > 0)
            {

                MessageBox.Show("This Item Already Exists");
                return true;
            }
            return false;
        }
 



        private void frmPriceUpdate_Load(object sender, EventArgs e)
        {


            //  LoadTestType();
            // LoadTestList();
            btnEdit_Click(null, null);
            ItemMasterSearch();
            SETSerchCombo();
            SetCustomFields();
            SetFrontSize();
        }
        private void SetFrontSize()
        {
            String S1 = "select * from tblFrontEdit where FromName='" + this.Name + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            if (dt1.Rows.Count > 0)
            {
                List<Control> allControls = GetAllControls(this);
                string frontstyle = dt1.Rows[0].ItemArray[1].ToString();
                int frontsize = Convert.ToInt32(dt1.Rows[0].ItemArray[2]);
                int frontsize2 = Convert.ToInt32(dt1.Rows[0].ItemArray[3]);
                allControls.ForEach(k => k.Font = new System.Drawing.Font(frontstyle, frontsize));

                dgvPriceUpdate.ColumnHeadersDefaultCellStyle.Font = new Font(frontstyle, frontsize2);
                this.dgvPriceUpdate.DefaultCellStyle.Font = new Font(frontstyle, frontsize2);
            }
        }

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

        private void LoadTestSearch()
        {
           
        }
        private void LoadTestList()
        {
            try
            {
                String S2 = "SELECT [ItemID],[ItemDescription],[Categoty],[UnitCost],[PriceLevel1],[Discount]FROM[tblItemMaster]";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                dgvPriceUpdate.Rows.Clear();
                if (dt2.Rows.Count > 0)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        dgvPriceUpdate.Rows.Add();
                        dgvPriceUpdate.Rows[i].Cells[0].Value = dt2.Rows[i].ItemArray[0].ToString().Trim();
                        dgvPriceUpdate.Rows[i].Cells[1].Value = dt2.Rows[i].ItemArray[1].ToString().Trim();
                        dgvPriceUpdate.Rows[i].Cells[2].Value = dt2.Rows[i].ItemArray[2].ToString().Trim();
                        dgvPriceUpdate.Rows[i].Cells[3].Value = dt2.Rows[i].ItemArray[3].ToString().Trim();
                        dgvPriceUpdate.Rows[i].Cells[4].Value = dt2.Rows[i].ItemArray[5].ToString().Trim();
                        dgvPriceUpdate.Rows[i].Cells[5].Value = dt2.Rows[i].ItemArray[4].ToString().Trim();
                        dgvPriceUpdate.Rows[i].Cells[6].Value = "0.00";
                        dgvPriceUpdate.Rows[i].Cells[7].Value = "0.00";

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadTestType()
        {
            DataSet dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                //StrSql = "SELECT distinct([ItemType]) as[ItemType] FROM[HospitalSystem].[dbo].[tblConsultantItemFee]";
                string StrSql = "SELECT [TypeName]FROM [tblPatientMasterType]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");

                cmbTestType.DataSource = dsCustomer.Tables["DtClient"];
                cmbTestType.DisplayMember = "TypeName";
                cmbTestType.ValueMember = "TypeName";

                cmbTestType.DisplayLayout.Bands["DtClient"].Columns["TypeName"].Width = 150;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmPriceUpdate_Activated(object sender, EventArgs e)
        {
            //if (intcon == 1)
            //{
            //    txtConsultant.Text = Search.PriceUpdate;
            //    String S1 = "Select * from tblConsultantMaster where ConsultantCode = '" + a.GetText11().ToString() + "' ";
            //    SqlCommand cmd1 = new SqlCommand(S1);
            //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //    DataSet ds1 = new DataSet();
            //    da1.Fill(ds1);
            //    txtItemId.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString();
            //    cmbTestType.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();
            //    txtItemDescription.Text = ds1.Tables[0].Rows[0].ItemArray[14].ToString();
            //    if (cmbTestType.Text.ToString() == "CHANNELLING")
            //    {
            //        txtItemId.Enabled = false;
            //        cmbTestType.Enabled = false;
            //        txtItemDescription.Enabled = false;
            //    }

            //    intcon = 0;
            //}
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadTestSearch();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //LoadTestList();
            ClearEvent();
           
        }

        private void ClearEvent()
        {
            txtItemIDSearch.Text = "";
            txtDescriptionSearch.Text = "";
            cmbCountry.Text = "";
            cmbBrand.Text = "";
            cmbCategory.Text = "";
            txtTyerSizeSearch.Text = "";
            txtUnitCost.Text = "";
            cmbItemType.Text = "";
            cmbWhiteWall.Text = "";
            txtListPrice.Text = "";
            checkBox1.Checked = false;
            ItemMasterSearch();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //ConnectorNew cn = new ConnectorNew();
            //if (cn.IsOpenPeachtree() == false)
            //{
            //    return;
            //}
            UpdateEvent();
            btnEdit_Click(sender, e);
           // ClearEvent();
        }
       
        private void UpdateEvent()
        {
       
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            try
            {
                dgvPriceUpdate.EndEdit();
                dgvPriceUpdate.CommitEdit(DataGridViewDataErrorContexts.Commit);
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                UpadteData(myConnection, myTrans);
                myTrans.Commit();
                MessageBox.Show("Price Updated SuccessFully");
                LoadTestList();
                ClearEvent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();

            }

        }

        private void UpadteData(SqlConnection myConnection, SqlTransaction myTrans)
        {
          
            string StrSql = null;
            for (int i = 0; i < dgvPriceUpdate.RowCount - 1; i++)
            {

                if (dgvPriceUpdate.Rows[i].Cells[7].Value == null)
                {
                    return;
                }

                if (dgvPriceUpdate.Rows[i].Cells[7].Value.ToString().Trim() != "0.00" || dgvPriceUpdate.Rows[i].Cells[6].Value.ToString().Trim() != "0.00")
                {
                    if (dgvPriceUpdate.Rows[i].Cells[7].Value.ToString().Trim() != "0.00" && dgvPriceUpdate.Rows[i].Cells[6].Value.ToString().Trim() != "0.00")
                    {
                         StrSql = "UPDATE [dbo].[tblItemMaster] SET [PriceLevel1] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[7].Value) + "' ,[Discount] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[6].Value) + "' WHERE ([ItemID] = '" + dgvPriceUpdate.Rows[i].Cells[0].Value + "')AND([ItemDescription] = '" + dgvPriceUpdate.Rows[i].Cells[1].Value + "')AND([Categoty]='" + dgvPriceUpdate.Rows[i].Cells[2].Value + "')";

                    }
                    else if (dgvPriceUpdate.Rows[i].Cells[7].Value.ToString().Trim() != "0.00" && dgvPriceUpdate.Rows[i].Cells[6].Value.ToString().Trim() == "0.00")
                    {
                         StrSql = "UPDATE [dbo].[tblItemMaster] SET [PriceLevel1] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[7].Value) + "' WHERE ([ItemID] = '" + dgvPriceUpdate.Rows[i].Cells[0].Value + "')AND([ItemDescription] = '" + dgvPriceUpdate.Rows[i].Cells[1].Value + "')AND([Categoty]='" + dgvPriceUpdate.Rows[i].Cells[2].Value + "')";

                    }
                    else if (dgvPriceUpdate.Rows[i].Cells[7].Value.ToString().Trim() == "0.00" && dgvPriceUpdate.Rows[i].Cells[6].Value.ToString().Trim() != "0.00")
                    {
                         StrSql = "UPDATE [dbo].[tblItemMaster] SET [Discount] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[6].Value) + "' WHERE ([ItemID] = '" + dgvPriceUpdate.Rows[i].Cells[0].Value + "')AND([ItemDescription] = '" + dgvPriceUpdate.Rows[i].Cells[1].Value + "')AND([Categoty]='" + dgvPriceUpdate.Rows[i].Cells[2].Value + "')";

                    }

                    //else if (dgvPriceUpdate.Rows[i].Cells[7].Value.ToString().Trim() == "0.00" && dgvPriceUpdate.Rows[i].Cells[6].Value.ToString().Trim() == "0.00" && dgvPriceUpdate.Rows[i].Cells[9].Value.ToString().Trim() != "0.00")
                    //{
                    //     StrSql = "UPDATE [dbo].[tblItemMaster] SET [Profit] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[9].Value) + "' WHERE ([ItemID] = '" + dgvPriceUpdate.Rows[i].Cells[0].Value + "')AND([ItemDescription] = '" + dgvPriceUpdate.Rows[i].Cells[1].Value + "')AND([Categoty]='" + dgvPriceUpdate.Rows[i].Cells[2].Value + "')";

                    //}

                    else if (dgvPriceUpdate.Rows[i].Cells[7].Value.ToString().Trim() != "0.00" && dgvPriceUpdate.Rows[i].Cells[6].Value.ToString().Trim() != "0.00")
                    {
                         StrSql = "UPDATE [dbo].[tblItemMaster] SET [PriceLevel1] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[7].Value) + "',[Discount] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[6].Value) + "' WHERE ([ItemID] = '" + dgvPriceUpdate.Rows[i].Cells[0].Value + "')AND([ItemDescription] = '" + dgvPriceUpdate.Rows[i].Cells[1].Value + "')AND([Categoty]='" + dgvPriceUpdate.Rows[i].Cells[2].Value + "')";

                    }

                    else if (dgvPriceUpdate.Rows[i].Cells[7].Value.ToString().Trim() == "0.00" && dgvPriceUpdate.Rows[i].Cells[6].Value.ToString().Trim() != "0.00")
                    {
                         StrSql = "UPDATE [dbo].[tblItemMaster] SET [Discount] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[6].Value) + "' WHERE ([ItemID] = '" + dgvPriceUpdate.Rows[i].Cells[0].Value + "')AND([ItemDescription] = '" + dgvPriceUpdate.Rows[i].Cells[1].Value + "')AND([Categoty]='" + dgvPriceUpdate.Rows[i].Cells[2].Value + "')";

                    }

                    else if (dgvPriceUpdate.Rows[i].Cells[7].Value.ToString().Trim() != "0.00" && dgvPriceUpdate.Rows[i].Cells[6].Value.ToString().Trim() == "0.00")
                    {
                         StrSql = "UPDATE [dbo].[tblItemMaster] SET [PriceLevel1] = '" + Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[7].Value) + "' WHERE ([ItemID] = '" + dgvPriceUpdate.Rows[i].Cells[0].Value + "')AND([ItemDescription] = '" + dgvPriceUpdate.Rows[i].Cells[1].Value + "')AND([Categoty]='" + dgvPriceUpdate.Rows[i].Cells[2].Value + "')";

                    }

                    SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                   // CreateXML(i, myConnection,myTrans);

                }

            }
           
        }
        public string GLSalesAcc, GLInventoryAcc, GLCostofSalaesACC;
        private void CreateXML(int i, SqlConnection Myconnection,SqlTransaction trans)
        {
           

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemMaster.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");



            Writer.WriteStartElement("PAW_Item");
            Writer.WriteAttributeString("xsi:type", "paw:item");

            Writer.WriteStartElement("ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(dgvPriceUpdate.Rows[i].Cells[0].Value.ToString().Trim());//Vendor ID should be here = Ptient No
            Writer.WriteEndElement();

           
            Writer.WriteStartElement("Sales_Prices");

            Writer.WriteStartElement("Sales_Price_Info");
            Writer.WriteAttributeString("Key", "1");

            Writer.WriteStartElement("Sales_Price");
            Writer.WriteString(Convert.ToDouble(dgvPriceUpdate.Rows[i].Cells[7].Value).ToString().Trim());
            Writer.WriteEndElement();


            Writer.WriteEndElement();



            GetItemGLACC(dgvPriceUpdate.Rows[i].Cells[0].Value.ToString().Trim(),Myconnection,trans);


            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_Sales_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(GLSalesAcc);
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_Inventory_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(GLInventoryAcc);
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_COGSSalary_Acct");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(GLCostofSalaesACC);
            Writer.WriteEndElement();


            Writer.WriteEndElement();
            //********************
            Writer.WriteEndElement();//last line
            Writer.Close();

            Connector conn = new Connector();
            conn.ImportItemMaster();
        }

        private void GetItemGLACC(string ItemId,SqlConnection con,SqlTransaction Trans)
        {
            try
            {
                String StrSql = "SELECT [SalesGLAccount],[InventoryAcc],[CostOfSalesAcc] FROM [tblItemMaster] where  ItemID ='" + ItemId+"'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                  
                        GLSalesAcc = dt.Rows[0].ItemArray[0].ToString().Trim();
                        GLInventoryAcc = dt.Rows[0].ItemArray[1].ToString().Trim();
                        GLCostofSalaesACC = dt.Rows[0].ItemArray[2].ToString().Trim();
                                       
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void DeletTable(SqlConnection myConnection, SqlTransaction myTrans)
        {
            string StrSql = "DELETE FROM [dbo].[tblConsultantItemFee]";
            SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

        }

        private void dgvPriceUpdate_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 3 || e.ColumnIndex == 4) && edit == true)
            {
                dgvPriceUpdate.CurrentCell.ReadOnly = false;
            }
            else
            {
                dgvPriceUpdate.CurrentCell.ReadOnly = true;
            }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTestList();
        }

        private void dgvPriceUpdate_Click(object sender, EventArgs e)
        {


        }

        private void dgvPriceUpdate_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txtCompanySearch_TextChanged(object sender, EventArgs e)
        {
            //ItemMasterSearch();
        }

        private void txtCountrySearch_TextChanged(object sender, EventArgs e)
        {
            //ItemMasterSearch();
        }

        private void txtCategorySearch_TextChanged(object sender, EventArgs e)
        {
            //ItemMasterSearch();
        }

        private void txtDescriptionSearch_TextChanged(object sender, EventArgs e)
        {
            ItemMasterSearch();
        }

        private void txtTyerSizeSearch_TextChanged(object sender, EventArgs e)
        {
            ItemMasterSearch();
        }

        private void txtItemIDSearch_TextChanged(object sender, EventArgs e)
        {
            ItemMasterSearch();
        }

        private void ItemMasterSearch()
        {
            String S2;
            if (CheckBox == "" || CheckBox == null)
            {
                 S2 = "SELECT [ItemID],[ItemDescription],[Categoty],convert(double precision,[OHQ]),[PriceLevel1],[Discount],isnull(Profit,0) FROM [view_ItemMaster] WHERE ItemID like'" + txtItemIDSearch.Text.ToString().Trim() + "%' and ItemDescription like'" + txtDescriptionSearch.Text.ToString().Trim() + "%' and Custom1 like'" + txtTyerSizeSearch.Text.ToString().Trim() + "%' and Custom2 like'" + cmbCountry.Text.ToString().Trim() + "%' and Categoty like'" + cmbCategory.Text.ToString().Trim() + "%' and Custom3 like'" + cmbBrand.Text.ToString().Trim() + "%' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "' and PriceLevel1  like'" + txtListPrice.Text.ToString().Trim() + "%' and UnitCost like'" + txtUnitCost.Text.ToString().Trim() + "%' and Custom4 like'" + cmbItemType.Text.ToString().Trim() + "%' and Custom5 like'" + cmbWhiteWall.Text.ToString().Trim() + "%'";
            }
            else
            {
                S2 = "SELECT [ItemID],[ItemDescription],[Categoty],convert(double precision,[OHQ]),[PriceLevel1],[Discount],isnull(Profit,0) FROM [view_ItemMaster] WHERE ItemID like'" + txtItemIDSearch.Text.ToString().Trim() + "%' and ItemDescription like'" + txtDescriptionSearch.Text.ToString().Trim() + "%' and Custom1 like'" + txtTyerSizeSearch.Text.ToString().Trim() + "%' and Custom2 like'" + cmbCountry.Text.ToString().Trim() + "%' and Categoty like'" + cmbCategory.Text.ToString().Trim() + "%' and Custom3 like'" + cmbBrand.Text.ToString().Trim() + "%' and Custom3 like'%" + cmbBrand.Text.ToString().Trim() + "' and PriceLevel1  like'" + txtListPrice.Text.ToString().Trim() + "%' and UnitCost like'" + txtUnitCost.Text.ToString().Trim() + "%' and OHQ !='" + CheckBox + "' and Custom4 like'" + cmbItemType.Text.ToString().Trim() + "%' and Custom5 like'" + cmbWhiteWall.Text.ToString().Trim() + "%'";

            }
            SqlCommand cmd2 = new SqlCommand(S2);
            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dgvPriceUpdate.Rows.Clear();
            if (dt2.Rows.Count > 0)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    dgvPriceUpdate.Rows.Add();
                    dgvPriceUpdate.Rows[i].Cells[0].Value = dt2.Rows[i].ItemArray[0].ToString().Trim();
                    dgvPriceUpdate.Rows[i].Cells[1].Value = dt2.Rows[i].ItemArray[1].ToString().Trim();
                    dgvPriceUpdate.Rows[i].Cells[2].Value = dt2.Rows[i].ItemArray[2].ToString().Trim();
                    dgvPriceUpdate.Rows[i].Cells[3].Value = dt2.Rows[i].ItemArray[3].ToString().Trim();
                    dgvPriceUpdate.Rows[i].Cells[4].Value = dt2.Rows[i].ItemArray[5].ToString().Trim();
                    dgvPriceUpdate.Rows[i].Cells[5].Value = Convert.ToDouble(dt2.Rows[i].ItemArray[4]).ToString("N2").Trim();
                    dgvPriceUpdate.Rows[i].Cells[6].Value = "0.00";
                    dgvPriceUpdate.Rows[i].Cells[7].Value = "0.00";
                    dgvPriceUpdate.Rows[i].Cells[8].Value = Convert.ToDouble(dt2.Rows[i].ItemArray[6]).ToString("N2").Trim();
                    dgvPriceUpdate.Rows[i].Cells[9].Value = "0.00";

                }
            }
        }

        private void txtBrand_TextChanged(object sender, EventArgs e)
        {
           // ItemMasterSearch();
        }

        private void txtListPrice_TextChanged(object sender, EventArgs e)
        {
            ItemMasterSearch();
        }

        private void txtUnitCost_TextChanged(object sender, EventArgs e)
        {
            ItemMasterSearch();
        }

        private void cmbBrand_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
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

        private void cmbItemType_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            ItemMasterSearch();
        }

        private void cmbWhiteWall_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
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

        private void dgvPriceUpdate_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPriceUpdate[7, dgvPriceUpdate.CurrentRow.Index].Value == null)
            {
                return;
            }

            for (int x = 0; x < dgvPriceUpdate.Rows.Count; x++)
            {
                dgvPriceUpdate.Rows[x].DefaultCellStyle.BackColor = Color.White;

            }
            dgvPriceUpdate.CurrentRow.DefaultCellStyle.BackColor = Color.CornflowerBlue;
        }

        private void dgvPriceUpdate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int icolumn = dgvPriceUpdate.CurrentCell.ColumnIndex;
                int irow = 0;
                if (dgvPriceUpdate.CurrentCell.RowIndex != 0)
                {
                     irow = dgvPriceUpdate.CurrentCell.RowIndex-1;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    if (icolumn == 7)
                    {
                        // dgvPriceUpdate.Rows.Add();
                        dgvPriceUpdate.CurrentCell = dgvPriceUpdate[1, irow + 1];
                    }
                    else
                    {
                        if (icolumn != 8 && icolumn != 9)
                        {
                            dgvPriceUpdate.CurrentCell = dgvPriceUpdate[icolumn + 1, irow];
                        }
                    }
                }
            }
            catch
            {

            }

        }

        //protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        //{
        //    try
        //    {
        //        int icolumn = dgvPriceUpdate.CurrentCell.ColumnIndex;
        //        int irow = dgvPriceUpdate.CurrentCell.RowIndex;

        //        if (keyData == Keys.Enter)
        //        {
        //            if (icolumn == 7)
        //            {
        //               // dgvPriceUpdate.Rows.Add();
        //                dgvPriceUpdate.CurrentCell = dgvPriceUpdate[1, irow + 1];
        //            }
        //            else
        //            {
        //                if (icolumn != 8 && icolumn != 9)
        //                {
        //                    dgvPriceUpdate.CurrentCell = dgvPriceUpdate[icolumn + 1, irow];
        //                }
        //            }
        //            return true;
        //        }
        //        else
        //            return base.ProcessCmdKey(ref msg, keyData);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        private void txtDescriptionSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void txtTyerSizeSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void txtItemIDSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void cmbBrand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void cmbCategory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void cmbCountry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void cmbItemType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void cmbWhiteWall_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void txtUnitCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void txtListPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dgvPriceUpdate.Focus() == false)
                {
                    dgvPriceUpdate.Focus();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ItemMasterSearch();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
