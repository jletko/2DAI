﻿using UnityEngine;

namespace Examples.Chase
{
    public class HuntedAgent : ChaseAgentBase
    {
        public bool IsCatched { get; private set; }

        public override void Restart(Vector2 startPosition)
        {
            base.Restart(startPosition);
            IsCatched = false;
        }

        protected override void OnCollisionStay2D(Collision2D other)
        {
            base.OnCollisionStay2D(other);
            IsCatched = other.collider.CompareTag(Tags.HunterHead);
        }

        protected override void OnCollisionExit2D()
        {
            base.OnCollisionExit2D();
            IsCatched = false;
        }
    }
}
