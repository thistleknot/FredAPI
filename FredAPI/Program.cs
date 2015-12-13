using System;
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

        static double reciprocal(double value)
        {
            return (1 / value);
            //return (value);
        }

        //assumes the dates are the same across each data series
        static void PrintData(Dictionary<string, SortedList<DateTime, double?>> dataDictionary)
        {
            //"rGDP", "pSaveRate", "fedFundRate", "empPopRatio", "consConfIndex", "consPriceIndex", "housingSeries"

            IList<DateTime> dates = dataDictionary["pSaveRate"].Keys.ToList();

            WriteLine("date,rGDP,pSaveRate,fedFundRate,empPopRatio,consConfIndex,consPriceIndex,housingSeries");

            foreach (DateTime date in dates)
            {
                WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},", date.ToShortDateString(), dataDictionary["rGDP"][date].Value.ToString(), dataDictionary["pSaveRate"][date].Value.ToString(), dataDictionary["fedFundRate"][date].Value.ToString(), dataDictionary["empPopRatio"][date].Value.ToString(), dataDictionary["consConfIndex"][date].Value.ToString(), dataDictionary["consPriceIndex"][date].Value.ToString(), dataDictionary["housingSeries"][date].Value.ToString());
            }

        }

        static void PrintDataToFile(Dictionary<string, SortedList<DateTime, double?>> dataDictionary, string fileName)
        {
            //"rGDP", "pSaveRate", "fedFundRate", "empPopRatio", "consConfIndex", "consPriceIndex", "housingSeries"

            IList<DateTime> dates = dataDictionary["pSaveRate"].Keys.ToList();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName + ".csv", true))
            {

                file.WriteLine("date,rGDP,pSaveRate,fedFundRate,empPopRatio,consConfIndex,consPriceIndex,housingSeries");

                foreach (DateTime date in dates)
                {
                    file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},", date.ToShortDateString(), dataDictionary["rGDP"][date].Value.ToString(), dataDictionary["pSaveRate"][date].Value.ToString(), dataDictionary["fedFundRate"][date].Value.ToString(), dataDictionary["empPopRatio"][date].Value.ToString(), dataDictionary["consConfIndex"][date].Value.ToString(), dataDictionary["consPriceIndex"][date].Value.ToString(), dataDictionary["housingSeries"][date].Value.ToString());
                }
            }


        }


        static void parseData(Dictionary<string, SortedList<DateTime, double?>> dataDictionary, string[] args)
        {

            int numOfSlides = dataDictionary["pSaveRate"].Count - 1; ; //# of passes, i.e. new neural network //based on sliding windows - slidingWindowSize?
                                                                       //counter = slide
            int numOfSlidingWindows = 7; //training sample size
                                         //counter = windowNumber
            int slidingWindowSize = 6; //# of input vars
                                       //counter = positionInWindow

            int numNeurons = 0;

            double complexity = 1.0;

            Random rnd = new Random();

            int duplication = 0;

            bool randomize = false;

            //predict movement or number
            bool price = true;

            //if (!args[].Equals(null))
            if (!(args.Length == 0))
            {
                price = false;
            }

            IList<DateTime> dates = dataDictionary["pSaveRate"].Keys.ToList();

            string entry = ""; //reusable

            Console.WriteLine("Number of Sliding Windows, i.e. training sets: [7]");

            entry = Console.ReadLine();

            if (entry == "")
            {
                numOfSlidingWindows = 7;
            }
            else
            {
                numOfSlidingWindows = Int32.Parse(entry);
            }

            Console.WriteLine("Size of 1 sliding windows i.e. inputs (months): [6]");

            entry = Console.ReadLine();

            if (entry == "")
            {
                //slidingWindowSize = 6;
            }
            else
            {
                slidingWindowSize = Int32.Parse(entry);
            }

            WriteLine("Duplication Base [2000] How much duplication? [Base/(slidingWindowSize[6]))]: ");

            entry = Console.ReadLine();

            if (entry != "")
            {
                duplication = (int)(Math.Ceiling((double)(Int32.Parse(entry) / slidingWindowSize)));

            }
            else
            {
                //keep default
                duplication = (int)(Math.Ceiling((double)(2000 / slidingWindowSize)));
            }
            //startYear = Convert.ToInt32(entry);
            WriteLine("Randomize [y] or (n) ?:");

            entry = Console.ReadLine();

            if (entry != "n")
            {
                randomize = true;
            }

            Console.WriteLine("Factor of neurons [default is 1, accepts decimals]: [1]");

            entry = Console.ReadLine();

            if (entry == "")
            {
                //nothing
            }
            else
            {
                complexity = Double.Parse(entry);
            }

            Console.WriteLine("Predict (m)ovement (i.e. up/down) or actual [n]umber?");

            numNeurons = (int)(Math.Ceiling(Math.Sqrt((dataDictionary.Count + 1) * slidingWindowSize) * 1) * complexity);

            WriteLine("# of Dates (i.e. months): {0} ", numOfSlides);

            //loop 1
            WriteLine(numOfSlides - numOfSlidingWindows - slidingWindowSize);
            for (int slide = 0; slide <= (numOfSlides - numOfSlidingWindows - slidingWindowSize); slide++)
            {
                //Console.WriteLine();
                //Console.WriteLine();
                //Console.WriteLine("slide: {0}", slide);

                String[] slideNames = new String[numOfSlides];
                String[] testNames = new String[numOfSlides];

                //leading zero
                slideNames[slide] = "train" + slide.ToString("D3") + ".txt";
                //slideNames[slide] = "train" + ".txt";

                //training data, slide # is used for file name
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(slideNames[slide], true))
                {
                    //inputs, hidden, output

                    //**HERE
                    //file.WriteLine("topology: {0} {1} 1", ((dataDictionary.Count) * numOfSlidingWindows), numNeurons);

                    //# of months to feed into input array (1 pass), i.e # of months in 1 window
                    //for (int positionInWindow = 0; positionInWindow <= slidingWindowSize; positionInWindow++)

                    //loop2
                    for (int windowNumber = 0; windowNumber <= numOfSlidingWindows; windowNumber++)
                    {

                        if (windowNumber == numOfSlidingWindows)
                        {
                            //test data
                            //header for next section
                            //Console.WriteLine();
                            //Console.WriteLine("  Test Data: ", windowNumber);
                            //header for next section
                            //Console.WriteLine("  Months 0 to {0}", slidingWindowSize);
                            //Console.Write("  ");

                            //non training data for each pass, slide # is used for file name
                            //leading zero

                            testNames[slide] = "test" + slide.ToString("D3") + ".txt";
                            using (System.IO.StreamWriter testFile =
                            new System.IO.StreamWriter(testNames[slide], true))
                            {
                                testFile.WriteLine("topology: {0} {1} 1", ((dataDictionary.Count) * slidingWindowSize), numNeurons);
                                testFile.Write("in: ");

                                //loop 3a
                                //test data
                                for (int positionInWindow = 0; positionInWindow <= slidingWindowSize; positionInWindow++)
                                {
                                    //inputs
                                    if (positionInWindow != (slidingWindowSize))
                                    {
                                        //inputs
                                        testFile.Write("{0} {1} {2} {3} {4} {5} {6}", (reciprocal((double)(1 + (dates[slide + windowNumber + positionInWindow] - dates[0]).Days))).ToString(".################"), (reciprocal((double)(dataDictionary["rGDP"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)(dataDictionary["pSaveRate"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)(dataDictionary["fedFundRate"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)(dataDictionary["empPopRatio"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)(dataDictionary["consConfIndex"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)(dataDictionary["housingSeries"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"));
                                        //Console.Write("{0}{1}", " ", slide + windowNumber + positionInWindow);

                                    }
                                    else
                                    //target
                                    {
                                        //The last training set I should be predicting?  Well... I still need to check against it.
                                        testFile.WriteLine();
                                        //HERE
                                        if (price)
                                        {
                                            testFile.WriteLine("out: {0}", (reciprocal((double)((dataDictionary["housingSeries"][dates[slide + windowNumber + positionInWindow]].Value)))).ToString(".################"));
                                        }
                                        else
                                        {
                                            double futurePrice = (double)((dataDictionary["housingSeries"][dates[slide + windowNumber + positionInWindow]].Value));
                                            double oldPrice = (double)((dataDictionary["housingSeries"][dates[slide + windowNumber + positionInWindow - 1]].Value));
                                            if (futurePrice > oldPrice)
                                            {
                                                //more
                                                testFile.WriteLine("out: 1");

                                            }
                                            else
                                            {
                                                //less
                                                testFile.WriteLine("out: 0");
                                            }
                                        }


                                        //Console.Write(" t:{0}", (slide + windowNumber + positionInWindow));
                                        //Console.WriteLine();
                                    }
                                    //space between values
                                    if (positionInWindow < (slidingWindowSize - 1))
                                    {
                                        testFile.Write(" ");
                                    }
                                }
                            }

                        }
                        //loop 3b
                        else //training data
                        {
                            file.WriteLine("topology: {0} {1} 1", ((dataDictionary.Count) * slidingWindowSize), numNeurons);
                            //duplication doesn't work unless it's random, repeats 1111, 2222, 3333.
                            for (int d = 0; d < duplication; d++)
                            {
                                //inputs
                                int holder = windowNumber;
                                if (randomize)
                                {
                                    windowNumber = rnd.Next(0, numOfSlidingWindows);
                                    //WriteLine(windowNumber);
                                    if (windowNumber == numOfSlidingWindows) { Console.ReadLine(); }
                                }
                                {

                                    file.Write("in: ");
                                    //# of windows in each training batch, i.e. x# of windows  * slidingWindowSize = # of months processed (some repeated)

                                    //for (int windowNumber = 0; windowNumber < slidingWindowSize; windowNumber++)
                                    for (int positionInWindow = 0; positionInWindow <= slidingWindowSize; positionInWindow++)
                                    {
                                        //inputs
                                        if (positionInWindow != (slidingWindowSize))
                                        {
                                            file.Write("{0} {1} {2} {3} {4} {5} {6}", (reciprocal((1 + (double)(dates[slide + windowNumber + positionInWindow] - dates[0]).Days))).ToString(".################"), (reciprocal((double)(dataDictionary["rGDP"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)(dataDictionary["pSaveRate"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)dataDictionary["fedFundRate"][dates[slide + windowNumber + positionInWindow]].Value)).ToString(".################"), (reciprocal((double)(dataDictionary["empPopRatio"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)(dataDictionary["consConfIndex"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"), (reciprocal((double)(dataDictionary["housingSeries"][dates[slide + windowNumber + positionInWindow]].Value))).ToString(".################"));
                                            //Console.Write("{0}{1}", " ", slide + windowNumber + positionInWindow);
                                            //space inbetween each write until end of line
                                        }
                                        else
                                        //target
                                        {
                                            file.WriteLine();
                                            //HERE
                                            if (price)
                                            {
                                                file.WriteLine("out: {0}", (reciprocal((double)((dataDictionary["housingSeries"][dates[slide + windowNumber + positionInWindow]].Value)))).ToString(".################"));
                                            }
                                            else
                                            {
                                                double futurePrice = (double)((dataDictionary["housingSeries"][dates[slide + windowNumber + positionInWindow]].Value));
                                                double oldPrice = (double)((dataDictionary["housingSeries"][dates[slide + windowNumber + positionInWindow - 1]].Value));
                                                if (futurePrice > oldPrice)
                                                {
                                                    //more
                                                    file.WriteLine("out: 1");

                                                }
                                                else
                                                {
                                                    //less
                                                    file.WriteLine("out: 0");
                                                }
                                            }


                                            //Console.Write(" t:{0}", (slide + windowNumber + positionInWindow));
                                            //Console.WriteLine();
                                        }
                                        //space between values
                                        if (positionInWindow < (slidingWindowSize - 1))
                                        {
                                            file.Write(" ");
                                        }
                                    }
                                }
                                //end randomization
                                windowNumber = holder;
                            }
                        }
                    }
                }
            }

        }

        //to deinflate must pick range of years GDP is based on
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
                    //WriteLine(shortDate);
                    //WriteLine(priceRunningTotal);
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
                    //WriteLine(shortDate);
                    //WriteLine(CPIRunningTotal);
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

                    if (counter != 0)
                    {
                        //flag for quarterly data
                        if ((ob.Key - lastDate).TotalDays > 31)
                        {
                            //WriteLine((ob.Key - lastDate).TotalDays);

                            //sortedDataList.Add(dataNames[dataCounter], new SortedList<DateTime, double?>(list.ToDictionary(x => x.Date, x => x.Value)));
                            //Collection was modified after the enumerator was instantiated
                            //can't do list.Add due to forEach loop already having enumerated the list to loop through.

                            //I could modify this for the # of days in the month, doa  weighted sum
                            holder.Add(lastDate.AddMonths(1), (ob.Value + lastValue) / 2);
                            holder.Add(lastDate.AddMonths(2), (ob.Value + lastValue) / 2);
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
                        WriteLine("holderBelowValue: {0}", obList.Key);
                        holder.Add(obList.Key, obList.Value);

                    }
                    if (obList.Key > highestDate2)
                    {
                        WriteLine("holderAboveValue: {0}", obList.Key);
                        holder.Add(obList.Key, obList.Value);
                    }
                }

                // remove from data here using holder, this removes it from list, but not SortedList?
                foreach (var temp in holder)
                {
                    //list.Remove(temp.Key);
                    obData.Value.Remove(temp.Key);
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

            var data = new Dictionary<string, IList<Observation>>
            {
            };

            //get year of rGDP dollars
            //if (data.ContainsKey("rGDP"))
            {

                //var series = data["rGDP"];

                var series = fred.GetSeries("GDPC1");
                var units = series.Units;
                //Console.WriteLine("Units: ");
                //Console.WriteLine(units);

                //string resultString = Regex.Match(units, @"\d+").Value;
                deflateYear = extractNumber(series.Units);

                Console.WriteLine("deflate year: {0}", deflateYear.ToString());

            }

            string entry;
            WriteLine("Pushing ENTER accepts default values");
            Write("Enter start year [2005]: ");
            entry = ReadLine();
            if (entry == "")
            {
                entry = "2005";
            }
            startYear = Convert.ToInt32(entry);

            WriteLine("Enter end year [2015] ");
            Write("years must cover rGDP real year to deinflate: ");
            entry = ReadLine();
            if (entry == "")
            {
                entry = "2015";
            }
            endYear = Convert.ToInt32(entry);

            Write("Enter start month, ex. Jan = 1 or Oct = [10]: ");
            entry = ReadLine();
            if (entry == "")
            {
                entry = "10";
            }
            startMonth = Convert.ToInt32(entry);

            Write("Enter end month, ex. Jan = 1 or Oct = [10]: ");
            entry = ReadLine();
            if (entry == "")
            {
                entry = "10";
            }
            endMonth = Convert.ToInt32(entry);

            DateTime startDate = new DateTime(startYear, startMonth, 1);
            DateTime endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));

            //can't use # in GetSeriesObservations.  Ignores the dates.

            //my names
            string[] dataNames = new string[] { "rGDP", "pSaveRate", "fedFundRate", "empPopRatio", "consConfIndex", "consPriceIndex", "housingSeries" };

            //online series names
            string[] obsNames = new string[] { "GDPC1", "PSAVERT", "DFF", "EMRATIO", "UMCSENT", "CP0000USM086NEST", "SPCS20RSA" };

            int dataCounter = 0;



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
            //PrintData(sortedDataList); - will crash now because of gaps in rGDP

            //insert records (into rGDP)
            //ideally might want to have this second.
            FillInGaps(ref sortedDataList);

            //identify minmax dates, and trim list, if ran before FillInGaps, rGDP doesn't populate up till HighestDate2
            MinMaxDates(ref sortedDataList);

            PrintData(sortedDataList);
            PrintDataToFile(sortedDataList, "preConversion");

            //deInflate

            deInflate(ref sortedDataList, deflateYear);

            PrintData(sortedDataList);

            PrintDataToFile(sortedDataList, "postConversion");

            parseData(sortedDataList, args);

        }

    }

}
