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
using Microsoft.AspNetCore.Components;
using ComponentBase = Microsoft.AspNetCore.Components.ComponentBase;

namespace IdeaIncubatorBlazor.Views.Components;

public partial class DetailMembers : ComponentBase
{
    [Parameter] public Idea SelectedIdea { get; set; }
    [Parameter] public bool IsProviderUser { get; set; }
    [Parameter] public int LoggedInUserId { get; set; }

    public List<MemberInfo> Members;
    public List<MemberInfo> Requesters;

    string[] labels = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
    private bool _IsJoinRequested;
    private bool _IsOutsider;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Members = new();
            Requesters = new();
            GetMembers().Wait();
            if (IsProviderUser) GetRequesters();

            Members = Members.OrderByDescending(x => x.PrivilegeLevel).ToList();            
            StateHasChanged();
        }
    }


    private async Task GetMembers()
    {
        User uProvider = ideaService.GetProviderOfIdea(SelectedIdea.IdeaId).Result;
        MemberInfo mProvider = new() { 
            UserId = uProvider.UserId,
            UserName = uProvider.UserName,
            EmailAddress = uProvider.EmailAddress,
            RoleId = (int)UserRoleEnum.Provider,
            PrivilegeLevel = 10,
            SkillSets = uProvider.SkillSets ?? "",
            IsProfileVisible = (bool)uProvider.IsProfileVisible
        };
        Members.Add(mProvider);

        List<UserIdeaRole> userIdeaRoles = ideaService.GetCollaboratorsOfIdea(SelectedIdea.IdeaId).Result;
        foreach (UserIdeaRole _role in userIdeaRoles)
        {
            Role _r = roleService.GetRole(_role.RoleId);
            Members.Add(new MemberInfo { UserId = _role.UserId,
                UserName = _role.User.UserName,
                EmailAddress = _role.User.EmailAddress,
                RoleId = _r.RoleId,
                PrivilegeLevel = _r.PrivilegeLevel ?? 0,
                SkillSets = _role.User.SkillSets ?? "",
                IsProfileVisible = (bool)_role.User.IsProfileVisible
            });
        }

        if (!Members.Any(m => m.UserId == LoggedInUserId)) _IsOutsider = true;
    }

    private void GetRequesters()
    {
        List<UserIdeaRole> userIdeaRoles = ideaService.GetRequestersOfIdea(SelectedIdea.IdeaId).Result;
        if (userIdeaRoles.Count > 0)
        {
            foreach (UserIdeaRole _role in userIdeaRoles)
            {
                MemberInfo mRequester = new() { UserId = _role.UserId,
                    UserName = _role.User.UserName,
                    EmailAddress = _role.User.EmailAddress,
                    RoleId = (int)UserRoleEnum.Requester,
                    PrivilegeLevel = 0,
                    SkillSets = _role.User.SkillSets ?? "",
                    IsProfileVisible = (bool)_role.User.IsProfileVisible
                };
                Requesters.Add(mRequester);
            }
            _IsJoinRequested = true;
        }
        else _IsJoinRequested = false;
    }

    protected async Task SelectedRoleChanged(int roleId, MemberInfo memberInfo)
    {
        int newId = Convert.ToInt32(roleId);
        Role newRole = roleService.GetRole(newId);
        memberInfo.PrivilegeLevel = newRole?.PrivilegeLevel ?? 0;
    }

    protected void UpdatePrivilege()
    {
        List<UserIdeaRole> orgRoles = ideaService.GetCollaboratorsOfIdea(SelectedIdea.IdeaId).Result;
        foreach (UserIdeaRole _org in orgRoles)
        {            
            if (Members.FirstOrDefault(m => m.UserId == _org.UserId).RoleId != _org.RoleId)
            {
                UserIdeaRole newRole = new UserIdeaRole() { UserId = _org.UserId, IdeaId = SelectedIdea.IdeaId, RoleId = Members.FirstOrDefault(m => m.UserId == _org.UserId).RoleId };
                if (userIdeaRoleService.UpdateUserIdeaRole(newRole) > 0)
                {
                    Snackbar.Add("Changes are saved.");
                }
            }
        }
    }

    protected async Task RespondToRequest(MemberInfo member, int newRoleId)
    {
        UserIdeaRole roleToUpdate = userIdeaRoleService.GetUserIdeaRole(member.UserId, SelectedIdea.IdeaId, member.RoleId);
        roleToUpdate.RoleId = newRoleId;
        if (userIdeaRoleService.UpdateUserIdeaRole(roleToUpdate) > 0)
        {
            Snackbar.Add(String.Format("{0} is {1}", member.UserName, newRoleId == 0 ? "accepted" : "rejected"));
            await OnAfterRenderAsync(true);
        }
    }    
}
