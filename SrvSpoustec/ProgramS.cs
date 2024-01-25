using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using SrvLib;

namespace Server {
  public class ProgramS {
    public static void Main() {
      int srvPort = 1234;
      string channelID = "channel1";
      IChannel channel = new TcpServerChannel(srvPort);
      ChannelServices.RegisterChannel(channel, false);

      SrvInit obj = new SrvInit();

      RemotingServices.Marshal(obj, channelID);
      Console.WriteLine("server bezi na portu: " + srvPort);
      while (true) { System.Threading.Thread.Sleep(1000); }
    }
  }
}
