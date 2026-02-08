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
    public partial class frmjoblist : Form
    {
        int IsFind = 0;
        clsCommon objclsCommon = new clsCommon();
        public static string ConnectionString;

        public frmjoblist(int _IsFind)
        {
            InitializeComponent();
            setConnectionString();
            IsFind = _IsFind;
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Load_IssueList()
        {
            try
            {
                string s = "select  Job_No,JobDate,DilDate,Cusid from tbl_jobmaster  order by  Job_No";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dttourlist.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dttourlist.Rows.Add();
                        DateTime Jdate = Convert.ToDateTime(dt.Rows[i].ItemArray[1]);
                        DateTime Dildate = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);                         
                        dttourlist.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();                     
                        dttourlist.Rows[i].Cells[1].Value = Jdate.ToShortDateString(); 
                        dttourlist.Rows[i].Cells[2].Value = Dildate.ToShortDateString(); 
                        dttourlist.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmjoblist_Load(object sender, EventArgs e)
        {
            Load_IssueList();
        }
        private void SeachOption()
        {
            try
            {
                dttourlist.Rows.Clear();

                String StrCode = null;

                if (cmbSearchby.Text.Trim() == "")
                {
                    return;
                }

                switch (cmbSearchby.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "Job_No";
                            break;
                        }
                    case 1:
                        {
                            StrCode = "Cusid";
                            break;
                        }                   

                }

                if (cmbSearchby.Text.Trim() != "")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s;

                    string ConnString = ConnectionString;
                    s = "select  Job_No,JobDate,DilDate,Cusid from tbl_jobmaster where  " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'  order by Job_No";                    
                    dttourlist.Rows.Clear();
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dttourlist.Rows.Add();
                            DateTime Jdate = Convert.ToDateTime(dt.Rows[i].ItemArray[1]);
                            DateTime Dildate = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dttourlist.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dttourlist.Rows[i].Cells[1].Value = Jdate.ToShortDateString();
                            dttourlist.Rows[i].Cells[2].Value = Dildate.ToShortDateString();
                            dttourlist.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SeachOption();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Job List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dttourlist.Rows.Clear();
                Load_IssueList();
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Tour List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dttourlist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((dttourlist["jobno", e.RowIndex].Value.ToString().Trim()) != "")
                {

                    string jobno = "";
                    try
                    {
                        jobno = dttourlist["jobno", e.RowIndex].Value.ToString().Trim();
                    }
                    catch { }
                    Search.searchIssueNoteNo = dttourlist["jobno", e.RowIndex].Value.ToString().Trim();

                }
                this.Close();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Job List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
    }
}
