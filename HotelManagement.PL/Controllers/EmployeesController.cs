using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;



namespace HotelManagement.Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        ILogger<EmployeesController> _logger;
        private readonly IPasswordHasherService _passwordHasher;
        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger, IPasswordHasherService passwordHasher)
        {
            _employeeService = employeeService;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync(); // Get domain entities

            // Map domain entities to ViewModels
            var employeeViewModels = employees.Select(e => new EmployeeViewModel
            {
                EmployeeID = e.EmployeeID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Username = e.Username,
                Role = (Employee.EmployeeRole)Enum.Parse(typeof(Employee.EmployeeRole), e.Role),
                HireDate = e.HireDate
            }).ToList();

            return View(employeeViewModels); // Pass the list of ViewModels to the view
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if(id<=0)
            {
                _logger.LogWarning("Details: Invalid Employee ID provided: {Id}", id);
                return BadRequest("Invalid Employee ID.");
            }

            var employee=await _employeeService.GetEmployeeByIdAsync(id);

            if(employee == null)
            {
                _logger.LogWarning("Details: Employee not found for ID: {Id}", id);
                return NotFound("Employee not found.");
            }

            var employeeViewModel = new EmployeeViewModel
            {
              //  EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Username = employee.Username,
                Role = (Employee.EmployeeRole)Enum.Parse(typeof(Employee.EmployeeRole), employee.Role),
                HireDate = employee.HireDate
            };

            return View(employeeViewModel); // Return the view with the employee details.
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("Create: Employee data model was null during POST request.");
                return BadRequest("Employee data is null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create: Model state invalid for Employee create. Errors: {Errors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            try
            {
                string hashPassword = _passwordHasher.HashPassword(model.Password);

                var employeeDomain = new Employee(
                    model.FirstName,
                    model.LastName,
                    model.Username,
                    hashPassword,
                    model.Role.ToString(),
                    model.HireDate
                );

                await _employeeService.AddEmployeeAsync(employeeDomain);
                _logger.LogInformation($"Create: Employee created successfully. EmployeeID: {employeeDomain.EmployeeID}, Username: {employeeDomain.Username}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error creating employee. ViewModel: {@Model}", model); // {@Model} لتسجيل كائن المودل كاملاً
                ModelState.AddModelError("", "An unexpected error occurred while adding the employee. Please try again.");
                return View(model);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Delete: Invalid Employee ID provided: {Id}", id);
                return BadRequest("Invalid Employee ID.");
            }


            var Employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (Employee == null)
            {
                _logger.LogWarning("Delete: Employee not found for ID: {Id}", id);
                return NotFound("Employee not found.");
            }
            try
            {
                bool isDeleted = await _employeeService.DeleteEmployeeAsync(id);
                if (isDeleted)
                {
                    _logger.LogInformation("Delete: Employee with ID {Id} deleted successfully.", id);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("Delete: Failed to delete employee with ID {Id}.", id);
                    return NotFound("Failed to delete employee.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with ID {Id}.", id);
                return StatusCode(500, "An error occurred while deleting the employee. Please try again later.");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Delete: Invalid Employee ID provided: {Id}", id);
                return BadRequest("Invalid Employee ID.");
            }


            var Employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (Employee == null)
            {
                _logger.LogWarning("Delete: Employee not found for ID: {Id}", id);
                return NotFound("Employee not found.");
            }

            var employeeViewModel = new EmployeeViewModel
            {
                EmployeeID = Employee.EmployeeID,
                FirstName = Employee.FirstName,
                LastName = Employee.LastName,
                Username = Employee.Username,
                Role = (Employee.EmployeeRole)Enum.Parse(typeof(Employee.EmployeeRole), Employee.Role),
                HireDate = Employee.HireDate
            };

            return View(employeeViewModel); // Return the view with the employee details for confirmation.
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if(id<=0)
            {
                _logger.LogWarning("Edit: Invalid Employee ID provided: {Id}", id);
                return BadRequest("Invalid Employee ID.");
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if(employee == null)
            {
                _logger.LogWarning("Edit: Employee not found for ID: {Id}", id);
                return NotFound("Employee not found.");
            }

            var employeeViewModel = new EmployeeEditViewModel
            {
               // EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Username = employee.Username,
                Role = (Employee.EmployeeRole)Enum.Parse(typeof(Employee.EmployeeRole), employee.Role),
                HireDate = employee.HireDate
            };

            return View(employeeViewModel); // Return the view with the employee details for editing.

        }

        [HttpPost]
        [ValidateAntiForgeryToken] // حماية ضد هجمات تزوير الطلبات عبر المواقع (CSRF)
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel model)
        {
            // 1. التحقق الأولي من صحة معرف الموظف (ID)
            if (id <= 0)
            {
                _logger.LogWarning("Edit (POST): Invalid Employee ID provided: {Id}", id);
                return BadRequest("Invalid Employee ID.");
            }

            // 2. التحقق من صحة النموذج (Model State) فوراً
            // إذا كانت البيانات المرسلة من النموذج غير صالحة، لا داعي لإكمال باقي المنطق.
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit (POST): Model state invalid for Employee ID: {Id}. Errors: {Errors}",
                    id, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model); // إعادة عرض النموذج مع رسائل الأخطاء
            }

            // 3. جلب بيانات الموظف الحالي من قاعدة البيانات
            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                _logger.LogWarning("Edit (POST): Employee not found for ID: {Id}", id);
                return NotFound("Employee not found.");
            }

            try
            {
                // 4. تحديث خصائص الموظف الموجودة ببيانات النموذج المدخلة
                // ننقل البيانات المحدثة من الـ ViewModel إلى كائن الـ Domain.
               
                existingEmployee.FirstName = model.FirstName;
                existingEmployee.LastName = model.LastName;
                existingEmployee.Username = model.Username;
                existingEmployee.Role = model.Role.ToString(); // تحويل الـ Enum إلى string
                existingEmployee.HireDate = model.HireDate;

                // 5. التعامل مع تغيير كلمة المرور بشكل شرطي ومحكم
                // نقوم بتشفير كلمة المرور فقط إذا أدخل المستخدم كلمة مرور جديدة.
                if (!string.IsNullOrEmpty(model.NewPassword)) // تحقق إذا كانت هناك كلمة مرور جديدة مدخلة
                {
                    // التحقق من تطابق كلمة المرور الجديدة وتأكيدها
                    if (model.NewPassword != model.ConfirmNewPassword)
                    {
                        ModelState.AddModelError("ConfirmNewPassword", "New password and confirmation do not match.");
                        _logger.LogWarning("Edit (POST): New password and confirmation do not match for Employee ID: {Id}", id);
                        return View(model); // إعادة عرض النموذج بالخطأ المحدد لكلمة المرور
                    }

                    // إذا كانت كلمة المرور الجديدة موجودة ومتطابقة، نقوم بتشفيرها وتعيينها
                    existingEmployee.PasswordHash = _passwordHasher.HashPassword(model.NewPassword);
                    _logger.LogInformation("Edit (POST): Password updated for Employee ID: {Id}", id);
                }
                // ملاحظة: إذا كانت model.NewPassword فارغة أو null، فإن PasswordHash الحالي للموظف
                // (existingEmployee.PasswordHash) لن يتغير، وهذا هو السلوك الصحيح.

                // 6. استدعاء خدمة طبقة الأعمال لتحديث الموظف في قاعدة البيانات
                bool isUpdated = await _employeeService.UpdateEmployeeAsync(existingEmployee);

                // 7. التعامل مع نتيجة عملية التحديث وتقديم التغذية الراجعة
                if (isUpdated)
                {
                    _logger.LogInformation("Edit (POST): Employee with ID {Id} updated successfully.", id);
                    return RedirectToAction(nameof(Index)); // إعادة توجيه المستخدم إلى صفحة القائمة عند النجاح
                }
                else
                {
                    // إذا فشل التحديث (مثل عدم تأثر أي صف في قاعدة البيانات)، نسجل تحذيراً ونعرض رسالة خطأ عامة.
                    _logger.LogWarning("Edit (POST): Employee with ID {Id} update failed in service layer.", id);
                    ModelState.AddModelError("", "Failed to update employee. Please check the data and try again.");
                    return View(model); // إعادة عرض النموذج مع رسالة الخطأ
                }
            }
            catch (Exception ex)
            {
                // معالجة أي أخطاء غير متوقعة تحدث أثناء عملية التحديث
                _logger.LogError(ex, "Edit (POST): An unexpected error occurred while updating employee with ID: {Id}. ViewModel: {@Model}", id, model);
                ModelState.AddModelError("", "An unexpected error occurred while updating the employee. Please try again.");
                return View(model); // إعادة عرض النموذج مع رسالة الخطأ العام
            }
        }






    }

}