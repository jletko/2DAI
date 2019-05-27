// Source: https://github.com/gomoku/PiskvorkySVitezem

namespace Examples.TicTac.PiskvorkySVitezem
{
    /// <summary>
    /// Enumerace urèující obsazenost daného pole
    /// </summary>
    public enum BarvaKamene
    {
        /// <summary>
        /// Pole obsazené èerným kamenem, numerická hodnota 1
        /// </summary>
        Cerny = 1,
        /// <summary>
        /// Pole obsazené bílým kamenem, numerická hodnota -1
        /// </summary>
        Bily = -1,
        /// <summary>
        /// Neobsazené pole, numerická hodnota 0
        /// </summary>
        Zadny = 0
    }
}