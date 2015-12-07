﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using Xaye.Fred;
//using System.Text.RegularExpressions.Regex;

namespace FredAPI
{

    class Program
    {

        //from http://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
        static int extractNumber(string text)
        {
            //string a = "str123";
            string b = string.Empty;
            int val = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]))
                    b += text[i];
            }

            if (b.Length > 0)
                val = int.Parse(b);

            return val;

        }

        static void PrintData(Dictionary<string, SortedList<DateTime, double?>> dataDictionary)
        {
            foreach (var obData in dataDictionary)
            {
                WriteLine(obData.Key);
                var list = obData.Value;
                foreach (var ob in list)
                {
                    //WriteLine("{0}-{1}, {2}", ob.Date.Year, ob.Date.Month, ob.Value);
                    WriteLine("{0}-{1}, {2}", ob.Key.Year, ob.Key.Month, ob.Value);
                }
            }
        }

        static void deInflate(ref Dictionary<string, SortedList<DateTime, double?>> dataDictionary, int year)
        {

            int counter = 0;
            double? priceRunningTotal = 0.0;
            double? CPIRunningTotal = 0.0;
            double? averagePrice = 0.0;
            double? averageCPI = 0.0;

            //need to definflate SPCS20RSA based on CPI
            foreach (var place in dataDictionary["housingSeries"])
            {
                DateTime temp = place.Key;

                int shortDate = temp.Year;

                if (place.Key.Year == year)
                {
                    priceRunningTotal += place.Value;
                    counter++;
                    WriteLine(shortDate);
                    WriteLine(priceRunningTotal);

                }

            }
            averagePrice = (priceRunningTotal / counter);

            //reset counter
            counter = 0;

            //need to definflate SPCS20RSA based on CPI
            foreach (var place in dataDictionary["consPriceIndex"])
            {
                DateTime temp = place.Key;

                int shortDate = temp.Year;

                if (place.Key.Year == year)
                {
                    CPIRunningTotal += place.Value;
                    counter++;
                    WriteLine(shortDate);
                    WriteLine(CPIRunningTotal);

                }

            }
            averageCPI = (CPIRunningTotal / counter);

            WriteLine("averagePrice: {0}", averagePrice);

            WriteLine("averageCPI: {0}", averageCPI);

            //deInflate housing

            var keys = new List<DateTime>(dataDictionary["housingSeries"].Keys);

            //values are read only
            //foreach (var place in dataDictionary["consPriceIndex"])
            foreach (DateTime key in keys)
            {
                

                double? divisor = averagePrice / dataDictionary["housingSeries"][key].Value;

                double? divided = dataDictionary["housingSeries"][key].Value / divisor;

                double? factor = dataDictionary["consPriceIndex"][key] / averageCPI;

                double? newValue = divided * factor;
                //WriteLine(dataDictionary["housingSeries"][key].ToString());
                //WriteLine("{0} {1} {2} {3} {4}", key, dataDictionary["housingSeries"][key].ToString(), divisor, divided, newValue);
                dataDictionary["housingSeries"][key] = newValue;

            }

        }

        //can't add elements to dictionary by reference...
        //actually I can, I just can't use a foreach... duh
        static void FillInGaps(ref Dictionary<string, SortedList<DateTime, double?>> dataDictionary)
        {
           
            //some bullshit date, gets overwritten after counter > 0
            DateTime lastDate = new DateTime(1776, 1, 1);
            double? lastValue = 0;

            //for each FredObject


            foreach (var obData in dataDictionary)
            //while (dataDictionary.count > 0)
            {
                WriteLine("Key {0}", obData.Key);

                int counter = 0;
                var list = obData.Value;
                //var list = dataDictionary[i];

                //for each Date, Value pair (in specific FredObject)

                //var holder = new Dictionary<string, SortedList<DateTime, double?>> { };

                var holder = new SortedList<DateTime, double?> { };

                foreach (var ob in list)
                {
                    
                    if (counter!=0)
                    {
                        //flag for quarterly data
                        if ((ob.Key - lastDate).TotalDays > 31)
                        {
                            WriteLine((ob.Key - lastDate).TotalDays);

                            //sortedDataList.Add(dataNames[dataCounter], new SortedList<DateTime, double?>(list.ToDictionary(x => x.Date, x => x.Value)));
                            //Collection was modified after the enumerator was instantiated
                            //can't do list.Add due to forEach loop already having enumerated the list to loop through.

                            //I could modify this for the # of days in the month, doa  weighted sum
                            holder.Add(lastDate.AddMonths(1), (ob.Value + lastValue)/2);
                            holder.Add(lastDate.AddMonths(2), (ob.Value + lastValue)/2);

                            //holder.Add(lastDate.AddMonths(2), lastValue);

                            WriteLine("FillInDates!");
                            WriteLine(lastDate.AddMonths(1));
                            WriteLine(lastDate.AddMonths(2));
                            WriteLine(lastValue.ToString());
                        }

                    }
                    
                    lastDate = ob.Key;
                    lastValue = ob.Value;
                    counter++;
                }

                //add to list out of the loop
                foreach (var ob in holder)
                {
                    list.Add(ob.Key, ob.Value);

                }

            }

        }

        //static void MinMaxDates(ref Dictionary<string, IList<Observation>> data)
        static void MinMaxDates(ref Dictionary<string, SortedList<DateTime, double?>> data)
        {

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
                    if (lowestDate == new DateTime(1600, 1, 1))
                    {
                        lowestDate = ob.Key;
                    }

                    if (highestDate == new DateTime(1600, 1, 1))
                    {
                        highestDate = ob.Key;
                    }

                    if (ob.Key < lowestDate)
                    {
                        lowestDate = ob.Key;
                    }

                    if (ob.Key > highestDate)
                    {
                        highestDate = ob.Key;
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
            
            //var holder = new Dictionary<string, IList<Observation>> { };

            //trim
            foreach (var obData in data)
            {
                var list = obData.Value;
                var holder = new SortedList<DateTime, double?> { };
                foreach (var obList in list)
                {
                    if (obList.Key < lowestDate2)
                    {
                        WriteLine("holderBelowValue");
                        holder.Add(obList.Key, obList.Value);

                    }
                    if (obList.Key > highestDate2)
                    {
                        WriteLine("holderBelowValue");
                        holder.Add(obList.Key, obList.Value);

                    }

                    //print holder
                    
                    foreach (var holdList in holder)
                    {
                        WriteLine("holder");
                        WriteLine(holdList.Key.ToString());
                        WriteLine(holdList.Value.ToString());
                    }

                }
                // remove from data here using holder

                foreach (var temp in holder)
                {
                    list.Remove(temp.Key);
                    
                }

            }

        }

        static void Main(string[] args)
        {

            var fred = new Fred("661c0a90e914477da5a7518293de5f8e");

            int startYear;
            int endYear;
            int startMonth;
            int endMonth;

            int deflateYear = 0;

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

            //my names
            string[] dataNames = new string[] { "rGDP", "pSaveRate", "fedFundRate", "empPopRatio", "consConfIndex", "consPriceIndex", "housingSeries" };

            //online series names
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

           //get year of rGDP dollars
           if (data.ContainsKey("rGDP"))
            {

                //var series = data["rGDP"];
                
                var series = fred.GetSeries("GDPC1");
                var units = series.Units;
                Console.WriteLine("Units: ");
                Console.WriteLine(units);

                //string resultString = Regex.Match(units, @"\d+").Value;
                deflateYear = extractNumber(series.Units);

                Console.WriteLine(deflateYear.ToString());

            }

            //MinMaxDates(ref data);

            //convert data to sortedList (the DateTime becomes key of the SortedList), the string is the Key of the Dictionary.
            var sortedDataList = new Dictionary<string, SortedList<DateTime, double?>> { };

            dataCounter = 0;

            foreach (var obData in data)
            {
                var list = obData.Value;
                sortedDataList.Add(dataNames[dataCounter], new SortedList<DateTime, double?>(list.ToDictionary(x => x.Date, x => x.Value)));
                dataCounter++;
            }

            //print data before changes
            PrintData(sortedDataList);

            //identify minmax dates, and trim list
            MinMaxDates(ref sortedDataList);

            //insert records (into rGDP)
            //ideally might want to have this second.
            FillInGaps(ref sortedDataList);
            
            PrintData(sortedDataList);

            //deInflate

            deInflate(ref sortedDataList, deflateYear);

            PrintData(sortedDataList);



        }

    }


}
