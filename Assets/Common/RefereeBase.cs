using UnityEngine;
using UnityEngine.Serialization;

namespace Common
{
    public abstract class RefereeBase : MonoBehaviour
    {
        [FormerlySerializedAs("Gym")] [SerializeField] protected Transform gym;

        protected float TimeSinceLastRestart => Time.fixedTime - _lastRestartTime;

        private float _lastRestartTime;

        public virtual void Restart()
        {
            _lastRestartTime = Time.fixedTime;
        }

        protected Vector2 GetRandomPositionInGym()
        {
            return (Vector2)transform.position + gym.localScale * new Vector2(
                  Mathf.Sign(Random.Range(-1f, 1f)) * Random.Range(0.1f, 0.9f),
                  Mathf.Sign(Random.Range(-1f, 1f)) * Random.Range(0.1f, 0.9f)
                 ) / 2;
        }
    }
}
