using System.Collections.Generic;
using TicTacToe.Strategies;
using Newtonsoft.Json;
using System;
using UnityEngine;
using System.Linq;
using System.IO;
using TicTacToe.Core;

namespace MachineLearning
{
    public class MachineLearningstrat : ISelectPositionStrategy
    {
        private static Dictionary<string, MarkData[]> boards = new Dictionary<string, MarkData[]>();

        private Dictionary<string, MarkData[]> boardsToSave = new Dictionary<string, MarkData[]>();

        private List<MarkData[]> boardsDuringTheMatch = new List<MarkData[]>();
        private List<MarkData> dumpsBTWIntervals = new List<MarkData>();

        private Action<int> _positionChosenCallback;

        //Interval to Save data
        private int saveInterval = 0;
        public int maxInterval = 500000;
       

        //Alterate GameBoardController.OnSimulate too
        private string path = @"Tic-Tac-Toe Bot data.txt";

        public bool CanPlayToWin = false;

        //public GameMode ml = GameMode.HigherScore;

        public MachineLearningstrat()
        {
            if (MachineLearningstrat.boards.Count == 0) ReadData();
        }

        private void RewriteFile()
        {
            File.WriteAllText(path, string.Empty);
            List<string> list = new List<string>();
            foreach (var key in MachineLearningstrat.boards)
            {
                var json = JsonConvert.SerializeObject(key);
                list.Add(json);
            }
            File.AppendAllLines(path, list);
        }

        private void ReadData()
        {
            string[] file = File.ReadAllLines(path);
            string line;
            int i = 0;
            while (i < file.Length)
            {
                line = file[i];
                var deserialized = JsonConvert.DeserializeObject<KeyValuePair<string, MarkData[]>>(line);
                MachineLearningstrat.boards.Add(deserialized.Key, deserialized.Value);
                i++;
            }
        }

        public void ChoosePosition(char[] board, char myID, Action<int> positionChosenCallback)
        {
            _positionChosenCallback = positionChosenCallback;

            int currentMove = RandomPosition(board);

            string boardString = new string(board);
            #region Training

            bool contains = false;

            if (MachineLearningstrat.boards.ContainsKey(boardString))
            {
                contains = true;
                boardsDuringTheMatch.Add(MachineLearningstrat.boards[boardString]);
            }

            if (contains == false)
            {

                if (boardsToSave.ContainsKey(boardString)) boardsDuringTheMatch.Add(boardsToSave[boardString]);
                else
                {
                    MarkData[] newBoardDumps = GenerateDumpArray(boardString);
                    boardsToSave.Add(boardString, newBoardDumps);
                    boardsDuringTheMatch.Add(newBoardDumps);
                }
            }
            #endregion
            #region PTW
            if (CanPlayToWin)
            {
                if (MachineLearningstrat.boards.ContainsKey(boardString))
                {
                    var temp = MachineLearningstrat.boards[boardString];
                    int higher = temp[0].val;
                    int posToMark = temp[0].pos;
                    foreach (var m in temp)
                    {
                        if (m.val >= higher)
                        {
                            posToMark = m.pos;
                            higher = m.val;
                        }
                    }
                    currentMove = posToMark;
                }
            }
            #endregion
            Input(currentMove);
        }

        public void Input(int position)
        {
            dumpsBTWIntervals.Add(new MarkData(position));
            var tempCallback = _positionChosenCallback;
            _positionChosenCallback = null;
            tempCallback?.Invoke(position);
        }
        /// <summary>
                /// Called in GameBoard Controller when the match ends
                /// </summary>
        public void EndMatch(MLValues mLValues)
        {
            ///dumpsBTWIntervals == pontos marcados pelo MachineLearning
            for (int i = dumpsBTWIntervals.Count - 1; i >= 0; i--)
            {
                //boardsDuringTheMatch == todos os boards durante uma rodada
                foreach (var b in boardsDuringTheMatch[i])
                {
                    if (b.pos == dumpsBTWIntervals[i].pos) b.val += (int)mLValues;
                }
                boardsDuringTheMatch.RemoveAt(i);
                dumpsBTWIntervals.RemoveAt(i);
            }
            SaveInterval();
        }

        private void SaveInterval()
        {
            saveInterval++;
            if (saveInterval == maxInterval)
            {

                foreach (var board in boardsToSave)
                {
                    MachineLearningstrat.boards.Add(board.Key, board.Value);
                }
                boardsToSave = new Dictionary<string, MarkData[]>();
                RewriteFile();
                saveInterval = 0;
            }
        }

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

        private int WhiteSpace(string txt)
        {
            int i = 0;
            foreach (var item in txt)
            {
                if (item == ' ') i++;
            }
            return i;
        }

        private MarkData[] GenerateDumpArray(string text)
        {
            List<MarkData> tempDumps = new List<MarkData>();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    tempDumps.Add(new MarkData(i));
                }
            }
            return tempDumps.ToArray();
        }
    }
}
    
public enum MLValues
{
    Victory = 3,
    Lose = -3,
    FirstPlayerDraw = -1,
    SecondPlayerDraw = 1
};

public enum GameMode
{
    Random = 0 ,
    SemiRandom = 1 ,
    HigherScore = 2
};
    
