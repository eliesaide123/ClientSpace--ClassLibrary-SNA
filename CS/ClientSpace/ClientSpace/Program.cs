//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//app.Run();


using BLC;
using Entities;

namespace ClientSpace
{
    class Program
    {
        static void Main(string[] args)
        {
            IBLC blc = new BusinessLogic();
            var credentials = new CredentialsDto { Username = "admin111", Password = "admin", UserID = "234"};
            bool isAuthenticated  = blc.Authenticate(credentials);
            Console.WriteLine("Authentication successful: " + isAuthenticated);
            // Add this line to keep the console window open
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
