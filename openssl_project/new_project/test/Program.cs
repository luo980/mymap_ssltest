using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
// using MQTTnet.Samples.Helpers;
using System.Collections.Generic;

public class MqttSubscriber
{
    // private IMqttClient mqttClient;
    // public string brokerAddress = "localhost";
    // public int brokerPort = 1883;
    private IMqttClient mqttClient;
    public string brokerAddress = "luo980.giga";
    public int brokerPort = 8883; // 通常TLS端口是8883
    public string certificatePath = "/home/luo980/cacerts/client3_macalg.pfx";
    public string certificatePassword = "password";

    public async Task StartAsync() // 修改为异步方法
    {
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        byte[] certificateBytes = File.ReadAllBytes(certificatePath);
        var certificate = new X509Certificate2(certificateBytes, certificatePassword);
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerAddress, brokerPort)
            .WithTls(new MqttClientOptionsBuilderTlsParameters
            {
                UseTls = true,
                Certificates = new List<X509Certificate> { certificate },
                AllowUntrustedCertificates = true,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
            })
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine("Received application message.");
                Console.WriteLine($"{e.ApplicationMessage.Topic} : {(System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload))}");

                return Task.CompletedTask;
            };

        await mqttClient.ConnectAsync(options, CancellationToken.None);


        var mqttFactory = new MqttFactory();    
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("testtopic/hello");
                    })
                .Build();

        var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        Console.WriteLine("MQTT client subscribed to topic.");

            // The response contains additional data sent by the server after subscribing.
        DumpToConsole(response);
    
    }
    private void DumpToConsole(MqttClientSubscribeResult subscribeResult)
    {
        Console.WriteLine("Subscribe Result:");
        foreach (var resultItem in subscribeResult.Items)
        {
            Console.WriteLine($"Topic: {resultItem.TopicFilter.Topic}, Result: {resultItem.ResultCode}");
        }
    }


    public async Task SubscribeAsync(string topic) // 修改为异步方法
    {
        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
    }

    void ProcessReceivedMessage(string topic, string message)
    {
        // Process the message
        Console.WriteLine("Topic: {topic}. Message Received: {message}");
        // Similar to previous example
    }

    void OnDestroy()
    {
        mqttClient?.DisconnectAsync().Wait();
    }

}


class Program
{
    static async Task Main(string[] args) // 修改Main为异步方法
    {
        try
        {
            var subscriber = new MqttSubscriber();
            await subscriber.StartAsync();
            // 这里可以等待连接成功后订阅主题
            await subscriber.SubscribeAsync("robot/position");
            await subscriber.SubscribeAsync("robot/rotation");
            
            Console.WriteLine("Press any key to exit...");
            // 使用Console.ReadKey()在用户按下任意键之前保持程序运行
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
