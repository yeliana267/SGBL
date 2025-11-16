using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;
using System;
using System.Collections.Generic;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "9")]
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly ILogger<LoanController> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanController> _logger;

        public LoanController(ILoanService loanService, ILoanStatusService loanStatusService, IBookService bookService, IEmailService emailService, IUserService userService, IMapper mapper, ILogger<LoanController> logger)
        {
            _loanService = loanService;
            _bookService = bookService;
            _emailService = emailService;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var cancelledCount = await _loanService.CancelLoansNotPickedUpAsync();
                ViewBag.CancelledLoans = cancelledCount;

                var dueSoon = await _loanService.GetLoansDueInDays(2);
                var dueSoonVm = _mapper.Map<List<LoanViewModel>>(dueSoon);
                ViewBag.LoansDueSoon = dueSoonVm;
                ViewBag.LoansDueSoonCount = dueSoonVm.Count;

                var loanDtos = await _loanService.GetAll();
                var loanVm = _mapper.Map<List<LoanViewModel>>(loanDtos);

                ViewData["LoanList"] = loanVm;

                return View(CreateDefaultLoanViewModel());
            }
            catch (Exception ex)
            {
                ViewBag.CancelledLoans = 0;
                ViewBag.LoansDueSoon = new List<LoanViewModel>();
                ViewBag.LoansDueSoonCount = 0;
                ViewData["LoanList"] = new List<LoanViewModel>();
                return View(CreateDefaultLoanViewModel());
            }
        }

        public IActionResult Create()
        {
            return View(new LoanViewModel()
            {
                Id = 0,
                IdBook = 0,
                IdUser = 0,
                IdLibrarian = null,
                DateLoan = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                ReturnDate = null,
                PickupDate = null,
                PickupDeadline = DateTime.UtcNow.AddDays(1), // 24 horas para recoger
                Status = 1, // 1 = Pending (asumiendo que 1 es el estado "Pendiente")
                FineAmount = 0,
                Notes = string.Empty,
                CreatedAt = DateTime.UtcNow,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(LoanViewModel model)
        {
            model.Notes ??= string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el libro está disponible
                    var book = await _bookService.GetById(model.IdBook);
                    if (book == null || book.AvailableCopies <= 0)
                    {
                        ModelState.AddModelError("", "El libro no está disponible para préstamo.");
                        return View(model);
                    }

                    // Configurar fechas y estado
                    model.PickupDeadline = DateTime.UtcNow.AddDays(1); // 24 horas para recoger
                    model.Status = 1; // Estado: Pendiente
                    model.DateLoan = DateTime.UtcNow;
                    model.DueDate = DateTime.UtcNow.AddDays(7); // 7 días para devolver

                    var loanDto = _mapper.Map<LoanDto>(model);
                    var result = await _loanService.AddAsync(loanDto);

                    if (result?.Status == 1)
                    {
                        // Disminuir la cantidad disponible del libro
                        await _bookService.DecreaseAvailableCopies(model.IdBook);

                        // Enviar email de notificación
                        await SendLoanConfirmationEmail(loanDto);

                        return RedirectToAction(nameof(Index));
                    }
                  
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el préstamo: " + ex.Message);
                }
            }
            return View(model);
        }

        private LoanViewModel CreateDefaultLoanViewModel()
        {
            var now = DateTime.UtcNow;

            return new LoanViewModel
            {
                Id = 0,
                IdBook = 0,
                IdUser = 0,
                IdLibrarian = null,
                DateLoan = now,
                DueDate = now.AddDays(7),
                ReturnDate = null,
                PickupDate = null,
                PickupDeadline = now.AddDays(1),
                Status = LoanStatusPending,
                FineAmount = 0m,
                Notes = string.Empty,
                CreatedAt = now,
            };
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _loanService.GetById(id);
            if (dto == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<LoanViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LoanViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingLoan = await _loanService.GetById(vm.Id);
                    if (existingLoan == null)
                    {
                        return NotFound();
                    }

                    var dto = _mapper.Map<LoanDto>(vm);
                    dto.UpdatedAt = DateTime.UtcNow;

                    var result = await _loanService.UpdateAsync(dto, dto.Id);

                        if (result != null)
                        {
                                // Si el estado cambió a "Recogido"
                                if (vm.Status == LoanStatusPickedUp && existingLoan.Status != LoanStatusPickedUp)
                                {
                                    await SendBookPickedUpNotification(dto);
                                }
                                // Si el estado cambió a "Devuelto"
                                if (vm.Status == LoanStatusReturned && existingLoan.Status != LoanStatusReturned)
                                {
                                    await SendBookReturnedNotification(dto);
                                }

                            return RedirectToAction(nameof(Index));
                        }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el préstamo: " + ex.Message);
                }
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var loan = await _loanService.GetById(id);

                if (loan?.Status is 1 or 2)
                {
                    TempData["Error"] = "No se puede eliminar un préstamo activo.";
                    return RedirectToAction(nameof(Index));
                }

                await _loanService.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el préstamo: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Métodos auxiliares para enviar emails
        private async Task SendLoanConfirmationEmail(LoanDto loanDto)
        {
            try
            {
                var emailBody = $@"
                    <h3>Confirmación de Préstamo</h3>
                    <p>Su préstamo ha sido registrado exitosamente.</p>
                    <p><strong>Información importante:</strong></p>
                    <ul>
                        <li>Tiene 24 horas para recoger el libro en la biblioteca</li>
                        <li>Fecha límite de recogida: {loanDto.PickupDeadline:dd/MM/yyyy HH:mm}</li>
                        <li>Fecha de devolución: {loanDto.DueDate:dd/MM/yyyy}</li>
                    </ul>
                    <p>Si no recoge el libro en el plazo establecido, el préstamo será cancelado automáticamente.</p>";
                var email = await GetUserEmailAsync(loanDto.IdUser);
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("No se envió correo de confirmación porque el usuario {UserId} no tiene email registrado.", loanDto.IdUser);
                    return;
                }

                var email = await GetUserEmailAsync(loanDto.IdUser);
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("No se envió correo de confirmación porque el usuario {UserId} no tiene email registrado.", loanDto.IdUser);
                    return;
                }

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = email,
                    Subject = "Confirmación de Préstamo - Biblioteca",
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de confirmación para el préstamo {LoanId}.", loanDto.Id);
            }
        }

        private async Task SendBookPickedUpNotification(LoanDto loanDto)
        {
            try
            {
                var emailBody = $@"
                    <h3>Libro Recogido</h3>
                    <p>El libro ha sido recogido exitosamente.</p>
                    <p><strong>Recordatorio:</strong></p>
                    <ul>
                        <li>Fecha de devolución: {loanDto.DueDate:dd/MM/yyyy}</li>
                        <li>Recuerde devolver el libro a tiempo para evitar multas</li>
                    </ul>";
                var email = await GetUserEmailAsync(loanDto.IdUser);
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("No se envió correo de recogida porque el usuario {UserId} no tiene email registrado.", loanDto.IdUser);
                    return;
                }

                var email = await GetUserEmailAsync(loanDto.IdUser);
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("No se envió correo de recogida porque el usuario {UserId} no tiene email registrado.", loanDto.IdUser);
                    return;
                }

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = email,
                    Subject = "Libro Recogido - Biblioteca",
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de recogida para el préstamo {LoanId}.", loanDto.Id);
            }
        }

        private async Task SendBookReturnedNotification(LoanDto loanDto)
        {
            try
            {
                var emailBody = $@"
                    <h3>Libro Devuelto</h3>
                    <p>El libro ha sido devuelto exitosamente.</p>
                    <p>¡Gracias por usar nuestro servicio de biblioteca!</p>";

                var email = await GetUserEmailAsync(loanDto.IdUser);
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("No se envió correo de devolución porque el usuario {UserId} no tiene email registrado.", loanDto.IdUser);
                    return;
                }

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = email,
                    Subject = "Libro Devuelto - Biblioteca",
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de devolución para el préstamo {LoanId}.", loanDto.Id);
            }
        }

        private async Task<string?> GetUserEmailAsync(int userId)
        {
            try
            {
                var user = await _userService.GetById(userId);
                return user?.Email;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo obtener el correo electrónico del usuario {UserId}.", userId);
                return null;
            }
        }

    }
}