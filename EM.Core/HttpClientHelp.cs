using EM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EM.Public.Api.Framework
{
    /// <summary>
    /// httpclient help
    /// </summary>
    public static class HttpClientHelp
    {
        private static readonly HttpClient _client;

        static HttpClientHelp()
        {
            _client = new HttpClient(GetClientHandler());
          
        }

        private static HttpClientHandler GetClientHandler()
        {
            var reqHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false,
                 
            };

            return reqHandler;
        }

        /// <summary>
        /// 异步获取
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpmethod"></param>
        /// <param name="postdata"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> GetRequestAsync(string url, string httpmethod, string postdata, string contenttype, HttpClient client = null)
        {
            var httpclient = client ?? _client;

            if (httpmethod.ToLower() == "get")
                return httpclient.GetAsync(url, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
            else
            {
                var httprequestmsg = new HttpRequestMessage
                {
                    Method = new HttpMethod(httpmethod),
                    RequestUri = new Uri(url),
                    Content = new StringContent(postdata)
                };
                httprequestmsg.Content.Headers.ContentType = new MediaTypeHeaderValue(contenttype);
                return httpclient.SendAsync(httprequestmsg, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetRequest(string url, HttpClient client = null)
        {
            var httpclient = client ?? _client;
        
            return httpclient.GetAsync(url, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result;
        }

        public static HttpResponseMessage GetRequestByUTF8(string url, HttpClient client = null)
        {
            var httpclient = client ?? _client;

            var data = httpclient.GetByteArrayAsync(url).Result;
            var responseString = Encoding.UTF8.GetString(data, 0, data.Length);
            return new HttpResponseMessage { Content = new StringContent(responseString), StatusCode = System.Net.HttpStatusCode.OK };

        }

        public static HttpResponseMessage GetRequestByGB2312(string url, HttpClient client = null)
        {
            var httpclient = client ?? _client;

            var data = httpclient.GetByteArrayAsync(url).Result;
            var responseString = Encoding.GetEncoding("gb2312").GetString(data, 0, data.Length);
            return new HttpResponseMessage { Content = new StringContent(responseString), StatusCode = System.Net.HttpStatusCode.OK };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postdata"></param>
        /// <param name="contenttype"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static HttpResponseMessage PostRequest(string url, string postdata, string contenttype, HttpClient client = null)
        {
            try
            {
                var httpclient = client ?? _client;

                var httprequestmsg = new HttpRequestMessage
                {
                    Method = new HttpMethod("post"),
                    RequestUri = new Uri(url),
                    Content = new StringContent(postdata)
                };
                httprequestmsg.Content.Headers.ContentType = new MediaTypeHeaderValue(contenttype);
                return httpclient.SendAsync(httprequestmsg, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result;
            }
            catch (Exception ex)
            {
                ILogger logger = EngineContext.Current.Resolve<ILogger>();
                logger?.WriteErrorLog(url, ex);

                return null;
            }
        }

        public static Task<HttpResponseMessage> PostRequestasync(string url, string postdata, string contenttype, HttpClient client = null)
        {
            var httpclient = client ?? _client;

            var httprequestmsg = new HttpRequestMessage();
            httprequestmsg.Method = new HttpMethod("post");
            httprequestmsg.RequestUri = new Uri(url);
            httprequestmsg.Content = new StringContent(postdata);
            httprequestmsg.Content.Headers.ContentType = new MediaTypeHeaderValue(contenttype);
            return httpclient.SendAsync(httprequestmsg, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        }

        public static T PostRequest<T>(string url, string postdata, string contenttype, HttpClient client = null) where T : class
        {
            try
            {
                var httpclient = client ?? _client;

                var httprequestmsg = new HttpRequestMessage();
                httprequestmsg.Method = new HttpMethod("post");
                httprequestmsg.RequestUri = new Uri(url);
                httprequestmsg.Content = new StringContent(postdata);
                httprequestmsg.Content.Headers.ContentType = new MediaTypeHeaderValue(contenttype);
                var response = httpclient.SendAsync(httprequestmsg, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents);
            }
            catch (Exception ex)
            {
                ILogger logger = EngineContext.Current.Resolve<ILogger>();
                logger?.WriteErrorLog(url, ex);

                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="url"></param>
        /// <param name="postdata"></param>
        /// <param name="contenttype"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Task<T> PostRequestAsync<T, V>(string url, V postdata, string contenttype, HttpClient client = null)
            where T : class
            where V : class
        {
            var httpclient = client ?? _client;

            var httprequestmsg = new HttpRequestMessage
            {
                Method = new HttpMethod("post"),
                RequestUri = new Uri(url),
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postdata))
            };
            httprequestmsg.Content.Headers.ContentType = new MediaTypeHeaderValue(contenttype);
            httpclient.SendAsync(httprequestmsg, HttpCompletionOption.ResponseContentRead, CancellationToken.None).ContinueWith(
                (requesttask) =>
                {
                    var response = requesttask.Result;
                    response.EnsureSuccessStatusCode();
                    response.Content.ReadAsStringAsync().ContinueWith((readtask) =>
                    {
                        var contents = readtask.Result;
                        return Task.FromResult(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents));
                    });
                });

            return Task.FromResult(default(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Task<T> GetRequestAsync<T>(string url, HttpClient client = null)
        where T : class
        {
            var httpclient = client ?? _client;

            httpclient.GetAsync(url, HttpCompletionOption.ResponseContentRead, CancellationToken.None).ContinueWith(
                    (reqeusettask) =>
                    {
                        var response = reqeusettask.Result;
                        response.EnsureSuccessStatusCode();
                        response.Content.ReadAsStringAsync().ContinueWith((readtask) =>
                        {
                            var contents = readtask.Result;
                            return Task.FromResult(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents));
                        });
                    }, CancellationToken.None);

            return Task.FromResult(default(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="url"></param>
        /// <param name="postdata"></param>
        /// <param name="contenttype"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static T PostRequest<T, V>(string url, V postdata, string contenttype, HttpClient client = null)
          where T : class
          where V : class
        {
            try
            {
                var httpclient = client ?? _client;

                var httprequestmsg = new HttpRequestMessage
                {
                    Method = new HttpMethod("post"),
                    RequestUri = new Uri(url),
                    Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postdata))
                };
                httprequestmsg.Content.Headers.ContentType = new MediaTypeHeaderValue(contenttype);
                var response = httpclient.SendAsync(httprequestmsg, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result;
                response.EnsureSuccessStatusCode();
                var contents = response.Content.ReadAsStringAsync().Result;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents);
            }
            catch (Exception ex)
            {
                ILogger logger = EngineContext.Current.Resolve<ILogger>();
                logger?.WriteErrorLog(url, ex);

                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static T GetRequest<T>(string url, HttpClient client = null) where T : class
        {
            try
            {
                var httpclient = client ?? _client;
                var response = httpclient.GetAsync(url, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result;
                response.EnsureSuccessStatusCode();
                var contents = response.Content.ReadAsStringAsync().Result;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents);
            }
            catch (Exception ex)
            {
                ILogger logger = EngineContext.Current.Resolve<ILogger>();
                logger?.WriteErrorLog(url, ex);

                return null;
            }
        }
    }
}