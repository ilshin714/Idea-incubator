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
using IdeaIncubatorBlazor.Views.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;

namespace IdeaIncubatorBlazor.Views.Pages;

public partial class Profile : ComponentBase
{
    [Inject] 
    ProtectedSessionStorage ProtectedSessionStore { get; set; }

    [Inject] 
    NavigationManager UriHelper { get; set; }

    [Inject] 
    IUserService userService { get; set; }

    [Inject] 
    IIdeaService ideaService { get; set; }
    
    [Inject]
    IDialogService Dialog { get; set; }


    User? User { get; set; }

    public User Provider { get; set; }

    public List<UserIdeaRole> Participants { get; set; }

    public int CollaboratorsCount { get; set; }

    DialogOptions maxWidthForIdeaDetail;

    private List<Idea> ownIdeas;
    private List<Idea> collIdeas;
    private List<Idea> wishList;

    protected override async Task OnInitializedAsync()
    {
        maxWidthForIdeaDetail = new DialogOptions() { MaxWidth = MaxWidth.Large, FullWidth = true };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if ((await ProtectedSessionStore.GetAsync<string>("UserId")).Value == null)
            {
                await ProtectedSessionStore.SetAsync("PreviousPage", "profile");
                UriHelper.NavigateTo("login");
            }
            else
            {
                string userId = (await ProtectedSessionStore.GetAsync<string>("UserId")).Value ?? "";
                int userIdInt = Convert.ToInt32(userId);
                User = userService.GetUser(userIdInt);
                if (User != null)
                {
                    ownIdeas = ideaService.GetOwnIdeas(userIdInt);
                    collIdeas = ideaService.GetCollabIdeas(userIdInt);
                    wishList = ideaService.GetWishList(userIdInt);
                }                
                StateHasChanged();
            }
        }
    }

    protected void ShowIdeaDetail(Idea idea)
    {
        Provider = ideaService.GetProviderOfIdea(idea.IdeaId).Result;
        Participants = ideaService.GetCollaboratorsOfIdea(idea.IdeaId).Result;
        CollaboratorsCount = Participants.Count;

        DialogParameters parameters = new();
        parameters.Add("SelectedIdea", idea);
        parameters.Add("Provider", Provider);
        parameters.Add("Participants", Participants);
        parameters.Add("CollaboratorsCount", CollaboratorsCount);

        Dialog.Show<IdeaDetailDialog>(idea.Name, parameters, maxWidthForIdeaDetail);
    }
}
