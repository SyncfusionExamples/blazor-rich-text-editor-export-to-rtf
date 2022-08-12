using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;
namespace RTF_Export
{
  
  
    public static class SampleInterop
    {

        public static ValueTask<T> SaveAs<T>(this IJSRuntime JSRuntime, string filename)
        {
            try
            {
                var filePath = Path.Combine(System.IO.Directory.GetCurrentDirectory() + "\\Sample.rtf");
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return JSRuntime.InvokeAsync<T>("saveAsFile", filename, Convert.ToBase64String(fileBytes));
            }
            catch (Exception e)
            {
                return SampleInterop.LogError<T>(JSRuntime, e, "");
            }
        }

        public static ValueTask<T> LogError<T>(IJSRuntime jsRuntime, Exception e, string message = "")
        {

            ErrorMessage error = new ErrorMessage();
            error.Message = message + e.Message;
            error.Stack = e.StackTrace;
            if (e.InnerException != null)
            {
                error.Message = message + e.InnerException.Message;
                error.Stack = e.InnerException.StackTrace;
            }
            return jsRuntime.InvokeAsync<T>(
                "jsInterop.throwError", error);
        }

        
    }
    public class ErrorMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("stack")]
        public string Stack { get; set; }
    }
}
