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
			var listing = new Listing_Standard();
			listing.Begin(rect);
			{
				listing.CheckboxLabeled( "Allow" , ref _settings.allow , "Is auto-allow enabled?" );
				listing.CheckboxLabeled( "Notify" , ref _settings.notify , "Are notifications enabled?" );

				var section = listing.BeginSection(24*4);
					section.CheckboxLabeled( "Animal" , ref _settings.allowAnimal );
					section.CheckboxLabeled( "Insect" , ref _settings.allowInsect );
					section.CheckboxLabeled( "Humanlike" , ref _settings.allowHumanlike );
					section.CheckboxLabeled( "Mechanoid" , ref _settings.allowMechanoid );
				listing.EndSection( section );

				string mass_threshold_label =_settings.mass_threshold.ToString();
				listing.TextFieldNumericLabeled( "Ignore corpses lighter than:" , ref _settings.mass_threshold , ref mass_threshold_label );
			}
			listing.End();
			base.DoSettingsWindowContents(rect);
		}

	}
}
