using System.Drawing;
using System.Runtime.Serialization;

using Battleships.Engine.PlayingField;

namespace Battleships.Engine.Boxes
{
    /// <summary>
    /// Sagt über Kästchen aus, ob diese zu einem Schiff gehören oder zu Wasser.
    /// </summary>
    public enum BoxKind
    {
        Ship, Water
    }

    [DataContract]
    /// <summary>
    /// Ein grundlegendes Kästchen
    /// </summary>
    public abstract class Box
    {
        [DataMember(Name = "Rect")]
        private Rectangle rect;
        /// <summary>
        /// Das zu zeichende Objekt, das jedes Kästchen auf dem Spielfeld darstellt.
        /// </summary>
        public Rectangle Rect
        {
            get { return rect; }
        }

        [DataMember(Name = "IsHit")]
        private bool isHit = false;
        /// <summary>
        /// Sagt aus, ob das Kästchen schon einmal angeklickt wurde oder nicht.
        /// </summary>
        public bool IsHit
        {
            get { return isHit; }
            set { isHit = value; }
        }

        [DataMember(Name = "Kind")]
        private BoxKind kind = BoxKind.Water;
        /// <summary>
        /// Definiert die Zugehörigkeit des Kästchen.
        /// </summary>
        public BoxKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        /// <summary>
        /// Erstellt ein neues Kästchen an angegebener Position.
        /// </summary>
        /// <param name="x">Die X-Koordinate des Kästchen</param>
        /// <param name="y">Die Y-Koordinate des Kästchen</param>
        public Box(int x, int y)
        {
            rect = new Rectangle(x, y, PlayingField.PlayingField.BoxSize.Width, PlayingField.PlayingField.BoxSize.Height);
        }

        // Zum Serialisieren hinzugefügt...
        internal Box() { }
    }
}
