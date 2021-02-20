using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
namespace Crossp
{
    public class CrosspTestScript : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer = default;
        [SerializeField]
        private VideoPlayer asd;
        // Start is called before the first frame update
        void Start()
        {
            Crossp.Initialize(CrosspSettings.Instance.ServerURL, CrosspSettings.Instance.ExternalId);
        }
        bool loaded = false;
        // Update is called once per frame
        void Update()
        {
            if(Crossp.IsReady() && !loaded)
            {
                //pcxFile = File.ReadAllBytes("Assets/5_ImageParser/bagit_icon.pcx");
                //int startPoint = 128;
                //int height = 1000;
                //int width = 1000;
                Texture2D texture2D = new Texture2D(0, 0);
                texture2D.LoadImage(Crossp.image);// LoadRawTextureData(pcxFile);
                texture2D.Apply();
                
                texture2D.EncodeToJPG();
                
                Sprite asd = Sprite.Create(texture2D, new Rect(0,0, texture2D.width, texture2D.height), Vector2.zero);
                spriteRenderer.sprite = asd;

                loaded = true;

                
            }
        }
    }
}
