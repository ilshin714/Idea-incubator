/*
 * Copyright (C) 2023 IKTSolution
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using IdeaIncubatorBlazor.Models;
using IdeaIncubatorBlazor.Utils.Loggings;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
namespace IdeaIncubatorBlazor.Services.Users;

public class UserService : IUserService
{
    private readonly ILoggingIdeaIncubator loggingIdeaIncubator;
    private readonly IRoleService roleService;
    private readonly IdeaIncubatorDbContext _dbContext;

    public UserService(IdeaIncubatorDbContext dbContext, ILoggingIdeaIncubator loggingIdeaIncubator, IRoleService roleService)
    {
        _dbContext = dbContext;
        this.loggingIdeaIncubator = loggingIdeaIncubator;
        this.roleService = roleService;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.GUID = Guid.NewGuid();
        user.Password = OneWayHashSHA256(user.GUID.ToString() + user.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUser(User user)
    {
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public List<User> GetAllUsers()
    {
        List<User> users = new List<User>();
        users = _dbContext.Users.ToList();

        return users;

    }

    public User GetUser(int id)
    {
        User user = _dbContext.Users.FirstOrDefault(u => u.UserId == id);
        return user;
    }

    public async Task<Boolean> UpdateUser(User user, Boolean newPassword)
    {
        Boolean result = false;
//        try
//        {
            if (newPassword)
            {
                user.Password = OneWayHashSHA256(user.GUID.ToString() + user.Password);
            }
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            result = true;
 //       } catch (Exception e)
 //       {

 //       }
        return result;
    }

    public User GetUserByEmail(string email)
    {
        User user = _dbContext.Users.FirstOrDefault(u => u.EmailAddress == email);
        return user;
    }

    public int LoginUser(string username, string password)
    {
        try
        {
            User user = _dbContext.Users.FirstOrDefault(u => u.UserName.Equals(username));

            if (user != null && user.UserName.Equals(username, StringComparison.Ordinal))
            {
                if (user.Password.Equals(OneWayHashSHA256(user.GUID + password)))
                {
                    return user.UserId;
                }
            }


        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
            Debug.Print("Error reading database for login");
        }

        return 0;

    }

    public string OneWayHashSHA256 (string inString)
    {
        String passwordSalt = new ConfigurationBuilder().AddEnvironmentVariables("IDEA_").Build().GetValue<String>("AppSalt");
        byte[] bytePassword = Encoding.ASCII.GetBytes(inString + passwordSalt);
        byte[] result;
        SHA256 sha256 = SHA256.Create();
        result = sha256.ComputeHash(bytePassword);
        return Encoding.ASCII.GetString(result);
    }

    public async Task<bool> IsUniqueUserName(string userName)
    {
        string? _dbUserName = _dbContext.Users?.Select(u => u.UserName)?.FirstOrDefault(n => n == userName);
        return _dbUserName.IsNullOrEmpty();
    }
    public async Task<bool> IsUniqueEmail(string email)
    {
        string? _dbEmail = _dbContext.Users?.Select(u => u.EmailAddress)?.FirstOrDefault(e => e == email);
        return _dbEmail.IsNullOrEmpty();
    }

    public string GeneratePassword(PasswordOptions opts = null)
    {
        if (opts == null) opts = new PasswordOptions()
        {
            RequiredLength = 64,
            RequiredUniqueChars = 8,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true
        };

        string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
        "!@$?_-"                        // non-alphanumeric
    };
        CryptoRandom rand = new CryptoRandom();
        List<char> chars = new List<char>();

        if (opts.RequireUppercase)
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[0][rand.Next(0, randomChars[0].Length)]);

        if (opts.RequireLowercase)
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[1][rand.Next(0, randomChars[1].Length)]);

        if (opts.RequireDigit)
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[2][rand.Next(0, randomChars[2].Length)]);

        if (opts.RequireNonAlphanumeric)
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[3][rand.Next(0, randomChars[3].Length)]);

        for (int i = chars.Count; i < opts.RequiredLength
            || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
        {
            string rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count),
                rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }
}
