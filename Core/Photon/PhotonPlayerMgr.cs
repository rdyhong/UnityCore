using ExitGames.Client.Photon;
using Photon.Pun;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Linq;

public enum EPlayerCustomPropertyKey
{
    CharacterSkin,  // int
    Team,           // int (0, 1)
    CurrentScene,   // string
    Hero,
    Deck,           // int[]
}

public class PhotonPlayerMgr : SingletonPun<PhotonPlayerMgr>
{
    public void ResetCustomProperty()
    {
        PhotonHashTable customProperties = new PhotonHashTable
        {
            { EPlayerCustomPropertyKey.CharacterSkin.ToString(), 0 },
            { EPlayerCustomPropertyKey.Team.ToString(), -1 },
            { EPlayerCustomPropertyKey.CurrentScene.ToString(), EScene.LobbyScene.ToString() },
            { EPlayerCustomPropertyKey.Hero.ToString(), UserDataMgr.Inst.PlayerSaveData.equipedHeroId },
            { EPlayerCustomPropertyKey.Deck.ToString(), UserDataMgr.Inst.PlayerSaveData.equipedDeckIds }
        };

        // 내 플레이어에 프로퍼티 설정
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public T GetCustomProperty<T>(Player player, EPlayerCustomPropertyKey key)
    {
        return (T)player.CustomProperties[key.ToString()];
    }

    public void SetCustomProperty<T>(EPlayerCustomPropertyKey key, T value)
    {
        PhotonHashTable customProperties = new PhotonHashTable
        {
            { $"{key.ToString()}", (T)value }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }
    /// <summary>
    /// Master Client Only
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetOtherUserCustomProperty<T>(Player player, EPlayerCustomPropertyKey key, T value)
    {
        PhotonHashTable customProperties = new PhotonHashTable
        {
            { $"{key.ToString()}", (T)value }
        };

        player.SetCustomProperties(customProperties);
    }

    /// <summary>
    /// 나의 팀 인덱스 리턴
    /// </summary>
    /// <returns></returns>
    public int GetMyTeamIdx()
    {
        return PhotonPlayerMgr.Inst.GetCustomProperty<int>(PhotonNetwork.LocalPlayer, EPlayerCustomPropertyKey.Team);
    }

    /// <summary>
    /// 전체 플레이어의 씬 로드 확인
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
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

    public void SetNickName(string n)
    {
        PhotonNetwork.LocalPlayer.NickName = n;
    }

    public Player[] GetPlayerSortedByActorNumber()
    {
        Player[] sortedByActorNumber = PhotonNetwork.PlayerList
            .OrderBy(player => player.ActorNumber)
            .ToArray();

        return sortedByActorNumber;
    }

    /// <summary>
    /// 룸 입장시점 내 팀 자동 세팅
    /// </summary>
    private void SetMyTeamOnJoinRoom()
    {
        // Team
        int team0Count = 0;
        int team1Count = 0;

        foreach(Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // 자신 제외
            if (player == PhotonNetwork.LocalPlayer) continue;

            if (GetCustomProperty<int>(player, EPlayerCustomPropertyKey.Team) == 0)
            {
                team0Count++;
            }
            else if (GetCustomProperty<int>(player, EPlayerCustomPropertyKey.Team) == 1)
            {
                team1Count++;
            }
        }

        int selectedTeam = 0;

        if(team0Count <= team1Count)
        {
            selectedTeam = 0;
        }
        else
        {
            selectedTeam = 1;
        }

        SetCustomProperty<int>(EPlayerCustomPropertyKey.Team, selectedTeam);

        Debug.Log($"Set Local Player Team ({selectedTeam})");
    }

    // --------------------------------------------------
    // Photon Callback

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        //SetMyTeamOnJoinRoom();

    }
}
