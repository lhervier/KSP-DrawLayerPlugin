using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.badge;
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
            BadgeController countBadge = BuildCountBadge(right);

            // "New marker" button
            ButtonController add = NewButton("New", NewGlyph);
            add.OnClick.Add(() => _viewModel.NewMarker());
            add.transform.SetParent(right, false);
            Tooltips.Attach(add.gameObject, ModLocalization.GetString("buttonNew"));

            // "Settings" button
            ButtonController settings = NewButton("Settings", DefaultPalette.PickGlyph("⚙", "≡", "…", "*"));
            settings.OnClick.Add(() => _viewModel.OpenSettings());
            settings.transform.SetParent(right, false);
            Tooltips.Attach(settings.gameObject, ModLocalization.GetString("buttonSettings"));

            return rightColumnGo
                .AddComponent<TitleBarController>()
                .WithViewModel(_viewModel)
                .WithCountBadge(countBadge);
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

        // "visible / total" accent count badge. Returns the badge so the controller keeps its text updated.
        private BadgeController BuildCountBadge(Transform parent)
        {
            return new BadgeBuilder()
                .WithParent(parent)
                .WithObjectName("Count")
                .WithFontSize(DrawLayerPalette.CountFontSize)
                .WithPadding(DrawLayerPalette.CountPaddingH, 2)
                .WithTooltip(ModLocalization.GetString("countTooltip"))
                .Build();
        }
    }
}
