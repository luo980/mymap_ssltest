using System;
using System.IO;
using Mono.Sercurity.X5091;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            string pkcs12FilePath = "/home/luo980/cacerts/client3_macalg.pfx";
            string password = "password";

            // 读取文件内容
            byte[] fileData = File.ReadAllBytes(pkcs12FilePath);

            // 创建PKCS12实例并调用Decode
            PKCS12 pkcs12 = new PKCS12(fileData, password);
            pkcs12.Decode(fileData); // 根据需要调用Decode

            Console.WriteLine("PKCS12 Decode successful.");
            // 进一步处理或验证解码的数据
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}