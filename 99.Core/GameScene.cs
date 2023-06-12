using System;

public enum SceneKind
{
    None = -1,
    Splash = 0,
    Title,
    Lobby,
    InGame
}

public abstract class GameScene
{
    public abstract void Init(Action action = null);
    public abstract void DeInit(Action action = null);

    public abstract void DataInitialize(Action action = null);
}
