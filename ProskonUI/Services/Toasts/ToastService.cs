namespace ProskonUI.Services.Toasts;

public enum ToastType { Info, Success, Warning, Error }

public record ToastRequest(
                ToastType Type,
                string Title,
                string Content,
                int? TimeOutMs = null
);

public class ToastService
{
    // Layout'taki ToastHost buraya abone olur
    public event Func<ToastRequest, Task>? OnShow;
    public event Func<Task>? OnHideAll;

    public Task Info(string title, string message, int? ms = null) => Show(new(ToastType.Info, title, message, ms));
    public Task Success(string title, string message, int? ms = null) => Show(new(ToastType.Success, title, message, ms));
    public Task Warning(string title, string message, int? ms = null) => Show(new(ToastType.Warning, title, message, ms));
    public Task Error(string title, string message, int? ms = null) => Show(new(ToastType.Error, title, message, ms));

    public Task HideAll() => OnHideAll?.Invoke() ?? Task.CompletedTask;

    private Task Show(ToastRequest req) => OnShow?.Invoke(req) ?? Task.CompletedTask;
}
