using System;

namespace Battleships.Exceptions
{
    [Serializable]
    /// <summary>
    /// KnockingException: Eine Exception die geworfen werden kann wenn Schiffe sich berühren.
    /// </summary>
    public class KnockingException : ShipException
	{
		/// <summary>
		/// Erzeugt eine neue KnockingException mit der übergebenden Nachricht.
		/// </summary>
		/// <param name="message">Die Nachricht die ausgegeben werden soll, wenn diese Exception geworfen wird.</param>
		public KnockingException(string message) : base(message)
		{
		}
	}
}
