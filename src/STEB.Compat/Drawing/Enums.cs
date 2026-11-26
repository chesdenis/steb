namespace System.Drawing
{
    [Flags]
    public enum FontStyle { Regular = 0, Bold = 1, Italic = 2, Underline = 4, Strikeout = 8 }

    public enum GraphicsUnit { World, Display, Pixel, Point, Inch, Document, Millimeter }

    public enum ContentAlignment
    {
        TopLeft = 1, TopCenter = 2, TopRight = 4,
        MiddleLeft = 16, MiddleCenter = 32, MiddleRight = 64,
        BottomLeft = 256, BottomCenter = 512, BottomRight = 1024
    }
}

namespace System.Drawing.Drawing2D
{
    public enum DashStyle { Solid, Dash, Dot, DashDot, DashDotDot, Custom }

    public enum LineCap
    {
        Flat = 0, Square = 1, Round = 2, Triangle = 3,
        NoAnchor = 16, SquareAnchor = 17, RoundAnchor = 18, DiamondAnchor = 19, ArrowAnchor = 20,
        AnchorMask = 240, Custom = 255
    }

    public enum SmoothingMode { Invalid = -1, Default = 0, HighSpeed = 1, HighQuality = 2, None = 3, AntiAlias = 4 }

    public enum HatchStyle
    {
        Horizontal = 0, Vertical = 1, ForwardDiagonal = 2, BackwardDiagonal = 3, Cross = 4, DiagonalCross = 5,
        Percent05 = 6, Percent10 = 7, Percent20 = 8, Percent25 = 9, Percent30 = 10, Percent40 = 11,
        Percent50 = 12, Percent60 = 13, Percent70 = 14, Percent75 = 15, Percent80 = 16, Percent90 = 17,
        LightDownwardDiagonal = 18, LightUpwardDiagonal = 19, DarkDownwardDiagonal = 20, DarkUpwardDiagonal = 21,
        WideDownwardDiagonal = 22, WideUpwardDiagonal = 23, LightVertical = 24, LightHorizontal = 25,
        NarrowVertical = 26, NarrowHorizontal = 27, DarkVertical = 28, DarkHorizontal = 29,
        DashedDownwardDiagonal = 30, DashedUpwardDiagonal = 31, DashedHorizontal = 32, DashedVertical = 33,
        SmallConfetti = 34, LargeConfetti = 35, ZigZag = 36, Wave = 37, DiagonalBrick = 38, HorizontalBrick = 39,
        Weave = 40, Plaid = 41, Divot = 42, DottedGrid = 43, DottedDiamond = 44, Shingle = 45, Trellis = 46,
        Sphere = 47, SmallGrid = 48, SmallCheckerBoard = 49, LargeCheckerBoard = 50, OutlinedDiamond = 51,
        SolidDiamond = 52,
        Min = 0, Max = 4, LargeGrid = 4
    }
}
