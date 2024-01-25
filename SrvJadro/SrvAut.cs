using System;
using System.Collections.Generic;
using System.Text;
using SrvInterface;                                                     //i do references

namespace SrvLib {
  public class SrvAut : MarshalByRefObject, ISrvAut {           //
    public int sum(int a, int b) {
      Console.WriteLine("Server pocita soucet cisel " + a + " a " + b);
      return a + b;
    }
    public int diff(int a, int b) {
      Console.WriteLine("Server pocita rozdil cisel " + a + " a " + b);
      return a - b;
    }
  }
}

