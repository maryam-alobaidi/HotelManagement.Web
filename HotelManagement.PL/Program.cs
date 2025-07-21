using HotelManagement.BLL.Interfaces;
using HotelManagement.BLL.Services;
using HotelManagement.DAL.Interfaces;
using HotelManagement.DAL.Repositories;
using HotelManagement.Infrastructure.DAL.Interfaces;
using HotelManagement.Infrastructure.DAL.Repositories;

namespace HotelManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
        
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("ConnectionString");


            // Register Data Access Layer (DAL) services

            builder.Services.AddScoped<ICustomerRepository>(provider => new CustomerRepository(connectionString));

            builder.Services.AddScoped<IRoomRepository>(providor => new RoomRepository(connectionString));

            builder.Services.AddScoped<IRoomTypesRepository>(providor => new RoomTypesRepository(connectionString));

            builder.Services.AddScoped<IRoomStatusesRepository>(providor => new RoomStatusesRepository(connectionString));

            builder.Services.AddScoped<IEmployeeRepository>(provider => new EmployeeRepository(connectionString));

            builder.Services.AddScoped<IBookingRepository>(provider => new BookingRepository(connectionString));

            builder.Services.AddScoped<IInvoiceItemRepository>(provider => new InvoiceItemRepository(connectionString));

            builder.Services.AddScoped<IInvoiceRepository>(provider => new InvoiceRepository(connectionString));

            builder.Services.AddScoped<IPaymentMethodsRepository>(provider => new PaymentMethodsRepository(connectionString));

            builder.Services.AddScoped<IPaymentsRepository>(provider => new PaymentsRepository(connectionString));



            // AddAsync services to the container.
            builder.Services.AddControllersWithViews();


            // Register Business Logic Layer (BLL) services
          

            builder.Services.AddScoped<ICustomerService,CustomerService>();

            builder.Services.AddScoped<IRoomService, RoomService>();

            builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();

            builder.Services.AddScoped<IRoomStatuseService, RoomStatuseService>();

            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            builder.Services.AddScoped<IBookingService, BookingService>();

            builder.Services.AddScoped<IInvoiceItemService, InvoiceItemService>();

            builder.Services.AddScoped<IInvoiceService, InvoiceService>();

            builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();

            builder.Services.AddScoped<IPaymentService, PaymentService>();




            builder.Services.AddAuthorization();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            // Middlewares are components that are assembled into an application pipeline to handle requests and responses.
            //البرامج الوسيطة هي مكونات يتم تجميعها في خط أنابيب "بايبلين" التطبيق للتعامل مع الطلبات والاستجابات.
            app.UseHttpsRedirection();
            app.UseStaticFiles();// ملفات ثابته Middleware 

            app.UseRouting();// ملفات يستخدم لتحديد الـ Controller والـ Action المناسبين للطلب بناءً على الـ URL ..Middleware 

            app.UseAuthorization();// يستخدم للتحقق من هوية المستخدم (هل المستخدم الذي يرسل الطلب هو من يدعي أنه هو؟). Middleware 

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
