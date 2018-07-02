using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DotNetCore.CAP;
using DotNetCore.CAP.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [NonAction]
        [CapSubscribe("xxx.services.bar")]
        public async Task<int> CheckReceivedMessage(string msg)
        {
            Console.WriteLine(msg);

            var connString = "server=127.0.0.1;port=3306;database=test;uid=ts;pwd=1111;SslMode=None";
            using (var connection = new MySqlConnection(connString))
            {
                connection.Open();
                return await connection.ExecuteAsync($"INSERT INTO aaa VALUES1 ('{msg}')");
            }
        }


        public static void FailureCallBack(MessageType type,string name,string content)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            
        }
    }
}
