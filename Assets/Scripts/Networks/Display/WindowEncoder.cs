using UnityEngine;
using System.Collections;
using uWindowCapture;
using Unity.Netcode;
using Newtonsoft.Json.Linq;

public class WindowEncoder : NetworkBehaviour
{
    [SerializeField]
    public Renderer texture = null;
    public UwcWindowTexture window = null;

    [SerializeField]
    public uNvEncoder.Encoder encoder = null;

    [SerializeField]
    public uNvEncoder.EncoderDesc setting = new uNvEncoder.EncoderDesc
    {
        width = 1920,
        height = 1080,
        frameRate = 30,
        format = uNvEncoder.Format.B8G8R8A8_UNORM,
        bitRate = 1228800,
        maxFrameSize = 40960,
    };

    public int idrFrameIntervalFrame = 30;
    int idrFrameCounter_ = 0;
    public int ResolutionDivider = 2;
    public Texture2D IdleTexture = null;
    public Texture2D sTexture;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        GetSetting();
        StartCoroutine(EncodeLoop());
    }

    void OnDisable()
    {
        if (!IsOwner) return;
        StopAllCoroutines();
        encoder.Destroy();
    }
    void GetSetting()
    {
        if (JsonConfig.HasKey("EncoderSetting"))
        {
            var _setting = JsonConfig.GetJObject("EncoderSetting");
            ResolutionDivider = _setting.Value<int>("ResolutionDivider");
            _setting.Remove("ResolutionDivider");
            _setting["format"] = (int)uNvEncoder.Format.B8G8R8A8_UNORM;
            setting = _setting.ToObject<uNvEncoder.EncoderDesc>();
        }
            
        SetSetting(setting, ResolutionDivider);
    }
    void SetSetting(uNvEncoder.EncoderDesc _setting, int resolutionDivider)
    {
        var JObj= JObject.FromObject(_setting);
        JObj.Remove("width"); JObj.Remove("height"); JObj.Remove("format");
        JObj["ResolutionDivider"] = resolutionDivider;
        JsonConfig.SetJObject("EncoderSetting", JObj);
    }   

    RenderTexture rt;
    void Resize()
    {
        if (texture.material.mainTexture == null)
            Graphics.Blit(IdleTexture, rt);
        else
            Graphics.Blit(texture.material.mainTexture, rt);
        sTexture.ReadPixels(new Rect(0,0,setting.width, setting.height),0,0);
        sTexture.Apply();
    }
    IEnumerator EncodeLoop()
    {
        Debug.Log("EncodeLoop");
        for (;;)
        {
            if (texture.material.mainTexture != null) break;
            Debug.Log("Wait for texture");
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("EncodeLoop SetSetting");
        
        setting.width = window.window.width / ResolutionDivider;
        setting.height = window.window.height / ResolutionDivider;
        idrFrameIntervalFrame = setting.frameRate;
        encoder.Create(setting);

        rt = new RenderTexture(setting.width, setting.height, 24);
        sTexture = new Texture2D(setting.width, setting.height, TextureFormat.BGRA32, false);
        RenderTexture.active = rt;
        Debug.Log("EncodeLoop Start");
        for (;;)
        {
            if (setting.frameRate < 60)
            {
                yield return new WaitForSeconds(1f / setting.frameRate);
            }

            bool idr = idrFrameCounter_++ % idrFrameIntervalFrame == 0;
            Resize();
            encoder.Encode(sTexture, idr);
            encoder.Update();
        }
    }

    [ContextMenu("Reconfigure")]
    public void Reconfigure()
    {
        if (encoder == null) return;
        encoder.Reconfigure(setting);
    }
}