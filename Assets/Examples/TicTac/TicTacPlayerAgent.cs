using MLAgents;
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

        public override void InitializeAgent()
        {
            _heuristicPlayer = GetComponent<HeuristicPlayer>();
            _behaviorParameters = GetComponent<BehaviorParameters>();
            base.InitializeAgent();
        }

        public override void CollectObservations()
        {
            _myLastMaskedActions.Clear();

            int index = 0;
            for (int i = 0; i < _gym.GymSize; i++)
            {
                for (int j = 0; j < _gym.GymSize; j++)
                {
                    AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.EMPTY));
                    if (PlayerTag.Equals(CellState.PLAYER_O))
                    {
                        AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.PLAYER_O));
                        AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.PLAYER_X));
                    }
                    else
                    {
                        AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.PLAYER_X));
                        AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.PLAYER_O));
                    }

                    if (!_gym.Cells[i, j].State.Equals(CellState.EMPTY))
                    {
                        _myLastMaskedActions.Add(index);
                    }

                    index++;
                }
            }

            SetActionMask(0, _myLastMaskedActions);
        }

        public override void AgentAction(float[] vectorAction)
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

        public override void AgentReset()
        {
            _heuristicPlayer.Done();
            base.AgentReset();
        }

        public bool IsHeuristic =>
                _behaviorParameters.Behavior == BehaviorParameters.BehaviorType.HeuristicOnly
                || (_behaviorParameters.Behavior == BehaviorParameters.BehaviorType.Default
                    && _behaviorParameters.Model == null);

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
