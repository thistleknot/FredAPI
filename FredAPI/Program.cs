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


        public class Item
        {

        }

        static void Main(string[] args)
        {

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

            //can't use # in GetSeriesObservations.  Ignores the dates.

            var data = new Dictionary<string, IList<Observation>>
            {
                //independent variables

                    //rGDP
                    {"rGDP", fred.GetSeriesObservations("GDPC1", startDate, endDate).ToList() },

                    //savings
                    { "pSaveRate", fred.GetSeriesObservations("PSAVERT", startDate, endDate, frequency: Frequency.Monthly).ToList() },

                    //federal fund rate
                    { "fedFundRate", fred.GetSeriesObservations("DFF", startDate, endDate, frequency: Frequency.Monthly).ToList() },

                    //employee Population Ratio
                    { "empPopRatio", fred.GetSeriesObservations("EMRATIO", startDate, endDate, frequency: Frequency.Monthly).ToList() },

                    //consumer confidence index
                    { "consConfIndex", fred.GetSeriesObservations("UMCSENT", startDate, endDate, frequency: Frequency.Monthly).ToList() },

                //inflationary variable

                    //consumer price index harmonized
                    { "consPriceIndex", fred.GetSeriesObservations("CP0000USM086NEST", startDate, endDate, frequency: Frequency.Monthly).ToList() },

                //dependent variables

                    { "housingSeries", fred.GetSeriesObservations("SPCS20RSA", startDate, endDate, frequency: Frequency.Monthly).ToList() },

        };

            DateTime[] minDates = new DateTime[data.Count];
            DateTime[] maxDates = new DateTime[data.Count];

            int counter = 0;

            foreach (var obData in data)
            {
                DateTime lowestDate = new DateTime(1600, 1, 1);
                DateTime highestDate = new DateTime(1600, 1, 1);

                WriteLine(obData.Key);
                var list = obData.Value;
                
                foreach (var ob in list)
                {
                    if (lowestDate == new DateTime(1600,1,1))
                    {
                        lowestDate = ob.Date;
                    }

                    if (highestDate == new DateTime(1600,1,1))
                    {
                        highestDate = ob.Date;
                    }

                    if (ob.Date < lowestDate)
                    {
                        lowestDate = ob.Date;
                    }

                    if (ob.Date > highestDate)
                    {
                        highestDate = ob.Date;
                    }

                }
                Console.WriteLine(lowestDate.ToShortDateString());
                Console.WriteLine(highestDate.ToShortDateString());

                minDates[counter] = lowestDate;
                maxDates[counter] = highestDate;

                counter++;
            }

            Console.WriteLine("Min dates");
            foreach (var ob in minDates)
            {
                
                Console.WriteLine(ob.ToShortDateString());
                
            }
            Console.WriteLine("Max dates");
            foreach (var ob in maxDates)
            {
                
                Console.WriteLine(ob.ToShortDateString());

            }

            DateTime lowestDate2 = new DateTime(1600, 1, 1);
            DateTime highestDate2 = new DateTime(1600, 1, 1);

            foreach (var ob in minDates)
            {
                if (lowestDate2 == new DateTime(1600, 1, 1))
                {
                    lowestDate2 = ob.Date;
                }

                if (ob.Date > lowestDate2)
                {
                    lowestDate2 = ob.Date;
                }

            }

            foreach (var ob in maxDates)
            {

                if (highestDate2 == new DateTime(1600, 1, 1))
                {
                    highestDate2 = ob.Date;
                }

                if (ob.Date < highestDate2)
                {
                    highestDate2 = ob.Date;
                }

            }

            Console.WriteLine("Min Date:");
            Console.WriteLine(lowestDate2.ToShortDateString());
            Console.WriteLine("Max Date:");
            Console.WriteLine(highestDate2.ToShortDateString());

        }


    }


}
