using HotelManagement.BLL.Interfaces;
using HotelManagement.BLL.Services;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Web.Models.ViewModels.BookingModel;
using HotelManagement.Web.Models.ViewModels.InvoiceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace HotelManagement.Web.Controllers
{
    public class InvoiceController : Controller
    {

        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly IBookingService _bookingService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger,IBookingService bookingService)
        {
            _invoiceService = invoiceService;
            _logger = logger;
            _bookingService = bookingService;
        }



        // GET: InvoiceController
        public async Task<IActionResult> Index()
        {
           var invoices = await  _invoiceService.GetAllInvoicesAsync();
            if(invoices == null || !invoices.Any())
            {
                _logger.LogWarning("No invoices found.");
                return NotFound("No invoices available.");
            }


            var InvoiceViewModel = invoices.Select(i => new InvoiceViewModel
            {
                InvoiceID = i.InvoiceID,
                BookingID = i.BookingID,
                CustomerID = i.CustomerID,
                InvoiceDate = i.InvoiceDate??DateTime.MinValue,
                TotalAmount = i.TotalAmount,
                AmountPaid = i.AmountPaid ?? 0m, // Fixes CS0266 and CS8629
                InvoiceStatus = i.InvoiceStatus.ToString(),
                GeneratedByEmployeeID = i.GeneratedByEmployeeID
            }).ToList();


            return View(InvoiceViewModel);
        }

        // GET: InvoiceController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if(id <= 0)
            {
                _logger.LogError("Invalid invoice ID: {Id}", id);
                return BadRequest("Invalid invoice ID.");
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if(invoice == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found.", id);
                return NotFound($"Invoice with ID {id} not found.");
            }
            var invoiceViewModel = new InvoiceViewModel
            {
                InvoiceID = invoice.InvoiceID,
                BookingID = invoice.BookingID,
                CustomerID = invoice.CustomerID,
                InvoiceDate = invoice.InvoiceDate??DateTime.MinValue,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid?? 0m,
                InvoiceStatus = invoice.InvoiceStatus.ToString(),
                GeneratedByEmployeeID = invoice.GeneratedByEmployeeID
            };

            return View(invoiceViewModel);
        }

        // GET: InvoiceController/Create
        public async Task<IActionResult> Create()
        {
           
            var invoiceCreateEditViewModel = new InvoiceCreateEditViewModel
            {
                InvoiceDate = DateTime.Now,
                TotalAmount = 0m, // Default value, can be updated later
                AmountPaid = 0m, // Default value, can be updated later
                InvoiceStatus = "UnPaid", // Default status
            
            };

            await PopulateBookingsDropdownAsync(invoiceCreateEditViewModel);

            return View(invoiceCreateEditViewModel);
        }

        // POST: InvoiceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceCreateEditViewModel model)
        {
            if(!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid.");
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            if(model.BookingID <= 0 || model.CustomerID <= 0 || model.GeneratedByEmployeeID <= 0)
            {
                _logger.LogError("Invalid BookingID, CustomerID, or GeneratedByEmployeeID.");
                ModelState.AddModelError("", "Invalid Booking, Customer, or Employee selection.");
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            if(model.TotalAmount <= 0)
            {
                _logger.LogError("Total amount must be greater than zero.");
                ModelState.AddModelError("TotalAmount", "Total amount must be greater than zero.");
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            if(model.AmountPaid < 0)
            {
                _logger.LogError("Amount paid cannot be negative.");
                ModelState.AddModelError("AmountPaid", "Amount paid cannot be negative.");
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            string invoiceStatus;

            if(model.AmountPaid < model.TotalAmount && model.AmountPaid > 0)
            {
                invoiceStatus = InvoiceStatusEnum.PartiallyPaid.ToString();
            }
            else if(model.AmountPaid == 0)
            {
                invoiceStatus = InvoiceStatusEnum.Unpaid.ToString();
            }
            else if(model.AmountPaid == model.TotalAmount)
            {
                invoiceStatus = InvoiceStatusEnum.Paid.ToString();
            }
            else
            {
                _logger.LogError("Invalid amount paid: {AmountPaid}", model.AmountPaid);
                ModelState.AddModelError("AmountPaid", "Invalid amount paid.");
                await PopulateDropdownsAsync(model);
                return View(model);
            }



            var invoice = new Invoice
                {
                    BookingID = model.BookingID,
                    // In the POST: InvoiceController/Create method, replace this line:
                    // CustomerID = model.CustomerID,

                    CustomerID = model.CustomerID.HasValue
                        ? model.CustomerID.Value
                        : throw new InvalidOperationException("CustomerID must not be null."),

                    InvoiceDate = model.InvoiceDate,
                    TotalAmount = model.TotalAmount,
                    AmountPaid = model.AmountPaid,
                    InvoiceStatus = Enum.Parse<InvoiceStatusEnum>(invoiceStatus),
                    // Replace this line in the POST: InvoiceController/Create method:
                    // GeneratedByEmployeeID = model.GeneratedByEmployeeID?? null // Nullable to handle cases where this might not be set

                    GeneratedByEmployeeID = model.GeneratedByEmployeeID.HasValue
                        ? model.GeneratedByEmployeeID.Value
                        : throw new InvalidOperationException("GeneratedByEmployeeID must not be null.")

                };
          
            try
            {

                int? id= await _invoiceService.AddInvoiceAsync(invoice);
                if(id == null)
                {
                    _logger.LogError("Failed to create invoice.");
                    ModelState.AddModelError("", "Failed to create invoice. Please try again.");
                    await PopulateDropdownsAsync(model);
                    return View(model);

                }
                else
                {
                    _logger.LogInformation("Invoice created successfully with ID: {Id}", id);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                _logger.LogError("An error occurred while creating the invoice.");
                await PopulateDropdownsAsync(model);
                return View(model);
            }
        }

        // GET: InvoiceController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if(id <= 0)
            {
                _logger.LogError("Invalid invoice ID: {Id}", id);
                return BadRequest("Invalid invoice ID.");
            }


           
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if(invoice == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found.", id);
                return NotFound($"Invoice with ID {id} not found.");
            }

            if (invoice.InvoiceStatus == InvoiceStatusEnum.Paid)
            {
                _logger.LogInformation("Attempt to edit a paid invoice with ID {Id}.", id);
                TempData["ErrorMessage"] = "Paid invoices cannot be edited. Please create a new invoice for any adjustments.";
                return RedirectToAction(nameof(Index));
            }

            var invoiceCreateEditViewModel = new InvoiceCreateEditViewModel
            {
               InvoiceID= invoice.InvoiceID,
                BookingID = invoice.BookingID,
                CustomerID = invoice.CustomerID,
                InvoiceDate = invoice.InvoiceDate ?? DateTime.MinValue,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid ?? 0m,
                InvoiceStatus = invoice.InvoiceStatus.ToString(),
                GeneratedByEmployeeID = invoice.GeneratedByEmployeeID
            };

            return View(invoiceCreateEditViewModel);
        }

        // POST: InvoiceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoiceCreateEditViewModel invoiceCreateEditView)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid for invoice ID: {Id}", id);
                await PopulateDropdownsAsync(invoiceCreateEditView);
                return View(invoiceCreateEditView);
            }
            if (invoiceCreateEditView.BookingID <= 0 || invoiceCreateEditView.CustomerID <= 0 || invoiceCreateEditView.GeneratedByEmployeeID <= 0)
            {
                _logger.LogError("Invalid BookingID, CustomerID, or GeneratedByEmployeeID for invoice ID: {Id}", id);
                ModelState.AddModelError("", "Invalid Booking, Customer, or Employee selection.");
                await PopulateDropdownsAsync(invoiceCreateEditView);
                return View(invoiceCreateEditView);
            }


            if (id <= 0)
            {
                _logger.LogError("Invalid invoice ID: {Id}", id);
                return BadRequest("Invalid invoice ID.");
            }

            if (id != invoiceCreateEditView.InvoiceID)
            {
                _logger.LogError("Invoice ID mismatch: {Id} != {ModelId}", id, invoiceCreateEditView.InvoiceID);
                ModelState.AddModelError("", "Invoice ID mismatch.");
                await PopulateDropdownsAsync(invoiceCreateEditView);
                return View(invoiceCreateEditView);
            }

            string invoiceStatus;

            if (invoiceCreateEditView.AmountPaid < invoiceCreateEditView.TotalAmount && invoiceCreateEditView.AmountPaid > 0)
            {
                invoiceStatus = InvoiceStatusEnum.PartiallyPaid.ToString();
            }
            else if (invoiceCreateEditView.AmountPaid == 0)
            {
                invoiceStatus = InvoiceStatusEnum.Unpaid.ToString();
            }
            else if (invoiceCreateEditView.AmountPaid == invoiceCreateEditView.TotalAmount)
            {
                invoiceStatus = InvoiceStatusEnum.Paid.ToString();
            }
            else
            {
                _logger.LogError("Invalid amount paid: {AmountPaid}", invoiceCreateEditView.AmountPaid);
                ModelState.AddModelError("AmountPaid", "Invalid amount paid.");
                await PopulateDropdownsAsync(invoiceCreateEditView);
                return View(invoiceCreateEditView);
            }



            var invoices = new Invoice
            {
                InvoiceID = invoiceCreateEditView.InvoiceID,
                BookingID = invoiceCreateEditView.BookingID,
                CustomerID = invoiceCreateEditView.CustomerID.HasValue
                ? invoiceCreateEditView.CustomerID.Value : throw new InvalidOperationException("CustomerID must not be null.")
                ,
                InvoiceDate = invoiceCreateEditView.InvoiceDate,
                TotalAmount = invoiceCreateEditView.TotalAmount,
                AmountPaid = invoiceCreateEditView.AmountPaid,
                InvoiceStatus = Enum.Parse<InvoiceStatusEnum>(invoiceCreateEditView.InvoiceStatus),
                GeneratedByEmployeeID = invoiceCreateEditView.GeneratedByEmployeeID.HasValue
                    ? invoiceCreateEditView.GeneratedByEmployeeID.Value
                    : throw new InvalidOperationException("GeneratedByEmployeeID must not be null.")
            };

            try
            {


                var result = await _invoiceService.UpdateInvoiceAsync(invoices);
                if (result)
                {
                    _logger.LogInformation("Invoice with ID {Id} updated successfully.", id);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Failed to update invoice with ID: {Id}", id);
                    ModelState.AddModelError("", "Failed to update invoice. Please try again.");
                    await PopulateDropdownsAsync(invoiceCreateEditView);
                    return View(invoiceCreateEditView);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the invoice with ID: {Id}", id);
                await PopulateDropdownsAsync(invoiceCreateEditView);
                return View(invoiceCreateEditView);

            }
        
        }

        // GET: InvoiceController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
           if(id <= 0)
            {
                _logger.LogError("Invalid invoice ID: {Id}", id);
                return BadRequest("Invalid invoice ID.");
            }
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if(invoice == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found.", id);
                return NotFound($"Invoice with ID {id} not found.");
            }
            var invoiceViewModel = new InvoiceViewModel
            {
                InvoiceID = invoice.InvoiceID,
                BookingID = invoice.BookingID,
                CustomerID = invoice.CustomerID,
                InvoiceDate = invoice.InvoiceDate ?? DateTime.MinValue,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid ?? 0m,
                InvoiceStatus = invoice.InvoiceStatus.ToString(),
                GeneratedByEmployeeID = invoice.GeneratedByEmployeeID
            };

          
                if (invoiceViewModel.InvoiceStatus == "Unpaid" || invoiceViewModel.InvoiceStatus == "PartiallyPaid")
                {
                    _logger.LogInformation("Invoice with ID {Id} is not eligible for deletion.", id);
                    TempData["ErrorMessage"] = "Only paid invoices can be deleted.";
                    return RedirectToAction(nameof(Index));
                }
            
                return View(invoiceViewModel);
        }

        // POST: InvoiceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, InvoiceViewModel invoiceViewModel)
        {
            if(id <= 0)
            {
                _logger.LogError("Invalid invoice ID: {Id}", id);
                return BadRequest("Invalid invoice ID.");
            }

            if(id != invoiceViewModel.InvoiceID)
            {
                _logger.LogError("Invoice ID mismatch: {Id} != {ModelId}", id, invoiceViewModel.InvoiceID);
                ModelState.AddModelError("", "Invoice ID mismatch.");
                return View(invoiceViewModel);
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if(invoice == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found.", id);
                return NotFound($"Invoice with ID {id} not found.");
            }
            try
            {
                bool isDeleted = await _invoiceService.DeleteInvoiceAsync(id);
                if(!isDeleted)
                {
                    _logger.LogError("Failed to delete invoice with ID: {Id}", id);
                    ModelState.AddModelError("", "Failed to delete invoice. Please try again.");
                    return View(invoiceViewModel);
                }
                else
                {
                                       _logger.LogInformation("Invoice with ID {Id} deleted successfully.", id);
                    return RedirectToAction(nameof(Index));
                }   
            }
            catch
            {
                _logger.LogError("An error occurred while deleting the invoice with ID: {Id}", id);
                ModelState.AddModelError("", "An error occurred while deleting the invoice. Please try again.");
             
                return View(invoiceViewModel);
            }
        }


        private async Task PopulateDropdownsAsync(InvoiceCreateEditViewModel model)
        {

            // Fetch all bookings that do not have an invoice, with their related customer and employee details.
            // The stored procedure's INNER JOINs guarantee that these properties are not null.
            var bookings = await _bookingService.GetBookingsWithAllDetails();

            // Fetch all bookings with their related details.
            var distinctCustomers = bookings
                 .Where(b => b.Customer != null)
                 .Select(b => b.Customer)
                 .DistinctBy(c => c.CustomerID)
                 .ToList();

            // Select all distinct employees from the list of bookings.
            // Corrected to use 'Employee' property as per your model.
            var distinctEmployees = bookings
                .Where(b => b.Employee != null)
                .Select(b => b.Employee)
                .DistinctBy(e => e.EmployeeID)
                .ToList();

           
            model.Bookings = bookings
                .Select(b => new SelectListItem
                {
                    Value = b.BookingID.ToString(),
                    Text = $"{b.BookingID} - {b.Customer.Firstname ?? "Unknown"} {b.Customer.Lastname ?? "Customer"} ({b.CheckInDate.ToShortDateString()} to {b.CheckOutDate.ToShortDateString()})"
                }).ToList();
        }

        private async Task PopulateBookingsDropdownAsync(InvoiceCreateEditViewModel model)
        {
            var bookings = await _bookingService.GetBookingsWithAllDetails();

            model.Bookings = bookings
                .Where(b => b.Customer != null && b.Employee != null) // تأكد أن البيانات كاملة
                .Select(b => new SelectListItem
                {
                    Value = b.BookingID.ToString(),
                    Text = $"{b.BookingID} - {b.Customer.Firstname} {b.Customer.Lastname} ({b.CheckInDate.ToShortDateString()} to {b.CheckOutDate.ToShortDateString()})",
                }).ToList();
        }

        // A new method for get the details of Booking => AJAX
        [HttpGet]
        public async Task<IActionResult> GetBookingDetails(int bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            var result = new
            {
                customerID = booking.Customer.CustomerID,
                customerName = $"{booking.Customer.Firstname} {booking.Customer.Lastname}",
                employeeID = booking.Employee.EmployeeID,
                employeeName = $"{booking.Employee.FirstName} {booking.Employee.LastName}",
                totalAmount = booking.TotalPrice, 
                checkInDate = booking.CheckInDate.ToShortDateString(),
                checkOutDate = booking.CheckOutDate.ToShortDateString()
            };

            return Json(result);
        }

    }



}
