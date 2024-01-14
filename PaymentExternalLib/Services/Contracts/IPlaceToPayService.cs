using PaymentExternalLib.Models;

namespace PaymentExternalLib.Services.Contracts
{
    public interface IPlaceToPayService
    {
        Task<ResumConsumeWebServiceDTO> GetStatusPaymentByRef(string paymentRef);
    }
}
