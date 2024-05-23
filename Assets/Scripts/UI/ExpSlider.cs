using Examen.Player.PlayerDataManagement;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.UI
{

    public class ExpSlider : NetworkBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _text;

        /// <summary>
        /// Sets up the exp bar in the playerDatabase.
        /// </summary>
        public void SetUPSlider()
        {
            if (!IsOwner)
                return;

            PlayerDatabase.Instance.ExpBar = _slider;
            PlayerDatabase.Instance.ExpText = _text;
        }
    }
}