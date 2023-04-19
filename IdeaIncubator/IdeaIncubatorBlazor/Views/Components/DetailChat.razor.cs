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
using IdeaIncubatorBlazor.Services.Ideas;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Reflection.Metadata;
using ComponentBase = Microsoft.AspNetCore.Components.ComponentBase;

namespace IdeaIncubatorBlazor.Views.Components;

public partial class DetailChat : ComponentBase
{
    [Inject]
    IChatService chatService { get; set; }

    [Inject]
    ISnackbar Snackbar { get; set; }

    [Parameter]
    public Idea SelectedIdea { get; set; }

    [Parameter]
    public int UserId { get; set; }
    [Parameter]
    public User Provider { get; set; }

    [Parameter]
    public List<UserIdeaRole> Participants { get; set; }

    public List<MemberInfo> Members;

    public List<ChatGroup> ChatGroups { get; set; }
    public List<ChatGroupMember> ChatMembers { get; set; }
    MudTabs? ParentTab { get; set; }
    object FocusPanelID = 0;

    protected override void OnInitialized()
    {
        //ChatGroups = chatService.GetChatGroups().Where(g => g.IdeaId == SelectedIdea.IdeaId).ToList();
        ChatGroups = chatService.GetChatGroupsByIdeaId(SelectedIdea.IdeaId);
        if (ChatMembers == null)
        {
            ChatMembers = new List<ChatGroupMember>();
        }
        if (ChatGroups.Any())
        {
            foreach (var chatGroup in ChatGroups)
            {
                ChatMembers.AddRange(chatService.GetChatMembersByChatGroupId(chatGroup.ChatGroupId));
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Members = new();
            GetMembers().Wait();

            Members = Members.OrderByDescending(x => x.PrivilegeLevel).ToList();
            StateHasChanged();
        }
        else if ((int)FocusPanelID > 0)
        {
            // Set Focus to chat
            ParentTab?.ActivatePanel(FocusPanelID);
            FocusPanelID = 0;
        }
    }


    private async Task GetMembers()
    {
        User uProvider = ideaService.GetProviderOfIdea(SelectedIdea.IdeaId).Result;

        MemberInfo mProvider = new() { UserId = uProvider.UserId, UserName = uProvider.UserName, RoleId = (int)UserRoleEnum.Provider, PrivilegeLevel = 10, SkillSets = uProvider.SkillSets ?? "", IsCurrentUser = uProvider.UserId == UserId ? true : false };
        Members.Add(mProvider);

        List<UserIdeaRole> userIdeaRoles = ideaService.GetCollaboratorsOfIdea(SelectedIdea.IdeaId).Result;
        foreach (UserIdeaRole _role in userIdeaRoles)
        {
            Role _r = roleService.GetRole(_role.RoleId);
            Members.Add(new MemberInfo { UserId = _role.UserId, UserName = _role.User.UserName, RoleId = _r.RoleId, PrivilegeLevel = _r.PrivilegeLevel ?? 0, SkillSets = _role.User.SkillSets ?? "", IsCurrentUser = _role.UserId == UserId ? true : false });
        }
    }

    protected void StartOneOnOneChat(int userId)
    {
        ChatGroups = chatService.GetChatGroupsByIdeaId(SelectedIdea.IdeaId);
        foreach (var chatgroup in ChatGroups)
        {
            if (ChatMembers.Any(m => m.ChatGroupId == chatgroup.ChatGroupId && m.UserId == userId)
                && ChatMembers.Any(m => m.ChatGroupId == chatgroup.ChatGroupId && m.UserId == UserId))
            {
                //Snackbar.Add( "You already have the chat with the user", Severity.Warning);
                // when the chat already exists for the selected user, set focus to the corresponding panel 
                FocusPanelID = chatgroup.ChatGroupId;
                return;
            }
        }
        ChatGroup chatGroup = chatService.CreateChatGroup(SelectedIdea.IdeaId).Result;
        ChatGroups.Add(chatGroup);
        List<ChatGroupMember> members = new List<ChatGroupMember>();
        ChatGroupMember memberFirst = new ChatGroupMember { ChatGroupId = chatGroup.ChatGroupId, UserId = userId };
        ChatGroupMember memberSecond = new ChatGroupMember { ChatGroupId = chatGroup.ChatGroupId, UserId = UserId };
        members.Add(memberFirst);
        members.Add(memberSecond);

        chatService.CreateChatGroupMembers(members);

        ChatMembers.AddRange(members);
        FocusPanelID = chatGroup.ChatGroupId;
    }
}
