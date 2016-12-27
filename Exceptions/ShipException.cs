using System;

namespace Battleships.Exceptions
{
    [Serializable]
    /// <summary>
    /// ShipException: Eine Exception die geworfen werden kann wenn es Probleme mit Schiffen gibt.
    /// </summary>
    public abstract class ShipException : Exception
    {
        /// <summary>
        /// Erzeugt eine neue ShipException mit der übergebenden Nachricht.
        /// </summary>
        /// <param name="message">Die Nachricht die ausgegeben werden soll, wenn diese Exception geworfen wird.</param>
        public ShipException(string message) : base(message)
		{
        }
    }
}
