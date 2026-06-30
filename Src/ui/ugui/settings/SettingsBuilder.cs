using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.checkbox;
using com.github.lhervier.ksp.shared.ugui.sprites;

namespace com.github.lhervier.ksp.ui.ugui.settings
{
    /// <summary>
    /// Settings sub-view: a back-arrow header and a single section with the debug-mode checkbox and an
    /// explanatory hint.
    /// </summary>
    public class SettingsBuilder : IUGUIBuilder<SettingsController>
    {
        private DrawLayerViewModel _viewModel;
        public SettingsBuilder WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public SettingsController Build()
        {
            var rootGo = new GameObject("DrawLayer.Settings", typeof(RectTransform));
            var layout = rootGo.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            SubViewHeader.Build(rootGo.transform, ModLocalization.GetString("DLM_settingsTitle"),
                out ButtonController backButton, out TextMeshProUGUI _);

            // Section
            var sectionGo = new GameObject("Section", typeof(RectTransform));
            sectionGo.transform.SetParent(rootGo.transform, false);
            var sectionLayout = sectionGo.AddComponent<VerticalLayoutGroup>();
            sectionLayout.padding = new RectOffset(
                Mathf.RoundToInt(DrawLayerPalette.SectionPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.SectionPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.SectionPaddingV),
                Mathf.RoundToInt(DrawLayerPalette.SectionPaddingV));
            sectionLayout.spacing = DrawLayerPalette.FieldSpacing;
            sectionLayout.childAlignment = TextAnchor.UpperLeft;
            sectionLayout.childControlWidth = true;
            sectionLayout.childControlHeight = true;
            sectionLayout.childForceExpandWidth = true;
            sectionLayout.childForceExpandHeight = false;

            var sectionLabelGo = new GameObject("SectionLabel", typeof(RectTransform));
            sectionLabelGo.transform.SetParent(sectionGo.transform, false);
            var sectionLabel = UGUILabels.AddLabel(sectionLabelGo);
            sectionLabel.text = ModLocalization.GetString("DLM_settingsSectionDisplay").ToUpperInvariant();
            sectionLabel.fontSize = DrawLayerPalette.SectionLabelFontSize;
            sectionLabel.fontStyle = FontStyles.Bold;
            sectionLabel.color = DrawLayerPalette.SectionLabelColor;
            sectionLabel.alignment = TextAlignmentOptions.Left;

            // Debug checkbox (whole row clickable)
            CheckboxController debugCheckbox = new CheckboxBuilder()
                .WithLabel(ModLocalization.GetString("DLM_settingsDebug"))
                .WithGreedyState(true)
                .WithCheckedState(_viewModel.DebugMode)
                .Build();
            debugCheckbox.transform.SetParent(sectionGo.transform, false);

            // Hint
            BuildHint(sectionGo.transform, ModLocalization.GetString("DLM_settingsHint"));

            return rootGo
                .AddComponent<SettingsController>()
                .WithViewModel(_viewModel)
                .WithControls(backButton, debugCheckbox);
        }

        private static void BuildHint(Transform parent, string text)
        {
            var hintGo = new GameObject("Hint", typeof(RectTransform));
            hintGo.transform.SetParent(parent, false);

            var bg = hintGo.AddComponent<Image>();
            bg.sprite = SpritesGlobal.FillSprite;
            bg.type = Image.Type.Simple;
            bg.color = DrawLayerPalette.SettingsHintBgColor;
            bg.raycastTarget = false;

            // Left accent bar
            var barGo = new GameObject("Bar", typeof(RectTransform));
            barGo.transform.SetParent(hintGo.transform, false);
            var barLe = barGo.AddComponent<LayoutElement>();
            barLe.ignoreLayout = true;
            var barRect = barGo.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0f, 0f);
            barRect.anchorMax = new Vector2(0f, 1f);
            barRect.pivot = new Vector2(0f, 0.5f);
            barRect.sizeDelta = new Vector2(DrawLayerPalette.SettingsHintBorderThickness, 0f);
            barRect.anchoredPosition = Vector2.zero;
            var bar = barGo.AddComponent<Image>();
            bar.sprite = SpritesGlobal.FillSprite;
            bar.type = Image.Type.Simple;
            bar.color = DrawLayerPalette.SettingsHintBorderColor;
            bar.raycastTarget = false;

            var hintLayout = hintGo.AddComponent<HorizontalLayoutGroup>();
            hintLayout.padding = new RectOffset(
                Mathf.RoundToInt(DrawLayerPalette.SettingsHintPaddingH + DrawLayerPalette.SettingsHintBorderThickness),
                Mathf.RoundToInt(DrawLayerPalette.SettingsHintPaddingH),
                Mathf.RoundToInt(DrawLayerPalette.SettingsHintPaddingV),
                Mathf.RoundToInt(DrawLayerPalette.SettingsHintPaddingV));
            hintLayout.childControlWidth = true;
            hintLayout.childControlHeight = true;
            hintLayout.childForceExpandWidth = true;
            hintLayout.childForceExpandHeight = false;

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(hintGo.transform, false);
            var textLe = textGo.AddComponent<LayoutElement>();
            textLe.flexibleWidth = 1f;
            var label = UGUILabels.AddLabel(textGo);
            label.text = text;
            label.fontSize = DrawLayerPalette.SettingsHintFontSize;
            label.color = DrawLayerPalette.SettingsHintTextColor;
            label.alignment = TextAlignmentOptions.TopLeft;
            label.enableWordWrapping = true;
        }
    }
}
