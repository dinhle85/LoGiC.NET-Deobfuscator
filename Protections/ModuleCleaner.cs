using System.Linq;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class ModuleCleaner : IProtection
    {
        public string Name => nameof(ModuleCleaner);
        public string Description => "Cleans <Module>";
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes())
            {
                if (type != ctx.Module.GlobalType) continue;
                foreach (var method in type.Methods.ToArray())
                {
                    type.Methods.Remove(method);
                }
            }
        }
    }
}