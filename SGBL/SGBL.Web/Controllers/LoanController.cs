using Application.Interfaces.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;
using System.ComponentModel.Design;

namespace SGBL.Web.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly ILoanStatusService _loanStatusService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly ILogger<LoanController> _logger;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private const int LoanStatusPending = 1;
        private const int LoanStatusPickedUp = 2;
        private const int LoanStatusReturned = 3;

        public LoanController(ILoanService loanService, ILoanStatusService loanStatusService, IBookService bookService, IEmailService emailService, IUserService userService, IMapper mapper, ILogger<LoanController> logger)
        {
            _loanService = loanService;
            _loanStatusService = loanStatusService;
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
                ViewBag.LoansDueSoon = dueSoon;
                var loanDtos = await _loanService.GetAll();
                var statusDtos = await _loanStatusService.GetAll();
                var loanVm = _mapper.Map<List<LoanViewModel>>(loanDtos);
                return View(loanVm);
            }
            catch (Exception ex)
            {
                return View(new List<LoanViewModel>());
            }
        }

        public IActionResult Create()
        {
            return View(new LoanViewModel()
            {
                Id = 0,
                IdBook = 0,
                IdUser = 0,
                IdLibrarian = 0,
                DateLoan = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                ReturnDate = null,
                PickupDate = null,
                PickupDeadline = DateTime.Now.AddDays(1), // 24 horas para recoger
                Status = LoanStatusPending,
                FineAmount = 0,
                Notes = string.Empty,
                CreatedAt = DateTime.Now,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(LoanViewModel model)
        {
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
                    model.PickupDeadline = DateTime.Now.AddDays(1); // 24 horas para recoger
                    model.Status = LoanStatusPending;
                    model.DateLoan = DateTime.Now;
                    model.DueDate = DateTime.Now.AddDays(7); // 7 días para devolver

                    var loanDto = _mapper.Map<LoanDto>(model);
                    var result = await _loanService.AddAsync(loanDto);
                    await SendLoanConfirmationEmail(loanDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el préstamo: " + ex.Message);
                }
            }
            return View(model);
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
                    dto.UpdatedAt = DateTime.Now;

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

                // Verificar si el préstamo está activo (no se puede eliminar)
                if (loan.Status == LoanStatusPending || loan.Status == LoanStatusPickedUp)
                {
                    TempData["Error"] = "No se puede eliminar un préstamo activo.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _loanService.DeleteAsync(id);
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

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = email,
                    Subject = "Confirmación de Préstamo - Biblioteca",
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                // Log the error but don't break the flow
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

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = email, // Aquí deberías obtener el email del usuario
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