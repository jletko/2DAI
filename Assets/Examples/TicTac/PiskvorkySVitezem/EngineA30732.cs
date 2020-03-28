// Source: https://github.com/gomoku/PiskvorkySVitezem

using System;

namespace Examples.TicTac.PiskvorkySVitezem
{
    public class EngineA30732 : IEngine
    {
        private class Tprior
        {
            public int Pv, Ps0, Ps1, Ps2, Ps3; //ohodnocení ve 4 smìrech a jejich souèet
            public int I;       //v kterém je seznamu dobrých tahù
            public int Nxt, Pre; //další a pøedchozí prvky seznamu dobrých tahù
        };

        private struct Tsquare
        {
            public int Z;       //0=prázndné, 1=bílý, 2=èerný, 3=hranice okna
            public Tprior[] H;  //ohodnocení pro oba hráèe
            public int X, Y;        //souøadnice
        };

        private int
            _player, //hráè na tahu, 0=bílý, 1=èerný
            _moves,  //poèet tahù
            _width,  //šíøka hracího pole
            _height, //výška hracího pole
            _height2;//height+2
        private int[] _diroff = new int[9]; //vzdálenosti na sousední políèka v hracím poli

        //---------------------------------------------------------------------------
        private const int  //konstanty pro ohodnocovací funkci
            H10 = 2, H11 = 6, H12 = 10,
            H20 = 23, H21 = 158, H22 = 175,
            H30 = 256, H31 = 511, H4 = 2047;
        private int[,] _priority = { { 0, 1, 1 }, { H10, H11, H12 }, { H20, H21, H22 }, { H30, H31, 0 }, { H4, 0, 0 } };
        private int[] _sum = { 0, 0 }; //celkové souèty priorit všech polí pro oba hráèe
        private int _dpth = 0, _depth; //hloubka rekurze
        private int[] _d = { 7, 5, 4 };
        private const int McurMoves = 384, MwinMoves = 400;
        private int[]
            _curMoves = new int[McurMoves],  //buffer na zpracovávané tahy
            _winMoves1 = new int[MwinMoves], //buffer na výherní kombinace
            _winEval = new int[MwinMoves];
        private int[,]
            _goodMoves = new int[4, 2],  //seznamy políèek s velkým ohodnocením; pro oba hráèe
            _winMove = new int[2, 2];      //místo, kde už se dá vyhrát; pro oba hráèe

        private int
            _uwinMoves,
            _lastMove,
            _bestMove;  //výsledný tah poèítaèe

        private Tsquare[] _board;  //hrací plocha
        private int _boardk; //konec hrací plochy
        private Random _generator;

        //---------------------------------------------------------------------------
        private int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        private int Abs(int x)
        {
            return x >= 0 ? x : -x;
        }

        private int Distance(int p1, int p2)
        {
            return Max(Abs(_board[p1].X - _board[p2].X),
                Abs(_board[p1].Y - _board[p2].Y));
        }
        //---------------------------------------------------------------------------
        public SouradnicePole NajdiNejlepsiTah(BarvaKamene[,] hraciPole, BarvaKamene barvaNaTahu, TimeSpan zbyvajiciCasNaPartii)
        {
            int p, x, y;

            //nastav hloubku pøemýšlení podle zbývajícího èasu
            _depth = 4 + (int)zbyvajiciCasNaPartii.TotalSeconds / 60;
            //pøi prvním tahu zjisti, jestli jsem bílý nebo èerný
            if (_moves == 0)
            {
                _player = 0;
                if (barvaNaTahu == BarvaKamene.Bily)
                {
                    _player = 1;
                }
            }
            //najdi v hracím poli poslední tah soupeøe
            p = 6 * _height2 + 1;
            for (x = 0; x < _width; x++)
            {
                for (y = 0; y < _height; y++)
                {
                    switch (hraciPole[y, x])
                    {
                        case BarvaKamene.Bily:
                            if (_board[p].Z != 1)
                            {
                                DoMove(p);
                                goto mujTah;
                            }
                            break;
                        case BarvaKamene.Cerny:
                            if (_board[p].Z != 2)
                            {
                                DoMove(p);
                                goto mujTah;
                            }
                            break;
                    }
                    p++;
                }
                p += 2;
            }
            //tah soupeøe nenalezen => já zaèínám		
            _player = 0;
            if (barvaNaTahu == BarvaKamene.Cerny)
            {
                _player = 1;
            }
        //proveï svùj tah
        mujTah:
            Computer1();
            return new SouradnicePole(_board[_lastMove].Y, _board[_lastMove].X);
        }
        //---------------------------------------------------------------------------

        public EngineA30732()
        {
            int x, y, k;
            int p;
            Tprior pr;

            _generator = new Random();
            //alokuj hrací plochu
            _width = _height = SouradnicePole.VelikostPlochy;
            _height2 = _height + 2;
            _board = new Tsquare[(_width + 12) * (_height2)]; //jednorozmìrné pole !
            _boardk = (_width + 6) * _height2;
            //offsety pro pohyb do všech osmi smìrù
            _diroff[0] = 1;
            _diroff[4] = -_diroff[0];
            _diroff[1] = (1 + _height2);
            _diroff[5] = -_diroff[1];
            _diroff[2] = _height2;
            _diroff[6] = -_diroff[2];
            _diroff[3] = (-1 + _height2);
            _diroff[7] = -_diroff[3];
            _diroff[8] = 0;

            //vynuluj pole
            p = 0;
            for (x = -5; x <= _width + 6; x++)
            {
                for (y = 0; y <= _height + 1; y++)
                {
                    _board[p].Z = (x < 1 || y < 1 || x > _width || y > _height) ? 3 : 0;
                    _board[p].X = x - 1;
                    _board[p].Y = y - 1;
                    _board[p].H = new Tprior[2];
                    for (k = 0; k < 2; k++)
                    {
                        _board[p].H[k] = pr = new Tprior();
                        pr.I = 0;
                        pr.Pv = 4;
                        pr.Ps0 = pr.Ps1 = pr.Ps2 = pr.Ps3 = 1;
                    }
                    p++;
                }
            }
            _moves = 0;
            //vytvoø pomocnou tabulku pro zrychlení ohodnocovací funkce
            Gen();
        }

        //---------------------------------------------------------------------------
        //udìlá tah na pole p
        private bool DoMove(int p)
        {
            if (_board[p].Z != 0)
            {
                return false;
            }

            _board[p].Z = _player + 1;
            _player = 1 - _player;
            //zvyš poèítadlo tahù
            _moves++;
            //pøepoèítej ohodnocení
            Evaluate(p);
            _lastMove = p;
            return true;
        }

        //---------------------------------------------------------------------------
        private short[,] _k = new short[2, 262144]; //ohodnocení pro všechny kombinace 9 polí
        private static int[] _comb = new int[10];
        private static int _ind;
        private static int[] _n = new int[4];

        //---------------------------------------------------------------------------
        private void Gen2(int pos)
        {
            int pb, pe, a1, a2;
            int n1, n2, n3;
            int s;

            if (pos == 9)
            {
                a1 = a2 = 0;
                if (_comb[4] == 0)
                {
                    n1 = _n[1]; n2 = _n[2]; n3 = _n[3];
                    pb = 0;
                    pe = 4;
                    while (pe != 9)
                    {
                        if (n3 == 0)
                        {
                            if (n2 == 0)
                            {
                                s = 0;
                                if (_comb[pb] == 0 && _comb[pe + 1] < 2 && pb != 4)
                                {
                                    s++;
                                    if (_comb[pe] == 0 && pe != 4)
                                    {
                                        s++;
                                    }
                                }
                                int pri = _priority[n1, s];
                                if (a1 < pri)
                                {
                                    a1 = pri;
                                }
                            }
                            if (n1 == 0)
                            {
                                s = 0;
                                if (_comb[pb] == 0 && (_comb[pe + 1] & 1) == 0 && pb != 4)
                                {
                                    s++;
                                    if (_comb[pe] == 0 && pe != 4)
                                    {
                                        s++;
                                    }
                                }
                                int pri = _priority[n2, s];
                                if (a2 < pri)
                                {
                                    a2 = pri;
                                }
                            }
                        }
                        switch (_comb[++pe])
                        {
                            case 1: n1++; break;
                            case 2: n2++; break;
                            case 3: n3++; break;
                        }
                        switch (_comb[pb++])
                        {
                            case 1: n1--; break;
                            case 2: n2--; break;
                            case 3: n3--; break;
                        }
                    }
                }
                _k[0, _ind] = (short)a1;
                _k[1, _ind] = (short)a2;
                _ind++;
            }
            else
            {
                //vygeneruj postupnì všechny kombinace 
                for (int z = 0; z < 4; z++)
                {
                    _comb[pos] = z;
                    Gen2(pos + 1);
                }
            }
        }

        private void Gen1(int pos)
        {
            if (pos == 5)
            {
                Gen2(pos);
            }
            else
            {
                for (int z = 0; z < 4; z++)
                {
                    _comb[pos] = z;
                    _n[z]++;
                    Gen1(pos + 1);
                    _n[z]--;
                }
            }
        }

        private void Gen()
        {
            _ind = 0;
            Gen1(0);
        }

        //---------------------------------------------------------------------------
        //pøepoèítá ohodnocení polí do vzdálenosti 4 od políèka p0
        private void Evaluate(int p0)
        {
            int i, k, m, s, h;
            Tprior pr;
            int p, q, qk, pe, pk1;
            int ind;
            int pattern;

            //zaplnìné pole odstraò ze seznamu a dej mu nulovou prioritu
            if (_board[p0].Z != 0)
            {
                for (k = 0; k < 2; k++)
                {
                    pr = _board[p0].H[k];
                    if (pr.Pv != 0)
                    {
                        if (pr.I != 0)
                        {
                            _board[pr.Nxt].H[k].Pre = pr.Pre;
                            if (pr.Pre != 0)
                            {
                                _board[pr.Pre].H[k].Nxt = pr.Nxt;
                            }
                            else
                            {
                                _goodMoves[pr.I, k] = pr.Nxt;
                            }

                            pr.I = 0;
                        }
                        _sum[k] -= pr.Pv;
                        pr.Pv = pr.Ps0 = pr.Ps1 = pr.Ps2 = pr.Ps3 = 0;
                    }
                }
            }
            //zpracuj všechny 4 smìry
            for (i = 0; i < 4; i++)
            {
                s = _diroff[i];
                pk1 = p0;
                pk1 += s * 5;
                pe = p0;
                p = p0;
                for (m = 4; m > 0; m--)
                {
                    p -= s;
                    if (_board[p].Z == 3)
                    {
                        pe += s * m;
                        p += s;
                        break;
                    }
                }
                pattern = 0;
                qk = pe;
                qk -= s * 9;
                for (q = pe; q != qk; q -= s)
                {
                    pattern *= 4;
                    pattern += _board[q].Z;
                }
                while (_board[p].Z != 3)
                {
                    if (_board[p].Z == 0)
                    {
                        for (k = 0; k < 2; k++)
                        { //pro oba hráèe
                            pr = _board[p].H[k];
                            //oprav prioritu v jednom smìru
                            h = _k[k, pattern];
                            switch (i)
                            {
                                case 0:
                                    m = pr.Ps0; pr.Ps0 = h;
                                    break;
                                case 1:
                                    m = pr.Ps1; pr.Ps1 = h;
                                    break;
                                case 2:
                                    m = pr.Ps2; pr.Ps2 = h;
                                    break;
                                case 3:
                                    m = pr.Ps3; pr.Ps3 = h;
                                    break;
                            }
                            m = h - m;
                            if (m != 0)
                            {
                                _sum[k] += m;
                                pr.Pv += m;
                                //podle ohodnocení urèi seznam
                                ind = 0;
                                if (pr.Pv >= H21)
                                {
                                    ind++;
                                    if (pr.Pv >= 2 * H21)
                                    {
                                        ind++;
                                        if (pr.Pv >= H4)
                                        {
                                            ind++;
                                        }
                                    }
                                }
                                //pøehoï políèko do jiného seznamu
                                if (ind != pr.I)
                                {
                                    //odpoj
                                    if (pr.I != 0)
                                    {
                                        _board[pr.Nxt].H[k].Pre = pr.Pre;
                                        if (pr.Pre != 0)
                                        {
                                            _board[pr.Pre].H[k].Nxt = pr.Nxt;
                                        }
                                        else
                                        {
                                            _goodMoves[pr.I, k] = pr.Nxt;
                                        }
                                    }
                                    //pøipoj
                                    if ((pr.I = ind) != 0)
                                    {
                                        q = pr.Nxt = _goodMoves[ind, k];
                                        _goodMoves[ind, k] = _board[q].H[k].Pre = p;
                                        pr.Pre = 0;
                                    }
                                }
                            }
                        }
                    }
                    p += s;
                    if (p == pk1)
                    {
                        break;
                    }
                    //rotuj pattern vpravo; zleva naèti další políèko
                    pe += s;
                    pattern >>= 2;
                    pattern += _board[pe].Z << 16;
                }
            }
        }

        //---------------------------------------------------------------------------
        //hlavní rekurzivní funkce
        //zjistí, jestli prohraju nebo vyhraju
        //pøi dpth==0 nastaví promìnnou tah
        private int Alfabeta(int player1, int ucurMoves, int logWin, int last, int strike)
        {
            int p, q, t, defendMoves1, defendMoves2, uwinMoves0;
            int y, m;
            int i, j, s;
            int pr, hr;
            int mustDefend, mustAttack;

            //když už jsou ètyøi v øadì, tak táhni bez rozmýšlení
            p = _goodMoves[3, player1];
            if (p != 0)
            {
                if (logWin != 0 && (strike & 1) != 0)
                {
                    _winMoves1[_uwinMoves++] = p;
                }

                return 1000 - _dpth; //vyhrál jsem :)
            }
            int player2 = 1 - player1;
            p = _goodMoves[3, player2];
            if (p != 0)
            {
                _board[p].Z = player1 + 1;
                Evaluate(p);
                if ((strike & 1) != 0)
                {
                    y = -Alfabeta(player2, ucurMoves, logWin, last, 2);
                }
                else
                {
                    y = -Alfabeta(player2, ucurMoves, logWin, last, 1);
                }

                _board[p].Z = 0;
                Evaluate(p);
                if (logWin != 0 && y != 0 && ((y > 0) == ((strike & 1) != 0)))
                {
                    _winMoves1[_uwinMoves++] = p;
                }

                return y;
            }

            //nejdøíve najdi všechny dobré tahy a pøekopíruj je do statického pole
            int utahy0 = ucurMoves;
            if ((strike & 1) == 0)
            {
                hr = player2;
            }
            else
            {
                hr = player1;
            }

            mustDefend = mustAttack = 0;
            p = _goodMoves[2, player1];
            if (p != 0)
            {
                mustAttack++;
                do
                {
                    //už mám tøi v øadì => mìl bych vyhrát
                    if (logWin == 0 && _board[p].H[player1].Pv >= H31)
                    {
                        if (_dpth == 0)
                        {
                            _bestMove = p;
                        }

                        return 999 - _dpth;
                    }
                    if (ucurMoves == McurMoves)
                    {
                        break;
                    }

                    pr = _board[p].H[hr].Pv;
                    for (q = ucurMoves++; q > utahy0 &&
                        _board[_curMoves[q - 1]].H[hr].Pv < pr; q--)
                    {
                        _curMoves[q] = _curMoves[q - 1];
                    }
                    _curMoves[q] = p;
                    p = _board[p].H[player1].Nxt;
                } while (p != 0);
            }
            defendMoves1 = ucurMoves;
            for (p = _goodMoves[2, player2]; p != 0; p = _board[p].H[player2].Nxt)
            {
                //soupeø má tøi v øadì => musím se bránit
                if (_board[p].H[player2].Pv >= H30 + H21)
                {
                    if (mustDefend == 0)
                    {
                        mustDefend = 1;
                    }

                    if (_board[p].H[player2].Pv >= H31)
                    {
                        mustDefend = 2;
                    }
                }
                else
                {
                    if (mustAttack != 0)
                    {
                        continue;
                    }
                }
                if (ucurMoves == McurMoves)
                {
                    break;
                }

                pr = _board[p].H[hr].Pv;
                for (q = ucurMoves++; q > defendMoves1 &&
                    _board[_curMoves[q - 1]].H[hr].Pv < pr; q--)
                {
                    _curMoves[q] = _curMoves[q - 1];
                }
                _curMoves[q] = p;
            }
            defendMoves2 = ucurMoves;

            if (_dpth < _depth)
            {
                //dívej se jen na okolí posledního tahu
                if (strike < 2 && last != 0)
                {
                    for (i = 0; i < 8; i++)
                    {
                        s = _diroff[i];
                        p = last;
                        p += s;
                        for (j = 0; j < 4 && (_board[p].Z != 3); j++, p += s)
                        {
                            if ((strike & 1) == 0 && _board[p].H[player2].I == 1
                                && (mustAttack == 0 || _board[p].H[player2].Pv >= H30)
                                || _board[p].H[player1].I == 1 &&
                                (mustDefend == 0 || _board[p].H[player1].Pv >= H30))
                            {
                                if (ucurMoves < McurMoves)
                                {
                                    pr = _board[p].H[hr].Pv;
                                    for (q = ucurMoves++; q > defendMoves2 &&
                                        _board[_curMoves[q - 1]].H[hr].Pv < pr; q--)
                                    {
                                        _curMoves[q] = _curMoves[q - 1];
                                    }
                                    _curMoves[q] = p;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //obrana
                    if (strike == 2 && mustDefend < 2)
                    {
                        for (p = _goodMoves[1, player2]; p != 0; p = _board[p].H[player2].Nxt)
                        {
                            if (ucurMoves == McurMoves)
                            {
                                break;
                            }

                            if ((last == 0 || Distance(p, last) < _d[mustDefend])
                                && (mustAttack == 0 || _board[p].H[player2].Pv >= H30)
                                )
                            {
                                if (ucurMoves == McurMoves)
                                {
                                    break;
                                }

                                pr = _board[p].H[hr].Pv;
                                for (q = ucurMoves++; q > defendMoves2 &&
                                    _board[_curMoves[q - 1]].H[hr].Pv < pr; q--)
                                {
                                    _curMoves[q] = _curMoves[q - 1];
                                }
                                _curMoves[q] = p;
                            }
                        }
                        defendMoves2 = ucurMoves;
                    }
                    //útok
                    for (p = _goodMoves[1, player1]; p != 0; p = _board[p].H[player1].Nxt)
                    {
                        if (ucurMoves == McurMoves)
                        {
                            break;
                        }

                        if ((last == 0 || Distance(p, last) < 7)
                            && (mustDefend == 0 || _board[p].H[player1].Pv >= H30)
                            )
                        {
                            if (ucurMoves == McurMoves)
                            {
                                break;
                            }

                            pr = _board[p].H[hr].Pv;
                            for (q = ucurMoves++; q > defendMoves2 &&
                                _board[_curMoves[q - 1]].H[hr].Pv < pr; q--)
                            {
                                _curMoves[q] = _curMoves[q - 1];
                            }
                            _curMoves[q] = p;
                        }
                    }
                }
            }

            if (utahy0 == ucurMoves)
            {
                return 0; //nelze nikde zaútoèit nebo už jsem moc hluboko
            }

            //dobré tahy jsou v poli curMoves => ohodno je a vyber nejlepší
            uwinMoves0 = _uwinMoves;
            m = -0x7ffe;
            for (t = utahy0; t < ucurMoves; t++)
            {
                _dpth++;
                p = _curMoves[t];
                //proveï tah
                _board[p].Z = player1 + 1;
                Evaluate(p);
                //rekurze
                if ((strike & 1) != 0)
                {
                    if (t >= defendMoves2 || t < defendMoves1)
                    {
                        //útoèný tah, aktualizuj místo posledního útoku
                        y = -Alfabeta(player2, ucurMoves, logWin, p, 0);
                    }
                    else
                    {
                        //obranný tah, nemìním políèko posledního útoku
                        //soupeø získal tah navíc a mùže se bránit, kde bude chtít
                        y = -Alfabeta(player2, ucurMoves, logWin, last, 2);
                    }
                }
                else
                {
                    y = -Alfabeta(player2, ucurMoves, logWin, last, 1);
                }
                //smaž, co jsi pøidal
                _board[p].Z = 0;
                Evaluate(p);
                _dpth--;
                if (y > 0)
                {
                    //vyhraju
                    if (_dpth == 0)
                    {
                        _bestMove = p;
                    }

                    if (logWin != 0 && (strike & 1) != 0)
                    {
                        _winMoves1[_uwinMoves++] = p;
                    }

                    return y;
                }
                if (y == 0)
                {
                    if ((strike & 1) == 0)
                    {
                        //ubráním se
                        _uwinMoves = uwinMoves0;
                        if (_dpth == 0)
                        {
                            _bestMove = p;
                        }

                        return y;
                    }
                    m = y;
                }
                else if (y >= m)
                {
                    //asi prohraju, musím zkoušet další možné tahy
                    if (logWin != 0 && (strike & 1) == 0) { _winMoves1[_uwinMoves++] = p; logWin = 0; }
                    if (_dpth == 0)
                    {
                        //vyber tah, kterým prohraju co nejpozdìji
                        if (y > m || _board[p].H[player2].Pv > _board[_bestMove].H[player2].Pv)
                        {
                            _bestMove = p;
                        }
                    }

                    m = y;
                }
            }
            return m;
        }

        //---------------------------------------------------------------------------
        private int Try4(int player1, int last)
        {
            int i, j, s;
            int p, p2 = 0, y = 0;

            p = _goodMoves[3, player1];
            if (p != 0)
            {
                _winMoves1[_uwinMoves++] = p;
                return p; //vyhrál jsem
            }
            int player2 = 1 - player1;

            for (i = 0; i < 8; i++)
            {
                s = _diroff[i];
                p = last;
                p += s;
                for (j = 0; j < 4 && (_board[p].Z != 3); j++, p += s)
                {
                    if (_board[p].H[player1].Pv >= H30)
                    {
                        //útok
                        _board[p].Z = player1 + 1;
                        Evaluate(p);
                        if (_goodMoves[3, player2] == 0)
                        {
                            p2 = _goodMoves[3, player1];
                            if (p2 != 0)
                            {
                                //obrana - jen jediná možnost
                                _board[p2].Z = 2 - player1;
                                Evaluate(p2);
                                //rekurze
                                y = Try4(player1, p);
                                _board[p2].Z = 0;
                                Evaluate(p2);
                            }
                        }
                        _board[p].Z = 0;
                        Evaluate(p);
                        if (y != 0)
                        {
                            _winMoves1[_uwinMoves++] = p2;
                            _winMoves1[_uwinMoves++] = p;
                            return p;
                        }
                    }
                }
            }
            return 0;
        }

        //---------------------------------------------------------------------------
        //zkoušej jen vynucené tahy, kdy útoèník dìlá jen ètveøice
        //hloubka rekurze není omezena 
        private int Try4(int player1)
        {
            int p, p2, y = 0, t;
            int j;

            _uwinMoves = 0;
            t = 0;
            for (j = 1; j <= 2; j++)
            {
                for (p = _goodMoves[j, player1]; p != 0; p = _board[p].H[player1].Nxt)
                {
                    if (_board[p].H[player1].Pv >= H30)
                    {
                        if (t == McurMoves)
                        {
                            break;
                        }

                        _curMoves[t++] = p;
                    }
                }
            }
            for (t--; t >= 0; t--)
            {
                p = _curMoves[t];
                _board[p].Z = player1 + 1;
                Evaluate(p);
                if (_goodMoves[3, 1 - player1] == 0)
                {
                    p2 = _goodMoves[3, player1];
                    if (p2 != 0)
                    {
                        _board[p2].Z = 2 - player1;
                        Evaluate(p2);
                        y = Try4(player1, p);
                        _board[p2].Z = 0;
                        Evaluate(p2);
                    }
                    _board[p].Z = 0;
                    Evaluate(p);
                    if (y != 0)
                    {
                        _winMoves1[_uwinMoves++] = p2;
                        _winMoves1[_uwinMoves++] = p;
                        return p;
                    }
                }
            }
            return 0;
        }

        //---------------------------------------------------------------------------
        private int Alfabeta(int strike, int player1, int logWin, int last)
        {
            /* int y=0;
			 if(depth>5 && player==1){
				 depth-=5;
				 y=alfabeta(player1,curMoves,logWin,last,strike);
				 setDepth();
			 }
			 if(!y) y=*/
            return Alfabeta(player1, 0, logWin, last, strike);
        }

        //---------------------------------------------------------------------------
        //zjisti ohodnocení políèka p0 pro hráèe player1
        private int GetEval(int player1, int p0)
        {
            int i, s, y, c1, c2, n;
            int p;

            y = 0;
            //podívej se na okolní políèka
            c1 = c2 = 0;
            for (i = 0; i < 8; i++)
            {
                s = _diroff[i];
                p = p0;
                p += s;
                if (_board[p].Z == player1 + 1)
                {
                    c1++;
                }

                if (_board[p].Z == 2 - player1)
                {
                    c2++;
                }
            }
            n = 0;
            if (_board[p0].H[player1].Ps0 < 2)
            {
                n++;
            }

            if (_board[p0].H[player1].Ps1 < 2)
            {
                n++;
            }

            if (_board[p0].H[player1].Ps2 < 2)
            {
                n++;
            }

            if (_board[p0].H[player1].Ps3 < 2)
            {
                n++;
            }

            if (n > 2)
            {
                y -= 8;
            }

            if (c1 + c2 == 0)
            {
                y -= 20;
            }

            if (c2 == 0 && c1 > 0 && _board[p0].H[player1].Pv > 9)
            {
                y += (c1 + 1) * 5;
            }
            if (_board[p0].H[1 - player1].Pv < 5)
            {
                n = 0;
                if (_board[p0].H[player1].Ps0 >= H12)
                {
                    n++;
                }

                if (_board[p0].H[player1].Ps1 >= H12)
                {
                    n++;
                }

                if (_board[p0].H[player1].Ps2 >= H12)
                {
                    n++;
                }

                if (_board[p0].H[player1].Ps3 >= H12)
                {
                    n++;
                }

                y += 15;
                if (n > 1)
                {
                    y += n * 64;
                }
            }
            return y + _board[p0].H[player1].Pv;
        }

        //---------------------------------------------------------------------------
        private int GetEval(int p)
        {
            int a, b;
            a = GetEval(0, p);
            b = GetEval(1, p);
            //zkombinuj ohodnocení obou hráèù
            return a > b ? a + b / 2 : a / 2 + b;
        }

        //---------------------------------------------------------------------------
        //obrana, zkoušej táhnout na políèka z pole winMoves1
        private int Defend(int player1)
        {
            int p, t;
            int m, mv, mh, y, yh, nwins, i, j;
            int player2 = 1 - player1;
            int th, thm = 0;

            _dpth++;
            nwins = _uwinMoves;
            //vypoèti ohodnocení všech políèek v seznamu
            for (t = _uwinMoves - 1, th = nwins - 1; t != -1; t--, th--)
            {
                _winEval[th] = GetEval(player2, _winMoves1[t]);
            }

            mh = m = -0x7ffe;
            for (i = 0; nwins > 0 && i < 20; i++)
            {
                //vyber políèko s nejvìtším ohodnocením
                mv = -0x7ffe;
                for (th = nwins - 1; th != -1; th--)
                {
                    if (_winEval[th] > mv) { thm = th; mv = _winEval[th]; }
                }
                if (mv < 25)
                {
                    break;
                }
                //vyjmi ho ze seznamu
                j = thm;
                p = _winMoves1[j];
                nwins--;
                _winMoves1[j] = _winMoves1[nwins];

                _board[p].Z = player1 + 1;
                Evaluate(p);
                y = -Alfabeta(3, player2, 0, 0);
                _board[p].Z = 0;
                Evaluate(p);
                yh = _winEval[thm] + y * 20;
                if (yh > mh)
                {
                    m = y;
                    mh = yh;
                    _bestMove = p;
                    if (y > 0 || y == 0 && _winMove[_player, player1] == 0)
                    {
                        break;
                    }
                }
                _winEval[thm] = _winEval[nwins];
            }
            if (m < 0)
            {
                //když mám prohrát, zkusím ještì zaútoèit (nìkdy to pomùže)
                t = 0;
                for (p = _goodMoves[1, player1]; p != 0; p = _board[p].H[player1].Nxt)
                {
                    if (_board[p].H[player1].Pv >= H30)
                    {
                        if (t == MwinMoves)
                        {
                            break;
                        }

                        _winMoves1[t++] = p;
                    }
                }
                for (t--; t >= 0; t--)
                {
                    p = _winMoves1[t];
                    _board[p].Z = player1 + 1;
                    Evaluate(p);
                    y = -Alfabeta(3, player2, 0, 0);
                    _board[p].Z = 0;
                    Evaluate(p);
                    if (y > m)
                    {
                        m = y;
                        _bestMove = p;
                        if (y >= 0)
                        {
                            break;
                        }
                    }
                }
            }
            _dpth--;
            return m;
        }

        //---------------------------------------------------------------------------
        //najde políèko s nejvìtším ohodnocením
        //když je ohodnocení moc malé, vrátí 0
        private int FindMax(int player1)
        {
            int p, t;
            int m, r;
            int i, k;

            m = -1;
            t = 0;
            for (i = 2; i > 0 && t == 0; i--)
            {
                for (k = 0; k < 2; k++)
                {
                    for (p = _goodMoves[i, k]; p != 0; p = _board[p].H[k].Nxt)
                    {
                        r = GetEval(p);
                        if (r > m)
                        {
                            m = r;
                            t = p;
                        }
                    }
                }
            }

            return t;
        }

        //---------------------------------------------------------------------------
        //zjisti, jaké bude celkové ohodnocení po nìkolika tazích
        private int LookAhead(int player1)
        {
            int p;
            int y;

            if (_goodMoves[3, player1] != 0)
            {
                return 500; //byla nalezena výhra
            }

            int player2 = 1 - player1;
            p = _goodMoves[3, player2];
            if (p == 0 && _dpth < 4)
            {
                p = FindMax(player1);
            }

            if (p == 0)
            {
                return (_sum[player1] - _sum[player2]) / 3;
            }
            _dpth++;
            _board[p].Z = player1 + 1;
            Evaluate(p);
            y = -LookAhead(player2);
            _board[p].Z = 0;
            Evaluate(p);
            _dpth--;
            return y;
        }

        //---------------------------------------------------------------------------
        private void Computer1()
        {
            int p;
            int nresults = 0;
            int m, y = 0, rnd;
            int r;
            int player1 = _player, player2 = 1 - player1;

            //první tah bude uprostøed hrací plochy
            if (_moves == 0)
            {
                DoMove((_width / 2 + 6) * _height2 + _height / 2 + 1);
                return;
            }
            //druhý tah dej náhodnì na nìkteré sousední políèko
            if (_moves == 1)
            {
                for (; ; ) //první tah mohl být na okraji nebo v rohu !
                {
                    switch (_generator.Next(0, 4))
                    {
                        case 0:
                            if (DoMove(_lastMove + 1))
                            {
                                return;
                            }

                            break;
                        case 1:
                            if (DoMove(_lastMove - 1))
                            {
                                return;
                            }

                            break;
                        case 2:
                            if (DoMove(_lastMove + _height2))
                            {
                                return;
                            }

                            break;
                        case 3:
                            if (DoMove(_lastMove - _height2))
                            {
                                return;
                            }

                            break;
                    }
                }
            }
            _lastMove = -1;

            //když už jsou ètyøi v øadì, tak táhni bez rozmýšlení
            if (DoMove(_goodMoves[3, player1]))
            {
                return; //právì jsem vyhrál
            }

            if (DoMove(_goodMoves[3, player2]))
            {
                return; //musím se bránit
            }

            //zkoušej dìlat všechny možné ètveøice
            _bestMove = 0;
            if (DoMove(Try4(player1)))
            {
                return; //urèitì vyhraju
            }

            //co když soupeø bude dìlat jen ètveøice 
            p = Try4(player2);
            if (p != 0)
            {
                //soupeø mùže vyhrát => musím se bránit 	
                _winMove[player1, player2] = p;
                y = 1;
            }
            else
            {
                _bestMove = 0;
                if (_winMove[player1, player1] != 0)
                {
                    //v pøedchozím tahu byla nalezena výhra
                    y = Alfabeta(1, player1, 0, _winMove[player1, player1]);
                    if (y <= 0)
                    {
                        _winMove[player1, player1] = 0; //soupeøi se podaøilo se ubránit
                    }
                }
                //zjisti, zda už mùžu vyhrát
                if (_bestMove == 0)
                {
                    y = Alfabeta(3, player1, 0, 0);
                }
                if (y > 0 && _bestMove != 0)
                {
                    //pravdìpodobnì vyhraju
                    DoMove(_bestMove);
                    //zapamatuj si toto místo, abys v pøíštím tahu nehrál nìkde jinde
                    _winMove[player1, player1] = _bestMove;
                    return;
                }
                //zjisti, zda soupeø nemùže vyhrát
                y = 0;
                if (_winMove[player1, player2] != 0)
                {
                    _uwinMoves = 0;
                    y = Alfabeta(1, player2, 1, _winMove[player1, player2]);
                    if (y <= 0)
                    {
                        _winMove[player1, player2] = 0; //soupeø mohl vyhrát, ale pokazil to
                    }
                }
                if (y <= 0)
                {
                    _uwinMoves = 0;
                    y = Alfabeta(3, player2, 1, 0);
                    if (y > 0)
                    {
                        _winMove[player1, player2] = _bestMove; //asi prohraju
                    }
                }
            }
            _bestMove = 0;

            if (y > 0)
            {
                //obrana
                if (_uwinMoves > 0)
                {
                    //zkoušej se bránit jen na políèkách, kde byla nalezena výhra soupeøe
                    Defend(player1);
                }
                if (_bestMove == 0)
                {
                    //zkoušej se bránit kdekoli
                    Alfabeta(2, player1, 0, 0);
                }
            }

            if (_bestMove == 0 && _moves > 9)
            {
                //prohledávání do hloubky nenalezlo výherní tahy
                m = -0x7ffffffe;
                for (p = 0; p < _boardk; p++)
                {
                    if (_board[p].Z == 0 && (_board[p].H[0].Pv > 10 || _board[p].H[1].Pv > 10))
                    {
                        r = GetEval(p);
                        _board[p].Z = player1 + 1;
                        Evaluate(p);
                        r -= LookAhead(player2);
                        _board[p].Z = 0;
                        Evaluate(p);
                        if (r > m)
                        {
                            m = r;
                            _bestMove = p;
                            nresults = 1;
                        }
                        else if (r > m - 20)
                        {
                            nresults++;
                            if (_generator.Next(0, nresults) == 0)
                            {
                                _bestMove = p;
                            }
                        }
                    }
                }
            }
            if (_bestMove == 0)
            {
                //vyber políèko s nejvìtším ohodnocením
                m = -1;
                for (p = 0; p < _boardk; p++)
                {
                    if (_board[p].Z == 0)
                    {
                        r = GetEval(p);
                        if (r > m)
                        {
                            m = r;
                        }
                    }
                }
                //náhodnì zvol políèko, které má ohodnocení o trochu menší než nejlepší
                rnd = m / 12;
                if (rnd > 30)
                {
                    rnd = 30;
                }

                nresults = 0;
                for (p = 0; p < _boardk; p++)
                {
                    if (_board[p].Z == 0)
                    {
                        if (GetEval(p) >= m - rnd)
                        {
                            nresults++;
                            if (_generator.Next(0, nresults) == 0)
                            {
                                _bestMove = p;
                            }
                        }
                    }
                }

            }
            //koneènì proveï svùj tah 
            DoMove(_bestMove);
        }
        //---------------------------------------------------------------------------
    }
}