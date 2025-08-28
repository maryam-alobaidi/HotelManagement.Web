using HotelManagement.BLL.Interfaces;
using HotelManagement.BLL.Services;
using HotelManagement.DAL.Interfaces;
using HotelManagement.DAL.Repositories;
using HotelManagement.Infrastructure.DAL.Interfaces;
using HotelManagement.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace HotelManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Connection string
            var connectionString = builder.Configuration.GetConnectionString("ConnectionString");

            // -----------------------------
            // Register DAL Repositories
            // -----------------------------
            builder.Services.AddScoped<ICustomerRepository>(provider =>
                new CustomerRepository(connectionString, provider.GetRequiredService<ILogger<CustomerRepository>>()));
            builder.Services.AddScoped<IEmployeeRepository>(provider =>
                new EmployeeRepository(connectionString, provider.GetRequiredService<ILogger<EmployeeRepository>>()));
            builder.Services.AddScoped<IRoomRepository>(provider =>
                new RoomRepository(connectionString, provider.GetRequiredService<ILogger<RoomRepository>>()));
            builder.Services.AddScoped<IRoomTypesRepository>(provider =>
                new RoomTypesRepository(connectionString, provider.GetRequiredService<ILogger<RoomTypesRepository>>()));
            builder.Services.AddScoped<IRoomStatusesRepository>(provider =>
                new RoomStatusesRepository(connectionString, provider.GetRequiredService<ILogger<RoomStatusesRepository>>()));
            builder.Services.AddScoped<IBookingRepository>(provider =>
                new BookingRepository(connectionString, provider.GetRequiredService<ILogger<BookingRepository>>()));
            builder.Services.AddScoped<IInvoiceItemRepository>(provider =>
                new InvoiceItemRepository(connectionString));
            builder.Services.AddScoped<IInvoiceRepository>(provider =>
                new InvoiceRepository(connectionString));
            builder.Services.AddScoped<IPaymentMethodsRepository>(provider =>
                new PaymentMethodsRepository(connectionString));
            builder.Services.AddScoped<IPaymentsRepository>(provider =>
                new PaymentsRepository(connectionString));

            // -----------------------------
            // Register BLL Services
            // -----------------------------
            builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
            builder.Services.AddScoped<IRoomStatuseService, RoomStatuseService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IInvoiceItemService, InvoiceItemService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();



            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

            // Add MVC
            builder.Services.AddControllersWithViews();

            // -----------------------------
            // Localization Settings
            // -----------------------------
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var defaultCulture = new CultureInfo("en-US");
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = new[] { defaultCulture };
                options.SupportedUICultures = new[] { defaultCulture };
                options.RequestCultureProviders.Clear();
            });

            // Authorization
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseAuthorization();

            // -----------------------------
            // Default Route: Login page
            // -----------------------------
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
