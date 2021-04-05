using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using System.Text;

namespace SpaceX
{
    public class SpaceXManager : MonoBehaviour
    {
        public Transform LaunchesContainer;
        public Transform ShipsContainer;
        public GameObject LaunchesPrefab;
        public GameObject ShipDataPrefab;
        public GameObject LoadingAnimation;
        public string UrlLaunches= "https://api.spacexdata.com/v3/launches?filter=ships,mission_name,upcoming,rocket/(rocket_id,rocket_name,second_stage/payloads/payload_id)";
        public string UrlRockets = "https://api.spacexdata.com/v3/rockets/";
        public string UrlShips = "https://api.spacexdata.com/v3/ships/";
        public Sprite LaunchedSprite;
        public CanvasGroup PopupWindow;
        public Camera cam;

        LaunchesPooling launchesPool;
        ShipsPooling shipsPool;
        Transform[] shipsData;
        RectTransform rectPopupWindow;

        void Start()
        {
            launchesPool = GetComponent<LaunchesPooling>();
            shipsPool = GetComponent<ShipsPooling>();
            shipsData = ShipsContainer.GetComponentsInChildren<Transform>();
            LoadLaunches();
        }

        //Show ship popup window
        void ShipPopup(string[] ships)
        {
            //Make the ship popup window opaque
            PopupWindow.alpha = 1f;
            PopupWindow.transform.position = Input.GetTouch(0).position;
            //If there is no ship info show one file with empty data
            if (ships.Length==0)
            {
                GameObject shipDataObj = shipsPool.ObjectPoolArray[0];
                shipDataObj.SetActive(true);
                shipDataObj.transform.SetParent(ShipsContainer, false);
                ShipData shipData = shipDataObj.GetComponent<ShipData>();
                shipData.ShipName.text = "";
                shipData.ShipType.text = "";
                shipData.HomePort.text = "";
                shipData.ShowImageText.text = "";
                shipData.ShipMissionsNumber.text = "";
                shipData.ShowImage.enabled = false;
                shipData.ShowImageButton.onClick.AddListener(() => { });
                for (int i = 1; i < shipsPool.ObjectPoolArray.Length; i++)
                {
                    shipsPool.ObjectPoolArray[i].gameObject.SetActive(false);
                }
            }
            else
            {
                //Show all ships using pooling
                int nextShip = 0;
                for (int i = 0; i < ships.Length; i++)
                {
                    StringBuilder sb = new StringBuilder(UrlShips);
                    sb.Append(ships[i]);
                    sb.Append("?filter=ship_name,ship_type,home_port,missions,image");
                    StartCoroutine(WebRequestHelper.GetShipsJson(sb.ToString(), (shipsJson) =>
                    {
                        if (string.IsNullOrEmpty(shipsJson.ship_name))
                        {
                            Debug.LogError("Server did not give ship info");
                            return;
                        }
                        GameObject shipDataObj = shipsPool.ObjectPoolArray[nextShip];
                        shipDataObj.SetActive(true);
                        shipDataObj.transform.SetParent(ShipsContainer, false);
                        ShipData shipData = shipDataObj.GetComponent<ShipData>();
                        shipData.ShipName.text = shipsJson.ship_name;
                        shipData.ShipType.text = shipsJson.ship_type;
                        shipData.HomePort.text = shipsJson.home_port;

                        if (!string.IsNullOrEmpty(shipsJson.image))
                        {
                            shipData.ShowImageButton.onClick.AddListener(() => ShowShipImage(shipsJson.image, shipData.ShowImage));
                            shipData.ShowImageText.text = "Show Image";
                        }
                        else
                        {
                            shipData.ShowImageText.text = "No Image";
                            Debug.LogError("Ship has no image available");
                        }
                        shipData.ShipMissionsNumber.text = shipsJson.missions.Length.ToString();
                        nextShip++;
                    }));
                }
                for (int i = ships.Length; i < shipsPool.ObjectPoolArray.Length; i++)
                {
                    shipsPool.ObjectPoolArray[i].gameObject.SetActive(false);
                }
            }

        }

        //Show ship image when click button
        void ShowShipImage(string imageUrl, RawImage shipImage)
        {
            StartCoroutine(WebRequestHelper.GetTexture(imageUrl, (returnedImage) =>
            {
                if (returnedImage==null)
                {
                    Debug.LogError("Server did not give ship image");
                    return;
                }
                shipImage.enabled = true;
                shipImage.texture = returnedImage;
                //shipImage.sprite = Sprite.Create(returnedImage as Texture2D, new Rect(0.0f, 0.0f, returnedImage.width, returnedImage.height), new Vector2(0.5f, 0.5f)); ;
            }));
        }

        void LoadLaunches()
        {
            StartCoroutine(WebRequestHelper.GetLaunchesJson(UrlLaunches, (arrLaunches) =>
            {
                if(arrLaunches.launches == null)
                {
                    Debug.LogError("Server did not give any array of launches");
                    return;
                }
                foreach (JsonLaunches launch in arrLaunches.launches)
                {
                    GameObject launchObj = launchesPool.GetAvailableObjectInPool();
                    if (launchObj == null)
                    {
                        launchObj = Instantiate(LaunchesPrefab, Vector3.zero, Quaternion.identity);
                    }
                    launchObj.transform.SetParent(LaunchesContainer,false);
                    if (launchObj.GetComponent<LaunchData>())
                    {
                        LaunchData data = launchObj.GetComponent<LaunchData>();
                        data.MissionName.text = launch.mission_name;
                        data.PayloadsNumber.text = launch.rocket.second_stage.payloads.Length.ToString();
                        data.RocketName.text = launch.rocket.rocket_name;
                        data.Launched.enabled = launch.upcoming;
                        StringBuilder sb = new StringBuilder(UrlRockets);
                        sb.Append(launch.rocket.rocket_id);
                        sb.Append("?filter=country");
                        //Show the popup window with the ship information
                        if(launchObj.GetComponent<Button>()) launchObj.GetComponent<Button>().onClick.AddListener(() => ShipPopup(launch.ships));
                        StartCoroutine(WebRequestHelper.GetRocketsJson(sb.ToString(), (countryObj) =>
                        {
                            if (countryObj == null)
                            {
                                Debug.LogError("Server did not give any data of rockets");
                                return;
                            }
                            data.RocketCountryName.text = countryObj.country;
                        }));
                    }
                }
            }));
        }
    }
}
