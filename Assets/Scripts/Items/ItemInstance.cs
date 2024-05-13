namespace Examen.Items
{
    public readonly struct ItemInstance 
    {
        private readonly string _name;

        public readonly string Name => _name;

        public ItemInstance(string name)
        {
            _name = name;
        }
    }
}