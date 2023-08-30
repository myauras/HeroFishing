using Realms;
using MongoDB.Bson;
using System;

[MapTo("player")]
public partial class DBPlayer : IRealmObject {
    [PrimaryKey]
    [MapTo("_id")]
    public string ID { get; set; }
    [MapTo("createAt")]
    public DateTimeOffset CreateAt { get; set; }
    [MapTo("authType")]
    public string AuthType { get; set; }
    [MapTo("point")]
    public long Point { get; set; }
    [MapTo("onlineState")]
    public string OnlineState { get; set; }



}