using System.Linq;
using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace LogicDeobfuscator.Core
{
    public static class Utilities
    {
        public static bool CanRename(this object member, MethodDef ep)
        {
            return member switch
            {
                MethodDef mDef => !mDef.IsRuntimeSpecialName && !mDef.DeclaringType.IsForwarder && !mDef.IsConstructor,
                FieldDef fDef => !fDef.IsRuntimeSpecialName && !(fDef.IsLiteral && fDef.DeclaringType.IsEnum),
                PropertyDef pDef => !pDef.IsRuntimeSpecialName && !pDef.IsEmpty && pDef.IsSpecialName,
                EventDef evDef => !evDef.IsRuntimeSpecialName,
                _ => false
            };
        }
        public static void RemoveUselessNops(this MethodDef method)
        {
            var instr = method.Body.Instructions;
            for (var i = 0; i < instr.Count; i++)
            {
                checkNext:
                if (instr[i].OpCode != OpCodes.Nop || IsNopBranchTarget(method, instr[i]) || IsNopSwitchTarget(method, instr[i]) || IsNopExceptionHandlerTarget(method, instr[i])) continue;
                method.Body.Instructions.RemoveAt(i);
                if (instr[i] != instr.Last())
                {
                    goto checkNext;
                }
            }
        }

        public static void RemoveNegativeInstructions(this MethodDef method)
        {
            var instr = method.Body.Instructions;
            for (var i = 0; i < instr.Count; i++)
            {
                checkNext:
                if (instr[i].OpCode != OpCodes.Neg) continue;
                method.Body.Instructions.RemoveAt(i);
                if (instr[i] != instr.Last())
                {
                    goto checkNext;
                }
            }
        }
        
        public static void De4dotBlocks(this MethodDef method)
        {
            var blocksCflowDeobfuscator = new BlocksCflowDeobfuscator();
            var blocks = new Blocks(method);
            blocksCflowDeobfuscator.Initialize(blocks);
            blocksCflowDeobfuscator.Deobfuscate();
            blocks.RepartitionBlocks();
            blocks.GetCode(out var list, out var exceptionHandlers);
            DotNetUtils.RestoreBody(method, list, exceptionHandlers);
        }
        
        private static bool IsNopBranchTarget(MethodDef method, Instruction nopInstr)
        {
            var instr = method.Body.Instructions;
            return (from t in instr where t.OpCode.OperandType == OperandType.InlineBrTarget || t.OpCode.OperandType == OperandType.ShortInlineBrTarget && t.Operand != null select (Instruction) t.Operand).Any(instruction2 => instruction2 == nopInstr);
        }

        private static bool IsNopSwitchTarget(MethodDef method, Instruction nopInstr)
        {
            var instr = method.Body.Instructions;
            return (from t in instr where t.OpCode.OperandType == OperandType.InlineSwitch && t.Operand != null select (Instruction[]) t.Operand).Any(source => source.Contains(nopInstr));
        }

        private static bool IsNopExceptionHandlerTarget(MethodDef method, Instruction nopInstr)
        {
            if (!method.Body.HasExceptionHandlers) return false;
            var exceptionHandlers = method.Body.ExceptionHandlers;
            return exceptionHandlers.Any(exceptionHandler => exceptionHandler.FilterStart == nopInstr || exceptionHandler.HandlerEnd == nopInstr || exceptionHandler.HandlerStart == nopInstr || exceptionHandler.TryEnd == nopInstr || exceptionHandler.TryStart == nopInstr);
        }
    }
}