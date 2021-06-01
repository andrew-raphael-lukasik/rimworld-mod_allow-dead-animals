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
		// public int ticksBetweenUpdates = 1000;
		// public int allowedAlreadyBufferLength = 128;

		public override void ExposeData ()
		{
			Scribe_Values.Look( ref allow , nameof(allow) , defaultValue:true );
			Scribe_Values.Look( ref notify , nameof(notify) , defaultValue:true );
			// Scribe_Values.Look( ref ticksBetweenUpdates , nameof(ticksBetweenUpdates) );
			// Scribe_Values.Look( ref allowedAlreadyBufferLength , nameof(allowedAlreadyBufferLength) );
			base.ExposeData();
		}

	}
}
