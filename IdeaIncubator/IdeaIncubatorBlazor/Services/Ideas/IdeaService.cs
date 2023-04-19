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
using IdeaIncubatorBlazor.Services.Users;
using IdeaIncubatorBlazor.Utils.Loggings;
using Microsoft.EntityFrameworkCore;

namespace IdeaIncubatorBlazor.Services.Ideas;

public class IdeaService : IIdeaService
{
    private readonly ILoggingIdeaIncubator _loggingIdeaIncubator;

    private readonly IdeaIncubatorDbContext _dbContext;

    private readonly IUserService _userService;
    
    private readonly IRoleService _roleService;

    private readonly IUserIdeaRoleService _userIdeaRoleService;

    const int PROVIDER_ROLE_ID = 1;

    private List<UserIdeaRole> originalUserIdeaRoles;

    public IdeaService(IdeaIncubatorDbContext dbContext, IUserService userService, IRoleService roleService, IUserIdeaRoleService userIdeaRoleService, ILoggingIdeaIncubator loggingIdeaIncubator)
    {
        _dbContext = dbContext;
        _userService = userService;
        _loggingIdeaIncubator = loggingIdeaIncubator;
        _roleService = roleService;
        _userIdeaRoleService = userIdeaRoleService;
        originalUserIdeaRoles = new List<UserIdeaRole>();
    }

    public async Task<Idea> CreateIdeaAsync(Idea idea, int userId)
    {
        _dbContext.Ideas.Add(idea);
        await _dbContext.SaveChangesAsync();
        CreateUserIdeaRole(userId, idea.IdeaId, PROVIDER_ROLE_ID);
        return idea;
    }

    private void CreateUserIdeaRole(int userId, int ideaId, int roleId)
    {
        UserIdeaRole newUserIdeaRole = _userIdeaRoleService.CreateUserIdeaRole(userId, ideaId, roleId).Result;     
        originalUserIdeaRoles.Add(newUserIdeaRole);
    }

    public async Task<string> DeleteIdea(int ideaId)
    {
        string result = "";
        try
        {
            // delete contents
            var contentIdList = _dbContext.IdeaContents.Where(x => x.IdeaId == ideaId).Select(x => x.ContentId).ToList();
            _dbContext.IdeaContents.Where(x => x.IdeaId == ideaId).ExecuteDelete();
            _dbContext.Contents.Where(c => contentIdList.Contains(c.ContentId)).ExecuteDelete();

            // delete user roles
            _dbContext.UserIdeaRoles.Where(x => x.IdeaId == ideaId).ExecuteDelete();
            _dbContext.UserIdeas.Where(x => x.IdeaId == ideaId).ExecuteDelete();

            _dbContext.Ideas.Where(x => x.IdeaId == ideaId).ExecuteDelete();
            await _dbContext.SaveChangesAsync();
            result = "deleted";
        }
        catch (Exception e)
        {
            result = e.Message;
        }
        return result;
    }

    #region GET IDEA
    public Idea GetIdea(int id)
    {
        Idea idea = _dbContext.Ideas.FirstOrDefault(i => i.IdeaId == id);
        return idea;
    }

    public List<Idea> GetIdeas()
    {
        List<Idea> ideas = _dbContext.Ideas?.ToList();

        return ideas;
    }

    public List<Idea> GetOwnIdeas(int userId)
    {
        List<Idea> result = new();
        List<UserIdeaRole> temp = GetUserIdeaRoles().Where(x => x.UserId == userId && x.RoleId == (int)UserRoleEnum.Provider)?.ToList();
        foreach (int _ideaID in temp.Select(t => t.IdeaId))
        {
            result.Add(_dbContext.Ideas.FirstOrDefault(i => i.IdeaId == _ideaID));
        }
        return result;
    }

    public List<Idea> GetCollabIdeas(int userId)
    {
        List<Idea> result = new();
        List<UserIdeaRole> temp = GetUserIdeaRoles().Where(x => x.UserId == userId &&
                                                        (x.RoleId == (int)UserRoleEnum.Collaborator1 || x.RoleId == (int)UserRoleEnum.Collaborator2))?.ToList();
        foreach (int _ideaID in temp.Select(t => t.IdeaId))
        {
            result.Add(_dbContext.Ideas.FirstOrDefault(i => i.IdeaId == _ideaID));
        }
        return result;
    }

    public List<Idea> GetWishList(int userId)
    {
        List<Idea> result = new();
        List<UserIdea> temp = _dbContext.UserIdeas.Where(x => x.UserId == userId && x.IsWishlist)?.ToList();
        foreach (int _ideaID in temp.Select(t => t.IdeaId))
        {
            result.Add(_dbContext.Ideas.FirstOrDefault(i => i.IdeaId == _ideaID));
        }
        return result;
    }

    /// <summary>
    /// Search for name and keywords fields
    /// </summary>
    /// <param name="searchInput"></param>
    /// <returns></returns>
    public List<Idea> SearchIdeas(string searchInput)
    {
        searchInput = searchInput.ToLower().Trim();

        List<Idea> ideas = new List<Idea>();
        ideas = _dbContext.Ideas.Where(i => i.Name.ToLower().Contains(searchInput) ||
                                    (string.IsNullOrWhiteSpace(i.Keywords) ? false : i.Keywords.ToLower().Contains(searchInput)))?.ToList();
        return ideas;
    }
    #endregion


    #region GET USER
    public async Task<User> GetProviderOfIdea(int ideaID)
    {
        UserIdeaRole temp = GetUserIdeaRoles().FirstOrDefault(x => x.IdeaId == ideaID && x.RoleId == (int)UserRoleEnum.Provider);
        if (temp != null)
        {
            List<User> allUsers = _userService.GetAllUsers();
            return allUsers.FirstOrDefault(x => x.UserId == temp.UserId);
        }
        return null;
    }

    public async Task<List<UserIdeaRole>> GetCollaboratorsOfIdea(int ideaID, bool incProvider = false)
    {
        List<UserIdeaRole> temp = GetUserIdeaRoles().Where(x => x.IdeaId == ideaID &&
                                                (x.RoleId == (int)UserRoleEnum.None || x.RoleId == (int)UserRoleEnum.Collaborator1 || x.RoleId == (int)UserRoleEnum.Collaborator2 || (incProvider && x.RoleId == (int)UserRoleEnum.Provider)))?.ToList();
        return temp;
    }

    public async Task<List<UserIdeaRole>> GetRequestersOfIdea(int ideaID)
    {
        List<UserIdeaRole> temp = GetUserIdeaRoles().Where(x => x.IdeaId == ideaID && x.RoleId == (int)UserRoleEnum.Requester)?.ToList();
        return temp;
    }
    #endregion


    public Idea UpdateIdea(Idea idea)
    {
        _dbContext.Ideas.Update(idea);
        _dbContext.SaveChanges();
        return idea;
    }


    
    public List<UserIdeaRole> GetUserIdeaRoles()
    {
        if (!originalUserIdeaRoles.Any())
        {
            originalUserIdeaRoles = _dbContext.UserIdeaRoles.ToList();
        }
        return originalUserIdeaRoles;
    }


    public async Task<List<UserIdeaRole>> GetUserIdeaRolesAsync()
    {
        if (!originalUserIdeaRoles.Any())
        {
            originalUserIdeaRoles = await _dbContext.UserIdeaRoles.ToListAsync();
        }
        return originalUserIdeaRoles;
    }

    public async Task<UserIdea> CreateUserIdeaAsync(UserIdea userIdea)
    {
        _dbContext.UserIdeas.Add(userIdea);
        await _dbContext.SaveChangesAsync();
        return userIdea;
    }

    public UserIdea CreateUserIdea(UserIdea userIdea)
    {
        _dbContext.UserIdeas.Add(userIdea);
        _dbContext.SaveChanges();
        return userIdea;
    }

    public async Task<UserIdea> UpdateUserIdeaAsync(UserIdea userIdea)
    {
        _dbContext.UserIdeas.Update(userIdea);
        await _dbContext.SaveChangesAsync();
        return userIdea;
    }

    public UserIdea UpdateUserIdea(UserIdea userIdea)
    {
        _dbContext.UserIdeas.Update(userIdea);
        _dbContext.SaveChanges();
        return userIdea;
    }

    public async Task<List<UserIdea>> GetUserIdeasAsync(int userId)
    {
        return await _dbContext.UserIdeas.Where(ui => ui.UserId == userId).ToListAsync();
    }
}

