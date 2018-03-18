using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace empilab
{
    public partial class Form1 : Form
    {
        private DataProvider dataProvider;
        private FileReader fileReader;
        private FirstLevelStat firstLevelStat;
        private CorrelationStat correlationStat;
        private IEnumerable<string> fileContent;
        private IEnumerable<Model> listCorrelationData;
        public Form1()
        {
            InitializeComponent();
            dataProvider = new DataProvider();
            fileReader = new FileReader();
            firstLevelStat = new FirstLevelStat();
            correlationStat = new CorrelationStat();
        }

        private void ProvideFirstLevelStatAnalytics()
        {
            listCorrelationData = dataProvider.Parse(fileContent);
            firstLevelStat.Activate(listCorrelationData);

            FillTextBoxes();
        }

        private void FillTextBoxes()
        {
            // average
            txtAverageMarkX.Text = firstLevelStat.GetAverage().Key.ToString();
            txtAverageMarkY.Text = firstLevelStat.GetAverage().Value.ToString();
            txtLowAverageX.Text = firstLevelStat.GetLowLimit(firstLevelStat.GetAverage().Key, firstLevelStat.GetDeviationForAvarageX()).ToString();
            txtLowAverageY.Text = firstLevelStat.GetLowLimit(firstLevelStat.GetAverage().Value, firstLevelStat.GetDeviationForAvarageY()).ToString();
            txtHighAverageX.Text = firstLevelStat.GetHighLimit(firstLevelStat.GetAverage().Key, firstLevelStat.GetDeviationForAvarageX()).ToString();
            txtHighAverageY.Text = firstLevelStat.GetHighLimit(firstLevelStat.GetAverage().Value, firstLevelStat.GetDeviationForAvarageY()).ToString();

            //deviation
            txtDeviationX.Text = firstLevelStat.GetDeviation().Key.ToString();
            txtDeviationY.Text = firstLevelStat.GetDeviation().Value.ToString();
            txtLowDeviationX.Text = firstLevelStat.GetLowLimit(firstLevelStat.GetDeviation().Key, firstLevelStat.GetDeviationForDeviationX()).ToString();
            txtLowDeviationY.Text = firstLevelStat.GetLowLimit(firstLevelStat.GetDeviation().Value, firstLevelStat.GetDeviationForDeviationY()).ToString();
            txtHighDeviationX.Text = firstLevelStat.GetHighLimit(firstLevelStat.GetDeviation().Key, firstLevelStat.GetDeviationForDeviationX()).ToString();
            txtHighDeviationY.Text = firstLevelStat.GetHighLimit(firstLevelStat.GetDeviation().Value, firstLevelStat.GetDeviationForDeviationY()).ToString();

            //assemetry
            txtAssemetryX.Text = firstLevelStat.GetAssemetry().Key.ToString();
            txtAssemetryY.Text = firstLevelStat.GetAssemetry().Value.ToString();
            txtLowAssemetryX.Text = firstLevelStat.GetLowLimit(firstLevelStat.GetAssemetry().Key, firstLevelStat.GetDeviationForAssemetry()).ToString();
            txtLowAssemetryY.Text = firstLevelStat.GetLowLimit(firstLevelStat.GetAssemetry().Value, firstLevelStat.GetDeviationForAssemetry()).ToString();
            txtHighAssemetryX.Text = firstLevelStat.GetHighLimit(firstLevelStat.GetAssemetry().Key, firstLevelStat.GetDeviationForAssemetry()).ToString();
            txtHighAssemetryY.Text = firstLevelStat.GetHighLimit(firstLevelStat.GetAssemetry().Value, firstLevelStat.GetDeviationForAssemetry()).ToString();

            //kurtosis
            txtKurtosisX.Text = firstLevelStat.GetKurtosis().Key.ToString();
            txtKurtosisY.Text = firstLevelStat.GetKurtosis().Value.ToString();
            txtLowKurtosisX.Text = firstLevelStat.GetLowLimit(firstLevelStat.GetKurtosis().Key, firstLevelStat.GetDeviationForKurtosis()).ToString();
            txtLowKurtosisY.Text = firstLevelStat.GetLowLimit(firstLevelStat.GetKurtosis().Value, firstLevelStat.GetDeviationForKurtosis()).ToString();
            txtHighKurtosisX.Text = firstLevelStat.GetHighLimit(firstLevelStat.GetKurtosis().Key, firstLevelStat.GetDeviationForKurtosis()).ToString();
            txtHighKurtosisY.Text = firstLevelStat.GetHighLimit(firstLevelStat.GetKurtosis().Value, firstLevelStat.GetDeviationForKurtosis()).ToString();
        }

        private void btnDownloadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    fileContent = fileReader.Read(dialog.FileName);
                }
            }
            ProvideFirstLevelStatAnalytics();
            BuildCorrelationChart();
            ProvideCorrelationStat();
        }

        private void ProvideCorrelationStat()
        {
            correlationStat.Activate(listCorrelationData);
            FillTextBoxesCorrelation();
        }

        private void FillTextBoxesCorrelation()
        {
            var alpha = 0.1;
            var normalQuantile = Math.Round(correlationStat.BuildNormalQuantile(1 - alpha / 2), 4);
            // pair correlation
            var pairCorrelation = correlationStat.GetCoefPairCorrelation(firstLevelStat.GetAverage().Key, firstLevelStat.GetAverage().Value);
            var pairCorrelationStat = correlationStat.GetStatisticForPairCorrelation(pairCorrelation);
            var pairCorrelationQuantile = correlationStat.GetQuantileStudent(1 - alpha / 2, correlationStat.listModels.Count() - 2);
            var pairCorrelationSignificance = correlationStat.GetSignificanceForPairCorrelation(pairCorrelationStat, pairCorrelationQuantile);
            txtPairCorrelationValue.Text = pairCorrelation.ToString();
            txtPairCorrelationStat.Text = pairCorrelationStat.ToString();
            txtPairCorrelationQuantile.Text = pairCorrelationQuantile.ToString();
            txtSignificance.Text = pairCorrelationSignificance.ToString();
            txtLowLimitPairCorrelation.Text = correlationStat.GetLowLimitForPairCorrelation(pairCorrelation, correlationStat.BuildNormalQuantile(1 - alpha / 2)).ToString();
            txtHighLimitPairCorrelation.Text = correlationStat.GetHighLimitForPairCorrelation(pairCorrelation, correlationStat.BuildNormalQuantile(1 - alpha / 2)).ToString();

            // correlation ratio
            var correlationRatio = correlationStat.GetCorrelationRelation();
            var correlationRatioStat = correlationStat.GetStatisticCorrelationRatio(correlationRatio);
            var quantileFisher = correlationStat.GetQuantileFisher(1 - alpha, 60 - 2, correlationStat.listModels.Count() - 60);
            txtCorrelationRatio.Text =  correlationRatio.ToString();
            txtCorrelationRatioStat.Text = correlationRatioStat.ToString();
            txtCorrelationRatioQuantile.Text = quantileFisher.ToString();
            txtCorrelationRatioSign.Text = correlationStat.GetSignificanceForCorrelationRatio(correlationRatioStat).ToString();

            // spirman coef
            txtSpiramnCoef.Text = correlationStat.GetSpirmanCoef().ToString();
            txtSpirmanStat.Text = correlationStat.GetSpirmanStat(correlationStat.GetSpirmanCoef()).ToString();
            txtSpirmanCoefQuantile.Text = pairCorrelationQuantile.ToString();
            txtSpirmanSign.Text = correlationStat.GetSignificanceForSpirmanCoef(correlationStat.GetSpirmanStat(correlationStat.GetSpirmanCoef()),pairCorrelationQuantile).ToString();

            // kandela coef
            txtKandelaCoef.Text = correlationStat.GetKandelaCoef().ToString();
            txtCandelaStat.Text = correlationStat.GetCandelaStat(correlationStat.GetKandelaCoef()).ToString();
            txtCandelaSign.Text = correlationStat.GetSignificanceForCandelaCoef(correlationStat.GetCandelaStat(correlationStat.GetKandelaCoef())).ToString();
            txtCandelaCoefQuantile.Text = normalQuantile.ToString();
        }

        private void BuildCorrelationChart()
        {
            foreach (var item in listCorrelationData)
            {
                chart1.Series[0].Points.AddXY(item.X, item.Y);
            }
        }
    }
}
