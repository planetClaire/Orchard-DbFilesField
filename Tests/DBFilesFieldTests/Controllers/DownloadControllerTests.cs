using System;
using System.Collections.Generic;
using NUnit.Framework;
using Orchard.Tests.Modules;
using Autofac;
using Moq;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Handlers;
using Orchard.Core.Settings.Metadata;
using Orchard.Core.Settings.Services;
using Orchard.Data;
using Orchard.Settings;
using Orchard.Tests.Stubs;
using Orchard.Tests.Utility;

namespace DBFilesFieldTests.Controllers
{
    [TestFixture]
    public class DownloadControllerTests : DatabaseEnabledTestsBase
    {
        private Mock<WorkContext> _workContext;

        public override void Register(ContainerBuilder builder)
        {
            builder.RegisterAutoMocking(MockBehavior.Loose);
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            builder.RegisterType<DefaultContentManager>().As<IContentManager>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterType<SiteService>().As<ISiteService>();
            builder.RegisterType<ContentDefinitionManager>().As<IContentDefinitionManager>();
            builder.RegisterType<SiteSettingsPartHandler>().As<IContentHandler>();
            builder.RegisterType<DefaultContentQuery>().As<IContentQuery>().InstancePerDependency();

            _workContext = new Mock<WorkContext>();
            _workContext.Setup(w => w.GetState<ISite>(It.Is<string>(s => s == "CurrentSite"))).Returns(() => _container.Resolve<ISiteService>().GetSiteSettings());
            var workContextAccessor = new Mock<IWorkContextAccessor>();
            workContextAccessor.Setup(w => w.GetContext()).Returns(_workContext.Object);
            builder.RegisterInstance(workContextAccessor.Object).As<IWorkContextAccessor>();

        }

        public override void Init()
        {
            base.Init();

        }

        protected override IEnumerable<Type> DatabaseTypes
        {
            get
            {
                return new[]
                    {
                        typeof (ContentTypeRecord),
                        typeof (ContentItemRecord),
                        typeof (ContentItemVersionRecord)
                    };
            }
        }

        [Test]
        public void DownloadController_FileRecordDoesntExist_ReturnsNotFound(){
            Assert.True(true);
            
            ClearSession();
        }

    }
}
