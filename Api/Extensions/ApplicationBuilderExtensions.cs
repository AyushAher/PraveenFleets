namespace Api.Extensions;

internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseExceptionHandling(
                                            this IApplicationBuilder app,
                                                    IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsStaging())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios,
            // see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        return app;
    }
    internal static IApplicationBuilder UseEndpoints(
                                            this IApplicationBuilder app)
        => app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });

    internal static IApplicationBuilder ConfigureSwagger(
                                            this IApplicationBuilder app,
                                                    IWebHostEnvironment env)
    {
        if (env.IsProduction()) return app;

        // Enable the Swagger
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
            options.RoutePrefix = "apitest";
            options.DisplayRequestDuration();
        });

        return app;
    }

}
