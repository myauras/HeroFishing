using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
using Service.Realms;

[MapTo("playerMatchgame")]
public partial class DBPlayerMatchgame : IRealmObject {
    [Required]
    [MapTo("_id")]
    [PrimaryKey]
    public string ID { get; private set; }
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; private set; }
    [Required]
    [MapTo("authType")]
    public string AuthType { get; private set; }
    [MapTo("ban")]
    public bool Ban { get; private set; }
    [Required]
    [MapTo("deviceUID")]
    public string DeviceUID { get; private set; }
    [MapTo("lastSigninAt")]
    public DateTimeOffset LastSigninAt { get; private set; }
    [MapTo("lastSignoutAt")]
    public DateTimeOffset LastSignoutAt { get; private set; }
    [Required]
    [MapTo("onlineState")]
    public string OnlineState { get; private set; }
    [MapTo("point")]
    public long Point { get; private set; }

    public void SetDeviceUID(string _deviceUID) {
        RealmManager.MyRealm.WriteAsync(() => {
            DeviceUID = _deviceUID;
        });
    }

}