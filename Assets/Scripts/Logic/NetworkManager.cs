using Newtonsoft.Json;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using SanicballCore.Server;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public Server server;
    private CommandQueue commandQueue = new CommandQueue();
    public static bool isServer { get; private set; } = false;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void OnDestroy()
    {
        if (instance != this) return;
        if(server != null) server.Dispose();
    }
    public bool StartServer(string serverName, int maxPlayers, int port, bool useNat, bool showOnList, out string error)
    {
        try
        {
            CreateServerConfig(serverName, maxPlayers, port, showOnList);
            if (!File.Exists(MOTDPath))
            {
                File.WriteAllText(MOTDPath, $"Welcome to {ActiveData.GameSettings.nickname}'s server!");
            }
            if (!File.Exists(MatchSettingsPath))
            {
                File.WriteAllText(MatchSettingsPath, JsonConvert.SerializeObject(MatchSettings.CreateDefault()));
            }
            StartServer(port);
        }
        catch (System.Exception e)
        {
            error = e.ToString();
            return false;
        }
        error = string.Empty;
        return true;
    }

    private static void CreateServerConfig(string serverName, int maxPlayers, int port, bool showOnList)
    {
        var config = new SanicballCore.Server.ServerConfig();
        config.ServerName = serverName;
        config.ShowOnList = showOnList;
        config.ServerListURLs = new[] { ActiveData.GameSettings.serverListURL };
        config.stageCount = ActiveData.Stages.Length;
        config.characterTiers = ActiveData.Characters.Select(x => x.tier).ToArray();
        config.PrivatePort = port;
        config.PublicIP = new WebClient().DownloadString("http://ipinfo.io/ip").Trim();
        config.PublicPort = port;
        config.MaxPlayers = maxPlayers;
        File.WriteAllText(ServerConfigPath, JsonConvert.SerializeObject(config));
    }

    public static string ServerConfigPath => Path.Combine(Application.persistentDataPath, "ServerConfig.json");
    public static string MatchSettingsPath => Path.Combine(Application.persistentDataPath, "MatchSettings.json");
    public static string MOTDPath => Path.Combine(Application.persistentDataPath, "MOTD.txt");

    public void StartServer(int port)
    {
        Debug.Log("Server Starting");
        StartServerInternal();
        server.matchSettings = ActiveData.MatchSettings;
        FindObjectOfType<MatchStarter>().JoinOnlineGame("127.0.0.1", port);
    }

    private void StartServerInternal()
    {
        server = new(commandQueue, true, ServerConfigPath, MatchSettingsPath, MOTDPath);
        server.Start();
        isServer = true;
    }

    private void Update()
    {
        if (server != null && server.running)
        {
            server.Tick();
        }
    }

    public void Close()
    {
        Debug.Log("Server Closing");
        if(server != null) server.Dispose();
        isServer = false;
    }
}
