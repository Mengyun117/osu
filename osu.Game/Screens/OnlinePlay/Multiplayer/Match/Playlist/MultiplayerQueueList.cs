// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Containers;
using osu.Game.Online.Rooms;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Multiplayer.Match.Playlist
{
    /// <summary>
    /// A gameplay-ordered list of <see cref="DrawableRoomPlaylistItem"/>s.
    /// </summary>
    public class MultiplayerQueueList : DrawableRoomPlaylist
    {
        protected override FillFlowContainer<RearrangeableListItem<PlaylistItem>> CreateListFillFlowContainer() => new QueueFillFlowContainer
        {
            Spacing = new Vector2(0, 2)
        };

        protected override OsuRearrangeableListItem<PlaylistItem> CreateOsuDrawable(PlaylistItem item)
            => base.CreateOsuDrawable(item).With(d => ((DrawableRoomPlaylistItem)d).ShowItemOwner = true);

        private class QueueFillFlowContainer : FillFlowContainer<RearrangeableListItem<PlaylistItem>>
        {
            [Resolved(typeof(Room), nameof(Room.Playlist))]
            private BindableList<PlaylistItem> roomPlaylist { get; set; }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                roomPlaylist.BindCollectionChanged((_, __) => InvalidateLayout());
            }

            public override IEnumerable<Drawable> FlowingChildren => base.FlowingChildren.OfType<RearrangeableListItem<PlaylistItem>>().OrderBy(item => item.Model.PlaylistOrder);
        }
    }
}
