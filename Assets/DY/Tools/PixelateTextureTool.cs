#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

public class PixelateTextureTool : EditorWindow
{
    Texture2D sourceTexture;
    int blockSize = 4;
    Texture2D previewTexture;
    string customFileName = "";

    [MenuItem("Tools/Pixelate Texture (Safe)")]
    public static void ShowWindow()
    {
        GetWindow<PixelateTextureTool>("Pixelate Texture");
    }

    void OnGUI()
    {
        var newTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);
        if (newTexture != sourceTexture)
        {
            sourceTexture = newTexture;
            if (sourceTexture != null)
                customFileName = sourceTexture.name + "_pixelated";
        }

        blockSize = EditorGUILayout.IntSlider("Block Size", blockSize, 1, 64);
        customFileName = EditorGUILayout.TextField("Output File Name", customFileName);

        if (sourceTexture != null)
        {
            string path = AssetDatabase.GetAssetPath(sourceTexture);
            EnsureTextureIsReadable(path);

            if (GUILayout.Button("Generate Preview"))
            {
                previewTexture = GeneratePixelatedCopy(sourceTexture, blockSize);
                PixelPreviewWindow.ShowPreview(Texture.Instantiate(previewTexture));
            }

            GUILayout.Space(10);
            GUI.enabled = previewTexture != null;
            if (GUILayout.Button("Save As New Asset"))
            {
                string savePath = Path.GetDirectoryName(path);
                string fileName = string.IsNullOrWhiteSpace(customFileName) ? sourceTexture.name + "_pixelated.png" : customFileName + ".png";
                string fullPath = Path.Combine(savePath, fileName);
                SaveAsNewAsset(previewTexture, fullPath);
            }
            GUI.enabled = true;
        }
    }

    void EnsureTextureIsReadable(string assetPath)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }
    }

    Texture2D GeneratePixelatedCopy(Texture2D original, int blockSize)
    {
        Texture2D copy = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        copy.filterMode = FilterMode.Point;
        copy.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < original.height; y += blockSize)
        {
            for (int x = 0; x < original.width; x += blockSize)
            {
                Color avg = AverageBlock(original, x, y, blockSize);
                if (avg.a < 0.9f)
                    avg = new Color(0, 0, 0, 0);
                FillBlock(copy, x, y, blockSize, avg);
            }
        }

        copy.Apply();
        return copy;
    }

    Color AverageBlock(Texture2D tex, int startX, int startY, int size)
    {
        Color sum = Color.black;
        int count = 0;

        for (int y = startY; y < startY + size && y < tex.height; y++)
        {
            for (int x = startX; x < startX + size && x < tex.width; x++)
            {
                sum += tex.GetPixel(x, y);
                count++;
            }
        }

        return count > 0 ? sum / count : Color.clear;
    }

    void FillBlock(Texture2D tex, int startX, int startY, int size, Color color)
    {
        for (int y = startY; y < startY + size && y < tex.height; y++)
        {
            for (int x = startX; x < startX + size && x < tex.width; x++)
            {
                tex.SetPixel(x, y, color);
            }
        }
    }

    void SaveAsNewAsset(Texture2D texture, string fullPath)
    {
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(fullPath, pngData);

        string assetPath = fullPath.Substring(fullPath.IndexOf("Assets"));
        AssetDatabase.ImportAsset(assetPath);

        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
        importer.textureType = TextureImporterType.Default;
        importer.isReadable = true;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();

        Debug.Log("Saved pixelated texture to: " + assetPath);
    }
}

public class PixelPreviewWindow : EditorWindow
{
    Texture2D previewTexture;
    Material previewMaterial;

    public static void ShowPreview(Texture2D texture)
    {
        var window = GetWindow<PixelPreviewWindow>("Preview");
        window.previewTexture = texture;
        window.minSize = new Vector2(256, 256);
        window.Show();
    }

    void OnEnable()
    {
        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader != null)
            previewMaterial = new Material(shader);
    }

    void OnGUI()
    {
        if (previewTexture != null)
        {
            float ratio = (float)previewTexture.width / previewTexture.height;
            float maxWidth = position.width - 10;
            float maxHeight = position.height - 10;
            float width = maxWidth;
            float height = width / ratio;

            if (height > maxHeight)
            {
                height = maxHeight;
                width = height * ratio;
            }

            Rect rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            EditorGUI.DrawTextureTransparent(rect, previewTexture, ScaleMode.ScaleToFit);
        }
        else
        {
            EditorGUILayout.LabelField("No preview available.");
        }
    }

}
#endif