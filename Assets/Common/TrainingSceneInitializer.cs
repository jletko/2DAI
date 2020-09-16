using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Common
{
    internal class TrainingSceneInitializer : MonoBehaviour
    {
        [FormerlySerializedAs("_rootPrefab")] [SerializeField] private GameObject rootPrefab;
        [FormerlySerializedAs("_gym")] [SerializeField] private Transform gym;
        [FormerlySerializedAs("_padding")] [SerializeField] private float padding;
        [FormerlySerializedAs("_gymsCount")] [SerializeField] private TraningGymsCount gymsCount = TraningGymsCount.TwentyFive;

        private int _rootsPerRowCount;

        private Vector2 _lastViewSize;
        private void Start()
        {
            _lastViewSize = GetMainGameViewSize();
            _rootsPerRowCount = IsCommunicatorOn ? (int)Mathf.Sqrt((int)gymsCount) : 1;

            InstantiateRoots(rootPrefab, gym.transform.localScale, padding, _rootsPerRowCount);
            AlignCameraSize(GetMainGameViewSize(), gym.transform.localScale, padding, _rootsPerRowCount);
        }

        private void Update()
        {
            var viewSize = GetMainGameViewSize();
            if (_lastViewSize.x != viewSize.x || _lastViewSize.y != viewSize.y)
            {
                AlignCameraSize(GetMainGameViewSize(), gym.transform.localScale, padding, _rootsPerRowCount);
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
