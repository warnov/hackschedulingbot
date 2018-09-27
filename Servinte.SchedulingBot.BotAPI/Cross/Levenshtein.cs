﻿using System;
using System.Collections.Generic;

namespace Contoso.SchedulingBot.BotAPI
{
    public static class Levenshtein
    {
        ///*****************************
        /// Compute Levenshtein distance 
        /// Memory efficient version
        ///*****************************
        public static int CalculateDistance(String sRow, String sCol)
        {
            int RowLen = sRow.Length;  // length of sRow
            int ColLen = sCol.Length;  // length of sCol
            int RowIdx;                // iterates through sRow
            int ColIdx;                // iterates through sCol
            char Row_i;                // ith character of sRow
            char Col_j;                // jth character of sCol
            int cost;                   // cost

            /// Test string length
            if (Math.Max(sRow.Length, sCol.Length) > Math.Pow(2, 31))
                throw (new Exception("\nMaximum string length in Levenshtein.iLD is " + Math.Pow(2, 31) + ".\nYours is " + Math.Max(sRow.Length, sCol.Length) + "."));

            // Step 1
            if (RowLen == 0)
            {
                return ColLen;
            }

            if (ColLen == 0)
            {
                return RowLen;
            }

            /// Create the two vectors
            int[] v0 = new int[RowLen + 1];
            int[] v1 = new int[RowLen + 1];
            int[] vTmp;

            /// Step 2
            /// Initialize the first vector
            for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
            {
                v0[RowIdx] = RowIdx;
            }

            // Step 3
            /// Fore each column
            for (ColIdx = 1; ColIdx <= ColLen; ColIdx++)
            {
                /// Set the 0'th element to the column number
                v1[0] = ColIdx;
                Col_j = sCol[ColIdx - 1];

                // Step 4
                /// Fore each row
                for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
                {
                    Row_i = sRow[RowIdx - 1];
                    // Step 5
                    if (Row_i == Col_j)
                    {
                        cost = 0;
                    }
                    else
                    {
                        cost = 1;
                    }
                    // Step 6
                    /// Find minimum
                    int m_min = v0[RowIdx] + 1;
                    int b = v1[RowIdx - 1] + 1;
                    int c = v0[RowIdx - 1] + cost;

                    if (b < m_min)
                    {
                        m_min = b;
                    }
                    if (c < m_min)
                    {
                        m_min = c;
                    }

                    v1[RowIdx] = m_min;
                }

                /// Swap the vectors
                vTmp = v0;
                v0 = v1;
                v1 = vTmp;
            }

            // Step 7

            /// Value between 0 - 100
            /// 0==perfect match 100==totaly different
            /// 
            /// The vectors where swaped one last time at the end of the last loop,
            /// that is why the result is now in v0 rather than in v1
            int max = System.Math.Max(RowLen, ColLen);
            return ((100 * v0[RowLen]) / max);
        }

        public static SortedList<double,string>CaculateDistances(string sample, string[] references)
        {
            var ret = new SortedList<double, string>();

            //This dictionary is used to know where a given score have been already produced and mantains an accumulated value
            //in order to generate slightly different keys to the sortedlist that is going to be returned,
            //as sorted liust cannot have duplicated keys
            var duplicatedScoreIndex = new Dictionary<int, double>();
            //This is the factor that will be added to the duplicatedScoreIndex each time a duplicated score is found
            //It is warranted that addition of subsequent factors won't ve greater than one unit
            var factor = 1d / references.Length;
            

            foreach (var reference in references)
            {
                var score = CalculateDistance(sample, reference);
                var addend = 0d;

                //Look for a duplicated score
                if (duplicatedScoreIndex.ContainsKey(score))
                {
                    addend = duplicatedScoreIndex[score] += factor;
                }
                else
                {
                    duplicatedScoreIndex.Add(score, 0d);
                }
                ret.Add(score + addend, reference);
            }
            return ret;
        }
    }
}