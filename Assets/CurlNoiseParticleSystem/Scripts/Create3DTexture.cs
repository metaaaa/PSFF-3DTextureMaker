using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 3Dテクスチャを作成する
/// </summary>
public class Create3DTexture : MonoBehaviour
{
    [SerializeField] int size = 64; //立方体3Dテクスチャの1辺のサイズ
    [SerializeField] Vector3 offset = new Vector3(0f,0f,0f);//ノイズの位相のずれ
    [SerializeField] float density = 3f;//ノイズの密度
    [SerializeField] TextureWrapMode wrapMode = TextureWrapMode.Clamp; //テクスチャのラップモード
    [SerializeField] ComputeShader computeshader = null; //計算に使うComputeShader
    [SerializeField] string savePath = "Assets/CurlNoiseParticleSystem/Textures/";//3Dテクスチャ保存パス
    [SerializeField] string fileName = "CurlNoise3DTexture";//3Dテクスチャ名
    private TextureFormat format = TextureFormat.RGBAHalf;//テクスチャの色のフォーマット
    const int threadCount = 8;//compute shader のスレッド数

    /// <summary>
    /// ContextMenuから3Dテクスチャを生成する
    /// </summary>
    [ContextMenu ("Gen 3dtexture")]
    void GenCurlNoise3DTex()
    {
        // Texture3D 初期化
        Texture3D texture3d = new Texture3D(size, size, size, format, false);
        texture3d.wrapMode = wrapMode;

        // 色情報を保存する3次元の配列を初期化する
        Color[] colors3d = new Color[size * size * size];

        // float4 配列の compute buffer
        ComputeBuffer colorBuffer = new ComputeBuffer(size * size * size, sizeof(float) * 4);

        // ComputeShaderに値とバッファ渡して計算する
        computeshader.SetFloat("size", (float)size);
        computeshader.SetFloat("density", density);
        computeshader.SetVector("offset", offset);
        computeshader.SetBuffer(0,"velocityBuffer",colorBuffer);
        computeshader.Dispatch(0, size/threadCount, size/threadCount, size/threadCount);

        // 結果を貰う
        colorBuffer.GetData(colors3d);

        // 配列のデータを3D texture にうつす
        texture3d.SetPixels(colors3d);
        texture3d.Apply();

        // アセットファイルとして3DTextureを保存
        AssetDatabase.CreateAsset(texture3d, savePath+fileName+".asset");

        // メモリ開放
        colorBuffer.Release();
        Resources.UnloadUnusedAssets();
    }
}
