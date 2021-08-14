# Welcome to Gears

* Gears is a small and lightweight networking library over TCP, written in C# .NET Core 3.1

# Features

* High level socket API

```csharp

static void Main(string[] args)
{
    MyClient client = new MyClient();
    client.Connect("127.0.0.1", 500);
}
public class MyClient : Client
{
    public override void OnConnect()
    {
        Logger.Write("Connected to server");
    }
    public override void OnDisconnect()
    {
        Logger.Write("Connection closed.");
    }
    public override void OnError(Exception ex)
    {
        Logger.Write("Unable to connect to server.");
    }
    public override void OnReceive(Message message)
    {
        Logger.Write("Received " + message);
    }
    public override void OnSend(IAsyncResult result)
    {
        Logger.Write("Sended " + result.AsyncState);
    }
}

```

* Clean and modular protocol implementation

```csharp

public class HelloServerMessage : Message
{
    public const ushort Id = 1;
    public override ushort MessageId => Id;

    public string Username;
    public string Mail;

    public HelloServerMessage(string userName,string mail)
    {
        this.Username = userName;
        this.Mail = mail;
    }
    public HelloServerMessage()
    {

    }

```
* Smart message handler bindings

```csharp
[MessageHandler]
public static void HandleHelloMessage(HelloMessage message, MyClient client)
{
    // handle HelloMessage
}
```
* Logging system

```csharp
Logger.Enable()
Logger.Disable()
Logger.DisableChannel(Channels channels)
Logger.EnableChannel(Channels channels)
```

* Do not hesitate to consult the sample project ``Gears.ExampleClient`` & ``Gears.ExampleServer`` for more details 