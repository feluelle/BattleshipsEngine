using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

using Battleships.Engine.Ships;
using Battleships.Engine.Boxes;

namespace Battleships.Engine.PlayingField
{
    /// <summary>
    /// Sagt aus in welche Richtung das Schiff vom Startpunkt aus verläuft.
    /// </summary>
    public enum ShipDirection
    {
        Up, Down, Left, Right, None
    }

    [DataContract]
    /// <summary>
    /// Bezeichnet das Spielfeld beim Schiffe versenken.
    /// </summary>
    public class PlayingField
    {
        /// <summary>
        /// Definiert die Kästchengröße.
        /// </summary>
        public static readonly Size BoxSize = new Size(40, 40);

        /// <summary>
        /// Sagt aus, wo das Spielfeld anfängt.
        /// </summary>
        public readonly Point Location = new Point(0, 0);

        [DataMember(Name = "Size")]
        private Size size;
        /// <summary>
        /// Gibt die Größe des Spielfeldes an.
        /// </summary>
        public Size Size
        {
            get { return size; }
        }

        [DataMember(Name = "Ships")]
        private List<Ship> ships;
        /// <summary>
        /// Beinhaltet alle Schiffe auf dem Spielfeld.
        /// </summary>
        public List<Ship> Ships
        {
            get { return ships; }
        }

        [DataMember(Name = "Water")]
        private List<WaterBox> water;
        /// <summary>
        /// Beinhaltet das ganze Wasser auf dem Spielfeld.
        /// </summary>
        public List<WaterBox> Water
        {
            get { return water; }
        }

        /// <summary>
        /// Erzeugt ein neues Spielfeld bei angegebener Größe.
        /// </summary>
        /// <param name="size">Die Größe des Spielfeldes.</param>
        public PlayingField(Size size)
        {
            this.size = size;

            water = new List<WaterBox>();
            ships = new List<Ship>();
        }

        // Zum Serialisieren hinzugefügt...
        PlayingField() { }
    }
}
