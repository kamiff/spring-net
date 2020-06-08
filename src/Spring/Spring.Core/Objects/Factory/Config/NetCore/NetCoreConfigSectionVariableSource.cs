#region License

/*
 * Copyright ?2002-2011 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using Microsoft.Extensions.Configuration;
using Spring.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Objects.Factory.Config
{
    /// <summary>
    /// Implementation of <see cref="IVariableSource"/> that
    /// resolves variable name against name-value sections in
    /// the .NET CORE Json Style configuration file.
    /// </summary>
    /// <example>
	/// <![CDATA[
    /// {
    ///     "DaoConfiguration": {
    ///         "maxResults": 1000,
    ///         "Cache": {
    ///            "ExpiredMinutes":  10
    ///         }
    ///      }
    /// }
    /// ]]>
	/// </example>
    /// <author>Aleksandar Seovic</author>
    /// <author>Kamiff Lee</author>
    [Serializable]
    public class NetCoreConfigSectionVariableSource : IVariableSource
    {
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// Before requesting a variable resolution, a client should
        /// ask, whether the source can resolve a particular variable name.
        /// </summary>
        /// <param name="name">the name of the variable to resolve</param>
        /// <returns><c>true</c> if the variable can be resolved, <c>false</c> otherwise</returns>
        public bool CanResolveVariable(string name)
        {
            if (StringUtils.IsNullOrEmpty(name))
            {
                return false;
            }
            if (this.configurationSection != null)
            {
                string resolveValue = this.ResolveVariable(name);
                return StringUtils.IsNullOrEmpty(resolveValue) == false;
            }
            return false;
        }
        /// <summary>
        /// Resolves variable value for the specified variable name.
        /// </summary>
        /// <param name="name">
        /// The name of the variable to resolve.
        /// </param>
        /// <returns>
        /// The variable value if able to resolve, <c>null</c> otherwise.
        /// </returns>
        public string ResolveVariable(string name)
        {
            AssertUtils.ArgumentHasText(name, "name");
            string val = this.configurationSection.GetValue<string>(name);
            if (StringUtils.HasText(val)) {
                return val;
            }
            if (name.IndexOf(".") > 0)
            {
                string keyName = name.Replace(".", ":");
                return this.configurationSection[keyName];
            }
            return null;
        }
    }
}
