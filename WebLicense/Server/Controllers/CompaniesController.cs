using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebLicense.Core.Enums;
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
        #region C-tor | Fields

        private readonly ISender sender;
        private readonly ILogger<CompaniesController> logger;

        public CompaniesController(ISender sender, ILogger<CompaniesController> logger)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Actions

        #region Company methods

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

        [HttpPost]
        public async Task<CompanyInfo> Add(CompanyInfo company)
        {
            try
            {
                var data = await sender.Send(new AddCompany(company, User.GetId<long>()));
                data.ThrowOnFail();

                return data.Data;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpPatch]
        public async Task<CompanyInfo> Update(CompanyInfo customer)
        {
            try
            {
                var data = await sender.Send(new UpdateCompany(customer, User.GetId<long>()));
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
                var data = await sender.Send(new DeleteCompany(id, User.GetId<long>()));
                data.ThrowOnFail();
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        #endregion

        #region Lists

        [HttpGet]
        [Route("list")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ListData<CompanyInfo>> List(int skip = 0, int take = 100, string filters = null, string sorts = null)
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

        [HttpGet]
        [Route("list/clients")]
        public async Task<ListData<CompanyInfo>> ClientList(int skip = 0, int take = 100, string filters = null, string sorts = null)
        {
            try
            {
                if (skip < 0) skip = 0;
                if (take > 1000) take = 1000;
                if (skip > 1000) skip = 1000;
                var criteriaFilter = CriteriaFilter.TryParse(filters);
                var criteriaSort = CriteriaSort.TryParse(sorts);

                var data = await sender.Send(new GetClientCompanies(new Criteria<Company>(skip, take, criteriaSort, criteriaFilter), User.GetId<long>()));
                data.ThrowOnFail();

                return data.Data;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("list/providers")]
        public async Task<ListData<CompanyInfo>> ProviderList(int skip = 0, int take = 100, string filters = null, string sorts = null)
        {
            try
            {
                if (skip < 0) skip = 0;
                if (take > 1000) take = 1000;
                if (skip > 1000) skip = 1000;
                var criteriaFilter = CriteriaFilter.TryParse(filters);
                var criteriaSort = CriteriaSort.TryParse(sorts);

                var data = await sender.Send(new GetProviderCompanies(new Criteria<Company>(skip, take, criteriaSort, criteriaFilter), User.GetId<long>()));
                data.ThrowOnFail();

                return data.Data;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        #endregion

        #region Access methods

        [HttpGet]
        [Route("{id:int}/access")]
        public async Task<CompanyAccessInfo> GetAccess(int id)
        {
            try
            {
                return await sender.Send(new GetCompanyAccess(id, User.GetId<long>()));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        #endregion

        #endregion
    }
}
