using AutoMapper;
using BankAPI.Models;
using BankAPI.ViewModels;

namespace BankAPI.Profiles
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles() 
        {
            CreateMap<RegisterModel, Account>();
            CreateMap<Account, GetAccountModel>();
            CreateMap<TransactionModel, Transaction>();
        }
    }
}
