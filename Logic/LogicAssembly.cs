using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]

namespace WebLicense.Logic
{
    public static class LogicAssembly
    {
        public static Assembly Assembly => typeof(LogicAssembly).Assembly;
    }
}
