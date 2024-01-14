
using System.Net;

namespace PaymentExternalLib.Models
{
    public class ResponseWebServiceDTO
    {
        /// <summary>
        /// Indica si la respuesta es o no satisfactoria
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Indica si se produjo un error
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Mensaje informacion de respuesta
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Respuesta del serivicio
        /// </summary>
        public dynamic Response { get; set; }

        /// <summary>
        /// Error producido
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Estado de resuesta del servicio
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

    }
}
