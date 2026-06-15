using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.scrollableview;
using com.github.lhervier.ksp.shared.ugui.sprites;

namespace com.github.lhervier.ksp.ui.ugui.editor
{
    /// <summary>
    /// Editor sub-view: a back-arrow header, a scrollable form (built by EditorFormBuilder) and a footer
    /// with Cancel / Save. The form binds itself to the editing draft; this builder wires navigation.
    /// </summary>
    public class EditorBuilder : IUGUIBuilder<EditorController>
    {
        private DrawLayerViewModel _viewModel;
        public EditorBuilder ViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public EditorController Build()
        {
            var rootGo = new GameObject("DrawLayer.Editor", typeof(RectTransform));
            var layout = rootGo.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Header (back + title)
            SubViewHeader.Build(rootGo.transform, ModLocalization.GetString("editorTitleNew"),
                out ButtonController backButton, out TextMeshProUGUI titleLabel);

            // Scrollable form — fills the height between header and footer
            ScrollableViewController scroll = new ScrollableViewBuilder<EditorFormController>()
                .ObjectName("EditorScroll")
                .ContentBuilder(new EditorFormBuilder().ViewModel(_viewModel))
                .Build();
            scroll.transform.SetParent(rootGo.transform, false);
            var scrollLe = scroll.gameObject.AddComponent<LayoutElement>();
            scrollLe.flexibleHeight = 1f;
            scrollLe.minHeight = 0f;

            // Footer (Cancel / Save)
            ButtonController cancelButton;
            ButtonController saveButton;
            BuildFooter(rootGo.transform, out cancelButton, out saveButton);

            return rootGo
                .AddComponent<EditorController>()
                .ViewModel(_viewModel)
                .Header(titleLabel)
                .Buttons(backButton, cancelButton, saveButton);
        }

        private static void BuildFooter(Transform parent, out ButtonController cancelButton, out ButtonController saveButton)
        {
            var footerGo = new GameObject("Footer", typeof(RectTransform));
            footerGo.transform.SetParent(parent, false);

            var bg = footerGo.AddComponent<Image>();
            bg.sprite = SpritesGlobal.HorizontalBorders(DrawLayerPalette.FooterBgColor, DrawLayerPalette.FooterBorderColor, 1);
            bg.type = Image.Type.Sliced;
            bg.color = Color.white;
            bg.raycastTarget = false;

            var layout = footerGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DrawLayerPalette.FooterPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.FooterPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.FooterPaddingV),
                Mathf.RoundToInt(DrawLayerPalette.FooterPaddingV));
            layout.spacing = DrawLayerPalette.FooterSpacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            cancelButton = NewFooterButton("Cancel", ModLocalization.GetString("buttonCancel"),
                DrawLayerPalette.FooterCancelTextColor, DrawLayerPalette.FooterCancelBgColor, DrawLayerPalette.FooterCancelHoverColor);
            cancelButton.transform.SetParent(footerGo.transform, false);

            saveButton = NewFooterButton("Save", ModLocalization.GetString("buttonSave"),
                DrawLayerPalette.FooterOkTextColor, DrawLayerPalette.FooterOkBgColor, DrawLayerPalette.FooterOkHoverColor);
            saveButton.transform.SetParent(footerGo.transform, false);
        }

        // Full-width footer button: ButtonBuilder in fixed mode, then the LayoutElement width is relaxed
        // so the two buttons share the footer width equally.
        private static ButtonController NewFooterButton(string objectName, string label, Color textColor, Color bgColor, Color hoverColor)
        {
            ButtonController button = new ButtonBuilder()
                .ObjectName(objectName)
                .Label(label)
                .Size(DrawLayerPalette.FooterButtonHeight)
                .FontSize(DrawLayerPalette.FooterButtonFontSize)
                .TextColor(textColor)
                .BackgroundColor(bgColor)
                .HoverColor(hoverColor)
                .Build();

            var le = button.GetComponent<LayoutElement>();
            le.minWidth = 0f;
            le.preferredWidth = 0f;
            le.flexibleWidth = 1f;
            return button;
        }
    }
}
