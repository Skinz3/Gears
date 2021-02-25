# Welcome to Tcp.Net

* Tcp.Net is a TCP asynchronous networking library written in C# .NET Core 3.1

# Example of usage

* Handle message easily 

```csharp
[MessageHandler]
public static void HandleHelloMessage(HelloMessage message,MyClient client)
{
    Console.WriteLine("The client username is " + message.username);
}
```