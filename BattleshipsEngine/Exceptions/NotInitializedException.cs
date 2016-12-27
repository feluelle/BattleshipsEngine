using System;

namespace Battleships.Exceptions
{
    [Serializable]
    /// <summary>
    /// NotInitializedException: Eine Exception die geworfen werden kann wenn das Schiff nicht vollständig initialisiert wurde.
    /// </summary>
    public class NotInitializedException : ShipException
    {
        /// <summary>
        /// Erzeugt eine neue NotInitializedException mit der übergebenden Nachricht.
        /// </summary>
        /// <param name="message">Die Nachricht die ausgegeben werden soll, wenn diese Exception geworfen wird.</param>
        public NotInitializedException(string message) : base(message)
        {
        }
    }
}
