namespace FastColoredTextBoxNS
{
    /// <summary>
    /// Char and style
    /// </summary>
    public struct Char(char c)
    {
        /// <summary>
        /// Unicode character
        /// </summary>
        public char C = c;

        /// <summary>
        /// Style bit mask
        /// </summary>
        /// <remarks>Bit 1 in position n means that this char will rendering by FastColoredTextBox.Styles[n]</remarks>
        public StyleIndex Style = StyleIndex.None;
    }
}