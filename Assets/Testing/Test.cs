using UnityEngine;

namespace Testing
{
    public class Test : MonoBehaviour
    {
        private void Start()
        {
            Physics2D.gravity = 9.81f * Vector2.down;
        }
    }
}
