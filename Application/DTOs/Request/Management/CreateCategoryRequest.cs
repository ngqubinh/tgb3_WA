namespace Application.DTOs.Request.Management
{
    public class CreateCategoryRequest
    {
        public Guid Id { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UserId { get; set; }
    }
}
