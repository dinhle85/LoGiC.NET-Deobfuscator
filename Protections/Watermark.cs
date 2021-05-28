using System.Linq;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class Watermark : IProtection
    {
        public string Name => nameof(Watermark);
        public string Description => "Removes LoGiC.NET Custom Attribute.";
        public void Execute(Context ctx)
        {
            foreach (var customAttr in ctx.Module.CustomAttributes.Where(c => c.TypeFullName.Contains("LoGiCdotNet")).ToArray())
            {
                ctx.Module.CustomAttributes.Remove(customAttr);
            }
        }
    }
}