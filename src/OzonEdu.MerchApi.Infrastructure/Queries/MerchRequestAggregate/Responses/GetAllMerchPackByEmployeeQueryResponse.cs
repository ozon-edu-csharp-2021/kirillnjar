﻿using System.Collections.Generic;
using OzonEdu.MerchApi.Infrastructure.Models;

namespace OzonEdu.MerchApi.Infrastructure.Queries.MerchRequestAggregate.Responses
{
    public class GetAllMerchPackByEmployeeQueryResponse
    {
        public IReadOnlyList<GivenMerchPackDTO> Items { get; set; }
    }
}