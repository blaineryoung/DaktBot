using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core
{
    public static class BotUtilities
    {
        public static IEnumerable<Type> GetSubclasses(Type baseType)
        {
            return Assembly.GetAssembly(baseType).GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(baseType));
        }
    }
}
