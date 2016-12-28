using System.Collections.Generic;

using Battleships.Engine.Boxes;

namespace Battleships.Engine.Ships
{
	/// <summary>
	/// Schiffstyp: Kreuzer
	/// </summary>
	public class Cruiser : Ship
	{
		/// <summary>
		/// Erstellt einen Kreuzer (bestehend aus 4 Kästchen).
		/// </summary>
		public Cruiser()
		{
			Boxes = new List<ShipBox>(4);
		}
	}
}
