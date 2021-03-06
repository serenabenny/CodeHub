﻿using System;
using ReactiveUI;
using System.Collections.Generic;
using CodeHub.Core.ViewModels.Organizations;
using CodeHub.Core.ViewModels.Users;
using Octokit;
using System.Threading.Tasks;
using CodeHub.Core.Services;

namespace CodeHub.Core.ViewModels.Users
{
    public abstract class BaseUsersViewModel : BaseSearchableListViewModel<User, UserItemViewModel>
    {
        protected ISessionService SessionService { get; private set; }

        protected BaseUsersViewModel(ISessionService sessionService)
        {
            SessionService = sessionService;

            Items = InternalItems.CreateDerivedCollection(x => CreateItemViewModel(x),
            x => x.Login.StartsWith(SearchKeyword ?? string.Empty, StringComparison.OrdinalIgnoreCase),
            signalReset: this.WhenAnyValue(x => x.SearchKeyword));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t => {
                InternalItems.Reset(await RetrieveUsers());
            });
        }

        private UserItemViewModel CreateItemViewModel(User user)
        {
            var isOrg = user.Type.HasValue && user.Type.Value == AccountType.Organization;
            return new UserItemViewModel(user.Login, user.AvatarUrl, isOrg, () => {
                if (isOrg)
                {
                    var vm = this.CreateViewModel<OrganizationViewModel>();
                    vm.Init(user.Login);
                    NavigateTo(vm);
                }
                else
                {
                    var vm = this.CreateViewModel<UserViewModel>();
                    vm.Init(user.Login, user);
                    NavigateTo(vm);
                }
            });
        }

        private async Task<IReadOnlyList<User>> RetrieveUsers(int page = 1)
        {
            var connection = SessionService.GitHubClient.Connection;
            var parameters = new Dictionary<string, string>();
            parameters["page"] = page.ToString();
            parameters["per_page"] = 100.ToString();
            var ret = await connection.Get<IReadOnlyList<User>>(RequestUri, parameters, "application/json");

            if (ret.HttpResponse.ApiInfo.Links.ContainsKey("next"))
            {
                LoadMoreCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                    InternalItems.AddRange(await RetrieveUsers(page + 1));
                });
            }
            else
            {
                LoadMoreCommand = null;
            }

            return ret.Body;
        }

        protected abstract Uri RequestUri { get; }
    }
}
