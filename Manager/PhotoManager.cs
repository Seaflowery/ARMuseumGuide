using System.Collections.Generic;
using System.Linq;
using Character;
using UI;
using UnityEngine;
using UnityEngine.Windows.WebCam;


public class PhotoManager : MonoBehaviour
{
    
    public static PhotoManager Instance;

    public GameObject objPhotoBoard; // 照片面板
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }



    private PhotoCapture photoCaptureObject;
    private bool captureIsActive;
    
    
    private int _photoWidth; // 图片宽度
    private int _photoHeight; // 图片高度
    private byte[] _photoByte; // 图片的字节
    
    /// <summary>
    /// 点击【拍照】按钮触发
    /// </summary>
    public void StartCapture()
    {
        if (!captureIsActive)
        {
            Debug.Log("StartCapture...");
            captureIsActive = true;
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }
        else
        {
            captureIsActive = false;
        }
    }

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;
        var cameraResolution = PhotoCapture.SupportedResolutions
            .OrderByDescending(res => res.width * res.height)
            .First();
        var cameraParams = new CameraParameters
        {
            hologramOpacity = 0f,
            cameraResolutionWidth = cameraResolution.width,
            cameraResolutionHeight = cameraResolution.height,
            pixelFormat = CapturePixelFormat.JPEG
        };

        _photoWidth = cameraResolution.width;
        _photoHeight = cameraResolution.height;

        captureObject.StartPhotoModeAsync(cameraParams, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync((captureResult, frame) =>
                {
                    if (captureResult.success)
                    {
                        Debug.Log("Photo capture done.");

                        var buffer = new List<byte>();
                        frame.CopyRawImageDataIntoBuffer(buffer);
                        byte[] photoByte = buffer.ToArray();
                        _photoByte = photoByte;
                        GetPhoto(photoByte);
                    }

                    photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
                }
            );
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    private void GetPhoto(byte[] photoByte)
    {
        Texture2D texture = new Texture2D(_photoWidth, _photoHeight, TextureFormat.ARGB32, true);
        texture.LoadImage(photoByte);
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        
        objPhotoBoard.SetActive(true);
        objPhotoBoard.transform.GetComponent<UIPhotoBoard>().SetPhoto(sprite);
        GuideBehavior.Instance.OnFinishTakePhoto();
    }

    private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
        captureIsActive = false;
    }
    
}