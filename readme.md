# Welcome to Gears

* Gears is a small and lightweight networking library over TCP, written in C# .NET Core 3.1

# Features

* High level socket API

```csharp

static void Main(string[] args)
{
    ProtocolManager.Initialize(Assembly.GetAssembly(typeof(HelloServerMessage)), Assembly.GetExecutingAssembly());
    MyClient client = new MyClient();
    client.Connect("127.0.0.1", 500);
}
public class MyClient : Client
{
    public override void OnConnect();
    public override void OnDisconnect();
    public override void OnError(Exception ex);
    public override void OnReceive(Message message);
    public override void OnSend(IAsyncResult result);
}

```

* Clean and modular protocol implementation

```csharp

public class HelloServerMessage : Message
{
    public const ushort Id = 1;
    public override ushort MessageId => Id;

    public string Content;

    public HelloServerMessage(string content)
    {
        this.Content = content;
    }
    public HelloServerMessage()
    {
        // Empty constructor is for json deserialization
    }
}
```
* Smart message handler bindings

```csharp

/*
This code is client side
*/
static void Main(string[] args) 
{
    ProtocolManager.Initialize(Assembly.GetAssembly(typeof(HelloServerMessage)), Assembly.GetExecutingAssembly());
    
    MyClient client = new MyClient();

    client.Connect("127.0.0.1",500);

    client.OnConnected += () =>
    {
        client.Send(new HelloMessage("Hi server!"));
    }
}

/*
This code is server side
*/
[MessageHandler]
public static void HandleHelloMessage(HelloMessage message, MyClient client) 
{
    Console.WriteLine(message.Content); // 'Hi server!'
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