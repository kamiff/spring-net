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

#region Imports

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NUnit.Framework;

#endregion

namespace Spring.Objects.Factory.Config
{
	/// <summary>
    /// Unit tests for the ConfigSectionVariableSource class.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    [TestFixture]
    public sealed class NetCoreConfigSectionVariableSourceTests
    {
        [Test]
        public void TestVariablesResolutionWithSingleSection()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            var config = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();

            NetCoreConfigSectionVariableSource vs = new NetCoreConfigSectionVariableSource("DaoConfiguration", config);
            

            // existing vars
            Assert.AreEqual("1000", vs.ResolveVariable("maxResults"));
            Assert.AreEqual("1000", vs.ResolveVariable("MAXResults"));

            // non-existant variable
            Assert.IsNull(vs.ResolveVariable("dummy"));
        }

        

        [Test]
        public void TestVariableResolutionFromApplicationSettingsSchema()
        {
            ConfigSectionVariableSource vs = new ConfigSectionVariableSource();
            vs.SectionName = "applicationSettings/MyApp.Properties.Settings";
            Assert.AreEqual("1000", vs.ResolveVariable("maxResults"));
            Assert.AreEqual(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\Northwind.mdb;User ID=Admin;Password=;",
                vs.ResolveVariable("connection.string"));
        }
    }

}
