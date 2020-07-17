using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WeIgnite
{
    public class TimerScript : MonoBehaviour
    {
        public float timeSpent = 0.0f;
        private bool canStart = false;

        private void OnEnable()
        {
            StartTimer();
        }
        void Update()
        {
            if (canStart)
            {
                timeSpent += Time.deltaTime;
                string hr = ((int)timeSpent / 3600).ToString("00");
                string min = ((int)(timeSpent % 3600) / 60).ToString("00");
                string sec = (timeSpent % 60).ToString("00");
            }
        }// END OF UPDATE FUNCTION
        public void StartTimer()
        {
            canStart = true;
        }
        public void StopTimer()
        {
            canStart = false;
        }
    }
}
