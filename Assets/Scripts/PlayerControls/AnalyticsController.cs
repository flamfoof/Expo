using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using WeIgnite;

public class AnalyticsController : SingletonClass<AnalyticsController>
{
    //Profile and Profile Information
    public void ProfileInfoAnalytics()
    {
        Analytics.CustomEvent("profile_info", new Dictionary<string, object>
        {
            { "name", PlayerPrefs.GetString("Name") },
            { "organization", PlayerPrefs.GetString("Organization")},
            { "email",  PlayerPrefs.GetString("Email")},
            { "password",PlayerPrefs.GetString("Password")}
        });
    }
    //Average Time Spent
    public void AverageTimeSpent(float timespent)
    {
        Analytics.CustomEvent("average_time_spent", new Dictionary<string, object>
        {
            { "timeSpent", timespent },
        });
    }
    //Number of Attendees
    public void AttendesNumber(int attendeeNumber)
    {
        Analytics.CustomEvent("number_of_attendees", new Dictionary<string, object>
        {
            { "attendeeNumber", attendeeNumber },
        });
    }
    //Website Click and what website it was
    public void WebsiteClick(string websiteUrl)
    {
        Analytics.CustomEvent("website_click", new Dictionary<string, object>
        {
            { "websiteClick",websiteUrl },
        });
    }
}
