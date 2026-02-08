using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class Regisration6 : Form
    {
        public Regisration6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DialogResult reply = MessageBox.Show("Do you want to Restart Import Costing  ?",
            //                     "Yes or No Demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (reply == DialogResult.Yes)
            //{
            //    IsRestart = true;
            MessageBox.Show("You have Suucessfully registered in the system");
                System.Windows.Forms.Application.Restart();
                // Environment.Exit(0);
            //}
            //else if (reply == DialogResult.No)
            //{
            //    // e.Cancel = true; //  state = 1;
            //    return;
            //}
        }
    }
}