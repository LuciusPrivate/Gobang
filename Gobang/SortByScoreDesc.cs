using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobang
{
    public class SortByScoreDesc : IComparer<Move>
    {
        public int Compare(Move x, Move y)
        {
            if(y.score ==x.score)
            {
                if(y.row ==x.row)
                {
                    return y.col.CompareTo(x.col);
                }
                return y.row.CompareTo(x.row);
            }
            
            return y.score.CompareTo(x.score);
        }
    }
}
