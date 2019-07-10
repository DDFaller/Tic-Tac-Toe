using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TicTacToe.Strategies;
using System;
using MachineLearning;


namespace TicTacToe.Core
{
    public class Player 
    {
        public static Dictionary<string, MarkData[]> boards = new Dictionary<string, MarkData[]>();

        public char id;
        public char[] board;

        public int attempts { get; set; }

        public ISelectPositionStrategy selectPositionStrategy = null;
        public string strategyName;

        public Player(char ID, string strat)
        {
            id = ID;
            strategyName = strat;
            ResetPlayer();
        }


        public void ResetPlayer()
        {
            board = new char[]
            {
                ' ',' ',' ',
                ' ',' ',' ',
                ' ',' ',' '
            };
            attempts = 0;
        }

        public void Mark(Action<int> positionChosenCallback)
        {
            attempts++;
            selectPositionStrategy.ChoosePosition(board, id, positionChosenCallback);
        }      
    }
}
