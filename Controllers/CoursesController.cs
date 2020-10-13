using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using CourseSysAPI.Helpers;
using CourseSysAPI.Services;
using CourseSysAPI.Entities;
using CourseSysAPI.Models.Courses;
using AutoMapper;

namespace CourseSysAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public CoursesController(ICourseService courseService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _courseService = courseService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var courses = _courseService.GetAll();
            return Ok(courses);
        }

        [HttpGet("info")]
        public IActionResult GetAllForDisplay()
        {
            var courses = _courseService.GetAllForDisplay();
            return Ok(courses);
        }

        [HttpGet("enrollinfo")]
        public IActionResult GetAllWithEnrollment()
        {
            var courses = _courseService.GetAllWithEnrollments();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var course = _courseService.GetById(id);

            if (course == null)
                return NotFound();

            return Ok(course);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("create")]
        public IActionResult Create([FromBody] AddEditCourseModel model)
        {
            // map model to entity
            var course = _mapper.Map<Course>(model);

            try
            {
                // create user
                _courseService.Create(course);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin + ", " + Role.Evaluator)]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AddEditCourseModel model)
        {
            // map model to entity and set id
            var course = _mapper.Map<Course>(model);
            course.Id = id;

            try
            {
                // update course 
                _courseService.Update(course);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _courseService.Delete(id);
            return Ok();
        }

        [Authorize(Roles = Role.Student)]
        [HttpPost("enroll")]
        public IActionResult Enroll([FromBody] EnrollCourseModel model)
        {
            _courseService.EnrollCourse(model.StudentId, model.CourseId);
            return Ok();
        }

        [Authorize(Roles = Role.Student)]
        [HttpGet("deenroll/{id}")]
        public IActionResult DeEnroll(int id)
        {
            _courseService.DeEnrollCourse(id);
            return Ok();
        }


        [Authorize(Roles = Role.Admin + ", " + Role.Student)]
        [HttpGet("enrolllist/{id}")]
        public IActionResult GetCourseByStudent(int id)
        {
            var courses = _courseService.GetCourseForStudent(id);
            return Ok(courses);
        }

        [Authorize(Roles = Role.Evaluator)]
        [HttpGet("evaluator/{id}")]
        public IActionResult GetCourseByEvaluator(int id)
        {
            var courses = _courseService.GetCourseForEvaluator(id);
            return Ok(courses);
        }
    }
}