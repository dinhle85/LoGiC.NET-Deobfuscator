using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class Renamer : IProtection
    {
        public string Name => nameof(Renamer);
        public string Description => "Renames random names to \"not broken\" names?";
        private readonly HashSet<string> GeneratedNames = new();
        private static bool CanRename(object obj) =>
            obj switch
            {
                MethodDef {IsRuntimeSpecialName: true} => false,
                MethodDef method when method.DeclaringType.IsForwarder => false,
                MethodDef method => !method.IsConstructor && !method.IsStaticConstructor,
                PropertyDef {IsRuntimeSpecialName: true} => false,
                PropertyDef {IsEmpty: true} => false,
                PropertyDef propertyDef => !propertyDef.IsSpecialName,
                EventDef eventDef => eventDef.IsRuntimeSpecialName,
                FieldDef {IsRuntimeSpecialName: true} => false,
                FieldDef fieldDef => !fieldDef.IsLiteral || !fieldDef.DeclaringType.IsEnum,
                Parameter parameter => parameter.Name == string.Empty,
                _ => false
            };

        private string GenerateName()
        {
            var rnd = new Random();
            string generated;
            do 
            {
                generated = $"dio{rnd.Next(0, 999999999)}";
            } while (GeneratedNames.Contains(generated));
            GeneratedNames.Add(generated);
            return generated;
        }
        public void Execute(Context ctx)
        {
            ctx.Module.Name = Path.GetFileNameWithoutExtension(ctx.InputPath);
            foreach (var type in ctx.Module.GetTypes())
            {
                if (type.HasMethods)
                    foreach (var method in type.Methods.Where(m => CanRename(m) && m.Name.Length >= 50))
                        method.Name = GenerateName();

                if (type.HasProperties)
                    foreach (var property in type.Properties.Where(p => CanRename(p) && p.Name.Length >= 50))
                        property.Name = GenerateName();
                
                if (type.HasFields)
                    foreach (var field in type.Fields.Where(f => CanRename(f) && f.Name.Length >= 50))
                        field.Name = GenerateName();
                
                if (!type.HasEvents) continue;
                foreach (var ev in type.Events.Where(e => CanRename(e) && e.Name.Length >= 50))
                    ev.Name = GenerateName();
            }
        }
    }
}