namespace Application.DTOs.Response
{
    public record GeneralResponse(bool Success = false, string Message = null!);
}
