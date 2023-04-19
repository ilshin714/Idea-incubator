using FluentValidation;
using IdeaIncubatorBlazor.Models;
using IdeaIncubatorBlazor.Services.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace IdeaIncubatorBlazor.Views.Components;

public partial class UserRegistrationDialog
{

    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    [Inject] ISnackbar Snackbar { get; set; }
    public User inputUser = new User();

    MudForm? form;

    UserFluentValidator userValidator = new UserFluentValidator();


    protected override void OnInitialized()
    {
        inputUser.PhoneNumber = "";
        inputUser.UserName = "";
        inputUser.IsProfileVisible = false;
        inputUser.JoinDate = DateTime.Now;
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            bool doingRegistration = ((await ProtectedSessionStore.GetAsync<string>("DoRegister")).Value ?? "").Equals("1");
            if (doingRegistration)
            {
                inputUser.EmailAddress = (await ProtectedSessionStore.GetAsync<string>("RegisterEmail")).Value ?? "";
                inputUser.Password = userService.GeneratePassword();
                await ProtectedSessionStore.SetAsync("DoRegister", "");
                await ProtectedSessionStore.SetAsync("RegisterEmail", "");
                StateHasChanged();
            }
        }
    }

    private async Task Submit()
    {
        await form.Validate();
                
        if (form.IsValid)
        {
            inputUser.PhoneNumber = inputUser.PhoneNumber.Replace("-", "");
            await userService.CreateUserAsync(inputUser);
            Snackbar.Add("User Added!");
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    void Cancel() => MudDialog.Cancel();

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

            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(1, 100);
            RuleFor(x => x.PhoneNumber)
                .Matches("^(\\(?\\d{3}\\)?[-]?\\d{3}[-]?\\d{4})?$");
        }

        private async Task<bool> IsUniqueUserName(string userName)
        {
            // Simulates a long running http call
            await Task.Delay(2000);
            IdeaIncubatorDbContext dbContext = new();
            IUserService userService = new UserService(dbContext, null, null);
            return await userService.IsUniqueUserName(userName);
        }

        private async Task<bool> IsUniqueEmail(string email)
        {
            // Simulates a long running http call
            await Task.Delay(2000);
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
