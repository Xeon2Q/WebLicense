using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace WebLicense.Client.Auxiliary
{
    public sealed class JsLog
    {
        private readonly IJSRuntime jsr;

        public JsLog(IJSRuntime jsr)
        {
            this.jsr = jsr ?? throw new ArgumentNullException(nameof(jsr));
        }

        public async Task LogAsync(string message)
        {
            await jsr.InvokeVoidAsync("console.log", message);
        }

        public void Log(string message)
        {
            Task.Run(async () => await LogAsync(message));
        }
    }
}