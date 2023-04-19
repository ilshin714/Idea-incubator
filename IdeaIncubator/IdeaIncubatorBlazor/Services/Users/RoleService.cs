﻿/*
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

namespace IdeaIncubatorBlazor.Services.Users;

public class RoleService : IRoleService
{
    private readonly ILoggingIdeaIncubator _loggingIdeaIncubator;
    
    private readonly IdeaIncubatorDbContext _dbContext;

    public RoleService(IdeaIncubatorDbContext dbContex, ILoggingIdeaIncubator loggingIdeaIncubator)
    {
        _dbContext = dbContex;
        _loggingIdeaIncubator = loggingIdeaIncubator;
    }

    public Task<Role> CreateRoleAsync(Role role)
    {
        throw new NotImplementedException();
    }

    public void DeleteRole(int id)
    {
        throw new NotImplementedException();
    }

    public Role GetRole(int id)
    {
        Role role = _dbContext.Roles.FirstOrDefault(r => r.RoleId == id);
        return role;
    }

    public List<Role> GetRoles()
    {
        return _dbContext.Roles.ToList();
    }

    public int UpdateRole(Role role)
    {
        throw new NotImplementedException();
    }
}
