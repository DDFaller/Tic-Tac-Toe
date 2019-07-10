using UnityEngine;

using TicTacToe.Domain;
using TicTacToe.Domain.Impl;

namespace TicTacToe.Core
{
    public class App : MonoBehaviour
    {

        private static App _instance;
        public static App Instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = FindObjectOfType<App>();

                    if( _instance == null )
                    {
                        GameObject go = new GameObject( "[APPLICATION]" );
                        _instance = go.AddComponent<App>();
                    }
                }

                return _instance;
            }
        }

        public IGameBoard Board { get; private set; }
        public bool GameState = false;
        private void Awake()
        {
            _instance = this;
            
            InitializeDataStructures();
        }

        private void InitializeDataStructures()
        {
            // Normally we would use Dependency Injection, but for simplicity we'll initialize the data structures here
            Board = new GameBoard();
        }
        private void Update()
        {
            if (GameState) print("Winner");
        }
    }
}