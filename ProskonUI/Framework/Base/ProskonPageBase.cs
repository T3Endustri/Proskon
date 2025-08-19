using _02_Application.Interfaces;
using Microsoft.AspNetCore.Components;
using ProskonUI.Services.Toasts;

namespace ProskonUI.Framework.Base;

public abstract class ProskonPageBase : ProskonLayoutBase
{
    [Inject] public IRoleService RoleService { get; set; } = default!;
    [Inject] public IUserService UserService { get; set; } = default!;

    [Inject] protected ToastService Toast { get; set; } = default!;

    protected Task Info(string t, string m, int? ms = null) => Toast.Info(t, m, ms);
    protected Task Success(string t, string m, int? ms = null) => Toast.Success(t, m, ms);
    protected Task Warning(string t, string m, int? ms = null) => Toast.Warning(t, m, ms);
    protected Task Error(string t, string m, int? ms = null) => Toast.Error(t, m, ms);
}
