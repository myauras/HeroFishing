using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using Realms.Sync;
using MongoDB.Bson;
using Realms.Exceptions;
using System;

public static class RealmManager 
{
    public static App MyApp { get; private set; }
    public static Realm MyRealm { get; private set; }

    /// <summary>
    /// �ǤJ��appID���͹���App�A�̪�C����l�ƭn��New�@��Ream App
    /// </summary>
    /// <param name="_appID">MongoDB App Services��AppID</param>
    public static void NewApp(string _appID) {
        MyApp = App.Create(_appID); // �Ы� Realm App
    }
    /// <summary>
    /// �ΦW���U
    /// </summary>
    public static async void AnonymousSignUp() {
         await MyApp.LogInAsync(Credentials.Anonymous());
        OnSignin();
    }
    /// <summary>
    /// �H�c�K�X���U
    /// </summary>
    /// <param name="_email">�H�c</param>
    /// <param name="_pw">�K�X</param>
    public static async void EmailPWSignUp(string _email, string _pw) {
         await MyApp.LogInAsync(
            Credentials.EmailPassword(_email, _pw));
        OnSignin();
    }

    /// <summary>
    /// ���a�n�J�����
    /// </summary>
    public static void OnSignin() {
        SetConfiguration();
    }

    /// <summary>
    /// �]�wFlexibleSyncConfg
    /// </summary>
    static async void SetConfiguration() {
        Debug.LogError("SetConfiguration: "+ MyApp.CurrentUser);

        var config = new FlexibleSyncConfiguration(MyApp.CurrentUser) {
            PopulateInitialSubscriptions = (realm) =>
            {
                var players = realm.All<Player>();
                realm.Subscriptions.Add(players);
            }
        };



        // The process will complete when all the user's items have been downloaded.
        MyRealm = await Realm.GetInstanceAsync(config);
        Debug.LogError("realm=" + MyRealm);
    }



    public static void GetData() {
        var playerId = new ObjectId("649152d10d942c2bf0ccafb5");
        var player = MyRealm.Find<Player>(playerId);

        // �b�o�̨ϥ� player ����
        if (player != null) {
            // ���F���������
            Debug.LogError("Name=" + player.Name);
            Debug.LogError("Id=" + player.ID);
            // ...
        } else {
            Debug.LogError("�䤣��");
            // �S�������������
        }
    }

    public static async void CallFunc(string _funcName,params object[] _params) {
        Debug.LogError("CallFunc");
        try {
            var bsonValue = await MyApp.CurrentUser.Functions.CallAsync(_funcName, _params);
            Debug.LogError(bsonValue.ToString());
            var result = bsonValue.ToBsonDocument().ToDictionary();
            Debug.LogError("test");
        } catch(Exception _e) {
            Debug.LogError("�I�sFunction���~: " + _funcName+"//// E: "+ _e);
        }
 
    }

}
