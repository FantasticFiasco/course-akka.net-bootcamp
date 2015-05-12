using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;
using Akka.Util.Internal;
using ChartApp.Actors;

namespace ChartApp
{
    public partial class Main : Form
    {
        private readonly AtomicCounter _seriesCounter = new AtomicCounter(1);
        private IActorRef _chartActor;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _chartActor = Program.ChartActors.ActorOf(
                Props.Create(() => new ChartingActor(sysChart)),
                "charting");

            var series = ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement());
            
            _chartActor.Tell(
                new ChartingActor.InitializeChart(
                    new Dictionary<string, Series>
                    {
                        { series.Name, series }
                    }));
        }

        private void AddSeriesButton_Click(object sender, EventArgs e)
        {
            var series = ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement());
            _chartActor.Tell(new ChartingActor.AddSeries(series));
        }
    }
}