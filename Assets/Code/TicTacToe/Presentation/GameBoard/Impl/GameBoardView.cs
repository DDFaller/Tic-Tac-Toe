
using System.Collections.Generic;
using TicTacToe.Core;
using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Strategies;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Reflection;

namespace TicTacToe.Presentation.GameBoard.Impl
{
    public class  GameBoardView : View<IGameBoardController>, IGameBoardView
    {
       
        [SerializeField] private InputField _player1IDInput;
        [SerializeField] private InputField _player2IDInput;
        
        [SerializeField] private Dropdown _player1StratDropdown;
        [SerializeField] private Dropdown _player2StratDropdown;
        
        public Text EndMessage;
        public Text playerIDText;
        #region PlayerMenuPrefabs
        public Dropdown dropdownPrefab;
        public InputField inputFieldPrefab;
        public Toggle togglePrefab;
        public Button buttonPrefab;
        public InputField matches;
        private List<GameObject> spawnedPrefabs = new List<GameObject>();
        #endregion
        [SerializeField] GameObject[] _tiles = new GameObject[9];
        public GameObject[] LinePositions = new GameObject[8];

        public StrategyVariable strategy;

        List<FieldInfo> fieldInfo = new List<FieldInfo>();
        bool displaySecondPlayer;

        public event Action PlayersReady;

        public GameObject gameBoard;

        private void Start()
        {
            List<string> stratNames = new List<string>();
            foreach (var strat in strategy.Value)
            {
                stratNames.Add(strat);
            }
            _player1StratDropdown.AddOptions(stratNames);
            _player2StratDropdown.AddOptions(stratNames);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        #region StrategiesImp
      
        public Player GetPlayer1()
        {
            string strat = strategy.Value[_player1StratDropdown.value];
            
            if (_player1IDInput.text == "") return new Player('X', strat);
            char playerID;
            if (int.Parse(_player2IDInput.text) >= int.Parse(_player1IDInput.text)) playerID = 'O';
            else playerID = 'O';
            return new Player(playerID, strat);
        }

        public Player GetPlayer2()
        {
            string strat = strategy.Value[_player2StratDropdown.value];
            if (_player2IDInput.text == "") return new Player('O', strat);
            char playerID;
            if (int.Parse(_player1IDInput.text) >= int.Parse(_player2IDInput.text)) playerID = 'X';
            else playerID = 'X';
            return new Player(playerID, strat);
        }
        #endregion


        public void ResetBoard(char id)
        {
            foreach (var tile in _tiles)
            {
                tile.GetComponentInChildren<Text>().text = "";
            }
            foreach (var go in LinePositions)
            {
                go.SetActive(false);
            }
            //LinePositions[_position].SetActive(false);
            
            EndMessage.text = "";
            playerIDText.text = "Player " + id.ToString() + ":";
        }

        public void ShowEndMessage(string text)
        {
            EndMessage.color = Color.red;
            EndMessage.text = text;
        }

        public void ShowVictoryLine(int position)
        {
            LinePositions[position].GetComponent<Image>().color = Color.gray;
            LinePositions[position].SetActive(true);
            //_position = position;
        }

        public void ChangeTurnID(char id)
        {
            playerIDText.text = "Player " + id.ToString() + ":";
        }

        public void SetPlayerMark( char playerID, int position )
        {
            // Update UI to reflect new information
            _tiles[position].GetComponentInChildren<Text>().text = playerID.ToString();
            //Debug.LogFormat( "Player {0} played position {1}", playerID, position );
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

        public int GetMatches()
        {
            return int.Parse(matches.text);
        }

        public int GetChosenPosition(int position)
        {
            throw new NotImplementedException();
        }

        public void PlayerSettings( Player player)
        {
            PlayersReady = new Action(()=> Debug.Log("PlayersReadyCalled"));
            const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public |
             BindingFlags.Instance | BindingFlags.Static;
            FieldInfo[] fields = player.selectPositionStrategy.GetType().GetFields(flags);
            int i = 0;
            foreach (var item in fields)
            {

                //Debug.LogFormat("Type: |{0}| Name: |{1}| Value: |{2}|", item.IsAssembly.GetType(), item.ToString()
                //    , item.GetValue(player.selectPositionStrategy));
                fieldInfo.Add(item);
                i++;
            }
            GeneratePlayerSettingsMenu(player);

        }

        private void GeneratePlayerSettingsMenu(Player p)
        {
            float y =  0;
            float x = 1;
            if (displaySecondPlayer == true) x = 3;
            foreach(var field in fieldInfo)
            {
                //Debug.LogFormat("Type: |{0}| Name: |{1}| Value: |{2}|", fieldInfo[i].IsAssembly.GetType(), fieldInfo[i].ToString()
                  //  , fieldInfo[i].GetValue(p.selectPositionStrategy));
                y -= 40f;
                if (field.FieldType == typeof(int))
                {
                    var go = InstatiatePrefab(inputFieldPrefab.gameObject, field, x, y);
                    go.GetComponent<InputField>().onEndEdit.AddListener((value) => field.SetValue(p.selectPositionStrategy, int.Parse(value)));
                    go.GetComponent<InputField>().text = field.GetValue(p.selectPositionStrategy).ToString();
                }
                if (field.FieldType == typeof(bool))
                {
                    var go = InstatiatePrefab(togglePrefab.gameObject, field, x, y);
                    go.GetComponent<Toggle>().onValueChanged.AddListener((value) => field.SetValue(p.selectPositionStrategy, value));
                    if ((bool)field.GetValue(p.selectPositionStrategy)) go.GetComponent<Toggle>().isOn = true;
                    else go.GetComponent<Toggle>().isOn = false;
                }
                #region SpawnEnum
                if (field.FieldType.BaseType == typeof(Enum))
                {
                    Array itemValues = System.Enum.GetValues(field.FieldType);
                    String[] itemNames = System.Enum.GetNames(field.FieldType);
                   
                    GameObject go = InstatiatePrefab(dropdownPrefab.gameObject, field, x, y);
                    go.GetComponent<Dropdown>().onValueChanged.AddListener((value) => {
                        field.SetValue(p.selectPositionStrategy, value);
                        Debug.Log(value);
                    });
                    List<string> list = new List<string>(itemNames);
                    go.GetComponent<Dropdown>().AddOptions(list);
                }
                #endregion
            }
            var readyButton = Instantiate(buttonPrefab, new Vector3(Screen.width * x / 4, Screen.height / 2 + y -40, 0)
                , Quaternion.identity, this.transform.parent);
            readyButton.GetComponentInChildren<Text>().text = "Ready";
            
            readyButton.onClick.AddListener(() => {
                gameBoard.SetActive(true);
                DestroyList(spawnedPrefabs);
                PlayersReady();
                
            ;});
            spawnedPrefabs.Add(readyButton.gameObject);
            displaySecondPlayer = !displaySecondPlayer;
            fieldInfo = new List<FieldInfo>();
        }

        void DestroyList(List<GameObject> list) { 
            foreach(var item in list)
            {
                Destroy(item);
            }
        }

        private GameObject InstatiatePrefab(GameObject prefab, FieldInfo field,float x, float y)
        {
            var go = Instantiate(prefab,new Vector3(x * Screen.width/4,Screen.height/2 - y,0), Quaternion.identity ,this.transform.parent);
            go.name = field.Name;
            go.GetComponentInChildren<Text>().text = field.Name;
            spawnedPrefabs.Add(go);
            return go;
        }

    }
}