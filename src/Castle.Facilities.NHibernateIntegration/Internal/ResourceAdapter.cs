#region License

//  Copyright 2004-2010 Castle Project - http://www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 

#endregion

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	using System;
    using System.Data;
	using Services.Transaction;
    using NHibernate;
	using ITransaction = NHibernate.ITransaction;
    
    

	/// <summary>
	/// Adapter to <see cref="IResource"/> so a
	/// NHibernate transaction can be enlisted within
	/// <see cref="Castle.Services.Transaction.ITransaction"/> instances.
	/// </summary>
	public class ResourceAdapter : IResource, IDisposable
	{
		private ITransaction transaction;
        private FlushMode previousFlushMode;
        private readonly ISession session;
        private readonly IsolationLevel level;
        private readonly bool isReadOnly;
		private readonly bool isAmbient;

		/// <summary>
        /// Initializes a new instance of the <see cref="ResourceAdapter"/> class.
		/// </summary>
		/// <param name="level">The isolation level of the wrapped transaction</param>
		/// <param name="session">The nhibernate session that the wrapped transaction will be started on</param>
		/// <param name="isAmbient">Is th transaction ambient</param>
		/// <param name="isReadOnly">Is the transaction readonly?</param>
		public ResourceAdapter(IsolationLevel level, ISession session, bool isAmbient, bool isReadOnly)
		{
            this.level = level;
            this.session = session;
			this.isAmbient = isAmbient;
            this.isReadOnly = isReadOnly;
		}

		/// <summary>
		/// Implementors should start the
		/// transaction on the underlying resource
		/// </summary>
		public void Start()
		{
            transaction = session.BeginTransaction(level);
            if (isReadOnly)
            {
                previousFlushMode = session.FlushMode;
                session.FlushMode = FlushMode.Never;
            }
		}

		/// <summary>
		/// Implementors should commit the
		/// transaction on the underlying resource
		/// </summary>
		public void Commit()
		{
			transaction.Commit();
            if (isReadOnly)
            {
                ResetFlushMode();
            }
		}

		/// <summary>
		/// Implementors should rollback the
		/// transaction on the underlying resource
		/// </summary>
		public void Rollback()
		{
			//HACK: It was supossed to only a test but it fixed the escalated tx rollback issue. not sure if 
			//		this the right way to do it (probably not).
            if (!isAmbient)
            {
                transaction.Rollback();
                // Not sure if this should join in with the hacks if clause?
                if (isReadOnly)
                {
                    ResetFlushMode();
                }
            }

		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			transaction.Dispose();
		}


        private void ResetFlushMode()
        {
            session.FlushMode = previousFlushMode;
        }
	}
}