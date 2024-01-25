using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrvInterface {
  public interface ISrvInit {                      //doplnit public
    ISrvAut authorize(string name, string pass); //odstranit public, v interface nemůže být
  }
}
