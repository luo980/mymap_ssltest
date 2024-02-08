# .net Mono Security x509 HMAC Validation

使用了Mono源码进行修改，编译了Security库，添加了本地的MQTTnet.dll，简单改写示例工程完成了验证。

Unity的Mono库从源码定位：https://github.com/mono/mono/blob/main/mcs/class/Mono.Security/Mono.Security.X509/PKCS12.cs#L368-L369

```
if (macOid != "1.3.14.3.2.26")
					throw new ArgumentException ("unsupported HMAC");
```

原因在与只要不是使用sha1生成的pfx的都是unsupported.
