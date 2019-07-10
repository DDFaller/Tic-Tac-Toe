using System;
using System.Collections.Generic;
using TicTacToe.Events;
using UnityEngine;

namespace TicTacToe.Domain.Impl
{
    public class GameBoard : IGameBoard
    {
        public event EventHandler<GameBoardEventData> E_BoardPositionChanged = delegate { };
        public event EventHandler<GameBoardEventData> E_BoardPositionInvalid = delegate { };
        public event EventHandler<GameBoardEventData> E_BoardPositionOccupied = delegate { };

        public event Action<char> E_BoardWinner;
        public event Action E_BoardCatsGame;

        private List<int> Positions = new List<int>();

        private char[] _board;

        private int _pos;
        private char _ID;

        public GameBoard()
        {
            ResetBoard();
        }

        public void ResetBoard()
        {
            _board = new char[]{
                ' ', ' ',' ',
                ' ', ' ',' ',
                ' ', ' ',' '
            };
        }

        public int GetPositionID(int i)
        {
            return _board[i];
        }

        public bool CheckValidPosition(int position)
        {
            if (position < 0 || position >= _board.Length) return false;
            return true;
        }

        public bool AvailableSpaceInBoard()
        {
            for (int i = 0; i < _board.Length; i++)
            {
                if (_board[i] == ' ') return true;
            }
            return false;
        }

        public void SetPosition( int position, char id )
        {
            if(!CheckValidPosition(position))
            {
                E_BoardPositionInvalid( this, new GameBoardEventData( position, id ) );
                return;
            }
            if (GetPositionID(position) != ' ')
            {
                E_BoardPositionOccupied(this, new GameBoardEventData(position, id));
                return;
            }
            
            GameBoardEventData e = new GameBoardEventData(position, id);
            _board[position] = id;
            
            _pos = position;
            _ID = id;
            CheckWinner(e);
            E_BoardPositionChanged( this, e);
        }

        public int VictoryLinePosition()
        {
            GameBoardEventData e = new GameBoardEventData(_pos, _ID); 
            int pos = e.Position;
            if (CheckDiagonal(e))
                if (GetPositionID(0) == e.ID) return 6;
                else return 7;
            else if (CheckRown(e))
            {
                if (pos >= 6) return 2;
                if (pos >= 3) return 1;
                else return 0;
            }
            else
            {
                if (pos % 3 == 0) return 3;
                if (pos % 3 == 1) return 4;
                else return 5;
            }
        }

        private void CheckWinner(GameBoardEventData e)
        {
            if(CheckDiagonal(e) || CheckRown(e) || CheckColumn(e))
            {
                E_BoardWinner(e.ID);
                //board.Winner = e.ID;
                return;
            }
            if (!AvailableSpaceInBoard())
            {
                E_BoardCatsGame();
                return;
            }
        }
        #region Auxiliares
        private bool CheckDiagonal(GameBoardEventData e)
        {
            int id = e.ID;
            int position = e.Position;
            if (GetPositionID(4) != id) return false;
            if (GetPositionID(0) == id && GetPositionID(8) == id) return true;
            if (GetPositionID(2) == id && GetPositionID(6) == id) return true;
            return false;
        }
        private bool CheckRown(GameBoardEventData e)
        {
            int id = e.ID;
            int position = e.Position;
            if (e.Position % 3 == 0) if (GetPositionID(position + 1) == id && GetPositionID(position + 2) == id)  return true;
            if (e.Position % 3 == 1) if (GetPositionID(position - 1) == id && GetPositionID(position + 1) == id)  return true;
            if (e.Position % 3 == 2) if (GetPositionID(position - 1) == id && GetPositionID(position - 2) == id)  return true;
            return false;
        }
        private bool CheckColumn(GameBoardEventData e)
        {
            int id = e.ID;
            int position = e.Position;
            if (position < 3 && position >= 0) if (GetPositionID(position + 3) == id && GetPositionID(position + 6) == id) return true;
            if (position < 6 && position >= 3) if (GetPositionID(position - 3) == id && GetPositionID(position + 3) == id) return true;
            if (position < 9 && position >= 6) if (GetPositionID(position - 3) == id && GetPositionID(position - 6) == id) return true;
            return false;

        }
        #endregion
    }
}