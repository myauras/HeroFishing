using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
using Service.Realms;

[MapTo("player")]
public partial class DBPlayerState : IRealmObject {
    [Required]
    [MapTo("_id")]
    [PrimaryKey]
    public string ID { get; private set; }
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; private set; }
    [MapTo("lastUpdateAt")]
    public DateTimeOffset LastUpdateAt { get; private set; }
    public void SetLastUpdateAt(DateTimeOffset _lastUpdateAt) {
        RealmManager.MyRealm.WriteAsync(() => {
            LastUpdateAt = _lastUpdateAt;
        });
    }

}