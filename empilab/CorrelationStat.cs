using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace empilab
{
    public class CorrelationStat
    {
        public IEnumerable<Model> listModels { get; set; }
        private double laplasCoef = 2.81;
        private double fisherCoef = 1.33;

        public virtual void Activate(IEnumerable<Model> listCorrelationData)
        {
            listModels = listCorrelationData.OrderBy(x => x.X);
        }

        public double GetCoefPairCorrelation(double avarageX, double avarageY)
        {
            double high = 0;
            double lowPart1 = 0;
            double lowPart2 = 0;
            for (int i = 0; i < listModels.Count(); i++)
            {
                high += (listModels.ElementAt(i).X - avarageX) * (listModels.ElementAt(i).Y - avarageY);
                lowPart1 += Math.Pow((listModels.ElementAt(i).X - avarageX), 2);
                lowPart2 += Math.Pow((listModels.ElementAt(i).Y - avarageY), 2);
            }
            return Math.Round(high / Math.Sqrt(lowPart1 * lowPart2), 4);
        }

        public double GetCorrelationRelation()
        {
            var avarageY = listModels.Average(y => y.Y);
            var droppedList = DropArrayOnGroups(60);
            var highPart = GetHighPartForCorrelationRatio(avarageY, droppedList);
            var lowPart = GetLowPartForCorrelationRatio(avarageY);
            return Math.Round(Math.Sqrt(highPart / lowPart), 4);
        }

        private double GetHighPartForCorrelationRatio(double avarage, List<KeyValuePair<double, double>> droppedList)
        {
            double result = 0;
            for (int i = 0; i < droppedList.Count; i++)
            {
                result += Math.Pow((droppedList[i].Value - avarage), 2) * droppedList[i].Key;
            }
            return result / listModels.Count();
        }

        private double GetLowPartForCorrelationRatio(double avarage)
        {
            double sum = 0;
            for (int i = 0; i < listModels.Count(); i++)
            {
                sum += Math.Pow((listModels.ElementAt(i).Y - avarage), 2);
            }

            return sum / listModels.Count();
        }

        private List<KeyValuePair<double, double>> DropArrayOnGroups(int countGroups)
        {
            var separator = (listModels.Max(x => x.X) - listModels.Min(x => x.X)) / countGroups;
            var result = new List<KeyValuePair<double, double>>();
            for (int i = 0; i < countGroups; i++)
            {
                var listOfItemsInGroup = new List<double>();
                for (int j = 0; j < listModels.Count(); j++)
                {
                    if (listModels.ElementAt(j).X >= i * separator && listModels.ElementAt(j).X < (i + 1) * separator)
                    {
                        listOfItemsInGroup.Add(listModels.ElementAt(j).X);
                    }
                }
                result.Add(new KeyValuePair<double, double>(listOfItemsInGroup.Count, listOfItemsInGroup.Count == 0 ? 0 : listOfItemsInGroup.Average()));
            }
            return result;
        }

        public double GetSpirmanCoef()
        {
            var listToSort = GetSortedYList(); 
            double sum = 0;
            for (int i = 0; i < listModels.Count(); i++)
            {
                sum += Math.Pow((i+1) - listToSort.IndexOf(listModels.ElementAt(i).Y)+1, 2);
            }
            var count = listModels.Count();
            var result = 1 - ((6 * sum) / (Math.Pow(count, 3) - count));
            return Math.Round(result, 4);
        }

        public double GetKandelaCoef()
        {
            var list = GetSortedYList();
            var rankDictionary = new Dictionary<int, int>();
            for (int i = 0; i < listModels.Count(); i++)
            {
                rankDictionary.Add(i + 1, list.IndexOf(listModels.ElementAt(i).Y) + 1);
            }
            var listRankThatBiggerThenPosition = new List<int>();
            for (int i = 0; i < listModels.Count(); i++)
            {
                var currentRank = rankDictionary.ElementAt(i).Value;
                int count = 0;
                for (int j = i; j < listModels.Count()-1; j++)
                {
                    if (currentRank < rankDictionary.ElementAt(j + 1).Value)
                    {
                        count++;
                    }
                }
                listRankThatBiggerThenPosition.Add(count);
            }

            var kandelaCoef = 4 * listRankThatBiggerThenPosition.Sum() / (double)(rankDictionary.Count * (rankDictionary.Count - 1)) - 1;

            return Math.Round(kandelaCoef, 4);
        }

        public double GetStatisticForPairCorrelation(double value)
        {
            return Math.Round(((value * Math.Sqrt(listModels.Count() - 2)) / (Math.Sqrt(1 - value * value))), 4);
        }
        public bool GetSignificanceForPairCorrelation(double value, double quantile)
        {
            return value > quantile;
        }

        public double GetStatisticCorrelationRatio(double ratio)
        {
            var high = Math.Pow(ratio, 2) / (60 - 1);
            var low = (1 - Math.Pow(ratio, 2)) / (listModels.Count() - 60);
            return Math.Round(high / low, 4);
        }

        public bool GetSignificanceForCorrelationRatio(double value)
        {
            return value > fisherCoef;
        }

        public double GetSpirmanStat(double value)
        {
            var high = value * Math.Sqrt(listModels.Count() - 2);
            var low = Math.Sqrt(1 - value * value);
            return Math.Round(high / low, 4);
        }

        public bool GetSignificanceForSpirmanCoef(double value)
        {
            return value > laplasCoef;
        }

        public double GetCandelaStat(double value)
        {
            var high = 3 * value * Math.Sqrt(listModels.Count() * (listModels.Count() - 1));
            var low = Math.Sqrt(2 * (2 * listModels.Count() + 5));
            return Math.Round(high / low, 4);
        }

        public bool GetSignificanceForCandelaCoef(double value)
        {
            return value > laplasCoef;
        }

        private IList<double> GetSortedYList()
        {
            var listToSort = new List<double>();
            foreach (var item in listModels)
            {
                listToSort.Add(item.Y);
            }
            listToSort.Sort();
            return listToSort;
        }

        public double GetQuantileStudent(double probability, int count)
        {
            var u = BuildNormalQuantile(probability);
            var g1 = (Math.Pow(u, 3) + u) / 4;
            var g2 = (5 * Math.Pow(u, 5) + 16 * Math.Pow(u, 3) + 3 * u) / 96;
            var g3 = (3 * Math.Pow(u, 7) + 19 * Math.Pow(u, 5) + 17 * Math.Pow(u, 3) - 15 * u) / 384;
            var g4 = (79 * Math.Pow(u, 9) + 779 * Math.Pow(u, 7) + 1482 * Math.Pow(u, 5) - 1920 * Math.Pow(u, 3) - 945 * u) / 92160;
            return Math.Round((u + g1 / count + g2 / Math.Pow(count, 2) + g3 / Math.Pow(count, 3) + g4 / Math.Pow(count, 4)), 4);
        }

        private double BuildNormalQuantile(double probability)
        {
            if (probability <= 0.5)
            {
                return -BuildNormalQuantileEquation(probability);
            }
            else
            {
                return BuildNormalQuantileEquation(1 - probability);
            }
        }

        private double BuildNormalQuantileEquation(double probability)
        {
            var t = Math.Sqrt(-2 * Math.Log(probability));
            var c0 = 2.515517;
            var c1 = 0.802853;
            var c2 = 0.010328;
            var d1 = 1.432788;
            var d2 = 0.1892659;
            var d3 = 0.001308;
            var high = c0 + c1 * t + c2 * t * t;
            var low = 1 + d1 * t + d2 * t * t + d3 * Math.Pow(t, 3);
            return t - high / low;
        }
    }
}
