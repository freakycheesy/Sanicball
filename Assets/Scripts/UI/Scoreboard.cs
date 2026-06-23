using Sanicball.Logic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Sanicball.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField]
        private ScoreboardEntry entryPrefab = null;
        [SerializeField]
        private RectTransform entryContainer = null;
        [SerializeField]
        private SlideCanvasGroup slide = null;

        private bool slideShouldOpen = false;
        public GameObject selectedButton;
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(selectedButton);
        }

        private List<ScoreboardEntry> activeEntries = new List<ScoreboardEntry>();

        private void Update()
        {
            if (slideShouldOpen && !slide.isOpen)
            {
                slide.Open();
            }
        }

        public void DisplayResults(RaceManager manager)
        {
            slideShouldOpen = true;

            for (int i = 0; i < manager.PlayerCount; i++)
            {
                if (activeEntries.Any(a => a.Player == manager[i])) continue;

                if (manager[i].RaceFinished && !manager[i].FinishReport.Disqualified)
                {
                    ScoreboardEntry e = Instantiate(entryPrefab);
                    e.transform.SetParent(entryContainer, false);
                    e.Init(manager[i]);
                    activeEntries.Add(e);
                }
            }
        }

        public void BackToLobby()
        {
            var matchManager = FindObjectOfType<MatchManager>();
            if (matchManager)
            {
                matchManager.RequestLoadLobby();
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
        }
    }
}