using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.button;

namespace com.github.lhervier.ksp.ui.ugui.overlays.remove
{
    /// <summary>
    /// Builds the footer of the remove-confirmation internal popup: a right-aligned row with Cancel and
    /// a danger-styled Remove button.
    /// </summary>
    public class RemoveConfirmFooterBuilder : IUGUIBuilder<RemoveConfirmFooterController>
    {
        public RemoveConfirmFooterController Build()
        {
            var rootGo = new GameObject("RemoveConfirmFooter", typeof(RectTransform));

            var layout = rootGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = DrawLayerPalette.CardFootSpacing;
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            ButtonController cancel = new ButtonBuilder()
                .WithObjectName("Cancel")
                .WithLabel(ModLocalization.GetString("buttonCancel"))
                .WithAutoWidth(DrawLayerPalette.CardButtonPaddingH)
                .WithSize(DrawLayerPalette.CardButtonHeight)
                .WithFontSize(DrawLayerPalette.CardButtonFontSize)
                .WithBackgroundColor(DrawLayerPalette.FooterCancelBgColor)
                .WithHoverColor(DrawLayerPalette.FooterCancelHoverColor)
                .WithTextColor(DrawLayerPalette.FooterCancelTextColor)
                .Build();
            cancel.transform.SetParent(rootGo.transform, false);

            ButtonController remove = new ButtonBuilder()
                .WithObjectName("Remove")
                .WithLabel(ModLocalization.GetString("dialogButtonRemove"))
                .WithAutoWidth(DrawLayerPalette.CardButtonPaddingH)
                .WithSize(DrawLayerPalette.CardButtonHeight)
                .WithFontSize(DrawLayerPalette.CardButtonFontSize)
                .WithBackgroundColor(DrawLayerPalette.CardButtonDangerBgColor)
                .WithHoverColor(DrawLayerPalette.CardButtonDangerBgColor)
                .WithTextColor(DrawLayerPalette.CardButtonDangerTextColor)
                .Build();
            remove.transform.SetParent(rootGo.transform, false);

            return rootGo
                .AddComponent<RemoveConfirmFooterController>()
                .WithCancelButtonController(cancel)
                .WithRemoveButtonController(remove);
        }
    }
}
