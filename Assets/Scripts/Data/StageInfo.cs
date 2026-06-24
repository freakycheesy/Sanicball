using UnityEngine;

namespace Sanicball.Data
{
    [System.Serializable]
    public class StageInfo
    {
        public string name;
        public int id;
        public SceneReference scene;
        public Sprite picture;
        public GameObject overviewPrefab;
    }
}
