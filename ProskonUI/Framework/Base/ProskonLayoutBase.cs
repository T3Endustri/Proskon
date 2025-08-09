using Microsoft.AspNetCore.Components;
using ProskonUI.Services.Authorization;

namespace ProskonUI.Framework.Base;

public class ProskonLayoutBase : LayoutComponentBase
{
    [Inject] protected ICurrentUserService CurrentUser { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await CurrentUser.EnsureUserLoadedAsync();
    }
}

