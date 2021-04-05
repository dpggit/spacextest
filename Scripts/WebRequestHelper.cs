using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;

namespace SpaceX
{
    [Serializable]
    public class JsonLaunches
    {
        public string mission_name;
        public bool upcoming;
        public JsonLaunchesRocket rocket;
        public string[] ships;
    }

    [Serializable]
    public class ArrLaunches
    {
        public JsonLaunches[] launches;
    }

    [Serializable]
    public class JsonLaunchesRocket
    {
        public string rocket_id;
        public string rocket_name;
        public JsonRocketSecondStage second_stage;
    }

    [Serializable]
    public class JsonRocketSecondStage
    {
        public JsonRocketSecondStagePayloads[] payloads;
    }

    [Serializable]
    public class JsonRocketSecondStagePayloads
    {
        public string payload_id;
    }

    [Serializable]
    public class JsonRocketsName
    {
        public string country;
    }

    [Serializable]
    public class JsonShips
    {
        public string ship_name;
        public string ship_type;
        public string home_port;
        public ShipMission[] missions;
        public string image;
        public JsonShips(string shipName, string shipType, string homePort, ShipMission[] missionsParam, string shipImage)
        {
            ship_name = shipName;
            ship_type = shipType;
            home_port = homePort;
            missions = missionsParam;
            image = shipImage;
        }
    }

    [Serializable]
    public class ShipMission
    {
        public string name;
        public string flight;
    }


    //Class to get or set the different operations in server
    public static class WebRequestHelper 
    {
        public static IEnumerator GetLaunchesJson(string url, System.Action<ArrLaunches> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    callback(new ArrLaunches());
                }
                else
                {
                    StringBuilder sb = new StringBuilder("{\"launches\":");
                    sb.Append(www.downloadHandler.text);
                    sb.Append("}");
                    callback(JsonUtility.FromJson<ArrLaunches>(sb.ToString()));
                }
            }
        }

        public static IEnumerator GetRocketsJson(string url, System.Action<JsonRocketsName> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    callback(new JsonRocketsName());
                }
                else
                {
                    callback(JsonUtility.FromJson<JsonRocketsName>(www.downloadHandler.text));
                }
            }
        }

        public static IEnumerator GetShipsJson(string url, System.Action<JsonShips> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    callback(new JsonShips("","","",new ShipMission[0],""));
                }
                else
                {
                    callback(JsonUtility.FromJson<JsonShips>(www.downloadHandler.text));
                }
            }
        }

        public static IEnumerator GetCSV(string url, System.Action<byte[]> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    callback(new byte[0]);
                }
                else
                {
                    callback(www.downloadHandler.data);
                }
            }
        }

        public static IEnumerator GetTexture(string url, System.Action<Texture> callback)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                callback(null);
            }
            else
            {
                callback(((DownloadHandlerTexture)www.downloadHandler).texture);
            }
        }
    }
}
