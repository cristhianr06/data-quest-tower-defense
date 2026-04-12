// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo
{
    /// <summary>
    /// Monitors and displays the Frames Per Second (FPS) statistics in real-time, including mean, minimum, and maximum values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Fps"/> class calculates and displays the mean, minimum, and maximum FPS values over a specified history size.
    /// </para>
    /// </remarks>
    public class Fps : MonoBehaviour
    {
        /// <summary>
        /// The text to display the mean FPS.
        /// </summary>
        public UnityEngine.UI.Text MeanText;

        /// <summary>
        /// The text to display the min FPS.
        /// </summary>
        public UnityEngine.UI.Text MinText;

        /// <summary>
        /// The text to display the max FPS.
        /// </summary>
        public UnityEngine.UI.Text MaxText;

        /// <summary>
        /// The count of last read values, used to calculate min/mean/max values.
        /// </summary>
        private int historySize = 50;

        /// <summary>
        /// List of the last 'historySize' performance values.
        /// </summary>
        private float[] values = new float[0];

        /// <summary>
        /// The minimum value of the last 'historySize' values.
        /// </summary>
        private float valueMin = 0;

        /// <summary>
        /// The mean value of the last 'historySize' values.
        /// </summary>
        private float valueMean = 0;

        /// <summary>
        /// The maximum value of the last 'historySize' values.
        /// </summary>
        private float valueMax = 0;

        /// <summary>
        /// The last refresh time for the min and max values.
        /// </summary>
        private float refreshTime = 0;

        private void Awake()
        {
            // Initialize the value list.
            this.values = new float[this.historySize];
        }

        private void Update()
        {
            // Calculate the FPS and add to the list.
            this.AddValue(1f / Time.unscaledDeltaTime);
        }

        private void AddValue(float _Value)
        {
            // Store the min and max values.
            float var_MinValue = float.MaxValue;
            float var_MaxValue = 0;

            // Store the mean value.
            float var_MeanValue = 0;
            int var_MeanCounter = 0;

            // Push new values to the end of the array and shift the array to the left.
            for (int i = 0; i < this.historySize; i++)
            {
                // Push to the left.
                if (i < this.historySize - 1)
                {
                    this.values[i] = this.values[i + 1];
                }
                // Store the current value.
                else
                {
                    this.values[i] = _Value;
                }

                // Find the min and max value.
                if (this.values[i] < var_MinValue)
                {
                    var_MinValue = this.values[i];
                }
                if (this.values[i] > var_MaxValue)
                {
                    var_MaxValue = this.values[i];
                }

                // Increase the mean value, if the value is greater than zero.
                if (this.values[i] > 0)
                {
                    var_MeanValue += this.values[i];
                    var_MeanCounter += 1;
                }
            }


            // Find the mean value.
            this.valueMean = var_MeanCounter > 0 ? var_MeanValue / var_MeanCounter : 0;

            // Find the min and max value.
            this.valueMin = var_MinValue < this.valueMin ? var_MinValue : this.valueMin;
            this.valueMax = var_MaxValue > this.valueMax ? var_MaxValue : this.valueMax;

            // Min and max are also time dependent.
            if (Time.time - this.refreshTime > 20f)
            {
                this.valueMin = var_MinValue;
                this.valueMax = var_MaxValue;
                this.refreshTime = Time.time;
            }
        }

        private void FixedUpdate()
        {
            // Update the mean text.
            if (this.MeanText != null)
            {
                this.MeanText.text = string.Format("FPS: {0:0}", this.valueMean);
            }

            // Update the min text.
            if (this.MinText != null)
            {
                this.MinText.text = string.Format("Min: {0:0}", this.valueMin);
            }

            // Update the max text.
            if (this.MaxText != null)
            {
                this.MaxText.text = string.Format("Max: {0:0}", this.valueMax);
            }
        }
    }
}
