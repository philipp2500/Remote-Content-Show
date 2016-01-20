namespace ConfigManager
{
    public class ErrorEventHandler
    {
        public ErrorEventHandler(string text)
        {
            this.Text = text;
        }

        public string Text
        {
            get;
            private set;
        }
    }
}