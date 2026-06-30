using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui.editor
{
    /// <summary>
    /// Segmented control to pick the marker type (Cross / Circle): two side-by-side cells inside a
    /// bordered box, the selected one highlighted in accent. Mod-specific (no shared equivalent).
    /// </summary>
    public class TypeSelectorBuilder : IUGUIBuilder<TypeSelectorController>
    {
        private static string CrossGlyph => DefaultPalette.PickGlyph("✛", "✚", "+");
        private static string CircleGlyph => DefaultPalette.PickGlyph("◎", "◯", "O");

        public TypeSelectorController Build()
        {
            var rootGo = new GameObject("TypeSelector", typeof(RectTransform));
            var le = rootGo.AddComponent<LayoutElement>();
            le.minHeight = le.preferredHeight = DrawLayerPalette.SegmentHeight;

            var bg = rootGo.AddComponent<Image>();
            bg.sprite = SpritesGlobal.Border(
                DrawLayerPalette.SegmentBgColor,
                DrawLayerPalette.SegmentBorderColor,
                DrawLayerPalette.SegmentBorderThickness);
            bg.type = Image.Type.Sliced;
            bg.color = Color.white;
            bg.raycastTarget = false;

            var layout = rootGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(1, 1, 1, 1);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            Image crossBg = BuildSegment(rootGo.transform, CrossGlyph + " " + ModLocalization.GetString("DLM_typeCross"), out PointerHandler crossPointer, out TextMeshProUGUI crossLabel);
            BuildDivider(rootGo.transform);
            Image circleBg = BuildSegment(rootGo.transform, CircleGlyph + " " + ModLocalization.GetString("DLM_typeCircle"), out PointerHandler circlePointer, out TextMeshProUGUI circleLabel);

            TypeSelectorController controller = rootGo
                .AddComponent<TypeSelectorController>()
                .WithSegments(crossBg, crossLabel, circleBg, circleLabel);

            crossPointer.OnClick = () => controller.Select(0);
            circlePointer.OnClick = () => controller.Select(1);

            return controller;
        }

        private static Image BuildSegment(Transform parent, string text, out PointerHandler pointer, out TextMeshProUGUI label)
        {
            var segGo = new GameObject("Segment", typeof(RectTransform));
            segGo.transform.SetParent(parent, false);
            var segLe = segGo.AddComponent<LayoutElement>();
            segLe.flexibleWidth = 1f;

            var segBg = segGo.AddComponent<Image>();
            segBg.sprite = SpritesGlobal.FillSprite;
            segBg.type = Image.Type.Simple;
            segBg.color = Color.clear;
            segBg.raycastTarget = true;

            pointer = segGo.AddComponent<PointerHandler>();

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(segGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            label = UGUILabels.AddLabel(labelGo);
            label.text = text;
            label.fontSize = DrawLayerPalette.SegmentFontSize;
            label.color = DrawLayerPalette.SegmentTextColor;
            label.alignment = TextAlignmentOptions.Center;

            return segBg;
        }

        private static void BuildDivider(Transform parent)
        {
            var go = new GameObject("Divider", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minWidth = le.preferredWidth = 1f;
            var img = go.AddComponent<Image>();
            img.sprite = SpritesGlobal.FillSprite;
            img.type = Image.Type.Simple;
            img.color = DrawLayerPalette.SegmentBorderColor;
            img.raycastTarget = false;
        }
    }
}
