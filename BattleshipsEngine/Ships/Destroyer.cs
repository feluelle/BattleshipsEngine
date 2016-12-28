using System.Collections.Generic;

using Battleships.Engine.Boxes;

namespace Battleships.Engine.Ships
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
