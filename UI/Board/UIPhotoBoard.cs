using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class UIPhotoBoard : MonoBehaviour
    {
        public Image uiImage;
        
        public void SetPhoto(Sprite sprite)
        {
            uiImage.sprite = sprite;
        }
    }
}