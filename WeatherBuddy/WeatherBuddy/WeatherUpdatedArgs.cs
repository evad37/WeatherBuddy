using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherBuddy
{
    public delegate void WeatherUpdatedDelegate(WeatherUpdatedArgs e);
    public class WeatherUpdatedArgs : EventArgs
    {

        public DateTime updatedAt;

        public WeatherUpdatedArgs(DateTime updatedAt)

        {
            this.updatedAt = updatedAt;
        }
    }
}
