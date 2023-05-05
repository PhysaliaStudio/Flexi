using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;

namespace Physalia.Flexi
{
    public class LinkerProcessor : IUnityLinkerProcessor
    {
        public int callbackOrder => 0;

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            const string filePath = "Packages/studio.physalia.flexi/link.xml";
            return Path.GetFullPath(filePath);
        }
    }
}
