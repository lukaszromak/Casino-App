namespace PZ_PROJEKT.Models.Exception
{

	[Serializable]
	public class NoFundsException : System.Exception
	{
		public NoFundsException() { }
		public NoFundsException(string message) : base(message) { }
		public NoFundsException(string message, System.Exception inner) : base(message, inner) { }
		protected NoFundsException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
