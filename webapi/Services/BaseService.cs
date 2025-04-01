namespace URL_ShortenerAPI.Services;

public class BaseService<TModel> where TModel : class
{
    protected string FailedToActionStr(string modelName, string action, string? message = null)
    {
        string result = $"Failed to {action} {modelName}";

        if (!string.IsNullOrEmpty(message))
            result += $": {message}";

        return result + ".";
    }

    protected string FailedToActionStr(string modelName, string action, Exception ex)
        => FailedToActionStr(modelName, action, ex.InnerException?.Message ?? ex.Message);
}
