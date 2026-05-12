using System.Text.Json;
using Refit;

namespace RetailCore.Customer.Services;

public static class ApiErrorHelper
{
    public static string ExtractMessage(Exception ex)
    {
        if (ex is ApiException apiException)
        {
            if (!string.IsNullOrWhiteSpace(apiException.Content))
            {
                try
                {
                    using var document = JsonDocument.Parse(apiException.Content);

                    if (document.RootElement.TryGetProperty("message", out var message))
                        return message.GetString() ?? "Something went wrong.";
                }
                catch
                {
                    // ignored
                }
            }

            return apiException.Message;
        }

        return ex.Message;
    }
}
