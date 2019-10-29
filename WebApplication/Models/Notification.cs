using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string NotificationId { set; get; }
        public DateTime NotificationTime { set; get; }
        public ObjectId IncidentId { set; get; }
        public List<string> MailsNotified { set; get; }

    }
}