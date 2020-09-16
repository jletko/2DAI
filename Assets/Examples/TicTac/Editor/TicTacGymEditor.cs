using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Examples.TicTac.Editor
{
    [CustomEditor(typeof(TicTacGym))]
    public class TicTacGymEditor : UnityEditor.Editor
    {
        private TicTacGym _ticTacGym;
        private GameObject _cellPrefab;
        private int _gymSize;

        private void OnEnable()
        {
            _ticTacGym = (TicTacGym)target;
            _cellPrefab = _ticTacGym.cellPrefab;
            _gymSize = _ticTacGym.GymSize;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_ticTacGym.cellPrefab == _cellPrefab && _ticTacGym.GymSize == _gymSize)
            {
                return;
            }

            _cellPrefab = _ticTacGym.cellPrefab;
            _gymSize = _ticTacGym.GymSize;
            ClearCells();
            CreateCells(_ticTacGym.transform, _cellPrefab, _gymSize);
        }

        private void ClearCells()
        {
            var cells = _ticTacGym.GetComponentsInChildren<Cell>().ToList();
            cells.ForEach(o => DestroyImmediate(o.gameObject));
        }

        private void CreateCells(Transform gymTransform, GameObject cellPrefab, int size)
        {
            gymTransform.localScale = size * Vector3.one;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var cellGameObject = Instantiate(cellPrefab,
                                                     new Vector2(j - gymTransform.localScale.x / 2 + 0.5f, i - gymTransform.localScale.y / 2 + 0.5f), Quaternion.identity, gymTransform);
                    cellGameObject.transform.localScale = new Vector3(
                                                                      1 / gymTransform.localScale.x,
                                                                      1 / gymTransform.localScale.y,
                                                                      1 / gymTransform.localScale.z);
                }
            }
        }
    }
}