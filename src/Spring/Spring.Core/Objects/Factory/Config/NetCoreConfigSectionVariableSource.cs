using Microsoft.Extensions.Configuration;
using Spring.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Objects.Factory.Config
{
    [Serializable]
    public class NetCoreConfigSectionVariableSource : IVariableSource
    {
        private readonly IConfigurationSection configurationSection;
        /// <summary>
        /// 
        /// </summary>
        private readonly string sectionName;
        /// <summary>
        /// Convinience property. Gets or sets a single section
        /// to read properties from.
        /// </summary>
        /// <remarks>
        /// The section specified needs to be handled by the <see cref="NameValueSectionHandler"/>
        /// in order to be processed successfully.
        /// </remarks>
        /// <value>
        /// A section to read properties from.
        /// </value>
        public string SectionName
        {
            get { return this.sectionName; }
        }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigSectionVariableSource"/> from the given <paramref name="sectionName"/>
        /// </summary>
        public NetCoreConfigSectionVariableSource(string sectionName, IConfiguration configuration)
        {
            AssertUtils.ArgumentHasText(sectionName, "sectionName");
            AssertUtils.ArgumentNotNull(configuration, "configuration");
            this.sectionName = sectionName;
            this.configurationSection = configuration.GetSection(sectionName);
        }
        public bool CanResolveVariable(string name)
        {
            if (this.configurationSection != null)
            {
                return !StringUtils.IsNullOrEmpty(this.configurationSection[name]);
            }
            return false;
        }

        public string ResolveVariable(string name)
        {
            return this.configurationSection[name];
        }
    }
}
