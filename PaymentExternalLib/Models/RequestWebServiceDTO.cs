namespace PaymentExternalLib.Models
{
    public class RequestWebServiceDTO
    {
        public RequestWebServiceDTO() 
        {
            this.RegisterDate = DateTime.Now;
        }

        /// <summary>
        /// Fecha de registro
        /// </summary>
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// Url consumida
        /// </summary>
        public string UrlExternalService { get; set; }

        /// <summary>
        /// Objeto-data a enviar como string
        /// </summary>
        public string DataSended { get; set; }

        /// <summary>
        /// Nombre archivo
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Referencia hacia otro proceso
        /// </summary>
        public string IdReference { get; set; }

        /// <summary>
        /// Tipo de proceso referenciado
        /// </summary>
        public string Reference { get; set; }
    }
}
