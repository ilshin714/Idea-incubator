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

using FluentValidation;
using IdeaIncubatorBlazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace IdeaIncubatorBlazor.Views.Components;

public partial class IdeaCreationDialog : ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public EventCallback<bool> IsUpdated { get; set; }

    [Inject] ISnackbar Snackbar { get; set; }

    IList<IBrowserFile> files = new List<IBrowserFile>();

    MudForm? form = new MudForm();

    IdeaFluentValidator ideaValidator = new IdeaFluentValidator();

    public Idea inputIdea = new Idea();

    protected override void OnInitialized()
    {
        inputIdea.Name = "";
        inputIdea.Keywords = "";
        inputIdea.Vote = 0;
        inputIdea.Description = "";
        inputIdea.Status = (int)IdeaStatusEnum.Initial;
        base.OnInitialized();
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            string userId = (await ProtectedSessionStore.GetAsync<string>("UserId")).Value ?? "";

            int userIdInt = Convert.ToInt32(userId);

            inputIdea.CreatedDate = DateTime.Now;

            Idea idea = await ideaService.CreateIdeaAsync(inputIdea, userIdInt);

            MudDialog.Close(DialogResult.Ok(idea));
            Snackbar.Add("New item is created.");

            CloseDialog();
        }
    }



    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="OrderModel"></typeparam>
    public class IdeaFluentValidator : AbstractValidator<Idea>
    {
        public IdeaFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 100);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(1, 300);
        }


        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<Idea>.CreateWithOptions((Idea)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }



    void CloseDialog() => MudDialog.Close(DialogResult.Ok(true));

    void Cancel() => MudDialog.Cancel();

}
