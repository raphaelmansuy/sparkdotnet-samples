using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleTPCServer
{
  class Server
  {
    TcpListener server = null;
    public bool IsRunning { get; set; }
    public Server(string ip, int port)
    {
      IPAddress localAddr = IPAddress.Parse(ip);
      server = new TcpListener(localAddr, port);
      this.IsRunning = true;

      Console.WriteLine($"Start TCP Server on Ip={localAddr} and port={port}");

      server.Start();
      StartListener();
    }
    public void StartListener()
    {
      try
      {
        while (this.IsRunning)
        {
          Console.WriteLine("Waiting for a connection");
          TcpClient client = server.AcceptTcpClient();
          Console.WriteLine("Connected!");
          Thread t = new Thread(new ParameterizedThreadStart(HandleConnection));
          t.Start(client);
        }
      }
      catch (SocketException e)
      {
        Console.WriteLine("SocketException: {0}", e.Message);
      }
      finally
      {
        server.Stop();
      }
    }
    public void HandleConnection(Object obj)
    {
      TcpClient client = (TcpClient)obj;
      var stream = client.GetStream();


      var variables = new String[] { "A", "B", "C", "D", "E" };

      var random = new Random();
      try
      {
        while (this.IsRunning)
        {
          var now = DateTime.Now.ToUniversalTime().ToString("o", System.Globalization.CultureInfo.InvariantCulture);
          var val = random.Next(0, 1000);
          var index = random.Next(0,variables.Length-1);
          var str = $"{now};{variables[index]};{val}\n";
          var message = Encoding.UTF8.GetBytes(str);
          stream.Write(message, 0, message.Length);
          Console.WriteLine("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
          Thread.Sleep(1000);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception: {0}", e.ToString());
        client.Close();
      }
    }
  }
}