using UnityEngine;
using UnityEngine.UI;

public class PlayerBtnClicked : MonoBehaviour
{
    public string playerName;

    [HideInInspector] 
    public GameObject CurrentPlayerScreen;
    public GameObject MsgScreensParent;
    public Text PlayerBtnCountText;
    public Image PlayerBtnCountImage;
    private void Start()
    {
        MsgScreensParent = ChatMasenger.Instance.ButonContent;
    }
    public void OnPlayerBtnClick()
    {
        this.PlayerBtnCountText.text = 0.ToString();
        PlayerBtnCountImage.gameObject.SetActive(false);
        playerName = this.gameObject.transform.GetChild(0).GetComponent<Text>().text;
        ChatMasenger.Instance.ReciverPlayer.text = playerName;
        ChatMasenger.Instance.PrivateChatPanel.SetActive(true);
        CurrentPlayerScreen = ChatMasenger.Instance.currentScreen = MsgScreensParent.transform.Find(playerName).gameObject;
        GetChildWithName(CurrentPlayerScreen, playerName);
        ChatMasenger.Instance.messageToSend.Select();
    }
    public void GetChildWithName(GameObject currentPlayerScreen, string playerName)
    {
        currentPlayerScreen.transform.SetAsLastSibling();
    }
}
