namespace Remote_Content_Show_Container
{
    public enum RenderMessage
    {
        NotSupported = 0,
        Supported = 1,

        /// <summary>
        /// Indicates that the captured process has exited while capturing.
        /// </summary>
        ProcessExited = 2
    }
}
