using System;
using System.Windows.Forms;
using Interop.PeachwServer;

namespace UserAutherization
{
	public class Connect
	{
        public Interop.PeachwServer.Application app;
        public Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();

		public Connect()
		{
            
            try
            {
               // app = (Interop.PeachwServer.Application)login.GetApplication(frmMain.sName, frmMain.sPassword);

                //string StrComname=null;
                 app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");
                 //StrComname = app.CurrentCompanyName;
                 
                 //if (app.CompanyIsOpen == true)
                 //{
                 //    app.CloseCompany();
                 //}
               // "Tom Aligood", "3M3336RJP111X7A"
            }
            catch (System.UnauthorizedAccessException e )
            {
                MessageBox.Show(e.Message);
            }
		}
	}
}
