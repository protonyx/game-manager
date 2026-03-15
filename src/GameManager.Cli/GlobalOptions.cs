using System.CommandLine;

namespace GameManager.Cli;

public static class GlobalOptions
{
    public static Option<string> ServerUrlOption = new Option<string>("--server-url")
    {
        Description = "The URL of the GameManager server to connect to",
        DefaultValueFactory = _ => "http://localhost:5000",
        Required = true
    };

    public static Option<string> UsernameOption = new Option<string>("--username")
    {
        Description = "The username to use for authentication",
        DefaultValueFactory = _ => "admin",
        Required = false
    };

    public static Option<string> PasswordOption = new Option<string>("--password")
    {
        Description = "The password to use for authentication",
        DefaultValueFactory = _ => "password",
        Required = false
    };
}