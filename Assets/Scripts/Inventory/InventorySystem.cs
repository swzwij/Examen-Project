
namespace Examen_Project.Inventory
{
    public static class InventorySystem
    {
        public static int WoodCount { get; private set; }
        public static int StoneCount { get; private set; }

        /// <summary>
        /// Add given wood amount to the current wood count.
        /// </summary>
        /// <param name="amountOfWood"> amount of wood you want to add.</param>
        public static void AddWood(int amountOfWood) => WoodCount += amountOfWood;

        /// <summary>
        /// remove given wood amount to the current wood count.
        /// </summary>
        /// <param name="amountOfWood"> amount of wood you want to remove.</param>
        public static void RemoveWood(int amountOfWood) => WoodCount -= amountOfWood;

        /// <summary>
        /// Add given stone amount to the current stone count.
        /// </summary>
        /// <param name="amountOfStone"> amount of stone you want to add.</param>
        public static void AddStone(int amountOfStone) => StoneCount += amountOfStone;

        /// <summary>
        /// remove given stone amount to the current stone count.
        /// </summary>
        /// <param name="amountOfStone"> amount of stone you want to remove.</param>
        public static void RemoveStone(int amountOfStone) => StoneCount -= amountOfStone;
    }
}
