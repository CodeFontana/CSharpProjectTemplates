namespace WebApi.SharedLibrary.Models;

public class ServiceResponseModel<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = false;
    public string Message { get; set; } = "";
}
