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
    public partial class frmMasterCustomerSales : Form
    {
        private string _msgTitle = "Login";      
        bool run = false;
        bool Add = false;
        
        DataTable dtUser = new DataTable();

        public frmMasterCustomerSales()
        {
            InitializeComponent();           
        }            

        private void cmbDelNote_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {            
            if (e.ClickedItem.Text == "Delivery Note")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDeliveryNote");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)//
                {
                    //Connector conn = new Connector();
                    //conn.ImportSalesOrderListD();
                    //conn.InsertSOD();
                    if (frmMain.ObjDeliveryNote == null || frmMain.ObjDeliveryNote.IsDisposed)
                    {
                        frmMain.ObjDeliveryNote = new frmDeliveryNote();
                    }

                    frmMain.ObjDeliveryNote.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjDeliveryNote.WindowState == FormWindowState.Minimized)
                        frmMain.ObjDeliveryNote.WindowState = FormWindowState.Normal;                   

                    frmMain.ObjDeliveryNote.Show();                   

                    if (Add)
                    {
                        frmMain.ObjDeliveryNote.btnNew.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (e.ClickedItem.Text == "Delivery Note List")
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
                    if (frmMain.ObjDNoteList == null || frmMain.ObjDNoteList.IsDisposed)
                    {
                        frmMain.ObjDNoteList = new frmDNoteList();
                    }
                    frmMain.ObjDNoteList.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjDNoteList.WindowState == FormWindowState.Minimized)
                        frmMain.ObjDNoteList.WindowState = FormWindowState.Normal;
                    frmMain.ObjDNoteList.Show();                    
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
                if (user.IsIndrctINVEnbl)
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoicing");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.ObInvoicing == null || frmMain.ObInvoicing.IsDisposed)
                        {
                            frmMain.ObInvoicing = new frmInvoicing();
                        }
                        frmMain.ObInvoicing.WindowState = FormWindowState.Minimized;
                        if (frmMain.ObInvoicing.WindowState == FormWindowState.Minimized)
                            frmMain.ObInvoicing.WindowState = FormWindowState.Normal;
                        
                        frmMain.ObInvoicing.Show();
                        
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (user.IsInclTaxINVEnbl)
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceAR");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.ObInvoicing == null || frmMain.ObInvoicing.IsDisposed)
                        {
                            frmMain.objfrmInvoiceAR = new frmInvoiceAR();
                        }
                        frmMain.objfrmInvoiceAR.WindowState = FormWindowState.Minimized;
                        if (frmMain.objfrmInvoiceAR.WindowState == FormWindowState.Minimized)
                            frmMain.objfrmInvoiceAR.WindowState = FormWindowState.Normal;
                    
                        frmMain.objfrmInvoiceAR.Show();                       
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (user.IsPMINVEnbl)
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceMetrics");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.objfrmInvoiceMetrics == null || frmMain.objfrmInvoiceMetrics.IsDisposed)
                        {
                            frmMain.objfrmInvoiceMetrics = new frmInvoiceMetrics();
                        }
                        frmMain.objfrmInvoiceMetrics.WindowState = FormWindowState.Minimized;
                        if (frmMain.objfrmInvoiceMetrics.WindowState == FormWindowState.Minimized)
                            frmMain.objfrmInvoiceMetrics.WindowState = FormWindowState.Normal;                        
                        frmMain.objfrmInvoiceMetrics.Show();                       
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                
            }
            if (e.ClickedItem.Text == "Invoice List")
            {
                
            //    if (user.IsInclTaxINVEnbl)
            //    {

            //        run = false;
            //        dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceARList");
            //        if (dtUser.Rows.Count > 0)
            //        {
            //            run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //            Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //        }
            //        if (run)
            //        {
            //            if (frmMain.objfrmInvoiceARList == null || frmMain.objfrmInvoiceARList.IsDisposed)
            //            {
            //                frmMain.objfrmInvoiceARList = new frmInvoiceARList();
            //            }

            //            frmMain.objfrmInvoiceAR.WindowState = FormWindowState.Minimized;
            //            if (frmMain.objfrmInvoiceAR.WindowState == FormWindowState.Minimized)
            //                frmMain.objfrmInvoiceAR.WindowState = FormWindowState.Normal;                        

            //            frmMain.objfrmInvoiceARList.Show();                   
                //    }
                //    else
                //    {
                //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    }
                //}
                //else 
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDInvoiceList");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.ObjInvoiceList == null || frmMain.ObjInvoiceList.IsDisposed)
                        {
                            frmMain.ObjInvoiceList = new frmDInvoiceList();
                        }
                        frmMain.ObjInvoiceList.WindowState = FormWindowState.Minimized;
                        if (frmMain.ObjInvoiceList.WindowState == FormWindowState.Minimized)
                            frmMain.ObjInvoiceList.WindowState = FormWindowState.Normal;
                        frmMain.ObjInvoiceList.Show();
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }       

        private void cmdReturn_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {           
            if (e.ClickedItem.Text == "Return Note")
            {               
                if (user.IsIndRtnEnbl)
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmCustomerReturns");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.ObjCusRetern == null || frmMain.ObjCusRetern.IsDisposed)
                        {
                            frmMain.ObjCusRetern = new frmCustomerReturns();
                        }

                        frmMain.ObjCusRetern.WindowState = FormWindowState.Minimized;
                        if (frmMain.ObjCusRetern.WindowState == FormWindowState.Minimized)
                            frmMain.ObjCusRetern.WindowState = FormWindowState.Normal;

                        frmMain.ObjCusRetern.Show();                      
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (user.IsIncluRtnEnbl)
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmCustomerReturns");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.objfrmInvoiceARRtn == null || frmMain.objfrmInvoiceARRtn.IsDisposed)
                        {
                            frmMain.objfrmInvoiceARRtn = new frmInvoiceARRtn();
                        }

                        frmMain.objfrmInvoiceARRtn.WindowState = FormWindowState.Minimized;
                        if (frmMain.objfrmInvoiceARRtn.WindowState == FormWindowState.Minimized)
                            frmMain.objfrmInvoiceARRtn.WindowState = FormWindowState.Normal;

                        frmMain.objfrmInvoiceARRtn.Show();
                        
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            if (e.ClickedItem.Text == "Return Note List")
            {
                if (user.IsDirectRtnEnbl)
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmReturnNotenewSearch");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.objfrmCusReturnList == null || frmMain.objfrmCusReturnList.IsDisposed)
                        {
                            frmMain.objfrmCusReturnList = new frmCusReturnList();
                        }

                        frmMain.objfrmCusReturnList.WindowState = FormWindowState.Minimized;
                        if (frmMain.objfrmCusReturnList.WindowState == FormWindowState.Minimized)
                            frmMain.objfrmCusReturnList.WindowState = FormWindowState.Normal;

                        frmMain.objfrmCusReturnList.Show();
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (user.IsIndRtnEnbl)
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmCusReturnList");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.objfrmCusReturnList == null || frmMain.objfrmCusReturnList.IsDisposed)
                        {
                            frmMain.objfrmCusReturnList = new frmCusReturnList();
                        }
                        frmMain.objfrmCusReturnList.WindowState = FormWindowState.Minimized;
                        if (frmMain.objfrmCusReturnList.WindowState == FormWindowState.Minimized)
                            frmMain.objfrmCusReturnList.WindowState = FormWindowState.Normal; 

                        frmMain.objfrmCusReturnList.Show();
                        
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (user.IsIncluRtnEnbl)
                {
                    run = false;
                    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceARRtnList");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                    }
                    if (run)
                    {
                        if (frmMain.objfrmInvoiceARRtnList == null || frmMain.objfrmInvoiceARRtnList.IsDisposed)
                        {
                            frmMain.objfrmInvoiceARRtnList = new frmInvoiceARRtnList();
                        }
                        frmMain.objfrmInvoiceARRtnList.WindowState = FormWindowState.Minimized;
                        if (frmMain.objfrmInvoiceARRtnList.WindowState == FormWindowState.Minimized)
                            frmMain.objfrmInvoiceARRtnList.WindowState = FormWindowState.Normal; 

                        frmMain.objfrmInvoiceARRtnList.Show();
                        
                    }
                    else
                    {
                        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void cmbDirRetun_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Direct Return Note")
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmCustomerReturns");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjDirectR == null || frmMain.ObjDirectR.IsDisposed)
                    {
                        frmMain.ObjDirectR = new frmReturnNotenew();
                    }
                    frmMain.ObjDirectR.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjDirectR.WindowState == FormWindowState.Minimized)
                        frmMain.ObjDirectR.WindowState = FormWindowState.Normal;

                    frmMain.ObjDirectR.Show();
                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (e.ClickedItem.Text == "Direct Return List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmReturnNotenewSearch");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmReturnNotenewSearch == null || frmMain.objfrmReturnNotenewSearch.IsDisposed)
                    {
                        frmMain.objfrmReturnNotenewSearch = new frmReturnNotenewSearch();
                    }
                    frmMain.objfrmReturnNotenewSearch.WindowState = FormWindowState.Minimized;
                    if (frmMain.objfrmReturnNotenewSearch.WindowState == FormWindowState.Minimized)
                        frmMain.objfrmReturnNotenewSearch.WindowState = FormWindowState.Normal; 

                    frmMain.objfrmReturnNotenewSearch.Show();
                 
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbDirInvoice_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Invoice")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoices");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjInvoices == null || frmMain.ObjInvoices.IsDisposed)
                    {
                        frmMain.ObjInvoices = new frmInvoices(0);
                    }
                    frmMain.ObjInvoices.WindowState = FormWindowState.Minimized;
                    if (frmMain.ObjInvoices.WindowState == FormWindowState.Minimized)
                        frmMain.ObjInvoices.WindowState = FormWindowState.Normal;
                   
                    frmMain.ObjInvoices.Show();                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Invoice List")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDInvoiceList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmInvoiceSearch == null || frmMain.objfrmInvoiceSearch.IsDisposed)
                    {
                        frmMain.objfrmInvoiceSearch = new frmInvoiceSearch ();
                    }
                    frmMain.objfrmInvoiceSearch.WindowState = FormWindowState.Minimized;
                    if (frmMain.objfrmInvoiceSearch.WindowState == FormWindowState.Minimized)
                        frmMain.objfrmInvoiceSearch.WindowState = FormWindowState.Maximized;

                 

                    frmMain.objfrmInvoiceSearch.Show();                   
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }        

        private void cmbCustomer_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Import Customer")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportCustomer");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.ObjImpCus == null || frmMain.ObjImpCus.IsDisposed)
                    {
                        frmMain.ObjImpCus = new frmImportCustomer();
                    }
                    frmMain.ObjImpCus.Show();                   
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if(e.ClickedItem.Text == "Create Customer")
            {
                frmCustomerMaster cm = new frmCustomerMaster();
                cm.Show();
            }
        }

        private void cmbReports_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Daily Cash Sales")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceListPrint");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.objfrmInvoiceListPrint == null || frmMain.objfrmInvoiceListPrint.IsDisposed)
                        frmMain.objfrmInvoiceListPrint = new frmInvoiceListPrint();

                    frmMain.objfrmInvoiceListPrint.WindowState = FormWindowState.Minimized;
                    if (frmMain.objfrmInvoiceListPrint.WindowState == FormWindowState.Minimized)
                        frmMain.objfrmInvoiceListPrint.WindowState = FormWindowState.Normal;

                    frmMain.objfrmInvoiceListPrint.Show();                 
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }            
        }

        private void lblCustomer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblCustomer.PointToScreen(e.Location);
                cmbCustomer.Show(pt);
            }
        }

        private void lblGRN_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblGRN.PointToScreen(e.Location);
                cmbDelNoteCS.Show(pt);
            }
        }

        private void lblInvoice_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblInvoice.PointToScreen(e.Location);
                cmbInvoiceCS.Show(pt);
            }
        }

        private void lblDirInv_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblDirInv.PointToScreen(e.Location);
                cmbDirInvoiceCS.Show(pt);
            }
        }

        private void lblReturn_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblReturn.PointToScreen(e.Location);
                cmdReturnCS.Show(pt);
            }
        }

        private void lblDirReturn_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblDirReturn.PointToScreen(e.Location);
                cmbDirRetunCS.Show(pt);
            }
        }

        private void lblReports_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblReports.PointToScreen(e.Location);
                cmbReportsCS.Show(pt);
            }
        }

        private void ultraPanel1_PaintClient(object sender, PaintEventArgs e)
        {

        }

        private void lblInvoice_Click(object sender, EventArgs e)
        {

        }

        private void lblSalesOrder_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblSalesOrder.PointToScreen(e.Location);
                cmbSaleOrder.Show(pt);
            }
        }

        private void cmbSaleOrder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "Sales Order")
            //{
            //    frmOrder ObjOrder = new frmOrder();
            //    ObjOrder.Show();
            //}
            //else if (e.ClickedItem.Text == "Sales Order List")
            //{
            //    frmOrderSearch ObjOrdersearch = new frmOrderSearch();
            //    ObjOrdersearch.Show();
            //}
            if (e.ClickedItem.Text == "Sales Order")
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmOrder");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (frmMain.objsalesorder == null || frmMain.objsalesorder.IsDisposed)
                        frmMain.objsalesorder = new frmOrder();

                    frmMain.objsalesorder.WindowState = FormWindowState.Minimized;
                    if (frmMain.objsalesorder.WindowState == FormWindowState.Minimized)
                        frmMain.objsalesorder.WindowState = FormWindowState.Normal;

                    frmMain.objsalesorder.Show();
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

        private void lblReturn_Click(object sender, EventArgs e)
        {

        }

        private void lblSalesOrder_Click(object sender, EventArgs e)
        {

        }

        private void lblDirReturn_Click(object sender, EventArgs e)
        {

        }

        private void cmdReturnCS_Opening(object sender, CancelEventArgs e)
        {

        }

        private void cmdReturnCS_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void cmbjob_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            frmjobNote ObjOrder = new frmjobNote();
            ObjOrder.Show();
        }

        private void lblGRN_Click(object sender, EventArgs e)
        {

        }

        private void lblJob_Entry_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblJob_Entry.PointToScreen(e.Location);
                cmbjob.Show(pt);
            }
        }

        private void lblJob_Entry_Click(object sender, EventArgs e)
        {

        }

        private void lbl_po_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lbl_po.PointToScreen(e.Location);
                cmbpo.Show(pt);
            }
        }

        private void cmbpo_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Quotation")
            {
                frmpurchesorder ObjPOOrder = new frmpurchesorder();
                ObjPOOrder.Show();
            }
            else
            {
                frmPoOrderSearch objPOOS = new frmPoOrderSearch();
                objPOOS.Show();
            }
        }

        private void lblDirInv_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void cmbDirInvoiceCS_Opening(object sender, CancelEventArgs e)
        {

        }

        private void cmbDirRetunCS_Opening(object sender, CancelEventArgs e)
        {

        }

        private void lbl_po_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {

        }

        private void frmMasterCustomerSales_Load(object sender, EventArgs e)
        {

        }
    }
}