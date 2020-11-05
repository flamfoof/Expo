using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;

public class APIHandler : MonoBehaviour
{
    [SerializeField]
    private LoginResponse loginResponse = new LoginResponse();

    [HideInInspector]
    public string accessToken = string.Empty;

    [HideInInspector]
    public string accessEmail , accessDate , accessHorainicio , accessHorafin = string.Empty;

    async void Start()
    {
        //accessToken = await Login();

        Debug.Log("Time now " + DateTime.Now);
    }


    public async Task<string> Login(string secretKey = "Z4d2Vvcg9bCRafwFhr31H", int projectId = 1 , String key = "project_secretkey")
    {
        List<IMultipartFormSection> FormData = new List<IMultipartFormSection>();
        //FormData.Add(new MultipartFormDataSection("email", Email));
        //FormData.Add(new MultipartFormDataSection("password", Password));

        UnityWebRequest www = UnityWebRequest.Post("https://weignite.it/api/project/"+ projectId + "/login", FormData);

        byte[] bytesToEncode = Encoding.UTF8.GetBytes(key + ":" + secretKey);

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Basic '." + Convert.ToBase64String(bytesToEncode));

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Login Error : " + www.error);
            return null;
        }
        else
        {
            loginResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

            if (loginResponse.status == 200)
            {
                Debug.Log("Login Success");
                return loginResponse.tokenAccess;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public async Task Access(string accessToken , int projectId = 1, String key = "project_accesstoken")
    {
        List<IMultipartFormSection> FormData = new List<IMultipartFormSection>();
        FormData.Add(new MultipartFormDataSection("access_email", accessEmail));
        FormData.Add(new MultipartFormDataSection("access_date", accessDate));
        FormData.Add(new MultipartFormDataSection("access_horainicio", accessHorainicio));
        FormData.Add(new MultipartFormDataSection("access_horafin", accessHorainicio));


        UnityWebRequest www = UnityWebRequest.Post("https://weignite.it/api/project/" + projectId + "/access", FormData);

        byte[] bytesToEncode = Encoding.UTF8.GetBytes(key + ":" + accessToken);

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Basic '." + Convert.ToBase64String(bytesToEncode));

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Access Error : " + www.error);
        }
        else
        {
            /// success
        }
    }

    public async Task Actions(string accessToken, int projectId = 1, String key = "project_accesstoken")
    {
        List<IMultipartFormSection> FormData = new List<IMultipartFormSection>();
        FormData.Add(new MultipartFormDataSection("action_email", accessEmail));
        FormData.Add(new MultipartFormDataSection("action_type", accessDate));
        FormData.Add(new MultipartFormDataSection("action_data", accessHorainicio));
        FormData.Add(new MultipartFormDataSection("action_date", accessHorafin));


        UnityWebRequest www = UnityWebRequest.Post("https://weignite.it/api/project/" + projectId + "/action", FormData);

        byte[] bytesToEncode = Encoding.UTF8.GetBytes(key + ":" + accessToken);

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Basic '." + Convert.ToBase64String(bytesToEncode));

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Action Error : " + www.error);
        }
        else
        {
            /// success
        }
    }   

}
