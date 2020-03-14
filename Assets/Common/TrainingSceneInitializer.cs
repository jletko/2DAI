using MLAgents;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common
{
    internal class TrainingSceneInitializer : MonoBehaviour
    {
        [SerializeField] private GameObject _rootPrefab;
        [SerializeField] private Transform _gym;
        [SerializeField] private float _padding;
        [SerializeField] private TraningGymsCount _gymsCount = TraningGymsCount.Nine;

        private int _rootsPerRowCount;

        private Vector2 _lastViewSize;
        private void Start()
        {
            _lastViewSize = GetMainGameViewSize();
            _rootsPerRowCount = IsCommunicatorOn ? (int)Mathf.Sqrt((int)_gymsCount) : 1;

            InstantiateRoots(_rootPrefab, _gym.transform.localScale, _padding, _rootsPerRowCount);
            AlignCameraSize(GetMainGameViewSize(), _gym.transform.localScale, _padding, _rootsPerRowCount);
        }

        private void Update()
        {
            var viewSize = GetMainGameViewSize();
            if (_lastViewSize.x != viewSize.x || _lastViewSize.y != viewSize.y)
            {
                AlignCameraSize(GetMainGameViewSize(), _gym.transform.localScale, _padding, _rootsPerRowCount);
                _lastViewSize = viewSize;
            }
        }

        private bool IsCommunicatorOn => Academy.Instance.IsCommunicatorOn;

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
            return new Vector2(Screen.width, Screen.height);
        }
    }
}
