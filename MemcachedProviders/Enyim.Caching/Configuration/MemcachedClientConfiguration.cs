using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Configuration
{
	public class MemcachedClientConfiguration : IMemcachedClientConfiguration
	{
		private List<IPEndPoint> servers;
		private ISocketPoolConfiguration socketPool;
		private Type keyTransformer;
		private Type nodeLocator;
		private Type transcoder;

		public MemcachedClientConfiguration()
		{
			this.servers = new List<IPEndPoint>();
			this.socketPool = new _SocketPoolConfig();
		}

		public IList<IPEndPoint> Servers
		{
			get { return this.servers; }
		}

		public ISocketPoolConfiguration SocketPool
		{
			get { return this.socketPool; }
		}

		public Type KeyTransformer
		{
			get { return this.keyTransformer; }
			set
			{
				ConfigurationHelper.CheckForInterface(value, typeof(IKeyTransformer));

				this.keyTransformer = value;
			}
		}

		public Type NodeLocator
		{
			get { return this.nodeLocator; }
			set
			{
				ConfigurationHelper.CheckForInterface(value, typeof(IMemcachedNodeLocator));

				this.nodeLocator = value;
			}
		}

		public Type Transcoder
		{
			get { return this.transcoder; }
			set
			{
				ConfigurationHelper.CheckForInterface(value, typeof(ITranscoder));

				this.transcoder = value;
			}
		}

		#region [ IMemcachedClientConfiguration]

		IList<System.Net.IPEndPoint> IMemcachedClientConfiguration.Servers
		{
			get { return this.Servers; }
		}

		ISocketPoolConfiguration IMemcachedClientConfiguration.SocketPool
		{
			get { return this.SocketPool; }
		}

		Type IMemcachedClientConfiguration.KeyTransformer
		{
			get { return this.KeyTransformer; }
			set { this.KeyTransformer = value; }
		}

		Type IMemcachedClientConfiguration.NodeLocator
		{
			get { return this.NodeLocator; }
			set { this.NodeLocator = value; }
		}

		Type IMemcachedClientConfiguration.Transcoder
		{
			get { return this.Transcoder; }
			set { this.Transcoder = value; }
		}
		#endregion
		#region [ T:SocketPoolConfig           ]
		private class _SocketPoolConfig : ISocketPoolConfiguration
		{
			private int minPoolSize = 10;
			private int maxPoolSize = 200;
			private TimeSpan connectionTimeout = new TimeSpan(0, 0, 10);
			private TimeSpan deadTimeout = new TimeSpan(0, 2, 0);

			int ISocketPoolConfiguration.MinPoolSize
			{
				get { return this.minPoolSize; }
				set
				{
					if (value > 1000 || value > this.maxPoolSize)
						throw new ArgumentOutOfRangeException("MinPoolSize must be <= MaxPoolSize and must be <= 1000");

					this.minPoolSize = value;
				}
			}

			int ISocketPoolConfiguration.MaxPoolSize
			{
				get { return this.maxPoolSize; }
				set
				{
					if (value > 1000 || value < this.minPoolSize)
						throw new ArgumentOutOfRangeException("MaxPoolSize must be >= MinPoolSize and must be <= 1000");

					this.maxPoolSize = value;
				}
			}

			TimeSpan ISocketPoolConfiguration.ConnectionTimeout
			{
				get { return this.connectionTimeout; }
				set
				{
					if (value < TimeSpan.Zero)
						throw new ArgumentOutOfRangeException("value must be positive");

					this.connectionTimeout = value;
				}
			}

			TimeSpan ISocketPoolConfiguration.DeadTimeout
			{
				get { return this.deadTimeout; }
				set
				{
					if (value < TimeSpan.Zero)
						throw new ArgumentOutOfRangeException("value must be positive");

					this.deadTimeout = value;
				}
			}
		}
		#endregion
	}
}

#region [ License information          ]
/* ************************************************************
 *
 * Copyright (c) Attila Kiskó, enyim.com, 2007
 *
 * This source code is subject to terms and conditions of 
 * Microsoft Permissive License (Ms-PL).
 * 
 * A copy of the license can be found in the License.html
 * file at the root of this distribution. If you can not 
 * locate the License, please send an email to a@enyim.com
 * 
 * By using this source code in any fashion, you are 
 * agreeing to be bound by the terms of the Microsoft 
 * Permissive License.
 *
 * You must not remove this notice, or any other, from this
 * software.
 *
 * ************************************************************/
#endregion