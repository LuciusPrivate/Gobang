using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobang
{
    public class Evaluation
    {
        static int number = 15;
        public int[,] POS;

        //模式
        int STWO = 1; //冲二
        int STHREE = 2; //冲三
        int SFOUR = 3; //冲四
        int TWO = 4; //活二
        int THREE = 5; //活三
        int FOUR = 6; //活四
        int FIVE = 7; //活五
        //int DFOUR = 8; //双四
        //int FOURT = 9; //四三
        //int DTHREE = 10; //双三
        //int NOTYPE = 11;
        int ANALYZED = 255; //已经分析过
        int TODO = 0; //没有分析过

        int BLACK = 1; //黑棋
        int WHITE = 2; //白棋

        int[,] count; //每种棋局的个数：count[黑棋 / 白棋][模式]

        int[] result; //保存当前直线分析值
        int[] line; //当前直线数据
        int[,,] record; //全盘分析结果 [row][col][方向]

        public void Init()
        {
            count = new int[3, 20];
            result = new int[number * 2];
            line = new int[number * 2];
            record = new int[number, number, 4];

            POS = new int[number, number];
            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    POS[row, col] = 7 - getMax(Math.Abs(row - 7), Math.Abs(col - 7));
                }
            }

            for (int i = 0; i < number * 2; i++)
            {
                result[i] = TODO;
            }

            Reset();
        }

        int getMax(int a, int b)
        {
            return a > b ? a : b;
        }

        //复位数据
        void Reset()
        {

            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    for (int dir = 0; dir < 4; dir++)
                    {
                        record[row, col, dir] = TODO;
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    count[i, j] = 0;
                }
            }
        }

        void ResetLineResult()
        {
            for (int i = 0; i < number * 2; i++)
            {
                result[i] = TODO;
            }

            for (int i = 0; i < number * 2; i++)
            {
                line[i] = 0;
            }
        }

        public int Evaluate(int[,] board, int turn)
        {
            int score = _Evaluate(board, turn);
            int other = 1;
            if(turn ==1)
            {
                other = 2;
            }
            if (score < -9000)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (count[other, i] > 0)
                    {
                        score -= i;
                    }
                }
            }
            else if (score > 9000)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (count[turn, i] > 0)
                    {
                        score += i;
                    }
                }
            }
            return score;
        }

        //四个方向（水平，垂直，左斜，右斜）分析评估棋盘，然后根据分析结果打分
        int _Evaluate(int[,] board, int turn)
        {
            Reset();
            //四个方向分析
            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    if (board[row, col] != 0)
                    {
                        if (record[row, col, 0] == TODO) //水平
                        {
                            Analysis_horizon(board, row, col);
                        }
                        if (record[row, col, 1] == TODO) //垂直
                        {
                            Analysis_vertical(board, row, col);
                        }
                        if (record[row, col, 2] == TODO) //左斜
                        {
                            Analysis_leftInclined(board, row, col);
                        }
                        if (record[row, col, 3] == TODO) //右斜
                        {
                            Analysis_rightInclined(board, row, col);
                        }
                    }
                }
            }

            //分别对白棋黑棋计算：FIVE, FOUR, THREE, TWO等出现的次数
            int[] check = new int[] { FIVE, FOUR, SFOUR, THREE, STHREE, TWO, STWO };

            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    int value = board[row, col];
                    if (value != 0)
                    {
                        int ch;
                        for (int dir = 0; dir < 4; dir++)
                        {
                            ch = record[row, col, dir];
                            if (check.Contains<int>(ch))
                            {
                                count[value, ch] += 1;
                            }
                        }
                    }
                }
            }

            //如果有五连则马上返回分
            if (turn == WHITE)
            {
                if (count[BLACK, FIVE] != 0)
                {
                    return -9999;
                }
                if (count[WHITE, FIVE] != 0)
                {
                    return 9999;
                }
            }
            else if (turn == BLACK)
            {
                if (count[WHITE, FIVE] != 0)
                {
                    return -9999;
                }
                if (count[BLACK, FIVE] != 0)
                {
                    return 9999;
                }
            }

            //如果存在两个冲四，则相当于有一个活四
            if (count[WHITE, SFOUR] >= 2)
            {
                count[WHITE, FOUR] += 1;
            }

            if (count[BLACK, SFOUR] >= 2)
            {
                count[BLACK, FOUR] += 1;
            }

            //具体打分
            int bvalue = 0, wvalue = 0;

            if (turn == WHITE)
            {

                if (count[WHITE, FOUR] > 0)
                {
                    return 9990;
                }
                if (count[WHITE, SFOUR] > 0)
                {
                    return 9980;
                }
                if (count[BLACK, FOUR] > 0)
                {
                    return -9970;
                }
                if (count[BLACK, SFOUR] != 0 && count[BLACK, THREE] != 0)
                {
                    return -9960;
                }
                if (count[WHITE, THREE] != 0 && count[BLACK, SFOUR] == 0)
                {
                    return 9950;
                }
                if (count[BLACK, THREE] > 1 &&
                count[WHITE, SFOUR] == 0 &&
                count[WHITE, THREE] == 0 &&
                count[WHITE, STHREE] == 0)
                {
                    return -9940;
                }
                if (count[WHITE, THREE] > 1)
                {
                    wvalue += 2000;
                }

                else if (count[WHITE, THREE] != 0)
                {
                    wvalue += 200;
                }
                if (count[BLACK, THREE] > 1)
                {
                    bvalue += 500;
                }
                else if (count[BLACK, THREE] != 0)
                {
                    bvalue += 100;
                }
                if (count[WHITE, STHREE] != 0)
                {
                    wvalue += count[WHITE, STHREE] * 10;
                }
                if (count[BLACK, STHREE] != 0)
                {
                    bvalue += count[BLACK, STHREE] * 10;
                }
                if (count[WHITE, TWO] != 0)
                {
                    wvalue += count[WHITE, TWO] * 4;
                }
                if (count[BLACK, TWO] != 0)
                {
                    bvalue += count[BLACK, TWO] * 4;
                }
                if (count[WHITE, STWO] != 0)
                {
                    wvalue += count[WHITE, STWO];
                }
                if (count[BLACK, STWO] != 0)
                {
                    bvalue += count[BLACK, STWO];
                }
            }
            else
            {
                if (count[BLACK, FOUR] > 0)
                {
                    return 9990;
                }
                if (count[BLACK, SFOUR] > 0)
                {
                    return 9980;
                }
                if (count[WHITE, FOUR] > 0)
                {
                    return -9970;
                }
                if (count[WHITE, SFOUR] != 0 && count[WHITE, THREE] != 0)
                {
                    return -9960;
                }
                if (count[BLACK, THREE] != 0 && count[WHITE, SFOUR] == 0)
                {
                    return 9950;
                }
                if (count[WHITE, THREE] > 1 &&
                count[BLACK, SFOUR] == 0 &&
                count[BLACK, THREE] == 0 &&
                count[BLACK, STHREE] == 0)
                {
                    return -9940;
                }
                if (count[BLACK, THREE] > 1)
                {
                    bvalue += 2000;
                }

                else if (count[BLACK, THREE] != 0)
                {
                    bvalue += 200;
                }
                if (count[WHITE, THREE] > 1)
                {
                    wvalue += 500;
                }
                else if (count[WHITE, THREE] != 0)
                {
                    wvalue += 100;
                }
                if (count[BLACK, STHREE] != 0)
                {
                    bvalue += count[BLACK, STHREE] * 10;
                }
                if (count[WHITE, STHREE] != 0)
                {
                    wvalue += count[WHITE, STHREE] * 10;
                }
                if (count[BLACK, TWO] != 0)
                {
                    bvalue += count[BLACK, TWO] * 4;
                }
                if (count[WHITE, TWO] != 0)
                {
                    wvalue += count[WHITE, TWO] * 4;
                }
                if (count[BLACK, STWO] != 0)
                {
                    bvalue += count[BLACK, STWO];
                }
                if (count[WHITE, STWO] != 0)
                {
                    wvalue += count[WHITE, STWO];
                }
            }

            //加上位置权值，棋盘最中心点权值是7，往外一格-1，最外圈是0
            int wc = 0, bc = 0;
            for (int row = 0; row < number; row++)
            {
                for (int col = 0; col < number; col++)
                {
                    int value = board[row, col];
                    if (value != 0)
                    {
                        if (value == WHITE)
                        {
                            wc += POS[row, col];
                        }
                        else
                        {
                            bc += POS[row, col];
                        }
                    }
                }
            }

            wvalue += wc;

            bvalue += bc;

            if(turn ==WHITE)
            {
                return wvalue - bvalue;
            }

            return bvalue - wvalue;
        }

        //分析水平
        int Analysis_horizon(int[,] board, int row, int col)
        {
            ResetLineResult();

            for (int i = 0; i < number; i++)
            {
                line[i] = board[row, i];
            }

            Analysis_line(line, result, number, col);

            for (int i = 0; i < number; i++)
            {
                if (result[i] != TODO)
                {
                    record[row, i, 0] = result[i];
                }
            }

            return record[row, col, 0];
        }

        //分析垂直
        int Analysis_vertical(int[,] board, int row, int col)
        {
            ResetLineResult();

            for (int i = 0; i < number; i++)
            {
                line[i] = board[i, col];
            }

            Analysis_line(line, result, number, row);

            for (int i = 0; i < number; i++)
            {
                if (result[i] != TODO)
                {
                    record[i, col, 1] = result[i];
                }
            }

            return record[row, col, 1];
        }

        //分析左斜
        int Analysis_leftInclined(int[,] board, int row, int col)
        {
            ResetLineResult();

            int x, y, k;
            if (row < col)
            {
                x = col - row;
                y = 0;
            }
            else
            {
                x = 0;
                y = row - col;
            }

            k = 0;
            while (k < number)
            {
                if (x + k > 14 || y + k > 14)
                {
                    break;
                }
                line[k] = board[(y + k), (x + k)];
                k++;
            }

            Analysis_line(line, result, k, col - x);

            for (int i = 0; i < k; i++)
            {
                if (result[i] != TODO)
                {
                    record[y + i, x + i, 2] = result[i];
                }
            }

            return record[row, col, 2];
        }

        //分析右斜
        int Analysis_rightInclined(int[,] board, int row, int col)
        {
            ResetLineResult();

            int x, y, k;
            if (14 - row < col)
            {
                x = col - 14 + row;
                y = 14;
            }
            else
            {
                x = 0;
                y = row + col;
            }

            k = 0;
            while (k < number)
            {
                if (x + k > 14 || y - k < 0)
                {
                    break;
                }
                line[k] = board[(y - k), (x + k)];
                k++;
            }

            Analysis_line(line, result, k, col - x);

            for (int i = 0; i < k; i++)
            {
                if (result[i] != TODO)
                {
                    record[y - i, x + i, 3] = result[i];
                }
            }

            return record[row, col, 3];
        }

        //int Test(int[,] board)
        //{
        //    return 0;
        //}

        //分析一条线：五四三二等棋型
        int Analysis_line(int[] line, int[] result, int num, int position)
        {
            for (int i = num; i < number * 2; i++)
            {
                line[i] = number;
            }
            for (int i = num; i < num; i++)
            {
                result[i] = TODO;
            }
            if (num < 5)
            {
                for (int i = num; i < num; i++)
                {
                    result[i] = ANALYZED;
                }
                return 0;
            }

            int value = line[position];

            int[] list = new int[] { 0, 2, 1 };
            int inverse = list[value];

            num -= 1;

            int xl = position;
            int xr = position;

            while (xl > 0) //探索左边界
            {
                if (line[xl - 1] != value)
                {
                    break;
                }
                xl -= 1;
            }
            while (xr < num) //探索右边界
            {
                if (line[xr + 1] != value)
                {
                    break;
                }
                xr += 1;
            }

            int left_range = xl;
            int right_range = xr;

            while (left_range > 0) // 探索左边范围（非对方棋子的格子坐标）
            {
                if (line[left_range - 1] == inverse)
                {
                    break;
                }
                left_range -= 1;
            }
            while (right_range < num) //探索右边范围（非对方棋子的格子坐标
            {
                if (line[right_range + 1] == inverse)
                {
                    break;
                }
                right_range += 1;
            }

            //如果该直线范围小于 5，则直接返回
            if (right_range - left_range < 4)
            {
                for (int i = left_range; i < right_range + 1; i++)
                {
                    result[i] = ANALYZED;
                }
                return 0;
            }

            //设置已经分析过
            for (int i = xl; i < xr + 1; i++)
            {
                result[i] = ANALYZED;
            }

            int range = xr - xl;
            //如果是 5连
            if (range >= 4)
            {
                result[position] = FIVE;
                return FIVE;
            }
            //如果是 4连
            else if (range == 3)
            {
                bool leftFour = false; //是否左边是空格
                if (xl > 0)
                {
                    if (line[xl - 1] == 0) //活四
                    {
                        leftFour = true;
                    }
                }
                if (xr < num)
                {
                    if (line[xr + 1] == 0)
                    {
                        if (leftFour)
                        {
                            result[position] = FOUR; //活四
                        }
                        else
                        {
                            result[position] = SFOUR; //冲四
                        }
                    }
                    else
                    {
                        if (leftFour)
                        {
                            result[position] = SFOUR; //冲四
                        }
                    }
                }
                else
                {
                    if (leftFour)
                    {
                        result[position] = SFOUR; //冲四
                    }
                }

                return result[position];
            }
            //如果是 3连
            else if (range == 2)
            {
                bool leftThree = false; //是否左边是空格
                if (xl > 0)
                {
                    if (line[xl - 1] == 0) //左边有空
                    {
                        if (xl > 1 && line[xl - 2] == value)
                        {
                            result[xl] = SFOUR;
                            result[xl - 2] = ANALYZED;
                        }
                        else
                        {
                            leftThree = true;
                        }
                    }
                    else if (xr == num || line[xr + 1] != 0)
                    {
                        return 0;
                    }
                }
                if (xr < num)
                {
                    if (line[xr + 1] == 0) //右边有气
                    {
                        if (xr < num - 1 && line[xr + 2] == value)
                        {
                            result[xr] = SFOUR; //XXX-X 相当于冲四
                            result[xr - 2] = ANALYZED;
                        }
                        else if (leftThree)
                        {
                            result[xr] = THREE;
                        }
                        else
                        {
                            result[xr] = STHREE;
                        }
                    }
                    else if (result[xl] == SFOUR)
                    {
                        return result[xl];
                    }
                    else if (leftThree)
                    {
                        result[position] = STHREE;
                    }
                }
                else
                {
                    if (result[xl] == SFOUR)
                    {
                        return result[xl];
                    }
                    if (leftThree)
                    {
                        result[position] = STHREE;
                    }
                }

                return result[position];
            }
            //如果是 2连
            else if (range == 1)
            {
                bool leftTwo = false;
                if (xl > 2)
                {
                    if (line[xl - 1] == 0) //左边用空
                    {
                        if (line[xl - 2] == value)
                        {
                            if (line[xl - 3] == value)
                            {
                                result[xl] = SFOUR;
                                result[xl - 2] = ANALYZED;
                                result[xl - 3] = ANALYZED;
                            }
                            else if (line[xl - 3] == 0)
                            {
                                result[xl - 2] = ANALYZED;
                                result[xl] = STHREE;
                            }
                        }
                        else
                        {
                            leftTwo = true;
                        }
                    }
                    //else if (xr == num || line[xr + 1] != 0)
                    //{
                    //    return 0;
                    //}
                }
                if (xr < num)
                {
                    if (line[xr + 1] == 0) //右边有空
                    {
                        if (xr < num - 2 && line[xr + 2] == value)
                        {
                            if (line[xr - 3] == value)
                            {
                                result[xr] = SFOUR;
                                result[xr - 2] = ANALYZED;
                                result[xr - 3] = ANALYZED;
                            }
                            else if (line[xr - 3] == value)
                            {
                                result[xr + 2] = ANALYZED;
                                if (leftTwo)
                                {
                                    result[xr] = THREE;
                                }
                                else
                                {
                                    result[xr] = STHREE;
                                }
                            }
                        }
                        else
                        {
                            if (result[xl] == SFOUR)
                            {
                                return result[xl];
                            }
                            if (result[xl] == STHREE)
                            {
                                result[xl] = THREE;
                                return result[xl];
                            }
                            if (leftTwo)
                            {
                                result[position] = TWO;
                            }
                            else
                            {
                                result[position] = STWO;
                            }
                        }
                    }
                    else
                    {
                        if (result[xl] == SFOUR)
                        {
                            return result[xl];
                        }
                        if (leftTwo)
                        {
                            result[position] = STWO;
                        }
                    }
                }

                return result[position];
            }

            return 0;
        }
    }
}
