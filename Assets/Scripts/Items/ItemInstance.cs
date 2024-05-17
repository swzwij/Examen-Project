using System;

namespace Examen.Items
{
    [Serializable]
    public struct ItemInstance 
    {
        private string _name;

        public string Name => _name;

        public ItemInstance(string name)
        {
            _name = name;
        }
    }
}