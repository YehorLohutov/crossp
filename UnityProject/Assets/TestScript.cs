using UnityEngine;
using Crossp;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    private CrosspSticker crosspSticker = default;
    [SerializeField]
    private KeyCode showStickerKeyCode = default;
    [SerializeField]
    private KeyCode hideStickerKeyCode = default;
    [SerializeField]
    private KeyCode playStickerKeyCode = default;

    void Update()
    {
        if (Input.GetKeyDown(showStickerKeyCode))
            crosspSticker.Show();
        if (Input.GetKeyDown(hideStickerKeyCode))
            crosspSticker.Hide();
        if (Input.GetKeyDown(playStickerKeyCode))
            crosspSticker.Play();
    }
}
