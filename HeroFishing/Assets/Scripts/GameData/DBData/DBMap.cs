using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
using Service.Realms;

[MapTo("map")]
public partial class DBMap : IRealmObject {
    [MapTo("_id")]
    [Required]
    [PrimaryKey]
    public string Id { get; set; }
    [MapTo("bet")]
    public int Bet { get; set; }
    [MapTo("betThreshold")]
    public long BetThreshold { get; set; }
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
    [MapTo("enable")]
    public bool Enable { get; set; }
    [MapTo("jsonMapID")]
    [Required]
    public string JsonMapID { get; set; }
    [MapTo("matchType")]
    [Required]
    public string MatchType { get; set; }
    [MapTo("prefabID")]
    [Required]
    public string PrefabID { get; set; }
    [MapTo("priority")]
    public int Priority { get; set; }

}