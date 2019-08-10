using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeetleX.Buffers;
using BeetleX.FastHttpApi;

namespace BeetleX.WebApiTester.Codes
{
    [BeetleX.FastHttpApi.Controller]
    public class WebApi
    {

        public object ListServer()
        {
            return UseCaseStorage.Default.ListServer();
        }

        public void AddServer(string server)
        {
            UseCaseStorage.Default.AddServer(server);
        }

        public void DelServer(string server)
        {
            UseCaseStorage.Default.DeleteServer(server);
        }

        public RequestSetting Create()
        {
            return new RequestSetting();
        }

        public RequestSetting Get(string id)
        {
            return UseCaseStorage.Default.Get(id);

        }
        public void DeleteWithCategory(string category)
        {
            UseCaseStorage.Default.DeleteWithCategory(category);
        }
        public void Delete(string id)
        {
            UseCaseStorage.Default.Delete(id);
        }

        public bool Save(RequestSetting requestSetting)
        {
            return UseCaseStorage.Default.Modif(requestSetting);
        }

        public object ListCategory()
        {
            var items = UseCaseStorage.Default.List();

            var result = from a in items
                         group a by a.Category into g
                         select new { g.Key };
            return result;
        }

        public Object List(string category)
        {
            var items = UseCaseStorage.Default.List(category);

            var result = from a in items
                         group a by a.Category into g
                         select new { g.Key, Show = false, Items = from i in g.ToArray() orderby i.BaseUrl ascending select i };
            return from a in result orderby a.Key ascending select a;
        }

        private Loader mLoader;


        public object PerformanceTestStatus()
        {
            var result = mLoader?.GetStatusInfo();
            if (result != null && (mLoader.Status == Loader.LoaderStatus.Completed || mLoader.Status == Loader.LoaderStatus.Error))
            {
                mLoader.Stop();
                mLoader = null;
            }
            return result;
        }

        public async Task<int> UnitTest(string c, string host)
        {
            var requestSetting = UseCaseStorage.Default.Get(c);
            if (requestSetting == null)
                return 404;
            Uri uri = new Uri(host);
            using (HttpTcpClient httpTcpClient = new HttpTcpClient(uri.Host, uri.Port))
            {
                requestSetting.Build(uri.Host, httpTcpClient.Port);
                Request request = new Request(httpTcpClient);
                var result = await request.Execute(requestSetting);
                return result.Code;
            }
        }

        public void PerformanceTestStop()
        {
            mLoader?.Stop();
        }

        public void PerformanceTest(Setting setting, string[] cases)
        {
            if (mLoader != null)
            {
                mLoader.Stop();
            }
            mLoader = new Loader(setting);
            foreach (var c in cases)
            {
                var item = UseCaseStorage.Default.Get(c);
                if (item != null)
                {
                    mLoader.Add(item);
                }
            }
            mLoader.Run();
        }

        public async Task<Request.TestResult> TestCase(RequestSetting requestSetting, string host)
        {
            Uri uri = new Uri(host);
            using (HttpTcpClient httpTcpClient = new HttpTcpClient(uri.Host, uri.Port))
            {
                requestSetting.Build(uri.Host, httpTcpClient.Port);
                Request request = new Request(httpTcpClient);
                var result = await request.Execute(requestSetting);
                return request.GetResult();
            }

        }

        public object Download()
        {
            var items = UseCaseStorage.Default.List(null);
            string text = Newtonsoft.Json.JsonConvert.SerializeObject(items);
            return new DownLoad(text);
        }

        public object GetFileID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public object Upload(string name, bool completed, string data)
        {
            byte[] array = Convert.FromBase64String(data);
            using (System.IO.Stream writer = System.IO.File.Open(name, System.IO.FileMode.Append))
            {
                writer.Write(array, 0, array.Length);
                writer.Flush();
            }
            if (completed)
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(name))
                {
                    string value = reader.ReadToEnd();
                    List<RequestSetting> cases = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RequestSetting>>(value);
                    foreach (var item in cases)
                    {
                        UseCaseStorage.Default.Modif(item);
                    }
                }
                System.IO.File.Delete(name);
            }
            return true;
        }

        public class DownLoad : BeetleX.FastHttpApi.IResult
        {
            public DownLoad(string text)
            {
                Text = text;
            }

            public string Text { get; set; }



            public int Length { get; set; }

            public bool HasBody => true;

            public IHeaderItem ContentType => BeetleX.FastHttpApi.ContentTypes.OCTET_STREAM;

            public void Setting(HttpResponse response)
            {
                response.Header.Add("Content-Disposition", "attachment;filename=TestCases.json");
            }

            public void Write(PipeStream stream, HttpResponse response)
            {
                stream.Write(Text);
            }
        }

    }
}
