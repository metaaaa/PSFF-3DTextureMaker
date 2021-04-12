using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Create3DTexture : MonoBehaviour
{
    [SerializeField] int size = 32;
    [SerializeField] TextureWrapMode wrapMode = TextureWrapMode.Clamp;
    [SerializeField] ComputeShader computeshader = null;
    private TextureFormat format = TextureFormat.RGBAHalf;
    private RenderTexture renderTex = null;

    [ContextMenu ("Gen 3dtexture")]
    void GenCurlNoise3DTex()
    {
        // Initialize Texture3D
        Texture3D texture3d = new Texture3D(size, size, size, format, false);
        texture3d.wrapMode = wrapMode;



        // Create a 3-dimensional array to store color data
        Color[] colors3d = new Color[size * size * size];

        for(int i=0; i<size; i++)
        {
            // Initialize RenderTexture
            renderTex = new RenderTexture(size,size,1,RenderTextureFormat.ARGBHalf);
            renderTex.enableRandomWrite = true;
            renderTex.useMipMap = false;
            renderTex.wrapMode = wrapMode;
            renderTex.useDynamicScale = false;

            // Initialize Texture2D
            Texture2D texture2d = new Texture2D(size,size,format,false);
            texture2d.wrapMode = wrapMode;

            RenderTexture.active = renderTex;
            // calc curl-noise and save RenderTexture
            computeshader.SetFloat("z", (float)i);
            computeshader.SetFloat("size", (float)size);
            computeshader.SetTexture(0,"tex",renderTex);
            computeshader.Dispatch(0,size/8,size/8,1);

            // copy rendertex to tex2d
            RenderTexture.active = renderTex;
            texture2d.ReadPixels(new Rect(0,0,renderTex.width,renderTex.height),0,0);
            texture2d.Apply();

            // tex2d to color array
            var colors = texture2d.GetPixels();

            int index = 0;
            foreach (var color in colors)
            {
                // Debug.Log(color);
                colors3d[size*size*i+index] = color;
                index += 1;
            }

            Resources.UnloadUnusedAssets();
        }

        // 3D texture setpixels
        texture3d.SetPixels(colors3d);
        texture3d.Apply();

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture3d, "Assets/CurlNoiseParticleSystem/Textures/CurlNoise3DTexture.asset");

    }

    void OnDestroy()
    {
        renderTex.Release();
    }

}
