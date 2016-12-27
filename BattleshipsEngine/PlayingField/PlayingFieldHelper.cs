using System;
using System.Linq;
using System.Drawing;

using Battleships.Boxes;
using Battleships.Ships;
using Battleships.Exceptions;

namespace Battleships.PlayingField
{
	/// <summary>
	/// Spielfeld Helper: Hier werden andere/weitere Funktionen bereitgestellt
	/// </summary>
	public static class PlayingFieldHelper
	{
		/// <summary>
		/// Korrigiert den übergebenden Punkt der mitten in einem Kästchen liegen kann, auf einen Punkt des Kästchen, der zum zeichnen benutzt wird.
		/// </summary>
		/// <param name="point">Der Punkt der korrigiert werden soll.</param>
		/// <returns>Gibt die Position des Kästchen zurück.</returns>
		public static Point GetBoxPoint(Point point)
		{
			return GetBox(point).Rect.Location;
		}
		/// <summary>
		/// Erstellt aus einer X- und Y-Koordinate einen Punkt der relativ zur Kästchengröße ist.
		/// </summary>
		/// <param name="x">Die ungefähre X-Koordinate von der gestartet werden soll.</param>
		/// <param name="y">Die ungefähre Y-Koordinate von der gestartet werden soll.</param>
		/// <returns>Gibt einen korrigierten Punkt zurück.</returns>
		public static Point AdjustCoordinatesToBoxSize(int x, int y)
		{
			int startX = (int)Math.Round(x / (double)PlayingField.BoxSize.Width) * PlayingField.BoxSize.Width;
			int startY = (int)Math.Round(y / (double)PlayingField.BoxSize.Height) * PlayingField.BoxSize.Height;

			return new Point(startX, startY);
		}

		/// <summary>
		/// Findet die Richtung des Schiffes heraus, in der es auf dem Spielfeld liegt.
		/// </summary>
		/// <param name="ship">Das Schiff, dessen Richtung herausgefunden werden soll.</param>
		/// <returns>Gibt die Richtung des Schiffes zurück</returns>
		/// <exception cref="KnockingException">Das Schiff berührt andere Schiffe.</exception>
		public static ShipDirection GetShipDirection(Ship ship)
		{
			if (PlayingFieldController.IsNotFullyInitialized(ship))
				throw new NotInitializedException("Ship is not fully initialized!");

			Point start = ship.Boxes[0].Rect.Location;
			Point secondBox = ship.Boxes[1].Rect.Location;
			ShipDirection direction = ShipDirection.None;

			int dx = secondBox.X - start.X;
			int dy = secondBox.Y - start.Y;
			direction = Math.Abs(dx) > Math.Abs(dy) ?
				(dx > 0) ? ShipDirection.Right : ShipDirection.Left :
				(dy > 0) ? ShipDirection.Down : ShipDirection.Up;

			return direction;
		}

        /// <summary>
        /// Holt sich die Box an der Stelle des übergebenden Punktes.
        /// </summary>
        /// <param name="point">Der Punkt wo die Box liegt.</param>
        /// <returns>Gibt eine Box zurück, welche entweder von einem Schiff ist oder Wasser.</returns>
        public static Box GetBox(Point point)
        {
            Box hitBox = null;
            Box box = PlayingFieldController.Active.Water.FirstOrDefault(b => b.Rect.Contains(point));
            if (box != null && box.Rect != Rectangle.Empty)
                hitBox = box;
            else
            {
                Box sBox = null;
                PlayingFieldController.Active.Ships.ForEach(s => {
                    sBox = s.Boxes.FirstOrDefault(b => b.Rect.Contains(point));
                    if (sBox != null && sBox.Rect != Rectangle.Empty)
                        hitBox = sBox;
                });
            }

            return hitBox;
        }

        /// <summary>
        /// Holt sich das Schiff der übergebenden Box.
        /// </summary>
        /// <param name="shipBox">Die Box die überprüft werden soll.</param>
        /// <returns>Gibt das Schiff zurück, zu der die Box gehört.</returns>
        public static Ship GetShip(ShipBox shipBox)
        {
            foreach (var ship in PlayingFieldController.Active.Ships)
                if (ship.Boxes.Contains(shipBox))
                    return ship;

            return null;
        }

        /// <summary>
        /// Überprüft, ob alle Schiffe auf dem aktiven Spielfeld versunken sind.
        /// </summary>
        /// <returns>Gibt "true" zurück, wenn alle Schiffe auf dem aktiven Spielfeld versunken sind. Ansonsten "false".</returns>
        public static bool AreAllShipsSunk()
        {
            return PlayingFieldController.Active.Ships.Count > 0
                && PlayingFieldController.Active.Ships.TrueForAll(s => s.IsSunk);
        }
    }
}
