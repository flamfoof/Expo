using UnityEngine.InputSystem;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Assertions;

public class ISimpleBtnClick : Interactables
{
    public PhotonView photonView;

    private void Start()
    {
        Assert.IsNotNull(photonView);
    }
    public override void Perform(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            SendPlayerNick();
        }
    }
    public void SendPlayerNick()
    {
        if (ChatMasenger.Instance != null)
        {
            ChatMasenger.Instance.SendDirectMsg(photonView.Owner.NickName);
        }
    }
}
