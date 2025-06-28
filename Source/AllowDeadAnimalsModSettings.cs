using Verse;

namespace AllowDeadAnimals
{
	public class AllowDeadAnimalsModSettings : ModSettings
	{
		
		public bool allow = true;
		public bool allowAnimal = true;
		public bool allowInsect = false;
		public bool allowHumanlike = false;
		public bool allowMechanoid = false;
		public bool notify = true;
		public float mass_threshold = 10;

		public override void ExposeData ()
		{
			Scribe_Values.Look(		ref allow ,				nameof(allow) ,				defaultValue:true	);
			Scribe_Values.Look(		ref allowAnimal ,		nameof(allowAnimal) ,		defaultValue:true	);
			Scribe_Values.Look(		ref allowInsect ,		nameof(allowInsect) ,		defaultValue:false	);
			Scribe_Values.Look(		ref allowHumanlike ,	nameof(allowHumanlike) ,	defaultValue:false	);
			Scribe_Values.Look(		ref allowMechanoid ,	nameof(allowMechanoid) ,	defaultValue:false	);
			Scribe_Values.Look(		ref notify ,			nameof(notify) ,			defaultValue:true	);
			Scribe_Values.Look(		ref mass_threshold ,	nameof(mass_threshold) ,	defaultValue:10		);
			base.ExposeData();
		}

	}
}
