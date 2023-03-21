# DdsConnector

DDS.Net.Connector.**DdsConnector** is the main class for using the connector. Its constructor takes following arguments:

```
DdsConnector(
            string applicationName,
            string serverIPv4,
            ushort serverPortTCP,
            ILogger logger)
```

> **applicationName** is the name of the application that is using the connector - it sent to the server for identification purposes.

> **serverIPv4** is the IP address of target server in standard IPv4 format; for example, "127.0.0.1" is a loopback address.

> **serverPortTCP** is the TCP port number on which the target server is listening for connections.

> **logger** is interface implementation for [*ILogger*](./ILogger.md), the library includes common logger implementations.



## Starting / Stopping the connector

The connector can be started and stopped after successful initialization. When *Start* is invoked on *DdsConnector* object, the connector connects with the server for data exchange, *Stop* does the opposite.

```csharp
DdsConnector connector = new DdsConnector(
        "My app name", "127.0.0.1", 44556, new BlankLogger());

// ...

// ...

connector.Start();

// ...

// ...

connector.Stop();
```

## Variables' Registration

Following variable types can be registered with the server:


| Type              | Represented data                                    |
|-------------------|-----------------------------------------------------|
| *String*          | Sequence of characters in Unicode                   |
| *Boolean*         | A boolean (True or False)                           |
| *Byte*            | 1-byte Signed Integer                               |
| *Word*            | 2-byte Signed Integer                               |
| *DWord*           | 4-byte Signed Integer                               |
| *QWord*           | 8-byte Signed Integer                               |
| *Unsigned Byte*   | 1-byte Unsigned Integer                             |
| *Unsigned Word*   | 2-byte Unsigned Integer                             |
| *Unsigned DWord*  | 4-byte Unsigned Integer                             |
| *Unsigned QWord*  | 8-byte Unsigned Integer                             |
| *Single*          | A single precision (4-byte) Floating-point number   |
| *Double*          | A double precision (8-byte) Floating-point number   |
| *Raw Bytes*       | Sequence of bytes                                   |


### Providers

Providers provide variable data to the server.


### Consumers

Consumers get data from the server provided by any provider.

