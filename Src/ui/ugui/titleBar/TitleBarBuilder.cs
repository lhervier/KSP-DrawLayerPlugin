using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    /// <summary>
    /// Right-side content of the popup's title bar: the "visible / total" count badge, then the ＋ (new
    /// marker) and ⚙ (settings) buttons. The title bar frame, the title on the left and the ✕ close
    /// button are provided by the shared PopupBuilder.
    /// </summary>
    public class TitleBarBuilder : IUGUIBuilder<TitleBarController>
    {
        private const string NewGlyph = "+";

        // ====================================
        // Builder parameters
        // ====================================

        private DrawLayerViewModel _viewModel;
        public TitleBarBuilder WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // ===================================
        // Build
        // ===================================

        public TitleBarController Build()
        {
            var rightColumnGo = new GameObject("DrawLayer.TitleBar.RightColumn", typeof(RectTransform));

            // Right column: count badge (first) then the ＋ ⚙ buttons. Width driven by the content
            // (no flexibleWidth), so the group stays pinned to the right of the shared title bar.
            var rightLayout = rightColumnGo.AddComponent<HorizontalLayoutGroup>();
            rightLayout.spacing = DefaultPalette.Spacing;
            rightLayout.childAlignment = TextAnchor.MiddleLeft;
            rightLayout.childControlWidth = true;
            rightLayout.childControlHeight = true;
            rightLayout.childForceExpandWidth = false;
            rightLayout.childForceExpandHeight = false;
            Transform right = rightColumnGo.transform;

            // "visible / total" count badge — first element of the right column
            TextMeshProUGUI countLabel = BuildCountBadge(right);

            // "New marker" button
            ButtonController add = NewButton("New", NewGlyph);
            add.OnClick.Add(() => _viewModel.NewMarker());
            add.transform.SetParent(right, false);
            Tooltips.Attach(add.gameObject, ModLocalization.GetString("DLM_buttonNew"));

            // "Settings" button
            ButtonController settings = NewButton("Settings", DefaultPalette.PickGlyph("⚙", "≡", "…", "*"));
            settings.OnClick.Add(() => _viewModel.OpenSettings());
            settings.transform.SetParent(right, false);
            Tooltips.Attach(settings.gameObject, ModLocalization.GetString("DLM_buttonSettings"));

            return rightColumnGo
                .AddComponent<TitleBarController>()
                .WithViewModel(_viewModel)
                .WithCountLabelComponent(countLabel);
        }

        // Square title-bar button matching the shared ✕ close button (same size and colors), so the
        // buttons of the title bar stay homogeneous.
        private static ButtonController NewButton(string objectName, string glyph)
        {
            return new ButtonBuilder()
                .WithObjectName(objectName)
                .WithLabel(glyph)
                .WithInteractableState(true)
                .WithBackgroundColor(PopupPalette.TitleBarButtonColor)
                .WithHoverColor(PopupPalette.TitleBarButtonHoverColor)
                .Build();
        }

        // Chip: sliced accent-border Image + accent Text. Size driven by content + padding.
        private TextMeshProUGUI BuildCountBadge(Transform parent)
        {
            var badgeGo = new GameObject("Count", typeof(RectTransform));
            badgeGo.transform.SetParent(parent, false);

            var image = badgeGo.AddComponent<Image>();
            image.sprite = SpritesGlobal.Border(DefaultPalette.AccentBgColor, DefaultPalette.AccentBorderColor, 1);
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;

            var layout = badgeGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DrawLayerPalette.CountPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.CountPaddingH),
                2, 2);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(badgeGo.transform, false);
            var label = UGUILabels.AddLabel(labelGo);
            label.fontSize = DrawLayerPalette.CountFontSize;
            label.color = DefaultPalette.AccentColor;
            label.alignment = TextAlignmentOptions.Center;

            Tooltips.Attach(badgeGo, ModLocalization.GetString("DLM_countTooltip"));

            return label;
        }
    }
}
