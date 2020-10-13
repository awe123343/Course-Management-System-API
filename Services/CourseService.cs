using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using CourseSysAPI.Data;
using CourseSysAPI.Entities;
using CourseSysAPI.Helpers;
using CourseSysAPI.Models.Courses;
using AutoMapper;

namespace CourseSysAPI.Services
{
    public interface ICourseService
    {
        IEnumerable<Course> GetAll();
        IEnumerable<CourseDisplayModel> GetAllForDisplay();
        IEnumerable<CourseDisplayModel> GetCourseForStudent(int id);
        IEnumerable<CourseEnrollmentModel> GetCourseForEvaluator(int id);
        IEnumerable<CourseEnrollmentModel> GetAllWithEnrollments();
        Course GetById(int id);
        Course Create(Course course);
        void Update(Course course);
        void Delete(int id);
        void EnrollCourse(int studentId, int courseId);
        void DeEnrollCourse(int id);
    }

    public class CourseService : ICourseService
    {
        private readonly DataContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public CourseService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public IEnumerable<Course> GetAll()
        {
            return _context.Courses;
        }

        public IEnumerable<CourseDisplayModel> GetAllForDisplay()
        {
            var courses = _context.Courses;
            var users = _context.Users;
            var enrollments = _context.Enrollments;

            IEnumerable<CourseDisplayModel> res = courses.Join(users, c => c.EvaluatorId, u => u.Id, (c, u) => new CourseDisplayModel
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Capacity = c.Capacity,
                EvaluatorName = $"{u.FirstName} {u.LastName}",
                Description = c.Description,
                CurrentStuNo = enrollments.Where(e => e.CourseId == c.Id).Count()
            });

            return res;
        }

        public IEnumerable<CourseDisplayModel> GetCourseForStudent(int id)
        {
            var courses = _context.Courses;
            var users = _context.Users;
            HashSet<int> courseStudentTake = new HashSet<int>(_context.Enrollments.Where(enroll => enroll.StudentId == id).Select(enroll => enroll.CourseId));
            var enrollments = _context.Enrollments.Where(enroll => courseStudentTake.Contains(enroll.CourseId));

            IEnumerable<CourseDisplayModel> res = courses.Join(users, c => c.EvaluatorId, u => u.Id, (c, u) => new CourseDisplayModel
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Capacity = c.Capacity,
                EvaluatorName = $"{u.FirstName} {u.LastName}",
                Description = c.Description,
                CurrentStuNo = enrollments.Where(e => e.CourseId == c.Id).Count()
            });

            return res;
        }

        public IEnumerable<CourseEnrollmentModel> GetCourseForEvaluator(int id)
        {
            var courses = _context.Courses.Where(c => c.EvaluatorId == id);
            var users = _context.Users;
            var enrollments = _context.Enrollments;

            IEnumerable<CourseEnrollmentModel> res = courses.Join(users, c => c.EvaluatorId, u => u.Id, (c, u) => new CourseEnrollmentModel
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Capacity = c.Capacity,
                EvaluatorName = $"{u.FirstName} {u.LastName}",
                Description = c.Description,
                CurrentStuNo = enrollments.Where(e => e.CourseId == c.Id).Count(),
                Students = enrollments
                    .Join(users, e => e.StudentId, usr => usr.Id, (e, usr) => new
                    {
                        Id = usr.Id,
                        FirstName = usr.FirstName,
                        LastName = usr.LastName,
                        Username = usr.Username,
                        Role = usr.Role,
                        CourseId = e.CourseId
                    })
                    .Where(enroll => enroll.CourseId == c.Id)
                    .Select(obj => new User
                    {
                        Id = obj.Id,
                        FirstName = obj.FirstName,
                        LastName = obj.LastName,
                        Username = obj.Username,
                        Role = obj.Role,
                    }).ToList()
            });

            return res;
        }

        public IEnumerable<CourseEnrollmentModel> GetAllWithEnrollments()
        {
            var courses = _context.Courses;
            var users = _context.Users;
            var enrollments = _context.Enrollments;

            IEnumerable<CourseEnrollmentModel> res = courses.Join(users, c => c.EvaluatorId, u => u.Id, (c, u) => new CourseEnrollmentModel
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Capacity = c.Capacity,
                EvaluatorName = $"{u.FirstName} {u.LastName}",
                Description = c.Description,
                CurrentStuNo = enrollments.Where(e => e.CourseId == c.Id).Count(),
                Students = enrollments
                    .Join(users, e => e.StudentId, usr => usr.Id, (e, usr) => new
                    {
                        Id = usr.Id,
                        FirstName = usr.FirstName,
                        LastName = usr.LastName,
                        Username = usr.Username,
                        Role = usr.Role,
                        CourseId = e.CourseId
                    })
                    .Where(enroll => enroll.CourseId == c.Id)
                    .Select(obj => new User
                    {
                        Id = obj.Id,
                        FirstName = obj.FirstName,
                        LastName = obj.LastName,
                        Username = obj.Username,
                        Role = obj.Role,
                    }).ToList()
            });

            return res;
        }

        public Course GetById(int id)
        {
            return _context.Courses.Find(id);
        }

        public Course Create(Course course)
        {
            if (_context.Courses.Any(x => x.CourseCode == course.CourseCode))
                throw new AppException("Course code \"" + course.CourseCode + "\" is already taken");

            _context.Courses.Add(course);
            _context.SaveChanges();

            return course;
        }

        public void Update(Course newCourse)
        {
            var course = _context.Courses.Find(newCourse.Id);

            if (course == null)
                throw new AppException("Course not found");

            // update courseCode if it has changed
            if (!string.IsNullOrWhiteSpace(newCourse.CourseCode) && newCourse.CourseCode != course.CourseCode)
            {
                // throw error if the new courseCode is already taken
                if (_context.Users.Any(x => x.Username == newCourse.CourseCode))
                    throw new AppException("Course code " + newCourse.CourseCode + " is already taken");

                course.CourseCode = newCourse.CourseCode;
            }

            // update course properties if provided
            if (newCourse.CourseName != null)
                course.CourseName = newCourse.CourseName;

            if (!string.IsNullOrEmpty(newCourse.Description))
                course.Description = newCourse.Description;

            course.Capacity = newCourse.Capacity;
            course.EvaluatorId = newCourse.EvaluatorId;

            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
        }

        public void EnrollCourse(int studentId, int courseId)
        {
            Enrollment e = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId
            };

            _context.Enrollments.Add(e);
            _context.SaveChanges();
        }

        public void DeEnrollCourse(int id)
        {
            var e = _context.Enrollments.Find(id);
            if (e != null)
            {
                _context.Enrollments.Remove(e);
                _context.SaveChanges();
            }
        }
    }
}