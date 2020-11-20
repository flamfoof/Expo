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
public class ActionInsert
{
    public string action_email;
    public string action_type;
    public string action_data;
    public string action_date;
}