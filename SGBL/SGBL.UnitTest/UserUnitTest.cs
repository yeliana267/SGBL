using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGBL.Application.Dtos.User;
using SGBL.Application.Profiles;
using SGBL.Application.Services;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.UnitTest;

[TestClass]
public class UserUnitTest
{
    [TestMethod]
    public async Task AddAsync_ShouldPopulateGeneratedFields()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mapperConfig.CreateMapper();
        var service = new UserService(repository, mapper);

        var dto = new UserDto
        {
            Id = 0,
            Name = "Ada Lovelace",
            Email = "ada@example.com",
            Password = null!,
            Role = 1,
            Status = 2
        };

        var beforeCall = DateTime.UtcNow;

        // Act
        var result = await service.AddAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        var storedUser = repository.LastAddedUser;
        Assert.IsNotNull(storedUser, "The repository should receive the mapped user instance.");
        Assert.AreEqual("1234", storedUser!.Password, "A default password should be assigned when none is provided.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(storedUser.TokenActivation), "An activation token must be generated.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(storedUser.TokenRecuperation), "A recovery token must be generated.");
        Assert.IsTrue(storedUser.CreatedAt >= beforeCall, "The creation timestamp should be refreshed before persistence.");

        Assert.IsNull(result!.Password, "Sensitive fields must not leak to the returned DTO.");
        Assert.IsNull(result.TokenActivation, "Sensitive fields must not leak to the returned DTO.");
        Assert.IsNull(result.TokenRecuperation, "Sensitive fields must not leak to the returned DTO.");
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public User? LastAddedUser { get; private set; }

        public Task<User> AddAsync(User entity)
        {
            LastAddedUser = entity;
            if (entity.Id == 0)
            {
                entity.Id = _users.Count + 1;
            }

            _users.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<List<User>?> AddRangeAsync(List<User> entities)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAllAsync()
        {
            return Task.FromResult(_users.ToList());
        }

        public Task<List<User>> GetAllAsyncWithInclude(List<string> properties)
        {
            throw new NotImplementedException();
        }

        public IQueryable<User> GetAllQuery()
        {
            return _users.AsQueryable();
        }

        public IQueryable<User> GetAllQueryWithInclude(List<string> properties)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetById(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task<User?> GetByIdNoTrackingAsync(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task<User?> UpdateAsync(int id, User entity)
        {
            throw new NotImplementedException();
        }
    }
}