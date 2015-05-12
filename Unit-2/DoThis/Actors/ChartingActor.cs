using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        private readonly Chart _chart;

        private Dictionary<string, Series> _seriesIndex;

        public ChartingActor(Chart chart)
            : this(chart, new Dictionary<string, Series>())
        {
        }

        private ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;

            Receive<InitializeChart>(initializeChart => HandleInitialize(initializeChart));
            Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(InitializeChart initializeChart)
        {
            if (initializeChart.InitialSeries != null)
            {
                //swap the two series out
                _seriesIndex = initializeChart.InitialSeries;
            }

            //delete any existing series
            _chart.Series.Clear();

            //attempt to render the initial chart
            if (_seriesIndex.Any())
            {
                foreach (var series in _seriesIndex)
                {
                    //force both the chart and the internal index to use the same names
                    series.Value.Name = series.Key;
                    _chart.Series.Add(series.Value);
                }
            }
        }

        private void HandleAddSeries(AddSeries series)
        {
            if (!string.IsNullOrEmpty(series.Series.Name) && !_seriesIndex.ContainsKey(series.Series.Name))
            {
                _seriesIndex.Add(series.Series.Name, series.Series);
                _chart.Series.Add(series.Series);
            }
        }

        #endregion

        #region Messages

        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries)
            {
                InitialSeries = initialSeries;
            }

            public Dictionary<string, Series> InitialSeries { get; private set; }
        }

        /// <summary>
        /// Add a new <see cref="Series"/> to the chart.
        /// </summary>
        public class AddSeries
        {
            public AddSeries(Series series)
            {
                Series = series;
            }

            public Series Series { get; private set; }
        }

        #endregion
    }
}