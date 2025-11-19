using Application.Interfaces.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;
using System.Security.Claims;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "8,9")]
    public class LoanController : BaseController
    {
        private readonly ILoanService _loanService;
        private readonly ILoanStatusService _loanStatusService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public LoanController(
            ILoanService loanService,
            ILoanStatusService loanStatusService,
            IBookService bookService,
            IUserService userService,
            IEmailService emailService,
            IMapper mapper)
        {
            _loanService = loanService;
            _loanStatusService = loanStatusService;
            _bookService = bookService;
            _userService = userService;
            _emailService = emailService;
            _mapper = mapper;
        }
        const int DefaultLoanDays = 7;  
        const int MaxLoanDays = 14;  

        const int DefaultPickupDays = 1;   
        const int MaxPickupDays = 2;

        // multa diaria por día de atraso (cámbiala al valor que quieras)
        private const decimal DailyFine = 10m;

        private decimal CalculateFine(LoanDto loan)
        {
            // Solo prestamos recogidos o devueltos generan multa
            if (loan.Status != 2 && loan.Status != 3)
                return 0m;

            var dueDate = loan.DueDate.Date;
            var today = DateTime.Now.Date;

            var endDate = (loan.ReturnDate ?? today).Date;

            if (endDate <= dueDate)
                return 0m;

            var daysLate = (endDate - dueDate).Days;
            return daysLate * DailyFine;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return null;

            if (int.TryParse(claim.Value, out var id))
                return id;

            return null;
        }

        private bool IsLibrarianOrAdmin()
        {
            return User.IsInRole("8") || User.IsInRole("7"); // 8 = Bibliotecario, 7 = Admin
        }
        private void SetLayoutUserData()
        {
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;
        }
        /*
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                SetLayoutUserData();
                IEnumerable<LoanDto> loanDtos;

                var currentUserId = GetCurrentUserId();

                if (IsLibrarianOrAdmin() || currentUserId == null)
                {
                    // Bibliotecario/Admin: ven todo
                    loanDtos = await _loanService.GetAll();
                }
                else
                {
                    // Usuario normal: solo sus préstamos
                    loanDtos = await _loanService.GetLoansByUserAsync(currentUserId.Value);
                }

                var loanVm = _mapper.Map<List<LoanViewModel>>(loanDtos);
                decimal totalDebt = 0m;     
                // mapeamos y calculamos la multa de cada préstamo
                var loanVm = dtoList
                    .Select(dto =>
                    {
                        var vm = _mapper.Map<LoanViewModel>(dto);

                        var fine = CalculateFine(dto);
                        vm.CalculatedFine = fine;
                        totalDebt += fine;

                        return vm;
                    })
                    .ToList();

                // deuda total para mostrar arriba en la vista
                ViewBag.TotalDebt = totalDebt;

                var totalCount = loanVm.Count;
                var items = loanVm
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResultViewModel<LoanViewModel>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return View(pagedResult);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error al cargar los préstamos: " + ex.Message;
                return View(new PagedResultViewModel<LoanViewModel>());
            }
        }

        public async Task<IActionResult> Create(int? bookId)
        {
            SetLayoutUserData();
            await LoadDropdowns();

            var model = new LoanViewModel()
            {
                PickupDeadline = DateTime.Now.AddDays(DefaultPickupDays),
                DueDate = DateTime.Now.AddDays(DefaultLoanDays),
                Status = 1 // Pendiente por defecto
            };

            var currentUserId = GetCurrentUserId();

            // 👤 Si es usuario normal (rol 9) → forzamos su propio IdUser
            if (User.IsInRole("9") && currentUserId != null)
            {
                model.IdUser = currentUserId.Value;

                ViewBag.CurrentUserId = currentUserId.Value;
                ViewBag.CurrentUserName = CurrentUserName;
            }

            // 📚 Si viene bookId desde "Tomar prestado"
            if (bookId.HasValue)
            {
                model.IdBook = bookId.Value;
                ViewBag.PreSelectedBookId = bookId.Value;
            }

            return View(model);
        }
        */
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                SetLayoutUserData();
                IEnumerable<LoanDto> loanDtos;

                var currentUserId = GetCurrentUserId();

                if (IsLibrarianOrAdmin() || currentUserId == null)
                {
                    // Bibliotecario/Admin: ven todo
                    loanDtos = await _loanService.GetAll();
                }
                else
                {
                    // Usuario normal: solo sus préstamos
                    loanDtos = await _loanService.GetLoansByUserAsync(currentUserId.Value);
                }

                // Materializamos
                var dtoList = loanDtos.ToList();

                decimal totalDebt = 0m;

                // Mapeamos y calculamos multas
                var loanVm = dtoList
                    .Select(dto =>
                    {
                        var vm = _mapper.Map<LoanViewModel>(dto);

                        var fine = CalculateFine(dto);
                        vm.CalculatedFine = fine;
                        totalDebt += fine;

                        return vm;
                    })
                    .ToList();

                ViewBag.TotalDebt = totalDebt;

                var totalCount = loanVm.Count;
                var items = loanVm
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResultViewModel<LoanViewModel>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return View(pagedResult);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error al cargar los préstamos: " + ex.Message;
                return View(new PagedResultViewModel<LoanViewModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanViewModel model)
        {
            // 🔹 Detectar rol e Id actual desde el inicio
            var isLibrarian = User.IsInRole("8");
            var isUser = User.IsInRole("9");
            var currentUserId = GetCurrentUserId();

            // 1️⃣ Amarrar IdUser según el rol ANTES de revisar ModelState
            if (isUser)
            {
                // Quitamos el posible error de binding ("The value '' is invalid")
                ModelState.Remove(nameof(model.IdUser));

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "No se pudo identificar al usuario actual.");
                }
                else
                {
                    // Ignoramos lo que venga del form y usamos el usuario logueado
                    model.IdUser = currentUserId.Value;
                }
            }
            else if (isLibrarian)
            {
                if (model.IdUser <= 0)
                {
                    ModelState.AddModelError("IdUser", "Debes seleccionar el usuario al que se le presta el libro.");
                }
            }

            // 2️⃣ Primera validación general de modelo (ya con IdUser corregido)
            if (!ModelState.IsValid)
            {
                SetLayoutUserData();
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                // 3️⃣ Verificar libro disponible
                var book = await _bookService.GetById(model.IdBook);
                if (book == null || book.AvailableCopies <= 0)
                {
                    ModelState.AddModelError("IdBook", "El libro no está disponible para préstamo.");
                    SetLayoutUserData();
                    await LoadDropdowns();
                    return View(model);
                }

                // 4️⃣ Verificar préstamos del usuario
                var userLoans = await _loanService.GetLoansByUserAsync(model.IdUser);

                // 4.1 ✅ No permitir préstamo activo del MISMO libro
                bool hasActiveLoanForSameBook = userLoans.Any(l =>
                    l.IdBook == model.IdBook &&
                    (l.Status == 1 || l.Status == 2) // 1 = Pendiente, 2 = Recogido/Activo
                );

                if (hasActiveLoanForSameBook)
                {
                    ModelState.AddModelError(string.Empty,
                        "Ya tienes un préstamo activo de este libro. Debes devolverlo antes de solicitarlo de nuevo.");
                    SetLayoutUserData();
                    await LoadDropdowns();
                    return View(model);
                }

                // 4.2 🚫 Regla nueva: si tiene préstamos vencidos, NO puede crear otro (solo usuario normal)
                if (isUser)
                {
                    var today = DateTime.Now.Date;

                    bool hasOverdueLoan = userLoans.Any(l =>
                        // préstamo recogido/activo
                        l.Status == 2 &&
                        // y ya pasó la fecha de vencimiento
                        l.DueDate.Date < today
                    );

                    if (hasOverdueLoan)
                    {
                        ModelState.AddModelError(string.Empty,
                            "No puedes solicitar un nuevo préstamo porque tienes al menos un préstamo vencido.");
                        SetLayoutUserData();
                        await LoadDropdowns();
                        return View(model);
                    }
                }

                // 5️⃣ Manejo de fechas (usando fecha local)
                var baseDate = (model.DateLoan ?? DateTime.Now).Date;
                model.DateLoan = baseDate;

                // ⏰ PickupDeadline (límite para recoger)
                if (model.PickupDeadline == default)
                {
                    model.PickupDeadline = baseDate.AddDays(DefaultPickupDays);
                }
                else
                {
                    var pickup = model.PickupDeadline.Date;
                    if (pickup < baseDate || pickup > baseDate.AddDays(MaxPickupDays))
                    {
                        ModelState.AddModelError(
                            "PickupDeadline",
                            $"La fecha límite de retiro debe estar entre hoy y {MaxPickupDays} días desde la fecha de préstamo."
                        );
                    }
                }

                // 📅 DueDate (vencimiento)
                if (model.DueDate == default)
                {
                    model.DueDate = baseDate.AddDays(DefaultLoanDays);
                }
                else
                {
                    var due = model.DueDate.Date;

                    if (due <= baseDate)
                    {
                        ModelState.AddModelError(
                            "DueDate",
                            "La fecha de vencimiento debe ser posterior a la fecha de préstamo."
                        );
                    }

                    if (due > baseDate.AddDays(MaxLoanDays))
                    {
                        ModelState.AddModelError(
                            "DueDate",
                            $"La fecha de vencimiento no puede ser mayor a {MaxLoanDays} días desde la fecha de préstamo."
                        );
                    }
                }

                if (!ModelState.IsValid)
                {
                    SetLayoutUserData();
                    await LoadDropdowns();
                    return View(model);
                }

                // 6️⃣ Estado según quién crea
                if (isLibrarian)
                {
                    model.Status = 2; // Recogido / Activo
                    model.IdLibrarian = GetCurrentLibrarianId();
                }
                else if (isUser)
                {
                    model.Status = 1;   // Pendiente
                    model.IdLibrarian = null;
                }

                // 7️⃣ Guardar
                var loanDto = _mapper.Map<LoanDto>(model);
                var result = await _loanService.AddAsync(loanDto);

                if (result != null)
                {
                    await SendLoanConfirmationEmail(result);
                    TempData["success"] = "Préstamo creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al crear el préstamo - no se pudo guardar.");
            }
            catch (Exception ex)
            {
                SetLayoutUserData();
                ModelState.AddModelError("", "Error al crear el préstamo: " + ex.Message);
            }

            await LoadDropdowns();
            return View(model);
        }



        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanViewModel model)
        {
            // 0. Validaciones básicas de modelo
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            var isLibrarian = User.IsInRole("8"); // Bibliotecario
            var isUser = User.IsInRole("9"); // Usuario normal
            var currentUserId = GetCurrentUserId();

            // 1. Amarrar correctamente el IdUser según el rol
            if (isUser)
            {
                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "No se pudo identificar al usuario actual.");
                }
                else
                {
                    // Ignoramos cualquier cosa que venga del form y forzamos su propio id
                    model.IdUser = currentUserId.Value;
                }
            }
            else if (isLibrarian)
            {
                // Bibliotecario debe elegir un usuario
                if (model.IdUser <= 0)
                {
                    ModelState.AddModelError("IdUser", "Debes seleccionar el usuario al que se le presta el libro.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                // 2. Verificar si el libro está disponible
                var book = await _bookService.GetById(model.IdBook);
                if (book == null || book.AvailableCopies <= 0)
                {
                    ModelState.AddModelError("IdBook", "El libro no está disponible para préstamo.");
                    await LoadDropdowns();
                    return View(model);
                }

                // 3. Verificar que el mismo usuario no tenga ya un préstamo activo del mismo libro
                var userLoans = await _loanService.GetLoansByUserAsync(model.IdUser);

                bool hasActiveLoanForSameBook = userLoans.Any(l =>
                    l.IdBook == model.IdBook &&
                    (l.Status == 1 || l.Status == 2) // 1 = Pendiente, 2 = Recogido/Activo
                );

                if (hasActiveLoanForSameBook)
                {
                    ModelState.AddModelError(string.Empty,
                        "Ya tienes un préstamo activo de este libro. Debes devolverlo antes de solicitarlo de nuevo.");
                    await LoadDropdowns();
                    return View(model);
                }

                // 4. Fecha base del préstamo (si no viene, hoy)
                var baseDate = (model.DateLoan ?? DateTime.UtcNow).Date;
                model.DateLoan = baseDate;

                // 5. Configurar / validar fecha límite de retiro
                if (model.PickupDeadline == default)
                {
                    // Si no eligió nada → por defecto X días
                    model.PickupDeadline = baseDate.AddDays(DefaultPickupDays);
                }
                else
                {
                    var pickup = model.PickupDeadline.Date;

                    if (pickup < baseDate || pickup > baseDate.AddDays(MaxPickupDays))
                    {
                        ModelState.AddModelError(
                            "PickupDeadline",
                            $"La fecha límite de retiro debe estar entre hoy y {MaxPickupDays} días desde la fecha de préstamo."
                        );
                    }
                }

                // 6. Configurar / validar fecha de vencimiento
                if (model.DueDate == default)
                {
                    // Si no eligió nada → por defecto X días
                    model.DueDate = baseDate.AddDays(DefaultLoanDays);
                }
                else
                {
                    var due = model.DueDate.Date;

                    if (due <= baseDate)
                    {
                        ModelState.AddModelError(
                            "DueDate",
                            "La fecha de vencimiento debe ser posterior a la fecha de préstamo."
                        );
                    }

                    if (due > baseDate.AddDays(MaxLoanDays))
                    {
                        ModelState.AddModelError(
                            "DueDate",
                            $"La fecha de vencimiento no puede ser mayor a {MaxLoanDays} días desde la fecha de préstamo."
                        );
                    }
                }

                // Si alguna validación de fechas falló
                if (!ModelState.IsValid)
                {
                    await LoadDropdowns();
                    return View(model);
                }

                // 7. Estado según quién crea
                if (isLibrarian)
                {
                    // Bibliotecario: el libro ya sale activo
                    model.Status = 2; // Recogido / Activo
                    model.IdLibrarian = GetCurrentLibrarianId();
                }
                else if (isUser)
                {
                    // Usuario público: pendiente de recoger
                    model.Status = 1; // Pendiente
                    model.IdLibrarian = null;
                }

                // 8. Guardar
                var loanDto = _mapper.Map<LoanDto>(model);
                var result = await _loanService.AddAsync(loanDto);

                if (result != null)
                {
                    await SendLoanConfirmationEmail(result);
                    TempData["success"] = "Préstamo creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al crear el préstamo - no se pudo guardar.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el préstamo: " + ex.Message);
            }

            await LoadDropdowns();
            return View(model);
        }
    */
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _loanService.GetById(id);
            if (dto == null)
            {
                TempData["error"] = "Préstamo no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var isUser = User.IsInRole("9");
            var currentUserId = GetCurrentUserId();

            if (isUser)
            {
                // No puede editar préstamos de otros
                if (currentUserId == null || dto.IdUser != currentUserId.Value)
                {
                    TempData["error"] = "No puedes editar préstamos de otros usuarios.";
                    return RedirectToAction(nameof(Index));
                }

                // No puede editar préstamos que ya están activos o devueltos
                if (dto.Status != 1) // 1 = Pendiente
                {
                    TempData["error"] = "No puedes editar un préstamo que ya fue recogido o devuelto.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await LoadDropdowns();
            var vm = _mapper.Map<LoanViewModel>(dto);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoanViewModel model)
        {
            var isLibrarianOrAdmin = IsLibrarianOrAdmin();
            var isUser = User.IsInRole("9");
            var currentUserId = GetCurrentUserId();

            // Cargar el préstamo original
            var existingLoan = await _loanService.GetById(id);
            if (existingLoan == null)
            {
                TempData["error"] = "Préstamo no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // nos aseguramos que el Id coincide
            model.Id = id;

            if (isUser)
            {
                // 1) No puede editar préstamos de otros
                if (currentUserId == null || existingLoan.IdUser != currentUserId.Value)
                {
                    TempData["error"] = "No puedes editar préstamos de otros usuarios.";
                    return RedirectToAction(nameof(Index));
                }

                // 2) Solo puede editar si está PENDIENTE
                if (existingLoan.Status != 1) // 1 = Pendiente
                {
                    TempData["error"] = "No puedes editar un préstamo que ya fue recogido o devuelto.";
                    return RedirectToAction(nameof(Index));
                }

                // 3) Bloqueamos campos que NO debe tocar:
                model.IdUser = existingLoan.IdUser;
                model.IdBook = existingLoan.IdBook;
                model.Status = existingLoan.Status;      // sigue en Pendiente
                model.DateLoan = existingLoan.DateLoan;
                model.ReturnDate = existingLoan.ReturnDate;
                model.IdLibrarian = existingLoan.IdLibrarian;

                // OJO: aquí **NO** pisamos PickupDeadline ni DueDate ni Notes,
                // para que se actualicen si el usuario los cambió en el form.
            }

            // (para Bibliotecario/Admin no tocamos nada: pueden cambiar fechas, estado, etc.)

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                var dto = _mapper.Map<LoanDto>(model);
                var result = await _loanService.UpdateAsync(dto, model.Id);

                if (result != null)
                {
                    // Solo biblio/admin puede marcar como Devuelto (status 3)
                    if (isLibrarianOrAdmin &&
                        model.Status == 3 &&
                        existingLoan.Status != 3)
                    {
                        await SendBookReturnedNotification(result);
                    }

                    TempData["success"] = "Préstamo actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error al actualizar el préstamo.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el préstamo: " + ex.Message);
            }

            await LoadDropdowns();
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var loan = await _loanService.GetById(id);
                if (loan == null)
                {
                    TempData["error"] = "Préstamo no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar si el préstamo está activo
                if (loan.ReturnDate == null) // Préstamo activo
                {
                    TempData["error"] = "No se puede eliminar un préstamo activo.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _loanService.DeleteAsync(id);
                if (result)
                {
                    TempData["success"] = "Préstamo eliminado correctamente.";
                }
                else
                {
                    TempData["error"] = "Error al eliminar el préstamo.";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error al eliminar el préstamo: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int loanId)
        {
            // Solo bibliotecario/admin
            if (!IsLibrarianOrAdmin())
            {
                TempData["error"] = "Solo el bibliotecario puede marcar un libro como devuelto.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var librarianId = GetCurrentLibrarianId();

                var success = await _loanService.ReturnBookAsync(loanId, librarianId);
                if (success)
                {
                    TempData["success"] = "Libro devuelto correctamente.";

                    var loan = await _loanService.GetById(loanId);
                    if (loan != null)
                    {
                        await SendBookReturnedNotification(loan);
                    }
                }
                else
                {
                    TempData["error"] = "Error al devolver el libro.";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> ActiveLoans(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                SetLayoutUserData();
                var activeLoans = await _loanService.GetActiveLoansAsync();

                var currentUserId = GetCurrentUserId();
                if (!IsLibrarianOrAdmin() && currentUserId != null)
                {
                    // Usuario normal: filtrar solo los suyos
                    activeLoans = activeLoans
                        .Where(l => l.IdUser == currentUserId.Value);
                }

                var loanVm = _mapper.Map<List<LoanViewModel>>(activeLoans);

                var totalCount = loanVm.Count;
                var items = loanVm
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResultViewModel<LoanViewModel>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                ViewData["ViewType"] = "active";

                return View("Index", pagedResult);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error al cargar préstamos activos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPickedUp(int loanId)
        {
            if (!IsLibrarianOrAdmin())
            {
                TempData["error"] = "Solo el bibliotecario puede marcar un préstamo como recogido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var librarianId = GetCurrentLibrarianId();

                var success = await _loanService.MarkAsPickedUpAsync(loanId, librarianId);

                if (success)
                {
                    TempData["success"] = "Préstamo marcado como RECIBIDO.";
                }
                else
                {
                    TempData["error"] = "No se pudo marcar como recogido. Verifica que el préstamo esté en estado Pendiente.";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error al marcar como recogido: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task LoadDropdowns()
        {
            try
            {
                var books = await _bookService.GetAll();
                var users = await _userService.GetAll();
                var statuses = await _loanStatusService.GetAll();

                ViewBag.AvailableBooks = books.Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title
                }).ToList();

                ViewBag.AvailableUsers = users.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Name = u.Name
                }).ToList();

                ViewBag.AvailableStatuses = statuses.Select(s => new LoanStatusViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log error but don't break the flow
                Console.WriteLine($"Error loading dropdowns: {ex.Message}");
            }
        }

        private int GetCurrentLibrarianId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("No se pudo obtener el ID del usuario actual.");

            return int.Parse(userId);
        }

        private async Task SendBookReturnedNotification(LoanDto loanDto)
        {
            try
            {
                // 1. Obtener el usuario verdadero del préstamo
                var user = await _userService.GetById(loanDto.IdUser);

                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    Console.WriteLine("⚠ No se puede enviar notificación: usuario sin email.");
                    return;
                }

                var emailBody = $@"
            <h3>Libro Devuelto</h3>
            <p>Hola {user.Name}, el libro ha sido devuelto exitosamente.</p>
            <p>¡Gracias por usar nuestro servicio de biblioteca!</p>";

                // 2. Enviar email al usuario correcto
                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = user.Email, // ✔ correo real del usuario
                    Subject = "Libro Devuelto - Biblioteca",
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending return notification: {ex.Message}");
            }
        }

        private async Task SendLoanConfirmationEmail(LoanDto loanDto)
        {
            try
            {
                var user = await _userService.GetById(loanDto.IdUser);

                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    Console.WriteLine("⚠ No se puede enviar notificación: usuario sin email.");
                    return;
                }

                string subject;
                string emailBody;

                // Status: 1 = Pendiente, 2 = Recogido (Activo)
                if (loanDto.Status == 1)
                {
                    subject = "Confirmación de Solicitud de Préstamo";

                    emailBody = $@"
                <h3>Confirmación de Préstamo</h3>
                <p>Hola {user.Name}, tu solicitud de préstamo ha sido registrada exitosamente.</p>
                <ul>
                    <li><strong>Fecha límite de recogida:</strong> {loanDto.PickupDeadline:dd/MM/yyyy HH:mm}</li>
                    <li><strong>Fecha de devolución:</strong> {loanDto.DueDate:dd/MM/yyyy}</li>
                </ul>
                <p>Si no recoges el libro en el plazo establecido, el préstamo será cancelado automáticamente.</p>";
                }
                else if (loanDto.Status == 2)
                {
                    // 🔵 Préstamo creado por el bibliotecario (ya está activo)
                    subject = "Préstamo Activo - Biblioteca";

                    emailBody = $@"
                <h3>Préstamo Activo</h3>
                <p>Hola {user.Name}, se ha registrado un préstamo a tu nombre y ya se encuentra activo.</p>
                <ul>
                    <li><strong>Fecha de préstamo:</strong> {loanDto.DateLoan:dd/MM/yyyy}</li>
                    <li><strong>Fecha de devolución:</strong> {loanDto.DueDate:dd/MM/yyyy}</li>
                </ul>
                <p>Recuerda devolver el libro a tiempo para evitar multas.</p>";
                }
                else
                {
                    // Cualquier otro estado (por si acaso)
                    subject = "Actualización de Préstamo";

                    emailBody = $@"
                <h3>Actualización de Préstamo</h3>
                <p>Hola {user.Name}, se ha actualizado el estado de tu préstamo.</p>
                <ul>
                    <li><strong>Fecha de préstamo:</strong> {loanDto.DateLoan:dd/MM/yyyy}</li>
                    <li><strong>Fecha de devolución:</strong> {loanDto.DueDate:dd/MM/yyyy}</li>
                </ul>";
                }

                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = user.Email,
                    Subject = subject,
                    HtmlBody = emailBody
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

   

    }
}
