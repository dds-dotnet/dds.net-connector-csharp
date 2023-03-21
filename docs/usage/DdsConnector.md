# DdsConnector

DDS.Net.Connector.**DdsConnector** is the main class for using the connector. Its constructor takes following arguments:

```csharp
DdsConnector(
            string applicationName,
            string serverIPv4,
            ushort serverPortTCP,
            ILogger logger)
```

> **applicationName** is the name of the application that is using the connector - it sent to the server for identification purposes.
> 
> **serverIPv4** is the IP address of target server in standard IPv4 format; for example, "127.0.0.1" is a loopback address.
> 
> **serverPortTCP** is the TCP port number on which the target server is listening for connections.
> 
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

Providers provide variable data to the server. Following provider delegates are available (each delegate takes-in the variable's name and returns requisite data):

  * *string* **StringProvider** (string *variableName*)
  * *bool* **BooleanProvider** (string *variableName*)
  * *sbyte* **ByteProvider** (string *variableName*)
  * *short* **WordProvider** (string *variableName*)
  * *int* **DWordProvider** (string *variableName*)
  * *long* **QWordProvider** (string *variableName*)
  * *byte* **UnsignedByteProvider** (string *variableName*)
  * *ushort* **UnsignedWordProvider** (string *variableName*)
  * *uint* **UnsignedDWordProvider** (string *variableName*)
  * *ulong* **UnsignedQWordProvider** (string *variableName*)
  * *float* **SingleProvider** (string *variableName*)
  * *double* **DoubleProvider** (string *variableName*)
  * *byte[]* **RawBytesProvider** (string *variableName*)
    
For using, it is required to implement requisite methods as per delegates and register them through following provider registration methods in *DdsConnector*.

Here

> **variableName** is case-sensitive name for the variable.
> 
> **provider** is the data provider function as per delegate signature.
> 
> **periodicity** is enumerated update [Periodicity](./Periodicity.md)

  * **RegisterStringProvider** (string *variableName*, StringProvider *provider*, Periodicity *periodicity*)
  * **RegisterBooleanProvider** (string *variableName*, BooleanProvider *provider*, Periodicity *periodicity*)
  * **RegisterByteProvider** (string *variableName*, ByteProvider *provider*, Periodicity *periodicity*)
  * **RegisterWordProvider** (string *variableName*, WordProvider *provider*, Periodicity *periodicity*)
  * **RegisterDWordProvider** (string *variableName*, DWordProvider *provider*, Periodicity *periodicity*)
  * **RegisterQWordProvider** (string *variableName*, QWordProvider *provider*, Periodicity *periodicity*)
  * **RegisterUnsignedByteProvider** (string *variableName*, UnsignedByteProvider *provider*, Periodicity *periodicity*)
  * **RegisterUnsignedWordProvider** (string *variableName*, UnsignedWordProvider *provider*, Periodicity *periodicity*)
  * **RegisterUnsignedDWordProvider** (string *variableName*, UnsignedDWordProvider *provider*, Periodicity *periodicity*)
  * **RegisterUnsignedQWordProvider** (string *variableName*, UnsignedQWordProvider *provider*, Periodicity *periodicity*)
  * **RegisterSingleProvider** (string *variableName*, SingleProvider *provider*, Periodicity *periodicity*)
  * **RegisterDoubleProvider** (string *variableName*, DoubleProvider *provider*, Periodicity *periodicity*)
  * **RegisterRawBytesProvider** (string *variableName*, RawBytesProvider *provider*, Periodicity *periodicity*)
  * **UnregisterProvider** (string *variableName*)



### Consumers

Consumers get data from the server provided by any provider. Following consumer delegates are available (each is provided with the variable's name and requisite data):

  * *void* **StringConsumer** (string *variableName*, string *variableValue*)
  * *void* **BooleanConsumer** (string *variableName*, bool *variableValue*)
  * *void* **ByteConsumer** (string *variableName*, sbyte *variableValue*)
  * *void* **WordConsumer** (string *variableName*, short *variableValue*)
  * *void* **DWordConsumer** (string *variableName*, int *variableValue*)
  * *void* **QWordConsumer** (string *variableName*, long *variableValue*)
  * *void* **UnsignedByteConsumer** (string *variableName*, byte *variableValue*)
  * *void* **UnsignedWordConsumer** (string *variableName*, ushort *variableValue*)
  * *void* **UnsignedDWordConsumer** (string *variableName*, uint *variableValue*)
  * *void* **UnsignedQWordConsumer** (string *variableName*, ulong *variableValue*)
  * *void* **SingleConsumer** (string *variableName*, float *variableValue*)
  * *void* **DoubleConsumer** (string *variableName*, double *variableValue*)
  * *void* **RawBytesConsumer** (string *variableName*, byte[] *variableValue*)

For using, it is required to implement requisite methods as per delegates and register them through following consumer registration methods in *DdsConnector*.

Here

> **variableName** is case-sensitive name for the variable.
> 
> **consumer** is the data consumer function as per delegate signature.
> 
> **periodicity** is enumerated update [Periodicity](./Periodicity.md)

  * **RegisterStringConsumer** (string *variableName*, StringConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterBooleanConsumer** (string *variableName*, BooleanConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterByteConsumer** (string *variableName*, ByteConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterWordConsumer** (string *variableName*, WordConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterDWordConsumer** (string *variableName*, DWordConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterQWordConsumer** (string *variableName*, QWordConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterUnsignedByteConsumer** (string *variableName*, UnsignedByteConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterUnsignedWordConsumer** (string *variableName*, UnsignedWordConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterUnsignedDWordConsumer** (string *variableName*, UnsignedDWordConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterUnsignedQWordConsumer** (string *variableName*, UnsignedQWordConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterSingleConsumer** (string *variableName*, SingleConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterDoubleConsumer** (string *variableName*, DoubleConsumer *consumer*, Periodicity *periodicity*)
  * **RegisterRawBytesConsumer** (string *variableName*, RawBytesConsumer *consumer*, Periodicity *periodicity*)
  * **UnregisterConsumer** (string *variableName*)
 
