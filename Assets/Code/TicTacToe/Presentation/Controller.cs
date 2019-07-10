using UnityEngine;

using TicTacToe.Core;
using System.Collections.Generic;

namespace TicTacToe.Presentation
{
    public abstract class Controller<T> : MonoBehaviour, IController where T : IView
    {
        public T View { get { return GetComponent<T>(); } }

        public virtual void Awake()
        {
            Initialize();
        }

        public virtual void Show()
        {
            View.Show();
        }

        public virtual void Hide()
        {
            View.Hide();
        }

        public abstract List<Player> GetPlayers();

        public virtual void Initialize()
        {
        }
    }
}