namespace Battleships.Engine.Boxes
{
    /// <summary>
    /// Zonenkästchen
    /// </summary>
    public class ZoneBox : Box
    {
        /// <summary>
        /// Erzeugt ein Zonenkästchen an der angegebenen Position.
        /// </summary>
        /// <param name="x">X-Koordinate des Zonenkästchen</param>
        /// <param name="y">Y-Koordinate des Zonenkästchen</param>
        public ZoneBox(int x, int y) : base(x, y)
        {
            Kind = BoxKind.Water;
        }

        // Zum Serialisieren hinzugefügt...
        ZoneBox() { }
    }
}
