using System;

public enum EScene
{
    None = -1,
    AppStartScene = 0,
    Title,
    LobbyScene,
    InGameScene
}

public abstract class GameScene
{
    public abstract void Init(Action action = null);
    public abstract void DeInit(Action action = null);
    public abstract void DataInitialize(Action action = null);
}
