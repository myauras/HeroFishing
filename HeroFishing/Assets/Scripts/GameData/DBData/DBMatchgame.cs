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
    [Required]
    [MapTo("dbMapID")]
    public string DBMapID { get; private set; }
    [MapTo("playerIDs")]
    [Required]
    public IList<string> PlayerIDs { get; }
    [MapTo("ip")]
    [Required]
    public string IP { get; private set; }
    [MapTo("port")]
    public int Port { get; set; }

}