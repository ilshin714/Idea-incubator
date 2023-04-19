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
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;

namespace IdeaIncubatorBlazor.Views.Components;

public partial class IdeaDetailDialog
{
    [Inject]
    ProtectedSessionStorage ProtectedSessionStore { get; set; }

    [Inject]
    private IUserIdeaRoleService UserIdeaRoleService { get; set; }
    [Inject]
    IIdeaService IdeaService { get; set; }

    [Inject]
    ISnackbar Snackbar { get; set; }

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public Idea SelectedIdea { get; set; }

    [Parameter]
    public User Provider { get; set; }

    [Parameter]
    public List<UserIdeaRole> Participants { get; set; }

    [Parameter]
    public int CollaboratorsCount { get; set; }

    const int WAITER_ROLE_ID = (int)UserRoleEnum.Requester;

    public int UserId { get; set; }

    public bool IsOutSider { get; set; }

    MudButton JoinButton { get; set; }

    public string JoinIdeaButtonText { get; set; }
    public bool IsProviderUser { get; set; }
    MudTabPanel MembersTab { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsOutSider = false;
        JoinIdeaButtonText = "Join Idea";
    }

    protected override async Task OnParametersSetAsync()
    {
        string userId = (await ProtectedSessionStore.GetAsync<string>("UserId")).Value ?? "";
        UserId = string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId);
        if (UserId != 0 && UserId == Provider.UserId) IsProviderUser = true;
        UserIdeaRole userIdeaRole = UserIdeaRoleService.GetUserIdeaRole(UserId, SelectedIdea.IdeaId, WAITER_ROLE_ID);

        if (UserId == 0) { IsOutSider = true; }
        else if (UserId != Provider.UserId)
        {
            if (userIdeaRole?.RoleId == WAITER_ROLE_ID)
            {
                IsOutSider = true;
                DisableJoinButton();
            }
            else if (CollaboratorsCount == 0)
            {
                IsOutSider = true;
            }
            else
            {
                IsOutSider = Participants.FirstOrDefault(p => p.UserId == UserId)?.RoleId switch
                {
                    null => true,
                    _ => false,
                };
            }
        }
        if (IsProviderUser) UpdateMembersTabHeader();
    }

    protected void JoinIdea()
    {
        UserIdeaRole newUserIdeaRole = UserIdeaRoleService.CreateUserIdeaRole(UserId, SelectedIdea.IdeaId, WAITER_ROLE_ID).Result;
        if (newUserIdeaRole.RoleId == WAITER_ROLE_ID)
        {
            Snackbar.Add("Success Joining Request");
            DisableJoinButton();
        }

    }
    protected void DisableJoinButton()
    {
        JoinIdeaButtonText = "Join Requested";
        if (JoinButton != null)
        {
            JoinButton.Disabled = true;
            JoinButton.Color = Color.Info;
            JoinButton.Variant = Variant.Filled;
        }
    }

    private void UpdateMembersTabHeader()
    {
        List<UserIdeaRole> userIdeaRoles = IdeaService.GetRequestersOfIdea(SelectedIdea.IdeaId).Result;
        if (userIdeaRoles.Count > 0)
        {
            MembersTab.BadgeDot = true;
            MembersTab.BadgeColor = Color.Primary;
        }
    }

    private void UpdateDialogHeader()
    {
        // get latest data
        SelectedIdea = IdeaService.GetIdea(SelectedIdea.IdeaId);
    }

    void Submit() => MudDialog.Close(DialogResult.Ok(true));

    void Cancel() => MudDialog.Cancel();
}
