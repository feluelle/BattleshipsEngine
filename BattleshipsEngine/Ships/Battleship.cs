using System.Collections.Generic;

using Battleships.Boxes;

namespace Battleships.Ships
{
	/// <summary>
	/// Schiffstyp: Schlachtschiff
	/// </summary>
	public class Battleship : Ship
	{
		/// <summary>
		/// Erstellt ein Schlachtschiff (bestehend aus 5 Kästchen).
		/// </summary>
		public Battleship()
		{
			Boxes = new List<ShipBox>(5);
		}
	}
}
