namespace WebApi.SharedLibrary.Models;

public sealed class ServiceResponseModel<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = false;
    public string? Message { get; set; }
}
