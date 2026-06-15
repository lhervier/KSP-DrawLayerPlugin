using UnityEngine;
using com.github.lhervier.ksp.shared.ugui.styles;
using static com.github.lhervier.ksp.shared.ugui.styles.Utils;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>
    /// Colors and metrics of the DrawLayer uGUI interface, mirroring the validated mockup
    /// (drawlayer_mockup.html): dark theme, accent green #8dbe45. No color/dimension is hard-coded in
    /// the builders: everything comes from here (or from the shared DefaultPalette for common values).
    /// </summary>
    public static class DrawLayerPalette
    {
        // ==============================================================
        // Main window
        // ==============================================================
        public const float WindowWidth = 360f;
        public const float WindowHeight = 520f;

        // ==============================================================
        // Title bar
        // ==============================================================
        public const int CountFontSize = 10;
        public const float CountPaddingH = 6f;

        // ==============================================================
        // Sub-view header (back arrow + title), used by editor and settings
        // ==============================================================
        public const float HeaderHeight = 30f;
        public const int HeaderTitleFontSize = 11;
        public const float HeaderPaddingH = 8f;
        public const float HeaderSpacing = 7f;
        public static readonly Color HeaderBgColor = Rgb(26, 26, 26);          // #1a1a1a
        public static readonly Color HeaderBorderColor = Rgb(34, 34, 34);      // #222
        public static readonly Color HeaderTitleColor = Rgb(221, 221, 221);    // #ddd

        // ==============================================================
        // List rows
        // ==============================================================
        public const float RowPaddingH = 8f;
        public const float RowPaddingV = 7f;
        public const float RowSpacing = 8f;
        public static readonly Color RowSeparatorColor = Rgb(28, 28, 28);      // #1c1c1c
        public static readonly Color RowHoverColor = Rgba(255, 255, 255, 0.03f);

        // Visibility checkbox
        public const float CheckSize = 15f;
        public const int CheckBorderThickness = 1;
        public static readonly Color CheckBgColor = Rgb(26, 26, 26);           // #1a1a1a
        public static readonly Color CheckBorderColor = Rgb(85, 85, 85);       // #555
        public static readonly Color CheckOnBgColor = Rgba(141, 190, 69, 0.15f);
        public static readonly Color CheckOnBorderColor = Rgb(74, 110, 32);    // accent border

        // Type icon (colored glyph in a bordered box)
        public const float TypeIconSize = 22f;
        public const int TypeIconBorderThickness = 1;
        public const int TypeIconFontSize = 13;
        public static readonly Color TypeIconBgColor = Rgb(31, 31, 31);        // #1f1f1f
        public static readonly Color TypeIconBorderColor = Rgb(68, 68, 68);    // #444

        // Name + meta
        public const int NameFontSize = 13;
        public const int MetaFontSize = 10;
        public static readonly Color NameColor = Rgb(221, 221, 221);           // #ddd
        public static readonly Color MetaColor = Rgb(119, 119, 119);           // #777
        public static readonly Color HiddenAlphaColor = new Color(1f, 1f, 1f, 0.4f);

        // Color swatch dot on the meta line
        public const float SwatchSize = 9f;

        // Row buttons revealed on hover (✕)
        public const float RowButtonSize = 18f;
        public const int RowButtonFontSize = 11;
        public static readonly Color RowButtonBgColor = Rgb(42, 42, 42);       // #2a2a2a
        public static readonly Color RowButtonHoverColor = Rgb(56, 56, 56);    // #383838
        public static readonly Color RowButtonDangerHoverColor = Rgb(90, 36, 29); // #5a241d

        // Empty state
        public const int EmptyTitleFontSize = 12;
        public const int EmptyHintFontSize = 12;
        public const float EmptyPadding = 22f;
        public static readonly Color EmptyTitleColor = Rgb(170, 170, 170);     // #aaa
        public static readonly Color EmptyHintColor = Rgb(119, 119, 119);      // #777

        // ==============================================================
        // Editor / settings form
        // ==============================================================
        public const float SectionPaddingH = 11f;
        public const float SectionPaddingV = 12f;
        public const float FieldSpacing = 11f;
        public const int SectionLabelFontSize = 11;
        public const int FieldLabelFontSize = 11;
        public const int FieldValueFontSize = 11;
        public static readonly Color SectionLabelColor = DefaultPalette.AccentColor;
        public static readonly Color FieldLabelColor = Rgb(153, 153, 153);     // #999
        public static readonly Color FieldValueColor = DefaultPalette.AccentColor;
        public static readonly Color SeparatorColor = Rgb(34, 34, 34);         // #222

        // Type selector (segmented control)
        public const float SegmentHeight = 30f;
        public const int SegmentFontSize = 12;
        public const int SegmentBorderThickness = 1;
        public static readonly Color SegmentBgColor = Rgb(31, 31, 31);         // #1f1f1f
        public static readonly Color SegmentBorderColor = Rgb(85, 85, 85);     // #555
        public static readonly Color SegmentHoverColor = Rgb(42, 42, 42);      // #2a2a2a
        public static readonly Color SegmentSelectedColor = Rgba(141, 190, 69, 0.12f);
        public static readonly Color SegmentTextColor = Rgb(153, 153, 153);    // #999
        public static readonly Color SegmentSelectedTextColor = DefaultPalette.AccentColor;

        // Color grid
        public const int ColorGridColumns = 8;
        public const float ColorCellSize = 26f;
        public const float ColorCellSpacing = 5f;
        public const int ColorCellBorderThickness = 1;
        public const int ColorNameFontSize = 10;
        public static readonly Color ColorCellBorderColor = Rgba(255, 255, 255, 0.15f);
        public static readonly Color ColorCellSelectedBorderColor = Rgb(255, 255, 255);
        public static readonly Color ColorNameColor = Rgb(136, 136, 136);      // #888

        // Footer (Save / Cancel)
        public const float FooterPaddingH = 11f;
        public const float FooterPaddingV = 9f;
        public const float FooterSpacing = 8f;
        public const float FooterButtonHeight = 30f;
        public const int FooterButtonFontSize = 12;
        public static readonly Color FooterBgColor = PopupPalette.TitleBarBackgroundColor;     // #2e2e2e
        public static readonly Color FooterBorderColor = PopupPalette.TitleBarSeparatorColor;  // #444
        public static readonly Color FooterOkTextColor = DefaultPalette.AccentColor;
        public static readonly Color FooterOkBgColor = Rgba(141, 190, 69, 0.12f);
        public static readonly Color FooterOkHoverColor = Rgba(141, 190, 69, 0.22f);
        public static readonly Color FooterCancelTextColor = Rgb(187, 187, 187); // #bbb
        public static readonly Color FooterCancelBgColor = Rgb(56, 56, 56);      // #383838
        public static readonly Color FooterCancelHoverColor = Rgb(72, 72, 72);   // #484848

        // ==============================================================
        // Settings
        // ==============================================================
        public const int SettingsRowFontSize = 12;
        public static readonly Color SettingsRowColor = Rgb(187, 187, 187);    // #bbb
        public const int SettingsHintFontSize = 11;
        public const float SettingsHintPaddingH = 9f;
        public const float SettingsHintPaddingV = 7f;
        public const int SettingsHintBorderThickness = 2;       // left accent bar
        public static readonly Color SettingsHintBgColor = Rgb(24, 24, 24);    // #181818
        public static readonly Color SettingsHintBorderColor = Rgb(74, 110, 32); // accent border
        public static readonly Color SettingsHintTextColor = Rgb(153, 153, 153); // #999
    }
}
