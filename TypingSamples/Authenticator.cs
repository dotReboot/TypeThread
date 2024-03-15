public class Authenticator
{
    private string _username;
    private string _password;

    public Authenticator(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public bool Authenticate(string enteredUsername, string enteredPassword)
    {
        return enteredUsername == _username && enteredPassword == _password;
    }
}