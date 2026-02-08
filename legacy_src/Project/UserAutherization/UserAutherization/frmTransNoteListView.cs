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
    public partial class frmTransNoteListView : Form
    {
        DataSet ds;
        string reportOption= "Detail";
        // public WhuseTransfer op = new WhuseTransfer();
        public frmTransNoteListView(frmTransNoteList frmParent)
        {
            InitializeComponent();
            ds = frmParent.dsTNoteList;
            reportOption = frmParent.rptOpt;
        }
        

        private void frmTransNoteListView_Load(object sender, EventArgs e)
        {
            try
            {
                if (reportOption == "Detail")
                {
                    string Myfullpath;
                    ReportDocument crp = new ReportDocument();
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRTransferNoteList.rpt";
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvTransferNotelist.ReportSource = crp;
                }
                if(reportOption == "Summery")
                {
                    string Myfullpath;
                    ReportDocument crp = new ReportDocument();
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRTransferNoteListSummery.rpt";
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvTransferNotelist.ReportSource = crp;
                }

                if (reportOption == "FromToALL")
                {
                    string Myfullpath;
                    ReportDocument crp = new ReportDocument();
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRTransferSummeryFromToAll.rpt";
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvTransferNotelist.ReportSource = crp;
                }
                if (reportOption == "AllToFrom")
                {
                    string Myfullpath;
                    ReportDocument crp = new ReportDocument();
                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRTransferSummeryAllToFrom.rpt";
                    crp.Load(Myfullpath);
                    crp.SetDataSource(ds);
                    crvTransferNotelist.ReportSource = crp;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       
    }
}