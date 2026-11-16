using UnityEngine;

// Sets the frame rate target once when the app starts, so it is a stated choice
// and not the platform default. 30 fps is steady on the iPhone 13 for this light scene.
// It needs no GameObject in the scene: Unity calls it by itself at startup.
public static class FrameRateTarget
{
    private const int TargetFramesPerSecond = 30;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Apply()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFramesPerSecond;
    }
}
