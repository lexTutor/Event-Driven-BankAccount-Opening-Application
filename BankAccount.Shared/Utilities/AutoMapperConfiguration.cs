using AutoMapper;
using BankAccount.Shared.Domain.Entities;
using static BankAccount.Shared.Domain.RecordTypes;

namespace BankAccount.Shared.Utilities
{
    public class AutoMapperConfiguration
    {
        public static IMapper Current { get; } = ConfigureMappings();

        public static IMapper ConfigureMappings()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PotentialMemberPayload, PotentialMember>().ReverseMap();
                cfg.CreateMap<CreateAccountPayload, Account>().ReverseMap();
            });

            return config.CreateMapper();
        }
    }
}
