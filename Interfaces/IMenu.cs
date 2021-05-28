namespace LogicDeobfuscator.Interfaces
{
    public interface IMenu
    {
        public string Name { get; }
        public void Show();
        public void Hide();
    }
}