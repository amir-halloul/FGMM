using CitizenFX.Core;

namespace FGMM.SDK.Server.RPC
{
    /// <summary>
	/// Represents the ability send RPC events.
	/// </summary>
    public interface IRpcTrigger
    {
        /// <summary>
        /// Triggers the event with the specified payloads.
        /// </summary>
        /// <param name="payloads">The payloads to send with the event.</param>
        void Trigger(params object[] payloads);

        /// <summary>
        /// Triggers the event with the specified payloads for one client
        /// </summary>
        /// <param name="payloads">The payloads to send with the event.</param>
        void Trigger(Player player, params object[] payloads);

        /// <summary>
        /// Triggers the event with the specified payloads for one client
        /// </summary>
        /// <param name="payloads">The payloads to send with the event.</param>
        void Trigger(IClient client, params object[] payloads);
    }
}
