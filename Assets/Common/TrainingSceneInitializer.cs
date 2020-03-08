using MLAgents;
using UnityEngine;

namespace Common
{
    internal class TrainingSceneInitializer : MonoBehaviour
    {
        [SerializeField] private GameObject _rootPrefab;
        [SerializeField] private Academy _academy;
        [SerializeField] private Transform _gym;
        [SerializeField] private int _rootCount = 10;
        [SerializeField] private float _padding;

        private int _rootsPerRowCount;

#if !UNITY_EDITOR
        private Vector2 _lastViewSize;
#endif
        private void Start()
        {
#if !UNITY_EDITOR
            _lastViewSize = GetMainGameViewSize();
#endif
            _rootsPerRowCount = _academy.IsCommunicatorOn ? Mathf.FloorToInt(Mathf.Sqrt(_rootCount)) : 1;

            InstantiateRoots(_rootPrefab, _gym.transform.localScale, _padding, _rootsPerRowCount);
            AlignCameraSize(GetMainGameViewSize(), _gym.transform.localScale, _padding, _rootsPerRowCount);
        }

        private void Update()
        {
#if !UNITY_EDITOR
        var viewSize = GetMainGameViewSize();
        if (_lastViewSize.x != viewSize.x || _lastViewSize.y != viewSize.y)
        {
            AlignCameraSize(viewSize, TicTacGym.transform.localScale, Padding, _rootsPerRowCount);
            _lastViewSize = viewSize;
        }
#endif
        }

        private static void InstantiateRoots(Object rootPrefab, Vector2 areaSize, float padding, int rowColumnCount)
        {
            if (rowColumnCount == 1)
            {
                return;
            }

            for (int i = -rowColumnCount / 2; i <= rowColumnCount / 2; i++)
            {
                for (int j = -rowColumnCount / 2; j <= rowColumnCount / 2; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    Instantiate(rootPrefab, (2 * padding + areaSize.x) * i * Vector3.left + (2 * padding + areaSize.y) * j * Vector3.up, Quaternion.identity);
                }
            }
        }

        private static void AlignCameraSize(Vector2 viewSize, Vector2 areaSize, float padding, int rowColumnCount)
        {
            if (rowColumnCount == 1)
            {
                padding = 0;
            }

            float screenRatio = viewSize.x / viewSize.y;
            float areaRatio = areaSize.x / areaSize.y;
            if (screenRatio >= areaRatio)
            {
                Camera.main.orthographicSize = rowColumnCount * (padding + areaSize.y / 2);
            }
            else
            {
                Camera.main.orthographicSize = rowColumnCount * (padding + areaSize.x / screenRatio / 2);
            }
        }

        private static Vector2 GetMainGameViewSize()
        {
#if UNITY_EDITOR
            var T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var res = getSizeOfMainGameView.Invoke(null, null);
            return (Vector2)res;
#else
        return new Vector2(Screen.width, Screen.height);
#endif
        }
    }
}
