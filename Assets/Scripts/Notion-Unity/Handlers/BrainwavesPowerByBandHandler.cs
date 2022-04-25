using Newtonsoft.Json;
using System.Text;
using UnityEngine;

namespace Notion.Unity
{
    public class BrainwavesPowerByBandHandler : IMetricHandler
    {
        //DZ
        public DreamMesh dreamMesh;

        public Metrics Metric => Metrics.Brainwaves;
        public string Label => "powerByBand";

        private readonly StringBuilder _builder;

        public BrainwavesPowerByBandHandler()
        {
            _builder = new StringBuilder();
        }

        public void Handle(string metricData)
        {
            PowerByBand powerByBand = JsonConvert.DeserializeObject<PowerByBand>(metricData);

            _builder.AppendLine("Handling Power By Band Brainwaves")
                .Append("Label: ").AppendLine(powerByBand.Label)
                .Append("Has Alpha: ").AppendLine((powerByBand.Data.Alpha.Length > 0).ToString())
                .Append("Has Beta: ").AppendLine((powerByBand.Data.Beta.Length > 0).ToString())
                .Append("Has Delta: ").AppendLine((powerByBand.Data.Delta.Length > 0).ToString())
                .Append("Has Gamma: ").AppendLine((powerByBand.Data.Gamma.Length > 0).ToString())
                .Append("Has Theta: ").AppendLine((powerByBand.Data.Theta.Length > 0).ToString());

            Debug.Log(_builder.ToString());
            _builder.Clear();

            //PrintData(powerByBand.Data);
            DataToMesh(powerByBand.Data);
            
        }

        //DZ
        //for testing
        private void PrintData(PowerByBandData dt)
        {
            foreach (decimal item in dt.Delta)
            {
                Debug.Log(item);
            }
        }

        //DZ
        //for handling mesh deformation
        private void DataToMesh(PowerByBandData dt)
        {
            //check if dreamMesh is assigned by NotionTester
            if (dreamMesh != null)
            {
                //algorithm that maps power by band data to mesh parameters
                foreach (decimal item in dt.Delta)
                {
                    dreamMesh.MorphMesh(item);
                }

                
            }
        }
    }
}