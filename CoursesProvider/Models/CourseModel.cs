namespace CoursesProvider.Models;

public class CourseModel
{
    public string? Id { get; set; }
    public string Image { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string OriginalPrice { get; set; } = null!;
    public string? DiscountPrice { get; set; }
    public int Hours { get; set; }
    public string? LikesInProcent { get; set; }
    public string? NumberOfLikes { get; set; }
    public bool IsDigital { get; set; } = false;
    public bool IsBestseller { get; set; } = false;
}
