using CoursesProvider.Models;
using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoursesProvider.Functions;

public class UpdateCourse
{
    private readonly ILogger<UpdateCourse> _logger;
    private readonly DataContext _dataContext;

    public UpdateCourse(ILogger<UpdateCourse> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    [Function("UpdateCourse")]
    public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        string body = null!;
        try
        {
            body = await new StreamReader(req.Body).ReadToEndAsync();
        }
        catch (Exception ex) { _logger.LogError($" StreamReader UpdateCourse :: {ex.Message}"); }

        if (body != null)
        {
            CourseModel cm = null!;
            try
            {
                cm = JsonConvert.DeserializeObject<CourseModel>(body)!;
            }
            catch (Exception ex) { _logger.LogError($" JsonConvert.DeserializeObject<CourseModel/Update> :: {ex.Message} "); }

            if (cm != null && !string.IsNullOrEmpty(cm.Id))
            {
                var courseToUpdate = _dataContext.Courses.FirstOrDefault(x => x.Id == cm.Id);
                if (courseToUpdate != null)
                {
                    courseToUpdate = new Course
                    {
                        Image = cm.Image,
                        Title = cm.Title,
                        Author = cm.Author,
                        OriginalPrice = cm.OriginalPrice,
                        DiscountPrice = cm.DiscountPrice,
                        Hours = cm.Hours,
                        LikesInProcent = cm.LikesInProcent,
                        NumberOfLikes = cm.NumberOfLikes,
                        IsBestseller = cm.IsBestseller,
                        IsDigital = cm.IsDigital
                    };
                    try
                    {
                        _dataContext.Courses.Update(courseToUpdate);
                        var res = await _dataContext.SaveChangesAsync();
                        if (res == 200)
                        {
                            return new OkResult();
                        }
                        else
                        {
                            return new BadRequestResult();
                        }
                    }
                    catch (Exception ex) { _logger.LogError($" User Manager Create :: {ex.Message}"); }

                }
            }
            return new BadRequestResult();
        }
        return new BadRequestResult();
    }
}
