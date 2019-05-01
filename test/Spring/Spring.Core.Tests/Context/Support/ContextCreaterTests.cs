using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Context.Support
{
    [TestFixture]
    public class ContextCreaterTests
    {
        [Test]
        public void TestCreateContextFromJson()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            var config = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();

            var creater = new ContextCreater();
            var ctx = creater.Create(null, config, "springContext") as XmlApplicationContext;
            
            Assert.IsTrue(ctx.ContainsObject("service"));
            Assert.IsTrue(ctx.ContainsObject("logicOne"));
            Assert.IsTrue(ctx.ContainsObject("logicTwo"));
            Service service = (Service)ctx.GetObject("service");
            ctx.Refresh();
            Assert.IsTrue(service.ProperlyDestroyed);
            service = (Service)ctx.GetObject("service");
            ctx.Dispose();
            Assert.IsTrue(service.ProperlyDestroyed);
        }
    }
}
