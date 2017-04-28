using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator
{
    public class VirtualFile : Tuple<string, string>
    {
        public VirtualFile(string fileName, string content) : base(fileName, content)
        {
        }
    }
}
