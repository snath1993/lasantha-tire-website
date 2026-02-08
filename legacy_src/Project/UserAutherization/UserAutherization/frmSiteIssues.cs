using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PCMBLL;
using PCMBeans;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Diagnostics;
using ComFunction;
using System.Data.SqlClient;

namespace UserAutherization
{
    public partial class frmSiteIssues : Form
    {
        public static string ConnectionString;
        public dsRptIssueNote objdsRptIssueNote = new dsRptIssueNote();
        clsCommon objclsCommon = new clsCommon();
        clsBLLPhases objclsBLLPhases;
        clsBeansPhases objclsBeansPhases;
        enmFormMode FormMode;
        private string _msgTitle = "Site Issues";
        DataSet objDataSet;
        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        DataTable _DocumentList;
        clsCommonFunc objclsCommonFunc = new clsCommonFunc();

        //user can change the warehouse dif than the bom, but total should be same


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

        public frmSiteIssues()
        {
            InitializeComponent();
            ucmbIssueID.Focus();
            FormMode = enmFormMode.Initialize;
            SetFormMode();

            setConnectionString();
        }

        public frmSiteIssues(string IssueNo)
        {
            InitializeComponent();
            ucmbIssueID.Value = IssueNo;
            fillControls();
            FormMode = enmFormMode.Find;
            SetFormMode();

            setConnectionString();
        }

        public void Discard()
        {
            foreach (UltraGridRow ugR in udgvSubcontractors.Rows.All)
            {
                ugR.Delete(false);
            }
        }

        private void ClearControlers()
        {
            ucmbBOMID.Text = "";
            ucmbIssueID.Text = "";
            ucmbPhaseID.Text = "";
            txtDescription.Text = "";
            ucmbSiteID.Text = "";
            txtQtyBOMTemp.Value = 0.00;
            txtQtyOHTemp.Value = 0.00;
            txtQtyIssueTemp.Value = 0.00;
            ucmbCustomerID.Text = "";
            txtCustomerName.Text = "";

            dtpDate.Value = user.LoginDate;          

            Discard();
            objclsCommonFunc.fillBOMs_Active(ucmbBOMID);
            objclsCommonFunc.fillCustomer(ucmbCustomerID);
            objclsCommonFunc.fillIssues(ucmbIssueID);
            objclsCommonFunc.fillItems(ucmbItems);
            objclsCommonFunc.fillItems(ucmbItems1);
            objclsCommonFunc.fillPhases(ucmbPhaseID);
            objclsCommonFunc.fillPhases(ucmbPhaseID1);
            objclsCommonFunc.fillSites(ucmbSiteID);
            objclsCommonFunc.fillSubPhases(ucmbSubPhaseID);
            objclsCommonFunc.fillSubPhases(ucmbSubPhaseID1);

            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();

            _DocumentList = null;          
        }

        private void SetFormMode()
        {
            switch (FormMode)
            {
                case enmFormMode.Initialize:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
                case enmFormMode.Save:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
                case enmFormMode.Delete:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
                case enmFormMode.Find:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = true;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    break;
                case enmFormMode.Clear:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
            }
        }

        private bool IsValidQtyReferingBOM()
        {
            try
            {
                DataSet _dts = new DataSet();
                _dts = objclsBLLPhases.GetToBeIssuedQty(ucmbBOMID.Value.ToString().Trim());

                foreach (DataRow dr in _dts.Tables[0].Rows)
                {
                    double _IssQty = 0;
                    double _BOMQTY=double.Parse(dr["BOMQty"].ToString())-double.Parse(dr["IssuedQty"].ToString())+double.Parse(dr["ReturnedQty"].ToString());

                    foreach (UltraGridRow udr in udgvSubcontractors.Rows)
                    {
                        if (dr["PhaseID"].ToString() == udr.Cells["colPhase"].Value.ToString() && 
                            dr["SubPhaseID"].ToString() == udr.Cells["colSubPhase"].Value.ToString() && 
                            dr["ItemID"].ToString() == udr.Cells["colItem"].Value.ToString())
                        {
                            _IssQty = _IssQty + double.Parse(udr.Cells["colIssueQty"].Value.ToString().Trim());
                        }
                        if (_BOMQTY < _IssQty)
                        {
                            MessageBox.Show("You are going to Issue more than BOM Qty for " + dr["PhaseID"].ToString() + ", " + dr["SubPhaseID"].ToString() + ", " + dr["ItemID"].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        //
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsValidControls()
        {
            if (!user.IsJOBIssueNOAutoGen)
            {
                if (string.IsNullOrEmpty(ucmbIssueID.Text.Trim()))
                {
                    erpMaster.SetError(ucmbIssueID, "Issue No Can't be Empty");
                    ucmbIssueID.Focus();
                    return false;
                }
            }
            if (!user.IsMoreThanBOMQty)
            {
                if (!IsValidQtyReferingBOM())
                    return false;
            }
            if (string.IsNullOrEmpty(ucmbSiteID.Text.Trim()))
            {
                erpMaster.SetError(ucmbSiteID, "Site ID Can't be Empty");
                ucmbSiteID.Focus();
                return false;
            }
            else
                return true;
        }

        private void fillIssueNos()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbIssueID.DataSource = null;
                ucmbIssueID.DataSource = objclsBLLPhases.GetIssues_All_Dropdown();
                ucmbIssueID.DisplayMember = "IssueNo";
                ucmbIssueID.ValueMember = "IssueNo";
                ucmbIssueID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbIssueID.DisplayLayout.Bands[0].Columns[1].Width = 40;
                ucmbIssueID.DisplayLayout.Bands[0].Columns[2].Width = 100;
                ucmbIssueID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                ucmbIssueID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "SiteID";
                ucmbIssueID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Description";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillBOMs()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbBOMID.DataSource = null;
                ucmbBOMID.DataSource = objclsBLLPhases.GetBOMs_AllActive_Dropdown();
                ucmbBOMID.DisplayMember = "BOMID";
                ucmbBOMID.ValueMember = "BOMID";
                ucmbBOMID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbBOMID.DisplayLayout.Bands[0].Columns[1].Width = 40;
                ucmbBOMID.DisplayLayout.Bands[0].Columns[2].Width = 100;
                ucmbBOMID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                ucmbBOMID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "SiteID";
                ucmbBOMID.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Description";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillPhases()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbPhaseID.DataSource = null;
                ucmbPhaseID.DataSource = objclsBLLPhases.GetPhases_AllActive_ForDropDown();
                ucmbPhaseID.DisplayMember = "PhaseID";
                ucmbPhaseID.ValueMember = "PhaseID";
                ucmbPhaseID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbPhaseID.DisplayLayout.Bands[0].Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillSubPhases()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbSubPhaseID.DataSource = null;
                ucmbSubPhaseID.DataSource = objclsBLLPhases.GetSubPhases_AllActive_ForDropDown();
                ucmbSubPhaseID.DisplayMember = "SubPhaseID";
                ucmbSubPhaseID.ValueMember = "SubPhaseID";
                ucmbSubPhaseID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbSubPhaseID.DisplayLayout.Bands[0].Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillSiteIDs()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbSiteID.DataSource = null;
                ucmbSiteID.DataSource = objclsBLLPhases.GetJob_AllActive_ForDropDown();
                ucmbSiteID.DisplayMember = "SiteID";
                ucmbSiteID.ValueMember = "SiteID";
                ucmbSiteID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbSiteID.DisplayLayout.Bands[0].Columns[1].Width = 100;
                ucmbSiteID.DisplayLayout.Bands[0].Columns[2].Width = 80;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillCustomers()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbCustomerID.DataSource = null;
                ucmbCustomerID.DataSource = objclsBLLPhases.GetCustomer_AllActive_ForDropDown();
                ucmbCustomerID.DisplayMember = "ID";
                ucmbCustomerID.ValueMember = "Name";
                ucmbCustomerID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbCustomerID.DisplayLayout.Bands[0].Columns[1].Width = 100;
                //ucmbCustomerID.DisplayLayout.Bands[0].Columns[2].Width = 80;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillItems()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbItems.DataSource = null;
                ucmbItems.DataSource = objclsBLLPhases.GetItems_All();
                ucmbItems.DisplayMember = "ItemId";
                ucmbItems.ValueMember = "ItemId";
                ucmbItems.DisplayLayout.Bands[0].Columns[0].Width = 100;
                ucmbItems.DisplayLayout.Bands[0].Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmBOQ_Load(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbPhaseID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                //ucmbSubPhaseID.DataSource = null;

                if (ucmbPhaseID.Value != null)
                {
                    ucmbSubPhaseID.DataSource = objclsBLLPhases.GetSubPhases_ByPhaseID(ucmbPhaseID.Value.ToString());
                    ucmbSubPhaseID.DisplayMember = "SubPhaseID";
                    ucmbSubPhaseID.ValueMember = "SubPhaseID";
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Clear;
            SetFormMode();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Save;
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                //if(

                if (IsValidControls())
                {
                    if (ucmbIssueID.Text.Trim().Length > 0)
                    {
                        MessageBox.Show("Not Allowed to Save again....!");
                        return;
                    }

                    objclsBeansPhases.CustomerID = ucmbCustomerID.Text.Trim();

                    objclsBeansPhases.IssueNo = ucmbIssueID.Text.Trim();
                    objclsBeansPhases.BOMID = ucmbBOMID.Text.Trim();
                    objclsBeansPhases.SiteID = ucmbSiteID.Text.Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Date = dtpDate.Value;
                    //objclsBeansPhases.ActualAmt = double.Parse(txtActualAmount.Value.ToString());
                    objclsBeansPhases.Dtbl = getDatasource_InGrid();
                    if (objclsBeansPhases.Dtbl.Rows.Count == 0)
                    {
                        MessageBox.Show("Enter Issue Quantity....!");
                        return;
                    }
                    objclsBeansPhases.DtblPT = getDatasource_InGridPT();
                    objclsBeansPhases.DtblDocList = _DocumentList;
                    _DocumentList = null;

                    string _IssueID = objclsBLLPhases.SaveIssues(objclsBeansPhases,true);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK);

                    SetFormMode();
                    ucmbIssueID.Text = _IssueID;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            objdsRptIssueNote.Clear();

            try
            {
                if (ucmbIssueID.Text.Trim() != string.Empty)
                {
                    String S12 = "Select * from viewRptIssueNote where IssueNo='" + ucmbIssueID.Text.Trim() + "'";
                    SqlCommand cmd12 = new SqlCommand(S12);
                    SqlConnection con12 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                    da12.Fill(objdsRptIssueNote, "dtblIssueNote");

                    frmViewerJobActual printax = new frmViewerJobActual(objdsRptIssueNote,"rptSiteIssueNote");
                    printax.Show();
                }                
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Site Issue", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double GetTotals(double CostOfSAlesAmt,string SubPhase,string Phase)
        {
            double _Amt = 0;

            foreach (UltraGridRow udr in udgvSubcontractors.Rows)
            {
                if (udr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && udr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                {
                    if (udr.Cells["colPhase"].Value.ToString().Trim() == Phase && udr.Cells["colSubPhase"].Value.ToString().Trim() == SubPhase)
                        _Amt = _Amt + CostOfSAlesAmt;
                }
            }
            return _Amt;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                objDataSet = new DataSet();
                objDataSet = objclsBLLPhases.GetIssues_All();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                //if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
                {
                    frmMain.objfrmFind = new frmFind("Site Issue");
                }
                this.TopMost = false;
                frmMain.objfrmFind.ShowDialog();
                frmMain.objfrmFind.TopMost = true; 

                if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue))
                {
                    ucmbIssueID.Value = clsBeansFind.ReturnValue;
                    fillControls();
                    FormMode = enmFormMode.Find;
                    SetFormMode();
                }
                else
                {
                    FormMode = enmFormMode.Initialize;
                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void fillControls()
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                objclsBeansPhases = objclsBLLPhases.GetIssues_ByID(ucmbIssueID.Text.Trim());

                if (objclsBeansPhases.IssueNo != null)
                {
                    //txtActualAmount.Value = objclsBeansPhases.ActualAmt;
                    txtDescription.Text = objclsBeansPhases.Description;
                    dtpDate.Value = objclsBeansPhases.Date;
                    ucmbSiteID.Value = objclsBeansPhases.SiteID;
                    ucmbIssueID.Value = objclsBeansPhases.IssueNo;
                    ucmbBOMID.Value = objclsBeansPhases.BOMID;

                    chkActive.Checked = objclsBeansPhases.Inactive;

                    fillIssuesGrid(objclsBeansPhases.Dtbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillIssuesGrid(DataTable _dataTable)
        {
            int _Index = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                    udgvSubcontractors.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colItem"].Value = dr["ItemID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colLocation"].Value = dr["WareHouseID"].ToString();
                    //udgvSubcontractors.Rows[_Index].Cells["colRate"].Value = dr["Rate"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value = dr["Units"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colOnHandQty"].Value = double.Parse(dr["OnHandAtIssue"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colEstQty"].Value = double.Parse(dr["BOMQty"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colIssueQty"].Value = double.Parse(dr["IssuedQty"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colBalance"].Value = double.Parse(dr["BOMBalanceQty"].ToString()).ToString("0.00");
                    //udgvSubcontractors.Rows[_Index].Cells["colDelete"].Value = "Delete";
                    _Index = _Index + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                FormMode = enmFormMode.Delete;
                objclsBLLPhases = new clsBLLPhases();
                objclsBeansPhases = new clsBeansPhases();

                if (IsValidControls())
                {
                    objclsBeansPhases.IssueNo = ucmbIssueID.Text.Trim();

                    objclsBLLPhases.DeleteIssues(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Deleted Successfully", _msgTitle, MessageBoxButtons.OK);

                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private DataTable getDatasource_InGrid()
        {
            DataTable _dataTable = new DataTable();
            clsBLLPhases obj=new clsBLLPhases ();
            clsBeansPhases _objclsBeansPhases=new clsBeansPhases ();
            clsBBLPTImport objclsBBLPTImport = new clsBBLPTImport();
            double Qty = 0;
            double Amt = 0;

            try
            {
                if (udgvSubcontractors.Rows.Count > 0)
                {
                    _dataTable.Columns.Add("PhaseID");
                    _dataTable.Columns.Add("SubPhaseID");
                    _dataTable.Columns.Add("Balance");
                    _dataTable.Columns.Add("LocationID");
                    _dataTable.Columns.Add("Estimated");
                    _dataTable.Columns.Add("Units");
                    _dataTable.Columns.Add("Qty");
                    _dataTable.Columns.Add("Issued");
                    _dataTable.Columns.Add("Item");
                    _dataTable.Columns.Add("OH");
                    _dataTable.Columns.Add("GL");
                    _dataTable.Columns.Add("CostofSales");
                    _dataTable.Columns.Add("ItemClass");
                    _dataTable.Columns.Add("ItemDesc");
                    _dataTable.Columns.Add("LastUnitCost");



                    foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                    {
                        if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0
                            && dgvr.Cells["colIssueQty"].Value != null && dgvr.Cells["colIssueQty"].Value.ToString().Trim().Length > 0)
                        {
                            if (dgvr.Cells["colUnits"].Value.ToString() != "Sub Phase Total" && dgvr.Cells["colUnits"].Value.ToString() != "Phase Total")
                            {
                                if (double.Parse(dgvr.Cells["colIssueQty"].Value.ToString()) > 0)
                                {
                                    DataRow drow = _dataTable.NewRow();
                                    drow["PhaseID"] = dgvr.Cells["colPhase"].Text;

                                    drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Text;
                                    drow["Item"] = dgvr.Cells["colItem"].Text;
                                    DataSet _dtsitem = obj.GetItem_By_ItemID(dgvr.Cells["colItem"].Text);

                                    drow["LastUnitCost"] = objclsBBLPTImport.ImportItemUnitCost(dgvr.Cells["colItem"].Text);

                                    drow["ItemDesc"] = _dtsitem.Tables[0].Rows[0]["ItemDis"].ToString();
                                    drow["ItemClass"] = _dtsitem.Tables[0].Rows[0]["ItemClass"].ToString();
                                    drow["LocationID"] = dgvr.Cells["colLocation"].Text;


                                    if (dgvr.Cells["colEstQty"].Text == null || dgvr.Cells["colEstQty"].Text.Trim().Length == 0)
                                        drow["Estimated"] = "0";
                                    else
                                        drow["Estimated"] = dgvr.Cells["colEstQty"].Text;

                                    drow["Units"] = dgvr.Cells["colUnits"].Text;

                                    //if (dgvr.Cells["colEstQty"].Text == null || dgvr.Cells["colEstQty"].Text.Trim().Length == 0)
                                    //    dgvr.Cells["colIssueQty"].Text = 0;
                                    DataSet _dstIssued = objclsBLLPhases.getIssuedetails_BySiteID_Phase_SubPhase(ucmbSiteID.Text, dgvr.Cells["colPhase"].Text, dgvr.Cells["colSubPhase"].Text);

                                    //if (_dstIssued.Tables.Count > 0 && _dstIssued.Tables[0].Rows.Count > 0)
                                    //{
                                    //    Amt = double.Parse(_dstIssued.Tables[0].Rows[0]["Amount"].ToString());
                                    //    Qty = double.Parse(_dstIssued.Tables[0].Rows[0]["Qty"].ToString());
                                    //}

                                    drow["Issued"] = double.Parse(dgvr.Cells["colIssueQty"].Text) + Qty;

                                    if (dgvr.Cells["colBalance"].Text == null || dgvr.Cells["colBalance"].Text.Trim().Length == 0)
                                        drow["Balance"] = 0;
                                    else
                                        drow["Balance"] = dgvr.Cells["colBalance"].Text;

                                    if (dgvr.Cells["colOnHandQty"].Text.Trim().Length > 0)
                                        drow["OH"] = dgvr.Cells["colOnHandQty"].Text;
                                    else
                                        drow["OH"] = 0;
                                    //DataSet _dts=obj.GetWereHouse_By_ID(dgvr.Cells["colLocation"].Text);
                                    DataSet _dts = obj.GetItemMaster_By_ItemID(dgvr.Cells["colItem"].Text);
                                    if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                                        drow["CostofSales"] = double.Parse(_dts.Tables[0].Rows[0]["UnitCost"].ToString()) + Amt;
                                    else
                                        drow["CostofSales"] = 0 + Amt;
                                    drow["GL"] = _dts.Tables[0].Rows[0]["SalesGLAccount"].ToString();
                                    _dataTable.Rows.Add(drow);

                                    //_TempBalance = double.Parse(ugR.Cells["colEstQty"].Value.ToString().Trim()) - double.Parse(ugR.Cells["colIssueQty"].Value.ToString().Trim());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        private DataTable getDatasource_InGridPT()
        {
            DataTable _dataTable = new DataTable();
            clsBLLPhases obj = new clsBLLPhases();
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();
            clsBBLPTImport objclsBBLPTImport = new clsBBLPTImport();
            double Qty = 0;
            double Amt = 0;


            try
            {
                //_dataTable.Rows[0].
                if (udgvSubcontractors.Rows.Count > 0)
                {
                    _dataTable.Columns.Add("PhaseID");
                    _dataTable.Columns.Add("SubPhaseID");
                    _dataTable.Columns.Add("Balance");
                    _dataTable.Columns.Add("LocationID");
                    _dataTable.Columns.Add("Estimated");
                    _dataTable.Columns.Add("Units");
                    _dataTable.Columns.Add("Qty");
                    _dataTable.Columns.Add("Issued");
                    _dataTable.Columns.Add("Item");
                    _dataTable.Columns.Add("OH");
                    _dataTable.Columns.Add("GL");
                    _dataTable.Columns.Add("CostofSales");
                    _dataTable.Columns.Add("ItemClass");
                    _dataTable.Columns.Add("ItemDesc");
                    _dataTable.Columns.Add("LastUnitCost");
                    _dataTable.Columns.Add("BOMRate");


                    foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                    {
                        if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0
                            && dgvr.Cells["colIssueQty"].Value != null && dgvr.Cells["colIssueQty"].Value.ToString().Trim().Length > 0)
                        {
                            if (dgvr.Cells["colUnits"].Value.ToString() != "Sub Phase Total" && dgvr.Cells["colUnits"].Value.ToString() != "Phase Total")
                            {
                                if (double.Parse(dgvr.Cells["colIssueQty"].Value.ToString()) > 0)
                                {
                                    DataRow drow = _dataTable.NewRow();
                                    drow["PhaseID"] = dgvr.Cells["colPhase"].Text;

                                    drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Text;
                                    drow["Item"] = dgvr.Cells["colItem"].Text;
                                    DataSet _dtsitem = obj.GetItem_By_ItemID(dgvr.Cells["colItem"].Text);

                                    drow["LastUnitCost"] = objclsBBLPTImport.ImportItemUnitCost(dgvr.Cells["colItem"].Text);

                                    drow["ItemDesc"] = _dtsitem.Tables[0].Rows[0]["ItemDis"].ToString();
                                    drow["ItemClass"] = _dtsitem.Tables[0].Rows[0]["ItemClass"].ToString();
                                    drow["LocationID"] = dgvr.Cells["colLocation"].Text;

                                    DataSet _dt = objclsBLLPhases.GetBOM_By_ItemID_PhaseID_SubPhaseID(dgvr.Cells["colItem"].Text, dgvr.Cells["colPhase"].Text, dgvr.Cells["colSubPhase"].Text);
                                    drow["BOMRate"] = _dt.Tables[0].Rows[0]["Rate"].ToString();

                                    if (dgvr.Cells["colEstQty"].Text == null || dgvr.Cells["colEstQty"].Text.Trim().Length == 0)
                                        drow["Estimated"] = "0";
                                    else
                                        drow["Estimated"] = dgvr.Cells["colEstQty"].Text;

                                    drow["Units"] = dgvr.Cells["colUnits"].Text;

                                    DataSet _dstIssued = objclsBLLPhases.getIssuedetails_BySiteID_Phase_SubPhase(ucmbSiteID.Text, dgvr.Cells["colPhase"].Text, dgvr.Cells["colSubPhase"].Text);

                                    //double _exstQty = GetTotals_Qty(dgvr.Cells["colSubPhase"].Value.ToString(), dgvr.Cells["colPhase"].Value.ToString());
                                    if (_dstIssued.Tables.Count > 0 && _dstIssued.Tables[0].Rows.Count > 0)
                                    {
                                        Amt = double.Parse(_dstIssued.Tables[0].Rows[0]["Amount"].ToString());
                                        Qty = double.Parse(_dstIssued.Tables[0].Rows[0]["Qty"].ToString());
                                    }

                                    drow["Issued"] = double.Parse(dgvr.Cells["colIssueQty"].Text);

                                    if (dgvr.Cells["colBalance"].Text == null || dgvr.Cells["colBalance"].Text.Trim().Length == 0)
                                        drow["Balance"] = 0;
                                    else
                                        drow["Balance"] = dgvr.Cells["colBalance"].Text;

                                    if (dgvr.Cells["colOnHandQty"].Text.Trim().Length > 0)
                                        drow["OH"] = dgvr.Cells["colOnHandQty"].Text;
                                    else
                                        drow["OH"] = 0;
                                    //DataSet _dts=obj.GetWereHouse_By_ID(dgvr.Cells["colLocation"].Text);
                                    DataSet _dts = obj.GetItemMaster_By_ItemID(dgvr.Cells["colItem"].Text);
                                    //if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                                        drow["CostofSales"] = double.Parse(_dts.Tables[0].Rows[0]["UnitCost"].ToString()) ;//+ Amt;
                                    //else
                                    //    drow["CostofSales"] = 0 + Amt;
                                        drow["GL"] = _dts.Tables[0].Rows[0]["CostOfSalesAcc"].ToString(); _dataTable.Rows.Add(drow);

                                    //_TempBalance = double.Parse(ugR.Cells["colEstQty"].Value.ToString().Trim()) - double.Parse(ugR.Cells["colIssueQty"].Value.ToString().Trim());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        private double GetTotals_Amt(string SubPhase, string Phase)
        {
            double _Amt = 0;

            foreach (UltraGridRow udr in udgvSubcontractors.Rows)
            {
                if (udr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && udr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                {
                    if (udr.Cells["colPhase"].Value.ToString().Trim() == Phase && udr.Cells["colSubPhase"].Value.ToString().Trim() == SubPhase)
                        _Amt = _Amt + double.Parse(udr.Cells["colAmount"].Value.ToString());
                }
            }
            return _Amt;
        }

        private double GetTotals_Qty(string SubPhase, string Phase)
        {
            double _Amt = 0;

            foreach (UltraGridRow udr in udgvSubcontractors.Rows)
            {
                if (udr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && udr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                {
                    if (udr.Cells["colPhase"].Value.ToString().Trim() == Phase && udr.Cells["colSubPhase"].Value.ToString().Trim() == SubPhase)
                        _Amt = _Amt + double.Parse(udr.Cells["colIssueQty"].Value.ToString());
                }
            }
            return _Amt;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                frmBOQ_Load(sender, e);
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void udgvSubcontractors_AfterCellUpdate(object sender, CellEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                if (e.Cell == null) return;
                ugR = udgvSubcontractors.ActiveRow;
                DataTable _dtblItemOH = new DataTable();

                if (e.Cell.Column.Key == "colItem")
                {
                    if (ugR.Cells["colItem"].Value != null && ugR.Cells["colItem"].Value.ToString().Trim().Length > 0)
                    {
                        objDataSet = objclsBLLPhases.GetItem_By_ItemID(ugR.Cells["colItem"].Value.ToString());

                        if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                        {
                            ugR.Cells["colUnits"].Value = objDataSet.Tables[0].Rows[0]["UOM"].ToString();
                            //ugR.Cells["colRate"].Value = objDataSet.Tables[0].Rows[0]["UnitCost"].ToString();
                        }

                        ucmbLocCode.DataSource = null;
                        ucmbLocCode.DataSource = objclsBLLPhases.GetWarehouses_By_ItemID(ugR.Cells["colItem"].Value.ToString());
                        ucmbLocCode.DisplayMember = "WhseId";
                        ucmbLocCode.ValueMember = "WhseId";
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Width = 40;
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Width = 100;
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[2].Width = 100;
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[2].Header.Caption = "OnHand Qty.";

                        string SP=string.Empty;
                        string SsP = string.Empty;

                        if (ugR.Cells["colPhase"].Value != null) SP = ugR.Cells["colPhase"].Value.ToString().Trim();
                        if (ugR.Cells["colSubPhase"].Value != null) SsP = ugR.Cells["colSubPhase"].Value.ToString().Trim();

                        clsBeansPhases _objclsBeansPhases = objclsBLLPhases.GetBOMs_ByID_Max(ucmbBOMID.Text.Trim(), 
                            0, SP, SsP);
                        //fillGrid_On_BOMID(_objclsBeansPhases.Dtbl);
                        if (_objclsBeansPhases.Dtbl.Rows.Count > 0)
                        {
                            ugR.Cells["colEstQty"].Value = _objclsBeansPhases.Dtbl.Rows[0]["Qty"].ToString();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("");
                        }
                    }
                }

                if (FormMode != enmFormMode.Find)
                {
                    if (ugR.Cells["colItem"].Value != null && ugR.Cells["colItem"].Value.ToString().Trim() != string.Empty &&
                        ugR.Cells["colPhase"].Value != null && ugR.Cells["colPhase"].Value.ToString().Trim() != string.Empty)
                    {
                        DataSet _dts = objclsBLLPhases.GetBOM_By_ItemID_PhaseID_SubPhaseID(ugR.Cells["colItem"].Value.ToString().Trim(),
                            ugR.Cells["colPhase"].Value.ToString().Trim(), ugR.Cells["colSubPhase"].Value.ToString().Trim());
                        if (_dts.Tables[0].Rows.Count > 0)
                        {
                            double Qty = 0;
                            //ugR.Cells["colEstQty"].Value =_dts.Tables[0].Rows[0]["Qty"].ToString();
                            foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                            {
                                if(dgvr.Cells["colItem"].Value==ugR.Cells["colItem"].Value &&
                                    dgvr.Cells["colPhase"].Value == ugR.Cells["colPhase"].Value &&
                                    dgvr.Cells["colSubPhase"].Value == ugR.Cells["colSubPhase"].Value)

                                Qty=Qty+double.Parse(dgvr.Cells["colIssueQty"].Value.ToString());
                            }
                            if(!user.IsMoreThanBOMQty)
                            {
                                if (double.Parse(ugR.Cells["colEstQty"].Value.ToString()) < Qty)
                                {
                                    MessageBox.Show("Invalid Qty....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                   // ugR.Cells["colIssueQty"].Value = "0";
                                    return;
                                }
                            }
                        }
                        else
                        {
                            //ugR.Cells["colItem"].Value = "";
                            //ugR.Cells["colPhase"].Value = "";
                            //ugR.Cells["colSubPhase"].Value = "";
                            //ugR.Cells["colUnits"].Value = "";
                            //ugR.Cells["colLocation"].Value = "";
                            //ugR.Cells["colIssueQty"].Value = "0";
                            //ugR.Cells["colBalance"].Value = "0";
                            //ugR.Cells["colOnHandQty"].Value = "0";
                            //ugR.Cells["colEstQty"].Value = "0";
                            
                             DialogResult d = MessageBox.Show("This Item is Not in BOM. Do you want to add....?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                             if (d.ToString().ToUpper() == "YES")
                             {
                                 double Qty = 0;
                                 //ugR.Cells["colEstQty"].Value =_dts.Tables[0].Rows[0]["Qty"].ToString();
                                 foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                                 {
                                     if (dgvr.Cells["colItem"].Value == ugR.Cells["colItem"].Value &&
                                         dgvr.Cells["colPhase"].Value == ugR.Cells["colPhase"].Value &&
                                         dgvr.Cells["colSubPhase"].Value == ugR.Cells["colSubPhase"].Value)

                                         Qty = Qty + double.Parse(dgvr.Cells["colIssueQty"].Value.ToString());
                                 }
                                 if (!user.IsMoreThanBOMQty)
                                 {
                                     if (double.Parse(ugR.Cells["colEstQty"].Value.ToString()) < Qty)
                                     {
                                         MessageBox.Show("Invalid Qty....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                         // ugR.Cells["colIssueQty"].Value = "0";
                                         return;
                                     }
                                 }
                             }
                             else
                                 ugR.Cells["colItem"].Value = string.Empty;
                        }
                    }
                }


                //if (e.Cell.Column.Key == "colItem" || e.Cell.Column.Key == "colLocation")
                //{
                //    if (ugR.Cells["colLocation"].Value != null && ugR.Cells["colLocation"].Value.ToString().Trim().Length > 0)
                //    {
                //        _dtblItemOH = new DataTable();
                //        _dtblItemOH = objclsBLLPhases.GetItems_By_WH_ItemCode(ugR.Cells["colLocation"].Value.ToString().Trim(), ugR.Cells["colItem"].Value.ToString().Trim()).Tables[0];
                //        if(_dtblItemOH.Rows.Count > 0)
                //            ugR.Cells["colOnHandQty"].Value = double.Parse(_dtblItemOH.Rows[0]["QTY"].ToString()).ToString("0.00");
                //    }
                //}
                if (FormMode != enmFormMode.Find)
                {
                    if (e.Cell.Column.Key == "colItem" || e.Cell.Column.Key == "colLocation")
                    {
                        if (ugR.Cells["colLocation"].Value != null && ugR.Cells["colLocation"].Value.ToString().Trim().Length > 0
                            && ugR.Cells["colItem"].Value != null && ugR.Cells["colItem"].Value.ToString().Trim().Length > 0)
                        {
                            _dtblItemOH = new DataTable();
                            _dtblItemOH = objclsBLLPhases.GetItems_By_WH_ItemCode(ugR.Cells["colLocation"].Value.ToString().Trim(), ugR.Cells["colItem"].Value.ToString().Trim()).Tables[0];
                            if (_dtblItemOH.Rows.Count > 0)
                            {
                                ugR.Cells["colOnHandQty"].Value = double.Parse(_dtblItemOH.Rows[0]["QTY"].ToString()).ToString("0.00");
                            }
                        }
                    }

                    if (e.Cell.Column.Key == "colIssueQty")
                    {
                        if (ugR.Cells["colEstQty"].Value != null && ugR.Cells["colEstQty"].Value.ToString().Trim().Length > 0
                            && ugR.Cells["colIssueQty"].Value != null && ugR.Cells["colIssueQty"].Value.ToString().Trim().Length > 0)
                        {
                            double _TempBalance = 0.00;

                            _TempBalance = double.Parse(ugR.Cells["colEstQty"].Value.ToString().Trim()) - double.Parse(ugR.Cells["colIssueQty"].Value.ToString().Trim());

                            if (_TempBalance < 0)
                            {
                                if (!user.IsMoreThanBOMQty)
                                {
                                    ugR.Cells["colIssueQty"].Value = 0.00;
                                    MessageBox.Show("Issuing Quantity is Invalid.....!");
                                    return;
                                }
                            }

                            ugR.Cells["colBalance"].Value = (double.Parse(ugR.Cells["colEstQty"].Value.ToString().Trim()) - double.Parse(ugR.Cells["colIssueQty"].Value.ToString().Trim())).ToString("0.00");
                        }
                    }

                }

                //if (e.Cell.Column.Key == "colQty" || e.Cell.Column.Key == "colRate")
                //{
                //    if (ugR.Cells["colRate"].Value != null && ugR.Cells["colRate"].Value.ToString().Trim().Length > 0 &&
                //        ugR.Cells["colQty"].Value != null && ugR.Cells["colQty"].Value.ToString().Trim().Length > 0)
                //    {
                //        ugR.Cells["colAmount"].Value = double.Parse(ugR.Cells["colQty"].Value.ToString()) * double.Parse(ugR.Cells["colRate"].Value.ToString());
                //    }
                //}
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        private void ucmbLocCode_Enter(object sender, EventArgs e)
        {
            ucmbLocCode.PerformAction(UltraComboAction.Dropdown);
            //ucmbLocCode.ToggleDropdown(
        }

        private void ucmbLocCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ucmbLocCode.PerformAction(UltraComboAction.Dropdown);

        }

        private void udgvSubcontractors_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int _currntIndex = 0;
                _currntIndex = udgvSubcontractors.ActiveRow.Index;

                if (udgvSubcontractors.ActiveCell == null) return;
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                if (udgvSubcontractors.ActiveCell.Column.Key == "colIssueQty")
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                    {
                        if (GetNoOfEmptyRows() < 2)
                        {
                            ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                            udgvSubcontractors.ActiveCell = udgvSubcontractors.Rows[_currntIndex + 1].Cells[0];
                            udgvSubcontractors.PerformAction(UltraGridAction.PrevCell, true, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            erpMaster.Dispose();
        }

        private void ucmbSiteID_KeyPress(object sender, KeyPressEventArgs e)
        {
            erpMaster.Dispose();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void ucmbSiteID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (ucmbSiteID.Value != null)
                {
                    _objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                    ucmbCustomerID.Value = _objclsBeansPhases.CustomerID;
                    //objclsCommonFunc.fillBOMs_Active(
                    objclsCommonFunc.fillCustomer(ucmbCustomerID, ucmbSiteID);
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbCustomerID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            //clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (ucmbCustomerID.Value != null)
                {
                    //objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                    txtCustomerName.Text = ucmbCustomerID.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }





        private void fillBOMGrid_ON_BOQ_SELECTION(DataTable _dataTable)
        {
            int _Index = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                    udgvSubcontractors.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colActivity"].Value = dr["Activity"].ToString();
                    _Index = _Index + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ucmbBOMID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            clsBLLPhases objclsBLLPhases = new clsBLLPhases();
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (FormMode != enmFormMode.Find)
                {
                    if (ucmbBOMID.Value != null && ucmbBOMID.Value.ToString().Trim().Length > 0)
                    {
                        _objclsBeansPhases = objclsBLLPhases.GetBOMs_ByID_Max(ucmbBOMID.Text.Trim(), 0);
                        fillGrid_On_BOMID(_objclsBeansPhases.Dtbl);
                        ucmbSiteID.Text = _objclsBeansPhases.SiteID;
                    }
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void fillGrid_On_BOMID(DataTable _dataTable)
        {
            int _Index = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            DataTable _dtblItemOH = new DataTable();
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                    udgvSubcontractors.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colLocation"].Value = dr["WareHouseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colEstQty"].Value = double.Parse(dr["Qty"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colItem"].Value = dr["ItemID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colEstQty"].Value = double.Parse(dr["Qty"].ToString()).ToString("0.00"); 
                    udgvSubcontractors.Rows[_Index].Cells["colBalance"].Value = (double.Parse(dr["Qty"].ToString()) - double.Parse(dr["IssuedQty"].ToString())).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colOnHandQty"].Value = double.Parse(dr["OHQty"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value = dr["Units"].ToString();
                    _Index = _Index + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        private void ucmbIssueID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            
        }

        private void ucmbBOMID_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (ucmbBOMID.Value != null && ucmbBOMID.Value.ToString().Trim().Length > 0)
                {
                    frmBOM objfrmBOM = new frmBOM(ucmbBOMID.Value.ToString().Trim());
                    objfrmBOM.Show();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbSiteID_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (ucmbSiteID.Value != null && ucmbSiteID.Value.ToString().Trim().Length > 0)
                {
                    frmJob objfrmJob = new frmJob(ucmbSiteID.Value.ToString().Trim());
                    objfrmJob.Show();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbtnLast_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbIssueID.Text = objclsBLLPhases.GetCodeforArrow("Issue", ucmbIssueID.Text.Trim(), "RR");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        private void tsbtnFirst_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbIssueID.Text = objclsBLLPhases.GetCodeforArrow("Issue", ucmbIssueID.Text.Trim(), "LL");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbtnPrevious_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbIssueID.Text = objclsBLLPhases.GetCodeforArrow("Issue", ucmbIssueID.Text.Trim(), "L");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbtnNext_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbIssueID.Text = objclsBLLPhases.GetCodeforArrow("Issue", ucmbIssueID.Text.Trim(), "R");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            try
            {
                clsBeansPhases.DtblAttachment = _DocumentList;
                frmUpload objfrmUpload = new frmUpload();
                objfrmUpload.ShowDialog();

                if (clsBeansPhases.DtblAttachment != null)
                {
                    if (clsBeansPhases.DtblAttachment.Rows.Count > 0)
                    {
                        _DocumentList = clsBeansPhases.DtblAttachment;
                        clsBeansPhases.DtblAttachment = null;
                    }
                    else
                        _DocumentList = null;

                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbSiteID_ValueChanged(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            //clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (ucmbCustomerID.Value != null)
                {
                    //objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                    txtCustomerName.Text = ucmbCustomerID.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbCustomerID_ValueChanged(object sender, EventArgs e)
        {
            clsBLLPhases _oobjclsBLLPhases = new clsBLLPhases();
            //objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (ucmbCustomerID.Value != null)
                {
                    txtCustomerName.Text = _oobjclsBLLPhases.GetCustomer_ByID(ucmbCustomerID.Text.Trim().ToString()).Description;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Save;
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                //if(

                if (IsValidControls())
                {
                    if (ucmbIssueID.Text.Trim() == string.Empty)
                    {
                        MessageBox.Show("Save this first...!", "Issue Note", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    objclsBeansPhases.IssueNo = ucmbIssueID.Text.Trim();
                    objclsBeansPhases.BOMID = ucmbBOMID.Text.Trim();
                    objclsBeansPhases.SiteID = ucmbSiteID.Text.Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Date = dtpDate.Value;
                    //objclsBeansPhases.ActualAmt = double.Parse(txtActualAmount.Value.ToString());
                    objclsBeansPhases.Dtbl = getDatasource_InGrid();
                    objclsBeansPhases.DtblDocList = _DocumentList;
                    _DocumentList = null;

                    string _IssueID = objclsBLLPhases.SaveIssues(objclsBeansPhases, false);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK);


                    MessageBox.Show("Edit this record in Peachtree....!","Reminder",MessageBoxButtons.OK,MessageBoxIcon.Warning);

                    SetFormMode();
                    ucmbIssueID.Text = _IssueID;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private int GetNoOfEmptyRows()
        {
            int _NoOfEmptyRows = 0;

            try
            {
                foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                {
                    if ((dgvr.Cells["colPhase"].Value == null || dgvr.Cells["colPhase"].Value.ToString() == string.Empty)
                        && (dgvr.Cells["colSubPhase"].Value == null || dgvr.Cells["colSubPhase"].Value.ToString() == string.Empty))
                    {

                        _NoOfEmptyRows = _NoOfEmptyRows + 1;
                        //DataRow drow = _dataTable.NewRow();
                        //drow["Item"] = "";
                        //drow["PhaseID"] = dgvr.Cells["colPhase"].Value;
                        //drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Value;
                        //drow["Activity"] = dgvr.Cells["colActivity"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _NoOfEmptyRows;
        }

        private void udgvSubcontractors_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            try
            {
                if (GetNoOfEmptyRows() >= 2)
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbIssueID_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (ucmbIssueID.Value != null && ucmbIssueID.Value.ToString().Trim().Length > 0)
                {
                    FormMode = enmFormMode.Find;
                    fillControls();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbItems1_ValueChanged(object sender, EventArgs e)
        {
            udgvSubcontractors.ActiveCell.Value = "A001";
        }

        private void setSelection()
        {
            try
            {
                //if (ucmbItems1.Value == null) return;
                int rowCount = udgvSubcontractors.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    udgvSubcontractors.Rows[i].Selected = false;
                    if (ucmbItems1.Value != null && ucmbItems1.Value.ToString().Trim() != string.Empty
                        && ucmbPhaseID1.Value != null && ucmbPhaseID1.Value.ToString().Trim() != string.Empty
                        && ucmbSubPhaseID1.Value != null && ucmbSubPhaseID1.Value.ToString().Trim() != string.Empty)
                    {
                        if (udgvSubcontractors.Rows[i].Cells[0].Value.ToString().Contains(ucmbPhaseID1.Value.ToString().Trim())
                            && udgvSubcontractors.Rows[i].Cells[1].Value.ToString().Contains(ucmbSubPhaseID1.Value.ToString().Trim())
                            && udgvSubcontractors.Rows[i].Cells[2].Value.ToString().Contains(ucmbItems1.Value.ToString().Trim()))
                        {
                            udgvSubcontractors.Rows[i].Selected = true;
                            //udgvSubcontractors.Rows[i].Expanded = true;
                            udgvSubcontractors.ActiveRowScrollRegion.ScrollPosition = i;
                            //break;
                        }
                    }

                    else if (ucmbPhaseID1.Value != null && ucmbPhaseID1.Value.ToString().Trim() != string.Empty
                      && ucmbSubPhaseID1.Value != null && ucmbSubPhaseID1.Value.ToString().Trim() != string.Empty)
                    {
                        if (udgvSubcontractors.Rows[i].Cells[0].Value.ToString().Contains(ucmbPhaseID1.Value.ToString().Trim())
                            && udgvSubcontractors.Rows[i].Cells[1].Value.ToString().Contains(ucmbSubPhaseID1.Value.ToString().Trim()))
                        {
                            udgvSubcontractors.Rows[i].Selected = true;
                            udgvSubcontractors.ActiveRowScrollRegion.ScrollPosition = i;
                            //break;
                        }
                    }

                    else if (ucmbItems1.Value != null && ucmbItems1.Value.ToString().Trim() != string.Empty
                      && ucmbPhaseID1.Value != null && ucmbPhaseID1.Value.ToString().Trim() != string.Empty)
                    {
                        if (udgvSubcontractors.Rows[i].Cells[0].Value.ToString().Contains(ucmbPhaseID1.Value.ToString().Trim())
                            && udgvSubcontractors.Rows[i].Cells[2].Value.ToString().Contains(ucmbItems1.Value.ToString().Trim()))
                        {
                            udgvSubcontractors.Rows[i].Selected = true;
                            udgvSubcontractors.ActiveRowScrollRegion.ScrollPosition = i;
                            //break;
                        }
                    }

                    else if (ucmbItems1.Value != null && ucmbItems1.Value.ToString().Trim() != string.Empty
                      && ucmbSubPhaseID1.Value != null && ucmbSubPhaseID1.Value.ToString().Trim() != string.Empty)
                    {
                        if (udgvSubcontractors.Rows[i].Cells[1].Value.ToString().Contains(ucmbSubPhaseID1.Value.ToString().Trim())
                            && udgvSubcontractors.Rows[i].Cells[2].Value.ToString().Contains(ucmbItems1.Value.ToString().Trim()))
                        {
                            udgvSubcontractors.Rows[i].Selected = true;
                            udgvSubcontractors.ActiveRowScrollRegion.ScrollPosition = i;
                            //break;
                        }
                    }

                    else if (ucmbItems1.Value != null && ucmbItems1.Value.ToString().Trim() != string.Empty)
                    {
                        if (udgvSubcontractors.Rows[i].Cells[2].Value.ToString().Contains(ucmbItems1.Value.ToString().Trim()))
                        {
                            udgvSubcontractors.Rows[i].Selected = true;
                            udgvSubcontractors.ActiveRowScrollRegion.ScrollPosition = i;
                            //break;
                        }
                    }

                    else if (ucmbPhaseID1.Value != null && ucmbPhaseID1.Value.ToString().Trim() != string.Empty)
                    {
                        if (udgvSubcontractors.Rows[i].Cells[0].Value.ToString().Contains(ucmbPhaseID1.Value.ToString().Trim()))
                        {
                            udgvSubcontractors.Rows[i].Selected = true;
                            udgvSubcontractors.ActiveRowScrollRegion.ScrollPosition = i;
                            //break;
                        }
                    }

                    else if (ucmbSubPhaseID1.Value != null && ucmbSubPhaseID1.Value.ToString().Trim() != string.Empty)
                    {
                        if (udgvSubcontractors.Rows[i].Cells[1].Value.ToString().Contains(ucmbSubPhaseID1.Value.ToString().Trim()))
                        {
                            udgvSubcontractors.Rows[i].Selected = true;
                            udgvSubcontractors.ActiveRowScrollRegion.ScrollPosition = i;
                            //break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbItems1_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                setSelection();
                //if (ucmbItems1.Value == null) return;
                //int rowCount = udgvSubcontractors.Rows.Count;
                //for (int i = 0; i < rowCount; i++)
                //{
                //    udgvSubcontractors.Rows[i].Selected = false;
                //    if (udgvSubcontractors.Rows[i].Cells[2].Value.ToString().Contains(ucmbItems1.Value.ToString().Trim()))
                //    {
                //        udgvSubcontractors.Rows[i].Selected = true;
                //        //break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbPhaseID1_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                setSelection();
                //if (ucmbPhaseID1.Value == null) return;
                //int rowCount = udgvSubcontractors.Rows.Count;
                //for (int i = 0; i < rowCount; i++)
                //{
                //    udgvSubcontractors.Rows[i].Selected = false;
                //    if (udgvSubcontractors.Rows[i].Cells[0].Value.ToString().Contains(ucmbPhaseID1.Value.ToString().Trim()))
                //    {
                //        udgvSubcontractors.Rows[i].Selected = true;
                //        //break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbSubPhaseID1_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                setSelection();
                //if (ucmbSubPhaseID1.Value == null) return;

                //int rowCount = udgvSubcontractors.Rows.Count;
                //for (int i = 0; i < rowCount; i++)
                //{
                //    udgvSubcontractors.Rows[i].Selected = false;
                //    if (udgvSubcontractors.Rows[i].Cells[1].Value.ToString().Contains(ucmbSubPhaseID1.Value.ToString().Trim()))
                //    {
                //        udgvSubcontractors.Rows[i].Selected = true;
                //        //break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
       

    }
}