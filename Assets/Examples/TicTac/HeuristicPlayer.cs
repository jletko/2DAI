using Examples.TicTac.PiskvorkySVitezem;

using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Serialization;

namespace Examples.TicTac
{
    public class HeuristicPlayer : MonoBehaviour
    {
        [FormerlySerializedAs("_gym")] [SerializeField] private TicTacGym gym;
        [FormerlySerializedAs("_casNaPartii")] [SerializeField] private int casNaPartii;

        private IEngine _piskvorkyEngine;
        private BarvaKamene[,] _hraciPole;
        private BarvaKamene _barvaKamene;
        private Task<SouradnicePole> _najdiNejlepsiTahTask;

        public bool IsEnabled => enabled;

        private void Start()
        {
            _barvaKamene = ConvertToBarvaKamene(gameObject.tag);
            _hraciPole = new BarvaKamene[gym.GymSize, gym.GymSize];
            SouradnicePole.VelikostPlochy = gym.GymSize;
            Result = new int[1];
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

        public int[] Result { get; private set; }

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

            DecodeGym(gym);
            _najdiNejlepsiTahTask = Task.Run(() => _piskvorkyEngine.NajdiNejlepsiTah(_hraciPole, _barvaKamene, TimeSpan.FromSeconds(casNaPartii)));
        }

        public void Done()
        {
            if (!IsEnabled)
            {
                return;
            }

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
            Result[0] = souradnicePole.Radek * gym.GymSize + souradnicePole.Sloupec;
        }

        private void DecodeGym(TicTacGym ticTacGym)
        {
            for (int i = 0; i < ticTacGym.GymSize; i++)
            {
                for (int j = 0; j < ticTacGym.GymSize; j++)
                {
                    BarvaKamene barvaKamene = ConvertToBarvaKamene(ticTacGym.Cells[i, j].State);
                    _hraciPole[i, j] = barvaKamene;
                }
            }
        }

        private BarvaKamene ConvertToBarvaKamene(string cellState)
        {
            switch (cellState)
            {
                case CellState.PlayerO:
                    return BarvaKamene.Bily;
                case CellState.PlayerX:
                    return BarvaKamene.Cerny;
                case CellState.Empty:
                    return BarvaKamene.Zadny;
                default:
                    throw new Exception($"Unknown cell state: {cellState}");
            }
        }
    }
}
