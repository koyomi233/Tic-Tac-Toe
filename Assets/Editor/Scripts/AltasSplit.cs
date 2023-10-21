using UnityEngine;
using UnityEditor;
using System.IO;

public static class AltasSplit
{
    private const string defaultPlatform = "DefaultTexturePlatform";
#if UNITY_IOS
    private static string platform = "iPhone";
#elif UNITY_ANDROID
    private static string platform = "Android";
#else
    private static string platform = "DefaultTexturePlatform";
#endif
    
    [MenuItem("Assets/Split Atlas", true)]
    static private bool IsProcessAltasToSprites()
    {
        return (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)));
    }

    [MenuItem("Assets/Split Atlas", false, 11)]
    static void ProcessAltasToSprites()
    {
        //newSpritePath  图集 导出路径   默认为 Assets同级目录下。AltasToSprites文件夹+图片在Assets中的路径
        string newSpritePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "AltasToSprites");
        UnityEngine.Debug.Log("newSpritePath---->" + newSpritePath);
        newSpritePath = newSpritePath.Replace("\\","/");
        
        if (!Directory.Exists(newSpritePath)) 
            Directory.CreateDirectory(newSpritePath);
        Texture2D[] altases = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
        int index = 0;
        
        EditorApplication.update = () =>
        {
            string altasPath = AssetDatabase.GetAssetPath(altases[index]);
            UnityEngine.Debug.Log(altasPath);
            bool isCancel = EditorUtility.DisplayCancelableProgressBar("图集切割中~~", altasPath, (float)index / altases.Length);
            try
            {
                TextureImporter texImp = AssetImporter.GetAtPath(altasPath) as TextureImporter;
                if (texImp.textureType == TextureImporterType.Sprite && texImp.spritesheet.Length > 0) 
                    Process(texImp, altases[index], newSpritePath, altasPath);
                else 
                    UnityEngine.Debug.Log(string.Format("{0}-->格式不对 或者没有需要拆分的资源", altasPath));
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("图集切割错误---->" + altasPath);
                UnityEngine.Debug.LogError(e.Message);
            }
            index++;
            if (isCancel || index>= altases.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                Debug.Log("图集切割完成----->" + index.ToString());
                index = 0;
                System.Diagnostics.Process.Start(newSpritePath);
            }
        };
    }


    private static void Process(TextureImporter texImp, Texture2D texture, string rootPath,string altasPath)
    {
        altasPath = altasPath.Substring(altasPath.IndexOf("/") + 1);
        altasPath = altasPath.Replace(Path.GetExtension(altasPath), "");
        string filesDirectory = Path.Combine(rootPath, altasPath);
        bool isRead = texImp.isReadable;
        bool isRGB32 = texImp.GetAutomaticFormat(platform) == TextureImporterFormat.RGBA32;
        System.Action callBack = null;
        if (!isRead)
        {
            texImp.isReadable = true;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texImp));
            callBack += () => texImp.isReadable = isRead;
        }
        if (!isRGB32)
        {
            TextureImporterPlatformSettings textureImporterPlatformSettings = GetOldTextureImporterPlatformSettings(texImp);
            SetTexturePlatformSettings(!isRGB32, texImp, TextureImporterFormat.RGBA32);
            callBack += () => texImp.SetPlatformTextureSettings(textureImporterPlatformSettings);
        }
        
        if (!isRead||!isRGB32) 
            callBack += () => AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texImp));
        
        if (Directory.Exists(filesDirectory)) 
            Directory.Delete(filesDirectory,true);
        Directory.CreateDirectory(filesDirectory);
        
        foreach (SpriteMetaData metaData in texImp.spritesheet)
        {
            Texture2D myimage = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);
            for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++)
            {
                for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                    myimage.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, texture.GetPixel(x, y));
            }
            var pngData = myimage.EncodeToPNG();
            string fileName = Path.Combine(filesDirectory, metaData.name + ".PNG");
            fileName = fileName.Replace("\\", "/");
            File.WriteAllBytes(fileName, pngData);
        }
        if (callBack!=null) 
            callBack();
        callBack = null;
    }

    private static void SetTexturePlatformSettings(bool isNeedSet, TextureImporter texImp, TextureImporterFormat textureImporterFormat = TextureImporterFormat.RGBA32)
    {
        if (!isNeedSet) return;
        UnityEditor.TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings
        {
            name = platform,
            overridden = true,
            maxTextureSize = 2048,
            format = textureImporterFormat,
            compressionQuality = (int)TextureCompressionQuality.Normal,
            allowsAlphaSplitting = false,
            textureCompression = TextureImporterCompression.Compressed,
            crunchedCompression = false
        };
        texImp.SetPlatformTextureSettings(settings);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texImp));
    }


    private static TextureImporterPlatformSettings GetOldTextureImporterPlatformSettings(TextureImporter texImp)
    {
        int maxTextureSize;
        TextureImporterFormat textureFormat;
        bool etc1AlphaSplitEnabled;
        int compressionQuality;
        bool overridden = texImp.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality, out etc1AlphaSplitEnabled);
        if (platform != defaultPlatform)
        {
            if (!overridden)
            {
                platform = defaultPlatform;
                overridden = texImp.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality, out etc1AlphaSplitEnabled);
            }
        }
        return new TextureImporterPlatformSettings()
        {
            name = platform,
            overridden = overridden,
            maxTextureSize = 2048,
            format = textureFormat,
            compressionQuality = (int)TextureCompressionQuality.Normal,
            allowsAlphaSplitting = false,
            textureCompression = TextureImporterCompression.Compressed,
            crunchedCompression = false
        };
    }
}