using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobang
{
    public class Chessboard
    {
        public int[,] board;
        static int number = 15;
        int forbidden;
        int[,] dirs;
        int[,] checkDirs;
        List<int[]> won;

        public void Init(int forbidden = 0)
        {
            board = new int[number, number];
            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    board[row, col] = 0;
                }
            }
            this.forbidden = forbidden;
            dirs = new int[,] { { -1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
            checkDirs = new int[,] { { 1, -1 }, { 1, 0 }, { 0, 1 }, { 1, 1 } };
            won = new List<int[]> { };
        }

        void Reset()
        {
            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    board[row, col] = 0;
                }
            }
        }

        int[] GetItem(int row)
        {
            int[] item = new int[number];
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = board[row, i];
            }
            return item;
        }

        string Str()
        {
            string text = "  A B C D E F G H I J K L M N O\n";
            string[] mark = { ". ", "O ", "X " };
            for (int row = 0; row < number; row++)
            {
                text += ((char)(Convert.ToInt32('A') + row)).ToString();
                text += " ";
                for (int col = 0; col < number; col++)
                {
                    text += mark[board[row, col]];
                }
                text += "\n";
            }
            return text;
        }

        int Get(int row, int col)
        {
            if (row < 0 || row >= number || col < 0 || col >= number)
            {
                return 0;
            }
            return board[row, col];
        }

        bool Put(int row, int col, int value)
        {
            if (row >= 0 && row < number && col >= 0 && col < number)
            {
                board[row, col] = value;
                return true;
            }
            return false;
        }

        public int Check()
        {
            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    if (board[row, col] == 0)
                    {
                        continue;
                    }
                    int id = board[row, col];
                    for (int i = 0; i < checkDirs.GetLength(0); i++)
                    {
                        int count = 1;
                        for (int move = 1; move < 5; move++)
                        {
                            if (Get(row + checkDirs[i, 0] * move, col + checkDirs[i, 1] * move) != id)
                            {
                                break;
                            }
                            count++;
                        }
                        if (count == 5)
                        {
                            won.Add(new int[2] { row, col });
                            for (int move = 1; move < 5; move++)
                            {
                                won.Add(new int[2] { row + checkDirs[i, 0] * move, col + checkDirs[i, 1] * move });
                            }
                            return id;
                        }
                    }
                }
            }
            return 0;
        }

        void SetColor(int color)
        {

        }

        public void Show()
        {
            Console.Write(Str());
        }
    }
}
