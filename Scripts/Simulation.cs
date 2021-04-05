using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RG.OrbitalElements;
using System.IO;
using System.Linq;
using System;
using System.Text;
using UnityEngine.UI;

namespace SpaceX
{
    public class Simulation : MonoBehaviour
    {
        public Transform Tesla;
        public float TeslaSpeed=5f;
        public string DataPath = "Roadster";
        public int NumberOfConnectingLines = 20;
        public float DivideOrbitalDistanceBy = 10000000000000000000000000000000f;
        public Text Epoch;
        public Text LocalDate;
        public Text SemiMajorAxis;
        public Text Eccentricity;
        public Text Inclination;
        public Text LongitudeOfAsc;
        public Text PeriapsisDegres;
        public Text MeanAnomalyDegrees;
        public Text TrueAnomalyDegrees;

        private string textFilePath;
        private string[] teslaDataArray;
        private List<string> teslaDataList = new List<string>();
        private float lastMovementTime;
        private int dataIndex;
        private string[] orbitalData;
        private string[][] jaggedOrbitalData;
        private LineRenderer lineRenderer;
        private Vector3 actualTeslaPosition;
        private Vector3 nextTeslaPosition;
        private bool SimulationStarted;

        [HideInInspector]
        public List<Vector3> teslaPositions = new List<Vector3>();

        private void Awake()
        {
            Tesla.gameObject.SetActive(false);
        }

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();

            //Create an array from given csv
            TextAsset csvData = Resources.Load<TextAsset>(DataPath);

            using (StreamReader reader = new StreamReader(new MemoryStream(csvData.bytes)))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    teslaDataList.Add(reader.ReadLine());
                }
            }

            teslaDataArray = teslaDataList.Skip(1).ToArray();

            //Create jagged array for orbital data and order by date
            jaggedOrbitalData = new string[teslaDataArray.Length][];
            for (int i = 0; i < teslaDataArray.Length; i++)
            {
                jaggedOrbitalData[i] = teslaDataArray[i].Split(',');
                jaggedOrbitalData[i] = jaggedOrbitalData[i].OrderBy(p => DateTime.Parse(jaggedOrbitalData[i][1])).ToArray();
            }
        }

        void Update()
        {
            int nextDateIndex = dataIndex + 1;
            if (nextDateIndex > teslaDataArray.Length - 1) nextDateIndex = 0;
            DateTime actualTime = DateTime.Parse(jaggedOrbitalData[dataIndex][1]);
            DateTime nextTime = DateTime.Parse(jaggedOrbitalData[nextDateIndex][1]);
            double orbitalTime = actualTime.Subtract(nextTime).TotalSeconds * -1f;

            //One second is 24h in simulation
            float orbitalSpeed =(float)(orbitalTime / 86400);
            if (Time.time > lastMovementTime + orbitalSpeed || nextDateIndex == 0 || dataIndex==0)
            {
                orbitalData = jaggedOrbitalData[dataIndex];

                Epoch.text = orbitalData[0];
                LocalDate.text = actualTime.ToLocalTime().ToString();
                SemiMajorAxis.text = orbitalData[2];
                Eccentricity.text = orbitalData[3];
                Inclination.text = orbitalData[4];
                LongitudeOfAsc.text = orbitalData[5];
                PeriapsisDegres.text = orbitalData[6];
                MeanAnomalyDegrees.text = orbitalData[7];
                TrueAnomalyDegrees.text = orbitalData[8];

                actualTeslaPosition = Vector3DoublePosToVector3();
                Tesla.position = actualTeslaPosition;

                orbitalData = jaggedOrbitalData[dataIndex+1];
                nextTeslaPosition = Vector3DoublePosToVector3();

                //Show testla just when first position is set
                if (!SimulationStarted)
                {
                    SimulationStarted = true;
                    Tesla.gameObject.SetActive(true);
                }
                
                teslaPositions.Add(actualTeslaPosition);
                if (teslaPositions.Count > 20) teslaPositions.RemoveAt(0);
                
                //Create the linerendering
                for(int i=0;i<teslaPositions.Count;i++)
                {
                    lineRenderer.positionCount = i+1;
                    lineRenderer.SetPosition(i, teslaPositions[i]);
                }
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(teslaPositions.Count, nextTeslaPosition);

                dataIndex++;
                if (dataIndex > teslaDataArray.Length - 1) dataIndex = 0;
                lastMovementTime = Time.time;

            }

            Tesla.position = Vector3.Lerp(Tesla.position,nextTeslaPosition, Mathf.Clamp01(Time.time - lastMovementTime / orbitalSpeed));


            //Rotate the camera around the sun
            /*
            if (Input.GetMouseButton(0))
            {
                CameraMovement(mainCamera.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Moved);
            }
            */
        }


        //Get Vector3 from from Orbital data
        Vector3 Vector3DoublePosToVector3()
        {
            Vector3Double dpos = Calculations.CalculateOrbitalPosition(double.Parse(orbitalData[2]), double.Parse(orbitalData[3]), double.Parse(orbitalData[4]), double.Parse(orbitalData[5]), double.Parse(orbitalData[4]), double.Parse(orbitalData[6]));
            return new Vector3((float)dpos.x, (float)dpos.y, (float)dpos.z) / DivideOrbitalDistanceBy;
        }
    }
}
