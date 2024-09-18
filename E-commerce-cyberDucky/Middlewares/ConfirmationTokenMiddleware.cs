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
                        //// Send registration success email
                        //var emailSent = await Utils.SendMail.SendRegistrationSuccessEmail(user.Email);
                        //if (!emailSent)
                        //{
                        //    // Log or handle the error as needed
                        //    System.Console.WriteLine($"Failed to send registration success email to {user.Email}");
                        //}
                        //context.Response.Redirect($"https://zodiacjewerly.azurewebsites.net");
                        context.Response.Redirect("https://cyberducky.vercel.app/login");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
