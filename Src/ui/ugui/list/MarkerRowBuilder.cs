using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.checkbox;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui.list
{
    /// <summary>
    /// One marker row: visibility checkbox + colored type icon + name and meta line (color swatch +
    /// type/details). A delete button is revealed on hover; clicking the row opens the editor. Hidden
    /// markers are dimmed.
    /// </summary>
    public class MarkerRowBuilder : IUGUIBuilder<MarkerRowController>
    {
        private static string CrossGlyph => DefaultPalette.PickGlyph("✛", "✚", "+");
        private static string CircleGlyph => DefaultPalette.PickGlyph("◎", "◯", "O");
        private static string RemoveLabel => DefaultPalette.PickGlyph("✕", "✗", "×", "x");

        // ===========================================
        // Builder parameters
        // ===========================================

        private DrawLayerViewModel _viewModel;
        public MarkerRowBuilder ViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private VisualMarker _marker;
        public MarkerRowBuilder Marker(VisualMarker marker)
        {
            this._marker = marker;
            return this;
        }

        private int _index;
        public MarkerRowBuilder Index(int index)
        {
            this._index = index;
            return this;
        }

        // ==========================================
        // Build
        // ==========================================

        public MarkerRowController Build()
        {
            Color markerColor = _marker.color.ToColor();
            bool visible = _marker.visible;

            var rowGo = new GameObject("Row", typeof(RectTransform));

            var bg = rowGo.AddComponent<Image>();
            bg.sprite = SpritesGlobal.FillSprite;
            bg.type = Image.Type.Simple;
            bg.color = Color.clear;
            bg.raycastTarget = true;   // catches clicks/hover over the row's empty areas

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DrawLayerPalette.RowPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.RowPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.RowPaddingV),
                Mathf.RoundToInt(DrawLayerPalette.RowPaddingV));
            layout.spacing = DrawLayerPalette.RowSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // 1px bottom separator (overlaid, out of layout)
            BuildSeparator(rowGo.transform);

            // Visibility checkbox (shared component)
            CheckboxController checkbox = new CheckboxBuilder()
                .Checked(visible)
                .Build();
            checkbox.transform.SetParent(rowGo.transform, false);
            Tooltips.Attach(checkbox.gameObject, ModLocalization.GetString("tooltipVisible"));

            // Colored type icon
            BuildTypeIcon(rowGo.transform, markerColor);

            // Info column: name + meta line (flexible width, no raycast → row click bubbles to bg)
            var infoGo = new GameObject("Info", typeof(RectTransform));
            infoGo.transform.SetParent(rowGo.transform, false);
            var infoLe = infoGo.AddComponent<LayoutElement>();
            infoLe.flexibleWidth = 1f;
            var infoLayout = infoGo.AddComponent<VerticalLayoutGroup>();
            infoLayout.spacing = 1f;
            infoLayout.childAlignment = TextAnchor.MiddleLeft;
            infoLayout.childControlWidth = true;
            infoLayout.childControlHeight = true;
            infoLayout.childForceExpandWidth = true;
            infoLayout.childForceExpandHeight = false;

            var nameGo = new GameObject("Name", typeof(RectTransform));
            nameGo.transform.SetParent(infoGo.transform, false);
            var name = UGUILabels.AddLabel(nameGo);
            name.text = _marker.name;
            name.fontSize = DrawLayerPalette.NameFontSize;
            name.color = Dim(DrawLayerPalette.NameColor, visible);
            name.alignment = TextAlignmentOptions.Left;

            BuildMetaLine(infoGo.transform, markerColor, visible);

            // Delete button revealed on hover
            CanvasGroup buttonsGroup = BuildRowButtons(rowGo.transform, out ButtonController removeButton);

            var pointer = rowGo.AddComponent<PointerHandler>();

            return rowGo
                .AddComponent<MarkerRowController>()
                .ViewModel(_viewModel)
                .Index(_index)
                .Background(bg)
                .ButtonsGroup(buttonsGroup)
                .PointerHandler(pointer)
                .Checkbox(checkbox)
                .RemoveButtonController(removeButton);
        }

        // ----------------------------------------------------------------

        private static void BuildSeparator(Transform parent)
        {
            var go = new GameObject("Separator", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(0f, 1f);
            rect.anchoredPosition = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.sprite = SpritesGlobal.FillSprite;
            img.type = Image.Type.Simple;
            img.color = DrawLayerPalette.RowSeparatorColor;
            img.raycastTarget = false;
        }

        private void BuildTypeIcon(Transform parent, Color markerColor)
        {
            var boxGo = new GameObject("TypeIcon", typeof(RectTransform));
            boxGo.transform.SetParent(parent, false);
            var le = boxGo.AddComponent<LayoutElement>();
            le.minWidth = le.preferredWidth = DrawLayerPalette.TypeIconSize;
            le.minHeight = le.preferredHeight = DrawLayerPalette.TypeIconSize;

            var box = boxGo.AddComponent<Image>();
            box.sprite = SpritesGlobal.Border(
                DrawLayerPalette.TypeIconBgColor,
                DrawLayerPalette.TypeIconBorderColor,
                DrawLayerPalette.TypeIconBorderThickness);
            box.type = Image.Type.Sliced;
            box.color = Color.white;
            box.raycastTarget = false;

            var glyphGo = new GameObject("Glyph", typeof(RectTransform));
            glyphGo.transform.SetParent(boxGo.transform, false);
            var glyphRect = glyphGo.GetComponent<RectTransform>();
            glyphRect.anchorMin = Vector2.zero;
            glyphRect.anchorMax = Vector2.one;
            glyphRect.offsetMin = Vector2.zero;
            glyphRect.offsetMax = Vector2.zero;
            var glyph = UGUILabels.AddLabel(glyphGo);
            glyph.text = _marker.type == MarkerType.CrossLines ? CrossGlyph : CircleGlyph;
            glyph.fontSize = DrawLayerPalette.TypeIconFontSize;
            glyph.color = markerColor;
            glyph.alignment = TextAlignmentOptions.Center;
        }

        private void BuildMetaLine(Transform parent, Color markerColor, bool visible)
        {
            var lineGo = new GameObject("Meta", typeof(RectTransform));
            lineGo.transform.SetParent(parent, false);
            var layout = lineGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Color swatch dot
            var swatchGo = new GameObject("Swatch", typeof(RectTransform));
            swatchGo.transform.SetParent(lineGo.transform, false);
            var swatchLe = swatchGo.AddComponent<LayoutElement>();
            swatchLe.minWidth = swatchLe.preferredWidth = DrawLayerPalette.SwatchSize;
            swatchLe.minHeight = swatchLe.preferredHeight = DrawLayerPalette.SwatchSize;
            var swatch = swatchGo.AddComponent<Image>();
            swatch.sprite = SpritesGlobal.FillSprite;
            swatch.type = Image.Type.Simple;
            swatch.color = Dim(markerColor, visible);
            swatch.raycastTarget = false;

            var metaGo = new GameObject("MetaText", typeof(RectTransform));
            metaGo.transform.SetParent(lineGo.transform, false);
            var metaLe = metaGo.AddComponent<LayoutElement>();
            metaLe.flexibleWidth = 1f;
            var meta = UGUILabels.AddLabel(metaGo);
            meta.text = BuildMetaText();
            meta.fontSize = DrawLayerPalette.MetaFontSize;
            meta.color = Dim(DrawLayerPalette.MetaColor, visible);
            meta.alignment = TextAlignmentOptions.Left;
        }

        private string BuildMetaText()
        {
            string typeName = ModLocalization.GetString(
                _marker.type == MarkerType.CrossLines ? "typeCross" : "typeCircle");
            string details;
            if (_marker.type == MarkerType.CrossLines)
            {
                details = ModLocalization.GetString("metaCrossPosition", Fmt(_marker.positionX), Fmt(_marker.positionY));
            }
            else if (_marker.divisions > 1)
            {
                details = ModLocalization.GetString("metaCircleGrad", Fmt(_marker.radius), _marker.divisions);
            }
            else
            {
                details = ModLocalization.GetString("metaCircleNoGrad", Fmt(_marker.radius));
            }
            return typeName + "  ·  " + details;
        }

        private CanvasGroup BuildRowButtons(Transform parent, out ButtonController removeButton)
        {
            var groupGo = new GameObject("RowButtons", typeof(RectTransform));
            groupGo.transform.SetParent(parent, false);

            var layout = groupGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 3f;
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var group = groupGo.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;

            removeButton = new ButtonBuilder()
                .ObjectName("Remove")
                .Label(RemoveLabel)
                .Size(DrawLayerPalette.RowButtonSize)
                .FontSize(DrawLayerPalette.RowButtonFontSize)
                .BackgroundColor(DrawLayerPalette.RowButtonBgColor)
                .HoverColor(DrawLayerPalette.RowButtonDangerHoverColor)
                .Build();
            removeButton.transform.SetParent(groupGo.transform, false);
            Tooltips.Attach(removeButton.gameObject, ModLocalization.GetString("tooltipRemove"));

            return group;
        }

        private static string Fmt(float v) => v.ToString("0.#");

        private static Color Dim(Color c, bool visible)
        {
            return visible ? c : new Color(c.r, c.g, c.b, c.a * 0.4f);
        }
    }
}
