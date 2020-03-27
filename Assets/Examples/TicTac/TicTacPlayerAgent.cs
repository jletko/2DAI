using MLAgents;
using MLAgents.Policies;
using MLAgents.Sensors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.TicTac
{
    [RequireComponent(typeof(HeuristicPlayer))]
    [RequireComponent(typeof(BehaviorParameters))]
    public class TicTacPlayerAgent : Agent
    {
        [SerializeField] private TicTacGym _gym;

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
            for (int i = 0; i < _gym.GymSize; i++)
            {
                for (int j = 0; j < _gym.GymSize; j++)
                {
                    if (_gym.Cells[i, j].State == CellState.EMPTY)
                    {
                        sensor.AddOneHotObservation(0, 3);
                        return;
                    }
                    if (_gym.Cells[i, j].State == PlayerTag)
                    {
                        sensor.AddOneHotObservation(1, 3);
                        return;
                    }
                    if (_gym.Cells[i, j].State == Tags.GetOtherPlayer(PlayerTag))
                    {
                        sensor.AddOneHotObservation(2, 3);
                        return;
                    }
                }
            }
        }

        public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
        {
            _myLastMaskedActions.Clear();

            int index = 0;
            for (int i = 0; i < _gym.GymSize; i++)
            {
                for (int j = 0; j < _gym.GymSize; j++)
                {
                    if (!_gym.Cells[i, j].State.Equals(CellState.EMPTY))
                    {
                        _myLastMaskedActions.Add(index);
                    }

                    index++;
                }
            }

            actionMasker.SetMask(0, _myLastMaskedActions);
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            if (vectorAction.Length != 1)
            {
                throw new Exception($"Incorrect actions count. Expected: 1, actual: {vectorAction.Length}");
            }

            int cellIndex = Mathf.FloorToInt(vectorAction[0]);
            if (cellIndex < 0)
            {
                return;
            }

            _gym.SetMarkForCurrentPlayer(cellIndex / _gym.GymSize, cellIndex % _gym.GymSize);
        }

        public override float[] Heuristic()
        {
            return _heuristicPlayer.Result;
        }

        public override void OnEpisodeBegin()
        {
            _heuristicPlayer.Done();
            base.OnEpisodeBegin();
        }

        public bool IsHeuristic => _behaviorParameters.IsHeuristic;

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
