using Steamworks;
using UnityEngine.UI;
using UnityEngine;

public class SPlayerLıstItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionId;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public Text PlayerNameText;
    public RawImage PlayerIcon;
    
    public Text PlayerReadyText;
    public bool Ready;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    public void ChangeReadyStatus()
    {
        if (Ready) 
        {
            PlayerReadyText.text = "Ready"; // Ready
            PlayerReadyText.color = Color.green;
        }
        else 
        {
            PlayerReadyText.text = "UnReady"; //Not ready
            PlayerReadyText.color = Color.red;
        }
    }
    
    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        ChangeReadyStatus();
        if (!AvatarReceived) GetPlayerIcon();
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if(ImageID == -1) return;
        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }
    
    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else return;
    }

    private Texture2D GetSteamImageAsTexture(int Image)
    {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(Image, out uint width,out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(Image,image,(int)(width * height * 4));
            if (isValid)
            {
                texture = new Texture2D((int)width,(int)height,TextureFormat.RG32,false,true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;
    }
}
