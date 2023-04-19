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
using IdeaIncubatorBlazor.Services.Ideas;
using IdeaIncubatorBlazor.Services.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using MudBlazor;
namespace IdeaIncubatorBlazor.Views.Components;

public partial class MyAccount : ComponentBase
{
    [Inject]
    IUserService userService { get; set; }

    [Inject]
    ProtectedSessionStorage ProtectedSessionStore { get; set; }

    [Inject]
    NavigationManager UriHelper { get; set; }

    [Inject] ISnackbar Snackbar { get; set; }

    [Parameter]
    public User User { get; set; }

    public String newPassword { get; set; }

    MudForm? form;

    protected override async Task OnInitializedAsync()
    {
        newPassword = "";
        if (User == null)
        {
            await ProtectedSessionStore.SetAsync("PreviousPage", "profile");
            UriHelper.NavigateTo("login");
        }
    }

    private async Task Submit()
    {
        Boolean updatePassword = false;
        
        await form.Validate();

        if (form.IsValid)
        {
            if (!newPassword.Equals(""))
            {
                User.Password= newPassword;
                updatePassword = true;
            }
            Boolean updateGood = await userService.UpdateUser(User, updatePassword);

            if (updateGood)
            {
                Snackbar.Add("User Profile Updated Successfully");
            } else
            {
                Snackbar.Add("There was an internal error, please try again.");
            }
        }
    }

    private async Task DeleteUser()
    {
        // Note: Instead of deleting a user we really should be deactivating them which would be renaming their username to "-Inactive-", and setting a bit in the db to disable login
        if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you wish to delete your account '{User.UserName}'?"))
            return;
        await userService.DeleteUser(User);
        await ProtectedSessionStore.SetAsync("UserId", "");
        Snackbar.Add("Your user account has been deleted");
        StateHasChanged();
        UriHelper.NavigateTo("/", true);
    }

    private async Task LogoutUser()
    {
        await ProtectedSessionStore.SetAsync("UserId", "");
        if (((await ProtectedSessionStore.GetAsync<string>("MicrosoftSSOState")).Value ?? "") == "2")
        {
            await ProtectedSessionStore.SetAsync("MicrosoftSSOState", "");
            UriHelper.NavigateTo("/MicrosoftIdentity/Account/SignOut/", true);
        }
        Snackbar.Add("You have been logged out");
        StateHasChanged();
        UriHelper.NavigateTo("/", true);
    }

    private async Task Cancel()
    {
        String backDestination = (await ProtectedSessionStore.GetAsync<string>("PreviousPage")).Value ?? "";
        UriHelper.NavigateTo(backDestination);
    }

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="OrderModel"></typeparam>
    public class UserFluentValidator : AbstractValidator<User>
    {
        public UserFluentValidator()
        {
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(1, 20)
                .MustAsync(async (value, cancellationToken) => await IsUniqueUserName(value)).WithMessage("User Name already exists. Please choose a different name.");

            RuleFor(x => x.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(async (value, cancellationToken) => await IsUniqueEmail(value)).WithMessage("This email is already registered.");

            RuleFor(x => x.PhoneNumber)
                .Matches("^(\\(?\\d{3}\\)?[-]?\\d{3}[-]?\\d{4})?$");
        }

        private async Task<bool> IsUniqueUserName(string userName)
        {
            // Simulates a long running http call
            IdeaIncubatorDbContext dbContext = new();
            IUserService userService = new UserService(dbContext, null, null);
            return await userService.IsUniqueUserName(userName);
        }

        private async Task<bool> IsUniqueEmail(string email)
        {
            // Simulates a long running http call
            IdeaIncubatorDbContext dbContext = new();
            IUserService userService = new UserService(dbContext, null, null);
            return await userService.IsUniqueEmail(email);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<User>.CreateWithOptions((User)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

}
