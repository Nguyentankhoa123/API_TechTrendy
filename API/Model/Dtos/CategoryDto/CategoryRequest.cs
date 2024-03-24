namespace API.Model.Dtos.CategoryDto
{
    public class CategoryRequest
    {

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}
