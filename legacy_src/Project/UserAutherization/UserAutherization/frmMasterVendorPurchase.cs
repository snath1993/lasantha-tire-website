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
    public partial class frmMasterVendorPurchase : Form
    {       
        private string _msgTitle = "Login";       
        bool run = false;
        bool Add = false;        
        DataTable dtUser = new DataTable();

        public frmMasterVendorPurchase()
        {
            InitializeComponent();            
        }            

        private void cmbDelNote_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Good Receive Note")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmGRN");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    //Connector conn = new Connector();
                    //conn.ImportPurchaseOrderList();
                    //conn.Insert_PurchaseOrderList();

                    if (frmMain.ObjGRN == null || frmMain.ObjGRN.IsDisposed)
                    {
                        frmMain.ObjGRN = new frmGRN();
                    }

                    frmMain.ObjGRN.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjGRN.WindowState == FormWindowState.Minimized)
                        frmMain.ObjGRN.WindowState = FormWindowState.Normal;

                    frmMain.ObjGRN.Show();                   
                   
                    if (Add)
                    {
                        frmMain.ObjGRN.btnNew.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }        
            }
            if (e.ClickedItem.Text == "Good Receive Note List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDeliveryNoteList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjGRNList == null || frmMain.ObjGRNList.IsDisposed)
                    {
                        frmMain.ObjGRNList = new frmDeliveryNoteList();
                    }
                    frmMain.ObjGRNList.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjGRNList.WindowState == FormWindowState.Minimized)
                        frmMain.ObjGRNList.WindowState = FormWindowState.Normal;
                    frmMain.ObjGRNList.Show();                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbInvoice_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Invoice")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupInvoice");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjSupInvoice == null || frmMain.ObjSupInvoice.IsDisposed)
                    {
                        frmMain.ObjSupInvoice = new frmSupInvoice();
                    }

                    frmMain.ObjSupInvoice.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjSupInvoice.WindowState == FormWindowState.Minimized)
                        frmMain.ObjSupInvoice.WindowState = FormWindowState.Normal;
                    frmMain.ObjSupInvoice.Show();                   
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            if (e.ClickedItem.Text == "Invoice List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupInvoiceList");
                if (dtUser.Rows.Count > 0)
                {



















































                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjSupInvoicelist == null || frmMain.ObjSupInvoicelist.IsDisposed)
                    {
                        frmMain.ObjSupInvoicelist = new frmSupInvoiceList();
                    }
                    frmMain.ObjSupInvoicelist.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjSupInvoicelist.WindowState == FormWindowState.Minimized)
                        frmMain.ObjSupInvoicelist.WindowState = FormWindowState.Normal;
                    frmMain.ObjSupInvoicelist.Show();                  
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }               
            }
        }          

        private void cmdReturn_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Return Note")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupplierReturn");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjSupplierRetern == null || frmMain.ObjSupplierRetern.IsDisposed)
                    {
                        frmMain.ObjSupplierRetern = new frmSupplierReturn();
                    }
                    frmMain.ObjSupplierRetern.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjSupplierRetern.WindowState == FormWindowState.Minimized)
                        frmMain.ObjSupplierRetern.WindowState = FormWindowState.Normal;
                    frmMain.ObjSupplierRetern.Location = new Point(0, 0);
                    frmMain.ObjSupplierRetern.Show();                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (e.ClickedItem.Text == "Return Note List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupReturnList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjSupReturnlist == null || frmMain.ObjSupReturnlist.IsDisposed)
                    {
                        frmMain.ObjSupReturnlist = new frmSupReturnList();
                    }
                    frmMain.ObjSupReturnlist.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjSupReturnlist.WindowState == FormWindowState.Minimized)
                        frmMain.ObjSupReturnlist.WindowState = FormWindowState.Normal;
                    frmMain.ObjSupReturnlist.Show();                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }      

        private void lblGRN_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblGRN.PointToScreen(e.Location);
                cmbDelNote.Show(pt);
            }
        }

        private void lblCustomer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblCustomer.PointToScreen(e.Location);
                cmbVendor.Show(pt);
            }
        }

        private void lblInvoice_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblInvoice.PointToScreen(e.Location);
                cmbInvoice.Show(pt);
            }
        }       

        private void lblReturn_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblReturn.PointToScreen(e.Location);
                cmdReturn.Show(pt);
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

        private void cmbVendor_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //
            if (e.ClickedItem.Text == "Import Vendors")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportVendor");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjImpVendor == null || frmMain.ObjImpVendor.IsDisposed)
                    {
                        frmMain.ObjImpVendor = new frmImportVendor();
                    }
                    frmMain.ObjImpVendor.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjImpVendor.WindowState == FormWindowState.Minimized)
                        frmMain.ObjImpVendor.WindowState = FormWindowState.Normal;
                    frmMain.ObjImpVendor.Show();                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbReports_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "PO Varification")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "FrmPurchaseOrderVarification");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjFrmPurchaseOrderVarification == null || frmMain.ObjFrmPurchaseOrderVarification.IsDisposed)
                    {
                        frmMain.ObjFrmPurchaseOrderVarification = new FrmPurchaseOrderVarification();
                    }
                    frmMain.ObjFrmPurchaseOrderVarification.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjFrmPurchaseOrderVarification.WindowState == FormWindowState.Minimized)
                        frmMain.ObjFrmPurchaseOrderVarification.WindowState = FormWindowState.Normal;
                    frmMain.ObjFrmPurchaseOrderVarification.Show();                   
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }
            else if (e.ClickedItem.Text == "GRN LIst")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmGRNList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmGRNList == null || frmMain.objfrmGRNList.IsDisposed)
                    {
                        frmMain.objfrmGRNList = new frmGRNList();
                    }
                    frmMain.objfrmGRNList.WindowState = FormWindowState.Minimized;
                    if (frmMain.objfrmGRNList.WindowState == FormWindowState.Minimized)
                        frmMain.objfrmGRNList.WindowState = FormWindowState.Normal;
                    frmMain.objfrmGRNList.Show();                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Supply Invoice List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupInvList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmSupInvList == null || frmMain.objfrmSupInvList.IsDisposed)
                    {
                        frmMain.objfrmSupInvList = new frmSupInvList();
                    }
                    frmMain.objfrmSupInvList.WindowState = FormWindowState.Minimized;
                    if (frmMain.objfrmSupInvList.WindowState == FormWindowState.Minimized)
                        frmMain.objfrmSupInvList.WindowState = FormWindowState.Normal;
                    frmMain.objfrmSupInvList.Show();                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void lblDirectReturn_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblDirectReturn.PointToScreen(e.Location);
                cmdDirectRetun.Show(pt);
            }
        }

        private void cmdDirectRetun_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Direct Suppler Return")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupplierReturn");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjDirectSupplierReturn == null || frmMain.ObjDirectSupplierReturn.IsDisposed)
                    {
                        frmMain.ObjDirectSupplierReturn = new frmDirectSupplierReturn();
                    }
                    frmMain.ObjDirectSupplierReturn.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjDirectSupplierReturn.WindowState == FormWindowState.Minimized)
                        frmMain.ObjDirectSupplierReturn.WindowState = FormWindowState.Normal;
                    frmMain.ObjDirectSupplierReturn.Location = new Point(0, 0);
                    frmMain.ObjDirectSupplierReturn.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (e.ClickedItem.Text == "Direct Return Note List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupReturnList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjDirectSupplierReturnSearch == null || frmMain.ObjDirectSupplierReturnSearch.IsDisposed)
                    {
                        frmMain.ObjDirectSupplierReturnSearch = new frmDirectSupplierReturnSearch();
                    }
                    frmMain.ObjDirectSupplierReturnSearch.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjDirectSupplierReturnSearch.WindowState == FormWindowState.Minimized)
                        frmMain.ObjDirectSupplierReturnSearch.WindowState = FormWindowState.Normal;
                    frmMain.ObjDirectSupplierReturnSearch.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lblCustomer_Click(object sender, EventArgs e)
        {

        }

        private void ultraPanel1_PaintClient(object sender, PaintEventArgs e)
        {

        }

        private void lblGRN_Click(object sender, EventArgs e)
        {

        }

        private void lblReturn_Click(object sender, EventArgs e)
        {

        }

        private void lblDirectReturn_Click(object sender, EventArgs e)
        {

        }

        private void lblInvoice_Click(object sender, EventArgs e)
        {

        }

        private void ultraLabel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = ultraLabel1.PointToScreen(e.Location);
                cmbPurches.Show(pt);
            }
        }

        private void newPurchesOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMain.objfrmPurchaseOder == null || frmMain.objfrmPurchaseOder.IsDisposed)
            {
                frmMain.objfrmPurchaseOder = new frmPurchaseOder();
            }
            frmMain.objfrmPurchaseOder.WindowState = FormWindowState.Minimized;
            if (frmMain.objfrmPurchaseOder.WindowState == FormWindowState.Minimized)
                frmMain.objfrmPurchaseOder.WindowState = FormWindowState.Normal;
            frmMain.objfrmPurchaseOder.Show();
        }

        private void purchesOrderListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMain.objfrmPurchaseOrderList == null || frmMain.objfrmPurchaseOrderList.IsDisposed)
            {
                frmMain.objfrmPurchaseOrderList = new frmPurchaseOrderList();
            }
            frmMain.objfrmPurchaseOrderList.WindowState = FormWindowState.Minimized;
            if (frmMain.objfrmPurchaseOrderList.WindowState == FormWindowState.Minimized)
                frmMain.objfrmPurchaseOrderList.WindowState = FormWindowState.Normal;
            frmMain.objfrmPurchaseOrderList.Show();
        }

        private void ultraLabel1_Click(object sender, EventArgs e)
        {

        }

        private void lblDirectSupInvoice_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblDirectSupInvoice.PointToScreen(e.Location);
                cmbDirectSupInv.Show(pt);
            }
        }

        private void cmbDirectSupInv_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Direct Suplier Invoice")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDirectSuplierInvoice");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmDirectSupInvoice == null || frmMain.objfrmDirectSupInvoice.IsDisposed)
                    {
                        frmMain.objfrmDirectSupInvoice = new frmDirectSupInvoice();
                    }
                    frmMain.objfrmDirectSupInvoice.WindowState = FormWindowState.Minimized;
                    if (frmMain.objfrmDirectSupInvoice.WindowState == FormWindowState.Minimized)
                        frmMain.objfrmDirectSupInvoice.WindowState = FormWindowState.Normal;
                    frmMain.objfrmDirectSupInvoice.Location = new Point(0, 0);
                    frmMain.objfrmDirectSupInvoice.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (e.ClickedItem.Text == "Direct Suplier Invoice List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDirectSuplierInvoiceList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmDirectSupInvoiceList == null || frmMain.objfrmDirectSupInvoiceList.IsDisposed)
                    {
                        frmMain.objfrmDirectSupInvoiceList = new frmDirectSupInvoiceList();
                    }
                    frmMain.objfrmDirectSupInvoiceList.WindowState = FormWindowState.Minimized;
                    if (frmMain.objfrmDirectSupInvoiceList.WindowState == FormWindowState.Minimized)
                        frmMain.objfrmDirectSupInvoiceList.WindowState = FormWindowState.Normal;
                    frmMain.objfrmDirectSupInvoiceList.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            frmVendorMaster fvm = new frmVendorMaster();
            fvm.Show();
        }
    }
}