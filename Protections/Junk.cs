using System.Linq;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class Junk : IProtection
    {
        public string Name => nameof(Junk);
        public string Description => "Removes all junk methods";
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions && m.HasReturnType && (m.ReturnType.IsPrimitive || m.ReturnType.ToString() == "System.String") && m.Body.Instructions.Count is 5 or 3).ToArray())
                {
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count - 1; i++)
                    {
                        if (instr[i].IsLdcI4() && instr[i].GetLdcI4Value() >= 50)
                        {
                            type.Methods.Remove(method);
                            break;
                        }
                        if (instr[i].OpCode != OpCodes.Ldstr || instr[i].Operand.ToString().Length < 50) continue;
                        type.Methods.Remove(method);
                        break;
                    }
                }
            }
        }
    }
}