using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Nagominashare
{
	interface IFeeling
	{
		int Calm { get; }
		int Anger { get; }
		int Joy { get; }
		int Sorrow { get; }
		int Energy { get; }
		bool IsError();
		int GetErrorCode();
		int EvaluateSmile();
	}
}