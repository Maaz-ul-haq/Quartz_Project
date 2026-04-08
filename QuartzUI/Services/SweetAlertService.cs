using Microsoft.JSInterop;

namespace QuartzUI.Services
{
    public class SweetAlertService
    {
        private readonly IJSRuntime _js;

        public SweetAlertService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task Success(string message, string title = "Success")
        {
            await _js.InvokeVoidAsync("showAlert", "success", title, message);
        }

        public async Task Error(string message, string title = "Error")
        {
            await _js.InvokeVoidAsync("showAlert", "error", title, message);
        }

        public async Task Warning(string message, string title = "Warning")
        {
            await _js.InvokeVoidAsync("showAlert", "warning", title, message);
        }

        public async Task Info(string message, string title = "Info")
        {
            await _js.InvokeVoidAsync("showAlert", "info", title, message);
        }
    }
}
