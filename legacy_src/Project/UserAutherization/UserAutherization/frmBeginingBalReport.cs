using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;


namespace UserAutherization
{
    public partial class frmBeginingBalReport : Form
    {
        public frmBeginingBalReport()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;
        DataSet ds;
        DSBeginingBal ObjDSBegbal = new DSBeginingBal();

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }


        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                ObjDSBegbal.Clear();
                string TranType = "OpbBal";

                String S31 = "Select * from tblCompanyInformation";
                SqlCommand cmd31 = new SqlCommand(S31);
                SqlConnection con31 = new SqlConnection(ConnectionString);
                SqlDataAdapter da31 = new SqlDataAdapter(S31, con31);
                da31.Fill(ObjDSBegbal, "dt_CompanyDetails");


                if (cmbSelectWarehouse.Text == "All")
                {
                    String S3 = "Select * from tblItemWhse where TranType='" + TranType + "'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(ObjDSBegbal, "DTBeginBal");

                }
                else
                {
                    String S3 = "Select * from tblItemWhse where WhseId='" + cmbSelectWarehouse.Text.ToString().Trim() + "' and TranType='" + TranType + "'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(ObjDSBegbal, "DTBeginBal");
                }
                ds = ObjDSBegbal;
                InDirectPrint();
            }
            catch { }

        }

        private void InDirectPrint()
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRBeginingBal.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvBegBal.ReportSource = crp;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmBeginingBalReport_Load(object sender, EventArgs e)
        {
            Loadwarehouse();
        }
        private void Loadwarehouse()
        {
            try
            {
                String S1 = "Select WhseId,WhseName from tblWhseMaster";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    cmbSelectWarehouse.Items.Clear();
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        cmbSelectWarehouse.Items.Add(dt1.Rows[i].ItemArray[0].ToString().Trim());
                    }
                }
            }
            catch { }
        }
    }
}