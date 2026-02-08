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
using Interop.PeachwServer;
using UserAutherization;
using System.Deployment;



namespace UserAutherization
{
    public partial class frmMain : Form
    {
        public static string ItemMasterFormName;
        public static string CostomizeFormName;
        clsCommon objclsCommon = new clsCommon();
        public static string sName = "Tom Aligood";
        public static string sPassword = "3M3336RJP111X7A";
        DataTable dtUser = new DataTable();
        public bool IsRestart = false;
        bool run = false;
        bool Add = false;
        public string StrSql;

        public static frmMasterJobs objfrmMasterJobs;
        public static frmInventoryAdjustmentList objfrmInventoryAdjustmentList;
        public static frmSeachIssueNote objfrmSeachIssueNote;
        public static frmSeachTrans objfrmSeachTrans;
        public static frmSettingsTax objfrmSettingsTax;
        public static frmSettings objfrmSettings;
        public static frmSettingsAccounts objfrmSettingsAccounts;
        public static frmSettingsOther objfrmSettingsOther;
        public static frmSystem objfrmSystem;

        public static frmMasterMainPage ObjfrmMasterMainPage;
        public static frmMasterInventory objfrmMasterInventory;
        public static frmMasterVendorPurchase objfrmMasterVendorPurchase;
        public static frmMasterCustomerSales objfrmMasterCustomerSales;

        public static frmImportFromPeachTree objfrmImportFromPeachTree;
        public static frmLocation objfrmLocation;
        public static frmPhases objfrmPhases;
        public static frmSubPhases objfrmSubPhases;
        public static frmBOQ objfrmBOQ;
        public static frmBOM objfrmBOM;
        public static frmSiteIssues objfrmSiteIssues;
        public static frmSiteIssuesReturn objfrmSiteIssuesReturn;
        public static frmDeleteRecords objfrmDeleteRecords;
        public static frmPriceInquiry objfrmPriceInquiry;
        public static frmReturnNotenewSearch objfrmReturnNotenewSearch;
        public static frmInvoiceSearch objfrmInvoiceSearch;
        public static frmInsertToHeadoffice objfrmInsertToHeadoffice;
        public static frmImportHeadToBranch objfrmImportHeadToBranch;
        public static frmBranchToHeadOfficecs objfrmBranchToHeadOfficecs;
        public static frmMatrixDesign objfrmMatrixDesign;
        public static frmInvoiceMetrics objfrmInvoiceMetrics;
        public static frmInvoiceListPrint objfrmInvoiceListPrint;
        public static frmJob objfrmJob;
        public static frmInvoiceAR objfrmInvoiceAR;
        public static frmViewUsers objfrmViewUsers;
        public static frmCusReturnList objfrmCusReturnList;
        public static frmInventotyAdjustment objfrmInventotyAdjustment;
        public static frmBeginingBalances objfrmBeginingBalances;
        public static frmImportItemMaster ObjImpItem;
        public static frmWareHouseTrans ObjWareHouseTransfer;
        public static frmIssueNote objfrmIssueNote;
        public static frmInventoryMovement objfrmInventoryMovement;
        public static frmTransNoteList objfrmTransNoteList;
        public static frmIssueNoteLIstPrint objfrmIssueNoteLIstPrint;
        public static frmadustmentsListPrint objfrmadustmentsListPrint;
        public static frmInvoices ObjInvoices;
        public static frmInvoicing ObInvoicing;
        public static frmImportVendor ObjImpVendor;
        public static frmGRN ObjGRN;
        public static frmDeliveryNoteList ObjGRNList;
        public static frmSupInvoice ObjSupInvoice;
        public static frmSupInvoiceList ObjSupInvoicelist;
        public static frmSupplierReturn ObjSupplierRetern;
        public static frmDirectSupplierReturn ObjDirectSupplierReturn;
        public static frmDirectSupplierReturnSearch ObjDirectSupplierReturnSearch;
        public static frmSupReturnList ObjSupReturnlist;
        public static frmInvoiceARRtn objfrmInvoiceARRtn;
        public static frmInvoiceARRtnList objfrmInvoiceARRtnList;
        public static frmInvoiceARList objfrmInvoiceARList;
        public static frmValuation objfrmValuation;
        public static frmQtyOnHand objfrmQtyOnHand;
        public static frmWareHouse objfrmWareHouse;
        public static frmImportCustomer ObjImpCus;
        public static frmDeliveryNote ObjDeliveryNote;
        public static frmDeliveryNoteList ObjDeliveyNoteList;
        public static frmDInvoiceList ObjInvoiceList;
        public static frmCustomerReturns ObjCusRetern;
        public static frmJobEstimate objJobEstimate;
        public static frmJobActual objJobActual;
        public static frmJobReturn objJobReturn;
        public static frmJobStatus objJobStatus;
        public static frmSupInvList objfrmSupInvList;
        public static frmFinishGoodTransfer objFinishedGoodTrans;
        public static frmItemReport objItemReport;
        public static frmActivities objfrmActivities;
        public static frmReturnNotenew ObjDirectR;
        public static FrmPurchaseOrderVarification ObjFrmPurchaseOrderVarification;
        public static frmGRNList objfrmGRNList;
        public static frmDNoteList ObjDNoteList;
        public static frmDefaultSettings ObjDefaultseting;
        public static frmAddUser ObjAddUser;
        public static frmFind objfrmFind;
        public static frmOrder objsalesorder;
        public static frmjobNote objjobnote;
        public static frmjoblist objjoblist;

        public static frmDirectSupInvoice objfrmDirectSupInvoice;
        public static frmDirectSupInvoiceList objfrmDirectSupInvoiceList;

        public static frmPurchaseOder objfrmPurchaseOder;
        public static frmPurchaseOrderList objfrmPurchaseOrderList;
        public static frmFinalInvoiceSearch objfrmFinalInvoiceSearch;
        public static frmItemMaster objfrmItemMaster;
        public static frmItemMasterList objfrmItemMasterList;
        public static string ConnectionString;
        //public frmMain()
        //{
        //    InitializeComponent();
        //    setConnectionString();         
        //}

        public frmMain()
        {
            InitializeComponent();
            setConnectionString();
            //objfrmJob = new frmJob();
            //objfrmMasterVendorPurchase = new frmMasterVendorPurchase();
            //ObjDirectR = new frmReturnNotenew();
            //ObjDNoteList = new frmDNoteList();
            //ObjDefaultseting = new frmDefaultSettings();
            //objItemReport = new frmItemReport();
            //objFinishedGoodTrans = new frmFinishGoodTransfer(0);
            //objJobStatus = new frmJobStatus();
            //objJobReturn = new frmJobReturn(0);
            //objJobActual = new frmJobActual(0);
            //objJobEstimate = new frmJobEstimate(0);
            //ObjImpItem = new frmImportItemMaster();
            //objfrmInsertToHeadoffice = new frmInsertToHeadoffice();
            //objfrmImportHeadToBranch = new frmImportHeadToBranch();
            //objfrmBranchToHeadOfficecs = new frmBranchToHeadOfficecs();
            //objfrmSeachTrans = new frmSeachTrans();
            //ObjInvoiceList = new frmDInvoiceList();
            //objfrmInvoiceSearch = new frmInvoiceSearch();
            //ObjDeliveyNoteList = new frmDeliveryNoteList();
            //objfrmSettingsTax = new frmSettingsTax();
            //objfrmSettings = new frmSettings();
            //objfrmSettingsAccounts = new frmSettingsAccounts();
            //objfrmSettingsOther = new frmSettingsOther();
            //ObjDeliveryNote = new frmDeliveryNote();
            //objfrmImportFromPeachTree = new frmImportFromPeachTree();
            //objfrmLocation = new frmLocation();
            //objfrmPhases = new frmPhases();
            //objfrmSubPhases = new frmSubPhases();
            //objfrmBOM = new frmBOM();
            //objfrmBOQ = new frmBOQ();
            //objfrmActivities = new frmActivities();
            //ObjImpVendor = new frmImportVendor();
            //ObjGRN = new frmGRN();
            //ObjSupInvoice = new frmSupInvoice();
            //ObjSupplierRetern = new frmSupplierReturn();
            //ObjGRNList = new frmDeliveryNoteList();
            //ObjSupInvoicelist = new frmSupInvoiceList();
            //ObjSupReturnlist = new frmSupReturnList();
            //ObjImpCus = new frmImportCustomer();
            //ObjInvoices = new frmInvoices(0);
            //ObInvoicing = new frmInvoicing();
            //objfrmInvoiceAR = new frmInvoiceAR();
            //ObjCusRetern = new frmCustomerReturns();
            //objfrmInvoiceARRtn = new frmInvoiceARRtn();
            //objfrmCusReturnList = new frmCusReturnList();
            //objfrmReturnNotenewSearch = new frmReturnNotenewSearch();
            //objfrmInvoiceARRtnList = new frmInvoiceARRtnList();
            //objfrmInvoiceARList = new frmInvoiceARList();
            //objfrmMatrixDesign = new frmMatrixDesign();
            //objfrmBeginingBalances = new frmBeginingBalances();
            //ObjWareHouseTransfer = new frmWareHouseTrans(0);
            //objfrmInventotyAdjustment = new frmInventotyAdjustment();
            //objfrmPriceInquiry = new frmPriceInquiry();
            //ObjFrmPurchaseOrderVarification = new FrmPurchaseOrderVarification();
            //objfrmGRNList = new frmGRNList();
            //objfrmSupInvList = new frmSupInvList();
            //objfrmInvoiceListPrint = new frmInvoiceListPrint();
            //objfrmValuation = new frmValuation();
            //objfrmInventoryMovement = new frmInventoryMovement();
            //objfrmTransNoteList = new frmTransNoteList();
            //objfrmWareHouse = new frmWareHouse();
            //objfrmDeleteRecords = new frmDeleteRecords();
            //ObjAddUser = new frmAddUser();
            //objfrmMasterCustomerSales = new frmMasterCustomerSales();
            //objfrmSystem = new frmSystem();
            //objfrmSiteIssues = new frmSiteIssues();
            //objfrmSiteIssuesReturn = new frmSiteIssuesReturn();
            //objfrmIssueNote = new frmIssueNote(0);
        }
        // Connector conn = new Connector();

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                //objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);

            }
        }


        private void itemMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmImportItemMaster impitem = new frmImportItemMaster();
                impitem.Show();
            }
            catch { }

            // conn.ImportItem_List();
            // conn.fillID_Item_list();
        }



        private void newDispatchOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObjGRN == null || ObjGRN.IsDisposed)
                        ObjGRN = new frmGRN();

                    //frmGRN abc = new frmGRN();
                    ObjGRN.Show();
                    ObjGRN.WindowState = FormWindowState.Normal;


                    if (Add)
                    {
                        ObjGRN.btnNew.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void GetCampanyInformation()
        {
            try
            {
                StrSql = "SELECT CompanyName,IsManufactureCompany,Report_Path FROM tblCompanyInformation";
                SqlCommand command = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                clsPara clspara = new clsPara();
                if (dt.Rows.Count > 0)
                {
                    clsPara.StrComName = dt.Rows[0].ItemArray[0].ToString();
                    this.Text = dt.Rows[0].ItemArray[0].ToString();
                    clsPara.BlnManufactureCompany = Boolean.Parse(dt.Rows[0].ItemArray[1].ToString());
                    clsPara.StrRepFolder = dt.Rows[0].ItemArray[2].ToString().Trim();
                }
                else
                {
                    this.Text = "";
                    clsPara.StrComName = "";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {



                //if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                //    appVersion = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;

                //lblProjectName.Text = string.Format("SOFTWAVE : PERFEC BILLING ::: Version: {0}.{1}.{2}.{3}", appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);


                GetCampanyInformation();//issueAndReturnToolStripMenuItem

                if (user.IsInvIssueReturnEnbl) issueAndReturnToolStripMenuItem1.Visible = true;
                else issueAndReturnToolStripMenuItem1.Visible = false;

                if (user.IsJobPreFormsEnbl)
                //{
                //    jobToolStripMenuItem.Visible = true;
                //    jobToolStripMenuItem1.Visible = true;
                //}

                //else
                //{
                //    jobToolStripMenuItem.Visible = false;
                //    jobToolStripMenuItem1.Visible = false;
                //}

                if (user.IsInvAdjEnbl) inventoryAdjustmentToolStripMenuItem1.Visible = true;
                else inventoryAdjustmentToolStripMenuItem1.Visible = false;

                if (user.IsWHTransEnbl) transferNoteToolStripMenuItem.Visible = true;
                else transferNoteToolStripMenuItem.Visible = false;

                if (user.IsInvBegBalEnbl) openingBalanceToolStripMenuItem.Visible = true;
                else openingBalanceToolStripMenuItem.Visible = false;

                if (user.IsDelNoteEnbl) deliveryNoteToolStripMenuItem.Visible = true;
                else deliveryNoteToolStripMenuItem.Visible = false;

                if (user.IsSupRetEnbl) aupplyReturnsToolStripMenuItem.Visible = true;
                else aupplyReturnsToolStripMenuItem.Visible = false;

                if (user.IsSupInvEnbl) supplyInvoiceToolStripMenuItem.Visible = true;
                else supplyInvoiceToolStripMenuItem.Visible = false;

                if (user.IsGRNEnbl) goodReceivedNoteToolStripMenuItem.Visible = true;
                else goodReceivedNoteToolStripMenuItem.Visible = false;

                if (user.IsDirectINVEnbl)
                {
                    //customerReturnsToolStripMenuItem.Visible = true;
                    tsmiCustomerInvDirect.Visible = true;
                    tsmiviewInvoiceDirect.Visible = true;
                }
                else
                {
                    //customerReturnsToolStripMenuItem.Visible = false;
                    tsmiCustomerInvDirect.Visible = false;
                    tsmiviewInvoiceDirect.Visible = false;
                }

                if (user.IsInclTaxINVEnbl)
                {
                    //customerReturnsToolStripMenuItem.Visible = false;
                    invoiceTMToolStripMenuItem.Visible = true;
                    viewInvoiceToolStripMenuItemTM.Visible = true;
                }
                else
                {
                    invoiceTMToolStripMenuItem.Visible = false;
                    viewInvoiceToolStripMenuItemTM.Visible = false;
                }

                if (user.IsIndrctINVEnbl)
                {
                    //customerReturnsToolStripMenuItem.Visible = true;
                    directInvoiceKKKToolStripMenuItem.Visible = true;
                    tsmiviewInvoicesIndirect.Visible = true;
                }
                else
                {
                    //customerReturnsToolStripMenuItem.Visible = false;
                    directInvoiceKKKToolStripMenuItem.Visible = false;
                    tsmiviewInvoicesIndirect.Visible = false;
                }

                if (user.IsPMINVEnbl)
                {
                    //customerReturnsToolStripMenuItem.Visible = true;
                    priceMatrixDesigningToolStripMenuItem.Visible = true;
                    priceListToolStripMenuItem.Visible = true;
                    priceMatrixDesigningToolStripMenuItem.Visible = true;
                }
                else
                {
                    //customerReturnsToolStripMenuItem.Visible = false; 
                    priceMatrixDesigningToolStripMenuItem.Visible = false;
                    priceListToolStripMenuItem.Visible = false;
                    priceMatrixDesigningToolStripMenuItem.Visible = false;
                }

                if (user.IsDirectRtnEnbl)
                {
                    customerReturnsToolStripMenuItem.Visible = true;
                    applyToRToolStripMenuItem.Visible = true;
                    viewCustomerReturnsDirectToolStripMenuItem.Visible = true;
                }
                else
                {
                    customerReturnsToolStripMenuItem.Visible = false;
                    applyToRToolStripMenuItem.Visible = false;
                    viewCustomerReturnsDirectToolStripMenuItem.Visible = false;
                }

                if (user.IsIndRtnEnbl)
                {
                    customerReturnsToolStripMenuItem.Visible = true;
                    applyToInvoicesToolStripMenuItem1.Visible = true;
                    viewCustomerReturnsToolStripMenuItem.Visible = true;
                }
                else
                {
                    customerReturnsToolStripMenuItem.Visible = false;
                    applyToInvoicesToolStripMenuItem1.Visible = false;
                    viewCustomerReturnsToolStripMenuItem.Visible = false;
                }

                if (user.IsIncluRtnEnbl)
                {
                    customerReturnsToolStripMenuItem.Visible = false;
                    cRNTMToolStripMenuItem.Visible = true;
                    viewCustomerReturnsToolStripMenuItemTM.Visible = true;
                }
                else
                {
                    cRNTMToolStripMenuItem.Visible = false;
                    viewCustomerReturnsToolStripMenuItemTM.Visible = false;
                }

                //Connect ptapp = new Connect();
                //this.Text = ptapp.app.CurrentCompanyName;
                //btnSys_Click(sender, e);
                btnCS_Click(sender, e);
                btnSys_Click(sender, e);
                ultraLabel1_Click(sender, e);
                ultraLabel3_Click(sender, e);
                ultraLabel4_Click(sender, e);
                //btnCS_Click(sender, e);
                uLblMainPage_Click(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void salesOrderListToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "FrmSalesOrderVarification");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    FrmSalesOrderVarification svr = new FrmSalesOrderVarification();
                    svr.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void newGRNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoicing");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (ObInvoicing == null || ObInvoicing.IsDisposed)
                        ObInvoicing = new frmInvoicing();

                    ObInvoicing.Show();

                    if (ObInvoicing.WindowState == FormWindowState.Minimized)
                        ObInvoicing.WindowState = FormWindowState.Normal;

                    if (Add)
                    {
                        ObInvoicing.btnNew.Enabled = true;
                    }

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }


            // frmNewInvoice newinvo = new frmNewInvoice();
            //newinvo.Show();

            //  frmDespatch despatch = new frmDespatch();
            //  despatch.Show();
        }

        private void invoiceListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDInvoiceList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmDInvoiceList involist = new frmDInvoiceList();
                    involist.Show();
                    //  frmInvoiceList invList = new frmInvoiceList();
                    // invList.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void itemMasterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmItem");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmItem item = new frmItem();
                    //item.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }

        }

        private void newInvoiceForDeliveryNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ObInvoicing == null || ObInvoicing.IsDisposed)
                ObInvoicing = new frmInvoicing();
            ObInvoicing.Show();
            //ObInvoicing.TopMost = true;
            ObInvoicing.WindowState = FormWindowState.Normal;
        }

        private void connectToPeachtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Connector conn = new Connector();
            MessageBox.Show("Peachtree is now running.");
        }

        private void closePeachtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Connector conn = new Connector();
            Connect ptApp = new Connect();

            // conn.app.ExecuteCommand("File|Exit", null);
            ptApp.app.ExecuteCommand("File|Exit", null);
        }

        private void byGUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmOpenCompany");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {

                    frmOpenCompany openComp = new frmOpenCompany();
                    // openComp.MdiParent = this;
                    openComp.Show();
                    openComp.withGUID = true;
                    openComp.CompanyList(openComp.withGUID);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }

        }

        public void GetcompanyName()
        {
            //string CompName = "";
            Connect ptApp = new Connect();
            // CompName = ptApp.app.CurrentCompanyName;
            this.Text = ptApp.app.CurrentCompanyName;
        }

        private void byNameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmOpenCompany");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {

                    frmOpenCompany openComp = new frmOpenCompany();
                    // openComp.MdiParent = this;
                    openComp.Show();
                    openComp.withGUID = false;
                    openComp.CompanyList(openComp.withGUID);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void openPreviousCompanyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Connect myApp = new Connect();
            myApp.app.OpenPreviousCompany();
            myApp = null;
        }

        private void closeCompanyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Interop.PeachwServer.Application app;
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");
                app.CloseCompany();
            }
            catch { }

        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            try
            {
                Interop.PeachwServer.Application app;
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");
                app.CloseCompany();
            }
            catch { }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void invoiceListToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                frmInvoiceList invlist1 = new frmInvoiceList();
                invlist1.Show();
            }
            catch { }
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {

            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportItemMaster");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmImportItemMaster impitem = new frmImportItemMaster();
                    impitem.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }

        }

        private void importChartOfAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmChartofAccount");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmChartofAccount impch = new frmChartofAccount();
                    impch.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }

        }

        private void inventoryListToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmItemListForRp");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmItemListForRp itemS = new frmItemListForRp();
                    //itemS.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void warehousewiseItemListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmItemWhseSearch");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmItemWhseSearch search = new frmItemWhseSearch();
                    //search.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void assigningItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmItemAsign");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmItemAsign ItemAssig = new frmItemAsign();
                    //ItemAssig.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }

        }

        private void warehousewiseItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmWhseWiseItemRp");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmWhseWiseItemRp witem = new frmWhseWiseItemRp();
                    //witem.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void defualtNumberSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDefaultSetting");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmDefaultSettings defoset = new frmDefaultSettings();
                    defoset.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }

        }

        private void deleteRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDeleteRecords deletR = new frmDeleteRecords();
            deletR.Show();
        }

        private void btnImporVendor_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportVendor");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmImportVendor impv = new frmImportVendor();
                    impv.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void BtnNewGRN_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmGRN");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    Connector conn = new Connector();
                    conn.ImportPurchaseOrderList();
                    conn.Insert_PurchaseOrderList();
                    frmGRN abc = new frmGRN();
                    abc.Show();
                    abc.Enable();
                    if (Add)
                    {
                        abc.btnNew.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnViewGRN_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupplierReturn");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjSupplierRetern == null || ObjSupplierRetern.IsDisposed)
                        ObjSupplierRetern = new frmSupplierReturn();

                    ObjSupplierRetern.Show();
                    ObjSupplierRetern.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmCustomerReturns");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjCusRetern == null || ObjCusRetern.IsDisposed)
                        ObjCusRetern = new frmCustomerReturns();

                    ObjCusRetern.Show();
                    ObjCusRetern.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {

            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoicing");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (ObInvoicing == null || ObInvoicing.IsDisposed)
                        ObInvoicing = new frmInvoicing();
                    ObInvoicing.Show();
                    //ObInvoicing.TopMost = true;
                    ObInvoicing.WindowState = FormWindowState.Normal;
                    if (Add)
                    {
                        ObInvoicing.btnNew.Enabled = true;
                    }

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnViewInvopice_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDInvoiceList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmDInvoiceList involist = new frmDInvoiceList();
                    involist.Show();
                    //  frmInvoiceList invList = new frmInvoiceList();
                    // invList.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnImportInventory_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportItemMaster");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmImportItemMaster impitem = new frmImportItemMaster();
                    impitem.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnAssiginingItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmItemAsign");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmItemAsign ItemAssig = new frmItemAsign();
                    //ItemAssig.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnOpeningBalance_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmOpeningBal");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmOpeningBal bal = new frmOpeningBal();
                    //bal.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnUserManager_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmAddUser");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmAddUser adduser = new frmAddUser();
                    adduser.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnImportAccount_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmChartofAccount");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmChartofAccount impch = new frmChartofAccount();
                    impch.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnMasterFile_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmWareHouse");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmWareHouse wh = new frmWareHouse();
                    //wh.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnDefualtSetting_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDefaultSetting");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmDefaultSettings defoset = new frmDefaultSettings();
                    defoset.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnRecordDelete_Click(object sender, EventArgs e)
        {

            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDeleteRecords");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmDeleteRecords deletR = new frmDeleteRecords();
                    deletR.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnItemMastr_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmItem");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmItem item = new frmItem();
                    //item.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Do you want to Restart Distribution System  ?",
                                    "Perfect Distribution System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reply == DialogResult.Yes)
            {
                IsRestart = true;
                System.Windows.Forms.Application.Restart();
            }
            else if (reply == DialogResult.No)
            {
                return;
            }
        }

        private void tmnuSupInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupInvoice");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjSupInvoice == null || ObjSupInvoice.IsDisposed)
                        ObjSupInvoice = new frmSupInvoice();

                    //frmSupInvoice item = new frmSupInvoice();
                    ObjSupInvoice.Show();
                    ObjSupInvoice.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void tmnuSupplierReturn_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupplierReturn");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjSupplierRetern == null || ObjSupplierRetern.IsDisposed)
                        ObjSupplierRetern = new frmSupplierReturn();

                    ObjSupplierRetern.Show();
                    ObjSupplierRetern.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void customerReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmCustomerReturns");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjCusRetern == null || ObjCusRetern.IsDisposed)
                        ObjCusRetern = new frmCustomerReturns();

                    ObjCusRetern.Show();
                    ObjCusRetern.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void masterSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool run = false;
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDefaultSettingsTabs");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {

                    // frmDefaultSetting 
                    frmDefaultSettings aad = new frmDefaultSettings();
                    aad.Show();

                    //frmDefaultSettingTabs defaultSet = new frmDefaultSettingTabs();
                    //defaultSet.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void registrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Registration2 r2 = new Registration2();
            r2.Show();
        }

        private void purchaseOrderVarificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPurchaseOrderVarification povry = new FrmPurchaseOrderVarification();
            povry.Show();
        }

        private void btnsupplyReturn_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupplierReturn");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjSupplierRetern == null || ObjSupplierRetern.IsDisposed)
                        ObjSupplierRetern = new frmSupplierReturn();

                    ObjSupplierRetern.Show();
                    ObjSupplierRetern.WindowState = FormWindowState.Normal;
                    ObjSupplierRetern.Enable();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void btnsupplyInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupInvoice");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjSupInvoice == null || ObjSupInvoice.IsDisposed)
                        ObjSupInvoice = new frmSupInvoice();

                    ObjSupInvoice.Show();
                    ObjSupInvoice.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void setToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmsetDecimalPoint deci = new frmsetDecimalPoint();
            deci.Show();
        }

        private void btnImportInventory_Click_1(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportItemMaster");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmImportItemMaster impitem = new frmImportItemMaster();
                    impitem.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void btnOpeningBalance_Click_1(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmOpeningBal");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmOpeningBal bal = new frmOpeningBal();
                    //bal.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void importVendorLIstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportVendor");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjImpVendor == null || ObjImpVendor.IsDisposed)
                    {
                        ObjImpVendor = new frmImportVendor();
                    }
                    ObjImpVendor.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void goodReceivedNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    Connector conn = new Connector();
                    conn.ImportPurchaseOrderList();
                    conn.Insert_PurchaseOrderList();

                    if (ObjGRN == null || ObjGRN.IsDisposed)
                    {
                        ObjGRN = new frmGRN();
                    }
                    ObjGRN.WindowState = FormWindowState.Minimized;
                    if (ObjGRN.WindowState == FormWindowState.Minimized)
                        ObjGRN.WindowState = FormWindowState.Normal;

                    ObjGRN.Show();
                    if (Add)
                    {
                        ObjGRN.btnNew.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        private void supplyInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupInvoice");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjSupInvoice == null || ObjSupInvoice.IsDisposed)
                    {
                        ObjSupInvoice = new frmSupInvoice();
                    }

                    ObjSupInvoice.WindowState = FormWindowState.Minimized;
                    if (ObjSupInvoice.WindowState == FormWindowState.Minimized)
                        ObjSupInvoice.WindowState = FormWindowState.Normal;
                    ObjSupInvoice.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void aupplyReturnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupplierReturn");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjSupplierRetern == null || ObjSupplierRetern.IsDisposed)
                    {
                        ObjSupplierRetern = new frmSupplierReturn();
                    }
                    ObjSupplierRetern.WindowState = FormWindowState.Minimized;
                    if (ObjSupplierRetern.WindowState == FormWindowState.Minimized)
                        ObjSupplierRetern.WindowState = FormWindowState.Normal;
                    ObjSupplierRetern.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void deliveryNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    Connector conn = new Connector();
                    conn.ImportSalesOrderListD();
                    conn.InsertSOD();
                    if (ObjDeliveryNote == null || ObjDeliveryNote.IsDisposed)
                    {
                        ObjDeliveryNote = new frmDeliveryNote();
                    }

                    ObjDeliveryNote.WindowState = FormWindowState.Minimized;
                    if (ObjDeliveryNote.WindowState == FormWindowState.Minimized)
                        ObjDeliveryNote.WindowState = FormWindowState.Normal;

                    ObjDeliveryNote.Show();

                    if (Add)
                    {
                        ObjDeliveryNote.btnNew.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmInvoicing_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObInvoicing == null || ObInvoicing.IsDisposed)
                    {
                        ObInvoicing = new frmInvoicing();
                    }
                    ObInvoicing.WindowState = FormWindowState.Minimized;
                    if (ObInvoicing.WindowState == FormWindowState.Minimized)
                        ObInvoicing.WindowState = FormWindowState.Normal;
                    else
                    {
                        if (!ObInvoicing.TopMost)
                            ObInvoicing.TopLevel = true;
                    }
                    ObInvoicing.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void importItemMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportItemMaster");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //    }
            //    if (run)
            //    {
            //        if (ObjImpItem == null || ObjImpItem.IsDisposed)
            //        {
            //            ObjImpItem = new frmImportItemMaster();
            //        }
            //        ObjImpItem.WindowState = FormWindowState.Minimized;
            //        if (ObjImpItem.WindowState == FormWindowState.Minimized)
            //            ObjImpItem.WindowState = FormWindowState.Normal;
            //        ObjImpItem.Show();

            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }

            //}
            //catch { }
        }

        private void openingBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;

                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmBeginingBalances");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmBeginingBalances == null || objfrmBeginingBalances.IsDisposed)
                    {
                        objfrmBeginingBalances = new frmBeginingBalances();
                    }
                    objfrmBeginingBalances.WindowState = FormWindowState.Minimized;
                    if (objfrmBeginingBalances.WindowState == FormWindowState.Minimized)
                        objfrmBeginingBalances.WindowState = FormWindowState.Normal;

                    objfrmBeginingBalances.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void transferNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmWareHouseTrans");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjWareHouseTransfer == null || ObjWareHouseTransfer.IsDisposed)
                    {
                        ObjWareHouseTransfer = new frmWareHouseTrans(0);
                    }
                    ObjWareHouseTransfer.WindowState = FormWindowState.Minimized;
                    if (ObjWareHouseTransfer.WindowState == FormWindowState.Minimized)
                        ObjWareHouseTransfer.WindowState = FormWindowState.Normal;

                    ObjWareHouseTransfer.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void pOVarificatiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObjFrmPurchaseOrderVarification == null || ObjFrmPurchaseOrderVarification.IsDisposed)
                    {
                        ObjFrmPurchaseOrderVarification = new FrmPurchaseOrderVarification();
                    }
                    //ObjFrmPurchaseOrderVarification.MdiParent = this;
                    ObjFrmPurchaseOrderVarification.Show();
                    //ObjFrmPurchaseOrderVarification.TopMost = true;
                    //ObjFrmPurchaseOrderVarification.WindowState = FormWindowState.Maximized;
                    //ObjFrmPurchaseOrderVarification.Location = new Point(0, 0);                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void gRNLIstToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //frmGRNList grnlist = new frmGRNList();
            //grnlist.Show();

            try
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
                    if (objfrmGRNList == null || objfrmGRNList.IsDisposed)
                    {
                        objfrmGRNList = new frmGRNList();
                    }
                    //objfrmGRNList.MdiParent = this;
                    objfrmGRNList.Show();
                    //objfrmGRNList.TopMost = true;
                    //objfrmGRNList.WindowState = FormWindowState.Maximized;
                    //objfrmGRNList.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void supplyInvoiceListToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //frmSupInvList supllyinvlist = new frmSupInvList();
            //supllyinvlist.Show();

            try
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
                    if (objfrmSupInvList == null || objfrmSupInvList.IsDisposed)
                    {
                        objfrmSupInvList = new frmSupInvList();
                    }
                    //objfrmSupInvList.MdiParent = this;
                    objfrmSupInvList.Show();
                    //objfrmSupInvList.TopMost = true;
                    //objfrmSupInvList.WindowState = FormWindowState.Maximized;
                    //objfrmSupInvList.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void transferNoteListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmTransNoteList == null || objfrmTransNoteList.IsDisposed)
                    {
                        objfrmTransNoteList = new frmTransNoteList();
                    }
                    //objfrmTransNoteList.MdiParent = this;
                    objfrmTransNoteList.Show();
                    //objfrmTransNoteList.TopMost = true;
                    //objfrmTransNoteList.WindowState = FormWindowState.Maximized;
                    //objfrmTransNoteList.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void warehouseItemListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmWhseWiseItemRp");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    //frmWhseWiseItemRp witem = new frmWhseWiseItemRp();
                    //witem.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void addUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmAddUser");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjAddUser == null || ObjAddUser.IsDisposed)
                    {
                        ObjAddUser = new frmAddUser();
                    }
                    //ObjAddUser.MdiParent = this;
                    ObjAddUser.Show();
                    //ObjAddUser.TopMost = true;
                    //ObjAddUser.WindowState = FormWindowState.Maximized;
                    //ObjAddUser.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void viewUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmViewUsers");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmViewUsers == null || objfrmViewUsers.IsDisposed)
                    {
                        objfrmViewUsers = new frmViewUsers();
                    }
                    //objfrmViewUsers.MdiParent = this;
                    objfrmViewUsers.Show();
                    //objfrmViewUsers.TopMost = true;
                    //objfrmViewUsers.WindowState = FormWindowState.Maximized;
                    //objfrmViewUsers.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void activitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmActivities");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                //if (run)
                //{
                //    frmActivities activ = new frmActivities();
                //    activ.Show();
                //}
                if (run)
                {
                    if (objfrmActivities == null || objfrmActivities.IsDisposed)
                    {
                        objfrmActivities = new frmActivities();
                    }
                    //objfrmActivities.MdiParent = this;
                    objfrmActivities.Show();
                    //objfrmActivities.TopMost = true;
                    //objfrmActivities.WindowState = FormWindowState.Maximized;
                    //objfrmActivities.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void defualtSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmDefaultSettingsTabs");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //    }
            //    if (run)
            //    {

            //        if (ObjDefaultseting==null || ObjDefaultseting.IsDisposed)
            //        {
            //            ObjDefaultseting = new frmDefaultSettings();
            //        }
            //        ObjDefaultseting.Show();
            //        //ObjDefaultseting.TopMost = true;
            //        //ObjDefaultseting.WindowState = FormWindowState.Normal;
            //        //ObjDefaultseting
            //        //frmDefaultSettings aad = new frmDefaultSettings();
            //        //aad.Show();
            //        // aad.TopMost = true;

            //        //frmDefaultSettingTabs defaultSet = new frmDefaultSettingTabs();
            //        //defaultSet.Show();
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }

            //}
            //catch { }
        }

        private void deleteRecordsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //frmDeleteRecords deletR = new frmDeleteRecords();
            //deletR.Show();

            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmAddUser");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmDeleteRecords == null || objfrmDeleteRecords.IsDisposed)
                    {
                        objfrmDeleteRecords = new frmDeleteRecords();
                    }
                    //objfrmDeleteRecords.MdiParent = this;
                    objfrmDeleteRecords.Show();
                    //objfrmDeleteRecords.TopMost = true;
                    //objfrmDeleteRecords.WindowState = FormWindowState.Maximized;
                    //objfrmDeleteRecords.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void addWarehouseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmWareHouse");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmWareHouse == null || objfrmWareHouse.IsDisposed)
                    {
                        objfrmWareHouse = new frmWareHouse();
                    }
                    //objfrmWareHouse.MdiParent = this;
                    objfrmWareHouse.Show();
                    //objfrmWareHouse.TopMost = true;
                    //objfrmWareHouse.WindowState = FormWindowState.Maximized;
                    //objfrmWareHouse.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ////try
            ////{
            ////    frmVersion version = new frmVersion();
            ////    version.Show();
            ////}
            ////catch { }
        }

        private void locationWiseSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmLocationWiseSales locawsales = new frmLocationWiseSales();
            locawsales.Show();
        }

        private void vaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmQtyOnHand == null || objfrmQtyOnHand.IsDisposed)
                    {
                        objfrmQtyOnHand = new frmQtyOnHand();
                    }
                    //objfrmValuation.MdiParent = this;
                    objfrmQtyOnHand.Show();
                    //objfrmValuation.TopMost = true;
                    //objfrmValuation.WindowState = FormWindowState.Maximized;
                    //objfrmValuation.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void stockTakingVarianceReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStockTakingVarience stockVariance = new frmStockTakingVarience();
            stockVariance.Show();
        }

        private void invoicewiseSalesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmInvoiceWiseSales invsales = new frmInvoiceWiseSales();
            invsales.Show();
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmChartofAccount");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    frmChartofAccount impch = new frmChartofAccount();
                    impch.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void frmImportCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportCustomer");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjImpCus == null || ObjImpCus.IsDisposed)
                    {
                        ObjImpCus = new frmImportCustomer();
                    }
                    //ObjImpCus.MdiParent = this;
                    ObjImpCus.Show();
                    //ObjImpCus.TopMost = true;
                    //ObjImpCus.WindowState = FormWindowState.Maximized;
                    //ObjImpCus.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void frmDeliveryNoteList_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObjGRNList == null || ObjGRNList.IsDisposed)
                    {
                        ObjGRNList = new frmDeliveryNoteList();
                    }
                    ObjGRNList.WindowState = FormWindowState.Minimized;
                    if (ObjGRNList.WindowState == FormWindowState.Minimized)
                        ObjGRNList.WindowState = FormWindowState.Normal;
                    ObjGRNList.ShowDialog();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void frmSupInvoiceList_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObjSupInvoicelist == null || ObjSupInvoicelist.IsDisposed)
                    {
                        ObjSupInvoicelist = new frmSupInvoiceList();
                    }
                    ObjSupInvoicelist.WindowState = FormWindowState.Minimized;
                    if (ObjSupInvoicelist.WindowState == FormWindowState.Minimized)
                        ObjSupInvoicelist.WindowState = FormWindowState.Normal;
                    ObjSupInvoicelist.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void frmSupReturnList_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObjSupReturnlist == null || ObjSupReturnlist.IsDisposed)
                    {
                        ObjSupReturnlist = new frmSupReturnList();
                    }
                    ObjSupReturnlist.WindowState = FormWindowState.Minimized;
                    if (ObjSupReturnlist.WindowState == FormWindowState.Minimized)
                        ObjSupReturnlist.WindowState = FormWindowState.Normal;
                    ObjSupReturnlist.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void viewDeliveryNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObjDNoteList == null || ObjDNoteList.IsDisposed)
                    {
                        ObjDNoteList = new frmDNoteList();
                    }

                    ObjDNoteList.WindowState = FormWindowState.Minimized;
                    if (ObjDNoteList.WindowState == FormWindowState.Minimized)
                        ObjDNoteList.WindowState = FormWindowState.Normal;

                    ObjDNoteList.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void viewInvoicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceSearch");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmInvoiceSearch == null || objfrmInvoiceSearch.IsDisposed)
                    {
                        objfrmInvoiceSearch = new frmInvoiceSearch();
                    }

                    objfrmInvoiceSearch.WindowState = FormWindowState.Minimized;
                    if (objfrmInvoiceSearch.WindowState == FormWindowState.Minimized)
                        objfrmInvoiceSearch.WindowState = FormWindowState.Normal;

                    objfrmInvoiceSearch.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void inventoyMovementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmInventoryMovement == null || objfrmInventoryMovement.IsDisposed)
                    {
                        objfrmInventoryMovement = new frmInventoryMovement();
                    }
                    objfrmInventoryMovement.Show();
                    //objfrmInventoryMovement.TopMost = true;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void importEmployeeMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceListPrint");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    Connector conn = new Connector();
                    conn.ImportEmployeeMaster();
                    conn.Insert_Employee();
                    MessageBox.Show("Employee Successfully imported from Peachtree");
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { MessageBox.Show("Please Login into Peachtree"); }
        }

        private void invoiceLIstToolStripMenuItem_Click_2(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceListPrint");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmInvoiceListPrint == null || objfrmInvoiceListPrint.IsDisposed)
                        objfrmInvoiceListPrint = new frmInvoiceListPrint();

                    objfrmInvoiceListPrint.WindowState = FormWindowState.Minimized;
                    if (objfrmInvoiceListPrint.WindowState == FormWindowState.Minimized)
                        objfrmInvoiceListPrint.WindowState = FormWindowState.Normal;

                    objfrmInvoiceListPrint.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void ggggToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Connector dd = new Connector();
            dd.ExportCustomerReturn();
        }

        private void inventoryAdjustmentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "Inventory Adjustment");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmInventotyAdjustment == null || objfrmInventotyAdjustment.IsDisposed)
                        objfrmInventotyAdjustment = new frmInventotyAdjustment();

                    objfrmInventotyAdjustment.WindowState = FormWindowState.Minimized;
                    if (objfrmInventotyAdjustment.WindowState == FormWindowState.Minimized)
                        objfrmInventotyAdjustment.WindowState = FormWindowState.Normal;

                    objfrmInventotyAdjustment.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }

        }

        private void pricMetriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Are you sure, you want to Open a Price Matrix? ", "Information", MessageBoxButtons.YesNo);

            if (reply == DialogResult.Yes)
            {
                frmMatrixDesign objMe = new frmMatrixDesign();
                objMe.ShowDialog();
            }
            else
            {
                return;

            }
        }

        private void inquiryScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPriceInquiry ObjInqu = new frmPriceInquiry();
            ObjInqu.Show();
        }

        private void applyToInvoicesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmCustomerReturns");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjCusRetern == null || ObjCusRetern.IsDisposed)
                    {
                        ObjCusRetern = new frmCustomerReturns();
                    }

                    ObjCusRetern.WindowState = FormWindowState.Minimized;
                    if (ObjCusRetern.WindowState == FormWindowState.Minimized)
                        ObjCusRetern.WindowState = FormWindowState.Normal;

                    ObjCusRetern.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void priceMatrixAssigningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Are you sure, you want to Open a Price Matrix? ", "Information", MessageBoxButtons.YesNo);

            if (reply == DialogResult.Yes)
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
                    if (objfrmMatrixDesign == null || objfrmMatrixDesign.IsDisposed)
                    {
                        objfrmMatrixDesign = new frmMatrixDesign();
                    }

                    //objfrmMatrixDesign.MdiParent = this;
                    objfrmMatrixDesign.Show();
                    //objfrmMatrixDesign.TopMost = true;
                    //objfrmMatrixDesign.WindowState = FormWindowState.Maximized;
                    //objfrmMatrixDesign.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                return;
            }
        }

        private void priceListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            run = false;
            //    frmPriceInquiry ObjInqu = new frmPriceInquiry();
            //    ObjInqu.Show();
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmPriceInquiry");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmPriceInquiry == null || objfrmPriceInquiry.IsDisposed)
                    {
                        objfrmPriceInquiry = new frmPriceInquiry();
                    }

                    objfrmPriceInquiry.WindowState = FormWindowState.Minimized;
                    if (objfrmPriceInquiry.WindowState == FormWindowState.Minimized)
                        objfrmPriceInquiry.WindowState = FormWindowState.Normal;
                    objfrmPriceInquiry.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void applyToRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            run = false;
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmCustomerReturns");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjDirectR == null || ObjDirectR.IsDisposed)
                    {
                        ObjDirectR = new frmReturnNotenew();
                    }
                    ObjDirectR.WindowState = FormWindowState.Minimized;
                    if (ObjDirectR.WindowState == FormWindowState.Minimized)
                        ObjDirectR.WindowState = FormWindowState.Normal;

                    ObjDirectR.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void jobEstimateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (objJobEstimate == null || objJobEstimate.IsDisposed)
                {

                    objJobEstimate = new frmJobEstimate(0);
                }
                objJobEstimate.ShowDialog();
                objJobEstimate.WindowState = FormWindowState.Normal;

            }
            catch { }
        }

        private void jobActualToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (objJobActual == null || objJobActual.IsDisposed)
                {
                    objJobActual = new frmJobActual(0);
                }
                objJobActual.ShowDialog();
                objJobActual.WindowState = FormWindowState.Normal;

            }
            catch { }
        }

        private void jobReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (objJobReturn == null || objJobReturn.IsDisposed)
                {

                    objJobReturn = new frmJobReturn(0);
                }
                objJobReturn.ShowDialog();
                objJobReturn.WindowState = FormWindowState.Normal;

            }
            catch { }
        }

        private void jobStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (objJobStatus == null || objJobStatus.IsDisposed)
                {

                    objJobStatus = new frmJobStatus();
                }
                objJobStatus.ShowDialog();
                objJobStatus.WindowState = FormWindowState.Normal;

            }
            catch { }
        }

        private void MnuImportJob_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                Connector conn = new Connector();

                conn.ImportJobList();
                if (conn.InsertJoBData(myConnection, myTrans))
                {

                    conn.PhaseList();
                    conn.InsertPhase(myConnection, myTrans);

                    conn.CostCodelist();
                    conn.InsertCostCodeList(myConnection, myTrans);

                    myTrans.Commit();

                    MessageBox.Show("Job List Successfully imported from Peachtree");
                }
                else
                    return;

            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);

            }
        }

        private void MnuFinishedGoodsTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                if (clsPara.BlnManufactureCompany == false)
                {
                    MessageBox.Show("Manufacturing Process Not Allow");
                    return;
                }
                if (objFinishedGoodTrans == null || objFinishedGoodTrans.IsDisposed)
                {

                    objFinishedGoodTrans = new frmFinishGoodTransfer(0);
                }
                objFinishedGoodTrans.ShowDialog();
                objFinishedGoodTrans.WindowState = FormWindowState.Normal;

            }
            catch { }
        }

        private void jobVarienceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (objItemReport == null || objItemReport.IsDisposed)
                {

                    objItemReport = new frmItemReport();
                }
                clsPara.RepNo = 7001;
                objItemReport.ShowDialog();
                objItemReport.WindowState = FormWindowState.Normal;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void jobProductionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (objItemReport == null || objItemReport.IsDisposed)
                {

                    objItemReport = new frmItemReport();
                }
                clsPara.RepNo = 7002;
                objItemReport.ShowDialog();
                objItemReport.WindowState = FormWindowState.Normal;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void dailyProductionSalesAndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (objItemReport == null || objItemReport.IsDisposed)
                {

                    objItemReport = new frmItemReport();
                }
                clsPara.RepNo = 7003;
                objItemReport.ShowDialog();
                objItemReport.WindowState = FormWindowState.Normal;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void profitabilityReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (objItemReport == null || objItemReport.IsDisposed)
                {

                    objItemReport = new frmItemReport();
                }
                clsPara.RepNo = 7004;
                objItemReport.ShowDialog();
                objItemReport.WindowState = FormWindowState.Normal;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void mToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (objItemReport == null || objItemReport.IsDisposed)
                {

                    objItemReport = new frmItemReport();
                }
                clsPara.RepNo = 7005;
                objItemReport.ShowDialog();
                objItemReport.WindowState = FormWindowState.Normal;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void frmInvoices_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObjInvoices == null || ObjInvoices.IsDisposed)
                    {
                        ObjInvoices = new frmInvoices(0);
                    }

                    ObjInvoices.WindowState = FormWindowState.Minimized;
                    if (ObjInvoices.WindowState == FormWindowState.Minimized)
                        ObjInvoices.WindowState = FormWindowState.Normal;

                    ObjInvoices.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmInvoiceAR_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObInvoicing == null || ObInvoicing.IsDisposed)
                    {
                        objfrmInvoiceAR = new frmInvoiceAR();
                    }

                    objfrmInvoiceAR.WindowState = FormWindowState.Minimized;
                    if (objfrmInvoiceAR.WindowState == FormWindowState.Minimized)
                        objfrmInvoiceAR.WindowState = FormWindowState.Normal;

                    objfrmInvoiceAR.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void issueAndReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmIssueNote objfrmIssueNote = new frmIssueNote(0);
            //objfrmIssueNote.Show();

            try
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
                    if (objfrmIssueNote == null || objfrmIssueNote.IsDisposed)
                    {
                        objfrmIssueNote = new frmIssueNote(0);
                    }
                    objfrmIssueNote.WindowState = FormWindowState.Minimized;
                    if (objfrmIssueNote.WindowState == FormWindowState.Minimized)
                        objfrmIssueNote.WindowState = FormWindowState.Normal;
                    objfrmIssueNote.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void jobToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmJob");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmJob == null || objfrmJob.IsDisposed)
                    {
                        objfrmJob = new frmJob();
                    }
                    //objfrmJob.MdiParent = this;
                    objfrmJob.Show();
                    //objfrmJob.TopMost = true;
                    //objfrmJob.WindowState = FormWindowState.Maximized;
                    //objfrmJob.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void bOQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmBOQ");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmBOQ == null || objfrmBOQ.IsDisposed)
                    {
                        objfrmBOQ = new frmBOQ();
                    }
                    //objfrmBOQ.MdiParent = this;
                    objfrmBOQ.Show();
                    //objfrmBOQ.TopMost = true;
                    //objfrmBOQ.WindowState = FormWindowState.Maximized;
                    //objfrmBOQ.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void bOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;

                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmBOM");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmBOM == null || objfrmBOM.IsDisposed)
                    {
                        objfrmBOM = new frmBOM();
                    }
                    //objfrmBOM.MdiParent = this;
                    objfrmBOM.Show();
                    //objfrmBOM.TopMost = true;
                    //objfrmBOM.WindowState = FormWindowState.Maximized;
                    //objfrmBOM.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void issueNoteToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSiteIssues");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmSiteIssues == null || objfrmSiteIssues.IsDisposed)
                    {
                        objfrmSiteIssues = new frmSiteIssues();
                    }
                    //objfrmSiteIssues.MdiParent = this;
                    objfrmSiteIssues.Show();
                    //objfrmSiteIssues.TopMost = true;
                    //objfrmSiteIssues.WindowState = FormWindowState.Maximized;
                    //objfrmSiteIssues.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void phasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmPhases");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmPhases == null || objfrmPhases.IsDisposed)
                    {
                        objfrmPhases = new frmPhases();
                    }
                    //objfrmPhases.MdiParent = this;
                    objfrmPhases.Show();
                    //objfrmPhases.TopMost = true;
                    //objfrmPhases.WindowState = FormWindowState.Maximized;
                    //objfrmPhases.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void subPhasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSubPhases");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmSubPhases == null || objfrmSubPhases.IsDisposed)
                    {
                        objfrmSubPhases = new frmSubPhases();
                    }
                    //objfrmSubPhases.MdiParent = this;
                    objfrmSubPhases.Show();
                    //objfrmSubPhases.TopMost = true;
                    //objfrmSubPhases.WindowState = FormWindowState.Maximized;
                    //objfrmSubPhases.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cRNTMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmInvoiceARRtn == null || objfrmInvoiceARRtn.IsDisposed)
                    {
                        objfrmInvoiceARRtn = new frmInvoiceARRtn();
                    }

                    objfrmInvoiceARRtn.WindowState = FormWindowState.Minimized;
                    if (objfrmInvoiceARRtn.WindowState == FormWindowState.Minimized)
                        objfrmInvoiceARRtn.WindowState = FormWindowState.Normal;

                    objfrmInvoiceARRtn.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void priceMatrixsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmMatrixDesign == null || objfrmMatrixDesign.IsDisposed)
                    {
                        objfrmMatrixDesign = new frmMatrixDesign();
                    }
                    objfrmMatrixDesign.WindowState = FormWindowState.Minimized;
                    if (objfrmMatrixDesign.WindowState == FormWindowState.Minimized)
                        objfrmMatrixDesign.WindowState = FormWindowState.Normal;
                    objfrmMatrixDesign.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void headOfficeToBranchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmAddUser");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmImportHeadToBranch == null || objfrmImportHeadToBranch.IsDisposed)
                    {
                        objfrmImportHeadToBranch = new frmImportHeadToBranch();
                    }
                    objfrmImportHeadToBranch.Show();
                    objfrmImportHeadToBranch.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }

        private void fileCreationToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmAddUser");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmBranchToHeadOfficecs == null || objfrmBranchToHeadOfficecs.IsDisposed)
                    {
                        objfrmBranchToHeadOfficecs = new frmBranchToHeadOfficecs();
                    }
                    objfrmBranchToHeadOfficecs.Show();
                    objfrmBranchToHeadOfficecs.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }

        }

        private void fileImportingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmAddUser");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (objfrmInsertToHeadoffice.IsDisposed)
                    {
                        objfrmInsertToHeadoffice = new frmInsertToHeadoffice();
                    }
                    objfrmInsertToHeadoffice.Show();
                    objfrmInsertToHeadoffice.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void frmInvoiceMetrics_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmInvoiceMetrics == null || objfrmInvoiceMetrics.IsDisposed)
                    {
                        objfrmInvoiceMetrics = new frmInvoiceMetrics();
                    }
                    objfrmInvoiceMetrics.WindowState = FormWindowState.Minimized;
                    if (objfrmInvoiceMetrics.WindowState == FormWindowState.Minimized)
                        objfrmInvoiceMetrics.WindowState = FormWindowState.Normal;
                    else
                    {
                        if (!objfrmInvoiceMetrics.TopMost)
                            objfrmInvoiceMetrics.TopLevel = true;
                    }
                    objfrmInvoiceMetrics.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmCusReturnList_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmCusReturnList == null || objfrmCusReturnList.IsDisposed)
                    {
                        objfrmCusReturnList = new frmCusReturnList();
                    }
                    objfrmCusReturnList.WindowState = FormWindowState.Minimized;
                    if (objfrmCusReturnList.WindowState == FormWindowState.Minimized)
                        objfrmCusReturnList.WindowState = FormWindowState.Normal;

                    objfrmCusReturnList.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void viewCustomerReturnsDirectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmReturnNotenewSearch == null || objfrmReturnNotenewSearch.IsDisposed)
                    {
                        objfrmReturnNotenewSearch = new frmReturnNotenewSearch();
                    }
                    objfrmReturnNotenewSearch.WindowState = FormWindowState.Minimized;
                    if (objfrmReturnNotenewSearch.WindowState == FormWindowState.Minimized)
                        objfrmReturnNotenewSearch.WindowState = FormWindowState.Normal;

                    objfrmReturnNotenewSearch.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void viewCustomerReturnsToolStripMenuItemTM_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmInvoiceARRtnList == null || objfrmInvoiceARRtnList.IsDisposed)
                    {
                        objfrmInvoiceARRtnList = new frmInvoiceARRtnList();
                    }
                    objfrmInvoiceARRtnList.WindowState = FormWindowState.Minimized;
                    if (objfrmInvoiceARRtnList.WindowState == FormWindowState.Minimized)
                        objfrmInvoiceARRtnList.WindowState = FormWindowState.Normal;

                    objfrmInvoiceARRtnList.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsmiviewInvoiceDirect_Click(object sender, EventArgs e)
        {
            try
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
                    if (ObjInvoiceList == null || ObjInvoiceList.IsDisposed)
                    {
                        ObjInvoiceList = new frmDInvoiceList();
                    }
                    ObjInvoiceList.WindowState = FormWindowState.Minimized;
                    if (ObjInvoiceList.WindowState == FormWindowState.Minimized)
                        ObjInvoiceList.WindowState = FormWindowState.Normal;

                    ObjInvoiceList.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void viewInvoiceToolStripMenuItemTM_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmInvoiceARList");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmInvoiceARList == null || objfrmInvoiceARList.IsDisposed)
                    {
                        objfrmInvoiceARList = new frmInvoiceARList();
                    }
                    objfrmInvoiceARList.WindowState = FormWindowState.Minimized;
                    if (objfrmInvoiceARList.WindowState == FormWindowState.Minimized)
                        objfrmInvoiceARList.WindowState = FormWindowState.Normal;

                    objfrmInvoiceARList.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void siteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSiteIssuesReturn");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmSiteIssuesReturn == null || objfrmSiteIssuesReturn.IsDisposed)
                    {
                        objfrmSiteIssuesReturn = new frmSiteIssuesReturn();
                    }
                    //objfrmSiteIssuesReturn.MdiParent = this;
                    objfrmSiteIssuesReturn.Show();
                    //objfrmSiteIssuesReturn.TopMost = true;
                    //objfrmSiteIssuesReturn.WindowState = FormWindowState.Maximized;
                    //objfrmSiteIssuesReturn.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void locationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmLocation");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmLocation == null || objfrmLocation.IsDisposed)
                    {
                        objfrmLocation = new frmLocation();
                    }
                    //objfrmLocation.MdiParent = this;
                    objfrmLocation.Show();
                    //objfrmLocation.TopMost = true;
                    //objfrmLocation.WindowState = FormWindowState.Maximized;
                    //objfrmLocation.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void importJobsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportFromPeachTree");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmImportFromPeachTree == null || objfrmImportFromPeachTree.IsDisposed)
                    {
                        objfrmImportFromPeachTree = new frmImportFromPeachTree();
                    }
                    //objfrmImportFromPeachTree.MdiParent = this;
                    objfrmImportFromPeachTree.Show();
                    //objfrmImportFromPeachTree.TopMost = true;
                    //objfrmImportFromPeachTree.WindowState = FormWindowState.Maximized;
                    //objfrmImportFromPeachTree.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnCS_Click(object sender, EventArgs e)
        {
            if (objfrmMasterCustomerSales == null || objfrmMasterCustomerSales.IsDisposed)
            {
                objfrmMasterCustomerSales = new frmMasterCustomerSales();
            }
            objfrmMasterCustomerSales.MdiParent = this;
            // objfrmMasterCustomerSales.Location = new Point(10, 20);
            // objfrmMasterCustomerSales.StartPosition=ma


            objfrmMasterCustomerSales.Show();
            objfrmMasterCustomerSales.WindowState = FormWindowState.Maximized;
            objfrmMasterCustomerSales.TopMost = true;
            Point p = new Point(0, 0);
            objfrmMasterCustomerSales.Location = p;

        }

        private void btnSys_Click(object sender, EventArgs e)
        {
            if (objfrmSystem == null || objfrmSystem.IsDisposed)
            {
                objfrmSystem = new frmSystem();
            }
            objfrmSystem.MdiParent = this;
            // objfrmMasterCustomerSales.Location = new Point(10, 20);
            // objfrmMasterCustomerSales.StartPosition=ma


            objfrmSystem.Show();
            objfrmSystem.WindowState = FormWindowState.Maximized;
            objfrmSystem.TopMost = true;
            Point p = new Point(0, 0);
            objfrmSystem.Location = p;
        }

        private void setAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSettingsAccounts");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmSettingsAccounts == null || objfrmSettingsAccounts.IsDisposed)
                    {
                        objfrmSettingsAccounts = new frmSettingsAccounts();
                    }
                    //objfrmSettingsAccounts.MdiParent = this;
                    objfrmSettingsAccounts.Show();
                    //objfrmSettingsAccounts.TopMost = true;
                    //objfrmSettingsAccounts.WindowState = FormWindowState.Maximized;
                    //objfrmSettingsAccounts.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void userDefaultViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSettingsOther");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmSettingsOther == null || objfrmSettingsOther.IsDisposed)
                    {
                        objfrmSettingsOther = new frmSettingsOther();
                    }
                    //objfrmSettingsOther.MdiParent = this;
                    objfrmSettingsOther.Show();
                    //objfrmSettingsOther.TopMost = true;
                    //objfrmSettingsOther.WindowState = FormWindowState.Maximized;
                    //objfrmSettingsOther.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void codeFormatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSettings");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmSettings == null || objfrmSettings.IsDisposed)
                    {
                        objfrmSettings = new frmSettings();
                    }
                    //objfrmSettings.MdiParent = this;
                    objfrmSettings.Show();
                    //objfrmSettings.TopMost = true;
                    //objfrmSettings.WindowState = FormWindowState.Maximized;
                    //objfrmSettings.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void taxRatesDefineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSettingsTax");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                    Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    if (objfrmSettingsTax == null || objfrmSettingsTax.IsDisposed)
                    {
                        objfrmSettingsTax = new frmSettingsTax();
                    }
                    //objfrmSettingsTax.MdiParent = this;
                    objfrmSettingsTax.Show();
                    //objfrmSettingsTax.TopMost = true;
                    //objfrmSettingsTax.WindowState = FormWindowState.Maximized;
                    //objfrmSettingsTax.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void transferNoteListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmSeachTrans == null || objfrmSeachTrans.IsDisposed)
                    {
                        objfrmSeachTrans = new frmSeachTrans();
                    }
                    objfrmSeachTrans.WindowState = FormWindowState.Minimized;
                    if (objfrmSeachTrans.WindowState == FormWindowState.Minimized)
                        objfrmSeachTrans.WindowState = FormWindowState.Normal;

                    objfrmSeachTrans.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        private void ultraLabel1_Click(object sender, EventArgs e)
        {
            if (objfrmMasterVendorPurchase == null || objfrmMasterVendorPurchase.IsDisposed)
            {
                objfrmMasterVendorPurchase = new frmMasterVendorPurchase();
            }
            objfrmMasterVendorPurchase.MdiParent = this;
            // objfrmMasterCustomerSales.Location = new Point(10, 20);
            // objfrmMasterCustomerSales.StartPosition=ma


            objfrmMasterVendorPurchase.Show();
            objfrmMasterVendorPurchase.WindowState = FormWindowState.Maximized;
            objfrmMasterVendorPurchase.TopMost = true;
            Point p = new Point(0, 0);
            objfrmMasterVendorPurchase.Location = p;
        }

        private void issueAndReturnListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmSeachIssueNote == null || objfrmSeachIssueNote.IsDisposed)
                    {
                        objfrmSeachIssueNote = new frmSeachIssueNote();
                    }
                    objfrmSeachIssueNote.WindowState = FormWindowState.Minimized;
                    if (objfrmSeachIssueNote.WindowState == FormWindowState.Minimized)
                        objfrmSeachIssueNote.WindowState = FormWindowState.Normal;
                    objfrmSeachIssueNote.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void adjustmentListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            try
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
                    if (objfrmInventoryAdjustmentList == null || objfrmInventoryAdjustmentList.IsDisposed)
                    {
                        objfrmInventoryAdjustmentList = new frmInventoryAdjustmentList();
                    }
                    objfrmInventoryAdjustmentList.WindowState = FormWindowState.Minimized;
                    if (objfrmInventoryAdjustmentList.WindowState == FormWindowState.Minimized)
                        objfrmInventoryAdjustmentList.WindowState = FormWindowState.Normal;
                    objfrmInventoryAdjustmentList.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        private void ultraLabel3_Click(object sender, EventArgs e)
        {
            if (objfrmMasterInventory == null || objfrmMasterInventory.IsDisposed)
            {
                objfrmMasterInventory = new frmMasterInventory();
            }
            objfrmMasterInventory.MdiParent = this;
            // objfrmMasterCustomerSales.Location = new Point(10, 20);
            // objfrmMasterCustomerSales.StartPosition=ma


            objfrmMasterInventory.Show();
            objfrmMasterInventory.WindowState = FormWindowState.Maximized;
            objfrmMasterInventory.TopMost = true;
            Point p = new Point(0, 0);
            objfrmMasterInventory.Location = p;
        }

        private void ultraLabel4_Click(object sender, EventArgs e)
        {
            if (objfrmMasterJobs == null || objfrmMasterJobs.IsDisposed)
            {
                objfrmMasterJobs = new frmMasterJobs();
            }
            objfrmMasterJobs.MdiParent = this;
            objfrmMasterJobs.Show();
            objfrmMasterJobs.WindowState = FormWindowState.Maximized;
            objfrmMasterJobs.TopMost = true;
            Point p = new Point(0, 0);
            objfrmMasterJobs.Location = p;
        }

        private void locationLiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
            {
                frmMain.objfrmFind = new frmFind("Location");
            }
            frmMain.objfrmFind.ShowDialog();
            //frmMain.objfrmFind.TopMost = true; 
        }

        private void phaseListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
            {
                frmMain.objfrmFind = new frmFind("Phases");
            }
            frmMain.objfrmFind.ShowDialog();
            //frmMain.objfrmFind.TopMost = true; 
        }

        private void subPhaseListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
            {
                frmMain.objfrmFind = new frmFind("Sub Phases");
            }
            frmMain.objfrmFind.ShowDialog();
            //frmMain.objfrmFind.TopMost = true; 
        }

        private void jobEstimateToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (objJobEstimate == null || objJobEstimate.IsDisposed)
                {
                    objJobEstimate = new frmJobEstimate(0);
                }
                objJobEstimate.ShowDialog();
                objJobEstimate.WindowState = FormWindowState.Normal;

            }
            catch { }
        }

        private void spplierDirectReturnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupplierReturn");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                }
                if (run)
                {
                    if (ObjDirectSupplierReturn == null || ObjDirectSupplierReturn.IsDisposed)
                    {
                        ObjDirectSupplierReturn = new frmDirectSupplierReturn();
                    }
                    ObjDirectSupplierReturn.WindowState = FormWindowState.Minimized;
                    if (ObjDirectSupplierReturn.WindowState == FormWindowState.Minimized)
                        ObjDirectSupplierReturn.WindowState = FormWindowState.Normal;
                    ObjDirectSupplierReturn.Show();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        private void uLblMainPage_Click(object sender, EventArgs e)
        {
            if (ObjfrmMasterMainPage == null || ObjfrmMasterMainPage.IsDisposed)
            {
                ObjfrmMasterMainPage = new frmMasterMainPage();
            }
            ObjfrmMasterMainPage.MdiParent = this;
            ObjfrmMasterMainPage.Show();
            ObjfrmMasterMainPage.WindowState = FormWindowState.Maximized;
            ObjfrmMasterMainPage.TopMost = true;
            Point p = new Point(0, 0);
            ObjfrmMasterMainPage.Location = p;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                Connector conn = new Connector();

                conn.ImportJobList();
                conn.InsertJoBData(myConnection, myTrans);

                conn.PhaseList();
                conn.InsertPhase(myConnection, myTrans);

                conn.CostCodelist();
                conn.InsertCostCodeList(myConnection, myTrans);

                myTrans.Commit();

                MessageBox.Show("Job List Successfully imported from Peachtree");


            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);

            }
        }

        private void issueNoteListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmIssueNoteLIstPrint == null || objfrmIssueNoteLIstPrint.IsDisposed)
                    {
                        objfrmIssueNoteLIstPrint = new frmIssueNoteLIstPrint();
                    }

                    objfrmIssueNoteLIstPrint.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void adjustmentsListsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
                    if (objfrmadustmentsListPrint == null || objfrmadustmentsListPrint.IsDisposed)
                    {
                        objfrmadustmentsListPrint = new frmadustmentsListPrint();
                    }

                    objfrmadustmentsListPrint.Show();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Main", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void impoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmImportBOM ObjfrmImportBOM = new frmImportBOM();
            ObjfrmImportBOM.Show();

        }

        private void bOMBuldingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBOMBuilding ObjfrmBOMBuilding = new frmBOMBuilding(0);
            ObjfrmBOMBuilding.Show();

        }

        private void backupDatabaseeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 ObjBackupDB = new Form1();
            ObjBackupDB.Show();
        }

        private void cuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmCreditnotelist ObjfrmCreditnotelist = new frmCreditnotelist();
                ObjfrmCreditnotelist.Show();
            }
            catch { }
        }

        private void invoiceARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmInvoiceAR objfrmInvoiceAR = new frmInvoiceAR();
            objfrmInvoiceAR.Show();
        }

        private void importUnitsMeasuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmImportUnitofmesureData objfrmImportUnitofmesureData = new frmImportUnitofmesureData();
            objfrmImportUnitofmesureData.Show();
        }

        private void registrationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                Registration2 ObjRegistration2 = new Registration2();
                ObjRegistration2.Show();
            }
            catch { }
        }

        private void itemLotTrackingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmLotItems objfrmLotItems = new frmLotItems();
            objfrmLotItems.ShowDialog();

        }

        private void salesOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOrder ObjSalesOrder = new frmOrder();
            ObjSalesOrder.Show();
        }

        private void dailySalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDailySales objfrmLotItems = new frmDailySales();
            objfrmLotItems.ShowDialog();
        }

        private void daillyCollectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDaillyCollection objfrmLotItems = new frmDaillyCollection();
            objfrmLotItems.ShowDialog();
        }

        private void ultraLabel1_Click_1(object sender, EventArgs e)
        {
            frmDailySales objfrmLotItems = new frmDailySales();
            objfrmLotItems.ShowDialog();
        }

        private void salesSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSalesSummary objfrmLotItems = new frmSalesSummary();
            objfrmLotItems.ShowDialog();
        }

        private void salesReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void financialReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDepartmentWiseSales objfrmLotItems = new frmDepartmentWiseSales();
            objfrmLotItems.ShowDialog();
        }

        private void barCodePrintingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBarcodeGenerator objbarcode = new frmBarcodeGenerator();
            objbarcode.Show();
        }

        private void reportingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {

        }

        private void patientRegistrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //InpatientMaster ObjInpatientMaster = new InpatientMaster();
            //ObjInpatientMaster.Show();

        }

        private void patientRegistrationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                InpatientMaster ObjInpatientMaster = new InpatientMaster();
                ObjInpatientMaster.Show();
            }
            catch
            {

            }
        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmScan ObjScan = new frmScan();
            ObjScan.Show();
        }

        private void patentFinalBillToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                frmFinalInvoice ffi = new frmFinalInvoice();
                ffi.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {

        }

        private void referedDoctorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmAddReferenceDoctor ObjRefDoc = new UserAutherization.frmAddReferenceDoctor();
                ObjRefDoc.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cashierSummeryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmScanInformationReport objDaulySummery = new frmScanInformationReport();
                objDaulySummery.Show();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            try
            {
                frmLab ObjfrmLab = new frmLab();
                ObjfrmLab.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void patientDischargeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmFinalRecept ObjFinalReceipt = new UserAutherization.frmFinalRecept();
                ObjFinalReceipt.Show();

            }
            catch
            {

            }
        }

        private void packagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmPackage ObjFinalReceipt = new UserAutherization.frmPackage();
                ObjFinalReceipt.Show();

            }
            catch
            {

            }
        }

        private void packageCreatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmLabPackages ObjFinalReceipt = new UserAutherization.frmLabPackages();
                ObjFinalReceipt.Show();

            }
            catch
            {

            }
        }

        private void updatePriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmPriceUpdate ObjPriceUpdate = new frmPriceUpdate();
                ObjPriceUpdate.Show();

            }
            catch
            {

            }
        }

        private void pakageCreationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmLabPackages ObjFinalReceipt = new UserAutherization.frmLabPackages();
                ObjFinalReceipt.Show();

            }
            catch
            {

            }
        }

        private void channelligDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void channellingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void channellingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                frmChannellig ObjFinalReceipt = new UserAutherization.frmChannellig();
                ObjFinalReceipt.Show();

            }
            catch
            {

            }
        }

        private void patientManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void consultantDetailsAndNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmOPChannellingDetails ObjFinalReceipt = new UserAutherization.frmOPChannellingDetails();
                ObjFinalReceipt.Show();

            }
            catch
            {

            }
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {

        }

        private void updateAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAccountLink fal = new frmAccountLink();
            fal.Show();
        }

        private void pationReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPationReport fpr = new frmPationReport();
            fpr.Show();
        }

        private void reorderReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmReorderReport objReorder = new UserAutherization.frmReorderReport();
                objReorder.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripMenuItem12_Click_1(object sender, EventArgs e)
        {

        }
      
        private void categoryMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobDone ic = new frmJobDone();
            ItemMasterFormName = "categoryMaster";          
            ic.Show();
        }

        private void countryMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobDone ic = new frmJobDone();
            ItemMasterFormName = "CountryMaster";
            ic.Show();
        }

        private void typeMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobDone ic = new frmJobDone();
            ItemMasterFormName = "typeMaster";
            ic.Show();
        }

        private void brandMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobDone ic = new frmJobDone();
            ItemMasterFormName = "brandMaster";
            ic.Show();
        }

        private void sizeMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobDone ic = new frmJobDone();
            ItemMasterFormName = "sizeMaster";
            ic.Show();
        }

        private void jobDoneMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobDone ic = new frmJobDone();
            ItemMasterFormName = "jobDoneMaster";
            ic.Show();
        }

        private void whiteWallMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobDone ic = new frmJobDone();
            ItemMasterFormName = "WhiteWallMaster";
            ic.Show();
        }

        private void frontEditerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditFront ic = new EditFront();
            ic.Show();
        }

        private void exportItemMasterToPeachtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportItemMaster ez = new ExportItemMaster();
            ez.ShowDialog();
        }

        private void itemsPurchasedFromVendorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmViewPurchasedItems vpi = new frmViewPurchasedItems();
            vpi.Show();
        }

        private void exportSuplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectorNew cn = new ConnectorNew();
            if (cn.IsOpenPeachtree() == false)
            {
                return;
            }
            frmExportSupplierInvoice esi = new frmExportSupplierInvoice();
            esi.Show();
        }

        private void exportToPechtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void exportSalesInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectorNew cn = new ConnectorNew();
            if (cn.IsOpenPeachtree() == false)
            {
                return;
            }
            frmExportInvoice esi = new frmExportInvoice();
            esi.Show();
        }

        private void itemCategoryBrandWiseReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSalesWiseReport SWR = new frmSalesWiseReport();
            SWR.Show();
        }

        private void nBTVATReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNBTVATReport NV = new frmNBTVATReport();
            NV.Show();
        }

        private void vehicleHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCustomerHistory ch = new frmCustomerHistory();
            ch.Show();
        }

        private void itemPurchseSoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemPurchaseAndSold i = new ItemPurchaseAndSold();
            i.Show();
        }

        private void dispatchOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}