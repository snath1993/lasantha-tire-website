using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;

namespace UserAutherization
{
    public partial class frmViewerIsueNoteListprint : Form
    {
        public frmViewerIsueNoteListprint(frmIssueNoteLIstPrint frmParent)
        {
            InitializeComponent();
            ds = frmParent.dsobjIssueList;
        }

        DataSet ds = new DataSet();

        private void ViewerIsueNoteListprint_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRIssueNoteList.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CRVIssueNoteList.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
    }
}