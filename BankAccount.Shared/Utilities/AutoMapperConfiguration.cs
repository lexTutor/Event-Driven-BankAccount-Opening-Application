using AutoMapper;
using BankAccount.Shared.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
