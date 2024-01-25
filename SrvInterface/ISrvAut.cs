using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrvInterface {
  public interface ISrvAut { //doplnit public
    int sum(int a, int b);        //odstranit public, v interface nemůže být
    int diff(int a, int b);
  }
}
