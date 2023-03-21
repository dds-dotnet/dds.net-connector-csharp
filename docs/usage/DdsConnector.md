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



## 
