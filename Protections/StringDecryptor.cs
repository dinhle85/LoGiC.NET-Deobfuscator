using System;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class StringDecryptor : IProtection
    {
        public string Name => nameof(StringDecryptor);
        public string Description => "Decrypts strings encrypted by LoGiC.NET";
        public static string Decrypt(string str)
        {
            var array = "*$,;:!ù^*&é\"'(-è_çà)".ToCharArray();
            str = array.Aggregate(str, (current, c) => current.Replace(c.ToString(), string.Empty));
            return Encoding.UTF32.GetString(Convert.FromBase64String(str));
        }
        public void Execute(Context ctx)
        {
            MethodDef decMethod = null;
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods))
            {
                if (type == ctx.Module.GlobalType) continue;
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Ldstr || instr[i + 1].OpCode != OpCodes.Call || instr[i + 1].Operand is not MethodDef meth) continue;
                        try
                        {
                            if (meth.DeclaringType != ctx.Module.GlobalType && meth.DeclaringType2 != ctx.Module.GlobalType)
                            {
                                continue;
                            }
                            instr[i] = Instruction.Create(OpCodes.Ldstr, Decrypt(instr[i].Operand.ToString()));
                        }
                        catch
                        {
                            // If fail try to invoke method.
                            try
                            {
                                decMethod = instr[i + 1].Operand as MethodDef;
                                if (decMethod.DeclaringType == ctx.Module.GlobalType)
                                {
                                    var decrypted = ctx.Asm.ManifestModule.ResolveMethod(meth.MDToken.ToInt32()).Invoke(null, new object[] {instr[i].Operand.ToString()}).ToString();
                                    instr[i] = Instruction.Create(OpCodes.Ldstr, decrypted);
                                }
                                else
                                {
                                    ctx.Logger.Warning($"Couldn't decrypt string \"{instr[i].Operand}\" in {method.Name} at {i}");
                                }
                            }
                            catch
                            {
                                ctx.Logger.Warning($"Couldn't decrypt string \"{instr[i].Operand}\" in {method.Name} at {i}");
                            }
                        }
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i + 1].Operand = null;
                    }
                }
            }
            if (decMethod != null)
            {
                ctx.Module.GlobalType.Methods.Remove(decMethod);
            }
        }
    }
}