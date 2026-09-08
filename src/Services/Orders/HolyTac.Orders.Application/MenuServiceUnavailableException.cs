namespace HolyTac.Orders.Application;

/// <summary>
/// Se lanza cuando Orders no puede consultar el microservicio de Menú por una falla de
/// infraestructura (caído, timeout, circuito abierto), a diferencia de un 404 real que
/// significa que el producto simplemente no existe.
/// </summary>
public class MenuServiceUnavailableException : Exception
{
    public MenuServiceUnavailableException(string message) : base(message) { }

    public MenuServiceUnavailableException(string message, Exception innerException) : base(message, innerException) { }
}
