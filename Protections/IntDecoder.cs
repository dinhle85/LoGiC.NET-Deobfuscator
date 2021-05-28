using System.Linq;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class IntDecoder : IProtection
    {
        public string Name => nameof(IntDecoder);
        public string Description => "Decodes all encoded integers.";
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions && m.Body.Instructions.Any(i => i.OpCode == OpCodes.Call)))
                {
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Ldstr || instr[i + 1].OpCode != OpCodes.Call ||
                            !instr[i + 1].Operand.ToString().Contains("Module") ||
                            instr[i + 2].OpCode != OpCodes.Call ||
                            !instr[i + 2].Operand.ToString().Contains("get_Length") ||
                            instr[i + 3].OpCode != OpCodes.Call) continue;
                        
                        var res = System.Math.Abs(instr[i].Operand.ToString().Length);
                        instr[i] = Instruction.CreateLdcI4(res);
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i + 2].OpCode = OpCodes.Nop;
                        instr[i + 3].OpCode = OpCodes.Nop;
                    }
                }
            }
        }
    }
}