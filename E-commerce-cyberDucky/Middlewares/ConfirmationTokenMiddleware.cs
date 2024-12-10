using Application;

namespace E_commerce_cyberDucky.Middlewares
{
    public class ConfirmationTokenMiddleware
    {
        private readonly RequestDelegate _next;
        public ConfirmationTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // create area service temporary
            using (var scope = context.RequestServices.CreateScope())
            {
                // Get the IUnitOfWork from the temporary service scope
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var token = context.Request.Query["token"];

                if (!string.IsNullOrEmpty(token))
                {
                    var user = await unitOfWork.UserRepository.GetUserByConfirmationToken(token);

                    if (user != null && !user.IsConfirmed)
                    {
                        // confirm
                        user.IsConfirmed = true;
                        user.ConfirmationToken = null;
                        await unitOfWork.SaveChangeAsync();                       
                        context.Response.Redirect("https://cyberducky-thongnvts-projects.vercel.app/login");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
