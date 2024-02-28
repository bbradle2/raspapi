namespace first_test;

public class ABrian
{
    private readonly string _message;
    public ABrian(string message)
    {
        _message = message;
    }
    public void Go()
    {
        Console.WriteLine(_message);
    }

}