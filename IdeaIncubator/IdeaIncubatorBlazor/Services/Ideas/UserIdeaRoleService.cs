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
using Microsoft.EntityFrameworkCore;

namespace IdeaIncubatorBlazor.Services.Ideas;

public class UserIdeaRoleService : IUserIdeaRoleService
{
    private readonly ILoggingIdeaIncubator _loggingIdeaIncubator;

    private readonly IdeaIncubatorDbContext _dbContext;

    public UserIdeaRoleService(IdeaIncubatorDbContext dbContext, ILoggingIdeaIncubator loggingIdeaIncubator)
    {
        _dbContext = dbContext;
        _loggingIdeaIncubator = loggingIdeaIncubator;
    }
    public Task<UserIdeaRole> CreateUserIdeaRole(int userId, int ideaId, int roleId)
    {
        UserIdeaRole newUserIdeaRole = new UserIdeaRole { UserId = userId, IdeaId = ideaId, RoleId = roleId };
        _dbContext.UserIdeaRoles.Add(newUserIdeaRole);
        _dbContext.SaveChanges();
        return Task.FromResult(newUserIdeaRole);
    }

    public void DeleteUserIdeaRole(UserIdeaRole userIdeaRole)
    {
        throw new NotImplementedException();
    }

    public UserIdeaRole GetUserIdeaRole(int userId, int ideaId, int roleId)
    {
        return _dbContext.UserIdeaRoles.FirstOrDefault(u => u.UserId == userId && u.IdeaId == ideaId && u.RoleId == roleId);        
    }

    public List<UserIdeaRole> GetUserIdeaRoles()
    {
        throw new NotImplementedException();
    }

    public int UpdateUserIdeaRole(UserIdeaRole userIdeaRole)
    {
        try
        {
            _dbContext.UserIdeaRoles.Where(r => r.UserId == userIdeaRole.UserId && r.IdeaId == userIdeaRole.IdeaId).ExecuteDelete();
            _dbContext.UserIdeaRoles.Add(userIdeaRole);
            _dbContext.SaveChanges();
            UserIdeaRole u = _dbContext.UserIdeaRoles.FirstOrDefault(r => r.UserId == userIdeaRole.UserId && r.IdeaId == userIdeaRole.IdeaId);
            _dbContext.Entry(u).Reload();
        }
        catch (Exception e)
        {
            return 0;
        }

        return 1;
    }
}
