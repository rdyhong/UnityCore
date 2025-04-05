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
    LoadingScene,
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
            { EPlayerCustomPropertyKey.LoadingScene.ToString(), false }
        };

        // 내 플레이어에 프로퍼티 설정
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public T GetCurtomProperty<T>(Player player, EPlayerCustomPropertyKey key)
    {
        return (T)player.CustomProperties[key.ToString()];
    }

    public void SetCurtomProperty<T>(EPlayerCustomPropertyKey key, object value)
    {
        PhotonHashTable customProperties = new PhotonHashTable
        {
            { $"{key.ToString()}", (T)value }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public bool IsAllPlayerLoadScene()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList();

        for (int i = 0; i < playerList.Count; i++)
        {
            if (!GetCurtomProperty<bool>(playerList[i], EPlayerCustomPropertyKey.LoadingScene))
            {
                return false;
            }
        }

        return true;
    }
}
