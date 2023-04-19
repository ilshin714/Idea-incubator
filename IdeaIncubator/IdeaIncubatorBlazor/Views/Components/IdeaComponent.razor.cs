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
using MudBlazor;

namespace IdeaIncubatorBlazor.Views.Components;

public partial class IdeaComponent : ComponentBase
{
    [Inject] 
    IDialogService Dialog { get; set; }

    [Inject]
    NavigationManager UriHelper { get; set; }

    [Parameter]
    public Idea ParamIdea { get; set; }

    [Parameter]
    public int UserId { get; set; }

    [Parameter]
    public Color FavoriteButtonColor { get; set; }

    [Parameter]
    public Color VotedColor { get; set; }

    [Parameter]
    public EventCallback<int> OnFavoriteButtonClicked { get; set; }

    [Parameter]
    public EventCallback<int> OnVoteButtonClicked { get; set; }

    public User Provider { get; set; }

    public List<UserIdeaRole> Participants { get; set; }
    
    public int CollaboratorsCount { get; set; }

    DialogOptions maxWidthForIdeaDetail;

    private Idea _idea;
    private Idea _Idea
    {
        get => _idea;
        set
        {
            if (_idea != value)
            {
                _idea = value;
                ParamIdeaUpdated();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        maxWidthForIdeaDetail = new DialogOptions() { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
    }

    protected void OpenIdeaDetailDialog()
    {
        DialogParameters parameters = new();
        parameters.Add("SelectedIdea", ParamIdea);
        parameters.Add("Provider", Provider);
        parameters.Add("Participants", Participants);
        parameters.Add("CollaboratorsCount", CollaboratorsCount);

        Dialog.Show<IdeaDetailDialog>(ParamIdea.Name, parameters, maxWidthForIdeaDetail);
    }

    protected override void OnParametersSet()
    {
        if (_Idea != ParamIdea) _Idea = ParamIdea;
    }

    private void ParamIdeaUpdated()
    {
        Provider = ideaService.GetProviderOfIdea(ParamIdea.IdeaId).Result;
        Participants = ideaService.GetCollaboratorsOfIdea(ParamIdea.IdeaId).Result;
        CollaboratorsCount = Participants.Count;
    }

    private async Task ClickFavorite()
    {
        if (UserId == 0)
        {
            UriHelper.NavigateTo("login");
            return;
        }

        await OnFavoriteButtonClicked.InvokeAsync(ParamIdea.IdeaId);
    }

    private async Task ClickVote()
    {
        if (UserId == 0)
        {
            UriHelper.NavigateTo("login");
            return;
        }

        await OnVoteButtonClicked.InvokeAsync(ParamIdea.IdeaId);
    }
    
}
