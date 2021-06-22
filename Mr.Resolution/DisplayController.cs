using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Mr._Resolution
{
    public class DisplayController : EmbedIO.WebApi.WebApiController
    {
        enum GPU
        {
            Nvidia,
            AMD,
            Unknown
        }

        static GPU GetGPUType()
        {
            // Don't judge me
            try
            {
                NvAPIWrapper.NVIDIA.Initialize();
                return GPU.Nvidia;
            }
            catch
            {
                // Do AMD Stuff
            }

            return GPU.Unknown;
        }

        void SetCustomResolution(uint width, uint height, uint frequency = 59)
        {
            var gpu = GetGPUType();

            if (gpu == GPU.Nvidia)
            {
                var displays = NvAPIWrapper.Display.Display.GetDisplays();

                if (displays.Length == 0)
                {
                    throw new Exception("No displays found");
                }

                var display = displays[0];

                var timings = display.DisplayDevice.CalculateTiming(width, height, frequency, false);
                var customResolution = new NvAPIWrapper.Display.CustomResolution(width, height, (uint)32, timings) { };

                display.DisplayDevice.TrialCustomResolution(customResolution);
            }

        }

        [Route(HttpVerbs.Post, "/resolution")]
        public Task PostData([FormData] NameValueCollection data)
        {
            uint width = Convert.ToUInt32(data["width"]);
            uint height = Convert.ToUInt32(data["height"]);

            Console.WriteLine($"Adjusting the resolution to {width}x{height}");

            this.SetCustomResolution(width, height);

            return Task.CompletedTask;
        }
    }
}
