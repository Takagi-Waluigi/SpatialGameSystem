using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Diagnostics;

public class TextureSender : MonoBehaviour
{
    [SerializeField] RenderTexture renderTexture;             //mainCameraにつけるRendertexture(アタッチしてね)
    private Texture2D inputTexture;                       //ほんとに保存されているかの確認用
    private Texture2D outputTexture;
    [SerializeField] RawImage img;
    [SerializeField] RawImage imgResize;
    private Camera mainCamera;

    const int PIXELS = 25;
    const int NUMTAPES = 2;
    const int Div = 60;
    [SerializeField] int FPS = 30;
    private const int unit = 1;
    private const int Bright = 100;
    private const int LED0Bright = 10;
    private string colorData = "";
    string[,] l = new string[Div, PIXELS * NUMTAPES];
    [SerializeField] string remoteHost = "192.168.11.31";
    [SerializeField] int remotePort = 1234;
    private System.Net.Sockets.UdpClient udp = new System.Net.Sockets.UdpClient();
    Stopwatch stopwatch = new Stopwatch();
    long pastTime = 0;
    long pastTime2 = 0;
    void Start ()
    {
        mainCamera = this.gameObject.GetComponent<Camera>();
        
        for(int j=0; j<Div; j++) //送信するデータ量で空の配列データを作成
        {
            for(int i=0; i<PIXELS*NUMTAPES; i++)
            {
                l[j,i] = "";
            }
        }
        stopwatch.Start();
    }
    void Update()
    {
        long currentTime = stopwatch.ElapsedMilliseconds;
        if(currentTime - pastTime > 1000/FPS){
            UnityEngine.Debug.Log("exec");
            //create texture
            inputTexture = CreateTexture2D(renderTexture);

            //convert texture
            outputTexture = ConvertTexture2D(inputTexture);

            
            //debug
            // img.texture = outputTexture;
            // imgResize.texture = ResizeTexture(inputTexture, PIXELS * 2 - 1, PIXELS * 2 - 1);

            pastTime = currentTime;
        }
    }

    /// <summary>
    /// ここでTextur2Dに変換しているよ
    /// </summary>
    /// <param name="rt"></param>
    /// <returns></returns>
    Texture2D CreateTexture2D(RenderTexture rt)
    {
        //Texture2Dを作成
        Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, false);

        //subCameraにRenderTextureを入れる
        mainCamera.targetTexture = rt;

        //手動でカメラをレンダリングします
        mainCamera.Render();


        RenderTexture.active = rt;
        texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture2D.Apply();

        //元に戻す別のカメラを用意してそれをRenderTexter用にすれば下のコードはいらないです。
        mainCamera.targetTexture = null;
        RenderTexture.active = null;

        return texture2D;
    }
    Texture2D ConvertTexture2D(Texture2D tex)
    {
        //resize texture
        Texture2D textureReduction = ResizeTexture(tex, PIXELS * 2 - 1, PIXELS * 2 - 1);
        // Texture2D textureReduction = tex;
        //readable texture by CPU for GetPixel
        Texture2D readableTexture2D = createReadabeTexture2D(textureReduction);

        //get center point
        int wC = Mathf.FloorToInt(textureReduction.width / 2);
        int hC = Mathf.FloorToInt(textureReduction.height / 2);

        //make texture2D for visualize polar img
        Texture2D texPolar = new Texture2D(hC, Div);

        for(int j=0; j<Div; j++)
        {
            for(int i=0; i< hC + 1; i++)
            {
                Color c = readableTexture2D.GetPixel
                (
                    wC - Mathf.CeilToInt(i * Mathf.Sin(2 * Mathf.PI / Div * j)),
                    hC - Mathf.CeilToInt(i * Mathf.Cos(2 * Mathf.PI / Div * j))
                );

                int rP = Mathf.FloorToInt(c.r * 255.0f * ((100 - LED0Bright) / PIXELS * i + LED0Bright) / 100 * Bright / 100);
                int gP = Mathf.FloorToInt(c.g * 255.0f * ((100 - LED0Bright) / PIXELS * i + LED0Bright) / 100 * Bright / 100);
                int bP = Mathf.FloorToInt(c.b * 255.0f * ((100 - LED0Bright) / PIXELS * i + LED0Bright) / 100 * Bright / 100);

                string strR = rP.ToString("X2"); // "x"はアルファベット部分が小文字, "X"は大文字となる
                string strG = gP.ToString("X2"); // "x"はアルファベット部分が小文字, "X"は大文字となる
                string strB = bP.ToString("X2"); // "x"はアルファベット部分が小文字, "X"は大文字となる
                
                l[j,hC-i] = string.Format("{0,0:X2}{1,0:X2}{2,0:X2}",strG,strR,strB);
                // colorData += l[j,i];

                texPolar.SetPixel(j,i,c);
            }

           
            texPolar.Apply();
            colorData = string.Format("{0,0:X2}",j);
            for(int i=0; i<PIXELS; i++){
                colorData += l[j,i];
            }
            for(int i=0; i<PIXELS; i++){
                colorData += l[(j+Div/2-1)%Div,PIXELS-1-i];
                if(i == PIXELS - 1){
                    long currentTime = stopwatch.ElapsedMilliseconds;
                    if(currentTime - pastTime2 > 1){
                        byte[] colorBytes = System.Text.Encoding.UTF8.GetBytes(colorData);
                        // Thread.Sleep(1);
                        udp.Send(colorBytes, colorBytes.Length, remoteHost, remotePort);
                        pastTime2 = currentTime;
                    }
                }
            }
                
            
        }
        return texPolar;
    }
    static Texture2D ResizeTexture(Texture2D srcTexture, int newWidth, int newHeight) { //https://mu-777.hatenablog.com/entry/2020/02/22/185225
        var resizedTexture = new Texture2D(newWidth, newHeight);
        Graphics.ConvertTexture(srcTexture, resizedTexture);
        return resizedTexture;
    }
    Texture2D createReadabeTexture2D(Texture2D texture2d)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(
                    texture2d.width,
                    texture2d.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(texture2d, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D readableTextur2D = new Texture2D(texture2d.width, texture2d.height);
        readableTextur2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTextur2D.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);
        return readableTextur2D;
    }
    private void OnApplicationQuit()
    {
        // ここにセーブ処理を記述します。
        UnityEngine.Debug.Log("Game is quitting. Saving game data...");
        udp.Close();
    }
}
