using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
using Service.Realms;

[MapTo("player")]
public partial class DBPlayer : IRealmObject {
    [MapTo("_id")]
    [PrimaryKey]
    [Required]
    public string ID { get; private set; }
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; private set; }
    [MapTo("authType")]
    [Required]
    public string AuthType { get; private set; }
    [MapTo("ban")]
    public bool Ban { get; private set; }
    [MapTo("deviceUID")]
    [Required]
    public string DeviceUID { get; private set; }
    [MapTo("lastSigninAt")]
    public DateTimeOffset LastSigninAt { get; private set; }
    [MapTo("lastSignoutAt")]
    public DateTimeOffset LastSignoutAt { get; private set; }
    [MapTo("onlineState")]
    [Required]
    public string OnlineState { get; private set; }
    [MapTo("point")]
    public long Point { get; private set; }

    public void SetDeviceUID(string _deviceUID) {
        RealmManager.MyRealm.WriteAsync(() => {
            DeviceUID = _deviceUID;
        });
    }

}