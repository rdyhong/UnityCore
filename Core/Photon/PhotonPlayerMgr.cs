using ExitGames.Client.Photon;
using Photon.Pun;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Linq;

public enum EPlayerCustomPropertyKey
{
    NickName,
    CharacterSkin,
    Team,
    CurrentScene,
}

public class PhotonPlayerMgr : SingletonPun<PhotonPlayerMgr>
{

    public void ResetCustomProperty()
    {
        PhotonHashTable customProperties = new PhotonHashTable
        {
            { EPlayerCustomPropertyKey.NickName.ToString(), $"Player_{Random.Range(0, 10000)}" },
            { EPlayerCustomPropertyKey.CharacterSkin.ToString(), 0 },
            { EPlayerCustomPropertyKey.Team.ToString(), 0 },
            { EPlayerCustomPropertyKey.CurrentScene.ToString(), EScene.LobbyScene.ToString() }
        };

        // 내 플레이어에 프로퍼티 설정
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public T GetCustomProperty<T>(Player player, EPlayerCustomPropertyKey key)
    {
        return (T)player.CustomProperties[key.ToString()];
    }

    public void SetCurtomProperty<T>(EPlayerCustomPropertyKey key, T value)
    {
        PhotonHashTable customProperties = new PhotonHashTable
        {
            { $"{key.ToString()}", (T)value }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public bool IsAllPlayerInTargetScene(EScene scene)
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList();

        for (int i = 0; i < playerList.Count; i++)
        {
            if (scene.ToString() != GetCustomProperty<string>(playerList[i], EPlayerCustomPropertyKey.CurrentScene))
            {
                return false;
            }
        }

        return true;
    }
}
