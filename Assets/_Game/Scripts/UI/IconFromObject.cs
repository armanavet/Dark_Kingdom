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
        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, true);

        var oldGL = GL.sRGBWrite;
        var oldRt = RenderTexture.active;
        GL.sRGBWrite = false;
        RenderTexture.active = rt;

        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();

        GL.sRGBWrite = oldGL;
        RenderTexture.active = oldRt;
        Directory.CreateDirectory(iconSavePath);

        string fileName = $"Capture_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.png";
        string savePath = Path.Combine(iconSavePath, fileName);
        while (File.Exists(savePath))
        {
            fileName = $"Capture_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}" + "(1).png";
            savePath = Path.Combine(iconSavePath, fileName);
        }

        byte[] icon = texture.EncodeToPNG();
        File.WriteAllBytes(savePath, icon);
        Destroy(texture);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
