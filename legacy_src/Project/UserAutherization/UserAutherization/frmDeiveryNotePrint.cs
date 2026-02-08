using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmDeiveryNotePrint : Form
    {
        public static string ConnectionString;
        DataSet ds;

        public frmDeiveryNotePrint(frmGRN frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.objGRN;
        }

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
            catch { }
        }

        private void frmDeiveryNotePrint_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptGoodsRecevedNote.rpt";
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvDeliveryNote.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}