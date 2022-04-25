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

        //DZ
        //remapping upper and lower bounds
        private float _upper = 1f;
        private float _lower = 0;

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

                for (int i = 0; i < dt.Delta.Length; i++)
                {
                    //remap per channel power value to range of 0-1
                    //because the upper bound is unknown, it is continuously updated
                    float fValue = (float)dt.Delta[i];
                    if (fValue > _upper)
                    {
                        _upper = fValue;
                    }
                    float mappedValue = Remap(fValue, _lower, _upper, 0f, 1f);

                    //send mapped value and index (corresp. to vertex index) to DreamMesh.cs


                }

                
            }
        }

        static float Remap(float val, float in1, float in2, float out1, float out2)
        {
            return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
        }
    }
}