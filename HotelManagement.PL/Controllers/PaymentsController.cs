using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models.ViewModels.PaymentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;


namespace HotelManagement.Web.Controllers
{
    public class PaymentsController : Controller
    {

        private readonly ILogger<PaymentsController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IEmployeeService _employeeService;
        private readonly IInvoiceService _invoiceService;
        private readonly IBookingService _bookingService;

        public PaymentsController(ILogger<PaymentsController> logger, IPaymentService paymentService, IPaymentMethodService paymentMethodService, IEmployeeService employeeService, IInvoiceService invoiceService, IBookingService bookingService)
        {
            _logger = logger;
            _paymentService = paymentService;
            _paymentMethodService = paymentMethodService;
            _employeeService = employeeService;
            _invoiceService = invoiceService;
            _bookingService = bookingService;
        }


        // GET: PaymentController
        public async Task<IActionResult> Index()
        {
            var payments = await _paymentService.GetPaymentDetailsAsync();

            if (payments == null || !payments.Any())
            {
                _logger.LogInformation("No payments found.");
                ViewBag.Message = "There are no payments yet.";
                return View(new List<PaymentDetailsViewModel>()); // قائمة فارغة
            }

            var model = payments.Select(p => new PaymentDetailsViewModel
            {
                PaymentID = p.PaymentID,
                InvoiceID = p.InvoiceID,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethodID = p.PaymentMethodID,
                TransactionReference = p.TransactionReference,
                RecordedByEmployeeID = p.RecordedByEmployeeID,
                Username = p.Username,
                InvoiceStatus = p.InvoiceStatus,
                PaymentMethodName = p.PaymentMethodName
            }).ToList();

            return View(model);
        }


        // GET: PaymentController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Details: Invalid Payment ID.");
                ViewBag.Message = "Invalid Payment ID.";
                return View("DetailsEmpty");
            }

            var payment = await _paymentService.GetPaymentDetailsByIdAsync(id);

            if (payment == null)
            {
                _logger.LogInformation($"Details: Payment with ID {id} not found.");
                ViewBag.Message = $"Payment with ID {id} not found.";
                return View("DetailsEmpty"); // أنشئ View جديد يعرض الرسالة
            }

            var model = new PaymentDetailsByIdViewModel
            {
                PaymentID = payment.PaymentID,
                InvoiceID = payment.InvoiceID,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethodName = payment.PaymentMethodName,
                TransactionReference = payment.TransactionReference ?? string.Empty,
                RecordedByEmployeeID = payment.RecordedByEmployeeID ?? 0,
                EmployeeName = payment.Username,
                InvoiceStatus = payment.InvoiceStatus.ToString()
            };

            return View(model); // View يعرض تفاصيل الدفع
        }



        // GET: PaymentController/Create
        public async Task<IActionResult> Create()
        {
            var unpaidBookings = await _bookingService.GetUnpaidBookingsAsync();

            // Setup the ViewModel
            var viewModel = new PaymentCreateViewModel
            {
                // Create a SelectList to populate the dropdown on the view
                UnpaidBookings = new SelectList(unpaidBookings, "BookingID", "FullName")
            };

            //fill the dropdowns and other necessary data
            await PopulatePaymentAsync(viewModel);

            // Send the ViewModel to the View
            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PaymentCreateViewModel model)
        {
            // Re-populate the dropdown lists in case the model state is invalid
            await PopulatePaymentAsync(model);

            // Manual check for InvoiceID to enforce business logic
            if (!model.InvoiceID.HasValue || model.InvoiceID.Value <= 0)
            {
                if (model.BookingID.HasValue && model.BookingID != 0)
                {
                    var invoices = await _invoiceService.GetInvoicesBybookingIdAsync(model.BookingID.Value);
                    var invoice = invoices.FirstOrDefault();
                    if (invoice != null)
                    {
                        model.InvoiceID = invoice.InvoiceID;
                    }
                    else
                    {
                        ModelState.AddModelError("InvoiceID", "No invoices found for the selected booking.");
                    }
                }
                else
                {
                    ModelState.AddModelError("InvoiceID", "An invoice is required.");
                }
            }

            // Now, check the model state. It will include any errors we added manually above.
            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid. Found {Count} validation errors.", ModelState.Values.SelectMany(v => v.Errors).Count());

                foreach (var state in ModelState)
                {
                    // Fix: use FirstOrDefault() and check for null to avoid "Sequence contains no elements"
                    var error = state.Value.Errors.FirstOrDefault();
                    if (error != null)
                    {
                        _logger.LogError("Validation error on field: {Field}, Error: {Error}", state.Key, error.ErrorMessage);
                    }
                }

                return View("Create", model);
            }

            var payment = new Payment
            {
                InvoiceID = model.InvoiceID.Value,
                Amount = model.Amount,
                PaymentDate = model.PaymentDate,
                PaymentMethodID = model.PaymentMethodID,
                TransactionReference = model.TransactionReference,
                RecordedByEmployeeID = model.RecordedByEmployeeID,
            };

            try
            {
                await _paymentService.AddPaymentAsync(payment);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogError(ex, "Invalid operation while adding a payment: {Message}", ex.Message);
                return View("Create", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                _logger.LogError(ex, "An unexpected error occurred while adding a payment.");
                return View("Create", model);
            }
        }



        // GET: PaymentController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if(id <= 0)
            {
                _logger.LogError("Edit:Invalid Payment ID.");
                return BadRequest("Invalid Payment ID.");
            }

            var payment =await _paymentService.GetPaymentByIdAsync(id);
            if(payment == null)
            {
                _logger.LogError($"Edit:Payment with ID {id} not found.");
                return NotFound($"Payment with ID {id} not found.");
            }

           

            var model = new PaymentEditViewModel
            {

                PaymentID = payment.PaymentID,
                InvoiceID = payment.InvoiceID,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate ?? DateTime.Now,
                PaymentMethodID = payment.PaymentMethodID,
                TransactionReference = payment.TransactionReference,
                RecordedByEmployeeID = payment.RecordedByEmployeeID

            };

            int? bookingID = await _invoiceService.GetBookingIdByInvoiceIdAsync(payment.InvoiceID);
            if(bookingID.HasValue && bookingID.Value > 0)
            {
                model.BookingID = bookingID.Value;
            }

            await PopulateDropdowns(model);

            return View(model);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, PaymentEditViewModel model)
        {
            if (id <= 0)
            {
                _logger.LogError("Edit:Invalid Payment ID.");
                return BadRequest("Invalid Payment ID.");
            }

            if (id != model.PaymentID)
            {
                _logger.LogError("Edit:Payment ID mismatch.");
                ModelState.AddModelError("", "Payment ID mismatch.");
                // Populate dropdowns before returning the view
                await PopulateDropdowns(model);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid. Found {Count} validation errors.", ModelState.Values.SelectMany(v => v.Errors).Count());
                foreach (var state in ModelState)
                {
                    var error = state.Value.Errors.FirstOrDefault();
                    if (error != null)
                    {
                        _logger.LogError("Validation error on field: {Field}, Error: {Error}", state.Key, error.ErrorMessage);
                    }
                }
                // Populate dropdowns before returning the view
                await PopulateDropdowns(model);
                return View("Edit", model);
            }

            try
            {
                var payment = new Payment
                {
                    PaymentID = model.PaymentID,
                    InvoiceID = model.InvoiceID.Value,
                    Amount = model.Amount,
                    PaymentDate = model.PaymentDate,
                    PaymentMethodID = model.PaymentMethodID,
                    TransactionReference = model.TransactionReference,
                    RecordedByEmployeeID = model.RecordedByEmployeeID
                };

                bool isUpdated = await _paymentService.UpdatePaymentAsync(payment);
                if (!isUpdated)
                {
                    _logger.LogError($"Edit:Failed to update payment with ID {id}.");
                    ModelState.AddModelError("", "Failed to update the payment. It may not exist or is already deleted.");
                    // Populate dropdowns before returning the view
                    await PopulateDropdowns(model);
                    return View(model);
                }
                else
                {
                    _logger.LogInformation($"Edit:Payment with ID {id} updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the payment with ID {ID}.", id);
                ModelState.AddModelError("", "An unexpected error occurred while updating the payment. Please try again.");
                // Populate dropdowns before returning the view
                await PopulateDropdowns(model);
                return View("Edit", model);
            }
        }

        private async Task PopulateDropdowns(PaymentEditViewModel model)
        {
            var paymentMethods = await _paymentMethodService.GetAllPaymentMethodAsync();
            model.PaymentMethodsList = paymentMethods.Select(pm => new SelectListItem
            {
                Value = pm.MethodID.ToString(),
                Text = pm.MethodName
            }).ToList();

            var employees = await _employeeService.GetAllEmployeesAsync();
            model.EmployeeList = employees.Select(e => new SelectListItem
            {
                Value = e.EmployeeID.ToString(),
                Text = $"{e.FirstName} {e.LastName} ({e.Username})"
            }).ToList();
        }
    


        // GET: PaymentController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
          var payment = await _paymentService.GetPaymentDetailsByIdAsync(id);
            if (payment == null)
            {
                _logger.LogError($"Delete:Payment with ID {id} not found.");
                return NotFound($"Payment with ID {id} not found.");
            }
            var model = new PaymentDetailsByIdViewModel
            {
                PaymentID = payment.PaymentID,
                InvoiceID = payment.InvoiceID,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethodName = payment.PaymentMethodName,
                TransactionReference = payment.TransactionReference ?? string.Empty,
                RecordedByEmployeeID = payment.RecordedByEmployeeID ?? 0,
                EmployeeName = payment.Username,
                InvoiceStatus = payment.InvoiceStatus.ToString()
            };
            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Delete:Invalid Payment ID.");
                return BadRequest("Invalid Payment ID.");
            }

            try
            {
                var payment = await _paymentService.GetPaymentDetailsByIdAsync(id);
                if (payment == null)
                {
                    _logger.LogError($"Delete:Payment with ID {id} not found.");
                    return NotFound($"Payment with ID {id} not found.");
                }
                // Call the service to delete the payment
                var result = await _paymentService.DeletePaymentAsync(id);
                if (!result)
                {
                    _logger.LogError($"Delete:Failed to delete payment with ID {id}.");

                    ModelState.AddModelError("", "Failed to delete the payment. It may not exist or is already deleted.");
                    return View(new PaymentDetailsByIdViewModel
                    {
                        PaymentID = payment.PaymentID,
                        InvoiceID = payment.InvoiceID,
                        Amount = payment.Amount,
                        PaymentDate = payment.PaymentDate,
                        PaymentMethodName = payment.PaymentMethodName,
                        TransactionReference = payment.TransactionReference ?? string.Empty,
                        RecordedByEmployeeID = payment.RecordedByEmployeeID ?? 0,
                        EmployeeName = payment.Username,
                        InvoiceStatus = payment.InvoiceStatus.ToString()
                    });
                }
                else
                {
                    _logger.LogInformation($"Delete:Payment with ID {id} deleted successfully.");

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the payment with ID {ID}.", id);
                ModelState.AddModelError("", "An unexpected error occurred while deleting the payment. Please try again.");
                // Optionally, you can try to reload the payment details for the view, or just return a generic error view.
                return View();
            }
        }

     

        private async Task PopulatePaymentAsync(PaymentCreateViewModel model)
        {
            // Populate Payment Methods
            var paymentMethods = await _paymentMethodService.GetAllPaymentMethodAsync();
            model.PaymentMethodList = paymentMethods.Select(pm => new SelectListItem
            {
                Value = pm.MethodID.ToString(),
                Text = pm.MethodName
            }).ToList();

            // Populate Employees
            var employees = await _employeeService.GetAllEmployeesAsync();
            model.EmployeeList = employees.Select(e => new SelectListItem
            {
                Value = e.EmployeeID.ToString(),
                Text = $"{e.FirstName} {e.LastName} ({e.Username})"
            }).ToList();

            // Populate Bookings. Using the DTO is a great idea to make this more specific.
            var bookings = await _bookingService.GetUnpaidBookingsAsync(); // Use the unpaid bookings
            model.UnpaidBookings = new SelectList(bookings, "BookingID", "FullName", model.BookingID);

            // Now you can populate the invoice list based on the selected booking
            if (model.BookingID.HasValue &&  model.BookingID != 0)
            {
                var filteredInvoices = await _invoiceService.GetInvoicesBybookingIdAsync(model.BookingID.Value);
                model.InvoiceList = filteredInvoices.Select(i => new SelectListItem
                {
                    Value = i.InvoiceID.ToString(),
                    Text = $"Invoice #{i.InvoiceID} - BalanceDue: {i.BalanceDue:C}"
                }).ToList();
            }
            else
            {
                model.InvoiceList = new List<SelectListItem>();
            }
        }

        // Additional helper actions for AJAX
        [HttpGet]
        public async Task<IActionResult> GetInvoicesByBooking(int bookingId)
        {
            var filteredInvoices = await _invoiceService.GetInvoicesBybookingIdAsync(bookingId);
            var invoiceList = filteredInvoices.Select(i => new
            {
                value = i.InvoiceID.ToString(),
                text = $"Invoice #{i.InvoiceID} - BalanceDue: {i.BalanceDue:C}"
            }).ToList();
            return Json(invoiceList);
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceDetails(int invoiceId)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }
            return Json(new { balanceDue = invoice.BalanceDue });
        }

        [HttpGet]
        public async Task<IActionResult> UnpaidBookings()
        {
            var unpaidBookings = await _bookingService.GetUnpaidBookingsAsync();
            return View(unpaidBookings);
        }
    }
    }