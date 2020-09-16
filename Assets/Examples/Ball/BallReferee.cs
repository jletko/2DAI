using Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Examples.Ball
{
    public class BallReferee : RefereeBase
    {
        [FormerlySerializedAs("_ballAgent")] [SerializeField] private BallAgent ballAgent;
        [FormerlySerializedAs("_target")] [SerializeField] private Transform target;

        public override void Restart()
        {
            base.Restart();
            ballAgent.EndEpisode();
            ballAgent.Restart(transform.position);
            target.position = GetRandomPositionInGym();
        }

        private void Start()
        {
            target.position = GetRandomPositionInGym();
        }

        private void FixedUpdate()
        {
            ballAgent.AddReward(-0.001f);
            ballAgent.AddReward(-ballAgent.Power * 0.001f);

            if (TimeSinceLastRestart > 15)
            {
                ballAgent.SetReward(-1);
                Restart();
            }

            if (ballAgent.IsTouchingTarget)
            {
                ballAgent.SetReward(1);
                Restart();
            }

            if (ballAgent.IsCrashed)
            {
                ballAgent.AddReward(-0.1f);
            }
        }
    }
}
