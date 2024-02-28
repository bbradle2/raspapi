namespace first_test;

public class BBrian
{
    private readonly string _message;
    public BBrian(string message) 
    {
        _message = message;
    }
    public void Go()
    {
        Console.WriteLine(_message);
    }
   
}
