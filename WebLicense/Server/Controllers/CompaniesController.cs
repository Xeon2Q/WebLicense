using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    [Produces(MediaTypeNames.Application.Json)]
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

        /// <summary>
        /// Get Company by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a Customer</returns>
        /// <response code="200">Customer information</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="401">Not authorized request</response>
        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyInfo))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var data = await sender.Send(new GetCompany(id, User.GetId<long>()));
                data.ThrowOnFail();

                return Ok(data.Data);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Create Company
        /// </summary>
        /// <param name="company"></param>
        /// <returns>Returns newly created Company</returns>
        /// <response code="201">Company is created</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="401">Not authorized request</response>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CompanyInfo))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Add(CompanyInfo company)
        {
            try
            {
                var data = await sender.Send(new AddCompany(company, User.GetId<long>()));
                data.ThrowOnFail();

                return CreatedAtAction(nameof(Get), new { id = data.Data.Id ?? 0 }, data.Data);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return Problem(e.Message);
            }
        }

        /// <summary>
        /// Update Company
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Returns updated Company</returns>
        /// <response code="200">Company is updated</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="401">Not authorized request</response>
        [HttpPatch]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyInfo))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(CompanyInfo customer)
        {
            try
            {
                var data = await sender.Send(new UpdateCompany(customer, User.GetId<long>()));
                data.ThrowOnFail();

                return Ok(data.Data);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Delete Company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Company is deleted</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="401">Not authorized request</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var data = await sender.Send(new DeleteCompany(id, User.GetId<long>()));
                data.ThrowOnFail();
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Lists

        /// <summary>
        /// Return list of Companies
        /// </summary>
        /// <param name="skip">Amount of Companies to be skipped in List</param>
        /// <param name="take">Amount of Companies to be included in List</param>
        /// <param name="filters">Filters applied to list of Companies</param>
        /// <param name="sorts">Sort settings applied to list of Companies</param>
        /// <returns></returns>
        /// <response code="200">Returns list of Companies</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="401">Not authorized request</response>
        [HttpGet]
        [Route("list")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListData<CompanyInfo>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> List(int skip = 0, int take = 100, string filters = null, string sorts = null)
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

                return Ok(data.Data);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Return list of client Companies
        /// </summary>
        /// <param name="skip">Amount of Companies to be skipped in List</param>
        /// <param name="take">Amount of Companies to be included in List</param>
        /// <param name="filters">Filters applied to list of client Companies</param>
        /// <param name="sorts">Sort settings applied to list of client Companies</param>
        /// <returns></returns>
        /// <response code="200">Return list of client Companies</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="401">Not authorized request</response>
        [HttpGet]
        [Route("list/clients")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListData<CompanyInfo>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClientList(int skip = 0, int take = 100, string filters = null, string sorts = null)
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

                return Ok(data.Data);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Returns list of provider Companies
        /// </summary>
        /// <param name="skip">Amount of Companies to be skipped in List</param>
        /// <param name="take">Amount of Companies to be included in List</param>
        /// <param name="filters">Filters applied to list of provider Companies</param>
        /// <param name="sorts">Sort settings applied to list of provider Companies</param>
        /// <returns></returns>
        /// <response code="200">Return list of provider Companies</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="401">Not authorized request</response>
        [HttpGet]
        [Route("list/providers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListData<CompanyInfo>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ProviderList(int skip = 0, int take = 100, string filters = null, string sorts = null)
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

                return Ok(data.Data);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Access methods

        /// <summary>
        /// Return access information for Company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Return access information for Company</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="401">Not authorized request</response>
        [HttpGet]
        [Route("{id:int}/access")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyAccessInfo))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAccess(int id)
        {
            try
            {
                var data = await sender.Send(new GetCompanyAccess(id, User.GetId<long>()));
                return Ok(data);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        #endregion

        #endregion
    }
}
