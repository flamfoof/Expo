using UnityEngine;
using System.Collections.Generic;
#if UNITY_ANDROID
using UnityEngine.Android;	
#endif
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
using Microphone = UnityEngine.Microphone;
#endif

namespace Photon.Voice.Unity
{
	public class CustomMicrophone
	{
		
		public static string[] devices
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			get { return Photon.Voice.Unity.MicrophoneGL.Instance.GetMicrophoneDevices(); }
#else
			get { return new string[0]; }
#endif
		}

		public static string[] GetMicrophoneDevices()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.Instance.GetMicrophoneDevices(); 
#else
			return new string[0]; 
#endif
		}

		public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.Instance.Start(deviceName, loop, lengthSec, frequency);
#else
			throw new System.NotImplementedException("microphone not implemented yet");
#endif
		}

		public static bool IsRecording(string deviceName)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.Instance.IsRecording(deviceName);
#else
			return false;
#endif
		}

		public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxfreq)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			Photon.Voice.Unity.MicrophoneGL.Instance.GetDeviceCaps(deviceName, out minFreq, out maxfreq);
#else
			minFreq = 0;
			maxfreq = 0;
#endif
		}

		public static int GetPosition(string deviceName)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.Instance.GetPosition(deviceName);
#else
			return 0;
#endif
		}

		public static void End(string deviceName)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			Photon.Voice.Unity.MicrophoneGL.Instance.End(deviceName);
#else
			throw new System.NotImplementedException("microphone not implemented yet");
#endif
		}

		public static bool HasConnectedMicrophoneDevices()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.Instance.HasConnectedMicrophoneDevices();
#else
			return false;
#endif
		}

		public static void RequestMicrophonePermission()
		{
			if (!HasMicrophonePermission())
			{
#if UNITY_ANDROID
				Permission.RequestUserPermission(Permission.Microphone);
#elif UNITY_WEBGL  && !UNITY_EDITOR
				Photon.Voice.Unity.MicrophoneGL.Instance.RequestPermission();
#endif
			}
		}

		public static bool HasMicrophonePermission()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.Instance.HasUserAuthorizedPermission();
#else
			return true;
#endif
		}

		public static bool GetRawData(ref float[] output, AudioClip source = null)
		{
#if UNITY_WEBGL  && !UNITY_EDITOR
			output = Photon.Voice.Unity.MicrophoneGL.Instance.GetRawData();
			return true;
#else
			if (source == null)
				return false;

			source.GetData(output, 0);
			return true;
#endif
		}

		public static void Init()
		{
			#if UNITY_WEBGL  && !UNITY_EDITOR
			Photon.Voice.Unity.MicrophoneGL.Init();
			#endif
		}

        public static void QueryAudioInput()
		{
			#if UNITY_WEBGL  && !UNITY_EDITOR
			Photon.Voice.Unity.MicrophoneGL.QueryAudioInput();
			#endif
		}

        private static int GetNumberOfMicrophones()
		{
			#if UNITY_WEBGL  && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.GetNumberOfMicrophones();
			#else
			return 24;
			#endif
		}

        private static string GetMicrophoneDeviceName(int index)
		{
			#if UNITY_WEBGL  && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.GetMicrophoneDeviceName(index);
			#else
			return "-2.0f";
			#endif
		}

        private static float GetMicrophoneVolume(int index)
		{
			#if UNITY_WEBGL  && !UNITY_EDITOR
			return Photon.Voice.Unity.MicrophoneGL.GetMicrophoneVolume(index);
			#else
			return -2.0f;
			#endif
		}

		public static float[] volumes
        {
            get
            {
                List<float> list = new List<float>();
                int size = GetNumberOfMicrophones();
                for (int index = 0; index < size; ++index)
                {
                    float volume = GetMicrophoneVolume(index);
                    list.Add(volume);
                }
                return list.ToArray();
            }
        }
	}


}