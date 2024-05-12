using CoursesProvider.Models;
using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CoursesProvider.Functions;

public class CreateCourse
{
    private readonly ILogger<CreateCourse> _logger;
    private readonly DataContext _dataContext;

    public CreateCourse(ILogger<CreateCourse> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    [Function("CreateCourse")]
    public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        string body = null!;
        try
        {
            body = await new StreamReader(req.Body).ReadToEndAsync();
        }
        catch (Exception ex) 
        { 
            _logger.LogError($" StreamReader create course :: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (body != null)
        {
            CourseModel cm = null!;
            try
            {
                cm = JsonConvert.DeserializeObject<CourseModel>(body)!;
            }
            catch (Exception ex) { _logger.LogError($" JsonConvert.DeserializeObject<CourseModel/Create> :: {ex.Message} "); }

            if (cm != null && !string.IsNullOrEmpty(cm.Image) && !string.IsNullOrEmpty(cm.Title) && !string.IsNullOrEmpty(cm.Author) && !string.IsNullOrEmpty(cm.OriginalPrice))
            {
                if (!await _dataContext.Courses.AnyAsync(x => x.Title == cm.Title))
                {
                    var courseModel = new Course
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
                        _dataContext.Courses.Add(courseModel);
                        await _dataContext.SaveChangesAsync();
                        return new CreatedResult();
                        
                    }
                    catch (Exception ex) 
                    { 
                        _logger.LogError($" Create course :: {ex.Message}");
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }

                }
            }
            return new ConflictResult();
        }
        return new BadRequestResult();
    }
}