using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models.ViewModels.RoomStatusModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace HotelManagement.Web.Controllers
{
    public class RoomStatusController : Controller
    {
        private readonly IRoomStatuseService _roomStatuseService;



        private readonly ILogger<IRoomTypeService> _logger;

        public RoomStatusController(IRoomStatuseService roomStatuseService, ILogger<IRoomTypeService> logger)
        {
            _roomStatuseService = roomStatuseService;

            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var roomStatuses = await _roomStatuseService.GetAllRoomsStatusesAsync();

            if (roomStatuses == null || !roomStatuses.Any())
            {
                _logger.LogInformation("No room statuses found.");
                return View(new List<RoomStatusViewModel>());
            }

            var roomStatusViewModels = roomStatuses.Select(rs => new RoomStatusViewModel
            {
                RoomStatusID = rs.RoomStatusID,
                StatusName = rs.StatusName,
                Description = rs.Description
            }).ToList();

            return View(roomStatusViewModels);
        }


        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid RoomStatus ID: {Id}", id);
                return NotFound();
            }

            var roomStatus = await _roomStatuseService.GetRoomStatusByIdAsync(id);
            if (roomStatus == null)
            {
                _logger.LogInformation("RoomStatus with ID {Id} not found.", id);
                return NotFound();
            }

            var roomStatusViewModel = new RoomStatusViewModel
            {
                RoomStatusID = roomStatus.RoomStatusID,
                StatusName = roomStatus.StatusName,
                Description = roomStatus.Description
            };

            return View(roomStatusViewModel);
        }


        [HttpGet]
        public ActionResult Create()
        {
            var roomStatus = new RoomStatusCreateEditViewModel
            {
                RoomStatusID = 0, // Default value for new status
                StatusName = string.Empty,
                Description = string.Empty
            };
            return View(roomStatus);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoomStatusCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for RoomStatus creation. Errors: {Errors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            // Map ViewModel to Domain Model
            var roomStatus = new RoomStatus(model.StatusName, model.Description);


            try
            {
                int? id = await _roomStatuseService.AddNewRoomStatusAsync(roomStatus);

                if (id.HasValue && id.Value > 0)
                {
                    _logger.LogInformation("RoomStatus created successfully with ID: {Id}", id.Value);
                    TempData["SuccessMessage"] = "Room status created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Failed to create RoomStatus. Service returned invalid ID: {Id}", id);
                    ModelState.AddModelError("", "Failed to create RoomStatus. Please try again.");
                    return View(model);
                }
            }
            catch (Exception ex) // Catch specific exception for better logging
            {
                _logger.LogError(ex, "An unexpected error occurred while creating RoomStatus.");
                ModelState.AddModelError("", "An unexpected error occurred while creating the room status. Please try again.");
                return View(model);
            }
        }

        // GET: RoomStatusController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid RoomStatus ID: {Id}", id);
                return NotFound();
            }
            var roomStatus = _roomStatuseService.GetRoomStatusByIdAsync(id).Result;
            if (roomStatus == null)
            {
                _logger.LogInformation("RoomStatus with ID {Id} not found.", id);
                return NotFound();
            }


            var roomStatusViewModel = new RoomStatusCreateEditViewModel
            {
                RoomStatusID = roomStatus.RoomStatusID,
                StatusName = roomStatus.StatusName,
                Description = roomStatus.Description
            };
            return View(roomStatusViewModel);

        }

        // POST: RoomStatusController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, RoomStatusCreateEditViewModel model)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid RoomStatus ID: {Id}", id);
                return NotFound();
            }

            if (id != model.RoomStatusID)
            {
                _logger.LogWarning("RoomStatus ID mismatch. Expected: {ExpectedId}, Provided: {ProvidedId}", id, model.RoomStatusID);
                ModelState.AddModelError("", "RoomStatus ID mismatch.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for RoomStatus edit. Errors: {Errors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            var roomStatus = new RoomStatus
            {
                RoomStatusID = model.RoomStatusID
                ,
                StatusName = model.StatusName
                ,
                Description = model.Description
            };

            try
            {
                bool isUpdated = await _roomStatuseService.UpdateRoomStatusAsync(roomStatus);
                if (isUpdated)
                {
                    _logger.LogInformation("RoomStatus with ID {Id} updated successfully.", id);
                    TempData["SuccessMessage"] = "Room status updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Failed to update RoomStatus with ID: {Id}", id);
                    ModelState.AddModelError("", "Failed to update RoomStatus. Please try again.");
                    return View(model);
                }
            }
            catch
            {
                _logger.LogError("An unexpected error occurred while updating RoomStatus with ID: {Id}", id);
                ModelState.AddModelError("", "An unexpected error occurred while updating the room status. Please try again.");
                return View(model);
            }
        }

        // GET: RoomStatusController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid RoomStatus ID: {Id}", id);
                return NotFound();
            }
            var roomStatus = await _roomStatuseService.GetRoomStatusByIdAsync(id);
            if (roomStatus == null)
            {
                _logger.LogInformation("RoomStatus with ID {Id} not found.", id);
                return NotFound();
            }
            var roomStatusViewModel = new RoomStatusViewModel
            {
                RoomStatusID = roomStatus.RoomStatusID,
                StatusName = roomStatus.StatusName,
                Description = roomStatus.Description
            };
            return View(roomStatusViewModel);
        }

        // POST: RoomStatusController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (id <= 0)
            {
                _logger.LogWarning("Invalid RoomStatus ID: {Id}", id);
                return NotFound();
            }

            var roomStatus = await _roomStatuseService.GetRoomStatusByIdAsync(id);
            if (roomStatus == null)
            {
                _logger.LogInformation("RoomStatus with ID {Id} not found.", id);
                return NotFound();
            }

            var roomStatusViewModel = new RoomStatusViewModel
            {
                RoomStatusID = roomStatus.RoomStatusID,
                StatusName = roomStatus.StatusName,
                Description = roomStatus.Description
            };

            try
            {
                bool isDeleted = await _roomStatuseService.DeleteRoomStatusAsync(id);
                if (isDeleted)
                {
                    _logger.LogInformation("RoomStatus with ID {Id} deleted successfully.", id);
                    TempData["SuccessMessage"] = "Room status deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Failed to delete RoomStatus with ID: {Id}", id);
                    // This is the important part: add an error message to the ModelState
                    ModelState.AddModelError("", "Failed to delete RoomStatus. Please try again.");
                    return View(roomStatusViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting RoomStatus with ID: {Id}", id);
                // This is the important part: add an error message to the ModelState
                ModelState.AddModelError("", "An unexpected error occurred while deleting the room status. Please try again.");
                return View(roomStatusViewModel);
            }
        }
    }
}