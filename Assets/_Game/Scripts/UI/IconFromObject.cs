using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class IconFromObject : MonoBehaviour
{
    [SerializeField] private RenderTexture rt;
    [SerializeField] private string iconSavePath = "Assets/_Game/Art/Icons/UI/Icons";

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(KeyCode.C))
        {
            CaptureObject();
        }
    }

    private void CaptureObject()
    {
        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);

        var oldGL = GL.sRGBWrite;
        var oldRt = RenderTexture.active;
        RenderTexture.active = rt;

        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();

        RenderTexture.active = oldRt;
        Directory.CreateDirectory(iconSavePath);

        int counter = 0;
        string fileName = $"Capture_{counter}.png";
        string savePath = Path.Combine(iconSavePath, fileName);
        while (File.Exists(savePath))
        {
            counter++;
            fileName = $"Capture_{counter}.png";
            savePath = Path.Combine(iconSavePath, fileName);
        }

        byte[] icon = texture.EncodeToPNG();
        File.WriteAllBytes(savePath, icon);
        Destroy(texture);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
