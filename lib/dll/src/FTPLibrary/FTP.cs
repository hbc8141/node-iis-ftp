using Microsoft.Web.Administration;
using System;

namespace FTPLibrary
{
    public class FTP
    {
        /// <summary>
        /// FTP 추가
        /// </summary>
        /// <param name="role">유저명</param>
        /// <param name="siteName">사이트이름</param>
        /// <param name="serverIp">IP주소</param>
        /// <param name="physicalPath">경로</param>
        /// <returns></returns>
        public static bool AddFtp(string role, string siteName = "localhost", string serverIp = "127.0.0.1", string physicalPath = @"C:\ftp")
        {
            bool IsSuccess = false;

            try
            {
                using (ServerManager serverManager = ServerManager.OpenRemote(serverIp))
                {
                    Configuration config = serverManager.GetApplicationHostConfiguration();
                    ConfigurationSection sitesSection = config.GetSection("system.applicationHost/sites");
                    ConfigurationElementCollection sitesCollection = sitesSection.GetCollection();

                    ConfigurationElement siteElement = sitesCollection.CreateElement("site");
                    siteElement["name"] = @"localhost";
                    siteElement["id"] = 3;
                    siteElement["serverAutoStart"] = true;

                    ConfigurationElementCollection bindingsCollection = siteElement.GetCollection("bindings");
                    ConfigurationElement bindingElement = bindingsCollection.CreateElement("binding");
                    bindingElement["protocol"] = @"ftp";
                    bindingElement["bindingInformation"] = @"*:21:";
                    bindingsCollection.Add(bindingElement);

                    // 기본인증 허용 
                    ConfigurationElement ftpServerElement = siteElement.GetChildElement("ftpServer");
                    ConfigurationElement securityElement = ftpServerElement.GetChildElement("security");
                    ConfigurationElement authenticationElement = securityElement.GetChildElement("authentication");
                    ConfigurationElement basicAuthenticationElement = authenticationElement.GetChildElement("basicAuthentication");
                    basicAuthenticationElement["enabled"] = true;

                    // SSL 허용으로 변경
                    ConfigurationElement sslElement = securityElement.GetChildElement("ssl");
                    sslElement["controlChannelPolicy"] = "SslAllow";
                    sslElement["dataChannelPolicy"] = "SslAllow";

                    // FTP 폴더 경로 추가
                    ConfigurationElementCollection siteCollection = siteElement.GetCollection();
                    ConfigurationElement applicationElement = siteCollection.CreateElement("application");
                    applicationElement["path"] = @"/";
                    ConfigurationElementCollection applicationCollection = applicationElement.GetCollection();
                    ConfigurationElement virtualDirectoryElement = applicationCollection.CreateElement("virtualDirectory");
                    virtualDirectoryElement["path"] = @"/";
                    virtualDirectoryElement["physicalPath"] = physicalPath;
                    applicationCollection.Add(virtualDirectoryElement);
                    siteCollection.Add(applicationElement);
                    sitesCollection.Add(siteElement);

                    serverManager.CommitChanges();

                    IsSuccess = AddRole(siteName, role);
                }
            } catch (Exception e)
            {
                throw e;
            }

            return IsSuccess;
        }

        // 유저 추가
        /// <summary>
        /// FTP 접근 가능한 유저 추가
        /// </summary>
        /// <param name="siteName">사이트이름</param>
        /// <param name="role">유저명</param>
        /// <returns></returns>
        public static bool AddRole(string siteName, string role)
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Configuration config = serverManager.GetApplicationHostConfiguration();
                    ConfigurationSection authorizationSection = config.GetSection("system.ftpServer/security/authorization", siteName);
                    ConfigurationElementCollection authorizationCollection = authorizationSection.GetCollection();

                    ConfigurationElement addElement = authorizationCollection.CreateElement("add");
                    addElement["accessType"] = @"Allow";
                    addElement["roles"] = role;
                    addElement["permissions"] = @"Read, Write";
                    authorizationCollection.Add(addElement);

                    serverManager.CommitChanges();

                    return true;
                }
            } catch(Exception e)
            {
                throw e;
            }
        }
    }
}
