﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using WebLicense.Client.Auxiliary.Extensions;
using WebLicense.Shared.Customers;

namespace WebLicense.Client.Pages.Customers
{
    public partial class Add : ComponentBase
    {
        #region C-tor | Properties

        [Inject]
        private HttpClient Client { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public CustomerInfo Data { get; set; } = new();

        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            Data = GetEmptyCustomerInfo(state.User);
        }

        #endregion

        #region Private methods

        private static CustomerInfo GetEmptyCustomerInfo(ClaimsPrincipal user)
        {
            var entity = new CustomerInfo
            {
                Id = 0,
                Settings = new CustomerSettingsInfo(),
                Administrators = new List<CustomerUserInfo>(),
                Managers = new List<CustomerUserInfo>(),
                Users = new List<CustomerUserInfo>()
            };

            entity.Settings.NotificationsEmail = user.GetEmail();
            entity.Users.Add(new() {Id = user.GetId(), Name = user.GetName()});

            return entity;
        }

        #endregion
    }
}