using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models.ViewModels.RoomModel;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Web.Controllers
{

    public class RoomsController : Controller
    {

        private readonly IRoomService _roomService;
        private readonly ILogger<CustomersController> _logger;
        private readonly IRoomTypeService _roomTypeService;
        private readonly IRoomStatuseService _roomStatusService;
        public RoomsController(IRoomService roomService, ILogger<CustomersController> logger, IRoomTypeService roomTypeService, IRoomStatuseService roomStatusService)
        {
            _roomService = roomService;
            _logger = logger;
            _roomTypeService = roomTypeService;
            _roomStatusService = roomStatusService;
        }

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            if (rooms == null || !rooms.Any())
            {
                _logger.LogWarning("No rooms found.");
                return View("No rooms available");
            }


            var roomViewModelTasks = rooms.Select(async room => new RoomViewModel
            {
                RoomID = room.RoomID,
                RoomNumber = room.RoomNumber,
                BedCount = room.BedCount,
                PricePerNight = room.PricePerNight ?? 0,
                // هذه الدوال غير المتزامنة ستُنفذ كمهام
                RoomStatusName = (await _roomStatusService.GetRoomStatusByIdAsync(room.RoomStatusID))?.StatusName ?? "Unknown",
                RoomTypeName = (await _roomTypeService.GetRoomTypeByIdAsync(room.RoomTypeID))?.TypeName ?? "Unknown"
            });

            // الخطوة 2: انتظار اكتمال جميع المهام
            // await Task.WhenAll() ستُرجع IEnumerable<RoomViewModel> بعد اكتمال جميع المهام
            var roomViewModels = (await Task.WhenAll(roomViewModelTasks)).ToList();

            return View(roomViewModels);
        }


        [HttpGet]
        [ActionName("Details")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Details: Invalid Room ID provided.");
                return BadRequest("Invalid Room ID.");
            }

            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning($"Details: Room with ID {id} not found.");
                return NotFound("Room not found.");
            }

            var roomViewModel = new RoomViewModel
            {
                RoomID = room.RoomID,
                RoomNumber = room.RoomNumber,
                BedCount = room.BedCount,
                RoomTypeName = room.RoomType?.TypeName ?? "Unknown",
                PricePerNight = room.PricePerNight ?? 0,
                RoomStatusName = room.RoomStatus?.StatusName ?? "Unknown"
            };

            return View(roomViewModel);
        }


        [HttpGet] // for initial GET request
        public async Task<IActionResult> Create()
        {

            var viewModel = new RoomCreateViewModel();

            //  تعبئة القوائم المنسدلة (Dropdowns)
            // نستخدم الدوال من RoomService التي تُرجع IEnumerable<SelectListItem> مباشرةً
            viewModel.RoomTypes = await _roomService.GetAllRoomTypesAsSelectListItemsAsync();
            viewModel.RoomStatuses = await _roomService.GetAllRoomStatusesAsSelectListItemsAsync();


            return View(viewModel);
        }


        [HttpPost]// for form submission
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomCreateViewModel model)
        {
            // Check for null model early
            if (model == null)
            {
                _logger.LogWarning("Create: Room model is null.");
                return BadRequest("Room data is null.");
            }

            // --- IMPORTANT: Define this outside the try-catch for accessibility ---
            // We'll use this flag to decide if we need to re-populate dropdowns
            bool requiresDropdownRepopulation = false;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create: Model state is invalid.");
                requiresDropdownRepopulation = true; // Mark that dropdowns are needed
            }
            else
            {
                // Only map if model state is valid
                var room = new Room
                (
                    model.RoomNumber,
                    model.BedCount,
                    model.RoomTypeID,
                    model.PricePerNight,
                    model.RoomStatusID

                );

                try
                {
                    int? roomID = await _roomService.AddNewRoomAsync(room);
                    if (roomID.HasValue && roomID.Value > 0)
                    {
                        _logger.LogInformation($"Create: Room created successfully with ID {roomID.Value}.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning("Create: Room creation failed, no ID returned.");
                        ModelState.AddModelError(string.Empty, "Room creation failed. Please try again.");
                        requiresDropdownRepopulation = true; // Mark that dropdowns are needed
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Create: Error occurred while creating Room.");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the Room. Please try again later.");
                    requiresDropdownRepopulation = true; // Mark that dropdowns are needed
                }
            }

            // --- Re-populate dropdowns ONLY if necessary (i.e., we're returning the view with errors) ---
            if (requiresDropdownRepopulation)
            {
                model.RoomTypes = await _roomService.GetAllRoomTypesAsSelectListItemsAsync();
                model.RoomStatuses = await _roomService.GetAllRoomStatusesAsSelectListItemsAsync();
            }

            // Always return the view with the model if we haven't redirected
            return View(model);
        }


        [HttpGet]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Edit: Invalid Room ID provided.");
                return BadRequest("Invalid Room ID.");
            }

            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning($"Edit: Room with ID {id} not found.");
                return NotFound("Room not found.");
            }

            var viewModel = new RoomEditViewModel
            {
                RoomID = room.RoomID,
                RoomNumber = room.RoomNumber,
                BedCount = room.BedCount,
                RoomTypeID = room.RoomTypeID,
                PricePerNight = room.PricePerNight ?? 0,
                RoomStatusID = room.RoomStatusID,
            };

            viewModel.RoomTypes = await _roomService.GetAllRoomTypesAsSelectListItemsAsync();
            viewModel.RoomStatuses = await _roomService.GetAllRoomStatusesAsSelectListItemsAsync();

            return View(viewModel);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, RoomEditViewModel model)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Edit: Invalid Room ID provided.");
                return BadRequest("Invalid Room ID.");
            }

            if (!ModelState.IsValid)
            {
                // إذا كان الـ model غير صالح، يجب إعادة تعبئة الـ dropdowns
                model.RoomTypes = await _roomService.GetAllRoomTypesAsSelectListItemsAsync();
                model.RoomStatuses = await _roomService.GetAllRoomStatusesAsSelectListItemsAsync();
                return View(model);

            }
            if (model == null)
            {
                _logger.LogWarning("Edit: Room model is null.");
                return BadRequest("Room data is null.");
            }
            if (id != model.RoomID)
            {
                _logger.LogWarning($"Edit: Room ID mismatch. Provided ID: {id}, Model ID: {model.RoomID}");
                return BadRequest("Room ID mismatch.");
            }


            var room = new Room
            (
                model.RoomID, // RoomID مطلوب هنا للتحديث
                model.RoomNumber,
                model.BedCount,
                model.RoomTypeID,
                model.PricePerNight,
                model.RoomStatusID
            );

            try
            {
                bool success = await _roomService.UpdateRoomAsync(room);
                if (success)
                {
                    TempData["SuccessMessage"] = "Room updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Room update failed. Please try again.");
                    model.RoomTypes = await _roomService.GetAllRoomTypesAsSelectListItemsAsync();
                    model.RoomStatuses = await _roomService.GetAllRoomStatusesAsSelectListItemsAsync();
                    return View(model);
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Edit: Room with ID {model.RoomID} not found during update.");
                ModelState.AddModelError(string.Empty, ex.Message);
                model.RoomTypes = await _roomService.GetAllRoomTypesAsSelectListItemsAsync();
                model.RoomStatuses = await _roomService.GetAllRoomStatusesAsSelectListItemsAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Edit: Error occurred while updating Room with ID {model.RoomID}.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the Room. Please try again later.");
                model.RoomTypes = await _roomService.GetAllRoomTypesAsSelectListItemsAsync();
                model.RoomStatuses = await _roomService.GetAllRoomStatusesAsSelectListItemsAsync();
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Room ID.");
            }

            var rooms = await _roomService.GetRoomByIdAsync(id);
            if (rooms == null)
            {
                return NotFound("Room not found.");
            }

            var roomViewModel = new RoomViewModel
            {
                RoomID = rooms.RoomID,
                RoomNumber = rooms.RoomNumber,
                BedCount = rooms.BedCount,
                RoomTypeName = rooms.RoomType?.TypeName ?? "Unknown",
                PricePerNight = rooms.PricePerNight ?? 0,
                RoomStatusName = rooms.RoomStatus?.StatusName ?? "Unknown"
            };

            return View(roomViewModel);

        }



        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Room ID.");
            }

            var rooms = await _roomService.GetRoomByIdAsync(id);
            if (rooms == null)
            {
                return NotFound("Room not found.");
            }

            try
            {

                bool IsDeleted = await _roomService.DeleteRoomAsync(id);
                if (IsDeleted)
                {
                    _logger.LogInformation($"DeleteConfirmed: Room with ID {id} deleted successfully.");
                    TempData["SuccessMessage"] = "Room deleted successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning($"DeleteConfirmed: Room with ID {id} could not be deleted.");
                    ModelState.AddModelError(string.Empty, "Room deletion failed. Please try again.");
                    return View(new RoomViewModel
                    {
                        RoomID = rooms.RoomID,
                        RoomNumber = rooms.RoomNumber,
                        BedCount = rooms.BedCount,
                        RoomTypeName = rooms.RoomType?.TypeName ?? "Unknown",
                        PricePerNight = rooms.PricePerNight ?? 0,
                        RoomStatusName = rooms.RoomStatus?.StatusName ?? "Unknown"
                    });
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteConfirmed: Error occurred while deleting Room with ID {id}.");
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the Room. Please try again later.");
                return View(new RoomViewModel
                {
                    RoomID = rooms.RoomID,
                    RoomNumber = rooms.RoomNumber,
                    BedCount = rooms.BedCount,
                    RoomTypeName = rooms.RoomType?.TypeName ?? "Unknown",
                    PricePerNight = rooms.PricePerNight ?? 0,
                    RoomStatusName = rooms.RoomStatus?.StatusName ?? "Unknown"
                });
            }
        }
    }
}
