using BeetleX.Buffers;
using BeetleX.Clients;
using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.WebApiTester.Codes
{
    public class Request
    {

        public Request(HttpTcpClient clientAgent)
        {
            mTransferEncoding = false;
            mRequestLength = 0;
            Code = 0;
            mClient = clientAgent;
            mClient.Client.ClientError = OnSocketError;
            mClient.Client.DataReceive = OnReveive;
            mBuffer = mClient.Buffer;
            Status = RequestStatus.None;
            mStartTime = TimeWatch.GetElapsedMilliseconds();
            mRequestID = System.Threading.Interlocked.Increment(ref mRequestIDSqueue);
        }



        public const int REQUEST_WRITE_ERROR = 591;

        public const int RECEIVE_SOCKET_ERROR = 592;

        public const int RECEIVE_PROCESS_ERROR = 593;

        public const int RECEIVE_DATA_ERROR = 594;

        private System.Threading.Tasks.TaskCompletionSource<Request> taskCompletionSource;

        public List<Property> Header { get; set; } = new List<Property>();

        private HttpTcpClient mClient;

        public HttpTcpClient Client => mClient;

        private static long mRequestIDSqueue;

        private long mStartTime;

        private long mRequestID;

        private byte[] mBuffer;

        private int mRequestLength;

        private bool mTransferEncoding = false;

        public long Time { get; set; }

        public string Error { get; set; }

        public int Code { get; set; }

        public RequestStatus Status { get; set; }

        private void OnSocketError(IClient c, ClientErrorArgs e)
        {
            Error = e.Error.Message;
            if (Status == RequestStatus.Requesting)
            {
                if (e.Error is SocketException)
                {
                    Code = RECEIVE_SOCKET_ERROR;

                }
                else
                {
                    Code = RECEIVE_PROCESS_ERROR;
                }
                OnCompleted();
            }
            else
            {
                Code = RECEIVE_DATA_ERROR;
                if (Status > RequestStatus.None)
                {
                    OnCompleted();
                }
            }
        }

        private void ResponseStatus(PipeStream pipeStream)
        {
            if (Status == RequestStatus.Responding)
            {
                var indexof = pipeStream.IndexOf(HeaderTypeFactory.LINE_BYTES);
                if (indexof.EofData != null)
                {
                    pipeStream.Read(mBuffer, 0, indexof.Length);

                    var result = HttpParse.AnalyzeResponseLine(new ReadOnlySpan<byte>(mBuffer, 0, indexof.Length - 2));
                    Code = result.Item2;
                    Status = RequestStatus.RespondingHeader;
                }
            }
        }

        private void ResponseHeader(PipeStream pipeStream)
        {
            if (Status == RequestStatus.RespondingHeader)
            {
                var indexof = pipeStream.IndexOf(HeaderTypeFactory.LINE_BYTES);
                while (indexof.End != null)
                {
                    pipeStream.Read(mBuffer, 0, indexof.Length);
                    if (indexof.Length == 2)
                    {
                        Status = RequestStatus.RespondingBody;
                        return;
                    }
                    else
                    {
                        var header = HttpParse.AnalyzeHeader(new ReadOnlySpan<byte>(mBuffer, 0, indexof.Length - 2));
                        Header.Add(new Property { Name = header.Item1, Value = header.Item2 });
                        if (string.Compare(header.Item1, HeaderTypeFactory.TRANSFER_ENCODING, true) == 0 && string.Compare(header.Item2, "chunked", true) == 0)
                        {
                            mTransferEncoding = true;
                        }
                        if (string.Compare(header.Item1, HeaderTypeFactory.CONTENT_LENGTH, true) == 0)
                        {
                            mRequestLength = int.Parse(header.Item2);
                        }
                    }
                    indexof = pipeStream.IndexOf(HeaderTypeFactory.LINE_BYTES);
                }
            }
        }

        private void ResponseBody(PipeStream pipeStream)
        {
            if (Status == RequestStatus.RespondingBody)
            {
                if (mTransferEncoding)
                {
                    var lastbuffer = pipeStream.LastBuffer;
                    var data = lastbuffer.Data;
                    bool end = true;
                    for (int i = 0; i < 5; i++)
                    {
                        if (HeaderTypeFactory.CHUNKED_BYTES[i] != data[lastbuffer.Length - 5 + i])
                        {
                            end = false;
                            break;
                        }
                    }
                    if (end)
                    {
                        OnCompleted();
                    }
                }
                else
                {
                    if (mRequestLength == 0)
                    {
                        OnCompleted();
                    }
                    else
                    {
                        if (mRequestLength == pipeStream.Length)
                        {
                            OnCompleted();
                        }
                    }
                }
            }
        }

        private void OnReveive(IClient c, ClientReceiveArgs reader)
        {
            PipeStream stream = reader.Stream.ToPipeStream();
            if (Status >= RequestStatus.Responding)
            {
                ResponseStatus(stream);
                ResponseHeader(stream);
                ResponseBody(stream);
            }
            else
            {
                stream.ReadFree((int)stream.Length);
            }
        }

        public Task<Request> Execute(RequestSetting requestSetting)
        {

            taskCompletionSource = new System.Threading.Tasks.TaskCompletionSource<Request>();
            Status = RequestStatus.Requesting;
            mClient.Client.Connect();
            if (mClient.Client.IsConnected)
            {
                try
                {
                    PipeStream pipeStream = mClient.Client.Stream.ToPipeStream();
                    Status = RequestStatus.Responding;
                    requestSetting.Write(pipeStream);
                    mClient.Client.Stream.Flush();
                }
                catch (Exception e_)
                {
                    try
                    {
                        if (mClient.Client != null)
                            mClient.Client.DisConnect();
                        Code = REQUEST_WRITE_ERROR;
                    }
                    finally
                    {
                        OnCompleted();
                    }
                }
            }
            return taskCompletionSource.Task;
        }

        private int mCompletedStatus = 0;

        private void OnCompleted()
        {
            if (System.Threading.Interlocked.CompareExchange(ref mCompletedStatus, 1, 0) == 0)
            {
                Time = TimeWatch.GetElapsedMilliseconds() - mStartTime;
                mClient.Client.ClientError = null;
                mClient.Client.DataReceive = null;
                try
                {
                    taskCompletionSource.SetResult(this);
                }

                finally
                {
                    if (mClient.Client.IsConnected)
                    {
                        PipeStream pipeStream = mClient.Client.Stream.ToPipeStream();
                        if (pipeStream.Length > 0)
                            pipeStream.ReadFree((int)pipeStream.Length);
                    }

                }

            }
        }


        public class TestResult
        {
            public bool IsJSON { get; set; } = false;

            public string Code { get; set; }

            public string Header { get; set; } = "";

            public string Body { get; set; } = "";

        }

        public TestResult GetResult()
        {
            TestResult result = new TestResult();
            result.Code = Code.ToString();
            foreach (var item in Header)
            {
                result.Header += ($"{item.Name}= {item.Value}\r\n");
                if (item.Name.ToLower() == "content-type" && item.Value.ToLower().IndexOf("application/json") >= 0)
                {
                    result.IsJSON = true;
                }
            }
            if (mClient.Client.IsConnected)
            {
                if (mRequestLength > 0 || Client.Client.Stream.ToPipeStream().Length > 0)
                {
                    result.Body = (Client.Client.Stream.ToPipeStream().ReadToEnd());
                }
            }
            return result;
        }


        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"status:{Code}");
            if (!string.IsNullOrEmpty(Error))
            {
                stringBuilder.AppendLine($"Error: {Error}");
            }
            foreach (var item in Header)
            {
                stringBuilder.AppendLine($"{item.Name}= {item.Value}");
            }
            stringBuilder.AppendLine("");
            if (mClient.Client.IsConnected)
            {
                if (mRequestLength > 0 || Client.Client.Stream.ToPipeStream().Length > 0)
                {
                    stringBuilder.Append(Client.Client.Stream.ToPipeStream().ReadToEnd());
                }
            }

            return stringBuilder.ToString();
        }


        public enum RequestStatus : int
        {
            None = 1,
            Requesting = 2,
            Responding = 8,
            RespondingHeader = 32,
            RespondingBody = 64
        }
    }
}
