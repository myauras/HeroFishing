using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinEffectItemUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtPoints;
    private int _points;
    public void SetPoints(int points) {
        if (_points == points) return;
        _txtPoints.text = points.ToString();
        _points = points;
    }
}
