using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using UserAutherization;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Interop.PeachwServer;

namespace UserAutherization
{
    public partial class frmBOMBuilding : Form
    {
        public DSEstimate DsEst = new DSEstimate();
        bool run;
        bool add;
        bool edit;
        bool delete;
        DataTable dtUser = new DataTable();
        Controlers objControlers = new Controlers();

        string sMsg = "Build Assembly";

        public string StrSql;
        public double dblLineTot;
        public double dblSubTot;
        public double dblGrocessAmt;
        public double dblGrandTot;
        public double dblDiscAmt;
        public double dblDiscPer;
        public double dblVatAmount;
        public double dblNbtAmount;
        public double dblNetAmount;
        public double dblVat;
        public double dblNbt;
        public static string ConnectionString;
        int intEstomateProcode;
        string BuildReferenceNo;
        public Boolean blnEdit;
        public int intProcess;

        public string StrFItem;
        public string StrAssmblyGL;
        public string StrDefaultWH;

        public DataSet dsWarehouse;
        public DataSet dsJobCode;
        public DataSet dsHeaderItem;

        public bool boolSerch = false;
        public int intEdit = 0;

        SqlConnection con;
        SqlTransaction Trans;



        public static DateTime UserWiseDate = System.DateTime.Now;

        public frmBOMBuilding(int intNo)
        {
            InitializeComponent();
            setConnectionString();
            if (intNo == 0)
            {
                intEstomateProcode = 0;
            }
        }

        public void GetWH()
        {

            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster order by WhseId";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWH");
                cmbWH.DataSource = dsWarehouse.Tables["DtWH"];
                cmbWH.DisplayMember = "WhseId";
                cmbWH.ValueMember = "WhseId";
                cmbWH.DisplayLayout.Bands["DtWH"].Columns["WhseId"].Width = 70;
                cmbWH.DisplayLayout.Bands["DtWH"].Columns["WhseName"].Width = 150;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void setValue()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                string RefNo = Search.AssemblyReference;
                ClearHeader();
                DeleteRows();
                EnableHeader(true);
                EnableFoter(true);
                ViewHeader(RefNo, myConnection, myTrans);
                ViewDetails(RefNo, myConnection, myTrans);
                EnableHeader(false);
                EnableFoter(false);
                myTrans.Commit();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
                throw;

            }

        }

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;

            }
            catch { }
        }

        public void UpdateInvoiceNo(Int32 intInvoiceNo, SqlConnection con, SqlTransaction Trans)
        {

            SqlCommand command;
            Int32 intX;
            Int32 intZ;
            string StrInvNo;
            Int32 intP;
            Int32 intI;
            String StrInV;
            string StrUpdateInvNo;

            try
            {
                StrSql = "SELECT BuildReferencePref, BuildReferencePad, BuildReferenceNum FROM tblDefualtSetting";
                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = (int.Parse(dt.Rows[0].ItemArray[1].ToString().Trim()));
                    intZ = (int.Parse(dt.Rows[0].ItemArray[2].ToString().Trim()));

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();
                    StrUpdateInvNo = StrInvNo + StrInV.Substring(1, intX);

                    StrSql = "UPDATE EST_HEADER SET  EstimateNo='" + StrUpdateInvNo + "' WHERE AutoIndex=" + intInvoiceNo + "";
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;
                StrSql = "SELECT  TOP 1(BuildReferenceNum) FROM tblDefualtSetting ORDER BY BuildReferenceNum DESC";
                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intInvNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                }
                else
                {
                    intInvNo = 1;
                }
                StrSql = "UPDATE tblDefualtSetting SET BuildReferenceNum='" + intInvNo + "'";
                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }

        }
        public void GetInvNo()
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT BuildReferencePref, BuildReferencePad, BuildReferenceNum FROM tblDefualtSetting";


                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    txtBuildRefNo.Text = StrInvNo + StrInV.Substring(1, intX);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetInvNoField(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT BuildReferencePref, BuildReferencePad, BuildReferenceNum FROM tblDefualtSetting";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }
                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    return StrInvNo + StrInV.Substring(1, intX);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetJobStatus()
        {
            //cmbJobStatus.Items.Clear();
            //cmbJobStatus.Items.Add("Quote");
            //cmbJobStatus.Items.Add("Active");
            //cmbJobStatus.Items.Add("Complete");
            //cmbJobStatus.Items.Add("Finished");

            //cmbJobStatus.SelectedIndex = 2;
        }

        public void GetItemDataSet()
        {
            try
            {

                StrSql = "SELECT ItemID,ItemDescription,UnitCost,ItemClass,SalesGLAccount FROM tblItemMaster";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";

                }
                else
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }

        }

        private void DeleteRows()
        {
            foreach (UltraGridRow ugR in ug.Rows.All)
            {
                ugR.Delete(false);
            }
        }

        public void DeleteEmpGrid()
        {
            ug.PerformAction(UltraGridAction.CommitRow);
            foreach (UltraGridRow ugR in ug.Rows.All)
            {
                if (ugR.Cells[1].Value.ToString().Trim().Length == 0)
                {
                    ugR.Delete(false);
                }
            }
        }

        private void GrandTotal()
        {
            //dblGrandTot = 0;
            //dblGrocessAmt = 0;
            //dblSubTot = 0;
            //dblVatAmount = 0;
            //dblNbtAmount = 0;

            //int intGridRow;

            //for (intGridRow = 0; intGridRow < ug.Rows.Count; intGridRow++)
            //{
            //    dblSubTot += double.Parse(ug.Rows[intGridRow].Cells[5].Value.ToString());

            //}
            //dblDiscPer = double.Parse(txtDiscPer.Value.ToString());
            //if (double.Parse(txtDiscPer.Value.ToString()) > 0)
            //{
            //    dblDiscAmt = (dblSubTot * dblDiscPer) / 100;

            //}
            //else
            //{
            //    dblDiscAmt = 0;
            //}

            //dblGrocessAmt = dblSubTot - dblDiscAmt;


            //if (double.Parse(txtNBTPer.Value.ToString()) > 0)
            //{
            //    dblNbtAmount = ((dblGrocessAmt * double.Parse(txtNBTPer.Value.ToString())) / 100);
            //}
            //else
            //{
            //    dblNbtAmount = 0;
            //}

            //if (double.Parse(txtVatPer.Value.ToString()) > 0)
            //{
            //    dblVatAmount = (((dblGrocessAmt + dblNbtAmount) * double.Parse(txtVatPer.Value.ToString())) / 100);
            //}
            //else
            //{
            //    dblVatAmount = 0;
            //}

            //dblNetAmount = dblGrocessAmt + dblNbtAmount + dblVatAmount;

            //txtSubValue.Value = dblSubTot;
            //txtDiscAmount.Value = dblDiscAmt;
            //txtGrossValue.Value = dblGrocessAmt;
            //txtNBT.Value = dblNbtAmount;
            //txtVat.Value = dblVatAmount;
            //txtNetValue.Value = dblNetAmount;

        }

        private void ug_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {

                case 37:
                    {
                        //ug.PerformAction(UltraGridAction.ExitEditMode);
                        ug.PerformAction(UltraGridAction.PrevCell);
                        //ug.PerformAction(UltraGridAction.EnterEditMode);
                        break;
                    }
                case 38:
                    {
                        //ug.PerformAction(UltraGridAction.ExitEditMode);
                        ug.PerformAction(UltraGridAction.AboveCell);
                        //ug.PerformAction(UltraGridAction.EnterEditMode);
                        break;
                    }
                case 39:
                    {
                        //ug.PerformAction(UltraGridAction.ExitEditMode);
                        ug.PerformAction(UltraGridAction.NextCell);
                        //ug.PerformAction(UltraGridAction.EnterEditMode);
                        break;
                    }
                case 40:
                    {
                        //ug.PerformAction(UltraGridAction.ExitEditMode);
                        ug.PerformAction(UltraGridAction.BelowCell);
                        //ug.PerformAction(UltraGridAction.EnterEditMode);
                        break;
                    }

                case 9:
                    {

                        if (ug.ActiveCell.Column.Key == "Quantity")
                        {
                            if (ug.ActiveRow.HasNextSibling() == false)
                            {
                                UltraGridRow ugR;
                                ugR = ug.DisplayLayout.Bands[0].AddNew();
                                ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                ugR.Cells["LineNo"].Selected = true;
                                ugR.Cells["LineNo"].Activated = true;

                            }


                        }
                        break;
                    }
            }

        }

        private double LineCalculation(double UnitPrice, double Quantity)
        {
            dblLineTot = 0;
            double lineTotal = 0;
            dblLineTot = UnitPrice * Quantity;
            lineTotal = dblLineTot;
            return lineTotal;

        }


        private void ug_Click(object sender, EventArgs e)
        {

            UltraGridRow ugR;

            if (HeaderValidation() == false)
            {
                return;
            }

            if (ug.Rows.Count == 0)
            {
                ugR = ug.DisplayLayout.Bands[0].AddNew();
                ugR.Cells["LineNo"].Value = ugR.Index + 1;
                ugR.Cells["LineNo"].Selected = true;
                ugR.Cells["LineNo"].Activated = true;
            }
        }

        public Boolean IsGridValidation()
        {

            if (ug.Rows.Count == 0)
            {
                return false;
            }

            foreach (UltraGridRow ugR in ug.Rows)
            {

                if (IsGridExitCode(ugR.Cells["ItemCode"].Text) == false)
                {
                    MessageBox.Show("Invalid Item Code.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        public Boolean HeaderValidation()
        {
            if (cmbAssemID.Value.ToString() == "")
            {
                return false;
            }
            return true;
        }

        public void ViewDetails(string RefNo, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = " SELECT dbo.tblAssemblyDetails.BOMReference, dbo.tblAssemblyDetails.ComponentID, dbo.tblAssemblyDetails.CompDesc, dbo.tblAssemblyDetails.WarehouseID," +
                         " dbo.tblAssemblyDetails.QtyRequired, dbo.tblAssemblyDetails.QtyOnHand, dbo.tblItemMaster.ItemClass, dbo.tblItemMaster.UnitPrice, " +
                         " dbo.tblItemMaster.SalesGLAccount, dbo.tblItemMaster.UnitCost" +
                         " FROM  dbo.tblAssemblyDetails INNER JOIN" +
                         " dbo.tblItemMaster ON dbo.tblAssemblyDetails.ComponentID = dbo.tblItemMaster.ItemID" +
                         " WHERE (dbo.tblAssemblyDetails.BOMReference = '" + RefNo + "')";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int intItemClass;
                if (dt.Rows.Count > 0)
                {
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = "0";
                        ugR.Cells["ItemCode"].Value = Dr["ComponentID"];
                        ugR.Cells["Description"].Value = Dr["CompDesc"];
                        ugR.Cells["ItemClass"].Value = Dr["ItemClass"];
                        ugR.Cells["GL"].Value = Dr["SalesGLAccount"];
                        ugR.Cells["WH"].Value = Dr["WarehouseID"];
                        ugR.Cells["Quantity"].Value = Dr["QtyRequired"];
                        ugR.Cells["OnHand"].Value = Dr["QtyOnHand"];
                        ugR.Cells["UnitPrice"].Value = Dr["UnitCost"];
                        ugR.Cells["UnitCost"].Value = Dr["UnitCost"];
                        ugR.Cells["TotalPrice"].Value = "0.00";
                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private double CheckWarehouseItem(string StrItemCode, string StrWarehouseCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT QTY FROM tblItemWhse WHERE ItemId='" + StrItemCode + "' AND WhseId='" + StrWarehouseCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return double.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw;
                return 0;

            }
        }

        public void ViewHeader(string RefNo, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "SELECT BOMReference,AssemblyDate,AssemblyWID,AssemblyWHDesc,AssemblyID,AssemblyIDDesc,AssmblyQty,OnHandQty,NewQty,Action,IsProcess FROM tblAsseblyHeader WHERE BOMReference='" + RefNo + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtBuildRefNo.Text = dt.Rows[0]["BOMReference"].ToString().Trim();
                    cmbWarehouse.Text = dt.Rows[0]["AssemblyWID"].ToString().Trim();
                    lblWHName.Text = dt.Rows[0]["AssemblyWHDesc"].ToString().Trim();
                    cmbAssemID.Text = dt.Rows[0]["AssemblyID"].ToString().Trim();
                    lblItemDesc.Text = dt.Rows[0]["AssemblyIDDesc"].ToString().Trim();
                    if (bool.Parse(dt.Rows[0]["Action"].ToString()) == true)
                    {
                        rdoBuild.Checked = true;//Action
                    }
                    else
                    {
                        rdoBuild.Checked = false;//Action
                    }

                    if (int.Parse(dt.Rows[0]["IsProcess"].ToString()) == 1)
                    {
                        btnEdit.Enabled = false;
                    }
                    else
                    {
                        btnEdit.Enabled = true;
                    }
                    txtQtyBuild.Text = double.Parse(dt.Rows[0]["AssmblyQty"].ToString()).ToString("N2");
                    dtpDate.Value = DateTime.Parse(dt.Rows[0]["AssemblyDate"].ToString().Trim());
                    lblQtyBuild.Text = double.Parse(dt.Rows[0]["AssmblyQty"].ToString()).ToString("N2");
                    lblQtyOnHnad.Text = double.Parse(dt.Rows[0]["OnHandQty"].ToString()).ToString("N2");
                    lblNewQty.Text = double.Parse(dt.Rows[0]["NewQty"].ToString()).ToString("N2");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }






        public void GetCurrentUserDate()
        {

            try
            {
                String S = "Select CurrentDate from tblUserWiseDate where UserName='" + UserAutherization.user.userName.ToString() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                    dtpDate.Value = UserWiseDate;

                }
            }
            catch { }

        }



        private void btnclose_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //if (add)
            //{
            btnSave.Enabled = true;
            btnNew.Enabled = false;
            btnPrint.Enabled = false;
            btnSNO.Enabled = false;
            btnSearch.Enabled = false;
            btnReset.Enabled = true;
            btnEdit.Enabled = false;
            boolSerch = false;

            EnableHeader(true);
            EnableFoter(true);

            ClearHeader();
            DeleteRows();
            GetInvNo();
            GetWH();
            cmbAssemID.Focus();

            //UltraGridRow ugR;
            //ugR = ug.DisplayLayout.Bands[0].AddNew();
            //}
            //else
            //{
            //    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }




        private int GetEstimateCode(string strJobID, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT AutoIndex FROM tblJobMaster WHERE JobID='" + strJobID + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception)
            {

                throw;
            }


        }
        //peahtree update=========================

        public void GetGLGT()
        {
            try
            {
                StrSql = "SELECT AssemblyGL FROM tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    StrAssmblyGL = dt.Rows[0].ItemArray[0].ToString().Trim();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }



        private void CreateXmlToExortAssemblyItems(string AssembyID, string Reference, DateTime AsemblyDate, string GLAccount, double QtyBuild)
        {

            DateTime DTP = Convert.ToDateTime(AsemblyDate);
            string Dformat = "MM/dd/yyyy";
            string Assemblydate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\AssembyBuild.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");
            // int GridrowCount = GetFilledRows();//get row count from data grid

            Writer.WriteStartElement("PAW_Item");
            Writer.WriteAttributeString("xsi:type", "paw:item");

            Writer.WriteStartElement("ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(AssembyID.Trim());//dgvItem[0, c].Value
            Writer.WriteEndElement();

            //this sis crating tag for reference============(2)
            Writer.WriteStartElement("Reference");
            Writer.WriteString(Reference.Trim());
            Writer.WriteEndElement();

            //This is a Tag for Adjusment Date==============(3)
            Writer.WriteStartElement("Date ");
            Writer.WriteAttributeString("xsi:type", "paw:date");
            Writer.WriteString(Assemblydate);//Date format must be (MM/dd/yyyy)
            Writer.WriteEndElement();

            //This is a Tag for numberof dsistribution=======(4)

            Writer.WriteStartElement("Number_of_Distributions ");
            Writer.WriteString("1");
            Writer.WriteEndElement();

            //////Adjustmne Lines=================================(5)
            ////Writer.WriteStartElement("AdjustmentItems");
            //////Adjustmne Lines=================================(6)
            ////Writer.WriteStartElement("AdjustmentItem");


            //Gl ASccount======================================(7)
            Writer.WriteStartElement("GLSourceAccount ");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(GLAccount.Trim());
            Writer.WriteEndElement();


            //Unit Cost========================================(8)
            //Writer.WriteStartElement("UnitCost");
            //Writer.WriteString(dgvGRNTransaction[5, k].Value.ToString().Trim());
            //Writer.WriteEndElement();

            //Quantity========================================(9)

            Writer.WriteStartElement("QuantityBuilt");
            // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
            Writer.WriteString(QtyBuild.ToString().Trim());
            //Adjust_qty
            Writer.WriteEndElement();
            Writer.WriteEndElement();//Item Closed Tag


            Writer.Close();//finishing writing xml file
        }


        private void CreateXmlToExportInvAdjust(string StrItemCode, string strIssueNoteNo, DateTime dtDate, string StrJobId, double dblUnitcost, double dblQty, double dblLineTotal, string StrGL, SqlTransaction myTrans, SqlConnection myConnection)
        {

            Connector Conn = new Connector();
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\IssueAdjustment.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            Writer.WriteStartElement("PAW_Item");
            Writer.WriteAttributeString("xsi:type", "paw:item");

            //crate a ID element (tag)=====================(1)
            Writer.WriteStartElement("ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrItemCode);//dgvItem[0, c].Value
            Writer.WriteEndElement();

            //this sis crating tag for reference============(2)
            Writer.WriteStartElement("Reference");
            Writer.WriteString(strIssueNoteNo);
            Writer.WriteEndElement();

            //This is a Tag for Adjusment Date==============(3)
            Writer.WriteStartElement("Date ");
            Writer.WriteAttributeString("xsi:type", "paw:date");
            Writer.WriteString(GetDateTime(dtDate));//Date format must be (MM/dd/yyyy)
            Writer.WriteEndElement();

            Writer.WriteStartElement("JobID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrJobId);
            Writer.WriteEndElement();

            //This is a Tag for numberof dsistribution=======(4)

            Writer.WriteStartElement("Number_of_Distributions ");
            Writer.WriteString("1");
            Writer.WriteEndElement();

            Writer.WriteStartElement("GLSourceAccount ");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            //Writer.WriteString(StrGLGT);
            Writer.WriteString(StrAssmblyGL);
            Writer.WriteEndElement();

            //Adjustmne Lines=================================(5)
            Writer.WriteStartElement("AdjustmentItems");
            //Adjustmne Lines=================================(6)
            Writer.WriteStartElement("AdjustmentItem");


            //Gl ASccount======================================(7)

            //StrGL
            //Writer.WriteStartElement("GLSourceAccount ");
            //Writer.WriteAttributeString("xsi:type", "paw:id");
            ////Writer.WriteString(StrGLGT);
            //Writer.WriteString(StrGLGT);
            //Writer.WriteEndElement();

            //Writer.WriteStartElement("GLSourceAccount ");
            //Writer.WriteAttributeString("xsi:type", "paw:id");
            //Writer.WriteString(StrGLGT);
            //Writer.WriteEndElement();


            Writer.WriteStartElement("UnitCost");
            Writer.WriteString(dblUnitcost.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Quantity");
            Writer.WriteString(dblQty.ToString());
            Writer.WriteEndElement();


            Writer.WriteStartElement("Amount");
            Writer.WriteString("-" + dblLineTotal.ToString().Trim());
            Writer.WriteEndElement();


            Writer.WriteEndElement();//Adjustment Line
            Writer.WriteEndElement();//Adjustment lines

            Writer.WriteEndElement();//Item Closed Tag
            Writer.Close();//finishing writing xml file

            Conn.IssueAdjustmentExport("IssueAdjustment.xml");
        }

        private int CheckJobStatus(string strJobCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT Job_Status FROM tblJobMaster WHERE JOBID='" + strJobCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {

                throw;
                return 0;
            }
        }
        //============================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            int intGrid;
            int intAutoIndex;
            int intPreAutoIndex;
            int intProcess;
            double dblAvailableQty;
            string StrReference = null;
            bool boolAction;
            if (rdoBuild.Checked)
            {
                boolAction = true;
            }
            else
            {
                boolAction = false;
            }
            int intItemClass;
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {

                DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record?", "Information", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.Cancel)
                {
                    return;
                }
                reply = MessageBox.Show("Are You Sure, You Want To Process This?", "Information", MessageBoxButtons.YesNo);

                if (reply == DialogResult.Yes)
                {
                    intProcess = 1;
                }
                else
                {
                    intProcess = 0;
                }
                if (!objControlers.HeaderValidation_Warehouse(cmbWarehouse.Text, sMsg))
                {
                    return;
                }
                if (!objControlers.HeaderValidation_ItemID(cmbAssemID.Text, sMsg))
                {
                    return;
                }
                if (dtpDate.Value < user.Period_begin_Date)
                {
                    MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (dtpDate.Value > user.Period_End_Date)
                {
                    MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                DeleteEmpGrid();
                if (IsGridValidation() == false)
                {
                    MessageBox.Show("No Data To Save", "Information", MessageBoxButtons.YesNo);
                    return;
                }
                if (Convert.ToDouble(txtQtyBuild.Text) <= 0)
                {
                    MessageBox.Show("Please Enter Assembly Quantity", "Information", MessageBoxButtons.OK);
                    return;
                }
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                //intPreAutoIndex = GetEstimateCode(cmbAssemID.Value.ToString(), myConnection, myTrans);
                if (intEdit == 1)
                {
                    StrReference = txtBuildRefNo.Text.ToString().Trim();
                    DeleteHeader(intProcess, boolAction, myConnection, myTrans);
                    DeleteDetails(intProcess, boolAction, myConnection, myTrans);
                }
                else
                {
                    txtBuildRefNo.Text = StrReference = GetInvNoField(myConnection, myTrans);
                    UpdatePrefixNo(myConnection, myTrans);
                }
                //if (intEdit == 1)
                //{
                //    UpdateHeader(intProcess, boolAction, myConnection, myTrans);
                //    UpdateAssembyQty(cmbAssemID.Text.ToString().Trim(), cmbWarehouse.Text.ToString().Trim(), Convert.ToDouble(txtQtyBuild.Text), myConnection, myTrans);
                //    UpdateActivityInAssembly(StrReference, dtpDate.Value, cmbAssemID.Text.ToString().Trim(), cmbWarehouse.Text.ToString().Trim(), double.Parse(txtQtyBuild.Text), 0.00, 0.00, myConnection, myTrans);
                //}
                //else
                //{
                    SaveHeader(intProcess, boolAction, myConnection, myTrans);
                    if (intProcess == 1)
                    {
                        UpdateAssembyQty(cmbAssemID.Text.ToString().Trim(), cmbWarehouse.Text.ToString().Trim(), Convert.ToDouble(txtQtyBuild.Text), myConnection, myTrans);
                        UpdateActivityInAssembly(StrReference, dtpDate.Value, cmbAssemID.Text.ToString().Trim(), cmbWarehouse.Text.ToString().Trim(), double.Parse(txtQtyBuild.Text), 0.00, 0.00, myConnection, myTrans);
                    }
                //}
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    SaveDetails(StrReference, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["OnHand"].Value.ToString()), myConnection, myTrans);
                    if (intProcess == 1)
                    {
                        intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());
                        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), ug.Rows[intGrid].Cells["WH"].Value.ToString(), myConnection, myTrans);
                        if (dblAvailableQty < double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()))
                        {
                            MessageBox.Show("There is not enough of Inventory Item '" + ug.Rows[intGrid].Cells["ItemCode"].Value.ToString() + "' to bulid the number of '" + cmbAssemID.Text.ToString().Trim() + "'", "Information", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;

                        }
                        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);
                        InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitCost"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
                        UpdateStatus(StrReference, intProcess, myConnection, myTrans);
                    }
                }
                if (intProcess == 1)
                {
                    CreateXmlToExortAssemblyItems(cmbAssemID.Text.ToString().Trim(), txtBuildRefNo.Text.ToString().Trim(), dtpDate.Value, StrAssmblyGL, Convert.ToDouble(txtQtyBuild.Text));
                    Connector ObjImportP = new Connector();
                    ObjImportP.AssebmlyAdjustmentExport();

                    MessageBox.Show("BOM Building Process SuccessFully", "Information", MessageBoxButtons.OK);
                }
                myTrans.Commit();
                if (intProcess == 0)
                {
                    MessageBox.Show("BOM Building Saved SuccessFully", "Information", MessageBoxButtons.OK);
                }

                //Print(intAutoIndex);
                ButtonClear();

            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
                return;
                //  throw;

            }
        }

        private double CheckWarehouseItem(string StrItemCode, string StrItemDesc, double dblUnitCost, string StrWarehouseCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT QTY FROM tblItemWhse WHERE ItemId='" + StrItemCode + "' AND WhseId='" + StrWarehouseCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return double.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;

                }
                //else
                //{

                //    StrSql = "INSERT INTO tblItemWhse (WhseId,WhseName,ItemId,ItemDis,QTY,UOM,TraDate,UnitCost) VALUES ('" + StrWarehouseCode + "','','" + StrItemCode + "','" + StrItemDesc + "',0,'','" + GetDateTime(DateTime.Now) + "'," + dblUnitCost + ")";

                //    command = new SqlCommand(StrSql, con, Trans);
                //    command.CommandType = CommandType.Text;
                //    command.ExecuteNonQuery();

                //    return 0;



                //}
            }
            catch (Exception ex)
            {
                throw;
                return 0;


            }
        }

        public static string GetDateTime(DateTime DtGetDate)
        {
            DateTime DTP = Convert.ToDateTime(DtGetDate);
            string Dformat = "MM/dd/yyyy";
            return DTP.ToString(Dformat);

        }

        private void DeleteDetails(string BuildRef, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "DELETE FROM tblAssemblyDetails WHERE BOMReference='" + BuildRef + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void UpdateAssembyQty(string StrItemCode, string StrWarehouse, double dblQty, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblItemWhse SET QTY=QTY+" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }


        }

        private void UpdateItemWarehouse(string StrItemCode, string StrWarehouse, double dblQty, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblItemWhse SET QTY=QTY-" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }


        }

        private void UpdateStatus(string Reference, int IsProcess, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblAsseblyHeader SET IsProcess='" + IsProcess + "' WHERE BOMReference='" + Reference + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }


        }


        private void UpdateEstimateVsActualHeader(int intAutoIndex, double dblActualAmount, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE EST_HEADER SET RetHed_NetAmt=RetHed_NetAmt +  " + dblActualAmount + " WHERE AutoIndex=" + intAutoIndex + " ";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }



        }



        private void ModifyHeader(string BuildReferencer, bool Action, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "UPDATE [tblAsseblyHeader] SET [AssemblyDate] ='" + GetDateTime(dtpDate.Value) + "',[AssemblyWID] ='" + cmbWarehouse.Text.ToString().Trim() + "',[AssemblyWHDesc] = '" + lblWHName.Text.ToString().Trim() + "',[AssemblyID] = '" + cmbAssemID.Value.ToString() + "', " +
                         " [AssemblyIDDesc]=" + lblItemDesc.Text.ToString().Trim() + ",[AssmblyQty] ='" + Convert.ToDouble(lblQtyBuild.Text) + "',[OnHandQty]='" + Convert.ToDouble(lblQtyOnHnad.Text) + "',[NewQty] ='" + Convert.ToDouble(lblNewQty.Text) + "'," +
                         " Action=" + Action + " WHERE BOMReference='" + BuildReferenceNo + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }


        }
        private void SaveHeader(int intProcess, bool Action, SqlConnection con, SqlTransaction Trans)
        {

            try
            {
                StrSql = "INSERT INTO [tblAsseblyHeader]([BOMReference],[AssemblyDate],[AssemblyWID],[AssemblyWHDesc] " +
                        " ,[AssemblyID],[AssemblyIDDesc],[AssmblyQty],[OnHandQty],[NewQty],[Action],[IsProcess]) " +
                        " VALUES('" + txtBuildRefNo.Text.ToString().Trim() + "','" + GetDateTime(dtpDate.Value) + "','" + cmbWarehouse.Text.ToString().Trim() + "','" + lblWHName.Text.ToString().Trim() + "'," +
                         " '" + cmbAssemID.Text.ToString().Trim() + "','" + lblItemDesc.Text.ToString().Trim() + "','" + Convert.ToDouble(txtQtyBuild.Text) + "','" + Convert.ToDouble(lblQtyOnHnad.Text) + "','" + Convert.ToDouble(lblNewQty.Text) + "','" + Action + "','" + intProcess + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        private void DeleteDetails(int intProcess, bool Action, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "DELETE FROM [tblAssemblyDetails] WHERE BOMReference='" + txtBuildRefNo.Text.ToString().Trim() + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private void DeleteHeader(int intProcess, bool Action, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
               // StrSql = "UPDATE [tblAsseblyHeader] SET [AssemblyDate]='" + GetDateTime(dtpDate.Value) + "'," +
               //" [AssemblyWID]='" + cmbWarehouse.Text.ToString().Trim() + "'," +
               //" [AssemblyWHDesc]='" + lblWHName.Text.ToString().Trim() + "'," +
               //" [AssemblyID]='" + cmbAssemID.Text.ToString().Trim() + "'," +
               //" [AssemblyIDDesc]='" + lblItemDesc.Text.ToString().Trim() + "'," +
               //" [AssmblyQty]='" + Convert.ToDouble(txtQtyBuild.Text) + "'," +
               //" [OnHandQty]='" + Convert.ToDouble(lblQtyOnHnad.Text) + "'," +
               //" [NewQty]='" + Convert.ToDouble(lblNewQty.Text) + "'," +
               //" [Action]='" + Action + "'," +
               //" WHERE BOMReference='" + StrWarehouse + "'";

                StrSql = "DELETE FROM [tblAsseblyHeader] WHERE BOMReference='" + txtBuildRefNo.Text.ToString().Trim() + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private int GetLastTransactionNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT TOP 1 [AutoIndex] FROM EST_HEADER ORDER BY [AutoIndex] DESC";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 1;
                }

            }
            catch (Exception)
            {

                throw;
            }


        }

        private void InvTransaction(string Reference, DateTime dtDate, String StrItemCode, string StrLocCode, double dblQuantity, double dblPrice, double dblLineNetAmt, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(15,'" + Reference + "','" + GetDateTime(dtDate) + "','BOMBuild',0,'" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void UpdateActivityInAssembly(string Reference, DateTime dtDate, String StrItemCode, string StrLocCode, double dblQuantity, double dblPrice, double dblLineNetAmt, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(15,'" + Reference + "','" + GetDateTime(dtDate) + "','BOMBuild',1,'" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private Boolean IsFItem(String StrItemCode)
        {
            try
            {
                if (StrItemCode == StrFItem)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        //private void UpdateAssembyItemItem(string Reference,string StrItemCode, double dblQuantity, SqlConnection con, SqlTransaction Trans)
        //{
        //    try
        //    {
        //        StrSql = "UPDATE EST_HEADER SET EstHed_Productivirt_Qty=EstHed_Productivirt_Qty+" + dblQuantity + " WHERE AutoIndex=" + intPreAutoIndex + "";
        //        SqlCommand command = new SqlCommand(StrSql, con, Trans);
        //        command.CommandType = CommandType.Text;
        //        command.ExecuteNonQuery();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private void SaveDetails(string reference, string componentID, string CompDesc, string WID, double QtyRequired, double QtyOnHand, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO [tblAssemblyDetails]([BOMReference],[ComponentID],[CompDesc],[QtyRequired],[QtyOnHand],[WarehouseID]) " +
                   " VALUES('" + reference + "','" + componentID + "','" + CompDesc + "','" + QtyRequired + "','" + QtyOnHand + "','" + WID + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                blnEdit = true;
                btnEdit.Enabled = true;
                btnPrint.Enabled = true;
                boolSerch = true;

                frmBOMList ObjfrmBOMList = new frmBOMList();
                ObjfrmBOMList.ShowDialog();

                setValue();

            }
            catch { }
        }


        private void DataSetHeader(int intEstimateNo)
        {


            StrSql = "SELECT * FROM EST_HEADER WHERE AutoIndex=" + intEstimateNo + "";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtEstimateHeader);

        }


        private void DataSetDetails(int intEstimateNo)
        {
            StrSql = "SELECT * FROM EST_DETAILS WHERE AutoIndex=" + intEstimateNo + "";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtEstimateDETAILS);

        }


        private void Print(int intAutoIndex)
        {
            try
            {

                try
                {
                    if (intAutoIndex == 0)
                    {
                        return;
                    }

                    DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

                    if (reply == DialogResult.Cancel)
                    {
                        return;
                    }

                    if (intAutoIndex > 0)
                    {
                        DsEst.Clear();
                        DataSetHeader(intAutoIndex);
                        DataSetDetails(intAutoIndex);
                        DatasetWhse();

                        //frmViewerFinishgoodsTransfer frmviewer = new frmViewerFinishgoodsTransfer(this);
                        //frmviewer.Show();

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error :" + ex.Message);
                }

            }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print(intEstomateProcode);
        }



        private void ClearHeader()
        {
            try
            {
                dtpDate.Value = user.LoginDate;
                cmbAssemID.Text = "";
                txtQtyBuild.Text = "0.00";
                cmbWarehouse.Text = "";
                txtBuildRefNo.Text = "";

                lblItemDesc.Text = "Item Desc";
                lblWHName.Text = "WH Desc";

                lblNewQty.Text = "0.00";
                lblQtyOnHnad.Text = "0.00";
                lblQtyBuild.Text = "0.00";

                rdoBuild.Checked = true;
                DeleteRows();

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }


        }


        private void EnableFoter(Boolean blnEnable)
        {
            //txtVatPer.Enabled = blnEnable;
            //txtNBTPer.Enabled = blnEnable;
            //txtDescription.Enabled = blnEnable;
            //txtDiscPer.Enabled = blnEnable;
            ug.Enabled = blnEnable;

        }

        private void EnableHeader(Boolean blnEnable)
        {

            dtpDate.Enabled = blnEnable;
            cmbAssemID.Enabled = blnEnable;
            cmbWarehouse.Enabled = blnEnable;
            txtQtyBuild.Enabled = blnEnable;
        }

        private void ButtonClear()
        {
            btnSave.Enabled = false;
            btnNew.Enabled = true;
            btnPrint.Enabled = true;
            //btnSNO.Enabled = false;
            btnSearch.Enabled = true;
            btnReset.Enabled = true;
            btnEdit.Enabled = false;

            ClearHeader();
            EnableHeader(false);
            EnableFoter(false);
            DeleteRows();
            // GetInvNo();
            ug.Enabled = false;

        }

        private void btnReset_Click(object sender, EventArgs e)
        {

            ButtonClear();

        }

        private void DatasetWhse()
        {
            StrSql = "SELECT * FROM tblWhseMaster";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtWhseMaster);

        }



        private void ug_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
        }

        private void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "UnitPrice" || e.Cell.Column.Key == "Quantity")
            {
                e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                GrandTotal();
            }
        }

        private void txtDiscPer_Leave(object sender, EventArgs e)
        {
            //if (double.Parse(txtDiscPer.Value.ToString()) > 100)
            //{
            //    MessageBox.Show("Invalid Discount Percentage", "Information", MessageBoxButtons.OK);
            //    txtDiscPer.Focus();

            //}
        }

        private void txtDiscPer_ValueChanged(object sender, EventArgs e)
        {
            GrandTotal();
        }

        private void txtNBTPer_ValueChanged(object sender, EventArgs e)
        {
            GrandTotal();
        }

        private void txtVatPer_ValueChanged(object sender, EventArgs e)
        {
            GrandTotal();
        }





        private int JobProcessed()
        {
            StrSql = "Select EstHed_Process from EST_HEADER where AutoIndex=" + intEstomateProcode + "";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {

                return (Boolean.Parse(dt.Rows[0].ItemArray[0].ToString()) == true ? 1 : 0);
            }
            else
            {
                return 0;

            }

        }

        private int ProjectStatus()
        {
            StrSql = "Select Job_Status from EST_HEADER where AutoIndex=" + intEstomateProcode + "";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0].ItemArray[0].ToString());
            }
            else
            {
                return 0;

            }
        }

        private void Edit()
        {
            EnableHeader(true);
            EnableFoter(true);
            dtpDate.Enabled = true;
            btnReset.Enabled = true;
            btnSave.Enabled = true;
            ug.Enabled = true;
        }

        public Boolean IsGridExitCode(String StrCode)
        {
            foreach (UltraGridRow ugR in ultraCombo1.Rows)
            {
                if (ugR.Cells["ItemID"].Text == StrCode)
                {
                    return true;
                }
            }
            return false;

        }



        private void btnEdit_Click(object sender, EventArgs e)
        {

            if (JobProcessed() == 0)
            {
                intEdit = 1;
                GetItemDataSet();
                Edit();


            }
            else
            {
                MessageBox.Show("This Transaction Already Processed. You can not modify this record.", "Information", MessageBoxButtons.OK);
            }
        }

        private void cmbWarehouse_Leave(object sender, EventArgs e)
        {


            if (ug.Rows.Count > 0)
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to channge Warehouse?", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }
                {
                    DeleteRows();
                }
            }

            GetItemDataSet();

        }



        private double GetPrice(string StrJobCode)
        {
            try
            {
                StrSql = "SELECT isnull([EstHed_Standed_Cost_Per_Unit],0) FROM EST_HEADER WHERE DocType=1 AND JobCode='" + StrJobCode + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    if (double.Parse(dt.Rows[0].ItemArray[0].ToString()) > 0)
                    {
                        return (double.Parse(dt.Rows[0].ItemArray[0].ToString()));
                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return 0;

                }

            }
            catch (Exception)
            {
                return 0;
                throw;
            }

        }

        //private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        //{
        //    if (ug.ActiveCell.Column.Key == "ItemCode")
        //    {
        //        if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            ug.ActiveCell.Value = ug.ActiveCell.Text;
        //            if (IsGridExitCode(ug.ActiveCell.Row.Cells[1].Text) == false)
        //            {
        //                e.Cancel = true;
        //            }
        //            foreach (UltraGridRow ugR in ultraCombo1.Rows)
        //            {
        //                if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemId"].Value.ToString())
        //                {
        //                    ug.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDescription"].Value;
        //                    ug.ActiveCell.Row.Cells["UnitPrice"].Value = GetPrice(cmbJobCode.Value.ToString());
        //                    ug.ActiveCell.Row.Cells["UnitCost"].Value = ugR.Cells["UnitCost"].Value;
        //                    ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
        //                    ug.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
        //                    ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
        //                    ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
        //                }
        //            }
        //        }

        //    }

        //    //else if (ug.ActiveCell.Column.Key == "Quantity")
        //    //{
        //    //    if (double.Parse(ug.ActiveCell.Row.Cells["OnHand"].Value.ToString()) < double.Parse(ug.ActiveCell.Text))
        //    //    {
        //    //        MessageBox.Show("Insufficient quantity available!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //        e.Cancel = true;

        //    //    }
        //    //}

        //}



        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            try
            {
                int intItemClass = 0;
                if (ug.ActiveCell.Column.Key == "ItemCode")
                {
                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        return;
                    }
                    else
                    {
                        ug.ActiveCell.Value = ug.ActiveCell.Text;
                        if (IsGridExitCode(ug.ActiveCell.Row.Cells[1].Text) == false)
                        {
                            e.Cancel = true;
                        }
                        foreach (UltraGridRow ugR in ultraCombo1.Rows)
                        {
                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemId"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDescription"].Value;
                                //ug.ActiveCell.Row.Cells["UnitCost"].Value = GetPrice(cmbJobCode.Value.ToString());
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = GetPrice(cmbAssemID.Value.ToString());
                                ug.ActiveCell.Row.Cells["UnitCost"].Value = ugR.Cells["UnitCost"].Value;
                                //ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
                                ug.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 1;

                            }
                        }
                    }

                }

                else if (ug.ActiveCell.Column.Key == "WH")
                {
                    intItemClass = int.Parse(ug.ActiveCell.Row.Cells["ItemClass"].Value.ToString());

                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {

                        //if (ug.ActiveCell.Text == "")
                        //{
                        //    e.Cancel = true;
                        //    return;
                        //}
                        //if (ug.ActiveCell.Value.ToString() != "")
                        //{
                        //    ug.ActiveCell.Value = "";
                        //    e.Cancel = true;
                        //    return;
                        //}

                        //if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                        //{
                        //    return;

                        //}

                        ug.ActiveCell.Value = ug.ActiveCell.Text;

                        if (IsGridExitWHCode(ug.ActiveCell.Value.ToString()) == false)
                        {
                            e.Cancel = true;
                        }
                        if (IsGridExitWHItem(ug.ActiveCell.Value.ToString(), ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString()) == false)
                        {
                            DialogResult reply = MessageBox.Show("Invalid Warehouse.Do You Want To Delete This Record?", "Information", MessageBoxButtons.YesNo);

                            if (reply == DialogResult.Yes)
                            {
                                ug.PerformAction(UltraGridAction.CommitRow);
                                ug.ActiveRow.Delete(false);

                            }
                            else
                            {

                                reply = MessageBox.Show("Do You Want To Allocate for Warehouse?", "Information", MessageBoxButtons.YesNo);
                                if (reply == DialogResult.Yes)
                                {

                                    if (IsAddWarehouseItem(ug.ActiveCell.Row.Cells["WH"].Value.ToString(), ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString()) == true)
                                    {
                                        MessageBox.Show("Succesfully Allocated For the Warehouse.", "Information", MessageBoxButtons.OK);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Unable to Allocate for the Warehouse.", "Information", MessageBoxButtons.OK);
                                    }
                                }
                                else
                                {
                                    e.Cancel = true;
                                    return;

                                }
                            }
                        }
                        else
                        {
                            ug.ActiveCell.Value = ug.ActiveCell.Text;

                            foreach (UltraGridRow ugR in cmbWH.Rows)
                            {

                                if (ug.ActiveCell.Value.ToString() == ugR.Cells["WhseId"].Value.ToString())
                                {

                                    ug.ActiveCell.Row.Cells["OnHand"].Value = DblGridExitWHItem(ug.ActiveCell.Row.Cells["WH"].Value.ToString(), ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString());

                                }
                            }
                        }


                    }


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }

        }

        private double DblGridExitWHItem(string StrWH, string StrItem)
        {
            StrSql = "Select QTY from tblItemWhse where ItemId='" + StrItem + "' and WhseId='" + StrWH + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return double.Parse(dt.Rows[0].ItemArray[0].ToString().Trim());
            }
            else
            {
                return 0;
            }
        }


        private Boolean IsAddWarehouseItem(string StrWH, string StrItem)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            myConnection.Open();
            myTrans = myConnection.BeginTransaction();

            try
            {

                StrSql = "SELECT ItemID,ItemDescription,UOM,UnitPrice FROM tblItemMaster WHERE ItemID='" + StrItem + "'";


                SqlCommand cmd = new SqlCommand(StrSql, myConnection, myTrans);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {

                    StrSql = "INSERT INTO tblItemWhse (WhseId,ItemId,ItemDis,QTY,UOM,TraDate,UnitCost,TranType,TotalCost,OPBQtry) values ('" + StrWH + "','" + StrItem + "','" + dt.Rows[0].ItemArray[1].ToString().Trim() + "',0,'" + dt.Rows[0].ItemArray[2].ToString().Trim() + "','" + GetDateTime(dtpDate.Value) + "'," + double.Parse(dt.Rows[0].ItemArray[3].ToString().Trim()) + ",'OpbBal',0,0)";

                    cmd = new SqlCommand(StrSql, myConnection, myTrans);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    myTrans.Commit();
                    return true;
                }
                else
                {
                    return false;

                }

                myTrans.Commit();

            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                throw;
            }


        }

        private Boolean IsGridExitWHItem(string StrWH, string StrItem)
        {
            StrSql = "Select ItemId from tblItemWhse where ItemId='" + StrItem + "' and WhseId='" + StrWH + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;

            }
        }
        public Boolean IsGridExitWHCode(String StrCode)
        {


            foreach (UltraGridRow ugR in cmbWH.Rows)
            {
                if (ugR.Cells["WhseId"].Text == StrCode)
                {

                    return true;
                }
            }
            return false;

        }



        public void GetWhDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " Select WhseId,WhseName from tblWhseMaster order by WhseId";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "dtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["dtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["dtWarehouse"].Columns["WhseId"].Width = 75;
                cmbWarehouse.DisplayLayout.Bands["dtWarehouse"].Columns["WhseName"].Width = 150;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void GetHeaderItemDataSet()
        {
            dsHeaderItem = new DataSet();
            try
            {
                dsHeaderItem.Clear();
                StrSql = "SELECT dbo.tblItemMaster.ItemID, dbo.tblItemMaster.ItemDescription, dbo.tblItemWhse.QTY, dbo.tblItemMaster.UnitCost, dbo.tblItemMaster.SalesGLAccount," +
                         "dbo.tblItemWhse.WhseId FROM dbo.tblItemMaster INNER JOIN dbo.tblItemWhse ON dbo.tblItemMaster.ItemID = dbo.tblItemWhse.ItemId" +
                         " WHERE (dbo.tblItemWhse.WhseId = '" + cmbWarehouse.Text.ToString().Trim() + "')";

                // StrSql = " Select ItemId,ItemDis,QTY,UnitCost,SalesGLAccount from tblItemWhse where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsHeaderItem, "dtHeaderItem");
                cmbAssemID.DataSource = dsHeaderItem.Tables["dtHeaderItem"];
                cmbAssemID.DisplayMember = "ItemID";
                cmbAssemID.ValueMember = "ItemID";
                cmbAssemID.DisplayLayout.Bands["dtHeaderItem"].Columns["ItemID"].Width = 150;
                cmbAssemID.DisplayLayout.Bands["dtHeaderItem"].Columns["ItemDescription"].Width = 250;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        public string GetFItem(int intPreAutoIndex, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "Select isnull(ProductionItem_ID,'') as ProductionItem_ID from EST_HEADER where AutoIndex=" + intPreAutoIndex + "";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return (dt.Rows[0].ItemArray[0].ToString().Trim());

                }
                else
                {
                    return String.Empty;
                }
            }
            catch (Exception)
            {
                return String.Empty;
                throw;
            }

        }


        public void LoadtheQtyRequired()
        {
            try
            {
                if (txtQtyBuild.Text.ToString().Trim() == string.Empty)
                {
                    txtQtyBuild.Text = "0.00";
                }
                if (Convert.ToDouble(txtQtyBuild.Text) < 0)
                    return;
                DeleteRows();
                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                StrSql = " SELECT     tblAssemblyMaster.CompID, tblAssemblyMaster.CompDesc, tblAssemblyMaster.CompQtyNeeded, " +
                    " MAX(tblAssemblyMaster.RevisionNumber) AS RevisionNumber, " +
                    " tblItemMaster.UnitPrice, tblItemMaster.SalesGLAccount, " +
                    " tblItemMaster.ItemClass, tblItemMaster.UnitCost, tblAssemblyMaster.AssemblyID " +
                    " FROM         tblItemMaster RIGHT OUTER JOIN " +
                    " tblAssemblyMaster ON tblItemMaster.ItemDescription = tblAssemblyMaster.CompID " +
                    " GROUP BY tblAssemblyMaster.CompID, " +
                    " tblAssemblyMaster.CompDesc, tblAssemblyMaster.CompQtyNeeded, " +
                    " tblItemMaster.UnitPrice, tblItemMaster.SalesGLAccount, " +
                    " tblItemMaster.ItemClass, tblItemMaster.UnitCost, tblAssemblyMaster.AssemblyID " +
                    " HAVING (tblAssemblyMaster.AssemblyID = '" + cmbAssemID.Text.ToString().Trim() + "') ";

                SqlCommand command = new SqlCommand(StrSql, myConnection, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = "0";
                        ugR.Cells["ItemCode"].Value = Dr["CompID"];
                        ugR.Cells["Description"].Value = Dr["CompDesc"];
                        ugR.Cells["ItemClass"].Value = Dr["ItemClass"];
                        ugR.Cells["GL"].Value = Dr["SalesGLAccount"];
                        ugR.Cells["WH"].Value = StrDefaultWH;
                        ugR.Cells["Quantity"].Value = Convert.ToDouble(Dr["CompQtyNeeded"]) * Convert.ToDouble(txtQtyBuild.Text.ToString().Trim());
                        ugR.Cells["OnHand"].Value = "0.00";
                        ugR.Cells["UnitPrice"].Value = Dr["UnitCost"];
                        ugR.Cells["UnitCost"].Value = Dr["UnitCost"];
                        ugR.Cells["TotalPrice"].Value = "0.00";

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                throw;

            }
        }
        public void LoadBOMMasterDeatila()
        {
            try
            {
                //if (txtQtyBuild.Text.ToString().Trim() == string.Empty)
                //{
                //    txtQtyBuild.Text = "0.00";
                //}
                DeleteRows();
                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                StrSql = " SELECT     tblAssemblyMaster.CompID, tblAssemblyMaster.CompDesc, tblAssemblyMaster.CompQtyNeeded, " +
                    " MAX(tblAssemblyMaster.RevisionNumber) AS RevisionNumber, " +
                    " tblItemMaster.UnitPrice, tblItemMaster.SalesGLAccount, " +
                    " tblItemMaster.ItemClass, tblItemMaster.UnitCost, tblAssemblyMaster.AssemblyID " +
                    " FROM         tblItemMaster RIGHT OUTER JOIN " +
                    " tblAssemblyMaster ON tblItemMaster.ItemDescription = tblAssemblyMaster.CompID " +
                    " GROUP BY tblAssemblyMaster.CompID, " +
                    " tblAssemblyMaster.CompDesc, tblAssemblyMaster.CompQtyNeeded, " +
                    " tblItemMaster.UnitPrice, tblItemMaster.SalesGLAccount, " +
                    " tblItemMaster.ItemClass, tblItemMaster.UnitCost, tblAssemblyMaster.AssemblyID " +
                    " HAVING (tblAssemblyMaster.AssemblyID = '" + cmbAssemID.Text.ToString().Trim() + "') ";

                // StrSql = "SELECT CompID,CompDesc,CompQtyNeeded,RevisionNumber from tblAssemblyMaster WHERE AssemblyID ='" + cmbAssemID.Text.ToString().Trim() + "' and RevisionNumber=(Select MAX(RevisionNumber) from tblAssemblyMaster where AssemblyID='" + cmbAssemID.Text.ToString().Trim() + "')";
                SqlCommand command = new SqlCommand(StrSql, myConnection, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = "0";
                        ugR.Cells["ItemCode"].Value = Dr["CompID"];
                        ugR.Cells["Description"].Value = Dr["CompDesc"];
                        ugR.Cells["ItemClass"].Value = Dr["ItemClass"];
                        ugR.Cells["GL"].Value = Dr["SalesGLAccount"];
                        ugR.Cells["WH"].Value = StrDefaultWH;
                        ugR.Cells["Quantity"].Value = Dr["CompQtyNeeded"];
                        //ugR.Cells["Quantity"].Value = Convert.ToDouble(Dr["CompQtyNeeded"]) * Convert.ToDouble(txtQtyBuild.Text.ToString().Trim());
                        ugR.Cells["OnHand"].Value = "0.00";
                        ugR.Cells["UnitPrice"].Value = Dr["UnitCost"];
                        ugR.Cells["UnitCost"].Value = Dr["UnitCost"];
                        ugR.Cells["TotalPrice"].Value = "0.00";

                    }


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                throw;

            }
        }


        public void GetDefaultWH()
        {
            try
            {
                StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster where IsDefault=1";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrDefaultWH = dt.Rows[0].ItemArray[0].ToString().Trim();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void loadformState()
        {
            btnSave.Enabled = false;
            btnPrint.Enabled = false;
            btnSearch.Enabled = true;
            btnReset.Enabled = true;
            btnNew.Enabled = true;
            btnEdit.Enabled = false;
            dtpDate.Enabled = false;
        }


        private void frmBOMBuilding_Load(object sender, EventArgs e)
        {
            try
            {
                loadformState();
                GetWhDataSet();
                GetInvNo();
                GetDefaultWH();
                GetCurrentUserDate();
                GetItemDataSet();
                ClearHeader();
                DeleteRows();
                EnableHeader(false);
                EnableFoter(false);
                GetJobStatus();

                GetGLGT();

                GetWH();

            }
            catch { }
        }





        private void cmbjob_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeleteRows();
        }

        private void cmbjob_Leave(object sender, EventArgs e)
        {

        }

        private void cmbjob_SelectedValueChanged(object sender, EventArgs e)
        {
            DeleteRows();
        }

        private void cmbWH_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void cmbJobCode_RowSelected(object sender, RowSelectedEventArgs e)
        {

            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        //  txtDescription.Text = cmbAssemID.ActiveRow.Cells[1].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void rdoFG_CheckedChanged(object sender, EventArgs e)
        {
            //  GetInvNo();
        }

        private void rdoRM_CheckedChanged(object sender, EventArgs e)
        {
            // GetInvNo();
        }

        private void frmBOMBuilding_Load()
        {

        }

        private void cmbWarehouse_Leave_1(object sender, EventArgs e)
        {
            GetHeaderItemDataSet();
        }

        private void cmbWarehouse_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                lblWHName.Text = "";
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        lblWHName.Text = cmbWarehouse.ActiveRow.Cells[1].Value.ToString();
                    }
                }
                if (boolSerch == false)
                    GetHeaderItemDataSet();
            }
            catch { }

        }

        private void CalculateNewQty()
        {
            try
            {
                if (Convert.ToDouble(txtQtyBuild.Text) < 0)
                    return;
                double dblOnhandQty = 0.00;
                double dblBulidQty = 0.00;
                double dblNewQty = 0.00;

                dblOnhandQty = Convert.ToDouble(lblQtyOnHnad.Text);
                dblBulidQty = Convert.ToDouble(txtQtyBuild.Text);

                if (rdoBuild.Checked)
                    dblNewQty = dblOnhandQty + dblBulidQty;
                if (rdoUnbuild.Checked)
                    dblNewQty = dblOnhandQty - dblBulidQty;

                lblQtyBuild.Text = dblBulidQty.ToString("N2");
                lblNewQty.Text = dblNewQty.ToString("N2");
            }
            catch { }
        }

        private void cmbAssemID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                lblItemDesc.Text = "";
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        lblItemDesc.Text = cmbAssemID.ActiveRow.Cells[1].Value.ToString();
                        lblQtyOnHnad.Text = Convert.ToDouble(cmbAssemID.ActiveRow.Cells[2].Value).ToString("N2");
                        StrAssmblyGL = cmbAssemID.ActiveRow.Cells[4].Value.ToString();
                        CalculateNewQty();
                        if (boolSerch == false)
                            LoadBOMMasterDeatila();
                    }
                }
            }
            catch { }
        }

        private void cmbWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbWarehouse, cmbAssemID, e);
        }

        private void cmbAssemID_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbAssemID, lblItemDesc, e);
        }

        private void cmbWarehouse_Leave_2(object sender, EventArgs e)
        {

        }

        private void validateINput()
        {
            try
            {
                Convert.ToDouble(txtQtyBuild.Text);
                if (Convert.ToDouble(txtQtyBuild.Text) < 0)
                {
                    MessageBox.Show("Please Enter Positive Number");
                    txtQtyBuild.Focus();
                    return;
                }
                txtQtyBuild.Text = Convert.ToDouble(txtQtyBuild.Text).ToString("N2");
            }
            catch
            {
                MessageBox.Show("Please enter Valid Number");
                txtQtyBuild.Focus();
                return;
            }
        }
        private void txtQtyBuild_Leave(object sender, EventArgs e)
        {
            validateINput();
            LoadtheQtyRequired();
            CalculateNewQty();
        }

        private void txtVat_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtDiscAmount_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void txtNBT_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtGrossValue_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void ug_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }



































    }
}