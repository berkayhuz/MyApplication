//using Forum.Api.Middlewares;
//using Forum.Infrastructure.EventHandlers;
//using Forum.Infrastructure.Notifications;
//using Forum.Shared.EventBus;
//using Forum.Shared.Events;

//namespace Forum.Api.Extensions
//{
//    public static class ConfigureExtensions
//    {
//        public static WebApplication ConfigureMiddleware(this WebApplication app)
//        {
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseMiddleware<ExceptionHandlingMiddleware>();

//            app.UseHttpsRedirection();
//            app.UseRouting();

//            app.UseCors("AllowAllOrigins");

//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapControllers();
//            });

//            app.MapControllers();

//            return app;
//        }
//        public static WebApplication ConfigureEventBus(this WebApplication app)
//        {
//            var eventBus = app.Services.GetRequiredService<IEventBus>();
//            var emailNotificationService = app.Services.GetRequiredService<EmailNotificationService>();

//            eventBus.Subscribe<UserResetPasswordEvent>(async (eventData) =>
//            {
//                var handler = new UserResetPasswordEventHandler(emailNotificationService);
//                await handler.HandleAsync(eventData);
//            });

//            eventBus.Subscribe<UserForgotPasswordEvent>(async (eventData) =>
//            {
//                var handler = new UserForgotPasswordEventHandler(emailNotificationService, eventBus);
//                await handler.HandleAsync(eventData);
//            });


//            return app;
//        }
//    }

//}
