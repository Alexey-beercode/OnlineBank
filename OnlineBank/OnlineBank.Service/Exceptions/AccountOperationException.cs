namespace OnlineBank.Service.Exceptions;

public class AccountOperationException:Exception
{
    public AccountOperationException(string message) : base(message)
    {
        
    }
}