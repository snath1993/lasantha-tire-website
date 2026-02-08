using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmPationReport : Form
    {
        private string ConnectionString;

        public frmPationReport()
        {
            InitializeComponent();
            setConnectionString();
        }
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {


        }

        private void loadGrid()
        {
            DataTable dt;
            dt = LoadDataTable();
            if (dt == null)
            {
                return;
            }
            LoadGrid(dt);
        }

        private void LoadGrid(DataTable dt)
        {
            try
            {
                dgvPationReport.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvPationReport.Rows.Add();
                        dgvPationReport.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        var date = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvPationReport.Rows[i].Cells[1].Value = Convert.ToDateTime(date).ToShortDateString();
                        dgvPationReport.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvPationReport.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DataTable LoadDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                string ReceiptNo = "";
                string PatientName = "";
                string ContactNo = "";

                if (txtrecept.Text != null) ReceiptNo = txtrecept.Text.ToString().Trim();
                if (txtName.Text != null) PatientName = txtName.Text.ToString().Trim();
                if (txtContact.Text != null) ContactNo = txtContact.Text.ToString().Trim();

                string sSQL = "SELECT * FROM [View_PationReport] where [Date] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [Date]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                            " and [ReceiptNo] like '%" + ReceiptNo + "%'" +
                            " and PatientName like '%" + PatientName + "%'" +
                            " and ContactNo like '%" + ContactNo + "%' ";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(dt);


                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            loadGrid();
        }
        public dspationReport dt = new dspationReport();
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string ReceiptNo = "";
                string PatientName = "";
                string ContactNo = "";

                if (txtrecept.Text != null) ReceiptNo = txtrecept.Text.ToString().Trim();
                if (txtName.Text != null) PatientName = txtName.Text.ToString().Trim();
                if (txtContact.Text != null) ContactNo = txtContact.Text.ToString().Trim();

                string sSQL = "SELECT * FROM [View_PationReport] where [Date] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [Date]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                            " and [ReceiptNo] like '%" + ReceiptNo + "%'" +
                            " and PatientName like '%" + PatientName + "%'" +
                            " and ContactNo like '%" + ContactNo + "%' ";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(dt.View_PationReport);

                //string sSQL1 = "SELECT * FROM [tblCompanyInformation]";

                //SqlCommand cmd31 = new SqlCommand(sSQL1);
                //SqlConnection con31 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da31 = new SqlDataAdapter(sSQL1, con31);
                //da3.Fill(dt._tblCompanyInformation_);

                frmPationReportPrint fprp = new frmPationReportPrint(this);
                fprp.Show();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmPationReport_Load(object sender, EventArgs e)
        {

        }
    }
}
