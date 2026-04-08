using Mapster;
using QuartzAPI.Models;
using QuartzData;
using QuartzData.Entities;

namespace QuartzAPI.Utilities
{
    public static class MapsterConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<Job, JobDto>
                .NewConfig()
                .Map(dest => dest.JobType, src => (JobType)src.JobType);

            TypeAdapterConfig<JobDto, Job>
                .NewConfig()
                .Map(dest => dest.JobType, src => (int)src.JobType);

            TypeAdapterConfig<APICallHistory, APICallHistoryDto>
                .NewConfig();


            TypeAdapterConfig<APICallHistoryDto, APICallHistory>
                .NewConfig();

        }
    }
}
