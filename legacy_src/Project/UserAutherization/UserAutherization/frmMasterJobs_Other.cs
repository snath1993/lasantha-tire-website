using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PCMBeans;
using PCMBLL;
using ComFunction;
//using TJaySmGUI.Reports;
using System.Reflection;

namespace UserAutherization
{
    public partial class frmMasterJobs_Other : Form
    {
        clsBLLPhases objclsBLLPhases = new clsBLLPhases();
        private string _msgTitle = "Login";
       
        bool run = false;
        bool Add = false;
        DataSet objDataSet;
        
        DataTable dtUser = new DataTable();

        public frmMasterJobs_Other()
        {
            InitializeComponent();           
        }    

        private void frmTransactions_Load(object sender, EventArgs e)
        {            
        }

        private void lblLocation_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    Point pt = lblLocation.PointToScreen(e.Location);
            //    cmbLocation.Show(pt);
            //}
        }

        private void lblPhases_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    Point pt = lblPhases.PointToScreen(e.Location);
            //    cmbPhases.Show(pt);
            //}
        }

        private void lblSubPhase_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    Point pt = lblSubPhase.PointToScreen(e.Location);
            //    cmbSubPhases.Show(pt);
            //}
        }

        private void lblJob_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblJob.PointToScreen(e.Location);
                cmbJob_Other.Show(pt);
            }
        }

        private void lblBOQ_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblBOQ.PointToScreen(e.Location);
                cmbBOQ.Show(pt);
            }
        }

        private void lblBOM_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblBOM.PointToScreen(e.Location);
                cmbBOM.Show(pt);
            }
        }

        private void lblSiteIssue_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblSiteIssue.PointToScreen(e.Location);
                cmbIssue.Show(pt);
            }
        }

        private void lblSiteReturn_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = lblSiteReturn.PointToScreen(e.Location);
                cmbReturn.Show(pt);
            }
        }

        private void lblReports_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Point pt = lblReports.PointToScreen(e.Location);
                //cmbrep.Show(pt);
            }
        }

        private void cmbLocation_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Locations")
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
                    if (frmMain.objfrmLocation == null || frmMain.objfrmLocation.IsDisposed)
                    {
                        frmMain.objfrmLocation = new frmLocation();
                    }
                    frmMain.objfrmLocation.Show();
                    //frmMain.objfrmLocation.TopMost = true;                    
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.ClickedItem.Text == "Locations List")
            {
                objDataSet = new DataSet();
                objDataSet = objclsBLLPhases.GetLocations_AllActive_Dropdown();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                frmMain.objfrmFind = new frmFind("Location");
                frmMain.objfrmFind.ShowDialog();
                //frmMain.objfrmFind.TopMost = true;  
            }
        }

        private void cmbPhases_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "Phases")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmPhases");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (frmMain.objfrmPhases == null || frmMain.objfrmPhases.IsDisposed)
            //        {
            //            frmMain.objfrmPhases = new frmPhases();
            //        }
            //        frmMain.objfrmPhases.Show();
            //        frmMain.objfrmPhases.TopMost = true;                    
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "Phases List")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmPhases");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        objDataSet = new DataSet();
            //        objDataSet = objclsBLLPhases.GetPhases_AllActive_ForDropDown();
            //        clsBeansFind.DataTable = objDataSet.Tables[0];

            //        frmMain.objfrmFind = new frmFind("Phases");
            //        frmMain.objfrmFind.ShowDialog();
            //        frmMain.objfrmFind.TopMost = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "Import Phases")
            //{
            //     run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportFromPeachTree");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (objfrmImportFromPeachTree==null || objfrmImportFromPeachTree.IsDisposed)
            //        {
            //            objfrmImportFromPeachTree = new frmImportFromPeachTree();
            //        }
            //        //objfrmImportFromPeachTree.MdiParent = this;
            //        objfrmImportFromPeachTree.Show();
            //        objfrmImportFromPeachTree.TopMost = true;
            //        //objfrmImportFromPeachTree.WindowState = FormWindowState.Maximized;
            //        //objfrmImportFromPeachTree.Location = new Point(0, 0);
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void cmbSubPhases_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "Sub Phases")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSubPhases");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (frmMain.objfrmSubPhases == null || frmMain.objfrmSubPhases.IsDisposed)
            //        {
            //            frmMain.objfrmSubPhases = new frmSubPhases();
            //        }
            //        frmMain.objfrmSubPhases.Show();
            //        frmMain.objfrmSubPhases.TopMost = true;                    
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "Sub Phases List")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSubPhases");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        objDataSet = new DataSet();
            //        objDataSet = objclsBLLPhases.GetSubPhases_AllActive_ForDropDown();
            //        clsBeansFind.DataTable = objDataSet.Tables[0];

            //        frmMain.objfrmFind = new frmFind("Sub Phases");
            //        frmMain.objfrmFind.ShowDialog();
            //        frmMain.objfrmFind.TopMost = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "Import Sub Phases")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportFromPeachTree");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (objfrmImportFromPeachTree == null || objfrmImportFromPeachTree.IsDisposed)
            //        {
            //            objfrmImportFromPeachTree = new frmImportFromPeachTree();
            //        }
            //        //objfrmImportFromPeachTree.MdiParent = this;
            //        objfrmImportFromPeachTree.Show();
            //        objfrmImportFromPeachTree.TopMost = true;
            //        //objfrmImportFromPeachTree.WindowState = FormWindowState.Maximized;
            //        //objfrmImportFromPeachTree.Location = new Point(0, 0);
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void cmbJob_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "Job")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmJob");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (frmMain.objfrmJob == null || frmMain.objfrmJob.IsDisposed)
            //        {
            //            frmMain.objfrmJob = new frmJob();
            //        }
            //        frmMain.objfrmJob.Show();
            //        frmMain.objfrmJob.TopMost = true;                    
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "Job List")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmJob");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        objDataSet = new DataSet();
            //        objDataSet = objclsBLLPhases.GetJob_AllActive_ForDropDown();
            //        clsBeansFind.DataTable = objDataSet.Tables[0];

            //        frmMain.objfrmFind = new frmFind("Job");
            //        frmMain.objfrmFind.ShowDialog();
            //        frmMain.objfrmFind.TopMost = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "Import Jobs")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmImportFromPeachTree");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (objfrmImportFromPeachTree == null || objfrmImportFromPeachTree.IsDisposed)
            //        {
            //            objfrmImportFromPeachTree = new frmImportFromPeachTree();
            //        }
            //        //objfrmImportFromPeachTree.MdiParent = this;
            //        objfrmImportFromPeachTree.Show();
            //        objfrmImportFromPeachTree.TopMost = true;
            //        //objfrmImportFromPeachTree.WindowState = FormWindowState.Maximized;
            //        //objfrmImportFromPeachTree.Location = new Point(0, 0);
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void cmbBOQ_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "BOQ")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmBOQ");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (frmMain.objfrmBOQ == null || frmMain.objfrmBOQ.IsDisposed)
            //        {
            //            frmMain.objfrmBOQ = new frmBOQ();
            //        }
                    
            //        frmMain.objfrmBOQ.Show();
            //        frmMain.objfrmBOQ.TopMost = true;
                    
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "BOQ List")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmBOQ");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        objDataSet = new DataSet();
            //        objDataSet = objclsBLLPhases.GetBOQs_AllActive_Dropdown();
            //        clsBeansFind.DataTable = objDataSet.Tables[0];

            //        frmMain.objfrmFind = new frmFind("BOQ");
            //        frmMain.objfrmFind.ShowDialog();
            //        frmMain.objfrmFind.TopMost = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void cmbBOM_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "BOM")
            //{
            //    run = false;

            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmBOM");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (frmMain.objfrmBOM == null || frmMain.objfrmBOM.IsDisposed)
            //        {
            //            frmMain.objfrmBOM = new frmBOM();
            //        }                    
            //        frmMain.objfrmBOM.Show();
            //        frmMain.objfrmBOM.TopMost = true;               
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "BOM List")
            //{
            //    run = false;

            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmBOM");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        objDataSet = new DataSet();
            //        objDataSet = objclsBLLPhases.GetBOMs_AllActive_Dropdown();
            //        clsBeansFind.DataTable = objDataSet.Tables[0];

            //        frmMain.objfrmFind = new frmFind("BOM");
            //        frmMain.objfrmFind.ShowDialog();
            //        frmMain.objfrmFind.TopMost = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void cmbIssue_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "Site Issue")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSiteIssues");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (frmMain.objfrmSiteIssues == null || frmMain.objfrmSiteIssues.IsDisposed)
            //        {
            //            frmMain.objfrmSiteIssues = new frmSiteIssues();
            //        }

            //        frmMain.objfrmSiteIssues.Show();
            //        frmMain.objfrmSiteIssues.TopMost = true;                   
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "Site Issue List")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSiteIssues");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        objDataSet = new DataSet();
            //        objDataSet = objclsBLLPhases.GetIssues_All_Dropdown();
            //        clsBeansFind.DataTable = objDataSet.Tables[0];

            //        frmMain.objfrmFind = new frmFind("Site Issue");
            //        frmMain.objfrmFind.ShowDialog();
            //        frmMain.objfrmFind.TopMost = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void cmbReturn_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (e.ClickedItem.Text == "Site Returns")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSiteIssuesReturn");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        if (frmMain.objfrmSiteIssuesReturn == null || frmMain.objfrmSiteIssuesReturn.IsDisposed)
            //        {
            //            frmMain.objfrmSiteIssuesReturn = new frmSiteIssuesReturn();
            //        }
            //        frmMain.objfrmSiteIssuesReturn.Show();
            //        frmMain.objfrmSiteIssuesReturn.TopMost = true;                    
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (e.ClickedItem.Text == "Site Returns List")
            //{
            //    run = false;
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSiteIssuesReturn");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //        Add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //    }
            //    if (run)
            //    {
            //        objDataSet = new DataSet();
            //        objDataSet = objclsBLLPhases.GetReturns_All();
            //        clsBeansFind.DataTable = objDataSet.Tables[0];

            //        frmMain.objfrmFind = new frmFind("Site Return");
            //        frmMain.objfrmFind.ShowDialog();
            //        frmMain.objfrmFind.TopMost = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        
       

        
        

       

       

      


        

    }
}