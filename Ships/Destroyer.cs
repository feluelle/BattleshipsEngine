using System.Collections.Generic;

using Battleships.Boxes;

namespace Battleships.Ships
{
	/// <summary>
	/// Schiffstyp: Zerstörer
	/// </summary>
	public class Destroyer : Ship
	{
		/// <summary>
		/// Erstellt einen Zerstörer (bestehend aus 3 Kästchen).
		/// </summary>
		public Destroyer()
		{
			Boxes = new List<ShipBox>(3);
		}
	}
}
