using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using SrvInterface;
using System.IO;                                               //1  kvůli File
using System.Xml;                                              //3

namespace Klient {
  public class ProgramKxml {
    public static void Mainx(string[] args) {
      string cestaxml;
      //if (args.Length < 1)        {Console.WriteLine("chybi parametr -> cesta k XML."+                    "Zadej cestu k souboru"); cestaxml = Console.ReadLine();}   //1
      //else cestaxml = args[0];
      //if (!File.Exists(cestaxml)) { Console.WriteLine("konfiguracni soubor " + cestaxml + " neexistuje." + "Zadej cestu k souboru"); cestaxml = Console.ReadLine(); } //1

      try {                                                     //2 totéž pomocí výjimky
        if (args.Length < 1)                                    //2
          throw new Exception("chybi parametr -> cesta k XML.");//2
        if (!File.Exists(args[0]))                              //2
          throw new Exception("konfiguracni soubor '" + args[0] + "' neexistuje.");//2
        cestaxml = args[0];
      }                                                         //2
      catch (Exception e) {                                     //2
        Console.WriteLine(e.Message + " Zadej cestu k souboru"); cestaxml = Console.ReadLine();//2 Zde již musí být cesta správně, jinak to pak zhavaruje. Správně by mělo být ve while, zadávat tak dlouho, dokud nebude cesta v pořádku
      }                                                         //2                              configInner.xml
     
      //string srvIP = "127.0.0.1";                              //
      //int srvPort = 1234;                                      //
      string channelID = "channel1";
      XmlDocument xml_document = new XmlDocument();                //3
      try {                                                        //4
        xml_document.Load(cestaxml);                               //3 
      }
      catch (XmlException e) { Console.WriteLine("Soubor nemá formát XML. " + e); return; }//4 e.Message je např.: data na kořenové úrovni nejsou platná      
      XmlElement root = xml_document.DocumentElement;                      //3 XmlElement dědí z XmlNode
      string srvIP = "", name = "", pass = "";
      int srvPort = 0;
      try {
        srvIP = root.SelectSingleNode("server/ip").InnerText;         //3
        srvPort = Int32.Parse(root.SelectSingleNode("server/port").InnerText); //3 může způsobit FormatException
        name = root.SelectSingleNode("uzivatel/name").InnerText;    //3
        pass = root.SelectSingleNode("uzivatel/pass").InnerText;    //3
        //pokud není některý element nalezen, nastane NullReferenceException, protože SelectSingleNode má vrátit objekt XmlNode
      }                                                                                               //4
      catch (FormatException e) { Console.WriteLine("číslo portu v souboru nemá formát čísla " + e); return; }//4
      catch (NullReferenceException e) { Console.WriteLine("element v XML souboru nenalezen " + e); return; } //4

      Console.WriteLine(srvIP + srvPort + name + pass);
      IChannel channel;
      int registeredCanalsCount = ChannelServices.RegisteredChannels.Length;
      if (registeredCanalsCount < 1) {
        channel = new TcpClientChannel();
        ChannelServices.RegisterChannel(channel, false);
      }
      string srvAdr = "tcp://" + srvIP + ":" + srvPort + "/" + channelID;
      
      bool connectionOK = false;
      Console.WriteLine("cekam na spojeni se serverem");
      ISrvAut objAut = null;
      while (!connectionOK) {
        try {
          ISrvInit obj = (ISrvInit)Activator.GetObject(typeof(ISrvInit), srvAdr);
          //objAut = obj.autorizuj("Agent W4C", "abraka dabra");        //
          objAut = obj.authorize(name, pass);                         //3

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
        else Console.WriteLine("Špatné jméno nebo heslo");
      }
      catch (System.Net.Sockets.SocketException se) {
        Console.WriteLine("Přerušeno spojení se serverem, info: " + se.Message); Console.ReadKey();
      }
    }
  }
}