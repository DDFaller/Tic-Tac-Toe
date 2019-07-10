using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MachineLearning;


namespace TicTacToe.Strategies
{
    public class AIStrategy : ISelectPositionStrategy
    {
        private Action<int> _positionChosenCallback;

        public void ChoosePosition(char[] board, char myID, Action<int> positionChosenCallback)
        {

            _positionChosenCallback = positionChosenCallback;
            int currentMove = -1;
            if (currentMove == -1)
            {
                currentMove = RandomPosition(board);
            }

            Input(currentMove);
        }

        public void EndMatch(MLValues value)
        {
            return;
        }
 
        public void Input(int position)
        {
            _positionChosenCallback?.Invoke(position);
            _positionChosenCallback = null;
        }

        public Dictionary<string, MarkData[]> GetStratBoard()
        {
            return null;
        }

        //private int FindLastPositionChanged(int[] board, int[] lastBoard)
        //{
        //    for (int i = 0; i < board.Length; i++)
        //    {
        //        if(board[i] != lastBoard[i])
        //        {
        //            EnemyID = board[i];
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //private int FirstMove()
        //{
        //    int[] InitialPositions = new int[] { 0, 2, 6, 8 };
        //    return InitialPositions[UnityEngine.Random.Range(0, InitialPositions.Length)];
        //}

        //private int BlockEnemy(int lastPos, int[] board)
        //{
        //    List<int> list = PositionsAround(lastPos,EnemyID, board);
        //    if (list.Count == 0) return -1;
        //    foreach(int i in list)
        //    {
        //        if (!CheckValidPosition(lastPos + (i - lastPos) * 2)) break;
        //        if(board[lastPos + (i - lastPos) * 2] == -1) return lastPos + (i - lastPos) * 2;

        //    }
        //    return -1;
        //}

        //private int CheckWin(char[] board,int id)
        //{
        //    List<int> list = PositionsAround(myLastMove, id, board);
        //    if (list.Count == 0) return -1;
        //    foreach(int i in list)
        //    {
        //        if (!CheckValidPosition(myLastMove + (i - myLastMove) * 2),board) break;
        //        if (board[myLastMove + (i - myLastMove) * 2] == -1) return myLastMove + (i - myLastMove) * 2;
        //    }
        //    return -1;
        //}
        ///// <summary>
        ///// Check Positions around the param lastPos, looking for position with same id.
        ///// </summary>
        //private List<int> PositionsAround(int lastPos, int id, char[] board)
        //{
        //    List<int> list = new List<int>();
        //    for(int i = lastPos - 4; i < lastPos; i++)
        //    {
        //        if (!CheckValidPosition(i),board) break;
        //        if (board[i] == id) list.Add(i);

        //    }
        //    for (int i = lastPos + 1; i < lastPos + 4; i++)
        //    {
        //        if (!CheckValidPosition(i)) { return list; }
        //        if (board[i] == id) list.Add(i);
        //    }

        //    return list;
        //}

        private int RandomPosition(char[] board)
        {
            int move = -1;
            List<int> list = new List<int>();
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == ' ') list.Add(i);
            }
            if (list.Count != 0)
            {
                move = list[UnityEngine.Random.Range(0, list.Count)];
                return move;
            }
            return 0;
        }
        //    //Debug.Log("Random Move : " + move.ToString());
        //    return 0;
        //}
        ///// <summary>
        ///// Checks if board can possibily contains the param pos, pos must be a value between [0,8]
        ///// </summary>
        //private bool CheckValidPosition(int pos,char[] board)
        //{
        //    if (pos >= 0 && pos < board.Length) return true;
        //    return false;
        //}
    }
}