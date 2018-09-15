using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RESTApiNetCore.Models;

namespace RESTApiNetCore
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
            services.AddMvc(options =>
                {
                    options.MaxModelValidationErrors = 1;
                    options.ReturnHttpNotAcceptable = true;
                })
                .AddXmlDataContractSerializerFormatters();

            services.AddSingleton<IEducationSystem, EducationSystem>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //notesController

                routes.MapRoute(
                    name: "AddNewNote",
                    template: "{action}/{idNote}"
                    );

                //lecturesController

                routes.MapRoute(
                    name: "AddNewLecture",
                    template: "{action}/{indexLecture}"
                    );

                routes.MapRoute(
                    name: "AssignNoteToLecture",
                    template: "{action}/{idLecture}/notes/{idNote}"
                    );



                //studentsController

                routes.MapRoute(
                   name: "SaveStudentNoteFromLecture",
                   template: "{action}/{idStudent}/lectures/{idLecture}/notes"
                   );

                routes.MapRoute(
                   name: "SaveStudentToLecture",
                   template: "{action}/{idStudent}/lectures"
                   );

                routes.MapRoute(
                    name: "Default",
                    template: "{action}/{id}"
                    );
            });
        }
    }
}
