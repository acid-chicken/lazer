// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;

namespace osu.Game.Rulesets.Mods
{
    public class ModWindUp : ModTimeRamp
    {
        public override string Name => "Wind Up";
        public override string Acronym => "WU";
        public override LocalisableString Description => "Can you keep up?";
        public override IconUsage? Icon => FontAwesome.Solid.ChevronCircleUp;

        [SettingSource("Initial rate", "The starting speed of the track")]
        public override BindableNumber<double> InitialRate { get; } = new BindableDouble
        {
            MinValue = 0.5,
            MaxValue = 1.99,
            Default = 1,
            Value = 1,
            Precision = 0.01,
        };

        [SettingSource("Final rate", "The speed increase to ramp towards")]
        public override BindableNumber<double> FinalRate { get; } = new BindableDouble
        {
            MinValue = 0.51,
            MaxValue = 2,
            Default = 1.5,
            Value = 1.5,
            Precision = 0.01,
        };

        [SettingSource("Adjust pitch", "Should pitch be adjusted with speed")]
        public override BindableBool AdjustPitch { get; } = new BindableBool
        {
            Default = true,
            Value = true
        };

        public override Type[] IncompatibleMods => base.IncompatibleMods.Append(typeof(ModWindDown)).ToArray();

        public ModWindUp()
        {
            InitialRate.BindValueChanged(val =>
            {
                if (val.NewValue >= FinalRate.Value)
                    FinalRate.Value = val.NewValue + FinalRate.Precision;
            });

            FinalRate.BindValueChanged(val =>
            {
                if (val.NewValue <= InitialRate.Value)
                    InitialRate.Value = val.NewValue - InitialRate.Precision;
            });
        }
    }
}
