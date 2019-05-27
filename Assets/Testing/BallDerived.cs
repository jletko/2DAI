using UnityEngine;

namespace Testing
{
    public class BallDerived : BallBase
    {
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            base.OnCollisionEnter2D(other);
        }
    }
}
