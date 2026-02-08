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
    public partial class frmInvMovementPrint : Form
    {
        DateTime _FromDate = DateTime.Now.Date;

        public frmInvMovementPrint(frmInventoryMovement frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ObjMovement;
        }
        public static string GetDateTime(DateTime DtGetDate)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(DtGetDate);
                string Dformat = "MM/dd/yyyy";
                return DTP.ToString(Dformat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public frmInvMovementPrint(frmInventoryMovement frmParent,DateTime Param)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ObjMovement;
            _FromDate = Param;
        }

        public static string ConnectionString;
        DataSet ds;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        private void frmInvMovementPrint_Load(object sender, EventArgs e)
        {
            try
            {
                
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvMovement.rpt";
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);
                //crp.SetParameterValue("FROMDATE", _FromDate);
                crp.SetDataSource(ds);
                crvInvmovement.ReportSource = crp;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void crvInvmovement_Click(object sender, EventArgs e)
        {

        }
    }
}