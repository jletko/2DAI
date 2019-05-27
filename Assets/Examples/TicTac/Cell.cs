using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Examples.TicTac
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Sprite _foregroundOSprite;
        [SerializeField] private Sprite _foregroundXSprite;

        private SpriteRenderer _renderer;

        private string _state = CellState.EMPTY;

        public event EventHandler Clicked;

        public string State
        {
            get => _state;

            set
            {
                if (!_state.Equals(CellState.EMPTY) && !value.Equals(CellState.EMPTY))
                {
                    //throw new Exception("Trying to overwrite existing non empty cell state.");
                    return;
                }

                _state = value;
                UpdateStateVisuals();
            }
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();

            UpdateStateVisuals();
        }

        private void UpdateStateVisuals()
        {
            switch (_state)
            {
                case CellState.EMPTY:
                    _renderer.sprite = null;
                    break;
                case CellState.PLAYER_O:
                    _renderer.sprite = _foregroundOSprite;
                    break;
                case CellState.PLAYER_X:
                    _renderer.sprite = _foregroundXSprite;
                    break;
            }
        }

        private void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Clicked?.Invoke(this, new EventArgs());
            }
        }
    }
}
