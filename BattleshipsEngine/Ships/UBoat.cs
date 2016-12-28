using System.Collections.Generic;

using Battleships.Engine.Boxes;

namespace Battleships.Engine.Ships
{
	/// <summary>
	/// Schiffstyp: U-Boot
	/// </summary>
	public class UBoat : Ship
	{
		/// <summary>
		/// Erstellt ein U-Boot (bestehend aus 2 Kästchen).
		/// </summary>
		public UBoat()
		{
			Boxes = new List<ShipBox>(2);
		}
	}
}
