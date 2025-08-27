// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;
using osu.Game.Localisation;

namespace osu.Game.Configuration
{
    public enum ExclusiveModeBehaviour
    {
        [LocalisableDescription(typeof(AudioSettingsStrings), nameof(AudioSettingsStrings.ExclusiveModeBehaviourNever))]
        Never,

        [LocalisableDescription(typeof(AudioSettingsStrings), nameof(AudioSettingsStrings.ExclusiveModeBehaviourAlways))]
        Always,

        [LocalisableDescription(typeof(AudioSettingsStrings), nameof(AudioSettingsStrings.ExclusiveModeBehaviourDuringActive))]
        DuringActive,

        [LocalisableDescription(typeof(AudioSettingsStrings), nameof(AudioSettingsStrings.ExclusiveModeBehaviourDuringGameplay))]
        DuringGameplay,
    }
}
