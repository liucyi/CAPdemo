using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApplication2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCap(x =>
            {
                x.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
                x.UseRabbitMQ(p => {
                    p.UserName = "test";
                    p.Password = "test";
                    p.HostName = "你的服务器IP";
                    p.Port = 5672;
                });
                x.FailedRetryInterval = 5;
                x.FailedRetryCount = 3;
                //TODO 不好用
                x.FailedThresholdCallback = Controllers.ValuesController.FailureCallBack;

                //TODO 官方文档没写 不知道怎么用
                x.UseDashboard(p=>
                {
                    p.PathMatch = "/cap";
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCap();
            app.UseMvc();
        }
    }
}
