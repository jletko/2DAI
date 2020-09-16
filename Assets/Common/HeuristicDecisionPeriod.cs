using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.Serialization;

namespace Common
{
    [RequireComponent(typeof(DecisionRequester))]
    [RequireComponent(typeof(BehaviorParameters))]
    public class HeuristicDecisionPeriod : MonoBehaviour
    {
        [FormerlySerializedAs("_period")] [SerializeField] private int period = 1;

        private void Start()
        {
            BehaviorParameters behaviorParameters = GetComponent<BehaviorParameters>();

            if (behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly ||
                behaviorParameters.BehaviorType == BehaviorType.Default &&
                !Academy.Instance.IsCommunicatorOn && behaviorParameters.Model == null)
            {
                GetComponent<DecisionRequester>().DecisionPeriod = period;
            }
        }
    }
}
