using HotelManagement.BLL.Interfaces;
using HotelManagement.BLL.Services;
using HotelManagement.DAL.Interfaces;
using HotelManagement.DAL.Repositories;
using HotelManagement.Infrastructure.DAL.Interfaces;
using HotelManagement.Infrastructure.DAL.Repositories;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
        
            var builder = WebApplication.CreateBuilder(args);


            // هنا يتم إعداد خدمات التسجيل
            builder.Logging.ClearProviders(); // لإزالة أي موفرات تسجيل افتراضية لا تريدينها
            builder.Logging.AddConsole(); // لإضافة التسجيل إلى الكونسول
            builder.Logging.AddDebug();   // لإضافة التسجيل إلى نافذة Debug في Visual Studio
                                          // يمكنك إضافة موفرات أخرى هنا إذا كنت تستخدمين Serilog, NLog, إلخ.

            
            var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
           
        // Register Data Access Layer (DAL) services

            builder.Services.AddScoped<ICustomerRepository>(provider => new CustomerRepository(connectionString,provider.GetRequiredService<ILogger<CustomerRepository>>()));

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



            //ex for mi
            // ستقوم باختيار واحدة فقط من هذه بناءً على احتياجاتك:

            // الخيار الأكثر شيوعًا للخدمات والمستودعات في تطبيقات الويب
            // builder.Services.AddScoped<ICustomerService, CustomerService>();

            // أو هذا إذا كنت تحتاج إلى نسخة جديدة في كل مرة يتم فيها طلبها
            // builder.Services.AddTransient<ICustomerService, CustomerService>();

            // أو هذا إذا كنت تحتاج إلى نسخة واحدة فقط طوال عمر التطبيق
            // builder.Services.AddSingleton<ICustomerService, CustomerService>();



            // Register Business Logic Layer (BLL) services
            builder.Services.AddScoped<ICustomerService, CustomerService>();

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
                app.UseExceptionHandler("/Home/Error");//  لاجل عدم ضهور الاخطاء البرمجيه الحساسه للمستخدم اذا كان ليس في بيئة التطوير (أي في بيئات الإنتاج أو الاختبار)، يستخدم هذا Middleware للتعامل مع الأخطاء في التطبيق. في حالة حدوث خطأ، سيتم توجيه المستخدم إلى صفحة الخطأ المحددة.
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            // Middlewares are components that are assembled into an application pipeline to handle requests and responses.
            //البرامج الوسيطة هي مكونات يتم تجميعها في خط أنابيب "بايبلين" التطبيق للتعامل مع الطلبات والاستجابات.

            app.UseHttpsRedirection();//: إذا تلقى التطبيق طلب HTTP غير آمن (على المنفذ 80 مثلاً)، فإنه يقوم بإعادة توجيه (redirect) هذا الطلب تلقائيًا إلى عنوان URL الآمن الخاص بـ HTTPS (على المنفذ 443 عادةً).

           // لماذا مهم؟ لضمان أن جميع الاتصالات بين العميل والخادم مشفرة، مما يزيد من الأمان.

            app.UseStaticFiles();// ملفات ثابته Middleware 

            app.UseRouting();// ملفات يستخدم لتحديد الـ Controller والـ Action المناسبين للطلب بناءً على الـ URL ..Middleware 

            app.UseAuthorization();// يستخدم للتحقق من هوية المستخدم (هل المستخدم الذي يرسل الطلب هو من يدعي أنه هو؟). Middleware 


                // ماذا يفعل: يحدد كيف يجب أن تبدو عناوين URL وكيفية ربطها بـ Controllers و Actions.
                
                //name: "default": اسم لهذا المسار.
                
                //pattern: "{controller=Home}/{action=Index}/{id?}": هذا هو قالب المسار.
                
                //{ controller = Home}: يعني أن الجزء الأول من URL سيكون اسم الـ Controller، وإذا لم يتم تحديده، فسيكون الافتراضي هو Home.
                
                //{ action = Index}: يعني أن الجزء الثاني سيكون اسم الـ Action(الدالة) داخل الـ Controller، والافتراضي هو Index.
                
               //{ id ?}: يعني أن الجزء الثالث اختياري ويمثل معرفًا(ID).



            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
