/*
 * This script is modified based on the Neurosity Notion SDK Unity by ryanturney.
 * This script takes the raw brainwave data and calls the Mesh manipulator script
 * to form the mesh in real-time
 * 
*/

using Newtonsoft.Json;
using System.Text;
using UnityEngine;

namespace Notion.Unity
{
    public class BrainwavesRawToMeshHandler : IMetricHandler
    {
        public Metrics Metric => Metrics.Brainwaves;
        public string Label => "raw";

        private readonly StringBuilder _builder;

        public BrainwavesRawToMeshHandler()
        {
            _builder = new StringBuilder();
        }

        public void Handle(string metricData)
        {
            Epoch epoch = JsonConvert.DeserializeObject<Epoch>(metricData);

            _builder.AppendLine("Handling Raw Brainwaves To Mesh")
                .Append("Label: ").AppendLine(epoch.Label)
                .Append("Notch Frequency: ").AppendLine(epoch.Info.NotchFrequency)
                .Append("Sampling Rate: ").AppendLine(epoch.Info.SamplingRate.ToString())
                .Append("Star Time: ").AppendLine(epoch.Info.StartTime.ToString())
                .Append("Channel Names: ").AppendLine(string.Join(", ", epoch.Info.ChannelNames));

            Debug.Log(_builder.ToString());
            _builder.Clear();

            //PrintData(epoch.Data);
        }

        private void PrintData(decimal[][] arr)
        {
            foreach (decimal[] item in arr)
            {
                foreach (decimal dat in item)
                {
                    Debug.Log(dat.ToString());
                }
            }
        }
    }
}