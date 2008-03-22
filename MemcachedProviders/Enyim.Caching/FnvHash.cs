using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;

namespace Enyim
{
	/// <summary>
	/// Implements a 64 bit long Fowler-Noll-Vo hash
	/// </summary>
	/// <remarks>
	/// Calculation found at http://lists.danga.com/pipermail/memcached/2007-April/003846.html, but 
	/// it is pretty much available everywhere
	/// </remarks>
	public sealed class FnvHash64 : System.Security.Cryptography.HashAlgorithm
	{
		private const ulong FNV_64_INIT = 0xcbf29ce484222325L;
		private const ulong FNV_64_PRIME = 0x100000001b3L;

		private ulong currentHashValue;

		public FnvHash64()
		{
			base.HashSizeValue = 64;

			this.Initialize();
		}

		public override void Initialize()
		{
			this.currentHashValue = FNV_64_INIT;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int end = ibStart + cbSize;

			for (int i = ibStart; i < end; i++)
			{
				this.currentHashValue = (this.currentHashValue * FNV_64_PRIME) ^ array[i];
			}
		}

		protected override byte[] HashFinal()
		{
			return BitConverter.GetBytes(this.currentHashValue);
		}

		public ulong CurrentHashValue
		{
			get { return this.currentHashValue; }
		}
	}

	// algorithm found at http://bretm.home.comcast.net/hash/6.html
	// provides better distribution but it's only 32 bit long
	public sealed class ModifiedFNV : HashAlgorithm
	{
		private const uint MOD_FNV_PRIME = 0x1000193;
		private const uint MOD_FNV_INIT = 0x811C9DC5;

		private uint currentHashValue;

		public ModifiedFNV()
		{
			this.HashSizeValue = 32;

			this.Initialize();
		}

		public override void Initialize()
		{
			this.currentHashValue = MOD_FNV_INIT;
		}
		
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int end = ibStart + cbSize;

			for (int i = ibStart; i < end; i++)
			{
				this.currentHashValue = (this.currentHashValue ^ array[i]) * MOD_FNV_PRIME;
			}
		}

		protected override byte[] HashFinal()
		{
			this.currentHashValue += this.currentHashValue << 13;
			this.currentHashValue ^= this.currentHashValue >> 7;
			this.currentHashValue += this.currentHashValue << 3;
			this.currentHashValue ^= this.currentHashValue >> 17;
			this.currentHashValue += this.currentHashValue << 5;

			return BitConverter.GetBytes(this.currentHashValue);
		}

		public uint CurrentHashValue
		{
			get { return this.currentHashValue; }
		}
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