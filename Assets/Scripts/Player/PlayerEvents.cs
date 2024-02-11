using System;
using UnityEngine.Events;

namespace Framework.Player
{
    [Serializable]
    public class PlayerStateEvent : UnityEvent<PlayerState>{ }
}
