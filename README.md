Log4NetMessageEncryptor
=======================

The Log4Net MessageEncryptor tool lets you encrypt log messages whilst keeping the log meta data (timestamp, level etc.) in plaintext. The encryption used is a standard symmetric key encryption algorithm (Rijndael) to securely lock away any sensitive information present within the log messages.

There is a current trend for developers to serialize all the parameters and return values from functions to the log file. This helps track down any errors quickly and efficiently, whilst this is a very useful technique it has one large flaw... you are potentially exposing and storing sensitive information within your applications log files. Ideally we don't want the whole log encrypted as this just means we can't look at the logs and quickly pinpoint the areas of the code going wrong, also, most of your logs wont contain any sensitive information. This message encryptor only encrypts the potentially sensitive part of the logs... the message. By leaving all the log message metadata intact this allows you to continue to use any log analysis tools that use this data to derive usage and error statistics whilst ensuring that any sensitive data in the log message is securely encrypted.


Installation
------------

The log message encryptor can be downloaded and installed from NuGet: https://www.nuget.org/packages/Log4Net.MessageEncryptor


Usage
-----

Once the NuGet package has been installed, all that is required to use the tool is a few configuration changes. An example configuration file can be found in the example project: https://github.com/ArtisanCode/Log4NetMessageEncryptor/blob/master/sample/MessageEncryptorExample/App.config

I'm not going to go through an in-depth log4net config tutorial, just the basics of getting the encryption working. If you do want a log4net tutorial, this one appears to do the trick http://www.codeproject.com/Articles/140911/log-net-Tutorial. If starting from a blank configuration file here are the steps that you need to go through to plumb the message encryptor into the log4net pipeline:

### 1) Add the log4net and the Log4NetMessageEncryption configuration sections

In order to let .NET know that there are some custom configuration settings that can be accessed, you need to update the configSections portion of the configuration file with the following:

```xml
<configsections>
  <section name="Log4NetMessageEncryption" type="ArtisanCode.Log4NetMessageEncryptor.Log4NetMessageEncryptorConfiguration, ArtisanCode.Log4NetMessageEncryptor" />
  <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
  <!-- Additional sections may be defined but not duplicated -->
</configsections>
```

### 2) Add encryption configuration

This is the heart of the tool, this tells the encryption algorithm everything it needs in order to function correctly.

```xml
<Log4NetMessageEncryption>
  <EncryptionKey KeySize="256" Key="3q2+796tvu/erb7v3q2+796tvu/erb7v3q2+796tvu8="/>
</Log4NetMessageEncryption>
```

**Important**

- The recommended KeySize value is 256 (the maximum permitted). This represents the length in bits of the encryption key. The encryption key must be _exactly_ this length.
- Please **do not use this example key ... EVER**!! Please ensure that you generate a new encryption key in a safe and secure manner. This key was generated using the following code:
```cs
Convert.ToBase64String(new byte[32] {
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF
            });
```
- This key is the same key used to encrypt _and_ decrypt so if this gets compromised then you will need to re-encrypt everything that was encrypted with the compromised key. To help keep this encryption information more secure, it is **_highly recommended_** that you encrypt the `<Log4NetMessageEncryption>...</Log4NetMessageEncryption>` configuration section using a tool like [Aspnet_regiis](http://msdn.microsoft.com/en-US/library/k6h9cz8h(v=vs.100).ASPX)

### 3) Add the encryption forwarding appender into the Log4Net configuration

This is the part where we tell Log4Net about the encryption and plumb it into the log pipeline. This is done with the following configuration:
```xml
<appender name="MessageEncryptingAppender" type="ArtisanCode.Log4NetMessageEncryptor.MessageEncryptingForwardingAppender">
  <!-- Add other appenders here and the log messages will be sent to every listed appender with the encrypted messages -->
  <appender-ref ref="RollingFileAppender"/> <!-- Link to another Log4Net appender -->
  <appender-ref ref="ColoredConsoleAppender"/> <!-- Link to another Log4Net appender -->
</appender>

<root>
  <!-- WARNING: Any other appenders added here will NOT receive encrypted messages. All the messages will appear in plain text. -->
  <appender-ref ref="MessageEncryptingAppender"/>
</root>
```

Very simply, what happens is the `MessageEncryptingAppender` derives from the [Log4Net.ForwardingAppender](http://logging.apache.org/log4net/release/sdk/log4net.Appender.ForwardingAppender.html) class. All log messages get routed through the encryption component, have the messages encrypted and then forwarded onto the other appenders listed within the configuration.

**Important**
If you have **ANY** other appenders referenced in the root node, then they will not have their log messages encrypted. This is due to the encryption appender not being able to intercept the log messages before they are appended to the log.