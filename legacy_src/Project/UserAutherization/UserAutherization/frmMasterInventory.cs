using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using TJaySmGUI.Transaction;
//using TJaySmBLL;
using ComFunction;
//using TJaySmGUI.Reports;
using System.Reflection;

namespace UserAutherization
{
    public partial class frmMasterInventory : Form
    {
        private string _msgTitle = "Login";



        bool run = false;
        bool Add = false;

        DataTable dtUser = new DataTable();

        public frmMasterInventory()
        {
            InitializeComponent();

        }

        private void frmTransactions_Load(object sender, EventArgs e)
        {

        }

        private void lblInventory_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblInventory.PointToScreen(e.Location);
                cmbInventory.Show(pt);
            }
        }



        private void lblOpBal_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblOpBal.PointToScreen(e.Location);
                cmbOPBal.Show(pt);
            }
        }

        private void lblWHTrans_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblWHTrans.PointToScreen(e.Location);
                cmbWHTrans.Show(pt);
            }
        }

        private void lblReports_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblReports.PointToScreen(e.Location);
                cmbReports.Show(pt);
            }
        }

        private void lblIssRet_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblIssRet.PointToScreen(e.Location);
                cmdISsue.Show(pt);
            }
        }

        private void lblAdj_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblAdj.PointToScreen(e.Location);
                cmbAdjustment.Show(pt);
            }
        }

        private void cmbInventory_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "Import Inventory")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportItemMaster");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //    }
            //    if (run)
            //    {
            //        if (frmMain.ObjImpItem == null || frmMain.ObjImpItem.IsDisposed)
            //        {
            //            frmMain.ObjImpItem = new frmImportItemMaster();
            //        }
            //        //ObjImpItem.ShowDialog();
            //        //ObjImpItem.WindowState = FormWindowState.Normal;
            //        //ObjImpItem.MdiParent = this;
            //        frmMain.ObjImpItem.Show();
            //        //frmMain.ObjImpItem.TopMost = true;
            //        //ObjImpItem.WindowState = FormWindowState.Maximized;
            //        //ObjImpItem.Location = new Point(0, 0);
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}            

            //frmItemMaster i = new frmItemMaster();
            //i.Show();
        }

        private void cmbPM_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Price Metrix Designing")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmMatrixDesign");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmMatrixDesign == null || frmMain.objfrmMatrixDesign.IsDisposed)
                    {
                        frmMain.objfrmMatrixDesign = new frmMatrixDesign();
                    }
                    //objfrmMatrixDesign.MdiParent = this;
                    frmMain.objfrmMatrixDesign.Show();
                    //frmMain.objfrmMatrixDesign.TopMost = true;
                    //objfrmMatrixDesign.WindowState = FormWindowState.Maximized;
                    //objfrmMatrixDesign.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Price Metrix Assigning")
            {
                run = false;
                //frmMatrixDesign objMe = new frmMatrixDesign();
                //objMe.ShowDialog();
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmMatrixDesign");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmMatrixDesign == null || frmMain.objfrmMatrixDesign.IsDisposed)
                    {
                        frmMain.objfrmMatrixDesign = new frmMatrixDesign();
                    }

                    //objfrmMatrixDesign.MdiParent = this;
                    frmMain.objfrmMatrixDesign.Show();
                    //frmMain.objfrmMatrixDesign.TopMost = true;
                    //objfrmMatrixDesign.WindowState = FormWindowState.Maximized;
                    //objfrmMatrixDesign.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Price List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmPriceInquiry");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmPriceInquiry == null || frmMain.objfrmPriceInquiry.IsDisposed)
                    {
                        frmMain.objfrmPriceInquiry = new frmPriceInquiry();
                    }

                    //objfrmPriceInquiry.MdiParent = this;
                    frmMain.objfrmPriceInquiry.Show();
                    // frmMain.objfrmPriceInquiry.TopMost = true;
                    //objfrmPriceInquiry.WindowState = FormWindowState.Maximized;
                    //objfrmPriceInquiry.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbOPBal_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Opening Balance")
            {
                run = false;

                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmBeginingBalances");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmBeginingBalances == null || frmMain.objfrmBeginingBalances.IsDisposed)
                    {
                        frmMain.objfrmBeginingBalances = new frmBeginingBalances();
                    }
                    //objfrmBeginingBalances.MdiParent = this;
                    frmMain.objfrmBeginingBalances.Show();
                    //frmMain.objfrmBeginingBalances.TopMost = true;
                    //objfrmBeginingBalances.WindowState = FormWindowState.Maximized;
                    //objfrmBeginingBalances.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbWHTrans_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Warehouse Transfer")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmWareHouseTrans");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjWareHouseTransfer == null || frmMain.ObjWareHouseTransfer.IsDisposed)
                    {
                        frmMain.ObjWareHouseTransfer = new frmWareHouseTrans(0);
                    }
                    //ObjWareHouseTransfer.MdiParent = this;
                    frmMain.ObjWareHouseTransfer.Show();
                    //frmMain.ObjWareHouseTransfer.TopMost = true;
                    //ObjWareHouseTransfer.WindowState = FormWindowState.Maximized;
                    //ObjWareHouseTransfer.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Warehouse Transfer List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSeachTrans");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmSeachTrans == null || frmMain.objfrmSeachTrans.IsDisposed)
                    {
                        frmMain.objfrmSeachTrans = new frmSeachTrans();
                    }
                    //objfrmSeachTrans.MdiParent = this;
                    frmMain.objfrmSeachTrans.Show();
                    //frmMain.objfrmSeachTrans.TopMost = true;
                    //objfrmSeachTrans.WindowState = FormWindowState.Maximized;
                    //objfrmSeachTrans.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmdISsue_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Issue/Return Note")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmIssueNote");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmIssueNote == null || frmMain.objfrmIssueNote.IsDisposed)
                    {
                        frmMain.objfrmIssueNote = new frmIssueNote(0);
                    }
                    //objfrmIssueNote.MdiParent = this;
                    frmMain.objfrmIssueNote.Show();
                    //frmMain.objfrmIssueNote.TopMost = true;
                    //objfrmIssueNote.WindowState = FormWindowState.Maximized;
                    //objfrmIssueNote.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Issue/Return Note List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSeachIssueNote");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmSeachIssueNote == null || frmMain.objfrmSeachIssueNote.IsDisposed)
                    {
                        frmMain.objfrmSeachIssueNote = new frmSeachIssueNote();
                    }
                    //objfrmSeachIssueNote.MdiParent = this;
                    frmMain.objfrmSeachIssueNote.Show();
                    //frmMain.objfrmSeachIssueNote.TopMost = true;
                    //frmMain.objfrmSeachIssueNote.WindowState = FormWindowState.Normal;
                    //objfrmSeachTrans.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbAdjustment_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Inventory Adjustment")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "Inventory Adjustment");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmInventotyAdjustment == null || frmMain.objfrmInventotyAdjustment.IsDisposed)
                        frmMain.objfrmInventotyAdjustment = new frmInventotyAdjustment();

                    //objfrmInventotyAdjustment.MdiParent = this;
                    frmMain.objfrmInventotyAdjustment.Show();
                    //frmMain.objfrmInventotyAdjustment.TopMost = true;
                    //objfrmInventotyAdjustment.WindowState = FormWindowState.Maximized;
                    //objfrmInventotyAdjustment.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Inventory Adjustment List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInventoryAdjustmentList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmInventoryAdjustmentList == null || frmMain.objfrmInventoryAdjustmentList.IsDisposed)
                    {
                        frmMain.objfrmInventoryAdjustmentList = new frmInventoryAdjustmentList();
                    }
                    //objfrmSeachIssueNote.MdiParent = this;
                    frmMain.objfrmInventoryAdjustmentList.Show();
                    //frmMain.objfrmInventoryAdjustmentList.TopMost = true;
                    //frmMain.objfrmInventoryAdjustmentList.WindowState = FormWindowState.Normal;
                    //objfrmSeachTrans.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbReports_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Item List with Quantity Available")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmValuation");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmValuation == null || frmMain.objfrmValuation.IsDisposed)
                    {
                        frmMain.objfrmValuation = new frmValuation();
                    }
                    frmMain.objfrmValuation.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Inventoy movement")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInventoryMovement");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmInventoryMovement == null || frmMain.objfrmInventoryMovement.IsDisposed)
                    {
                        frmMain.objfrmInventoryMovement = new frmInventoryMovement();
                    }
                    frmMain.objfrmInventoryMovement.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Transfer Note List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmTransNoteList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmTransNoteList == null || frmMain.objfrmTransNoteList.IsDisposed)
                    {
                        frmMain.objfrmTransNoteList = new frmTransNoteList();
                    }
                    frmMain.objfrmTransNoteList.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else if (e.ClickedItem.Text == "Issue / Return Notes List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmTransNoteList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmIssueNoteLIstPrint == null || frmMain.objfrmIssueNoteLIstPrint.IsDisposed)
                    {
                        frmMain.objfrmIssueNoteLIstPrint = new frmIssueNoteLIstPrint();
                    }
                    frmMain.objfrmIssueNoteLIstPrint.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else if (e.ClickedItem.Text == "Adjustments List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmTransNoteList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmadustmentsListPrint == null || frmMain.objfrmadustmentsListPrint.IsDisposed)
                    {
                        frmMain.objfrmadustmentsListPrint = new frmadustmentsListPrint();
                    }
                    frmMain.objfrmadustmentsListPrint.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //Adjuments List
        }

        private void lblPriceMet_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblPriceMet.PointToScreen(e.Location);
                cmbPM.Show(pt);
            }
        }

        private void lblAssemblys_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblAssemblys.PointToScreen(e.Location);
                cmbAssembiles.Show(pt);
            }
        }

        private void cmbAssembiles_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Import Assemblies Master")
            {
                frmImportBOM ObjfrmImportBOM = new frmImportBOM();
                ObjfrmImportBOM.Show();
            }

            if (e.ClickedItem.Text == "Build Assemblies")
            {
                frmBOMBuilding ObjfrmBOMBuilding = new frmBOMBuilding(0);
                ObjfrmBOMBuilding.Show();
            }
        }

        private void lblInventory_Click(object sender, EventArgs e)
        {

        }

        private void ultraLabel1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            if (frmMain.objfrmItemMaster == null || frmMain.objfrmItemMaster.IsDisposed)
            {
                frmMain.objfrmItemMaster = new frmItemMaster();
            }
            frmMain.objfrmItemMaster.Show();

        }

        private void inventoryListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMain.objfrmItemMasterList == null || frmMain.objfrmItemMasterList.IsDisposed)
            {
                frmMain.objfrmItemMasterList = new frmItemMasterList();
            }
            frmMain.objfrmItemMasterList.Show();
        }
        private void ultraPanel1_PaintClient(object sender, PaintEventArgs e)
        {

        }

        private void ultraLabel2_MouseClick(object sender, MouseEventArgs e)
        {
            frmPriceUpdate fpu = new frmPriceUpdate();
            fpu.Show();
        }

        private void ultraLabel2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {

        }

        private void cmbReports_MouseCaptureChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void UlLableDiscountUpdate_MouseClick(object sender, MouseEventArgs e)
        {
            frmDiscountUpdate objDisCupdate = new frmDiscountUpdate();
            objDisCupdate.Show();
        }

        private void ultraLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                frmItemMaster i = new frmItemMaster();
                i.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraLabel1_MouseClick_1(object sender, MouseEventArgs e)
        {

        }

        //private void ultraPanel1_PaintClient(object sender, PaintEventArgs e)
        //{

        //}
















    }
}