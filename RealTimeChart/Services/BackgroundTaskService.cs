using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RealTimeChart.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeChart.Services
{
    public class BackgroundTaskService : IHostedService
    {
        private Timer _timer;
        private readonly IHubContext<StatisticHub> _hubContext;
        private IDataSourceService _dataSourceService;

        public BackgroundTaskService(IHubContext<StatisticHub> hubContext, IDataSourceService dataSourceService)
        {
            _hubContext = hubContext;
            _dataSourceService = dataSourceService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //var newRecord = new List<decimal>();
            //newRecord.Add((decimal)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds());
            //newRecord.Add((decimal)(new Random().NextDouble() * 1));
            //_dataSourceService.AddItem(newRecord);
            //_hubContext.Clients.All.SendAsync("OnChartDataUpdate", newRecord);

            for (int key = 0; key < Constants.NUMBER_OF_CHARTS; key++)
            {
                var newRecord = _dataSourceService.GenerateNewRecord(key.ToString());
                //_hubContext.Clients.All.SendAsync("OnChartDataUpdate", newRecord, key);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
