# Welcome to Tcp.Net

* Tcp.Net is a TCP asynchronous networking library, small and lightweight written in C# .NET Core 3.1

# Features

* Smart message handler bindings

```csharp
[MessageHandler]
public static void HandleHelloMessage(HelloMessage message, MyClient client)
{
    Console.WriteLine("The client username is " + message.username);
}
```
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
    public override void OnFailToConnect(Exception ex)
    {
        Logger.Write("Unable to connect to server.");
    }
    public override void OnMessageReceived(Message message)
    {
        Logger.Write("Received " + message);
    }
    public override void OnSended(IAsyncResult result)
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

    public string username;
    public string mail;

    public HelloServerMessage(string userName,string mail)
    {
        this.username = userName;
        this.mail = mail;
    }
    public HelloServerMessage()
    {

    }
    public override void Deserialize(BinaryReader reader)
    {
        this.username = reader.ReadString();
        this.mail = reader.ReadString();
    }
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(username);
        writer.Write(mail);
    }

```

* Logging system

```csharp
Logger.Enable()
Logger.Disable()
Logger.DisableChannel(Channels channels)
Logger.EnableChannel(Channels channels)
```

* Do not hesitate to consult the sample project ``Tcp.Net.ExampleClient`` & ``Tcp.Net.ExampleServer`` for more details 