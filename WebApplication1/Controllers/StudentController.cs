using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly SchoolContext _context;
        private readonly ICapPublisher _publisher;
        public StudentController(SchoolContext context, ICapPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        [Route("~/checkAccountWithTrans")]
        public async Task<IActionResult> PublishMessageWithTransaction([FromServices]SchoolContext dbContext)
        {
            using (var trans = dbContext.Database.BeginTransaction())
            {
                //指定发送的消息标题（供订阅）和内容
                await _publisher.PublishAsync("xxx.services.bar", "发布者->MQ","callback");

                // 你的业务代码。添加一个学生
                await PostStudent(new Student { FirstMidName = "xxx.services.bar", LastName = "Alexander", EnrollmentDate = DateTime.Parse("2005-09-01") });
                await _context.SaveChangesAsync();
                trans.Commit();
            }
            return Ok();
        }

        /// <summary>
        /// 成功后的回调, 参数根据消费者 返回的格式设置
        /// </summary>
        /// <param name="msg"></param>
        [NonAction]
        [CapSubscribe("callback")]
        public void callback(int msg)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
        }

        // GET: api/Students
        [HttpGet]
        public IEnumerable<Student> GetStudents()
        {
            return _context.Students;
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent([FromRoute] int id, [FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != student.ID)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Students
        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.ID }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}