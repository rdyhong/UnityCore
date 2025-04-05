using ExitGames.Client.Photon;
using Photon.Pun;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class PhotonPlayerMgr : SingletonPun<PhotonPlayerMgr>
{

    public void ResetCustomProperty()
    {
        PhotonHashTable customProperties = new PhotonHashTable
        {
            { "NickName", $"Player_{Random.Range(0, 10000)}" },
            { "CharacterSkin", 0 },
            { "Team", 0 },
            { "LoadingScene", false }
        };

        // 내 플레이어에 프로퍼티 설정
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public T GetCurtomProperty<T>(Player player, string key)
    {
        return (T)player.CustomProperties[key];
    }
}
