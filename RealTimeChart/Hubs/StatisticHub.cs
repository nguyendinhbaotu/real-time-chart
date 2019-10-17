using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RealTimeChart.Services;
using System;
using System.Threading.Tasks;

namespace RealTimeChart.Hubs
{
    public class StatisticHub : Hub
    {
        private readonly ILogger<StatisticHub> _logger;
        private readonly IDataSourceService _dataSourceService;

        public StatisticHub(ILogger<StatisticHub> logger, IDataSourceService dataSourceService)
        {
            _logger = logger;
            _dataSourceService = dataSourceService;
        }

        public async Task GetInitData(string key = null)
        {
            try
            {
                await Clients.Client(Context.ConnectionId).SendAsync("OnInitDataSet", _dataSourceService.GetData(key), key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get init data");
                throw;
            }
        }

        public async Task GetDataStartEnd(DateTime? startTime = null, DateTime? endTime = null, string key = null)
        {
            try
            {
                var data = _dataSourceService.GetData(startTime, endTime, key);
                await Clients.Client(Context.ConnectionId).SendAsync("OnDataStartEnd", data, key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to scale data");
                throw;
            }
        }
    }
}
