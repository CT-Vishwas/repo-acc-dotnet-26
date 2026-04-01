namespace Inventory.Core.DTOs.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set;} = false;
    public string Message { get; set;} = string.Empty;
    public T Data {get; set;} 
    public List<string> Errors {get; set;} = new List<string>();

    public ApiResponse(){}
    public ApiResponse(bool success, string message, T data, List<string> errors)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
    }
}