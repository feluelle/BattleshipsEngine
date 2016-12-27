using System.Collections.Generic;
using System.Runtime.Serialization;

using Battleships.Boxes;

namespace Battleships.Ships
{
    // Zum Serialisieren hinzugefügt...
    [KnownType(typeof(Battleship))]
    [KnownType(typeof(Cruiser))]
    [KnownType(typeof(Destroyer))]
    [KnownType(typeof(UBoat))]
    [DataContract]
    /// <summary>
    /// Ein grundlegendes Schiff
    /// </summary>
    public abstract class Ship
    {
        [DataMember(Name = "Boxes")]
        private List<ShipBox> boxes;
        /// <summary>
        /// Die zum Schiff gehörenden Kästchen
        /// </summary>
        public List<ShipBox> Boxes
        {
            get { return boxes; }
            set { boxes = value; }
        }

        //[DataMember(Name = "IsSunk")]
        //private bool isSunk;
        /// <summary>
        /// Sagt aus, ob das Schiff versunken ist oder nicht
        /// </summary>
        public bool IsSunk
        {
            get { return boxes.TrueForAll(b => b.IsHit); }
        }
    }
}
