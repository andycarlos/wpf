using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wpf.Models;

namespace wpf.Mapper
{
    public class GeneralProfile: Profile
    {
        public GeneralProfile()
        {
            CreateMap<Models.Customer.QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRet, IQbCustomer>()
                .ForMember(dest => dest.SaleRepListID, orig => orig.MapFrom(x=>x.SalesRepRef.ListID));

            CreateMap<Models.Customer.QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipToAddress, ShipToAddress>();
        }
    }
}
