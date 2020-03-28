// Source: https://github.com/gomoku/PiskvorkySVitezem

using System;

namespace Examples.TicTac.PiskvorkySVitezem
{
    /// <summary>
    /// Tøída pro vyjádøení souøadnic tahu jednoho z hráèù
    /// </summary>
    public class SouradnicePole
    {
        /// <summary>
        /// Velikost hrací plochy (ètverec o velikosti <see cref="VelikostPlochy"/>)
        /// </summary>
        public static int VelikostPlochy { get; set; }

        /// <summary>
        /// Konstruktor pro vytvoøení a inicializaci souøadnic pole
        /// </summary>
        /// <param name="radek">Èíselný index øádku - 0 až <see cref="VelikostPlochy"/> - 1</param>
        /// <param name="sloupec">Èíselný index sloupce - 0 až <see cref="VelikostPlochy"/> - 1</param>
        public SouradnicePole(int radek, int sloupec)
        {
            if ((radek >= 0) && (radek < VelikostPlochy))
            {
                Radek = radek;
            }
            else
            {
                throw (new Exception("Hodnota 'radek' mimo pøípustné hranice"));
            }

            if ((sloupec >= 0) && (sloupec < VelikostPlochy))
            {
                Sloupec = sloupec;
            }
            else
            {
                throw (new Exception("Hodnota 'sloupec' mimo pøípustné hranice"));
            }
        }

        /// <summary>
        /// Èíselný index øádku - 0 až <see cref="VelikostPlochy"/> - 1. Pouze ke ètení.
        /// </summary>
        public readonly int Radek;

        /// <summary>
        /// Èíselný index sloupce - 0 až <see cref="VelikostPlochy"/> - 1. Pouze ke ètení.
        /// </summary>
        public readonly int Sloupec;
    }
}