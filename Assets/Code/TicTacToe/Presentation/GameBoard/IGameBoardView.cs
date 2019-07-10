using TicTacToe.Core;
using System;
namespace TicTacToe.Presentation.GameBoard
{
    public interface IGameBoardView : IView
    {
        int GetChosenPosition(int position);
        void ChangeTurnID(char id);
        void ShowEndMessage(string text);
        void ShowVictoryLine(int position);
        void ResetBoard(char id);
        void SetPlayerMark( char playerID, int position );
        int GetMatches();
        #region Strategies
        Player GetPlayer1();
        Player GetPlayer2();

        event Action PlayersReady;

        void PlayerSettings( Player player );
        #endregion
    }
}