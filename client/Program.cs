using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Configuration;
using Newtonsoft.Json;

namespace Client
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
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["address"]), int.Parse(ConfigurationManager.AppSettings["port"]));
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                byte[] size = new byte[4];

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
                byte[] bynaryRSA = ReciveBytes(socket);
                rsa.FromXmlString(Encoding.UTF8.GetString(bynaryRSA));
                Console.WriteLine("Получена публичная часть ключа RSA.");
                Console.WriteLine("Хеш открытого ключа RSA : " + GetHash(bynaryRSA) + "\n");

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.GenerateKey();
                des.GenerateIV();
                string jsonDESKeyAndIV = JsonConvert.SerializeObject(new DESKeyAndIV(des.Key, des.IV));
                byte[] DESKeyandIV = Encoding.UTF8.GetBytes(jsonDESKeyAndIV);
                Console.WriteLine("Хеш ключа DES: " + GetHash(DESKeyandIV));
                byte[] encryptedDESKeyAndIV = rsa.Encrypt(DESKeyandIV, false);
                Console.WriteLine("Хеш зашифрованного ключа DES: " + GetHash(encryptedDESKeyAndIV));
                size = BitConverter.GetBytes(encryptedDESKeyAndIV.Length);
                socket.Send(size);
                socket.Send(encryptedDESKeyAndIV);
                Console.WriteLine("Отправлен DES ключ. \n");

                var games = SQLiteConnector.SelectAll();
                Console.WriteLine("Данные получены из БД.");

                byte[] gamesCount = new byte[4];
                gamesCount = BitConverter.GetBytes(games.Count);
                socket.Send(gamesCount);

                foreach (var game in games)
                {
                    string jsonGame = JsonConvert.SerializeObject(game);
                    byte[] binaryGame = Encoding.UTF8.GetBytes(jsonGame);
                    Console.WriteLine("Хеш данных: " + GetHash(binaryGame));
                    byte[] enctyptedGame= DESEncryptor.DESEncrypt(binaryGame, des);
                    Console.WriteLine("Хеш зашифрованных данных: " + GetHash(enctyptedGame));
                    size = BitConverter.GetBytes(enctyptedGame.Length);
                    socket.Send(size);
                    socket.Send(enctyptedGame);
                    Console.WriteLine("Данные отправлены.");
                }

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.Read();
        }
    }
}