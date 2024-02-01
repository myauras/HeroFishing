using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinEffectItemUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtPoints;
    public void SetPoints(int points) {
        _txtPoints.text = "+" + points.ToString();
    }
}
