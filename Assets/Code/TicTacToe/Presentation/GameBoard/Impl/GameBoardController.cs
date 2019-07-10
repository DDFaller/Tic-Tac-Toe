using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TicTacToe.Core;
using TicTacToe.Strategies;
using UnityEngine;
using UnityEngine.UI;
using MachineLearning;
using System.IO;

namespace TicTacToe.Presentation.GameBoard.Impl
{
    public class GameBoardController : Controller<IGameBoardView>, IGameBoardController
    {
        private bool canPlay = true;

        [SerializeField] public Player player1;
        [SerializeField] public Player player2;
        public List<Player> players = new List<Player>();
        public Player currentPlayer = null;

        public StrategyVariable strategyVariable;

        public Slider SimulateProgress;

        private string path = @"Tic-Tac-Toe Bot data.txt";

        public override void Initialize()
        {
            List<string> Strategy = GetStrategies<ISelectPositionStrategy>();
            strategyVariable.Value = Strategy;
            App.Instance.Board.E_BoardPositionChanged += OnBoardPositionChanged;
            App.Instance.Board.E_BoardPositionInvalid += OnBoardPositionInvalid;
            App.Instance.Board.E_BoardPositionOccupied += OnBoardPositionOccupied;
            App.Instance.Board.E_BoardWinner += OnPlayerVictory;
            App.Instance.Board.E_BoardCatsGame += OnTie;
        }

        override public List<Player> GetPlayers()
        {
            return players;
        }

        private void OnBoardPositionChanged( object sender, Events.GameBoardEventData e )
        {
            View.SetPlayerMark( e.ID, e.Position );
            player1.board[e.Position] = e.ID;
            player2.board[e.Position] = e.ID;
            if (canPlay) ChangeTurn();
        }

        private void OnBoardPositionInvalid( object sender, Events.GameBoardEventData e )
        {
            UnityEngine.Debug.LogErrorFormat( "Position {0} is invalid.", e.Position );
        }

        private void OnBoardPositionOccupied( object sender, Events.GameBoardEventData e )
        {
            //Debug.LogFormat( "Position {0} is occupied. Please try another position", e.Position );
            currentPlayer.Mark((int position) => App.Instance.Board.SetPosition(position, currentPlayer.id));
        }

        private void OnPlayerVictory(char id)
        {
            canPlay = false;
            if (id == 'X') id = '1';
            if (id == 'O') id = '2';
            View.ShowEndMessage("Player " + id.ToString()  + " wins");
            View.ShowVictoryLine(App.Instance.Board.VictoryLinePosition());

            if (currentPlayer.id == player1.id)
            {
                player2.selectPositionStrategy.EndMatch(MLValues.Lose);
                player1.selectPositionStrategy.EndMatch(MLValues.Victory);
            }
            else
            {
                player1.selectPositionStrategy.EndMatch(MLValues.Lose);
                player2.selectPositionStrategy.EndMatch(MLValues.Victory);
            }
            View.PlayersReady -= () => currentPlayer.Mark((int position) => App.Instance.Board.SetPosition(position, currentPlayer.id));
        }

        public void ResetGame()
        {
            App.Instance.Board.ResetBoard();
            player1.ResetPlayer();
            player2.ResetPlayer();
            
            canPlay = true;
            currentPlayer = player1;
            View.ResetBoard(currentPlayer.id);
            currentPlayer.Mark((int position) => App.Instance.Board.SetPosition(position, currentPlayer.id));
        }
        
        private void OnTie()
        {
            canPlay = false;
            View.ShowEndMessage("Tie");
            player1.selectPositionStrategy.EndMatch(MLValues.FirstPlayerDraw);
            player2.selectPositionStrategy.EndMatch(MLValues.SecondPlayerDraw);
            View.PlayersReady -= () => currentPlayer.Mark((int position) => App.Instance.Board.SetPosition(position, currentPlayer.id));
        }

        private void ChangeTurn()
        {
            if (currentPlayer.id == player1.id) currentPlayer = player2;
            else currentPlayer = player1;
            View.ChangeTurnID(currentPlayer.id);
            if (canPlay) currentPlayer.Mark((int position) => App.Instance.Board.SetPosition(position, currentPlayer.id));
        }

        public void SetCurrentPlayers()
        {
            player1 = View.GetPlayer1();
            player2 = View.GetPlayer2();

            player1.selectPositionStrategy = InstantiateStrategy<ISelectPositionStrategy>(player1.strategyName);
            player2.selectPositionStrategy = InstantiateStrategy<ISelectPositionStrategy>(player2.strategyName);

            View.PlayerSettings( player1 );
            View.PlayerSettings( player2 );

            players.Add(player1);
            players.Add(player2);

            Debug.LogFormat("Player 1 id {0}, Player 2 id {1}", player1.id, player2.id);
            currentPlayer = player1;
            View.ResetBoard(currentPlayer.id);
            App.Instance.Board.ResetBoard();
            canPlay = true;
            View.PlayersReady += () =>StartMark();
            
        }

        private void StartMark()
        {
            currentPlayer.Mark((int position) => App.Instance.Board.SetPosition(position, currentPlayer.id));
        }

        public void OnSimulate()
        {
            SetCurrentPlayers();
            StopCoroutine("Simulate");
            View.PlayersReady += () =>StartCoroutine("Simulate");
        }

        public IEnumerator Simulate()
        {
            SimulateProgress.gameObject.SetActive(true);
            int matches = View.GetMatches();
            float avgAttempts = 0;
            int player1Victories = 0;
            int player2Victories = 0;
            float updateTime = 0.3f;
            var currentTime = (DateTime.Now);
            for (int i = 0; i < matches; i++)
            {
                ResetGame();
                if (!canPlay)
                {
                    SimulateProgress.value = (float)i / matches;
                    if (currentPlayer.id == player1.id) player1Victories++;
                    else player2Victories++;

                    avgAttempts += player1.attempts;
                }
                if ((DateTime.Now - currentTime).TotalSeconds > updateTime) { currentTime = DateTime.Now; yield return null; }
            }
            //SaveFile();

            Debug.Log("Tentativas em Média : " + avgAttempts.ToString());
            Debug.Log("Player 1 won " + player1Victories + " matches / " + matches.ToString());
            yield return new WaitForSeconds(2f);
            SimulateProgress.gameObject.SetActive(false);

        }

        private void CheckRepeatedBoards()
        {
            if (!File.Exists(path)) File.CreateText(path);


            HashSet<string> configurations = new HashSet<string>();
            int duplicatedCount = 0;
            string line;
            string[] file = File.ReadAllLines(path);
            int i = 0;
            while (i < file.Length)
            {
                line = file[i];
                int first = line.IndexOf("Key") + "Key".Length + 3;
                int last = line.IndexOf("Key") + "Key".Length + 12;
                string str = line.Substring(first, last - first);
                if (configurations.Contains(str))
                {
                    Debug.Log("Duplicada!!!!!");
                    duplicatedCount++;
                }
                else
                {
                    configurations.Add(str);
                }
                i++;
            }
            Debug.Log(duplicatedCount);
            Debug.Log("Fim da checagem de repeticao");
        }

        public void InvokePlayerClick(int position)
        {
            if(canPlay)
            currentPlayer.selectPositionStrategy.Input(position);
        }
        
        #region  Reflection
        private List<string> GetStrategies<T>()
        {
            return typeof(T).Assembly.GetTypes()
                .Where(p => p.IsClass && typeof(T).IsAssignableFrom(p))
                .Select(t => GetTypeName(t))
                .OrderByDescending(x => x[0]).ToList();
        }

        private T InstantiateStrategy<T>(string strategy)
        {
            var type = typeof(T).Assembly.GetTypes()
                .FirstOrDefault(p => p.IsClass && typeof(T).IsAssignableFrom(p) && GetTypeName(p) == strategy);

            return (T)Activator.CreateInstance(type);
        }

        private string GetTypeName(Type type)
        {   
            return string.Join("", Regex.Matches(type.Name, @"([A-Z][A-z]+)").Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value));
        }
        #endregion
    }

}