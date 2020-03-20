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
        private bool _isRotated90Degree;

        public bool IsEnabled => enabled;

        private void Awake()
        {
            _barvaKamene = ConvertToBarvaKamene(gameObject.tag);
            _hraciPole = new BarvaKamene[_gym.GymSize, _gym.GymSize];
            Result = new float[1];
            _isRotated90Degree = false;
            ResetEngine();
            InvalidateResult();
        }

        private void FixedUpdate()
        {
            if (_najdiNejlepsiTahTask == null || !_najdiNejlepsiTahTask.IsCompleted)
            {
                return;
            }

            EncodeResult(_najdiNejlepsiTahTask.Result);
            _najdiNejlepsiTahTask = null;
            HasValidResult = true;
        }

        public bool HasValidResult { get; private set; }

        public float[] Result { get; private set; }

        public void InvalidateResult()
        {
            HasValidResult = false;
        }

        public void RequestDecision()
        {
            InvalidateResult();

            if (!IsEnabled)
            {
                return;
            }

            DecodeGym(_gym);
            _najdiNejlepsiTahTask = Task.Run(() => _piskvorkyEngine.NajdiNejlepsiTah(_hraciPole, _barvaKamene, TimeSpan.FromSeconds(_casNaPartii)));
        }

        public void Done()
        {
            if (!IsEnabled)
            {
                return;
            }

            //_isRotated90Degree = !_isRotated90Degree;
            ResetEngine();
            InvalidateResult();
        }

        private void ResetEngine()
        {
            _najdiNejlepsiTahTask = null;
            _piskvorkyEngine = new EngineA30732();
        }

        private void EncodeResult(SouradnicePole souradnicePole)
        {
            if (_isRotated90Degree)
            {
                Result[0] = souradnicePole.Radek + souradnicePole.Sloupec * _gym.GymSize;
            }
            else
            {
                Result[0] = souradnicePole.Radek * _gym.GymSize + souradnicePole.Sloupec;
            }
        }

        private void DecodeGym(TicTacGym gym)
        {
            for (int i = 0; i < gym.GymSize; i++)
            {
                for (int j = 0; j < gym.GymSize; j++)
                {
                    if (_isRotated90Degree)
                    {
                        BarvaKamene barvaKamene = ConvertToBarvaKamene(gym.Cells[j, i].State);
                        _hraciPole[j, i] = barvaKamene;
                    }
                    else
                    {
                        BarvaKamene barvaKamene = ConvertToBarvaKamene(gym.Cells[i, j].State);
                        _hraciPole[i, j] = barvaKamene;
                    }
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
