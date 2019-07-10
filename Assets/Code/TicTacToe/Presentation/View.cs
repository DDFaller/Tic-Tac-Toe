using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Core;
using System.Collections.Generic;

namespace TicTacToe.Presentation
{
    public abstract class View<T> : MonoBehaviour, IView where T : IController
    {
        
        
        public T Controller { get { return GetComponent<T>(); } }
        

        public virtual void Show()
        {
            
            
        }
        public virtual void Hide()
        {

        }
     
    }
}