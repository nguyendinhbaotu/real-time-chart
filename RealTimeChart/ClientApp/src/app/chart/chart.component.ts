import { Component, OnInit, Input, OnChanges, SimpleChange, SimpleChanges } from '@angular/core';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import * as Highcharts from 'highcharts';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
})
export class ChartComponent implements OnInit, OnChanges {
  @Input() key: string;
  @Input() title: string;

  colors = ['lightcoral', 'sandybrown', 'darkkhaki', 'darkseagreen', 'lightseagreen', 'slateblue', 'darkorchid', 'crimson', 'tan'];
  types = ['area', 'column', 'spline', 'spline', 'spline', 'spline', 'spline'];
  chart: Highcharts.Chart;
  connection: HubConnection;
  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {
    series: [{
      color: 'black',
      type: 'spline'
    }],
    chart: {
      zoomType: 'x',
      backgroundColor: '#2d3035'
    },
    rangeSelector: {
      buttons: [{
        type: 'hour',
        count: 1,
        text: '1h'
      }, {
        type: 'day',
        count: 1,
        text: '1d'
      }, {
        type: 'month',
        count: 1,
        text: '1m'
      }, {
        type: 'year',
        count: 1,
        text: '1y'
      }, {
        type: 'all',
        text: 'All'
      }],
      inputEnabled: false, // it supports only days
      selected: 4 // all
    },
    xAxis: {
      type: 'datetime',
      // labels: {
      //   style: {
      //     color: 'lightcoral'
      //   }
      // },
      events: {
        afterSetExtremes: (e) => this.afterSetExtremes(e)
      },
      minRange: 10000
    }
  };

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    const { key, title } = changes;
    if (key) {
      this.chartOptions = Object.assign({}, this.chartOptions, {
        series: [{
          color: this.colors[this.key] ? this.colors[this.key] : 'black',
          type: this.types[this.key] ? this.types[this.key] : 'spline',
        }]
      });
    }
    if (title) {
      this.chartOptions = Object.assign({}, this.chartOptions, {
        title: { text: this.title, style: { fontWeight: 'bold' } }
      });
    }
  }

  ngOnInit() {
    this.connection = new HubConnectionBuilder().withUrl('/real-time-data').build();

    this.connection.on('OnChartDataUpdate', (data: [number, number], key: string) => {
      if (+key === +this.key) {
        this.chart.series[0].removePoint(0);
        this.chart.series[0].addPoint(data);
        console.log(`Chart ${this.key} is updated with new record ${data}`);
      }
    });

    this.connection.on('OnInitDataSet', (data: [[number, number]], key: string) => {
      if (+key === +this.key) {
        this.generateChart(data);
        console.log(`Chart ${this.key} is inited.`);
      }
    });

    this.connection.on('OnDataStartEnd', (data: [[number, number]], key: string) => {
      if (+key === +this.key) {
        this.generateChart(data);
        console.log(`Chart ${this.key} is updated with time range.`);
      }
    });

    this.connection.start().then(() => this.connection.invoke('GetInitData', this.key));

    setTimeout(() => {
      const extremes = this.chart.xAxis[0].getExtremes();
      this.chart.zoomOut = () => {
        this.chart.xAxis[0].setExtremes(extremes.min, extremes.max);
      };
    });

  }

  logChartInstance(chart: Highcharts.Chart) {
    this.chart = chart;
  }

  private afterSetExtremes(e) {
    if (e.trigger) {
      this.chart.showLoading('Loading data from server...');
      this.connection.invoke('GetDataStartEnd', Math.round(e.min), Math.round(e.max), this.key);
    }
  }

  private generateChart(data) {
    this.chart.series[0].setData(data);
    this.chart.hideLoading();
  }
}
