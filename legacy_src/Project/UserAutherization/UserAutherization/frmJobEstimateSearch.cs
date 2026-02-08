using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DataAccess;

namespace UserAutherization
{
    
    public partial class frmJobEstimateSearch : Form
    {
        public static string ConnectionString;
        public frmJobEstimateSearch()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }

        }

        private void frmJobEstimateSearch_Load(object sender, EventArgs e)
        {
            try
            {
                Load_IssueList();
            }
            catch { }
        }

        private void Load_IssueList()
        {
            try
            {
                string s = "select  AutoIndex,EstimateNo,EstDate,JobCode,WarehouseCode,Job_Status from EST_HEADER WHERE DocType=1 order by AutoIndex";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSearchIssue.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchIssue.Rows.Add();
                        dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvSearchIssue.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        

                        switch(int.Parse( dt.Rows[i].ItemArray[5].ToString().Trim()))
                        {
                            case 0:
                                {
                                    dgvSearchIssue.Rows[i].Cells[5].Value = "Quote";
                                    break;
                                }

                            case 1:
                                {
                                    dgvSearchIssue.Rows[i].Cells[5].Value = "Active";
                                    break;
                                }

                            case 2:
                                {
                                    dgvSearchIssue.Rows[i].Cells[5].Value = "Complete";
                                    break;
                                }


                        }
                        
                       


                    }
                }
            }
            catch { }
        }

        private void dgvSearchIssue_CellDoubleClick(object sender,DataGridViewCellEventArgs e)
        {

            if (int.Parse(dgvSearchIssue["AutoIndex", e.RowIndex].Value.ToString().Trim()) > 0)
            {
                Search.searchIssueNoteNo = dgvSearchIssue["AutoIndex", e.RowIndex].Value.ToString().Trim();
            }
            this.Close();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SeachOption()
        {
            try
            {
                dgvSearchIssue.Rows.Clear();

                String StrCode = null;

                if (cmbSearchby.Text.Trim() == "")
                {
                    return;
                }

                switch (cmbSearchby.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "AutoIndex";
                            break;
                        }
                    case 1:
                        {
                            StrCode = "EstimateNo";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "EstDate";
                            break;
                        }
                    case 3:
                        {
                            StrCode = "JobCode";
                            break;
                        }
                    case 4:
                        {
                            StrCode = "WarehouseCode";
                            break;
                        }


                }

                if (cmbSearchby.Text.Trim() != "")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s;

                        string ConnString = ConnectionString;

                        if (StrCode != "EstDate")
                        {
                            if (SerchText == "")
                            {
                                Load_IssueList();
                                return;

                            }

                            SerchText = txtSearch.Text.ToString().Trim();

                        }
                        else
                        {
                            SerchText = dtpSearchDate.Text.Trim();
                        }

                        if (StrCode == "EstDate")
                        {
                            s = "select  AutoIndex,EstimateNo,EstDate,JobCode,WarehouseCode,Job_Status from EST_HEADER where EstDate= '" + SerchText + "' AND DocType=1 order by AutoIndex";
                        }
                        else
                        {
                            s = "select  AutoIndex,EstimateNo,EstDate,JobCode,WarehouseCode,Job_Status from EST_HEADER where " + StrCode + " like '" + txtSearch.Text.Trim() + "%' AND DocType=1 order by AutoIndex";
                        }
                        SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvSearchIssue.Rows.Clear();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvSearchIssue.Rows.Add();
                                dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvSearchIssue.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                //dgvSearchIssue.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                            }
                        }
                    
                }

            }
            catch { }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SeachOption();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvSearchIssue.Rows.Clear();
                Load_IssueList();
                txtSearch.Text = "";
            }
            catch { }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Focus();
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            SeachOption();
        }

        private void dgvSearchIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        

   

   

    

    

    }
}