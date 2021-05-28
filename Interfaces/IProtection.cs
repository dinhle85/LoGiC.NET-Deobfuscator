using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Interfaces
{
    public interface IProtection
    {
        public string Name { get; }
        public string Description { get; }
        public void Execute(Context ctx);
    }
}