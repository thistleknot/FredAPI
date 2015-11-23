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
        static void printData(var dict)
        {

        }
        */



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

            DateTime startDate = new DateTime(startYear, startMonth, 1);
            DateTime endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));

            //can't use # in GetSeriesObservations.  Ignores the dates.

            string[] dataNames = new string[] { "rGDP", "pSaveRate", "fedFundRate", "empPopRatio", "consConfIndex", "consPriceIndex", "housingSeries" };
            string[] obsNames = new string[] { "GDPC1", "PSAVERT", "DFF", "EMRATIO", "UMCSENT", "CP0000USM086NEST", "SPCS20RSA" };

            int dataCounter = 0;

            var data = new Dictionary<string, IList<Observation>>
            {
            };

            //create lists as FredAPI objects
            foreach (var instance in dataNames)
            {
                if (dataNames[dataCounter] == "rGDP")
                {
                    data.Add(dataNames[dataCounter], fred.GetSeriesObservations(obsNames[dataCounter], startDate, endDate).ToList());
                }
                else
                {
                    data.Add(dataNames[dataCounter], fred.GetSeriesObservations(obsNames[dataCounter], startDate, endDate, frequency: Frequency.Monthly).ToList());
                }
                
                

                dataCounter++;
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
                
                //the reason date works here is because the object type has two elements.  A .Date and a .Value
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
                //Console.WriteLine(lowestDate.ToShortDateString());
                //Console.WriteLine(highestDate.ToShortDateString());

                minDates[counter] = lowestDate;
                maxDates[counter] = highestDate;

                counter++;
            }

            //Console.WriteLine("Min dates");
            foreach (var ob in minDates)
            {
                
                Console.WriteLine(ob.ToShortDateString());
                
            }
            //Console.WriteLine("Max dates");
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

            //convert data to sortedList (the DateTime becomes key of the SortedList), the string is the Key of the Dictionary.
            var sortedDataList = new Dictionary<string, SortedList<DateTime, double?>> { };

            dataCounter = 0;

            foreach (var obData in data)
            {
                var list = obData.Value;
                sortedDataList.Add(dataNames[dataCounter], new SortedList<DateTime, double?>(list.ToDictionary(x => x.Date, x => x.Value)));
                dataCounter++;
            }

            //print data
            foreach (var obData in sortedDataList)
            {
                WriteLine(obData.Key);
                var list = obData.Value;
                foreach (var ob in list)
                {
                    //WriteLine("{0}-{1}, {2}", ob.Date.Year, ob.Date.Month, ob.Value);
                    WriteLine("{0}-{1}, {2}", ob.Key.Year, ob.Key.Month, ob.Value);
                }
              }

            //insert records into rGDP


        }


    }


}
