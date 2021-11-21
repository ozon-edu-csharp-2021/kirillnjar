﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchApi.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchApi.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchApi.Infrastructure.Repositories.Implementation.Mock
{
    public class MerchRequestMockRepository : IMerchRequestRepository
    {
        private IReadOnlyList<MerchRequest> _merchRequests;

        public MerchRequestMockRepository(IReadOnlyList<MerchRequest> merchRequests)
        {
            _merchRequests = merchRequests;
        }

        public MerchRequestMockRepository()
        {
            _merchRequests = new List<MerchRequest>
            {
                new(1, new Employee(
                        new Email("iivanov@mail.com"),
                        new EmployeeFullName("Ivan", "Ivanov", "Ivanovich")),
                    1,
                    new MerchRequestDateTime(DateTime.ParseExact("21.12.2020", "dd.MM.yyyy", CultureInfo.InvariantCulture)),
                    MerchRequestStatus.Done,
                    new MerchRequestFrom(MerchRequestFromType.Automatically)),
                new(2, new Employee(
                        new Email("ppetrov@mail.com"),
                        new EmployeeFullName("Petr", "Petrov", "Petrovich")),
                    2,
                    new MerchRequestDateTime(DateTime.ParseExact("22.12.2020", "dd.MM.yyyy", CultureInfo.InvariantCulture)),
                    MerchRequestStatus.AwaitingDelivery,
                    new MerchRequestFrom(MerchRequestFromType.Manually)),
                new(3, new Employee(
                        new Email("aivanova@mail.com"),
                        new EmployeeFullName("Anna", "Ivanova", "Ivanovna")),
                    5,
                    new MerchRequestDateTime(DateTime.ParseExact("23.12.2020", "dd.MM.yyyy", CultureInfo.InvariantCulture)),
                    MerchRequestStatus.Canceled,
                    new MerchRequestFrom(MerchRequestFromType.Manually))
            };
        }

        public async Task<MerchRequest> Create(MerchRequest createdItem, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
                {
                    var newId = _merchRequests.Max(mr => mr.Id) + 1;
                    var newItem = new MerchRequest(newId,
                        createdItem.Employee,
                        createdItem.MerchPackId,
                        createdItem.MerchRequestDateTime,
                        createdItem.MerchRequestStatus,
                        createdItem.MerchRequestFrom);
                    _merchRequests = _merchRequests.Append(newItem).ToList();
                    return newItem;
                }, cancellationToken);
        }

        public async Task<MerchRequest> Update(MerchRequest updatedItem, CancellationToken cancellationToken)
        {
            if (updatedItem.Id == default)
                throw new Exception($"for update id must have a value");
            return await Task.Run(() =>
                {
                    var itemInCollection = _merchRequests.SingleOrDefault(mr => mr.Id.Equals(updatedItem.Id));
                    var merchRequestsList = _merchRequests.ToList();
                    merchRequestsList.Remove(itemInCollection);
                    _merchRequests = merchRequestsList.Append(updatedItem).ToList();
                    return updatedItem;
                }, cancellationToken);
        }

        public async Task<IReadOnlyList<MerchRequest>> Get(int merchPackId, MerchRequestStatus status, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                return _merchRequests.Where(mr => mr.MerchRequestStatus.Id.Equals(status.Id)
                                                  && mr.MerchPackId.Equals(merchPackId))
                    .ToList();
            }, cancellationToken);
        }

        public async Task<IReadOnlyList<MerchRequest>> Get(Email employeeEmail, MerchRequestStatus status,
            CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                return _merchRequests.Where(mr => 
                        mr.Employee.Email.Equals(employeeEmail)
                        && mr.MerchRequestStatus.Id.Equals(status.Id))
                    .ToList();
            }, cancellationToken);
        }

        public async Task<IReadOnlyList<MerchRequest>> Get(Email employeeEmail, int merchPackTypeId,
            CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                return _merchRequests.Where(mr =>
                        mr.Employee.Email.Equals(employeeEmail)
                        && mr.MerchPackId.Equals(merchPackTypeId))
                    .ToList();
            }, cancellationToken);
        }

    }
}