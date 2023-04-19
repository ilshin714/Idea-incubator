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
using Microsoft.Extensions.Options;
using MudBlazor;

namespace IdeaIncubatorBlazor.Views.Pages;

public partial class Login : ComponentBase
{

    [Inject] 
    IDialogService Dialog { get; set; }
    
    [Inject] 
    ISnackbar Snackbar { get; set; }

    [Parameter]
    public string UserName { get; set; }

    [Parameter]
    public string Password { get; set; }

    protected string UseMicrosoftIdentity = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build()
        .GetValue<string>("UseMicrosoftIdentity");

    public MudTextField<string> UserNameTextBox { get; set; }

    public MudTextField<string> PasswordTextBox { get; set; }

    public ComponentState State { get; set; }

    private bool userLoggedIn = false;

    DialogOptions maxWidthForIdeaCreation
      = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

    protected override void OnInitialized()
    {
        this.State = ComponentState.Conetent;
        UserName = "";
        Password = "";
    }

    protected async Task DoLogin()
    {
        int result = userService.LoginUser(UserName, Password);
        if (result == 0)
        {
            Snackbar.Add("Login is invalid. Please try again.");
        }
        else
        {
            User tempUser = userService.GetUser(result);
            Snackbar.Add("User id " + result + " logged in");
            await ProtectedSessionStore.SetAsync("UserId", result.ToString());
            RedirectPage();
        }
    }

    protected async void RedirectPage()
    {
        string previousPage = (await ProtectedSessionStore.GetAsync<string>("PreviousPage")).Value ?? "";
        switch (previousPage)
        {
            case "profile":
                Navigation.NavigateTo("/profile", true);
                break;
            default:
                Navigation.NavigateTo("/", true);
                break;
        }
        try { 
            await ProtectedSessionStore.SetAsync("PreviousPage", string.Empty);
        } catch (TaskCanceledException )
        {
            // Do nothing
        }
    }

    protected void OpenUserRegistrationDialog(DialogOptions options)
        => Dialog.Show<UserRegistrationDialog>("Register Your Account", options);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            bool doingRegistration = ((await ProtectedSessionStore.GetAsync<string>("DoRegister")).Value ?? "").Equals("1");
            if (doingRegistration)
            {
                Dialog.Show<UserRegistrationDialog>("Register Your Account", maxWidthForIdeaCreation);
            } else
            {
                bool result = false;
                string tempId = (await ProtectedSessionStore.GetAsync<string>("UserId")).Value ?? "";
                if (tempId != "" && tempId != "0")
                {
                    result = true;
                }
                userLoggedIn = result;
                StateHasChanged();
            }
        }
    }

    protected void DoMicrosoftLogin()
    {
        ProtectedSessionStore.SetAsync("MicrosoftSSOState", "1");
        Navigation.NavigateTo("/MicrosoftIdentity/Account/SignIn/", true);
    }


    protected void Logout()
    {
        ProtectedSessionStore.SetAsync("UserId", null);
        Navigation.NavigateTo("/", true);
    }

}
