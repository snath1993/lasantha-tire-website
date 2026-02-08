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
    public partial class frmInventoryMovement : Form
    {
        public frmInventoryMovement()
        {
            InitializeComponent();
            setConnectionString();
        }


        public string StrSql;

        public DataSet dsItem;
        public DataSet dsDocType;
        public DataSet dsWarehouse;

        //public DataSet dsVendor;
        //public DataSet dsAR;
        public string sMsg = "Peachtree - GRN";
        public DSInventorymovement ObjMovement = new DSInventorymovement();
       // public In GRNData = new DSGRNList();

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
        public static string GetDateTime(DateTime DtGetDate)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(DtGetDate);
                string Dformat = "MM/dd/yyyy";
                return DTP.ToString(Dformat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void frmInventoryMovement_Load(object sender, EventArgs e)
        {
            GetItemID();
            GetDocType();
            GetWareHouseDataSet();
        }
        public void GetWareHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName FROM tblWhseMaster order by WhseId";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 75;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 150;
                // cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;  
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void GetItemID()
        {
            dsItem = new DataSet();
            try
            {
                dsItem.Clear();
                StrSql = " SELECT ItemID, ItemDescription FROM tblItemMaster order by ItemID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItem, "DtItem");

                cmbItem.DataSource = dsItem.Tables["DtItem"];
                cmbItem.DisplayMember = "ItemID";
                cmbItem.ValueMember = "ItemID";
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemID"].Width = 125;
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDescription"].Width = 200;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void GetDocType()
        {
            dsDocType = new DataSet();
            try
            {
                dsDocType.Clear();
                StrSql = " SELECT DocType, DocDescription FROM tblDocType order by DocType";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsDocType, "DtDoc");

                cmbDocType.DataSource = dsDocType.Tables["DtDoc"];
                cmbDocType.DisplayMember = "DocType";
                cmbDocType.ValueMember = "DocType";
                cmbDocType.DisplayLayout.Bands["DtDoc"].Columns["DocType"].Width = 50;
                cmbDocType.DisplayLayout.Bands["DtDoc"].Columns["DocDescription"].Width = 100;

            }
            catch (Exception)
            {

                throw;
            }
        }




        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                string Item = "";
                string DocType = "";
                string WareHouse = "";
                if (cmbItem.Value != null) Item = cmbItem.Value.ToString().Trim();
                if (cmbDocType.Value != null) DocType = cmbDocType.Value.ToString().Trim();
                if (cmbWarehouse.Value != null) WareHouse = cmbWarehouse.Value.ToString().Trim();

                ObjMovement.Clear();

                //string sSQL_IN = "SELECT ItemID, ISNULL(SUM(ISNULL(Qty, 0)),0) AS Qty" +
                //    " FROM dbo.tbItemlActivity " +
                //" where    (DocReference = 'True') and WarehouseID like '%'+'" + WareHouse + "' and ItemID like '%'+'" + Item + "'";
                //if (!chkDateRange.Checked)
                //    sSQL_IN = sSQL_IN + " and TransDate <='" + dtpFromDate.Text.ToString().Trim() + "'";
                //sSQL_IN = sSQL_IN + " GROUP BY ItemID  ";

                //SqlCommand cmd_IN = new SqlCommand(sSQL_IN);
                //SqlConnection con_IN = new SqlConnection(ConnectionString);
                //SqlDataAdapter da_IN = new SqlDataAdapter(sSQL_IN, con_IN);
                //da_IN.Fill(ObjMovement, "dtItemActivity_IN");
                //---------------------------------------------------------------

                //string sSQL_OUT = "SELECT ItemID, ISNULL(SUM(ISNULL(Qty, 0)),0) AS Qty" +
                //    " FROM dbo.tbItemlActivity " +
                //" where    (DocReference = 'False') and WarehouseID like '%'+'" + WareHouse + "' and ItemID like '%'+'" + Item + "'";
                //if (!chkDateRange.Checked)
                //    sSQL_OUT = sSQL_OUT + " and TransDate <='" + dtpFromDate.Text.ToString().Trim() + "'";
                //sSQL_OUT = sSQL_OUT + " GROUP BY ItemID  ";

                //SqlCommand cmd_OUT = new SqlCommand(sSQL_OUT);
                //SqlConnection con_OUT = new SqlConnection(ConnectionString);
                //SqlDataAdapter da_OUT = new SqlDataAdapter(sSQL_OUT, con_OUT);
                //da_OUT.Fill(ObjMovement, "dtItemActivity_OUT");
                //--------------------------------------------------------


                string sSQL = "Select tbItemlActivity.*,'" + GetDateTime(dtpFromDate.Value) + "' as FromDate from tbItemlActivity where TranType NOT IN ('Sup-Invoice','Del-Note','SalesOrder')" +
                    " and ItemID like '%'+'" + Item + "' " +
                    " and DocType like '%'+'" + DocType + "' " +
                    " and WarehouseID like '%" + WareHouse + "' and WarehouseID like '" + WareHouse + "%' ";

                if (!chkDateRange.Checked)
                    sSQL = sSQL + " and  [TransDate] <='" + GetDateTime(dtpToDate.Value) + "'";
                        //[TransDate]  >= '" + dtpFromDate.Text.ToString().Trim() + "' and 
                       

                
                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(ObjMovement, "dtItemActivity");

                frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this,dtpFromDate.Value);
                ObjMoveprint.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // //MachineCode like '%'+ @MacCode         
        }

        //private void btnView_Click(object sender, EventArgs e)
        //{
        //    if (rbtAll.Checked == true && ChkTranType.Checked == false && CheckItemID.Checked == false && ChkWarehouse.Checked == false)
        //    {
        //        try
        //        {
        //            ObjMovement.Clear();
        //            String S3 = "Select * from tbItemlActivity where TranType !='Sup-Invoice'";
        //            SqlCommand cmd3 = new SqlCommand(S3);
        //            SqlConnection con3 = new SqlConnection(ConnectionString);
        //            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
        //            da3.Fill(ObjMovement, "dtItemActivity");

        //            frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this);
        //            ObjMoveprint.Show();
        //        }
        //        catch { }

        //    }

        //    else if (rbtAll.Checked == true && CheckItemID.Checked == true && ChkTranType.Checked == false)
        //    {
        //        try
        //        {

        //            ObjMovement.Clear();
        //            String S3 = "Select * from tbItemlActivity where TranType !='Sup-Invoice' and ItemID='" + cmbItem.Value.ToString().Trim() + "'";
        //            SqlCommand cmd3 = new SqlCommand(S3);
        //            SqlConnection con3 = new SqlConnection(ConnectionString);
        //            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
        //            da3.Fill(ObjMovement, "dtItemActivity");

        //            frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this);
        //            ObjMoveprint.Show();


        //        }
        //        catch { }

        //    }


        //    else if (rbtAll.Checked == true && CheckItemID.Checked == true && ChkTranType.Checked == true)
        //    {
        //        try
        //        {

        //            ObjMovement.Clear();
        //            String S3 = "Select * from tbItemlActivity where TranType !='Sup-Invoice' and ItemID='" + cmbItem.Value.ToString().Trim() + "' and DocType='" + cmbDocType.Value.ToString().Trim() + "'";
        //            SqlCommand cmd3 = new SqlCommand(S3);
        //            SqlConnection con3 = new SqlConnection(ConnectionString);
        //            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
        //            da3.Fill(ObjMovement, "dtItemActivity");

        //            frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this);
        //            ObjMoveprint.Show();


        //        }
        //        catch { }

        //    }

        //    else if (rbtAll.Checked == true && CheckItemID.Checked == false && ChkTranType.Checked == true)
        //    {
        //        try
        //        {

        //            ObjMovement.Clear();
        //            String S3 = "Select * from tbItemlActivity where TranType !='Sup-Invoice'  and DocType='" + cmbDocType.Value.ToString().Trim() + "'";
        //            SqlCommand cmd3 = new SqlCommand(S3);
        //            SqlConnection con3 = new SqlConnection(ConnectionString);
        //            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
        //            da3.Fill(ObjMovement, "dtItemActivity");

        //            frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this);
        //            ObjMoveprint.Show();


        //        }
        //        catch { }

        //    }



        //    //================================================================
        //    else if (rbtDateRange.Checked == true && ChkTranType.Checked == false && CheckItemID.Checked == false)
        //    {
        //        try
        //        {
        //            ObjMovement.Clear();
        //            String S3 = "Select * from tbItemlActivity  where [TransDate]  >= '" + dtpFromDate.Text.ToString().Trim() + "' and  [TransDate] <='" + dtpToDate.Text.ToString().Trim() + "' and TranType !='Sup-Invoice'";
        //            SqlCommand cmd3 = new SqlCommand(S3);
        //            SqlConnection con3 = new SqlConnection(ConnectionString);
        //            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
        //            da3.Fill(ObjMovement, "dtItemActivity");

        //            frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this);
        //            ObjMoveprint.Show();

        //        }
        //        catch { }
        //        //  rbtDateRange

        //    }


        //    else if (rbtDateRange.Checked == true && CheckItemID.Checked == true && ChkTranType.Checked == false && ChkWarehouse.Checked == false)
        //    {
        //        try
        //        {
        //            ObjMovement.Clear();
        //            String S3 = "Select * from tbItemlActivity  where [TransDate]  >= '" + dtpFromDate.Text.ToString().Trim() + "' and  [TransDate] <='" + dtpToDate.Text.ToString().Trim() + "' and TranType !='Sup-Invoice'and ItemID='" + cmbItem.Value.ToString().Trim() + "'";
        //            SqlCommand cmd3 = new SqlCommand(S3);
        //            SqlConnection con3 = new SqlConnection(ConnectionString);
        //            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
        //            da3.Fill(ObjMovement, "dtItemActivity");

        //            frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this);
        //            ObjMoveprint.Show();

        //        }
        //        catch { }
        //        //  rbtDateRange

        //    }

        //    else if (rbtDateRange.Checked == true && CheckItemID.Checked == true && ChkTranType.Checked == true)
        //    {
        //        try
        //        {
        //            ObjMovement.Clear();
        //            String S3 = "Select * from tbItemlActivity  where [TransDate]  >= '" + dtpFromDate.Text.ToString().Trim() + "' and  [TransDate] <='" + dtpToDate.Text.ToString().Trim() + "' and TranType !='Sup-Invoice' and ItemID='" + cmbItem.Value.ToString().Trim() + "' and DocType='" + cmbDocType.Value.ToString().Trim() + "'";
        //            SqlCommand cmd3 = new SqlCommand(S3);
        //            SqlConnection con3 = new SqlConnection(ConnectionString);
        //            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
        //            da3.Fill(ObjMovement, "dtItemActivity");

        //            frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this);
        //            ObjMoveprint.Show();

        //        }
        //        catch { }
        //        //  rbtDateRange

        //    }
        //    else if (rbtDateRange.Checked == true && CheckItemID.Checked == false && ChkTranType.Checked == true)
        //    {
        //        try
        //        {
        //            ObjMovement.Clear();
        //            String S3 = "Select * from tbItemlActivity  where [TransDate]  >= '" + dtpFromDate.Text.ToString().Trim() + "' and  [TransDate] <='" + dtpToDate.Text.ToString().Trim() + "' and TranType !='Sup-Invoice' and DocType='" + cmbDocType.Value.ToString().Trim() + "'";
        //            SqlCommand cmd3 = new SqlCommand(S3);
        //            SqlConnection con3 = new SqlConnection(ConnectionString);
        //            SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
        //            da3.Fill(ObjMovement, "dtItemActivity");

        //            frmInvMovementPrint ObjMoveprint = new frmInvMovementPrint(this);
        //            ObjMoveprint.Show();

        //        }
        //        catch { }
        //        //  rbtDateRange

        //    }



        //}





        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RdoDocType_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtAll.Checked)
            {
                chkDateRange.Checked = true;
                ChkTranType.Checked = true;
                ChkWarehouse.Checked = true;
                CheckItemID.Checked = true;
            }
        }

        private void CheckItemID_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckItemID.Checked)
            {
                cmbItem.Text = "";
                cmbItem.Enabled = false;
            }
            else
            {
                rbtAll.Checked = false;
                cmbItem.Enabled = true;
            }
        }

        private void ChkTranType_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkTranType.Checked)
            {
                cmbDocType.Text = "";
                cmbDocType.Enabled = false;
            }
            else
            {
                rbtAll.Checked = false;
                cmbDocType.Enabled = true;
            }
        }

        private void ChkWarehouse_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkWarehouse.Checked)
            {
                cmbWarehouse.Text = "";
                cmbWarehouse.Enabled = false;
            }
            else
            {
                rbtAll.Checked = false;
                cmbWarehouse.Enabled = true;
            }
        }

        private void chkDateRange_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDateRange.Checked)
            {
                dtpFromDate.Enabled = false;
                dtpToDate.Enabled = false;               
            }
            else
            {
                dtpFromDate.Enabled = true;
                dtpToDate.Enabled = true;
                rbtAll.Checked = false;
            }
        }
    }
}