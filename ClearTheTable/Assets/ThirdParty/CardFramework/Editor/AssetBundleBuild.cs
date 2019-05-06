using UnityEditor;
using UnityEngine;

public class AssetBundleBuild
{

	[MenuItem("Assets/Build All Bundles Windows")]	
	static public void BuildAllBundlesWindows() 
	{
		BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
		BuildPipeline.BuildAssetBundles(DirectoryUtility.ExternalAssets(), assetBundleOptions, BuildTarget.StandaloneWindows);
	}

    [MenuItem("Assets/Build All Bundles OSX")]
    static public void BuildAllBundlesOSX()
    {
        BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
        BuildPipeline.BuildAssetBundles(DirectoryUtility.ExternalAssets(), assetBundleOptions, BuildTarget.StandaloneOSX);
    }

}
