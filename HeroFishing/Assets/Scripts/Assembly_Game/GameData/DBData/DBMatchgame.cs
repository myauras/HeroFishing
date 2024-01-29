using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
using Service.Realms;
using Scoz.Func;

[MapTo("matchgame")]
public partial class DBMatchgame : IRealmObject {
    [MapTo("_id")]
    [Required]
    [PrimaryKey]
    public string ID { get; private set; }
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; private set; }
    [MapTo("dbMapID")]
    public string DBMapID { get; private set; }
    [MapTo("playerIDs")]
    [Required]
    public IList<string> PlayerIDs { get; }
    [MapTo("ip")]
    public string IP { get; private set; }
    [MapTo("port")]
    public int? Port { get; set; }

    public DBMatchgame(BsonDocument _doc) {
        try {
            ID = _doc["_id"].AsString;
            CreatedAt = _doc["createdAt"].ToUniversalTime();
            DBMapID = _doc["dbMapID"].AsString;
            IP = _doc["ip"].AsString;
            Port = _doc["port"].AsInt32;
            PlayerIDs = ExtractPlayerIDs(_doc["playerIDs"]);
        }catch(Exception _e) {
            WriteLog.LogError("ÂàBsonDocument¿ù»~: " + _e);
        }
    }
    private static IList<string> ExtractPlayerIDs(BsonValue _bson) {
        var list = new List<string>();
        if (_bson is BsonArray bsonArray) {
            foreach (var item in bsonArray) {
                list.Add(item.AsString);
            }
        }
        return list;
    }

}