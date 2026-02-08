using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PCMBeans;
using PCMBLL;
using ComFunction;
using MulticolumPopup;
using System.IO;

namespace UserAutherization
{
    public partial class frmUpload : Form
    {
        clsCommon objclsCommon = new clsCommon();
        string _msgTitle = "Attachment";
        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();

        public frmUpload()
        {
            InitializeComponent();
        }

        private void frmEstimate_Load(object sender, EventArgs e)
        {
            try
            {
                if (clsBeansPhases.DtblAttachment != null)
                {
                    DataTable _AttachDTbl = clsBeansPhases.DtblAttachment;

                    foreach(DataRow dr in _AttachDTbl.Rows)
                    {
                        dgvFile.Rows.Add(dr["Name"].ToString(), dr["Path"].ToString(),(dr["Modified"].ToString()), dr["Size"].ToString());
                    }
                    clsBeansPhases.DtblAttachment = null;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog and get result.

                if (result == DialogResult.OK) // Test result.
                {
                    string filepath = openFileDialog1.FileName.ToString();
                    FileInfo file = new FileInfo(filepath);
                    string name = file.Name;       
             
                    dgvFile.Rows.Add(name,filepath,file.LastWriteTime.ToString("dd/MM/yyyy"),file.Length);
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                dgvFile.Rows.Remove(dgvFile.CurrentRow);
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable _AttachDTbl = new DataTable();
                _AttachDTbl.Columns.Add("Name");
                _AttachDTbl.Columns.Add("Path");
                _AttachDTbl.Columns.Add("Modified");
                _AttachDTbl.Columns.Add("Size");

                //_AttachDTbl = (DataTable)dgvFile.DataSource;
                foreach (DataGridViewRow dr in dgvFile.Rows)
                {
                    if (ValidateUploadFiles(dr.Cells[0].Value.ToString()))
                    {
                        DataRow drow = _AttachDTbl.NewRow();
                        drow["Name"] = dr.Cells[0].Value.ToString();
                        drow["Path"] = dr.Cells[1].Value.ToString();
                        drow["Modified"] = dr.Cells[2].Value.ToString();
                        drow["Size"] = dr.Cells[3].Value.ToString();
                        _AttachDTbl.Rows.Add(drow);

                        clsBeansPhases.DtblAttachment = _AttachDTbl;
                        this.Close();
                    }
                    else
                    {
                        DialogResult dialogresult = MessageBox.Show("Already contains a file named " + dr.Cells[0].Value.ToString() + "/n" + "Rename it", "Attachments", MessageBoxButtons.YesNo);
                        if (dialogresult == DialogResult.No)
                        {
                            clsBeansPhases.DtblAttachment = _AttachDTbl;
                            this.Close();
                        }
                        else
                            return;
                    }
                }
                clsBeansPhases.DtblAttachment = _AttachDTbl;
                this.Close();
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private bool ValidateUploadFiles(string fileName)
        {
            if (!System.IO.File.Exists(Application.StartupPath.ToString() + "//Attactments//" + fileName))
            {
                return true;
            }

            else
                return false;
        }

        private void dgvFile_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (System.IO.File.Exists(Application.StartupPath.ToString() + "//Attactments//" + dgvFile.CurrentRow.Cells[0].Value.ToString()))
            //{                
            //    FileInfo filePath = new FileInfo(Application.StartupPath.ToString() + "//Attactments//" + dgvFile.CurrentRow.Cells[0].Value.ToString());
            //    filePath.OpenRead();
            //    //f.Open(FileMode.Open);
            //}
        }

        
    }
}
