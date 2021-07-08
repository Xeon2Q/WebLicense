using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Customers;
using WebLicense.Shared;
using WebLicense.Shared.Customers;

namespace WebLicense.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ISender sender;
        private readonly ILogger<CustomersController> logger;

        #region C-tor | Fields

        public CustomersController(ISender sender, ILogger<CustomersController> logger)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Actions

        [AllowAnonymous]
        [HttpGet]
        public async Task<ListData<CustomerInfo>> Get(int skip = 0, int take = 100, string filters = null, string sorts = null)
        {
            try
            {
                if (skip < 0) skip = 0;
                if (take > 1000) take = 1000;
                if (skip > 1000) skip = 1000;
                var criteriaFilter = CriteriaFilter.TryParse(filters);
                var criteriaSort = CriteriaSort.TryParse(sorts);

                var data = await sender.Send(new GetCustomers(new Criteria<Customer>(skip, take, criteriaSort, criteriaFilter)));
                data.ThrowOnFail();

                return data.Data;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<CustomerInfo> Add(CustomerInfo customer)
        {
            try
            {
                var data = await sender.Send(new AddCustomer(customer));
                data.ThrowOnFail();

                return data.Data;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPatch]
        public async Task<CustomerInfo> Update(CustomerInfo customer)
        {
            try
            {
                var data = await sender.Send(new UpdateCustomer(customer));
                data.ThrowOnFail();

                return data.Data;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpDelete]
        public async Task Delete(int id)
        {
            try
            {
                var data = await sender.Send(new DeleteCompany(id));
                data.ThrowOnFail();
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        #endregion
    }
}
