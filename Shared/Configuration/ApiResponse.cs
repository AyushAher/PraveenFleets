using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Shared.Configuration;

[DataContract]
[Serializable]
public class BaseApiResponse
{
    [DataMember] public string Version { get; set; }

    [DataMember] public int StatusCode { get; set; }

    [DataMember] public bool Succeeded { get; set; }

    public bool Failed => !Succeeded;

    [DataMember] public List<string> Messages { get; set; } = new();

    [JsonConstructor]
    public BaseApiResponse()
    {
    }


    public BaseApiResponse(int statusCode, string message = "")
    {
        if (string.IsNullOrWhiteSpace(message))
            SetData(statusCode);
        else
            SetData(statusCode, new()
            {
                message
            });
    }

    public BaseApiResponse(int statusCode, List<string> messages)
    {
        SetData(statusCode, messages);
    }

    private void SetData(int statusCode, List<string>? messages = null)
    {
        StatusCode = statusCode;
        Succeeded = StatusCode is >= 200 and < 300;
        Version = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        if (messages == null || messages.Count == 0)
            Messages = new()
            {
                Succeeded ? "Operation Successful" : "Operation Failed"
            };
        else
            Messages = messages;
    }

    public static BaseApiResponse Fail() => new(510, "Unknown LogError!!!");

    public static BaseApiResponse Fail(int logErrorCode, string message, ILogger? logger)
    {
        if (logger != null)
            logger.LogError(message);
        logErrorCode = logErrorCode == 0 ? 428 : logErrorCode;
        return new(logErrorCode, message);
    }

    public static BaseApiResponse Fail(string message, ILogger logger)
    {
        if (logger != null)
            logger.LogError(message);
        return new(428, message);
    }

    public static BaseApiResponse Fail(List<string> messages, ILogger logger)
    {
        if (logger != null)
        {
            int count = messages.Count;
            for (int index = 0; index < count; ++index)
                logger.LogError(messages[index]);
        }

        return new(428, messages);
    }

    public static async Task<BaseApiResponse> FailAsync(string message, ILogger logger) =>
        await Task.FromResult(Fail(message, logger));

    public static async Task<BaseApiResponse> FailAsync(List<string> messages, ILogger logger) =>
        await Task.FromResult(Fail(messages, logger));

    public static async Task<BaseApiResponse> FatalAsync(Exception? exception, ILogger? logger)
    {
        if (logger == null || exception == null)
            return new(510, "Sorry! Something went wrong, Please contact support if it repeats!");

        logger.LogCritical(exception.Message);
        logger.LogError(exception.StackTrace);

        return new(510, "Sorry! Something went wrong, Please contact support if it repeats!");
    }

    public static BaseApiResponse Success() => new(200);

    public static BaseApiResponse Success(string message) => new(200, message);

    public static BaseApiResponse Success(List<string> messages) => new(200, messages);

    public static async Task<BaseApiResponse> SuccessAsync() => await Task.FromResult(Success());

    public static async Task<BaseApiResponse> SuccessAsync(string message) => await Task.FromResult(Success(message));

    public static async Task<BaseApiResponse> SuccessAsync(List<string> messages) =>
        await Task.FromResult(Success(messages));
}



public sealed class ApiResponse<T> : BaseApiResponse
{
    [DataMember(EmitDefaultValue = false)]
    public T Data { get; set; }

    [JsonConstructor]
    public ApiResponse()
    {
    }

    public ApiResponse(int statusCode)
        : base(statusCode)
    {
    }

    public ApiResponse(int statusCode, string message)
        : base(statusCode, message)
    {
    }

    public ApiResponse(int statusCode, List<string> message)
        : base(statusCode, message)
    {
    }

    public ApiResponse(int statusCode, string? message, T? result)
      : base(statusCode, message)
    {
        Data = result;
    }

    public ApiResponse(int statusCode, List<string> messages, T? result)
      : base(statusCode, messages)
    {
        Data = result;
    }

    public static ApiResponse<T> Fail() => new(428);

    public static ApiResponse<T> Fail(string message, ILogger logger)
    {
        if (logger != null)
            logger.LogError(message);
        return new(428, message);
    }

    public static ApiResponse<T> Fail(List<string> messages, ILogger logger)
    {
        if (logger != null)
        {
            int count = messages.Count;
            for (int index = 0; index < count; ++index)
                logger.LogError(messages[index]);
        }
        return new(428, messages);
    }

    public static async Task<ApiResponse<T>> FailAsync() => await Task.FromResult(Fail());

    public static async Task<ApiResponse<T>> FailAsync(List<ValidationFailure> validationFailure, ILogger logger)
    {
        var logErrors = validationFailure.Select(e => $"({e.ErrorCode}) {e.PropertyName} => {e.ErrorMessage}").ToList();
        return await Task.FromResult(Fail(logErrors, logger));
    }

    public static async Task<ApiResponse<T>> FailAsync(string message, ILogger logger) => await Task.FromResult(Fail(message, logger));

    public static async Task<ApiResponse<T>> FailAsync(List<string> messages, ILogger logger) => await Task.FromResult(Fail(messages, logger));

    public static async Task<ApiResponse<T>> FatalAsync(Exception exception, ILogger logger)
    {
        logger.LogCritical(exception.Message);
        logger.LogError(exception.StackTrace);

        return new(510, "Sorry! Something went wrong, Please contact support if it repeats!");
    }

    public static ApiResponse<T> Success() => new(200);

    public static ApiResponse<T> Success(string message) => new(200, message);

    public static ApiResponse<T> Success(T data) => new(200, string.Empty, data);

    public static ApiResponse<T> Success(T data, string message) => new(200, message, data);

    public static ApiResponse<T> Success(T data, List<string> messages) => new(200, messages, data);

    public static async Task<ApiResponse<T>> SuccessAsync() => await Task.FromResult(Success());

    public static async Task<ApiResponse<T>> SuccessAsync(string message) => await Task.FromResult(Success(message));

    public static async Task<ApiResponse<T>> SuccessAsync(T data) => await Task.FromResult(Success(data));

    public static async Task<ApiResponse<T>> SuccessAsync(T data, string message) => await Task.FromResult(Success(data, message));

    public static async Task<ApiResponse<T>> SuccessAsync(T data, List<string> messages) => await Task.FromResult(Success(data, messages));
}