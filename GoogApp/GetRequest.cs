using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoogApp
{
    public static class HttpExtensions
    {
        private static byte[] _readFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            var taskComplete = new TaskCompletionSource<HttpWebResponse>();
            request.BeginGetResponse(asyncResponse =>
            {
                try
                {
                    HttpWebRequest responseRequest = (HttpWebRequest)asyncResponse.AsyncState;
                    HttpWebResponse someResponse = (HttpWebResponse)responseRequest.EndGetResponse(asyncResponse);
                    taskComplete.TrySetResult(someResponse);
                }
                catch (WebException webExc)
                {
                    HttpWebResponse failedResponse = (HttpWebResponse)webExc.Response;
                    taskComplete.TrySetResult(failedResponse);
                }
            }, request);
            return taskComplete.Task;
        }

        public static Task<HttpWebResponse> GetRequestStreamAsync(this HttpWebRequest request, Stream stream)
        {
            var taskComplete = new TaskCompletionSource<HttpWebResponse>();
            request.BeginGetRequestStream(callbackResult =>
                {
                    try
                    {
                        HttpWebRequest responseRequest = (HttpWebRequest)callbackResult.AsyncState;
                        Stream postStream = responseRequest.EndGetRequestStream(callbackResult);
                        byte[] file = _readFully(stream);
                        postStream.Write(file, 0, file.Length);
                        postStream.Close();
                        //HttpWebResponse response = (HttpWebResponse)await responseRequest.GetResponseAsync();
                        responseRequest.BeginGetResponse((requestCallbackResult) =>
                            {
                                HttpWebRequest newRequest = (HttpWebRequest)requestCallbackResult.AsyncState;
                                HttpWebResponse response = (HttpWebResponse)newRequest.EndGetResponse(requestCallbackResult);
                                taskComplete.TrySetResult(response);
                            }, responseRequest);
                        
                    }
                    catch (WebException webExc)
                    {
                        HttpWebResponse failedResponse = (HttpWebResponse)webExc.Response;
                        taskComplete.TrySetResult(failedResponse);
                    }
                }, request);
            return taskComplete.Task;
        }
    }

    public static class HttpMethod
    {
        public static string Head { get { return "HEAD"; } }
        public static string Post { get { return "POST"; } }
        public static string Put { get { return "PUT"; } }
        public static string Get { get { return "GET"; } }
        public static string Delete { get { return "DELETE"; } }
        public static string Trace { get { return "TRACE"; } }
        public static string Options { get { return "OPTIONS"; } }
        public static string Connect { get { return "CONNECT"; } }
        public static string Patch { get { return "PATCH"; } }
    }
}
