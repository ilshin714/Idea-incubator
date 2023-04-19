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
using IdeaIncubatorBlazor.Models.Basics;
using IdeaIncubatorBlazor.Views.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using System.Data;
using ComponentBase = Microsoft.AspNetCore.Components.ComponentBase;

namespace IdeaIncubatorBlazor.Views.Pages;

public partial class Index : ComponentBase
{
    [Inject] 
    IDialogService Dialog { get; set; }

    [Parameter] 
    public string SearchInput { get; set; }

    [Parameter] 
    public string SortBy { get; set; }

    public ComponentState State { get; set; }

    public bool IsLoading { get; set; }

    private List<Idea> ideas;

    private bool userLoggedIn = false;

    DialogOptions maxWidthForIdeaCreation;

    public Color FavoriteButtonColor { get; set; }

    private List<UserIdea> allUserIdeas;

//    public List<UserIdea> WishList { get; set; }

    public List<DataSet> IdeaDisplayContents { get; set; }

    public int UserId { get; set; }

    public class DataSet
    {
        public Idea Idea { get; set; }

        public bool IsWishList { get; set; }

        public bool IsVoted { get; set; }
    }

    protected override void OnInitialized()
    {
        IsLoading = true;
        maxWidthForIdeaCreation = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        State = ComponentState.Conetent;
        if (ideas.IsNullOrEmpty()) ideas = ideaService.GetIdeas();
        IdeaDisplayContents = new List<DataSet>();
        foreach (var idea in ideas)
        {
            IdeaDisplayContents.Add(new DataSet() { Idea = idea, IsWishList = false, IsVoted = false });
        }
    }


    protected async Task OpenIdeaCreationDialog(DialogOptions options)
    {
        var dialog = Dialog.Show<IdeaCreationDialog>("Create Your Idea", options);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Idea newIdea = (Idea)result.Data;
            if (newIdea != null && ideas.IsNullOrEmpty())
            {
                ideas = ideaService.GetIdeas();
            }
            ideas.Add(newIdea);
            IdeaDisplayContents.Add(new DataSet() { Idea = newIdea, IsWishList = false, IsVoted = false});
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            bool result = false;
            string tempId = (await ProtectedSessionStore.GetAsync<string>("UserId")).Value ?? "";
            if (tempId != "" && tempId != "0")
            {
                UserId = Convert.ToInt32(tempId);
                result = true;
                if (allUserIdeas.IsNullOrEmpty()) allUserIdeas = await ideaService.GetUserIdeasAsync(UserId);
                //WishList = allUserIdeas.FindAll(a => a.UserId == UserId);
                foreach(var wish in allUserIdeas)
                {
                    IdeaDisplayContents.FirstOrDefault(i => i.Idea.IdeaId == wish.IdeaId).IsWishList = wish.IsWishlist;
                    IdeaDisplayContents.FirstOrDefault(i => i.Idea.IdeaId == wish.IdeaId).IsVoted = wish.IsVoted;
                }
            }
            userLoggedIn = result;         
        }
        IsLoading = false;
        StateHasChanged();
    }

    protected void Search()
    {
        if (string.IsNullOrEmpty(SearchInput)) ideas = ideaService.GetIdeas();
        else ideas = ideaService.SearchIdeas(SearchInput);
        
        RefreshIdeasAfterSearch();
    }

    protected void SortIdea()
    {
        switch (SortBy)
        {
            case "Latest":
                ideas = ideas.OrderByDescending(i => i.CreatedDate).ToList();
                break;
            case "Most Liked":
                ideas = ideas.OrderByDescending(i => i.Vote).ToList();
                break;
            case "Collaborators":
                ideas = ideas.OrderByDescending(i => i.UserIdeaRoles.Count(r => r.RoleId == (int)UserRoleEnum.Collaborator1 || r.RoleId == (int)UserRoleEnum.Collaborator2)).ToList();
                break;
            default:
                ideas = ideas.OrderBy(i => i.IdeaId).ToList();
                break;
        }

        RefreshIdeasAfterSearch();
    }

    protected void RefreshIdeasAfterSearch()
    {
        IdeaDisplayContents.Clear();

        foreach (var idea in ideas)
        {
            IdeaDisplayContents.Add(new DataSet() { Idea = idea, IsWishList = false, IsVoted = false });
        }

        if (UserId > 0)
        {
            foreach (var wish in allUserIdeas)
            {
                DataSet data = IdeaDisplayContents.FirstOrDefault(i => i.Idea.IdeaId == wish.IdeaId);
                if(data != null)
                {
                    data.IsWishList = wish.IsWishlist;
                    data.IsVoted = wish.IsVoted;
                }
            }
        }
    }

    protected async Task OnFavoriteClicked(int ideaId)
    {
        UserIdea userIdea = allUserIdeas.FirstOrDefault(l => l.IdeaId == ideaId);

        if (userIdea == null)
        {
            userIdea = new UserIdea() { IdeaId = ideaId, UserId = UserId, IsWishlist = true };
            ideaService.CreateUserIdea(userIdea);
            allUserIdeas.Add(userIdea);
            //WishList.Add(userIdea);
            IdeaDisplayContents.FirstOrDefault(i => i.Idea.IdeaId == ideaId).IsWishList = true;
        }
        else
        {
            if (userIdea.IsWishlist == false)
            {
                userIdea.IsWishlist = true;
                ideaService.UpdateUserIdea(userIdea);
                UpdateFaroviteIdeaToDisplay(ideaId, userIdea.IsWishlist);
            }
            else
            {
                userIdea.IsWishlist = false;
                ideaService.UpdateUserIdea(userIdea);
                UpdateFaroviteIdeaToDisplay(ideaId, userIdea.IsWishlist);
            }
        }
    }

    protected void UpdateFaroviteIdeaToDisplay(int ideaId, bool value)
    {
        //WishList.FirstOrDefault(w => w.IdeaId == ideaId).IsWishlist = value;
        allUserIdeas.FirstOrDefault(u => u.IdeaId == ideaId).IsWishlist = value;
        IdeaDisplayContents.FirstOrDefault(i => i.Idea.IdeaId == ideaId).IsWishList = value;
    }

    protected void OnVoteClicked (int ideaId)
    {
        UserIdea userIdea = allUserIdeas.FirstOrDefault(l => l.IdeaId == ideaId);

        if (userIdea == null)
        {
            userIdea = new UserIdea() { IdeaId = ideaId, UserId = UserId, IsVoted = true };
            ideaService.CreateUserIdea(userIdea);
            allUserIdeas.Add(userIdea);
            //WishList.Add(userIdea);
            Idea idea = ideas.FirstOrDefault(i => i.IdeaId == ideaId);
            idea.Vote += 1;
            ideaService.UpdateIdea(idea);
            IdeaDisplayContents.FirstOrDefault(i => i.Idea.IdeaId == ideaId).IsVoted = true;
        }
        else
        {
            if (userIdea.IsVoted == false)
            {
                userIdea.IsVoted = true;
                ideaService.UpdateUserIdea(userIdea);
                Idea idea = ideas.FirstOrDefault(i => i.IdeaId == ideaId);
                idea.Vote += 1;
                ideaService.UpdateIdea(idea);
                UpdateVotedIdeaToDisplay(ideaId, userIdea.IsVoted);
            }
            else
            {
                userIdea.IsVoted = false;
                ideaService.UpdateUserIdea(userIdea);
                Idea idea = ideas.FirstOrDefault(i => i.IdeaId == ideaId);
                idea.Vote -= 1;
                ideaService.UpdateIdea(idea);
                UpdateVotedIdeaToDisplay(ideaId, userIdea.IsVoted);
            }
        }
    }

    protected void UpdateVotedIdeaToDisplay(int ideaId, bool value)
    {
        //WishList.FirstOrDefault(w => w.IdeaId == ideaId).IsVoted = value;
        allUserIdeas.FirstOrDefault(u => u.IdeaId == ideaId).IsVoted = value;
        IdeaDisplayContents.FirstOrDefault(i => i.Idea.IdeaId == ideaId).IsVoted = value;
    }

}
