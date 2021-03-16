using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Configuration;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        public static byte[] ReciveBytes(Socket socket)
        {
            var binarySize = new byte[4];
            socket.Receive(binarySize);
            int size = BitConverter.ToInt32(binarySize);

            int position = 0;
            var data = new byte[size];
            while (size > 0)
            {
                int receivedBytes = socket.Receive(data, position, size, SocketFlags.None);
                size -= receivedBytes;
                position += receivedBytes;
            }

            return data;
        }

        public static string GetHash(byte[] input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(input);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        static void Main(string[] args)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["address"]), int.Parse(ConfigurationManager.AppSettings["port"]));
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);

                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    byte[] size = new byte[4];

                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
                    byte[] bynaryRSA = Encoding.UTF8.GetBytes(rsa.ToXmlString(false));
                    Console.WriteLine("Хеш открытого ключа RSA: " + GetHash(bynaryRSA));
                    Console.WriteLine("Хеш ключа RSA: " + GetHash(Encoding.UTF8.GetBytes(rsa.ToXmlString(true))));
                    size = BitConverter.GetBytes(bynaryRSA.Length);
                    handler.Send(size);
                    handler.Send(bynaryRSA);
                    Console.WriteLine("Отправлена публичная часть ключа RSA.\n");

                    byte[] encryptedDESKeyAndIV = ReciveBytes(handler);
                    Console.WriteLine("Хеш зашифрованного ключа DES: " + GetHash(encryptedDESKeyAndIV));
                    byte[] binaryDesKeyAndIV = rsa.Decrypt(encryptedDESKeyAndIV, false);
                    Console.WriteLine("Хеш дешифрованного ключа DES: " + GetHash(binaryDesKeyAndIV));
                    string jsonDesKeyAndIV = Encoding.UTF8.GetString(binaryDesKeyAndIV);
                    var desKeyAndIV = JsonConvert.DeserializeObject<DESKeyAndIV>(jsonDesKeyAndIV);
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    des.Key = desKeyAndIV.Key;
                    des.IV = desKeyAndIV.IV;
                    Console.WriteLine("Получен DES ключ.\n");

                    var games = new List<Game>();
                    var binaryGamesCount = new byte[4];
                    handler.Receive(binaryGamesCount);
                    int gamesCount = BitConverter.ToInt32(binaryGamesCount);

                    for (int i = 0; i < gamesCount; i++)
                    {
                        byte[] encryptedGame = ReciveBytes(handler);
                        Console.WriteLine("Хеш зашифрованных данных: " + GetHash(encryptedGame));
                        byte[] binaryGame = DESDencryptor.DESDecrypt(encryptedGame, des);
                        Console.WriteLine("Хеш расшифрованных данных: " + GetHash(binaryGame));
                        string jsonGame = Encoding.UTF8.GetString(binaryGame);
                        var game = JsonConvert.DeserializeObject<Game>(jsonGame);
                        Console.WriteLine("Данные получены.");
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                    PostgresConnector.postresConnection.Open();
                    foreach (var game in games)
                    {
                        if (!PostgresConnector.GanreCheck(game.genreId))
                            PostgresConnector.GanreInsert(game);
                        if (!PostgresConnector.DeveloperCheck(game.developerId))
                            PostgresConnector.DeveloperInsert(game);
                        if (!PostgresConnector.PublisherCheck(game.publisherId))
                            PostgresConnector.PublisherInsert(game);
                        if (!PostgresConnector.GameCheck(game.id))
                            PostgresConnector.GameInsert(game);
                        if (!PostgresConnector.GanreAndGameCheck(game.id, game.genreId))
                            PostgresConnector.GanreAndGameInsert(game);
                    }
                    Console.WriteLine("Данные добавлены в БД.");
                    PostgresConnector.postresConnection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}