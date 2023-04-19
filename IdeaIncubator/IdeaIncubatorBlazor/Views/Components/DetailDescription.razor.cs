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
using IdeaIncubatorBlazor.Services.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;

namespace IdeaIncubatorBlazor.Views.Components;

public partial class DetailDescription : ComponentBase
{
    [Parameter]
    public Idea SelectedIdea { get; set; }
    [Parameter]
    public User Provider { get; set; }
    [Parameter]
    public bool IsOutSider { get; set; }
    [Parameter]
    public EventCallback OnIdeaChanged { get; set; }

    [Inject]
    ICommentService commentService { get; set; }
    [Inject]
    IContentService contentService { get; set; }
    [Inject]
    IIdeaService ideaService { get; set; }
    [Inject]
    IRoleService roleService { get; set; }
    [Inject]
    IUserService userService { get; set; }
    [Inject]
    ProtectedSessionStorage ProtectedSessionStore { get; set; }

    public List<Comment> Comments { get; set; }
    public List<Content> Contents { get; set; }
    public Content inputContent { get; set; }
    public Content whichContent { get; set; }
    public string CommentString { get; set; }
    public int ShowContentId { get; set; }
    public Comment whichEditComment { get; set; }
    public int UserLevel { get; set; }
    public int UserId { get; set; }
    public bool DisplayEditComment { get; set; }
    public List<User> Users { get; set; }

    public bool DisplayAddContent { get; set; }
    public bool DisplayEditContent { get; set; }

    private bool _IsProviderUser;
    private bool _ShowEditForm;
    private MudForm? _Form;
    private Idea _OrgIdea;
    private bool _IsLoading;
    private bool _IsWishlist;
    private bool _IsVoted;
    private UserIdea? thisUserIdea;

    protected override async Task OnInitializedAsync()
    {
        _IsLoading = true;
        UserLevel = 0;
        ShowContentId = 0;
        DisplayAddContent = false;
        DisplayEditContent = false;
        DisplayEditComment = false;
        List<UserIdeaRole> userIdeaRoles = ideaService.GetCollaboratorsOfIdea(SelectedIdea.IdeaId, true).Result;

        string userId = (await ProtectedSessionStore.GetAsync<string>("UserId")).Value ?? "";
        if (!(String.IsNullOrEmpty(userId)))
        {
            int userIdInt = Convert.ToInt32(userId);
            UserId = userIdInt;
            foreach (UserIdeaRole userIdeaRole in userIdeaRoles)
            {
                if (userIdeaRole.UserId == userIdInt)
                {
                    UserLevel = roleService.GetRole(userIdeaRole.RoleId).PrivilegeLevel ?? 0;
                    break;
                }
            }
            Comments = new List<Comment>();
            Contents = contentService.GetAllContentsOfAnIdea(SelectedIdea.IdeaId, UserLevel);
        }
        int _id = string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId);
        if (_id != 0 && _id == Provider.UserId) _IsProviderUser = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_IsProviderUser)
            {
                _OrgIdea = new Idea()
                {
                    Name = SelectedIdea.Name,
                    Description = SelectedIdea.Description,
                    Status = SelectedIdea.Status,
                    Keywords = SelectedIdea.Keywords
                };
            }

            // find the UserIdea record
            if (UserId > 0)
            {
                thisUserIdea = (await ideaService.GetUserIdeasAsync(UserId))?.FirstOrDefault(i => i.IdeaId == SelectedIdea.IdeaId);
                _IsWishlist = thisUserIdea?.IsWishlist ?? false;
                _IsVoted = thisUserIdea?.IsVoted ?? false;
            }
        }
        _IsLoading = false;
        StateHasChanged();
    }

    protected async void AddComment()
    {
        string userId = (await ProtectedSessionStore.GetAsync<string>("UserId")).Value ?? "";
        if (!(String.IsNullOrEmpty(userId)))
        {
            int userIdInt = Convert.ToInt32(userId);
            Comment comment = new Comment();
            comment.CommentText = CommentString;
            comment.UserId = userIdInt;
            await commentService.CreateCommentAsync(comment, ShowContentId);
            Comments = commentService.GetAllCommentsOfAContent(ShowContentId);
        }
        CommentString = "";
        StateHasChanged();
    }

    protected void AddContent()
    {
        inputContent = new Content();
        DisplayAddContent = true;
        DisplayEditComment = false;
        StateHasChanged();
    }

    protected async void DoAddContent()
    {
        string userId = (await ProtectedSessionStore.GetAsync<string>("UserId")).Value ?? "";
        if (!(String.IsNullOrEmpty(userId)))
        {
            int userIdInt = Convert.ToInt32(userId);
            inputContent.ContentType = 1;
            inputContent.Writer = userIdInt;
            await contentService.CreateContentAsync(inputContent, SelectedIdea.IdeaId);
            Contents = contentService.GetAllContentsOfAnIdea(SelectedIdea.IdeaId, UserLevel);
        }
        DisplayAddContent = false;
        DisplayEditComment = false;
        StateHasChanged();
    }
    public async void ShowContent(int contentId)
    {
        ShowContentId = contentId;
        Comments = commentService.GetAllCommentsOfAContent(contentId);
        Users = userService.GetAllUsers();
        whichContent = contentService.GetContent(contentId);
        DisplayEditComment = false;
        StateHasChanged();
    }

    protected async void EditContent()
    {
        inputContent = new Content();
        inputContent.Name = whichContent.Name;
        inputContent.Description = whichContent.Description;
        inputContent.DisclosureLevel = whichContent.DisclosureLevel;
        DisplayEditContent = true;
        DisplayAddContent = false;
        DisplayEditComment = false;
        StateHasChanged();
    }

    protected async void CancelAddContent()
    {
        DisplayEditContent = false;
        DisplayAddContent = false;
        DisplayEditComment = false;
        ShowContentId = 0;
        StateHasChanged();
    }

    protected async void CancelEditContent()
    {
        DisplayEditContent = false;
        DisplayAddContent = false;
        DisplayEditComment = false;
        StateHasChanged();
    }

    protected async void SaveEditContent()
    {
        whichContent.Name = inputContent.Name;
        whichContent.Description = inputContent.Description;
        whichContent.DisclosureLevel = inputContent.DisclosureLevel;
        bool validUpdate = await contentService.UpdateContent(whichContent);
        if (validUpdate)
        {
            Contents = contentService.GetAllContentsOfAnIdea(SelectedIdea.IdeaId, UserLevel);
            DisplayEditContent = false;
            DisplayAddContent = false;
            StateHasChanged();
        }
        else
        {
            Snackbar.Add("There was an error updating the content in the database.");
        }
    }

    protected async void DeleteContent()
    {
        bool? result = await dialogService.ShowMessageBox(
                "Delete Content",
                "Are you sure you want to delete this content?",
                yesText: "Delete", cancelText: "Cancel",
                options: new DialogOptions() { FullWidth = true, MaxWidth = MaxWidth.Small }
                );

        if (result == true)
        {
            result = await contentService.DeleteContent(whichContent);
            if (result == true)
            {
                bool? ok = await dialogService.ShowMessageBox(
                    "Delete Content",
                    "Content is deleted.",
                    options: new DialogOptions() { FullWidth = true, MaxWidth = MaxWidth.Small }
                    );
                ShowContentId = 0;
                Contents = contentService.GetAllContentsOfAnIdea(SelectedIdea.IdeaId, UserLevel);
                DisplayEditComment = false;
                StateHasChanged();
            }
            else
            {
                Snackbar.Add($"Couldn't delete the content there was database issue");
            }
        }
    }

    protected async void EditComment(int commentId)
    {
        Comment tempComment = await commentService.GetCommentAsync(commentId);
        CommentString = tempComment.CommentText;
        whichEditComment = tempComment;
        DisplayEditComment = true;
        StateHasChanged();
    }


    protected async void SaveEditComment()
    {
        whichEditComment.CommentText = CommentString;
        bool validUpdate = await commentService.UpdateComment(whichEditComment);
        if (validUpdate)
        {
            Comments = commentService.GetAllCommentsOfAContent(ShowContentId);
            DisplayEditContent = false;
            DisplayAddContent = false;
            DisplayEditComment = false;
            CommentString = "";
            StateHasChanged();
        }
        else
        {
            Snackbar.Add("There was an error updating the comment in the database.");
        }
    }

    protected async void CancelEditComment()
    {
        CommentString = "";
        DisplayEditComment = false;
        DisplayEditComment = false;
        StateHasChanged();
    }
    protected async void DeleteComment(int commentId)
    {
        Comment tempComment = commentService.GetComment(commentId);
        bool? result = await dialogService.ShowMessageBox(
                "Delete Comment",
                "Are you sure you want to delete the comment: " + tempComment.CommentText,
                yesText: "Delete", cancelText: "Cancel",
                options: new DialogOptions() { FullWidth = true, MaxWidth = MaxWidth.Small }
                );

        if (result == true)
        {
            result = await commentService.DeleteComment(tempComment);
            if (result == true)
            {
                bool? ok = await dialogService.ShowMessageBox(
                    "Delete Comment",
                    "Comment is deleted.",
                    options: new DialogOptions() { FullWidth = true, MaxWidth = MaxWidth.Small }
                    );
                Comments = commentService.GetAllCommentsOfAContent(ShowContentId);
                DisplayEditComment = false;
                CommentString = "";
                StateHasChanged();
            }
            else
            {
                Snackbar.Add($"Couldn't delete the comment there was database issue");
            }
        }
    }

    protected void OpenEditForm()
    {
        _ShowEditForm = !_ShowEditForm;
    }

    protected void CancelChange()
    {
        SelectedIdea.Name = _OrgIdea.Name;
        SelectedIdea.Description = _OrgIdea.Description;
        SelectedIdea.Status = _OrgIdea.Status;
        SelectedIdea.Keywords = _OrgIdea.Keywords;
        OpenEditForm();
    }

    protected async void SaveChange()
    {
        if (_Form.IsTouched)
        {
            bool? result = await dialogService.ShowMessageBox(
                "Save Changes",
                "Do you want to save changes?",
                yesText: "Save", cancelText: "Cancel",
                options: new DialogOptions() { FullWidth=true, MaxWidth = MaxWidth.Small });

            if (result == true)
            {
                ideaService.UpdateIdea(SelectedIdea);
                StateHasChanged();
                OpenEditForm();
            }
        }
        else OpenEditForm();

        await OnIdeaChanged.InvokeAsync();
    }

    protected async Task DeleteIdea()
    {
        bool? result = await dialogService.ShowMessageBox(
                "Delete Idea",
                "Are you sure you want to delete this idea?",
                yesText: "Delete", cancelText: "Cancel",
                options: new DialogOptions() { FullWidth = true, MaxWidth = MaxWidth.Small }
                );

        if (result == true)
        {
            string dbResult = await ideaService.DeleteIdea(SelectedIdea.IdeaId);
            if (dbResult == "deleted")
            {
                bool? ok = await dialogService.ShowMessageBox(
                    "Delete Idea",
                    "Idea is deleted.",
                    options: new DialogOptions() { FullWidth = true, MaxWidth = MaxWidth.Small }
                    );
                if (ok == true)
                {
                    StateHasChanged();
                    UriHelper.NavigateTo("/", true);
                }
            }
            else
            {
                Snackbar.Add($"Couldn't delete the idea: {dbResult}");
            }
        }
    }
    private async Task ClickFavorite()
    {
        if (UserId == 0)
        {
            UriHelper.NavigateTo("login");
            return;
        }

        if (thisUserIdea == null)
        {
            thisUserIdea = new UserIdea() { IdeaId = SelectedIdea.IdeaId, UserId = this.UserId, IsWishlist = true };
            ideaService.CreateUserIdea(thisUserIdea);
        }
        else
        {
            thisUserIdea.IsWishlist = !thisUserIdea.IsWishlist;
            ideaService.UpdateUserIdea(thisUserIdea);
        }

        _IsWishlist = thisUserIdea.IsWishlist;
        StateHasChanged();
    }

    private async Task ClickVote()
    {
        if (UserId == 0)
        {
            UriHelper.NavigateTo("login");
            return;
        }

        if (thisUserIdea == null)
        {
            thisUserIdea = new UserIdea() { IdeaId = SelectedIdea.IdeaId, UserId = this.UserId, IsVoted = true };
            ideaService.CreateUserIdea(thisUserIdea);
            SelectedIdea.Vote += 1;
            ideaService.UpdateIdea(SelectedIdea);
        }
        else
        {
            thisUserIdea.IsVoted = !thisUserIdea.IsVoted;
            ideaService.UpdateUserIdea(thisUserIdea);
            SelectedIdea.Vote -= 1;
            ideaService.UpdateIdea(SelectedIdea);
        }
        _IsVoted = thisUserIdea.IsVoted;
        StateHasChanged();
    }
}