using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;

namespace SGBL.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, IUserRepository userRepository)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
        }

        public async Task<List<LoanDto>> GetAll()
        {
            var loans = await _loanRepository.GetAllWithDetailsAsync();
            return loans.Select(MapToDto).ToList();
        }

        public async Task<LoanDto?> GetById(int id)
        {
            var loan = await _loanRepository.GetById(id); // si tienes GetById con include, mejor
            return loan != null ? MapToDto(loan) : null;
        }

        public async Task<LoanDto?> UpdateAsync(LoanDto loanDto, int id)
        {
            var existing = await _loanRepository.GetById(id);
            if (existing == null) return null;

            DateTime normalize(DateTime dt) =>
                dt.Kind == DateTimeKind.Utc
                    ? dt
                    : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

            existing.IdBook = loanDto.IdBook;
            existing.IdUser = loanDto.IdUser;
            existing.DueDate = normalize(loanDto.DueDate);
            existing.PickupDeadline = normalize(loanDto.PickupDeadline);
            existing.Status = loanDto.Status;
            existing.Notes = loanDto.Notes;
            existing.UpdateDate = DateTime.UtcNow;

            var updated = await _loanRepository.UpdateAsync(id, existing);
            return updated != null ? MapToDto(updated) : null;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            await _loanRepository.DeleteAsync(id);
            return true;
        }


        public async Task<LoanDto?> AddAsync(LoanDto loanDto)
        {
            try
            {
                Console.WriteLine($"🔍 Iniciando creación de préstamo - Libro: {loanDto.IdBook}, Usuario: {loanDto.IdUser}");

                // 1. Validar que el libro esté disponible
                var book = await _bookRepository.GetById(loanDto.IdBook);
                if (book == null)
                    throw new InvalidOperationException("El libro no existe");

                if (book.AvailableCopies <= 0)
                    throw new InvalidOperationException("El libro no está disponible para préstamo");

                Console.WriteLine($"📚 Libro encontrado: {book.Title}, Copias disponibles: {book.AvailableCopies}");

                // 2. Validar que el usuario exista
                var user = await _userRepository.GetById(loanDto.IdUser);
                if (user == null)
                    throw new InvalidOperationException("El usuario no existe");

                Console.WriteLine($"👤 Usuario encontrado: {user.Name}");

                // 👉 Siempre trabajar en UTC
                var nowUtc = DateTime.UtcNow;

                // Normalizar fechas a UTC (muy importante)
                DateTime normalize(DateTime dt) =>
                    dt.Kind == DateTimeKind.Utc
                        ? dt
                        : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

                var loan = new Loan
                {
                    IdBook = loanDto.IdBook,
                    IdUser = loanDto.IdUser,
                    IdLibrarian = loanDto.IdLibrarian,
                    DateLoan = loanDto.DateLoan.HasValue
                                      ? normalize(loanDto.DateLoan.Value)
                                      : nowUtc,
                    DueDate = normalize(loanDto.DueDate),
                    PickupDeadline = normalize(loanDto.PickupDeadline),
                    PickupDate = loanDto.PickupDate.HasValue ? normalize(loanDto.PickupDate.Value) : null,
                    Status = loanDto.Status ?? 1,
                    FineAmount = 0,
                    Notes = loanDto.Notes,
                    CreationDate = nowUtc,
                    UpdateDate = nowUtc
                };

                Console.WriteLine("💾 Guardando préstamo en la base de datos...");
                var created = await _loanRepository.AddAsync(loan);

                if (created != null)
                {
                    Console.WriteLine($"✅ Préstamo creado exitosamente - ID: {created.Id}");

                    // 4. Actualizar inventario del libro
                    book.AvailableCopies--;
                    await _bookRepository.UpdateAsync(book.Id, book);
                    Console.WriteLine($"📉 Copias disponibles actualizadas: {book.AvailableCopies}");
                }
                else
                {
                    Console.WriteLine("❌ Error: No se pudo crear el préstamo");
                }

                return created != null ? MapToDto(created) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error en AddAsync: {ex.Message}");
                Console.WriteLine($"💥 StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> ReturnBookAsync(int loanId, int librarianId)
        {
            var loan = await _loanRepository.GetById(loanId);
            if (loan == null) return false;

            var nowUtc = DateTime.UtcNow;

            loan.ReturnDate = nowUtc;
            loan.IdLibrarian = librarianId;
            loan.Status = 3; // Devuelto
            loan.UpdateDate = nowUtc;

            var book = await _bookRepository.GetById(loan.IdBook);
            if (book != null)
            {
                book.AvailableCopies++;
                await _bookRepository.UpdateAsync(book.Id, book);
            }

            var updated = await _loanRepository.UpdateAsync(loanId, loan);
            return updated != null;
        }

        public async Task<IEnumerable<LoanDto>> GetLoansByUserAsync(int userId)
        {
            var loans = await _loanRepository.GetLoansByUserAsync(userId);
            return loans.Select(MapToDto);
        }
        public async Task<bool> MarkAsPickedUpAsync(int loanId, int librarianId)
        {
            var loan = await _loanRepository.GetById(loanId);
            if (loan == null)
                return false;

            // Solo puedes marcar como recogido si está Pendiente (1) y no devuelto
            if (loan.Status != 1 || loan.ReturnDate != null)
                return false;

            loan.Status = 2; // Recogido
            loan.PickupDate = DateTime.UtcNow;
            loan.IdLibrarian = librarianId;
            loan.UpdateDate = DateTime.UtcNow;

            var updated = await _loanRepository.UpdateAsync(loanId, loan);
            return updated != null;
        }

        public async Task<IEnumerable<LoanDto>> GetActiveLoansAsync()
        {
            var loans = await _loanRepository.GetActiveLoansAsync();
            return loans.Select(MapToDto);
        }

        private static LoanDto MapToDto(Loan loan) => new()
        {
            Id = loan.Id,
            IdBook = loan.IdBook,
            IdUser = loan.IdUser,
            IdLibrarian = loan.IdLibrarian,
            DateLoan = loan.DateLoan,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            PickupDate = loan.PickupDate,
            PickupDeadline = loan.PickupDeadline,
            Status = loan.Status,
            FineAmount = loan.FineAmount,
            Notes = loan.Notes,
            CreationDate = loan.CreationDate,
            UpdateDate = loan.UpdateDate,


            BookTitle = loan.Book != null ? loan.Book.Title : null,
            UserName = loan.User != null ? loan.User.Name : null,
            LibrarianName = loan.Librarian != null ? loan.Librarian.Name : null,
            StatusName = loan.LoanStatus != null ? loan.LoanStatus.Name : null
        };
    }
}