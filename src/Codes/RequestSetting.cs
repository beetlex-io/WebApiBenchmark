using BeetleX.Buffers;
using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.WebApiTester.Codes
{
    public class RequestSetting
    {

        public RequestSetting()
        {
            BaseUrl = "/";
            Method = "GET";
            QueryString = new List<Property>();
            Header = new List<Property>();
            ContentType = "application/json";
            Protocol = "HTTP/1.1";
            ID = Guid.NewGuid().ToString("N");
            Category = "Default";
        }

        public string Category { get; set; }

        public string Remark { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        private byte[] mContentTypeData;

        private byte[] mBodyData;

        private byte[] mContentLengthData;

        private UrlBuiler urlBuiler = new UrlBuiler();

        private Property mHost = null;

        public string Protocol { get; set; }

        public string BaseUrl { get; set; }

        public string Method { get; set; }

        public List<Property> QueryString { get; set; }

        public List<Property> Header { get; set; }

        public string ContentType { get; set; }

        public string Body { get; set; }

        public void Build(string host, int port)
        {
            if (Header == null)
                Header = new List<Property>();
            if (Header.Find(i => i.Name.ToLower() == "host") == null)
            {
                if (port == 80 || port == 443)
                    mHost = new Property { Name = "Host", Value = host };
                else
                    mHost = new Property { Name = "Host", Value = $"{host}:{port}" };
                mHost.Build();
            }

            Header.RemoveAll(p => string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value));
            if (QueryString != null)
                QueryString.RemoveAll(p => string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value));
            urlBuiler.BaseUrl = BaseUrl;
            urlBuiler.QueryString = QueryString;
            urlBuiler.Method = Method;
            urlBuiler.Protocol = Protocol;
            urlBuiler.Builder();
            if (!string.IsNullOrEmpty(Body))
            {
                mBodyData = Encoding.UTF8.GetBytes(Body);
                mContentLengthData = Encoding.UTF8.GetBytes($"Content-Length: {mBodyData.Length}\r\n");
            }
            else
            {
                mContentLengthData = Encoding.UTF8.GetBytes($"Content-Length: 0\r\n");
            }

            mContentTypeData = Encoding.UTF8.GetBytes($"Content-Type: {ContentType}\r\n");
            if (Header != null)
            {
                foreach (var item in Header)
                {
                    item.Build();
                }
            }
        }

        public void Write(PipeStream stream)
        {
            urlBuiler.Writer(stream);
            if (Header != null)
            {
                foreach (var item in Header)
                {
                    item.Write(stream);
                }
            }
            if (mHost != null)
                mHost.Write(stream);
            stream.Write(mContentTypeData, 0, mContentTypeData.Length);
            if ((Method == "POST" || Method == "PUT"))
            {
                stream.Write(mContentLengthData, 0, mContentLengthData.Length);
            }
            stream.Write("\r\n");
            if (!string.IsNullOrEmpty(Body) && (Method == "POST" || Method == "PUT"))
            {

                stream.Write(mBodyData, 0, mBodyData.Length);
            }
        }

    }

    public class Property
    {
        public string Name { get; set; }

        public string Value { get; set; }

        private List<IOutputTemplate> outputTemplates = new List<IOutputTemplate>();

        public override string ToString()
        {
            return $"{Name}={System.Web.HttpUtility.UrlEncode(Value)}";
        }

        public void Write(PipeStream stream)
        {
            for (int i = 0; i < outputTemplates.Count; i++)
            {
                outputTemplates[i].Writer(stream);
            }
        }

        public void Build()
        {
            outputTemplates.Clear();
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Value))
            {
                outputTemplates.Add(new StringTemplate($"{Name}: "));
                outputTemplates.Add(new StringTemplate(Value));
                outputTemplates.Add(new StringTemplate("\r\n"));
            }
        }

        public List<IOutputTemplate> GetOutputers(bool querystring)
        {
            List<IOutputTemplate> outputers = new List<IOutputTemplate>();
            outputers.Add(new StringTemplate(Name));
            if (querystring)
                outputers.Add(new StringTemplate("="));
            else
                outputers.Add(new StringTemplate(":"));
            if (querystring)
                outputers.Add(new StringTemplate(System.Web.HttpUtility.UrlEncode(Value)));
            else
                outputers.Add(new StringTemplate(Value));
            return outputers;
        }
    }

    public class UrlBuiler
    {
        public string Protocol { get; set; }

        public string BaseUrl { get; set; }

        public string Method { get; set; }

        public List<Property> QueryString { get; set; }

        private List<IOutputTemplate> outputTemplates = new List<IOutputTemplate>();

        public void Builder()
        {
            outputTemplates.Clear();
            outputTemplates.Add(new StringTemplate(Method + " " + BaseUrl));
            string url = BaseUrl;
            if (QueryString != null)
            {
                for (int i = 0; i < QueryString.Count; i++)
                {
                    if (i == 0)
                    {
                        if (BaseUrl.IndexOf("?") >= 0)
                            outputTemplates.Add(new StringTemplate("&"));
                        else
                            outputTemplates.Add(new StringTemplate("?"));
                    }
                    else
                    {
                        outputTemplates.Add(new StringTemplate("&"));
                    }
                    outputTemplates.AddRange(QueryString[i].GetOutputers(true));
                }
            }
            outputTemplates.Add(new StringTemplate($" {Protocol}\r\n"));
        }

        public void Writer(PipeStream stream)
        {
            for (int i = 0; i < outputTemplates.Count; i++)
            {
                outputTemplates[i].Writer(stream);
            }
        }
    }

    public interface IOutputTemplate
    {
        void Writer(PipeStream stream);
    }

    public class BytesTemplate : IOutputTemplate
    {
        public byte[] Data { get; set; }

        public void Writer(PipeStream stream)
        {
            stream.Write(Data, 0, Data.Length);
        }
    }

    public class StringTemplate : BytesTemplate
    {
        public StringTemplate(string text)
        {
            Data = Encoding.UTF8.GetBytes(text);
        }
    }

    public class StringArrayTemplate : IOutputTemplate
    {
        public StringArrayTemplate(string[] data)
        {
            Data = data;
        }

        private long mIndex = 0;

        public string[] Data { get; set; }

        public void Writer(PipeStream stream)
        {
            if (Data != null && Data.Length > 0)
            {
                long index = System.Threading.Interlocked.Increment(ref mIndex);
                string value = Data[index % Data.Length];
                stream.Write(value);
            }
        }
    }

    public class DateTemplate : IOutputTemplate
    {
        public DateTemplate()
        {

        }

        public string Formater { get; set; }

        public void Writer(PipeStream stream)
        {
            DateTime dt = DateTime.Now;
            string value = string.IsNullOrEmpty(Formater) ? dt.ToShortDateString() : dt.ToString(Formater);
            stream.Write(value);
        }
    }


    public class Ran : IOutputTemplate
    {
        public Ran()
        {
            Min = 0;
            Max = 1000000;
        }
        public int Min { get; set; }

        public int Max { get; set; }

        public void Writer(PipeStream stream)
        {
            Random ran = new Random(Environment.TickCount);
            string value = ran.Next(Min, Max).ToString();
            stream.Write(value);
        }
    }



}
