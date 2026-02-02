using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;

namespace Andro.Backend.Reference.OpenIddict;

/* Creates initial data that is needed to property run the application
 * and make client-to-server communication possible.
 */
public class OpenIddictDataSeedContributor : OpenIddictDataSeedContributorBase, IDataSeedContributor, ITransientDependency
{
    public OpenIddictDataSeedContributor(
        IConfiguration configuration,
        IOpenIddictApplicationRepository openIddictApplicationRepository,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeRepository openIddictScopeRepository,
        IOpenIddictScopeManager scopeManager)
        : base(configuration, openIddictApplicationRepository, applicationManager, openIddictScopeRepository, scopeManager)
    {
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopesAsync();
        await CreateApplicationsAsync();
    }

    private async Task CreateScopesAsync()
    {
        await CreateScopesAsync(new OpenIddictScopeDescriptor
        {
            Name = "Reference",
            DisplayName = "Reference API",
            Resources = { "Reference" }
        });
    }

    private async Task CreateApplicationsAsync()
    {
        var commonScopes = new List<string> {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles,
            "Reference"
        };

        var configurationSection = Configuration.GetSection("OpenIddict:Applications");


        //Console Test / Angular Client
        var consoleAndAngularClientId = configurationSection["Reference_App:ClientId"];
        if (!consoleAndAngularClientId.IsNullOrWhiteSpace())
        {
            var consoleAndAngularClientRootUrl = configurationSection["Reference_App:RootUrl"]?.TrimEnd('/');
            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: consoleAndAngularClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Console Test / Angular Application",
                secret: null,
                grantTypes: new List<string> {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.Password,
                    OpenIddictConstants.GrantTypes.ClientCredentials,
                    OpenIddictConstants.GrantTypes.RefreshToken,
                    "LinkLogin",
                    "Impersonation"
                },
                scopes: commonScopes,
                redirectUris: !string.IsNullOrEmpty(consoleAndAngularClientRootUrl) ? new List<string> { consoleAndAngularClientRootUrl } : new List<string>(),
                postLogoutRedirectUris: !string.IsNullOrEmpty(consoleAndAngularClientRootUrl) ? new List<string> { consoleAndAngularClientRootUrl } : new List<string>(),
                clientUri: consoleAndAngularClientRootUrl ?? string.Empty,
                logoUri: "/images/clients/angular.svg"
            );
        }







        // Swagger Client
        var swaggerClientId = configurationSection["Reference_Swagger:ClientId"];
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var swaggerRootUrl = configurationSection["Reference_Swagger:RootUrl"]?.TrimEnd('/');

            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: swaggerClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Swagger Application",
                secret: null,
                grantTypes: new List<string> { OpenIddictConstants.GrantTypes.AuthorizationCode, },
                scopes: commonScopes,
                redirectUris: new List<string> { $"{swaggerRootUrl}/swagger/oauth2-redirect.html" },
                postLogoutRedirectUris: new List<string> { swaggerRootUrl + "/swagger" },
                clientUri: !string.IsNullOrEmpty(swaggerRootUrl) ? swaggerRootUrl.EnsureEndsWith('/') + "swagger" : string.Empty,
                logoUri: "/images/clients/swagger.svg"
            );
        }


    }
}
