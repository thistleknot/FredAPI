using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using Xaye.Fred;

namespace FredAPI
{

    class Program
    {

        /*
        //used for iterating through dates see
        //http://stackoverflow.com/questions/3891564/create-array-of-months-between-two-dates
        //http://www.dotnetperls.com/ienumerable

        static IEnumerable<DateTime> monthsBetween(DateTime d0, DateTime d1)
        {

            return Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
                             .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m));
        }

        //http://stackoverflow.com/questions/3738748/create-an-array-or-list-of-all-dates-between-two-dates
        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("endDate must be greater than or equal to startDate");

            while (startDate <= endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }

        public static IEnumerable<DateTime> Range(DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d));
        }

        */

        static void Main(string[] args)
        {

            /*
            IEnumerable<int> result = from value in Enumerable.Range(0, 2)
                                      select value;
                                      */

            var fred = new Fred("661c0a90e914477da5a7518293de5f8e");

            int startYear;
            int endYear;
            int startMonth;
            int endMonth;

            string entry;

            Write("Enter start year, ex. 2005: ");
            entry = ReadLine();
            startYear = Convert.ToInt32(entry);

            Write("Enter end year, ex. 2015: ");
            entry = ReadLine();
            endYear = Convert.ToInt32(entry);

            Write("Enter start month, ex. Jan = 1, October = 10: ");
            entry = ReadLine();
            startMonth = Convert.ToInt32(entry);

            Write("Enter end month, ex. Jan = 1, Octoboer = 10: ");
            entry = ReadLine();
            endMonth = Convert.ToInt32(entry);

            DateTime startDate = new DateTime(2005, startMonth, 1);
            DateTime endDate = new DateTime(2015, endMonth, DateTime.DaysInMonth(endYear, endMonth));

            //independent variables

                //rGDP
                var rGDP = fred.GetSeriesObservations("GDPC1", startDate, endDate);

                //savings
                var pSaveRate = fred.GetSeriesObservations("PSAVERT", startDate, endDate, Fred.CstTime(), Fred.CstTime(), Enumerable.Empty<DateTime>(), frequency: Frequency.Monthly);

                //federal fund rate
                var fedFundRate = fred.GetSeriesObservations("DFF", startDate, endDate, Fred.CstTime(), Fred.CstTime(), Enumerable.Empty<DateTime>(), frequency: Frequency.Monthly);
            
                //employee Population Ratio
                var empPopRatio = fred.GetSeriesObservations("EMRATIO#", startDate, endDate, Fred.CstTime(), Fred.CstTime(), Enumerable.Empty<DateTime>(), frequency: Frequency.Monthly);

                //consumer confidence index
                var consConfIndex = fred.GetSeriesObservations("UMCSENT", startDate, endDate, Fred.CstTime(), Fred.CstTime(), Enumerable.Empty<DateTime>(), frequency: Frequency.Monthly);

            //inflationary variable
            
                //consumer price index harmonized
                var consPriceIndex = fred.GetSeriesObservations("CP0000USM086NEST#", startDate, endDate, Fred.CstTime(), Fred.CstTime(), Enumerable.Empty<DateTime>(), frequency: Frequency.Monthly);

            //dependent variables

                var housingSeries = fred.GetSeriesObservations("SPCS20RSA", startDate, endDate, Fred.CstTime(), Fred.CstTime(), Enumerable.Empty<DateTime>(), frequency: Frequency.Monthly);

            foreach (var ob in rGDP)
            {
                Console.WriteLine(ob.Date.ToShortDateString() + " : " + ob.Value);
            }
            
            


        }


    }


}
