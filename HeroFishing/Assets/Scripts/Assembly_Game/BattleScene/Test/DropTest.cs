using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropTest : MonoBehaviour
{
    [SerializeField]
    private Button _btnDarkStorm;
    [SerializeField]
    private Button _btnFrozen;

    private void Start() {
        _btnDarkStorm.onClick.AddListener(() => {
            DropManager.Instance.AddDrop(0, 15);
        });
        _btnFrozen.onClick.AddListener(() => {
            DropManager.Instance.AddDrop(0, 4);
        });
    }
}
