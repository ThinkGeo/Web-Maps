using Microsoft.AspNetCore.Components;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities;

/// <summary>
/// Provides helpers for wiring Blazor event callbacks that should not trigger an automatic component re-render.
/// </summary>
/// <remarks>
/// Useful for high-frequency or side-effect-only handlers where the callback work does not immediately require UI updates.
/// </remarks>
public static class NonRenderingEventHandlers
{
    /// <summary>
    /// Wraps an asynchronous value callback so Blazor does not automatically call <c>StateHasChanged</c> after invocation.
    /// </summary>
    /// <typeparam name="TValue">The event argument type.</typeparam>
    /// <param name="callback">The callback to execute for the event argument.</param>
    /// <returns>A delegate suitable for event binding that suppresses implicit re-rendering.</returns>
    public static Func<TValue, Task> AsNonRenderingEventHandler<TValue>(Func<TValue, Task> callback) =>
        new AsyncReceiver<TValue>(callback).InvokeAsync;

    /// <summary>
    /// Internal event receiver that executes callbacks while implementing <see cref="IHandleEvent"/>
    /// to bypass Blazor's default render scheduling behavior.
    /// </summary>
    /// <typeparam name="TValue">The callback argument type.</typeparam>
    private sealed class AsyncReceiver<TValue>(Func<TValue, Task> callback) : IHandleEvent
    {
        /// <summary>
        /// Invokes the wrapped callback with the provided argument.
        /// </summary>
        /// <param name="value">The callback argument.</param>
        /// <returns>The callback task.</returns>
        public Task InvokeAsync(TValue value) =>
            callback(value);

        /// <summary>
        /// Handles the event callback work item without enqueuing an automatic component re-render.
        /// </summary>
        /// <param name="item">The callback work item supplied by Blazor.</param>
        /// <param name="arg">The event argument passed by Blazor.</param>
        /// <returns>The task returned by the callback work item.</returns>
        public Task HandleEventAsync(EventCallbackWorkItem item, object? arg) =>
            item.InvokeAsync(arg);
    }
}
