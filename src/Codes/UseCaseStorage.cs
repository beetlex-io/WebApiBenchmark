using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Linq;

namespace BeetleX.WebApiTester.Codes
{
    public class UseCaseStorage
    {
        const string STORAGE_FILE = "UseCase.json";

        private ConcurrentDictionary<string, RequestSetting> mRequestCase = new ConcurrentDictionary<string, RequestSetting>();

        private List<string> mServers = new List<string>();


        public object ListServer()
        {
            return mServers;
        }

        public void AddServer(string uri)
        {
            Uri item = new Uri(uri);
            if (!mServers.Contains(item.ToString()))
            {
                mServers.Add(item.ToString());
                Save();
            }
        }

        public void DeleteServer(string uri)
        {
            Uri item = new Uri(uri);
            mServers.Remove(item.ToString());
            Save();
        }

        public bool Modif(RequestSetting requestSetting)
        {
            bool result = mRequestCase.ContainsKey(requestSetting.ID);
            mRequestCase[requestSetting.ID] = requestSetting;
            Save();
            return result;
        }

        public void DeleteWithCategory(string category)
        {
            foreach (var item in mRequestCase.Values)
            {
                if (item.Category == category)
                {
                    mRequestCase.TryRemove(item.ID, out RequestSetting requestSetting);
                }
            }
            Save();
        }

        public void Delete(string id)
        {
            mRequestCase.TryRemove(id, out RequestSetting value);
            Save();
        }

        public RequestSetting Get(string id)
        {
            if (!mRequestCase.TryGetValue(id, out RequestSetting result))
                result = new RequestSetting();
            return result;
        }

        public IEnumerable<RequestSetting> List(string category = null)
        {
            IEnumerable<RequestSetting> items;

            if (string.IsNullOrEmpty(category))
                items = from a in mRequestCase.Values orderby a.Category ascending select a;
            else
                items = from a in mRequestCase.Values where a.Category == category orderby a.Category ascending select a;
            return items;
        }

        public void Save()
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(STORAGE_FILE, false, Encoding.UTF8))
            {
                DB dB = new DB();
                dB.Cases = mRequestCase.Values.ToList();
                dB.Server = mServers;
                string value = Newtonsoft.Json.JsonConvert.SerializeObject(dB);
                writer.Write(value);
                writer.Flush();
            }
        }

        private static UseCaseStorage mDefault;

        public static UseCaseStorage Default
        {
            get
            {
                if (mDefault == null)
                {
                    mDefault = new UseCaseStorage();
                    mDefault.Load();
                }
                return mDefault;
            }

        }

        public void Load()
        {
            if (System.IO.File.Exists(STORAGE_FILE))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(STORAGE_FILE))
                {
                    string value = reader.ReadToEnd();
                    var dB = Newtonsoft.Json.JsonConvert.DeserializeObject<DB>(value);
                    mServers = dB.Server;
                    foreach (var item in dB.Cases)
                        mRequestCase[item.ID] = item;
                }
            }
        }

        public class DB
        {
            public List<string> Server { get; set; }

            public List<RequestSetting> Cases { get; set; }
        }
    }
}
