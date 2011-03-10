using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.Facilities.AutoTx;
using Castle.MicroKernel.Registration;
using Castle.Facilities.NHibernateIntegration.Tests.Transactions.Model;
using Castle.Services.Transaction;
using NHibernate;


namespace Castle.Facilities.NHibernateIntegration.Tests.Transactions
{
    [TestFixture]
    public class ReadOnlyTransactionsTestCase : AbstractNHibernateTestCase
    {
        protected override void ConfigureContainer()
        {
            container.AddFacility("transactions", new TransactionFacility());

            container.Register(Component.For<ReadOnlyTransactionsService>().Named("readOnlyTestService"));
            container.Register(Component.For<BlogDao>().Named("blogdao"));
        }
            

        [Test]
        public void TestFlushMode()
        {
            ReadOnlyTransactionsService service = container.Resolve<ReadOnlyTransactionsService>();
            ISessionManager sessionManager = container.Resolve<ISessionManager>();
            ISession session = sessionManager.OpenSession();

            service.ReadOnlyCall();
            
            service.WriteCall();
            
        }

  
    }
}
