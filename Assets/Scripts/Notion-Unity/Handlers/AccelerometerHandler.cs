using Newtonsoft.Json;
using System.Text;
using UnityEngine;

namespace Notion.Unity
{
    public class AccelerometerHandler : IMetricHandler
    {
        public Metrics Metric => Metrics.Accelerometer;
        public string Label => string.Empty;

        //DZ
        public VFXUpdateSDF vfx;

        private readonly StringBuilder _builder;

        public AccelerometerHandler()
        {
            _builder = new StringBuilder();
        }

        public void Handle(string metricData)
        {
            Accelerometer accelerometer = JsonConvert.DeserializeObject<Accelerometer>(metricData);

            _builder.AppendLine("Handling Accelerometer")
                .Append("X: ").AppendLine(accelerometer.X.ToString())
                .Append("Y: ").AppendLine(accelerometer.Y.ToString())
                .Append("Z: ").AppendLine(accelerometer.Z.ToString());

            Debug.Log(_builder.ToString());
            _builder.Clear();

            DataToVFX(accelerometer);
        }

        private void DataToVFX(Accelerometer accel)
        {
            Vector3 accelVector = new Vector3(0, 0, accel.Z*3f);
            vfx.AccelForce(accelVector);
        }
    }
}