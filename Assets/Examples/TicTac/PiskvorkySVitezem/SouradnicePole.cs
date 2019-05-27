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
        /// TODO: needs to be taken from Gym size
        /// Velikost hrací plochy (ètverec o velikosti <see cref="VELIKOST_PLOCHY"/>)
        /// </summary>
        public const int VELIKOST_PLOCHY = 10;

        /// <summary>
        /// Konstruktor pro vytvoøení a inicializaci souøadnic pole
        /// </summary>
        /// <param name="radek">Èíselný index øádku - 0 až <see cref="VELIKOST_PLOCHY"/> - 1</param>
        /// <param name="sloupec">Èíselný index sloupce - 0 až <see cref="VELIKOST_PLOCHY"/> - 1</param>
        public SouradnicePole(int radek, int sloupec)
        {
            if ((radek >= 0) && (radek < VELIKOST_PLOCHY))
            {
                Radek = radek;
            }
            else
            {
                throw (new Exception("Hodnota 'radek' mimo pøípustné hranice"));
            }

            if ((sloupec >= 0) && (sloupec < VELIKOST_PLOCHY))
            {
                Sloupec = sloupec;
            }
            else
            {
                throw (new Exception("Hodnota 'sloupec' mimo pøípustné hranice"));
            }
        }

        /// <summary>
        /// Èíselný index øádku - 0 až <see cref="VELIKOST_PLOCHY"/> - 1. Pouze ke ètení.
        /// </summary>
        public readonly int Radek;

        /// <summary>
        /// Èíselný index sloupce - 0 až <see cref="VELIKOST_PLOCHY"/> - 1. Pouze ke ètení.
        /// </summary>
        public readonly int Sloupec;
    }
}