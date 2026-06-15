using UnityEngine;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.checkbox;

namespace com.github.lhervier.ksp.ui.ugui.list
{
    /// <summary>
    /// Behaviour of a marker row: hover reveals the delete button and tints the background, clicking the
    /// row opens the editor, the checkbox toggles the marker visibility, the delete button removes it.
    /// </summary>
    public class MarkerRowController : MonoBehaviour
    {
        private DrawLayerViewModel _viewModel;
        public MarkerRowController ViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private int _index;
        public MarkerRowController Index(int index)
        {
            this._index = index;
            return this;
        }

        private UnityEngine.UI.Image _bg;
        public MarkerRowController Background(UnityEngine.UI.Image bg)
        {
            this._bg = bg;
            return this;
        }

        private CanvasGroup _buttonsGroup;
        public MarkerRowController ButtonsGroup(CanvasGroup buttonsGroup)
        {
            this._buttonsGroup = buttonsGroup;
            return this;
        }

        private PointerHandler _pointerHandler;
        public MarkerRowController PointerHandler(PointerHandler pointerHandler)
        {
            this._pointerHandler = pointerHandler;
            return this;
        }

        private CheckboxController _checkbox;
        public MarkerRowController Checkbox(CheckboxController checkbox)
        {
            this._checkbox = checkbox;
            return this;
        }

        private ButtonController _removeButton;
        public MarkerRowController RemoveButtonController(ButtonController removeButton)
        {
            this._removeButton = removeButton;
            return this;
        }

        public void Start()
        {
            SetHover(false);

            if (_pointerHandler != null)
            {
                _pointerHandler.OnEnter = () => SetHover(true);
                _pointerHandler.OnExit = () => SetHover(false);
                _pointerHandler.OnClick = OnRowClicked;
            }
            if (_checkbox != null)
            {
                _checkbox.OnToggled.Add(OnVisibilityToggled);
            }
            if (_removeButton != null)
            {
                _removeButton.OnClick.Add(OnRemove);
            }
        }

        public void OnDestroy()
        {
            if (_checkbox != null)
            {
                _checkbox.OnToggled.Remove(OnVisibilityToggled);
            }
            if (_removeButton != null)
            {
                _removeButton.OnClick.Remove(OnRemove);
            }
            if (_pointerHandler != null)
            {
                _pointerHandler.OnEnter = null;
                _pointerHandler.OnExit = null;
                _pointerHandler.OnClick = null;
            }
        }

        private void OnRowClicked()
        {
            _viewModel.EditMarker(_index);
        }

        private void OnVisibilityToggled(bool _)
        {
            _viewModel.ToggleVisibility(_index);
        }

        private void OnRemove()
        {
            _viewModel.RemoveMarker(_index);
        }

        private void SetHover(bool hovered)
        {
            if (_bg != null)
            {
                _bg.color = hovered ? DrawLayerPalette.RowHoverColor : Color.clear;
            }
            if (_buttonsGroup != null)
            {
                _buttonsGroup.alpha = hovered ? 1f : 0f;
                _buttonsGroup.blocksRaycasts = hovered;
                _buttonsGroup.interactable = hovered;
            }
        }
    }
}
