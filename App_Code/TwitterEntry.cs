using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

/// <summary>
/// Summary description for TwitterEntry
/// </summary>
public class TwitterEntry
{
    public TwitterEntry()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public override bool Equals(object other)
    {
        return AuthorName.Equals((other as TwitterEntry).AuthorName);
    }

    public ObjectId Id { get; set; }
    public string StatusId { get; set; }
    public DateTime Published { get; set; }
    public string Place { get; set; }
    public string Content { get; set; }
    public int RetweetsCount { get; set; }
    public string Source { get; set; }
    public string Name
    {
        get
        {
            return this.AuthorDisplayName + "(@" + this.AuthorName + ")";
        }
    }
    public string AuthorName { get; set; }
    public string AuthorDisplayName { get; set; }
    public int PersonTweetsCount { get; set; }
    public int PersonFollowersCount { get; set; }
    public string TimeZone { get; set; }
    public string AuthorURI { get; set; }
    public LinqToTwitter.UserType UserType { get; set; }
}