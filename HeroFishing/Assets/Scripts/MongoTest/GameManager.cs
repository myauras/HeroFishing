using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RealmManager.NewApp("application-0-pgfgf");
        if (RealmManager.MyApp.CurrentUser == null) {
            Debug.LogError("註冊使用者");
            RealmManager.AnonymousSignUp();
        } else {
            Debug.LogError("已經有使用者了");
            RealmManager.OnSignin();
        }

    }
    public void OnClick() {
        RealmManager.GetData();
    }

    public void CallFunc() {
        Debug.LogError("RealmManager.MyApp.CurrentUser.Id=" + RealmManager.MyApp.CurrentUser.Id);
        RealmManager.CallFunc("Player_Signup", RealmManager.MyApp.CurrentUser, RealmManager.MyApp.CurrentUser.Id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
