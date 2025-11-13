using System.ComponentModel.Design;
using Application.Interfaces.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly ILoanStatusService _loanStatusService;
        private readonly IBookService _bookService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public LoanController(ILoanService loanService, ILoanStatusService loanStatusService, IBookService bookService, IEmailService emailService, IMapper mapper)
        {
            _loanService = loanService;
            _loanStatusService = loanStatusService;
            _bookService = bookService;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
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
                Status = 1, // 1 = Pending (asumiendo que 1 es el estado "Pendiente")
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
                    model.Status = 1; // Estado: Pendiente
                    model.DateLoan = DateTime.Now;
                    model.DueDate = DateTime.Now.AddDays(7); // 7 días para devolver

                    var loanDto = _mapper.Map<LoanDto>(model);
                    var result = await _loanService.AddAsync(loanDto);

                    if (result.Status == 1)
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

                    if (result.Status == 1)
                    {
                        // Si el estado cambió a "Recogido" (asumiendo que 2 es "Recogido")
                        if (vm.Status == 2 && existingLoan.Status != 2)
                        {
                            await SendBookPickedUpNotification(dto);
                        }

                        // Si el estado cambió a "Devuelto" (asumiendo que 3 es "Devuelto")
                        if (vm.Status == 3 && existingLoan.Status != 3)
                        {
                            await _bookService.IncreaseAvailableCopies(vm.IdBook);
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
                if (loan.Status == 1 || loan.Status == 2) // 1 = Pendiente, 2 = Recogido
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

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = "user@email.com", // Aquí deberías obtener el email del usuario
                    Subject = "Confirmación de Préstamo - Biblioteca",
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                // Log the error but don't break the flow
                Console.WriteLine($"Error sending email: {ex.Message}");
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

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = "user@email.com", // Aquí deberías obtener el email del usuario
                    Subject = "Libro Recogido - Biblioteca",
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending pickup notification: {ex.Message}");
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

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = "user@email.com", // Aquí deberías obtener el email del usuario
                    Subject = "Libro Devuelto - Biblioteca",
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending return notification: {ex.Message}");
            }
        }

        // Método para notificar 2 días antes de la devolución (debería ejecutarse como background job)
        
    }
}