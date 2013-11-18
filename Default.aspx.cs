using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LinqToTwitter;
using MongoDB.Driver;
using System.Collections;

public partial class _Default : System.Web.UI.Page
{
    private SingleUserAuthorizer singleUserAuthorizer;

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnSearchTweets_Click(object sender, EventArgs e)
    {
        singleUserAuthorizer = new SingleUserAuthorizer//Instantiate new single user authorizer object
        {
            Credentials = new SingleUserInMemoryCredentials//Instantiating credentials object
            {
                //Get credential keys from the config file
                ConsumerKey =
                    ConfigurationManager.AppSettings["twitterConsumerKey"],
                ConsumerSecret =
                    ConfigurationManager.AppSettings["twitterConsumerSecret"],
                TwitterAccessToken =
                    ConfigurationManager.AppSettings["twitterAccessToken"],
                TwitterAccessTokenSecret =
                    ConfigurationManager.AppSettings["twitterAccessTokenSecret"]
            }
        };

        List<LinqToTwitter.Status> lstStatuses = new List<LinqToTwitter.Status>();
        List<TwitterEntry> lstTwitterEntries = new List<TwitterEntry>();
        using (var twitterCtx = new TwitterContext(singleUserAuthorizer))
        {
            //Search words   "#امطار_الرياض" || "#MongoDB" || "#mysql" || "#غرد_بذكر_الله";
            Search srch = twitterCtx.Search.Where(s => s.Type == SearchType.Search && s.Count == 100 && s.Query == tbQuery.Text).Single();
            lstStatuses = srch.Statuses.ToList();

            //while (lstStatuses.Count < 100)//To complete the tweets count to 100
            //{
            //    srch = twitterCtx.Search.Where(s => s.Type == SearchType.Search
            //        && s.MaxID == srch.Statuses[srch.Statuses.Count - 1].MaxID
            //        && s.Count == (100 - lstStatuses.Count) && s.Query == tbQuery.Text).Single();
            //    lstStatuses.AddRange(srch.Statuses.ToList());
            //}

            foreach (Status st in lstStatuses)
            {
                //Instantiating new Twitter Entry to add to the list
                lstTwitterEntries.Add(new TwitterEntry()
                {
                    StatusId = st.StatusID,
                    Published = DateTime.Parse(st.CreatedAt.ToString()),
                    Place = st.User.Location,
                    Content = st.Text,
                    RetweetsCount = st.RetweetCount,
                    Source = st.Source,
                    UserType = st.User.Type,
                    AuthorName = st.User.Identifier.ScreenName,
                    AuthorDisplayName = st.User.Name,
                    PersonTweetsCount = st.User.StatusesCount,
                    PersonFollowersCount = st.User.FollowersCount,
                    TimeZone = st.User.TimeZone,
                    AuthorURI = st.User.Url
                });
            }
        }

        SaveDB(lstTwitterEntries);

        ShowStatistics(lstTwitterEntries);

        //Viewing Tweets in the grid
        gvTweetsFromDB.DataSource = lstTwitterEntries;
        gvTweetsFromDB.DataBind();
    }

    private void ShowStatistics(List<TwitterEntry> lstTwitterEntries)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Statistics:<BR>");
        try
        {
            Dictionary<string, int> dicTop5 = null;
            #region    Top 5 Busy Hours

            sb.AppendLine("Top 5 Busy Hours: <BR>");
            dicTop5 = new Dictionary<string, int>();

            //Adding tweets-count corresponding to the hour in the dictionary
            //We should make a nother loop to divide them based on days and find out the most full day of tweets in the list
            for (int i = 0; i < lstTwitterEntries.Count; i++)
            {
                if (lstTwitterEntries[i].Published > DateTime.MinValue)
                {
                    if (!dicTop5.ContainsKey(lstTwitterEntries[i].Published.Hour.ToString()))
                    {
                        dicTop5.Add(lstTwitterEntries[i].Published.Hour.ToString(), 1);
                    }
                    else
                    {
                        dicTop5[lstTwitterEntries[i].Published.Hour.ToString()] = dicTop5[lstTwitterEntries[i].Published.Hour.ToString()] + 1;
                    }
                }
            }

            //Ordering the dictionary descending based on the value(tweets count)
            var result = dicTop5.OrderByDescending(c => c.Value); //or IEnumerable<KeyValuePair<int, string>> result1 = dicTop5.OrderByDescending(c => c.Value); 

            if (result.Count() > 4)
            {
                for (int i = 0; i < 5; i++)
                {
                    sb.AppendLine((i + 1) + "- Hour: " + result.ElementAt(i).Key + ", " + "Tweets count: " + result.ElementAt(i).Value + "<BR>");
                }
            }
            else
            {
                sb.AppendLine("Not enough data" + "<BR>");
            }

            sb.AppendLine("<BR>");

            #endregion Top 5 Busy Hours

            #region    Top 5 Top Sources

            sb.AppendLine("Top 5 Sources: <BR>");
            dicTop5.Clear();
            //Adding tweets-count corresponding to the source in the dictionary
            for (int i = 0; i < lstTwitterEntries.Count; i++)
            {
                if (!string.IsNullOrEmpty(lstTwitterEntries[i].Source))
                {
                    if (!dicTop5.ContainsKey(lstTwitterEntries[i].Source))
                    {
                        dicTop5.Add(lstTwitterEntries[i].Source, 1);
                    }
                    else
                    {
                        dicTop5[lstTwitterEntries[i].Source] = dicTop5[lstTwitterEntries[i].Source] + 1;
                    }
                }
            }

            //Ordering the dictionary descending based on the value (tweets count)
            result = dicTop5.OrderByDescending(c => c.Value);

            if (result.Count() > 4)
            {
                for (int i = 0; i < 5; i++)
                {
                    sb.AppendLine((i + 1) + "- Source: " + result.ElementAt(i).Key + ", " + "Tweets count: " + result.ElementAt(i).Value + "<BR>");
                }
            }
            else
            {
                sb.AppendLine("Not enough data" + "<BR>");
            }

            sb.AppendLine("<BR>");
            #endregion Top 5 Top Sources

            #region    Top Places

            sb.AppendLine("Top 5 Places: <BR>");
            dicTop5.Clear();
            //Adding tweets-count corresponding to the place in the dictionary
            for (int i = 0; i < lstTwitterEntries.Count; i++)
            {
                if (!string.IsNullOrEmpty(lstTwitterEntries[i].Place))
                {
                    if (!dicTop5.ContainsKey(lstTwitterEntries[i].Place))
                    {
                        dicTop5.Add(lstTwitterEntries[i].Place, 1);
                    }
                    else
                    {
                        dicTop5[lstTwitterEntries[i].Place] = dicTop5[lstTwitterEntries[i].Place] + 1;
                    }
                }
            }

            //Ordering the dictionary descending based on the value (tweets count)
            result = dicTop5.OrderByDescending(c => c.Value);

            if (result.Count() > 4)
            {
                for (int i = 0; i < 5; i++)
                {
                    sb.AppendLine((i + 1) + "- Place: " + result.ElementAt(i).Key + ", " + "Tweets count: " + result.ElementAt(i).Value + "<BR>");
                }
            }
            else
            {
                sb.AppendLine("Not enough data" + "<BR>");
            }

            sb.AppendLine("<BR>");
            #endregion Top Places

            #region    Top TimeZones

            sb.AppendLine("Top 5 TimeZones: " + "<BR>");
            dicTop5.Clear();
            //Adding tweets-count corresponding to the TimeZone in the dictionary
            for (int i = 0; i < lstTwitterEntries.Count; i++)
            {
                if (!string.IsNullOrEmpty(lstTwitterEntries[i].TimeZone))
                {
                    if (!dicTop5.ContainsKey(lstTwitterEntries[i].TimeZone))
                    {
                        dicTop5.Add(lstTwitterEntries[i].TimeZone, 1);
                    }
                    else
                    {
                        dicTop5[lstTwitterEntries[i].TimeZone] = dicTop5[lstTwitterEntries[i].TimeZone] + 1;
                    }
                }
            }

            //Ordering the dictionary descending based on the value (tweets count)
            result = dicTop5.OrderByDescending(c => c.Value);

            if (result.Count() > 4)
            {
                for (int i = 0; i < 5; i++)
                {
                    sb.AppendLine((i + 1) + "- TimeZone: " + result.ElementAt(i).Key + ", " + "Tweets count: " + result.ElementAt(i).Value + "<BR>");
                }
            }
            else
            {
                sb.AppendLine("Not enough data" + "<BR>");
            }

            sb.AppendLine("<BR>");
            #endregion Top TimeZones

            #region Order by Retweets Count

            sb.AppendLine("Top 5 Retweets: " + "<BR>");
            //Ordering by Retweets Count
            lstTwitterEntries.Sort((x, y) =>
            {
                return y.RetweetsCount.CompareTo(x.RetweetsCount);
            });
            if (lstTwitterEntries.Count > 4)
            {
                for (int i = 0; i < 5; i++)
                {
                    sb.AppendLine((i + 1) + "- User: @" + lstTwitterEntries[i].AuthorName + ", Retweets: " + lstTwitterEntries[i].RetweetsCount + "<BR>");
                    sb.AppendLine("Tweet: " + lstTwitterEntries[i].Content + "<BR>");
                    sb.AppendLine("-------------------------------" + "<BR>");
                }
            }
            else
            {
                sb.AppendLine("Not enough data" + "<BR>");
            }
            sb.AppendLine("<BR>");

            #endregion Order by Retweets Count

            #region Order by followers count

            sb.AppendLine("Top 5 Followed: " + "<BR>");
            //Ordering by followers Count
            lstTwitterEntries.Sort((x, y) =>
            {
                return y.PersonFollowersCount.CompareTo(x.PersonFollowersCount);
            });
            if (lstTwitterEntries.Count > 4)
            {
                for (int i = 0; i < 5; i++)
                {
                    sb.AppendLine((i + 1) + "- User: @" + lstTwitterEntries[i].AuthorName + ", Followers: " + lstTwitterEntries[i].PersonFollowersCount + "<BR>");
                }
            }
            else
            {
                sb.AppendLine("Not enough data" + "<BR>");
            }
            sb.AppendLine("<BR>");

            #endregion Order by followers count
        }
        catch (Exception ex)
        {
            //Write Error logging code here;
        }

        lblStatistics.Text = sb.ToString();
    }

    protected void btnGetTweetsFromDB_Click(object sender, EventArgs e)
    {
        List<TwitterEntry> lstTwitterEntries = ReadDB();

        ShowStatistics(lstTwitterEntries);

        //Viewing Tweets in the grid
        gvTweetsFromDB.DataSource = lstTwitterEntries;
        gvTweetsFromDB.DataBind();
    }

    protected void SaveDB(List<TwitterEntry> lstTwitterEntries)
    {
        try
        {
            //Save MongoDB code here
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            server.Connect();//This line could be removed, still could work
            var database = server.GetDatabase("DotNetBigDataDB"); // "DotNetBigDataDB" is the name of the database
            var collection = database.GetCollection<TwitterEntry>("TwitterEntries");
            collection.RemoveAll();
            collection.InsertBatch<TwitterEntry>(lstTwitterEntries);
        }
        catch (Exception ex)
        {
            //Write Error logging code here;
        }

    }

    protected List<TwitterEntry> ReadDB()
    {
        List<TwitterEntry> lstTwitterEntries = null;
        try
        {
            //Read MongoDB code here
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("DotNetBigDataDB"); // "DotNetBigDataDB" is the name of the database

            MongoCollection<TwitterEntry> collection = database.GetCollection<TwitterEntry>("TwitterEntries");
            MongoCursor<TwitterEntry> cur = collection.FindAll();
            lstTwitterEntries = cur.ToList();
        }
        catch (Exception ex)
        {
            //Write Error logging code here;
        }
        return lstTwitterEntries;
    }
}
