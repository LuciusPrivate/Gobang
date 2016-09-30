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
        static int number = 15;
        Move bestMove;
        int maxDepth;

        //初始化
        public void Init()
        {
            maxDepth = 3;
            evaluator = new Evaluation();
            evaluator.Init();
        }

        //产生当前棋局的走法
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
            return moves;
        }

        //http://www.xqbase.com/computer/search_minimax.htm
        //http://www.xqbase.com/computer/search_alphabeta.htm
        //递归搜索：返回最佳分数
        int _Search(int turn, int[,] board, int depth, int alpha = -0x7fffffff, int beta = 0x7fffffff)
        {
            int score;
            //深度为零则评估棋盘并返回
            if (depth <= 0)
            {
                score = evaluator.Evaluate(board, turn);
                return score;
            }
            //如果游戏结束则立马返回
            score = evaluator.Evaluate(board, turn);
            if (Math.Abs(score) >= 9999 && depth < maxDepth)
            {
                return score;
            }

            //产生新的走法
            List<Move> moves = GenMove(turn, board);
            Move best = null;

            int scoreValue, row, col;

            //计算下一回合该谁走              
            int other = 1;
            if (turn == 1)
            {
                other = 2;
            }

            //枚举当前所有走法
            foreach (Move move in moves)
            {
                scoreValue = move.score;
                row = move.row;
                col = move.col;
                //标记当前走法到棋盘
                board[row, col] = turn;

                //深度优先搜索，返回评分，走的行和走的列
                scoreValue = -_Search(other, board, depth - 1, -beta, -alpha);

                //棋盘上清除当前走法
                board[row, col] = 0;

                //计算最好分值的走法
                //alpha/beta 剪枝
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

            //如果是第一层则记录最好的走法
            if (depth == maxDepth && best != null)
            {
                bestMove = best;
            }

            return alpha;
        }

        //具体搜索：传入当前是该谁走(turn=1/2)，以及搜索深度(depth)
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

            return bestMove;
        }
    }
}
