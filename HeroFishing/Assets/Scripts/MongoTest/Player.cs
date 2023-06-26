using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
using System.Diagnostics;

public partial class Player : IRealmObject {
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId? ID { get; set; }
    [MapTo("Name")]
    public string? Name { get; set; }

}

//[MapTo("Timestamp")]
//public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;