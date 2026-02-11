using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LikeLion.LP0.Client.UnityWorld.View.GameScene
{
    public class StonePoint : MonoBehaviour, IPointerClickHandler
    {
        public event EventHandler ClickedEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            ClickedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
