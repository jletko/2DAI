using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// Ray perception component. Attach this to agents to enable "local perception"
    /// via the use of ray casts directed outward from the agent. 
    /// </summary>
    public class RayPerception2D : MonoBehaviour
    {
        [SerializeField] private int _approxRaysCount = 12;
        [SerializeField] private float _spreadAngle = 180f;
        [SerializeField] private float _rayDistance = 30f;
        [SerializeField] private float _rayCircleRadius = 0.5f;

        private List<float> _perceptionBuffer = new List<float>();
        private Bounds _parentColliderBounds;
        private float[] _rayAngles;

        public void Awake()
        {
            _parentColliderBounds = EncapsulateCollidersBounds(transform.parent);
            transform.position = _parentColliderBounds.center;
            _rayAngles = ComputeRayAngles(_approxRaysCount, Mathf.Deg2Rad * _spreadAngle);
        }

        public IEnumerable<float> Perceive(string[] detectableObjects)
        {
            return Perceive(_rayDistance, _rayAngles, detectableObjects, _rayCircleRadius, _parentColliderBounds.extents.magnitude);
        }

        /// <param name="rayAngles">Provided in radians.</param>
        private IEnumerable<float> Perceive(float rayDistance,
                                            float[] rayAngles, string[] detectableObjects, float rayCircleRadius, float startDistance)
        {
            _perceptionBuffer.Clear();
            // For each ray sublist stores categorical information on detected object
            // along with object distance.
            foreach (float angle in rayAngles)
            {
                Vector2 direction = transform.TransformDirection(DegreeToUnitVector2(angle));
                var rayStart = (Vector2)transform.position + (startDistance + rayCircleRadius) * direction.normalized;
                var adjustedRayLength = rayDistance - (startDistance + rayCircleRadius);
                if (Application.isEditor)
                {
                    // Debug.DrawRay(rayStart, adjustedRayLength * direction, Color.black, 0.01f, true);
                }

                float[] subList = new float[detectableObjects.Length + 2];
                RaycastHit2D hit = Physics2D.CircleCast(rayStart, rayCircleRadius, direction, adjustedRayLength);
                if (hit.collider != null)
                {
                    for (int i = 0; i < detectableObjects.Length; i++)
                    {
                        if (hit.collider.gameObject.CompareTag(detectableObjects[i]))
                        {
                            subList[i] = 1;
                            subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                            break;
                        }
                    }
                }
                else
                {
                    subList[detectableObjects.Length] = 1f;
                }

                _perceptionBuffer.AddRange(subList);
            }

            return _perceptionBuffer;
        }

        private static Bounds EncapsulateCollidersBounds(Transform root)
        {
            var bounds = new Bounds(root.position, Vector2.zero);
            var colliders = root.GetComponentsInChildren<Collider2D>().ToList();
            colliders.ForEach(o => bounds.Encapsulate(o.bounds));

            return bounds;
        }

        private static Vector2 DegreeToUnitVector2(float angle)
        {
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            return new Vector2(x, y);
        }

        private static float[] ComputeRayAngles(int raysCount, float spreadAngle)
        {
            int halfRaysCount = raysCount / 2;

            float[] rayAngles = new float[raysCount + 1];
            float angleIncrement = spreadAngle / 2.0f / halfRaysCount;
            for (int i = 0; i < raysCount - 1; i += 2)
            {
                rayAngles[i] = (i / 2 + 1) * angleIncrement;
                rayAngles[i + 1] = -(i / 2 + 1) * angleIncrement;
            }

            return rayAngles;
        }
    }
}
