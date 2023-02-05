namespace AzyWorks.Utilities
{
    /// <summary>
    /// Servers as an identifier of the currently running emulation runtime.
    /// </summary>
    public enum WinEmuFlag
    {
        /// <summary>
        /// There are no emulation runtimes currently running.
        /// </summary>
        None,

        /// <summary>
        /// This process is running under the Wine emulator.
        /// </summary>
        Wine
    }
}