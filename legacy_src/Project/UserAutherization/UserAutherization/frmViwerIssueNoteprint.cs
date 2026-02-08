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
    public partial class frmViwerIssueNoteprint : Form
    {

        DataSet ds = new DataSet();
        public frmViwerIssueNoteprint(frmIssueNote frmParent)
        {
            InitializeComponent();
            ds = frmParent.ObjDSIssueNote;
        }

        private void frmViwerIssueNoteprint_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptIssueNote.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CRVIssueNote.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
    }
}