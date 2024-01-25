using System;
using SrvInterface;                                                 //i do references
using System.Collections.Generic;

namespace SrvLib {
  public class SrvInit : MarshalByRefObject, ISrvInit {     //
    public ISrvAut authorize(string name, string pass) {  //ISrvAut místo SrvAut
      if (userAuthorization(name, pass)) {
        Console.WriteLine("připojil se uživatel: " + name);
        return new SrvAut();
      }
      else {
        return null;
      }
    }
    private bool userAuthorizationx(string name, string pass) {
      bool bul = false;
      switch (name) {
        case "Agent W4C": if (pass == "abraka dabra") bul = true; break;
        case "kralici": if (pass == "nakrmit je") bul = true; break;
      }
      return bul;
    }
    private bool userAuthorization(string name, string pass) {   //privátní, volá se jen z této třídy
      Dictionary<string, string> passwords = new Dictionary<string, string>();
      passwords.Add("Agent W4C", "abraka dabra");
      passwords.Add("kralici", "nakrmit je");

      if (passwords.ContainsKey(name))
        return (passwords[name] == pass);
      else
        return false;
    }

  }
}
