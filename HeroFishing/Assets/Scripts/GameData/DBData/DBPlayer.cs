using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;

[MapTo("player")]
public partial class DBPlayer : IRealmObject {
    [Required]
    [MapTo("_id")]
    [PrimaryKey]
    public string ID { get; set; }
    [Required]
    [MapTo("authType")]
    public string AuthType { get; set; }
    [MapTo("ban")]
    public bool Ban { get; set; }
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
    [Required]
    [MapTo("deviceUID")]
    public string DeviceUID { get; set; }
    [MapTo("lastSignin")]
    public DateTimeOffset LastSignin { get; set; }
    [MapTo("lastSignout")]
    public DateTimeOffset LastSignout { get; set; }
    [Required]
    [MapTo("onlineState")]
    public string OnlineState { get; set; }
    [MapTo("point")]
    public long Point { get; set; }
}