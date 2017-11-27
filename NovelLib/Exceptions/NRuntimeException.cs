using System.Collections;
using System.Collections.Generic;
using System;

namespace Novel.Exceptions
{
	public class NRuntimeException : Exception
	{
		public NRuntimeException() { }
		public NRuntimeException(string message) : base(message) { }
		public NRuntimeException(string message, Exception inner) : base(message, inner) { }
	}
}