using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
using System.Diagnostics;

public partial class Player : DBData {
    [MapTo("Name")]
    public string? Name { get; set; }

}
