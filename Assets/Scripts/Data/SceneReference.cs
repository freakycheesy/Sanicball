using System;
using UnityEngine.AddressableAssets;

namespace Sanicball.Data
{
    [Serializable]
    public class SceneReference : AssetReference
    {
        public override bool ValidateAsset(string path)
        {
            return path.EndsWith(".unity");
        }
    }
}