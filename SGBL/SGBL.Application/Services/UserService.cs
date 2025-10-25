
using AutoMapper;
using SGBL.Application.Dtos.User;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class UserService : GenericService<User, UserDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        public UserService(IUserRepository userRepository, IMapper mapper, IServiceLogs serviceLogs) : base(userRepository, mapper, serviceLogs)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }
        public override async Task<UserDto> AddAsync(UserDto dto)
        {
         
            try
            {
                _serviceLogs.CreateLogInfo($"Creación del usuario  {dto.Name}, email:{dto.Email}  iniciada.");
                var user = _mapper.Map<User>(dto);

                // Campos obligatorios
                user.Password = dto.Password ?? "1234";  // temporal si falta
                user.TokenActivation = Guid.NewGuid().ToString("N");
                user.TokenRecuperation = Guid.NewGuid().ToString("N");
                user.CreatedAt = DateTime.UtcNow;

                await _userRepository.AddAsync(user);
                return _mapper.Map<UserDto>(user);

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error en en la capa Aplication con la creacion del usuario {dto.Id}, email: {dto.Email}, id: {dto.Id}, " + ex);
                throw;
            }
        }
        public override async Task<UserDto> UpdateAsync(UserDto dto, int id)

        {

          
            try
            {
                _serviceLogs.CreateLogInfo($"Actualización del usuario {dto.Name}, email:{dto.Email} iniciada.");
                var existing = await _userRepository.GetById(id);
                if (existing == null) throw new InvalidOperationException("Usuario no encontrado");

                // Mapear sin pisar con nulls (configura AutoMapper para ignorar nulls)
                _mapper.Map(dto, existing);

                // Password: solo si viene
                if (!string.IsNullOrWhiteSpace(dto.Password))
                    existing.Password = dto.Password; // aquí aplica hash si corresponde

                existing.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(id, existing); // 👈 orden correcto
                return _mapper.Map<UserDto>(existing);

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error en en la capa Aplication con la actualización del usuario {dto.Id}, email: {dto.Email}, id: {dto.Id}, " + ex);
                throw;
            }
        }
    }
}