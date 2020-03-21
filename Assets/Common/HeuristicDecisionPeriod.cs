using MLAgents;
using UnityEngine;

namespace Common
{
    [RequireComponent(typeof(DecisionRequester))]
    [RequireComponent(typeof(BehaviorParameters))]
    public class HeuristicDecisionPeriod : MonoBehaviour
    {
        [SerializeField] private int _period = 1;

        private void Start()
        {
            BehaviorParameters behaviorParameters = GetComponent<BehaviorParameters>();

            if (behaviorParameters.Behavior == BehaviorParameters.BehaviorType.HeuristicOnly
                || (behaviorParameters.Behavior == BehaviorParameters.BehaviorType.Default
                    && behaviorParameters.Model == null))
            {
                GetComponent<DecisionRequester>().DecisionPeriod = _period;
            }
        }
    }
}
