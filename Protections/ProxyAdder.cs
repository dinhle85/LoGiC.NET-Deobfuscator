using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class ProxyAdder : IProtection
    {
        public string Name => nameof(ProxyAdder);
        private const int Intensity = 2;
        public void Execute(Context ctx)
        {
            for (var intens = 0; intens < Intensity; intens++)
            {
                foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods))
                {
                    if (type.IsGlobalModuleType) continue;
                    foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions && !m.IsUnmanaged).ToArray())
                    {
                        if (!method.HasParams()) continue;
                        var count = method.GetParamCount();
                        if (count is 0 or > 4) continue;
                        var instr = method.Body.Instructions;
                        foreach (var instruction in instr)
                        {
                            if (instruction.OpCode != OpCodes.Call || instruction.Operand is not MethodDef callMethod || callMethod.GetParamCount() != count) continue;
                            
                        }
                    }
                }
            }
        }
    }
}