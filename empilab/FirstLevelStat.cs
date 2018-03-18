using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace empilab
{
    public class FirstLevelStat
    {
        private double laplasCoef = 2.81;        
        private IList<double> listX;
        private IList<double> listY;
        public FirstLevelStat()
        {
            listX = new List<double>();
            listY = new List<double>();
        }
        public virtual void Activate(IEnumerable<Model> listCorrelationData)
        {            
            foreach(var item in listCorrelationData)
            {
                listX.Add(item.X);
                listY.Add(item.Y);
            }
        }

        public virtual KeyValuePair<double, double> GetAverage()
        {
            var averageX = Math.Round(listX.Average(), 4);
            var averageY = Math.Round(listY.Average(), 4);
            return new KeyValuePair<double, double>(averageX, averageY);
        }

        public virtual KeyValuePair<double, double> GetDeviation()
        {
            double sumX = 0;
            double sumY = 0;
            var averageX = GetAverage().Key;
            var averageY = GetAverage().Value;
            for (int i = 0; i < listX.Count; i++)
            {
                sumX += Math.Pow(listX[i] - averageX, 2);
                sumY += Math.Pow(listY[i] - averageY, 2);
            }

            var deviationX = Math.Sqrt(sumX / (listX.Count - 1));
            var deviationY = Math.Sqrt(sumY / (listY.Count - 1));
            return new KeyValuePair<double, double>(Math.Round(deviationX, 4), Math.Round(deviationY, 4));
        }

        public virtual KeyValuePair<double, double> GetAssemetry()
        {
            var count = listX.Count;
            var assemetryX = GetBaisedAssemetry().Key * Math.Sqrt(count * (count - 1)) / (count - 2);
            var assemetryY = GetBaisedAssemetry().Value * Math.Sqrt(count * (count - 1)) / (count - 2);
            return new KeyValuePair<double, double>(Math.Round(assemetryX, 4), Math.Round(assemetryY, 4));
        }

        public KeyValuePair<double, double> GetKurtosis()
        {
            var count = listX.Count;
            var baisedKurtosisX = GetBaisedKurtosis().Key;
            var baisedKurtosisY = GetBaisedKurtosis().Value;
            var kurtosisX = ((baisedKurtosisX - 3) + (6 / (count + 1))) * (count * count - 1) / ((count - 2) * (count - 3));
            var kurtosisY = ((baisedKurtosisY - 3) + (6 / (count + 1))) * (count * count - 1) / ((count - 2) * (count - 3));
            return new KeyValuePair<double, double>(Math.Round(kurtosisX, 4), Math.Round(kurtosisY, 4));
        }

        private KeyValuePair<double, double> GetBaisedKurtosis()
        {
            var baisedDeviationX = GetBasiedDeviation().Key;
            var baisedDeviationY = GetBasiedDeviation().Value;
            double sumX = 0;
            double sumY = 0;
            var averageX = GetAverage().Key;
            var averageY = GetAverage().Value;
            for (int i = 0; i < listX.Count; i++)
            {
                sumX += Math.Pow(listX[i] - averageX, 4);
                sumY += Math.Pow(listY[i] - averageY, 4);
            }

            var baisedKurtosisX = sumX / (listX.Count * Math.Pow(baisedDeviationX, 4));
            var baisedKurtosisY = sumY / (listY.Count * Math.Pow(baisedDeviationY, 4)); ;

            return new KeyValuePair<double, double>(Math.Round(baisedKurtosisX, 4), Math.Round(baisedKurtosisY, 4));
        }

        private KeyValuePair<double, double> GetBaisedAssemetry()
        {
            var baisedDeviationX = GetBasiedDeviation().Key;
            var baisedDeviationY = GetBasiedDeviation().Value;
            double sumX = 0;
            double sumY = 0;
            var averageX = GetAverage().Key;
            var averageY = GetAverage().Value;
            for (int i = 0; i < listX.Count; i++)
            {
                sumX += Math.Pow(listX[i] - averageX, 3);
                sumY += Math.Pow(listY[i] - averageY, 3);
            }

            var baisedAssemetryX = sumX / (listX.Count * Math.Pow(baisedDeviationX, 3));
            var baisedAssemetryY = sumY / (listY.Count * Math.Pow(baisedDeviationY, 3)); ;

            return new KeyValuePair<double, double>(Math.Round(baisedAssemetryX, 4), Math.Round(baisedAssemetryY, 4));
        }

        private KeyValuePair<double, double> GetBasiedDeviation()
        {
            double sumX = 0;
            double sumY = 0;
            var averageX = GetAverage().Key;
            var averageY = GetAverage().Value;
            for (int i = 0; i < listX.Count; i++)
            {
                sumX += Math.Pow(listX[i], 2) - Math.Pow(averageX, 2);
                sumY += Math.Pow(listY[i], 2) - Math.Pow(averageY, 2);
            }

            var deviationX = Math.Sqrt(sumX / listX.Count);
            var deviationY = Math.Sqrt(sumY / listY.Count);
            return new KeyValuePair<double, double>(Math.Round(deviationX, 4), Math.Round(deviationY, 4));
        }

        public double GetDeviationForAvarageX()
        {
            return GetBasiedDeviation().Key / (double)Math.Sqrt(listX.Count);
        }

        public double GetDeviationForAvarageY()
        {
            return GetBasiedDeviation().Value / (double)Math.Sqrt(listX.Count);
        }

        public double GetDeviationForDeviationX()
        {
            return GetBasiedDeviation().Key / (double)Math.Sqrt(2 * listX.Count);
        }

        public double GetDeviationForDeviationY()
        {
            return GetBasiedDeviation().Value / (double)Math.Sqrt(2 * listY.Count);
        }

        public double GetDeviationForAssemetry()
        {
            var firstPart = 6 / (double)listX.Count;
            var secondPart = 1 - 12 / (double)(2 * listX.Count + 7);
            return Math.Sqrt(firstPart * secondPart);
        }

        public double GetDeviationForKurtosis()
        {
            var firstPart = 24 / (double)listX.Count;
            var secondPart = 1 - (225 / (double)(15 * listX.Count + 124));
            return Math.Sqrt(firstPart * secondPart);
        }

        public double GetLowLimit(double parameter, double mark)
        {
            return parameter - mark * laplasCoef;
        }

        public double GetHighLimit(double parameter, double mark)
        {
            return parameter + mark * laplasCoef;
        }
    }
}
