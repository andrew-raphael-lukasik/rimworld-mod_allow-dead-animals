using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace AllowDeadAnimals
{
	public class AllowDeadAnimalsModSettings : ModSettings
	{
		
		public bool allow = true;
		public bool notify = true;
		public float mass_threshold;

		public override void ExposeData ()
		{
			Scribe_Values.Look( ref allow , nameof(allow) , defaultValue:true );
			Scribe_Values.Look( ref notify , nameof(notify) , defaultValue:true );
			Scribe_Values.Look( ref mass_threshold , nameof(mass_threshold) , defaultValue:10 );
			base.ExposeData();
		}

	}
}
