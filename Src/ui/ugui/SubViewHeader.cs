using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// Shared header for the replacing sub-views (editor / settings): a back arrow on the left followed
    /// by the view title. The caller wires the returned back button. Returns the header GameObject so it
    /// can be placed in a vertical layout (it carries a fixed-height LayoutElement).
    /// </summary>
    public static class SubViewHeader
    {
        private static string BackGlyph => DefaultPalette.PickGlyph("‹", "<");

        public static GameObject Build(Transform parent, string title, out ButtonController backButton, out TextMeshProUGUI titleLabel)
        {
            var headerGo = new GameObject("Header", typeof(RectTransform));
            headerGo.transform.SetParent(parent, false);

            var le = headerGo.AddComponent<LayoutElement>();
            le.minHeight = le.preferredHeight = DrawLayerPalette.HeaderHeight;

            var bg = headerGo.AddComponent<Image>();
            bg.sprite = SpritesGlobal.HorizontalBorders(
                DrawLayerPalette.HeaderBgColor,
                DrawLayerPalette.HeaderBorderColor,
                1);
            bg.type = Image.Type.Sliced;
            bg.color = Color.white;
            bg.raycastTarget = false;

            var layout = headerGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DrawLayerPalette.HeaderPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.HeaderPaddingH),
                0, 0);
            layout.spacing = DrawLayerPalette.HeaderSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            backButton = new ButtonBuilder()
                .WithObjectName("Back")
                .WithLabel(BackGlyph)
                .WithBackgroundColor(PopupPalette.TitleBarButtonColor)
                .WithHoverColor(PopupPalette.TitleBarButtonHoverColor)
                .Build();
            backButton.transform.SetParent(headerGo.transform, false);

            var titleGo = new GameObject("Title", typeof(RectTransform));
            titleGo.transform.SetParent(headerGo.transform, false);
            var titleLe = titleGo.AddComponent<LayoutElement>();
            titleLe.flexibleWidth = 1f;
            titleLabel = UGUILabels.AddLabel(titleGo);
            titleLabel.text = title.ToUpperInvariant();
            titleLabel.fontSize = DrawLayerPalette.HeaderTitleFontSize;
            titleLabel.fontStyle = FontStyles.Bold;
            titleLabel.color = DrawLayerPalette.HeaderTitleColor;
            titleLabel.alignment = TextAlignmentOptions.Left;

            return headerGo;
        }
    }
}
