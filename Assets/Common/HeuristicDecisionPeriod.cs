using MLAgents;
using MLAgents.Policies;
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
            if (GetComponent<BehaviorParameters>().IsHeuristic)
            {
                GetComponent<DecisionRequester>().DecisionPeriod = _period;
            }
        }
    }
}
