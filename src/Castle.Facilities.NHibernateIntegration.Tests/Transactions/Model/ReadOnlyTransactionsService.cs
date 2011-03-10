using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using NHibernate;
using NUnit.Framework;
using Castle.Facilities.AutoTx;
using ITransaction = Castle.Services.Transaction.ITransaction;

namespace Castle.Facilities.NHibernateIntegration.Tests.Transactions.Model
{
    [Transactional]
    public class ReadOnlyTransactionsService
    {
        private BlogDao dao;
        private ISessionManager sessionManager;

        public ReadOnlyTransactionsService(ISessionManager sessionManager,  BlogDao dao)
		{
			this.dao = dao;
			this.sessionManager = sessionManager;
		}

		[Transaction(TransactionMode.Requires, ReadOnly = true )]
		public virtual void ReadOnlyCall()
		{
			ISession session = sessionManager.OpenSession();
			Assert.AreEqual(FlushMode.Never, session.FlushMode);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void WriteCall()
		{
			ISession session = sessionManager.OpenSession();
			Assert.AreEqual(sessionManager.DefaultFlushMode, session.FlushMode);
		}


    }
}
