﻿using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Edit List Box ("elst")
    /// </summary>
    [Box("elst", "Edit List Box")]
    public sealed class EditListBox : FullBox, ITableBox<EditListBox.EditListEntry>
    {
        public EditListBox() : base() { }
        public EditListBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            ulong entryLength = (ulong)(Version == 1 ? 8 + 8 : 4 + 4) + 2 + 2;
            return base.CalculateSize() + 4 + (entryLength * (ulong)Entries.Count);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                var entry = new EditListEntry();
                if (Version == 1)
                {
                    entry.SegmentDuration = stream.ReadBEUInt64();
                    entry.MediaTime = stream.ReadBEInt64();
                }
                else
                {
                    entry.SegmentDuration = stream.ReadBEUInt32();
                    entry.MediaTime = stream.ReadBEInt32();
                }
                entry.MediaRateInteger = stream.ReadBEInt16();
                entry.MediaRateFraction = stream.ReadBEInt16();

                Entries.Add(entry);
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEInt32(Entries.Count);

            foreach (EditListEntry entry in Entries)
            {
                if (Version == 1)
                {
                    stream.WriteBEUInt64(entry.SegmentDuration);
                    stream.WriteBEInt64(entry.MediaTime);
                }
                else
                {
                    stream.WriteBEUInt32((uint)entry.SegmentDuration);
                    stream.WriteBEInt32((int)entry.MediaTime);
                }
                stream.WriteBEInt16(entry.MediaRateInteger);
                stream.WriteBEInt16(entry.MediaRateFraction);
            }
        }

        public IList<EditListEntry> Entries { get; } = new List<EditListEntry>();

        public int EntryCount => Entries.Count;

        public class EditListEntry
        {
            public EditListEntry() { }

            public ulong SegmentDuration { get; set; }
            public long MediaTime { get; set; }

            public short MediaRateInteger { get; set; }
            public short MediaRateFraction { get; set; }
        }
    }
}
