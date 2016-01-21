namespace Remote_Content_Show_Container
{
    public enum RenderMessage
    {
        NotSupported,
        Supported,

        /// <summary>
        /// Indicates that the captured process has exited while capturing.
        /// </summary>
        ProcessExited
    }
}
