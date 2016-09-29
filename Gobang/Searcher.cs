using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobang
{
    public class Searcher
    {
        Evaluation evaluator;
        //public int[,] board;
        static int number = 15;
        Move bestMove;
        int gameOver;
        int overValue;
        int maxDepth;

        public void Init()
        {
            //board = new int[number, number];
            //for (int row = 0; row < number; row++)
            //{
            //    for (int col = 0; col < number; col++)
            //    {
            //        board[row, col] = 0;
            //    }
            //}
            gameOver = 0;
            overValue = 0;
            maxDepth = 3;
            evaluator = new Evaluation();
            evaluator.Init();
            //bestMove = new int[2] { -100, -100 };
        }

        List<Move> GenMove(int turn, int[,] board)
        {
            var moves = new List<Move>();
            int[,] POSES = evaluator.POS;
            int score;
            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    if (board[row, col] == 0)
                    {
                        score = POSES[row, col];
                        moves.Add(new Move
                        {
                            score = score,
                            row = row,
                            col = col
                        });
                    }
                }
            }
            moves.Sort( new SortByScoreDesc());
//moves.Reverse();
            return moves;
        }

        //http://www.xqbase.com/computer/search_minimax.htm
        //http://www.xqbase.com/computer/search_alphabeta.htm
        int _Search(int turn, int[,] board, int depth, int alpha = -0x7fffffff, int beta = 0x7fffffff)
        {
            int score;
            if (depth <= 0)
            {
                score = evaluator.Evaluate(board, turn);
                return score;
            }
            score = evaluator.Evaluate(board, turn);

            if (Math.Abs(score) >= 9999 && depth < maxDepth)
            {
                return score;
            }

            List<Move> moves = GenMove(turn, board);
            Move best = null;

            int scoreValue, row, col;
                          
            int other = 1;
            if (turn == 1)
            {
                other = 2;
            }

            foreach (Move move in moves)
            {
                scoreValue = move.score;
                row = move.row;
                col = move.col;
                board[row, col] = turn;

                //if (row == 14 && col == 14)
                //{
                //    Console.WriteLine();
                //}

                scoreValue = -_Search(other, board, depth - 1, -beta, -alpha);

                board[row, col] = 0;

                if (scoreValue > alpha)
                {
                    best = new Move();
                    alpha = scoreValue;
                    best.score = alpha;
                    best.row = row;
                    best.col = col;
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
            }

            if (depth == maxDepth && best != null)
            {
                bestMove = best;
            }

            return alpha;
        }

        public Move Search(int turn, int[,] board, int depth = 3)
        {
            maxDepth = depth;
            bestMove  = new Move();
            int score = _Search(turn, board, depth);
            if (Math.Abs(score) > 8000)
            {
                maxDepth = depth;
                score = _Search(turn, board, 1);
            }

            //bestMove.score = score; //???

            return bestMove;
        }
    }
}
