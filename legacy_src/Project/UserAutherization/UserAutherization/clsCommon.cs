using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data;
//using MulticolumPopup;
using System.Drawing;
using System.Security.Cryptography;

namespace UserAutherization
{

    public class clsCommon
    {
        public static string ErrorLog_TextFile = "ErrorLog.txt";

        public void ErrorLog(string formName, string logEntry, string Sender,string OcrEvent)
        {
            try
            {
                if (!File.Exists(Application.StartupPath + "\\" + ErrorLog_TextFile))
                {
                    FileStream fs = new FileStream(Application.StartupPath + "\\" + ErrorLog_TextFile, FileMode.CreateNew);
                }
                using (StreamWriter sw = File.AppendText(Application.StartupPath + "\\" + ErrorLog_TextFile))
                {
                    sw.WriteLine("{0}  {1}  {2}  {3} {4}", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(), formName, Sender,OcrEvent, logEntry);
                    sw.Flush();
                    sw.Close();
                    MessageBox.Show(logEntry, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
