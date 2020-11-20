using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;

public class APIHandler : MonoBehaviour
{
    [SerializeField] string secretKey = "Z4d2Vvcg9bCRafwFhr31H";
    [SerializeField] int projectId = 1;
    [SerializeField] string accessToken = string.Empty;

    [SerializeField]
    private LoginResponse loginResponse = new LoginResponse();


    async void Start()
    {
        accessToken = await Login();
    }


    public async Task<string> Login()
    {
        List<IMultipartFormSection> FormData = new List<IMultipartFormSection>();

        UnityWebRequest www = UnityWebRequest.Post("https://weignite.it/api/project/"+ projectId + "/login", FormData);

        byte[] bytesToEncode = Encoding.UTF8.GetBytes("project_secretkey" + ":" + secretKey);

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

    public async Task Access(string accessEmail, string accessDate, string sessionStart, string sessionEnd)
    {
        List<IMultipartFormSection> FormData = new List<IMultipartFormSection>();

        AccessInsert accessData = new AccessInsert { 
            access_email = accessEmail,
            access_date = accessDate,
            access_horainicio = sessionStart,
            access_horafin = sessionEnd
        };

        byte[] rawJson = Encoding.UTF8.GetBytes(JsonUtility.ToJson(accessData));

        UnityWebRequest www = UnityWebRequest.Post("https://weignite.it/api/project/" + projectId + "/access", FormData);

        byte[] bytesToEncode = Encoding.UTF8.GetBytes("project_accesstoken" + ":" + accessToken);

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Basic '." + Convert.ToBase64String(bytesToEncode));
        www.uploadHandler = new UploadHandlerRaw(rawJson);

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Access Error : " + www.error);
        }
        else
        {
            Debug.Log("Access anlytics were synced " + www.downloadHandler.text);
        }
    }

    public async Task Actions(string accessEmail, string actionType, string actionData, string actionDate)
    {
        List<IMultipartFormSection> FormData = new List<IMultipartFormSection>();

        ActionInsert actionsData = new ActionInsert {
            action_email = accessEmail,
            action_type = actionType,
            action_data = actionData,
            action_date = actionDate
        };

        byte[] rawJson = Encoding.UTF8.GetBytes(JsonUtility.ToJson(actionsData));

        UnityWebRequest www = UnityWebRequest.Post("https://weignite.it/api/project/" + projectId + "/action", FormData);

        byte[] bytesToEncode = Encoding.UTF8.GetBytes("project_accesstoken" + ":" + accessToken);

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Basic '." + Convert.ToBase64String(bytesToEncode));
        www.uploadHandler = new UploadHandlerRaw(rawJson);

        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Action Error : " + www.error);
        }
        else
        {
            Debug.Log("Action anlytics were synced, data " + www.downloadHandler.text);
        }
    }
}               
