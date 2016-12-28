using System.Runtime.Serialization;

namespace Battleships.Engine.Boxes
{
    /// <summary>
    /// Wasserkästchen
    /// </summary>
    public class WaterBox : Box
    {
        /// <summary>
        /// Erzeugt ein Wasserkästchen an der angegebenen Position.
        /// </summary>
        /// <param name="x">X-Koordinate des Wasserkästchen</param>
        /// <param name="y">Y-Koordinate des Wasserkästchen</param>
        public WaterBox(int x, int y) : base(x, y)
        {
            Kind = BoxKind.Water;
        }

        // Zum Serialisieren hinzugefügt...
        WaterBox() { }
    }
}
