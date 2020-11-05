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

