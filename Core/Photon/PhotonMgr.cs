using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using WebSocketSharp;

public enum ELobbyType
{
    QuickMatch = 0,
    CustomMatch,
    RankMatch
}

public class RegionInfo
{
    public string Code;
    public string Name;
    public int Ping;

    public RegionInfo(string code, string name)
    {
        Code = code;
        Name = name;
        Ping = 999;
    }
}

public class PhotonMgr : SingletonPun<PhotonMgr>
{
    // 사용 가능한 리전 목록
    public List<RegionInfo> AvailableRegions { get; private set; } = new List<RegionInfo>
    {
       new RegionInfo("asia", "아시아 (Singapore)"),
    new RegionInfo("au", "호주 (Sydney)"),
    new RegionInfo("cae", "캐나다 동부 (Montreal)"),
    //new RegionInfo("cn", "중국 본토 (Shanghai)"),
    new RegionInfo("eu", "유럽 (Amsterdam)"),
    new RegionInfo("hk", "홍콩 (Hong Kong)"),
    new RegionInfo("in", "인도 (Chennai)"),
    new RegionInfo("jp", "일본 (Tokyo)"),
    new RegionInfo("za", "남아프리카공화국 (Johannesburg)"),
    new RegionInfo("sa", "남아메리카 (Sao Paulo)"),
    new RegionInfo("kr", "한국 (Seoul)"),
    new RegionInfo("tr", "터키 (Istanbul)"),
    new RegionInfo("uae", "아랍에미리트 (Dubai)"),
    new RegionInfo("us", "미국 동부 (Washington D.C.)"),
    new RegionInfo("usw", "미국 서부 (San José)"),
    new RegionInfo("ussc", "미국 남중부 (Dallas)")
    };

    public static string SelectedRegion = string.Empty; // 기본값

    private IEnumerator _joinLobbyCo = null;

    public ELobbyType LastLobbyType { get; private set; }

    public IEnumerator InitializeCo()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = $"{Application.version}";
        Debug.Log($"{Application.version}");

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();

        // 클라이언트 세팅 완료
        Debug.Log("ConnectUsingSettings");
        yield return PhotonNetwork.IsConnectedAndReady;

        // 마스터 서버 접속 대기
        Debug.Log("Try Connect To Master Server...");
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);

        PhotonRoomMgr.Inst.Initialize();

        PhotonPlayerMgr.Inst.SetNickName($"{LogInPlayFabOnSteamMgr.Inst.DisplayName}");

        // 로비 접속 대기
        //PhotonNetwork.JoinLobby();
        //yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);
        //Debug.Log("JoinedLobby");
        
        Debug.Log("포톤 초기화 성공");
    }

    public IEnumerator PingAllRegions()
    {
        Debug.Log("모든 리전 Ping 테스트 시작...");

        yield return CheckRegionsPing();

        foreach (RegionInfo regionInfo in AvailableRegions)
        {
            // AppSettings 설정
            var appSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
            appSettings.AppVersion = $"{Application.version}";
            appSettings.FixedRegion = regionInfo.Code;
            appSettings.UseNameServer = true;

            PhotonNetwork.ConnectUsingSettings();

            yield return PhotonNetwork.IsConnectedAndReady;
            yield return new WaitForSeconds(1);

            regionInfo.Ping = PhotonNetwork.GetPing();
            Debug.Log("Ping 테스트 완료, 연결 해제 중...");
            PhotonNetwork.Disconnect();

            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);

            Debug.Log($"{regionInfo.Name}-{regionInfo.Ping}-테스트 완료");

            yield return null;
        }

        Debug.Log("Ping 테스트 완료");
    }

    public IEnumerator JoinLobbyCo(ELobbyType lobbyType)
    {
        // 현재 로비 타입 확인
        if (PhotonNetwork.InLobby)
        {
            // 같은 로비면 스킵
            if (PhotonNetwork.CurrentLobby?.Name == lobbyType.ToString())
            {
                Debug.Log($"Already in {lobbyType} lobby");
                yield break;
            }

            PhotonNetwork.LeaveLobby();
            yield return new WaitUntil(() => !PhotonNetwork.InLobby);
            Debug.Log($"Leave Lobby");
            yield return null;
        }

        TypedLobby lobbyTyped = new TypedLobby(lobbyType.ToString(), LobbyType.Default);
        PhotonNetwork.JoinLobby(lobbyTyped);
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);
        LastLobbyType = lobbyType;
        Debug.Log($"Joined Lobby({lobbyType})");
    }

    public static int GetPing()
    {
        return PhotonNetwork.GetPing();
    }
    
    public static Player[] GetAllPlayer()
    {
        return PhotonNetwork.PlayerList;
    }

    public static T GetObjectFromViewId<T>(int viewId) where T : class
    {
        PhotonView foundPv = PhotonView.Find(viewId);
        if (foundPv == null) return null;

        return PhotonView.Find(viewId).GetComponent<T>();
    }




    private bool _regionListReceived = false;
    private RegionHandler _regionHandler;

    public IEnumerator CheckRegionsPing()
    {
        Debug.Log("<color=yellow>[Photon]</color> Region 핑 테스트 시작...");

        // 혹시 남은 연결이 있다면 정리
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);
        }

        // AppSettings 준비
        var appSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
        appSettings.UseNameServer = true;
        appSettings.AppVersion = Application.version;

        // PhotonNetwork.NetworkingClient 직접 사용
        var client = PhotonNetwork.NetworkingClient;
        client.AppId = appSettings.AppIdRealtime;
        client.AppVersion = appSettings.AppVersion;

        Debug.Log("<color=yellow>[Photon]</color> NameServer 연결 시도...");
        client.ConnectToNameServer();

        // 연결 대기
        yield return new WaitUntil(() => client.State == ClientState.ConnectedToNameServer);
        Debug.Log("<color=green>[Photon]</color> NameServer 연결 완료!");

        // Region 목록 수신 대기
        yield return new WaitUntil(() => _regionListReceived && _regionHandler != null);
        Debug.Log($"<color=yellow>[Photon]</color> 리전 목록 수신 완료 ({_regionHandler.EnabledRegions.Count}개)");

        // 핑 테스트 시작
        bool isDone = false;
        _regionHandler.PingMinimumOfRegions((RegionHandler handler) =>
        {
            Debug.Log("<color=green>[Photon]</color> 리전 핑 측정 완료");

            foreach (var region in handler.EnabledRegions)
            {
                Debug.Log($"[Region] {region.Code} - {region.Ping}ms");

                var found = AvailableRegions.Find(r => r.Code.Equals(region.Code, System.StringComparison.OrdinalIgnoreCase));
                if (found != null)
                    found.Ping = region.Ping;
            }

            // 가장 좋은 리전 저장
            SelectedRegion = handler.BestRegion?.Code ?? string.Empty;
            Debug.Log($"<color=cyan>[Best Region]</color> {SelectedRegion} ({handler.BestRegion?.Ping}ms)");

            isDone = true;
        }, null);

        yield return new WaitUntil(() => isDone);
        Debug.Log("<color=green>[Photon]</color> 모든 리전 핑 테스트 완료!");
    }

    // RegionHandler가 도착하면 Photon이 자동으로 이 콜백을 호출함
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        _regionHandler = regionHandler;
        _regionListReceived = true;
        Debug.Log($"<color=lime>[Photon]</color> OnRegionListReceived() 호출됨: {regionHandler.EnabledRegions.Count}개 리전 수신");
    }


}
