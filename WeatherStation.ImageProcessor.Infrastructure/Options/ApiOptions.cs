using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.ImageProcessor.Infrastructure.Options
{
    public record ApiOptions
    {
        public const string SectionName = "Api";
        public required bool IncludeDetailedErrors { get; init; } = false;
    }
}
