using Common;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Examples.TicTac
{
    public class Cell : MonoBehaviour
    {
        [FormerlySerializedAs("_foregroundOSprite")] [SerializeField] private Sprite foregroundOSprite;
        [FormerlySerializedAs("_foregroundXSprite")] [SerializeField] private Sprite foregroundXSprite;

        private SpriteRenderer _renderer;

        private string _state = CellState.Empty;

        public event EventHandler<Cell, EventArgs> Clicked;

        public string State
        {
            get => _state;

            set
            {
                if (!_state.Equals(CellState.Empty) && !value.Equals(CellState.Empty))
                {
                    //throw new Exception("Trying to overwrite existing non empty cell state.");
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
                case CellState.Empty:
                    _renderer.sprite = null;
                    break;
                case CellState.PlayerO:
                    _renderer.sprite = foregroundOSprite;
                    break;
                case CellState.PlayerX:
                    _renderer.sprite = foregroundXSprite;
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
