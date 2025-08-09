using Microsoft.AspNetCore.Components;
using ProskonUI.Services;

namespace ProskonUI.Framework.Base;

public abstract class ProskonComponentBase : ComponentBase
{
    [Inject] protected ToastService Toast { get; set; } = default!;
}
