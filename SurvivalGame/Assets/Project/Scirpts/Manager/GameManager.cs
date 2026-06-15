using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

/// <summary>
/// 네트워크 부트스트랩(간편 실행) 스크립트입니다.
/// - 에디터에서 Host/Client/Server를 빠르게 시작/종료할 수 있도록 도와줍니다.
/// - NetworkManager와 UnityTransport를 자동으로 참조하여 연결 정보를 적용합니다.
/// 주의: 실제 게임 로직(스폰, 권한 등)은 NetworkManager와 서버 측 코드에서 처리되어야 합니다.
/// </summary>
public class GameManager : MonoSingleton<GameManager>
{
    [Header("Connection")]
    public string connectAddress = "127.0.0.1"; // 접속할 IP (테스트용으로 로컬 사용)
    public ushort port = 7777;                  // 포트 번호

    NetworkManager netManager;                 // Netcode의 핵심 매니저(싱글턴으로도 접근 가능)
    UnityTransport transport;                  // 전송 계층(UTP)을 직접 설정하기 위해 참조

    protected override void Awake()
    {
        base.Awake();
        // NetworkManager가 동일 오브젝트에 있으면 우선 사용, 없으면 싱글턴에서 가져옵니다.
        // 이유: 테스트 편의성을 위해 이 스크립트가 NetworkManager와 함께 붙어있을 것을 가정합니다.
        netManager = GetComponent<NetworkManager>() ?? NetworkManager.Singleton;
        if (netManager == null)
        {
            Debug.LogWarning("[GameManager] NetworkManager를 찾을 수 없습니다. 이 스크립트는 NetworkManager 오브젝트에 붙이시길 권합니다.");
        }

        // Transport는 NetworkManager에 붙어있는 UnityTransport를 사용합니다.
        // 이유: 주소/포트 설정이나 커스텀 전송 설정을 코드에서 적용하기 위함입니다.
        transport = (netManager != null) ? netManager.GetComponent<UnityTransport>() : FindObjectOfType<UnityTransport>();
        if (transport == null)
        {
            Debug.LogWarning("[GameManager] UnityTransport를 찾을 수 없습니다. NetworkManager에 Unity Transport 컴포넌트를 추가해 주세요.");
        }
    }

    void Update()
    {
        // 단축키로 빠르게 테스트 가능하도록 함
        // H: Host 시작(서버+클라이언트), C: Client 시작, S: Server 시작(서버 전용)
        // Host는 로컬에서 서버 권한을 갖고 게임 상태를 관리하기 때문에 테스트 시 편리합니다.
        if (Input.GetKeyDown(KeyCode.H)) StartHost();
        if (Input.GetKeyDown(KeyCode.C)) StartClient();
        if (Input.GetKeyDown(KeyCode.S)) StartServer();
    }

    void OnGUI()
    {
        // 에디터 테스트용 간단 UI
        // 실제 게임에서는 별도의 UI를 사용하시고, 이 영역은 개발/디버그용으로만 사용하십시오.
        GUILayout.BeginArea(new Rect(10, 10, 260, 160), "Network Bootstrap", GUI.skin.window);

        // 현재 네트워크 상태 표시(Stopped / Client / Server / Host)
        GUILayout.Label($"Status: {(netManager != null && netManager.IsListening ? (netManager.IsServer ? (netManager.IsHost ? "Host" : "Server") : "Client") : "Stopped")}");
        GUILayout.Space(6);

        // 주소 입력
        GUILayout.BeginHorizontal();
        GUILayout.Label("Address:", GUILayout.Width(60));
        connectAddress = GUILayout.TextField(connectAddress, GUILayout.Width(140));
        GUILayout.EndHorizontal();

        // 포트 입력 (문자열로 받아 ushort로 파싱)
        GUILayout.BeginHorizontal();
        GUILayout.Label("Port:", GUILayout.Width(60));
        var portStr = GUILayout.TextField(port.ToString(), GUILayout.Width(140));
        ushort parsed;
        if (ushort.TryParse(portStr, out parsed)) port = parsed;
        GUILayout.EndHorizontal();
        GUILayout.Space(6);

        // 버튼 상태에 따른 동작
        if (netManager == null)
        {
            GUILayout.Label("NetworkManager 없음");
        }
        else if (!netManager.IsListening)
        {
            // 아직 네트워크가 시작되지 않았을 때: 시작 버튼 표시
            if (GUILayout.Button("Start Host (H)")) StartHost();
            if (GUILayout.Button("Start Client (C)")) StartClient();
            if (GUILayout.Button("Start Server (S)")) StartServer();
        }
        else
        {
            // 네트워크 동작 중일 때: Stop 버튼 표시
            if (GUILayout.Button("Stop"))
            {
                // NGO는 현재 실행 역할과 관계없이 Shutdown으로 종료합니다.
                netManager.Shutdown();
            }
        }

        GUILayout.EndArea();
    }

    /// <summary>
    /// UnityTransport에 주소/포트 정보를 적용합니다.
    /// - 이유: 에디터 입력으로 바꾼 접속 정보를 transport에 반영해야 클라이언트가 올바르게 서버에 연결됩니다.
    /// </summary>
    void ApplyTransportConnectionData()
    {
        if (transport == null) return;
        // UnityTransport의 SetConnectionData는 클라이언트가 접속할 주소와 포트를 설정합니다.
        // 주의: 멀티플레이에서 포트 포워딩이나 방화벽 설정이 필요할 수 있습니다(로컬 테스트에서는 보통 문제 없음).
        transport.SetConnectionData(connectAddress, port);
    }

    /// <summary>
    /// Host 시작: 서버+클라이언트 동시 실행(테스트용으로 가장 편리).
    /// - Host는 일반적으로 권한 있는 서버 역할을 하므로 게임 상태(스폰, 데미지 등)는 호스트가 결정합니다.
    /// </summary>
    public void StartHost()
    {
        if (netManager == null) return;
        ApplyTransportConnectionData();
        netManager.StartHost();
        Debug.Log("[GameManager] StartHost 호출");
    }

    /// <summary>
    /// Client 시작: 외부(또는 로컬) Host/Server에 접속합니다.
    /// - 클라이언트는 입력을 보내고 서버에서 승인/동기화된 상태를 받아 렌더링 합니다.
    /// </summary>
    public void StartClient()
    {
        if (netManager == null) return;
        ApplyTransportConnectionData();
        netManager.StartClient();
        Debug.Log("[GameManager] StartClient 호출");
    }

    /// <summary>
    /// Server 시작: 서버 전용 모드(클라이언트 UI 없이 서버만 실행).
    /// - 전용 서버를 테스트하려면 사용합니다. 일반적으로 게임 로직은 서버에서 실행됩니다.
    /// </summary>
    public void StartServer()
    {
        if (netManager == null) return;
        ApplyTransportConnectionData();
        netManager.StartServer();
        Debug.Log("[GameManager] StartServer 호출");
    }
}
