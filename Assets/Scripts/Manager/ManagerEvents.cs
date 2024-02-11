using System;
using UnityEngine.Events;

namespace Framework.Manager
{
    [Serializable]
    public class GameStateEvent : UnityEvent<GameState>{ }

    [Serializable]
    public class PlayerCountEvent : UnityEvent<int>{ }
}
