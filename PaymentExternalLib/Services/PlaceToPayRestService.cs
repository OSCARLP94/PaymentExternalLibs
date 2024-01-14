using PaymentExternalLib.Models;
using PaymentExternalLib.Services.Contracts;
using PaymentExternalLib.Utils;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace PaymentExternalLib.Services
{
    public class PlaceToPayRestService : IPlaceToPayService
    {
        private readonly string _url;

        #region Build
        public PlaceToPayRestService(string urlMain, string login, string trankey) 
        {
            _url = urlMain;
        }
        #endregion

        #region Public methods
        public async Task<ResumConsumeWebServiceDTO> GetStatusPaymentByRef(string paymentRef)
        {
            //opciones de politica de reintento
            AsyncRetryPolicy<HttpResponseMessage> _policyRetry = HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount: 3, //maximo 3 reintentos
                retry => TimeSpan.FromSeconds(Math.Pow(5, retry)), //por cada intento espera 5^intento
                onRetry: (exception, time, context) =>
                {
                    Console.WriteLine($"Reintento de consumo a servicio externo: {context["Url"]} ", nameof(PlaceToPayRestService.GetStatusPaymentByRef), "");
                });

            throw new NotImplementedException();
        }
        #endregion
    }
}
