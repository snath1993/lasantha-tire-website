using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace UserAutherization
{
    public partial class frmItemMaster : Form
    {
        private string ConnectionString;
        private DataSet dsLoan;
        private DataSet dsGL;
        private DataSet dsCustom;
        private DataSet dsItemClass;
        private DataSet dsItemType;
        private bool isedit=false;
        public string ItemSearch;
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
        public frmItemMaster()
        {
            InitializeComponent();
            setConnectionString();
          
        }


        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    return base.ProcessDialogKey(Keys.Tab);
            }
            return base.ProcessDialogKey(keyData);
        }
        private void frmItemMaster_Load(object sender, EventArgs e)
        {
          

            LoadComboData();
            LoadCustomFields();
            ClearFields();
            LoadDefaultGlAcc();
            ValidateDefaultData();
            LoadWH();
            cmbUM.SelectedIndex = 1;


        }
        private void LoadWH()
        {
            cblWarehouse.Items.Clear();
            String S = "SELECT[WhseId]FROM [tblWhseMaster]";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);


            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                cblWarehouse.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            }
        }
        private void ValidateDefaultData()
        {
            String SSql = "select CostMethod from tblItemMaster";
            SqlCommand cmdS2 = new SqlCommand(SSql);
            SqlDataAdapter daS2 = new SqlDataAdapter(SSql, ConnectionString);
            DataTable dt = new DataTable();
            daS2.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbCostMethod.Text = dt.Rows[0].ItemArray[0].ToString();
                cmbCostMethod.Enabled = false;
            }
            else
            {
                cmbCostMethod.Enabled = true;
            }
        }

        private void LoadDefaultGlAcc()
        {
            String SSql = "select SalesGLAccount,AdjustGL,LabChargGL from tblDefualtSetting ";
            SqlCommand cmdS2 = new SqlCommand(SSql);
            SqlDataAdapter daS2 = new SqlDataAdapter(SSql, ConnectionString);
            DataTable dt = new DataTable();
            daS2.Fill(dt);

            if(dt.Rows.Count>0)
            {
                cmbGLS.Text = dt.Rows[0].ItemArray[0].ToString();
                cmbGLI.Text = dt.Rows[0].ItemArray[1].ToString();
                cmbGLC.Text = dt.Rows[0].ItemArray[2].ToString();
            }
        }

        private void LoadCustomFields()
        {
            try
            {
               
                String StrSql = "SELECT * FROM tbl_ItemMasterCostomizeFields";
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
                        lblCustom6.Text = dt.Rows[i].ItemArray[5].ToString().Trim();
                        lblCustom7.Text = dt.Rows[i].ItemArray[6].ToString().Trim();
                        lblCustom8.Text = dt.Rows[i].ItemArray[7].ToString().Trim();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadComboData()
        {
            LoadGlAcc();
            LoadCustomComboFields();
            LoadItemClass();
            LoadItemType();
        }

        private void LoadItemType()
        {
            dsItemType = new DataSet();
            try
            {
                dsItemType.Clear();
                var StrSql = "SELECT [ID],[Description] FROM tbl_ItemType";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItemType, "dtCustom");

                cmbItemType.DataSource = dsItemType.Tables["dtCustom"];
                cmbItemType.DisplayMember = "Description";
                cmbItemType.ValueMember = "Description";

                cmbItemType.DisplayLayout.Bands["dtCustom"].Columns["ID"].Width = 60;
                cmbItemType.DisplayLayout.Bands["dtCustom"].Columns["Description"].Width = 160;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadItemClass()
        {
            dsItemClass = new DataSet();
            try
            {
                dsItemClass.Clear();
                var StrSql = "SELECT [ID],[Description] FROM tbl_ItemClass";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItemClass, "dtCustom");

                cmbItemClass.DataSource = dsItemClass.Tables["dtCustom"];
                cmbItemClass.DisplayMember = "Description";
                cmbItemClass.ValueMember = "ID";

                cmbItemClass.DisplayLayout.Bands["dtCustom"].Columns["ID"].Width = 40;
                cmbItemClass.DisplayLayout.Bands["dtCustom"].Columns["Description"].Width = 160;

                cmbItemClass.Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadCustomComboFields()
        {
            LoadCustom(cmbCustom1, "tbl_ItemCustom1");
            LoadCustom(cmbCustom2, "tbl_ItemCustom2");
            LoadCustom(cmbCustom3, "tbl_ItemCustom3");
            LoadCustom(cmbCustom4, "tbl_ItemCustom4");
            LoadCustom(cmbCustom5, "tbl_ItemCustom7");
            LoadCustom(cmbCustom6, "tbl_ItemCustom6");
            LoadCustom(cmbCustom7, "tbl_ItemCustom7");
            LoadCustom(cmbCustom8, "tbl_ItemCustom8");
        }

        private void LoadCustom(UltraCombo Combo,string Table)
        {
            dsCustom = new DataSet();
            try
            {
                dsCustom.Clear();
                var StrSql = "SELECT [ID],[Description] FROM " + Table;

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustom, "dtCustom");

                Combo.DataSource = dsCustom.Tables["dtCustom"];
                Combo.DisplayMember = "Description";
                Combo.ValueMember = "ID";
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadGlAcc()
        {
            dsGL = new DataSet();
            try
            {
                dsGL.Clear();
                var StrSql = "SELECT [AcountID],[AccountDescription] FROM [tblChartofAcounts]";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsGL, "dtGL");

                cmbGLS.DataSource = dsGL.Tables["dtGL"];
                cmbGLS.DisplayMember = "AcountID";
                cmbGLS.ValueMember = "AccountDescription";

                cmbGLS.DisplayLayout.Bands["dtGL"].Columns["AcountID"].Width = 100;
                cmbGLS.DisplayLayout.Bands["dtGL"].Columns["AccountDescription"].Width = 100;



                cmbGLI.DataSource = dsGL.Tables["dtGL"];
                cmbGLI.DisplayMember = "AcountID";
                cmbGLI.ValueMember = "AccountDescription";

                cmbGLI.DisplayLayout.Bands["dtGL"].Columns["AcountID"].Width = 100;
                cmbGLI.DisplayLayout.Bands["dtGL"].Columns["AccountDescription"].Width = 100;




                cmbGLC.DataSource = dsGL.Tables["dtGL"];
                cmbGLC.DisplayMember = "AcountID";
                cmbGLC.ValueMember = "AccountDescription";

                cmbGLC.DisplayLayout.Bands["dtGL"].Columns["AcountID"].Width = 100;
                cmbGLC.DisplayLayout.Bands["dtGL"].Columns["AccountDescription"].Width = 100;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            btnList.Enabled = true;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            ClearFields();
            EnableFields();
            LoadDefaultGlAcc();
            ValidateDefaultData();
            cmbUM.SelectedIndex = 1;
        }

        private void EnableFields()
        {
            txtItemID.Enabled = true;
            txtDescription.Enabled = true;
            txtDiscount.Enabled = true;
            txtDiscount2.Enabled = true;
            txtDiscount3.Enabled = true;
            txtPrice.Enabled = true;
            txtPrice2.Enabled = true;
            txtReorderQty.Enabled = true;
            txtUnitCost.Enabled = true;
          //  cmbCostMethod.Enabled = true;
            cmbCustom1.Enabled = true;
            cmbCustom2.Enabled = true;
            cmbCustom3.Enabled = true;
            cmbCustom4.Enabled = true;
            cmbCustom5.Enabled = true;
            cmbCustom6.Enabled = true;
            cmbCustom7.Enabled = true;
            cmbCustom8.Enabled = true;
            cmbGLC.Enabled = true;
            cmbGLI.Enabled = true;
            cmbGLS.Enabled = true;
            cmbItemClass.Enabled = true;
            cmbItemType.Enabled = true;
            cmbTaxType.Enabled = true;
            cmbUM.Enabled = true;
            cbInactive.Enabled = true;
            rtxtDescription.Enabled = true;
            txtProfit.Enabled = true;
        }
        private void DisableFields()
        {
            txtItemID.Enabled = false;
            txtDescription.Enabled = false;
            txtDiscount.Enabled = false;
            txtDiscount2.Enabled = false;
            txtDiscount3.Enabled = false;

            txtPrice.Enabled = false;
            txtPrice2.Enabled = false;
            txtReorderQty.Enabled = false;
            txtUnitCost.Enabled = false;
            cmbCostMethod.Enabled = false;
            cmbCustom1.Enabled = false;
            cmbCustom2.Enabled = false;
            cmbCustom3.Enabled = false;
            cmbCustom4.Enabled = false;
            cmbCustom5.Enabled = false;
            cmbCustom6.Enabled = false;
            cmbCustom7.Enabled = false;
            cmbCustom8.Enabled = false;
            cmbGLC.Enabled = false;
            cmbGLI.Enabled = false;
            cmbGLS.Enabled = false;
            cmbItemClass.Enabled = false;
            cmbItemType.Enabled = false;
            cmbTaxType.Enabled = false;
            cmbUM.Enabled = false;
            cbInactive.Enabled = false;
            rtxtDescription.Enabled = false;
            txtProfit.Enabled = false;
        }
        private void ClearFields()
        {
            txtItemID.Text = "";
            txtDescription.Text = "";
            rtxtDescription.Text = "";
            txtDiscount.Text = "0";
            txtDiscount2.Text = "0";
            txtDiscount3.Text = "0";
            txtPrice.Text = "0.00";
            txtPrice2.Text = "0.00";
            txtReorderQty.Text = "0";
            txtUnitCost.Text = "0.00";
            cmbCostMethod.Text = "";
            cmbCustom1.Text = "";
            cmbCustom2.Text = "";
            cmbCustom3.Text = "";
            cmbCustom4.Text = "";
            cmbCustom5.Text = "";
            cmbCustom6.Text = "";
            cmbCustom7.Text = "";
            cmbCustom8.Text = "";
            cmbGLC.Text = "";
            cmbGLI.Text = "";
            cmbGLS.Text = "";
            cmbItemClass.Text = "";
            cmbItemType.Text = "";
            cmbTaxType.Text = "";
            cmbUM.Text = "";
            cbInactive.Checked = false;
            txtProfit.Text = "0.00";


            int i = 0;
            while (i < cblWarehouse.Items.Count)
            {

                cblWarehouse.SetItemCheckState(i, (CheckState.Unchecked));


                i++;
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {

            frmItemMasterList fiml = new frmItemMasterList();
            fiml.ShowDialog();
            if (Class1.ItemSearch == "" || Class1.ItemSearch == null)
            {
                return;
            }
            btnNew_Click(null, null);
            LoadData();
            CheckWH();
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            Class1.ItemSearch = "";
            DisableFields();
        }
        private void CheckWH()
        {
            String StrSql = "SELECT WhseId FROM [tblItemWhse] WHERE ItemID='" + Class1.ItemSearch + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    int i = 0;
                    while (i < cblWarehouse.Items.Count)
                    {
                        if (cblWarehouse.Items[i].ToString().Trim() == dt.Rows[j]["WhseId"].ToString().Trim())
                        {
                            cblWarehouse.SetItemCheckState(i, (CheckState.Checked));

                        }
                        i++;
                    }

                }
            }
        }
        private void LoadItemMaster()
        {
            
           
          
        }

        private void LoadData()
        {
            try {
                String StrSql = "SELECT * FROM [tblItemMaster] WHERE ItemID='"+ Class1.ItemSearch + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
               
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        txtItemID.Text = dt.Rows[i]["ItemID"].ToString().Trim();
                        txtDescription.Text = dt.Rows[i]["ItemDescription"].ToString().Trim();
                        cmbItemClass.Text = dt.Rows[i]["ItemClass"].ToString().Trim();
                        rtxtDescription.Text = dt.Rows[i]["DescriptionForSale"].ToString().Trim();
                        txtPrice.Text = dt.Rows[i]["PriceLevel1"].ToString().Trim();
                        txtPrice2.Text = dt.Rows[i]["PriceLevel2"].ToString().Trim();
                        txtDiscount.Text = dt.Rows[i]["Discount"].ToString().Trim();
                        txtDiscount2.Text = dt.Rows[i]["Discount2"].ToString().Trim();
                        txtDiscount3.Text = dt.Rows[i]["Discount3"].ToString().Trim();


                        txtUnitCost.Text = dt.Rows[i]["UnitCost"].ToString().Trim();
                        cmbCostMethod.Text = dt.Rows[i]["CostMethod"].ToString().Trim();
                        cmbItemType.Text = dt.Rows[i]["Categoty"].ToString().Trim();
                        cmbUM.Text = dt.Rows[i]["UOM"].ToString().Trim();
                        cmbGLS.Text = dt.Rows[i]["SalesGLAccount"].ToString().Trim();
                        cmbGLI.Text = dt.Rows[i]["InventoryAcc"].ToString().Trim();
                        cmbGLC.Text = dt.Rows[i]["CostOfSalesAcc"].ToString().Trim();
                        cmbTaxType.Text = dt.Rows[i]["TaxType"].ToString().Trim();
                        txtReorderQty.Text = dt.Rows[i]["ReorderQty"].ToString().Trim();
                        var _Inactive= dt.Rows[i]["inactive"].ToString().Trim();
                        if (_Inactive == "True")
                        {
                            cbInactive.Checked = true;
                        }
                        cmbCustom1.Text = dt.Rows[i]["Custom1"].ToString().Trim();
                        cmbCustom2.Text = dt.Rows[i]["Custom2"].ToString().Trim();
                        cmbCustom3.Text = dt.Rows[i]["Custom3"].ToString().Trim();
                        cmbCustom4.Text = dt.Rows[i]["Custom4"].ToString().Trim();
                        cmbCustom5.Text = dt.Rows[i]["Custom5"].ToString().Trim();
                        cmbCustom6.Text = dt.Rows[i]["Custom6"].ToString().Trim();
                        cmbCustom7.Text = dt.Rows[i]["Custom7"].ToString().Trim();
                        cmbCustom8.Text = dt.Rows[i]["Custom8"].ToString().Trim();
                        txtProfit.Text = dt.Rows[i]["Profit"].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            isedit = true;
            EnableFields();
            txtItemID.Enabled = false;
            cmbItemClass.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ConnectorNew cn = new ConnectorNew();
            if (cn.IsOpenPeachtree() == false)
            {
                return;
            }
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlTransaction Trans = con.BeginTransaction();

            try
            {
                if (validation())
                {
                    return;
                }
                if (isedit == false)
                {
                    if (SearchInItemID(con, Trans))
                    {
                        return;
                    }
                    
                }

              
                    if (SearchInItemDis(con, Trans))
                    {
                        return;
                    }

                

                SaveEvent(con,Trans);
                CreateItemMaster();// import to peachtree
                btnSave.Enabled = false;
                btnEdit.Enabled = false;
                Trans.Commit();
                con.Close();
                MessageBox.Show("Item Save SucsessFully");
                DisableFields();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Trans.Rollback();
                con.Close();
            }
            
        }
        private void SaveWH(SqlConnection con, SqlTransaction trans)
        {
            int i = 0;
            while (i < cblWarehouse.Items.Count)
            {
                if (cblWarehouse.GetItemChecked(i) == true)
                {
                    var StrSql = "SELECT [ItemID] FROM [tblItemWhse] WHERE [ItemID]='" + txtItemID.Text.ToString() + "' AND WhseId='" + cblWarehouse.Items[i].ToString().Trim() + "'";

                    SqlCommand cmd = new SqlCommand(StrSql, con, trans);
                    SqlDataAdapter dAdapt = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dAdapt.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {

                    }
                    else
                    {
                        SqlCommand cmd21 = new SqlCommand("Insert into tblItemWhse (WhseId,WhseName,ItemId,ItemDis,QTY,TraDate,UnitCost,TranType,TotalCost,OPBQtry,UOM) values ('" + cblWarehouse.Items[i].ToString().Trim() + "','" + cblWarehouse.Items[i].ToString().Trim() + "','" + txtItemID.Text.ToString().Trim() + "','" + txtDescription.Text.ToString().Trim() + "','0','" + System.DateTime.Today + "','" + txtUnitCost.Text.ToString().Trim() + "','OpbBal','0.00','0','" + cmbUM.Text.ToString() + "')", con, trans);
                        cmd21.ExecuteNonQuery();

                        SqlCommand cmd11 = new SqlCommand("Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values ('0','1','OpbBal','" + System.DateTime.Today + "','OpbBal','0','" + txtItemID.Text.ToString().Trim() + "','0','" + txtUnitCost.Text.ToString().Trim() + "','0','" + cblWarehouse.Items[i].ToString().Trim() + "','" + txtPrice.Text.ToString().Trim() + "')", con, trans);
                        cmd11.ExecuteNonQuery();
                    }



                }
                i++;
            }
        }

        private bool SearchInItemDis(SqlConnection con , SqlTransaction trans)
        {
            var StrSql = "SELECT [ItemDescription] FROM [tblItemMaster] WHERE [ItemDescription]='" + txtDescription.Text.ToString() + "' and ItemID!='"+txtItemID.Text.ToString()+"'";

            SqlCommand cmd = new SqlCommand(StrSql, con, trans);
            SqlDataAdapter dAdapt = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            dAdapt.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("This Item Description Allready Exist In The System Please Fill Another Item Description");
                return true;
            }
            return false;
        }

        private void CreateItemMaster()
        {
            bool _Inactive = false;
            if (cbInactive.Checked)
            {
                _Inactive = true;
            }


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
            Writer.WriteString(txtItemID.Text.ToString().Trim());//Vendor ID should be here = Ptient No
            Writer.WriteEndElement();

            //if (i == 0)
            //{
            Writer.WriteStartElement("Description");
            //Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(txtDescription.Text.ToString().Trim());
            Writer.WriteEndElement();
            //}                       

            Writer.WriteStartElement("Class");
            Writer.WriteString(cmbItemClass.Value.ToString());//Date 
            Writer.WriteEndElement();

            Writer.WriteStartElement("isInactive");
            Writer.WriteString(_Inactive.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Description_for_Sales");
            Writer.WriteString(rtxtDescription.Text.ToString());//Cash Account
            Writer.WriteEndElement();//CreditMemoType

            Writer.WriteStartElement("Sales_Prices");

            Writer.WriteStartElement("Sales_Price_Info");
            Writer.WriteAttributeString("Key", "1");

            Writer.WriteStartElement("Sales_Price");
            Writer.WriteString(txtPrice.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteEndElement();

            Writer.WriteStartElement("Sales_Price_Info");
            Writer.WriteAttributeString("Key", "2");

            Writer.WriteStartElement("Sales_Price");
            Writer.WriteString(txtPrice2.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteEndElement();

            Writer.WriteEndElement();

            Writer.WriteStartElement("Last_Unit_Cost");
            Writer.WriteString(txtUnitCost.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_Sales_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbGLS.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_Inventory_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbGLI.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("GL_COGSSalary_Acct");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbGLC.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Type");
            Writer.WriteString(cmbItemType.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Stocking_UM");
            Writer.WriteString(cmbUM.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Tax_Type_Name");
            Writer.WriteString(cmbTaxType.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("CustomFields");

            Writer.WriteStartElement("CustomField");

            Writer.WriteStartElement("Value");
            Writer.WriteAttributeString("Index ", "1");//Change time and date both
            Writer.WriteString(cmbCustom1.Text.ToString());
            Writer.WriteEndElement();
           

            Writer.WriteStartElement("Value");
            Writer.WriteAttributeString("Index ", "2");//Change time and date both
            Writer.WriteString(cmbCustom2.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Value");
            Writer.WriteAttributeString("Index ", "3");//Change time and date both
            Writer.WriteString(cmbCustom3.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Value");
            Writer.WriteAttributeString("Index ", "4");//Change time and date both
            Writer.WriteString(cmbCustom4.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Value");
            Writer.WriteAttributeString("Index ", "5");//Change time and date both
            Writer.WriteString(cmbCustom5.Text.ToString());
            Writer.WriteEndElement();

            Writer.WriteEndElement();

            Writer.WriteEndElement();

            Writer.WriteEndElement();
            //********************
            Writer.WriteEndElement();//last line
            Writer.Close();

            Connector conn = new Connector();
            conn.ImportItemMaster();

        }

        private void SaveEvent(SqlConnection con, SqlTransaction trans)
        {
            if (isedit == true)
            {
                string S21 = "DELETE FROM [dbo].[tblItemMaster] WHERE ItemID='"+ txtItemID.Text.ToString() + "'";

                SqlCommand cmd1 = new SqlCommand(S21, con, trans);
                cmd1.ExecuteNonQuery();
            }
            bool _Inactive = false;
            if (cbInactive.Checked)
            {
                _Inactive = true;
            }

            string S2 = "INSERT INTO [tblItemMaster]([ItemID],[ItemDescription],[ItemClass],UnitPrice,[DescriptionForSale],[PriceLevel1],[PriceLevel2],[Discount],[UnitCost],[CostMethod],[Categoty],[UOM],[SalesGLAccount],[InventoryAcc],[CostOfSalesAcc],[TaxType],[ReorderQty],[inactive],[Custom1],[Custom2],[Custom3],[Custom4],[Custom5],[Custom6],[Custom7],[Custom8],Profit,Discount2,Discount3)" +
                            "VALUES ('" + txtItemID.Text.ToString() + "','" + txtDescription.Text.ToString() + "','" + cmbItemClass.Value.ToString() + "','"+Convert.ToDecimal( txtPrice.Text.ToString())+"','" + rtxtDescription.Text.ToString() + "','" + txtPrice.Text.ToString() + "','" + txtPrice2.Text.ToString() + "','" + txtDiscount.Text.ToString() + "','" + txtUnitCost.Text.ToString() + "','" + cmbCostMethod.Text.ToString() + "','" + cmbItemType.Text.ToString() + "','" + cmbUM.Text.ToString() + "','" + cmbGLS.Text.ToString() + "','" + cmbGLI.Text.ToString() + "','" + cmbGLC.Text.ToString() + "','" + cmbTaxType.Text.ToString() + "','" + txtReorderQty.Text.ToString() + "','"+_Inactive+"','"+cmbCustom1.Text.ToString()+ "','" + cmbCustom2.Text.ToString() + "','" + cmbCustom3.Text.ToString() + "','" + cmbCustom4.Text.ToString() + "','" + cmbCustom5.Text.ToString() + "','" + cmbCustom6.Text.ToString() + "','" + cmbCustom7.Text.ToString() + "','" + cmbCustom8.Text.ToString() + "','"+txtProfit.Text.ToString()+ "','" + txtDiscount2.Text.ToString() + "','" + txtDiscount3.Text.ToString() + "')";

            SqlCommand cmd = new SqlCommand(S2, con, trans);
            cmd.ExecuteNonQuery();


            SqlCommand command;

           string StrSql = "UPDATE tblP_Order SET Brand='" + cmbCustom2.Text.ToString() + "', Country ='"+ cmbCustom3.Text.ToString() + "' where ItemID='"+ txtItemID.Text.ToString() + "'";
            SqlCommand cmd3 = new SqlCommand(StrSql, con, trans);
            cmd3.ExecuteNonQuery();

            string StrSql2 = "UPDATE tblItemWhse SET ItemDis='" + txtDescription.Text.ToString() + "'  where ItemId='" + txtItemID.Text.ToString() + "'";

            SqlCommand cmd2 = new SqlCommand(StrSql2, con, trans);
            cmd2.ExecuteNonQuery();

            SaveWH(con, trans);
        }

        private bool SearchInItemID(SqlConnection con , SqlTransaction trans)
        {
            var StrSql = "SELECT [ItemID] FROM [tblItemMaster] WHERE [ItemID]='" + txtItemID.Text.ToString()+"'";

            SqlCommand cmd = new SqlCommand(StrSql,con,trans);
            SqlDataAdapter dAdapt = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            dAdapt.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("This Item ID Allready Exist In The System Please Fill Another Item ID");
                return true; 
            }
            return false;
        }

        private bool validation()
        {
            if (txtItemID.Text == "")
            {
                MessageBox.Show("Please Fill Item ID");
                return true;
            }
            if (txtDescription.Text == "")
            {
                MessageBox.Show("Please Fill Item Description");
                return true;
            }
            if (cmbItemClass.Text == "")
            {
                MessageBox.Show("Please Fill Item Class");
                return true;
            }
            if (txtPrice.Text == "")
            {
                MessageBox.Show("Please Fill Item Price 1");
                return true;
            }
            if (cmbCostMethod.Text == "")
            {
                MessageBox.Show("Please Fill Item Cost Method");
                return true;
            }
            if (cmbItemType.Text == "")
            {
                MessageBox.Show("Please Fill Item Type");
                return true;
            }
            if (cmbUM.Text == "")
            {
                MessageBox.Show("Please Fill Item Unit Of Measure");
                return true;
            }
            if (cmbGLS.Text == "")
            {
                MessageBox.Show("Please Fill Item GL Sales Account");
                return true;
            }
            if (cmbGLI.Text == "")
            {
                MessageBox.Show("Please Fill Item GL Inventory Account");
                return true;
            }
            if (cmbGLC.Text == "")
            {
                MessageBox.Show("Please Fill Item GL Cost OF Sales Account");
                return true;
            }

            if(cmbItemType.Text.Trim()== "DAG TYRES" || cmbItemType.Text.Trim() == "KATTA TYRES"|| cmbItemType.Text.Trim() == "RADIAL DAG TYRES" || cmbItemType.Text.Trim() == "REBUILD TYRES" || cmbItemType.Text.Trim() == "SAVIYA TYRES" || cmbItemType.Text.Trim() == "TYRES")
            {
                if(cmbCustom1.Text.Trim()=="")
                {
                    MessageBox.Show("Please Fill Item Size");
                    return true;
                }
                if (cmbCustom2.Text.Trim() == "")
                {
                    MessageBox.Show("Please Fill Country");
                    return true;
                }
                if (cmbCustom3.Text.Trim() == "")
                {
                    MessageBox.Show("Please Fill Brand");
                    return true;
                }
              
            }
            return false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmItemMasterCustomFields imcf = new frmItemMasterCustomFields();
            frmMain.CostomizeFormName = "ItemMaster";
            imcf.ShowDialog();
            LoadCustomFields();
        }

        private void txtItemID_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void linkLabel2_MouseClick(object sender, MouseEventArgs e)
        {
            frmItemMasterDefault id = new frmItemMasterDefault();
            id.ShowDialog();
            ValidateDefaultData();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void cmbCustom3_RowSelected(object sender, RowSelectedEventArgs e)
        {

            //if (cmbItemClass.Text.ToString().Trim() == "Charge Item")
            //{
            //    int i = 0;
            //    while (i < cblWarehouse.Items.Count)
            //    {

            //        cblWarehouse.SetItemCheckState(i, (CheckState.Checked));


            //        i++;
            //    }
            //}

            //if ((cmbItemType.Text != "DAG TYRES" && cmbItemType.Text != "KATTA TYRES" && cmbItemType.Text != "RADIAL DAG TYRES" && cmbItemType.Text != "REBUILD TYRES" && cmbItemType.Text != "SAVIYA TYRES" && cmbItemType.Text != "TYRES") && cmbItemType.Text != "")
            //{
            //    int i = 0;
            //    while (i < cblWarehouse.Items.Count)
            //    {

            //        cblWarehouse.SetItemCheckState(i, (CheckState.Checked));


            //        i++;
            //    }
            //}

             if (cmbCustom3.Text != "")
            {
                if (cmbCustom3.Text == "MAXXIES" || cmbCustom3.Text == "MAXXIS MOTOR BIKE")
                {
                    cblWarehouse.SetItemCheckState(1, (CheckState.Checked));
                    cblWarehouse.SetItemCheckState(0, (CheckState.Unchecked));
                }

                else
                {
                    cblWarehouse.SetItemCheckState(0, (CheckState.Checked));
                    cblWarehouse.SetItemCheckState(1, (CheckState.Unchecked));
                }
            }

             else
            {
                cblWarehouse.SetItemCheckState(0, (CheckState.Checked));
                cblWarehouse.SetItemCheckState(1, (CheckState.Unchecked));
            }


            if (cmbCustom3.Text.Trim()== "MAXXIES"||cmbCustom3.Text.Trim()== "MAXXIS MOTOR BIKE")
            {
                var StrSql = "SELECT SalesGLAccount,InventoryAcc,CostOfSalesAcc FROM [tblItemMaster] WHERE [Custom3]='MAXXIES' or [Custom3]='MAXXIS MOTOR BIKE'";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);
                if (dt.Rows.Count > 0 && dt.Rows[0].ItemArray[0].ToString()!="")
                {
                    cmbGLS.Text = dt.Rows[0].ItemArray[0].ToString();
                    cmbGLI.Text = dt.Rows[0].ItemArray[1].ToString();
                    cmbGLC.Text = dt.Rows[0].ItemArray[2].ToString();
                }
                
            }
            else
            {
                var StrSql = "SELECT SalesGLAccount,InventoryAcc,CostOfSalesAcc FROM [tblItemMaster] WHERE [Custom3] <>'MAXXIES' and [Custom3] <>'MAXXIS MOTOR BIKE'";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);
                if (dt.Rows.Count > 0 && dt.Rows[0].ItemArray[0].ToString() != "")
                {
                    cmbGLS.Text = dt.Rows[0].ItemArray[0].ToString();
                    cmbGLI.Text = dt.Rows[0].ItemArray[1].ToString();
                    cmbGLC.Text = dt.Rows[0].ItemArray[2].ToString();
                }
            }
        }

        private void cmbItemClass_RowSelected(object sender, RowSelectedEventArgs e)
        {
            if(cmbItemClass.Text.ToString().Trim()== "Charge Item")
            {
                cblWarehouse.SetItemCheckState(0, (CheckState.Checked));
                    cblWarehouse.SetItemCheckState(1, (CheckState.Unchecked));


            }

          
            else if(cmbCustom3.Text !="")
            {
                if(cmbCustom3.Text == "MAXXIES" || cmbCustom3.Text == "MAXXIS MOTOR BIKE")
                {
                    cblWarehouse.SetItemCheckState(1, (CheckState.Checked));
                    cblWarehouse.SetItemCheckState(0, (CheckState.Unchecked));
                }

                else
                {
                    cblWarehouse.SetItemCheckState(0, (CheckState.Checked));
                    cblWarehouse.SetItemCheckState(1, (CheckState.Unchecked));
                }
            }


        }

        private void cmbItemType_RowSelected(object sender, RowSelectedEventArgs e)
        {
            if (cmbItemClass.Text.ToString().Trim() == "Charge Item")
            {
                cblWarehouse.SetItemCheckState(0, (CheckState.Checked));
                cblWarehouse.SetItemCheckState(1, (CheckState.Unchecked));
            }


             if (cmbCustom3.Text != "")
            {
                if (cmbCustom3.Text == "MAXXIES" || cmbCustom3.Text == "MAXXIS MOTOR BIKE")
                {
                    cblWarehouse.SetItemCheckState(1, (CheckState.Checked));
                    cblWarehouse.SetItemCheckState(0, (CheckState.Unchecked));
                }

                else
                {
                    cblWarehouse.SetItemCheckState(0, (CheckState.Checked));
                    cblWarehouse.SetItemCheckState(1, (CheckState.Unchecked));
                }
            }
             else
            {
                cblWarehouse.SetItemCheckState(0, (CheckState.Checked));
                cblWarehouse.SetItemCheckState(1, (CheckState.Unchecked));
            }
        }
    }
}
