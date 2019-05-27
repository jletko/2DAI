using UnityEngine;

namespace Examples.Volleyball
{
    public class Hand : MonoBehaviour
    {
        public string CollisionTag { get; private set; }

        private Vector2 _initialLocalPosition;

        public void Awake()
        {
            _initialLocalPosition = transform.localPosition;
        }

        public void Restart()
        {
            transform.localPosition = _initialLocalPosition;
            transform.localRotation = Quaternion.identity;
            CollisionTag = string.Empty;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionTag = collision.collider.tag;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            CollisionTag = string.Empty;
        }
    }
}
