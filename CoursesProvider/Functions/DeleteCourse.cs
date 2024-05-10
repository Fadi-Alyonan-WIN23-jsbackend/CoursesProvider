using CoursesProvider.Models;
using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoursesProvider.Functions
{
    public class DeleteCourse
    {
        private readonly ILogger<DeleteCourse> _logger;
        private readonly DataContext _dataContext;
        public DeleteCourse(ILogger<DeleteCourse> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        [Function("DeleteCourse")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string body = null!;
            try
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex) { _logger.LogError($" StreamReader DeleteCourse :: {ex.Message}"); }

            if (body != null)
            {
                CourseModel cm = null!;
                try
                {
                    cm = JsonConvert.DeserializeObject<CourseModel>(body)!;
                }
                catch (Exception ex) { _logger.LogError($" JsonConvert.DeserializeObject<CourseModel/delete> :: {ex.Message} "); }

                if (cm != null && !string.IsNullOrEmpty(cm.Id))
                {

                    try
                    {
                        var course = await _dataContext.Courses.FirstOrDefaultAsync(x => x.Id == cm.Id);
                        if (course != null)
                        {
                            _dataContext.Courses.Remove(course);
                            var res = await _dataContext.SaveChangesAsync();
                            if (res == 200)
                            {
                                return new OkResult();
                            }
                            else
                            {
                                new BadRequestResult();
                            }

                        }
                        else
                        {
                            return new NotFoundResult();
                        }
                    }
                    catch (Exception ex) { _logger.LogError($" Get one Course :: {ex.Message}"); }

                }
            }
            return new BadRequestResult();
        }
    }
}
