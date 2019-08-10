using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace BeetleX.WebApiTester.Codes
{
    public class Loader
    {
        public Loader(Setting testSetting)
        {
            Setting = testSetting;
            All.Name = "All";
            Status = LoaderStatus.None;
        }

        private List<HttpTcpClient> httpTcpClients = new List<HttpTcpClient>();

        private Statistics All { get; set; } = new Statistics();

        public Setting Setting { get; set; }

        public List<TestRequestSetting> Items { get; set; } = new List<TestRequestSetting>();

        private long mIndex = 0;

        private TestRequestSetting GetTestRequestSetting()
        {
            long index = System.Threading.Interlocked.Increment(ref mIndex);
            return Items[(int)(index % Items.Count)];
        }

        public void Add(RequestSetting requestSetting)
        {
            Uri uri = new Uri(Setting.Host);
            TestRequestSetting item = new TestRequestSetting();
            requestSetting.Build(uri.Host, uri.Port);
            item.RequestSetting = requestSetting;
            item.Statistics.Name = requestSetting.BaseUrl;
            Items.Add(item);
        }

        public string Progress
        {
            get
            {
                if (Status == LoaderStatus.Error)
                    return Error.Message;
                if (Setting.Type == "time")
                {
                    return $"{(int)(TimeWatch.GetTotalSeconds() - mStartTime)}/{Setting.Value}秒";
                }
                else
                {
                    return $"{this.All.All.Count}/{Setting.Value}次";
                }
            }
        }

        public LoaderStatus Status { get; set; }

        public Exception Error { get; set; }

        private void CreateClient()
        {
            try
            {
                Uri uri = new Uri(Setting.Host);
                for (int i = 0; i < Setting.Connections; i++)
                {
                    HttpTcpClient httpTcpClient = new HttpTcpClient(uri.Host, uri.Port);
                    httpTcpClient.Client.Connect();
                    if (!httpTcpClient.Client.IsConnected)
                    {
                        throw httpTcpClient.Client.LastError;
                    }
                    httpTcpClients.Add(httpTcpClient);
                }
                Status = LoaderStatus.Runing;
            }
            catch (Exception e_)
            {
                Error = e_;
                Status = LoaderStatus.Error;
                foreach (var item in httpTcpClients)
                {
                    item.Dispose();
                }
                Status = LoaderStatus.Error;
            }
        }

        private double mStartTime;

        private long mTotalRequest;

        public void Stop()
        {
            Status = LoaderStatus.Completed;
        }

        private bool OnCompleted()
        {
            if (Status == LoaderStatus.Completed)
                return true;
            var completed = false;
            var count = System.Threading.Interlocked.Increment(ref mTotalRequest);
            if (Setting.Type == "time")
            {
                var time = TimeWatch.GetTotalSeconds();
                if (time - mStartTime > Setting.Value)
                {
                    completed = true;
                }
            }
            else
            {
                completed = count > Setting.Value;
            }
            if (completed)
                this.Status = LoaderStatus.Completed;
            return completed;
        }

        private async void OnTest(HttpTcpClient httpTcp)
        {
            try
            {
                while (!OnCompleted())
                {
                    var rsetting = GetTestRequestSetting();
                    Request request = new Request(httpTcp);
                    var result = await request.Execute(rsetting.RequestSetting);
                    All.Add(result.Code, result.Time);
                    rsetting.Statistics.Add(result.Code, result.Time);
                }
            }
            catch (Exception e_)
            {
                Error = e_;

            }
            finally
            {
                httpTcp.Dispose();
            }
        }

        private void OnRun()
        {
            if (Items.Count == 0)
            {
                Status = LoaderStatus.Error;
                Error = new Exception("没有可用的测试用例！");
                return;
            }
            CreateClient();

            mTotalRequest = 0;
            mStartTime = TimeWatch.GetTotalSeconds();
            foreach (var item in httpTcpClients)
            {
                OnTest(item);
            }
        }

        public void Run()
        {
            Status = LoaderStatus.Creating;
            Task.Run(() => OnRun());
        }

        public class TestRequestSetting
        {
            public RequestSetting RequestSetting { get; set; }

            public Statistics Statistics { get; set; } = new Statistics();
        }

        public enum LoaderStatus
        {
            None,
            Error,
            Creating,
            Runing,
            Completed,
        }

        public StatusInfo GetStatusInfo()
        {
            StatusInfo info = new StatusInfo();
            info.All = new StatsBaseItem(All.All.GetData());
            foreach (var item in Items)
            {
                UrlStats urlStats = new UrlStats();
                urlStats.ID = item.RequestSetting.ID;
                urlStats.Url = item.RequestSetting.BaseUrl;
                urlStats.Method = item.RequestSetting.Method;
                urlStats.Remark = item.RequestSetting.Remark;
                urlStats.All = new StatsBaseItem(item.Statistics.All.GetData()).PercentWith(info.All.value);
                urlStats.Codes.Add(new StatsBaseItem(item.Statistics.Status_2xx.GetData(), 1)
                    .PercentWith(urlStats.All.value).AddItems(item.Statistics.GetRegion(200, 300)));
                urlStats.Codes.Add(new StatsBaseItem(item.Statistics.Status_4xx.GetData(), 3)
                    .PercentWith(urlStats.All.value).AddItems(item.Statistics.GetRegion(400, 500)));
                urlStats.Codes.Add(new StatsBaseItem(item.Statistics.Status_5xx.GetData(), 4)
                    .PercentWith(urlStats.All.value).AddItems(item.Statistics.GetRegion(500, 600)));
                foreach (var t in item.Statistics.All.GetData().GetTimeCountStats())
                {

                    urlStats.Latency.Add(new StatsBaseItem(t).PercentWith(urlStats.All.value));

                }
                info.Urls.Add(urlStats);
            }

            info.Progress = this.Progress;
            info.Status = this.Status.ToString();

            info._1xx = new StatsBaseItem(All.Status_1xx.GetData(), 0);
            info._2xx = new StatsBaseItem(All.Status_2xx.GetData(), 1);
            info._3xx = new StatsBaseItem(All.Status_3xx.GetData(), 2);
            info._4xx = new StatsBaseItem(All.Status_4xx.GetData(), 3);
            info._5xx = new StatsBaseItem(All.Status_5xx.GetData(), 4);
            foreach (var item in All.All.GetData().GetTimeCountStats())
            {
                if (item.Count > 0)
                {
                    info.Latency.Add(new StatsBaseItem(item));
                }
            }
            return info;
        }

        public class StatusInfo
        {
            public string Status { get; set; }

            public string Progress { get; set; }

            public List<StatsBaseItem> Latency { get; set; } = new List<StatsBaseItem>();

            public List<UrlStats> Urls { get; set; } = new List<UrlStats>();

            public StatsBaseItem All { get; set; }

            public StatsBaseItem _1xx { get; set; }

            public StatsBaseItem _2xx { get; set; }

            public StatsBaseItem _3xx { get; set; }

            public StatsBaseItem _4xx { get; set; }

            public StatsBaseItem _5xx { get; set; }

        }

        public class UrlStats
        {
            public string ID { get; set; }

            public string Url { get; set; }

            public string Method { get; set; }

            public string Remark { get; set; }

            public StatsBaseItem All { get; set; }

            public List<StatsBaseItem> Codes { get; set; } = new List<StatsBaseItem>();

            public List<StatsBaseItem> Latency { get; set; } = new List<StatsBaseItem>();
        }
    }
}
