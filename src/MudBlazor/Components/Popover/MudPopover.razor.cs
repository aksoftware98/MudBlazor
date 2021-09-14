﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPopover : MudComponentBase, IAsyncDisposable
    {
        [Inject] public IMudPopoverService Service { get; set; }

        protected string PopoverClass =>
           new CssBuilder("mud-popover")
            .AddClass("mud-popover-fixed", Fixed)
            .AddClass("mud-popover-open", Open)
            .AddClass($"mud-popover-{Placement.ToDescriptionString()}")
            .AddClass("mud-popover-relative-width", RelativeWidth)
            .AddClass("mud-paper", Paper)
            .AddClass("mud-paper-square", Paper && Square)
            .AddClass($"mud-elevation-{Elevation}", Paper)
            .AddClass(Class)
           .Build();

        protected string PopoverStyles =>
            new StyleBuilder()
            .AddStyle("max-height", $"{MaxHeight}px", MaxHeight != null)
            .AddStyle(Style)
            .Build();

        private Direction ConvertDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Start => RightToLeft ? Direction.Right : Direction.Left,
                Direction.End => RightToLeft ? Direction.Left : Direction.Right,
                _ => direction
            };
        }

        [CascadingParameter] public bool RightToLeft { get; set; }

        /// <summary>
        /// Sets the maxheight the popover can have when open.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; } = null;

        /// <summary>
        /// If true, will apply default MudPaper classes.
        /// </summary>
        [Parameter] public bool Paper { get; set; } = true;

        /// <summary>
        /// The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 8;

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// If true, the popover is visible.
        /// </summary>
        [Parameter] public bool Open { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter] public bool Fixed { get; set; }

        /// <summary>
        /// Sets the direction the popover will start from relative to its parent.
        /// </summary>
        /// 
        [Obsolete("Direction is obsolete. Use Placement instead!", false)]
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// Sets the anchor point the popover will start from relative to its parent.
        /// </summary>
        [Parameter] public Placement Placement { get; set; } = Placement.Bottom;

        /// <summary>
        /// If true, the select menu will open either above or bellow the input depending on the direction.
        /// </summary>
        [Obsolete("OffsetX is obsolete. Use X instead!", false)]
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// If true, the select menu will open either before or after the input depending on the direction.
        /// </summary>
        [Obsolete("OffsetX is obsolete. Use X instead!", false)]
        [Parameter] public bool OffsetY { get; set; }

        /// <summary>
        /// If true, the popover will have the same width at its parent element, default to false
        /// </summary>
        [Parameter] public bool RelativeWidth { get; set; } = false;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private MudPopoverHandler _handler;

        protected override void OnInitialized()
        {
            _handler = Service.Register(ChildContent);
            _handler.SetComponentBaseParameters(this, PopoverClass, PopoverStyles, Open);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            _handler.UpdateFragment(ChildContent, this, PopoverClass, PopoverStyles, Open);

            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender == true)
            {
                await _handler.Initialized();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async ValueTask DisposeAsync() => await Service.Unregister(_handler);
    }
}
