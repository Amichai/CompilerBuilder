using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
    public class ToXmlHelper {
        public string ToXml(ParseTree tree) {
            return tree.ToXml();
        }

        public string Test() {
            return "testing!";
        }
    }
}
