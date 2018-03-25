using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace empilab
{
    public class RegressionStatistic
    {
        private IList<double> listX;
        private IList<double> listY;
        private CorrelationStat correlationStat;
        public RegressionStatistic(IEnumerable<Model> listModels, CorrelationStat correlationStat)
        {
            this.correlationStat = correlationStat;
            listX = new List<double>();
            listY = new List<double>();
            foreach (var item in listModels)
            {
                listX.Add(item.X);
                listY.Add(item.Y);
            }
        }

        public LinearRegressionModel GetLinearRegressionCoefs(double pairCorrelationCoef)
        {
            var sigmas = GetSigmas();
            var b = Math.Round(pairCorrelationCoef * sigmas.Value / sigmas.Key, 4);
            var a = Math.Round(listY.Average() - b * listX.Average(), 4);
            return new LinearRegressionModel { A = a, B = b };
        }

        public double GetDespersionForA(double linearRegressionCoefA, double linearRegressionCoefB)
        {
            var sigma = GetSigmas().Key;
            var firstHalf = GetBasicDeviation(linearRegressionCoefA, linearRegressionCoefB);
            var secondHalf = Math.Sqrt((1 / listX.Count) + (Math.Pow(listX.Average(), 2) / (sigma * sigma * (listX.Count - 1))));
            return Math.Round(Math.Pow(firstHalf * secondHalf, 2), 4);
        }

        public double GetDespersionForB(double linearRegressionCoefA, double linearRegressionCoefB)
        {
            var sigma = GetSigmas().Key;
            var basic = GetBasicDeviation(linearRegressionCoefA, linearRegressionCoefB);
            var length = Math.Sqrt(listX.Count - 1);
            return Math.Round(Math.Pow(basic / (sigma * length), 2), 4);
        }

        public double GetStatistic(double coef, double coefDeviation)
        {
            return Math.Round(Math.Abs(coef) / Math.Sqrt(coefDeviation), 4);
        }

        public double GetQuantile()
        {
            return Math.Round(correlationStat.GetQuantileStudent(0.975, listX.Count - 2), 4);
        }

        public bool GetSignificance(double quantile, double parameter)
        {
            return Math.Abs(parameter) < quantile;
        }

        public double GetLowLimit(double coef, double quantile, double deviation)
        {
            return Math.Round(coef - quantile * Math.Sqrt(deviation), 4);
        }

        public double GetHighLimit(double coef, double quantile, double deviation)
        {
            return Math.Round(coef + quantile * Math.Sqrt(deviation), 4);
        }

        private double GetBasicDeviation(double linearRegressionCoefA, double linearRegressionCoefB)
        {
            double sum = 0;
            for (int i = 0; i < listX.Count; i++)
            {
                sum += Math.Pow((listY[i] - linearRegressionCoefA - linearRegressionCoefB * listX[i]), 2);
            }
            return Math.Sqrt(sum / (listX.Count - 2));
        }

        private KeyValuePair<double, double> GetSigmas()
        {
            double sumX = 0;
            double sumY = 0;
            foreach (var item in listX)
            {
                sumX += Math.Pow(item - listX.Average(), 2);                
            }
            foreach (var item in listY)
            {
                sumY += Math.Pow(item - listY.Average(), 2);
            }
            var sigmaX = Math.Sqrt(sumX / listX.Count);
            var sigmaY = Math.Sqrt(sumY / listY.Count);
            return new KeyValuePair<double, double>(sigmaX, sigmaY);
        }
    }

    public class LinearRegressionModel
    {
        public double A { get; set; }
        public double B { get; set; }
    }
}
