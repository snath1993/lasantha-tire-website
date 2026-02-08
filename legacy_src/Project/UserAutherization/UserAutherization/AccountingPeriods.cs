using System;

namespace UserAutherization
{

	public class AccountingPeriods
	{
        Connector imp = new Connector();
        //public Conn ptApp = new Connect();
		public DateTime[] StartDate;
		public DateTime[] EndDate;
		public int PeriodsPerYear;
		public int CurrentPeriod;

		public AccountingPeriods()
		{
            imp.app.GetAccountingPeriods(out PeriodsPerYear, out CurrentPeriod, out StartDate, out EndDate);

		}
		public string GetLastDayOfCurrPer()
		{
            return EndDate[CurrentPeriod].ToString();
		}
		public DateTime getFirstOpenDay()
		{
            DateTime dtStart = DateTime.Parse(StartDate[PeriodsPerYear].ToString());
            return dtStart;
		}
		public DateTime getLastOpenDay()
        {
            DateTime dtEnd = DateTime.Parse(EndDate[PeriodsPerYear + PeriodsPerYear].ToString());
            return dtEnd;
            //return DateTime.Parse(dtEnd.ToString("MM/dd/yyyy"));
		}
	}
}
