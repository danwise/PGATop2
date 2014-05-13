using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Security;
using System.Runtime.InteropServices;
using System.DirectoryServices.AccountManagement;
using System.ServiceModel;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk;
using System.Text;


/// <summary>
/// Provides server connection information.
/// </summary>
public class ServerConnection
{
    #region Inner classes
    /// <summary>
    /// Stores Microsoft Dynamics CRM server configuration information.
    /// </summary>
    public class Configuration
    {
        public String ServerAddress;
        public String OrganizationName;
        public Uri DiscoveryUri;
        public Uri OrganizationUri;
        public Uri HomeRealmUri = null;
        public ClientCredentials DeviceCredentials = null;
        public ClientCredentials Credentials = null;
        public AuthenticationProviderType EndpointType;
        public String UserPrincipalName;
        #region internal members of the class
        internal IServiceManagement<IOrganizationService> OrganizationServiceManagement;
        internal SecurityTokenResponse OrganizationTokenResponse;
        internal Int16 AuthFailureCount = 0;
        #endregion

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            Configuration c = (Configuration)obj;

            if (!this.ServerAddress.Equals(c.ServerAddress, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (!this.OrganizationName.Equals(c.OrganizationName, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (this.EndpointType != c.EndpointType)
                return false;
            if (this.EndpointType == AuthenticationProviderType.ActiveDirectory)
            {
                if (!this.Credentials.Windows.ClientCredential.Domain.Equals(
                    c.Credentials.Windows.ClientCredential.Domain, StringComparison.InvariantCultureIgnoreCase))
                    return false;
                if (!this.Credentials.Windows.ClientCredential.UserName.Equals(
                    c.Credentials.Windows.ClientCredential.UserName, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            else if (this.EndpointType == AuthenticationProviderType.LiveId)
            {
                if (!this.Credentials.UserName.UserName.Equals(c.Credentials.UserName.UserName,
                    StringComparison.InvariantCultureIgnoreCase))
                    return false;
                if (!this.DeviceCredentials.UserName.UserName.Equals(
                    c.DeviceCredentials.UserName.UserName, StringComparison.InvariantCultureIgnoreCase))
                    return false;
                if (!this.DeviceCredentials.UserName.Password.Equals(
                    c.DeviceCredentials.UserName.Password, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            else
            {
                if (!this.Credentials.UserName.UserName.Equals(c.Credentials.UserName.UserName,
                    StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int returnHashCode = this.ServerAddress.GetHashCode()
                ^ this.OrganizationName.GetHashCode()
                ^ this.EndpointType.GetHashCode();

            if (this.EndpointType == AuthenticationProviderType.ActiveDirectory)
                returnHashCode = returnHashCode
                    ^ this.Credentials.Windows.ClientCredential.UserName.GetHashCode()
                    ^ this.Credentials.Windows.ClientCredential.Domain.GetHashCode();
            else if (this.EndpointType == AuthenticationProviderType.LiveId)
                returnHashCode = returnHashCode
                    ^ this.Credentials.UserName.UserName.GetHashCode()
                    ^ this.DeviceCredentials.UserName.UserName.GetHashCode()
                    ^ this.DeviceCredentials.UserName.Password.GetHashCode();
            else
                returnHashCode = returnHashCode
                    ^ this.Credentials.UserName.UserName.GetHashCode();

            return returnHashCode;
        }

    }
    #endregion Inner classes

    #region Public properties

    public List<Configuration> configurations = null;

    #endregion Public properties

    #region Private properties

    private Configuration config = new Configuration();
    private string crmInstance = "";

    #endregion Private properties

    #region Static methods
    /// <summary>
    /// Obtains the organization service proxy.
    /// </summary>
    /// <param name="serverConfiguration"></param>
    /// <returns></returns>



    public static OrganizationServiceProxy GetOrganizationProxy(
        ServerConnection.Configuration serverConfiguration)
    {
        // Obtain the organization service proxy for the Federated, LiveId, and OnlineFederated environments. 
        if (serverConfiguration.OrganizationServiceManagement != null
            && serverConfiguration.OrganizationTokenResponse != null)
        {
            return new OrganizationServiceProxy(
                serverConfiguration.OrganizationServiceManagement,
                serverConfiguration.OrganizationTokenResponse);
        }

        if (serverConfiguration.OrganizationServiceManagement == null)
            throw new ArgumentNullException("serverConfiguration.OrganizationServiceManagement");

        // Obtain the organization service proxy for the ActiveDirectory environment.
        return new OrganizationServiceProxy(
            serverConfiguration.OrganizationServiceManagement,
            serverConfiguration.Credentials);

    }
    #endregion Static methods

    #region Public methods
    /// <summary>
    /// Obtains the server connection information including the target organization's
    /// Uri and user logon credentials from the user.
    /// </summary>
    public virtual Configuration GetServerConfiguration(String serverAddress, String OrgName, String domain, String username, bool https, AuthenticationProviderType authenticationType)
    {

        String input;
        input = crmInstance;
        config.ServerAddress = serverAddress;
        if (https)
            config.DiscoveryUri = new Uri(String.Format("https://{0}/XRMServices/2011/Discovery.svc", config.ServerAddress));
        else
            config.DiscoveryUri = new Uri(String.Format("http://{0}/XRMServices/2011/Discovery.svc", config.ServerAddress));

        // Get the target organization.
        //config.OrganizationUri = GetOrganizationAddress();
        //if (https)
        //    config.OrganizationUri = new Uri(string.Format("https://{0}/{1}/XRMSErvices/2011/Organization.svc", config.ServerAddress, OrgName));
        //else
        //    config.OrganizationUri = new Uri(string.Format("http://{0}/{1}/XRMSErvices/2011/Organization.svc", config.ServerAddress, OrgName));

        if (https)
            config.OrganizationUri = new Uri(string.Format("https://{0}/XRMSErvices/2011/Organization.svc", config.ServerAddress));
        else
            config.OrganizationUri = new Uri(string.Format("http://{0}/XRMSErvices/2011/Organization.svc", config.ServerAddress));

        config.EndpointType = authenticationType;
        config.Credentials = GetUserLogonCredentials(domain, username);
        config.OrganizationName = OrgName;


        // Set IServiceManagement for the current organization.
        //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };  
        IServiceManagement<IOrganizationService> orgServiceManagement = ServiceConfigurationFactory.CreateManagement<IOrganizationService>(config.OrganizationUri);
        config.OrganizationServiceManagement = orgServiceManagement;

        // Set SecurityTokenResponse for the current organization.
        if (config.EndpointType != AuthenticationProviderType.ActiveDirectory)
        {
            // Set the credentials.
            AuthenticationCredentials authCredentials = new AuthenticationCredentials();
            // If UserPrincipalName exists, use it. Otherwise, set the logon credentials from the configuration.
            if (!String.IsNullOrWhiteSpace(config.UserPrincipalName))
            {
                authCredentials.UserPrincipalName = config.UserPrincipalName;
            }
            else
            {
                authCredentials.ClientCredentials = config.Credentials;
                if (config.EndpointType == AuthenticationProviderType.LiveId)
                {
                    authCredentials.SupportingCredentials = new AuthenticationCredentials();
                    authCredentials.SupportingCredentials.ClientCredentials = config.DeviceCredentials;
                }
            }
            AuthenticationCredentials tokenCredentials =
                orgServiceManagement.Authenticate(authCredentials);

            if (tokenCredentials != null)
            {
                if (tokenCredentials.SecurityTokenResponse != null)
                    config.OrganizationTokenResponse = tokenCredentials.SecurityTokenResponse;
            }
        }

        return config;
    }

    /// <summary>
    /// Discovers the organizations that the calling user belongs to.
    /// </summary>
    /// <param name="service">A Discovery service proxy instance.</param>
    /// <returns>Array containing detailed information on each organization that 
    /// the user belongs to.</returns>
    public OrganizationDetailCollection DiscoverOrganizations(IDiscoveryService service)
    {
        if (service == null) throw new ArgumentNullException("service");
        RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
        RetrieveOrganizationsResponse orgResponse =
            (RetrieveOrganizationsResponse)service.Execute(orgRequest);

        return orgResponse.Details;
    }

    /// <summary>
    /// Finds a specific organization detail in the array of organization details
    /// returned from the Discovery service.
    /// </summary>
    /// <param name="orgFriendlyName">The friendly name of the organization to find.</param>
    /// <param name="orgDetails">Array of organization detail object returned from the discovery service.</param>
    /// <returns>Organization details or null if the organization was not found.</returns>
    /// <seealso cref="DiscoveryOrganizations"/>
    public OrganizationDetail FindOrganization(string orgFriendlyName,
        OrganizationDetail[] orgDetails)
    {
        if (String.IsNullOrWhiteSpace(orgFriendlyName))
            throw new ArgumentNullException("orgFriendlyName");
        if (orgDetails == null)
            throw new ArgumentNullException("orgDetails");
        OrganizationDetail orgDetail = null;

        foreach (OrganizationDetail detail in orgDetails)
        {
            if (String.Compare(detail.FriendlyName, orgFriendlyName,
                StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                orgDetail = detail;
                break;
            }
        }
        return orgDetail;
    }

    /// <summary>
    /// Reads a server configuration file.
    /// Read the configuration from disk, if it exists, at C:\Users\YourUserName\AppData\Roaming\CrmServer\Credentials.xml.
    /// </summary>
    /// <returns>Is configuration settings already available on disk.</returns>
    public Boolean ReadConfigurations()
    {
        Boolean isConfigExist = false;

        if (configurations == null)
            configurations = new List<Configuration>();

        if (File.Exists(CrmServiceHelperConstants.ServerCredentialsFile))
        {
            XElement configurationsFromFile = XElement.Load(CrmServiceHelperConstants.ServerCredentialsFile);
            foreach (XElement config in configurationsFromFile.Nodes())
            {
                Configuration newConfig = new Configuration();
                var serverAddress = config.Element("ServerAddress");
                if (serverAddress != null)
                    if (!String.IsNullOrEmpty(serverAddress.Value))
                        newConfig.ServerAddress = serverAddress.Value;
                var organizationName = config.Element("OrganizationName");
                if (organizationName != null)
                    if (!String.IsNullOrEmpty(organizationName.Value))
                        newConfig.OrganizationName = organizationName.Value;
                var discoveryUri = config.Element("DiscoveryUri");
                if (discoveryUri != null)
                    if (!String.IsNullOrEmpty(discoveryUri.Value))
                        newConfig.DiscoveryUri = new Uri(discoveryUri.Value);
                var organizationUri = config.Element("OrganizationUri");
                if (organizationUri != null)
                    if (!String.IsNullOrEmpty(organizationUri.Value))
                        newConfig.OrganizationUri = new Uri(organizationUri.Value);
                var homeRealmUri = config.Element("HomeRealmUri");
                if (homeRealmUri != null)
                    if (!String.IsNullOrEmpty(homeRealmUri.Value))
                        newConfig.HomeRealmUri = new Uri(homeRealmUri.Value);

                var vendpointType = config.Element("EndpointType");
                if (vendpointType != null)
                    newConfig.EndpointType =
                            RetrieveAuthenticationType(vendpointType.Value);
                if (config.Element("Credentials").HasElements)
                {
                    newConfig.Credentials =
                        ParseInCredentials(config.Element("Credentials"), newConfig.EndpointType);
                }
                if (newConfig.EndpointType == AuthenticationProviderType.LiveId)
                {
                    newConfig.DeviceCredentials = GetDeviceCredentials();
                }
                var userPrincipalName = config.Element("UserPrincipalName");
                if (userPrincipalName != null)
                    if (!String.IsNullOrWhiteSpace(userPrincipalName.Value))
                        newConfig.UserPrincipalName = userPrincipalName.Value;
                configurations.Add(newConfig);
            }
        }

        if (configurations.Count > 0)
            isConfigExist = true;

        return isConfigExist;
    }

    /// <summary>
    /// Writes all server configurations to a file.
    /// </summary>
    /// <remarks>If the file exists, it is overwritten.</remarks>
    public void SaveConfigurations()
    {
        if (configurations == null)
            throw new Exception("No server connection configurations were found.");

        FileInfo file = new FileInfo(CrmServiceHelperConstants.ServerCredentialsFile);

        // Create directory if it does not exist.
        if (!file.Directory.Exists)
            file.Directory.Create();

        // Replace the file if it exists.
        using (FileStream fs = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
        {
            using (XmlTextWriter writer = new XmlTextWriter(fs, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("Configurations");
                writer.WriteFullEndElement();
                writer.WriteEndDocument();
            }
        }

        foreach (Configuration config in configurations)
            SaveConfiguration(CrmServiceHelperConstants.ServerCredentialsFile, config, true);
    }

    /// <summary>
    /// Writes a server configuration to a file.
    /// </summary>
    /// <param name="pathname">The file name and system path of the output configuration file.</param>
    /// <param name="config">A server connection configuration.</param>
    /// <param name="append">If true, the configuration is appended to the file, otherwise a new file
    /// is created.</param>
    public void SaveConfiguration(String pathname, Configuration config, bool append)
    {
        if (String.IsNullOrWhiteSpace(pathname)) throw new ArgumentNullException("pathname");
        if (config == null) throw new ArgumentNullException("config");
        XElement configurationsFromFile = XElement.Load(pathname);
        XElement newConfig =
            new XElement("Configuration",
                new XElement("ServerAddress", config.ServerAddress),
                new XElement("OrganizationName", config.OrganizationName),
                new XElement("DiscoveryUri",
                    (config.DiscoveryUri != null)
                    ? config.DiscoveryUri.OriginalString
                    : String.Empty),
                new XElement("OrganizationUri",
                    (config.OrganizationUri != null)
                    ? config.OrganizationUri.OriginalString
                    : String.Empty),
                new XElement("HomeRealmUri",
                    (config.HomeRealmUri != null)
                    ? config.HomeRealmUri.OriginalString
                    : String.Empty),
                ParseOutCredentials(config.Credentials, config.EndpointType),
                new XElement("EndpointType", config.EndpointType.ToString()),
                new XElement("UserPrincipalName",
                    (config.UserPrincipalName != null)
                    ? config.UserPrincipalName
                    : String.Empty)
            );

        if (append)
        {
            configurationsFromFile.Add(newConfig);
        }
        else
        {
            configurationsFromFile.ReplaceAll(newConfig);
        }

        using (XmlTextWriter writer = new XmlTextWriter(pathname, Encoding.UTF8))
        {
            writer.Formatting = Formatting.Indented;
            configurationsFromFile.Save(writer);
        }
    }

    #endregion Public methods

    #region Protected methods

    /// <summary>
    /// Obtains the name and port of the server running the Microsoft Dynamics CRM
    /// Discovery service.
    /// </summary>
    /// <returns>The server's network name and optional TCP/IP port.</returns>
    protected virtual String GetServerAddress(out bool ssl)
    {
        ssl = false;

        Console.Write("Enter a CRM server name and port [crm.dynamics.com]: ");
        String server = Console.ReadLine();

        if (server.EndsWith(".dynamics.com") || String.IsNullOrWhiteSpace(server))
        {
            ssl = true;
        }
        else
        {
            Console.Write("Is this server configured for Secure Socket Layer (https) (y/n) [n]: ");
            String answer = Console.ReadLine();

            if (answer == "y" || answer == "Y")
                ssl = true;
        }

        return server;
    }

    /// <summary>
    /// Is this organization provisioned in Microsoft Office 365?
    /// </summary>
    /// <param name="server">The server's network name.</param>
    protected virtual Boolean GetOrgType(String server)
    {
        Boolean isO365Org = false;
        if (String.IsNullOrWhiteSpace(server))
            return isO365Org;
        if (server.IndexOf('.') == -1)
            return isO365Org;

        Console.Write("Is this organization provisioned in Microsoft Office 365 (y/n) [n]: ");
        String answer = Console.ReadLine();

        if (answer == "y" || answer == "Y")
            isO365Org = true;

        return isO365Org;
    }

    /// <summary>
    /// Obtains the web address (Uri) of the target organization.
    /// </summary>
    /// <returns>Uri of the organization service or an empty string.</returns>
    //protected virtual Uri GetOrganizationAddress()
    //{
    //    using (DiscoveryServiceProxy serviceProxy = GetDiscoveryProxy())
    //    {
    //        // Obtain organization information from the Discovery service. 
    //        if (serviceProxy != null)
    //        {
    //            // Obtain information about the organizations that the system user belongs to.
    //            OrganizationDetailCollection orgs = DiscoverOrganizations(serviceProxy);

    //            if (orgs.Count > 0)
    //            {
    //                Console.WriteLine("\nList of organizations that you belong to:");
    //                for (int n = 0; n < orgs.Count; n++)
    //                {
    //                    Console.Write("\n({0}) {1} ({2})\t", n + 1, orgs[n].FriendlyName, orgs[n].UrlName);
    //                }

    //                Console.Write("\n\nSpecify an organization number (1-{0}) [1]: ", orgs.Count);
    //                String input = Console.ReadLine();
    //                if (input == String.Empty)
    //                {
    //                    input = "1";
    //                }
    //                int orgNumber;
    //                Int32.TryParse(input, out orgNumber);
    //                if (orgNumber > 0 && orgNumber <= orgs.Count)
    //                {
    //                    config.OrganizationName = orgs[orgNumber - 1].FriendlyName;
    //                    // Return the organization Uri.
    //                    return new System.Uri(orgs[orgNumber - 1].Endpoints[EndpointType.OrganizationService]);
    //                }
    //                else
    //                    throw new Exception("The specified organization does not exist.");
    //            }
    //            else
    //            {
    //                Console.WriteLine("\nYou do not belong to any organizations on the specified server.");
    //                return new System.Uri(String.Empty);
    //            }
    //        }
    //        else
    //            throw new Exception("An invalid server name was specified.");
    //    }
    //}

    /// <summary>
    /// Obtains the user's logon credentials for the target server.
    /// </summary>
    /// <returns>Logon credentials of the user.</returns>
    protected virtual ClientCredentials GetUserLogonCredentials(String domain, String userName)
    {
        ClientCredentials credentials = new ClientCredentials();
        SecureString password;
        Boolean isCredentialExist = (config.Credentials != null) ? true : false;
        switch (config.EndpointType)
        {
            // An on-premises Microsoft Dynamics CRM server deployment. 
            case AuthenticationProviderType.ActiveDirectory:
                if (userName == "CRMAdmin")
                    credentials.Windows.ClientCredential = new System.Net.NetworkCredential(userName, "ITP@ssw0rd", domain);
                if (userName == "seiumaster")
                    credentials.Windows.ClientCredential = new System.Net.NetworkCredential(userName, "WebFortis!", domain);
                    
                break;
            // A Microsoft Dynamics CRM Online server deployment. 
            case AuthenticationProviderType.LiveId:
                Console.Write("\n Enter Live ID: ");
                if (isCredentialExist)
                {
                    Console.WriteLine(config.Credentials.UserName.UserName);
                }
                userName = (isCredentialExist) ?
                    config.Credentials.UserName.UserName : Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userName))
                {
                    return null;
                }

                Console.Write("Enter Password: ");
                password = ReadPassword();

                credentials.UserName.UserName = userName;
                credentials.UserName.Password = ConvertToUnsecureString(password);
                break;
            // An internet-facing deployment (IFD) of Microsoft Dynamics CRM.          
            case AuthenticationProviderType.Federation:
                //Console.Write("\n Enter Username: ");
                //if (isCredentialExist)
                //{
                //    Console.WriteLine(config.Credentials.UserName.UserName);
                //}
                //userName = (isCredentialExist) ? 
                //    config.Credentials.UserName.UserName : Console.ReadLine();
                //if (string.IsNullOrWhiteSpace(userName))
                //{
                //    return null;
                //}

                //Console.Write(" Enter Password: ");
                //password = ReadPassword();
                //credentials.Windows.ClientCredential = new System.Net.NetworkCredential(userName, "ITP@ssw0rd", domain);
                credentials.UserName.UserName = userName;
                credentials.UserName.Password = "wolfp@ck99";
                break;
            // Managed Identity/Federated Identity users using Microsoft Office 365.
            case AuthenticationProviderType.OnlineFederation:
                if (!isCredentialExist && config.AuthFailureCount == 0)
                {
                    // Initial try with the current UserPrincipalName for the Federated Identity organization.
                    // else config.UserPrincipalName has value, and it will use the same existing UserPrincipalName.
                    if (String.IsNullOrWhiteSpace(config.UserPrincipalName))
                        config.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;
                    // using existing UserPrincipalName instead of UserName & Password.
                    return null;
                }

                // Fetch/Request user credentials. 
                config.UserPrincipalName = String.Empty;
                // Request Managed Identity/Federated Identity username and password.
                Console.Write("\n Enter Username: ");
                if (isCredentialExist)
                {
                    Console.WriteLine(config.Credentials.UserName.UserName);
                }
                userName = (isCredentialExist) ?
                    config.Credentials.UserName.UserName : Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userName))
                {
                    return null;
                }

                Console.Write(" Enter Password: ");
                password = ReadPassword();

                credentials.UserName.UserName = userName;
                credentials.UserName.Password = ConvertToUnsecureString(password);
                break;
            default:
                credentials = null;
                break;
        }
        return credentials;
    }

    /// <summary>
    /// Prompts user to enter password in console window 
    /// and capture the entered password into SecureString.
    /// </summary>
    /// <returns>Password stored in a secure string.</returns>
    protected SecureString ReadPassword()
    {
        SecureString ssPassword = new SecureString();

        ConsoleKeyInfo info = Console.ReadKey(true);
        while (info.Key != ConsoleKey.Enter)
        {
            if (info.Key == ConsoleKey.Backspace)
            {
                if (ssPassword.Length != 0)
                {
                    ssPassword.RemoveAt(ssPassword.Length - 1);
                    Console.Write("\b \b");     // erase last char
                }
            }
            else if (info.KeyChar >= ' ')           // no control chars
            {
                ssPassword.AppendChar(info.KeyChar);
                Console.Write("*");
            }
            info = Console.ReadKey(true);
        }

        Console.WriteLine();
        Console.WriteLine();

        // Lock the secure string password.
        ssPassword.MakeReadOnly();

        return ssPassword;
    }

    /// <summary>
    /// Get the device credentials by either loading from the local cache 
    /// or request new device credentials by registering the device.
    /// </summary>
    /// <returns>Device Credentials.</returns>
    protected virtual ClientCredentials GetDeviceCredentials()
    {
        return Microsoft.Crm.Services.Utility.DeviceIdManager.LoadOrRegisterDevice();
    }

    /// <summary>
    /// Get the discovery service proxy based on existing configuration data.
    /// Added new way of getting discovery proxy.
    /// Also preserving old way of getting discovery proxy to support old scenarios.
    /// </summary>
    /// <returns>An instance of DiscoveryServiceProxy</returns>
    //private DiscoveryServiceProxy GetDiscoveryProxy()
    //{

    //    IServiceManagement<IDiscoveryService> serviceManagement =
    //                ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(
    //                config.DiscoveryUri);

    //    // Get the EndpointType.
    //    config.EndpointType = serviceManagement.AuthenticationType;

    //    // Get the logon credentials.
    //    config.Credentials = GetUserLogonCredentials();

    //    AuthenticationCredentials authCredentials = new AuthenticationCredentials();

    //    if (!String.IsNullOrWhiteSpace(config.UserPrincipalName))
    //    {
    //        // Try to authenticate the Federated Identity organization with UserPrinicipalName.
    //        authCredentials.UserPrincipalName = config.UserPrincipalName;

    //        try
    //        {
    //            AuthenticationCredentials tokenCredentials = serviceManagement.Authenticate(
    //                authCredentials);
    //            DiscoveryServiceProxy discoveryProxy = new DiscoveryServiceProxy(serviceManagement,
    //                tokenCredentials.SecurityTokenResponse);
    //            // Checking authentication by invoking some SDK methods.
    //            OrganizationDetailCollection orgs = DiscoverOrganizations(discoveryProxy);
    //            return discoveryProxy;
    //        }
    //        catch (System.ServiceModel.Security.SecurityAccessDeniedException ex)
    //        {
    //            // If authentication failed using current UserPrincipalName, 
    //            // request UserName and Password to try to authenticate using user credentials.
    //            if (ex.Message.Contains("Access is denied."))
    //            {
    //                config.AuthFailureCount += 1;
    //                authCredentials.UserPrincipalName = String.Empty;                        

    //                config.Credentials = GetUserLogonCredentials();
    //            }
    //            else
    //            {
    //                throw ex;
    //            }
    //        }
    //        // You can also catch other exceptions to handle a specific situation in your code, for example, 
    //        //      System.ServiceModel.Security.ExpiredSecurityTokenException
    //        //      System.ServiceModel.Security.MessageSecurityException
    //        //      System.ServiceModel.Security.SecurityNegotiationException                
    //    }

    // Resetting credentials in the AuthenicationCredentials.  
    //    if (config.EndpointType != AuthenticationProviderType.ActiveDirectory)
    //    {
    //        authCredentials = new AuthenticationCredentials();
    //        authCredentials.ClientCredentials = config.Credentials;

    //        if (config.EndpointType == AuthenticationProviderType.LiveId)
    //        {
    //            authCredentials.SupportingCredentials = new AuthenticationCredentials();
    //            authCredentials.SupportingCredentials.ClientCredentials = config.DeviceCredentials;
    //        }
    //        // Try to authenticate with the user credentials.
    //        AuthenticationCredentials tokenCredentials1 = serviceManagement.Authenticate(
    //            authCredentials);
    //        return new DiscoveryServiceProxy(serviceManagement,
    //       tokenCredentials1.SecurityTokenResponse);
    //    }
    //    // For an on-premises environment.
    //    return new DiscoveryServiceProxy(serviceManagement, config.Credentials);
    //}

    /// <summary>
    /// Verify passed strings with the supported AuthenticationProviderType.
    /// </summary>
    /// <param name="authType">String AuthenticationType</param>
    /// <returns>Supported AuthenticatoinProviderType</returns>
    private AuthenticationProviderType RetrieveAuthenticationType(String authType)
    {
        switch (authType)
        {
            case "ActiveDirectory":
                return AuthenticationProviderType.ActiveDirectory;
            case "LiveId":
                return AuthenticationProviderType.LiveId;
            case "Federation":
                return AuthenticationProviderType.Federation;
            case "OnlineFederation":
                return AuthenticationProviderType.OnlineFederation;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid authentication type", authType));
        }
    }

    /// <summary>
    /// Parse credentials from an XML node to required ClientCredentials data type 
    /// based on passed AuthenticationProviderType.
    /// </summary>
    /// <param name="credentials">Credential XML node.</param>
    /// <param name="endpointType">AuthenticationProviderType of the credential.</param>
    /// <returns>Required ClientCredentials type.</returns>
    private ClientCredentials ParseInCredentials(XElement credentials, AuthenticationProviderType endpointType)
    {
        ClientCredentials result = new ClientCredentials();

        switch (endpointType)
        {
            case AuthenticationProviderType.ActiveDirectory:
                result.Windows.ClientCredential = new System.Net.NetworkCredential()
                {
                    UserName = credentials.Element("UserName").Value,
                    Domain = credentials.Element("Domain").Value
                };
                break;
            case AuthenticationProviderType.LiveId:
            case AuthenticationProviderType.Federation:
            case AuthenticationProviderType.OnlineFederation:
                result.UserName.UserName = credentials.Element("UserName").Value;
                break;
            default:
                break;
        }

        return result;
    }

    /// <summary>
    /// Parse ClientCredentials into XML node. 
    /// </summary>
    /// <param name="clientCredentials">ClientCredentials type.</param>
    /// <param name="endpointType">AuthenticationProviderType of the credentials.</param>
    /// <returns>XML node containing credentials data.</returns>
    private XElement ParseOutCredentials(ClientCredentials clientCredentials,
        AuthenticationProviderType endpointType)
    {
        if (clientCredentials != null)
        {
            switch (endpointType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    return new XElement("Credentials",
                        new XElement("UserName", clientCredentials.Windows.ClientCredential.UserName),
                        new XElement("Domain", clientCredentials.Windows.ClientCredential.Domain)
                        );
                case AuthenticationProviderType.LiveId:
                case AuthenticationProviderType.Federation:
                case AuthenticationProviderType.OnlineFederation:
                    return new XElement("Credentials",
                       new XElement("UserName", clientCredentials.UserName.UserName)
                       );
                default:
                    break;
            }
        }

        return new XElement("Credentials", "");
    }

    /// <summary>
    /// Convert SecureString to unsecure string.
    /// </summary>
    /// <param name="securePassword">Pass SecureString for conversion.</param>
    /// <returns>unsecure string</returns>
    private string ConvertToUnsecureString(SecureString securePassword)
    {
        if (securePassword == null)
            throw new ArgumentNullException("securePassword");

        IntPtr unmanagedString = IntPtr.Zero;
        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
            return Marshal.PtrToStringUni(unmanagedString);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }
    }

    /// <summary>
    /// Convert unsecure string to SecureString.
    /// </summary>
    /// <param name="password">Pass unsecure string for conversion.</param>
    /// <returns>SecureString</returns>
    private SecureString ConvertToSecureString(string password)
    {
        if (password == null)
            throw new ArgumentNullException("password");

        var securePassword = new SecureString();
        foreach (char c in password)
            securePassword.AppendChar(c);
        securePassword.MakeReadOnly();
        return securePassword;
    }
    #endregion Private methods

    #region Private Classes
    /// <summary>
    /// private static class to store constants required by the CrmServiceHelper class.
    /// </summary>
    private static class CrmServiceHelperConstants
    {
        /// <summary>
        /// Credentials file path.
        /// </summary>
        public static readonly string ServerCredentialsFile = Path.Combine(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CrmServer"),
            "Credentials.xml");
    }
    #endregion
}

//</snippetCrmServiceHelper>
