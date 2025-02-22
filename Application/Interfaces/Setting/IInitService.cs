using Application.DTOs.Request.Setting;
using Application.DTOs.Response;

namespace Application.Interfaces.Setting
{
    public interface IInitService
    {
        Task<GeneralResponse> SeedRoles();
        Task<GeneralResponse> SeedOrderStatus();
        Task<GeneralResponse> CreateInitAdmin();
        Task<GeneralResponse> CreateAsync(CreateInitAdminRequest model);
    }
}
