using System;

namespace Battleships.Engine.Exceptions
{
    [Serializable]
	/// <summary>
	/// NotOnPlayingFieldException: Eine Exception die geworfen werden kann wenn sich das Spiel nicht auf dem Spielfeld befindet.
	/// </summary>
	public class NotOnPlayingFieldException : ShipException
    {
		/// <summary>
		/// Erzeugt eine neue NotOnPlayingFieldException mit der übergebenden Nachricht.
		/// </summary>
		/// <param name="message">Die Nachricht die ausgegeben werden soll, wenn diese Exception geworfen wird.</param>
		public NotOnPlayingFieldException(string message) : base(message)
		{
		}
	}
}
