using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MachineLearning;

namespace TicTacToe.Strategies
{
    public class HumanStrategy : ISelectPositionStrategy
    {
        private Action<int> _positionChosenCallback;

        public void ChoosePosition( char[] board, char myID, Action<int> positionChosenCallback)
        {
            _positionChosenCallback = positionChosenCallback;
        }

        public void EndMatch(MLValues value)
        {
            return;
        }

        public void Input(int position)
        {
            var tempCallback = _positionChosenCallback;
            _positionChosenCallback = null;
            tempCallback?.Invoke(position);
        }

        public Dictionary<string, MarkData[]> GetStratBoard()
        {
            return null;
        }
    }
}