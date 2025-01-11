using UnityEngine;

namespace Examples.Volleyball
{
    public class Ball : MonoBehaviour
    {
        private Rigidbody2D _rigidBody;

        public string CollisionTag { get; private set; }

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public void Restart(Vector2 startPosition)
        {
            transform.position = startPosition;

            _rigidBody.linearVelocity = Vector2.zero;
            _rigidBody.angularVelocity = 0;

            CollisionTag = string.Empty;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionTag = collision.collider.tag;
        }

        private void OnCollisionExit2D()
        {
            CollisionTag = string.Empty;
        }
    }
}
