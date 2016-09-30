using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobang
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new Chessboard();
            b.Init();
            var s = new Searcher();
            s.Init();

            int A_int = Convert.ToInt32('A');// 'A' => int
            int number = 15; //棋盘格数
            var history = new List<int>();

            int depth = 2; //搜索深度

            while (true)
            {
                int row, col;
                while (true)
                {
                    Console.WriteLine(string.Format("<Round {0}>", history.Count + 1));
                    b.Show();
                    Console.WriteLine(string.Format("You move:"));
                    string text = Console.ReadLine();
                    char[] c = text.ToUpper().Trim().ToCharArray();
                    if (c.Length == 2)
                    {
                        int tr = Convert.ToInt32(c[0]) - A_int;
                        int tc = Convert.ToInt32(c[1]) - A_int;
                        if (tr >= 0 && tr < number && tc >= 0 && tc < number)
                        {
                            row = tr;
                            col = tc;
                            break;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Bad position!"));
                        }
                    }
                    else
                    {
                            Console.WriteLine(string.Format("Incorrect input!"));
                    }
                }

                history.Add(1);

                b.board[row, col] = 1; //人走棋
                b.Show();

                if (b.Check() == 1) //检查是否赢
                {
                    Console.WriteLine(string.Format("You win!"));
                    return;
                }

                Console.WriteLine(string.Format("Robot is thinking now ..."));

                Move move = s.Search(2, b.board, depth);

                Console.WriteLine(string.Format("Robot moves to {0}{1}({2})", (char)(move.row + A_int), (char)(move.col + A_int), move.score));
                b.board[move.row, move.col] = 2; //电脑走棋

                if (b.Check() == 2) //检查是否赢
                {
                    Console.WriteLine(string.Format("You lose!"));
                    return;
                }
            }
        }
    }
}
