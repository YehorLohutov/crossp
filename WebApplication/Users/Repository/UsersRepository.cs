using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Users.Models;
using System;

namespace Users.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UsersDBContext usersDBContext;
        
        public UsersRepository(UsersDBContext usersDBContext)
        {
            this.usersDBContext = usersDBContext;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>
        /// A list of users.
        /// </returns>
        public async Task<IEnumerable<User>> GetUsers() => await usersDBContext.Users.ToArrayAsync();

        /// <summary>
        /// Get an user by <paramref name="userId"/>
        /// </summary>
        /// <returns>
        /// An user object.
        /// </returns>
        /// <param name="userId">User id.</param>
        /// <exception cref="UserNotExistException">Thrown when user with <paramref name="userId"/> not exist.</exception>
        public async Task<User> GetUserBy(int userId)
        {
            User user = await usersDBContext.Users.FirstOrDefaultAsync(t => t.Id == userId);

            if (user == default)
                throw new NullReferenceException($"User with {nameof(userId)} = {userId} not exist.");

            return user;
        }

        /// <summary>
        /// Get an user by <paramref name="userExternalId"/>
        /// </summary>
        /// <returns>
        /// An user object.
        /// </returns>
        /// <param name="userExternalId">User external id.</param>
        /// <exception cref="UserNotExistException">Thrown when user with <paramref name="userExternalId"/> not exist.</exception>
        public async Task<User> GetUserBy(string userExternalId)
        {
            User user = await usersDBContext.Users.FirstOrDefaultAsync(t => t.ExternalId.Equals(userExternalId));

            if (user == default)
                throw new NullReferenceException($"User with {nameof(userExternalId)} = {userExternalId} not exist.");

            return user;
        }

        /// <summary>
        /// Get an user by <paramref name="userId"/>
        /// </summary>
        /// <returns>
        /// An user object.
        /// </returns>
        /// <param name="login">User login.</param>
        /// <param name="password">User password.</param>
        /// <exception cref="InvalidLoginException">Thrown when login string is null or empty.</exception>
        /// <exception cref="InvalidPasswordException">Thrown when password string is null or empty.</exception>
        /// <exception cref="UserNotExistException">Thrown user not found.</exception>
        public async Task<User> GetUserBy(string login, string password)
        {
            if (login == default || login.Length == 0)
                throw new ArgumentException($"Parameter {nameof(login)} passed into {nameof(GetUserBy)} is null or empty.");

            if (password == default || password.Length == 0)
                throw new ArgumentException($"Parameter {nameof(password)} passed into {nameof(GetUserBy)} is null or empty.");

            User user = await usersDBContext.Users.FirstOrDefaultAsync(t => t.Login.Equals(login) && t.Password.Equals(password));

            if (user == default)
                throw new NullReferenceException($"User with {nameof(login)} = {login} and {nameof(password)} = {password} not exist.");

            return user;
        }

        public async Task<bool> UserExists(string userExternalId) =>
            await usersDBContext.Users.AnyAsync(t => t.ExternalId.Equals(userExternalId));
        

        /// <summary>
        /// Delete an user by <paramref name="userId"/>
        /// </summary>
        /// <param name="user">User object.</param>
        /// <exception cref="UserNotExistException">Thrown when <paramref name="user"/> is null.</exception>
        public void Delete(User user)
        {
            if (user == default)
                throw new NullReferenceException($"Parameter {nameof(user)} passed into {nameof(Delete)} is null.");
            usersDBContext.Users.Remove(user);
        }

        /// <summary>
        /// Delete an user by <paramref name="userId"/>
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <exception cref="UserNotExistException">Thrown when user with <paramref name="userId"/> not exist.</exception>
        public async Task Delete(int userId)
        {
            User user = await GetUserBy(userId);
            Delete(user);
        }

        /// <summary>
        /// Delete an user by <paramref name="userExternalId"/>
        /// </summary>
        /// <param name="userExternalId">User external id.</param>
        /// <exception cref="UserNotExistException">Thrown when user with <paramref name="userExternalId"/> not exist.</exception>
        public async Task Delete(string userExternalId)
        {
            User user = await GetUserBy(userExternalId);
            Delete(user);
        }

        /// <summary>
        /// Insert new user.
        /// </summary>
        /// <param name="user">User object.</param>
        /// <exception cref="UserNotExistException">Thrown when <paramref name="user"/> is null.</exception>
        public async Task Insert(User user)
        {
            if (user == default)
                throw new NullReferenceException($"Parameter {nameof(user)} passed into {nameof(Insert)} is null.");
            await usersDBContext.AddAsync(user);
        }

        public async Task SaveChanges() => await usersDBContext.SaveChangesAsync();
    }
}
