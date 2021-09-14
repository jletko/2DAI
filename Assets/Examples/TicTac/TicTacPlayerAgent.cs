using System;
using System.Collections.Generic;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

using UnityEngine;
using UnityEngine.Serialization;

namespace Examples.TicTac
{
    [RequireComponent(typeof(HeuristicPlayer))]
    [RequireComponent(typeof(BehaviorParameters))]
    public class TicTacPlayerAgent : Agent
    {
        [FormerlySerializedAs("_gym")]
        [SerializeField]
        private TicTacGym gym;

        private HeuristicPlayer _heuristicPlayer;
        private BehaviorParameters _behaviorParameters;

        private string PlayerTag => gameObject.tag;

        private readonly List<int> _myLastMaskedActions = new List<int>();

        public override void Initialize()
        {
            _heuristicPlayer = GetComponent<HeuristicPlayer>();
            _behaviorParameters = GetComponent<BehaviorParameters>();
            base.Initialize();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            for (var i = 0; i < gym.GymSize; i++)
                for (var j = 0; j < gym.GymSize; j++)
                {
                    if (gym.Cells[i, j].State == CellState.Empty)
                    {
                        sensor.AddOneHotObservation(0, 3);
                        continue;
                    }

                    if (gym.Cells[i, j].State == PlayerTag)
                    {
                        sensor.AddOneHotObservation(1, 3);
                        continue;
                    }

                    if (gym.Cells[i, j].State == Tags.GetOtherPlayer(PlayerTag))
                    {
                        sensor.AddOneHotObservation(2, 3);
                    }
                }
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            _myLastMaskedActions.Clear();

            var index = 0;
            for (var i = 0; i < gym.GymSize; i++)
                for (var j = 0; j < gym.GymSize; j++)
                {
                    actionMask.SetActionEnabled(0, index, gym.Cells[i, j].State.Equals(CellState.Empty));
                    index++;
                }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var discreteActions = actions.DiscreteActions;

            if (discreteActions.Length != 1)
            {
                throw new Exception($"Incorrect actions count. Expected: 1, actual: {discreteActions.Length}");
            }

            int cellIndex = discreteActions[0];
            if (cellIndex < 0)
            {
                return;
            }

            gym.SetMarkForCurrentPlayer(cellIndex / gym.GymSize, cellIndex % gym.GymSize);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActions = actionsOut.DiscreteActions;

            for (var i = 0; i < discreteActions.Length; i++) discreteActions[i] = _heuristicPlayer.Result[i];
        }

        public override void OnEpisodeBegin()
        {
            _heuristicPlayer.Done();
            base.OnEpisodeBegin();
        }

        public bool IsHeuristic => _behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly ||
                                   _behaviorParameters.BehaviorType == BehaviorType.Default &&
                                   !Academy.Instance.IsCommunicatorOn && _behaviorParameters.Model == null;

        public bool IsHeuristicEnabled => IsHeuristic && _heuristicPlayer.IsEnabled;

        public void RequestDecisionEx()
        {
            if (!IsHeuristic)
            {
                RequestDecision();
                return;
            }

            if (IsHeuristicEnabled)
            {
                _heuristicPlayer.RequestDecision();
            }
        }

        private void FixedUpdate()
        {
            if (IsHeuristicEnabled && _heuristicPlayer.HasValidResult)
            {
                RequestDecision();
                _heuristicPlayer.InvalidateResult();
            }
        }
    }
}