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
    public partial class frmBOMList : Form
    {
        public frmBOMList()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;
        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }

        }


        private void SerchAssemblyList()
        {
            try
            {
                string s = "select  BOMReference,AssemblyDate,AssemblyWID,AssemblyID,AssemblyIDDesc,AssmblyQty,Action,IsProcess from tblAsseblyHeader where BOMReference like '%" + txtSearch.Text.Trim() + "%' order by BOMReference";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvAssembylist.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvAssembylist.Rows.Add();
                        dgvAssembylist.Rows[i].Cells["Reference"].Value = dt.Rows[i]["BOMReference"].ToString().Trim();
                        dgvAssembylist.Rows[i].Cells["AssemblyID"].Value = dt.Rows[i]["AssemblyID"].ToString().Trim();
                        dgvAssembylist.Rows[i].Cells["AssemblyDesc"].Value = dt.Rows[i]["AssemblyIDDesc"].ToString().Trim();
                        dgvAssembylist.Rows[i].Cells["date"].Value = DateTime.Parse(dt.Rows[i]["AssemblyDate"].ToString().Trim()).ToShortDateString();
                        dgvAssembylist.Rows[i].Cells["Qty"].Value = dt.Rows[i]["AssmblyQty"].ToString().Trim();
                        dgvAssembylist.Rows[i].Cells["WarehouseID"].Value = dt.Rows[i]["AssemblyWID"].ToString().Trim();

                        if (bool.Parse(dt.Rows[i]["Action"].ToString().Trim()) == true)
                            dgvAssembylist.Rows[i].Cells["Action"].Value = "Build";
                        else
                            dgvAssembylist.Rows[i].Cells["Action"].Value = "Unbuild";

                        if (bool.Parse(dt.Rows[i]["IsProcess"].ToString().Trim()) == true)
                            dgvAssembylist.Rows[i].Cells["Status"].Value = "Process";
                        else
                            dgvAssembylist.Rows[i].Cells["Status"].Value = "Unprocess";
                    }
                }
            }
            catch { }
        }
        private void LoadAssemblyList()
        {
            try
            {
                string s = "select  BOMReference,AssemblyDate,AssemblyWID,AssemblyID,AssemblyIDDesc,AssmblyQty,Action,IsProcess from tblAsseblyHeader order by BOMReference";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvAssembylist.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvAssembylist.Rows.Add();
                        dgvAssembylist.Rows[i].Cells["Reference"].Value = dt.Rows[i]["BOMReference"].ToString().Trim();
                        dgvAssembylist.Rows[i].Cells["AssemblyID"].Value = dt.Rows[i]["AssemblyID"].ToString().Trim();
                        dgvAssembylist.Rows[i].Cells["AssemblyDesc"].Value = dt.Rows[i]["AssemblyIDDesc"].ToString().Trim();
                        dgvAssembylist.Rows[i].Cells["date"].Value = DateTime.Parse(dt.Rows[i]["AssemblyDate"].ToString().Trim()).ToShortDateString();
                        dgvAssembylist.Rows[i].Cells["Qty"].Value = dt.Rows[i]["AssmblyQty"].ToString().Trim();
                        dgvAssembylist.Rows[i].Cells["WarehouseID"].Value = dt.Rows[i]["AssemblyWID"].ToString().Trim();

                        if (bool.Parse(dt.Rows[i]["Action"].ToString().Trim()) == true)
                            dgvAssembylist.Rows[i].Cells["Action"].Value = "Build";
                        else
                            dgvAssembylist.Rows[i].Cells["Action"].Value = "Unbuild";

                        if (int.Parse(dt.Rows[i]["IsProcess"].ToString().Trim()) == 1)
                            dgvAssembylist.Rows[i].Cells["Status"].Value = "Process";
                        else
                            dgvAssembylist.Rows[i].Cells["Status"].Value = "Unprocess";
                    }
                }
            }
            catch { }
        }
        private void frmBOMList_Load(object sender, EventArgs e)
        {
            LoadAssemblyList();
        }

        private void dgvAssembylist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Search.AssemblyReference = dgvAssembylist["Reference", e.RowIndex].Value.ToString().Trim();
            }
            catch { }
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text == "")
            {
                LoadAssemblyList();
            }
            else
            {
                SerchAssemblyList();
            }
        }
    }
}