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
using System.Diagnostics;

namespace CoursesProvider.Functions;

public class GetOneCourse
{
    private readonly ILogger<GetOneCourse> _logger;
    private readonly DataContext _dataContext;
    public GetOneCourse(ILogger<GetOneCourse> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    [Function("GetOneCourse")]
    public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        string body = null!;
        try
        {
            body = await new StreamReader(req.Body).ReadToEndAsync();
        }
        catch (Exception ex) 
        { 
            _logger.LogError($" StreamReader GetOneCourse :: {ex.Message}"); 
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        }

        if (body != null)
        {
            CourseModel cm = null!;
            try
            {
                cm = JsonConvert.DeserializeObject<CourseModel>(body)!;
            }
            catch (Exception ex) { _logger.LogError($" JsonConvert.DeserializeObject<CourseModel/GetOne> :: {ex.Message} "); }

            if (cm != null && !string.IsNullOrEmpty(cm.Id))
            {
                try
                {
                    var course = await _dataContext.Courses.FirstOrDefaultAsync(x => x.Id == cm.Id);
                    if (course != null)
                    {
                        var json = JsonConvert.SerializeObject(course);
                        return new OkObjectResult(json);
                    }
                    else
                    {
                        return new NotFoundResult();
                    }
                }
                catch (Exception ex) 
                { 
                    _logger.LogError($" Get one Course :: {ex.Message}"); 
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);

                }

            }
        }
        return new BadRequestResult();
    }
}
