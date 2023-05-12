using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Model;

namespace WPFClient
{
    internal class Startup
    {
        static void Start()
        {
            IWordProcessing lp = new LocalWordProcessing("");
        }
    }
}
