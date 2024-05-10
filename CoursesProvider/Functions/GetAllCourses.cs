using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CoursesProvider.Functions;

public class GetAllCourses
{
    private readonly ILogger<GetAllCourses> _logger;
    private readonly DataContext _dataContext;

    public GetAllCourses(ILogger<GetAllCourses> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    [Function("GetAllCourses")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        try
        {
            var courses = await _dataContext.Courses.ToListAsync();
            if (courses != null)
            {
                var json = JsonConvert.SerializeObject(courses);
                return new OkObjectResult(json);
            }
        }
        catch (Exception ex) 
        { 
            _logger.LogError($" Get All Courses :: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return new NotFoundResult();
    }
}
