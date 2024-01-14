using Newtonsoft.Json;
using PaymentExternalLib.Models;
using Polly.Retry;
using System.Diagnostics;
using System.Dynamic;
using System.Net;
using System.Text;

namespace PaymentExternalLib.Utils
{
    public static class GenericConsumeHelper 
    {

        #region Methods

        public static async Task<ResponseWebServiceDTO> GetConsume(RequestWebServiceDTO data, AsyncRetryPolicy<HttpResponseMessage> policyRetry, int minsTimeOut=5)
        {
            if (string.IsNullOrEmpty(data.UrlExternalService))
                throw new ArgumentNullException(nameof(data.UrlExternalService));

            try
            {
                string method = "GET";
                double timeout = minsTimeOut;

                HttpMethod httpMethod = new HttpMethod(method);
                return await ConsumeService(data.UrlExternalService, httpMethod, data, null, timeout, policyRetry);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<ResponseWebServiceDTO> PostConsume(RequestWebServiceDTO data, AsyncRetryPolicy<HttpResponseMessage> policyRetry, int minsTimeOut=5)
        {
            if (string.IsNullOrEmpty(data.UrlExternalService))
                throw new ArgumentNullException(nameof(data.UrlExternalService));

            try
            {
                string method = "POST";
                double timeout = minsTimeOut;

                HttpMethod httpMethod = new HttpMethod(method);
                return await ConsumeService(data.UrlExternalService, httpMethod, data, null, timeout, policyRetry);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private methods
        private static async Task<ResponseWebServiceDTO> ConsumeService(string url, HttpMethod httpMethod, dynamic data, List<KeyValuePair<String, String>> headers, double timeout, AsyncRetryPolicy<HttpResponseMessage> policyRetry)
        {
            double min = timeout;
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (HttpClient Client = new HttpClient(httpClientHandler) { Timeout = TimeSpan.FromMinutes(min) })
                {
                    #region en Caso sea GET
                    if (httpMethod == HttpMethod.Get)
                    {
                        url = $"{url}?";
                        if(data is ExpandoObject)
                        {
                            var dataDict = (IDictionary<string, object>)data;

                            foreach (var kvp in dataDict)
                            {
                                string fieldName = kvp.Key;
                                string fieldValue = kvp.Value?.ToString();
                                if (!string.IsNullOrEmpty(fieldValue))
                                    url += $"{fieldName}={Uri.EscapeDataString(fieldValue)}&";
                            }
                        }
                        else
                        {
                            var dataProperties = data.GetType().GetProperties();

                            //obtenemos nombres de campo y valor dinamicamente para armar la url
                            foreach (var property in dataProperties)
                            {
                                string fieldName = property.Name;
                                string fieldValue = property.GetValue(data)?.ToString();
                                if (!string.IsNullOrEmpty(fieldValue))
                                    url += $"{fieldName}={Uri.EscapeDataString(fieldValue)}&";
                            }

                        }
                        url = url.TrimEnd('&');
                    }
                    #endregion

                    //comsumir con politica de reintentos
                    var responseRetry = await policyRetry.ExecuteAsync(async ctx =>
                    {                      
                        using (var request = new HttpRequestMessage(httpMethod, url))
                        {
                            if (data != null && httpMethod != HttpMethod.Get)
                                request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                            if (headers != null)
                            {
                                foreach (KeyValuePair<String, String> header in headers)
                                    request.Headers.Add(header.Key, header.Value);
                            }

                            return Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
                        }
                    },
                    contextData: new Dictionary<string, object> { { "Url", url } });

                    //cuando la politica de reintento termine, se lee la respuesta
                    using (HttpContent content = responseRetry.Content)
                    {
                        string result = content.ReadAsStringAsync().Result;

                        if (responseRetry.StatusCode == HttpStatusCode.OK)
                            return new ResponseWebServiceDTO() { IsSuccess = true, IsError = false, Response = JsonConvert.DeserializeObject<dynamic>(result) , StatusCode= responseRetry.StatusCode };
                        else
                            return new ResponseWebServiceDTO { IsSuccess = false, Response = JsonConvert.DeserializeObject<dynamic>(result) , StatusCode = responseRetry.StatusCode };
                    }
                }
            }
        }

        #endregion
    }
}
