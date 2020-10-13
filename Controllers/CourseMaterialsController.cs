using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using CourseSysAPI.Helpers;
using CourseSysAPI.Services;
using CourseSysAPI.Entities;
using CourseSysAPI.Models.Materials;
using AutoMapper;

namespace CourseSysAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CourseMaterialsController : ControllerBase
    {
        private readonly ICourseMaterialService _courseMaterialService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public CourseMaterialsController(ICourseMaterialService courseMaterialService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _courseMaterialService = courseMaterialService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var materials = _courseMaterialService.GetAll();
            return Ok(materials);
        }

        [HttpGet("course/{id}")]
        public IActionResult GetByCourse(int id)
        {
            var materials = _courseMaterialService.GetByCourse(id);
            return Ok(materials);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var materials = _courseMaterialService.GetById(id);

            if (materials == null)
                return NotFound();

            return Ok(materials);
        }

        [Authorize(Roles = Role.Evaluator)]
        [HttpPost("create")]
        public IActionResult Create([FromBody] AddEditMaterialModel model)
        {
            // map model to entity
            var courseMaterial = _mapper.Map<CourseMaterial>(model);

            try
            {
                _courseMaterialService.Create(courseMaterial);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Evaluator)]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AddEditMaterialModel model)
        {
            // map model to entity and set id
            var courseMaterial = _mapper.Map<CourseMaterial>(model);
            courseMaterial.Id = id;

            try
            {
                // update course material
                _courseMaterialService.Update(courseMaterial);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Evaluator)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _courseMaterialService.Delete(id);
            return Ok();
        }

        [Authorize(Roles = Role.Student)]
        [HttpPost("submitassignment")]
        public IActionResult SubmitAssignment([FromBody] SubmitAssignmentModel model)
        {
            var assignment = _mapper.Map<Assignment>(model);

            _courseMaterialService.SubmitAssignment(assignment);
            return Ok();
        }

        [Authorize(Roles = Role.Student)]
        [HttpPut("updateassignment/{id}")]
        public IActionResult UpdateAssignment(int id, [FromBody] SubmitAssignmentModel model)
        {
            // map model to entity and set id
            var assignment = _mapper.Map<Assignment>(model);
            assignment.Id = id;

            try
            {
                // update assignment submission
                _courseMaterialService.UpdateAssignment(assignment);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Evaluator)]
        [HttpPatch("grading")]
        public IActionResult GradeAssignment([FromBody] GradeAssignmentModel model)
        {
            try
            {
                // update assignment submission
                _courseMaterialService.GradeAssignment(model.Id, model.Grade);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}