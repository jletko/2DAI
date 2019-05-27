using Examples.TicTac.PiskvorkySVitezem;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Examples.TicTac
{
    public class HeuristicPlayer : MonoBehaviour
    {
        [SerializeField] private TicTacGym _gym;
        [SerializeField] private int _casNaPartii;

        private IEngine _piskvorkyEngine;
        private BarvaKamene[,] _hraciPole;
        private BarvaKamene _barvaKamene;
        private Task<SouradnicePole> _najdiNejlepsiTahTask;

        public bool IsEnabled => enabled;

        private void Awake()
        {
            _barvaKamene = ConvertToBarvaKamene(gameObject.tag);
            _hraciPole = new BarvaKamene[_gym.GymSize, _gym.GymSize];
            ResetEngine();
        }

        private void FixedUpdate()
        {
            if (_najdiNejlepsiTahTask == null || !_najdiNejlepsiTahTask.IsCompleted)
            {
                return;
            }

            SouradnicePole nejlepsiTah = _najdiNejlepsiTahTask.Result;
            _najdiNejlepsiTahTask = null;

            _gym.SetMarkForCurrentPlayer(nejlepsiTah.Radek, nejlepsiTah.Sloupec);
        }

        public void RequestDecision()
        {
            if (!IsEnabled)
            {
                return;
            }

            UpdateHraciPole(_gym.Cells);
            _najdiNejlepsiTahTask = Task.Run(() => _piskvorkyEngine.NajdiNejlepsiTah(_hraciPole, _barvaKamene, TimeSpan.FromSeconds(_casNaPartii)));
        }

        public void Done()
        {
            if (!IsEnabled)
            {
                return;
            }

            ResetEngine();
        }

        private void ResetEngine()
        {
            _najdiNejlepsiTahTask = null;
            _piskvorkyEngine = new EngineA30732();
        }

        private void UpdateHraciPole(Cell[,] gymCells)
        {
            for (int i = 0; i < gymCells.GetLength(0); i++)
            {
                for (int j = 0; j < gymCells.GetLength(1); j++)
                {
                    BarvaKamene barvaKamene = ConvertToBarvaKamene(gymCells[i, j].State);
                    _hraciPole[i, j] = barvaKamene;
                }
            }
        }

        private BarvaKamene ConvertToBarvaKamene(string cellState)
        {
            switch (cellState)
            {
                case CellState.PLAYER_O:
                    return BarvaKamene.Bily;
                case CellState.PLAYER_X:
                    return BarvaKamene.Cerny;
                case CellState.EMPTY:
                    return BarvaKamene.Zadny;
                default:
                    throw new Exception($"Unknown cell state: {cellState}");
            }
        }
    }
}