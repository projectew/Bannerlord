using KNTLibrary.Components.Events;
using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Revolutions.Components.Events.Test
{
    [Serializable]
    public class TestEventOption : EventOption
    {
        public string Information;

        public TestEventOption() : base()
        {

        }

        public TestEventOption(string id, string text) : base(id, text)
        {

        }

        public override void Invoke()
        {
            InformationManager.AddQuickInformation(new TextObject(this.Information));
        }
    }
}