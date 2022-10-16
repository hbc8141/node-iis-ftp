using System;
using System.Threading.Tasks;

namespace FTPLibrary
{
	public class Startup
	{
		/// <summary>
        /// Asynchrously create ftp server
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public async Task<Object> CreateFtpAsync(dynamic input)
		{
			string role = (string)input.role;
			string siteName = (string)input.siteName;
			string serverIp = (string)input.serverIp;
			string physicalPath = (string)input.physicalPath;

			bool result = await Task.Run(() =>
			{
				return FTP.AddFtp(role, siteName, serverIp, physicalPath);
			});

			return result;
		}
	}
}
