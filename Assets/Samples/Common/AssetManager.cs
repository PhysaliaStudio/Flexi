using UnityEngine;

namespace Physalia.Flexi.Samples
{
    public class AssetManager
    {
        private readonly string rootFolderPath;

        public AssetManager(string rootFolderPath)
        {
            if (rootFolderPath != null)
            {
                this.rootFolderPath = rootFolderPath;
            }
            else
            {
                this.rootFolderPath = "";
            }
        }

        public T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>($"{rootFolderPath}/{path}");
        }

        public T[] LoadAll<T>(string folderPath) where T : Object
        {
            return Resources.LoadAll<T>($"{rootFolderPath}/{folderPath}");
        }
    }
}
