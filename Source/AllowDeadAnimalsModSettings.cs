using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace AllowDeadAnimals
{
	public class AllowDeadAnimalsModSettings : ModSettings
	{
		
		public bool enabled = true;
		public bool notifications = true;
		// public int ticksBetweenUpdates = 1000;
		// public int allowedAlreadyBufferLength = 128;

		public override void ExposeData ()
		{
			Scribe_Values.Look( ref enabled , nameof(enabled) );
			Scribe_Values.Look( ref notifications , nameof(notifications) );
			// Scribe_Values.Look( ref ticksBetweenUpdates , nameof(ticksBetweenUpdates) );
			// Scribe_Values.Look( ref allowedAlreadyBufferLength , nameof(allowedAlreadyBufferLength) );
			base.ExposeData();
		}

	}
}
