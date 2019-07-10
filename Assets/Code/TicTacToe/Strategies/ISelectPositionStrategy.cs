using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MachineLearning;

namespace TicTacToe.Strategies
{
    public interface ISelectPositionStrategy
    {
        void ChoosePosition(char[] board, char myID, Action<int> positionChosenCallback);

        void EndMatch(MLValues value);

        void Input(int position);
    }
}