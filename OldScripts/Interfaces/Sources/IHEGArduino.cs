public interface IHEGArduino
{
    public void Connect(string portName);
    public void Write(string line);
    public string Read();
    public void Start();
    public void Stop();
}

// Custom exception for interface
public class SerialDisconnectException : Exception
{
    public SerialDisconnectException()
    {
    }

    public SerialDisconnectException(string message)
        : base(message)
    {
    }

    public SerialDisconnectException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
