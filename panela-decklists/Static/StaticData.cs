using Decklists.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Static
{
    public static class StaticData
    {
        public static List<Tuple<string, string, int>> SupportedCollections = new List<Tuple<string, string, int>>()
        {
            new Tuple<string,string,int>( "XY10", "Fates Collide", 124 )
        };
    }
}
