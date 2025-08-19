using _02_Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using ProskonUI.Components.Manage.Grids;
using ProskonUI.Services.Toasts;

namespace ProskonUI.Framework.Base;

public abstract class ProskonComponentBase : ComponentBase, IDisposable, IAsyncDisposable
{
    // --- DI (component ortakları)
    [Inject] protected ToastService Toast { get; set; } = default!;
    [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IUserService UserService { get; set; } = default!;
    [Inject] protected IClaimService ClaimService { get; set; } = default!;
    [Inject] protected IModuleService ModuleService { get; set; } = default!;
    [Inject] protected ICurrentUser CurrentUser { get; set; } = default!;
    [Inject] protected ILogService LogService { get; set; } = default!;
    [Inject] protected IAuthService AuthService { get; set; } = default!;
    [Inject] protected ILogger<RolesGrid> Logger { get; set; } = default!;

    // --- Yaşam döngüsü kaynakları
    private readonly List<IDisposable> _disposables = [];
    private readonly List<IAsyncDisposable> _asyncDisposables = [];   // HubConnection vb.
    private readonly List<CancellationTokenSource> _ctss = [];
    private readonly List<Timer> _timers = [];
    private bool _disposed;

    // ---------------- Component lifecycle ----------------
    protected override async Task OnInitializedAsync()
    {
        try { await CurrentUser.EnsureLoadedAsync(); } catch { /* ignore */ }

        AuthStateProvider.AuthenticationStateChanged += OnAuthStateChanged;
        TrackUnsubscribe(() => AuthStateProvider.AuthenticationStateChanged -= OnAuthStateChanged);
    }

    private async void OnAuthStateChanged(Task<AuthenticationState> task)
    {
        if (_disposed) return;
        try
        {
            _ = await task;
            await CurrentUser.EnsureLoadedAsync();
        }
        catch { /* ignore */ }
        finally
        {
            try { await InvokeAsync(StateHasChanged); } catch { /* ignore */ }
        }
    }

    // ---------------- Helpers ----------------
    protected async Task<bool> SafeAsync(Func<Task> action, string? successToast = null, string? errorToast = null)
    {
        try
        {
            await action();
            if (!string.IsNullOrWhiteSpace(successToast))
                await Toast.Success("Başarılı", successToast);
            await InvokeAsync(StateHasChanged);
            return true;
        }
        catch (Exception ex)
        {
            LogService.Error(GetType().Name, ex.Message, ex);
            if (!string.IsNullOrWhiteSpace(errorToast))
                await Toast.Error("Hata", errorToast);
            else
                await Toast.Error("Hata", ex.Message);
            await InvokeAsync(StateHasChanged);
            return false;
        }
    }

    protected async Task<(bool ok, T? value)> SafeAsync<T>(Func<Task<T>> action, string? errorToast = null)
    {
        try
        {
            var v = await action();
            await InvokeAsync(StateHasChanged);
            return (true, v);
        }
        catch (Exception ex)
        {
            LogService.Error(GetType().Name, ex.Message, ex);
            if (!string.IsNullOrWhiteSpace(errorToast))
                await Toast.Error("Hata", errorToast);
            else
                await Toast.Error("Hata", ex.Message);
            await InvokeAsync(StateHasChanged);
            return (false, default);
        }
    }

    protected T Track<T>(T disposable) where T : IDisposable
    {
        _disposables.Add(disposable);
        return disposable;
    }

    protected T TrackAsync<T>(T disposable) where T : IAsyncDisposable
    {
        _asyncDisposables.Add(disposable);
        return disposable;
    }

    protected IDisposable TrackUnsubscribe(Action unsubscribe)
        => Track(new ActionDisposable(unsubscribe));

    protected CancellationTokenSource CreateLinkedTokenSource(params CancellationToken[] tokens)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(tokens);
        _ctss.Add(cts);
        return cts;
    }

    protected System.Threading.Timer StartTimer(TimeSpan dueTime, TimeSpan period, TimerCallback callback)
    {
        var timer = new System.Threading.Timer(callback, null, dueTime, period);
        _timers.Add(timer);
        return timer;
    }

    protected void SubscribeLocationChanged(EventHandler<LocationChangedEventArgs> handler)
    {
        Navigation.LocationChanged += handler;
        TrackUnsubscribe(() => Navigation.LocationChanged -= handler);
    }

    protected Task SafeStateHasChangedAsync() => InvokeAsync(StateHasChanged);

    // ---------------- Dispose ----------------

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            foreach (var t in _timers) t.Dispose();
            _timers.Clear();

            foreach (var d in _disposables) d.Dispose();
            _disposables.Clear();

            foreach (var c in _ctss)
            {
                try { c.Cancel(); } catch { /* ignore */ }
                c.Dispose();
            }
            _ctss.Clear();

            OnDispose();
        }
        finally
        {
            // Senkron dispose yolunda finalize'ı bastır
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Async disposable kaynakları da düzgün kapatır ve finalize'ı bastırır.
    /// Bu çağrı, türeyen tipler finalizer eklese bile ayrıca IDisposable'ı
    /// override etmeye gerek bırakmaz.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        // Önce senkron kaynağı temizle
        Dispose();

        // Sonra async disposable’lar (HubConnection vs.)
        foreach (var ad in _asyncDisposables)
        {
            try { await ad.DisposeAsync(); } catch { /* ignore */ }
        }
        _asyncDisposables.Clear();

        // Async dispose yolunda da finalize'ı bastır
        GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose() { }

    private sealed class ActionDisposable(Action action) : IDisposable
    {
        private Action? _action = action;
        public void Dispose() => Interlocked.Exchange(ref _action, null)?.Invoke();
    }
}
