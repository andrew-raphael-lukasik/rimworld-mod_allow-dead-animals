using UnityEngine;
using Verse;
using RimWorld;

namespace AllowDeadAnimals
{
	public class AllowDeadAnimalsMod : Mod
	{

		AllowDeadAnimalsModSettings _settings;

		public AllowDeadAnimalsMod ( ModContentPack content )
			: base( content )
			=> this._settings = GetSettings<AllowDeadAnimalsModSettings>();

		public override string SettingsCategory () => "Allow Dead Animals";

		public override void DoSettingsWindowContents ( Rect rect )
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(rect);
			{
				listing.CheckboxLabeled( "Allow" , ref _settings.allow , "Is auto-allow enabled?" );
				
				listing.CheckboxLabeled( "Notify" , ref _settings.notify , "Are notifications enabled?" );

				string mass_threshold_label =_settings.mass_threshold.ToString();
				listing.TextFieldNumericLabeled( "Mass Threshold" , ref _settings.mass_threshold , ref mass_threshold_label );
				// "Corpses below this mass will be ignored."
			}
			listing.End();
			base.DoSettingsWindowContents(rect);
		}

	}
}
