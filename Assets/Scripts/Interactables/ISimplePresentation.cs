using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ISimplePresentation : Interactables, IPunObservable
{
    public string linkURL;
    public GameObject[] slideWindows;
    int currentSlide = 0;
    public int slideCount = 0;
    public Sprite[] images;
    public Text text;

    private void Start()
    {
        slideCount = images.Length;
        text.text = "1 / " + slideCount;
    }

    public override void Perform(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {

            //NextSlide();
            GetComponent<PhotonView>().RPC("NextSlide", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void NextSlide()
    {
        //have it before setting sprite, sprite is already at default
        currentSlide++;

        if (currentSlide > slideCount - 1)
        {
            currentSlide = 0;
            foreach (GameObject g in slideWindows)
            {
                g.GetComponent<SpriteRenderer>().sprite = images[currentSlide];
            }
            text.text = (currentSlide + 1) + " / " + slideCount;
            return;
        }
        foreach (GameObject g in slideWindows)
        {
            g.GetComponent<SpriteRenderer>().sprite = images[currentSlide];
        }
        text.text = (currentSlide + 1) + " / " + slideCount;
    }

    public void SetSlide(int slideNum)
    {
        if (slideNum > slideCount - 1)
        {
            currentSlide = 0;
            foreach (GameObject g in slideWindows)
            {
                g.GetComponent<SpriteRenderer>().sprite = images[currentSlide];
            }
            text.text = (currentSlide + 1) + " / " + slideCount;
            return;
        }
        foreach (GameObject g in slideWindows)
        {
            g.GetComponent<SpriteRenderer>().sprite = images[currentSlide];
        }
        text.text = (slideNum + 1) + " / " + slideCount;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentSlide);
        }
        else
        {
            currentSlide = (int)stream.ReceiveNext();
            SetSlide(currentSlide);
        }

    }
}
