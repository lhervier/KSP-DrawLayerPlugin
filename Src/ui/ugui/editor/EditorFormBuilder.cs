using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.combo;
using com.github.lhervier.ksp.shared.ugui.slider;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.textfield;

namespace com.github.lhervier.ksp.ui.ugui.editor
{
    /// <summary>
    /// Builds the editor form fields (name, type, position sliders, recenter, circle radius/graduations,
    /// color grid) into a vertical stack, and wires them to the EditorFormController. Mounted by the
    /// shared ScrollableView as its scrolled content.
    /// </summary>
    public class EditorFormBuilder : IUGUIBuilder<EditorFormController>
    {
        // Predefined graduation divisions offered in the combo.
        public static readonly string[] DivisionOptions = { "1", "2", "3", "4", "6", "8", "12", "36" };

        private DrawLayerViewModel _viewModel;
        public EditorFormBuilder WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public EditorFormController Build()
        {
            var rootGo = new GameObject("EditorForm", typeof(RectTransform));
            var rootLayout = rootGo.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(0, 0, 0, 0);
            rootLayout.spacing = 0f;
            rootLayout.childAlignment = TextAnchor.UpperLeft;
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;

            // ---- Section: common properties ----
            GameObject propsSection = NewSection(rootGo.transform, "editorSectionProperties");

            BuildFieldLabel(propsSection.transform, "fieldName");
            TextFieldController name = new TextFieldBuilder()
                .WithParent(propsSection.transform)
                .Build();

            BuildFieldLabel(propsSection.transform, "fieldType");
            TypeSelectorController type = new TypeSelectorBuilder().Build();
            type.transform.SetParent(propsSection.transform, false);

            SliderController xSlider = BuildSliderField(propsSection.transform, "fieldPositionX", 0f, 100f, out TextMeshProUGUI xValue);
            SliderController ySlider = BuildSliderField(propsSection.transform, "fieldPositionY", 0f, 100f, out TextMeshProUGUI yValue);

            ButtonController recenter = new ButtonBuilder()
                .WithObjectName("Recenter")
                .WithLabel(ModLocalization.GetString("buttonRecenter"))
                .WithAutoWidth(10f)
                .WithBackgroundColor(DrawLayerPalette.RowButtonBgColor)
                .WithHoverColor(DrawLayerPalette.RowButtonHoverColor)
                .Build();
            recenter.transform.SetParent(propsSection.transform, false);

            BuildSeparator(rootGo.transform);

            // ---- Section: circle (shown only for circle markers) ----
            GameObject circleSection = NewSection(rootGo.transform, "editorSectionCircle");

            SliderController rSlider = BuildSliderField(circleSection.transform, "fieldRadius", 1f, 50f, out TextMeshProUGUI rValue);

            BuildFieldLabel(circleSection.transform, "fieldDivisions");
            ComboController divisions = new ComboBuilder()
                .WithParent(circleSection.transform)
                .WithLabelFor(DivisionLabel)
                .Build();

            BuildSeparator(rootGo.transform);

            // ---- Section: color ----
            GameObject colorSection = NewSection(rootGo.transform, "editorSectionColor");
            ColorGridController colorGrid = new ColorGridBuilder().Build();
            colorGrid.transform.SetParent(colorSection.transform, false);

            return rootGo
                .AddComponent<EditorFormController>()
                .WithViewModel(_viewModel)
                .WithControls(name, type, xSlider, xValue, ySlider, yValue, recenter, circleSection, rSlider, rValue, divisions, colorGrid);
        }

        // Localized label for a division count: "No graduation" for 1, "N divisions (X°)" otherwise.
        public static string DivisionLabel(string value)
        {
            if (!int.TryParse(value, out int n)) return value;
            if (n <= 1) return ModLocalization.GetString("divisionsNone");
            return ModLocalization.GetString("divisionsValue", n, Mathf.RoundToInt(360f / n));
        }

        // ----------------------------------------------------------------

        private static GameObject NewSection(Transform parent, string labelKey)
        {
            var go = new GameObject("Section", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DrawLayerPalette.SectionPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.SectionPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.SectionPaddingV),
                Mathf.RoundToInt(DrawLayerPalette.SectionPaddingV));
            layout.spacing = DrawLayerPalette.FieldSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var labelGo = new GameObject("SectionLabel", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var label = UGUILabels.AddLabel(labelGo);
            label.text = ModLocalization.GetString(labelKey).ToUpperInvariant();
            label.fontSize = DrawLayerPalette.SectionLabelFontSize;
            label.fontStyle = FontStyles.Bold;
            label.color = DrawLayerPalette.SectionLabelColor;
            label.alignment = TextAlignmentOptions.Left;

            return go;
        }

        private static void BuildFieldLabel(Transform parent, string key)
        {
            var go = new GameObject("FieldLabel", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var label = UGUILabels.AddLabel(go);
            label.text = ModLocalization.GetString(key);
            label.fontSize = DrawLayerPalette.FieldLabelFontSize;
            label.color = DrawLayerPalette.FieldLabelColor;
            label.alignment = TextAlignmentOptions.Left;
        }

        private static void BuildSeparator(Transform parent)
        {
            var go = new GameObject("Separator", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = le.preferredHeight = 1f;
            var img = go.AddComponent<Image>();
            img.sprite = SpritesGlobal.FillSprite;
            img.type = Image.Type.Simple;
            img.color = DrawLayerPalette.SeparatorColor;
            img.raycastTarget = false;
        }

        // Field made of a header row (label + value) and a slider below it.
        private static SliderController BuildSliderField(Transform parent, string labelKey, float min, float max, out TextMeshProUGUI valueLabel)
        {
            var fieldGo = new GameObject("SliderField", typeof(RectTransform));
            fieldGo.transform.SetParent(parent, false);
            var fieldLayout = fieldGo.AddComponent<VerticalLayoutGroup>();
            fieldLayout.padding = new RectOffset(0, 0, 0, 0);
            fieldLayout.spacing = 4f;
            fieldLayout.childAlignment = TextAnchor.UpperLeft;
            fieldLayout.childControlWidth = true;
            fieldLayout.childControlHeight = true;
            fieldLayout.childForceExpandWidth = true;
            fieldLayout.childForceExpandHeight = false;

            var headerGo = new GameObject("Header", typeof(RectTransform));
            headerGo.transform.SetParent(fieldGo.transform, false);
            var headerLayout = headerGo.AddComponent<HorizontalLayoutGroup>();
            headerLayout.childAlignment = TextAnchor.MiddleLeft;
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = false;

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(headerGo.transform, false);
            var labelLe = labelGo.AddComponent<LayoutElement>();
            labelLe.flexibleWidth = 1f;
            var label = UGUILabels.AddLabel(labelGo);
            label.text = ModLocalization.GetString(labelKey);
            label.fontSize = DrawLayerPalette.FieldLabelFontSize;
            label.color = DrawLayerPalette.FieldLabelColor;
            label.alignment = TextAlignmentOptions.Left;

            var valueGo = new GameObject("Value", typeof(RectTransform));
            valueGo.transform.SetParent(headerGo.transform, false);
            valueLabel = UGUILabels.AddLabel(valueGo);
            valueLabel.fontSize = DrawLayerPalette.FieldValueFontSize;
            valueLabel.color = DrawLayerPalette.FieldValueColor;
            valueLabel.alignment = TextAlignmentOptions.Right;

            SliderController slider = new SliderBuilder()
                .Parent(fieldGo.transform)
                .Min(min)
                .Max(max)
                .Build();

            return slider;
        }
    }
}
