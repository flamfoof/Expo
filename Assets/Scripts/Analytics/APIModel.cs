using System;
using System.Collections;
using System.Collections.Generic;

public class APIModel
{

}

[Serializable]
public class LoginResponse
{
    public int status;
    public string tokenAccess;
    public int expiration;
}

[Serializable]
public class AccessInsert
{
    public string access_email;
    public string access_date;
    public string access_horainicio;
    public string access_horafin;
}

[Serializable]
public class AuthenticateInsert
{
    public string user_email;
    public string user_password;
}

[Serializable]
public class ActionInsert
{
    public string action_email;
    public string action_type;
    public string action_data;
    public string action_date;
}

/// <summary>
//    User Authentication
/// </summary>

[Serializable]
public class User
{
    public string FirstName;
    public string LastName;
    public string image;
    public string email;
}

[Serializable]
public class Messages
{
    public string userStatus;
    public User user; 
    }

[Serializable]
public class AuthenticateUser
{
    public int status;
    public string code;
    public Messages messages;
}