using HotelManagement.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class ContactController : Controller
{
  
    private readonly IEmailService _emailService;

    public ContactController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Send(string name, string email, string message)
    {

        // Here you would typically send the email using an email service

        string subject = $"The new message from: {name}";
        string body = $"Name: {name}<br>Email: {email}<br>Message: {message}";

        _emailService.SendEmail("maryamalobaidi107@gmail.com", subject, body);


        ViewBag.Message = "Thank you! We have received your message.";
        return View("Index");
    }
}
