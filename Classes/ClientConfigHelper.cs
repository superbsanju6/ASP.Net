using System.Collections.Generic;
using System.Linq;
using Thinkgate.Core.Constants;
using Thinkgate.Services.Contracts.ConfigurationService;

namespace Thinkgate.Classes
{
    /// <summary>
    /// Used by E3 to encapsulate retrieval of parameter data from the Configuration Service
    /// </summary>
	public sealed class ClientConfigHelper
	{

	    private readonly string _clientId;
	    private ConfigurationServiceProxy _configurationServiceProxy;

		public ClientConfigHelper(string clientid)
		{
            _clientId = clientid;
		}

        /// <summary>
        /// Return a ConfigurationServiceProxy; create if not previously created
        /// </summary>
        private ConfigurationServiceProxy ConfigurationServiceProxy
        {
		    get {
		        return _configurationServiceProxy
		               ?? (_configurationServiceProxy = new ConfigurationServiceProxy());
		    }
        }

        /// <summary>
        /// List of key value pairs for every property returned for a given client id
        /// </summary>
        /// <returns></returns>
	    public IEnumerable<KeyValuePair<string, string>> GetAllProperties()
	    {
	        return
	            ConfigurationServiceProxy.GetClientConfigurationByClientId(_clientId)
	                                          .ConfigProperties.Select(
	                                              x => new KeyValuePair<string, string>());
	    }

        /// <summary>
        /// Return the value for a specific property for a given clientid
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public string GetClientConfigProperty(string key)
		{
		    return ConfigurationServiceProxy.GetPropertyValueByClientId(_clientId, key);
		}
        
        /// <summary>
        /// Return the value for the ImageUrl property for a given clientid
        /// </summary>
        /// <returns></returns>
		public string GetImagesUrl()
		{
            return ConfigurationServiceProxy.GetPropertyValueByClientId(_clientId, ConfigurationConstants.IMAGES_URL);
		}
	
	}
}