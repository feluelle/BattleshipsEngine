namespace Battleships.Boxes
{
	/// <summary>
	/// Schiffskästchen
	/// </summary>
	public class ShipBox : Box
	{
		/// <summary>
		/// Erzeugt ein Schiffskästchen an der angegebenen Position.
		/// </summary>
		/// <param name="x">X-Koordinate des Schiffskästchen</param>
		/// <param name="y">Y-Koordinate des Schiffskästchen</param>
		public ShipBox(int x, int y) : base(x, y)
		{
			Kind = BoxKind.Ship;
		}

		// Zum Serialisieren hinzugefügt...
		ShipBox() { }
	}
}
