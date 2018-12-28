namespace FGMM.SDK.Server.RPC
{
    /// <summary>
	/// Represents a received RPC event.
	/// </summary>
    public interface IRpcEvent
    {
        /// <summary>
        /// Gets the event name.
        /// </summary>
        /// <value>
        /// The event name.
        /// </value>
        string Event { get; }

        /// <summary>
        /// Gets the client that sent the event.
        /// </summary>
        /// <value>
        /// The client that sent the event.
        /// </value>
        IClient Client { get; }

        /// <summary>
        /// Replies to the event with the specified payloads.
        /// </summary>
        /// <param name="payloads">The payloads to reply to the event with.</param>
        void Reply(params object[] payloads);
    }
}
