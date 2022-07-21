namespace CollapsedLight.WebApp.Components.Styles
{
    public record ButtonStyle(string Button, string Text, string TextStyle)
    {
        public ButtonStyle()
            : this(string.Empty, string.Empty, string.Empty)
        {}
    };

    public static class ClButtonStyles
    {
        public static ButtonStyle Secondary(this ClButtonDyn style, string Label)
        {
            return new ButtonStyle 
            {
                Button = "bg-secondary hover:drop-shadow-md text-secondary-on rounded-lg",
                Text = Label,
                TextStyle = "flex justify-center text-xl text-gray-300 test-center"
            };
        }

        public static ButtonStyle Tertiary(this ClButtonDyn style, string Label)
        {
            return new ButtonStyle
            {
                Button = "bg-tertiary hover:drop-shadow-md text-tertiary-on",
                Text = Label,
                TextStyle = "text-lg text-gray-300"
            };
        }
    }
}
