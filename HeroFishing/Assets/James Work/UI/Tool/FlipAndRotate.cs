using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipAndRotate : MonoBehaviour
{
    public Transform parentTransform; // 參考父物件的 Transform
    public Transform childTransform; // 參考子物件的 Transform
    public Vector3 localRotationAngles = Vector3.zero; // 設定變換的局部角度

    private Vector3 originalScale; // 原始的縮放值

    private void Start()
    {
        originalScale = parentTransform.localScale;
    }

    private void Update()
    {
        // 檢查父物件的 x 縮放值是否為正數
        if (parentTransform.localScale.x > 0f)
        {
            // 父物件的縮放值為正數，將子物件的局部角度設為(0,0,0)
            childTransform.localRotation = Quaternion.identity;
        }
        else
        {
            // 父物件的縮放值為負數，將子物件的局部角度設為指定的localRotationAngles
            childTransform.localRotation = Quaternion.Euler(localRotationAngles);
        }
    }

    // 設定局部旋轉角度
    public void SetLocalRotationAngles(Vector3 angles)
    {
        localRotationAngles = angles;
    }
}

