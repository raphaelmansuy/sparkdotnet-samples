using System;

namespace SimpleTPCServer
{
  class Program
  {
    static void Main(string[] args)
    {

      try
      {

        Console.WriteLine("Simple TCP Server");
        var port = args.Length > 0 ? args[0] : "65001";
        int portNumber = 65001;
        int.TryParse(port, out portNumber);
        var server = new Server("127.0.0.1",portNumber);

      }
      catch (Exception e)
      {
        Console.Write(e.Message);
      }

    }
  }
}