using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using SrvInterface;
using System.IO;                                               //1  kvůli File
using System.Xml;                                              //3

namespace Klient {
  public class ProgramKxmlAttrib {
    public static void Main(string[] args) {
      string cestaxml="ConfigAttrib.xml";
      //if (args.Length < 1)        {Console.WriteLine("chybi parametr -> cesta k XML."+                    "Zadej cestu k souboru"); cestaxml = Console.ReadLine();}   //1
      //else cestaxml = args[0];
      //if (!File.Exists(cestaxml)) { Console.WriteLine("konfiguracni soubor " + cestaxml + " neexistuje." + "Zadej cestu k souboru"); cestaxml = Console.ReadLine(); } //1

      //try {                                                     //2 totéž pomocí výjimky
      //  if (args.Length < 1)                                    //2
      //    throw new Exception("chybi parametr -> cesta k XML.");//2
      //  if (!File.Exists(args[0]))                              //2
      //    throw new Exception("konfiguracni soubor '" + args[0] + "' neexistuje.");//2
      //  cestaxml = args[0];
      //}                                                         //2
      //catch (Exception e) {                                     //2
      //  Console.WriteLine(e.Message + " Zadej cestu k souboru"); cestaxml = Console.ReadLine();//2 Zde již musí být cesta správně, jinak to pak zhavaruje. Správně by mělo být ve while, zadávat tak dlouho, dokud nebude cesta v pořádku
      //}                                                         //2                              configInner.xml
     
      //string srvIP = "127.0.0.1";                              //
      //int srvPort = 1234;                                      
      string channelID = "channel1";
      XmlDocument xml_document = new XmlDocument();                
      try {                                                       
        xml_document.Load(cestaxml);                               
      }
      catch (XmlException e) { Console.WriteLine("Soubor nemá formát XML. " + e); return; }//4 e.Message je např.: data na kořenové úrovni nejsou platná      
      XmlElement root = xml_document.DocumentElement;                      
      string srvIP = "", name = "", pass = "";
      int srvPort = 0;
      try {    //ChildNodes[0] a [2] jsou komentáře
        //nejprve natvrdo (tedy vím, na kterých pozicích data jsou)
       // srvIP = root.ChildNodes[1].Attributes[0].Value;                //    
        //srvPort = Int32.Parse(root.ChildNodes[1].Attributes[1].Value); //
        //name = root.ChildNodes[3].Attributes[0].Value;                //
        //pass = root.ChildNodes[3].Attributes[1].Value;                //

        foreach (XmlNode nod in root.ChildNodes) {                 //průchod vnořenými uzly
          if (nod.Attributes != null) {                             //přeskočí řádky s komentářem
            XmlAttributeCollection attribs = nod.Attributes;       //získání kolekce atributů konkrétního elementu
            foreach (XmlAttribute atr in attribs) {                //průchod attribs   
              if (nod.Name.Equals("server") && atr.Name.Equals("ip")) srvIP = atr.Value;              
              if (nod.Name == "server" && atr.Name == "port") srvPort = Int32.Parse(atr.Value);
              if (nod.Name.Equals("uzivatel") && atr.Name.Equals("name")) name = atr.Value;
              if (nod.Name.Equals("uzivatel") && atr.Name.Equals("pass")) pass = atr.Value;
            }
          }
        }           
      }                                                                                               
      catch (FormatException e) { Console.WriteLine("číslo portu v souboru nemá formát čísla " + e); return; }
      catch (NullReferenceException e) { Console.WriteLine("element v XML souboru nenalezen " + e); return; } 

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
          //objAut = obj.authorize("Agent W4C", "abraka dabra");        
          objAut = obj.authorize(name, pass);                         

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