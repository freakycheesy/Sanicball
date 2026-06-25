using Sanicball.Logic;
using SanicballCore;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class PauseMenu : MonoBehaviour
    {
        private const string pauseTag = "Pause";

        [SerializeField]
        private GameObject firstSelected = null;
        [SerializeField]
        private GameObject firstServerSelected = null;


        [Header("Pause Menu Panel")]
        [SerializeField]
        private Button contextSensitiveButton;
        [SerializeField]
        private Text contextSensitiveButtonLabel;
        [SerializeField]
        private Button restartButton;
        

        [Header("Server Panel")]
        [SerializeField]
        private CanvasRenderer serverPanel;
        [SerializeField]
        private CanvasRenderer clientPanel;
        [SerializeField]
        private Button serverButton;
        [SerializeField]
        private ServerPanelClientItem clientItemPrefab;
        [SerializeField]
        private Text stageRotationText;
        [SerializeField]
        private Text allowedTiersText; 
        [SerializeField]
        private Text tierRotationText;
        [SerializeField]
        private Text voteRatioText; 
        [SerializeField]
        private Text disqualificationTimeText;

        private bool mouseWasLocked;

        public static bool GamePaused { get { return GameObject.FindWithTag(pauseTag); } }

        public bool OnlineMode { get; set; }

        private List<ServerPanelClientItem> clients = new ();


        private void Awake()
        {
            CloseServerPanel();
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                mouseWasLocked = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Start()
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstSelected);
            if (!OnlineMode)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;               
            }
            if (!NetworkManager.isServer || !OnlineMode)
            {
                Destroy(serverPanel.gameObject);
                Destroy(serverButton.gameObject);
            }
            if (MatchManager.instance.InLobby)
            {
                contextSensitiveButtonLabel.text = "Change match settings";
                contextSensitiveButton.onClick.AddListener(MatchSettings);
                if (OnlineMode && !NetworkManager.isServer)
                {
                    contextSensitiveButton.interactable = false;
                }
                Destroy(restartButton.gameObject);
            }
            else
            {
                contextSensitiveButtonLabel.text = "Return to lobby";
                contextSensitiveButton.onClick.AddListener(BackToLobby);
                if (OnlineMode)
                {
                    Destroy(restartButton.gameObject);
                }
            }
        }

        public void Close()
        {
            if (mouseWasLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            CloseServerPanel();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (!OnlineMode)
            {
                Time.timeScale = 1;
                AudioListener.pause = false;
            }
        }

        public void MatchSettings()
        {
            LobbyReferences.Active.MatchSettingsPanel.Show();
            Close();
        }

        public void RestartRace()
        {
            var matchManager = FindObjectOfType<MatchManager>();
            if (matchManager && !matchManager.OnlineMode)
            {
                matchManager.GoToStage();
            }
        }

        public void OpenServerPanel()
        {
            serverPanel.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstServerSelected);
            var settings = MatchManager.instance.CurrentSettings;
            stageRotationText.text = settings.StageRotationMode.ToString();
            allowedTiersText.text = settings.AllowedTiers.ToString();
            tierRotationText.text = settings.TierRotationMode.ToString();
            voteRatioText.text = settings.VoteRatio.ToString();
            disqualificationTimeText.text = settings.DisqualificationTime.ToString();
            foreach (var client in NetworkManager.server.clients.ToArray())
            {
                var instance = Instantiate(clientItemPrefab, clientPanel.transform);
                instance.Client = client;
                clients.Add(instance);
            }
        }

        public void CloseServerPanel()
        {
            serverPanel.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(firstSelected);
            foreach(var client in clients.ToArray())
            {
                clients.Remove(client);
                Destroy(client.gameObject);
            }
        }

        public void IncrementStageRotation()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if ((int)tempSettings.StageRotationMode < System.Enum.GetNames(typeof(StageRotationMode)).Length - 1)
            {
                tempSettings.StageRotationMode++;
            }
            stageRotationText.text = tempSettings.StageRotationMode.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);         
        }
        public void DecrementStageRotation()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if (tempSettings.StageRotationMode > 0)
            {
                tempSettings.StageRotationMode--;
            }
            stageRotationText.text = tempSettings.StageRotationMode.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }

        public void IncrementAllowedTiers()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if ((int)tempSettings.AllowedTiers < System.Enum.GetNames(typeof(AllowedTiers)).Length - 1)
            {
                tempSettings.AllowedTiers++;
            }
            allowedTiersText.text = tempSettings.AllowedTiers.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }
        public void DecrementAllowedTiers()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if (tempSettings.AllowedTiers > 0)
            {
                tempSettings.AllowedTiers--;
            }
            allowedTiersText.text = tempSettings.AllowedTiers.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }

        public void IncrementTierRotation()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if ((int)tempSettings.TierRotationMode < System.Enum.GetNames(typeof(TierRotationMode)).Length - 1)
            {
                tempSettings.TierRotationMode++;
            }
            tierRotationText.text = tempSettings.TierRotationMode.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }
        public void DecrementTierRotationMode()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if (tempSettings.TierRotationMode > 0)
            {
                tempSettings.TierRotationMode--;
            }
            tierRotationText.text = tempSettings.TierRotationMode.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }

        public void IncrementVoteRatio()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if ((int)tempSettings.VoteRatio < 1)
            {
                tempSettings.VoteRatio+=0.1f;
            }
            voteRatioText.text = tempSettings.VoteRatio.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }
        public void DecrementVoteRatio()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if (tempSettings.VoteRatio > 0)
            {
                tempSettings.VoteRatio-=0.1f;
            }
            voteRatioText.text = tempSettings.VoteRatio.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }
        public void IncrementDisqualificationTime()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if ((int)tempSettings.DisqualificationTime < 120)
            {
                tempSettings.DisqualificationTime++;
            }
            disqualificationTimeText.text = tempSettings.DisqualificationTime.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }
        public void DecrementDisqualificationTime()
        {
            var tempSettings = MatchManager.instance.CurrentSettings;
            if ((int)tempSettings.DisqualificationTime > 0)
            {
                tempSettings.DisqualificationTime--;
            }
            disqualificationTimeText.text = tempSettings.DisqualificationTime.ToString();
            MatchManager.instance.RequestSettingsChange(tempSettings);
        }


        public void BackToLobby()
        {
            var matchManager = FindObjectOfType<MatchManager>();
            if (matchManager)
            {
                matchManager.RequestLoadLobby();
                Close();
            }
            else
            {
                Debug.LogError("Cannot return to lobby: no match manager found to handle the request. Something is broken!");
            }
        }

        public void QuitMatch()
        {
            var matchManager = FindObjectOfType<MatchManager>();
            if (matchManager)
            {
                matchManager.QuitMatch();
            }
            else
            {
                //Backup solution in case the match manager bugs out for whatever reason
                //Why would it ever bug out? I have no clue
                SceneManager.LoadScene("Menu");
            }
            NetworkManager.Close();
        }
    }
}