using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Worklight;

namespace MobileFirstSample
{
	public class CustomChallengeHandler : SecurityCheckChallengeHandler
	{

		public LoginFormInfo LoginFormParameters { get; set; }

		private bool _authSuccess = false;
		private bool _isAdapterAuth = false;
		private bool _shouldSubmitLoginForm = false;
		private bool _shouldSubmitAnswer = false;

		public JObject ChallengeAnswer = null;

		public CustomChallengeHandler(string realm)
		{
			SecurityCheck = realm;
		}

		public override string SecurityCheck{ get; set; }

		public override JObject GetChallengeAnswer()
		{
			return ChallengeAnswer;
		}

		public override bool ShouldSubmitChallengeAnswer()
		{
			return _shouldSubmitAnswer;
		}

		public override void HandleChallenge(object challenge)
		{
			Debug.WriteLine("We were challenged.. so we are handling it");
			var parms = new Dictionary<String, String>();

			var creds = new JObject();
			creds.Add("username", "user");
			creds.Add("password", "user");
			ChallengeAnswer = creds;
			_shouldSubmitAnswer = true;
		}

		public override void HandleSuccess(JObject identity)
		{
			Debug.WriteLine("Success " + identity.ToString());
		}

		public override void HandleFailure(JObject error)
		{
			Debug.WriteLine("Failure " + error.ToString());
		}
	}
}