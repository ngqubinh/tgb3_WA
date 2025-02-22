using Domain.Models.Management;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Management
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext httpContext, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}