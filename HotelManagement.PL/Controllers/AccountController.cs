using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models.ViewModels.CustomerModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ICustomerService _customerService;

    public AccountController(IAuthService authService, IPasswordHasherService passwordHasher, ICustomerService customerService)
    {
        _authService = authService;
        _passwordHasher = passwordHasher;
        _customerService = customerService;
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View(); // سيبحث عن Login.cshtml داخل Views/Account
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(string userType, string username, string password, string email, string role)
    {
        if (userType == "Employee")
        {
            var employee = await _authService.LoginEmployee(username, password, role);
            if (employee != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,employee.Username),
                    new Claim(ClaimTypes.Role, employee.Role.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            
        }
        else if (userType == "Customer")
        {
           
            var customer = await _authService.LoginCustomer(email, password);
            if (customer != null)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, customer.FullName),
                    new Claim(ClaimTypes.Email, customer.Email),
                    new Claim(ClaimTypes.Role, "Customer")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid user type.");
        }

        return View(); 
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }



    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(CustomerRegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // تحقق إذا البريد موجود مسبقاً
        var existingCustomer = await _authService.LoginCustomer(model.Email, model.Password);
        if (existingCustomer != null)
        {
            ModelState.AddModelError("", "Email is already registered.");
            return View(model);
        }

        // إنشاء Customer جديد
        var hashedPassword = _passwordHasher.HashPassword(model.Password);
        var customer = new Customer(
            model.FirstName,
            model.LastName,
            model.Email,
            model.PhoneNumber,
            model.Address,
            model.Nationality,
            model.IDNumber,
            hashedPassword
        );

        await _customerService.AddCustomerAsync(customer);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, customer.FullName),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, "Customer")
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        // توجيه الزبون مباشرة إلى صفحة الحجز
        return RedirectToAction("Index", "Home");


    }


    public IActionResult AccessDenied()
    {
        return View();
    }

}
