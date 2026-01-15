using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Script.UI.Text.View
{
    public class survivalTimeView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI survivalTimeText;

        public void UpdateSurvivalTime(float time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);
            survivalTimeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
    }
}
