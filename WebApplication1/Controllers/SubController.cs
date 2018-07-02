using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class SubController : Controller
    {
        private readonly SchoolContext _context;
        public SubController(SchoolContext context)
        {
            _context = context;
        }

        [NonAction]
        [CapSubscribe("xxx.services.bar")]
        public async Task CheckReceivedMessage(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
