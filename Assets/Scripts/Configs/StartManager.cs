using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Netcode.Transports.Enet;
using UnityEngine.UI;


public class StartManager : MonoBehaviour
{
    public List<Transform> PlayerIOs;
    public List<Transform> PlayerIOsOpposite;
    public Transform Player1Anchor;
    public Transform Player2Anchor;
    public Transform SelectButton;
    public GameObject XRLocal;
    public Button StartHostButton;
    public Button StartClientButton;
    private PlayerSettingManager PlayerSettingManager;
    string hostIP = "127.0.0.1";
    int hostPort = 7777;
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnServerStarted += OnServerStartedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;

        if (JsonConfig.HasKey("HostIP")) hostIP = JsonConfig.GetString("HostIP");
        else JsonConfig.SetString("HostIP", hostIP);

        if (JsonConfig.HasKey("HostPort")) hostPort = JsonConfig.GetInt("HostPort");
        else JsonConfig.SetInt("HostPort", hostPort);

        GetComponent<EnetTransport>().Address = hostIP;
        GetComponent<EnetTransport>().Port = (ushort)hostPort;

        PlayerSettingManager = XRLocal.GetComponent<PlayerSettingManager>();

    }
    private void OnClientConnectedCallback(ulong callBack)
    {
        SetMode();
    }
    private void OnServerStartedCallback()
    {
        //SetMode();
    }
    private void OnClientDisconnectedCallback(ulong callBack)
    {
        Debug.Log("Client Disconnected");
        StopClient();
    }
    private void OnTransportFailure()
    {
        Debug.Log("Transport Failure");
        StopAll();
    }
    private void SetMode()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            foreach (var IO in PlayerIOs)
                IO.position = new Vector3(Player1Anchor.position.x, IO.position.y, IO.position.z);
            foreach (var IO in PlayerIOsOpposite)
                IO.position = new Vector3(Player2Anchor.position.x, IO.position.y, IO.position.z);

            PlayerSettingManager.SetTarget(NetworkManager.Singleton.LocalClient.PlayerObject.gameObject);
            XRLocal.SetActive(false);
            StartHostButton.interactable = false;
            Debug.Log("Start Host Success");
        }
        else if (NetworkManager.Singleton.IsConnectedClient)
        {
            foreach (var IO in PlayerIOs)
                IO.position = new Vector3(Player2Anchor.position.x, IO.position.y, IO.position.z);
            foreach (var IO in PlayerIOsOpposite)
                IO.position = new Vector3(Player1Anchor.position.x, IO.position.y, IO.position.z);
            SelectButton.localScale = new Vector3(SelectButton.localScale.x * -1, SelectButton.localScale.y, SelectButton.localScale.z);

            PlayerSettingManager.SetTarget(NetworkManager.Singleton.LocalClient.PlayerObject.gameObject);
            XRLocal.SetActive(false);
            StartClientButton.interactable = false;
            Debug.Log("Start Client Success");
        }
    }
    public void StartHost()
    {
        Debug.Log("Start Host");
        NetworkManager.Singleton.Shutdown();
        //PlayerSettingManager.SetTarget(XRLocal);
        //XRLocal.SetActive(true);
        
        if (!NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Start Host Failed");
            StopAll();
        }
    }
    public void StartClient()
    {
        Debug.Log("Start Client");
        NetworkManager.Singleton.Shutdown();
        //PlayerSettingManager.SetTarget(XRLocal);
        //XRLocal.SetActive(true);

        if (!NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Start Client Failed");
            StopAll();
        }
        StartClientButton.interactable = false;
    }
    public void StopAll()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsConnectedClient)
            NetworkManager.Singleton.Shutdown();
        PlayerSettingManager.SetTarget(XRLocal);
        XRLocal.SetActive(true);
        StartHostButton.interactable = true;
        StartClientButton.interactable = true;
        Debug.Log("Stop All");
    }
    private void StopClient()
    {
        if (NetworkManager.Singleton.IsHost)
            return;
        NetworkManager.Singleton.Shutdown();
        PlayerSettingManager.SetTarget(XRLocal);
        XRLocal.SetActive(true);
        StartHostButton.interactable = true;
        StartClientButton.interactable = true;
        Debug.Log("Stop Client");
    }
}
