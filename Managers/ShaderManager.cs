using Il2CppInterop.Runtime;
using MelonLoader.Utils;
using UnityEngine;
using Logger = FadeIn.Utilities.Logger;

namespace FadeIn.Managers;

internal static class ShaderManager
{
    private const string BundleName = "TestBundleName";
    private const string ShaderName = "Shader/SomethingShader";

    internal static Material? PressMaterial { get; private set; }
    internal static Shader? PressShader { get; private set; }

    private static readonly string BundlePath = Path.Join(
        MelonEnvironment.UserDataDirectory,
        BundleName
    );

    private static readonly Logger logger = new(nameof(ShaderManager));

    internal static void Init()
    {
        logger.Debug($"Asset bundle path: {BundlePath}!");

        var assetBundle = AssetBundle.LoadFromFile(BundlePath);
        if (assetBundle is null)
        {
            var error = $"Failed to load {BundleName}!";
            logger.Error(error);
            // TODO: Maybe use another exception...
            throw new FileLoadException(error);
        }

        if (SettingsManager.Debug)
        {
            var assets = assetBundle.GetAllAssetNames();
            logger.Debug($"Found {assets.Length} assets:");
            foreach (var asset in assetBundle.AllAssetNames())
            {
                logger.Debug(asset);
            }
        }

        PressShader = assetBundle.LoadAsset(ShaderName, Il2CppType.Of<Shader>()).Cast<Shader>();
        if (PressShader is null)
        {
            var error = $"{ShaderName} is null!";
            logger.Error(error);
            throw new NullReferenceException(error);
        }

        // var material = assetBundle
        //     .LoadAsset("assets/red.mat", Il2CppType.Of<Material>())
        //     .Cast<Material>();

        // Melon<Main>.Logger.Msg(material);
        // PressMaterial = material;
        // if(PressMaterial is null){
        //     logger.Error("");
        // }
    }
}
