using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
//using SrvJLib;                        //
using SrvInterface;                      //kvůli rozhraní SrvInitInterface. Musí být předtím doplněno i do referencí

namespace KlientSpoustec {
  class ProgramK {
    public static void Mainx() {
      string srvIP = "127.0.0.1";
      // string srvIP = "192.168.0.219";
      int srvPort = 1234;
      string channelID = "channel1";                                                         
      IChannel channel;
            //int registeredCanalsCount = ChannelServices.RegisteredChannels.Length;
            //if (registeredCanalsCount < 1) {
                channel = new TcpClientChannel();
                ChannelServices.RegisterChannel(channel, false);
            //}
            string srvAdr = "tcp://" + srvIP + ":" + srvPort + "/" + channelID;
      bool connectionOK = false;
      Console.WriteLine("cekam na spojeni se serverem");      
      ISrvAut objAut = null;                                                                    //ISrvAut místo SrvAut
      while (!connectionOK) {
        try {
          ISrvInit obj = (ISrvInit)Activator.GetObject(typeof(ISrvInit), srvAdr);//ISrvInit místo SrvInit
          objAut = obj.authorize("Agent W4C", "abraka dabra");
          connectionOK = true;
          Console.WriteLine("klient bezi, pripojen na server: " + srvIP + ":" + srvPort);
        }
        catch (System.Net.Sockets.SocketException) { Console.Write("."); }
        System.Threading.Thread.Sleep(1000);
      }

      try {
        if (objAut != null) {          
          int number_a;
          do {
            Console.Write("Zadej cislo a: "); number_a = int.Parse(Console.ReadLine());
            Console.Write("Zadej cislo b: "); int number_b = int.Parse(Console.ReadLine());
            Console.WriteLine("Soucet= " + objAut.sum(number_a, number_b));
            Console.WriteLine("Rozdíl= " + objAut.diff(number_a, number_b));
          } while (number_a != 0);
        }
        else Console.WriteLine("Špatná autorizace");
      }
      catch (System.Net.Sockets.SocketException se) {
        Console.WriteLine("chyba pri komunikace se serverem, info: " + se.Message);
        Console.ReadKey();
      }
    }
  }
}

