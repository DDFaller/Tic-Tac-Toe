using System;

using TicTacToe.Events;

namespace TicTacToe.Domain
{
    public interface IGameBoard
    {
        event EventHandler<GameBoardEventData> E_BoardPositionChanged;
        event EventHandler<GameBoardEventData> E_BoardPositionInvalid;
        event EventHandler<GameBoardEventData> E_BoardPositionOccupied;

        event Action<char> E_BoardWinner;
        event Action E_BoardCatsGame;

        void ResetBoard();
        void SetPosition( int position, char id );
        int VictoryLinePosition();



    }
}