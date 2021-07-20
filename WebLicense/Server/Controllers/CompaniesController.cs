using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebLicense.Core.Models.Companies;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Companies;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Shared;
using WebLicense.Shared.Companies;

namespace WebLicense.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ISender sender;
        private readonly ILogger<CompaniesController> logger;

        #region C-tor | Fields

        public CompaniesController(ISender sender, ILogger<CompaniesController> logger)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Actions

        [AllowAnonymous]
        [HttpGet]
        [Route("{id:int}")]
        public async Task<CompanyInfo> Get(int id)
        {
            try
            {
                var data = await sender.Send(new GetCompany(id, User.GetId<long>()));
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
        [HttpGet]
        [Route("list")]
        public async Task<ListData<CompanyInfo>> Get(int skip = 0, int take = 100, string filters = null, string sorts = null)
        {
            try
            {
                if (skip < 0) skip = 0;
                if (take > 1000) take = 1000;
                if (skip > 1000) skip = 1000;
                var criteriaFilter = CriteriaFilter.TryParse(filters);
                var criteriaSort = CriteriaSort.TryParse(sorts);

                var data = await sender.Send(new GetCompanies(new Criteria<Company>(skip, take, criteriaSort, criteriaFilter)));
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
        public async Task<CompanyInfo> Add(CompanyInfo company)
        {
            try
            {
                var data = await sender.Send(new AddCompany(company));
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
        public async Task<CompanyInfo> Update(CompanyInfo customer)
        {
            try
            {
                var data = await sender.Send(new UpdateCompany(customer));
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
