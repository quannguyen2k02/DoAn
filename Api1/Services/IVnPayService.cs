using Api1.Data;
using Api1.Models.VnPay;

namespace Api1.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(OrderDTO model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
