# Welcome to Tcp.Net

* Tcp.Net is a TCP asynchronous networking library written in C# .NET Core 3.1

# Features

* Automatic message handler - method binding

```csharp
[MessageHandler]
public static void HandleHelloMessage(HelloMessage message, MyClient client)
{
    Console.WriteLine("The client username is " + message.username);
}
```
* High level socket API

```csharp

static void main(string[] args)
{
    MyClient client = new MyClient();
    client.Connect("127.0.0.1", 500);
}
public class MyClient : Client
{
    public override void OnConnected()
    {
        Console.WriteLine("Connected to server");
    }

    public override void OnConnectionClosed()
    {
        Console.WriteLine("Connection closed by server.");
    }
    public override void OnDisconnected()
    {
        Console.WriteLine("Connection closed by client.");
    }
    public override void OnFailToConnect(Exception ex)
    {
        Console.WriteLine("Unable to connect to server.");
    }
    public override void OnMessageReceived(Message message)
    {
          Console.WriteLine("Received " + message);
    }
    public override void OnSended(IAsyncResult result)
    {
        Logger.Write("Sended " + result.AsyncState);
    }
}

```