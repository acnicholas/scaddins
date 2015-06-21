namespace SCaddins.SCulcase
{
    using System;
    
    [Flags]
    public enum ConversionTypes
    {
        None = 0,
        Text = 1,
        SheetNames = 2,
        ViewNames = 4,
        TitlesOnSheets = 8,
        RoomNames = 16
    }

    public enum ConversionMode
    {
        UpperCase,
        LowerCase,
        TitleCase
    }
}
