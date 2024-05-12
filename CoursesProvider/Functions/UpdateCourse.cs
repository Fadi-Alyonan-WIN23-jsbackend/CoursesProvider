using CoursesProvider.Models;
using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        string body = null!;
        try
        {
            body = await new StreamReader(req.Body).ReadToEndAsync();
        }
        catch (Exception ex) 
        { 
            _logger.LogError($" StreamReader UpdateCourse :: {ex.Message}"); 
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (body != null)
        {
            CourseModel cm = null!;
            try
            {
                cm = JsonConvert.DeserializeObject<CourseModel>(body)!;
            }
            catch (Exception ex) 
            { 
                _logger.LogError($" JsonConvert.DeserializeObject<CourseModel/Update> :: {ex.Message} ");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (cm != null && !string.IsNullOrEmpty(cm.Id))
            {
                var courseToUpdate = _dataContext.Courses.FirstOrDefault(x => x.Id == cm.Id);
                if (courseToUpdate != null)
                {
                    courseToUpdate.Image = cm.Image;
                    courseToUpdate.Title = cm.Title;
                    courseToUpdate.Author = cm.Author;
                    courseToUpdate.OriginalPrice = cm.OriginalPrice;
                    courseToUpdate.DiscountPrice = cm.DiscountPrice;
                    courseToUpdate.Hours = cm.Hours;
                    courseToUpdate.LikesInProcent = cm.LikesInProcent;
                    courseToUpdate.NumberOfLikes = cm.NumberOfLikes;
                    courseToUpdate.IsBestseller = cm.IsBestseller;
                    courseToUpdate.IsDigital = cm.IsDigital;

                    try
                    {
                        _dataContext.Courses.Update(courseToUpdate);
                        await _dataContext.SaveChangesAsync();
                        var json = JsonConvert.SerializeObject(courseToUpdate);
                        return new OkObjectResult(json);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($" Update course :: {ex.Message}");
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }
                }
                else
                {
                    return new NotFoundResult();
                }
            }
        }
        return new BadRequestResult();
    }
}
