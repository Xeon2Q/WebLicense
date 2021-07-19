using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace WebLicense.Client.Auxiliary
{
    public sealed class JsUtils
    {
        public IJSRuntime JSR { get; }

        #region C-tor | Properties

        public JsUtils(IJSRuntime jsr)
        {
            JSR = jsr ?? throw new ArgumentNullException(nameof(jsr));
        }

        #endregion

        #region Logging methods

        public async Task LogAsync(string message)
        {
            await JSR.InvokeVoidAsync("console.log", message);
        }

        public void Log(string message)
        {
            Task.Run(async () => await LogAsync(message));
        }

        #endregion

        #region Bootstrap functions

        public async Task ToggleModalWindowAsync(string windowName, bool state, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(windowName)) return;

            await JSR.InvokeVoidAsync("toggleModalWindow", cancellationToken, windowName, state);
        }

        public void ToggleModalWindow(string windowName, bool state)
        {
            Task.Run(async () => await ToggleModalWindowAsync(windowName, state));
        }

        #endregion
    }
}