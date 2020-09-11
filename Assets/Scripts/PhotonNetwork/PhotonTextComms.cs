using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

public class PhotonTextComms : MonoBehaviour, IChatClientListener
{
    public ChatClient chatClient;

    public string[] ChannelsToJoinOnConnect = {"General", "Whisper"};

    public int HistoryLengthToFetch = 5;
    

    public void OnConnected()
	{
		if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length > 0)
		{
			this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
		}



		this.chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
	}

	public void OnDisconnected()
	{
	    //this.ConnectingLabel.SetActive(false);
	}

	public void OnChatStateChange(ChatState state)
	{
		// use OnConnected() and OnDisconnected()
		// this method might become more useful in the future, when more complex states are being used.

		//this.StateText.text = state.ToString();
	}

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));


	}

    public void OnSubscribed(string[] channels, bool[] results)
	{
        /*
		// in this demo, we simply send a message into each channel. This is NOT a must have!
		foreach (string channel in channels)
		{
			this.chatClient.PublishMessage(channel, "says 'hi'."); // you don't HAVE to send a msg on join but you could.

			if (this.ChannelToggleToInstantiate != null)
			{
				this.InstantiateChannelButton(channel);

			}
		}*/

        this.chatClient.PublishMessage(channels[0], "says 'hi'."); // you don't HAVE to send a msg on join but you could.

		Debug.Log("OnSubscribed: " + string.Join(", ", channels));

		/*
        // select first subscribed channel in alphabetical order
        if (this.chatClient.PublicChannels.Count > 0)
        {
            var l = new List<string>(this.chatClient.PublicChannels.Keys);
            l.Sort();
            string selected = l[0];
            if (this.channelToggles.ContainsKey(selected))
            {
                ShowChannel(selected);
                foreach (var c in this.channelToggles)
                {
                    c.Value.isOn = false;
                }
                this.channelToggles[selected].isOn = true;
                AddMessageToSelectedChannel(WelcomeText);
            }
        }
        */

		// Switch to the first newly created channel
	    this.ShowChannel(channels[0]);
	}
    
	public void OnUnsubscribed(string[] channels)
	{
        /*
		foreach (string channelName in channels)
		{
			if (this.channelToggles.ContainsKey(channelName))
			{
				Toggle t = this.channelToggles[channelName];
				Destroy(t.gameObject);

				this.channelToggles.Remove(channelName);

				Debug.Log("Unsubscribed from channel '" + channelName + "'.");

				// Showing another channel if the active channel is the one we unsubscribed from before
				if (channelName == this.selectedChannelName && this.channelToggles.Count > 0)
				{
					IEnumerator<KeyValuePair<string, Toggle>> firstEntry = this.channelToggles.GetEnumerator();
					firstEntry.MoveNext();

				    this.ShowChannel(firstEntry.Current.Key);

					firstEntry.Current.Value.isOn = true;
				}
			}
			else
			{
				Debug.Log("Can't unsubscribe from channel '" + channelName + "' because you are currently not subscribed to it.");
			}
		}*/
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{

	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{

	}

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    /// <inheritdoc />
    public void OnChannelPropertiesChanged(string channel, string userId, Dictionary<object, object> properties)
    {
        Debug.LogFormat("OnChannelPropertiesChanged: {0} by {1}. Props: {2}.", channel, userId, Extensions.ToStringFull(properties));
    }

    public void OnUserPropertiesChanged(string channel, string targetUserId, string senderUserId, Dictionary<object, object> properties)
    {
        Debug.LogFormat("OnUserPropertiesChanged: (channel:{0} user:{1}) by {2}. Props: {3}.", channel, targetUserId, senderUserId, Extensions.ToStringFull(properties));
    }

    /// <inheritdoc />
    public void OnErrorInfo(string channel, string error, object data)
    {
        Debug.LogFormat("OnErrorInfo for channel {0}. Error: {1} Data: {2}", channel, error, data);
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
	{
		if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
		{
			Debug.LogError(message);
		}
		else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
		{
			Debug.LogWarning(message);
		}
		else
		{
			Debug.Log(message);
		}
	}

    public void ShowChannel(string channelName)
	{
		if (string.IsNullOrEmpty(channelName))
		{
			return;
		}

		ChatChannel channel = null;
		bool found = this.chatClient.TryGetChannel(channelName, out channel);
		if (!found)
		{
			Debug.Log("ShowChannel failed to find channel: " + channelName);
			return;
		}

		Debug.Log("ShowChannel: " + channelName);


	}
}
